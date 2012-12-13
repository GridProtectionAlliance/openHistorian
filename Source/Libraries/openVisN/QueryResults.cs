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
//  12/07/2012 - Ritchie
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openVisN
{
    public class QueryResults
    {
        Dictionary<ulong, List<KeyValuePair<ulong, ulong>>> m_results;
        public QueryResults()
        {
            m_results = new Dictionary<ulong, List<KeyValuePair<ulong, ulong>>>();
        }

        public void AddPointIfNotExists(ulong pointId)
        {
            if (!m_results.ContainsKey(pointId))
                m_results.Add(pointId, new List<KeyValuePair<ulong, ulong>>());
        }

        public List<KeyValuePair<ulong, ulong>> GetPointList(ulong pointId)
        {
            return m_results[pointId];
        }

        public void AddPoint(ulong time, ulong point, ulong value)
        {
            m_results[point].Add(new KeyValuePair<ulong, ulong>(time, value));
        }

        public IEnumerable<ulong> GetAllPoints()
        {
            return m_results.Keys;
        }

    }

}
