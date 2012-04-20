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
using openHistorian.V2.Collections;
using openHistorian.V2.Unmanaged;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged
{
    unsafe public class LeastRecentlyUsedPageReplacement
    {
        /// <summary>
        /// Contains meta data about each page that is allocated.  
        /// Check <see cref="BufferedFileStream.m_isPageMetaDataNotNull"/> to see if the page is used.
        /// </summary>
        public struct PageMetaDataStruct
        {
            public byte* LocationOfPage;
            public int BufferPoolIndex;
            public int ReferencedCount;
            public ushort IsDirtyFlags;
        }

        /// <summary>
        /// Contains meta data about each page that is allocated.  
        /// Check <see cref="BufferedFileStream.m_isPageMetaDataNotNull"/> to see if the page is used.
        /// </summary>
        public struct PageMetaData
        {
            public byte* LocationOfPage;
            public long StreamPosition;
            public ushort IsDirtyFlags;
            public PageMetaData(PageMetaDataStruct page, long streamPosition)
            {
                LocationOfPage = page.LocationOfPage;
                StreamPosition = streamPosition;
                IsDirtyFlags = page.IsDirtyFlags;
            }
        }

        /// <summary>
        /// contains a list of all the meta pages.
        /// </summary>
        /// <remarks>These items in the list are not in any particular order.
        /// To lookup the correct pages, use <see cref="m_allocatedPagesLookupList"/> </remarks>
        List<PageMetaDataStruct> m_listOfPageMetaData;

        /// <summary>
        /// A bit array to quickly find which <see cref="PageMetaDataStruct"/> entries are being used in
        /// <see cref="m_listOfPageMetaData"/>
        /// </summary>
        BitArray m_isPageMetaDataNotNull;

        /// <summary>
        /// Contains the currently active IO sessions that cannot be cleaned up by a collection.
        /// </summary>
        List<int> m_activeBlockIndexes;

        /// <summary>
        /// Contains all of the pages that are cached for the file stream.
        /// </summary>
        SortedList<int, int> m_allocatedPagesLookupList;

        public LeastRecentlyUsedPageReplacement()
        {
            m_listOfPageMetaData = new List<PageMetaDataStruct>();
            m_isPageMetaDataNotNull = new BitArray(80000, false);
            m_activeBlockIndexes = new List<int>();
            m_allocatedPagesLookupList = new SortedList<int, int>();
        }


        public PageMetaData TryGetPageOrCreateNew(long position, int ioSession, bool isWriting, out bool isEmptyPage)
        {
            int pageIndex = (int)(position >> BufferPool.PageShiftBits);
            int cachePageIndex;
            isEmptyPage = !m_allocatedPagesLookupList.TryGetValue(pageIndex, out cachePageIndex);
            if (isEmptyPage)
            {
                cachePageIndex = GetUnusedPageMetaDataIndex();
                m_allocatedPagesLookupList.Add(pageIndex, cachePageIndex);
                return new PageMetaData(m_listOfPageMetaData[cachePageIndex],position);
            }
            return new PageMetaData(m_listOfPageMetaData[cachePageIndex], position);
        }

        public void ReleaseIoSession(int ioSession)
        {
            m_activeBlockIndexes[ioSession] = -2;
        }

        public int GetFreeIoSession()
        {
            for (int x = 0; x < m_activeBlockIndexes.Count; x++)
            {
                if (m_activeBlockIndexes[x] == -2)
                {
                    m_activeBlockIndexes[x] = -1;
                    return x;
                }
            }
            m_activeBlockIndexes.Add(-1);
            return m_activeBlockIndexes.Count - 1;
        }


        /// <summary>
        /// Finds the next unused cache page index. Marks it as used.
        /// </summary>
        /// <returns></returns>
        int GetUnusedPageMetaDataIndex()
        {
            //Get a free block to read the data to.
            int cachePageIndex = m_isPageMetaDataNotNull.FindClearedBit();
            if (cachePageIndex < 0)
                throw new Exception("Out of room to store blocks");

            IntPtr ptr;

            //Allocate a new one
            PageMetaDataStruct cachePage;
            cachePage.BufferPoolIndex = BufferPool.AllocatePage(out ptr);
            cachePage.LocationOfPage = (byte*)ptr;
            cachePage.IsDirtyFlags = 0;
            cachePage.ReferencedCount = 0;

            m_isPageMetaDataNotNull.SetBit(cachePageIndex);
            if (cachePageIndex < m_listOfPageMetaData.Count)
                m_listOfPageMetaData[cachePageIndex] = cachePage;
            else
                m_listOfPageMetaData.Add(cachePage);
            return cachePageIndex;
        }

        public void DoCollection()
        {
            for (int x = 0; x < m_listOfPageMetaData.Count; x++)
            {
                if (m_isPageMetaDataNotNull.GetBit(x)) //if used.
                {
                    var block = m_listOfPageMetaData[x];
                    block.ReferencedCount >>= 1; //Divide by 2
                    m_listOfPageMetaData[x] = block;
                    if (block.ReferencedCount == 0 && block.IsDirtyFlags == 0) //If used counter is zero, release the block
                    {
                        if (!m_activeBlockIndexes.Contains(x))
                        {
                            BufferPool.ReleasePage(block.BufferPoolIndex);
                            m_isPageMetaDataNotNull.ClearBit(x);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Marks the entire page as written to the disk.
        /// </summary>
        /// <param name="position"></param>
        /// <remarks>These pages are 64KB in size. Calling this function does not write the data to the 
        /// base stream.  It only sets the status of the corresponding page.</remarks>
        public void ClearDirtyBits(long position)
        {

        }

        /// <summary>
        /// Returns all dirty pages in this class.  If pages are subsequentially written to the disk,
        /// be sure to clear the dirty bits via <see cref="ClearDirtyBits"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PageMetaData> GetDirtyPages()
        {
            foreach (var block in m_allocatedPagesLookupList)
            {
                var page = m_listOfPageMetaData[block.Value];
                if (page.IsDirtyFlags != 0)
                    yield return new PageMetaData(m_listOfPageMetaData[block.Value], block.Key * (long)BufferPool.PageSize);
            }
        }

    }
}
