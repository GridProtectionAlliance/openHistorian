//******************************************************************************************************
//  GrafanaPhasorController.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/27/2017 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using GrafanaAdapters;
using System.Linq;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana "phasor" based data source,
    /// accessible from Grafana data source as http://localhost:8180/api/grafanaphasor
    /// </summary>
    public class GrafanaPhasorController : GrafanaController
    {
        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        public Task<IEnumerable<Tuple<string, string, string>>> SearchPhasors(Target request)
        {
            string target = request.target == "select metric" ? "" : request.target;

            HistorianDataSource dataSource = DataSource;

            if ((object)dataSource == null)
                return Task.FromResult(Enumerable.Empty<Tuple<string, string, string>>());

            return Task.Factory.StartNew(() =>
            {
                return dataSource.Metadata.Tables["Phasors"].Select($"Label LIKE '%{target}%'").Take(dataSource.MaximumSearchTargetsPerRequest).Select(row => Tuple.Create(row["Label"].ToString(), row["MagPointTag"].ToString(), row["AnglePointTag"].ToString()));
            });
        }
    }
}
