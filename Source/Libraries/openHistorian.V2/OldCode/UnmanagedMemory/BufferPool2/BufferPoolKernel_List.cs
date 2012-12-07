////******************************************************************************************************
////  BufferPoolKernel.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
////  10/5/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using openHistorian.Collections;

//namespace openHistorian.UnmanagedMemory
//{
    
//    public partial class BufferPoolKernel
//    {
//        #region [ Members ]

//        /// <summary>
//        /// Pointers to each windows API allocation.
//        /// </summary>
//        NullableLargeArray<Memory> m_memoryBlocks;

//        /// <summary>
//        /// A bit index that contains free/used blocks.
//        /// Set means page is used.
//        /// </summary>
//        BitArray m_isPageAllocated;

//        /// <summary>
//        /// The number of pages that exist within a windows API allocaiton.
//        /// </summary>
//        int m_pagesPerMemoryBlock;
//        int m_pagesPerMemoryBlockShiftBits;
//        int m_pagesPerMemoryBlockMask;

//        #endregion

//        #region [ Methods ]

//        void InitializeList()
//        {
//            m_pagesPerMemoryBlock = (MemoryBlockSize / PageSize);
//            m_pagesPerMemoryBlockMask = m_pagesPerMemoryBlock - 1;
//            m_pagesPerMemoryBlockShiftBits = BitMath.CountBitsSet((uint)m_pagesPerMemoryBlockMask);
//            m_isPageAllocated = new BitArray(false);
//            m_memoryBlocks = new NullableLargeArray<Memory>();
//        }

//        /// <summary>
//        /// Requests a new block from the buffer pool.
//        /// </summary>
//        /// <param name="index">the index identifier of the block</param>
//        /// <param name="addressPointer">the address to the start of the block</param>
//        /// <exception cref="OutOfMemoryException">Thrown if the list is full</exception>
//        void GetNextPage(out int index, out IntPtr addressPointer)
//        {
//            index = m_isPageAllocated.FindClearedBit();
//            if (index<0)
//                throw new OutOfMemoryException("Buffer pool is full");

//            m_isPageAllocated.SetBit(index);
//            addressPointer = GetPageAddress(index);
//        }

//        /// <summary>
//        /// Releases a block back to the pool so it can be re-allocated.
//        /// </summary>
//        /// <param name="index">the index identifier of the block</param>
//        /// <returns>True if the page was released. False if the page was already marked as released.</returns>
//        bool TryReleasePage(int index)
//        {
//            return m_isPageAllocated.TryClearBit(index);
//        }

//        /// <summary>
//        /// Tries to shrink the buffer pool to the provided size
//        /// </summary>
//        /// <param name="size">The size of the buffer pool</param>
//        /// <returns>The final size of the buffer pool</returns>
//        /// <remarks>The buffer pool shrinks to a size less than or equal to <see cref="size"/>.</remarks>
//        long TryShrinkBufferPool(long size)
//        {
//            if (CurrentCapacity <= size)
//                return CurrentCapacity;

//            for (int x = 0; x < m_memoryBlocks.Capacity; x++)
//            {
//                Memory memory;
//                if (m_memoryBlocks.TryGetValue(x, out memory))
//                {
//                    if (m_isPageAllocated.AreBitsCleared(x * m_pagesPerMemoryBlock, m_pagesPerMemoryBlock))
//                    {
//                        memory.Dispose();
//                        m_memoryBlocks.SetNull(x);
//                        m_isPageAllocated.SetBits(x * m_pagesPerMemoryBlock, m_pagesPerMemoryBlock);
//                        if (CurrentCapacity <= size)
//                            return CurrentCapacity;
//                    }
//                }
//            }
//            return CurrentCapacity;
//        }

//        /// <summary>
//        /// Returns the pointer to the page with this address.
//        /// </summary>
//        /// <param name="pageIndex">The index value of the page.</param>
//        /// <returns></returns>
//        IntPtr GetPageAddress(int pageIndex)
//        {
//            if (pageIndex < 0 || pageIndex >= m_isPageAllocated.Count)
//                throw new ArgumentOutOfRangeException("pageIndex");
//            int allocationIndex = pageIndex >> m_pagesPerMemoryBlockShiftBits;
//            int blockOffset = pageIndex & m_pagesPerMemoryBlockMask;
//            var block = m_memoryBlocks[allocationIndex];
//            if (block.Address == null)
//                throw new NullReferenceException("Memory Block is null");
//            return block.Address + blockOffset * PageSize;
//        }

//        #endregion
//    }
//}
