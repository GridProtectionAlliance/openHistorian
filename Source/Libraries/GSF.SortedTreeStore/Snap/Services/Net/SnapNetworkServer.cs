//******************************************************************************************************
//  SnapNetworkServer.cs - Gbtc
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

namespace GSF.Snap.Services.Net
{
    /// <summary>
    /// This is a single server socket that handles an individual client connection.
    /// </summary>
    internal class SnapNetworkServer
        : SnapStreamingServer
    {
        private bool m_disposed;
        private readonly TcpClient m_client;
        private readonly NetworkStream m_rawStream;

        public SnapNetworkServer(SecureStreamServer<SocketUserPermissions> authentication, TcpClient client, SnapServer server, bool requireSsl = false)
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
        /// Releases the unmanaged resources used by the <see cref="SnapNetworkServer"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                base.Dispose(disposing); // Call base class Dispose().

                if (disposing)
                {
                    TryShutdownSocket();
                    m_client.Close();
                }
            }
            finally
            {
                m_disposed = true; // Prevent duplicate dispose.
            }
        }

        private void TryShutdownSocket()
        {
            try
            {
                m_client.Client.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException ex)
            {
                // In particular, the ConnectionReset socket error absolutely should not prevent
                // us from calling TcpClient.Close(), nor should it propagate an exception up the stack
                Log.Publish(MessageLevel.Debug, "SnapConnectionReset", null, null, ex);
            }
        }
    }
}