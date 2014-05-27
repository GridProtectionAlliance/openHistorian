//******************************************************************************************************
//  MemoryPoolBlocks.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  6/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.Collections;

namespace GSF.IO.Unmanaged
{
    //Bug: I cannot use m_isPageAllocated to determine how many pages are allocated. 
    //bug: This is because it will not be valid if a block is freed. The bits are not
    //bug: isPageAllcated, it is more isPageNOTAvailableForAllocation.
    //bug: I'll need aditional information to close the loop.
    /// <summary>
    /// Maintains a list of all of the memory allocations for the buffer pool.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    public partial class MemoryPool
    {
        #region [ Members ]

        /// <summary>
        /// Pointers to each windows API allocation.
        /// </summary>
        private NullableLargeArray<Memory> m_memoryBlocks;

        /// <summary>
        /// A bit array that contains available blocks.
        /// Set means page is available
        /// </summary>
        private BitArray m_isPageAvailable;

        /// <summary>
        /// The number of pages that exist within a windows API allocaiton.
        /// </summary>
        private int m_pagesPerMemoryBlock;

        private int m_pagesPerMemoryBlockShiftBits;
        private int m_pagesPerMemoryBlockMask;

        /// <summary>
        /// The number of pages that have been allocated.
        /// </summary>
        private int m_allocatedPagesCount;

        #endregion

        #region [ Constructors ]

        private void InitializeList()
        {
            m_allocatedPagesCount = 0;
            m_pagesPerMemoryBlock = (MemoryBlockSize / PageSize);
            m_pagesPerMemoryBlockMask = m_pagesPerMemoryBlock - 1;
            m_pagesPerMemoryBlockShiftBits = BitMath.CountBitsSet((uint)m_pagesPerMemoryBlockMask);
            m_isPageAvailable = new BitArray(false);
            m_memoryBlocks = new NullableLargeArray<Memory>();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Requests a new block from the buffer pool.
        /// </summary>
        /// <param name="index">the index identifier of the block</param>
        /// <param name="addressPointer">the address to the start of the block</param>
        /// <exception cref="OutOfMemoryException">Thrown if the list is full</exception>
        private void InternalGetNextPage(out int index, out IntPtr addressPointer)
        {
            index = m_isPageAvailable.FindSetBit();
            if (index < 0)
                throw new OutOfMemoryException("Buffer pool is full");
            m_allocatedPagesCount++;
            m_isPageAvailable.ClearBit(index);
            addressPointer = GetPageAddress(index);
            //if (!Memory.IsEmpty(addressPointer,PageSize))
            //    throw new Exception("Page has been modified since it was last released.");
        }

        /// <summary>
        /// Releases a block back to the pool so it can be re-allocated.
        /// </summary>
        /// <param name="index">the index identifier of the block</param>
        /// <returns>True if the page was released. False if the page was already marked as released.</returns>
        private bool InternalTryReleasePage(int index)
        {
            if (m_isPageAvailable.TrySetBit(index))
            {
                //IntPtr page = GetPageAddress(index);
                //Memory.Clear(page,PageSize);
                m_allocatedPagesCount--;
                return true;
            }
            throw new Exception("Cannot have duplicate calls to release pages.");
            return false;
        }

        /// <summary>
        /// Tries to shrink the buffer pool to the provided size
        /// </summary>
        /// <param name="size">The size of the buffer pool</param>
        /// <returns>The final size of the buffer pool</returns>
        /// <remarks>The buffer pool shrinks to a size less than or equal to <see cref="size"/>.</remarks>
        private long TryShrinkBufferPool(long size)
        {
            if (CurrentCapacity <= size)
                return CurrentCapacity;

            for (int x = 0; x < m_memoryBlocks.Capacity; x++)
            {
                Memory memory;
                if (m_memoryBlocks.TryGetValue(x, out memory))
                {
                    if (m_isPageAvailable.AreAllBitsCleared(x * m_pagesPerMemoryBlock, m_pagesPerMemoryBlock))
                    {
                        memory.Dispose();
                        m_memoryBlocks.SetNull(x);
                        m_isPageAvailable.ClearBits(x * m_pagesPerMemoryBlock, m_pagesPerMemoryBlock);
                        if (CurrentCapacity <= size)
                            return CurrentCapacity;
                    }
                }
            }
            return CurrentCapacity;
        }

        /// <summary>
        /// Returns the pointer to the page with this address.
        /// </summary>
        /// <param name="pageIndex">The index value of the page.</param>
        /// <returns></returns>
        private IntPtr GetPageAddress(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= m_isPageAvailable.Count)
                throw new ArgumentOutOfRangeException("pageIndex");
            int allocationIndex = pageIndex >> m_pagesPerMemoryBlockShiftBits;
            int blockOffset = pageIndex & m_pagesPerMemoryBlockMask;
            Memory block = m_memoryBlocks[allocationIndex];
            if (block.Address == null)
                throw new NullReferenceException("Memory Block is null");
            return block.Address + blockOffset * PageSize;
        }

        private void DisposeList()
        {
            if (!m_disposed)
            {
                try
                {
                    foreach (Memory block in m_memoryBlocks)
                    {
                        block.Dispose();
                    }
                    m_memoryBlocks = null;
                }
                finally
                {
                    m_disposed = true; // Prevent duplicate dispose.
                }
            }
        }

        #endregion
    }
}