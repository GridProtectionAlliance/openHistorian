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

namespace openHistorian.V2.Local
{
    public partial class HistorianServer
    {
        private class HistorianManage : IManageHistorian
        {
            HistorianServer m_server;
            public HistorianManage(HistorianServer server)
            {
                m_server = server;
            }
            public bool Contains(string databaseName)
            {
                return m_server.m_engine.Contains(databaseName);
            }
            public DatabaseConfig GetConfig(string databaseName)
            {
                throw new NotImplementedException();
            }
            public void SetConfig(string databaseName, DatabaseConfig config)
            {
                throw new NotImplementedException();
            }
            public void Add(string databaseName, DatabaseConfig config = null)
            {
                throw new NotImplementedException();
            }
            public void Drop(string databaseName, float waitTimeSeconds)
            {
                throw new NotImplementedException();
            }
            public void TakeOffline(string databaseName, float waitTimeSeconds)
            {
                throw new NotImplementedException();
            }
            public void BringOnline(string databaseName)
            {
                throw new NotImplementedException();
            }
            public void Shutdown(float waitTimeSeconds)
            {
                throw new NotImplementedException();
            }
        }
    }
}
