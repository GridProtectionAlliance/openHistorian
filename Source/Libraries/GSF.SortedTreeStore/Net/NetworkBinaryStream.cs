//******************************************************************************************************
//  NetworkBinaryStream.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Net.Sockets;
using System.Threading;
using GSF.Threading;

namespace GSF.Net
{
    public class NetworkBinaryStream
        : RemoteBinaryStream
    {
        private Socket m_socket;

        public NetworkBinaryStream(Socket socket, int timeout = -1, WorkerThreadSynchronization workerThreadSynchronization = null)
            : base(new NetworkStream(socket), workerThreadSynchronization)
        {
            if (!BitConverter.IsLittleEndian)
                throw new Exception("BigEndian processors are not supported");

            m_socket = socket;
            Timeout = timeout;
            m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
        }

        public Socket Socket => m_socket;

        public int Timeout
        {
            get => m_socket.ReceiveTimeout;
            set
            {
                m_socket.ReceiveTimeout = value;
                m_socket.SendTimeout = value;
            }
        }

        public bool Connected => m_socket != null && m_socket.Connected;

        public int AvailableReadBytes
        {
            get
            {
                WorkerThreadSynchronization.PulseSafeToCallback();
                //ToDo: Don't call m_socket.Available since it's a windows API Call and terribly slow.
                return ReceiveBufferAvailable + m_socket.Available;
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Disconnect();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Disconnects the socket.  Does not throw an exception.
        /// </summary>
        /// <remarks></remarks>
        public void Disconnect()
        {
            Socket socket = Interlocked.Exchange(ref m_socket, null);
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                try
                {
                    socket.Close();
                }
                catch
                {
                }
            }
            WorkerThreadSynchronization.BeginSafeToCallbackRegion();
        }
    }
}