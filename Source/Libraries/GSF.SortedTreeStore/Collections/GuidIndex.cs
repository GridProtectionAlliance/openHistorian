//******************************************************************************************************
//  GuidIndex.cs - Gbtc
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace GSF.Collections
{
    public unsafe class GuidDictionary<T>
    {
        GuidIndex m_index;
        T[] m_items;

        public GuidDictionary(IDictionary<Guid, T> source)
        {
            int count = source.Count;
            m_items = new T[count];
            source.Values.CopyTo(m_items, 0);
            m_index=new GuidIndex(source.Keys);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool ContainsKey(Guid key)
        {
            return m_index.FindIndex((byte*)&key) >= 0;
        }

        public bool TryGetValue(Guid key, out T value)
        {
            int index = m_index.FindIndex((byte*)&key);
            if (index < 0)
            {
                value = default(T);
                return false;
            }
            value = m_items[index];
            return true;
        }

        public T this[Guid key]
        {
            get
            {
                int index = m_index.FindIndex((byte*)&key);
                if (index < 0)
                    throw new KeyNotFoundException("Missing");
                return m_items[index];
            }
            set
            {
                throw new NotSupportedException("Cannot modify this dictionary");
            }
        }


    }

    unsafe public class GuidIndex
    {
        struct Entry
        {
            public long GuidLeft;
            public long GuidRight;
            public Entry(byte* id)
            {
                GuidLeft = *(long*)(id);
                GuidRight = *(long*)(id + 8);
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

            public int ComputeHashCode()
            {
                return (int)(GuidLeft ^ GuidRight);
            }
        }

        int m_bucketMask;
        int m_count;
        Entry[] m_entries;
        int[] m_hashLookup;

        public GuidIndex(ICollection<Guid> source)
        {
            //Fill all entries
            m_count = source.Count;
            m_entries = new Entry[m_count];

            int index = 0;
            foreach (var key in source)
            {
                Guid key1 = key;
                m_entries[index] = new Entry((byte*)&key1);
                index++;
            }

            int bucketCount = (int)BitMath.RoundUpToNearestPowerOfTwo((uint)m_count * 4);
            m_hashLookup = new int[bucketCount];
            m_bucketMask = bucketCount - 1;

            RebuildHashLookup();
        }

        void RebuildHashLookup()
        {
            //Clear the hash lookup table.
            for (int x = 0; x < m_hashLookup.Length; x++)
            {
                m_hashLookup[x] = -1;
            }

            //Rebuild it.
            for (int x = 0; x < m_count; x++)
            {
                int hash = m_entries[x].ComputeHashCode() & m_bucketMask;

                while (m_hashLookup[hash] >= 0)
                {
                    hash = (hash + 1) & m_bucketMask;
                }
                m_hashLookup[hash] = x;
            }
        }

        public int FindIndex(byte* key)
        {
            int hash = (int)(*(long*)(key) ^ *(long*)(key + 8)) & m_bucketMask;
            while (m_hashLookup[hash] >= 0)
            {
                if (m_entries[m_hashLookup[hash]].GuidLeft == *(long*)key && m_entries[m_hashLookup[hash]].GuidRight == *(long*)(key + 8))
                {
                    return m_hashLookup[hash];
                }
                hash = (hash + 1) & m_bucketMask;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool ContainsKey(Guid key)
        {
            return FindIndex((byte*)&key) >= 0;
        }

        public bool TryGetValue(Guid key, out int value)
        {
            value = FindIndex((byte*)&key);
            return value >= 0;
        }

        public int this[Guid key]
        {
            get
            {
                int index = FindIndex((byte*)&key);
                if (index < 0)
                    throw new KeyNotFoundException("Missing");
                return index;
            }
            set
            {
                throw new NotSupportedException("Cannot modify this dictionary");
            }
        }
    }
}
