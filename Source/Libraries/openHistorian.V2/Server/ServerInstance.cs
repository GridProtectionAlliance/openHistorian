//******************************************************************************************************
//  ServerInstance.cs - Gbtc
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
using openHistorian.V2.Server.Database;

namespace openHistorian.V2.Server
{
    /// <summary>
    /// The main engine of the openHistorian. Instance this class to host a historian.
    /// </summary>
    public class ServerInstance
    {
        object m_syncRoot = new object();
        SortedList<string, ArchiveManagementSystem> m_databases;

        public ServerInstance()
        {
            m_databases = new SortedList<string, ArchiveManagementSystem>();
        }

        public void Create(string name, DatabaseEngineSettings settings)
        {
            lock (m_syncRoot)
            {
                ArchiveManagementSystem engine = new ArchiveManagementSystem(settings);
                m_databases.Add(name.ToUpper(), engine);
            }
        }

        public void Drop(string name)
        {
            lock (m_syncRoot)
            {
                var engine = Get(name);
                m_databases.Remove(name.ToUpper());
            }
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

        public ArchiveManagementSystem Get(string name)
        {
            lock (m_syncRoot)
            {
                return m_databases[name.ToUpper()];
            }
        }

        public bool Exists(string name)
        {
            lock (m_syncRoot)
            {
                return m_databases.ContainsKey(name.ToUpper());
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
