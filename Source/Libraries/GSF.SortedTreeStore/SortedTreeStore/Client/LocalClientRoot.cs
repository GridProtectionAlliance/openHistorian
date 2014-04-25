//******************************************************************************************************
//  LocalClientRoot.cs - Gbtc
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
using System.Collections.Generic;
using GSF.SortedTreeStore.Server;

namespace GSF.SortedTreeStore.Client
{
    /// <summary>
    /// A client wrapper around a <see cref="ServerRoot"/>. Clients are intended to be single threaded. 
    /// </summary>
    internal class LocalClientRoot
        : ClientRootBase
    {
        bool m_disposed;
        ServerRoot m_collection;
        Action<LocalClientRoot> m_onDispose;
        ClientDatabaseBase m_connectedDatabase;

        /// <summary>
        /// Creates a <see cref="LocalClientRoot"/>
        /// </summary>
        /// <param name="collection">the collection to wrap</param>
        /// <param name="onDispose">the action to take on disposing</param>
        public LocalClientRoot(ServerRoot collection, Action<LocalClientRoot> onDispose)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (onDispose == null)
                throw new ArgumentNullException("onDispose");
            if (onDispose.Target != collection)
                throw new ArgumentException("delegate is not pointing to a method in collection", "onDispose");
            m_collection = collection;
            m_onDispose = onDispose;
        }

        /// <summary>
        /// Gets the database that matches <see cref="databaseName"/>
        /// </summary>
        /// <param name="databaseName">the case insensitive name of the databse</param>
        /// <returns></returns>
        public override ClientDatabaseBase GetDatabase(string databaseName)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_connectedDatabase != null && !m_connectedDatabase.IsDisposed)
                throw new Exception("Dispose previous database before connecting to another one.");
            databaseName = databaseName.ToUpper();
            m_connectedDatabase = m_collection.GetDatabase(databaseName).CreateLocalClientDatabase();
            return m_connectedDatabase;
        }

        /// <summary>
        /// Accesses <see cref="ClientDatabaseBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="ClientDatabaseBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public override ClientDatabaseBase<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName)
        {
            return (LocalClientDatabase<TKey, TValue>)GetDatabase(databaseName);
        }

        /// <summary>
        /// Determines if <see cref="databaseName"/> is contained in the database.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns></returns>
        public override bool Contains(string databaseName)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return m_collection.Contains(databaseName);
        }

        /// <summary>
        /// Gets basic information for every database connected to the server.
        /// </summary>
        /// <returns></returns>
        public override List<DatabaseInfo> GetDatabaseInfo()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return m_collection.GetDatabaseInfo();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (!m_disposed)
            {
                if (m_connectedDatabase != null && !m_connectedDatabase.IsDisposed)
                    m_connectedDatabase.Dispose();
                m_connectedDatabase = null;
                m_onDispose(this);
                m_onDispose = null;
                m_collection = null;
                m_disposed = true;
            }
        }
    }
}
