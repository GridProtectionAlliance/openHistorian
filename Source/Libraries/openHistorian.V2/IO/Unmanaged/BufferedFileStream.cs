//******************************************************************************************************
//  BufferedFileStream.cs - Gbtc
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
//  4/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using openHistorian.V2.IO;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.Unmanaged
{
    /// <summary>
    /// A buffered file stream utilizes the buffer pool to intellectually cache
    /// the contents of files.  
    /// </summary>
    /// <remarks>
    /// The cache algorithm is a least recently used algorithm.
    /// but will place more emphysis on object that are repeatidly accessed over 
    /// once that are rarely accessed. This is accomplised by incrementing a counter
    /// every time a page is accessed and dividing by 2 every time a collection occurs from the buffer pool.
    /// </remarks>
    unsafe public class BufferedFileStream : ISupportsBinaryStreamSizing
    {
        // Nested Types
        class IoSession : IBinaryStreamIoSession
        {
            bool m_disposed;
            BufferedFileStream m_stream;
            LeastRecentlyUsedPageReplacement.IoSession m_ioSession;

            public IoSession(BufferedFileStream stream, LeastRecentlyUsedPageReplacement.IoSession ioSession)
            {
                m_stream = stream;
                m_ioSession = ioSession;
            }

            /// <summary>
            /// Releases the unmanaged resources before the <see cref="IoSession"/> object is reclaimed by <see cref="GC"/>.
            /// </summary>
            ~IoSession()
            {
                Dispose(false);
            }

            /// <summary>
            /// Releases all the resources used by the <see cref="IoSession"/> object.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="IoSession"/> object and optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        // This will be done regardless of whether the object is finalized or disposed.
                        m_ioSession.Dispose();
                        if (disposing)
                        {
                            // This will be done only when the object is disposed by calling Dispose().
                        }
                    }
                    finally
                    {
                        m_disposed = true;  // Prevent duplicate dispose.
                    }
                }
            }

            public void GetBlock(long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
            {
                m_stream.GetBlock(m_ioSession, position, isWriting, out firstPointer, out firstPosition, out length, out supportsWriting);
            }

            public void Clear()
            {
                m_ioSession.Clear();
            }


            public bool IsDisposed
            {
                get
                {
                    return m_disposed;
                }
            }
        }

        /// <summary>
        /// A buffer to use to read/write from a disk.
        /// </summary>
        /// <remarks>Since all disk IO inside .NET must be with a managed type. 
        /// This buffer provides a means to do the disk IO</remarks>
        /// ToDo: Create multiple static blocks so concurrent IO can occur.
        static byte[] s_tmpBuffer = new byte[Globals.BufferPool.PageSize];

        /// <summary>
        /// An event raised when this class has been disposed.
        /// </summary>
        /// <remarks>It is important for anything that utilizes IO Sessions discontinue use of them
        /// after this event since that can cause data corruption.</remarks>
        public event EventHandler StreamDisposed;

        /// <summary>
        /// The file stream use by this class.
        /// </summary>
        FileStream m_baseStream;

        LeastRecentlyUsedPageReplacement m_pageReplacementAlgorithm;

        public BufferedFileStream(FileStream stream)
        {
            m_pageReplacementAlgorithm = new LeastRecentlyUsedPageReplacement();
            m_baseStream = stream;
            Globals.BufferPool.RequestCollection += BufferPool_RequestCollection;
        }

        public int RemainingSupportedIoSessions
        {
            get
            {
                return int.MaxValue;
            }
        }

        public void Flush(bool waitForWriteToDisk = false, bool skipPagesInUse = true)
        {
            foreach (var block in m_pageReplacementAlgorithm.GetDirtyPages(skipPagesInUse))
            {
                m_baseStream.Position = block.PositionIndex * (long)Globals.BufferPool.PageSize;
                Marshal.Copy((IntPtr)block.LocationOfPage, s_tmpBuffer, 0, s_tmpBuffer.Length);
                m_baseStream.Write(s_tmpBuffer, 0, s_tmpBuffer.Length);
                m_pageReplacementAlgorithm.ClearDirtyBits(block);
            }
            m_baseStream.Flush(waitForWriteToDisk);
            if (waitForWriteToDisk)
                WinApi.FlushFileBuffers(m_baseStream.SafeFileHandle);
        }

        void GetBlock(LeastRecentlyUsedPageReplacement.IoSession ioSession, long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
        {
            var pageMetaData = ioSession.TryGetSubPageOrCreateNew(position, isWriting, LoadFromFile);
            firstPointer = (IntPtr)pageMetaData.Location;
            length = pageMetaData.Length;
            firstPosition = pageMetaData.Position;
            supportsWriting = pageMetaData.IsDirty;
        }

        void LoadFromFile(IntPtr pageLocation, long pagePosition)
        {
            m_baseStream.Position = pagePosition;
            lock (s_tmpBuffer)
            {
                m_baseStream.Read(s_tmpBuffer, 0, s_tmpBuffer.Length);
                Marshal.Copy(s_tmpBuffer, 0, pageLocation, s_tmpBuffer.Length);
            }
        }

        void IDisposable.Dispose()
        {
            Globals.BufferPool.RequestCollection -= BufferPool_RequestCollection;

            if (StreamDisposed != null)
                StreamDisposed.Invoke(this, EventArgs.Empty);
            m_pageReplacementAlgorithm.Dispose();
        }

        void BufferPool_RequestCollection(object sender, CollectionEventArgs e)
        {
            if (e.CollectionMode == BufferPoolCollectionMode.Critical)
            {
                Flush();
            }
            m_pageReplacementAlgorithm.DoCollection();
        }

        IBinaryStreamIoSession ISupportsBinaryStream.GetNextIoSession()
        {
            return new IoSession(this, m_pageReplacementAlgorithm.CreateNewIoSession());
        }


        public IBinaryStream CreateBinaryStream()
        {
            return new BinaryStream(this);
        }

        long ISupportsBinaryStreamSizing.Length
        {
            get
            {
                return m_baseStream.Length;
            }
        }

        long ISupportsBinaryStreamSizing.SetLength(long length)
        {
            //if (m_baseStream.Length < length)
                m_baseStream.SetLength(length);
            return m_baseStream.Length;
        }

        public int BlockSize
        {
            get
            {
                return Globals.BufferPool.PageSize;
            }
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void TrimEditsAfterPosition(long position)
        {
            throw new NotImplementedException();
        }


        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDisposed
        {
            get { throw new NotImplementedException(); }
        }
    }
}
