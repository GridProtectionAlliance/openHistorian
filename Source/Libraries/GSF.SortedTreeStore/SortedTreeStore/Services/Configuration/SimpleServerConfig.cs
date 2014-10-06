////******************************************************************************************************
////  SimpleServerConfig.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  10/05/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System.Collections.Generic;
//using GSF.SortedTreeStore.Encoding;
//using GSF.SortedTreeStore.Services.Net;
//using GSF.SortedTreeStore.Tree;

//namespace GSF.SortedTreeStore.Services.Configuration
//{
//    /// <summary>
//    /// Provides a convient way to initialize a <see cref="ServerSettings"/> object.
//    /// </summary>
//    /// <typeparam name="TKey"></typeparam>
//    /// <typeparam name="TValue"></typeparam>
//    public class SimpleServerConfig<TKey, TValue>
//        : IToServerSettings
//        where TKey : SortedTreeTypeBase<TKey>, new()
//        where TValue : SortedTreeTypeBase<TValue>, new()
//    {

//        /// <summary>
//        /// The name of the database
//        /// </summary>
//        public string DatabaseName = string.Empty;
//        /// <summary>
//        /// The network socket port if a listener is defined.
//        /// </summary>
//        public int? SocketListenPort = null;
//        /// <summary>
//        /// The main path to the archive directory
//        /// </summary>
//        public string MainPath = string.Empty;
//        /// <summary>
//        /// Available write paths for the archive
//        /// </summary>
//        public List<string> WritePaths = new List<string>();
//        /// <summary>
//        /// The method for encoding 
//        /// </summary>
//        public EncodingDefinition ArchiveEncoding = CreateFixedSizeCombinedEncoding.TypeGuid;
//        /// <summary>
//        /// The supported streaming methods for the network interface.
//        /// </summary>
//        public List<EncodingDefinition> StreamingEncodingMethods = new List<EncodingDefinition>();

//        /// <summary>
//        /// Creates a <see cref="ServerSettings"/> configuration that can be used for <see cref="Server"/>
//        /// </summary>
//        /// <returns></returns>
//        public ServerSettings ToServerSettings()
//        {
//            AdvancedServerDatabaseConfig<TKey, TValue> config = new AdvancedServerDatabaseConfig<TKey, TValue>(DatabaseName, MainPath, true);
//            ServerConfig config = new ServerConfig();
//            {
//                SocketListenerSettings socket = new SocketListenerSettings();
//                config.SocketConfig.Add(socket);
//            }
//            if (SocketListenPort.HasValue)
//            {
//                SocketListenerSettings socket = new SocketListenerSettings();
//                socket.LocalTCPPort = SocketListenPort.Value;
//                config.SocketConfig.Add(socket);
//            }

//            ServerDatabaseConfig dbConfig = new ServerDatabaseConfig();
//            dbConfig.DatabaseName = DatabaseName;
//            dbConfig.ArchiveEncodingMethod = ArchiveEncoding;

//            dbConfig.StreamingEncodingMethods.AddRange(StreamingEncodingMethods);
//            if (dbConfig.StreamingEncodingMethods.Count == 0)
//                dbConfig.StreamingEncodingMethods.Add(CreateFixedSizeCombinedEncoding.TypeGuid);
//            dbConfig.MainPath = MainPath;
//            if (WritePaths.Count == 0)
//            {
//                dbConfig.FinalWritePaths.AddRange(WritePaths);
//            }
//            dbConfig.WriterMode = WriterMode.OnDisk;
//            dbConfig.KeyType = new TKey().GenericTypeGuid;
//            dbConfig.ValueType = new TValue().GenericTypeGuid;

//            config.Databases.Add(dbConfig);
//            return config.ToServerSettings();

//        }

//        public IToServerSettings Clone()
//        {
//            return ToServerSettings();
//        }
//    }
//}
