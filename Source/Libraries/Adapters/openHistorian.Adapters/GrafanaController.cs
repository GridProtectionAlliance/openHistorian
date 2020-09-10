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

using GrafanaAdapters;
using GSF;
using GSF.Collections;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.TimeSeries;
using Newtonsoft.Json;
using openHistorian.Snap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
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

        /// <summary>
        /// Represents a historian data source for the Grafana adapter.
        /// </summary>
        protected class HistorianDataSource : GrafanaDataSourceBase
        {
            private readonly ulong m_baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;

            /// <summary>
            /// Starts a query that will read data source values, given a set of point IDs and targets, over a time range.
            /// </summary>
            /// <param name="startTime">Start-time for query.</param>
            /// <param name="stopTime">Stop-time for query.</param>
            /// <param name="interval">Interval from Grafana request.</param>
            /// <param name="decimate">Flag that determines if data should be decimated over provided time range.</param>
            /// <param name="targetMap">Set of IDs with associated targets to query.</param>
            /// <returns>Queried data source data in terms of value and time.</returns>
            protected override IEnumerable<DataSourceValue> QueryDataSourceValues(DateTime startTime, DateTime stopTime, string interval, bool decimate, Dictionary<ulong, string> targetMap)
            {
                SnapServer server = GetAdapterInstance(InstanceName)?.Server?.Host;

                if (server == null)
                    yield break;

                using (SnapClient connection = SnapClient.Connect(server))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(InstanceName))
                {
                    if (database == null)
                        yield break;

                    TimeSpan resolutionInterval;

                    if (decimate)
                    {
                        if (!TryParseInterval(interval, out resolutionInterval))
                        {
                            Resolution resolution = TrendValueAPI.EstimatePlotResolution(InstanceName, startTime, stopTime, targetMap.Keys);
                            resolutionInterval = resolution.GetInterval();
                        }
                    }
                    else
                    {
                        resolutionInterval = TimeSpan.Zero;
                    }

                    bool subFullResolution = false;

                    // Set data scan resolution
                    if (decimate && resolutionInterval != TimeSpan.Zero)
                    {
                        subFullResolution = true;

                        BaselineTimeInterval timeInterval = BaselineTimeInterval.Second;

                        if (resolutionInterval.Ticks < Ticks.PerMinute)
                            timeInterval = BaselineTimeInterval.Second;
                        else if (resolutionInterval.Ticks < Ticks.PerHour)
                            timeInterval = BaselineTimeInterval.Minute;
                        else if (resolutionInterval.Ticks == Ticks.PerHour)
                            timeInterval = BaselineTimeInterval.Hour;

                        startTime = startTime.BaselinedTimestamp(timeInterval);
                        stopTime = stopTime.BaselinedTimestamp(timeInterval);
                    }

                    SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);

                    // Setup point ID selections
                    MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(targetMap.Keys);
                    Dictionary<ulong, ulong> lastTimes = new Dictionary<ulong, ulong>(targetMap.Count);
                    Dictionary<ulong, Tuple<float, float>> extremes = new Dictionary<ulong, Tuple<float, float>>(targetMap.Count);
                    ulong pointID, timestamp, resolutionSpan = (ulong)resolutionInterval.Ticks;
                    float pointValue, min = 0.0F, max = 0.0F;

                    // Start stream reader for the provided time window and selected points
                    using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter))
                    {
                        HistorianKey key = new HistorianKey();
                        HistorianValue value = new HistorianValue();

                        while (stream.Read(key, value))
                        {
                            pointID = key.PointID;
                            timestamp = key.Timestamp;
                            pointValue = value.AsSingle;

                            if (subFullResolution)
                            {
                                Tuple<float, float> stats = extremes.GetOrAdd(pointID, _ => new Tuple<float, float>(float.MaxValue, float.MinValue));

                                min = stats.Item1;
                                max = stats.Item2;

                                if (pointValue < min)
                                    min = pointValue;

                                if (pointValue > max)
                                    max = pointValue;

                                if (min != float.MaxValue && max != float.MinValue)
                                    pointValue = Math.Abs(max) > Math.Abs(min) ? max : min;
                                else if (min != float.MaxValue)
                                    pointValue = min;
                                else if (max != float.MinValue)
                                    pointValue = max;
                            }

                            if (timestamp - lastTimes.GetOrAdd(pointID, 0UL) > resolutionSpan)
                            {
                                yield return new DataSourceValue
                                {
                                    Target = targetMap[pointID],
                                    Time = (timestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                                    Value = pointValue,
                                    Flags = (MeasurementStateFlags)value.Value3
                                };
                                lastTimes[pointID] = timestamp;

                                // Reset extremes at each point publication
                                if (subFullResolution)
                                    extremes[pointID] = new Tuple<float, float>(float.MaxValue, float.MinValue);
                            }
                            else if (subFullResolution)
                            {
                                // Track extremes over interval
                                extremes[pointID] = new Tuple<float, float>(min, max);
                            }
                        }
                    }
                }
            }
        }

        // Fields
        private HistorianDataSource m_dataSource;
        private LocationData m_locationData;
        private string m_defaultApiPath;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the default API path string for this controller.
        /// </summary>
        protected virtual string DefaultApiPath
        {
            get
            {
                if (!string.IsNullOrEmpty(m_defaultApiPath))
                    return m_defaultApiPath;

                string controllerName = GetType().Name.ToLowerInvariant();

                if (controllerName.EndsWith("controller") && controllerName.Length > 10)
                    controllerName = controllerName.Substring(0, controllerName.Length - 10);

                m_defaultApiPath = $"/api/{controllerName}";

                return m_defaultApiPath;
            }
        }

        /// <summary>
        /// Gets historian data source for this Grafana adapter.
        /// </summary>
        protected HistorianDataSource DataSource
        {
            get
            {
                if (m_dataSource != null)
                    return m_dataSource;

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

                    if (adapterInstance != null)
                    {
                        m_dataSource = new HistorianDataSource
                        {
                            InstanceName = instanceName,
                            Metadata = adapterInstance.DataSource
                        };
                    }
                }

                return m_dataSource;
            }
        }

        private LocationData LocationData => m_locationData ?? (m_locationData = new LocationData { DataSource = DataSource });

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
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<List<TimeSeriesValues>> Query(QueryRequest request, CancellationToken cancellationToken)
        {
            if (request.targets.FirstOrDefault()?.target == null)
                return Task.FromResult(new List<TimeSeriesValues>());

            
            return DataSource?.Query(request, cancellationToken) ?? Task.FromResult(new List<TimeSeriesValues>());
        }


        /// <summary>
        /// Queries openHistorian for Device Alarm Status.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<IEnumerable<AlarmDeviceStateView>> GetAlarmState(QueryRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.GetAlarmState(request, cancellationToken) ?? Task.FromResult(new List<AlarmDeviceStateView>().AsEnumerable());
        }

        /// <summary>
        /// Queries openHistorian for Device Alarm States.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<IEnumerable<GrafanaAdapters.AlarmState>> GetDeviceAlarms(QueryRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.GetDeviceAlarms(request, cancellationToken) ?? Task.FromResult(new List<GrafanaAdapters.AlarmState>().AsEnumerable());
        }

        /// <summary>
        /// Queries openHistorian as a Grafana data source.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual void QueryAlarms(QueryRequest request, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                GrafanaAlarmManagement.UpdateAlerts(request, GetAlarms(request, cancellationToken).Result);
            }, cancellationToken);
            
        }

        /// <summary>
        /// Queries openHistorian for DeviceGroups.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<IEnumerable<DeviceGroup>> GetDeviceGroups(QueryRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.GetDeviceGroups(request, cancellationToken) ?? Task.FromResult(new List<DeviceGroup>().AsEnumerable());
        }

        /// <summary>
        /// Queries openHistorian as a Grafana Metadata source.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to meta-data access.")]
        public virtual Task<string> GetMetadata(Target request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => 
            {
                if (string.IsNullOrWhiteSpace(request.target))
                    return string.Empty;

                DataTable table = new DataTable();
                DataRow[] rows = DataSource?.Metadata.Tables["ActiveMeasurements"].Select($"PointTag = '{request.target}'") ?? new DataRow[0];

                if (rows.Length > 0)
                    table = rows.CopyToDataTable();

                return JsonConvert.SerializeObject(table);
            },
            cancellationToken);
        }

        /// <summary>
        /// Queries openHistorian location data for Grafana offsetting duplicate coordinates using a radial distribution.
        /// </summary>
        /// <param name="radius">Radius of overlapping coordinate distribution.</param>
        /// <param name="zoom">Zoom level.</param>
        /// <param name="request"> Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        /// <returns>JSON serialized location metadata for specified targets.</returns>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "CSRF exposure limited to meta-data access.")]
        public virtual Task<string> GetLocationData([FromUri] double radius, [FromUri] double zoom, [FromBody] List<Target> request, CancellationToken cancellationToken)
        {
            return LocationData.GetLocationData(radius, zoom, request, cancellationToken);
        }

        /// <summary>
        /// Queries openHistorian location data for Grafana.
        /// </summary>
        /// <param name="request"> Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        /// <returns>JSON serialized location metadata for specified targets.</returns>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "CSRF exposure limited to meta-data access.")]
        public virtual Task<string> GetLocationData(List<Target> request, CancellationToken cancellationToken)
        {
            return LocationData.GetLocationData(request, cancellationToken);
        }

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to meta-data access.")]
        public virtual Task<string[]> Search(Target request, CancellationToken cancellationToken)
        {
            return DataSource?.Search(request, cancellationToken) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Search openHistorian for a field.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to meta-data access.")]
        public virtual Task<string[]> SearchFields(Target request, CancellationToken cancellationToken)
        {
            return DataSource?.SearchFields(request, cancellationToken) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Search openHistorian for a table.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to meta-data access.")]
        public virtual Task<string[]> SearchFilters(Target request, CancellationToken cancellationToken)
        {
            return DataSource?.SearchFilters(request, cancellationToken) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Search openHistorian for a field.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to meta-data access.")]
        public virtual Task<string[]> SearchOrderBys(Target request, CancellationToken cancellationToken)
        {
            return DataSource?.SearchOrderBys(request, cancellationToken) ?? Task.FromResult(new string[0]);
        }

        /// <summary>
        /// Queries openHistorian for annotations in a time-range (e.g., Alarms).
        /// </summary>
        /// <param name="request">Annotation request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<List<AnnotationResponse>> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.Annotations(request, cancellationToken) ?? Task.FromResult(new List<AnnotationResponse>());
        }

        /// <summary>
        /// Queries openPDC Alarms as a Grafana alarm data source.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<List<GrafanaAlarm>> GetAlarms(QueryRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.GetAlarms(request, cancellationToken) ?? Task.FromResult(new List<GrafanaAlarm>());
        }

        /// <summary>
        /// Returns tag keys for ad hoc filters.
        /// </summary>
        /// <param name="request">Tag keys request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [ActionName("tag-keys")]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<TagKeysResponse[]> TagKeys(TagKeysRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.TagKeys(request, cancellationToken) ?? Task.FromResult(Array.Empty<TagKeysResponse>());
        }

        /// <summary>
        /// Returns tag values for ad hoc filters.
        /// </summary>
        /// <param name="request">Tag values request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [ActionName("tag-values")]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<TagValuesResponse[]> TagValues(TagValuesRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.TagValues(request, cancellationToken) ?? Task.FromResult(Array.Empty<TagValuesResponse>());
        }

        #endregion

        #region [ Static ]

        private static readonly Regex s_intervalExpression = new Regex(@"(?<Value>\d+\.?\d*)(?<Unit>\w+)", RegexOptions.Compiled);

        // Static Methods
        private static bool TryParseInterval(string interval, out TimeSpan timeSpan)
        {
            if (string.IsNullOrWhiteSpace(interval))
            {
                timeSpan = default(TimeSpan);
                return false;
            }

            Match match = s_intervalExpression.Match(interval);

            if (match.Success && double.TryParse(match.Result("${Value}"), out double value))
            {
                switch (match.Result("${Unit}").Trim().ToLowerInvariant())
                {
                    case "ms":
                        timeSpan = TimeSpan.FromMilliseconds(value);
                        return true;
                    case "s":
                        timeSpan = TimeSpan.FromSeconds(value);
                        return true;
                    case "m":
                        timeSpan = TimeSpan.FromMinutes(value);
                        return true;
                    case "h":
                        timeSpan = TimeSpan.FromHours(value);
                        return true;
                    case "d":
                        timeSpan = TimeSpan.FromDays(value);
                        return true;
                }
            }

            timeSpan = default(TimeSpan);
            return false;
        }

        private static LocalOutputAdapter GetAdapterInstance(string instanceName)
        {
            if (!string.IsNullOrWhiteSpace(instanceName))
            {
                if (LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter adapterInstance))
                    return adapterInstance;
            }

            return null;
        }

        #endregion
    }
}
