//******************************************************************************************************
//  PageMetaDataList.cs - Gbtc
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
//  4/21/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V2.Collections;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged
{
    /// <summary>
    /// Contains a list of page meta data. Provides a simplified way to interact with this list.
    /// </summary>
    unsafe public class PageMetaDataList : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// Contains meta data about each page that is allocated.  
        /// Check <see cref="PageMetaDataList.m_isPageUsed"/> to see if the page is used.
        /// </summary>
        /// <remarks>This structure is used internally in the <see cref="PageMetaDataList.m_listOfPages"/> list.</remarks>
        struct InternalPageMetaData
        {
            /// <summary>
            /// The pointer
            /// </summary>
            public byte* LocationOfPage;
            /// <summary>
            /// The index assigned by the <see cref="BufferPool"/>.
            /// </summary>
            public int BufferPoolIndex;
            /// <summary>
            /// The bytes that this page represents. Position * 64KB.
            /// </summary>
            public int PositionIndex;
            /// <summary>
            /// The number of times this object has been referenced
            /// </summary>
            public int ReferencedCount;
            /// <summary>
            /// Dirty flags representing the 4KB block.
            /// </summary>
            public ushort IsDirtyFlags;

            public PageMetaData ToPageMetaData(int metaDataIndex)
            {
                return new PageMetaData
                {
                    LocationOfPage = LocationOfPage,
                    PositionIndex = PositionIndex,
                    IsDirtyFlags = IsDirtyFlags,
                    MetaDataIndex = metaDataIndex
                };
            }
        }

        public struct PageMetaData
        {
            public int MetaDataIndex;
            public byte* LocationOfPage;
            public int PositionIndex;
            public ushort IsDirtyFlags;
        }

        List<InternalPageMetaData> m_listOfPages;

        BitArray m_isPageUsed;

        bool m_disposed;

        #endregion

        #region [ Constructors ]

        public PageMetaDataList()
        {
            m_listOfPages = new List<InternalPageMetaData>();
            m_isPageUsed = new BitArray(80, false);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="PageMetaDataList"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~PageMetaDataList()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        InternalPageMetaData GetPage(int index)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (!m_isPageUsed[index])
                throw new IndexOutOfRangeException("index does not exist");
            return m_listOfPages[index];
        }
        void SetPage(int index, InternalPageMetaData value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (!m_isPageUsed[index])
                throw new IndexOutOfRangeException("index does not exist");
            m_listOfPages[index] = value;
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="PageMetaDataList"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="PageMetaDataList"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    for (int x = 0; x < m_listOfPages.Count; x++)
                    {
                        if (m_isPageUsed[x])
                        {
                            BufferPool.ReleasePage(m_listOfPages[x].BufferPoolIndex);
                        }
                    }
                    m_listOfPages = null;
                    m_isPageUsed = null;
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

        /// <summary>
        /// Finds the next unused cache page index. Marks it as used.
        /// Allocates a page from the buffer pool.
        /// </summary>
        /// <returns></returns>
        public int AllocateNewPage(int positionIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            //Get a free block to read the data to.
            int cachePageIndex = m_isPageUsed.FindClearedBit();
            if (cachePageIndex < 0)
            {
                m_isPageUsed.SetCapacity(m_isPageUsed.Count * 2);
                cachePageIndex = m_isPageUsed.FindClearedBit();
            }

            IntPtr ptr;

            //Allocate a new one
            InternalPageMetaData cachePage;
            cachePage.BufferPoolIndex = BufferPool.AllocatePage(out ptr);
            cachePage.LocationOfPage = (byte*)ptr;
            cachePage.IsDirtyFlags = 0;
            cachePage.ReferencedCount = 0;
            cachePage.PositionIndex = positionIndex;

            m_isPageUsed.SetBit(cachePageIndex);
            if (cachePageIndex < m_listOfPages.Count)
                m_listOfPages[cachePageIndex] = cachePage;
            else
                m_listOfPages.Add(cachePage);
            return cachePageIndex;
        }

        public PageMetaData GetMetaDataPage(int index, ushort isWritingFlag = 0, bool incrementReferencedCount = false)
        {
            var metaData = GetPage(index);
            metaData.IsDirtyFlags |= isWritingFlag;
            if (incrementReferencedCount && metaData.ReferencedCount != int.MaxValue)
                metaData.ReferencedCount++;
            SetPage(index, metaData);
            return metaData.ToPageMetaData(index);
        }

        public void ClearDirtyBits(int index)
        {
            var metaData = GetPage(index);
            metaData.IsDirtyFlags = 0;
            SetPage(index, metaData);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="index"></param>
        //public void ReleasePage(int index)
        //{
        //    if (m_disposed)
        //        throw new ObjectDisposedException(GetType().FullName);

        //    BufferPool.ReleasePage(GetPage(index).BufferPoolIndex);
        //    m_isPageUsed.ClearBit(index);
        //}

        /// <summary>
        /// Executes a collection cycle on the pages in this list.
        /// </summary>
        /// <param name="shiftLevel">the number of bits to shift the referenced counter by.
        /// Value may be zero but cannot be negative</param>
        /// <param name="shouldCollect">a function that notifies the caller what page is about to be
        /// collected and gives the caller an opertunity to override this collection attempt.</param>
        /// <returns>The number of pages returned to the buffer pool</returns>
        public int DoCollection(int shiftLevel, Func<int, bool> shouldCollect)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (shiftLevel < 0)
                throw new ArgumentOutOfRangeException("shiftLevel", "must be non negative");

            int collectionCount = 0;
            for (int x = 0; x < m_listOfPages.Count; x++)
            {
                if (m_isPageUsed[x])
                {
                    var block = m_listOfPages[x];
                    block.ReferencedCount >>= shiftLevel;
                    m_listOfPages[x] = block;
                    if (block.ReferencedCount == 0 && block.IsDirtyFlags == 0)
                    {
                        if (shouldCollect(x))
                        {
                            collectionCount++;
                            BufferPool.ReleasePage(block.BufferPoolIndex);
                            m_isPageUsed[x] = false;
                        }
                    }
                }
            }
            return collectionCount;
        }

        #endregion

    }
}
