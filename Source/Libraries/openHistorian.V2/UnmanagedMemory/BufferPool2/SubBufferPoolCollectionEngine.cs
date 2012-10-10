////******************************************************************************************************
////  BufferPoolCollectionEngine.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  8/18/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;

//namespace openHistorian.V2.UnmanagedMemory
//{
//    /// <summary>
//    /// Does the garbage collection and manages the actual size of the buffer pool.
//    /// This class is not thread safe.
//    /// </summary>
//    public partial class SubBufferPool
//    {
//        /// <summary>
//        /// Delegates are placed in a List because
//        /// in a later version, some sort of concurrent garbage collection may be implemented
//        /// which means more control will need to be with the Event
//        /// </summary>
//        List<EventHandler<CollectionEventArgs>> m_requestCollectionEvent;

//        public void SubBufferPoolCollectionEngine()
//        {
//            m_requestCollectionEvent = new List<EventHandler<CollectionEventArgs>>();
//        }

//        /// <summary>
//        /// Determines whether to allocate more memory or to do a collection cycle on the existing pool.
//        /// </summary>
//        public void AllocateMoreFreeSpace()
//        {
//            long size = m_allocationList.BufferPoolSize;
//            int collectionCount = m_settings.GetCollectionBasedOnSize(size);
//            long stopShrinkingLimit = CalculateStopShrinkingLimit(size);

//            for (int x = 0; x < collectionCount; x++)
//            {
//                if (m_allocationList.AllocatedPagesBytes < stopShrinkingLimit)
//                    break;
//                var eventArgs = new CollectionEventArgs(m_allocationList.TryReleasePage, BufferPoolCollectionMode.Normal, 0);
//                foreach (var c in m_requestCollectionEvent)
//                {
//                    c.Invoke(m_pool, eventArgs);
//                }
//            }


//            long newSize = m_allocationList.AllocatedPagesBytes;

//            GrowBufferToSize(newSize + (long)(0.1 * m_settings.MaximumBufferSize));

//            if (m_allocationList.FreeSpaceBytes < 0.05 * m_settings.MaximumBufferSize)
//            {
//                int pagesToBeReleased = (int)((0.05 * m_settings.MaximumBufferSize - m_allocationList.FreeSpaceBytes) / m_settings.PageSize);
//                var eventArgs = new CollectionEventArgs(m_allocationList.TryReleasePage, BufferPoolCollectionMode.Emergency, pagesToBeReleased);
//                foreach (var c in m_requestCollectionEvent)
//                {
//                    if (eventArgs.DesiredPageReleaseCount == 0)
//                        break;
//                    c.Invoke(m_pool, eventArgs);
//                }

//                if (eventArgs.DesiredPageReleaseCount > 0)
//                {
//                    eventArgs = new CollectionEventArgs(m_allocationList.TryReleasePage, BufferPoolCollectionMode.Critical, eventArgs.DesiredPageReleaseCount);
//                    foreach (var c in m_requestCollectionEvent)
//                    {
//                        if (eventArgs.DesiredPageReleaseCount == 0)
//                            break;
//                        c.Invoke(m_pool, eventArgs);
//                    }
//                    if (m_allocationList.IsFull)
//                    {
//                        throw new OutOfMemoryException("Buffer pool is out of memory");
//                    }
//                }
//            }

//        }

//        /// <summary>
//        /// Calculates at what point a collection cycle will cease prematurely.
//        /// </summary>
//        /// <param name="size">the current size.</param>
//        /// <returns></returns>
//        long CalculateStopShrinkingLimit(long size)
//        {
//            //once the size has been reduced by 15% of Memory but no less than 5% of memory
//            long stopShrinkingLimit = size - (long)(m_settings.MaximumBufferSize * 0.15);
//            return Math.Max(stopShrinkingLimit, (long)(m_settings.MaximumBufferSize * 0.05));
//        }


//        /// <summary>
//        /// Grows the buffer pool to have the desired size
//        /// </summary>
//        void GrowBufferToSize(long size)
//        {
//            while (m_allocationList.BufferPoolSize < size)
//            {
//                //If this goes beyond the desired maximum, exit
//                if (!m_allocationList.CanGrowBuffer)
//                    return;
//                m_allocationList.AllocateWinApiBlock();
//            }
//        }

//        public void AddEvent(EventHandler<CollectionEventArgs> client)
//        {
//            m_requestCollectionEvent.Add(client);
//        }

//        public void RemoveEvent(EventHandler<CollectionEventArgs> client)
//        {
//            m_requestCollectionEvent.Remove(client);
//        }
//    }
//}
