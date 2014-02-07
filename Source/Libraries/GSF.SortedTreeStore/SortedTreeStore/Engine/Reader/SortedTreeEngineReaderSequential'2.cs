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
using GSF.SortedTreeStore.Tree.TreeNodes;
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
            SortedTreeValueMethodsBase<TValue> m_valueMethods;

            KeySeekFilterBase<TKey> m_keySeekFilter;
            KeyMatchFilterBase<TKey> m_keyMatchFilter;
            ValueMatchFilterBase<TValue> m_valueMatchFilter;

            private TimeoutOperation m_timeout;
            private List<BufferedArchiveStream<TKey, TValue>> m_tables;
            private UnionArchive<TKey, TValue> m_currentTables;

            public ReadStream(ArchiveListSnapshot<TKey, TValue> snapshot, SortedTreeEngineReaderOptions readerOptions,
                                       KeySeekFilterBase<TKey> keySeekFilter, KeyMatchFilterBase<TKey> keyMatchFilter,
                                       ValueMatchFilterBase<TValue> valueMatchFilter)
            {
                m_keyMethods = new TKey().CreateKeyMethods();
                m_valueMethods = new TValue().CreateValueMethods();
                m_keySeekFilter = keySeekFilter;
                m_keyMatchFilter = keyMatchFilter;
                m_valueMatchFilter = valueMatchFilter;
                m_keyMatchIsUniverse = (m_keyMatchFilter as KeyMatchFilterUniverse<TKey>) != null;

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

                m_tables = new List<BufferedArchiveStream<TKey, TValue>>();

                for (int x = 0; x < m_snapshot.Tables.Count(); x++)
                {
                    ArchiveTableSummary<TKey, TValue> table = m_snapshot.Tables[x];
                    if (table != null)
                    {
                        if (keySeekFilter == null || table.Contains(keySeekFilter.StartOfRange, keySeekFilter.EndOfRange))
                        {
                            m_tables.Add(new BufferedArchiveStream<TKey, TValue>(x, table));
                        }
                        else
                        {
                            m_snapshot.Tables[x] = null;
                        }
                    }
                }

                m_currentTables = new UnionArchive<TKey, TValue>(m_tables);

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

            public override bool Read(TKey key, TValue value)
            {
                bool isValid;
            TryAgain:
                if (m_timedOut)
                {
                    Cancel();
                }
                else
                {
                    if (m_keyMatchIsUniverse)
                    {
                        isValid = m_currentTables.Read(key, value);
                        if (isValid && key.Timestamp <= m_stopKey)
                        {
                            Stats.PointsScanned++;
                            Stats.PointsReturned++;
                            return true;
                        }
                    }
                    else
                    {
                        isValid = m_currentTables.Read(key, value, m_keyMatchFilter);
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
                    m_currentTables.SeekForward(tmpKey);
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

                if (m_tables != null)
                {
                    m_tables.ForEach(x => x.Dispose());
                    m_tables = null;
                    Array.Clear(m_snapshot.Tables, 0, m_snapshot.Tables.Length);
                }
                m_timedOut = true;
            }
        }
    }
}