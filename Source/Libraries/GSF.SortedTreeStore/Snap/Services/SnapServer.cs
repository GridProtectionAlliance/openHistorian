//******************************************************************************************************
//  SnapServer.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/09/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GSF.Collections;
using GSF.Diagnostics;
using GSF.Snap.Services.Net;

namespace GSF.Snap.Services
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
    public partial class SnapServer
        : DisposableLoggingClassBase
    {
        private bool m_disposed;
        private readonly object m_syncRoot = new object();
        
        /// <summary>
        /// Contains a list of databases that are UPPER case names.
        /// </summary>
        private readonly Dictionary<string, SnapServerDatabaseBase> m_databases;
        
        /// <summary>
        /// Contains a list of all clients such that a strong reference will not be maintained.
        /// </summary>
        private readonly WeakList<Client> m_clients;
        
        /// <summary>
        /// All of the socket listener per IPEndPoint.
        /// </summary>
        private readonly Dictionary<IPEndPoint, SnapSocketListener> m_sockets;

        /// <summary>
        /// Creates an empty server instance
        /// </summary>
        public SnapServer()
            : base(MessageClass.Framework)
        {
            m_sockets = new Dictionary<IPEndPoint, SnapSocketListener>();
            m_clients = new WeakList<Client>();
            m_databases = new Dictionary<string, SnapServerDatabaseBase>();

            Log.Publish(MessageLevel.Info, "Server Constructor Called");
        }

        /// <summary>
        /// Creates a new instance of <see cref="SnapServer"/> and adds the supplied database
        /// </summary>
        public SnapServer(IToServerDatabaseSettings settings)
            : this()
        {
            AddDatabase(settings);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SnapServer"/>
        /// </summary>
        public SnapServer(IToServerSettings settings)
            : this()
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));
            
            ServerSettings settings2 = settings.ToServerSettings();

            if (settings2 is null)
                throw new ArgumentNullException(nameof(settings), "The ToServerSettings method returned null");
            
            settings2.Validate();

            foreach (ServerDatabaseSettings db in settings2.Databases)
                AddDatabase(db);
            
            foreach (SnapSocketListenerSettings list in settings2.Listeners)
                AddSocketListener(list);
        }

        public void AddDatabase(IToServerDatabaseSettings databaseConfig)
        {
            AddDatabase(databaseConfig.ToServerDatabaseSettings());
        }

        /// <summary>
        /// Adds a database to the server
        /// </summary>
        /// <param name="databaseConfig"></param>
        public void AddDatabase(ServerDatabaseSettings databaseConfig)
        {
            if (databaseConfig is null)
                throw new ArgumentNullException(nameof(databaseConfig));

            // Pre check to prevent loading a database with the same name twice.
            lock (m_syncRoot)
            {
                if (m_databases.ContainsKey(databaseConfig.DatabaseName.ToUpper()))
                {
                    Log.Publish(MessageLevel.Error, "Database Already Exists", "Adding a database that already exists in the server: " + databaseConfig.DatabaseName);
                    return;
                }
            }

            SnapServerDatabaseBase database;
            
            try
            {
                using (Logger.AppendStackMessages(Log.InitialStackMessages))
                {
                    database = SnapServerDatabaseBase.CreateDatabase(databaseConfig);
                }
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Critical, "Database failed to load.", databaseConfig.DatabaseName, null, ex);
                return;
            }

            string databaseName = database.Info.DatabaseName.ToUpper();

            lock (m_syncRoot)
            {
                if (m_databases.ContainsKey(databaseName))
                {
                    Log.Publish(MessageLevel.Error, "Database Already Exists", "Adding a database that already exists in the server: " + databaseName);
                    database.Dispose();
                }
                else
                {
                    Log.Publish(MessageLevel.Info, "Added Database", "Adding a database to the server: " + databaseName);
                    m_databases.Add(database.Info.DatabaseName.ToUpper(), database);
                }
            }
        }

        /// <summary>
        /// Adds the socket interface to the database
        /// </summary>
        /// <param name="socketSettings">the config data for the socket listener</param>
        public void AddSocketListener(SnapSocketListenerSettings socketSettings)
        {
            if (socketSettings is null)
                throw new ArgumentNullException(nameof(socketSettings));

            using (Logger.AppendStackMessages(Log.InitialStackMessages))
            {
                SnapSocketListener listener = new SnapSocketListener(socketSettings, this);
                
                lock (m_syncRoot)
                {
                    m_sockets.Add(socketSettings.LocalEndPoint, listener);
                }
            }
        }


        /// <summary>
        /// Unloads the database name.
        /// </summary>
        /// <param name="database"></param>
        public void RemoveDatabase(string database)
        {
            // TODO: Should this dispose of the database? Or is it assumed instance is not owned by collection...
            // TODO: waitSeconds is not used - is this for waiting to flush? need to remove otherwise
            SnapServerDatabaseBase db;
            
            lock (m_syncRoot)
            {
                db = m_databases[database.ToUpper()];
                m_databases.Remove(database.ToUpper());
            }
            
            db.Dispose();
        }

        /// <summary>
        /// Unloads the specified socket interface.
        /// </summary>
        /// <param name="socketEndpoint"></param>
        public void UnloadSocket(IPEndPoint socketEndpoint)
        {
            SnapSocketListener listener;
            
            lock (m_syncRoot)
            {
                listener = m_sockets[socketEndpoint];
                m_sockets.Remove(socketEndpoint);
            }
            
            listener.Dispose();
        }

        /// <summary>
        /// Gets the database that matches <see cref="databaseName"/>
        /// </summary>
        /// <param name="databaseName">the case insensitive name of the database</param>
        /// <returns></returns>
        private SnapServerDatabaseBase GetDatabase(string databaseName)
        {
            lock (m_syncRoot)
                return m_databases[databaseName.ToUpper()];
        }

        /// <summary>
        /// Determines if <see cref="databaseName"/> is contained in the database.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns></returns>
        private bool Contains(string databaseName)
        {
            lock (m_syncRoot)
                return m_databases.ContainsKey(databaseName.ToUpper());
        }

        /// <summary>
        /// Gets basic information for every database connected to the server.
        /// </summary>
        /// <returns></returns>
        private List<DatabaseInfo> GetDatabaseInfo()
        {
            lock (m_syncRoot)
                return m_databases.Values.Select(database => database.Info).ToList();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SnapServer"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (!disposing)
                    return;

                foreach (SnapSocketListener socket in m_sockets.Values)
                    socket.Dispose();
                        
                m_sockets.Clear();
                        
                foreach (SnapServerDatabaseBase db in m_databases.Values)
                    db.Dispose();
                        
                m_databases.Clear();
            }
            finally
            {
                m_disposed = true;       // Prevent duplicate dispose.
                base.Dispose(disposing); // Call base class Dispose().
            }
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
                m_clients.Remove(client);
        }

        /// <summary>
        /// Gets the status of the server.
        /// </summary>
        /// <param name="status">Target status output <see cref="StringBuilder"/>.</param>
        /// <param name="maxFileListing">Maximum file listing.</param>
        public void GetFullStatus(StringBuilder status, int maxFileListing = -1)
        {
            lock (m_syncRoot)
            {
                status.AppendLine($"Historian Instances:{Environment.NewLine}");
                int count = 0;
                
                foreach (DatabaseInfo dbInfo in GetDatabaseInfo())
                {
                    status.AppendLine($"Instance {++count:N0}: {dbInfo.DatabaseName}{Environment.NewLine}");
                    
                    try
                    {
                        GetDatabase(dbInfo.DatabaseName).GetFullStatus(status, maxFileListing);
                    }
                    catch (Exception ex)
                    {
                        Log.Publish(MessageLevel.Warning, "Full Status", $"Failed to get full status for {dbInfo.DatabaseName}", exception: ex);
                    }
                }

                status.AppendLine($"{Environment.NewLine}Socket Connections:{Environment.NewLine}");
                count = 0;

                foreach (KeyValuePair<IPEndPoint, SnapSocketListener> socket in m_sockets)
                {
                    status.AppendLine($"Connection {++count:N0}: Port: {socket.Key}{Environment.NewLine}");
                    
                    try
                    {
                        SnapSocketListener historian = socket.Value;
                        historian.GetFullStatus(status);
                    }
                    catch (Exception ex)
                    {
                        Log.Publish(MessageLevel.Warning, "Full Status", $"Failed to get full status for port {socket.Key}", exception: ex);
                    }
                }
            }
        }
    }
}