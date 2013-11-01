////******************************************************************************************************
////  PageMetaDataList.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  4/21/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using GSF.Collections;
//using GSF.UnmanagedMemory;

//namespace GSF.IO.Unmanaged
//{
//    /// <summary>
//    /// Contains a list of page meta data. Provides a simplified way to interact with this list.
//    /// This class is not thread safe.
//    /// </summary>
//    /// <remarks>Maintains the following relationship: PositionIndex, ArrayIndex, PageMetaData</remarks>
//    public unsafe class PageMetaDataList : IDisposable
//    {
//        #region [ Members ]

//        private struct InternalPageMetaData
//        {
//            /// <summary>
//            /// Dirty flags representing the user defined block size.
//            /// </summary>
//            public ulong IsDirtyFlags;

//            /// <summary>
//            /// The pointer
//            /// </summary>
//            public byte* LocationOfPage;

//            /// <summary>
//            /// The index assigned by the <see cref="MemoryPool"/>.
//            /// </summary>
//            public int BufferPoolIndex;

//            /// <summary>
//            /// The bytes that this page represents. Position * BufferPoolSize.
//            /// </summary>
//            public int PositionIndex;

//            /// <summary>
//            /// The number of times this object has been referenced
//            /// </summary>
//            public int ReferencedCount;

//            /// <summary>
//            /// Convert struct to a PageMetaData struct.
//            /// </summary>
//            /// <param name="arrayIndex">the Array Index to store in the PageMetaData</param>
//            /// <returns></returns>
//            public PageMetaData ToPageMetaData(int arrayIndex)
//            {
//                return new PageMetaData
//                {
//                    LocationOfPage = LocationOfPage,
//                    PositionIndex = PositionIndex,
//                    IsDirtyFlags = IsDirtyFlags,
//                    ArrayIndex = arrayIndex
//                };
//            }
//        }

//        /// <summary>
//        /// Contains meta data about each page.
//        /// </summary>
//        public struct PageMetaData
//        {
//            /// <summary>
//            /// Dirty flags representing the user defined block size.
//            /// </summary>
//            public ulong IsDirtyFlags;

//            /// <summary>
//            /// The pointer
//            /// </summary>
//            public byte* LocationOfPage;

//            /// <summary>
//            /// The index position in the <see cref="PageMetaDataList"/> of this page so updates can occur rapidly.
//            /// </summary>
//            public int ArrayIndex;

//            /// <summary>
//            /// The bytes that this page represents. Position * BufferPoolSize.
//            /// </summary>
//            public int PositionIndex;
//        }

//        private readonly MemoryPool m_pool;

//        /// <summary>
//        /// Contains all of the pages that are cached for the file stream.
//        /// Map is PositionIndex,ArrayIndex
//        /// </summary>
//        /// ToDO: Change this type to a b+ tree so it can store more pages efficiently.
//        private readonly SortedList<int, int> m_indexMap;

//        /// <summary>
//        /// A list of all pages.
//        /// </summary>
//        private NullableLargeArray<InternalPageMetaData> m_listOfPages;

//        private bool m_disposed;

//        #endregion

//        #region [ Constructors ]

//        /// <summary>
//        /// Creates a new PageMetaDataList.
//        /// </summary>
//        /// <param name="pool">The buffer pool to utilize if any unmanaged memory needs to be created.</param>
//        public PageMetaDataList(MemoryPool pool)
//        {
//            m_pool = pool;
//            m_listOfPages = new NullableLargeArray<InternalPageMetaData>();
//            m_indexMap = new SortedList<int, int>();
//        }

//        #endregion

//        #region [ Properties ]

//        private InternalPageMetaData this[int index]
//        {
//            get
//            {
//                if (m_disposed)
//                    throw new ObjectDisposedException(GetType().FullName);
//                return m_listOfPages.GetValue(index);
//            }
//            set
//            {
//                if (m_disposed)
//                    throw new ObjectDisposedException(GetType().FullName);
//                m_listOfPages.OverwriteValue(index, value);
//            }
//        }

//        #endregion

//        #region [ Methods ]

//        /// <summary>
//        /// Releases all the resources used by the <see cref="PageMetaDataList"/> object.
//        /// </summary>
//        public void Dispose()
//        {
//            if (!m_disposed)
//            {
//                try
//                {
//                    if (!m_pool.IsDisposed)
//                    {
//                        m_pool.ReleasePages(m_listOfPages.GetEnumerator(x => x.BufferPoolIndex));
//                        m_listOfPages.Dispose();
//                        m_listOfPages = null;
//                    }
//                }
//                finally
//                {
//                    m_disposed = true; // Prevent duplicate dispose.
//                }
//            }
//        }

//        /// <summary>
//        /// Converts a number from its position index into an array index.
//        /// </summary>
//        /// <param name="positionIndex"></param>
//        /// <returns>returns a -1 if the position index does not exist</returns>
//        public int ToArrayIndex(int positionIndex)
//        {
//            int arrayIndex;
//            if (m_indexMap.TryGetValue(positionIndex, out arrayIndex))
//            {
//                return arrayIndex;
//            }
//            return -1;
//        }


