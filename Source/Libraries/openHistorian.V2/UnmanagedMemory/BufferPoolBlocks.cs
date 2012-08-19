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
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    class BufferPoolBlocks : IDisposable
    {
        #region [ Members ]

        bool m_disposed;

        /// <summary>
        /// Pointers to each windows API allocation.
        /// </summary>
        Memory[] m_memoryBlocks;

        /// <summary>
        /// A bit index that contains free/used blocks.
        /// Set means page is used.
        /// </summary>
        BitArray m_isPageAllocated;

        /// <summary>
        /// The maximum number of pages that can be allocated in this structure
        /// which is based on <see cref="BufferPoolSettings.MaximumBufferCeiling"/>.
        /// </summary>
        int m_maxAddressablePages;

        /// <summary>
        /// The number of pages that have been allocated.
        /// </summary>
        int m_allocatedPagesCount;

        /// <summary>
        /// The number of blocks that have been allocated from Windows API
        /// This figure is kept since some values of m_memoryBlocks[] can be null
        /// </summary>
        int m_allocatedBlocksCount;

        /// <summary>
        /// The number of bytes per page
        /// </summary>
        int m_pageSize;

        /// <summary>
        /// The number of bytes allocated each time memory is allocated from Windows API
        /// </summary>
        int m_memoryBlockSize;

        int m_pagesPerMemoryBlockShiftBits;
        int m_pagesPerMemoryBlockMask;

        /// <summary>
        /// The number of pages that exist within a windows API allocaiton.
        /// </summary>
        int m_pagesPerMemoryBlock;

        #endregion

        #region [ Constructors ]

        public BufferPoolBlocks(BufferPoolSettings settings)
        {
            m_pageSize = settings.PageSize;
            m_memoryBlockSize = settings.MemoryBlockSize;
            m_pagesPerMemoryBlock = (settings.MemoryBlockSize / settings.PageSize);
            m_pagesPerMemoryBlockMask = m_pagesPerMemoryBlock - 1;
            m_pagesPerMemoryBlockShiftBits = HelperFunctions.CountBits((uint)m_pagesPerMemoryBlockMask);

            //maximum number of allocations, plus 1 for rounding and good measure.
            int maxAllocationCount = (int)(settings.MaximumBufferCeiling / settings.MemoryBlockSize) + 1;
            m_memoryBlocks = new Memory[maxAllocationCount];

            m_maxAddressablePages = (int)(settings.MemoryBlockSize * (long)maxAllocationCount / settings.PageSize);

            m_isPageAllocated = new BitArray(m_maxAddressablePages, true);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if more blocks can be allocated from Windows API.
        /// </summary>
        public bool CanGrowBuffer
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
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
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_isPageAllocated.ClearCount == 0;
            }
        }

        /// <summary>
        /// Determines if the buffer pool cannot make another allocation because all available allocations are consumed.
        /// </summary>
        /// <remarks>Equivalent to IsFull && !CanGrowBuffer</remarks>
        public bool IsCompletelyFull
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return IsFull && !CanGrowBuffer;
            }
        }

        /// <summary>
        /// Returns the number of bytes allocated by all of the pages.
        /// This does not include any pages that have been allocated but are not in use.
        /// </summary>
        public long AllocatedPagesBytes
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
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
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_allocatedBlocksCount * (long)m_memoryBlockSize;
            }
        }

        /// <summary>
        /// Returns the number of bytes that are unallocated
        /// </summary>
        public long FreeSpaceBytes
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return BufferPoolSize - AllocatedPagesBytes;
            }
        }

        /// <summary>
        /// Gets the percentage of the buffer pool that is currently allocated.  
        /// If the size of the buffer pool is zero, the returned percentage is
        /// defined to be zero.
        /// </summary>
        /// <remarks>Returns a value between 0 and 1</remarks>
        public float FreeSpace
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (BufferPoolSize == 0)
                    return 0f;
                return 1f - (float)AllocatedPagesBytes / BufferPoolSize;
            }
        }

        /// <summary>
        /// Gets the percentage of the buffer pool that is currently utilized.  
        /// If the size of the buffer pool is zero, the returned percentage is
        /// defined to be zero.
        /// </summary>
        /// <remarks>Returns a value between 0 and 1</remarks>
        public float Utilization
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (BufferPoolSize == 0)
                    return 0f;
                return (float)AllocatedPagesBytes / BufferPoolSize;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Requests a new block from the buffer pool, allocating it if need be.
        /// </summary>
        /// <param name="index">the index identifier of the block</param>
        /// <param name="addressPointer">the address to the start of the block</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool TryAllocatePage(out int index, out IntPtr addressPointer)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (IsFull)
            {
                if (IsCompletelyFull)
                {
                    index = 0;
                    addressPointer = IntPtr.Zero;
                    return false;
                }
                AllocateWinApiBlock();
            }
            m_allocatedPagesCount++;
            index = m_isPageAllocated.FindClearedBit();
            m_isPageAllocated.SetBit(index);
            addressPointer = GetPageAddress(index);
            return true;
        }

        /// <summary>
        /// Releases a block back to the pool so it can be re-allocated.
        /// </summary>
        /// <param name="index">the index identifier of the block</param>
        /// <returns>True if the page was released. False if the page was already marked as released.</returns>
        public bool TryReleasePage(int index)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_isPageAllocated.TryClearBit(index))
            {
                m_allocatedPagesCount--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to shrink the buffer pool to the provided size
        /// </summary>
        /// <param name="size">The size of the buffer pool</param>
        /// <returns>The final size of the buffer pool</returns>
        /// <remarks>The buffer pool shrinks to a size less than or equal to <see cref="size"/>.</remarks>
        public long TryShrinkBufferPool(long size)
        {
            for (int x = 0; x < m_memoryBlocks.Length; x++)
            {
                if (BufferPoolSize <= size)
                    return BufferPoolSize;

                if (m_memoryBlocks[x] != null)
                {
                    if (m_isPageAllocated.AreBitsCleared(x * m_pagesPerMemoryBlock, m_pagesPerMemoryBlock))
                    {
                        m_memoryBlocks[x].Dispose();
                        m_memoryBlocks = null;
                        m_isPageAllocated.SetBits(x * m_pagesPerMemoryBlock, m_pagesPerMemoryBlock);
                        m_allocatedBlocksCount--;
                    }
                }
            }
            return BufferPoolSize;
        }

        /// <summary>
        /// Returns the pointer to the page with this address.
        /// </summary>
        /// <param name="pageIndex">The index value of the page.</param>
        /// <returns></returns>
        IntPtr GetPageAddress(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= m_isPageAllocated.Count)
                throw new ArgumentOutOfRangeException("pageIndex");
            int allocationIndex = pageIndex >> m_pagesPerMemoryBlockShiftBits;
            int blockOffset = pageIndex & m_pagesPerMemoryBlockMask;
            var block = m_memoryBlocks[allocationIndex];
            if (block == null || block.Address == null)
                throw new NullReferenceException("Memory Block is null");
            return block.Address + blockOffset * m_pageSize;
        }


        /// <summary>
        /// Allocates a new block and clears the associated bits in the page allocation table.
        /// </summary>
        public void AllocateWinApiBlock()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            int indexOfNullBlock = Array.IndexOf(m_memoryBlocks, null);
            if (indexOfNullBlock >= 0)
            {
                m_memoryBlocks[indexOfNullBlock] = new Memory(m_memoryBlockSize);
                m_allocatedBlocksCount++;

                m_isPageAllocated.ClearBits(indexOfNullBlock * m_pagesPerMemoryBlock, m_pagesPerMemoryBlock);

                return;
            }
            throw new Exception("Could not find memory block location to put block");
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="BufferPoolBlocks"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    for (int x = 0; x < m_memoryBlocks.Length; x++)
                    {
                        if (m_memoryBlocks[x] != null)
                        {
                            m_memoryBlocks[x].Dispose();
                        }
                    }
                    m_memoryBlocks = null;
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion
    }
}
