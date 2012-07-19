//******************************************************************************************************
//  DatabaseEngine.cs - Gbtc
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

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class DatabaseEngine
    {
        DataWriter m_dataWriter;
        //DataReader m_dataReader;
        DataList m_dataList;
        PartitionInitializer m_partitionInitializer;
        List<DataManagement> m_dataManagement;

        //RolloverEngine m_rolloverEngine;
        //DataWriter m_dataWriter;
        //InboundPointQueue m_newPointQueue;
        //DatabaseEngineSettings m_settings;

        public DatabaseEngine(DatabaseEngineSettings settings)
        {
            var partitionCriteria = default(NewPartitionCriteria);
            partitionCriteria.CommitCount = 1000;
            partitionCriteria.PartitionSize = 10 * 1024 * 1024; //10MB
            partitionCriteria.Interval = new TimeSpan(0, 0, 10); //10 Seconds
            partitionCriteria.IsCommitCountValid = true;
            partitionCriteria.IsPartitionSizeValid = true;
            partitionCriteria.IsIntervalValid = true;

            var partitionCriteria2 = default(NewPartitionCriteria);
            partitionCriteria2.CommitCount = 1000;
            partitionCriteria2.PartitionSize = 1 * 1024 * 1024 * 1024; //1 GB
            partitionCriteria2.Interval = new TimeSpan(0, 10, 0); //10 Minutes
            partitionCriteria2.IsCommitCountValid = true;
            partitionCriteria2.IsPartitionSizeValid = true;
            partitionCriteria2.IsIntervalValid = true;

            m_partitionInitializer = new PartitionInitializer(null);
            m_dataList = new DataList();
            m_dataWriter = new DataWriter(m_partitionInitializer, m_dataList, 100, 10000, partitionCriteria);

            m_dataManagement = new List<DataManagement>();
            m_dataManagement.Add(new DataManagement(m_partitionInitializer, m_dataList, partitionCriteria2, 0));


            //m_settings = settings;
            //m_dataWriter = new DataWriter(settings.ResourceEngineSettings);
            //m_rolloverEngine = new RolloverEngine();
            //m_newPointQueue = m_rolloverEngine.ProcessInserts.NewPointQueue;
        }

        public long LookupPointId(Guid pointId)
        {
            return -1;
        }

        public void WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            m_dataWriter.WriteData(key1, key2, value1, value2);
        }

    }
}
