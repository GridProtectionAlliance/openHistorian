////******************************************************************************************************
////  GuidIndex.cs - Gbtc
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
//    public unsafe class IntDictionary<T>
//    {
//        IntIndex m_index;
//        T[] m_items;

//        public IntDictionary(IDictionary<int, T> source)
//        {
//            int count = source.Count;
//            m_items = new T[count];
//            source.Values.CopyTo(m_items, 0);
//            m_index = new IntIndex(source.Keys);
//        }

//        [MethodImpl(MethodImplOptions.NoInlining)]
//        public bool ContainsKey(int key)
//        {
//            return m_index.FindIndex(key) >= 0;
//        }

//        public bool TryGetValue(int key, out T value)
//        {
//            int index = m_index.FindIndex(key);
//            if (index < 0)
//            {
//                value = default(T);
//                return false;
//            }
//            value = m_items[index];
//            return true;
//        }

//        public T this[int key]
//        {
//            get
//            {
//                int index = m_index.FindIndex(key);
//                if (index < 0)
//                    throw new KeyNotFoundException("Missing");
//                return m_items[index];
//            }
//            set
//            {
//                throw new NotSupportedException("Cannot modify this dictionary");
//            }
//        }
//    }

//    unsafe public class IntIndex
//    {
//        int m_bucketMask;
//        int m_count;
//        int[] m_entries;
//        int[] m_hashLookup;

//        public IntIndex(ICollection<int> source)
//        {
//            //Fill all entries
//            m_count = source.Count;
//            m_entries = new int[m_count];

//            int index = 0;
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
//                m_hashLookup[x] = -1;
//            }

//            //Rebuild it.
//            for (int x = 0; x < m_count; x++)
//            {
//                int hash = m_entries[x] & m_bucketMask;
//                while (m_hashLookup[hash] >= 0)
//                {
//                    hash = (hash + 1) & m_bucketMask;
//                }
//                m_hashLookup[hash] = x;
//            }
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public int FindIndex(int key)
//        {
//            int index = m_hashLookup[key & m_bucketMask];
//            if (index >= 0 && m_entries[index] == key)
//            {
//                return index;
//            }
//            return FindSlower(key);
//        }

//        int FindSlower(int key)
//        {
//            int hash = key & m_bucketMask;
//            int index = m_hashLookup[hash];
//            while (index >= 0)
//            {
//                if (m_entries[index] == key)
//                {
//                    return index;
//                }
//                hash = (hash + 1) & m_bucketMask;
//                index = m_hashLookup[hash];
//            }
//            return -1;
//        }

//        public bool ContainsKey(int key)
//        {
//            return FindIndex(key) >= 0;
//        }

//        public bool TryGetValue(int key, out int value)
//        {
//            value = FindIndex(key);
//            return value >= 0;
//        }

//        public int this[int key]
//        {
//            get
//            {
//                int index = FindIndex(key);
//                if (index < 0)
//                    throw new KeyNotFoundException("Missing");
//                return index;
//            }
//            set
//            {
//                throw new NotSupportedException("Cannot modify this dictionary");
//            }
//        }
//    }
//}
