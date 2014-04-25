//******************************************************************************************************
//  ServerProcessClient.cs - Gbtc
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using GSF.Net;
using GSF.SortedTreeStore.Client;
using GSF.SortedTreeStore.Server;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Net
{
    /// <summary>
    /// This is the server code that processes an individual client.
    /// </summary>
    internal class ServerProcessClient
        : IDisposable
    {
        public event SocketErrorEventHandler SocketError;

        public delegate void SocketErrorEventHandler(Exception ex);

        private NetworkBinaryStream m_stream;
        private readonly ServerRoot m_historian;

        public ServerProcessClient(NetworkBinaryStream netStream, ServerRoot historian)
        {
            m_stream = netStream;
            m_historian = historian;
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
        public void RunClient()
        {
            //m_netStream.Timeout = 5000;
            try
            {
                long code = m_stream.ReadInt64();
                if (code != 1122334455667788993L)
                {
                    m_stream.Write((byte)ServerResponse.UnknownProtocolIdentifier);
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
                if (SocketError != null)
                {
                    SocketError(ex);
                }
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
            while (true)
            {
                ServerCommand command = (ServerCommand)m_stream.ReadUInt8();
                switch (command)
                {
                    case ServerCommand.GetAllDatabases:
                        var info = m_historian.GetDatabaseInfo();
                        m_stream.Write((byte)ServerResponse.ListOfDatabases);
                        m_stream.Write(info.Count);
                        foreach (var i in info)
                        {
                            m_stream.Write(i.DatabaseName);
                            m_stream.Write(i.KeyType.FullName);
                            m_stream.Write(i.KeyTypeID);
                            m_stream.Write(i.ValueType.FullName);
                            m_stream.Write(i.ValueTypeID);
                        }
                        m_stream.Flush();
                        break;
                    case ServerCommand.ConnectToDatabase:
                        string databaseName = m_stream.ReadString();
                        Guid keyTypeId = m_stream.ReadGuid();
                        Guid valueTypeId = m_stream.ReadGuid();
                        if (!m_historian.Contains(databaseName))
                        {
                            m_stream.Write((byte)ServerResponse.DatabaseDoesNotExist);
                            m_stream.Write("Database Does Not Exist");
                            m_stream.Flush();
                            return;
                        }
                        var database = m_historian.GetDatabase(databaseName);
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
        [MethodImpl(MethodImplOptions.NoOptimization)] //Prevents removing this method as it may appera unused.
        bool ConnectToDatabase<TKey, TValue>(ClientDatabaseBase<TKey, TValue> database)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            m_stream.Write((byte)ServerResponse.SuccessfullyConnectedToDatabase);
            m_stream.Flush();
            var engine = new ServerProcessClientEngine<TKey, TValue>(m_stream, database);
            return engine.RunDatabaseLevel();
        }

        public void Dispose()
        {
            if (m_stream.Connected)
                m_stream.Disconnect();
        }
    }
}