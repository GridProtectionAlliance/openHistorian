//******************************************************************************************************
//  RemoteHistorian.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using openHistorian.IO;

namespace openHistorian.Streaming.Client
{
    public class RemoteHistorian : IHistorian
    {
        TcpClient m_client;
        NetworkBinaryStream m_netStream;
        BinaryStreamWrapper m_stream;
        IHistorianReadWrite m_historianRW;

        public RemoteHistorian(IPEndPoint server)
        {
            m_client = new TcpClient();
            m_client.Connect(server);
            m_netStream = new NetworkBinaryStream(m_client.Client);
            m_stream = new BinaryStreamWrapper(m_netStream);
            m_stream.Write(1122334455667788990L);
            m_netStream.Flush();
        }

        public void Dispose()
        {
            if (m_client != null)
                m_client.Close();
            m_client = null;
        }

        public IHistorianReadWrite ConnectToDatabase(string connectionString)
        {
            m_stream.Write((byte)ServerCommand.Connect);
            m_stream.Write(connectionString);
            m_netStream.Flush();
            m_historianRW = new HistorianReadWrite(this);
            return m_historianRW;
        }

        public IManageHistorian Manage()
        {
            throw new NotSupportedException();
        }

        public void Disconnect()
        {
            m_stream.Write((byte)ServerCommand.Disconnect);
            m_netStream.Flush();
        }

        class HistorianReadWrite : IHistorianReadWrite
        {
            RemoteHistorian m_client;
            //long m_lastCommittedTransactionId;
            //long m_lastDiskCommittedTransactionId;
            //long m_currentTransactionId;

            public HistorianReadWrite(RemoteHistorian client)
            {
                m_client = client;
            }


            public void Dispose()
            {
            }

            public IPointStream Read(ulong key)
            {
                return Read(key, key, null);
            }

            public IPointStream Read(ulong startKey, ulong endKey)
            {
                return Read(startKey, endKey, null);
            }

            public IPointStream Read(ulong startKey, ulong endKey, IEnumerable<ulong> points)
            {
                m_client.m_stream.Write((byte)ServerCommand.Read);
                m_client.m_stream.Write(startKey);
                m_client.m_stream.Write(endKey);
                if (points == null)
                {
                    m_client.m_stream.Write(0);
                }
                else
                {
                    m_client.m_stream.Write(points.Count());
                    foreach (var pt in points)
                    {
                        m_client.m_stream.Write(pt);
                    }
                }
                m_client.m_netStream.Flush();
                return new PointReader(m_client);
            }

            public void Write(IPointStream points)
            {
                ulong oldKey1 = 0, oldKey2 = 0, oldValue1 = 0, oldValue2 = 0;
                ulong key1, key2, value1, value2;
                m_client.m_stream.Write((byte)ServerCommand.Write);
                while (points.Read(out key1, out key2, out value1, out value2))
                {
                    m_client.m_stream.Write(true);
                    m_client.m_stream.Write7Bit(oldKey1 ^ key1);
                    m_client.m_stream.Write7Bit(oldKey2 ^ key2);
                    m_client.m_stream.Write7Bit(oldValue1 ^ value1);
                    m_client.m_stream.Write7Bit(oldValue2 ^ value2);
                    
                    oldKey1 = key1;
                    oldKey2 = key2;
                    oldValue1 = value1;
                    oldValue2 = value2;
                }
                m_client.m_stream.Write(false);
                m_client.m_netStream.Flush();
            }

            public void Write(ulong key1, ulong key2, ulong value1, ulong value2)
            {
                m_client.m_stream.Write((byte)ServerCommand.Write);
                m_client.m_stream.Write(true);
                m_client.m_stream.Write7Bit(key1);
                m_client.m_stream.Write7Bit(key2);
                m_client.m_stream.Write7Bit(value1);
                m_client.m_stream.Write7Bit(value2);
                m_client.m_stream.Write(false);
                m_client.m_netStream.Flush();
            }

            public long WriteBulk(IPointStream points)
            {
                throw new NotSupportedException();
            }

            public bool IsCommitted(long transactionId)
            {
                throw new NotSupportedException();
            }

            public bool IsDiskCommitted(long transactionId)
            {
                throw new NotSupportedException();
            }

            public bool WaitForCommitted(long transactionId)
            {
                throw new NotSupportedException();
            }

            public bool WaitForDiskCommitted(long transactionId)
            {
                throw new NotSupportedException();
            }

            public void Commit()
            {
                throw new NotSupportedException();
            }

            public void CommitToDisk()
            {
                throw new NotSupportedException();
            }

            public long LastCommittedTransactionId
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            public long LastDiskCommittedTransactionId
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            public long CurrentTransactionId
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            public void Disconnect()
            {
                m_client.m_stream.Write((byte)ServerCommand.Disconnect);
                m_client.m_netStream.Flush();
            }

            class PointReader : IPointStream
            {
                bool m_completed;
                ulong m_key1 = 0, m_key2 = 0, m_value1 = 0, m_value2 = 0;
                RemoteHistorian m_client;
                public PointReader(RemoteHistorian client)
                {
                    m_client = client;
                }

                public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
                {
                    if (!m_completed && m_client.m_stream.ReadBoolean())
                    {
                        m_key1 ^= m_client.m_stream.Read7BitUInt64();
                        m_key2 ^= m_client.m_stream.Read7BitUInt64();
                        m_value1 ^= m_client.m_stream.Read7BitUInt64();
                        m_value2 ^= m_client.m_stream.Read7BitUInt64();
                        key1 = m_key1;
                        key2 = m_key2;
                        value1 = m_value1;
                        value2 = m_value2;
                        return true;
                    }
                    else
                    {
                        key1 = 0;
                        key2 = 0;
                        value1 = 0;
                        value2 = 0;
                        m_completed = true;
                        return false;
                    }
                }

                public void Cancel()
                {
                    if (m_completed)
                        return;
                    m_client.m_stream.Write((byte)ServerCommand.CancelRead);
                    m_client.m_netStream.Flush();
                    //flush the rest of the data off of the receive queue.
                    while (m_client.m_stream.ReadBoolean())
                    {
                        m_client.m_stream.Read7BitUInt64();
                        m_client.m_stream.Read7BitUInt64();
                        m_client.m_stream.Read7BitUInt64();
                        m_client.m_stream.Read7BitUInt64();
                    }
                    m_completed = true;
                }
            }
        }
    }
}
