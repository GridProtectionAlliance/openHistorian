//******************************************************************************************************
//  NetworkClient.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Net;
using System.Net.Sockets;
using GSF.Security;

namespace GSF.SortedTreeStore.Services.Net
{
    /// <summary>
    /// A client that communicates over a network socket.
    /// </summary>
    public class NetworkClient
        : StreamingClient
    {
        private bool m_disposed;
        private TcpClient m_client;

        /// <summary>
        /// Creates a <see cref="NetworkClient"/>
        /// </summary>
        /// <param name="config">The config to use for the client</param>
        /// <param name="credentials">The network credentials to use. 
        /// If left null, the computers current credentials are use.</param>
        /// <param name="useSsl">Specifies if ssl encryption is desired for the connection.</param>
        public NetworkClient(NetworkClientConfig config, SecureStreamClientBase credentials = null, bool useSsl = false)
        {
            if (credentials == null)
                credentials = new SecureStreamClientIntegratedSecurity();

            m_client = new TcpClient(AddressFamily.InterNetworkV6);
            m_client.Client.DualMode = true;

            IPAddress ip;
            if (!IPAddress.TryParse(config.ServerNameOrIp, out ip))
            {
                ip = Dns.GetHostAddresses(config.ServerNameOrIp)[0];
            }

            IPEndPoint server = new IPEndPoint(ip, config.NetworkPort);
            m_client.Connect(server);

            m_client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            Initialize(new NetworkStream(m_client.Client), credentials, useSsl);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="NetworkClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {

            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {

                        try
                        {
                            m_client.Client.Shutdown(SocketShutdown.Both);
                            m_client.Close();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }
    }
}