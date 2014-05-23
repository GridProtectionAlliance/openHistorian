//******************************************************************************************************
//  ServerHosts.cs - Gbtc
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
//  12/9/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Net;
using System.Text;
using GSF.Collections;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Services.Net;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Hosts all of the components of a SortedTreeStore.
    /// </summary>
    /// <remarks>
    /// A centralized server hosting model for a SortedTreeStore.
    /// 
    /// This class contains all of the databases, client connections,
    /// sockets, user authentication, and core settings for the SortedTreeStore.
    /// </remarks>
    public partial class Server
        : ISortedTreeServer
    {
        private bool m_disposed;
        private readonly object m_syncRoot = new object();
        private readonly Dictionary<string, ServerDatabaseBase> m_databases;
        private readonly WeakList<Client> m_clients;
        private Dictionary<IPEndPoint, SocketListener> m_sockets;

        /// <summary>
        /// Creates an empty server instance
        /// </summary>
        public Server()
        {
            m_sockets = new Dictionary<IPEndPoint, SocketListener>();
            m_clients = new WeakList<Client>();
            m_databases = new Dictionary<string, ServerDatabaseBase>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="Server"/>
        /// </summary>
        public Server(ServerConfig config)
            : this()
        {
            config.Databases.ForEach(LoadConfig);
            config.SocketConfig.ForEach(LoadConfig);
        }

        public void LoadConfig(ServerDatabaseConfig databaseConfig)
        {
            Add(ServerDatabaseBase.CreateDatabase(databaseConfig));
        }

        public void Unload(string database)
        {
            Remove(database, 0);
        }

        public void LoadConfig(SocketListenerConfig socketConfig)
        {
            SocketListener listener = new SocketListener(socketConfig, this);
            m_sockets.Add(socketConfig.LocalEndPoint, listener);
        }

        /// <summary>
        /// Gets the database that matches <see cref="databaseName"/>
        /// </summary>
        /// <param name="databaseName">the case insensitive name of the databse</param>
        /// <returns></returns>
        private ServerDatabaseBase GetDatabase(string databaseName)
        {
            lock (m_syncRoot)
            {
                return m_databases[databaseName.ToUpper()];
            }
        }

        /// <summary>
        /// Adds the specified database to the collection
        /// </summary>
        /// <param name="database">The database to add</param>
        private void Add(ServerDatabaseBase database)
        {
            lock (m_syncRoot)
            {
                m_databases.Add(database.Info.DatabaseName.ToUpper(), database);
            }
        }

        /// <summary>
        /// Determines if <see cref="databaseName"/> is contained in the database.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns></returns>
        private bool Contains(string databaseName)
        {
            lock (m_syncRoot)
            {
                return m_databases.ContainsKey(databaseName.ToUpper());
            }
        }

        /// <summary>
        /// Gets basic information for every database connected to the server.
        /// </summary>
        /// <returns></returns>
        private List<DatabaseInfo> GetDatabaseInfo()
        {
            lock (m_syncRoot)
            {
                var lst = new List<DatabaseInfo>();
                foreach (var database in m_databases.Values)
                {
                    lst.Add(database.Info);
                }
                return lst;
            }
        }

        /// <summary>
        /// Detaches the provided database from the server. 
        /// </summary>
        /// <param name="databaseName">the name of the database</param>
        /// <param name="waitTimeSeconds">the time to wait for all clients to complete their queires 
        /// before terminating the query</param>
        private void Remove(string databaseName, float waitTimeSeconds = 0)
        {
            // TODO: Should this dispose of the database? Or is it assumed instance is not owned by collection...
            // TODO: waitSeconds is not used - is this for waiting to flush? need to remove otherwise
            lock (m_syncRoot)
            {
                m_databases.Remove(databaseName.ToUpper());
            }
        }

        /// <summary>
        /// Shuts down the entire server.
        /// </summary>
        /// <param name="waitTimeSeconds">the time to wait for all clients to complete their queires 
        /// before terminating the query</param>
        private void Shutdown(float waitTimeSeconds)
        {
            // TODO: Should this dispose of the database? Or is it assumed instance is not owned by collection...
            // TODO: waitSeconds is not used - is this for waiting to flush? need to remove otherwise
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
                foreach (ServerDatabaseBase db in m_databases.Values)
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates a client connection to the root of the server.
        /// </summary>
        /// <returns></returns>
        public Client CreateClientHost()
        {
            return new Client(this);
        }

        /// <summary>
        /// Creates a client connection to a specific database. 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public ClientDatabaseBase<TKey, TValue> CreateClientDatabase<TKey, TValue>(string databaseName)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return new ServerClientDatabaseWrapper<TKey, TValue>(this, databaseName);
        }

        /// <summary>
        /// Registers a client with the server host.
        /// </summary>
        /// <param name="client"></param>
        private void RegisterClient(Client client)
        {
            lock (m_syncRoot)
            {
                m_clients.Add(client);
            }
        }

        /// <summary>
        /// UnRegisters a client with the server host. 
        /// </summary>
        private void UnRegisterClient(Client client)
        {
            lock (m_syncRoot)
            {
                m_clients.Remove(client);
            }
        }

        public void GetFullStatus(StringBuilder status)
        {
            status.AppendFormat("Historian Instances:");
            foreach (var dbInfo in GetDatabaseInfo())
            {
                status.AppendFormat("DB Name:{0}\r\n", dbInfo.DatabaseName);
                GetDatabase(dbInfo.DatabaseName).GetFullStatus(status);
            }

            status.AppendFormat("Socket Connections");
            foreach (var socket in m_sockets)
            {
                status.AppendFormat("Port:{0}\r\n", socket.Key);
                var historian = socket.Value;
                historian.GetFullStatus(status);
            }
        }
    }
}