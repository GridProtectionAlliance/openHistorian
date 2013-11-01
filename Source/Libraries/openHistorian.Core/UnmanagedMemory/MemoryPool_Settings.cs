//******************************************************************************************************
//  MemoryPool_Settings.cs - Gbtc
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
using Microsoft.VisualBasic.Devices;

namespace GSF.UnmanagedMemory
{
    /// <summary>
    /// Maintains the core settings for the buffer pool and the methods for how they are calculated.
    /// </summary>
    public partial class MemoryPool
    {
        #region [ Members ]

        private long m_levelNone;
        private long m_levelLow;
        private long m_levelNormal;
        private long m_levelHigh;
        private long m_levelVeryHigh;

        #endregion

        #region [ Methods ]

        public int GetCollectionBasedOnSize(long size)
        {
            if (size < m_levelNone)
            {
                return 0;
            }
            else if (size < m_levelLow)
            {
                return 1;
            }
            else if (size < m_levelNormal)
            {
                return 2;
            }
            else if (size < m_levelHigh)
            {
                return 3;
            }
            else if (size < m_levelVeryHigh)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }


        /// <summary>
        /// Assigns an appropriate maximum allocation size
        /// Calculates an allocation size.
        /// Sets a minimum size of zero.
        /// </summary>
        private void InitializeSettings()
        {
            ComputerInfo info = new ComputerInfo();

            long totalMemory = (long)info.TotalPhysicalMemory;
            long availableMemory = (long)info.AvailablePhysicalMemory;

            if (!Environment.Is64BitProcess)
            {
                totalMemory = Math.Min(int.MaxValue, totalMemory); //Clip at 2GB
                availableMemory = Math.Min(int.MaxValue - GC.GetTotalMemory(false), availableMemory); //Clip at 2GB
            }

            MemoryBlockSize = CalculateRecommendedMemoryBlockSize(PageSize, totalMemory);
            SystemBufferCeiling = CalculateBufferSizeCeiling(MemoryBlockSize, totalMemory);

            //Maximum size is at least 128MB
            //At least 50% of the free space
            //At least 25% of the total system memory.
            MaximumBufferSize = Math.Max(MinimumTestedSupportedMemoryFloor, availableMemory / 2);
            MaximumBufferSize = Math.Max(MaximumBufferSize, totalMemory / 4);

            CalculateThresholds(MaximumBufferSize, TargetUtilizationLevels.Low);
        }

        private void UpdateCollectionState(long size)
        {
            CollectionModes newState;
            if (size < m_levelNone)
            {
                newState = CollectionModes.None;
            }
            else if (size < m_levelLow)
            {
                newState = CollectionModes.Low;
            }
            else if (size < m_levelNormal)
            {
                newState = CollectionModes.Normal;
            }
            else if (size < m_levelHigh)
            {
                newState = CollectionModes.High;
            }
            else if (size < m_levelVeryHigh)
            {
                newState = CollectionModes.VeryHigh;
            }
            else if (size < MaximumBufferSize)
            {
                newState = CollectionModes.Critical;
            }
            else
            {
                newState = CollectionModes.Full;
            }

            CollectionMode = newState;
        }

        private void CalculateThresholds(long maximumBufferSize, TargetUtilizationLevels levels)
        {
            switch (levels)
            {
                case TargetUtilizationLevels.Low:
                    m_levelNone = (long)(0.1 * maximumBufferSize);
                    m_levelLow = (long)(0.25 * maximumBufferSize);
                    m_levelNormal = (long)(0.50 * maximumBufferSize);
                    m_levelHigh = (long)(0.75 * maximumBufferSize);
                    m_levelVeryHigh = (long)(0.90 * maximumBufferSize);
                    break;
                case TargetUtilizationLevels.Medium:
                    m_levelNone = (long)(0.25 * maximumBufferSize);
                    m_levelLow = (long)(0.50 * maximumBufferSize);
                    m_levelNormal = (long)(0.75 * maximumBufferSize);
                    m_levelHigh = (long)(0.85 * maximumBufferSize);
                    m_levelVeryHigh = (long)(0.95 * maximumBufferSize);
                    break;
                case TargetUtilizationLevels.High:
                    m_levelNone = (long)(0.5 * maximumBufferSize);
                    m_levelLow = (long)(0.75 * maximumBufferSize);
                    m_levelNormal = (long)(0.85 * maximumBufferSize);
                    m_levelHigh = (long)(0.95 * maximumBufferSize);
                    m_levelVeryHigh = (long)(0.97 * maximumBufferSize);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("levels");
            }
        }

        #endregion

        #region [ Static ]

        /// <summary>
        /// Computes the ceiling of the buffer pool
        /// </summary>
        /// <returns></returns>
        private static long CalculateBufferSizeCeiling(int memoryBlockSize, long systemTotalPhysicalMemory)
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
        private static int CalculateRecommendedMemoryBlockSize(int pageSize, long totalSystemMemory)
        {
            long targetMemoryBlockSize = totalSystemMemory / 1000;
            targetMemoryBlockSize = Math.Min(targetMemoryBlockSize, 1024 * 1024 * 1024);
            targetMemoryBlockSize = targetMemoryBlockSize - (targetMemoryBlockSize % pageSize);
            targetMemoryBlockSize = (int)Math.Max(targetMemoryBlockSize, pageSize);
            targetMemoryBlockSize = (int)BitMath.RoundUpToNearestPowerOfTwo((ulong)targetMemoryBlockSize);
            return (int)targetMemoryBlockSize;
        }

        #endregion
    }
}