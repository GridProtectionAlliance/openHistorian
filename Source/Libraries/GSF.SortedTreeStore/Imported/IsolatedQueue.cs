//******************************************************************************************************
//  IsolatedQueue.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  1/4/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Collections
{
    /// <summary>
    /// Provides a buffer of point data where reads are isolated from writes.
    /// However, reads must be synchronized with other reads and writes must be synchronized with other writes.
    /// </summary>
    /// <remarks>
    /// This class is about twice as fast as a ConcurrentQueue. However, it has some decent overhead, so 
    /// small lists may not make much sense.
    /// 
    /// This also uses jagged arrays, and will automatically deallocate memory so is useful for 
    /// structures that vary in size with time. 
    /// </remarks>
    public class IsolatedQueue<T>
    {
        /// <summary>
        /// Represents an individual node that allows for items to be added and removed from the 
        /// queue independently and without locks. 
        /// </summary>
        class IsolatedNode
        {
            private readonly int m_lastBlock;
            private volatile int m_tail;
            private volatile int m_head;
            private readonly T[] m_blocks;

            /// <summary>
            /// Creates a <see cref="IsolatedNode"/>
            /// </summary>
            /// <param name="count">the number of items in each node.</param>
            public IsolatedNode(int count)
            {
                m_tail = 0;
                m_head = 0;
                m_blocks = new T[count];
                m_lastBlock = m_blocks.Length;
            }

            /// <summary>
            /// Gets if the current node is out of entries.
            /// </summary>
            public bool DequeueMustMoveToNextNode
            {
                get
                {
                    return m_tail == m_lastBlock;
                }
            }

            /// <summary>
            /// Gets if there are items that can be dequeued
            /// </summary>
            public bool CanDequeue
            {
                get
                {
                    return m_head != m_tail;
                }
            }

            /// <summary>
            /// Gets if this list can be enqueued
            /// </summary>
            public bool CanEnqueue
            {
                get
                {
                    return m_head != m_lastBlock;
                }
            }

            /// <summary>
            /// Gets the number of items that can be enqueued.
            /// </summary>
            public int AvailableEnqueueLength
            {
                get
                {
                    return m_lastBlock - m_head;
                }
            }

            /// <summary>
            /// Resets the queue. This operation must be synchronized external 
            /// from this class with the read and write operations. Therefore it 
            /// is only recommended to call this once the item has been returned to a 
            /// buffer pool of some sorts.
            /// </summary>
            public void Reset()
            {
                if (m_tail != m_head)
                    Array.Clear(m_blocks, m_tail, m_head - m_tail);
                m_tail = 0;
                m_head = 0;
            }

            /// <summary>
            /// Adds the following item to the queue. Be sure to check if it is full first.
            /// </summary>
            /// <param name="item"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Enqueue(T item)
            {
                m_blocks[m_head] = item;
                //No memory barior here since .NET 2.0 ensures that writes will not be reordered.
                m_head = m_head + 1;
            }

            /// <summary>
            /// Adds the following items to the queue. Be sure to check if it is full first. 
            /// Adjust the length if not the right size.
            /// </summary>
            /// <param name="items"></param>
            /// <param name="offset"></param>
            /// <param name="length"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Enqueue(T[] items, int offset, int length)
            {
                Array.Copy(items, offset, m_blocks, m_head, length);
                //No memory barior here since .NET 2.0 ensures that writes will not be reordered.
                m_head = m_head + length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Dequeue()
            {
                T item = m_blocks[m_tail];
                m_blocks[m_tail] = default(T);
                //No memory barior here since .NET 2.0 ensures that writes will not be reordered.
                m_tail = m_tail + 1;
                return item;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public int Dequeue(T[] items, ref int startingIndex, ref int length)
            {
                int count = Math.Min(m_head - m_tail, length);
                Array.Copy(m_blocks, m_tail, items, startingIndex, count);
                Array.Clear(m_blocks, m_tail, count);
                m_tail += count;
                startingIndex += count;
                length -= count;
                return count;
            }
        }


        private readonly ConcurrentQueue<IsolatedNode> m_blocks;
        private readonly ConcurrentQueue<IsolatedNode> m_pooledNodes;

        private IsolatedNode m_currentHead;
        private IsolatedNode m_currentTail;

        private readonly int m_unitCount;
        private long m_enqueueCount;
        private long m_dequeueCount;

        /// <summary>
        /// Creates an <see cref="IsolatedQueue{T}"/>
        /// </summary>
        public IsolatedQueue()
        {
            m_unitCount = 128;
            m_pooledNodes = new ConcurrentQueue<IsolatedNode>();
            m_blocks = new ConcurrentQueue<IsolatedNode>();
        }

        /// <summary>
        /// The number of elements in the queue
        /// </summary>
        public long Count
        {
            get
            {
                //ToDo: Figure out how to make this value always correct on a 32-bit computer.
                return m_enqueueCount - m_dequeueCount;
            }
        }

        /// <summary>
        /// Addes the provided items to the <see cref="IsolatedQueue{T}"/>.
        /// </summary>
        /// <param name="items">the items to add</param>
        /// <param name="offset">the offset postion</param>
        /// <param name="length">the length</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Enqueue(T[] items, int offset, int length)
        {
            items.ValidateParameters(offset, length);
            while (true)
            {
                //If the header is empty. Get a new one.
                if (m_currentHead == null || !m_currentHead.CanEnqueue)
                {
                    m_currentHead = GetNode();
                    m_currentHead.Reset();
                    Thread.MemoryBarrier();
                    m_blocks.Enqueue(m_currentHead);
                    m_enqueueCount++;
                }

                //If the remainder will fit, queue it all. 
                int enqueueLength = m_currentHead.AvailableEnqueueLength;
                if (length <= enqueueLength)
                {
                    m_currentHead.Enqueue(items, offset, length);
                    m_enqueueCount += length;
                    return;
                }

                //Some will not fit. Enqueue what will fit, then repeat.
                m_currentHead.Enqueue(items, offset, enqueueLength);
                m_enqueueCount += enqueueLength;
                offset += enqueueLength;
                length -= enqueueLength;
            }
        }

        /// <summary>
        /// Addes the provided item to the <see cref="IsolatedQueue{T}"/>.
        /// </summary>
        /// <param name="item"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Enqueue(T item)
        {
            if (m_currentHead != null && m_currentHead.CanEnqueue)
            {
                m_currentHead.Enqueue(item);
                m_enqueueCount++;
                return;
            }
            EnqueueSlower(item);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void EnqueueSlower(T item)
        {
            if (m_currentHead == null || !m_currentHead.CanEnqueue)
            {
                m_currentHead = GetNode();
                m_currentHead.Reset();
                Thread.MemoryBarrier();
                m_blocks.Enqueue(m_currentHead);
                m_enqueueCount++;
            }
            m_currentHead.Enqueue(item);
        }

        /// <summary>
        /// Attempts to dequeue the specified item from the <see cref="IsolatedQueue{T}"/>
        /// </summary>
        /// <param name="item">an output for the item</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool TryDequeue(out T item)
        {
            if (m_currentTail != null && m_currentTail.CanDequeue)
            {
                item = m_currentTail.Dequeue();
                m_dequeueCount++;
                return true;
            }
            return TryDequeueSlower(out item);
        }

        /// <summary>
        /// Dequeues all of the items into the provided array
        /// </summary>
        /// <param name="items">where to put the items</param>
        /// <param name="startingIndex">the starting index</param>
        /// <param name="length">the maximum number of times to store</param>
        /// <returns>the number of items dequeued</returns>
        public int Dequeue(T[] items, int startingIndex, int length)
        {
            items.ValidateParameters(startingIndex, length);
            int itemCount = 0;

        TryAgain:

            if (m_currentTail == null)
            {
                if (!m_blocks.TryDequeue(out m_currentTail))
                {
                    return 0;
                }
            }

            if (m_currentTail.CanDequeue)
            {
                //Dequeue as many items as possible
                itemCount += m_currentTail.Dequeue(items, ref startingIndex, ref length);

                //If no more items can fit in parameter: items, then exit
                if (length == 0)
                {
                    m_dequeueCount += itemCount;
                    return itemCount;
                }
            }

            //If tail position of node is not at the end, all of the items in the Isolated Queue have been read.
            //Note: because of a race condition, it may not still be empty,
            //but cordinating that closely may cause drastically decrease performance.
            if (!m_currentTail.DequeueMustMoveToNextNode)
            {
                m_dequeueCount += itemCount;
                return itemCount;
            }

            //Don't reset the node on return since it is still
            //possible for the enqueue thread to be using it. 
            //Note: If the enqueue thread pulls it off the queue
            //immediately, this is ok since it will be coordinated at that point.
            ReleaseNode(m_currentTail);

            if (!m_blocks.TryDequeue(out m_currentTail))
            {
                m_dequeueCount += itemCount;
                return itemCount;
            }

            goto TryAgain;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        bool TryDequeueSlower(out T item)
        {
            if (m_currentTail == null)
            {
                if (!m_blocks.TryDequeue(out m_currentTail))
                {
                    item = default(T);
                    return false;
                }
            }
            else if (m_currentTail.DequeueMustMoveToNextNode)
            {
                //Don't reset the node on return since it is still
                //possible for the enqueue thread to be using it. 
                //Note: If the enqueue thread pulls it off the queue
                //immediately, this is ok since it will be coordinated at that point.
                ReleaseNode(m_currentTail);

                if (!m_blocks.TryDequeue(out m_currentTail))
                {
                    item = default(T);
                    return false;
                }
            }
            if (m_currentTail.CanDequeue)
            {
                item = m_currentTail.Dequeue();
                m_dequeueCount++;
                return true;
            }
            item = default(T);
            return false;
        }


        /// <summary>
        /// Removes a node from the pool. If one does not exist, one is created.
        /// </summary>
        /// <returns></returns>
        IsolatedNode GetNode()
        {
            IsolatedNode item;
            if (m_pooledNodes.TryDequeue(out item))
                return item;

            return new IsolatedNode(m_unitCount);
        }

        /// <summary>
        /// Addes an item back to the queue.
        /// </summary>
        /// <param name="resource"></param>
        void ReleaseNode(IsolatedNode resource)
        {
            //if the queue has too many node items. just let the old one get garbage collected.
            if (m_pooledNodes.Count < 1000)
            {
                m_pooledNodes.Enqueue(resource);
            }
        }

    }
}