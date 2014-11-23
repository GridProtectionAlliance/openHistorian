//******************************************************************************************************
//  MigrationUtility.SnapDBWriter.Designer.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  11/21/2014 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF;
using GSF.Diagnostics;
using GSF.Historian;
using GSF.Snap.Services;
using GSF.Units;
using openHistorian.Net;
using openHistorian.Snap;

namespace MigrationUtility
{
    partial class MigrationUtility
    {
        private HistorianServer m_server;
        private HistorianServerDatabaseConfig m_archiveInfo;
        private SnapClient m_client;
        private ClientDatabaseBase<HistorianKey, HistorianValue> m_clientDatabase;
        private LogSubscriber m_logSubscriber;

        private void OpenSnapDBEngine(string instanceName, string destinationFilesLocation, string targetFileSize, string directoryNamingMethod)
        {
            m_logSubscriber = Logger.CreateSubscriber();
            m_logSubscriber.Subscribe(Logger.RootSource);
            m_logSubscriber.Subscribe(Logger.RootType);
            m_logSubscriber.Verbose = VerboseLevel.NonDebug ^ VerboseLevel.PerformanceIssue;
            m_logSubscriber.Log += m_logSubscriber_Log;

            if (string.IsNullOrEmpty(instanceName))
                instanceName = "PPA";
            else
                instanceName = instanceName.Trim();

            // Establish archive information for this historian instance
            m_archiveInfo = new HistorianServerDatabaseConfig(instanceName, destinationFilesLocation, true);

            double targetSize;

            if (!double.TryParse(targetFileSize, out targetSize))
                targetSize = 1.5D;

            m_archiveInfo.TargetFileSize = (long)(targetSize * SI.Giga);

            int methodIndex;

            if (!int.TryParse(directoryNamingMethod, out methodIndex))
                methodIndex = (int)ArchiveDirectoryMethod.YearThenMonth;

            m_archiveInfo.DirectoryMethod = (ArchiveDirectoryMethod)methodIndex;

            m_server = new HistorianServer(m_archiveInfo);
            m_client = SnapClient.Connect(m_server.Host);
            m_clientDatabase = m_client.GetDatabase<HistorianKey, HistorianValue>(instanceName);

            ShowUpdateMessage("[SnapDB] Engine initialized");
        }

        private void CloseSnapDBEngine()
        {
            m_client = null;
            m_clientDatabase = null;

            if ((object)m_server != null)
            {
                m_server.Dispose();
                m_server = null;
            }

            if ((object)m_logSubscriber != null)
            {
                m_logSubscriber.Log -= m_logSubscriber_Log;
                m_logSubscriber = null;
            }

            ShowUpdateMessage("[SnapDB] Engine terminated");
        }

        private void WriteSnapDBData(IDataPoint data)
        {
            // Write key information
            m_key.Timestamp = (ulong)data.Time.ToDateTime().Ticks;
            m_key.PointID = (ulong)data.HistorianID;

            // Note that third ulong in key can be used for storing duplicate timestamps
            // such as those duplicates that may come in during leap-seconds. IData will
            // need to expose this indication such that entry number could be updated
            m_key.EntryNumber = 0;

            // Since current time-series measurements are basically all floats - values fit into
            // first ulong, this will change as value types accepted by framework expands
            m_value.Value1 = BitMath.ConvertToUInt64(data.Value);

            // This value will be used when host framework expands to support multiple data types
            m_value.Value2 = 0;

            // While leaving second ulong available for expanded data type storage, we store
            // quality in third ulong for future consistency  - note that this still leaves
            // 32-bits of space available for future use
            m_value.Value3 = (ulong)data.Quality;

            // Write key/value pair to SnapDB engine
            m_clientDatabase.Write(m_key, m_value);
        }

        private void FlushSnapDB()
        {
            m_clientDatabase.HardCommit();
        }

        // Expose SnapDB log messages via Adapter status and exception event raisers
        private void m_logSubscriber_Log(LogMessage logMessage)
        {
            if ((object)logMessage.Exception != null)
                ShowUpdateMessage("[SnapDB] Exception during {0}: {1}", logMessage.EventName, logMessage.GetMessage(true));
            else
                ShowUpdateMessage("[SnapDB] {0}: {1}", logMessage.Level, logMessage.GetMessage(true));
        }
    }
}
