//******************************************************************************************************
//  ImmutableDictionary`2.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace GSF.Immutable
{
    /// <summary>
    /// A dictionary that can be modified until <see cref="ImmutableObjectBase{T}.IsReadOnly"/> is set to true. Once this occurs,
    /// the dictionary itself can no longer be modified.  Remember, this does not cause objects contained in this class to be Immutable 
    /// unless they implement <see cref="IImmutableObject"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ImmutableDictionary<TKey, TValue>
        : ImmutableObjectBase<ImmutableDictionary<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        private readonly bool m_isISupportsReadonlyKeyType;
        private readonly bool m_isISupportsReadonlyValueType;
        private Dictionary<TKey, TValue> m_dictionary;

        /// <summary>
        /// Creates a new <see cref="ImmutableDictionary{TKey,TValue}"/>.
        /// </summary>
        public ImmutableDictionary()
        {
            m_isISupportsReadonlyKeyType = typeof(IImmutableObject).IsAssignableFrom(typeof(TKey));
            m_isISupportsReadonlyValueType = typeof(IImmutableObject).IsAssignableFrom(typeof(TValue));
            m_dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableDictionary{TKey,TValue}"/>.
        /// </summary>
        public ImmutableDictionary(int capacity)
        {
            m_isISupportsReadonlyKeyType = typeof(IImmutableObject).IsAssignableFrom(typeof(TKey));
            m_isISupportsReadonlyValueType = typeof(IImmutableObject).IsAssignableFrom(typeof(TValue));
            m_dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableDictionary{TKey,TValue}"/>.
        /// </summary>
        public ImmutableDictionary(Dictionary<TKey, TValue> baseDictionary)
        {
            m_dictionary = baseDictionary;
            m_isISupportsReadonlyKeyType = typeof(IImmutableObject).IsAssignableFrom(typeof(TKey));
            m_isISupportsReadonlyValueType = typeof(IImmutableObject).IsAssignableFrom(typeof(TValue));
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return m_dictionary.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_dictionary).GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            TestForEditable();
            ((ICollection<KeyValuePair<TKey, TValue>>)m_dictionary).Add(item);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear()
        {
            TestForEditable();
            m_dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)m_dictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)m_dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TestForEditable();
            return ((ICollection<KeyValuePair<TKey, TValue>>)m_dictionary).Remove(item);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            get
            {
                return m_dictionary.Count;
            }
        }

        protected override void SetMembersAsReadOnly()
        {
            if (m_isISupportsReadonlyKeyType || m_isISupportsReadonlyValueType)
            {
                foreach (var kvp in m_dictionary)
                {
                    if (m_isISupportsReadonlyKeyType)
                    {
                        var item = kvp.Key as IImmutableObject;
                        if (item != null)
                        {
                            item.IsReadOnly = true;
                        }
                    }
                    if (m_isISupportsReadonlyValueType)
                    {
                        var item = kvp.Value as IImmutableObject;
                        if (item != null)
                        {
                            item.IsReadOnly = true;
                        }
                    }
                }
            }
        }

        protected override void CloneMembersAsEditable()
        {
            if (m_isISupportsReadonlyKeyType || m_isISupportsReadonlyValueType)
            {
                var oldList = m_dictionary;
                m_dictionary = new Dictionary<TKey, TValue>();
                foreach (var kvp in oldList)
                {
                    var k = kvp.Key;
                    var v = kvp.Value;
                    if (m_isISupportsReadonlyKeyType)
                    {
                        var item = k as IImmutableObject;
                        if (item != null)
                        {
                            k = (TKey)item.CloneEditable();
                        }
                    }
                    if (m_isISupportsReadonlyValueType)
                    {
                        var item = k as IImmutableObject;
                        if (item != null)
                        {
                            v = (TValue)item.CloneEditable();
                        }
                    }
                    m_dictionary.Add(k, v);
                }
            }
            else
            {
                m_dictionary = new Dictionary<TKey, TValue>(m_dictionary);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return m_dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            TestForEditable();
            m_dictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            TestForEditable();
            return m_dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                return m_dictionary[key];
            }
            set
            {
                TestForEditable();
                m_dictionary[key] = value;
            }
        }

        public ICollection<TKey> Keys => m_dictionary.Keys;

        public ICollection<TValue> Values => m_dictionary.Values;
    }
}