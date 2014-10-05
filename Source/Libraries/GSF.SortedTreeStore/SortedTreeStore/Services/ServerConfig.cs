//******************************************************************************************************
//  ServerConfig.cs - Gbtc
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
//  05/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Collections.Generic;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Services.Net;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Specifies the configuration data of the <see cref="Server"/>.
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// Contains the configuration for the socket layer.
        /// </summary>
        public readonly List<SocketListenerSettings> SocketConfig = new List<SocketListenerSettings>();
        /// <summary>
        /// Contains the configuration for the databases.
        /// </summary>
        public readonly List<ServerDatabaseConfig> Databases = new List<ServerDatabaseConfig>();

        /// <summary>
        /// Creates a <see cref="ServerConfig"/> with some basic parameters set.
        /// </summary>
        /// <typeparam name="TKey">The key for the default database</typeparam>
        /// <typeparam name="TValue">the value for the default database</typeparam>
        /// <param name="path">the main import path of the server</param>
        /// <param name="port">the port number to use. -1 means do not network host. 0 means use the default port number.</param>
        /// <param name="databaseName">the name of the default database. if left null, <see cref="string.Empty"/> is substituted</param>
        /// <param name="archiveEncoding">the encoding to write the archive file with. Null will default to a fixed size encoding.</param>
        /// <param name="streamEncoding">the prefered encoding to use for a network stream. Null will default to a fixed size encoding. </param>
        /// <returns></returns>
        public static ServerConfig Create<TKey, TValue>(string path, int port = -1, string databaseName = null, EncodingDefinition archiveEncoding = null, EncodingDefinition streamEncoding = null)
            where TKey : SortedTreeTypeBase, new()
            where TValue : SortedTreeTypeBase, new()
        {
            if ((object)databaseName == null)
                databaseName = string.Empty;
            if ((object)archiveEncoding == null)
                archiveEncoding = CreateFixedSizeCombinedEncoding.TypeGuid;


            ServerConfig config = new ServerConfig();
            if (port == 0)
            {
                SocketListenerSettings socket = new SocketListenerSettings();
                config.SocketConfig.Add(socket);
            }
            if (port > 0)
            {
                SocketListenerSettings socket = new SocketListenerSettings();
                socket.LocalTCPPort = port;
                config.SocketConfig.Add(socket);
            }

            ServerDatabaseConfig dbConfig = new ServerDatabaseConfig();
            dbConfig.DatabaseName = databaseName;
            dbConfig.ArchiveEncodingMethod = archiveEncoding;

            if ((object)streamEncoding != null)
                dbConfig.StreamingEncodingMethods.Add(streamEncoding);
            dbConfig.StreamingEncodingMethods.Add(CreateFixedSizeCombinedEncoding.TypeGuid);
            dbConfig.MainPath = path;
            dbConfig.WriterMode = WriterMode.OnDisk;
            dbConfig.KeyType = new TKey().GenericTypeGuid;
            dbConfig.ValueType = new TValue().GenericTypeGuid;
          
            config.Databases.Add(dbConfig);
            return config;
        }

    }
}
