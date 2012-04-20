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
        struct PageMetaData
        {
            public byte* Location;
            public int Index;
            public int ReferencedCount;
            public ushort IsDirtyFlags;
        }
        /// <summary>
        /// contains a list of all the meta pages.
        /// </summary>
        /// <remarks>These items in the list are not in any particular order.
        /// To lookup the correct pages, use <see cref="m_allocatedPagesLookupList"/> </remarks>
        List<PageMetaData> m_listOfPageMetaData;

        /// <summary>
        /// A bit array to quickly find which <see cref="PageMetaData"/> entries are being used in
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
            m_listOfPageMetaData = new List<PageMetaData>();
            m_isPageMetaDataNotNull = new BitArray(80000, false);
            m_activeBlockIndexes = new List<int>();
            m_allocatedPagesLookupList = new SortedList<int, int>();
        }

        /// <summary>
        /// Finds the next available cache page index that is not being used.
        /// If all are being used, a new block is allocated.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public int GetUnusedCachePageBlockIndex(out IntPtr ptr)
        {
            int cachePageIndex;
            //Get a free block to read the data to.
            cachePageIndex = m_isPageMetaDataNotNull.FindClearedBit();
            PageMetaData cachePage;
            if (cachePageIndex < 0)
            {
                //Allocate a new one
                cachePage.Index = BufferPool.AllocatePage(out ptr);
                cachePage.Location = (byte*)ptr;
                cachePage.IsDirtyFlags = 0;
                cachePage.ReferencedCount = 0;
                m_listOfPageMetaData.Add(cachePage);
                cachePageIndex = m_listOfPageMetaData.Count - 1;
            }
            else
            {
                //Use a free one
                m_isPageMetaDataNotNull.SetBit(cachePageIndex);
                cachePage = m_listOfPageMetaData[cachePageIndex];
                ptr = (IntPtr)cachePage.Location;
            }
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
                            BufferPool.ReleasePage(block.Index);
                            m_isPageMetaDataNotNull.ClearBit(x);
                        }
                    }
                }
            }
        }
    }
}
