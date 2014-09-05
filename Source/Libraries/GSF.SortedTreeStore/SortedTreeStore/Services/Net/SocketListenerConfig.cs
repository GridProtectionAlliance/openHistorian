//******************************************************************************************************
//  SocketListenerConfig.cs - Gbtc
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
using System.Net;

namespace GSF.SortedTreeStore.Services.Net
{
    /// <summary>
    /// Contains the basic config for a socket interface.
    /// </summary>
    public class SocketListenerConfig
    {
        /// <summary>
        /// Defines the default network port for a <see cref="SocketListener"/>.
        /// </summary>
        public const int DefaultNetworkPort = 38402;

        /// <summary>
        /// Defines the default network IP address for the <see cref="SocketListener"/>.
        /// </summary>
        public const string DefaultIPAddress = "127.0.0.1";

        /// <summary>
        /// A server name that must be supplied at startup before a key exchange occurs.
        /// </summary>
        public const string DefaultServerName = "openHistorian";

        /// <summary>
        /// The local IP address to host on. Leave empty to bind to all local interfaces.
        /// </summary>
        public string LocalIPAddress = DefaultIPAddress;

        /// <summary>
        /// The local TCP port to host on. 
        /// </summary>
        public int LocalTCPPort = DefaultNetworkPort;

        /// <summary>
        /// A server name that must be supplied at startup before a key exchange occurs.
        /// </summary>
        public string ServerName = DefaultServerName;

        /// <summary>
        /// A list of all windows users that are allowed to connnect to the historian.
        /// </summary>
        public readonly List<string> Users = new List<string>();

        /// <summary>
        /// Gets the local <see cref="IPEndPoint"/> from the values in <see cref="LocalIPAddress"/> and <see cref="LocalTCPPort"/>
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LocalIPAddress))
                {
                    return new IPEndPoint(IPAddress.Any, LocalTCPPort);
                }
                return new IPEndPoint(IPAddress.Parse(LocalIPAddress), LocalTCPPort);
            }
        }
    }
}
