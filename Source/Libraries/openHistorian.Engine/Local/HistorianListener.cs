//******************************************************************************************************
//  HistorianListener.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  12/07/2012 - Ritchie
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GSF;
using GSF.Communication;
using GSF.Units;

namespace openHistorian.Local
{
    public class HistorianListener
    {
        // Nested Types
        private class ClientConnection
        {
            public Guid ClientID;
            public IHistorianReadWrite ReaderWriter;
            public volatile bool ReadInProgress;
        }
        
        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the default database for in-memory historian when no other is provided.
        /// </summary>
        public const string DefaultDatabaseName = "Default";

        /// <summary>
        /// Specifies the default value for the <see cref="Port"/> property.
        /// </summary>
        public const int DefaultPort = 8175;

        // Events

        /// <summary>
        /// Occurs when the <see cref="HistorianListener"/> has started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when <see cref="HistorianListener"/> has stopped.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Event is raised when there is an exception encountered while processing.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the exception that was thrown.
        /// </remarks>
        public event EventHandler<EventArgs<Exception>> ProcessException;

        // Fields
        private readonly IHistorian m_historian;
        private readonly bool m_historianCreated;
        private readonly TcpServer m_tcpServer;
        private readonly ConcurrentDictionary<Guid, ClientConnection> m_clientConnections;
        private int m_port;
        private long m_totalBytesReceived;
        private long m_totalPacketsReceived;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="HistorianListener"/> class.
        /// </summary>
        public HistorianListener(IHistorian historian = null)
        {
            m_port = DefaultPort;

            m_tcpServer = new TcpServer
            {
                AllowDualStackSocket = true
            };
            
            m_tcpServer.ServerStarted += m_tcpServer_ServerStarted;
            m_tcpServer.ServerStopped += m_tcpServer_ServerStopped;
            m_tcpServer.ReceiveClientDataComplete += m_tcpServer_ReceiveClientDataComplete;
            m_tcpServer.ClientConnected += m_tcpServer_ClientConnected;
            m_tcpServer.ClientDisconnected += m_tcpServer_ClientDisconnected;

            m_clientConnections = new ConcurrentDictionary<Guid, ClientConnection>();

            if ((object)historian == null)
            {
                // Create a new in-memory historian if one was not provided
                m_historian = new HistorianServer();
                m_historian.Manage().Add(DefaultDatabaseName);
                m_historianCreated = true;
            }
            else
            {
                m_historian = historian;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets reference to historian engine interface.
        /// </summary>
        public IHistorian Historian
        {
            get
            {
                return m_historian;
            }
        }

        /// <summary>
        /// Gets or sets the network port of the <see cref="Server"/> where the <see cref="HistorianListener"/> will connect to get the time-series data.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 0 and 65535.</exception>
        public int Port
        {
            get
            {
                return m_port;
            }
            set
            {
                if (!Transport.IsPortNumberValid(value.ToString()))
                    throw new ArgumentException("Value must be between 0 and 65535");

                m_port = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the <see cref="HistorianListener"/> is currently enabled.
        /// </summary>
        /// <remarks>
        /// <see cref="Enabled"/> property is not be set by user-code directly.
        /// </remarks>
        public bool Enabled
        {
            get
            {
                return (RunTime != 0.0);
            }
            set
            {
                if (value && !Enabled)
                    Start();
                else if (!value && Enabled)
                    Stop();
            }
        }

        /// <summary>
        /// Gets the up-time (in seconds) of the <see cref="HistorianListener"/>.
        /// </summary>
        public Time RunTime
        {
            get
            {
                if (m_tcpServer.CurrentState == ServerState.Running)
                    return m_tcpServer.RunTime;
                
                return 0;
            }
        }

        /// <summary>
        /// Gets the total number of bytes received by the <see cref="HistorianListener"/> since it was <see cref="Start"/>ed.
        /// </summary>
        public long TotalBytesReceived
        {
            get
            {
                return m_totalBytesReceived;
            }
        }

        /// <summary>
        /// Gets the total number of packets received by the <see cref="HistorianListener"/> since it was <see cref="Start"/>ed.
        /// </summary>
        public long TotalPacketsReceived
        {
            get
            {
                return m_totalPacketsReceived;
            }
        }

        /// <summary>
        /// Gets the descriptive status of the <see cref="HistorianListener"/>.
        /// </summary>
        public string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.AppendFormat("               Server port: {0}", m_port);
                status.AppendLine();
                status.AppendFormat("      Total bytes received: {0}", m_totalBytesReceived);
                status.AppendLine();
                status.AppendFormat("    Total packets received: {0}", m_totalPacketsReceived);
                status.AppendLine();
                status.AppendFormat("                  Run time: {0}", RunTime.ToString());
                status.AppendLine();

                if ((object)m_tcpServer != null)
                    status.Append(m_tcpServer.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HistorianListener"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        Stop();

                        if ((object)m_tcpServer != null)
                        {
                            m_tcpServer.ServerStarted -= m_tcpServer_ServerStarted;
                            m_tcpServer.ServerStopped -= m_tcpServer_ServerStopped;
                            m_tcpServer.ReceiveClientDataComplete -= m_tcpServer_ReceiveClientDataComplete;
                            m_tcpServer.Dispose();
                        }

                        if ((object)m_clientConnections != null)
                        {
                            m_clientConnections.Values.AsParallel().ForAll(cc =>
                            {
                                if ((object)cc != null && (object)cc.ReaderWriter != null)
                                    cc.ReaderWriter.Dispose();
                            });
                        }

                        if (m_historianCreated && (object)m_historian != null)
                            m_historian.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Starts the <see cref="HistorianListener"/> synchronously.
        /// </summary>
        public void Start()
        {
            if (!Enabled)
            {
                m_tcpServer.ConfigurationString = string.Format("Port={0}", Port);
                m_tcpServer.Start();
            }
        }

        /// <summary>
        /// Stops the <see cref="HistorianListener"/>.
        /// </summary>
        public void Stop()
        {
            m_tcpServer.Stop();
            m_totalBytesReceived = 0;
            m_totalPacketsReceived = 0;
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        protected virtual void OnStarted()
        {
            if ((object)Started != null)
                Started(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="Stopped"/> event.
        /// </summary>
        protected virtual void OnStopped()
        {
            if ((object)Stopped != null)
                Stopped(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises <see cref="ProcessException"/> event.
        /// </summary>
        /// <param name="ex">Processing <see cref="Exception"/>.</param>
        internal protected virtual void OnProcessException(Exception ex)
        {
            if (ProcessException != null)
                ProcessException(this, new EventArgs<Exception>(ex));
        }

        private void m_tcpServer_ServerStarted(object sender, EventArgs e)
        {
            OnStarted();
        }

        private void m_tcpServer_ServerStopped(object sender, EventArgs e)
        {
            OnStopped();
        }

        private void m_tcpServer_ClientConnected(object sender, EventArgs<Guid> e)
        {
            Guid clientID = e.Argument;

            m_clientConnections.TryAdd(clientID, new ClientConnection()
            {
                ClientID = clientID
            });
        }

        private void m_tcpServer_ClientDisconnected(object sender, EventArgs<Guid> e)
        {
            ClientConnection connection;
            m_clientConnections.TryRemove(e.Argument, out connection);

            if ((object)connection != null && (object)connection.ReaderWriter != null)
                connection.ReaderWriter.Dispose();
        }

        private void m_tcpServer_ReceiveClientDataComplete(object sender, EventArgs<Guid, byte[], int> e)
        {
            try
            {
                m_totalPacketsReceived++;
                m_totalBytesReceived += e.Argument3;

                ClientConnection connection;
                Guid clientID = e.Argument1;
                byte[] buffer = e.Argument2;
                int count = e.Argument3;

                if (count > 0 && buffer != null)
                {
                    if (m_clientConnections.TryGetValue(clientID, out connection) && (object)connection != null)
                    {
                        ServerCommand command = (ServerCommand)buffer[0];
                        IHistorianReadWrite readerWriter;

                        switch (command)
                        {
                            case ServerCommand.Connect:
                                // Expecting database name to follow connect command
                                int length = EndianOrder.LittleEndian.ToInt32(buffer, 1);
                                string databaseName = Encoding.Default.GetString(buffer, 5, length);

                                connection.ReaderWriter = m_historian.ConnectToDatabase(databaseName);
                                SendClientResponse(clientID, ServerResponse.Success, ServerCommand.Connect, "Successfully connected to {0}", databaseName);
                                break;
                            case ServerCommand.Disconnect:
                                readerWriter = connection.ReaderWriter;

                                if ((object)readerWriter == null)
                                {
                                    SendClientResponse(clientID, ServerResponse.Error, ServerCommand.Disconnect, "Not currently connected to any database.");
                                }
                                else
                                {
                                    // TODO: Get datbase name from IHistorianReadWrite
                                    readerWriter.Disconnect();
                                    SendClientResponse(clientID, ServerResponse.Success, ServerCommand.Disconnect, "Successfully disconnected from {0}", "[DATABASENAME]");
                                }

                                connection.ReaderWriter = null;
                                break;
                            case ServerCommand.Read:
                                readerWriter = connection.ReaderWriter;

                                if ((object)readerWriter == null)
                                {
                                    SendClientResponse(clientID, ServerResponse.Error, ServerCommand.Read, "Not currently connected to any database.");
                                }
                                else
                                {
                                    if (connection.ReadInProgress)
                                    {
                                        SendClientResponse(clientID, ServerResponse.Error, ServerCommand.Read, "A read is currently already in progress.");
                                    }
                                    else
                                    {
                                        connection.ReadInProgress = true;
                                        ulong startKey = EndianOrder.LittleEndian.ToUInt64(buffer, 1);
                                        ulong endKey = EndianOrder.LittleEndian.ToUInt64(buffer, 9);
                                        List<ulong> points = new List<ulong>();

                                        int pointCount = EndianOrder.LittleEndian.ToInt32(buffer, 17);

                                        for (int i = 0; i < pointCount; i++)
                                        {
                                            points.Add(EndianOrder.LittleEndian.ToUInt64(buffer, 21 + i * 8));
                                        }

                                        ThreadPool.QueueUserWorkItem(ProcessSend, new Tuple<ClientConnection, IPointStream>(connection, readerWriter.Read(startKey, endKey, points)));
                                    }
                                }

                                break;
                            case ServerCommand.CancelRead:
                                readerWriter = connection.ReaderWriter;

                                if ((object)readerWriter == null)
                                {
                                    SendClientResponse(clientID, ServerResponse.Error, ServerCommand.Write, "Not currently connected to any database.");
                                }
                                else
                                {
                                    if (connection.ReadInProgress)
                                    {
                                        connection.ReadInProgress = false;
                                        SendClientResponse(clientID, ServerResponse.Success, ServerCommand.CancelRead, "Read successfully cancelled.");
                                    }
                                    else
                                    {
                                        SendClientResponse(clientID, ServerResponse.Error, ServerCommand.CancelRead, "No read is currently in progress.");
                                    }
                                }

                                break;
                            case ServerCommand.Write:
                                readerWriter = connection.ReaderWriter;

                                if ((object)readerWriter == null)
                                {
                                    SendClientResponse(clientID, ServerResponse.Error, ServerCommand.Write, "Not currently connected to any database.");
                                }
                                else
                                {
                                    ulong key1 = EndianOrder.LittleEndian.ToUInt64(buffer, 1);
                                    ulong key2 = EndianOrder.LittleEndian.ToUInt64(buffer, 9);
                                    ulong value1 = EndianOrder.LittleEndian.ToUInt64(buffer, 17);
                                    ulong value2 = EndianOrder.LittleEndian.ToUInt64(buffer, 25);

                                    readerWriter.Write(key1, key2, value1, value2);
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Exceptions should be report to client as an error
                OnProcessException(ex);
            }
        }

        private void ProcessSend(object state)
        {
            // Along with headers a max of 46 points will fit within an MTU of 1500 bytes
            const int PointsPerPacket = 46;

            Tuple<ClientConnection, IPointStream> args = state as Tuple<ClientConnection, IPointStream>;

            if ((object)args != null)
            {
                ClientConnection connection = args.Item1;
                IPointStream pointStream = args.Item2;

                if ((object)connection != null && (object)pointStream != null)
                {
                    MemoryStream buffer = new MemoryStream();
                    ulong key1, key2, value1, value2;
                    int count = 0;
                    bool readSuccess = false;

                    // Reserve length for count
                    buffer.Write(s_zeroLengthBytes, 0, 4);

                    // Start writing points to the buffer
                    while (connection.ReadInProgress && count < PointsPerPacket && (readSuccess = pointStream.Read(out key1, out key2, out value1, out value2)))
                    {
                        buffer.Write(EndianOrder.LittleEndian.GetBytes(key1), 0, 8);
                        buffer.Write(EndianOrder.LittleEndian.GetBytes(key2), 0, 8);
                        buffer.Write(EndianOrder.LittleEndian.GetBytes(value1), 0, 8);
                        buffer.Write(EndianOrder.LittleEndian.GetBytes(value2), 0, 8);
                        count++;
                    }

                    if (connection.ReadInProgress)
                    {
                        buffer.Seek(0, SeekOrigin.Begin);
                        buffer.Write(EndianOrder.LittleEndian.GetBytes(count), 0, 4);

                        SendClientResponse(connection.ClientID, ServerResponse.DataPacket, ServerCommand.Read, buffer.ToArray());

                        // Continue processing only as there is more data to read
                        if (readSuccess)
                            ThreadPool.QueueUserWorkItem(ProcessSend, state);
                    }
                }
            }
        }

        private bool SendClientResponse(Guid clientID, ServerResponse responseCode, ServerCommand commandCode, string format, params object[] args)
        {
            return SendClientResponse(clientID, responseCode, commandCode, Encoding.Default.GetBytes(string.Format(format, args)));
        }

        // Send binary response packet to client
        private bool SendClientResponse(Guid clientID, ServerResponse responseCode, ServerCommand commandCode, byte[] data)
        {
            bool success = false;

            try
            {
                MemoryStream responsePacket = new MemoryStream();

                // Add response code
                responsePacket.WriteByte((byte)responseCode);

                // Add original in response to command code
                responsePacket.WriteByte((byte)commandCode);

                if ((object)data == null || data.Length == 0)
                {
                    // Add zero sized data buffer to response packet
                    responsePacket.Write(s_zeroLengthBytes, 0, 4);
                }
                else
                {
                    // Add size of data buffer to response packet
                    responsePacket.Write(EndianOrder.BigEndian.GetBytes(data.Length), 0, 4);

                    // Add data buffer
                    responsePacket.Write(data, 0, data.Length);
                }

                // Send response packet
                if (m_tcpServer.CurrentState == ServerState.Running)
                {
                    byte[] responseData = responsePacket.ToArray();

                    m_tcpServer.SendToAsync(clientID, responseData, 0, responseData.Length);

                    success = true;
                }
            }
            catch (ObjectDisposedException)
            {
                // This happens when there is still data to be sent to a disconnected client - we can safely ignore this exception
            }
            catch (NullReferenceException)
            {
                // This happens when there is still data to be sent to a disconnected client - we can safely ignore this exception
            }
            catch (SocketException ex)
            {
                if (!HandleSocketException(clientID, ex))
                    OnProcessException(new InvalidOperationException("Failed to send response packet to client due to exception: " + ex.Message, ex));
            }
            catch (InvalidOperationException ex)
            {
                // Could still be processing threads with client data after client has been disconnected, this can be safely ignored
                if (!ex.Message.StartsWith("No client found"))
                    OnProcessException(new InvalidOperationException("Failed to send response packet to client due to exception: " + ex.Message, ex));
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException("Failed to send response packet to client due to exception: " + ex.Message, ex));
            }

            return success;
        }

        // Socket exception handler
        private bool HandleSocketException(Guid clientID, SocketException ex)
        {
            if ((object)ex != null)
            {
                // WSAECONNABORTED and WSAECONNRESET are common errors after a client disconnect,
                // if they happen for other reasons, make sure disconnect procedure is handled
                if (ex.ErrorCode == 10053 || ex.ErrorCode == 10054)
                {
                    try
                    {
                        ThreadPool.QueueUserWorkItem(DisconnectClient, clientID);
                    }
                    catch (Exception queueException)
                    {
                        // Process exception for logging
                        OnProcessException(new InvalidOperationException("Failed to queue client disconnect due to exception: " + queueException.Message, queueException));
                    }

                    return true;
                }
            }

            return false;
        }

        // Disconnect client - this should be called from non-blocking thread (e.g., thread pool)
        private void DisconnectClient(object state)
        {
            try
            {
                Guid clientID = (Guid)state;
                m_tcpServer.DisconnectOne(clientID);
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException(string.Format("Encountered an exception while processing client disconnect: {0}", ex.Message), ex));
            }
        }

        #endregion

        #region [ Static ]

        // Static Fields

        // Constant zero length integer byte array
        private readonly static byte[] s_zeroLengthBytes = new byte[] { 0, 0, 0, 0 };

        #endregion
    }
}
