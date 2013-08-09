//******************************************************************************************************
//  HistorianDatabaseCollection.cs - Gbtc
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
//  12/9/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Collections;

namespace openHistorian
{
    /// <summary>
    /// A simple way to implement <see cref="IHistorianDatabaseCollection"/>.
    /// </summary>
    public class HistorianDatabaseCollection<TKey, TValue>
        : IDisposable, IHistorianDatabaseCollection<TKey, TValue>
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : class, new()
    {
        private bool m_disposed;

        private readonly object m_syncRoot = new object();

        private readonly SortedList<string, IHistorianDatabase<TKey, TValue>> m_databases;

        public HistorianDatabaseCollection()
        {
            m_databases = new SortedList<string, IHistorianDatabase<TKey, TValue>>();
        }

        //public HistorianDatabaseCollection(string configFileName)
        //    : this()
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Accesses <see cref="IHistorianDatabase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="IHistorianDatabase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public IHistorianDatabase<TKey, TValue> this[string databaseName]
        {
            get
            {
                lock (m_syncRoot)
                {
                    return m_databases[databaseName.ToUpper()];
                }
            }
        }

        //public IHistorianDatabase<TKey, TValue> ConnectToDatabase(string databaseName)
        //{
        //    lock (m_syncRoot)
        //    {
        //        return m_databases[databaseName.ToUpper()];
        //    }
        //}

        public void Add(string databaseName, IHistorianDatabase<TKey, TValue> database)
        {
            lock (m_syncRoot)
            {
                m_databases.Add(databaseName.ToUpper(), database);
            }
        }

        public bool Contains(string databaseName)
        {
            lock (m_syncRoot)
            {
                return m_databases.ContainsKey(databaseName.ToUpper());
            }
        }

        public List<string> GetDatabaseNames()
        {
            lock (m_syncRoot)
            {
                return new List<string>(m_databases.Keys);
            }
        }

        public object SyncRoot
        {
            get
            {
                return m_syncRoot;
            }
        }

        /// <summary>
        /// Detaches the provided database from the server. 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="waitTimeSeconds"></param>
        public void Remove(string databaseName, float waitTimeSeconds = 0)
        {
            // TODO: Should this dispose of the database? Or is it assumed instance is not owned by collection...
            // TODO: waitSeconds is not used - is this for waiting to flush? need to remove otherwise
            lock (m_syncRoot)
            {
                m_databases.Remove(databaseName.ToUpper());
            }
        }

        /// <summary>
        /// Shuts down the entire server.
        /// </summary>
        /// <param name="waitTimeSeconds"></param>
        public void Shutdown(float waitTimeSeconds)
        {
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                foreach (IHistorianDatabase<TKey, TValue> db in m_databases.Values)
                {
                    db.Dispose();
                }
            }
        }
    }
}