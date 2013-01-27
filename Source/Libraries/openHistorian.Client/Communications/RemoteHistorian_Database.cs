//******************************************************************************************************
//  RemoteHistorian_Database.cs - Gbtc
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
using GSF;

namespace openHistorian.Communications
{
    public partial class RemoteHistorian
    {
        class HistorianDatabase : IHistorianDatabase
        {
            bool m_disposed;
            RemoteHistorian m_client;

            HistorianDataReader m_historianReader;
            Action m_onDispose;

            public HistorianDatabase(RemoteHistorian client, Action onDispose)
            {
                m_onDispose = onDispose;
                m_client = client;
            }

            /// <summary>
            /// Opens a stream connection that can be used to read 
            /// and write data to the current historian database.
            /// </summary>
            /// <returns></returns>
            public IHistorianDataReader OpenDataReader()
            {
                if (m_historianReader != null)
                    throw new Exception("Only one datareader can process at a time when using sockets.");

                m_client.m_stream.Write((byte)ServerCommand.OpenReader);
                m_client.m_netStream.Flush();

                m_historianReader = new HistorianDataReader(m_client, () => m_historianReader = null);
                return m_historianReader;
            }

            public void Write(IStream256 points)
            {
                if (m_historianReader != null)
                    throw new Exception("Cannot write to the database when a reader is open when using sockets.");

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
                if (m_historianReader != null)
                    throw new Exception("Cannot write to the database when a reader is open when using sockets.");

                m_client.m_stream.Write((byte)ServerCommand.Write);
                m_client.m_stream.Write(true);
                m_client.m_stream.Write7Bit(key1);
                m_client.m_stream.Write7Bit(key2);
                m_client.m_stream.Write7Bit(value1);
                m_client.m_stream.Write7Bit(value2);
                m_client.m_stream.Write(false);
                m_client.m_netStream.Flush();
            }

            public void SoftCommit()
            {
                throw new NotImplementedException();
            }

            public void HardCommit()
            {
                throw new NotImplementedException();
            }

            public void Disconnect()
            {
                Dispose();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    m_disposed = true;
                    if (m_historianReader != null)
                        m_historianReader.Close();

                    m_client.m_stream.Write((byte)ServerCommand.DisconnectDatabase);
                    m_client.m_netStream.Flush();
                    m_onDispose();
                }
            }
        }

    }
}
