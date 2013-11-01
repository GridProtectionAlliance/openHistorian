//******************************************************************************************************
//  BufferedFileStream.cs - Gbtc
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
//  4/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Linq;
using System.Threading;
using GSF.UnmanagedMemory;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// A buffered file stream utilizes the buffer pool to intellectually cache
    /// the contents of files.  
    /// </summary>
    /// <remarks>
    /// The cache algorithm is a least recently used algorithm.
    /// but will place more emphysis on object that are repeatidly accessed over 
    /// ones that are rarely accessed. This is accomplised by incrementing a counter
    /// every time a page is accessed and dividing by 2 every time a collection occurs from the buffer pool.
    /// </remarks>
    //ToDo: Consider allowing this class to scale horizontally like how the concurrent dictionary scales.
    //ToDo: this will reduce the concurrent contention on the class at the cost of more memory required.
    public unsafe partial class BufferedFileStream : ISupportsBinaryStream
    {
        /// <summary>
        /// To synchronize all calls to this class.
        /// </summary>
        private readonly object m_syncRoot;

        /// <summary>
        /// To limit flushing to a single flush call
        /// </summary>
        private readonly object m_syncFlush;

        private MemoryPool m_pool;

        private int m_dirtyPageSize;

        /// <summary>
        /// The file stream use by this class.
        /// </summary>
        private readonly FileStream m_baseStream;

        private readonly LeastRecentlyUsedPageReplacement m_pageReplacementAlgorithm;


        private bool m_disposed;
        private readonly bool m_ownsStream;

        private readonly IoQueue m_queue;

        public BufferedFileStream(FileStream stream, bool ownsStream = false)
            : this(stream, Globals.MemoryPool, 4096, ownsStream)
        {
        }

        /// <summary>
        /// Creates a file backed memory stream.
        /// </summary>
        /// <param name="stream">The file stream to back</param>
        /// <param name="pool"></param>
        /// <param name="dirtyPageSize"></param>
        public BufferedFileStream(FileStream stream, MemoryPool pool, int dirtyPageSize, bool ownsStream = false)
        {
            m_ownsStream = ownsStream;
            m_pool = pool;
            m_dirtyPageSize = dirtyPageSize;
            m_queue = new IoQueue(stream, pool.PageSize, dirtyPageSize, this);

            m_syncRoot = new object();
            m_syncFlush = new object();

            m_pageReplacementAlgorithm = new LeastRecentlyUsedPageReplacement(dirtyPageSize, pool);
            m_baseStream = stream;
            pool.RequestCollection += BufferPool_RequestCollection;
        }

        /// <summary>
        /// Gets the number of available simultaneous read/write sessions.
        /// </summary>
        /// <remarks>This value is used to determine if a binary stream can be cloned
        /// to improve read/write/copy performance.</remarks>
        public int RemainingSupportedIoSessions
        {
            get
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// Writes all of the current data to the disk subsystem.
        /// </summary>
        public void Flush()
        {
            //Flush(false, true, -1);
            Flush(true, false, -1);
        }


        private void Flush(bool waitForWriteToDisk, bool skipPagesInUse, int desiredFlushCount)
        {
            lock (m_syncFlush)
            {
                PageMetaDataList.PageMetaData[] dirtyPages;
                lock (m_syncRoot)
                {
                    dirtyPages = m_pageReplacementAlgorithm.GetDirtyPages(skipPagesInUse).ToArray();
                    foreach (PageMetaDataList.PageMetaData block in dirtyPages)
                    {
                        m_pageReplacementAlgorithm.ClearDirtyBits(block);
                    }
                }
                m_queue.Write(dirtyPages, waitForWriteToDisk);
            }
        }

        private void TryFlush(bool waitForWriteToDisk, bool skipPagesInUse, int desiredFlushCount)
        {
            PageMetaDataList.PageMetaData[] dirtyPages;
            if (Monitor.TryEnter(m_syncFlush))
            {
                try
                {
                    if (Monitor.TryEnter(m_syncRoot))
                    {
                        try
                        {
                            dirtyPages = m_pageReplacementAlgorithm.GetDirtyPages(skipPagesInUse).ToArray();
                            foreach (PageMetaDataList.PageMetaData block in dirtyPages)
                            {
                                m_pageReplacementAlgorithm.ClearDirtyBits(block);
                            }
                        }
                        finally
                        {
                            Monitor.Exit(m_syncRoot);
                        }
                        m_queue.Write(dirtyPages, waitForWriteToDisk);
                    }
                }
                finally
                {
                    Monitor.Exit(m_syncFlush);
                }
            }
        }

        private void GetBlock(LeastRecentlyUsedPageReplacement.IoSession ioSession, BlockArguments args)
        {
            LeastRecentlyUsedPageReplacement.SubPageMetaData subPage;

            lock (m_syncRoot)
            {
                if (ioSession.TryGetSubPage(args.Position, args.IsWriting, out subPage))
                {
                    args.FirstPointer = (IntPtr)subPage.Location;
                    args.Length = subPage.Length;
                    args.FirstPosition = subPage.Position;
                    args.SupportsWriting = subPage.IsDirty;
                    return;
                }
            }

            Action<byte[]> callback = data =>
            {
                lock (m_syncRoot)
                {
                    ioSession.TryAddNewPage(args.Position & ~(long)(data.Length - 1), data, 0, data.Length);
                    ioSession.TryGetSubPage(args.Position, args.IsWriting, out subPage);
                }
            };

            m_queue.Read(args.Position, callback);

            args.FirstPointer = (IntPtr)subPage.Location;
            args.Length = subPage.Length;
            args.FirstPosition = subPage.Position;
            args.SupportsWriting = subPage.IsDirty;
            return;
        }

        private void Clear(LeastRecentlyUsedPageReplacement.IoSession ioSession)
        {
            lock (m_syncRoot)
            {
                ioSession.Clear();
            }
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                //Unregistering from this event gaurentees that a collection will no longer
                //be called since this class utilizes custom code to garentee this.
                Globals.MemoryPool.RequestCollection -= BufferPool_RequestCollection;

                //Flush(true, false, -1);
                m_disposed = true;

                m_pageReplacementAlgorithm.Dispose();
                if (m_ownsStream)
                    m_baseStream.Dispose();
            }
        }

        private void BufferPool_RequestCollection(object sender, CollectionEventArgs e)
        {
            if (e.CollectionMode == BufferPoolCollectionMode.Critical)
            {
                if (Monitor.TryEnter(m_syncRoot))
                {
                    try
                    {
                        m_pageReplacementAlgorithm.DoCollection(e);
                    }
                    finally
                    {
                        Monitor.Exit(m_syncRoot);
                    }
                }

                TryFlush(false, true, e.DesiredPageReleaseCount);
            }

            if (Monitor.TryEnter(m_syncRoot))
            {
                try
                {
                    m_pageReplacementAlgorithm.DoCollection(e);
                }
                finally
                {
                    Monitor.Exit(m_syncRoot);
                }
            }
        }

        BinaryStreamIoSessionBase ISupportsBinaryStream.CreateIoSession()
        {
            lock (m_syncRoot)
            {
                return new IoSession(this, m_pageReplacementAlgorithm.CreateNewIoSession());
            }
        }

        public BinaryStreamBase CreateBinaryStream()
        {
            return new BinaryStream(this);
        }

        public int BlockSize
        {
            get
            {
                return Globals.MemoryPool.PageSize;
            }
        }

        public void TrimEditsAfterPosition(long position)
        {
            lock (m_syncFlush)
            {
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return !m_baseStream.CanWrite;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }
    }
}