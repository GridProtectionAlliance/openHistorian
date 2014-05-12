//******************************************************************************************************
//  ServerSocketListener.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GSF.Net;
using GSF.SortedTreeStore.Server;

namespace GSF.SortedTreeStore.Net
{
    // TODO: Need the ability to dynamically add and remove database instances from the socket historian - or maybe better - just "replace" the collection it's using...
    // TODO: Initial glance looks like replacement of collection might be simple...??
    /// <summary>
    /// Hosts a <see cref="ServerRoot"/> on a network socket.
    /// </summary>
    public class ServerSocketListener
        : IDisposable
    {
        private volatile bool m_isRunning = true;
        private TcpListener m_listener;
        private ServerRoot m_historian;
        private readonly int m_port;
        private bool m_disposed;
        private readonly List<RemoteServerRoot> m_clients = new List<RemoteServerRoot>();

        // TODO: Replace this with a connection string instead of a port - allows easier specification of interface, etc.
        public ServerSocketListener(int port, ServerRoot historian)
        {
            if (historian == null)
                throw new ArgumentNullException("historian");

            m_historian = historian;
            m_port = port;

            // TODO: Shouldn't we use GSF.Communications async library here for scalability? If not, why not? 
            // TODO: I think async communication classes could pass NetworkBinaryStream to a handler like ProcessClient...
            // TODO: Communications library may need a simple modification... Check with S. Wills for thoughts here...
            m_isRunning = true;
            m_listener = new TcpListener(m_port);
            m_listener.Start();

            //var socket = m_listener.AcceptSocketAsync();
            //socket.ContinueWith(ProcessDataRequests);
            //socket.Start();
            ThreadPool.QueueUserWorkItem(ProcessDataRequests);
        }

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

        public int Port
        {
            get
            {
                return m_port;
            }
        }

        private void ProcessDataRequests(object state)
        {
            while (m_isRunning && !m_listener.Pending())
            {
                Thread.Sleep(10);
            }
            if (!m_isRunning)
            {
                m_listener.Stop();
                return;
            }

            Socket socket = m_listener.AcceptSocket();
            NetworkBinaryStream netStream = new NetworkBinaryStream(socket);

            //ToDo: Should probably not process a client on the threadpool since I do blocking.
            ThreadPool.QueueUserWorkItem(ProcessDataRequests);

            RemoteServerRoot clientProcessing = new RemoteServerRoot(netStream, m_historian);
            lock (m_clients)
            {
                if (m_isRunning)
                    m_clients.Add(clientProcessing);
                else
                {
                    netStream.Disconnect();
                    return;
                }
            }
            clientProcessing.RunClient();
            lock (m_clients)
            {
                m_clients.Remove(clientProcessing);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (m_disposed)
            {
                m_disposed = true;
                if (m_listener != null)
                    m_listener.Stop();
                m_historian = null;
                m_listener = null;
                m_isRunning = false;
                lock (m_clients)
                {
                    foreach (RemoteServerRoot client in m_clients)
                        client.Dispose();
                    m_clients.Clear();
                }
            }
        }
    }
}