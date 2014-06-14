//******************************************************************************************************
//  NetworkClient.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
using System.Net;
using System.Net.Sockets;
using GSF.Net;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Net
{
    /// <summary>
    /// A client that communicates over a network socket.
    /// </summary>
    public class NetworkClient
        : Client
    {
        private bool m_disposed;
        private TcpClient m_client;
        private NetworkBinaryStream m_stream;
        private ClientDatabaseBase m_sortedTreeEngine;
        string m_historianDatabaseString;

        /// <summary>
        /// Creates a <see cref="NetworkClient"/>
        /// </summary>
        /// <param name="config">The config to use for the client</param>
        public NetworkClient(NetworkClientConfig config)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(config.ServerNameOrIp, out ip))
            {
                ip = Dns.GetHostAddresses(config.ServerNameOrIp)[0];
            }

            Start(new IPEndPoint(ip, config.NetworkPort));
        }

        /// <summary>
        /// Connects to the remote historian.
        /// </summary>
        /// <param name="server">The server to connect to</param>
        private void Start(IPEndPoint server)
        {
            m_client = new TcpClient(AddressFamily.InterNetworkV6);
            m_client.Client.DualMode = true;
            m_client.Connect(server);
            m_stream = new NetworkBinaryStream(m_client.Client);
            m_stream.Write(1122334455667788993L);
            m_stream.Flush();

            var command = (ServerResponse)m_stream.ReadUInt8();
            switch (command)
            {
                case ServerResponse.UnhandledException:
                    string exception = m_stream.ReadString();
                    throw new Exception("Server UnhandledExcetion: \n" + exception);
                case ServerResponse.UnknownProtocolIdentifier:
                    throw new Exception("Client and server cannot agree on a protocol, this is commonly because they are running incompatible versions.");
                case ServerResponse.ConnectedToRoot:
                    break;
                default:
                    throw new Exception("Unknown server response: " + command.ToString());
            }
        }

        public override ClientDatabaseBase GetDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override ClientDatabaseBase<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName)
        {
            return GetDatabase<TKey, TValue>(databaseName, null);
        }

        public override List<DatabaseInfo> GetDatabaseInfo()
        {
            throw new NotImplementedException();
        }

        public override bool Contains(string databaseName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Accesses <see cref="NetworkClientDatabase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <param name="encodingMethod"></param>
        /// <returns><see cref="NetworkClientDatabase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        NetworkClientDatabase<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName, EncodingDefinition encodingMethod = null)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            if ((object)encodingMethod == null)
                encodingMethod = SortedTree.FixedSizeNode;

            if (m_sortedTreeEngine != null)
            {
                throw new Exception("Can only connect to one database at a time. Please disconnect from database" + m_historianDatabaseString);
            }

            m_stream.Write((byte)ServerCommand.ConnectToDatabase);
            m_stream.Write(databaseName);
            m_stream.Write(new TKey().GenericTypeGuid);
            m_stream.Write(new TValue().GenericTypeGuid);
            m_stream.Flush();

            var command = (ServerResponse)m_stream.ReadUInt8();
            switch (command)
            {
                case ServerResponse.UnhandledException:
                    string exception = m_stream.ReadString();
                    throw new Exception("Server UnhandledExcetion: \n" + exception);
                case ServerResponse.DatabaseDoesNotExist:
                    throw new Exception("Database does not exist on the server: " + databaseName);
                case ServerResponse.DatabaseKeyUnknown:
                    throw new Exception("Database key does not match that passed to this function");
                case ServerResponse.DatabaseValueUnknown:
                    throw new Exception("Database value does not match that passed to this function");
                case ServerResponse.SuccessfullyConnectedToDatabase:
                    break;
                default:
                    throw new Exception("Unknown server response: " + command.ToString());
            }

            var db = new NetworkClientDatabase<TKey, TValue>(m_stream, () => m_sortedTreeEngine = null);
            m_sortedTreeEngine = db;
            m_historianDatabaseString = databaseName;

            db.SetEncodingMode(encodingMethod);

            return db;
        }


        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="NetworkClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        if (m_sortedTreeEngine != null)
                            m_sortedTreeEngine.Dispose();
                        m_sortedTreeEngine = null;

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
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }
    }
}