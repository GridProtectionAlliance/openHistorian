//******************************************************************************************************
//  MemoryPool.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
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
using System.Diagnostics;
using System.Text;
using System.Threading;
using GSF.Diagnostics;
using GSF.Threading;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// Deteremines the desired buffer pool utilization level.
    /// Setting to Low will cause collection cycles to occur more often to keep the 
    /// utilization level to low. 
    /// </summary>
    public enum TargetUtilizationLevels
    {
        /// <summary>
        /// Collections won't occur until over 25% of the memory is consumed.
        /// </summary>
        Low = 0,
        /// <summary>
        /// Collections won't occur until over 50% of the memory is consumed.
        /// </summary>
        Medium = 1,
        /// <summary>
        /// Collections won't occur until over 75% of the memory is consumed.
        /// </summary>
        High = 2
    }

    /// <summary>
    /// This class allocates and pools unmanaged memory.
    /// Designed to be internally thread safe.
    /// </summary>
    /// <remarks>
    /// Be careful how this class is referenced. Deadlocks can occur
    /// when registering to event <see cref="RequestCollection"/> and
    /// when calling <see cref="AllocatePage"/>. See comments for these methods
    /// for considerations.
    /// </remarks>
    public class MemoryPool
        : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(MemoryPool), MessageClass.Component);

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
        /// Delegates are placed in a List because
        /// in a later version, some sort of concurrent garbage collection may be implemented
        /// which means more control will need to be with the Event
        /// </summary>
        private readonly ThreadSafeList<WeakEventHandler<CollectionEventArgs>> m_requestCollectionEvent;
        private long m_levelNone;
        private long m_levelLow;
        private long m_levelNormal;
        private long m_levelHigh;
        private long m_levelVeryHigh;
        private bool m_disposed;

        private volatile int m_releasePageVersion;

        /// <summary>
        /// Used for synchronizing modifications to this class.
        /// </summary>
        private readonly object m_syncRoot;

        /// <summary>
        /// All allocates are synchronized seperately since an allocation can request a collection. 
        /// This will create a queuing nature of the allocations.
        /// </summary>
        private readonly object m_syncAllocate;

        private readonly MemoryPoolPageList m_pageList;

        /// <summary>
        /// Gets the current <see cref="TargetUtilizationLevels"/>.
        /// </summary>
        public TargetUtilizationLevels TargetUtilizationLevel { get; private set; }

        /// <summary>
        /// Each page will be exactly this size (Based on RAM)
        /// </summary>
        public readonly int PageSize;

        /// <summary>
        /// Provides a mask that the user can apply that can 
        /// be used to get the offset position of a page.
        /// </summary>
        public readonly int PageMask;

        /// <summary>
        /// Gets the number of bits that must be shifted to calculate an index of a position.
        /// This is not the same as a page index that is returned by the allocate functions.
        /// </summary>
        public readonly int PageShiftBits;

        /// <summary>
        /// Requests that classes using this <see cref="MemoryPool"/> release any unused buffers.
        /// Failing to do so may result in an <see cref="OutOfMemoryException"/> to occur.
        /// <remarks>IMPORTANT NOTICE: No not call <see cref="AllocatePage"/> via the thread
        /// that raises this event. Also, be careful about entering a lock via this thread
        /// because a potential deadlock might occur. 
        /// 
        /// Also, Do not remove a handler from within a lock context as the remove
        /// blocks until all events have been called. A potential for another deadlock.</remarks>
        /// </summary>
        public event EventHandler<CollectionEventArgs> RequestCollection
        {
            add
            {
                m_requestCollectionEvent.Add(new WeakEventHandler<CollectionEventArgs>(value));
                RemoveDeadEvents();
            }
            remove
            {
                m_requestCollectionEvent.RemoveAndWait(new WeakEventHandler<CollectionEventArgs>(value));
                RemoveDeadEvents();
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MemoryPool"/>.
        /// </summary>
        /// <param name="pageSize">The desired page size. Must be between 4KB and 256KB</param>
        /// <param name="maximumBufferSize">The desired maximum size of the allocation. Note: could be less if there is not enough system memory.</param>
        /// <param name="utilizationLevel">Specifies the desired utilization level of the allocated space.</param>
        public MemoryPool(int pageSize = 64 * 1024, long maximumBufferSize = -1, TargetUtilizationLevels utilizationLevel = TargetUtilizationLevels.Low)
        {
            if (pageSize < 4096 || pageSize > 256 * 1024)
                throw new ArgumentOutOfRangeException("pageSize", "Page size must be between 4KB and 256KB and a power of 2");

            if (!BitMath.IsPowerOfTwo((uint)pageSize))
                throw new ArgumentOutOfRangeException("pageSize", "Page size must be between 4KB and 256KB and a power of 2");

            m_syncRoot = new object();
            m_syncAllocate = new object();
            PageSize = pageSize;
            PageMask = PageSize - 1;
            PageShiftBits = BitMath.CountBitsSet((uint)PageMask);

            m_pageList = new MemoryPoolPageList(PageSize, maximumBufferSize);
            m_requestCollectionEvent = new ThreadSafeList<WeakEventHandler<CollectionEventArgs>>();
            SetTargetUtilizationLevel(utilizationLevel);
        }

#if DEBUG
        ~MemoryPool()
        {
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
        }
#endif

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Returns the number of bytes currently allocated by the buffer pool to other objects
        /// </summary>
        public long AllocatedBytes => CurrentAllocatedSize;

        /// <summary>
        /// The maximum amount of RAM that this memory pool is configured to support
        /// Attempting to allocate more than this will cause an out of memory exception
        /// </summary>
        public long MaximumPoolSize => m_pageList.MaximumPoolSize;

        /// <summary>
        /// Returns the number of bytes allocated by all buffer pools.
        /// This does not include any pages that have been allocated but are not in use.
        /// </summary>
        public long CurrentAllocatedSize => m_pageList.CurrentAllocatedSize;

        /// <summary>
        /// Gets the number of bytes that have been allocated to this buffer pool 
        /// by the OS.
        /// </summary>
        public long CurrentCapacity => m_pageList.CurrentCapacity;

        /// <summary>
        /// Gets if this pool has been disposed.
        /// </summary>
        public bool IsDisposed => m_disposed;

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
        /// <remarks>
        /// IMPORTANT NOTICE: Be careful when calling this method as the calling thread
        /// will block if no memory is available to have a background collection to occur.
        /// 
        /// There is a possiblity for a deadlock if calling this method from within a lock.
        /// 
        /// The page allocated will not be initialized, 
        /// so assume that the data is garbage.</remarks>
        public void AllocatePage(out int index, out IntPtr addressPointer)
        {
            if (m_pageList.TryGetNextPage(out index, out addressPointer))
                return;

            lock (m_syncAllocate)
            {
                //m_releasePageVersion is approximately the number of times that a release page function has been called.
                //                     due to race conditions, the number may not be exact, but it will have at least changed.

                while (true)
                {
                    int releasePageVersion = m_releasePageVersion;
                    if (m_pageList.TryGetNextPage(out index, out addressPointer))
                        return;

                    RequestMoreFreeBlocks();
                    if (releasePageVersion == m_releasePageVersion)
                    {
                        Log.Publish(MessageLevel.Critical, MessageFlags.PerformanceIssue, "Out Of Memory", string.Format("Memory pool has run out of memory: Current Usage: {0}MB", CurrentCapacity / 1024 / 1024));
                        throw new OutOfMemoryException("Memory pool is full");
                    }

                    //Due to a race condition, it is possible that someone else get the freed block
                    //and we must request freeing again.
                }
            }
        }

        /// <summary>
        /// Releases the page back to the buffer pool for reallocation.
        /// </summary>
        /// <param name="pageIndex">A value of zero or less will return silently</param>
        /// <remarks>The page released will not be initialized.
        /// Releasing a page is on the honor system.  
        /// Rereferencing a released page will most certainly cause 
        /// unexpected crashing or data corruption or any other unexplained behavior.
        /// </remarks>
        public void ReleasePage(int pageIndex)
        {
            m_pageList.ReleasePage(pageIndex);
            m_releasePageVersion++;
        }

        /// <summary>
        /// Releases all of the supplied pages
        /// </summary>
        /// <param name="pageIndexes">A collection of pages.</param>
        public void ReleasePages(IEnumerable<int> pageIndexes)
        {
            foreach (int x in pageIndexes)
            {
                m_pageList.ReleasePage(x);
            }
            m_releasePageVersion++;
        }

        /// <summary>
        /// Changes the allowable buffer size
        /// </summary>
        /// <param name="value">the number of bytes to set.</param>
        /// <returns></returns>
        public long SetMaximumBufferSize(long value)
        {
            lock (m_syncRoot)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                long rv = m_pageList.SetMaximumPoolSize(value);
                CalculateThresholds(rv, TargetUtilizationLevel);

                Log.Publish(MessageLevel.Info, MessageFlags.PerformanceIssue, "Pool Size Changed", string.Format("Memory pool maximum set to: {0}MB", rv >> 20));

                return rv;
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
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                TargetUtilizationLevel = utilizationLevel;
                CalculateThresholds(MaximumPoolSize, utilizationLevel);
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="MemoryPool"/> object.
        /// </summary>
        public void Dispose()
        {
            lock (m_syncRoot)
            {
                if (!m_disposed)
                {
                    try
                    {
                        m_pageList.Dispose();
                    }
                    finally
                    {
                        m_disposed = true; // Prevent duplicate dispose.
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether to allocate more memory or to do a collection cycle on the existing pool.
        /// </summary>
        private void RequestMoreFreeBlocks()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Collection Cycle Started");
            bool lockTaken;

            Monitor.Enter(m_syncRoot); lockTaken = true;
            try
            {
                long size = CurrentCapacity;
                int collectionLevel = GetCollectionLevelBasedOnSize(size);
                long stopShrinkingLimit = CalculateStopShrinkingLimit(size);

                RemoveDeadEvents();

                sb.Append("Level: " + GetCollectionLevelString(collectionLevel));
                sb.AppendFormat(" Desired Size: {0}/{1}MB", stopShrinkingLimit >> 20, CurrentCapacity >> 20);
                sb.AppendLine();

                for (int x = 0; x < collectionLevel; x++)
                {
                    if (CurrentAllocatedSize < stopShrinkingLimit)
                        break;
                    CollectionEventArgs eventArgs = new CollectionEventArgs(ReleasePage, MemoryPoolCollectionMode.Normal, 0);

                    Monitor.Exit(m_syncRoot); lockTaken = false;

                    foreach (WeakEventHandler<CollectionEventArgs> c in m_requestCollectionEvent)
                    {
                        c.TryInvoke(this, eventArgs);
                    }

                    Monitor.Enter(m_syncRoot); lockTaken = true;

                    sb.AppendFormat("Pass {0} Usage: {1}/{2}MB", x + 1, CurrentAllocatedSize >> 20, CurrentCapacity >> 20);
                    sb.AppendLine();
                }

                long currentSize = CurrentAllocatedSize;
                long sizeBefore = CurrentCapacity;
                if (m_pageList.GrowMemoryPool(currentSize + (long)(0.1 * MaximumPoolSize)))
                {
                    long sizeAfter = CurrentCapacity;
                    m_releasePageVersion++;

                    sb.AppendFormat("Grew buffer pool {0}MB -> {1}MB", sizeBefore >> 20, sizeAfter >> 20);
                    sb.AppendLine();
                }

                if (m_pageList.FreeSpaceBytes < 0.05 * MaximumPoolSize)
                {
                    int pagesToBeReleased = (int)((0.05 * MaximumPoolSize - m_pageList.FreeSpaceBytes) / PageSize);

                    sb.AppendFormat("* Emergency Collection Occuring. Attempting to release {0} pages.", pagesToBeReleased);
                    sb.AppendLine();

                    Log.Publish(MessageLevel.Warning, MessageFlags.PerformanceIssue, "Pool Emergency", string.Format("Memory pool is reaching an Emergency level. Desiring Pages To Release: {0}", pagesToBeReleased));

                    CollectionEventArgs eventArgs = new CollectionEventArgs(ReleasePage, MemoryPoolCollectionMode.Emergency, pagesToBeReleased);

                    Monitor.Exit(m_syncRoot); lockTaken = false;

                    foreach (WeakEventHandler<CollectionEventArgs> c in m_requestCollectionEvent)
                    {
                        if (eventArgs.DesiredPageReleaseCount == 0)
                            break;
                        c.TryInvoke(this, eventArgs);
                    }

                    Monitor.Enter(m_syncRoot); lockTaken = true;

                    if (eventArgs.DesiredPageReleaseCount > 0)
                    {
                        sb.AppendFormat("** Critical Collection Occuring. Attempting to release {0} pages.", pagesToBeReleased);
                        sb.AppendLine();

                        Log.Publish(MessageLevel.Warning, MessageFlags.PerformanceIssue, "Pool Critical", string.Format("Memory pool is reaching an Critical level. Desiring Pages To Release: {0}", eventArgs.DesiredPageReleaseCount));

                        eventArgs = new CollectionEventArgs(ReleasePage, MemoryPoolCollectionMode.Critical, eventArgs.DesiredPageReleaseCount);

                        Monitor.Exit(m_syncRoot); lockTaken = false;

                        foreach (WeakEventHandler<CollectionEventArgs> c in m_requestCollectionEvent)
                        {
                            if (eventArgs.DesiredPageReleaseCount == 0)
                                break;
                            c.TryInvoke(this, eventArgs);
                        }

                        Monitor.Enter(m_syncRoot); lockTaken = true;
                    }
                }

                sw.Stop();
                sb.AppendFormat("Elapsed Time: {0}ms", sw.Elapsed.TotalMilliseconds.ToString("0.0"));
                Log.Publish(MessageLevel.Info, "Memory Pool Collection Occured", sb.ToString());

                RemoveDeadEvents();
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(m_syncRoot);
            }
        }

        /// <summary>
        /// Searches the collection events and removes any events that have been collected by
        /// the garbage collector.
        /// </summary>
        private void RemoveDeadEvents()
        {
            m_requestCollectionEvent.RemoveIf(obj => !obj.IsAlive);
        }

        /// <summary>
        /// Gets the number of collection rounds base on the size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private int GetCollectionLevelBasedOnSize(long size)
        {
            if (size < m_levelNone)
                return 0;
            if (size < m_levelLow)
                return 1;
            if (size < m_levelNormal)
                return 2;
            if (size < m_levelHigh)
                return 3;
            if (size < m_levelVeryHigh)
                return 4;
            return 5;
        }

        private string GetCollectionLevelString(int iterations)
        {
            switch (iterations)
            {
                case 0:
                    return "0 (None)";
                case 1:
                    return "1 (Low)";
                case 2:
                    return "2 (Normal)";
                case 3:
                    return "3 (High)";
                case 4:
                    return "4 (Very High)";
                case 5:
                    return "5 (Critical)";
                default:
                    return iterations + " (Unknown)";
            }
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

        /// <summary>
        /// Calculates at what point a collection cycle will cease prematurely.
        /// </summary>
        /// <param name="size">the current size.</param>
        /// <returns></returns>
        private long CalculateStopShrinkingLimit(long size)
        {
            //once the size has been reduced by 15% of Memory but no less than 5% of memory
            long stopShrinkingLimit = size - (long)(MaximumPoolSize * 0.15);
            return Math.Max(stopShrinkingLimit, (long)(MaximumPoolSize * 0.05));
        }

        #endregion
    }
}