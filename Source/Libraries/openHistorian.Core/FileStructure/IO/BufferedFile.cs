//******************************************************************************************************
//  BufferedFile.cs - Gbtc
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
using GSF;
using GSF.IO.Unmanaged;
using GSF.UnmanagedMemory;

namespace openHistorian.FileStructure.IO
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
    internal partial class BufferedFile : IDiskMedium
    {
        private MemoryPoolStreamCore m_writeBuffer;

        /// <summary>
        /// Contains the number of bytes contained in the committed area of the file.
        /// </summary>
        private long m_lengthOfCommittedData;

        /// <summary>
        /// Contains the length of the 10 header pages. 
        /// </summary>
        private readonly long m_lengthOfHeader;

        /// <summary>
        /// To synchronize all calls to this class.
        /// </summary>
        private readonly object m_syncRoot;

        private readonly MemoryPool m_pool;

        /// <summary>
        /// The file stream use by this class.
        /// </summary>
        private FileStream m_baseStream;

        private PageReplacementAlgorithm m_pageReplacementAlgorithm;

        private bool m_disposed;

        /// <summary>
        /// All I/O to the disk is done at this maximum block size. Usually 64KB
        /// This value must be less than or equal to the MemoryPool's Buffer Size.
        /// </summary>
        private readonly int m_diskBlockSize;

        /// <summary>
        /// The size of an individual block of the FileStructure. Usually 4KB.
        /// </summary>
        private readonly int m_fileStructureBlockSize;

        private IoQueue m_queue;

        /// <summary>
        /// Creates a file backed memory stream.
        /// </summary>
        /// <param name="stream">The file stream to back</param>
        /// <param name="pool"></param>
        /// <param name="header"></param>
        /// <param name="isNewFile"></param>
        public BufferedFile(FileStream stream, MemoryPool pool, FileHeaderBlock header, bool isNewFile)
        {
            m_fileStructureBlockSize = header.BlockSize;
            m_diskBlockSize = pool.PageSize;
            m_lengthOfHeader = header.BlockSize * 10;
            m_writeBuffer = new MemoryPoolStreamCore(pool);
            m_pool = pool;
            m_queue = new IoQueue(stream, pool.PageSize, header.BlockSize, this);
            m_syncRoot = new object();
            m_pageReplacementAlgorithm = new PageReplacementAlgorithm(pool);
            m_baseStream = stream;
            pool.RequestCollection += BufferPool_RequestCollection;

            if (isNewFile)
            {
                byte[] headerBytes = header.GetBytes();
                stream.Position = 0;
                for (int x = 0; x < 10; x++)
                {
                    stream.Write(headerBytes, 0, headerBytes.Length);
                }
            }
            m_lengthOfCommittedData = (header.LastAllocatedBlock + 1) * (long)header.BlockSize;
            m_writeBuffer.ConfigureAlignment(m_lengthOfCommittedData, pool.PageSize);
        }

        private void GetBlock(PageLock pageLock, BlockArguments args)
        {
            pageLock.Clear();
            if (args.position >= m_lengthOfCommittedData)
            {
                args.supportsWriting = true;
                m_writeBuffer.GetBlock(args);
            }
            else if (args.position < m_lengthOfHeader)
            {
                throw new ArgumentOutOfRangeException("args", "Cannot use this function to modify the file header.");
            }
            else
            {
                if (args.isWriting)
                    throw new ArgumentException("Cannot write to committed data space", "args");
                args.supportsWriting = false;
                args.length = m_diskBlockSize;

                args.firstPosition = args.position & ~(long)m_pool.PageMask; //rounds to the beginning of the block to be looked up.

                GetBlockFromCommittedSpace(pageLock, args.firstPosition, out args.firstPointer);

                //Make sure the block does not go beyond the end of the uncommitted space.
                if (args.firstPosition + args.length > m_lengthOfCommittedData)
                    args.length = (int)(m_lengthOfCommittedData - args.firstPosition);
            }
        }

        private void GetBlockFromCommittedSpace(PageLock pageLock, long position, out IntPtr firstPointer)
        {
            lock (m_syncRoot)
            {
                if (m_pageReplacementAlgorithm.TryGetSubPage(pageLock, position, out firstPointer))
                {
                    return;
                }
            }

            //If the address doesn't exist in the current list. Read it from the disk.
            int poolPageIndex;
            IntPtr poolAddress;
            m_pool.AllocatePage(out poolPageIndex, out poolAddress);

            m_queue.Read(position, poolAddress);
            bool wasPageAdded;
            lock (m_syncRoot)
            {
                firstPointer = m_pageReplacementAlgorithm.AddOrGetPage(pageLock, position, poolAddress, poolPageIndex, out wasPageAdded);
            }
            if (!wasPageAdded)
                m_pool.ReleasePage(poolPageIndex);
        }

        public void FlushWithHeader(FileHeaderBlock header)
        {
            //Determine how much committed data to write
            long lengthOfAllData = (header.LastAllocatedBlock + 1) * (long)m_fileStructureBlockSize;
            long copyLength = lengthOfAllData - m_lengthOfCommittedData;

            //Write the uncommitted data.
            m_queue.Write(m_writeBuffer, m_lengthOfCommittedData, copyLength, waitForWriteToDisk: true);

            //Update the new header to position 0, position 1, and one of position 2-9
            byte[] bytes = header.GetBytes();
            m_queue.WriteToDisk(0, bytes, m_fileStructureBlockSize);
            m_queue.WriteToDisk(m_fileStructureBlockSize, bytes, m_fileStructureBlockSize);
            m_queue.WriteToDisk(m_fileStructureBlockSize * ((header.SnapshotSequenceNumber & 7) + 2), bytes, m_fileStructureBlockSize);
            m_queue.FlushFileBuffers();

            long startPos;

            //Copy recently committed data to the buffer pool
            if ((m_lengthOfCommittedData & (m_diskBlockSize - 1)) != 0) //Only if there is a split page.
            {
                startPos = m_lengthOfCommittedData & (~(long)(m_diskBlockSize - 1));
                //Finish filling up the split page in the buffer.
                lock (m_syncRoot)
                {
                    IntPtr ptrDest;

                    if (m_pageReplacementAlgorithm.TryGetSubPageNoLock(startPos, out ptrDest))
                    {
                        int length;
                        IntPtr ptrSrc;
                        m_writeBuffer.ReadBlock(m_lengthOfCommittedData, out ptrSrc, out length);
                        Footer.WriteChecksumResultsToFooter(ptrSrc, m_fileStructureBlockSize, length);
                        ptrDest += (m_diskBlockSize - length);
                        Memory.Copy(ptrSrc, ptrDest, length);
                    }
                }
                startPos += m_diskBlockSize;
            }
            else
            {
                startPos = m_lengthOfCommittedData;
            }

            while (startPos < lengthOfAllData)
            {
                //If the address doesn't exist in the current list. Read it from the disk.
                int poolPageIndex;
                IntPtr poolAddress;
                m_pool.AllocatePage(out poolPageIndex, out poolAddress);
                m_writeBuffer.CopyTo(startPos, poolAddress, m_diskBlockSize);
                Footer.WriteChecksumResultsToFooter(poolAddress, m_fileStructureBlockSize, m_diskBlockSize);

                bool wasPageAdded;
                lock (m_syncRoot)
                {
                    wasPageAdded = m_pageReplacementAlgorithm.TryAddPage(startPos, poolAddress, poolPageIndex);
                }
                if (!wasPageAdded)
                    m_pool.ReleasePage(poolPageIndex);

                startPos += m_diskBlockSize;
            }

            m_lengthOfCommittedData = lengthOfAllData;
            m_writeBuffer.ConfigureAlignment(m_lengthOfCommittedData, m_pool.PageSize);
        }

        public void RollbackChanges()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    m_disposed = true;
                    //Unregistering from this event gaurentees that a collection will no longer
                    //be called since this class utilizes custom code to garentee this.
                    Globals.MemoryPool.RequestCollection -= BufferPool_RequestCollection;

                    lock (m_syncRoot)
                    {
                        m_pageReplacementAlgorithm.Dispose();
                        m_baseStream.Dispose();
                        m_writeBuffer.Dispose();
                    }
                }
                finally
                {
                    m_baseStream = null;
                    m_disposed = true;
                    m_pageReplacementAlgorithm = null;
                    m_writeBuffer = null;
                    m_queue = null;
                }
            }
        }

        private void BufferPool_RequestCollection(object sender, CollectionEventArgs e)
        {
            if (m_disposed)
                return;

            lock (m_syncRoot)
            {
                if (m_disposed)
                    return;
                m_pageReplacementAlgorithm.DoCollection(e);
            }

            if (e.CollectionMode == BufferPoolCollectionMode.Critical)
            {
                //ToDo: actually do something differently if collection level reaches critical
                lock (m_syncRoot)
                {
                    if (m_disposed)
                        return;
                    m_pageReplacementAlgorithm.DoCollection(e);
                }
            }
        }

        public BinaryStreamIoSessionBase CreateIoSession()
        {
            lock (m_syncRoot)
            {
                return new IoSession(this, m_pageReplacementAlgorithm.GetPageLock());
            }
        }

        public long Length
        {
            get
            {
                return m_baseStream.Length;
            }
        }
    }
}