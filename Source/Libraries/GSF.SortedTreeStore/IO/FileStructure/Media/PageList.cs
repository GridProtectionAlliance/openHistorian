//******************************************************************************************************
//  PageList.cs - Gbtc
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
using GSF.Collections;
using GSF.Diagnostics;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure.Media
{
    /// <summary>
    /// Contains a list of page meta data. Provides a simplified way to interact with this list.
    /// This class is not thread safe.
    /// </summary>
    /// <remarks>Maintains the following relationship: PositionIndex, ArrayIndex, PageMetaData</remarks>
    internal sealed unsafe class PageList
        : IDisposable
    {
        private static readonly LogType Log = Logger.LookupType(typeof(PageList));


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

        private readonly MemoryPool m_memoryPool;

        /// <summary>
        /// Contains all of the pages that are cached for the file stream.
        /// Map is PositionIndex,ArrayIndex
        /// </summary>
        /// ToDO: Change this to something faster than a sorted list.
        private readonly SortedList<int, int> m_pageLookupByPositionIndex;

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
            m_pageLookupByPositionIndex = new SortedList<int, int>();
        }

#if DEBUG
        ~PageList()
        {
            Log.Publish(VerboseLevel.Information, "Finalizer Called", GetType().FullName);
        }
#endif

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Converts a number from its position index into an array index.
        /// </summary>
        /// <param name="positionIndex"></param>
        /// <returns>returns a -1 if the position index does not exist</returns>
        public int ToArrayIndex(int positionIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            int arrayIndex;
            if (m_pageLookupByPositionIndex.TryGetValue(positionIndex, out arrayIndex))
            {
                return arrayIndex;
            }
            return -1;
        }


        /// <summary>
        /// Finds the next unused cache page index. Marks it as used.
        /// Allocates a page from the buffer pool.
        /// </summary>
        /// <returns>The Array Index</returns>
        public int AllocateNewPage(int positionIndex, IntPtr locationOfPage, int bufferPoolIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            InternalPageMetaData cachePage;
            cachePage.MemoryPoolIndex = bufferPoolIndex;
            cachePage.LocationOfPage = (byte*)locationOfPage;
            cachePage.ReferencedCount = 0;
            int arrayIndex = m_listOfPages.AddValue(cachePage);
            m_pageLookupByPositionIndex.Add(positionIndex, arrayIndex);
            return arrayIndex;
        }


        /// <summary>
        /// Returns the meta data page for the provided index. 
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <param name="incrementReferencedCount">the level to increment the referenced count.</param>
        /// <returns></returns>
        public IntPtr GetPointerToPage(int arrayIndex, int incrementReferencedCount)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            InternalPageMetaData metaData = m_listOfPages.GetValue(arrayIndex);
            if (incrementReferencedCount > 0)
            {
                long newValue = metaData.ReferencedCount + (long)incrementReferencedCount;
                if (newValue > int.MaxValue)
                    metaData.ReferencedCount = int.MaxValue;
                else
                    metaData.ReferencedCount = (int)newValue;
            }
            m_listOfPages.OverwriteValue(arrayIndex, metaData);
            return (IntPtr)metaData.LocationOfPage;
        }

        /// <summary>
        /// Executes a collection cycle on the pages in this list.
        /// </summary>
        /// <param name="shiftLevel">the number of bits to shift the referenced counter by.
        /// Value may be zero but cannot be negative</param>
        /// <param name="shouldCollect">a function that notifies the caller what page is about to be
        /// collected and gives the caller an opertunity to override this collection attempt.</param>
        /// <param name="e">Arguments for the collection.</param>
        /// <returns>The number of pages returned to the buffer pool</returns>
        /// <remarks>If the collection mode is Emergency or Critical, it will only release the required number of pages and no more</remarks>
        //ToDo: Since i'll be parsing the entire list, rebuilding a new sorted tree may be quicker than removing individual blocks and copying.
        //ToDo: Also, I should probably change the ShouldCollect callback to an IEnumerable<int>.
        public int DoCollection(int shiftLevel, Func<int, bool> shouldCollect, CollectionEventArgs e)
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

            for (int x = 0; x < m_pageLookupByPositionIndex.Count; x++)
            {
                int position = m_pageLookupByPositionIndex.Keys[x];
                int index = m_pageLookupByPositionIndex.Values[x];

                if (m_listOfPages.HasValue(index))
                {
                    InternalPageMetaData block = m_listOfPages.GetValue(index);
                    block.ReferencedCount >>= shiftLevel;
                    m_listOfPages.SetValue(index, block);
                    if (block.ReferencedCount == 0)
                    {
                        if (maxCollectCount != collectionCount)
                        {
                            if (shouldCollect(index))
                            {
                                collectionCount++;
                                m_pageLookupByPositionIndex.RemoveAt(x);
                                x--;
                                m_listOfPages.SetNull(index);
                                e.ReleasePage(block.MemoryPoolIndex);
                            }
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
                    Log.Publish(VerboseLevel.Critical, "Unhandled exception when returning resources to the memory pool", null, null, ex);
                    throw;
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