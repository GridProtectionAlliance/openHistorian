//******************************************************************************************************
//  HistorianClient_Database.cs - Gbtc
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

namespace openHistorian
{
    public partial class HistorianClient<TKey, TValue>
    {
        private class HistorianDatabase
            : HistorianDatabaseBase<TKey, TValue>
        {
            private bool m_disposed;
            private readonly HistorianClient<TKey, TValue> m_client;

            private HistorianDataReader m_historianReader;
            private readonly Action m_onDispose;

            public HistorianDatabase(HistorianClient<TKey, TValue> client, Action onDispose)
            {
                m_onDispose = onDispose;
                m_client = client;
            }

            /// <summary>
            /// Opens a stream connection that can be used to read 
            /// and write data to the current historian database.
            /// </summary>
            /// <returns></returns>
            public override HistorianDataReaderBase<TKey, TValue> OpenDataReader()
            {
                if (m_historianReader != null)
                    throw new Exception("Only one datareader can process at a time when using sockets.");

                m_client.m_stream.Write((byte)ServerCommand.OpenReader);
                m_client.m_stream.Flush();

                m_historianReader = new HistorianDataReader(m_client, () => m_historianReader = null);
                return m_historianReader;
            }

            public override void Write(KeyValueStream<TKey, TValue> points)
            {
                if (m_historianReader != null)
                    throw new Exception("Cannot write to the database when a reader is open when using sockets.");

                TKey oldKey = new TKey();
                TValue oldValue = new TValue();
                m_client.m_stream.Write((byte)ServerCommand.Write);
                
                m_client.m_compressionMode.ResetEncoder();
                while (points.Read())
                {
                    m_client.m_compressionMode.Encode(m_client.m_stream, points.CurrentKey, points.CurrentValue);
                }
                m_client.m_compressionMode.WriteEndOfStream(m_client.m_stream);
                m_client.m_stream.Flush();
            }

            public override void Write(TKey key, TValue value)
            {
                if (m_historianReader != null)
                    throw new Exception("Cannot write to the database when a reader is open when using sockets.");

                m_client.m_stream.Write((byte)ServerCommand.Write);
                m_client.m_compressionMode.ResetEncoder();
                m_client.m_compressionMode.Encode(m_client.m_stream, key, value);
                m_client.m_compressionMode.WriteEndOfStream(m_client.m_stream);
                m_client.m_stream.Flush();
            }

            public override void SoftCommit()
            {
                //throw new NotImplementedException();
            }

            public override void HardCommit()
            {
                //throw new NotImplementedException();
            }

            public override void Disconnect()
            {
                Dispose();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public override void Dispose()
            {
                if (!m_disposed)
                {
                    m_disposed = true;
                    if (m_historianReader != null)
                        m_historianReader.Close();

                    m_client.m_stream.Write((byte)ServerCommand.DisconnectDatabase);
                    m_client.m_stream.Flush();
                    m_onDispose();
                }
            }
        }
    }
}