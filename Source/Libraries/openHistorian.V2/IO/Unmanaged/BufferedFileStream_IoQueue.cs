//******************************************************************************************************
//  BufferedFileStream_IoQueue.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
using openHistorian.V2.Collections;

namespace openHistorian.V2.IO.Unmanaged
{
    public partial class BufferedFileStream
    {
        /// <summary>
        /// Manages the I/O for the file stream
        /// </summary>
        unsafe internal class IoQueue
        {
            int m_bufferPoolSize;
            int m_dirtyPageSize;
            FileStream m_stream;
            ResourceQueue<byte[]> m_bufferQueue;
            object m_syncRoot;

            static ResourceQueueCollection<int, byte[]> s_resourceList;

            static IoQueue()
            {
                s_resourceList = new ResourceQueueCollection<int, byte[]>((blockSize => (() => new byte[blockSize])), 10, 20);
            }

            public IoQueue(FileStream stream, int bufferPoolSize, int dirtyPageSize)
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

                m_bufferPoolSize = bufferPoolSize;
                m_dirtyPageSize = dirtyPageSize;

                m_bufferQueue = s_resourceList.GetResourceQueue(bufferPoolSize);
                m_syncRoot = new object();
                m_stream = stream;
            }

            public void Read(long position, Action<byte[]> callback)
            {
                var buffer = m_bufferQueue.Dequeue();
                IAsyncResult results;
                lock (m_syncRoot)
                {
                    m_stream.Position = position;
                    results = m_stream.BeginRead(buffer, 0, buffer.Length, null, null);
                }
                m_stream.EndRead(results);
                callback(buffer);
                m_bufferQueue.Enqueue(buffer);
            }

            public void Write(PageMetaDataList.PageMetaData[] pagesToWrite, bool waitForWriteToDisk)
            {
                var buffer = m_bufferQueue.Dequeue();
                int dirtyPagesPerBlock = (m_bufferPoolSize / m_dirtyPageSize);
                ulong allPagesAreDirty = BitMath.CreateBitMask(dirtyPagesPerBlock);
                foreach (var block in pagesToWrite)
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
                    }
                    else //otherwise, write each dirty page at a time.
                    {
                        for (int x = 0; x < dirtyPagesPerBlock; x++)
                        {
                            if (((block.IsDirtyFlags >> x) & 1) == 1) //if page is dirty
                            {
                                Marshal.Copy((IntPtr)block.LocationOfPage + (x * m_dirtyPageSize), buffer, 0, m_dirtyPageSize);
                                IAsyncResult results;
                                lock (m_syncRoot)
                                {
                                    m_stream.Position = block.PositionIndex * (long)m_bufferPoolSize +
                                                                     x * m_dirtyPageSize;
                                    results = m_stream.BeginWrite(buffer, 0, m_dirtyPageSize, null, null);
                                }
                                m_stream.EndWrite(results);
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
