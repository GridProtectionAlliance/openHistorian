//******************************************************************************************************
//  Client.cs - Gbtc
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
//  05/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Diagnostics;
using GSF.SortedTreeStore.Services.Net;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{

    /// <summary>
    /// Represents a client connection to a <see cref="Server"/>.
    /// </summary>
    public abstract class Client
        : LogSourceBase
    {
        private bool m_disposed;

        /// <summary>
        /// Gets the database that matches <see cref="databaseName"/>
        /// </summary>
        /// <param name="databaseName">the case insensitive name of the databse</param>
        /// <returns></returns>
        public abstract ClientDatabaseBase GetDatabase(string databaseName);

        /// <summary>
        /// Accesses <see cref="ClientDatabaseBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="ClientDatabaseBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public abstract ClientDatabaseBase<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new();

        /// <summary>
        /// Gets basic information for every database connected to the server.
        /// </summary>
        /// <returns></returns>
        public abstract List<DatabaseInfo> GetDatabaseInfo();

        /// <summary>
        /// Determines if <see cref="databaseName"/> is contained in the database.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns></returns>
        public abstract bool Contains(string databaseName);

        #region [ Static ]

        /// <summary>
        /// Connects to a local <see cref="Server"/>.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static Client Connect(Server host)
        {
            return new Server.Client(host);
        }

        /// <summary>
        /// Connects to a server over a network socket.
        /// </summary>
        /// <param name="serverOrIp">The name of the server to connect to, or the IP address to use.</param>
        /// <param name="port">The port number to connect to.</param>
        /// <param name="password">The password for connecting to the server.</param>
        /// <returns>A <see cref="Client"/></returns>
        public static Client Connect(string serverOrIp, int port, string password)
        {
            NetworkClientConfig config = new NetworkClientConfig();
            config.ServerNameOrIp = serverOrIp;
            config.NetworkPort = port;
            config.Password = password;
            config.IsReadOnly = false;
            return new NetworkClient(config);
        }

        #endregion

    }
}
