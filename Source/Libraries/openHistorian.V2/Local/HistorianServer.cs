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
//  10/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using openHistorian.V2.Server;

namespace openHistorian.V2.Local
{
    public partial class HistorianServer : IHistorian
    {
        HistorianEngine m_engine;
        ConfigHistorian m_config;

        public HistorianServer()
        {
        }

        public IHistorianReadWrite ConnectToDatabase(string databaseName)
        {
            var database = m_engine.Get(databaseName);
            return new HistorianReadWrite(this, database);
        }

        public bool IsCommitted(long transactionId)
        {
            return true;
        }

        public bool IsDiskCommitted(long transactionId)
        {
            return true;
        }

        public IManageHistorian Manage()
        {
            return new HistorianManage(this);
        }
    }
}
