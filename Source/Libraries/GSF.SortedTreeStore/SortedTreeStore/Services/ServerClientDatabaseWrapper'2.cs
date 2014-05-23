//******************************************************************************************************
//  ServerClientDatabaseWrapper'2.cs - Gbtc
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
//  05/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Services.Reader;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Creates a helper wrapper around a client when a user wants to connect directly to a database 
    /// without dealing with the root database information.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ServerClientDatabaseWrapper<TKey, TValue>
        : ClientDatabaseBase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private Server.Client m_client;
        private ClientDatabaseBase<TKey, TValue> m_database;

        public ServerClientDatabaseWrapper(Server host, string database)
        {
            m_client = host.CreateClientHost();
            m_database = m_client.GetDatabase<TKey, TValue>(database);
        }

        public override bool IsDisposed
        {
            get
            {
                return m_database.IsDisposed;
            }
        }

        public override DatabaseInfo Info
        {
            get
            {
                return m_database.Info;
            }
        }

        public override void SoftCommit()
        {
            m_database.SoftCommit();
        }

        public override void HardCommit()
        {
            m_database.HardCommit();
        }

        public override void Dispose()
        {
            m_database.Dispose();
            m_client.Dispose();
        }

        public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter)
        {
            return m_database.Read(readerOptions, keySeekFilter, keyMatchFilter);
        }

        public override void Write(TreeStream<TKey, TValue> stream)
        {
            m_database.Write(stream);
        }

        public override void Write(TKey key, TValue value)
        {
            m_database.Write(key, value);
        }
    }
}
