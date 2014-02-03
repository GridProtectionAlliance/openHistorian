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
            private bool m_timedOut;
            private long m_pointCount;

            SortedTreeKeyMethodsBase<TKey> m_keyMethods;
            SortedTreeValueMethodsBase<TValue> m_valueMethods;

            KeySeekFilterBase<TKey> m_keySeekFilter;
            KeyMatchFilterBase<TKey> m_keyMatchFilter;
            ValueMatchFilterBase<TValue> m_valueMatchFilter;

            private TimeoutOperation m_timeout;
            private List<ArchiveTablePointEnumerator<TKey, TValue>> m_tables;
            private EngineUnionSeekableTreeStream<ArchiveTablePointEnumerator<TKey, TValue>, TKey, TValue> m_currentTables;

            public ReadStream(ArchiveListSnapshot<TKey, TValue> snapshot, SortedTreeEngineReaderOptions readerOptions, 
                                       KeySeekFilterBase<TKey> keySeekFilter, KeyMatchFilterBase<TKey> keyMatchFilter, 
                                       ValueMatchFilterBase<TValue> valueMatchFilter)
            {
                m_keyMethods = new TKey().CreateKeyMethods();
                m_valueMethods = new TValue().CreateValueMethods();
                m_keySeekFilter = keySeekFilter;
                m_keyMatchFilter = keyMatchFilter;
                m_valueMatchFilter = valueMatchFilter;

                if (readerOptions.Timeout.Ticks > 0)
                {
                    m_timeout = new TimeoutOperation();
                    m_timeout.RegisterTimeout(readerOptions.Timeout, () => m_timedOut = true);
                }

                //m_timestampFilter = timestampFilter;
                //m_poingIdFilter = poingIdFilter;
                //m_isKey2Universal = poingIdFilter.IsUniverseFilter;
                //m_startKey = timestampFilter.FirstTime;
                //m_stopKey = timestampFilter.LastTime;
                m_snapshot = snapshot;
                m_snapshot.UpdateSnapshot();

                //startKey.SetMin();
                //stopKey.SetMax();

                m_tables = new List<ArchiveTablePointEnumerator<TKey, TValue>>();

                for (int x = 0; x < m_snapshot.Tables.Count(); x++)
                {
                    ArchiveTableSummary<TKey, TValue> table = m_snapshot.Tables[x];
                    if (table != null)
                    {
                        if (keySeekFilter == null || table.Contains(keySeekFilter.StartOfRange, keySeekFilter.EndOfRange))
                        {
                            m_tables.Add(new ArchiveTablePointEnumerator<TKey, TValue>(x, table));
                        }
                        else
                        {
                            m_snapshot.Tables[x] = null;
                        }
                    }
                }

                m_currentTables = new EngineUnionSeekableTreeStream<ArchiveTablePointEnumerator<TKey, TValue>, TKey, TValue>(m_tables);
                SetKeyValueReferences(m_currentTables.CurrentKey, m_currentTables.CurrentValue);

                m_keySeekFilter.Reset();
                if (m_keySeekFilter.NextWindow())
                {
                    m_startKey = m_keySeekFilter.StartOfFrame.Timestamp;
                    m_stopKey = m_keySeekFilter.EndOfFrame.Timestamp;
                    m_currentTables.SeekToKey(m_keySeekFilter.StartOfFrame);
                }
                else
                {
                    Cancel();
                }
            }

            bool AdvanceTimestampFilter()
            {
            TryAgain:
                if (m_keySeekFilter != null && m_keySeekFilter.NextWindow())
                {
                    m_startKey = m_keySeekFilter.StartOfFrame.Timestamp;
                    m_stopKey = m_keySeekFilter.EndOfFrame.Timestamp;

                    //If the current point is a valid point.
                    if (m_currentTables.IsValid)
                    {
                        //If the current point is within this window
                        if (m_keyMethods.IsGreaterThanOrEqualTo(m_currentTables.CurrentKey, m_keySeekFilter.StartOfFrame) &&
                            m_keyMethods.IsLessThanOrEqualTo(m_currentTables.CurrentKey, m_keySeekFilter.EndOfFrame))
                        {
                            IsValid = true;
                            return true;
                        }

                        //If the current point is after this window, see to the next window.
                        if (m_keyMethods.IsGreaterThan(m_currentTables.CurrentKey, m_keySeekFilter.EndOfFrame))
                            goto TryAgain;
                    }

                    //If the current point is not valid, or is before m_startKey
                    //Advance the scanner to the next window.
                    TKey key = new TKey();
                    m_keyMethods.SetMin(key); //key.SetMin();
                    key.Timestamp = m_startKey;
                    m_currentTables.SeekForward(key);

                    //If the current point is not valid, then make it valid.
                    if (!m_currentTables.IsValid)
                    {
                        if (!m_currentTables.Read())
                        {
                            //The read was unsuccessful, end of the stream encountered.
                            Cancel();
                            return false;
                        }
                    }

                    //If the current point is within this window
                    if (m_currentTables.CurrentKey.Timestamp >= m_startKey &&
                        m_currentTables.CurrentKey.Timestamp <= m_stopKey)
                    {
                        IsValid = true;
                        return true;
                    }

                    //If the current point is after this window, see to the next window.
                    if (m_currentTables.CurrentKey.Timestamp > m_stopKey)
                        goto TryAgain;

                    throw new Exception("The seek behavior of the stream did not function properly");
                }
                Cancel();
                return false;
            }

            public override bool Read()
            {
            TryAgain:
                if (m_timedOut)
                    Cancel();
                else
                {
                    if (m_currentTables.Read() && m_currentTables.CurrentKey.Timestamp <= m_stopKey)
                    {
                        Stats.PointsScanned++;
                        if (m_keyMatchFilter == null || m_keyMatchFilter.Contains(CurrentKey))
                        {
                            Stats.PointsReturned++;
                            IsValid = true;
                            return true;
                        }
                        goto TryAgain;
                    }
                    if (AdvanceTimestampFilter())
                    {
                        IsValid = true;
                        return true;
                    }
                }
                IsValid = false;
                return false;
            }

            public override void Cancel()
            {
                if (m_timeout != null)
                {
                    m_timeout.Cancel();
                    m_timeout = null;
                }

                if (m_tables != null)
                {
                    m_tables.ForEach(x => x.Dispose());
                    m_tables = null;
                    Array.Clear(m_snapshot.Tables, 0, m_snapshot.Tables.Length);
                }
                IsValid = false;
                m_timedOut = true;
            }
        }
    }
}