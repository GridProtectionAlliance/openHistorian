//******************************************************************************************************
//  LocalClientDatabase'2.cs - Gbtc
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
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Server;
using GSF.SortedTreeStore.Server.Reader;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Client
{
    /// <summary>
    /// A client database that is one part of a <see cref="LocalClientRoot"/> that wraps a 
    /// <see cref="ServerDatabase{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class LocalClientDatabase<TKey, TValue>
        : ClientDatabaseBase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        bool m_disposed;
        ServerDatabase<TKey, TValue> m_server;
        TreeStream<TKey, TValue> m_lastRead;

        public LocalClientDatabase(ServerDatabase<TKey, TValue> server)
        {
            m_server = server;
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
            if (m_lastRead != null && !m_lastRead.IsDisposed)
                throw new Exception("Call to commit before disposing of previous read");
            m_server.SoftCommit();
        }

        public override void HardCommit()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_lastRead != null && !m_lastRead.IsDisposed)
                throw new Exception("Call to commit before disposing of previous read");
            m_server.HardCommit();
        }

        public override void Dispose()
        {
            if (!m_disposed)
            {
                if (m_lastRead != null && !m_lastRead.IsDisposed)
                    m_lastRead.Dispose();
                m_lastRead = null;
                m_disposed = true;
            }
        }

        public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_lastRead != null && !m_lastRead.IsDisposed)
                throw new Exception("Duplicate calls to Read without disposing previous read.");

            m_lastRead = m_server.Read(readerOptions, keySeekFilter, keyMatchFilter);
            return m_lastRead;
        }

        public override void Write(TreeStream<TKey, TValue> stream)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_lastRead != null && !m_lastRead.IsDisposed)
                throw new Exception("Call to write before disposing of previous read");
            m_server.Write(stream);
        }

        public override void Write(TKey key, TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_lastRead != null && !m_lastRead.IsDisposed)
                throw new Exception("Call to write before disposing of previous read");
            m_server.Write(key, value);
        }
    }
}
