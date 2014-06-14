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
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Tree.TreeNodes;
using openHistorian.Collections;

namespace openHistorian
{
    /// <summary>
    /// Represents a historian server instance that can be used to read and write time-series data.
    /// </summary>
    public class HistorianServer : IDisposable
    {
        #region [ Members ]

        // Fields
        private Server m_host;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public HistorianServer()
        {
            m_host = new Server();
        }

        /// <summary>
        /// 
        /// </summary>
        public HistorianServer(string path)
            : this(
                ServerConfig.Create<HistorianKey, HistorianValue>(path, 0, null, CreateHistorianCompressionTs.TypeGuid))
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public HistorianServer(string path, int port)
            : this(ServerConfig.Create<HistorianKey, HistorianValue>(path, port, null, CreateHistorianCompressionTs.TypeGuid))
        {

        }

        /// <summary>
        /// Creates a new <see cref="HistorianServer"/> instance.
        /// </summary>
        public HistorianServer(ServerConfig config)
        {
            // Maintain a member level list of all established archive database engines
            m_host = new Server(config);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the underlying host ending for the historian.
        /// </summary>
        public Server Host
        {
            get
            {
                return m_host;
            }
        }

        /// <summary>
        /// Accesses <see cref="ServerDatabaseBase"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="ServerDatabaseBase"/> for given <paramref name="databaseName"/>.</returns>
        public HistorianIArchive this[string databaseName]
        {
            get
            {
                return new HistorianIArchive(this, databaseName);
            }
        }





        #endregion

        #region [ Methods ]

        public void Dispose()
        {
            m_host.Dispose();
        }

        #endregion


    }

}