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

namespace openHistorian.Core.Unmanaged
{
    public enum BufferPoolCollectionMode
    {
        Normal,
        Emergency
    }
    /// <summary>
    /// This class allocates and pools unmanaged memory.
    /// </summary>
    public static class BufferPool
    {
        /// <summary>
        /// Used for synchronizing modifications to this class.
        /// </summary>
        static object s_syncRoot;
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
        /// Returns the total amount of ram installed in the computer
        /// </summary>
        public static long SystemTotalPhysicalMemory { get; private set; }
        /// <summary>
        /// Determines if large pages are in use for this buffer pool.
        /// </summary>
        public static bool IsUsingLargePageSizes { get; private set; }
        /// <summary>
        /// Requests that classes using this buffer pool release any unused buffers.
        /// Failing to do so may result in an out of memory exception.
        /// </summary>
        public static event Action<BufferPoolCollectionMode> RequestCollection;

        /// <summary>
        /// Represents a counter for the number of free pages available in this allocation block.
        /// -1 means that this block is not currently allocated.
        /// </summary>
        static short[] s_allocationFreePageCount;
        /// <summary>
        /// Pointers to each allocation.
        /// </summary>
        static Memory[] s_allocationPointer;
        /// <summary>
        /// A bit index that contains free/used blocks.
        /// </summary>
        static BitArray s_pageAllocations;
        /// <summary>
        /// The number of bytes that must be requested at each allocation.
        /// </summary>
        static int s_allocationSize;

        static int s_allocationMask;

        static int s_allocationShiftBits;

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

        public static int AllocationCount { get; private set; }
        public static int PageCount { get; private set; }
        public static int PagesPerAllocation { get; private set; }

        static BufferPool()
        {
            s_syncRoot = new object();
            InitializeDefaultMemorySettings();
        }

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
            MaximumMemoryUsage = Math.Max(128 * 1024 * 1024, availableMemory / 2);//, totalMemory / 4);
            MaximumMemoryUsage = Math.Max(MaximumMemoryUsage, totalMemory / 4);


            //Allocation size at least the page size, but no more than ~1000 allocations over the total system memory;
            if (IsUsingLargePageSizes)
                s_allocationSize = (int)WinApi.GetLargePageMinimum();
            else
                s_allocationSize = PageSize;

            long targetAllocationSize = totalMemory / 1000;
            //if there is more than 1TB of ram, clip the allocation size to 1GB allocations
            targetAllocationSize = Math.Min(targetAllocationSize, 1024 * 1024 * 1024);

            //round down the allocation to a multiple of the minimum page size
            targetAllocationSize = targetAllocationSize - (targetAllocationSize % s_allocationSize);

            //Assign if larger
            s_allocationSize = (int)Math.Max(targetAllocationSize, s_allocationSize);
            //Go to a power of 2.
            s_allocationSize = (int)RoundUpToNearestPowerOfTwo(s_allocationSize);

            MaximumMemoryUsage = GetValidMemoryValue(MaximumMemoryUsage);

            MinimumMemoryUsage = 0;
        }
        
        public static int AllocatePage()
        {
            while (true)
            {
                int index = s_pageAllocations.FindClearedBit();
                
                if (index >=0)
                {
                    s_pageAllocations.SetBit(index);
                    return index;
                }
                if (RequestCollection != null)
                    RequestCollection(BufferPoolCollectionMode.Normal);
            }
        }

        public static void ReleasePage(int pageIndex)
        {
            s_pageAllocations.ClearBit(pageIndex);
        }

        public static IntPtr GetPageAddress(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= s_pageAllocations.Count)
                throw new ArgumentOutOfRangeException("pageIndex");
            int allocationIndex = pageIndex / PagesPerAllocation;
            int blockOffset = pageIndex - allocationIndex * PagesPerAllocation;
            return s_allocationPointer[allocationIndex].Address + blockOffset * PageSize;
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
            size = GetValidMemoryValue(size);
            MaximumMemoryUsage = size;
            if (size < MinimumMemoryUsage)
            {
                MinimumMemoryUsage = size;
            }
            VerifyMemoryWithinBounds();
            return size;
        }

        public static long SetMinimumMemoryUsage(long size)
        {
            if (size < MinimumTestedSupportedMemoryFloor)
            {
                if (size < 0)
                    size = 0;
                size = size - size % s_allocationSize;
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
            VerifyMemoryWithinBounds();
            return size;
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

            size = size - size % s_allocationSize; //Rounds down to the nearest allocation size
            //verifies that rounding down did not cause it to go below the floor.
            if (size < MinimumTestedSupportedMemoryFloor)
                size += s_allocationSize;

            return size;
        }


        static long RoundUpToNearestPowerOfTwo(long value)
        {
            long result = 1;
            while (result <= value)
            {
                result <<= 1;
            }
            return result;
        }

        static void VerifyMemoryWithinBounds()
        {

            //for (int x = 0; x < AllocationCount; x++)
            //{
            //    s_allocationFreePageCount[x] = PagesPerAllocation;
            //    if (UseLargePageSizes)
            //        s_allocationPointer[x] = WinApi.VirtualAlloc((uint)AllocationSize);
            //    else
            //        s_allocationPointer[x] = Marshal.AllocHGlobal(AllocationSize);
            //}
        }

    }
}
