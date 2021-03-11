//******************************************************************************************************
//  HistorianServer.cs - Gbtc
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
//  12/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  07/24/2013 - J. Ritchie Carroll
//       Updated code to allow dynamic addition and removal of archive engines and associated sockets.
//
//******************************************************************************************************

using System;
using GSF.Snap.Services;
using GSF.Snap.Services.Net;

namespace openHistorian.Net
{
    /// <summary>
    /// Represents a historian server instance that can be used to read and write time-series data.
    /// </summary>
    public class HistorianServer : IDisposable
    {
        #region [ Members ]

        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="HistorianServer"/>
        /// </summary>
        public HistorianServer()
        {
            Host = new SnapServer();
        }

        /// <summary>
        /// Creates a new <see cref="HistorianServer"/> instance.
        /// </summary>
        public HistorianServer(int? port, string networkInterfaceIP = null)
        {
            ServerSettings server = new ServerSettings();
            
            if (port.HasValue || !string.IsNullOrWhiteSpace(networkInterfaceIP))
            {
                SnapSocketListenerSettings settings = new SnapSocketListenerSettings
                {
                    LocalTcpPort = port ?? SnapSocketListenerSettings.DefaultNetworkPort,
                    LocalIpAddress = networkInterfaceIP,
                    DefaultUserCanRead = true,
                    DefaultUserCanWrite = true,
                    DefaultUserIsAdmin = true
                };

                server.Listeners.Add(settings);
            }
            
            // Maintain a member level list of all established archive database engines
            Host = new SnapServer(server);
        }

        public HistorianServer(HistorianServerDatabaseConfig database, int? port = null, string networkInterfaceIP = null)
            : this(port, networkInterfaceIP)
        {
            AddDatabase(database);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the underlying host ending for the historian.
        /// </summary>
        public SnapServer Host { get; }

        /// <summary>
        /// Adds the supplied database to this server.
        /// </summary>
        /// <param name="database"></param>
        public void AddDatabase(HistorianServerDatabaseConfig database) => Host.AddDatabase(database);

        /// <summary>
        /// Removes the supplied database from the historian.
        /// </summary>
        /// <param name="database"></param>
        public void RemoveDatabase(string database) => Host.RemoveDatabase(database);

#if !SQLCLR
        /// <summary>
        /// Accesses <see cref="SnapServerDatabaseBase"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="SnapServerDatabaseBase"/> for given <paramref name="databaseName"/>.</returns>
        public HistorianIArchive this[string databaseName] => new HistorianIArchive(this, databaseName);
#endif

        #endregion

        #region [ Methods ]

        public void Dispose()
        {
            Host.Dispose();
        }

        #endregion
    }
}