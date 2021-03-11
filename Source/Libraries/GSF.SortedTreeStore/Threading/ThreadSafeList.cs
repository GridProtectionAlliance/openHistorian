//******************************************************************************************************
//  ThreadSafeList.cs - Gbtc
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
//  1/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// This list allows for iterating through the list 
    /// while object can be removed from the list. Once an object has been
    /// removed, is garenteed not to be called again by a seperate thread. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class ThreadSafeList<T>
        : IEnumerable<T>
    {
        private class Wrapper
        {
            public int ReferencedCount;
            public readonly T Item;

            public Wrapper(T item)
            {
                Item = item;
            }
        }

        private readonly SortedList<long, Wrapper> m_list;
        private long m_sequenceNumber;
        private readonly object m_syncRoot;
        private long m_version;

        /// <summary>
        /// Creates a new <see cref="ThreadSafeList{T}"/>
        /// </summary>
        public ThreadSafeList()
        {
            m_syncRoot = new object();
            m_list = new SortedList<long, Wrapper>();
            m_sequenceNumber = 0;
            m_version = 0;
        }

        /// <summary>
        /// Adds the supplied item to the list
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            lock (m_syncRoot)
            {
                m_list.Add(m_sequenceNumber, new Wrapper(item));
                m_sequenceNumber++;
                m_version++;
            }
        }

        /// <summary>
        /// Removes an item from the list. 
        /// This method will block until the item has successfully been removed 
        /// and will no longer show up in the Iterator.
        /// DO NOT call this function from within a ForEach loop as it will block indefinately
        /// since the for each loop reads all items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveAndWait(T item)
        {
            SpinWait wait = new SpinWait();
            Wrapper itemToRemove = null;
            lock (m_syncRoot)
            {
                for (int x = 0; x < m_list.Count; x++)
                {
                    if (m_list.Values[x].Item.Equals(item))
                    {
                        itemToRemove = m_list.Values[x];
                        m_list.RemoveAt(x);
                        m_version++;
                        break;
                    }
                }
            }

            if (itemToRemove is null)
                return false;

            while (Interlocked.CompareExchange(ref itemToRemove.ReferencedCount, -1, 0) != 0)
            {
                wait.SpinOnce();
            }
            return true;
        }

        /// <summary>
        /// Removes an item from the list. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            Wrapper itemToRemove = null;
            lock (m_syncRoot)
            {
                for (int x = 0; x < m_list.Count; x++)
                {
                    if (m_list.Values[x].Item.Equals(item))
                    {
                        itemToRemove = m_list.Values[x];
                        m_list.RemoveAt(x);
                        m_version++;
                        break;
                    }
                }
            }

            if (itemToRemove is null)
                return false;

            return true;
        }

        /// <summary>
        /// Removes the specified item if the lamda expression is true.
        /// </summary>
        /// <param name="condition"></param>
        public void RemoveIf(Func<T, bool> condition)
        {
            lock (m_syncRoot)
            {
                for (int x = 0; x < m_list.Count; x++)
                {
                    if (condition(m_list.Values[x].Item))
                    {
                        m_list.RemoveAt(x);
                        m_version++;
                    }
                }
            }
        }

        /// <summary>
        /// Calls a foreach iterator on the supplied action.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T> action)
        {
            foreach (T item in this)
            {
                action(item);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(new Iterator(this));
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}