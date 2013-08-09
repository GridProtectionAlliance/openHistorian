//******************************************************************************************************
//  BufferedFileStream_IoQueue.cs - Gbtc
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
//  9/21/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using GSF.Collections;

namespace GSF.IO.Unmanaged
{
    public partial class BufferedFileStream
    {
        public static long BytesWritten = 0;
        public static long BytesRead = 0;

        /// <summary>
        /// Manages the I/O for the file stream
        /// Also provides a way to synchronize calls to the FileStream.
        /// </summary>
        internal unsafe class IoQueue
        {
            private readonly int m_bufferPoolSize;
            private readonly int m_dirtyPageSize;
            private BufferedFileStream m_baseStream;
            private readonly FileStream m_stream;
            private readonly ResourceQueue<byte[]> m_bufferQueue;
            private readonly object m_syncRoot;

            private static readonly ResourceQueueCollection<int, byte[]> s_resourceList;

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
            public IoQueue(FileStream stream, int bufferPoolSize, int dirtyPageSize, BufferedFileStream baseStream)
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
            /// <param name="callback">Provides the temporary block of data so something can be done with it. After returning, the byte[] will be reclaimed.</param>
            public void Read(long position, Action<byte[]> callback)
            {
                position = position & ~(long)(m_bufferPoolSize - 1);

                byte[] buffer = m_bufferQueue.Dequeue();
                IAsyncResult results;
                lock (m_syncRoot)
                {
                    m_stream.Position = position;
                    results = m_stream.BeginRead(buffer, 0, buffer.Length, null, null);
                }
                int bytesRead = m_stream.EndRead(results);
                BytesRead += bytesRead;
                if (bytesRead < buffer.Length)
                    Array.Clear(buffer, bytesRead, buffer.Length - bytesRead);

                callback(buffer);
                m_bufferQueue.Enqueue(buffer);
            }

            /// <summary>
            /// Writes all of the dirty blocks passed onto the disk subsystem.
            /// </summary>
            /// <param name="pagesToWrite">The list of all pages to write to the disk</param>
            /// <param name="waitForWriteToDisk">True to wait for a complete commit to disk before returning from this function.</param>
            public void Write(PageMetaDataList.PageMetaData[] pagesToWrite, bool waitForWriteToDisk)
            {
                byte[] buffer = m_bufferQueue.Dequeue();
                int dirtyPagesPerBlock = (m_bufferPoolSize / m_dirtyPageSize);
                ulong allPagesAreDirty = BitMath.CreateBitMask(dirtyPagesPerBlock);
                foreach (PageMetaDataList.PageMetaData block in pagesToWrite)
                {
                    if (block.IsDirtyFlags == allPagesAreDirty) //if all pages need to be written, one can shortcut
                    {
                        Marshal.Copy((IntPtr)block.LocationOfPage, buffer, 0, buffer.Length);
                        IAsyncResult results;
                        lock (m_syncRoot)
                        {
                            m_stream.Position = block.PositionIndex * (long)m_bufferPoolSize;
                            results = m_stream.BeginWrite(buffer, 0, buffer.Length, null, null);
                        }
                        m_stream.EndWrite(results);

                        BytesWritten += buffer.Length;
                    }
                    else //otherwise, write each dirty page one at a time.
                    {
                        for (int x = 0; x < dirtyPagesPerBlock; x++)
                        {
                            if (((block.IsDirtyFlags >> x) & 1) == 1) //if page is dirty
                            {
                                long position = block.PositionIndex * (long)m_bufferPoolSize + x * m_dirtyPageSize;
                                IntPtr location = (IntPtr)block.LocationOfPage + (x * m_dirtyPageSize);

                                Marshal.Copy(location, buffer, 0, m_dirtyPageSize);
                                IAsyncResult results;
                                lock (m_syncRoot)
                                {
                                    m_stream.Position = position;
                                    results = m_stream.BeginWrite(buffer, 0, m_dirtyPageSize, null, null);
                                }
                                m_stream.EndWrite(results);

                                BytesWritten += m_dirtyPageSize;
                            }
                        }
                    }
                }
                m_bufferQueue.Enqueue(buffer);

                m_stream.Flush(waitForWriteToDisk);
                if (waitForWriteToDisk)
                    WinApi.FlushFileBuffers(m_stream.SafeFileHandle);
            }
        }
    }
}