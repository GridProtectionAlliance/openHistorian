//******************************************************************************************************
//  HistorianServer.cs - Gbtc
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
//  12/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  07/24/2013 - J. Ritchie Carroll
//       Updated code to allow dynamic addition and removal of archive engines and associated sockets.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Net;
using GSF.SortedTreeStore.Tree.TreeNodes;
using openHistorian.Collections;
using GSF.SortedTreeStore.Engine;

namespace openHistorian
{
    /// <summary>
    /// Represents a historian server instance that can be used to read and write time-series data.
    /// </summary>
    public class HistorianServer
        : IDisposable
    {
        #region [ Members ]

        // Fields
        private Dictionary<int, SortedTreeServerSocket> m_sockets;
        private SortedTreeCollection m_databases;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="HistorianServer"/> instance.
        /// </summary>
        public HistorianServer()
        {
            // Maintain a member level list of all established archive database engines
            m_databases = new SortedTreeCollection();

            // Maintain a member level list of socket connections so that they can be disposed later
            m_sockets = new Dictionary<int, SortedTreeServerSocket>();
        }

        /// <summary>
        /// Creates a new <see cref="HistorianServer"/> instance for given <paramref name="databaseInstance"/>.
        /// </summary>
        /// <param name="databaseInstance"><see cref="HistorianDatabaseInstance"/> to initialize.</param>
        public HistorianServer(HistorianDatabaseInstance databaseInstance)
            : this(new[] { databaseInstance })
        {
        }

        /// <summary>
        /// Creates a new <see cref="HistorianServer"/> instance for given <paramref name="databaseInstances"/>.
        /// </summary>
        /// <param name="databaseInstances">Collection of <see cref="HistorianDatabaseInstance"/>s to initialize.</param>
        public HistorianServer(IEnumerable<HistorianDatabaseInstance> databaseInstances)
            : this()
        {
            Initialize(databaseInstances);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="HistorianServer"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~HistorianServer()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Accesses <see cref="SortedTreeEngineBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="SortedTreeEngineBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public HistorianDatabaseEngine this[string databaseName]
        {
            get
            {
                return m_databases.GetDatabase(databaseName) as HistorianDatabaseEngine;
            }
        }

        #endregion

        #region [ Methods ]

        public void GetFullStatus(StringBuilder status)
        {
            status.AppendFormat("Historian Instances:");
            foreach (var dbInfo in m_databases.GetDatabaseInfo())
            {
                status.AppendFormat("DB Name:{0}\r\n", dbInfo.DatabaseName);
                this[dbInfo.DatabaseName].GetFullStatus(status);
            }

            status.AppendFormat("Socket Connections");
            foreach (var socket in m_sockets)
            {
                status.AppendFormat("Port:{0}\r\n", socket.Key);
                var historian = socket.Value;
                historian.GetFullStatus(status);
            }
        }


        /// <summary>
        /// Releases all the resources used by the <see cref="HistorianServer"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HistorianServer"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if ((object)m_databases != null)
                            m_databases.Dispose();

                        m_databases = null;

                        if ((object)m_sockets != null)
                        {
                            foreach (SortedTreeServerSocket socketHistorian in m_sockets.Values)
                            {
                                if ((object)socketHistorian != null)
                                    socketHistorian.Dispose();
                            }
                        }

                        m_sockets = null;
                    }
                }
                finally
                {
                    m_disposed = true; // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Gets default database instance, if it exists.
        /// </summary>
        /// <returns>Default database instance.</returns>
        public HistorianDatabaseEngine GetDefaultDatabase()
        {
            return m_databases.GetDatabase("default") as HistorianDatabaseEngine;
        }

        /// <summary>
        /// Adds a new database instance to the <see cref="HistorianServer"/>.
        /// </summary>
        /// <param name="databaseInstance"><see cref="HistorianDatabaseInstance"/> to add.</param>
        public void AddDatabaseInstance(HistorianDatabaseInstance databaseInstance)
        {
            HistorianDatabaseEngine databaseEngine;

            if (databaseInstance.InMemoryArchive)
                databaseEngine = new HistorianDatabaseEngine(databaseInstance.DatabaseName, WriterMode.InMemory, databaseInstance.Paths);
            else
                databaseEngine = new HistorianDatabaseEngine(databaseInstance.DatabaseName, WriterMode.OnDisk, databaseInstance.Paths);

            m_databases.Add(databaseEngine);

            if (databaseInstance.IsNetworkHosted)
            {
                // TODO: The "add" method can only add a new socket layer - not append new database to existing socket historian (note that this will work for time-series
                // TODO: adapters, but not for general use case when sharing port for multiple databases is desired), to fix this SocketHistorian needs to be modified to
                // TODO: allow dynamic addition of databases to its collection (or at least collection replacement)
                SortedTreeCollection databaseCollection = new SortedTreeCollection();
                databaseCollection.Add(databaseEngine);

                lock (m_sockets)
                {
                    m_sockets.Add(databaseInstance.PortNumber, new SortedTreeServerSocket(databaseInstance.PortNumber, databaseCollection));
                }
            }
        }

        /// <summary>
        /// Removes an existing database instance from the <see cref="HistorianServer"/>.
        /// </summary>
        /// <param name="databaseInstance"><see cref="HistorianDatabaseInstance"/> to remove.</param>
        public void RemoveDatabaseInstance(HistorianDatabaseInstance databaseInstance)
        {
            SortedTreeServerSocket sortedTreeServerSocket;

            lock (m_databases.SyncRoot)
            {
                if (m_databases.Contains(databaseInstance.DatabaseName))
                {
                    m_databases.GetDatabase(databaseInstance.DatabaseName).Dispose();
                    m_databases.Remove(databaseInstance.DatabaseName);
                }
            }

            // TODO: This method currently removes the socket layer for all databases available on this port (note that this will work for time-series adapters,
            // TODO: but will break general use case), to fix this SocketHistorian needs to be modified to allow dynamic removal of databases from its collection
            // TODO: (or at least collection replacement)
            lock (m_sockets)
            {
                if (m_sockets.TryGetValue(databaseInstance.PortNumber, out sortedTreeServerSocket))
                {
                    if ((object)sortedTreeServerSocket != null)
                    {
                        sortedTreeServerSocket.Dispose();
                    }

                    m_sockets.Remove(databaseInstance.PortNumber);
                }
            }
        }

        /// <summary>
        /// Initializes collection of <see cref="HistorianDatabaseInstance"/>s.
        /// </summary>
        /// <param name="databaseInstances">Collection of <see cref="HistorianDatabaseInstance"/>s to initialize.</param>
        protected void Initialize(IEnumerable<HistorianDatabaseInstance> databaseInstances)
        {
            // Create socket specific historian database collections
            Dictionary<int, SortedTreeCollection> socketDatabases = new Dictionary<int, SortedTreeCollection>();
            SortedTreeCollection databaseCollection;
            HistorianDatabaseEngine databaseEngine;

            // Initialize each archive database engine
            foreach (HistorianDatabaseInstance databaseInstance in databaseInstances)
            {
                if (databaseInstance.InMemoryArchive)
                    databaseEngine = new HistorianDatabaseEngine(databaseInstance.DatabaseName, WriterMode.InMemory, databaseInstance.Paths);
                else
                    databaseEngine = new HistorianDatabaseEngine(databaseInstance.DatabaseName, WriterMode.OnDisk, databaseInstance.Paths);

                m_databases.Add(databaseEngine);

                if (databaseInstance.IsNetworkHosted)
                {
                    // Maintain a per socket collection of archive database engines
                    int port = databaseInstance.PortNumber;

                    if (!socketDatabases.TryGetValue(port, out databaseCollection))
                    {
                        databaseCollection = new SortedTreeCollection();
                        socketDatabases.Add(port, databaseCollection);
                    }

                    // Add database associated with specific socket
                    socketDatabases[port].Add(databaseEngine);
                }
            }

            // Create a new instance of the socket historian per-port with associated database collections
            lock (m_sockets)
            {
                foreach (KeyValuePair<int, SortedTreeCollection> connection in socketDatabases)
                {
                    m_sockets.Add(connection.Key, new SortedTreeServerSocket(connection.Key, connection.Value));
                }
            }
        }

        #endregion

    }

    #region [ Old Code ]

    //public IHistorianDatabaseCollection<HistorianKey, HistorianValue> GetDatabaseCollection()
    //{
    //    return m_databases;
    //}

    // Removed this class - was not necessary - users can create simple collections of HistorianServerOptions
    //public class HistorianServerDatabaseCollectionOptions
    //{
    //    public bool IsNetworkHosted = false;

    //    public string ConnectionString = "port=38402";

    //    public List<HistorianServerOptions> Databases = new List<HistorianServerOptions>();
    //}

    // Removed this class - was redundant with HistorianServerOptions
    //public class HistorianDatabaseConnection
    //{
    //    public List<string> Paths = new List<string>();

    //    public bool InMemoryArchive = true;

    //    public string DatabaseName;
    //}

    #endregion
}