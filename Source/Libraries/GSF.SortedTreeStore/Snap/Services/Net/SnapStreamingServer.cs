//******************************************************************************************************
//  SnapStreamingServer.cs - Gbtc
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using GSF.Diagnostics;
using GSF.Net;
using GSF.Security;
using GSF.IO;

namespace GSF.Snap.Services.Net
{
    /// <summary>
    /// This is a single server socket that handles an individual client connection.
    /// </summary>
    public class SnapStreamingServer
        : DisposableLoggingClassBase
    {
        private bool m_disposed;
        private SnapServer m_server;
        private SnapClient m_host;
        private Stream m_rawStream;
        private Stream m_secureStream;
        private RemoteBinaryStream m_stream;
        private SecureStreamServer<SocketUserPermissions> m_authentication;
        private SocketUserPermissions m_permissions;
        public bool RequireSSL;

        public SnapStreamingServer(SecureStreamServer<SocketUserPermissions> authentication, Stream stream, SnapServer server, bool requireSsl = false)
            : base(MessageClass.Framework)
        {
            Initialize(authentication, stream, server, requireSsl);
        }

        /// <summary>
        /// Allows derived classes to call <see cref="Initialize"/> after the inheriting class 
        /// has done something in the constructor.
        /// </summary>
        protected SnapStreamingServer()
            : base(MessageClass.Framework)
        {
        }

        /// <summary>
        /// Creates a <see cref="SnapStreamingServer"/>
        /// </summary>
        protected void Initialize(SecureStreamServer<SocketUserPermissions> authentication, Stream stream, SnapServer server, bool requireSsl)
        {
            RequireSSL = requireSsl;
            m_rawStream = stream;
            m_authentication = authentication;
            m_server = server;
        }

        /// <summary>
        /// This function will verify the connection, create all necessary streams, set timeouts, and catch any exceptions and terminate the connection
        /// </summary>
        /// <remarks></remarks>
        public void ProcessClient()
        {
            try
            {
                long code = m_rawStream.ReadInt64();
                
                if (code != 0x2BA517361121L)
                {
                    m_rawStream.Write((byte)ServerResponse.UnknownProtocol);
                    m_rawStream.Flush();
                    return;
                }
                
                bool useSsl = m_rawStream.ReadBoolean() || RequireSSL;

                m_rawStream.Write((byte)ServerResponse.KnownProtocol);
                m_rawStream.Write(useSsl);

                if (!m_authentication.TryAuthenticateAsServer(m_rawStream, useSsl, out m_secureStream, out m_permissions))
                {
                    return;
                }

                m_stream = new RemoteBinaryStream(m_secureStream);
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
                catch (Exception ex2)
                {
                    Logger.SwallowException(ex2);
                }
                Log.Publish(MessageLevel.Warning, "Socket Exception", "Exception occured, Client will be disconnected.", null, ex);
            }
            finally
            {
                Dispose();
                Log.Publish(MessageLevel.Info, "Client Disconnected", "Client has been disconnected");
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
            m_host = SnapClient.Connect(m_server);

            while (true)
            {
                ServerCommand command = (ServerCommand)m_stream.ReadUInt8();
                switch (command)
                {
                    case ServerCommand.GetAllDatabases:
                        List<DatabaseInfo> info = m_host.GetDatabaseInfo();
                        m_stream.Write((byte)ServerResponse.ListOfDatabases);
                        m_stream.Write(info.Count);
                        foreach (DatabaseInfo i in info)
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
                        ClientDatabaseBase database = m_host.GetDatabase(databaseName);
                        DatabaseInfo dbinfo = database.Info;
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
                        
                        Type type = typeof(SnapStreamingServer);
                        MethodInfo method = type.GetMethod("ConnectToDatabase", BindingFlags.NonPublic | BindingFlags.Instance);
                        MethodInfo reflectionMethod = method?.MakeGenericMethod(database.Info.KeyType, database.Info.ValueType);
                        bool success = (bool?)reflectionMethod?.Invoke(this, new object[] { database }) ?? false;
                        
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
        private bool ConnectToDatabase<TKey, TValue>(SnapServerDatabase<TKey, TValue>.ClientDatabase database)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            m_stream.Write((byte)ServerResponse.SuccessfullyConnectedToDatabase);
            m_stream.Flush();
            StreamingServerDatabase<TKey, TValue> engine = new StreamingServerDatabase<TKey, TValue>(m_stream, database);
            return engine.RunDatabaseLevel();
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
                if (disposing)
                {
                    m_host?.Dispose();
                    m_host = null;
                }
            }
            finally
            {
                m_disposed = true;       // Prevent duplicate dispose.
                base.Dispose(disposing); // Call base class Dispose().
            }
        }
    }
}