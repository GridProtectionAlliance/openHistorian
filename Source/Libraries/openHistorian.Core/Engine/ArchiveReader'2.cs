//******************************************************************************************************
//  ArchiveReader.cs - Gbtc
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
using GSF.Threading;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian.Engine
{
    internal class ArchiveReader<TKey, TValue>
        : HistorianDataReaderBase<TKey, TValue>
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : class, new()
    {
        private readonly ArchiveList<TKey, TValue> m_list;
        private readonly ArchiveListSnapshot<TKey, TValue> m_snapshot;

        public ArchiveReader(ArchiveList<TKey, TValue> list)
        {
            m_list = list;
            m_snapshot = m_list.CreateNewClientResources();
        }

        public override TreeStream<TKey, TValue> Read(QueryFilterTimestamp key1, QueryFilterPointId key2, DataReaderOptions readerOptions)
        {
            Stats.QueriesExecuted++;
            return new ReadStream(key1, key2, m_snapshot, readerOptions);
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
            private readonly QueryFilterTimestamp m_key1;
            private readonly QueryFilterPointId m_key2;
            private bool m_timedOut;
            private long m_pointCount;

            private TimeoutOperation m_timeout;
            private readonly Queue<KeyValuePair<int, ArchiveTableSummary<TKey, TValue>>> m_tables;

            private int m_currentIndex;
            private ArchiveTableSummary<TKey, TValue> m_currentSummary;
            private ArchiveTableReadSnapshot<TKey, TValue> m_currentInstance;
            private TreeScannerCoreBase<TKey, TValue> m_currentScanner;

            public ReadStream(QueryFilterTimestamp key1, QueryFilterPointId key2, ArchiveListSnapshot<TKey, TValue> snapshot, DataReaderOptions readerOptions)
            {
                if (readerOptions.Timeout.Ticks > 0)
                {
                    m_timeout = new TimeoutOperation();
                    m_timeout.RegisterTimeout(readerOptions.Timeout, () => m_timedOut = true);
                }

                m_key1 = key1;
                m_key2 = key2;
                m_startKey = key1.FirstTime;
                m_stopKey = key1.LastTime;
                m_snapshot = snapshot;
                m_snapshot.UpdateSnapshot();
                TKey startKey = new TKey();
                TKey stopKey = new TKey();

                startKey.SetMin();

                stopKey.SetMax();

                m_tables = new Queue<KeyValuePair<int, ArchiveTableSummary<TKey, TValue>>>();

                for (int x = 0; x < m_snapshot.Tables.Count(); x++)
                {
                    ArchiveTableSummary<TKey, TValue> table = m_snapshot.Tables[x];
                    if (table != null)
                    {
                        startKey.Timestamp = key1.FirstTime;
                        stopKey.Timestamp = key1.LastTime;
                        if (table.Contains(startKey, stopKey))
                        {
                            m_tables.Enqueue(new KeyValuePair<int, ArchiveTableSummary<TKey, TValue>>(x, table));
                        }
                        else
                        {
                            m_snapshot.Tables[x] = null;
                        }
                    }
                }
                prepareNextFile();
            }

            public override bool Read()
            {
                TryAgain:
                if (m_timedOut)
                    Cancel();
                if (m_currentScanner.Read())
                {
                    if (m_currentScanner.CurrentKey.Timestamp <= m_stopKey)
                    {
                        Stats.PointsScanned++;
                        if (m_key2.ContainsKey(m_currentScanner.CurrentKey.PointID))
                        {
                            Stats.PointsReturned++;
                            return true;
                        }
                        goto TryAgain;
                    }

                    if (m_key1.GetNextWindow(out m_startKey, out m_stopKey))
                    {
                        TKey key = new TKey();
                        key.Timestamp = m_startKey;
                        Stats.SeeksRequested++;
                        m_currentScanner.SeekToKey(key);
                        goto TryAgain;
                    }
                }
                if (!prepareNextFile())
                {
                    if (m_timeout != null)
                    {
                        m_timeout.Cancel();
                        m_timeout = null;
                    }
                    return false;
                }
                goto TryAgain;
            }

            private bool prepareNextFile()
            {
                if (m_currentInstance != null)
                {
                    m_currentInstance.Dispose();
                    m_snapshot.Tables[m_currentIndex] = null;
                    m_currentInstance = null;
                }
                if (m_tables.Count > 0)
                {
                    m_key1.Reset();
                    if (!m_key1.GetNextWindow(out m_startKey, out m_stopKey))
                    {
                        throw new Exception("No keys in the list");
                    }

                    KeyValuePair<int, ArchiveTableSummary<TKey, TValue>> kvp = m_tables.Dequeue();
                    m_currentIndex = kvp.Key;
                    m_currentInstance = kvp.Value.ActiveSnapshotInfo.CreateReadSnapshot();
                    m_currentScanner = m_currentInstance.GetTreeScanner();
                    TKey key = new TKey();
                    key.Timestamp = m_startKey;
                    m_currentScanner.SeekToKey(key);
                    SetKeyValueReferences(m_currentScanner.CurrentKey, m_currentScanner.CurrentValue);

                    Stats.SeeksRequested++;
                }
                else
                {
                    m_currentScanner = NullTreeScanner<TKey, TValue>.Instance;
                    SetKeyValueReferences(m_currentScanner.CurrentKey, m_currentScanner.CurrentValue);

                    return false;
                }
                return true;
            }

            public void Cancel()
            {
                if (m_timeout != null)
                {
                    m_timeout.Cancel();
                    m_timeout = null;
                }

                if (m_currentInstance != null)
                {
                    m_currentInstance.Dispose();
                    m_snapshot.Tables[m_currentIndex] = null;
                    m_currentInstance = null;
                }
                m_currentScanner = NullTreeScanner<TKey, TValue>.Instance;
                SetKeyValueReferences(m_currentScanner.CurrentKey, m_currentScanner.CurrentValue);

                while (m_tables.Count > 0)
                {
                    KeyValuePair<int, ArchiveTableSummary<TKey, TValue>> kvp = m_tables.Dequeue();
                    m_snapshot.Tables[kvp.Key] = null;
                }
            }
        }
    }
}