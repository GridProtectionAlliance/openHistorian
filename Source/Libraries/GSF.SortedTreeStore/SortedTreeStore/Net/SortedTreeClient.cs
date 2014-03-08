//******************************************************************************************************
//  SortedTreeClient.cs - Gbtc
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

namespace GSF.SortedTreeStore.Net
{
    /// <summary>
    /// Connects to a socket based remoted historian database collection.
    /// </summary>
    public class SortedTreeClient
        : IDisposable
    {
        private TcpClient m_client;
        private NetworkBinaryStream m_stream;
        private SortedTreeEngineBase m_sortedTreeEngine;
        string m_historianDatabaseString;

        private readonly string m_defaultDatabase;

        public SortedTreeClient(SortedTreeClientOptions options)
        {
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
        public SortedTreeClientEngine<TKey, TValue> GetDefaultDatabase<TKey, TValue>()
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return GetDatabase<TKey, TValue>(m_defaultDatabase);
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

        /// <summary>
        /// Accesses <see cref="SortedTreeEngineBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <param name="encodingMethod"></param>
        /// <returns><see cref="SortedTreeEngineBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public SortedTreeClientEngine<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName, EncodingDefinition encodingMethod = null)
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

            var db = new SortedTreeClientEngine<TKey, TValue>(m_stream, () => m_sortedTreeEngine = null);
            m_sortedTreeEngine = db;
            m_historianDatabaseString = databaseName;
           
            db.SetEncodingMode(encodingMethod);

            return db;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
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
}