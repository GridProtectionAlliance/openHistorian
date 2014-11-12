//******************************************************************************************************
//  HistorianQuery.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.SortedTreeStore.Services.Net;
using GSF.SortedTreeStore.Services.Reader;
using openHistorian.Collections;

namespace openHistorian.Data.Query
{
    public class HistorianQuery
    {
        private readonly NetworkClient m_historian;
        private int m_samplesPerSecond = 30;

        public HistorianQuery(string server, int port)
        {
            //IPAddress ip = Dns.GetHostAddresses(server)[0];
            //m_historian = new RemoteHistorian<HistorianKey, HistorianValue>(new IPEndPoint(ip, port));
        }

        public HistorianQuery(NetworkClient historian)
        {
            m_historian = historian;
        }

        public IDictionary<Guid, SignalDataBase> GetQueryResult(DateTime startTime, DateTime endTime, int zoomLevel, IEnumerable<ISignalCalculation> signals)
        {
            using (var db = m_historian.GetDatabase<HistorianKey, HistorianValue>("PPA"))
            {
                //var db = m_historian.ConnectToDatabase("Full Resolution Synchrophasor");
                PeriodicScanner scanner = new PeriodicScanner(m_samplesPerSecond);
                var timestamps = scanner.GetParser(startTime, endTime, 1500u);
                SortedTreeEngineReaderOptions options = new SortedTreeEngineReaderOptions(TimeSpan.FromSeconds(1));
                var results = db.GetSignalsWithCalculations(timestamps, signals, options);
                return results;
            }
        }
    }
}