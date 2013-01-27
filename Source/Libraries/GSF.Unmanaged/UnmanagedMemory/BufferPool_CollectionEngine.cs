//******************************************************************************************************
//  BufferPoolCollectionEngine.cs - Gbtc
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
//  8/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Threading;

namespace GSF.UnmanagedMemory
{
    /// <summary>
    /// Does the garbage collection and manages the actual size of the buffer pool.
    /// This class is not thread safe.
    /// </summary>
    public partial class BufferPool
    {
        internal class CollectionEngine
        {
            /// <summary>
            /// Delegates are placed in a List because
            /// in a later version, some sort of concurrent garbage collection may be implemented
            /// which means more control will need to be with the Event
            /// </summary>
            ThreadSafeList<WeakEventHandler<CollectionEventArgs>> m_requestCollectionEvent;

            BufferPool m_pool;

            public CollectionEngine(BufferPool pool)
            {
                m_pool = pool;
                m_requestCollectionEvent = new ThreadSafeList<WeakEventHandler<CollectionEventArgs>>();
            }

            /// <summary>
            /// Determines whether to allocate more memory or to do a collection cycle on the existing pool.
            /// </summary>
            public void AllocateMoreFreeSpace()
            {
                long size = m_pool.CurrentCapacity;
                int collectionCount = m_pool.GetCollectionBasedOnSize(size);
                long stopShrinkingLimit = CalculateStopShrinkingLimit(size);

                RemoveDeadEvents();

                for (int x = 0; x < collectionCount; x++)
                {
                    if (m_pool.CurrentAllocatedSize < stopShrinkingLimit)
                        break;
                    var eventArgs = new CollectionEventArgs(m_pool.TryReleasePage, BufferPoolCollectionMode.Normal, 0);

                    foreach (var c in m_requestCollectionEvent)
                    {
                        c.TryInvoke(m_pool, eventArgs);
                    }
                }

                long newSize = m_pool.CurrentAllocatedSize;

                m_pool.GrowBufferToSize(newSize + (long)(0.1 * m_pool.MaximumBufferSize));

                if (m_pool.FreeSpaceBytes < 0.05 * m_pool.MaximumBufferSize)
                {
                    int pagesToBeReleased =
                        (int)((0.05 * m_pool.MaximumBufferSize - m_pool.FreeSpaceBytes) / m_pool.PageSize);
                    var eventArgs = new CollectionEventArgs(m_pool.TryReleasePage, BufferPoolCollectionMode.Emergency,
                                                            pagesToBeReleased);

                    foreach (var c in m_requestCollectionEvent)
                    {
                        if (eventArgs.DesiredPageReleaseCount == 0)
                            break;
                        c.TryInvoke(m_pool, eventArgs);
                    }

                    if (eventArgs.DesiredPageReleaseCount > 0)
                    {
                        eventArgs = new CollectionEventArgs(m_pool.TryReleasePage, BufferPoolCollectionMode.Critical,
                                                            eventArgs.DesiredPageReleaseCount);
                        foreach (var c in m_requestCollectionEvent)
                        {
                            if (eventArgs.DesiredPageReleaseCount == 0)
                                break;
                            c.TryInvoke(m_pool, eventArgs);
                        }
                        if (m_pool.IsFull)
                        {
                            throw new OutOfMemoryException("Buffer pool is out of memory");
                        }
                    }
                }

                RemoveDeadEvents();

            }

            /// <summary>
            /// Calculates at what point a collection cycle will cease prematurely.
            /// </summary>
            /// <param name="size">the current size.</param>
            /// <returns></returns>
            long CalculateStopShrinkingLimit(long size)
            {
                //once the size has been reduced by 15% of Memory but no less than 5% of memory
                long stopShrinkingLimit = size - (long)(m_pool.MaximumBufferSize * 0.15);
                return Math.Max(stopShrinkingLimit, (long)(m_pool.MaximumBufferSize * 0.05));
            }

            public void AddEvent(EventHandler<CollectionEventArgs> client)
            {
                m_requestCollectionEvent.Add(new WeakEventHandler<CollectionEventArgs>(client));
                RemoveDeadEvents();
            }

            public void RemoveEvent(EventHandler<CollectionEventArgs> client)
            {
                m_requestCollectionEvent.RemoveAndWait(new WeakEventHandler<CollectionEventArgs>(client));
                RemoveDeadEvents();
            }

            /// <summary>
            /// Searches the collection events and removes any events that have been collected by
            /// the garbage collector.
            /// </summary>
            void RemoveDeadEvents()
            {
                m_requestCollectionEvent.RemoveIf(obj => !obj.IsAlive);
            }
        }
    }
}
