//******************************************************************************************************
//  ServerRoot.cs - Gbtc
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
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Collections;
using GSF.SortedTreeStore.Client;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Server
{
    /// <summary>
    /// Contains a named set of <see cref="ServerDatabase{TKey,TValue}"/>
    /// </summary>
    public class ServerRoot
        : ISortedTreeServer, IDisposable
    {
        private bool m_disposed;

        private readonly object m_syncRoot = new object();

        private readonly SortedList<string, ServerDatabaseBase> m_databases;

        private WeakList<ClientRootBase> m_clients;

        /// <summary>
        /// Creates a new instance of <see cref="ServerRoot"/>
        /// </summary>
        public ServerRoot()
        {
            m_clients = new WeakList<ClientRootBase>();
            m_databases = new SortedList<string, ServerDatabaseBase>();
        }

        /// <summary>
        /// Gets the database that matches <see cref="databaseName"/>
        /// </summary>
        /// <param name="databaseName">the case insensitive name of the databse</param>
        /// <returns></returns>
        public ServerDatabaseBase GetDatabase(string databaseName)
        {
            lock (m_syncRoot)
            {
                return m_databases[databaseName.ToUpper()];
            }
        }

        /// <summary>
        /// Accesses <see cref="ServerDatabase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="ServerDatabase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public ServerDatabase<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return (ServerDatabase<TKey, TValue>)(object)GetDatabase(databaseName);
        }

        /// <summary>
        /// Adds the specified database to the collection
        /// </summary>
        /// <param name="database">The database to add</param>
        public void Add(ServerDatabaseBase database)
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
        public bool Contains(string databaseName)
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
        public List<DatabaseInfo> GetDatabaseInfo()
        {
            lock (m_syncRoot)
            {
                var lst = new List<DatabaseInfo>();

                for (int x = 0; x < m_databases.Count; x++)
                {
                    lst.Add(m_databases.Values[x].Info);
                }
                return lst;
            }
        }

        /// <summary>
        /// A synchronization object.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return m_syncRoot;
            }
        }

        /// <summary>
        /// Detaches the provided database from the server. 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="waitTimeSeconds"></param>
        public void Remove(string databaseName, float waitTimeSeconds = 0)
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
        /// <param name="waitTimeSeconds"></param>
        public void Shutdown(float waitTimeSeconds)
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
                foreach (ServerDatabaseBase db in m_databases.Values)
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates a client connection to the server.
        /// </summary>
        /// <returns></returns>
        public ClientRootBase CreateClient()
        {
            lock (m_syncRoot)
            {
                var client = new LocalClientRoot(this, ReleaseClient);
                m_clients.Add(client);
                return client;
            }
        }

        /// <summary>
        /// Releases a client
        /// </summary>
        void ReleaseClient(LocalClientRoot client)
        {
            lock (m_syncRoot)
            {
                m_clients.Remove(client);
            }
        }
    }
}