//        /// <summary>
//        /// Finds the next unused cache page index. Marks it as used.
//        /// Allocates a page from the buffer pool.
//        /// </summary>
//        /// <returns>The Array Index</returns>
//        public int AllocateNewPage(int positionIndex)
//        {
//            if (m_disposed)
//                throw new ObjectDisposedException(GetType().FullName);
//            InternalPageMetaData cachePage;
//            IntPtr ptr;
//            int index;
//            m_pool.AllocatePage(out index, out ptr);
//            cachePage.BufferPoolIndex = index;
//            cachePage.LocationOfPage = (byte*)ptr;
//            cachePage.IsDirtyFlags = 0;
//            cachePage.ReferencedCount = 0;
//            cachePage.PositionIndex = positionIndex;
//            int arrayIndex = m_listOfPages.AddValue(cachePage);
//            m_indexMap.Add(positionIndex, arrayIndex);
//            return arrayIndex;
//        }


//        /// <summary>
//        /// Returns the meta data page for the provided index. 
//        /// </summary>
//        /// <param name="arrayIndex"></param>
//        /// <param name="isWritingMask">A write mask that is used to set the dirty flags if they need to be modified.</param>
//        /// <param name="incrementReferencedCount">the level to increment the referenced count.</param>
//        /// <returns></returns>
//        public PageMetaData GetMetaDataPage(int arrayIndex, ulong isWritingMask, int incrementReferencedCount)
//        {
//            InternalPageMetaData metaData = this[arrayIndex];
//            metaData.IsDirtyFlags |= isWritingMask;
//            if (incrementReferencedCount > 0)
//            {
//                long newValue = metaData.ReferencedCount + (long)incrementReferencedCount;
//                if (newValue > int.MaxValue)
//                    metaData.ReferencedCount = int.MaxValue;
//                else
//                    metaData.ReferencedCount = (int)newValue;
//            }
//            this[arrayIndex] = metaData;
//            return metaData.ToPageMetaData(arrayIndex);
//        }

//        /// <summary>
//        /// Clears all of the dirty bits for a given index.
//        /// </summary>
//        /// <param name="arrayIndex"></param>
//        public void ClearDirtyBits(int arrayIndex)
//        {
//            InternalPageMetaData metaData = this[arrayIndex];
//            metaData.IsDirtyFlags = 0;
//            this[arrayIndex] = metaData;
//        }

//        /// <summary>
//        /// Returns all dirty pages in this class.  If pages are subsequentially written to the disk,
//        /// be sure to clear the dirty bits via <see cref="ClearDirtyBits"/>.
//        /// </summary>
//        /// <returns></returns>
//        //ToDo: I should probably change the isFiltered callback to an IEnumerable<int>.
//        public IEnumerable<PageMetaData> GetDirtyPages(Func<int, bool> isFiltered)
//        {
//            if (m_disposed)
//                throw new ObjectDisposedException(GetType().FullName);

//            foreach (KeyValuePair<int, int> block in m_indexMap)
//            {
//                InternalPageMetaData pageMetaData = this[block.Value];
//                if (pageMetaData.IsDirtyFlags != 0 && !isFiltered(block.Value))
//                    yield return pageMetaData.ToPageMetaData(block.Value);
//            }
//        }

//        /// <summary>
//        /// Executes a collection cycle on the pages in this list.
//        /// </summary>
//        /// <param name="shiftLevel">the number of bits to shift the referenced counter by.
//        /// Value may be zero but cannot be negative</param>
//        /// <param name="shouldCollect">a function that notifies the caller what page is about to be
//        /// collected and gives the caller an opertunity to override this collection attempt.</param>
//        /// <param name="e">Arguments for the collection.</param>
//        /// <returns>The number of pages returned to the buffer pool</returns>
//        /// <remarks>If the collection mode is Emergency or Critical, it will only release the required number of pages and no more</remarks>
//        //ToDo: Since i'll be parsing the entire list, rebuilding a new sorted tree may be quicker than removing individual blocks and copying.
//        //ToDo: Also, I should probably change the ShouldCollect callback to an IEnumerable<int>.
//        public int DoCollection(int shiftLevel, Func<int, bool> shouldCollect, CollectionEventArgs e)
//        {
//            int maxCollectCount = -1;
//            if (e.CollectionMode == BufferPoolCollectionMode.Emergency || e.CollectionMode == BufferPoolCollectionMode.Critical)
//            {
//                maxCollectCount = e.DesiredPageReleaseCount;
//            }

//            if (m_disposed)
//                throw new ObjectDisposedException(GetType().FullName);

//            if (shiftLevel < 0)
//                throw new ArgumentOutOfRangeException("shiftLevel", "must be non negative");

//            int collectionCount = 0;
//            for (int x = 0; x < m_listOfPages.Capacity; x++)
//            {
//                if (m_listOfPages.HasValue(x))
//                {
//                    InternalPageMetaData block = m_listOfPages.GetValue(x);
//                    block.ReferencedCount >>= shiftLevel;
//                    m_listOfPages.SetValue(x, block);
//                    if (block.ReferencedCount == 0 && block.IsDirtyFlags == 0)
//                    {
//                        if (maxCollectCount != collectionCount)
//                        {
//                            if (shouldCollect(x))
//                            {
//                                collectionCount++;
//                                m_indexMap.Remove(block.PositionIndex);
//                                m_listOfPages.SetNull(x);
//                                e.ReleasePage(block.BufferPoolIndex);
//                            }
//                        }
//                    }
//                }
//            }
//            return collectionCount;
//        }

//        #endregion
//    }
//}