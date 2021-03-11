//******************************************************************************************************
//  PageList.cs - Gbtc
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
    /// Contains a list of page meta data. Provides a simplified way to interact with this list.
    /// This class is not thread safe.
    /// </summary>
    internal sealed unsafe class PageList
        : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(PageList), MessageClass.Component);


        #region [ Members ]

        /// <summary>
        /// The internal data stored about each page. This is address information, Position information
        /// </summary>
        private struct InternalPageMetaData
        {
            /// <summary>
            /// The pointer to the page.
            /// </summary>
            public byte* LocationOfPage;

            /// <summary>
            /// The index assigned by the <see cref="MemoryPool"/>.
            /// </summary>
            public int MemoryPoolIndex;

            /// <summary>
            /// The number of times this page has been referenced
            /// </summary>
            public int ReferencedCount;
        }

        /// <summary>
        /// Note: Memory pool must not be used to allocate memory since this is a blocking method.
        /// Otherwise, there exists the potential to deadlock.
        /// </summary>
        private readonly MemoryPool m_memoryPool;

        /// <summary>
        /// Contains all of the pages that are cached for the file stream.
        /// Map is PositionIndex,PageIndex
        /// </summary>
        /// ToDO: Change this to something faster than a sorted list.
        private readonly SortedList<int, int> m_pageIndexLookupByPositionIndex;

        /// <summary>
        /// A list of all pages that have been cached.
        /// </summary>
        private NullableLargeArray<InternalPageMetaData> m_listOfPages;

        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new PageMetaDataList.
        /// </summary>
        /// <param name="memoryPool">The buffer pool to utilize if any unmanaged memory needs to be created.</param>
        public PageList(MemoryPool memoryPool)
        {
            m_memoryPool = memoryPool;
            m_listOfPages = new NullableLargeArray<InternalPageMetaData>();
            m_pageIndexLookupByPositionIndex = new SortedList<int, int>();
        }

        ~PageList()
        {
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
            Dispose();
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Converts a number from its position index into a page index.
        /// </summary>
        /// <param name="positionIndex">the position divided by the page size.</param>
        /// <param name="pageIndex">the page index</param>
        /// <returns>true if found, false if not found</returns>
        public bool TryGetPageIndex(int positionIndex, out int pageIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return m_pageIndexLookupByPositionIndex.TryGetValue(positionIndex, out pageIndex);
        }

        /// <summary>
        /// Finds the next unused cache page index. Marks it as used.
        /// Assigns the page information that comes from the memory pool.
        /// </summary>
        /// <returns>The Page Index</returns>
        public int AddNewPage(int positionIndex, IntPtr locationOfPage, int memoryPoolIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            InternalPageMetaData cachePage;
            cachePage.MemoryPoolIndex = memoryPoolIndex;
            cachePage.LocationOfPage = (byte*)locationOfPage;
            cachePage.ReferencedCount = 0;
            int pageIndex = m_listOfPages.AddValue(cachePage);
            m_pageIndexLookupByPositionIndex.Add(positionIndex, pageIndex);
            return pageIndex;
        }


        /// <summary>
        /// Returns the pointer for the provided page index. 
        /// </summary>
        /// <param name="pageIndex">the index of the page that has been looked up for the position.</param>
        /// <param name="incrementReferencedCount">the value to increment the referenced count.</param>
        /// <returns></returns>
        public IntPtr GetPointerToPage(int pageIndex, int incrementReferencedCount)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            InternalPageMetaData metaData = m_listOfPages.GetValue(pageIndex);
            if (incrementReferencedCount > 0)
            {
                long newValue = metaData.ReferencedCount + (long)incrementReferencedCount;
                if (newValue > int.MaxValue)
                {
                    metaData.ReferencedCount = int.MaxValue;
                }
                else if (newValue < 0)
                {
                    metaData.ReferencedCount = 0;
                }
                else
                {
                    metaData.ReferencedCount = (int)newValue;
                }
                m_listOfPages.OverwriteValue(pageIndex, metaData);
            }
            return (IntPtr)metaData.LocationOfPage;
        }

        /// <summary>
        /// Executes a collection cycle on the pages in this list.
        /// </summary>
        /// <param name="shiftLevel">the number of bits to shift the referenced counter by.
        /// Value may be zero but cannot be negative</param>
        /// <param name="excludedList">A set of values to exclude from the collection process</param>
        /// <param name="e">Arguments for the collection.</param>
        /// <returns>The number of pages returned to the memory pool</returns>
        /// <remarks>If the collection mode is Emergency or Critical, it will only release the required number of pages and no more</remarks>
        //ToDo: Since i'll be parsing the entire list, rebuilding a new sorted tree may be quicker than removing individual blocks and copying.
        //ToDo: Also, I should probably change the ShouldCollect callback to an IEnumerable<int>.
        public int DoCollection(int shiftLevel, HashSet<int> excludedList, CollectionEventArgs e)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (shiftLevel < 0)
                throw new ArgumentOutOfRangeException("shiftLevel", "must be non negative");

            int collectionCount = 0;
            int maxCollectCount = -1;
            if (e.CollectionMode == MemoryPoolCollectionMode.Emergency || e.CollectionMode == MemoryPoolCollectionMode.Critical)
            {
                maxCollectCount = e.DesiredPageReleaseCount;
            }

            for (int x = 0; x < m_pageIndexLookupByPositionIndex.Count; x++)
            {
                int pageIndex = m_pageIndexLookupByPositionIndex.Values[x];

                InternalPageMetaData block = m_listOfPages.GetValue(pageIndex);
                block.ReferencedCount >>= shiftLevel;
                m_listOfPages.OverwriteValue(pageIndex, block);
                if (block.ReferencedCount == 0)
                {
                    if (maxCollectCount != collectionCount)
                    {
                        if (!excludedList.Contains(pageIndex))
                        {
                            collectionCount++;
                            m_pageIndexLookupByPositionIndex.RemoveAt(x);
                            x--;
                            m_listOfPages.SetNull(pageIndex);
                            e.ReleasePage(block.MemoryPoolIndex);
                        }
                    }
                }
            }
            return collectionCount;
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="PageList"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (!m_memoryPool.IsDisposed)
                    {
                        m_memoryPool.ReleasePages(m_listOfPages.Select(x => x.MemoryPoolIndex));
                        m_listOfPages = null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Publish(MessageLevel.Critical, "Unhandled exception when returning resources to the memory pool", null, null, ex);
                }
                finally
                {
                    GC.SuppressFinalize(this);
                    m_disposed = true; // Prevent duplicate dispose.
                }
            }
        }

        #endregion
    }
}