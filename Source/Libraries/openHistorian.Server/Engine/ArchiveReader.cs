//******************************************************************************************************
//  ArchiveReader.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
using openHistorian.Collections.KeyValue;
using openHistorian.Archive;

namespace openHistorian.Engine
{
    internal class ArchiveReader : HistorianDataReaderBase
    {
        ArchiveList m_list;
        ArchiveListSnapshot m_snapshot;

        public ArchiveReader(ArchiveList list)
        {
            m_list = list;
            m_snapshot = m_list.CreateNewClientResources();
        }

        public override IPointStream Read(KeyParserPrimary key1, KeyParserSecondary key2, DataReaderOptions readerOptions)
        {
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

        private class ReadStream : IPointStream
        {
            ArchiveListSnapshot m_snapshot;
            ulong m_startKey;
            ulong m_stopKey;
            KeyParserPrimary m_key1;
            KeyParserSecondary m_key2;
            bool m_timedOut;
            long m_pointCount;

            TimeoutOperation m_timeout;
            Queue<KeyValuePair<int, ArchiveFileSummary>> m_tables;

            int m_currentIndex;
            ArchiveFileSummary m_currentSummary;
            ArchiveFileReadSnapshot m_currentInstance;
            ITreeScanner256 m_currentScanner;

            public ReadStream(KeyParserPrimary key1, KeyParserSecondary key2, ArchiveListSnapshot snapshot, DataReaderOptions readerOptions)
            {
                if (readerOptions.Timeout.Ticks > 0)
                {
                    m_timeout = new TimeoutOperation();
                    m_timeout.RegisterTimeout(readerOptions.Timeout, () => m_timedOut = true);
                }

                m_key1 = key1;
                m_key2 = key2;
                m_startKey = key1.StartKey;
                m_stopKey = key1.EndKey;
                m_snapshot = snapshot;
                m_snapshot.UpdateSnapshot();

                m_tables = new Queue<KeyValuePair<int, ArchiveFileSummary>>();

                for (int x = 0; x < m_snapshot.Tables.Count(); x++)
                {
                    var table = m_snapshot.Tables[x];
                    if (table != null)
                    {
                        if (table.Contains(key1.StartKey, key1.EndKey))
                        {
                            m_tables.Enqueue(new KeyValuePair<int, ArchiveFileSummary>(x, table));
                        }
                        else
                        {
                            m_snapshot.Tables[x] = null;
                        }
                    }
                }
                prepareNextFile();
            }

            public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
            {
            TryAgain:
                if (m_timedOut)
                    Cancel();
                if (m_currentScanner.GetNextKey(out key1, out key2, out value1, out value2))
                {
                    if (key1 <= m_stopKey)
                    {
                        if (m_key2.ContainsKey(key2))
                            return true;
                        goto TryAgain;
                    }

                    if (m_key1.GetNextWindow(out m_startKey, out m_stopKey))
                    {
                        m_currentScanner.SeekToKey(m_startKey, 0);
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

            bool prepareNextFile()
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

                    var kvp = m_tables.Dequeue();
                    m_currentIndex = kvp.Key;
                    m_currentInstance = kvp.Value.ActiveSnapshotInfo.CreateReadSnapshot();
                    m_currentScanner = m_currentInstance.GetTreeScanner();
                    m_currentScanner.SeekToKey(m_startKey, 0);
                }
                else
                {
                    m_currentScanner = NullTreeScanner256.Instance;
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
                m_currentScanner = NullTreeScanner256.Instance;
                while (m_tables.Count > 0)
                {
                    var kvp = m_tables.Dequeue();
                    m_snapshot.Tables[kvp.Key] = null;
                }
            }
        }
    }
}
