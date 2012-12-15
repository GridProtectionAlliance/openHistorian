//******************************************************************************************************
//  QueryResults.cs - Gbtc
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

namespace openVisN
{
    public class QueryResults
    {
        Dictionary<ulong, PointResults> m_results;
        Dictionary<Guid, PointResults> m_resultsCalculated;

        public QueryResults()
        {
            m_results = new Dictionary<ulong, PointResults>();
            m_resultsCalculated = new Dictionary<Guid, PointResults>();
        }

        public QueryResults(IHistorianDatabase database, ulong startKey1, ulong endKey1, IEnumerable<MetadataBase> points)
            : this()
        {
            List<ulong> ids = new List<ulong>();
            foreach (var pt in points)
            {
                AddPointIfNotExists(pt);
                ids.Add(pt.HistorianId);
            }

            using (var reader = database.OpenDataReader())
            {
                var stream = reader.Read(startKey1, endKey1, ids);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    AddPoint(time, point, value);
                }
            }
        }

        public void AddPointIfNotExists(MetadataBase point)
        {
            if (!m_results.ContainsKey(point.HistorianId))
                m_results.Add(point.HistorianId, new PointResults(point));
        }

        public PointResults GetPointList(ulong pointId)
        {
            return m_results[pointId];
        }
        public PointResults GetPointList(Guid pointId)
        {
            return m_resultsCalculated[pointId];
        }

        public void AddPoint(ulong time, ulong point, ulong value)
        {
            m_results[point].AddPoint(time, value);
        }

        public IEnumerable<ulong> GetAllPoints()
        {
            return m_results.Keys;
        }

    }

}
