//******************************************************************************************************
//  WeakList`1.cs - Gbtc
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
//  4/11/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace GSF.Collections
{
    /// <summary>
    /// Creates a list of items that will be weak referenced.
    /// This list is thread safe and allows enumeration while adding and removing from the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This is a special case list where all items in the list are weak referenced. Only include
    /// instances that are strong referenced somewhere. 
    /// For example, delegates will not work in this list unless it is strong referenced in the instance.
    /// This is because a delegate is a small wrapper of (Object,Method) and is usually recreated on the fly
    /// rather than stored. Therefore if the only reference to the delegate is passed to this list, it will be
    /// collected at the next GC cycle.
    /// </remarks>
    public class WeakList<T> : IEnumerable<T>
        where T : class
    {
        /// <summary>
        /// Contains a snapshot of the data so read operations can be non-blocking
        /// </summary>
        private class Snapshot
        {
            public WeakReference[] Items;
            public int Size;

            public Snapshot(int capacity)
            {
                Items = new WeakReference[capacity];
                Size = 0;
            }

            /// <summary>
            /// Grows the snapshot, doubling the size of the number of entries.
            /// </summary>
            public Snapshot Grow()
            {
                int itemCount = 0;

                // Count the number of entries that are valid
                for (int x = 0; x < Items.Length; x++)
                {
                    WeakReference reference = Items[x];

                    if (reference is null)
                        continue;

                    if (reference.Target is T)
                        itemCount++;
                    else
                        Items[x] = null;
                }

                // copy the snapshot.
                int capacity = Math.Max(itemCount * 2, 8);

                Snapshot clone = new Snapshot(capacity)
                {
                    Items = new WeakReference[capacity], 
                    Size = 0
                };

                for (int x = 0; x < Items.Length; x++)
                {
                    WeakReference reference = Items[x];

                    if (reference is null)
                        continue;

                    // Since checking the weak reference is slow, just assume that 
                    // it still has reference. It won't hurt anything
                    if (!clone.TryAdd(reference))
                        throw new Exception("List is full");
                }

                return clone;
            }

            /// <summary>
            /// Removes all occurrences of <see cref="item"/> from the list
            /// </summary>
            /// <param name="item">Item to remove.</param>
            public void Remove(T item)
            {
                if (item is null)
                    return;

                int count = Size;
                EqualityComparer<T> compare = EqualityComparer<T>.Default;

                for (int x = 0; x < count; x++)
                {
                    WeakReference reference = Items[x];

                    if (reference is null)
                        continue;

                    if (reference.Target is T itemCompare)
                    {
                        if (compare.Equals(itemCompare, item))
                            Items[x] = null;
                    }
                    else
                    {
                        Items[x] = null;
                    }
                }
            }

            /// <summary>
            /// Attempts to add <see cref="item"/> to the list. 
            /// </summary>
            /// <param name="item"></param>
            /// <returns>returns true if added, false otherwise.</returns>
            public bool TryAdd(T item)
            {
                return TryAdd(new WeakReference(item));
            }

            private bool TryAdd(WeakReference item)
            {
                if (Size < Items.Length)
                {
                    Items[Size] = item;
                    Size++;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// An <see cref="IEnumerator{T}"/> for <see cref="WeakList{T}"/>
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly WeakReference[] m_items;
            private readonly int m_lastItem;
            private int m_currentIndex;
            private T m_current;

            /// <summary>
            /// Creates a <see cref="Enumerator"/>
            /// </summary>
            /// <param name="items">the weak referenced items.</param>
            /// <param name="count">the number of valid items in the list.</param>
            public Enumerator(WeakReference[] items, int count)
            {
                m_items = items;
                m_lastItem = count - 1;
                m_currentIndex = -1;
                m_current = null;
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public T Current => m_current;

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <returns>
            /// The current element in the collection.
            /// </returns>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
            public bool MoveNext()
            {
                while (m_currentIndex < m_lastItem)
                {
                    m_currentIndex++;
                    WeakReference reference = m_items[m_currentIndex];

                    if (reference is null)
                        continue;

                    if (reference.Target is T item)
                    {
                        m_current = item;
                        return true;
                    }

                    m_items[m_currentIndex] = null;
                }

                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
            public void Reset()
            {
                m_current = null;
                m_currentIndex = -1;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() => m_current = null;
        }

        private readonly object m_syncRoot;
        private Snapshot m_data;

        /// <summary>
        /// Creates a <see cref="WeakList{T}"/>
        /// </summary>
        public WeakList()
        {
            m_syncRoot = new object();
            m_data = new Snapshot(8);
        }

        /// <summary>
        /// Clears all of the times in the list. Method is thread safe.
        /// </summary>
        public void Clear()
        {
            lock (m_syncRoot)
            {
                m_data.Size = 0;
                Array.Clear(m_data.Items, 0, m_data.Items.Length);
            }
        }

        /// <summary>
        /// Adds the <see cref="item"/> to the list. Method is thread safe.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(T item)
        {
            lock (m_syncRoot)
            {
                if (!m_data.TryAdd(item))
                {
                    m_data = m_data.Grow();

                    if (!m_data.TryAdd(item))
                        throw new Exception("Could not grow list");
                }
            }
        }

        /// <summary>
        /// Removes all occurrences of the <see cref="item"/> from the list. Method is thread safe.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public void Remove(T item)
        {
            lock (m_syncRoot)
                m_data.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public Enumerator GetEnumerator()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            Snapshot snapshot = m_data;
            return new Enumerator(snapshot.Items, snapshot.Size);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
