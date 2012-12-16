//******************************************************************************************************
//  HistorianQuery.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using openHistorian;
using openHistorian.Communications;

namespace openVisN.Query
{
    public class HistorianQuery
    {
        IHistorianDatabaseCollection m_historian;
        public HistorianQuery(string server, int port)
        {
            var ip = Dns.GetHostAddresses(server)[0];
            m_historian = new RemoteHistorian(new IPEndPoint(ip, port));
        }
        public HistorianQuery(IHistorianDatabaseCollection historian)
        {
            m_historian = historian;
        }

        public QueryResultsCalculation GetQueryResult(DateTime startTime, DateTime endTime, int zoomLevel, List<MetadataBase> signals)
        {
            //ToDo: Modify the query base on the zoom level
            var db = m_historian.ConnectToDatabase("Full Resolution Synchrophasor");
            return new QueryResultsCalculation(db, (ulong)startTime.Ticks, (ulong)endTime.Ticks, signals);
        }

    }

}
