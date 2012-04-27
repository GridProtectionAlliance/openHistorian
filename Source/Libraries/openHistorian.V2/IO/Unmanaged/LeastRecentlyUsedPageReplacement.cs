//******************************************************************************************************
//  LeastRecentlyUsedPageReplacement.cs - Gbtc
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
using System.Collections.Generic;
using openHistorian.V2.Unmanaged;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged
{
    /// <summary>
    /// A page replacement algorithm that utilizes a quasi LRU algorithm.
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="BufferedFileStream"/> to decide which pages should be replaced.
    /// </remarks>
    unsafe public class LeastRecentlyUsedPageReplacement : IDisposable
    {
        #region [ Members ]

        // Nested Types
        public class IoSession : IDisposable
        {
            LeastRecentlyUsedPageReplacement m_lru;
            bool m_disposed;
            public readonly int IoSessionId;

            public IoSession(LeastRecentlyUsedPageReplacement lru, int ioSessionId)
            {
                m_lru = lru;
                IoSessionId = ioSessionId;
            }
            /// <summary>
            /// Releases the unmanaged resources before the <see cref="IoSession"/> object is reclaimed by <see cref="GC"/>.
            /// </summary>
            ~IoSession()
            {
                Dispose(false);
            }

            public SubPageMetaData TryGetSubPageOrCreateNew(long position, bool isWriting, Action<IntPtr, long> delLoadFromFile)
            {
                return m_lru.TryGetSubPageOrCreateNew(position, IoSessionId, isWriting, delLoadFromFile);
            }

            public void Clear()
            {
                m_lru.ClearIoSession(IoSessionId);
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
            protected virtual void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        // This will be done regardless of whether the object is finalized or disposed.
                        m_lru.ReleaseIoSession(IoSessionId);

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
        }

        /// <summary>
        /// Contains meta data about each page that is allocated.  
        /// </summary>
        /// <remarks>This structure is passed to the users of this class.</remarks>
        public struct SubPageMetaData
        {
            public byte* Location;
            public long Position;
            public bool IsDirty;
            public int Length;
        }

        // Constants
        /// <summary>
        /// Gets the size of a sub page. A sub page is the smallest
        /// unit of I/O that can occur. This will then set dirty bits on this level.
        /// </summary>
        const int SubPageSize = 4096;

        const int SubPageShiftBits = 12;

        const int SubPageMask = SubPageSize - 1;

        // Delegates

        // Events

        // Fields
        bool m_disposed;

        /// <summary>
        /// contains a list of all the meta pages.
        /// </summary>
        /// <remarks>These items in the list are not in any particular order.
        /// To lookup the correct pages, use <see cref="m_allocatedPagesLookupList"/> </remarks>
        PageMetaDataList m_pageList;

        /// <summary>
        /// Contains the currently active IO sessions that cannot be cleaned up by a collection.
        /// </summary>
        List<int> m_activeBlockIndexes;

        /// <summary>
        /// Contains all of the pages that are cached for the file stream.
        /// </summary>
        /// ToDO: Change this type to a b+ tree so it can store more pages efficiently.
        SortedList<int, int> m_allocatedPagesLookupList;

        #endregion

        #region [ Constructors ]

        public LeastRecentlyUsedPageReplacement()
        {
            m_pageList = new PageMetaDataList();
            m_activeBlockIndexes = new List<int>();
            m_allocatedPagesLookupList = new SortedList<int, int>();
        }
        /// <summary>
        /// Releases the unmanaged resources before the <see cref="LeastRecentlyUsedPageReplacement"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~LeastRecentlyUsedPageReplacement()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        SubPageMetaData TryGetSubPageOrCreateNew(long position, int ioSession, bool isWriting, Action<IntPtr, long> delLoadFromFile)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            int pageIndex = (int)(position >> BufferPool.PageShiftBits);
            int subPageIndex = (int)((position & BufferPool.PageMask) >> SubPageShiftBits);
            ushort subPageDirtyFlag = (ushort)(1 << subPageIndex);
            bool existsInLookupTable;
            int pageMetaDataIndex = GetPageMetaDataIndex(pageIndex, out existsInLookupTable);

            m_activeBlockIndexes[ioSession] = pageMetaDataIndex;

            var pageMetaData = GetPageMetaData(isWriting, subPageDirtyFlag, pageMetaDataIndex);
            bool isSubPageDirty = ((pageMetaData.IsDirtyFlags & subPageDirtyFlag) != 0);

            if (!existsInLookupTable)
                delLoadFromFile.Invoke((IntPtr)pageMetaData.LocationOfPage, (long)pageMetaData.PositionIndex * BufferPool.PageSize);

            return new SubPageMetaData
            {
                IsDirty = isSubPageDirty,
                Location = pageMetaData.LocationOfPage + subPageIndex * SubPageSize,
                Position = position & ~(long)SubPageMask,
                Length = SubPageSize
            };
        }

        /// <summary>
        /// Creates a new IO Session.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// In order to prevent pages that are currently in use from being garbage collected,
        /// IO Sessions are used to keep track of currently used blocks.
        /// One IO session should be used for each unique request.  
        /// </remarks>
        public IoSession CreateNewIoSession()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            for (int x = 0; x < m_activeBlockIndexes.Count; x++)
            {
                if (m_activeBlockIndexes[x] == -2)
                {
                    m_activeBlockIndexes[x] = -1;
                    return new IoSession(this, x);
                }
            }
            m_activeBlockIndexes.Add(-1);
            return new IoSession(this, m_activeBlockIndexes.Count - 1);
        }

        void ReleaseIoSession(int ioSession)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_activeBlockIndexes[ioSession] = -2;
        }

        void ClearIoSession(int ioSession)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_activeBlockIndexes[ioSession] = -1;
        }

        /// <summary>
        /// Executes a collection cycle of the pages that are unused.
        /// </summary>
        /// <returns></returns>
        public int DoCollection()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return m_pageList.DoCollection(1, x => !m_activeBlockIndexes.Contains(x));
        }

        //public bool PageInUse(long position)
        //{
        //    if (m_disposed)
        //        throw new ObjectDisposedException(GetType().FullName);

        //    int pageIndex = (int)(position >> BufferPool.PageShiftBits);
        //    int pageMetaDataIndex;

        //    if (m_allocatedPagesLookupList.TryGetValue(pageIndex, out pageMetaDataIndex))
        //    {
        //        var metaDataPage = m_pageList[pageMetaDataIndex];
        //        metaDataPage.IsDirtyFlags = 0;
        //        m_pageList[pageMetaDataIndex] = metaDataPage;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Marks the entire page as written to the disk.
        /// </summary>
        /// <param name="pageMetaData"></param>
        /// <remarks>These pages are 64KB in size. Calling this function does not write the data to the 
        /// base stream.  It only sets the status of the corresponding page.</remarks>
        public void ClearDirtyBits(PageMetaDataList.PageMetaData pageMetaData)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (m_activeBlockIndexes.Contains(pageMetaData.MetaDataIndex))
                throw new NotSupportedException("A page's dirty bits can only be cleared if the page is currently not being used.");

            m_pageList.ClearDirtyBits(pageMetaData.MetaDataIndex);
        }

        /// <summary>
        /// Returns all dirty pages in this class.  If pages are subsequentially written to the disk,
        /// be sure to clear the dirty bits via <see cref="ClearDirtyBits"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PageMetaDataList.PageMetaData> GetDirtyPages(bool skipPagesInUse = false)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            foreach (var block in m_allocatedPagesLookupList)
            {
                var pageMetaData = m_pageList.GetMetaDataPage(block.Value);
                if (pageMetaData.IsDirtyFlags != 0 &&  //Page must be dirty
                    !(skipPagesInUse && m_activeBlockIndexes.Contains(block.Value)) //and not an actively used page if skip pages in use is set
                    )
                    yield return pageMetaData;
            }
        }


        /// <summary>
        /// Releases all the resources used by the <see cref="LeastRecentlyUsedPageReplacement"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="LeastRecentlyUsedPageReplacement"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    m_pageList.Dispose();
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

        #region [ Helper Methods ]

        PageMetaDataList.PageMetaData GetPageMetaData(bool isWriting, ushort subPageDirtyFlag, int pageMetaDataIndex)
        {
            if (isWriting)
                return m_pageList.GetMetaDataPage(pageMetaDataIndex, subPageDirtyFlag, true);
            else
                return m_pageList.GetMetaDataPage(pageMetaDataIndex, 0, true);
        }

        int GetPageMetaDataIndex(int pageIndex, out bool existsInLookupTable)
        {
            int pageMetaDataIndex;
            existsInLookupTable = m_allocatedPagesLookupList.TryGetValue(pageIndex, out pageMetaDataIndex);
            if (!existsInLookupTable)
            {
                pageMetaDataIndex = m_pageList.AllocateNewPage(pageIndex);
                m_allocatedPagesLookupList.Add(pageIndex, pageMetaDataIndex);
            }
            return pageMetaDataIndex;
        }

        #endregion

        #endregion

    }
}
