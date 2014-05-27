////******************************************************************************************************
////  Index.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
////  4/24/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;

//namespace GSF.Collections
//{
//    public class FastDictionary<TKey, TValue>
//        where TKey : IEquatable<TKey>
//    {
//        TValue[] m_values;
//        TKey[] m_keys;
//        int[] m_hashLookup;
//        int m_bucketMask;
//        int m_count;

//        public FastDictionary(IDictionary<TKey, TValue> source)
//        {
//            m_count = source.Count;

//            //Make a copy of the key value collection
//            m_keys = new TKey[m_count + 1];
//            m_keys[0] = default(TKey);
//            m_values = new TValue[m_count + 1];
//            m_values[0] = default(TValue);

//            int index = 1;
//            foreach (var kvp in source)
//            {
//                m_keys[index] = kvp.Key;
//                m_values[index] = kvp.Value;
//                index++;
//            }

//            int bucketCount = (int)BitMath.RoundUpToNearestPowerOfTwo((uint)m_count * 2);
//            m_hashLookup = new int[bucketCount];
//            m_bucketMask = bucketCount - 1;

//            RebuildHashLookup();

//        }

//        void RebuildHashLookup()
//        {
//            //Clear the hash lookup table.
//            for (int x = 0; x < m_hashLookup.Length; x++)
//            {
//                m_hashLookup[x] = 0;
//            }

//            //Rebuild it.
//            for (int x = 1; x <= m_count; x++)
//            {
//                int hash = m_keys[x].GetHashCode() & m_bucketMask;
//                while (m_hashLookup[hash] > 0)
//                {
//                    hash = (hash + 1) & m_bucketMask;
//                }
//                m_hashLookup[hash] = x;
//            }
//        }


//        [MethodImpl(MethodImplOptions.NoInlining)]
//        public bool ContainsKey(TKey key)
//        {
//            int hash = key.GetHashCode() & m_bucketMask;
//            int index = m_hashLookup[hash];
//            while (index > 0)
//            {
//                if (key.Equals(m_keys[index]))
//                    return true;
//                hash = (hash + 1) & m_bucketMask;
//                index = m_hashLookup[hash];
//            }
//            return false;
//        }

//        [MethodImpl(MethodImplOptions.NoInlining)]
//        public bool TryGetValue(TKey key, out TValue value)
//        {
//            int hash = key.GetHashCode() & m_bucketMask;
//            int index = m_hashLookup[hash];
//            while (index > 0)
//            {
//                if (key.Equals(m_keys[index]))
//                {
//                    value = m_values[index];
//                    return true;
//                }
//                hash = (hash + 1) & m_bucketMask;
//                index = m_hashLookup[hash];
//            }
//            value = m_values[index];
//            return false;
//        }

//        public TValue this[TKey key]
//        {
//            get
//            {
//                int hash = key.GetHashCode() & m_bucketMask;
//                int index = m_hashLookup[hash];
//                while (index > 0)
//                {
//                    if (key.Equals(m_keys[index]))
//                    {
//                        return m_values[index];
//                    }
//                    hash = (hash + 1) & m_bucketMask;
//                    index = m_hashLookup[hash];
//                }
//                throw new KeyNotFoundException();
//            }
//            set
//            {
//                throw new NotSupportedException("Cannot modify this dictionary");
//            }
//        }
//    }

//    public class Index<T>
//        where T : IEquatable<T>
//    {
//        int m_bucketMask;
//        int m_count;
//        T[] m_entries;
//        int[] m_hashLookup;

//        public Index(ICollection<T> source)
//        {
//            //Fill all entries
//            m_count = source.Count;
//            m_entries = new T[m_count + 1];

//            m_entries[0] = default(T);
//            int index = 1;
//            foreach (var key in source)
//            {
//                m_entries[index] = key;
//                index++;
//            }

//            int bucketCount = (int)BitMath.RoundUpToNearestPowerOfTwo((uint)m_count * 4);
//            m_hashLookup = new int[bucketCount];
//            m_bucketMask = bucketCount - 1;

//            RebuildHashLookup();
//        }

//        void RebuildHashLookup()
//        {
//            //Clear the hash lookup table.
//            for (int x = 0; x < m_hashLookup.Length; x++)
//            {
//                m_hashLookup[x] = 0;
//            }

//            //Rebuild it.
//            for (int x = 1; x <= m_count; x++)
//            {
//                int hash = m_entries[x].GetHashCode() & m_bucketMask;
//                while (m_hashLookup[hash] > 0)
//                {
//                    hash = (hash + 1) & m_bucketMask;
//                }
//                m_hashLookup[hash] = x;
//            }
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public int FindIndex(T key)
//        {
//            int index = m_hashLookup[key.GetHashCode() & m_bucketMask];
//            if (index > 0 && m_entries[index].Equals(key))
//            {
//                return index;
//            }
//            return FindSlower(key);
//        }

//        int FindSlower(T key)
//        {
//            int hash = key.GetHashCode() & m_bucketMask;
//            int index = m_hashLookup[hash];
//            while (index > 0)
//            {
//                if (m_entries[index].Equals(key))
//                {
//                    return index;
//                }
//                hash = (hash + 1) & m_bucketMask;
//                index = m_hashLookup[hash];
//            }
//            return 0;
//        }

//        public bool ContainsKey(T key)
//        {
//            return FindIndex(key) > 0;
//        }

//        public bool TryGetValue(T key, out int value)
//        {
//            value = FindIndex(key) - 1;
//            return value >= 0;
//        }

//        public int this[T key]
//        {
//            get
//            {
//                int index = FindIndex(key);
//                if (index <= 0)
//                    throw new KeyNotFoundException("Missing");
//                return index - 1;
//            }
//            set
//            {
//                throw new NotSupportedException("Cannot modify this dictionary");
//            }
//        }
//    }
//}
