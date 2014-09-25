//******************************************************************************************************
//  BufferedFile_IoQueue.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  2/1/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using GSF;
using GSF.Collections;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure.Media
{
    internal partial class BufferedFile
    {
        ///// <summary>
        ///// Basic Statistics if I/O needs to be measured.
        ///// </summary>
        //public static long BytesWritten = 0;
        ///// <summary>
        ///// Basic Statistics if I/O needs to be measured.
        ///// </summary>
        //public static long BytesRead = 0;

        /// <summary>
        /// Manages the I/O for the file stream
        /// Also provides a way to synchronize calls to the FileStream.
        /// This also computes checksums for all of the data.
        /// </summary>
        private class IoQueue
        {
            /// <summary>
            /// Needed since this class computes footer checksums.
            /// </summary>
            private readonly int m_fileStructureBlockSize;
            private readonly FileStream m_stream;
            private readonly ResourceQueue<byte[]> m_bufferQueue;

            /// <summary>
            /// Needed to properly synchronize Read/Write operations.
            /// </summary>
            private readonly object m_syncRoot;
            
            /// <summary>
            /// Creates a new IoQueue
            /// </summary>
            /// <param name="stream">The filestream to use as the base stream</param>
            /// <param name="bufferPoolSize">The size of a buffer pool entry</param>
            /// <param name="fileStructureBlockSize">The size of an individual block</param>
            public IoQueue(FileStream stream, int bufferPoolSize, int fileStructureBlockSize)
            {
                if (bufferPoolSize < 4096)
                    throw new ArgumentOutOfRangeException("bufferPoolSize", "Must be greater than 4096");
                if (fileStructureBlockSize > bufferPoolSize)
                    throw new ArgumentOutOfRangeException("fileStructureBlockSize", "Must not be greater than BufferPoolSize");
                if (!BitMath.IsPowerOfTwo(bufferPoolSize))
                    throw new ArgumentException("Must be a power of 2", "bufferPoolSize");
                if (!BitMath.IsPowerOfTwo(fileStructureBlockSize))
                    throw new ArgumentException("Must be a power of 2", "fileStructureBlockSize");

                m_fileStructureBlockSize = fileStructureBlockSize;

                m_bufferQueue = s_resourceList.GetResourceQueue(bufferPoolSize);
                m_syncRoot = new object();
                m_stream = stream;
            }

            /// <summary>
            /// Reads an entire page at the provided locaiton
            /// </summary>
            /// <param name="position">The stream position. May be any position inside the desired block</param>
            /// <param name="locationToCopyData">The place where to write the data to.</param>
            public void Read(long position, IntPtr locationToCopyData)
            {
                byte[] buffer = m_bufferQueue.Dequeue();
                int bytesRead = Read(position, buffer, buffer.Length);
                //BytesRead += bytesRead;
                if (bytesRead < buffer.Length)
                    Array.Clear(buffer, bytesRead, buffer.Length - bytesRead);

                Marshal.Copy(buffer, 0, locationToCopyData, buffer.Length);
                
                m_bufferQueue.Enqueue(buffer);

                Footer.WriteChecksumResultsToFooter(locationToCopyData, m_fileStructureBlockSize, buffer.Length);
            }


            /// <summary>
            /// Writes all of the dirty blocks passed onto the disk subsystem.
            /// </summary>
            /// <param name="stream">the source of the data to dump to the disk</param>
            /// <param name="currentEndOfCommitPosition">the last valid byte of the file system where this data will be appended to.</param>
            /// <param name="length">The number by bytes to write to the file system.</param>
            /// <param name="waitForWriteToDisk">True to wait for a complete commit to disk before returning from this function.</param>
            public void Write(MemoryPoolStreamCore stream, long currentEndOfCommitPosition, long length, bool waitForWriteToDisk)
            {
                byte[] buffer = m_bufferQueue.Dequeue();
                long endPosition = currentEndOfCommitPosition + length;
                long currentPosition = currentEndOfCommitPosition;
                while (currentPosition < endPosition)
                {
                    IntPtr ptr;
                    int streamLength;
                    stream.ReadBlock(currentPosition, out ptr, out streamLength);
                    int subLength = (int)Math.Min(streamLength, endPosition - currentPosition);
                    Footer.ComputeChecksumAndClearFooter(ptr, m_fileStructureBlockSize, subLength);
                    Marshal.Copy(ptr, buffer, 0, subLength);
                    Write(currentPosition, buffer, subLength);

                    //BytesWritten += subLength;
                    currentPosition += subLength;
                }
                m_bufferQueue.Enqueue(buffer);

                if (waitForWriteToDisk)
                {
                    FlushFileBuffers();
                }
                else
                {
                    m_stream.Flush(false);
                }
            }

            /// <summary>
            /// Flushes any temporary data to the disk. This also calls WindowsAPI function 
            /// to have the OS flush to the disk.
            /// </summary>
            public void FlushFileBuffers()
            {
                //.NET's stream.Flush(FlushToDisk:=true) actually doesn't do what it says. 
                //Therefore WinApi must be called.
                m_stream.Flush(true);
                WinApi.FlushFileBuffers(m_stream.SafeFileHandle);
            }

            /// <summary>
            /// Writes data to the disk
            /// </summary>
            /// <param name="position">The starting position</param>
            /// <param name="buffer">the byte buffer of data to write</param>
            /// <param name="length">the number of bytes to write</param>
            public void Write(long position, byte[] buffer, int length)
            {
                IAsyncResult results;
                lock (m_syncRoot)
                {
                    m_stream.Position = position;
                    results = m_stream.BeginWrite(buffer, 0, length, null, null);
                }
                m_stream.EndWrite(results);
            }

            /// <summary>
            /// Reads data from the disk
            /// </summary>
            /// <param name="position">The starting position</param>
            /// <param name="buffer">the byte buffer of data to read</param>
            /// <param name="length">the number of bytes to read</param>
            /// <returns>the number of bytes read</returns>
            public int Read(long position, byte[] buffer, int length)
            {
                IAsyncResult results;
                lock (m_syncRoot)
                {
                    m_stream.Position = position;
                    results = m_stream.BeginRead(buffer, 0, length, null, null);
                }
                return m_stream.EndRead(results);
            }


            /// <summary>
            /// Queues byte[] blocks.
            /// </summary>
            private static readonly ResourceQueueCollection<int, byte[]> s_resourceList;

            /// <summary>
            /// Creates a resource list that everyone shares.
            /// </summary>
            static IoQueue()
            {
                s_resourceList = new ResourceQueueCollection<int, byte[]>((blockSize => (() => new byte[blockSize])), 10, 20);
            }

        }
    }
}