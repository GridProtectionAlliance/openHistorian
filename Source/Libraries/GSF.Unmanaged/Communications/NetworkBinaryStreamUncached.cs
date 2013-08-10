//******************************************************************************************************
//  NetworkBinaryStream.cs - Gbtc
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
//
//******************************************************************************************************

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace GSF.Communications
{
    public class NetworkBinaryStreamUnCached : Stream
    {
        private Socket m_socket;

        public NetworkBinaryStreamUnCached(Socket socket, int timeout = -1)
            : base()
        {
            m_socket = socket;
            Timeout = timeout;
            m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
        }

        public int Timeout
        {
            get
            {
                return m_socket.ReceiveTimeout;
            }
            set
            {
                m_socket.ReceiveTimeout = value;
                m_socket.SendTimeout = value;
            }
        }

        public bool Connected
        {
            get
            {
                return (m_socket != null) && m_socket.Connected;
            }
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public int AvailableReadBytes
        {
            get
            {
                return m_socket.Available;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            //Send(m_sendBuffer.InternalBuffer, 0, m_sendBuffer.DataAvailable);
            //m_sendBuffer.Clear();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count <= 0)
                return 0;
            int origionalCount = count;
            int length;

            while (count > 0)
            {
                length = Receive(buffer, offset, count);
                offset += length;
                count -= length;
            }
            return origionalCount;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Send(buffer, offset, count);
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
        }

        private void Send(byte[] buffer, int offset, int count)
        {
            if (!Connected)
                throw new Exception("Not Connected");
            try
            {
                if (m_socket.Send(buffer, offset, count, SocketFlags.None) != count)
                    throw new Exception("Something isn't right");
            }
            catch
            {
                Disconnect();
                throw;
            }
        }

        private int Receive(byte[] buffer, int offset, int count)
        {
            if (!Connected)
                throw new Exception("Not Connected");
            try
            {
                int rec = m_socket.Receive(buffer, offset, count, SocketFlags.None);
                if (rec == 0)
                {
                    Disconnect();
                    return 0;
                }
                return rec;
            }
            catch
            {
                Disconnect();
                throw;
            }
        }
    }
}