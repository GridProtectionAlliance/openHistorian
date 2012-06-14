//******************************************************************************************************
//  BufferPoolBlocks.cs - Gbtc
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
//  6/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using openHistorian.V2.Collections;

namespace openHistorian.V2.UnmanagedMemory
{
    /// <summary>
    /// Maintains a list of all of the memory blocks for the buffer pool
    /// and their allocation status.
    /// This class will also handle the Windows API allocations.
    /// </summary>
    class BufferPoolBlocks : IDisposable
    {

        bool m_disposed;

        /// <summary>
        /// Pointers to each allocation.
        /// </summary>
        Memory[] m_memoryBlocks;

        /// <summary>
        /// A bit index that contains free/used blocks.
        /// Set means page is used.
        /// </summary>
        BitArray m_pageAllocations;

        int m_maxAddressablePages;

        /// <summary>
        /// The number of blocks that have been allocated.
        /// </summary>
        int m_allocatedPagesCount;

        int m_allocatedBlocksCount;

        int m_pageSize;
        int m_memoryBlockSize;

        int m_pagesPerMemoryBlockShiftBits;
        int m_pagesPerMemoryBlockMask;
        int m_pagesPerMemoryBlock;

        public BufferPoolBlocks(BufferPoolSettings settings)
        {
            m_pageSize = settings.PageSize;
            m_memoryBlockSize = settings.MemoryBlockSize;
            m_pagesPerMemoryBlock = (int)(settings.MemoryBlockSize / settings.PageSize);
            m_pagesPerMemoryBlockMask = m_pagesPerMemoryBlock - 1;
            m_pagesPerMemoryBlockShiftBits = HelperFunctions.CountBits((uint)m_pagesPerMemoryBlockMask);

            //maximum number of allocations, plus 1 for rounding and good measure.
            int maxAllocationCount = (int)(settings.MaximumBufferCeiling / settings.MemoryBlockSize) + 1;
            m_memoryBlocks = new Memory[maxAllocationCount];

            m_maxAddressablePages = (int)(settings.MemoryBlockSize * (long)maxAllocationCount / settings.PageSize);

            m_pageAllocations = new BitArray(m_maxAddressablePages, true);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="BufferPoolBlocks"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~BufferPoolBlocks()
        {
            Dispose(false);
        }

        /// <summary>
        /// Determines if the class can allocate more blocks from Windows API.
        /// </summary>
        public bool CanAllocateMore
        {
            get
            {
                return m_allocatedBlocksCount < m_memoryBlocks.Length;
            }
        }

        /// <summary>
        /// Determines if the buffer pool is full 
        /// and another page will require additional allocations.
        /// </summary>
        public bool IsFull
        {
            get
            {
                return m_pageAllocations.ClearCount == 0;
            }
        }

        /// <summary>
        /// Determines if the buffer pool cannot make another allocation because all available allocations are consumed.
        /// </summary>
        public bool IsCompletelyFull
        {
            get
            {
                return (m_allocatedPagesCount == m_maxAddressablePages);
            }
        }

        /// <summary>
        /// Returns the number of bytes allocated by all of the pages.
        /// This does not include any pages that have been allocated but are not in use.
        /// </summary>
        public long AllocatedBytes
        {
            get
            {
                return m_allocatedPagesCount * (long)m_pageSize;
            }
        }

        /// <summary>
        /// Gets the number of bytes that have been allocated to this buffer pool 
        /// by the OS.
        /// </summary>
        public long BufferPoolSize
        {
            get
            {
                return m_allocatedBlocksCount * (long)m_memoryBlockSize;
            }
        }

        /// <summary>
        /// Gets the percentage of the buffer pool that is currently allocated.  
        /// If the size of the buffer pool is zero, the returned percentage is
        /// defined to be zero.
        /// </summary>
        public float FreeSpace
        {
            get
            {
                if (BufferPoolSize == 0)
                    return 0f;
                return 1f - (float)AllocatedBytes / BufferPoolSize;
            }
        }


        /// <summary>
        /// Requests a new block from the buffer pool, allocating it if need be.
        /// </summary>
        /// <param name="index">the index identifier of the block</param>
        /// <param name="addressPointer">the address to the start of the block</param>
        public void AllocatePage(out int index, out IntPtr addressPointer)
        {
            if (IsFull)
            {
                if (IsCompletelyFull)
                    throw new Exception("Blocks are all used");
                AllocateWinApiBlock();
            }
            m_allocatedPagesCount++;
            index = m_pageAllocations.FindClearedBit();
            m_pageAllocations.SetBit(index);
            addressPointer = GetPageAddress(index);
        }

        /// <summary>
        /// Releases a block back to the pool so it can be re-allocated.
        /// </summary>
        /// <param name="index">the index identifier of the block</param>
        public void ReleasePage(int index)
        {
            m_allocatedPagesCount--;
            m_pageAllocations.ClearBit(index);
        }

        /// <summary>
        /// Returns the pointer to the page with this address.
        /// </summary>
        /// <param name="pageIndex">The index value of the page.</param>
        /// <returns></returns>
        IntPtr GetPageAddress(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= m_pageAllocations.Count)
                throw new ArgumentOutOfRangeException("pageIndex");
            int allocationIndex = pageIndex >> m_pagesPerMemoryBlockShiftBits;
            int blockOffset = pageIndex & m_pagesPerMemoryBlockMask;
            return m_memoryBlocks[allocationIndex].Address + blockOffset * m_pageSize;
        }


        /// <summary>
        /// Allocates a new block and clears the associated bits in the page allocation table.
        /// </summary>
        public void AllocateWinApiBlock()
        {
            for (int x = 0; x < m_memoryBlocks.Length; x++)
            {
                if (m_memoryBlocks[x] == null)
                {
                    m_memoryBlocks[x] = Memory.Allocate(m_memoryBlockSize);
                    m_allocatedBlocksCount++;
                    int start = x * m_pagesPerMemoryBlock;
                    int stop = start + m_pagesPerMemoryBlock;
                    for (int i = start; i < stop; i++)
                    {
                        m_pageAllocations.ClearBit(i);
                    }
                    return;
                }
            }
            throw new Exception("Could not find memory block location to put block");
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="BufferPoolBlocks"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BufferPoolBlocks"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        for (int x = 0; x < m_memoryBlocks.Length; x++)
                        {
                            if (m_memoryBlocks[x] != null)
                            {
                                m_memoryBlocks[x].Dispose();
                            }
                        }
                        m_memoryBlocks = null;
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }
    }
}
