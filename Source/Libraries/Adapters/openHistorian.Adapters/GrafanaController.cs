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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using GrafanaAdapters;
using GSF;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using openHistorian.Snap;
using CancellationToken = System.Threading.CancellationToken;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana data source.
    /// </summary>
    public class GrafanaController : ApiController
    {
        #region [ Members ]

        // Nested Types
        private class HistorianDataSource : GrafanaDataSourceBase
        {
            private readonly ulong m_baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;
            
            protected override IEnumerable<DataSourceValue> QueryDataSourceValues(DateTime startTime, DateTime stopTime, string interval, bool decimate, Dictionary<ulong, string> targetMap)
            {
                SnapServer server = GetAdapterInstance(InstanceName)?.Server?.Host;

                if ((object)server == null)
                    yield break;

                using (SnapClient connection = SnapClient.Connect(server))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(InstanceName))
                {
                    if ((object)database == null)
                        yield break; 

                    Resolution resolution = TrendValueAPI.EstimatePlotResolution(InstanceName, startTime, stopTime, targetMap.Keys);
                    SeekFilterBase<HistorianKey> timeFilter;

                    // Set data scan resolution
                    if (!decimate || resolution == Resolution.Full)
                    {
                        timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);
                    }
                    else
                    {
                        TimeSpan resolutionInterval = resolution.GetInterval();
                        BaselineTimeInterval timeInterval = BaselineTimeInterval.Second;

                        if (resolutionInterval.Ticks < Ticks.PerMinute)
                            timeInterval = BaselineTimeInterval.Second;
                        else if (resolutionInterval.Ticks < Ticks.PerHour)
                            timeInterval = BaselineTimeInterval.Minute;
                        else if (resolutionInterval.Ticks == Ticks.PerHour)
                            timeInterval = BaselineTimeInterval.Hour;

                        startTime = startTime.BaselinedTimestamp(timeInterval);
                        stopTime = stopTime.BaselinedTimestamp(timeInterval);

                        timeFilter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, resolutionInterval, new TimeSpan(TimeSpan.TicksPerMillisecond));
                    }

                    // Setup point ID selections
                    MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(targetMap.Keys);

                    // Start stream reader for the provided time window and selected points
                    using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter))
                    {
                        HistorianKey key = new HistorianKey();
                        HistorianValue value = new HistorianValue();

                        while (stream.Read(key, value))
                        {
                            yield return new DataSourceValue
                            {
                                Target = targetMap[key.PointID],
                                Time = (key.Timestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                                Value = value.AsSingle
                            };
                        }
                    }
                }
            }
        }

        // Fields
        private HistorianDataSource m_dataSource;

        #endregion

        #region [ Properties ]

        private HistorianDataSource DataSource
        {
            get
            {
                if ((object)m_dataSource == null)
                {
                    string uriPath = Request.RequestUri.PathAndQuery;
                    string instanceName;

                    if (uriPath.StartsWith(DefaultApiPath, StringComparison.OrdinalIgnoreCase))
                    {
                        // No instance provided in URL, use default instance name
                        instanceName = TrendValueAPI.DefaultInstanceName;
                    }
                    else
                    {
                        string[] pathElements = uriPath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                        if (pathElements.Length > 2)
                            instanceName = pathElements[1].Trim();
                        else
                            throw new InvalidOperationException($"Unexpected API URL route destination encountered: {Request.RequestUri}");
                    }

                    if (!string.IsNullOrWhiteSpace(instanceName))
                    {
                        LocalOutputAdapter adapterInstance = GetAdapterInstance(instanceName);

                        if ((object)adapterInstance != null)
                        {
                            m_dataSource = new HistorianDataSource
                            {
                                InstanceName = instanceName,
                                Metadata = adapterInstance.DataSource
                            };
                        }
                    }
                }

                return m_dataSource;
            }
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
            return DataSource?.Query(request, cancellationToken) ?? Task.FromResult(new List<TimeSeriesValues>());
        }

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        public Task<string[]> Search(Target request)
        {
            return DataSource?.Search(request) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Search openHistorian for a field.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        public Task<string[]> SearchFields(Target request)
        {
            return DataSource?.SearchFields(request) ?? Task.FromResult(new string[0]);
        }


        /// <summary>
        /// Search openHistorian for a table.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        public Task<string[]> SearchFilters(Target request)
        {
            return DataSource?.SearchFilters(request) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Search openHistorian for a field.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        public Task<string[]> SearchOrderBys(Target request)
        {
            return DataSource?.SearchOrderBys(request) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Queries openHistorian for annotations in a time-range (e.g., Alarms).
        /// </summary>
        /// <param name="request">Annotation request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public Task<List<AnnotationResponse>> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.Annotations(request, cancellationToken) ?? Task.FromResult(new List<AnnotationResponse>());
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly string DefaultApiPath = "/api/grafana";

        // Static Methods
        private static LocalOutputAdapter GetAdapterInstance(string instanceName)
        {
            if (!string.IsNullOrWhiteSpace(instanceName))
            {
                LocalOutputAdapter adapterInstance;

                if (LocalOutputAdapter.Instances.TryGetValue(instanceName, out adapterInstance))
                    return adapterInstance;
            }

            return null;
        }

        #endregion
    }
}
