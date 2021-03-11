//******************************************************************************************************
//  SnapSocketListenerSettings.cs - Gbtc
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
//  05/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//  08/15/2019 - J. Ritchie Carroll
//       Updated to allow for IPv6 bindings.
//
//******************************************************************************************************

using System.Data;
using System.IO;
using System.Net;
#if !SQLCLR
using GSF.Communication;
#endif
using GSF.Immutable;
using GSF.IO;

// ReSharper disable RedundantDefaultMemberInitializer
namespace GSF.Snap.Services.Net
{
    /// <summary>
    /// Contains the basic config for a socket interface.
    /// </summary>
    public class SnapSocketListenerSettings
        : SettingsBase<SnapSocketListenerSettings>
    {
        /// <summary>
        /// Defines the default network port for a <see cref="SnapSocketListener"/>.
        /// </summary>
        public const int DefaultNetworkPort = 38402;

        /// <summary>
        /// Defines the default network IP address for the <see cref="SnapSocketListener"/>.
        /// </summary>
        public const string DefaultIPAddress = "";

        /// <summary>
        /// A server name that must be supplied at startup before a key exchange occurs.
        /// </summary>
        public const string DefaultServerName = "openHistorian";

        /// <summary>
        /// The local IP address to host on. Leave empty to bind to all local interfaces.
        /// </summary>
        private string m_localIpAddress = DefaultIPAddress;

        /// <summary>
        /// The local TCP port to host on. 
        /// </summary>
        private int m_localTcpPort = DefaultNetworkPort;

        /// <summary>
        /// A server name that must be supplied at startup before a key exchange occurs.
        /// </summary>
        private string m_serverName = DefaultServerName;

        /// <summary>
        /// Force the use of SSL for all clients connecting to this socket.
        /// </summary>
        private bool m_forceSsl = false;

        /// <summary>
        /// Gets the local <see cref="IPEndPoint"/> from the values in <see cref="m_localIpAddress"/> and <see cref="m_localTcpPort"/>
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get
            {
#if SQLCLR
                if (string.IsNullOrWhiteSpace(m_localIpAddress))
                    return new IPEndPoint(IPAddress.Any, m_localTcpPort);

                return new IPEndPoint(IPAddress.Parse(m_localIpAddress), m_localTcpPort);
#else
                // SnapSocketListener automatically enables dual-stack socket for IPv6 to support legacy client implementations expecting IPv4 hosting
                IPStack ipStack = string.IsNullOrWhiteSpace(m_localIpAddress) ? Transport.GetDefaultIPStack() : Transport.IsIPv6IP(m_localIpAddress) ? IPStack.IPv6 : IPStack.IPv4;               

                return Transport.CreateEndPoint(m_localIpAddress, m_localTcpPort, ipStack);
#endif                
            }
        }

        /// <summary>
        /// A list of all Windows users that are allowed to connect to the historian.
        /// </summary>
        public ImmutableList<string> Users { get; } = new ImmutableList<string>();

        /// <summary>
        /// Force the use of SSL for all clients connecting to this socket.
        /// </summary>
        public bool ForceSsl
        {
            get => m_forceSsl;
            set
            {
                TestForEditable();
                m_forceSsl = value;
            }
        }

        /// <summary>
        /// The local TCP port to host on. 
        /// </summary>
        public int LocalTcpPort
        {
            get => m_localTcpPort;
            set
            {
                TestForEditable();
                m_localTcpPort = value;
            }
        }

        /// <summary>
        /// The local IP address to host on. Leave empty to bind to all local interfaces.
        /// </summary>
        public string LocalIpAddress
        {
            get => m_localIpAddress;
            set
            {
                TestForEditable();
                m_localIpAddress = value;
            }
        }

        public bool DefaultUserCanRead = false;
        public bool DefaultUserCanWrite = false;
        public bool DefaultUserIsAdmin = false;

        public override void Save(Stream stream)
        {
            stream.Write((byte)1);
        }

        public override void Load(Stream stream)
        {
            TestForEditable();
            
            byte version = stream.ReadNextByte();

            if (version != 1)
                throw new VersionNotFoundException("Unknown Version Code: " + version);
        }

        public override void Validate()
        {
        }
    }
}
