//******************************************************************************************************
//  QueryResultsCalculation.cs - Gbtc
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
    public class QueryResultsCalculation
    {
        public class MetaDataSignal
        {
            bool m_calculated;
            public MetadataBase Metadata;
            public SignalDataBase Data;
            public MetaDataSignal(MetadataBase metadata, SignalDataBase data)
            {
                Metadata = metadata;
                Data = data;
            }
            public void Calculate(QueryResultsCalculation results)
            {
                if (!m_calculated)
                {
                     Metadata.Calculations.Calculate(results);
                     m_calculated = true; 
                }
            }
            public void HasBeenCalculated()
            {
                m_calculated = true;
            }
        }

        Dictionary<ulong, SignalDataBase> m_results;
        Dictionary<Guid, MetaDataSignal> m_resultsCalculated;

        public QueryResultsCalculation(IHistorianDatabase database, ulong startKey1, ulong endKey1, IEnumerable<MetadataBase> points)
        {
            m_resultsCalculated = new Dictionary<Guid, MetaDataSignal>();
            m_results = new QueryResults(database, startKey1, endKey1, points).Results;

            foreach (var point in points)
            {
                if (point.HistorianId < 0)
                {
                    m_resultsCalculated.Add(point.UniqueId, new MetaDataSignal(point, new SignalData(point)));
                }
                else
                {
                    m_resultsCalculated.Add(point.UniqueId, new MetaDataSignal(point, m_results[(ulong)point.HistorianId]));
                }
            }
            CalculateAllPoints();
        }

        void CalculateAllPoints()
        {
            foreach (var point in m_resultsCalculated.Values)
            {
                point.Calculate(this);
            }
        }

        public MetaDataSignal TryGetSignal(Guid pointId)
        {
            MetaDataSignal signal;
            if (m_resultsCalculated.TryGetValue(pointId, out signal))
            {
                return signal;
            }
            return null;
        }
        public bool TryGetSignal(Guid pointId, out MetaDataSignal signal)
        {
            return m_resultsCalculated.TryGetValue(pointId, out signal);
        }
        public MetaDataSignal GetSignal(Guid pointId)
        {
            return m_resultsCalculated[pointId];
        }

        public IEnumerable<Guid> GetAllPoints()
        {
            return m_resultsCalculated.Keys;
        }

    }

}
