//******************************************************************************************************
//  MemoryPool.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
using System.Collections.Generic;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// This class allocates and pools unmanaged memory.
    /// Designed to be internally thread safe.
    /// </summary>
    public partial class MemoryPool : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// Represents the ceiling for the amount of memory the buffer pool can use (124GB)
        /// </summary>
        public const long MaximumTestedSupportedMemoryCeiling = 124 * 1024 * 1024 * 1024L;

        /// <summary>
        /// Represents the minimum amount of memory that the buffer pool needs to work properly (10MB)
        /// </summary>
        public const long MinimumTestedSupportedMemoryFloor = 10 * 1024 * 1024;

        /// <summary>
        /// Contains 7 different levels of garbage collection that <see cref="MemoryPool"/> classes
        /// will need to consider when allocating new space.
        /// </summary>
        internal enum CollectionModes
        {
            None = 0,
            Low = 1,
            Normal = 2,
            High = 3,
            VeryHigh = 4,
            Critical = 5,
            Full = 6
        }

        /// <summary>
        /// Deteremines the desired buffer pool utilization level.
        /// Setting to Low will cause collection cycles to occur more often to keep the 
        /// utilization level to low. 
        /// </summary>
        public enum TargetUtilizationLevels
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        private bool m_isCollecting;

        private bool m_disposed;

        private readonly CollectionEngine m_collectionEngine;

        /// <summary>
        /// Each page will be exactly this size (Based on RAM)
        /// </summary>
        public int PageSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Provides a mask that the user can apply that can 
        /// be used to get the offset position of a page.
        /// </summary>
        public int PageMask
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of bits that must be shifted to calculate an index of a position.
        /// This is not the same as a page index that is returned by the allocate functions.
        /// </summary>
        public int PageShiftBits
        {
            get;
            private set;
        }

        internal CollectionModes CollectionMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Requests that classes using this buffer pool release any unused buffers.
        /// Failing to do so may result in an out of memory exception.
        /// <remarks>IMPORTANT NOTICE:  All collection must be performed on this thread.
        /// if calling any method of <see cref="MemoryPool"/> with a different thread 
        /// this will likely cause a deadlock.  
        /// You must use the calling thread to release all objects.</remarks>
        /// </summary>
        public event EventHandler<CollectionEventArgs> RequestCollection
        {
            add
            {
                m_collectionEngine.AddEvent(value);
            }
            remove
            {
                m_collectionEngine.RemoveEvent(value);
            }
        }

        /// <summary>
        /// Used for synchronizing modifications to this class.
        /// </summary>
        private readonly object m_syncRoot;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MemoryPool"/>.
        /// </summary>
        /// <param name="pageSize">The desired page size. Must be between 4KB and 256KB</param>
        /// <param name="maximumBufferSize">The desired maximum size of the allocation. Note: could be less if there is not enough system memory.</param>
        /// <param name="utilizationLevel">Specifies the desired utilization level of the allocated space.</param>
        public MemoryPool(int pageSize = 64 * 1024, long maximumBufferSize = MaximumTestedSupportedMemoryCeiling, TargetUtilizationLevels utilizationLevel = TargetUtilizationLevels.Low)
        {
            if (pageSize < 4096 || pageSize > 256 * 1024)
                throw new ArgumentOutOfRangeException("pageSize", "Page size must be between 4KB and 256KB and a power of 2");

            if (!BitMath.IsPowerOfTwo((uint)pageSize))
                throw new ArgumentOutOfRangeException("pageSize", "Page size must be between 4KB and 256KB and a power of 2");

            m_syncRoot = new object();
            PageSize = pageSize;
            PageMask = PageSize - 1;
            PageShiftBits = BitMath.CountBitsSet((uint)PageMask);

            InitializeSettings();
            InitializeList();
            SetMaximumBufferSize(maximumBufferSize);
            SetTargetUtilizationLevel(utilizationLevel);

            m_collectionEngine = new CollectionEngine(this);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the current <see cref="TargetUtilizationLevels"/>.
        /// </summary>
        public TargetUtilizationLevels TargetUtilizationLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// The number maximum supported number of bytes that can be allocated based
        /// on the amount of RAM in the system.  This is not user configurable.
        /// </summary>
        public long SystemBufferCeiling
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of bytes per Windows API allocation block
        /// </summary>
        public int MemoryBlockSize
        {
            get;
            private set;
        }

        /// <summary>
        /// The maximum amount of RAM that this buffer pool is configured to support
        /// Attempting to allocate more than this will cause an out of memory exception
        /// </summary>
        public long MaximumBufferSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the number of bytes currently allocated by the buffer pool to other objects
        /// </summary>
        public long AllocatedBytes
        {
            get
            {
                return CurrentAllocatedSize;
            }
        }

        /// <summary>
        /// Returns the number of bytes allocated by all buffer pools.
        /// This does not include any pages that have been allocated but are not in use.
        /// </summary>
        public long CurrentAllocatedSize
        {
            get
            {
                return m_allocatedPagesCount * (long)PageSize;
            }
        }

        /// <summary>
        /// Gets the number of bytes that have been allocated to this buffer pool 
        /// by the OS.
        /// </summary>
        public long CurrentCapacity
        {
            get
            {
                return m_memoryBlocks.CountUsed * (long)MemoryBlockSize;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        private bool IsFull
        {
            get
            {
                return CurrentCapacity == CurrentAllocatedSize;
            }
        }

        private long FreeSpaceBytes
        {
            get
            {
                return CurrentCapacity - CurrentAllocatedSize;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Requests a page from the buffered pool.
        /// If there is not a free one available, method will block
        /// and request a collection of unused pages by raising 
        /// <see cref="RequestCollection"/> event.
        /// </summary>
        /// <param name="index">the index id of the page that was allocated</param>
        /// <param name="addressPointer"> outputs a address that can be used
        /// to access this memory address.  You cannot call release with this parameter.
        /// Use the returned index to release pages.</param>
        /// <remarks>The page allocated will not be initialized, 
        /// so assume that the data is garbage.</remarks>
        public void AllocatePage(out int index, out IntPtr addressPointer)
        {
            lock (m_syncRoot)
            {
                if (m_isCollecting)
                    throw new Exception("Cannot allocate data while a collection is occuring");

                if (CurrentAllocatedSize == CurrentCapacity)
                {
                    m_isCollecting = true;
                    m_collectionEngine.AllocateMoreFreeSpace();
                    m_isCollecting = false;
                    //Grow the allocated pool

                    //long newSize = CurrentAllocatedSize;
                    //GrowBufferToSize(newSize + (long)(0.1 * MaximumBufferSize));
                }
                InternalGetNextPage(out index, out addressPointer);
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
        public void ReleasePage(int pageIndex)
        {
            if (pageIndex >= 0)
            {
                lock (m_syncRoot)
                {
                    if (m_isCollecting)
                        throw new Exception("Cannot release data while a collection is occuring through this method");

                    if (!InternalTryReleasePage(pageIndex))
                        throw new Exception("Duplicate calls to release is not supported");
                }
            }
        }

        public void ReleasePages(IEnumerable<int> pageIndexes)
        {
            lock (m_syncRoot)
            {
                if (m_isCollecting)
                    throw new Exception("Cannot release data while a collection is occuring through this method");
                foreach (int x in pageIndexes)
                {
                    if (!InternalTryReleasePage(x))
                        throw new Exception("Duplicate calls to release is not supported");
                    //if (x >= 0)
                    //    TryReleasePage(x);
                }
            }
        }

        /// <summary>
        /// Changes the allowable buffer size
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public long SetMaximumBufferSize(long value)
        {
            lock (m_syncRoot)
            {
                MaximumBufferSize = Math.Max(Math.Min(SystemBufferCeiling, value), MinimumTestedSupportedMemoryFloor);
                CalculateThresholds(MaximumBufferSize, TargetUtilizationLevel);
                return MaximumBufferSize;
            }
        }

        /// <summary>
        /// Changes the utilization level
        /// </summary>
        /// <param name="utilizationLevel"></param>
        /// <returns></returns>
        public void SetTargetUtilizationLevel(TargetUtilizationLevels utilizationLevel)
        {
            lock (m_syncRoot)
            {
                TargetUtilizationLevel = utilizationLevel;
                CalculateThresholds(MaximumBufferSize, TargetUtilizationLevel);
            }
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Grows the buffer pool to have the desired size
        /// </summary>
        private void GrowBufferToSize(long size)
        {
            size = Math.Min(size, MaximumBufferSize);
            while (CurrentCapacity < size)
            {
                int pageIndex = m_memoryBlocks.AddValue(new Memory(MemoryBlockSize));
                m_isPageAvailable.EnsureCapacity((pageIndex + 1) * m_pagesPerMemoryBlock);
                m_isPageAvailable.SetBits(pageIndex * m_pagesPerMemoryBlock, m_pagesPerMemoryBlock);
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="MemoryPool"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    DisposeList();
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