//******************************************************************************************************
//  ConcurrentIndexedDictionary.cs - Gbtc
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
//  09/06/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace GSF.Collections
{
    /// <summary>
    /// A thread safe indexed dictionary that can only be added to.
    /// </summary>
    /// <remarks>
    /// This is a special purpose class that supports only the Add and Get operations.
    /// It is designed to have indexing capabilities or dictionary lookup.
    /// </remarks>
    public class ConcurrentIndexedDictionary<TKey, TValue>
    {
        private TValue[] m_items = new TValue[4];
        private readonly Dictionary<TKey, int> m_lookup = new Dictionary<TKey, int>();
        private readonly object m_syncRoot = new object();

        /// <summary>
        /// Gets the number of items in the dictionary
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the indexed item
        /// </summary>
        /// <param name="index">the index of the field</param>
        /// <returns></returns>
        public TValue this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= Count)
                    ThrowIndexException();
                return m_items[index];
            }
        }

        /// <summary>
        /// Gets the key associated with the value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            int index;
            lock (m_syncRoot)
                index = m_lookup[key];
            return this[index];
        }

        /// <summary>
        /// Gets the index of the <see cref="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>the index, or -1 if not found</returns>
        public int IndexOf(TKey key)
        {
            lock (m_syncRoot)
            {
                if (m_lookup.TryGetValue(key, out int index))
                {
                    return index;
                }
                return -1;
            }
        }

        /// <summary>
        /// Adds the specified item to the list
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>the index of the item</returns>
        public int Add(TKey key, TValue value)
        {
            lock (m_syncRoot)
            {
                if (m_lookup.ContainsKey(key))
                    throw new DuplicateNameException("Key already exists");
                return InternalAdd(key, value);
            }
        }

        private int InternalAdd(TKey key, TValue value)
        {
            m_lookup.Add(key, Count);

            if (Count == m_items.Length)
            {
                TValue[] newItems = new TValue[m_items.Length * 2];
                m_items.CopyTo(newItems, 0);
                m_items = newItems;
            }
            m_items[Count] = value;
            Count++;
            return Count - 1;
        }

        /// <summary>
        /// Gets or adds a value for the specified key in one atomic operation.
        /// </summary>
        /// <param name="key">the key to get</param>
        /// <param name="createFunction">A method to create the value if it does not exist</param>
        /// <returns> The value. </returns>
        public TValue GetOrAdd(TKey key, Func<TValue> createFunction)
        {
            lock (m_syncRoot)
            {
                if (m_lookup.TryGetValue(key, out int index))
                {
                    return this[index];
                }
                TValue value = createFunction();
                InternalAdd(key, value);
                return value;
            }
        }

        private void ThrowIndexException()
        {
            throw new IndexOutOfRangeException("specified index is outside the range of valid indexes");
        }

    }
}
