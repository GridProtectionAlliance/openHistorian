//******************************************************************************************************
//  ServerDatabase'2_ClientDatabase.cs - Gbtc
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
//  4/19/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.Collections;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Services.Reader;
using GSF.Threading;

namespace GSF.SortedTreeStore.Services
{
    public partial class ServerDatabase<TKey, TValue>
    {
        /// <summary>
        /// A client database that is one part of a <see crServer.ClientHost"/> that wraps a <see cref="ServerDatabase{TKey,TValue}"/>.
        /// </summary>
        public class ClientDatabase
            : ClientDatabaseBase<TKey, TValue>
        {
            private bool m_disposed;
            private ServerDatabase<TKey, TValue> m_server;
            private Server.Client m_client;
            private Action<ClientDatabaseBase> m_onDispose;
            private WeakList<SequentialReaderStream<TKey, TValue>> m_openStreams;

            public ClientDatabase(ServerDatabase<TKey, TValue> server, Server.Client client, Action<ClientDatabaseBase> onDispose)
            {
                if ((object)server == null)
                    throw new ArgumentNullException("server");
                if ((object)client == null)
                    throw new ArgumentNullException("client");
                if ((object)onDispose == null)
                    throw new ArgumentNullException("onDispose");
                if (!ReferenceEquals(client, onDispose.Target))
                    throw new ArgumentException("Does not reference a method in clientHost", "onDispose");

                m_openStreams = new WeakList<SequentialReaderStream<TKey, TValue>>();
                m_server = server;
                m_client = client;
                m_onDispose = onDispose;
            }

            public override bool IsDisposed
            {
                get
                {
                    return m_disposed;
                }
            }

            public override DatabaseInfo Info
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_server.Info;
                }
            }

            public override void SoftCommit()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_server.SoftCommit();
            }

            public override void HardCommit()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_server.HardCommit();
            }

            public override void Dispose()
            {
                if (!m_disposed)
                {
                    foreach (var stream in m_openStreams)
                    {
                        stream.Dispose();
                    }
                    m_onDispose(this);
                    m_disposed = true;
                }
            }

            public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter)
            {
                return Read(readerOptions, keySeekFilter, keyMatchFilter, null);
            }

            public TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter, WorkerThreadSynchronization workerThreadSynchronization)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                var stream = m_server.Read(readerOptions, keySeekFilter, keyMatchFilter, workerThreadSynchronization);

                if (!stream.EOS)
                {
                    stream.Disposed += OnStreamDisposal;
                    m_openStreams.Add(stream);
                }

                return stream;
            }

            void OnStreamDisposal(SequentialReaderStream<TKey, TValue> stream)
            {
                m_openStreams.Remove(stream);
            }

            public override void Write(TreeStream<TKey, TValue> stream)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_server.Write(stream);
            }

            public override void Write(TKey key, TValue value)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_server.Write(key, value);
            }
        }
    }
}
