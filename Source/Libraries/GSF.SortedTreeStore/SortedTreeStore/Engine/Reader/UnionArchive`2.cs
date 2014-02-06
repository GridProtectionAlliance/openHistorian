//******************************************************************************************************
//  UnionArchive'2.cs - Gbtc
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
//  10/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.SortedTreeStore.Engine;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Reader
{
    /// <summary>
    /// Creates a <see cref="SeekableTreeStream{TKey,TValue}"/> that is the union of a collection of 
    /// other SeekableKeyValueStream.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class UnionArchive<TKey, TValue>
        where TKey : EngineKeyBase<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        CustomSortHelper<BufferedArchiveStream<TKey,TValue>> m_tables;
        BufferedArchiveStream<TKey, TValue> m_firstTable;
        SortedTreeKeyMethodsBase<TKey> m_keyMethods;
        SortedTreeValueMethodsBase<TValue> m_valueMethods;

        ulong m_nextTime;

        public UnionArchive(IEnumerable<BufferedArchiveStream<TKey, TValue>> list)
        {
            m_keyMethods = new TKey().CreateKeyMethods();
            m_valueMethods = new TValue().CreateValueMethods();
            m_tables = new CustomSortHelper<BufferedArchiveStream<TKey, TValue>>(list, CompareStreams);
        }

        int CompareStreams(BufferedArchiveStream<TKey, TValue> item1, BufferedArchiveStream<TKey, TValue> item2)
        {
            if (!item1.IsValid && !item2.IsValid)
                return 0;
            if (!item1.IsValid)
                return 1;
            if (!item2.IsValid)
                return -1;
            return m_keyMethods.CompareTo(item1.CurrentKey, item2.CurrentKey);// item1.CurrentKey.CompareTo(item2.CurrentKey);
        }

        void VerifyOrder()
        {
            //If list is no longer in order
            int compare = CompareStreams(m_firstTable, m_tables[1]);
            if (compare == 0 && m_firstTable.IsValid)
            {
                //If a duplicate entry is found, advance the position of the duplicate entry
                RemoveDuplicatesFromList();
                SetCacheValue();
            }
            if (compare > 0)
            {
                m_tables.SortAssumingIncreased(0);
                m_firstTable = m_tables[0];
                SetCacheValue();
            }
        }

        void SetCacheValue()
        {
            if (m_tables.Items.Length > 1 && m_tables[1].IsValid)
            {
                m_nextTime = m_tables[1].CurrentKey.Timestamp;
            }
            else
            {
                m_nextTime = ulong.MaxValue;
            }
        }

        public bool Read(TKey key, TValue value)
        {
            var firstTable = m_firstTable;
            if (firstTable == null || !firstTable.IsValid)
            {
                return false;
            }
            else
            {
                m_keyMethods.Copy(firstTable.CurrentKey, key);
                m_valueMethods.Copy(firstTable.CurrentValue, value);

                firstTable.Read();

                if (m_tables.Items.Length > 1)
                {
                    if (!firstTable.IsValid || firstTable.CurrentKey.Timestamp >= m_nextTime) //A 99% check
                    {
                        VerifyOrder();
                    }
                }
                return true;
            }
        }

        public bool Read(TKey key, TValue value, KeyMatchFilterBase<TKey> filter)
        {
            var firstTable = m_firstTable;
            if (firstTable == null || !firstTable.IsValid)
            {
                return false;
            }
            else
            {
                m_keyMethods.Copy(firstTable.CurrentKey, key);
                m_valueMethods.Copy(firstTable.CurrentValue, value);

                firstTable.Read(filter);

                if (m_tables.Items.Length > 1)
                {
                    if (!firstTable.IsValid || firstTable.CurrentKey.Timestamp >= m_nextTime) //A 99% check
                    {
                        VerifyOrder();
                    }
                }
                return true;
            }
        }

        public void SeekToKey(TKey key)
        {
            foreach (var table in m_tables.Items)
            {
                table.SeekToKey(key);
                table.Read();
            }
            m_tables.Sort();

            //Remove any duplicates
            if (m_tables.Items.Length >= 2)
            {
                if (CompareStreams(m_tables[0], m_tables[1]) == 0 && m_tables[0].IsValid)
                {
                    //If a duplicate entry is found, advance the position of the duplicate entry
                    RemoveDuplicatesFromList();
                }
            }

            if (m_tables.Items.Length > 0)
                m_firstTable = m_tables[0];

            SetCacheValue();
        }

        /// <summary>
        /// Seeks the streams only in the forward direction.
        /// This means that if the current position in any stream is invalid or past this point,
        /// the stream will not seek backwards.
        /// After returning, the <see cref="TreeStream{TKey,TValue}.CurrentKey"/> 
        /// and <see cref="TreeStream{TKey,TValue}.CurrentValue"/> will only be valid
        /// if it's position is greater then or equal to <see cref="key"/>.
        /// Bug Consideration: When seeking forward, Don't forget to check <see cref="TreeStream{TKey,TValue}.IsValid"/> to see if the first
        /// sample point in this list is still valid. If not, you will accidentially skip the first sample point.
        /// </summary>
        /// <param name="key"></param>
        public void SeekForward(TKey key)
        {
            foreach (var table in m_tables.Items)
            {
                if (table.IsValid && m_keyMethods.IsLessThan(table.CurrentKey, key)) // table.CurrentKey.IsLessThan(key))
                {
                    table.SeekToKey(key);
                    table.Read();
                }
                //ToDo: Consider commenting out this debug code.
                if (table.IsValid && m_keyMethods.IsLessThan(table.CurrentKey, key)) // table.CurrentKey.IsLessThan(key))
                {
                    table.SeekToKey(key);
                    table.Read();
                    throw new Exception("should never occur");
                }
            }
            m_tables.Sort();

            //Remove any duplicates
            if (m_tables.Items.Length >= 2)
            {
                if (CompareStreams(m_tables[0], m_tables[1]) == 0 && m_tables[0].IsValid)
                {
                    //If a duplicate entry is found, advance the position of the duplicate entry
                    RemoveDuplicatesFromList();
                }
            }

            if (m_tables.Items.Length > 0)
                m_firstTable = m_tables[0];

            SetCacheValue();
        }

        void RemoveDuplicatesFromList()
        {
            for (int index = 1; index < m_tables.Items.Length; index++)
            {
                if (CompareStreams(m_tables[0], m_tables[index]) == 0)
                {
                    m_tables[index].Read();
                }
                else
                {
                    for (int j = index; j > 0; j--)
                        m_tables.SortAssumingIncreased(j);
                    break;
                }
            }

            SetCacheValue();
            //m_tables.Sort();
        }

    }
}
