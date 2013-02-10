//******************************************************************************************************
//  MemoryFile.cs - Gbtc
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
using System.Collections.Generic;
using System.Threading;
using GSF.IO.Unmanaged;
using GSF.UnmanagedMemory;

namespace openHistorian.FileStructure.IO
{
    /// <summary>
    /// Provides a in memory stream that uses pages that are pooled in the unmanaged buffer pool.
    /// </summary>
    unsafe internal partial class MemoryFile : DiskMediumBase
    {
        /// <summary>
        /// This class was created to allow settings update to be atomic.
        /// </summary>
        class Settings
        {
            const int Mask = 1023;
            const int ElementsPerRow = 1024;
            const int ShiftBits = 10;

            public int PageCount { get; private set; }
            int[][] m_pageIndex;
            byte*[][] m_pagePointer;

            public Settings()
            {
                m_pageIndex = new int[4][];
                m_pagePointer = new byte*[4][];
                PageCount = 0;
            }

            public byte* GetPointer(int page)
            {
                return m_pagePointer[page >> ShiftBits][page & Mask];
            }
            int GetIndex(int page)
            {
                return m_pageIndex[page >> ShiftBits][page & Mask];
            }

            public bool AddingRequiresClone
            {
                get
                {
                    return m_pagePointer.Length * ElementsPerRow == PageCount;
                }
            }

            void EnsureCapacity()
            {
                if (AddingRequiresClone)
                {
                    var oldIndex = m_pageIndex;
                    var oldPointer = m_pagePointer;

                    m_pageIndex = new int[m_pageIndex.Length * 2][];
                    m_pagePointer = new byte*[m_pagePointer.Length * 2][];
                    oldIndex.CopyTo(m_pageIndex, 0);
                    oldPointer.CopyTo(m_pagePointer, 0);
                }

                int bigIndex = PageCount >> ShiftBits;
                if (m_pageIndex[bigIndex] == null)
                {
                    m_pageIndex[bigIndex] = new int[ElementsPerRow];
                    m_pagePointer[bigIndex] = new byte*[ElementsPerRow];
                }

            }

            public void AddNewPage(byte* pagePointer, int pageIndex)
            {
                EnsureCapacity();
                int index = PageCount;
                int bigIndex = index >> ShiftBits;
                int smallIndex = index & Mask;
                m_pageIndex[bigIndex][smallIndex] = pageIndex;
                m_pagePointer[bigIndex][smallIndex] = pagePointer;
                Thread.MemoryBarrier(); //Incrementing the page count must occur after the data is correct.
                PageCount++;
            }

            public Settings Clone()
            {
                return (Settings)MemberwiseClone();
            }

            /// <summary>
            /// Returns all of the buffer pool page indexes used by this class
            /// </summary>
            /// <returns></returns>
            public IEnumerable<int> GetAllPageIndexes()
            {
                for (int x = 0; x < PageCount; x++)
                {
                    yield return GetIndex(x);
                }
            }


        }

        #region [ Members ]

        object m_syncRoot;

        volatile Settings m_settings;

        /// <summary>
        /// The buffer pool to utilize
        /// </summary>
        BufferPool m_pool;
        /// <summary>
        /// The number of bits in the page size.
        /// </summary>
        int m_shiftLength;
        /// <summary>
        /// The size of each page.
        /// </summary>
        int m_diskBlockSize;

        /// <summary>
        /// Releases all the resources used by the <see cref="MemoryFile"/> object.
        /// </summary>
        bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MemoryFile"/> using the default <see cref="BufferPool"/>.
        /// </summary>
        public MemoryFile(int blockSize)
            : this(GSF.Globals.BufferPool, blockSize)
        {
        }

        /// <summary>
        /// Create a new <see cref="MemoryFile"/>
        /// </summary>
        public MemoryFile(BufferPool pool, int fileStructureBlockSize)
            : base(pool.PageSize, fileStructureBlockSize)
        {
            m_pool = pool;
            m_shiftLength = pool.PageShiftBits;
            m_diskBlockSize = pool.PageSize;
            m_settings = new Settings();
            m_syncRoot = new object();
            Initialize(FileHeaderBlock.CreateNew(fileStructureBlockSize));
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="MemoryFile"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~MemoryFile()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets if the stream can be written to.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets if the stream has been disposed.
        /// </summary>
        public override bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Gets the length of the current stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return (long)m_settings.PageCount * DiskBlockSize;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MemoryFile"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    m_pool.ReleasePages(m_settings.GetAllPageIndexes());
                }
                finally
                {
                    m_pool = null;
                    m_settings = null;
                    m_disposed = true;
                }
            }
        }

        /// <summary>
        /// Aquire an IO Session.
        /// </summary>
        public override IBinaryStreamIoSession GetNextIoSession()
        {
            return new IoSession(this);
        }

        protected override void FlushWithHeader(FileHeaderBlock headerBlock)
        {

        }
        public override void RollbackChanges()
        {

        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Returns the page that corresponds to the absolute position.  
        /// This function will also autogrow the stream.
        /// </summary>
        /// <param name="position">The position to use to calculate the page to retrieve</param>
        /// <returns></returns>
        byte* GetPage(long position)
        {
            Settings settings = m_settings;

            int pageIndex = (int)(position >> m_shiftLength);

            if (pageIndex >= settings.PageCount)
            {
                IncreasePageCount(pageIndex + 1);
                settings = m_settings;
            }

            return settings.GetPointer(pageIndex);
        }

        /// <summary>
        /// Increases the size of the Memory Stream and updated the settings if needed
        /// </summary>
        /// <param name="pageCount"></param>
        void IncreasePageCount(int pageCount)
        {
            lock (m_syncRoot)
            {
                bool cloned = false;
                Settings settings = m_settings;
                if (settings.AddingRequiresClone)
                {
                    cloned = true;
                    settings = settings.Clone();
                }

                //If there are not enough pages in the stream, add enough.
                while (pageCount > settings.PageCount)
                {
                    int pageIndex;
                    IntPtr pagePointer;
                    m_pool.AllocatePage(out pageIndex, out pagePointer);
                    Memory.Clear(pagePointer, m_pool.PageSize);

                    //Footer.WriteChecksumResultsToFooter(pagePointer, DiskBlockSize, m_pool.PageSize);

                    settings.AddNewPage((byte*)pagePointer, pageIndex);
                }

                if (cloned)
                {
                    Thread.MemoryBarrier(); // make sure that all of the settings are saved before assigning
                    m_settings = settings;
                }
            }
        }

        void GetBlock(long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
        {
            if (m_disposed)
                throw new ObjectDisposedException("MemoryStream");

            length = m_diskBlockSize;
            firstPosition = position & ~(length - 1);
            firstPointer = (IntPtr)GetPage(position);
            supportsWriting = true;
        }

        #endregion

    }
}
