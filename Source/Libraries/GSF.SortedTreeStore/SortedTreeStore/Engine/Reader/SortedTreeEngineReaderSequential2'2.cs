//******************************************************************************************************
//  SortedTreeEngineReaderSequential'2.cs - Gbtc
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
//  10/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using GSF.SortedTreeStore.Filters;
using GSF.Threading;
using openHistorian;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Reader
{
    /// <summary>
    /// A <see cref="SortedTreeEngineReaderBase{TKey,TValue}"/> that can read from a <see cref="ArchiveList{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class SortedTreeEngineReaderSequential<TKey, TValue>
        : SortedTreeEngineReaderBase<TKey, TValue>
        where TKey : EngineKeyBase<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        private readonly ArchiveList<TKey, TValue> m_list;
        private readonly ArchiveListSnapshot<TKey, TValue> m_snapshot;

        public SortedTreeEngineReaderSequential(ArchiveList<TKey, TValue> list)
        {
            m_list = list;
            m_snapshot = m_list.CreateNewClientResources();
        }

        ///// <summary>
        ///// Reads data from the historian with the provided filters.
        ///// </summary>
        ///// <param name="timestampFilter">filters for the timestamp</param>
        ///// <param name="pointIdFilter">filters for the pointId</param>
        ///// <param name="readerOptions">options for the reader, such as automatic timeouts.</param>
        ///// <returns></returns>
        //public override TreeStream<TKey, TValue> Read(QueryFilterTimestamp timestampFilter, QueryFilterPointId pointIdFilter, SortedTreeEngineReaderOptions readerOptions)
        //{
        //    Stats.QueriesExecuted++;
        //    return new ReadStream(timestampFilter, pointIdFilter, m_snapshot, readerOptions);
        //}

        public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, KeySeekFilterBase<TKey> keySeekFilter,
                                        KeyMatchFilterBase<TKey> keyMatchFilter, ValueMatchFilterBase<TValue> valueMatchFilter)
        {
            if (readerOptions == null)
                readerOptions = SortedTreeEngineReaderOptions.Default;
            if (keySeekFilter == null)
                keySeekFilter = new KeySeekFilterUniverse<TKey>();
            if (keyMatchFilter == null)
                keyMatchFilter = new KeyMatchFilterUniverse<TKey>();
            if (valueMatchFilter == null)
                valueMatchFilter = new ValueMatchFilterUniverse<TValue>();

            Stats.QueriesExecuted++;
            return new ReadStream(m_snapshot, readerOptions, keySeekFilter, keyMatchFilter, valueMatchFilter);
        }

        /// <summary>
        /// Closes the current reader.
        /// </summary>
        public override void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            m_snapshot.Dispose();
        }

        private class ReadStream
            : TreeStream<TKey, TValue>
        {
            private readonly ArchiveListSnapshot<TKey, TValue> m_snapshot;
            private ulong m_startKey;
            private ulong m_stopKey;
            private volatile bool m_timedOut;
            private long m_pointCount;

            bool m_keyMatchIsUniverse;
            SortedTreeKeyMethodsBase<TKey> m_keyMethods;

            KeySeekFilterBase<TKey> m_keySeekFilter;
            KeyMatchFilterBase<TKey> m_keyMatchFilter;
            ValueMatchFilterBase<TValue> m_valueMatchFilter;

            private TimeoutOperation m_timeout;
            private List<BufferedArchiveStream<TKey, TValue>> m_tablesOrigList;

            public ReadStream(ArchiveListSnapshot<TKey, TValue> snapshot, SortedTreeEngineReaderOptions readerOptions,
                                       KeySeekFilterBase<TKey> keySeekFilter, KeyMatchFilterBase<TKey> keyMatchFilter,
                                       ValueMatchFilterBase<TValue> valueMatchFilter)
            {
                m_keyMethods = new TKey().CreateKeyMethods();
                m_keySeekFilter = keySeekFilter;
                m_keyMatchFilter = keyMatchFilter;
                m_valueMatchFilter = valueMatchFilter;
                m_keyMatchIsUniverse = (m_keyMatchFilter as KeyMatchFilterUniverse<TKey>) != null;

                if (readerOptions.Timeout.Ticks > 0)
                {
                    m_timeout = new TimeoutOperation();
                    m_timeout.RegisterTimeout(readerOptions.Timeout, () => m_timedOut = true);
                }

                m_snapshot = snapshot;
                m_snapshot.UpdateSnapshot();

                m_tablesOrigList = new List<BufferedArchiveStream<TKey, TValue>>();

                for (int x = 0; x < m_snapshot.Tables.Count(); x++)
                {
                    ArchiveTableSummary<TKey, TValue> table = m_snapshot.Tables[x];
                    if (table != null)
                    {
                        if (keySeekFilter == null || table.Contains(keySeekFilter.StartOfRange, keySeekFilter.EndOfRange))
                        {
                            m_tablesOrigList.Add(new BufferedArchiveStream<TKey, TValue>(x, table));
                        }
                        else
                        {
                            m_snapshot.Tables[x] = null;
                        }
                    }
                }

                UnionArchive2(m_tablesOrigList);

                m_keySeekFilter.Reset();
                if (m_keySeekFilter.NextWindow())
                {
                    m_startKey = m_keySeekFilter.StartOfFrame.Timestamp;
                    m_stopKey = m_keySeekFilter.EndOfFrame.Timestamp;
                    SeekToKey(m_keySeekFilter.StartOfFrame);
                }
                else
                {
                    Cancel();
                }
            }

            public override bool Read(TKey key, TValue value)
            {
                if (!m_timedOut &&
                    m_keyMatchIsUniverse &&
                    m_firstTable != null &&
                    m_firstTable.Scanner.ReadUntil(key, value, m_nextTableKey))
                {
                    if (key.Timestamp <= m_stopKey || AdvanceTimestampFilter(true, key, value))
                    {
                        Stats.PointsScanned++;
                        Stats.PointsReturned++;
                        return true;
                    }
                }
                return Read2(key, value);
            }

            bool Read2(TKey key, TValue value)
            {
                bool isValid;
            TryAgain:
                if (!m_timedOut)
                {
                    if (m_keyMatchIsUniverse)
                    {
                        if (m_firstTable != null)
                        {
                            if (m_firstTable.Scanner.ReadUntil(key, value, m_nextTableKey))
                            {
                                isValid = true;
                            }
                            else
                            {
                                isValid = ReadUnion2(key, value);
                            }
                        }
                        else
                        {
                            isValid = false;
                        }

                        //isValid = ReadUnion(key, value);
                        if (isValid && key.Timestamp <= m_stopKey)
                        {
                            Stats.PointsScanned++;
                            Stats.PointsReturned++;
                            return true;
                        }
                    }
                    else
                    {
                        isValid = ReadUnionFilter(key, value, m_keyMatchFilter);
                        if (isValid && key.Timestamp <= m_stopKey)
                        {
                            Stats.PointsScanned++;
                            if (m_keyMatchFilter.Contains(key))
                            {
                                Stats.PointsReturned++;
                                return true;
                            }
                            goto TryAgain;
                        }
                    }
                    if (isValid)
                    {
                        if (AdvanceTimestampFilter(isValid, key, value))
                        {
                            return true;
                        }
                        goto TryAgain;
                    }
                }
                Cancel();
                return false;
            }



            /// <summary>
            /// Does a seek operation on the current stream when there is a seek filter on the reader.
            /// </summary>
            /// <returns></returns>
            bool AdvanceTimestampFilter(bool isValid, TKey key, TValue value)
            {
            TryAgain:
                if (m_keySeekFilter != null && m_keySeekFilter.NextWindow())
                {
                    m_startKey = m_keySeekFilter.StartOfFrame.Timestamp;
                    m_stopKey = m_keySeekFilter.EndOfFrame.Timestamp;

                    //If the current point is a valid point.
                    if (isValid)
                    {
                        //If the current point is within this window
                        if (m_keyMethods.IsGreaterThanOrEqualTo(key, m_keySeekFilter.StartOfFrame) &&
                            m_keyMethods.IsLessThanOrEqualTo(key, m_keySeekFilter.EndOfFrame))
                        {
                            return true;
                        }

                        //If the current point is after this window, see to the next window.
                        if (m_keyMethods.IsGreaterThan(key, m_keySeekFilter.EndOfFrame))
                            goto TryAgain;
                    }

                    //If the current point is not valid, or is before m_startKey
                    //Advance the scanner to the next window.
                    TKey tmpKey = new TKey();
                    m_keyMethods.SetMin(tmpKey); //key.SetMin();
                    tmpKey.Timestamp = m_startKey;
                    SeekForward(tmpKey);
                }
                return false;

            }

            public override void Cancel()
            {
                EOS = true;
                if (m_timeout != null)
                {
                    m_timeout.Cancel();
                    m_timeout = null;
                }

                if (m_tablesOrigList != null)
                {
                    m_tablesOrigList.ForEach(x => x.Dispose());
                    m_tablesOrigList = null;
                    Array.Clear(m_snapshot.Tables, 0, m_snapshot.Tables.Length);
                }
                m_timedOut = true;
            }


            //------------------------------------------------------------------
            //------------------------------------------------------------------
            //------------------------------------------------------------------
            //------------------------------------------------------------------
            //------------------------------------------------------------------


            CustomSortHelper<BufferedArchiveStream<TKey, TValue>> m_tables;
            BufferedArchiveStream<TKey, TValue> m_firstTable;
            TKey m_nextTableKey = new TKey();

            void UnionArchive2(IEnumerable<BufferedArchiveStream<TKey, TValue>> list)
            {
                m_tables = new CustomSortHelper<BufferedArchiveStream<TKey, TValue>>(list, CompareStreams);
            }

            int CompareStreams(BufferedArchiveStream<TKey, TValue> item1, BufferedArchiveStream<TKey, TValue> item2)
            {
                if (!item1.SortByIsValid && !item2.SortByIsValid)
                    return 0;
                if (!item1.SortByIsValid)
                    return 1;
                if (!item2.SortByIsValid)
                    return -1;
                return m_keyMethods.CompareTo(item1.SortByKey, item2.SortByKey);// item1.CurrentKey.CompareTo(item2.CurrentKey);
            }

            bool ReadUnion(TKey key, TValue value)
            {
                if (m_firstTable != null)
                {
                    if (!m_firstTable.Scanner.ReadUntil(key, value, m_nextTableKey))
                    {
                        return ReadUnion2(key, value);
                    }
                    return true;
                }
                return false;
            }

            bool ReadUnion2(TKey key, TValue value)
            {
                //Set the first table's cache to the correct value
                m_firstTable.SortByIsValid = m_firstTable.Scanner.Peek(m_firstTable.SortByKey, m_firstTable.SortByValue);

                if (m_tables.Items.Length > 1)
                {
                    //If list is no longer in order
                    int compare = CompareStreams(m_firstTable, m_tables[1]);
                    if (compare == 0 && m_firstTable.SortByIsValid)
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
                return m_firstTable.Scanner.ReadUntil(key, value, m_nextTableKey);
            }

            bool ReadUnionFilter(TKey key, TValue value, KeyMatchFilterBase<TKey> filter)
            {
                if (m_firstTable != null)
                {
                    if (!m_firstTable.Scanner.ReadUntil(key, value, m_nextTableKey, filter))
                    {
                        return ReadUnionFilter2(key, value, filter);
                    }
                    return true;
                }
                return false;
            }

            bool ReadUnionFilter2(TKey key, TValue value, KeyMatchFilterBase<TKey> filter)
            {
            TryAgain:

                //Set the first table's cache to the correct value
                m_firstTable.SortByIsValid = m_firstTable.Scanner.Peek(m_firstTable.SortByKey, m_firstTable.SortByValue);

                if (m_tables.Items.Length > 1)
                {
                    //If list is no longer in order
                    int compare = CompareStreams(m_firstTable, m_tables[1]);
                    if (compare == 0 && m_firstTable.SortByIsValid)
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
                    if (compare == 0 && !m_firstTable.SortByIsValid)
                    {
                        EOS = true;
                        return false;
                    }
                }
                else
                {
                    if (!m_firstTable.SortByIsValid)
                    {
                        EOS = true;
                        return false;
                    }
                }

                if (m_firstTable.Scanner.ReadUntil(key, value, m_nextTableKey, filter))
                {
                    return true;
                }
                goto TryAgain;
            }

            void SeekToKey(TKey key)
            {
                foreach (var table in m_tables.Items)
                {
                    table.Scanner.SeekToKey(key);
                    table.SortByIsValid = table.Scanner.Peek(table.SortByKey, table.SortByValue);
                }
                m_tables.Sort();

                //Remove any duplicates
                if (m_tables.Items.Length >= 2)
                {
                    if (CompareStreams(m_tables[0], m_tables[1]) == 0 && m_tables[0].SortByIsValid)
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
            void SeekForward(TKey key)
            {
                foreach (var table in m_tables.Items)
                {
                    if (table.SortByIsValid && m_keyMethods.IsLessThan(table.SortByKey, key)) // table.CurrentKey.IsLessThan(key))
                    {
                        table.Scanner.SeekToKey(key);
                        table.SortByIsValid = table.Scanner.Peek(table.SortByKey, table.SortByValue);
                    }
                    //ToDo: Consider commenting out this debug code.
                    if (table.SortByIsValid && m_keyMethods.IsLessThan(table.SortByKey, key)) // table.CurrentKey.IsLessThan(key))
                    {
                        throw new Exception("should never occur");
                    }
                }
                m_tables.Sort();

                //Remove any duplicates
                if (m_tables.Items.Length >= 2)
                {
                    if (CompareStreams(m_tables[0], m_tables[1]) == 0 && m_tables[0].SortByIsValid)
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
                        m_tables[index].SortByIsValid = m_tables[index].Scanner.Read(m_tables[index].SortByKey, m_tables[index].SortByValue);
                        m_tables[index].SortByIsValid = m_tables[index].Scanner.Peek(m_tables[index].SortByKey, m_tables[index].SortByValue);
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

            void SetCacheValue()
            {
                if (m_tables.Items.Length > 1 && m_tables[1].SortByIsValid)
                {
                    m_keyMethods.Copy(m_tables[1].SortByKey, m_nextTableKey);
                }
                else
                {
                    m_keyMethods.SetMax(m_nextTableKey);
                }
            }

        }
    }
}