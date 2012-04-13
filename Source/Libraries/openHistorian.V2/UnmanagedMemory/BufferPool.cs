//******************************************************************************************************
//  BufferPool.cs - Gbtc
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
//  3/16/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using openHistorian.V2.Collections;

namespace openHistorian.V2.UnmanagedMemory
{
    public enum BufferPoolCollectionMode
    {
        Normal,
        Emergency,
        Critical
    }
    /// <summary>
    /// This class allocates and pools unmanaged memory.
    /// </summary>
    public static class BufferPool
    {
        #region [ Members ]

        /// <summary>
        /// Represents the ceiling for the amount of memory the buffer pool can use (124GB)
        /// </summary>
        public const long MaximumTestedSupportedMemoryCeiling = 124 * 1024 * 1024 * 1024L;
        /// <summary>
        /// Represents the minimum amount of memory that the buffer pool needs to work properly
        /// </summary>
        public const long MinimumTestedSupportedMemoryFloor = 128 * 1024 * 1024;
        /// <summary>
        /// Each page will be exactly this size (64KB)
        /// </summary>
        public const int PageSize = 65536;
        public const int PageMask = 65535;
        public const int PageShiftBits = 16;
        /// <summary>
        /// If after a collection the free space percentage is not at least this much
        /// grow the buffer pool to have this much free.
        /// </summary>
        const float DesiredFreeSpaceAfterCollection = 0.25f;
        /// <summary>
        /// If after a collection the free space percentage is greater than this
        /// then shrink the size of the buffer. 
        /// </summary>
        const float ShrinkIfFreeSpaceAfterCollection = 0.75f;

        /// <summary>
        /// Requests that classes using this buffer pool release any unused buffers.
        /// Failing to do so may result in an out of memory exception.
        /// <remarks>IMPORTANT NOTICE:  All collection must be performed on this thread.
        /// if calling any method of <see cref="BufferPool"/> with a different thread 
        /// this will likely cause a deadlock.  
        /// You must use the calling thread to release all objects.</remarks>
        /// </summary>
        public static event Action<BufferPoolCollectionMode> RequestCollection;

        /// <summary>
        /// Used for synchronizing modifications to this class.
        /// </summary>
        static object s_syncRoot;

        /// <summary>
        /// Pointers to each allocation.
        /// </summary>
        static Memory[] s_memoryBlocks;
        /// <summary>
        /// A bit index that contains free/used blocks.
        /// </summary>
        static BitArray s_pageAllocations;

        /// <summary>
        /// The number of bytes that must be requested at each allocation.
        /// </summary>
        static int s_memoryBlockSize;

        static int s_memoryBlockMask;

        static int s_memoryBlockShiftBits;

        static int s_memoryBlockCount;

        static int s_pagesPerMemoryBlock;
        static int s_pagesPerMemoryBlockShiftBits;
        static int s_pagesPerMemoryBlockMask;

        #endregion

        #region [ Constructors ]

