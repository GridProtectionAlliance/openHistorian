//******************************************************************************************************
//  ServerHostConfig.cs - Gbtc
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

using System;
using System.Collections.Generic;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Services.Net;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    public class ServerConfig
    {
        /// <summary>
        /// Contains the configuration for the socket layer.
        /// </summary>
        public List<SocketListenerConfig> SocketConfig = new List<SocketListenerConfig>();
        /// <summary>
        /// Contains the configuration for the databases.
        /// </summary>
        public List<ServerDatabaseConfig> Databases = new List<ServerDatabaseConfig>();

        public ServerConfig()
        {

        }

        public static ServerConfig Create<TKey, TValue>(string path, int port = -1, string databaseName = null, EncodingDefinition encoding = null)
            where TKey : SortedTreeTypeBase, new()
            where TValue : SortedTreeTypeBase, new()
        {
            if ((object)databaseName == null)
                databaseName = string.Empty;
            if ((object)encoding == null)
                encoding = CreateFixedSizeCombinedEncoding.TypeGuid;

            ServerConfig config = new ServerConfig();
            if (port == 0)
            {
                SocketListenerConfig socket = new SocketListenerConfig();
                config.SocketConfig.Add(socket);
            }
            if (port > 0)
            {
                SocketListenerConfig socket = new SocketListenerConfig();
                socket.LocalTCPPort = port;
                config.SocketConfig.Add(socket);
            }

            ServerDatabaseConfig dbConfig = new ServerDatabaseConfig();
            dbConfig.DatabaseName = databaseName;
            dbConfig.EncodingMethod = encoding;
            dbConfig.MainPath = path;
            dbConfig.WriterMode = WriterMode.OnDisk;
            dbConfig.KeyType = new TKey().GenericTypeGuid;
            dbConfig.ValueType = new TValue().GenericTypeGuid;

            config.Databases.Add(dbConfig);
            return config;
        }

    }
}
