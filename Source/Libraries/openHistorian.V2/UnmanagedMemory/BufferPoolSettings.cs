//******************************************************************************************************
//  BufferPoolSettings.cs - Gbtc
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

namespace openHistorian.V2.UnmanagedMemory
{
    /// <summary>
    /// Maintains the core settings for the buffer pool and the methods for how they are calculated.
    /// </summary>
    public class BufferPoolSettings
    {
        /// <summary>
        /// Represents the ceiling for the amount of memory the buffer pool can use (124GB)
        /// </summary>
        public const long MaximumTestedSupportedMemoryCeiling = 124 * 1024 * 1024 * 1024L;
        /// <summary>
        /// Represents the minimum amount of memory that the buffer pool needs to work properly
        /// </summary>
        public const long MinimumTestedSupportedMemoryFloor = 128 * 1024 * 1024;

        /// <summary>
        /// The number of bytes in the smallest buffer pool allocation.
        /// </summary>
        int m_pageSize;

        /// <summary>
        /// The number maximum supported number of bytes that can be allocated based
        /// on the amount of RAM in the system.  This is not user configurable.
        /// </summary>
        long m_maximumBufferCeiling;

        /// <summary>
        /// The maximum amount of RAM that this buffer pool is configured to support
        /// Attempting to allocate more than this will cause an out of memory exception
        /// </summary>
        long m_maximumBufferSize;

        /// <summary>
        /// The target amount of memory to allocate before issuing garbage collection cycles.
        /// </summary>
        long m_targetBufferSize;

        /// <summary>
        /// The number of bytes per Windows API allocation block
        /// </summary>
        int m_memoryBlockSize;


        /// <summary>
        /// The number of bytes per page.
        /// Must be a power of 2. Greater than 4KB and less than 256KB
        /// </summary>
        public int PageSize
        {
            get
            {
                return m_pageSize;
            }
        }

        /// <summary>
        /// Defines the maximum amount of memory before excessive garbage collection will occur.
        /// </summary>
        public long MaximumBufferSize
        {
            get
            {
                return m_maximumBufferSize;
            }
            set
            {

            }
        }

        /// <summary>
        /// Defines the target amount of memory. No garbage collection will occur until this threshold has been reached.
        /// </summary>
        public long TargetBufferSize
        {
            get
            {
                return m_targetBufferSize;
            }
            set
            {

            }
        }

        /// <summary>
        /// The number maximum supported number of bytes that can be allocated based
        /// on the amount of RAM in the system.  This is not user configurable.
        /// </summary>
        public long MaximumBufferCeiling
        {
            get
            {
                return m_maximumBufferCeiling;
            }
        }

        /// <summary>
        /// The number of bytes per Windows API allocation block
        /// </summary>
        public int MemoryBlockSize
        {
            get
            {
                return m_memoryBlockSize;
            }
        }

        public BufferPoolSettings(int pageSize)
        {
            Initialize(pageSize);
        }

        /// <summary>
        /// Assigns an appropriate maximum allocation size
        /// Calculates an allocation size.
        /// Sets a minimum size of zero.
        /// </summary>
        void Initialize(int pageSize)
        {
            m_pageSize = pageSize;

            var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
            long totalMemory = (long)info.TotalPhysicalMemory;
            long availableMemory = (long)info.AvailablePhysicalMemory;

            m_memoryBlockSize = CalculateRecommendedMemoryBlockSize(pageSize, totalMemory);
            m_maximumBufferCeiling = CalculateBufferSizeCeiling(m_memoryBlockSize, totalMemory);

            //Maximum size is at least 128MB
            //At least 50% of the free space
            //At least 25% of the total system memory.
            m_maximumBufferSize = Math.Max(MinimumTestedSupportedMemoryFloor, availableMemory / 2);
            m_maximumBufferSize = Math.Max(m_maximumBufferSize, totalMemory / 4);
            m_targetBufferSize = 0;
        }


        /// <summary>
        /// Computes the ceiling of the buffer pool
        /// </summary>
        /// <returns></returns>
        static long CalculateBufferSizeCeiling(int memoryBlockSize, long systemTotalPhysicalMemory)
        {
            long size = MaximumTestedSupportedMemoryCeiling;

            //Physical upper limit is
            //the greater of 
            //  75% of memory
            //or
            //  all but 4GB of RAM
            long physicalUpperLimit = Math.Max(systemTotalPhysicalMemory / 4 * 3, systemTotalPhysicalMemory - 4 * 1024 * 1024 * 1024L);

            size = Math.Min(size, physicalUpperLimit);

            size = size - size % memoryBlockSize; //Rounds down to the nearest allocation size

            return size;
        }

        /// <summary>
        /// Calculates the desired allocation block size to request from the OS.
        /// </summary>
        /// <param name="pageSize">the size of each page</param>
        /// <param name="totalSystemMemory">the total amount of system memory</param>
        /// <returns>The recommended block size</returns>
        /// <remarks>
        /// The recommended block size is the <see cref="totalSystemMemory"/> divided by 1000 
        /// but must be a multiple of the system allocation size and the page size and cannot be larger than 1GB</remarks>
        static int CalculateRecommendedMemoryBlockSize(int pageSize, long totalSystemMemory)
        {
            long targetMemoryBlockSize = totalSystemMemory / 1000;
            targetMemoryBlockSize = Math.Min(targetMemoryBlockSize, 1024 * 1024 * 1024);
            targetMemoryBlockSize = targetMemoryBlockSize - (targetMemoryBlockSize % pageSize);
            targetMemoryBlockSize = (int)Math.Max(targetMemoryBlockSize, pageSize);
            targetMemoryBlockSize = (int)HelperFunctions.RoundUpToNearestPowerOfTwo(targetMemoryBlockSize);
            return (int)targetMemoryBlockSize;
        }
    }
}
