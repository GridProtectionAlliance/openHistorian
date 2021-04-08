//******************************************************************************************************
//  SnapStreamingClient.cs - Gbtc
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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GSF.Diagnostics;
using GSF.Net;
using GSF.Security;
using GSF.IO;

// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace GSF.Snap.Services.Net
{
    /// <summary>
    /// A client that communicates over a stream.
    /// </summary>
    public class SnapStreamingClient
        : SnapClient
    {
        private bool m_disposed;
        private Stream m_rawStream;
        private Stream m_secureStream;
        private RemoteBinaryStream m_stream;
        private ClientDatabaseBase m_sortedTreeEngine;
        private string m_historianDatabaseString;
        private SecureStreamClientBase m_credentials;
        private Dictionary<string, DatabaseInfo> m_databaseInfos;

        /// <summary>
        /// Creates a <see cref="SnapStreamingClient"/>
        /// </summary>
        /// <param name="stream">The config to use for the client</param>
        /// <param name="credentials">Authenticates using the supplied user credentials.</param>
        /// <param name="useSsl">specifies if a ssl connection is desired.</param>
        public SnapStreamingClient(Stream stream, SecureStreamClientBase credentials, bool useSsl)
        {
            Initialize(stream, credentials, useSsl);
        }

        /// <summary>
        /// Allows derived classes to call <see cref="Initialize"/> after the inheriting class 
        /// has done something in the constructor.
        /// </summary>
        protected SnapStreamingClient()
        {
        }

        /// <summary>
        /// Creates a <see cref="SnapStreamingClient"/>
        /// </summary>
        /// <param name="stream">The config to use for the client</param>
        /// <param name="credentials">Authenticates using the supplied user credentials.</param>
        /// <param name="useSsl">specifies if a ssl connection is desired.</param>
        protected void Initialize(Stream stream, SecureStreamClientBase credentials, bool useSsl)
        {
            m_credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            m_rawStream = stream ?? throw new ArgumentNullException(nameof(stream));
            m_rawStream.Write(0x2BA517361121L);
            m_rawStream.Write(useSsl); //UseSSL

            ServerResponse command = (ServerResponse)m_rawStream.ReadNextByte();

            switch (command)
            {
                case ServerResponse.UnknownProtocol:
                    throw new Exception("Client and server cannot agree on a protocol, this is commonly because they are running incompatible versions.");
                case ServerResponse.KnownProtocol:
                    break;
                default:
                    throw new Exception($"Unknown server response: {command}");
            }

            useSsl = m_rawStream.ReadBoolean();

            if (!m_credentials.TryAuthenticate(m_rawStream, useSsl, out m_secureStream))
                throw new Exception("Authentication Failed");

            m_stream = new RemoteBinaryStream(m_secureStream);

            command = (ServerResponse)m_stream.ReadUInt8();
            
            switch (command)
            {
                case ServerResponse.UnhandledException:
                    string exception = m_stream.ReadString();
                    throw new Exception($"Server UnhandledExcetion: \n{exception}");
                case ServerResponse.UnknownProtocol:
                    throw new Exception("Client and server cannot agree on a protocol, this is commonly because they are running incompatible versions.");
                case ServerResponse.ConnectedToRoot:
                    break;
                default:
                    throw new Exception($"Unknown server response: {command}");
            }

            RefreshDatabaseInfo();
        }

        private void RefreshDatabaseInfo()
        {
            m_stream.Write((byte)ServerCommand.GetAllDatabases);
            m_stream.Flush();

            ServerResponse command = (ServerResponse)m_stream.ReadUInt8();
            
            switch (command)
            {
                case ServerResponse.UnhandledException:
                    string exception = m_stream.ReadString();
                    throw new Exception($"Server UnhandledExcetion: \n{exception}");
                case ServerResponse.ListOfDatabases:
                    int count = m_stream.ReadInt32();
                    Dictionary<string, DatabaseInfo> dict = new Dictionary<string, DatabaseInfo>();

                    while (count > 0)
                    {
                        count--;
                        DatabaseInfo info = new DatabaseInfo(m_stream);
                        dict.Add(info.DatabaseName.ToUpper(), info);
                    }

                    m_databaseInfos = dict;
                    break;
                default:
                    throw new Exception($"Unknown server response: {command}");
            }
        }

        /// <summary>
        /// Gets the database that matches <see cref="databaseName"/>
        /// </summary>
        /// <param name="databaseName">the case insensitive name of the databse</param>
        /// <returns></returns>
        public override ClientDatabaseBase GetDatabase(string databaseName)
        {
            DatabaseInfo info = m_databaseInfos[databaseName.ToUpper()];
            Type type = typeof(SnapStreamingClient);
            MethodInfo method = type.GetMethod("InternalGetDatabase", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // ReSharper disable once PossibleNullReferenceException
            MethodInfo reflectionMethod = method.MakeGenericMethod(info.KeyType, info.ValueType);
            ClientDatabaseBase db = (ClientDatabaseBase)reflectionMethod.Invoke(this, new object[] { databaseName });
            
            return db;
        }

        //Called through reflection. Its the only way to call a generic function only knowing the Types
        [MethodImpl(MethodImplOptions.NoOptimization)] //Prevents removing this method as it may appear unused.
        private ClientDatabaseBase<TKey, TValue> InternalGetDatabase<TKey, TValue>(string databaseName)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            return GetDatabase<TKey, TValue>(databaseName, null);
        }

        /// <summary>
        /// Accesses <see cref="ClientDatabaseBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="ClientDatabaseBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public override ClientDatabaseBase<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName) => 
            GetDatabase<TKey, TValue>(databaseName, null);

        /// <summary>
        /// Gets basic information for every database connected to the server.
        /// </summary>
        /// <returns></returns>
        public override List<DatabaseInfo> GetDatabaseInfo() => 
            m_databaseInfos.Values.ToList();

        /// <summary>
        /// Determines if <see cref="databaseName"/> is contained in the database.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns></returns>
        public override bool Contains(string databaseName) => 
            m_databaseInfos.ContainsKey(databaseName.ToUpper());

        /// <summary>
        /// Accesses <see cref="StreamingClientDatabase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <param name="encodingMethod"></param>
        /// <returns><see cref="StreamingClientDatabase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        private StreamingClientDatabase<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName, EncodingDefinition encodingMethod)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            DatabaseInfo dbInfo = m_databaseInfos[databaseName.ToUpper()];

            if (encodingMethod is null)
                encodingMethod = dbInfo.SupportedStreamingModes.First();

            if (m_sortedTreeEngine != null)
                throw new Exception($"Can only connect to one database at a time. Please disconnect from database{m_historianDatabaseString}");

            if (dbInfo.KeyType != typeof(TKey))
                throw new InvalidCastException("Key types do not match");

            if (dbInfo.ValueType != typeof(TValue))
                throw new InvalidCastException("Value types do not match");

            m_stream.Write((byte)ServerCommand.ConnectToDatabase);
            m_stream.Write(databaseName);
            m_stream.Write(new TKey().GenericTypeGuid);
            m_stream.Write(new TValue().GenericTypeGuid);
            m_stream.Flush();

            ServerResponse command = (ServerResponse)m_stream.ReadUInt8();

            switch (command)
            {
                case ServerResponse.UnhandledException:
                    string exception = m_stream.ReadString();
                    throw new Exception($"Server UnhandledExcetion: \n{exception}");
                case ServerResponse.DatabaseDoesNotExist:
                    throw new Exception($"Database does not exist on the server: {databaseName}");
                case ServerResponse.DatabaseKeyUnknown:
                    throw new Exception("Database key does not match that passed to this function");
                case ServerResponse.DatabaseValueUnknown:
                    throw new Exception("Database value does not match that passed to this function");
                case ServerResponse.SuccessfullyConnectedToDatabase:
                    break;
                default:
                    throw new Exception($"Unknown server response: {command}");
            }

            StreamingClientDatabase<TKey, TValue> db = new StreamingClientDatabase<TKey, TValue>(m_stream, () => m_sortedTreeEngine = null, dbInfo);
            m_sortedTreeEngine = db;
            m_historianDatabaseString = databaseName;

            db.SetEncodingMode(encodingMethod);

            return db;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SnapNetworkClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (!disposing)
                    return;

                if (m_sortedTreeEngine != null)
                    m_sortedTreeEngine.Dispose();

                m_sortedTreeEngine = null;

                try
                {
                    m_stream.Write((byte)ServerCommand.Disconnect);
                    m_stream.Flush();
                }
                catch (Exception ex)
                {
                    Logger.SwallowException(ex);
                }

                if (m_rawStream != null)
                    m_rawStream.Dispose();

                m_rawStream = null;
            }
            finally
            {
                m_disposed = true;       // Prevent duplicate dispose.
                base.Dispose(disposing); // Call base class Dispose().
            }
        }
    }
}