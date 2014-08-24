//******************************************************************************************************
//  ServerSocket.cs - Gbtc
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using GSF.Diagnostics;
using GSF.Net;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Net
{
    /// <summary>
    /// This is a single server socket that handles an individual client connection.
    /// </summary>
    internal class ServerSocket
        : LogReporterBase
    {
        private bool m_disposed;

        private readonly Server m_server;
        private NetworkBinaryStream m_stream;
        private Client m_host;
        private string m_serverString;

        public ServerSocket(NetworkBinaryStream netStream, Server server, LogSource parent, string serverString)
            : base(parent)
        {
            m_serverString = serverString;
            m_stream = netStream;
            m_server = server;
        }

        public void GetFullStatus(StringBuilder status)
        {
            try
            {
                status.AppendLine(m_stream.Socket.RemoteEndPoint.ToString());
            }
            catch (Exception)
            {
                status.AppendLine("Error getting remote endpoint");
            }
        }

        /// <summary>
        /// This function will verify the connection, create all necessary streams, set timeouts, and catch any exceptions and terminate the connection
        /// </summary>
        /// <remarks></remarks>
        public void ProcessClient()
        {
            try
            {
                long code = m_stream.ReadInt64();
                if (code != 0x2BA517361120)
                {
                    m_stream.Write((byte)ServerResponse.UnknownProtocolIdentifier);
                    m_stream.Flush();
                    return;
                }
                string serverName;
                if (!m_stream.TryReadString(100, out serverName))
                {
                    m_stream.Write((byte)ServerResponse.ServerNameTooLong);
                    m_stream.Flush();
                    return;
                }
                if (serverName != m_serverString)
                {
                    m_stream.Write((byte)ServerResponse.ServerNameDoesNotMatch );
                    m_stream.Flush();
                    return;
                }




                m_stream.Write((byte)ServerResponse.ConnectedToRoot);
                m_stream.Flush();
                ProcessRootLevelCommands();
            }
            catch (Exception ex)
            {
                try
                {
                    m_stream.Write((byte)ServerResponse.UnhandledException);
                    m_stream.Write(ex.ToString());
                    m_stream.Flush();
                }
                catch (Exception)
                {
                }
                Log.LogMessage(VerboseLevel.Warning, "Socket Exception", "Exception occured, Client will be disconnected.", null, ex);
            }
            finally
            {
                try
                {
                    m_stream.Disconnect();
                }
                catch (Exception ex)
                {
                }
                Log.LogMessage(VerboseLevel.Information, "Client Disconnected", "Client has been disconnected");
                m_stream = null;
            }
        }

        /// <summary>
        /// This function will process any of the packets that come in.  It will throw an error if anything happens.  
        /// This will cause the calling function to close the connection.
        /// </summary>
        /// <remarks></remarks>
        private void ProcessRootLevelCommands()
        {
            m_host = Client.Connect(m_server);

            while (true)
            {
                ServerCommand command = (ServerCommand)m_stream.ReadUInt8();
                switch (command)
                {
                    case ServerCommand.GetAllDatabases:
                        var info = m_host.GetDatabaseInfo();
                        m_stream.Write((byte)ServerResponse.ListOfDatabases);
                        m_stream.Write(info.Count);
                        foreach (var i in info)
                        {
                            i.Save(m_stream);
                        }
                        m_stream.Flush();
                        break;
                    case ServerCommand.ConnectToDatabase:
                        string databaseName = m_stream.ReadString();
                        Guid keyTypeId = m_stream.ReadGuid();
                        Guid valueTypeId = m_stream.ReadGuid();
                        if (!m_host.Contains(databaseName))
                        {
                            m_stream.Write((byte)ServerResponse.DatabaseDoesNotExist);
                            m_stream.Write("Database Does Not Exist");
                            m_stream.Flush();
                            return;
                        }
                        var database = m_host.GetDatabase(databaseName);
                        var dbinfo = database.Info;
                        if (dbinfo.KeyTypeID != keyTypeId)
                        {
                            m_stream.Write((byte)ServerResponse.DatabaseKeyUnknown);
                            m_stream.Write("Database Key Type Is Invalid");
                            m_stream.Flush();
                            return;
                        }
                        if (dbinfo.ValueTypeID != valueTypeId)
                        {
                            m_stream.Write((byte)ServerResponse.DatabaseValueUnknown);
                            m_stream.Write("Database Value Type Is Invalid");
                            m_stream.Flush();
                            return;
                        }
                        var type = GetType();
                        var method = type.GetMethod("ConnectToDatabase", BindingFlags.NonPublic | BindingFlags.Instance);
                        var reflectionMethod = method.MakeGenericMethod(database.Info.KeyType, database.Info.ValueType);
                        var success = (bool)reflectionMethod.Invoke(this, new object[] { database });
                        if (!success)
                            return;
                        break;
                    case ServerCommand.Disconnect:
                        m_stream.Write((byte)ServerResponse.GoodBye);
                        m_stream.Write("Good bye!");
                        m_stream.Flush();
                        return;
                    default:
                        m_stream.Write((byte)ServerResponse.UnknownCommand);
                        m_stream.Write((byte)command);
                        m_stream.Flush();
                        return;
                }
            }
        }

        //Called through reflection. Its the only way to call a generic function only knowing the Types
        [MethodImpl(MethodImplOptions.NoOptimization)] //Prevents removing this method as it may appear unused.
        bool ConnectToDatabase<TKey, TValue>(ServerDatabase<TKey, TValue>.ClientDatabase database)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            m_stream.Write((byte)ServerResponse.SuccessfullyConnectedToDatabase);
            m_stream.Flush();
            var engine = new ServerSocketDatabase<TKey, TValue>(m_stream, database);
            return engine.RunDatabaseLevel();
        }


        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ServerSocket"/> object and optionally releases the managed resources.
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
                        if (m_host != null)
                            m_host.Dispose();
                        m_host = null;

                        if (m_stream.Connected)
                            m_stream.Disconnect();
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