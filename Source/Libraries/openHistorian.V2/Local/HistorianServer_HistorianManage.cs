//******************************************************************************************************
//  HistorianServer_HistorianManage.cs - Gbtc
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
//  10/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;

namespace openHistorian.V2.Local
{
    public partial class HistorianServer
    {
        private class HistorianManage : IManageHistorian
        {
            SortedList<string, DatabaseConfig> m_config;
            HistorianServer m_server;

            string m_configFileName;

            public HistorianManage(HistorianServer server, string configFileName = null)
            {
                m_config = new SortedList<string, DatabaseConfig>();
                m_server = server;
                m_configFileName = configFileName;

                if (m_configFileName != null)
                {
                    if (File.Exists(configFileName))
                    {
                        Load();
                    }
                    else
                    {
                        Save();
                    }
                }
            }
            
            public bool Contains(string databaseName)
            {
                return m_config.ContainsKey(databaseName.ToUpper());
            }
            public IDatabaseConfig GetConfig(string databaseName)
            {
                throw new NotImplementedException();
                return m_config[databaseName.ToUpper()].Clone();
            }
            public void SetConfig(string databaseName, IDatabaseConfig config)
            {
                throw new NotImplementedException();
                DatabaseConfig cfg = config as DatabaseConfig;
                if (cfg == null)
                    throw new ArgumentException("Must be the same type as received from GetConfig.", "config");
                m_config[databaseName.ToUpper()] = cfg.Clone();
                Save();
            }
            public void Add(string databaseName, IDatabaseConfig config = null)
            {
                if (config == null)
                    config = new DatabaseConfig();
                DatabaseConfig cfg = config as DatabaseConfig;
                if (cfg == null)
                    throw new ArgumentException("Must be the same type as received from GetConfig.", "config");
                m_server.m_engine.CreateDatabase(databaseName, config);
                m_config.Add(databaseName.ToUpper(), cfg.Clone());
                Save();
            }
            public void Drop(string databaseName, float waitTimeSeconds)
            {
                throw new NotImplementedException();
                m_config.Remove(databaseName.ToUpper());
                Save();
            }
            public void TakeOffline(string databaseName, float waitTimeSeconds)
            {
                throw new NotImplementedException();
                m_config[databaseName].IsOnline = false;
                Save();
            }
            public void BringOnline(string databaseName)
            {
                throw new NotImplementedException();
                m_config[databaseName].IsOnline = true;
                Save();
            }

            public void Shutdown(float waitTimeSeconds)
            {
                m_server.Dispose();
            }

            public IDatabaseConfig CreateConfig()
            {
                return new DatabaseConfig();
            }

            public IDatabaseConfig CreateConfig(WriterOptions writerOptions)
            {
                var cfg = new DatabaseConfig();
                cfg.Writer = writerOptions;
                return cfg;
            }

            void Save()
            {
                if (m_configFileName != null)
                {

                }
            }
            void Load()
            {
                
            }

        }
    }
}
