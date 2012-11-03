//******************************************************************************************************
//  Historian_HistorianReadWrite.cs - Gbtc
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
//  9/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.V2.Collections.KeyValue;

namespace openHistorian.V2.Local
{
    public partial class Historian
    {
        public class HistorianReadWrite : IHistorianReadWrite
        {
            class PointStream : IPointStream
            {
                ITreeScanner256 m_scanner;
                ulong m_lastValidKey1;
                ulong m_lastValidKey2;
                public PointStream(ITreeScanner256 scanner, ulong lastValidKey1, ulong lastValidKey2)
                {
                    m_lastValidKey1 = lastValidKey1;
                    m_lastValidKey2 = lastValidKey2;
                    m_scanner = scanner;
                }
                public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
                {
                    bool isValid = m_scanner.GetNextKey(out key1, out key2, out value1, out value2);
                    return (isValid && key1 <= m_lastValidKey1 && key2 <= m_lastValidKey2);
                }
                public void Cancel()
                {

                }
            }
            class PointStreamList : IPointStream
            {
                ITreeScanner256 m_scanner;
                ulong m_lastValidKey1;
                ulong m_lastValidKey2;
                ulong[] m_points;
                public PointStreamList(ITreeScanner256 scanner, ulong lastValidKey1, ulong lastValidKey2, ulong[] points)
                {
                    m_points = points;
                    m_lastValidKey1 = lastValidKey1;
                    m_lastValidKey2 = lastValidKey2;
                    m_scanner = scanner;
                }
                public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
                {
                    while (m_scanner.GetNextKey(out key1, out key2, out value1, out value2))
                    {
                        if (key1 > m_lastValidKey1 || key2 > m_lastValidKey2)
                            return false;
                        if (m_points.Contains(key2))
                            return true;
                    }
                    return false;
                }
                public void Cancel()
                {

                }
            }

            Historian m_historian;

            public HistorianReadWrite(Historian historian)
            {
                m_historian = historian;
            }

            public IPointStream Read(ulong key)
            {
                return Read(key, key);
            }

            public IPointStream Read(ulong startKey, ulong endKey)
            {
                var scanner = m_historian.m_tree.GetDataRange();
                scanner.SeekToKey(startKey, 0);
                return new PointStream(scanner, endKey, ulong.MaxValue);
            }

            public IPointStream Read(ulong startKey, ulong endKey, IEnumerable<ulong> points)
            {
                var scanner = m_historian.m_tree.GetDataRange();
                scanner.SeekToKey(startKey, 0);
                return new PointStreamList(scanner, endKey, ulong.MaxValue, points.ToArray());
            }

            public void Write(IPointStream points)
            {
                ulong key1, key2, value1, value2;
                while (points.Read(out key1, out key2, out value1, out value2))
                    Write(key1, key2, value1, value2);
            }

            public void Write(ulong key1, ulong key2, ulong value1, ulong value2)
            {
                m_historian.m_tree.Add(key1, key2, value1, value2);
            }

            public long WriteBulk(IPointStream points)
            {
                Write(points);
                return 1;
            }

            public bool IsCommitted(long transactionId)
            {
                throw new NotImplementedException();
            }

            public bool IsDiskCommitted(long transactionId)
            {
                throw new NotImplementedException();
            }

            public bool WaitForCommitted(long transactionId)
            {
                throw new NotImplementedException();
            }

            public bool WaitForDiskCommitted(long transactionId)
            {
                throw new NotImplementedException();
            }

            public void Commit()
            {
                throw new NotImplementedException();
            }

            public void CommitToDisk()
            {
                throw new NotImplementedException();
            }

            public long LastCommittedTransactionId { get; private set; }
            public long LastDiskCommittedTransactionId { get; private set; }
            public long CurrentTransactionId { get; private set; }

            public void Disconnect()
            {
                
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {

            }
        }
    }
}
