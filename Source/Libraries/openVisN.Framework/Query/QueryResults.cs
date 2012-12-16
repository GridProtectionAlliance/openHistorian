//******************************************************************************************************
//  QueryResultsRaw.cs - Gbtc
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
using openHistorian;

namespace openVisN.Query
{
    /// <summary>
    /// Queries the historian database for the provided points. 
    /// Does not support any point that may be calculated.
    /// </summary>
    public class QueryResults
    {
        Dictionary<ulong, SignalDataBase> m_results;

        public QueryResults(IHistorianDatabase database, ulong startKey1, ulong endKey1)
        {
            m_results = new Dictionary<ulong, SignalDataBase>();

            using (var reader = database.OpenDataReader())
            {
                var stream = reader.Read(startKey1, endKey1);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    AddSignalIfNotExists(time, point, value);
                }
            }
        }

        public QueryResults(IHistorianDatabase database, ulong startKey1, ulong endKey1, IEnumerable<ulong> points)
        {
            m_results = new Dictionary<ulong, SignalDataBase>();

            using (var reader = database.OpenDataReader())
            {
                var stream = reader.Read(startKey1, endKey1, points);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    AddSignalIfExists(time, point, value);
                }
            }
        }

        public QueryResults(IHistorianDatabase database, ulong startKey1, ulong endKey1, IEnumerable<MetadataBase> points)
        {
            m_results = new Dictionary<ulong, SignalDataBase>();

            List<ulong> pointIds = new List<ulong>();
            foreach (var point in points)
            {
                if (point.HistorianId >= 0)
                {
                    if (!m_results.ContainsKey((ulong)point.HistorianId))
                    {
                        pointIds.Add((ulong)point.HistorianId);
                        m_results.Add((ulong)point.HistorianId, new SignalData(point));
                    }
                }
            }
            using (var reader = database.OpenDataReader())
            {
                var stream = reader.Read(startKey1, endKey1, pointIds);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    AddSignalIfExists(time, point, value);
                }
            }
        }

        public SignalDataBase GetSignalData(ulong pointId)
        {
            return m_results[pointId];
        }

        void AddSignalIfNotExists(ulong time, ulong point, ulong value)
        {
            SignalDataBase signalData;
            if (!m_results.TryGetValue(point, out signalData))
            {
                signalData = new SignalDataRaw();
                m_results.Add(point, signalData);
            }
            signalData.AddDataRaw(time, value);
        }

        void AddSignalIfExists(ulong time, ulong point, ulong value)
        {
            SignalDataBase signalData;
            if (m_results.TryGetValue(point, out signalData))
                signalData.AddDataRaw(time, value);
        }

        public IEnumerable<ulong> GetAllSignals()
        {
            return m_results.Keys;
        }

        public Dictionary<ulong, SignalDataBase> Results
        {
            get
            {
                return m_results;
            }
        }

    }

}
