//******************************************************************************************************
//  PageReplacementAlgorithm.cs - Gbtc
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
using System.Collections.Generic;
using System.Linq;
using GSF.Diagnostics;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure.Media
{
    /// <summary>
    /// A page replacement algorithm that utilizes a quasi LRU algorithm. (NOT THREAD SAFE)
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="BufferedFile"/> to decide which pages should be replaced.
    /// This class is not thread safe.
    /// </remarks>
    internal class PageReplacementAlgorithm 
        : IDisposable
    {
        private static readonly LogType Log = Logger.LookupType(typeof(PageReplacementAlgorithm));

        #region [ Members ]

        private readonly int m_bufferPageSizeMask;
        private readonly int m_bufferPageSizeShiftBits;

        private bool m_disposed;

        /// <summary>
        /// contains a list of all the meta pages.
        /// </summary>
        /// <remarks>These items in the list are not in any particular order.</remarks>
        private readonly PageList m_pageList;

        /// <summary>
        /// Contains the currently active IO sessions.
        /// The value is either the array index or <see cref="IsNull"/> or <see cref="IsCleared"/>.
        /// </summary>
        private readonly List<PageLock> m_arrayIndexLocks;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of <see cref="PageReplacementAlgorithm"/>.
        /// </summary>
        /// <param name="pool">The buffer pool that blocks will be allocated on.</param>
        public PageReplacementAlgorithm(MemoryPool pool)
        {
            if (pool.PageSize < 4096)
                throw new ArgumentOutOfRangeException("PageSize Must be greater than 4096", "pool");
            if (!BitMath.IsPowerOfTwo(pool.PageSize))
                throw new ArgumentException("PageSize Must be a power of 2", "pool");

            m_bufferPageSizeMask = pool.PageSize - 1;
            m_bufferPageSizeShiftBits = BitMath.CountBitsSet((uint)m_bufferPageSizeMask);

            m_pageList = new PageList(pool);
            m_arrayIndexLocks = new List<PageLock>();
        }

        #endregion

#if DEBUG
        ~PageReplacementAlgorithm()
        {
            Log.Publish(VerboseLevel.Information, "Finalizer Called", GetType().FullName);
        }
#endif

        #region [ Methods ]

        /// <summary>
        /// Attempts to get a sub page. 
        /// </summary>
        /// <param name="pageLock">the lock to use for the read operation</param>
        /// <param name="position">the absolute position in the stream to get the page for.</param>
        /// <param name="location">a pointer for the page</param>
        /// <returns>False if the page does not exists and needs to be added.</returns>
        public bool TryGetSubPage(PageLock pageLock, long position, out IntPtr location)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if ((position & m_bufferPageSizeMask) != 0)
                throw new ArgumentOutOfRangeException("position", "must lie on a page boundary");

            int positionIndex = (int)(position >> m_bufferPageSizeShiftBits);
            int arrayIndex = m_pageList.ToArrayIndex(positionIndex);
            if (arrayIndex < 0)
            {
                location = default(IntPtr);
                return false;
            }
            pageLock.SetActiveBlock(arrayIndex);
            location = m_pageList.GetPointerToPage(arrayIndex, 1);
            ;
            return true;
        }

        /// <summary>
        /// Attempts to get a sub page. 
        /// </summary>
        /// <param name="position">the absolute position in the stream to get the page for.</param>
        /// <param name="location">a pointer for the page</param>
        /// <returns>False if the page does not exists and needs to be added.</returns>
        public bool TryGetSubPageNoLock(long position, out IntPtr location)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if ((position & m_bufferPageSizeMask) != 0)
                throw new ArgumentOutOfRangeException("position", "must lie on a page boundary");

            int positionIndex = (int)(position >> m_bufferPageSizeShiftBits);
            int arrayIndex = m_pageList.ToArrayIndex(positionIndex);
            if (arrayIndex < 0)
            {
                location = default(IntPtr);
                return false;
            }
            location = m_pageList.GetPointerToPage(arrayIndex, 1);
            ;
            return true;
        }

        /// <summary>
        /// Adds a page to the list of available pages if it does not exist.
        /// otherwise, returns the page already in the list.
        /// </summary>
        /// <param name="pageLock">the lock to use for the read operation</param>
        /// <param name="position">The position of the first byte in the page</param>
        /// <param name="startOfBufferPoolPage">the pointer acquired by the buffer pool to this data.</param>
        /// <param name="bufferPoolIndex">the buffer pool index for this data</param>
        /// <param name="wasPageAdded"> Determines if the page provided was indeed added to the list.</param>
        /// <returns>The pointer to the first block</returns>
        /// <remarks>If <see cref="wasPageAdded"/> is false, the calling function should 
        /// return the page back to the buffer pool.
        /// </remarks>
        public IntPtr AddOrGetPage(PageLock pageLock, long position, IntPtr startOfBufferPoolPage, int bufferPoolIndex, out bool wasPageAdded)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if ((position & m_bufferPageSizeMask) != 0)
                throw new ArgumentOutOfRangeException("position", "must lie on a page boundary");

            int positionIndex = (int)(position >> m_bufferPageSizeShiftBits);
            int arrayIndex = m_pageList.ToArrayIndex(positionIndex);
            if (arrayIndex >= 0) //If page already existed in the pool
            {
                pageLock.SetActiveBlock(arrayIndex);
                IntPtr location = m_pageList.GetPointerToPage(arrayIndex, 1);
                wasPageAdded = false;
                return location;
            }
            arrayIndex = m_pageList.AllocateNewPage(positionIndex, startOfBufferPoolPage, bufferPoolIndex);
            pageLock.SetActiveBlock(arrayIndex);
            wasPageAdded = true;
            return startOfBufferPoolPage;
        }

        public bool TryAddPage(long position, IntPtr startOfBufferPoolPage, int bufferPoolIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if ((position & m_bufferPageSizeMask) != 0)
                throw new ArgumentOutOfRangeException("position", "must lie on a page boundary");

            int positionIndex = (int)(position >> m_bufferPageSizeShiftBits);
            int arrayIndex = m_pageList.ToArrayIndex(positionIndex);
            if (arrayIndex >= 0) //If page already existed in the pool
            {
                return false;
            }
            arrayIndex = m_pageList.AllocateNewPage(positionIndex, startOfBufferPoolPage, bufferPoolIndex);
            return true;
        }

        /// <summary>
        /// Gets a object that can be used to acquire locks on pages so they won't be 
        /// released during a collection cycle.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// In order to prevent pages that are currently in use from being garbage collected,
        /// </remarks>
        public PageLock GetPageLock()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            foreach (PageLock pageLock in m_arrayIndexLocks)
            {
                if (pageLock.IsDisposed)
                {
                    pageLock.ResurrectLock();
                    return pageLock;
                }
            }
            PageLock pl = new PageLock();
            m_arrayIndexLocks.Add(pl);
            pl.ResurrectLock();
            return pl;
        }

        /// <summary>
        /// Executes a collection cycle of the pages that are unused.
        /// </summary>
        /// <returns></returns>
        public int DoCollection(CollectionEventArgs e)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return m_pageList.DoCollection(1, CheckForCollection, e);
        }

        /// <summary>
        /// Returns true if the page can be collected because it is not currently being referenced.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool CheckForCollection(int index)
        {
            return !m_arrayIndexLocks.Any(pageLock => (pageLock.CurrentBlock == index));
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="PageReplacementAlgorithm"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
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

        #region [ Helper Methods ]

        #endregion

        #endregion
    }
}