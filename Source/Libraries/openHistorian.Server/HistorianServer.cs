//******************************************************************************************************
//  HistorianServer.cs - Gbtc
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
//  12/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using openHistorian.Communications;
using openHistorian.Engine;

namespace openHistorian
{
    public class HistorianServerOptions
    {
        public bool IsNetworkHosted = false;
        public List<string> Paths = new List<string>();
        public bool IsReadOnly = true;
        public int NetworkPort = 38402;
    }

    public class HistorianServerDatabaseCollectionOptions
    {
        public bool IsNetworkHosted = false;
        public int NetworkPort = 38402;
        public List<HistorianServerDatabaseSettings> Databases = new List<HistorianServerDatabaseSettings>();
    }

    public class HistorianServerDatabaseSettings
    {
        public List<string> Paths = new List<string>();
        public bool IsReadOnly = true;
        public string DatabaseName;
    }
    
    public class HistorianServer : IDisposable
    {
        SocketHistorian m_socket;
        HistorianDatabaseCollection m_databases;

        public IHistorianDatabaseCollection GetDatabaseCollection()
        {
            return m_databases;
        }

        public IHistorianDatabase GetDatabase()
        {
            return m_databases.ConnectToDatabase("default");
        }

        public HistorianServer(HistorianServerOptions options)
        {
            var serverOptions = new HistorianServerDatabaseCollectionOptions();
            var dbOptions = new HistorianServerDatabaseSettings();
            dbOptions.DatabaseName = "default";
            dbOptions.Paths = options.Paths;
            dbOptions.IsReadOnly = options.IsReadOnly;
            serverOptions.Databases.Add(dbOptions);
            serverOptions.IsNetworkHosted = options.IsNetworkHosted;
            serverOptions.NetworkPort = options.NetworkPort;
            Initialize(serverOptions);
        }

        public HistorianServer(HistorianServerDatabaseCollectionOptions options)
        {
           Initialize(options);
        }

        void Initialize(HistorianServerDatabaseCollectionOptions options)
        {
            m_databases = new HistorianDatabaseCollection();

            foreach (var dbOption in options.Databases)
            {
                if (dbOption.IsReadOnly)
                {
                    var database = new ArchiveDatabaseEngine(null, dbOption.Paths.ToArray());
                    m_databases.Add(dbOption.DatabaseName, database);
                }
                else
                {
                    var database = new ArchiveDatabaseEngine(WriterOptions.IsFileBased(), dbOption.Paths.ToArray());
                    m_databases.Add(dbOption.DatabaseName, database);
                }
            }

            if (options.IsNetworkHosted)
            {
                m_socket = new SocketHistorian(options.NetworkPort, m_databases);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            m_databases.Dispose();
        }
    }
}
