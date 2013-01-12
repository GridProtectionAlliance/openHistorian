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
using openHistorian.UnmanagedMemory;

namespace openHistorian.IO.Unmanaged
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
    unsafe public partial class BufferedFileStream : ISupportsBinaryStreamAdvanced
    {
        /// <summary>
        /// To synchronize all calls to this class.
        /// </summary>
        object m_syncRoot;
        /// <summary>
        /// To limit flushing to a single flush call
        /// </summary>
        object m_syncFlush;

        BufferPool m_pool;

        int m_dirtyPageSize;

        /// <summary>
        /// The file stream use by this class.
        /// </summary>
        FileStream m_baseStream;

        LeastRecentlyUsedPageReplacement m_pageReplacementAlgorithm;

        bool m_disposed;
        bool m_ownsStream;

        IoQueue m_queue;

        /// <summary>
        /// This event occurs any time new data is added to the BinaryStream's 
        /// internal memory. It gives the consumer of this class an opportunity to 
        /// properly initialize the data before it is handed to an IoSession.
        /// </summary>
        public event EventHandler<StreamBlockEventArgs> BlockLoadedFromDisk;

        /// <summary>
        /// This event occurs right before something is committed to the disk. 
        /// This gives the opportunity to finalize the data, such as updating checksums.
        /// After the block has been successfully written <see cref="ISupportsBinaryStreamAdvanced.BlockLoadedFromDisk"/>
        /// is called if the block is to remain in memory.
        /// </summary>
        public event EventHandler<StreamBlockEventArgs> BlockAboutToBeWrittenToDisk;

        public BufferedFileStream(FileStream stream, bool ownsStream = false)
            : this(stream, Globals.BufferPool, 4096, ownsStream)
        {

        }

        /// <summary>
        /// Creates a file backed memory stream.
        /// </summary>
        /// <param name="stream">The file stream to back</param>
        /// <param name="pool"></param>
        /// <param name="dirtyPageSize"></param>
        public BufferedFileStream(FileStream stream, BufferPool pool, int dirtyPageSize, bool ownsStream = false)
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


        void Flush(bool waitForWriteToDisk, bool skipPagesInUse, int desiredFlushCount)
        {
            lock (m_syncFlush)
            {
                PageMetaDataList.PageMetaData[] dirtyPages;
                lock (m_syncRoot)
                {
                    dirtyPages = m_pageReplacementAlgorithm.GetDirtyPages(skipPagesInUse).ToArray();
                    foreach (var block in dirtyPages)
                    {
                        m_pageReplacementAlgorithm.ClearDirtyBits(block);
                    }
                }
                m_queue.Write(dirtyPages, waitForWriteToDisk);
            }
        }

        void GetBlock(LeastRecentlyUsedPageReplacement.IoSession ioSession, long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
        {

            LeastRecentlyUsedPageReplacement.SubPageMetaData subPage;

            lock (m_syncRoot)
            {
                if (ioSession.TryGetSubPage(position, isWriting, out subPage))
                {
                    firstPointer = (IntPtr)subPage.Location;
                    length = subPage.Length;
                    firstPosition = subPage.Position;
                    supportsWriting = subPage.IsDirty;
                    return;
                }
            }

            Action<byte[]> callback = data =>
                {
                    lock (m_syncRoot)
                    {
                        ioSession.TryAddNewPage(position & ~(long)(data.Length - 1), data, 0, data.Length);
                        ioSession.TryGetSubPage(position, isWriting, out subPage);
                    }
                };

            m_queue.Read(position, callback);

            firstPointer = (IntPtr)subPage.Location;
            length = subPage.Length;
            firstPosition = subPage.Position;
            supportsWriting = subPage.IsDirty;
            return;

        }

        void Clear(LeastRecentlyUsedPageReplacement.IoSession ioSession)
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
                //Flush(true, false, -1);
                m_disposed = true;
                Globals.BufferPool.RequestCollection -= BufferPool_RequestCollection;
                m_pageReplacementAlgorithm.Dispose();
                if (m_ownsStream)
                    m_baseStream.Dispose();
            }
        }

        void BufferPool_RequestCollection(object sender, CollectionEventArgs e)
        {
            if (e.CollectionMode == BufferPoolCollectionMode.Critical)
            {
                lock (m_syncRoot)
                {
                    m_pageReplacementAlgorithm.DoCollection(e);
                }
                Flush(false, true, e.DesiredPageReleaseCount);
            }
            lock (m_syncRoot)
            {
                m_pageReplacementAlgorithm.DoCollection(e);
            }
        }

        IBinaryStreamIoSession ISupportsBinaryStream.GetNextIoSession()
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

        long ISupportsBinaryStreamAdvanced.Length
        {
            get
            {
                return m_baseStream.Length;
            }
        }

        long ISupportsBinaryStreamAdvanced.SetLength(long length)
        {
            lock (m_syncRoot)
            {
                //if (m_baseStream.Length < length)
                m_baseStream.SetLength(length);
                return m_baseStream.Length;
            }
        }

        public int BlockSize
        {
            get
            {
                return Globals.BufferPool.PageSize;
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
