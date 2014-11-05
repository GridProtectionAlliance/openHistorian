//******************************************************************************************************
//  NetworkServer.cs - Gbtc
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
//  12/08/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Net.Sockets;
using System.Text;
using GSF.Diagnostics;
using GSF.Security;
using GSF.Security.Authentication;

namespace GSF.SortedTreeStore.Services.Net
{
    /// <summary>
    /// This is a single server socket that handles an individual client connection.
    /// </summary>
    internal class NetworkServer
        : StreamingServer
    {
        private bool m_disposed;
        private TcpClient m_client;
        private NetworkStream m_rawStream;

        public NetworkServer(SecureStreamServer<SocketUserPermissions> authentication, TcpClient client, Server server, LogSource parent, bool requireSsl = false)
            : base(parent)
        {
            m_client = client;
            m_rawStream = new NetworkStream(m_client.Client);
            m_client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

            Initialize(authentication, m_rawStream, server, requireSsl);
        }

        public void GetFullStatus(StringBuilder status)
        {
            try
            {
                status.AppendLine(m_client.Client.RemoteEndPoint.ToString());
            }
            catch (Exception)
            {
                status.AppendLine("Error getting remote endpoint");
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="NetworkServer"/> object and optionally releases the managed resources.
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
                        // This will be done only when the object is disposed by calling Dispose().
                        m_client.Client.Shutdown(SocketShutdown.Both);
                        m_client.Close();
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