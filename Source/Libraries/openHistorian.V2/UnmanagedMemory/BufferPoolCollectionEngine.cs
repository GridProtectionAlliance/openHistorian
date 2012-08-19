//******************************************************************************************************
//  BufferPoolCollectionEngine.cs - Gbtc
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
//  8/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace openHistorian.V2.UnmanagedMemory
{
    /// <summary>
    /// Does the garbage collection and manages the actual size of the buffer pool.
    /// This class is not thread safe.
    /// </summary>
    class BufferPoolCollectionEngine
    {
        List<EventHandler<CollectionEventArgs>> m_requestCollectionEvent;

        BufferPoolBlocks m_blocks;

        BufferPool m_pool;

        BufferPoolSettings m_settings;

        public BufferPoolCollectionEngine(BufferPool pool, BufferPoolBlocks blocks, BufferPoolSettings settings)
        {
            m_pool = pool;
            m_requestCollectionEvent = new List<EventHandler<CollectionEventArgs>>();
            m_blocks = blocks;
            m_settings = settings;
        }

        public void AllocateMoreFreeSpace()
        {
            long size = m_blocks.BufferPoolSize;
            int collectionCount = m_settings.GetCollectionBasedOnSize(size);
            long stopShrinkingLimit = CalculateStopShrinkingLimit(size);

            for (int x = 0; x < collectionCount; x++)
            {
                if (m_blocks.AllocatedPagesBytes < stopShrinkingLimit)
                    break;
                var eventArgs = new CollectionEventArgs(m_blocks.TryReleasePage, BufferPoolCollectionMode.Normal, 0);
                foreach (var c in m_requestCollectionEvent)
                {
                    c.Invoke(m_pool, eventArgs);
                }
            }


            long newSize = m_blocks.AllocatedPagesBytes;

            GrowBufferToSize(newSize + (long)(0.1 * m_settings.MaximumBufferSize));

            if (m_blocks.FreeSpaceBytes < 0.05 * m_settings.MaximumBufferSize)
            {
                int pagesToBeReleased = (int)((0.05 * m_settings.MaximumBufferSize - m_blocks.FreeSpaceBytes) / m_settings.PageSize);
                var eventArgs = new CollectionEventArgs(m_blocks.TryReleasePage, BufferPoolCollectionMode.Emergency, pagesToBeReleased);
                foreach (var c in m_requestCollectionEvent)
                {
                    if (eventArgs.DesiredPageReleaseCount == 0)
                        break;
                    c.Invoke(m_pool, eventArgs);
                }

                if (eventArgs.DesiredPageReleaseCount > 0)
                {
                    eventArgs = new CollectionEventArgs(m_blocks.TryReleasePage, BufferPoolCollectionMode.Critical, eventArgs.DesiredPageReleaseCount);
                    foreach (var c in m_requestCollectionEvent)
                    {
                        if (eventArgs.DesiredPageReleaseCount == 0)
                            break;
                        c.Invoke(m_pool, eventArgs);
                    }
                    if (m_blocks.IsFull)
                    {
                        throw new OutOfMemoryException("Buffer pool is out of memory");
                    }
                }
            }

        }

        /// <summary>
        /// Calculates at what point a collection cycle will cease prematurely.
        /// </summary>
        /// <param name="size">the current size.</param>
        /// <returns></returns>
        long CalculateStopShrinkingLimit(long size)
        {
            //once the size has been reduced by 15% of Memory but no less than 5% of memory
            long stopShrinkingLimit = size - (long)(m_settings.MaximumBufferSize * 0.15);
            return Math.Max(stopShrinkingLimit, (long)(m_settings.MaximumBufferSize * 0.05));
        }


        /// <summary>
        /// Grows the buffer pool to have the desired size
        /// </summary>
        void GrowBufferToSize(long size)
        {
            while (m_blocks.BufferPoolSize < size)
            {
                //If this goes beyond the desired maximum, exit
                if (!m_blocks.CanGrowBuffer)
                    return;
                m_blocks.AllocateWinApiBlock();
            }
        }

        public void AddEvent(EventHandler<CollectionEventArgs> client)
        {
            m_requestCollectionEvent.Add(client);
        }

        public void RemoveEvent(EventHandler<CollectionEventArgs> client)
        {
            m_requestCollectionEvent.Remove(client);
        }
    }
}
