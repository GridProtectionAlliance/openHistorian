//******************************************************************************************************
//  RemoteHistorian_DataReader.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using openHistorian.Collections.Generic;

namespace openHistorian.Communications
{
    public partial class RemoteHistorian<TKey, TValue>
    {
        private class HistorianDataReader
            : HistorianDataReaderBase<TKey, TValue>
        {
            private readonly Action m_onDispose;
            private readonly RemoteHistorian<TKey, TValue> m_client;
            private PointReader m_reader;
            private bool m_closed;

            public HistorianDataReader(RemoteHistorian<TKey, TValue> client, Action onDispose)
            {
                m_onDispose = onDispose;
                m_client = client;
            }

            public override void Dispose()
            {
                Close();
            }

            public override TreeStream<TKey, TValue> Read(QueryFilterTimestamp key1, QueryFilterPointId key2, DataReaderOptions readerOptions)
            {
                if (m_reader != null)
                    throw new Exception("Sockets do not support concurrent readers.");

                m_client.m_stream.Write((byte)ServerCommand.Read);
                key1.Save(m_client.m_stream);
                key2.Save(m_client.m_stream);
                readerOptions.Save(m_client.m_stream);
                m_client.m_stream.Flush();
                return new PointReader(m_client, () => m_reader = null);
            }

            public override void Close()
            {
                if (!m_closed)
                {
                    if (m_reader != null)
                        m_reader.Cancel();

                    m_client.m_stream.Write((byte)ServerCommand.DisconnectReader);
                    m_client.m_stream.Flush();
                    m_onDispose();
                }
            }

            private class PointReader
                : TreeStream<TKey, TValue>
            {
                private bool m_completed;
                private readonly RemoteHistorian<TKey, TValue> m_client;
                private readonly Action m_onComplete;

                public PointReader(RemoteHistorian<TKey, TValue> client, Action onComplete)
                {
                    m_client = client;
                    m_onComplete = onComplete;
                    m_client.m_compressionMode.ResetEncoder();
                }

                public override bool Read()
                {
                    
                    if (!m_completed && m_client.m_compressionMode.TryDecode(m_client.m_stream, CurrentKey, CurrentValue))
                    {
                        //CurrentKey.ReadCompressed(m_client.m_stream, CurrentKey);
                        //CurrentValue.ReadCompressed(m_client.m_stream, CurrentValue);
                        return true;
                    }
                    else
                    {
                        Complete();
                        return false;
                    }
                }

                public override void Cancel()
                {
                    if (m_completed)
                        return;
                    m_client.m_stream.Write((byte)ServerCommand.CancelRead);
                    m_client.m_stream.Flush();
                    //flush the rest of the data off of the receive queue.
                    while (m_client.m_compressionMode.TryDecode(m_client.m_stream, CurrentKey, CurrentValue))
                    {
                        //CurrentKey.ReadCompressed(m_client.m_stream, CurrentKey);
                        //CurrentValue.ReadCompressed(m_client.m_stream, CurrentValue);
                    }
                    Complete();
                }

                private void Complete()
                {
                    m_completed = true;
                    m_onComplete();
                }
            }
        }
    }
}