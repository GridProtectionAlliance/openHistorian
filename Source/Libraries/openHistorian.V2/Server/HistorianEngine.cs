//******************************************************************************************************
//  HistorianEngine.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Server.Database;

namespace openHistorian.V2.Server
{
    /// <summary>
    /// The main engine of the openHistorian. Instance this class to host a historian.
    /// </summary>
    public class HistorianEngine
    {
        object m_syncRoot = new object();
        SortedList<string, ArchiveDatabaseEngine> m_databases;

        public HistorianEngine(HistorianSettings settings)
        {
            m_databases = new SortedList<string, ArchiveDatabaseEngine>();
            foreach (var database in settings.Databases)
            {
                m_databases.Add(database.Key, new ArchiveDatabaseEngine(database.Value));
            }
        }

        public void Create(string databaseName, DatabaseSettings settings)
        {

        }

        public void Drop(string databaseName)
        {
            lock (m_syncRoot)
            {

            }
        }

        public void CreateDatabase(string databaseName, IDatabaseConfig config)
        {
            m_databases.Add(databaseName, new ArchiveDatabaseEngine(new DatabaseSettings(config)));
        }

        //public void Attach(string name)
        //{

        //}

        //public void Detach(string name)
        //{

        //}

        //public void TakeOffline(string instanceName)
        //{

        //}

        //public void BringOnline(string instanceName)
        //{

        //}

        public ArchiveDatabaseEngine Get(string databaseName)
        {
            lock (m_syncRoot)
            {
                return m_databases[databaseName.ToUpper()];
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

    }
}
