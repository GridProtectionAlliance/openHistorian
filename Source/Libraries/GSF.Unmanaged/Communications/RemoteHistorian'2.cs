//******************************************************************************************************
//  RemoteHistorian.cs - Gbtc
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
using System.Net;
using System.Net.Sockets;
using GSF.Communications;
using openHistorian.Collections;
using openHistorian.Communications.Compression;
using openHistorian.Communications.Initialization;

namespace openHistorian.Communications
{
    /// <summary>
    /// Connects to a socket based remoted historian database collection.
    /// </summary>
    public partial class RemoteHistorian<TKey, TValue> :
        IHistorianDatabaseCollection<TKey, TValue>, IDisposable
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : HistorianValueBase<TValue>, new()
    {
        private TcpClient m_client;
        private readonly NetworkBinaryStream2 m_stream;
        private HistorianDatabase m_historianDatabase;
        KeyValueStreamCompressionBase<TKey, TValue> m_compressionMode;

        public RemoteHistorian(IPEndPoint server)
        {
            m_client = new TcpClient();
            m_client.Connect(server);
            m_stream = new NetworkBinaryStream2(m_client.Client);
            m_stream.Write(1122334455667788991L);
            m_stream.Flush();
        }

        public HistorianDatabaseBase<TKey, TValue> this[string databaseName]
        {
            get
            {
                if (m_historianDatabase != null)
                    throw new Exception("Can only connect to one database at a time.");
                //m_compressionMode = KeyValueStreamCompression.CreateKeyValueStreamCompression<TKey, TValue>(CreateFixedSizeStream.TypeGuid);
                //m_compressionMode = KeyValueStreamCompression.CreateKeyValueStreamCompression<TKey, TValue>(CreateCompressedStream.TypeGuid);
                m_compressionMode = KeyValueStreamCompression.CreateKeyValueStreamCompression<TKey, TValue>(CreateHistorianCompressedStream.TypeGuid);

                m_stream.Write((byte)ServerCommand.SetCompressionMode);
                m_stream.Write(m_compressionMode.CompressionType);
                m_stream.Write((byte)ServerCommand.ConnectToDatabase);
                m_stream.Write(databaseName);
                m_stream.Flush();
                m_historianDatabase = new HistorianDatabase(this, () => m_historianDatabase = null);
                return m_historianDatabase;
            }
        }

        public void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_historianDatabase != null)
                m_historianDatabase.Dispose();

            m_stream.Write((byte)ServerCommand.Disconnect);
            m_stream.Flush();

            if (m_client != null)
                m_client.Close();
            m_client = null;
        }
    }
}