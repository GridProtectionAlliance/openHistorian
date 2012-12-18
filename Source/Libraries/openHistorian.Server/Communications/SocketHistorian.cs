//******************************************************************************************************
//  SocketHistorian.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using openHistorian.Engine;

namespace openHistorian.Communications
{
    /// <summary>
    /// Hosts a <see cref="IHistorianDatabaseCollection"/> on a network socket.
    /// </summary>
    public class SocketHistorian : IDisposable
    {
        volatile bool m_isRunning = true;
        TcpListener m_listener;
        IHistorianDatabaseCollection m_historian;
        bool m_ownsHistorian;
        bool m_disposed;

        public SocketHistorian(int port, IHistorianDatabaseCollection historian = null)
        {
            if (historian == null)
            {
                m_ownsHistorian = true;
                var tmpHistorian = new HistorianDatabaseCollection();
                tmpHistorian.Add("Default", new ArchiveDatabaseEngine(WriterOptions.IsMemoryOnly()));
                m_historian = tmpHistorian;
            }
            else
            {
                m_historian = historian;
            }
            m_isRunning = true;
            m_listener = new TcpListener(port);
            m_listener.Start();
            ThreadPool.QueueUserWorkItem(ProcessDataRequests);
        }

        List<ProcessClient> m_clients = new List<ProcessClient>();

        void ProcessDataRequests(object state)
        {
            while (m_isRunning && !m_listener.Pending())
            {
                Thread.Sleep(10);
            }
            if (!m_isRunning)
                return;

            Socket socket = m_listener.AcceptSocket();
            NetworkBinaryStream netStream = new NetworkBinaryStream(socket);

            ThreadPool.QueueUserWorkItem(ProcessDataRequests);

            ProcessClient clientProcessing = new ProcessClient(netStream, m_historian);
            lock (m_clients)
            {
                if (m_isRunning)
                    m_clients.Add(clientProcessing);
                else
                    return;
            }
            clientProcessing.Run();
            lock (m_clients)
            {
                m_clients.Remove(clientProcessing);
            }

        }

        public void Dispose()
        {
            if (m_disposed)
            {

                m_disposed = true;
                if (m_ownsHistorian && m_historian != null)
                    ((HistorianDatabaseCollection)m_historian).Dispose();
                if (m_listener != null)
                    m_listener.Stop();
                m_historian = null;
                m_listener = null;
                m_isRunning = false;
                lock (m_clients)
                {
                    foreach (var client in m_clients)
                        client.Dispose();
                    m_clients.Clear();
                }
                if (m_ownsHistorian && m_historian != null)
                    ((HistorianDatabaseCollection)m_historian).Shutdown(1);
            }
        }
    }
}

