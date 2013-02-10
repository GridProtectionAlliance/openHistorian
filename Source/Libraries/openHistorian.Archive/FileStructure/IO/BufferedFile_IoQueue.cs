//******************************************************************************************************
//  CustomBufferedFileStream_IoQueue.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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

namespace openHistorian.FileStructure.IO
{
    partial class BufferedFile
    {
        public static long BytesWritten = 0;
        public static long BytesRead = 0;

        /// <summary>
        /// Manages the I/O for the file stream
        /// Also provides a way to synchronize calls to the FileStream.
        /// </summary>
        unsafe internal class IoQueue
        {
            int m_bufferPoolSize;
            int m_dirtyPageSize;
            BufferedFile m_baseStream;
            FileStream m_stream;
            ResourceQueue<byte[]> m_bufferQueue;
            object m_syncRoot;

            static ResourceQueueCollection<int, byte[]> s_resourceList;

            static IoQueue()
            {
                s_resourceList = new ResourceQueueCollection<int, byte[]>((blockSize => (() => new byte[blockSize])), 10, 20);
            }

            /// <summary>
            /// Creates a new IoQueue
            /// </summary>
            /// <param name="stream">The filestream to use as the base stream</param>
            /// <param name="bufferPoolSize">The size of a buffer pool entry</param>
            /// <param name="dirtyPageSize">The size of an individual dirty page</param>
            public IoQueue(FileStream stream, int bufferPoolSize, int dirtyPageSize, BufferedFile baseStream)
            {
                if (bufferPoolSize < 4096)
                    throw new ArgumentOutOfRangeException("Must be greater than 4096", "bufferPoolSize");
                if (dirtyPageSize > bufferPoolSize)
                    throw new ArgumentOutOfRangeException("Must not be greater than BufferPoolSize", "dirtyPageSize");
                if (!BitMath.IsPowerOfTwo(bufferPoolSize))
                    throw new ArgumentException("Must be a power of 2", "bufferPoolSize");
                if (!BitMath.IsPowerOfTwo(dirtyPageSize))
                    throw new ArgumentException("Must be a power of 2", "dirtyPageSize");
                if (dirtyPageSize * 64 < bufferPoolSize)
                    throw new ArgumentException("Cannot be greater than 64 * dirtyPageSize", "bufferPoolSize");

                m_baseStream = baseStream;
                m_bufferPoolSize = bufferPoolSize;
                m_dirtyPageSize = dirtyPageSize;

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
                var buffer = m_bufferQueue.Dequeue();
                var bytesRead = ReadBytesFromDisk(position, buffer, buffer.Length);
                BytesRead += bytesRead;
                if (bytesRead < buffer.Length)
                    Array.Clear(buffer, bytesRead, buffer.Length - bytesRead);

                Marshal.Copy(buffer, 0, locationToCopyData, buffer.Length);
                m_bufferQueue.Enqueue(buffer);
                
                Footer.WriteChecksumResultsToFooter(locationToCopyData, m_dirtyPageSize, buffer.Length);
            }

            /// <summary>
            /// Writes all of the dirty blocks passed onto the disk subsystem.
            /// </summary>
            /// <param name="stream">the source of the data to dump to the disk</param>
            /// <param name="currentEndOfCommitPosition">the last valid byte of the file system where this data will be appended to.</param>
            /// <param name="length">The number by bytes to write to the file system.</param>
            /// <param name="waitForWriteToDisk">True to wait for a complete commit to disk before returning from this function.</param>
            public void Write(IBinaryStreamIoSession stream, long currentEndOfCommitPosition, long length, bool waitForWriteToDisk)
            {
                var buffer = m_bufferQueue.Dequeue();
                long readPosition = 0;
                long writePosition = currentEndOfCommitPosition;
                while (readPosition < length)
                {
                    int subLength = (int)Math.Min((long)buffer.Length, length - readPosition);

                    IntPtr ptr;
                    int streamLength;
                    stream.ReadBlock(readPosition, out ptr, out streamLength);
                    if (streamLength < buffer.Length)
                        throw new Exception("Stream is not aligned as expected");

                    Footer.ComputeChecksumAndClearFooter(ptr, m_dirtyPageSize, subLength);
                    Marshal.Copy(ptr, buffer, 0, subLength);

                    WriteToDisk(writePosition, buffer, subLength);

                    BytesWritten += subLength;
                    readPosition += subLength;
                    writePosition += subLength;
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

            public void FlushFileBuffers()
            {
                m_stream.Flush(true);
                WinApi.FlushFileBuffers(m_stream.SafeFileHandle);
            }

            public void WriteToDisk(long position, byte[] buffer, int length)
            {
                IAsyncResult results;
                lock (m_syncRoot)
                {
                    m_stream.Position = position;
                    results = m_stream.BeginWrite(buffer, 0, length, null, null);
                }
                m_stream.EndWrite(results);
            }

            public int ReadBytesFromDisk(long position, byte[] buffer, int length)
            {
                IAsyncResult results;
                lock (m_syncRoot)
                {
                    m_stream.Position = position;
                    results = m_stream.BeginRead(buffer, 0, length, null, null);
                }
                return m_stream.EndRead(results);
            }


        }
    }
}