        static BufferPool()
        {
            s_syncRoot = new object();
            InitializeDefaultMemorySettings();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Returns the total amount of ram installed in the computer
        /// </summary>
        public static long SystemTotalPhysicalMemory { get; private set; }

        /// <summary>
        /// Determines if large pages are in use for this buffer pool.
        /// </summary>
        public static bool IsUsingLargePageSizes { get; private set; }

        /// <summary>
        /// The minimum amount of memory that must be allocated to the buffer pool.
        /// call SetMinimumMemoryUsage to set this value;
        /// </summary>
        public static long MinimumMemoryUsage { get; private set; }
        /// <summary>
        /// The maximum amount of memory that may be allocated to the buffer pool.
        /// call SetMaximumMemoryUsage to set this value;
        /// </summary>
        public static long MaximumMemoryUsage { get; private set; }
        /// <summary>
        /// Returns the percentage of the current allocated buffer that is free.
        /// </summary>
        public static float FreeSpacePercentage
        {
            get
            {
                if (s_memoryBlockCount == 0)
                    return 0;
                return s_pageAllocations.ClearCount * (float)PageSize / AllocatedMemory;
            }
        }

        /// <summary>
        /// Returns the number of bytes currently allocated
        /// </summary>
        public static long AllocatedMemory
        {
            get
            {
                return (long)s_memoryBlockCount * s_memoryBlockSize;
            }
        }

        #endregion

        #region [ Methods ]
        /// <summary>
        /// Requests a 64KB page from the unbuffered pool.
        /// If there is not a free one available, method will block
        /// and request a collection of unused pages by raising 
        /// <see cref="RequestCollection"/> event.
        /// </summary>
        /// <param name="addressPointer">outputs a address that can be used
        /// to access this memory address.  You cannot call release with this parameter.
        /// Use the returned index to release pages.</param>
        /// <returns>The index of the page.</returns>
        /// <remarks>The page allocated will not be initialized, 
        /// so assume that the data is garbage.</remarks>
        public static int AllocatePage(out IntPtr addressPointer)
        {
            lock (s_syncRoot)
            {
                while (true)
                {
                    int index = s_pageAllocations.FindClearedBit();
                    if (index >= 0)
                    {
                        s_pageAllocations.SetBit(index);
                        addressPointer = GetPageAddress(index);
                        return index;
                    }
                    RequestMoreSpace();
                }
            }
        }

        /// <summary>
        /// Releases the page back to the buffer pool for reallocation.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <remarks>The page released will not be initialized.
        /// Releasing a page is on the honor system.  
        /// Rereferencing a released page will most certainly cause 
        /// unexpected crashing or data corruption or any other unexplained behavior.
        /// </remarks>
        public static void ReleasePage(int pageIndex)
        {
            lock (s_syncRoot)
            {
                s_pageAllocations.ClearBit(pageIndex);
            }
        }

        /// <summary>
        /// Sets the maximum amount of memory 
        /// that can be allocated by this buffer pool.
        /// </summary>
        /// <param name="size">The size in bytes</param>
        /// <returns>the actual size of the buffer pool</returns>
        /// <remarks>There are some limitations in place 
        /// that will be applied to the number. These limitations include
        /// not permitting size to be larger than 75% of the total ram 
        /// in the system or larger than 4GB of free memory. Which ever is larger.
        /// There is also a 124GB internal limit that can be raised once we
        /// have our hands on a PC that exceeds this threshold and run our test procedures on it.
        /// </remarks>
        public static long SetMaximumMemoryUsage(long size)
        {
            lock (s_syncRoot)
            {
                size = GetValidMemoryValue(size);
                MaximumMemoryUsage = size;
                if (size < MinimumMemoryUsage)
                {
                    MinimumMemoryUsage = size;
                }
                VerifyMaximumMemoryBounds();
                return size;
            }
        }

        /// <summary>
        /// Sets the minimum amount of memory that the buffer pool
        /// must maintain. This value would be useful to set if 
        /// a dedicated amount of memory should be devoted to this app.
        /// Giving this buffer pool more memory will decrease the amount
        /// of garbage collection that this class must do.
        /// </summary>
        /// <param name="size">size in byte</param>
        /// <returns>The actual size, which may be rounded</returns>
        public static long SetMinimumMemoryUsage(long size)
        {
            lock (s_syncRoot)
            {
                if (size < MinimumTestedSupportedMemoryFloor)
                {
                    if (size < 0)
                        size = 0;
                    size = size - size % s_memoryBlockSize;
                }
                else
                {
                    size = GetValidMemoryValue(size);
                    if (size > MaximumMemoryUsage)
                    {
                        MaximumMemoryUsage = size;
                    }
                }
                MinimumMemoryUsage = size;
                VerifyMinimumMemoryBounds();
                return size;
            }
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Determines if large pages should be used.
        /// Assigns an appropriate maximum allocation size
        /// Calculates an allocation size.
        /// Sets a minimum size of zero.
        /// </summary>
        static void InitializeDefaultMemorySettings()
        {
            var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
            long totalMemory = (long)info.TotalPhysicalMemory;
            long availableMemory = (long)info.AvailablePhysicalMemory;
            SystemTotalPhysicalMemory = totalMemory;

            //only use large pages if 64 bit OS detected, 
            //There is at least 4GB of physical memory, 
            //and Windows supports
            IsUsingLargePageSizes = (Environment.Is64BitProcess && totalMemory >= 4 * 1024 * 1024 * 1024L &&
                                     WinApi.CanAllocateLargePage && HelperFunctions.IsPowerOfTwo(WinApi.GetLargePageMinimum()));

            //Maximum size is at least 128MB
            //At least 50% of the free space
            //At least 25% of the total system memory.
            MaximumMemoryUsage = Math.Max(MinimumTestedSupportedMemoryFloor, availableMemory / 2);//, totalMemory / 4);
            MaximumMemoryUsage = Math.Max(MaximumMemoryUsage, totalMemory / 4);


            //Allocation size at least the page size, but no more than ~1000 allocations over the total system memory;
            if (IsUsingLargePageSizes)
                s_memoryBlockSize = (int)WinApi.GetLargePageMinimum();
            else
                s_memoryBlockSize = PageSize;

            long targetMemoryBlockSize = totalMemory / 1000;
            //if there is more than 1TB of ram, clip the allocation size to 1GB allocations
            targetMemoryBlockSize = Math.Min(targetMemoryBlockSize, 1024 * 1024 * 1024);

            //round down the allocation to a multiple of the minimum page size
            targetMemoryBlockSize = targetMemoryBlockSize - (targetMemoryBlockSize % s_memoryBlockSize);

            //Assign if larger
            s_memoryBlockSize = (int)Math.Max(targetMemoryBlockSize, s_memoryBlockSize);
            //Go to a power of 2.
            s_memoryBlockSize = (int)HelperFunctions.RoundUpToNearestPowerOfTwo(s_memoryBlockSize);

            MaximumMemoryUsage = GetValidMemoryValue(MaximumMemoryUsage);

            MinimumMemoryUsage = 0;
            s_memoryBlockCount = 0;
            s_pagesPerMemoryBlock = s_memoryBlockSize / PageSize;
            s_pagesPerMemoryBlockMask = s_pagesPerMemoryBlock - 1;
            s_pagesPerMemoryBlockShiftBits = HelperFunctions.CountBits((uint)s_pagesPerMemoryBlockMask);

            InitialLookupEntities(s_memoryBlockSize, SystemTotalPhysicalMemory);
        }

        /// <summary>
        /// Creates enough room to allocate 100% of system memory.
        /// </summary>
        /// <param name="memoryBlockSize">the size of each memory allocation.</param>
        /// <param name="maximumInstalledMemory">the size of system memory.</param>
        static void InitialLookupEntities(int memoryBlockSize, long maximumInstalledMemory)
        {
            //maximum number of allocations, plus 1 for rounding and good measure.
            int maxAllocationCount = (int)(maximumInstalledMemory / memoryBlockSize) + 1;

            s_memoryBlocks = new Memory[maxAllocationCount];

            int maxAddressablePages = (int)(memoryBlockSize * (long)maxAllocationCount / PageSize);

            s_pageAllocations = new BitArray(maxAddressablePages, true);
        }

        /// <summary>
        /// This procedure will free up unused blocks/allocate more space.
        /// If no more space can be allocated, an out of memory exception will occur.
        /// </summary>
        static void RequestMoreSpace()
        {
            if (RequestCollection != null)
                RequestCollection(BufferPoolCollectionMode.Normal);

            if (FreeSpacePercentage < DesiredFreeSpaceAfterCollection)
            {
                GrowBufferToDesiredMaxPercentage();
            }
            else if (FreeSpacePercentage > ShrinkIfFreeSpaceAfterCollection)
            {
                //ToDo: Figure out how to shrink the buffer pool.
            }

            if (s_pageAllocations.ClearCount == 0)
                RequestCollection(BufferPoolCollectionMode.Emergency);
            if (s_pageAllocations.ClearCount == 0)
                RequestCollection(BufferPoolCollectionMode.Critical);
            if (s_pageAllocations.ClearCount == 0)
            {
                throw new OutOfMemoryException("Buffer pool is out of memory");
            }

        }

        /// <summary>
        /// Grows the buffer region to have the proper desired amount of free memory.
        /// </summary>
        static void GrowBufferToDesiredMaxPercentage()
        {
            while (FreeSpacePercentage < DesiredFreeSpaceAfterCollection)
            {
                //If this goes beyond the desired maximum, exit
                if (AllocatedMemory + s_memoryBlockSize > MaximumMemoryUsage)
                    return;
                AllocateBlock();
            }
        }

        /// <summary>
        /// Allocates a new block and clears the associated bits in the page allocation table.
        /// </summary>
        static void AllocateBlock()
        {
            for (int x = 0; x < s_memoryBlocks.Length; x++)
            {
                if (s_memoryBlocks[x] == null)
                {
                    s_memoryBlocks[x] = Memory.Allocate((uint)s_memoryBlockSize, IsUsingLargePageSizes);
                    s_memoryBlockCount++;
                    int start = x * s_pagesPerMemoryBlock;
                    int stop = start + s_pagesPerMemoryBlock;
                    for (int i = start; i < stop; i++)
                    {
                        s_pageAllocations.ClearBit(i);
                    }
                    return;
                }
            }
            throw new Exception("Could not find memory block location to put block");
        }

        /// <summary>
        /// Returns the pointer to the page with this address.
        /// </summary>
        /// <param name="pageIndex">The index value of the page.</param>
        /// <returns></returns>
        static IntPtr GetPageAddress(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= s_pageAllocations.Count)
                throw new ArgumentOutOfRangeException("pageIndex");
            int allocationIndex = pageIndex >> s_pagesPerMemoryBlockShiftBits;
            int blockOffset = pageIndex & s_pagesPerMemoryBlockMask;
            return s_memoryBlocks[allocationIndex].Address + blockOffset * PageSize;
        }

        /// <summary>
        /// Computes the closest valid memory buffer size from the provided value
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        static long GetValidMemoryValue(long size)
        {
            size = Math.Max(size, MinimumTestedSupportedMemoryFloor);
            size = Math.Min(size, MaximumTestedSupportedMemoryCeiling);
            long physicalUpperLimit = Math.Max(SystemTotalPhysicalMemory / 4 * 3, SystemTotalPhysicalMemory - 4 * 1024 * 1024 * 1024L);
            size = Math.Min(size, physicalUpperLimit);

            size = size - size % s_memoryBlockSize; //Rounds down to the nearest allocation size
            //verifies that rounding down did not cause it to go below the floor.
            if (size < MinimumTestedSupportedMemoryFloor)
                size += s_memoryBlockSize;

            return size;
        }

        /// <summary>
        /// Makes attempts to compact memory to 
        /// bring the total memory usage below
        /// this new maximum bounds.
        /// </summary>
        static void VerifyMaximumMemoryBounds()
        {
            return;
            //todo: actually decrease the amount of memory used by the buffer.
            //while (AllocatedMemory > MaximumMemoryUsage)
            //{

            //}
        }

        /// <summary>
        /// Continues to allocate memory until the minimum threshold is reached.
        /// </summary>
        static void VerifyMinimumMemoryBounds()
        {
            while (AllocatedMemory < MinimumMemoryUsage)
            {
                AllocateBlock();
            }
        }

        #endregion
    }
}
