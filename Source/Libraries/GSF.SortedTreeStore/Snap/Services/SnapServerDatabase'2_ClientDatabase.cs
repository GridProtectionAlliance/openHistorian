//******************************************************************************************************
//  SnapServerDatabase'2_ClientDatabase.cs - Gbtc
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
//  4/19/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Collections;
using GSF.Snap.Filters;
using GSF.Snap.Services.Reader;
using GSF.Threading;

// ReSharper disable NotAccessedField.Local
namespace GSF.Snap.Services
{
    public partial class SnapServerDatabase<TKey, TValue>
    {
        /// <summary>
        /// A client database that is one part of a  <see cref="SnapServer.Client"/> that wraps a <see cref="SnapServerDatabase{TKey,TValue}"/>.
        /// </summary>
        internal class ClientDatabase
            : ClientDatabaseBase<TKey, TValue>
        {
            private readonly object m_syncRoot;
            private bool m_disposed;
            private readonly SnapServerDatabase<TKey, TValue> m_server;
            private readonly SnapServer.Client m_client;
            private readonly Action<ClientDatabaseBase> m_onDispose;
            private readonly WeakList<SequentialReaderStream<TKey, TValue>> m_openStreams;

            public ClientDatabase(SnapServerDatabase<TKey, TValue> server, SnapClient client, Action<ClientDatabaseBase> onDispose)
            {
                m_server = server ?? throw new ArgumentNullException(nameof(server));
                m_client = client as SnapServer.Client ?? throw new ArgumentNullException(nameof(client));
                m_onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));
                
                if (!ReferenceEquals(client, onDispose.Target))
                    throw new ArgumentException("Does not reference a method in clientHost", nameof(onDispose));

                m_syncRoot = new object();
                m_openStreams = new WeakList<SequentialReaderStream<TKey, TValue>>();
                
            }

            public override void AttachFilesOrPaths(IEnumerable<string> paths)
            {
                m_server.AttachFilesOrPaths(paths);
            }

            public override List<ArchiveDetails> GetAllAttachedFiles()
            {
                return m_server.GetAllAttachedFiles();
            }

            public override void DetatchFiles(List<Guid> files)
            {
                m_server.DetatchFiles(files);
            }

            public override void DeleteFiles(List<Guid> files)
            {
                m_server.DeleteFiles(files);
            }

            /// <summary>
            /// Gets if has been disposed.
            /// </summary>
            public override bool IsDisposed => m_disposed;

            /// <summary>
            /// Gets basic information about the current Database.
            /// </summary>
            public override DatabaseInfo Info
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);

                    return m_server.Info;
                }
            }

            /// <summary>
            /// Forces a soft commit on the database. A soft commit 
            /// only commits data to memory. This allows other clients to read the data.
            /// While soft committed, this data could be lost during an unexpected shutdown.
            /// Soft commits usually occur within microseconds. 
            /// </summary>
            public override void SoftCommit()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_server.SoftCommit();
            }

            /// <summary>
            /// Forces a commit to the disk subsystem. Once this returns, the data will not
            /// be lost due to an application crash or unexpected shutdown.
            /// Hard commits can take 100ms or longer depending on how much data has to be committed. 
            /// This requires two consecutive hardware cache flushes.
            /// </summary>
            public override void HardCommit()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_server.HardCommit();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public override void Dispose()
            {
                if (m_disposed)
                    return;

                lock (m_syncRoot)
                {
                    foreach (SequentialReaderStream<TKey, TValue> stream in m_openStreams)
                        stream.Dispose();

                    m_onDispose(this);
                    m_disposed = true;
                }
            }

            /// <summary>
            /// Reads data from the SortedTreeEngine with the provided read options and server side filters.
            /// </summary>
            /// <param name="readerOptions">read options supplied to the reader. Can be null.</param>
            /// <param name="keySeekFilter">a seek based filter to follow. Can be null.</param>
            /// <param name="keyMatchFilter">a match based filer to follow. Can be null.</param>
            /// <returns>A stream that will read the specified data.</returns>
            public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter)
            {
                return Read(readerOptions, keySeekFilter, keyMatchFilter, null);
            }

            public TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter, WorkerThreadSynchronization workerThreadSynchronization)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                SequentialReaderStream<TKey, TValue> stream = m_server.Read(readerOptions, keySeekFilter, keyMatchFilter, workerThreadSynchronization);

                if (!stream.EOS)
                {
                    stream.Disposed += OnStreamDisposal;

                    lock (m_syncRoot)
                        m_openStreams.Add(stream);
                }

                return stream;
            }

            private void OnStreamDisposal(SequentialReaderStream<TKey, TValue> stream)
            {
                lock (m_syncRoot)
                    m_openStreams.Remove(stream);
            }

            /// <summary>
            /// Writes the tree stream to the database. 
            /// </summary>
            /// <param name="stream">all of the key/value pairs to add to the database.</param>
            public override void Write(TreeStream<TKey, TValue> stream)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_server.Write(stream);
            }

            /// <summary>
            /// Writes an individual key/value to the sorted tree store.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public override void Write(TKey key, TValue value)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_server.Write(key, value);
            }
        }
    }
}
