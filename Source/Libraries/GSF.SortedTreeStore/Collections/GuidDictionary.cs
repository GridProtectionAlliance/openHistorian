//******************************************************************************************************
//  ReadonlyGuidDictionary.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  4/24/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GSF.Collections
{
    unsafe public class ReadonlyGuidDictionary<T>
        : IDictionary<Guid, T>
    {
        class Entry
        {
            public long GuidLeft;
            public long GuidRight;
            public T Value;
            public int Next;
            public Entry(byte* id, T value)
            {
                GuidLeft = *(long*)(id);
                GuidRight = *(long*)(id + 8);
                Value = value;
                Next = -1;
            }

            public Guid ID
            {
                get
                {
                    Guid value = default(Guid);
                    *(long*)(&value) = GuidLeft;
                    *(long*)((byte*)&value + 8) = GuidRight;
                    return value;
                }
            }
        }

        int m_bucketMask;
        int m_count;
        int[] m_buckets;
        Entry[] m_entries;

        public ReadonlyGuidDictionary(Dictionary<Guid, T> source)
        {
            //Fill all entries
            Guid key;
            m_count = source.Count;
            m_entries = new Entry[m_count];
            int index = 0;
            foreach (var kvp in source)
            {
                key = kvp.Key;
                m_entries[index] = new Entry((byte*)&key, kvp.Value);
                index++;
            }

            int bucketCount = (int)BitMath.RoundUpToNearestPowerOfTwo((uint)m_count * 3);
            m_buckets = new int[bucketCount];
            for (int x = 0; x < bucketCount; x++)
            {
                m_buckets[x] = -1;
            }
            m_bucketMask = bucketCount - 1;

            for (int x = 0; x < m_count; x++)
            {
                int mask = (int)m_entries[x].GuidLeft & m_bucketMask;
                if (m_buckets[mask] < 0)
                {
                    m_buckets[mask] = x;
                }
                else
                {
                    int i = m_buckets[mask];
                    while (m_entries[i].Next >= 0)
                    {
                        i = m_entries[i].Next;
                    }
                    m_entries[i].Next = x;
                }
            }

            m_bucketMask = m_bucketMask;

        }

        public IEnumerator<KeyValuePair<Guid, T>> GetEnumerator()
        {
            foreach (var bucket in m_entries)
            {
                yield return new KeyValuePair<Guid, T>(bucket.ID, bucket.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<Guid, T> item)
        {
            throw new NotSupportedException("Cannot modify this dictionary");
        }

        public void Clear()
        {
            throw new NotSupportedException("Cannot modify this dictionary");
        }

        int FindIndex(byte* key)
        {
            int bucket = (int)key & m_bucketMask;
            int index = m_buckets[bucket];
            while (index >= 0)
            {
                if (m_entries[index].GuidLeft == *(long*)key && m_entries[index].GuidRight == *(long*)(key + 8))
                {
                    return index;
                }
                index = m_entries[index].Next;
            }
            return -1;
        }

        public bool Contains(KeyValuePair<Guid, T> item)
        {
            Guid key = item.Key;
            int id = FindIndex((byte*)&key);
            if (id <= 0)
                return false;
            return item.Value.Equals(m_entries[id].Value);
        }

        public void CopyTo(KeyValuePair<Guid, T>[] array, int arrayIndex)
        {
            this.ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<Guid, T> item)
        {
            throw new NotSupportedException("Cannot modify this dictionary");
        }

        public int Count
        {
            get
            {
                return m_count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool ContainsKey(Guid key)
        {
            return FindIndex((byte*)&key) >= 0;
        }

        public void Add(Guid key, T value)
        {
            throw new NotSupportedException("Cannot modify this dictionary");
        }

        public bool Remove(Guid key)
        {
            throw new NotSupportedException("Cannot modify this dictionary");
        }

        public bool TryGetValue(Guid key, out T value)
        {
            int index = FindIndex((byte*)&key);
            if (index >= 0)
            {
                value = m_entries[index].Value;
                return true;
            }
            value = default(T);
            return false;
        }

        public T this[Guid key]
        {
            get
            {
                int index = FindIndex((byte*)&key);
                if (index <= 0)
                    throw new KeyNotFoundException("Missing");
                return m_entries[index].Value;
            }
            set
            {
                throw new NotSupportedException("Cannot modify this dictionary");
            }
        }

        public ICollection<Guid> Keys { get; private set; }

        public ICollection<T> Values { get; private set; }
    }
}
