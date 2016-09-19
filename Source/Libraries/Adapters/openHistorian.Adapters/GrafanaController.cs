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
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using GSF.Collections;
using GSF.Historian.DataServices.Grafana;
using GSF.Snap.Services;
using GSF.Threading;
using GSF.TimeSeries.Adapters;
using GSF.Web.Security;
using Newtonsoft.Json;
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
            // Task allows processing of multiple simultaneous queries
            return Task.Factory.StartNew(() =>
            {
                if (!request.format?.Equals("json", StringComparison.OrdinalIgnoreCase) ?? false)
                    throw new InvalidOperationException("Only JSON formatted query requests are currently supported.");

                DateTime startTime = ParseJsonTimestamp(request.range.from);
                DateTime stopTime = ParseJsonTimestamp(request.range.to);
                Dictionary<ulong, DataRow> metadata = LocalOutputAdapter.Instances[TrendValueAPI.InstanceName].Measurements;
                HashSet<string> targets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (string requestTarget in request.targets.Select(requestTarget => requestTarget.target))
                {
                    if (string.IsNullOrWhiteSpace(requestTarget))
                        continue;

                    foreach (string targetItem in requestTarget.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string target = targetItem.Trim();
                        
                        if (target.StartsWith("FILTER", StringComparison.OrdinalIgnoreCase))
                        {
                            string tableName, whereExpression, sortField;
                            int takeCount;

                            if (AdapterBase.ParseFilterExpression(target, out tableName, out whereExpression, out sortField, out takeCount))
                            {
                                if (takeCount == int.MaxValue)
                                    takeCount = 10;


                            }
                        }
                        else
                        {
                            targets.Add(target);
                        }
                    }
                }

                Dictionary<ulong, string> targetMap = targets.Select(target => new KeyValuePair<ulong, string>(metadata.FirstOrDefault(kvp => target.Split(' ')[0].Equals(kvp.Value["ID"].ToString(), StringComparison.OrdinalIgnoreCase)).Key, target)).Where(kvp => kvp.Key > 0UL).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                Dictionary<ulong, TimeSeriesValues> queriedTimeSeriesValues = new Dictionary<ulong, TimeSeriesValues>();

                if (targetMap.Count > 0)
                {
                    ulong[] measurementIDs = targetMap.Keys.ToArray();
                    Resolution resolution = TrendValueAPI.EstimatePlotResolution(startTime, stopTime);

                    using (SnapClient connection = SnapClient.Connect(LocalOutputAdapter.Instances[TrendValueAPI.InstanceName].Server.Host))
                    using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(TrendValueAPI.InstanceName))
                    {
                        foreach (TrendValue trendValue in TrendValueAPI.GetHistorianData(database, startTime, stopTime, measurementIDs, resolution, request.maxDataPoints, true, (CompatibleCancellationToken)cancellationToken))
                        {
                            queriedTimeSeriesValues.GetOrAdd((ulong)trendValue.ID, id => new TimeSeriesValues { target = targetMap[id], datapoints = new List<double[]>() })
                                .datapoints.Add(new[] { trendValue.Value, trendValue.Timestamp });
                        }
                    }
                }

                return new List<TimeSeriesValues>(queriedTimeSeriesValues.Values);
            },
            cancellationToken);
        }

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        public string[] Search(Target request)
        {
            return LocalOutputAdapter.Instances[TrendValueAPI.InstanceName].Measurements.Take(200).Select(entry => $"{entry.Value["ID"]} [{entry.Value["PointTag"]}]").ToArray();
        }

        /// <summary>
        /// Queries openHistorian for annotations in a time-range (e.g., Alarms).
        /// </summary>
        /// <param name="request">Annotation request.</param>
        public List<AnnotationResponse> Annotations(AnnotationRequest request)
        {
            return new List<AnnotationResponse>();
        }

        private static DateTime ParseJsonTimestamp(string timestamp)
        {
            DateTimeOffset dto = JsonConvert.DeserializeObject<DateTimeOffset>($"\"{timestamp}\"");
            return dto.UtcDateTime;
        }
    }
}
