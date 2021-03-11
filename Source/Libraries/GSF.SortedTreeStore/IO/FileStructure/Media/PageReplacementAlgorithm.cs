//******************************************************************************************************
//  PageReplacementAlgorithm.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
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
using System.Linq;
using GSF.Collections;
using GSF.Diagnostics;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure.Media
{
    /// <summary>
    /// A page replacement algorithm that utilizes a quasi LRU algorithm. This class is thread safe.
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="BufferedFile"/> to decide which pages should be replaced.
    /// </remarks>
    internal partial class PageReplacementAlgorithm
        : IDisposable
    {

        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(PageReplacementAlgorithm), MessageClass.Component);

        #region [ Members ]

        private bool m_disposed;
        private readonly object m_syncRoot;
        private readonly int m_memoryPageSizeMask;
        private readonly int m_memoryPageSizeShiftBits;
        private readonly long m_maxValidPosition;

        /// <summary>
        /// Contains a list of all the memory pages.
        /// </summary>
        /// <remarks>These items in the list are not in any particular order.</remarks>
        private readonly PageList m_pageList;

        /// <summary>
        /// Contains the currently active IO sessions.
        /// </summary>
        private readonly WeakList<PageLock> m_arrayIndexLocks;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of <see cref="PageReplacementAlgorithm"/>.
        /// </summary>
        /// <param name="pool">The memory pool that blocks will be allocated from.</param>
        public PageReplacementAlgorithm(MemoryPool pool)
        {
            if (pool.PageSize < 4096)
                throw new ArgumentOutOfRangeException("PageSize Must be greater than 4096", "pool");
            if (!BitMath.IsPowerOfTwo(pool.PageSize))
                throw new ArgumentException("PageSize Must be a power of 2", "pool");

            m_maxValidPosition = (int.MaxValue - 1) * (long)pool.PageSize; //Max position 

            m_syncRoot = new object();
            m_memoryPageSizeMask = pool.PageSize - 1;
            m_memoryPageSizeShiftBits = BitMath.CountBitsSet((uint)m_memoryPageSizeMask);
            m_pageList = new PageList(pool);
            m_arrayIndexLocks = new WeakList<PageLock>();
        }

        #endregion

#if DEBUG
        ~PageReplacementAlgorithm()
        {
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
            //Don't dispose since only the page list contains data that must be released.
        }
#endif

        #region [ Methods ]

        //Two Methods Exist in PageLock subclass:
        //TryGetSubPage
        //GetOrAddPage

        /// <summary>
        /// Attempts to add the page to this <see cref="PageReplacementAlgorithm"/>. 
        /// Fails if the page already exists.
        /// </summary>
        /// <param name="position">the absolute position that the page references</param>
        /// <param name="locationOfPage">the pointer to the page</param>
        /// <param name="memoryPoolIndex">the index value of the memory pool page so it can be released back to the memory pool</param>
        /// <returns>True if the page was added to the class. False if the page already exists and the data was not replaced.</returns>
        public bool TryAddPage(long position, IntPtr locationOfPage, int memoryPoolIndex)
        {
            lock (m_syncRoot)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (position < 0)
                    throw new ArgumentOutOfRangeException("position", "Cannot be negative");
                if (position > m_maxValidPosition)
                    throw new ArgumentOutOfRangeException("position", "Position index can no longer be specified as an Int32");
                if ((position & m_memoryPageSizeMask) != 0)
                    throw new ArgumentOutOfRangeException("position", "must lie on a page boundary");
                int positionIndex = (int)(position >> m_memoryPageSizeShiftBits);

                if (m_pageList.TryGetPageIndex(positionIndex, out int pageIndex))
                {
                    return false;
                }
                m_pageList.AddNewPage(positionIndex, locationOfPage, memoryPoolIndex);
                return true;
            }
        }

        /// <summary>
        /// Executes a collection cycle of the pages that are unused.
        /// </summary>
        /// <returns></returns>
        public int DoCollection(CollectionEventArgs e)
        {
            lock (m_syncRoot)
            {
                if (m_disposed)
                    return 0;

                HashSet<int> pages = new HashSet<int>(m_arrayIndexLocks.Select(pageLock => pageLock.CurrentPageIndex));

                return m_pageList.DoCollection(1, pages, e);
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="PageReplacementAlgorithm"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                lock (m_syncRoot)
                {
                    try
                    {
                        m_pageList.Dispose();
                    }
                    finally
                    {
                        GC.SuppressFinalize(this);
                        m_disposed = true; // Prevent duplicate dispose.
                    }
                }

            }
        }

        #endregion
    }
}