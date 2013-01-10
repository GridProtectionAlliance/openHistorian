//******************************************************************************************************
//  ContinuousQueue.cs - Gbtc
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
//  1/5/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//*****************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using System;

namespace openHistorian.Collections
{
    /// <summary>
    /// Implements a circular buffer that can be absolutely indexed. This means that when items are added to this buffer, their index 
    /// will be unique and sequentially numbered.  This item can be retrieved calling the GetItem() function until
    /// it is removed from this buffer.
    /// This class behaves much like a circular buffer that can be indexed.
    /// </summary>
    /// <typeparam name="T">The type to make the elements.</typeparam>
    public class ContinuousQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// Contains the array of objects
        /// </summary>
        T[] m_items;

        /// <summary>
        /// Contains the head pointer of the circular buffer.
        /// </summary>
        int m_head;
        /// <summary>
        /// Contains the tail of the circular buffer
        /// </summary>
        int m_tail;
        /// <summary>
        /// The number of items in the buffer
        /// </summary>
        int m_count;
        /// <summary>
        /// The first index that exists in the buffer.
        /// </summary>
        long m_tailIndex;

        /// <summary>
        /// Gets the first index that exists in the buffer.  
        /// Trying to retrieve any index lower than this will throw an out of bounds exception.
        /// </summary>
        public long TailIndex
        {
            get
            {
                return m_tailIndex;
            }
        }

        /// <summary>
        /// The number of items that are currently in the buffer.
        /// </summary>
        public int Count
        {
            get
            {
                return m_count;
            }
        }
        /// <summary>
        /// Represents the last item that can be indexed in this buffer.
        /// </summary>
        public long HeadIndex
        {
            get
            {
                return m_count + m_tailIndex - 1;
            }
        }

        /// <summary>
        /// Gets the current capacity of the ContinuousQueue
        /// </summary>
        public int Capacity
        {
            get
            {
                return m_items.Length;
            }
        }

        /// <summary>
        /// Creates a new ContinuousQueue
        /// </summary>
        /// <param name="capacity"></param>
        public ContinuousQueue(int capacity = 16)
        {
            m_head = 0;
            m_tail = 0;
            m_count = 0;
            m_tailIndex = 0;
            SetCapacity(capacity);
        }

        /// <summary>
        /// Adds an item to the head of the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public long Enqueue(T item)
        {
            return AddToHead(item);
        }

        /// <summary>
        /// Adds an item to the head of the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public long Push(T item)
        {
            return AddToHead(item);
        }

        /// <summary>
        /// Removes an item from the tail of the queue.
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            return RemoveFromTail();
        }

        /// <summary>
        /// Removes an item from the head of the queue.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            return RemoveFromHead();
        }

        public T PeekAtHead()
        {
            return this[HeadIndex];
        }

        public T PeekAtTail()
        {
            return this[TailIndex];
        }

        /// <summary>
        /// Adds an item to the head of the queue. 
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The index of the item</returns>
        public long AddToHead(T item)
        {
            if (Count == Capacity)
                SetCapacity(Count * 2);
            m_items[m_head] = item;
            IncrementHead();
            return HeadIndex;
        }

        /// <summary>
        /// Adds an item to the tail of the queue. 
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The index of the item</returns>
        public long AddToTail(T item)
        {
            if (Count == Capacity)
                SetCapacity(Count * 2);
            DecrementTail();
            m_items[m_tail] = item;
            return TailIndex;
        }

        /// <summary>
        /// Removes an item from the beginning of the buffer.
        /// </summary>
        /// <returns></returns>
        public T RemoveFromHead()
        {
            if (Count == 0)
                throw new Exception("Queue is empty");
            DecrementHead();
            T rv = m_items[m_head];
            m_items[m_head] = default(T);
            return rv;
        }

        /// <summary>
        /// Removes an item from the end of the buffer.
        /// </summary>
        /// <returns></returns>
        public T RemoveFromTail()
        {
            if (Count == 0)
                throw new Exception("Queue is empty");
            T rv = m_items[m_tail];
            m_items[m_tail] = default(T);
            IncrementTail();
            return rv;
        }

        /// <summary>
        /// Gets the item from the list.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetItem(long index)
        {
            if (index < TailIndex || index > HeadIndex)
            {
                throw new ArgumentOutOfRangeException("index", "index must be between the HeadIndex and TailIndex of the queue.");
            }
            int relativeIndex = (int)(index - TailIndex);
            relativeIndex += m_tail;
            if (relativeIndex >= Capacity)
                relativeIndex -= Capacity;
            return m_items[relativeIndex];
        }

        /// <summary>
        /// Sets the value of an item in the list. 
        /// The index for the item must already exist in the list
        /// </summary>
        /// <param name="index">the index of the position</param>
        /// <param name="item"></param>
        /// <returns></returns>
        public void SetItem(long index, T item)
        {
            if (index < TailIndex || index > HeadIndex)
            {
                throw new ArgumentOutOfRangeException("index", "index must be between the HeadIndex and TailIndex of the queue.");
            }
            int relativeIndex = (int)(index - TailIndex);
            relativeIndex += m_tail;
            if (relativeIndex >= Capacity)
                relativeIndex -= Capacity;
            m_items[relativeIndex] = item;
        }

        /// <summary>
        /// Indexer that gets/sets an existing item in the list.
        /// </summary>
        /// <param name="index">the univerals index for the item.</param>
        /// <returns></returns>
        public T this[long index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, value);
            }
        }

        /// <summary>
        /// Sets the capacity of the queue
        /// Can shrink or grow the size, but will always be large enough to store all
        /// of the elements in the queue.
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        int SetCapacity(int capacity)
        {
            capacity = Math.Max(capacity, Count);
            T[] items = new T[capacity];
            if (Count > 0)
            {
                if (m_head > m_tail)
                {
                    Array.Copy(m_items, m_tail, items, 0, Count);
                }
                else
                {
                    int remainingAtEnd = m_items.Length - m_tail;
                    Array.Copy(m_items, m_tail, items, 0, remainingAtEnd);
                    Array.Copy(m_items, 0, items, remainingAtEnd, m_head);
                }
            }
            m_items = items;
            m_tail = 0;
            m_head = Count;
            if (m_head >= capacity)
                m_head -= capacity;

            return capacity;
        }

        void IncrementTail()
        {
            m_tailIndex++;
            m_count--;
            m_tail++;
            if (m_tail == Capacity)
                m_tail -= Capacity;
        }
        void DecrementTail()
        {
            m_tailIndex--;
            m_count++;
            m_tail--;
            if (m_tail < 0)
                m_tail += Capacity;
        }
        void IncrementHead()
        {

            m_count++;
            m_head++;
            if (m_head == Capacity)
                m_head -= Capacity;
        }
        void DecrementHead()
        {
            m_count--;
            m_head--;
            if (m_head < 0)
                m_head += Capacity;

        }
        public T[] ToArray()
        {
            T[] items = new T[Count];
            if (Count > 0)
            {
                if (m_head > m_tail)
                {
                    Array.Copy(m_items, m_tail, items, 0, Count);
                }
                else
                {
                    int remainingAtEnd = m_items.Length - m_tail;
                    Array.Copy(m_items, m_tail, items, 0, remainingAtEnd);
                    Array.Copy(m_items, 0, items, remainingAtEnd, m_head);
                }
            }
            return items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)ToArray()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
