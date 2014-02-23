//******************************************************************************************************
//  SortedTreeStoreClient`2.cs - Gbtc
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
using GSF.Net;
using GSF.SortedTreeStore.Engine;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Net.Initialization;

namespace GSF.SortedTreeStore.Net
{

    public class HistorianClientOptions
    {
        public bool IsReadOnly = true;
        public int NetworkPort = 38402;
        public string ServerNameOrIp = "localhost";
        public string DefaultDatabase = "default";
    }

    /// <summary>
    /// Connects to a socket based remoted historian database collection.
    /// </summary>
    public partial class SortedTreeStoreClient<TKey, TValue> :
        HistorianCollection<TKey, TValue>, IDisposable
        where TKey : EngineKeyBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private TcpClient m_client;
        private NetworkBinaryStream m_stream;
        private SortedTreeEngine m_sortedTreeEngine;
        private Guid m_compressionMethod;
        string m_historianDatabaseString;

        KeyValueStreamCompressionBase<TKey, TValue> m_compressionMode;

        private readonly string m_defaultDatabase;

        public SortedTreeStoreClient(HistorianClientOptions options, Guid compressionMethod)
        {
            m_compressionMethod = compressionMethod;
            IPAddress ip;
            if (!IPAddress.TryParse(options.ServerNameOrIp, out ip))
            {
                ip = Dns.GetHostAddresses(options.ServerNameOrIp)[0];
            }

            Start(new IPEndPoint(ip, options.NetworkPort));
            m_defaultDatabase = options.DefaultDatabase;
        }

        /// <summary>
        /// Gets the default database as defined in the constructor's options.
        /// </summary>
        /// <returns></returns>
        public SortedTreeEngineBase<TKey, TValue> GetDefaultDatabase()
        {
            return this[m_defaultDatabase];
        }

        //protected RemoteHistorian(IPEndPoint server)
        //{
        //    Initialize(server);
        //}

        /// <summary>
        /// Connects to the remote historian.
        /// </summary>
        /// <param name="server"></param>
        private void Start(IPEndPoint server)
        {
            m_client = new TcpClient();
            m_client.Connect(server);
            m_stream = new NetworkBinaryStream(m_client.Client);
            m_stream.Write(1122334455667788991L);
            m_stream.Flush();
        }

        /// <summary>
        /// Accesses <see cref="SortedTreeEngineBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="SortedTreeEngineBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public override SortedTreeEngineBase<TKey, TValue> this[string databaseName]
        {
            get
            {
                if (m_sortedTreeEngine != null)
                {
                    if (m_historianDatabaseString == databaseName)
                        return m_sortedTreeEngine;

                    throw new Exception("Can only connect to one database at a time. Please disconnect from database" + m_historianDatabaseString);
                }

                //m_compressionMode = KeyValueStreamCompression.CreateKeyValueStreamCompression<TKey, TValue>(CreateFixedSizeStream.TypeGuid);
                //m_compressionMode = KeyValueStreamCompression.CreateKeyValueStreamCompression<TKey, TValue>(CreateCompressedStream.TypeGuid);
                m_compressionMode = KeyValueStreamCompression.CreateKeyValueStreamCompression<TKey, TValue>(m_compressionMethod);

                m_stream.Write((byte)ServerCommand.SetCompressionMode);
                m_stream.Write(m_compressionMode.CompressionType);
                m_stream.Write((byte)ServerCommand.ConnectToDatabase);
                m_stream.Write(databaseName);
                m_stream.Flush();

                m_sortedTreeEngine = new SortedTreeEngine(this, () => m_sortedTreeEngine = null);
                m_historianDatabaseString = databaseName;

                return m_sortedTreeEngine;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (m_sortedTreeEngine != null)
                m_sortedTreeEngine.Dispose();

            try
            {
                m_stream.Write((byte)ServerCommand.Disconnect);
                m_stream.Flush();
            }
            catch (Exception)
            {

            }


            if (m_client != null)
                m_client.Close();
            m_client = null;
        }
    }
}