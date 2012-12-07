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
using System.Text;
using openHistorian.Collections;
using openHistorian.Collections.KeyValue;
using openHistorian.Server.Database.Archive;

namespace openHistorian.Server.Database
{
    public class ArchiveReader : IDisposable
    {
        ArchiveList m_list;
        ArchiveListSnapshot m_snapshot;

        public ArchiveReader(ArchiveList list)
        {
            m_list = list;
            m_snapshot = m_list.CreateNewClientResources();
        }

        public IPointStream Read(ulong key)
        {
            return new ReadStream(key, key, m_snapshot);
        }

        public IPointStream Read(ulong startKey, ulong endKey)
        {
            return new ReadStream(startKey, endKey, m_snapshot);
        }

        public IPointStream Read(ulong startKey, ulong endKey, IEnumerable<ulong> points)
        {
            ulong maxValue = points.Max();
            if (maxValue < 8 * 1024 * 64) //524288
            {
                return new ReadStreamFilteredBitArray(startKey, endKey, m_snapshot, points, (int)maxValue);
            }
            else if (maxValue <= uint.MaxValue)
            {
                return new ReadStreamFilteredIntDictionary(startKey, endKey, m_snapshot, points);
            }
            else
            {
                return new ReadStreamFilteredLongDictionary(startKey, endKey, m_snapshot, points);
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            m_snapshot.Dispose();
        }

        private class ReadStream : IPointStream
        {
            ArchiveListSnapshot m_snapshot;
            ulong m_startKey;
            ulong m_stopKey;
            Queue<KeyValuePair<int, ArchiveFileSummary>> m_tables;

            int m_currentIndex;
            ArchiveFileSummary m_currentSummary;
            ArchiveFileReadOnlySnapshotInstance m_currentInstance;
            ITreeScanner256 m_currentScanner;

            public ReadStream(ulong startKey, ulong stopKey, ArchiveListSnapshot snapshot)
            {
                m_startKey = startKey;
                m_stopKey = stopKey;
                m_snapshot = snapshot;
                m_snapshot.UpdateSnapshot();

                m_tables = new Queue<KeyValuePair<int, ArchiveFileSummary>>();

                for (int x = 0; x < m_snapshot.Tables.Count(); x++)
                {
                    var table = m_snapshot.Tables[x];
                    if (table != null)
                    {
                        if (table.Contains(startKey, stopKey))
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
                if (m_currentScanner.GetNextKey(out key1, out key2, out value1, out value2))
                {
                    if (key1 <= m_stopKey)
                        return true;
                }
                if (!prepareNextFile())
                    return false;
                return Read(out key1, out key2, out value1, out value2);
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
                    var kvp = m_tables.Dequeue();
                    m_currentIndex = kvp.Key;
                    m_currentInstance = kvp.Value.ActiveSnapshot.OpenInstance();
                    m_currentScanner = m_currentInstance.GetDataRange();
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

        private class ReadStreamFilteredBitArray : IPointStream
        {
            ReadStream m_stream;
            BitArray m_points;
            ulong m_maxValue;

            public ReadStreamFilteredBitArray(ulong startKey, ulong stopKey, ArchiveListSnapshot snapshot, IEnumerable<ulong> points, int maxValue)
            {
                m_maxValue = (ulong)maxValue;
                m_points = new BitArray(maxValue + 1, false);
                foreach (ulong pt in points)
                {
                    m_points.SetBit((int)pt);
                }
                m_stream = new ReadStream(startKey, stopKey, snapshot);
            }

            public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
            {
                while (m_stream.Read(out key1, out key2, out value1, out value2))
                {
                    if (key2 <= m_maxValue && m_points[(int)key2])
                        return true;
                }
                return false;
            }

            public void Cancel()
            {
                m_stream.Cancel();
            }
        }

        private class ReadStreamFilteredLongDictionary : IPointStream
        {
            ReadStream m_stream;
            Dictionary<ulong, byte> m_points;

            public ReadStreamFilteredLongDictionary(ulong startKey, ulong stopKey, ArchiveListSnapshot snapshot, IEnumerable<ulong> points)
            {
                m_points = new Dictionary<ulong, byte>(points.Count() * 5);
                foreach (ulong pt in points)
                {
                    m_points.Add(pt, 0);
                }
                m_stream = new ReadStream(startKey, stopKey, snapshot);
            }

            public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
            {
                while (m_stream.Read(out key1, out key2, out value1, out value2))
                {
                    if (m_points.ContainsKey(key2))
                        return true;
                }
                return false;
            }

            public void Cancel()
            {
                m_stream.Cancel();
            }
        }

        private class ReadStreamFilteredIntDictionary : IPointStream
        {
            ReadStream m_stream;
            Dictionary<uint, byte> m_points;

            public ReadStreamFilteredIntDictionary(ulong startKey, ulong stopKey, ArchiveListSnapshot snapshot, IEnumerable<ulong> points)
            {
                m_points = new Dictionary<uint, byte>(points.Count() * 5);
                foreach (ulong pt in points)
                {
                    m_points.Add((uint)pt, 0);
                }
                m_stream = new ReadStream(startKey, stopKey, snapshot);
            }

            public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
            {
                while (m_stream.Read(out key1, out key2, out value1, out value2))
                {
                    if (key2 <= uint.MaxValue && m_points.ContainsKey((uint)key2))
                        return true;
                }
                return false;
            }

            public void Cancel()
            {
                m_stream.Cancel();
            }
        }

    }
}
