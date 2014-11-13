//******************************************************************************************************
//  SnapSocketListener.cs - Gbtc
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
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GSF.Diagnostics;
using GSF.Security;

namespace GSF.Snap.Services.Net
{
    // TODO: Need the ability to dynamically add and remove database instances from the socket historian - or maybe better - just "replace" the collection it's using...
    // TODO: Initial glance looks like replacement of collection might be simple...??
    /// <summary>
    /// Hosts a <see cref="SnapServer"/> on a network socket.
    /// </summary>
    public class SnapSocketListener
        : LogSourceBase
    {
        private volatile bool m_isRunning = true;
        private TcpListener m_listener;
        private SnapServer m_server;
        private bool m_disposed;
        private readonly List<SnapNetworkServer> m_clients = new List<SnapNetworkServer>();
        private readonly SnapSocketListenerSettings m_settings;
        private SecureStreamServer<SocketUserPermissions> m_authenticator;

        /// <summary>
        /// Creates a <see cref="SnapSocketListener"/>
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="server"></param>
        /// <param name="parent"></param>
        public SnapSocketListener(SnapSocketListenerSettings settings, SnapServer server, LogSource parent)
            : base(parent)
        {
            if ((object)server == null)
                throw new ArgumentNullException("server");
            if ((object)settings == null)
                throw new ArgumentNullException("settings");

            m_server = server;
            m_settings = settings.CloneReadonly();
            m_settings.Validate();

            m_authenticator = new SecureStreamServer<SocketUserPermissions>();
            if (settings.DefaultUserCanRead || settings.DefaultUserCanWrite || settings.DefaultUserIsAdmin)
            {
                m_authenticator.SetDefaultUser(true, new SocketUserPermissions()
                {
                    CanRead = settings.DefaultUserCanRead,
                    CanWrite = settings.DefaultUserCanWrite,
                    IsAdmin = settings.DefaultUserIsAdmin
                });
            }
            foreach (var user in settings.Users)
            {
                m_authenticator.AddUserIntegratedSecurity(user, new SocketUserPermissions()
                    {
                        CanRead = true,
                        CanWrite = true,
                        IsAdmin = true
                    });
            }

            // TODO: Shouldn't we use GSF.Communications async library here for scalability? If not, why not? 
            // TODO: I think async communication classes could pass NetworkBinaryStream to a handler like ProcessClient...
            // TODO: Communications library may need a simple modification... Check with S. Wills for thoughts here...
            m_isRunning = true;
            m_listener = new TcpListener(m_settings.LocalEndPoint);
            //m_listener.Server.DualMode = true;
            m_listener.Start();

            //var socket = m_listener.AcceptSocketAsync();
            //socket.ContinueWith(ProcessDataRequests);
            //socket.Start();

            Log.Publish(VerboseLevel.Information, "Constructor Called", "Listening on " + m_settings.LocalEndPoint.ToString());

            Thread th = new Thread(ProcessDataRequests);
            th.IsBackground = true;
            th.Start();
        }

        /// <summary>
        /// Gets the status of the <see cref="SnapSocketListener"/>.
        /// </summary>
        /// <param name="status"></param>
        public void GetFullStatus(StringBuilder status)
        {
            lock (m_clients)
            {
                status.AppendFormat("Active Client Count: {0}\r\n", m_clients.Count);
                foreach (var client in m_clients)
                {
                    client.GetFullStatus(status);
                }
            }
        }

        /// <summary>
        /// Processes the client
        /// </summary>
        /// <param name="state"></param>
        private void ProcessDataRequests(object state)
        {
            try
            {
                while (m_isRunning && !m_listener.Pending())
                {
                    Thread.Sleep(10);
                }
                if (!m_isRunning)
                {
                    m_listener.Stop();
                    Log.Publish(VerboseLevel.Information, "Socket Listener Stopped");
                    return;
                }

                TcpClient client = m_listener.AcceptTcpClient();
                Log.Publish(VerboseLevel.Information, "Client Connection", "Client connection attempted from: " + client.Client.RemoteEndPoint.ToString());

                Thread th = new Thread(ProcessDataRequests);
                th.IsBackground = true;
                th.Start();

                SnapNetworkServer networkServerProcessing = new SnapNetworkServer(m_authenticator, client, m_server, Log);
                lock (m_clients)
                {
                    if (m_isRunning)
                    {
                        m_clients.Add(networkServerProcessing);
                    }
                    else
                    {
                        client.Client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        return;
                    }
                }
                networkServerProcessing.ProcessClient();
                lock (m_clients)
                {
                    m_clients.Remove(networkServerProcessing);
                }

            }
            catch (Exception ex)
            {
                Log.Publish(VerboseLevel.Critical, "Client Processing Failed", "An unhandled Exception occured while processing clients", null, ex);
            }

        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SnapSocketListener"/> object and optionally releases the managed resources.
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
                        m_isRunning = false;
                        if (m_listener != null)
                            m_listener.Stop();
                        m_server = null;
                        lock (m_clients)
                        {
                            foreach (SnapNetworkServer client in m_clients)
                                client.Dispose();
                            m_clients.Clear();
                        }
                        // This will be done only when the object is disposed by calling Dispose().
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