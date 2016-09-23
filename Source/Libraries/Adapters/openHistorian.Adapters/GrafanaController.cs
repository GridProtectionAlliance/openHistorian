//******************************************************************************************************
//  GrafanaController.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  09/15/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using GrafanaAdapters;
using GSF.Collections;
using GSF.Snap.Services;
using GSF.Threading;
using GSF.Web.Security;
using openHistorian.Model;
using openHistorian.Snap;
using CancellationToken = System.Threading.CancellationToken;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana data source.
    /// </summary>
    [AuthorizeControllerRole]
    public class GrafanaController : ApiController
    {
        #region [ Members ]

        // Nested Types
        private class HistorianDataSource : GrafanaDataSourceBase
        {
            protected override List<TimeSeriesValues> QueryTimeSeriesValues(DateTime startTime, DateTime stopTime, int maxDataPoints, Dictionary<ulong, string> targetMap, CancellationToken cancellationToken)
            {
                Dictionary<ulong, TimeSeriesValues> queriedTimeSeriesValues = new Dictionary<ulong, TimeSeriesValues>();

                if (targetMap.Count > 0)
                {
                    ulong[] measurementIDs = targetMap.Keys.ToArray();
                    Resolution resolution = TrendValueAPI.EstimatePlotResolution(startTime, stopTime);

                    using (SnapClient connection = SnapClient.Connect(LocalOutputAdapter.Instances[InstanceName].Server.Host))
                    using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(InstanceName))
                    {
                        foreach (TrendValue trendValue in TrendValueAPI.GetHistorianData(database, startTime, stopTime, measurementIDs, resolution, maxDataPoints, true, (CompatibleCancellationToken)cancellationToken))
                        {
                            queriedTimeSeriesValues.GetOrAdd((ulong)trendValue.ID, id => new TimeSeriesValues { target = targetMap[id], datapoints = new List<double[]>() })
                                .datapoints.Add(new[] { trendValue.Value, trendValue.Timestamp });
                        }
                    }
                }

                return queriedTimeSeriesValues.Values.ToList();
            }
        }

        // Fields
        private readonly HistorianDataSource m_dataSource;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="GrafanaController"/>.
        /// </summary>
        public GrafanaController()
        {
            m_dataSource = new HistorianDataSource
            {
                InstanceName = TrendValueAPI.InstanceName,
                Metadata = LocalOutputAdapter.Instances[TrendValueAPI.InstanceName].DataSource
            };
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Validates that openHistorian Grafana data source is responding as expected.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Index()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Queries openHistorian as a Grafana data source.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public Task<List<TimeSeriesValues>> Query(QueryRequest request, CancellationToken cancellationToken)
        {
            return m_dataSource.Query(request, cancellationToken);
        }

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        public Task<string[]> Search(Target request)
        {
            return m_dataSource.Search(request);
        }

        /// <summary>
        /// Queries openHistorian for annotations in a time-range (e.g., Alarms).
        /// </summary>
        /// <param name="request">Annotation request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public Task<List<AnnotationResponse>> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
        {
            return m_dataSource.Annotations(request, cancellationToken);
        }

        #endregion
    }
}
