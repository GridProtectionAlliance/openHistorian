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
using GrafanaAdapters.DataSources;
using GrafanaAdapters.DataSources.BuiltIn;
using GrafanaAdapters.Functions;
using GrafanaAdapters.Model.Annotations;
using GrafanaAdapters.Model.Common;
using GrafanaAdapters.Model.Database;
using GrafanaAdapters.Model.Functions;
using GrafanaAdapters.Model.Metadata;
using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Diagnostics;
using GSF.Historian;
using GSF.Historian.Files;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.TimeSeries;
using openHistorian.Snap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using AlarmState = GrafanaAdapters.Model.Database.AlarmState;
using CancellationToken = System.Threading.CancellationToken;

// ReSharper disable CompareOfFloatsByEqualityOperator
namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana data source.
    /// </summary>
    public class GrafanaController : ApiController
    {
        #region [ Members ]

        // Nested Types
        private class Peak
        {
            public float Min;
            public float Max;

            public ulong MinTimestamp;
            public ulong MaxTimestamp;

            public ulong MinFlags;
            public ulong MaxFlags;

            public void Set(float value, ulong timestamp, ulong flags)
            {
                if (value < Min)
                {
                    Min = value;
                    MinTimestamp = timestamp;
                    MinFlags = flags;
                }

                if (value > Max)
                {
                    Max = value;
                    MaxTimestamp = timestamp;
                    MaxFlags = flags;
                }
            }

            public void Reset()
            {
                Min = float.MaxValue;
                Max = float.MinValue;
                MinTimestamp = MaxTimestamp = 0UL;
                MinFlags = MaxFlags = 0UL;
            }

            public static readonly Peak Default = new();
        }

        // Represents a historian 2.0 data source for the Grafana adapter.
        internal class OH2DataSource : GrafanaDataSourceBase
        {
            private readonly ulong m_baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;

            /// <summary>
            /// Starts a query that will read data source values, given a set of point IDs and targets, over a time range.
            /// </summary>
            /// <param name="queryParameters">Parameters that define the query.</param>
            /// <param name="targetMap">Set of IDs with associated targets to query.</param>
            /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
            /// <returns>Queried data source data in terms of value and time.</returns>
            //protected override IEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, Dictionary<ulong, string> targetMap, CancellationToken cancellationToken)
<<<<<<< HEAD
            protected override async IAsyncEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, Dictionary<ulong, string> targetMap, [EnumeratorCancellation] CancellationToken cancellationToken)
=======
            protected override async IAsyncEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, Dictionary<ulong, string> targetMap, CancellationToken cancellationToken)
>>>>>>> 26c5c82f8b (Updates to accommodate grafana async implementation)
            {
                SnapServer server = GetAdapterInstance(InstanceName)?.Server?.Host;

                if (server is null)
                    yield break;

                using SnapClient connection = SnapClient.Connect(server);
                using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(InstanceName);

                if (database is null)
                    yield break;

                DateTime startTime = queryParameters.StartTime;
                DateTime stopTime = queryParameters.StopTime;
                string interval = queryParameters.Interval;
                bool includePeaks = queryParameters.IncludePeaks;

                if (!TryParseInterval(interval, out TimeSpan resolutionInterval))
                {
                    Resolution resolution = TrendValueAPI.EstimatePlotResolution(InstanceName, startTime, stopTime, targetMap.Keys);
                    resolutionInterval = resolution.GetInterval();
                }

                BaselineTimeInterval timeInterval = resolutionInterval.Ticks switch
                {
                    < Ticks.PerMinute => BaselineTimeInterval.Second,
                    < Ticks.PerHour => BaselineTimeInterval.Minute,
                    Ticks.PerHour => BaselineTimeInterval.Hour,
                    _ => BaselineTimeInterval.Second
                };

                startTime = startTime.BaselinedTimestamp(timeInterval);
                stopTime = stopTime.BaselinedTimestamp(timeInterval);

                if (startTime == stopTime)
                    stopTime = stopTime.AddSeconds(1.0D);

                SeekFilterBase<HistorianKey> timeFilter;

                // Set timestamp filter resolution
                if (includePeaks || resolutionInterval == TimeSpan.Zero)
                {
                    // Full resolution query
                    timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);
                }
                else
                {
                    // Interval query
                    timeFilter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, resolutionInterval, new TimeSpan(TimeSpan.TicksPerMillisecond));
                }

                // Setup point ID selections
                MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(targetMap.Keys);
                Dictionary<ulong, ulong> lastTimes = new(targetMap.Count);
                Dictionary<ulong, Peak> peaks = new(targetMap.Count);
                ulong resolutionSpan = (ulong)resolutionInterval.Ticks;

                if (includePeaks)
                    resolutionSpan *= 2UL;

                // Start stream reader for the provided time window and selected points
                using TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);
                HistorianKey key = new();
                HistorianValue value = new();
                Peak peak = Peak.Default;

                while (await stream.ReadAsync(key, value))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ulong pointID = key.PointID;
                    ulong timestamp = key.Timestamp;
                    float pointValue = value.AsSingle;

                    if (includePeaks)
                    {
                        peak = peaks.GetOrAdd(pointID, _ => new Peak());
                        peak.Set(pointValue, timestamp, value.Value3);
                    }

                    if (resolutionSpan > 0UL && timestamp - lastTimes.GetOrAdd(pointID, 0UL) < resolutionSpan)
                        continue;

                    // New value is ready for publication
                    string target = targetMap[pointID];

                    if (includePeaks)
                    {
                        if (peak.MinTimestamp > 0UL)
                        {
                            yield return new DataSourceValue
                            {
                                Target = target,
                                Value = peak.Min,
                                Time = (peak.MinTimestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                                Flags = (MeasurementStateFlags)peak.MinFlags
                            };
                        }

                        if (peak.MaxTimestamp != peak.MinTimestamp)
                        {
                            yield return new DataSourceValue
                            {
                                Target = target,
                                Value = peak.Max,
                                Time = (peak.MaxTimestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                                Flags = (MeasurementStateFlags)peak.MaxFlags
                            };
                        }

                        peak.Reset();
                    }
                    else
                    {
                        yield return new DataSourceValue
                        {
                            Target = target,
                            Value = pointValue,
                            Time = (timestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                            Flags = (MeasurementStateFlags)value.Value3
                        };
                    }

                    lastTimes[pointID] = timestamp;
                }
            }

            public override Task<List<AnnotationResponse>> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new List<AnnotationResponse>());
            }
        }

        // Represents a historian 1.0 data source for the Grafana adapter.
        internal sealed class OH1DataSource : GrafanaDataSourceBase, IDisposable
        {
            private readonly ArchiveReader m_archiveReader;
            private readonly long m_baseTicks;

            public OH1DataSource(string instanceName)
            {
                m_archiveReader = new ArchiveReader();
                m_archiveReader.DataReadException += (_, args) => Logger.SwallowException(args.Argument);
                m_archiveReader.Open(GetArchiveFileName(instanceName));

                m_baseTicks = UnixTimeTag.BaseTicks.Value;

                InstanceName = instanceName;
            }

            //protected override IEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, Dictionary<ulong, string> targetMap, CancellationToken cancellationToken)
<<<<<<< HEAD
            protected override async IAsyncEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, Dictionary<ulong, string> targetMap, [EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await foreach (IDataPoint dataPoint in m_archiveReader.ReadData(targetMap.Keys.Select(pointID => (int)pointID), queryParameters.StartTime, queryParameters.StopTime, false).ToAsyncEnumerable().WithCancellation(cancellationToken))
=======
            protected override IAsyncEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, Dictionary<ulong, string> targetMap, CancellationToken cancellationToken)
            {
                return m_archiveReader.ReadData(targetMap.Keys.Select(pointID => (int)pointID), queryParameters.StartTime, queryParameters.StopTime, false).ToAsyncEnumerable().Select(dataPoint => new DataSourceValue
>>>>>>> 26c5c82f8b (Updates to accommodate grafana async implementation)
                {
                    yield return new DataSourceValue
                    {
                        Target = targetMap[(ulong)dataPoint.HistorianID],
                        Value = dataPoint.Value,
                        Time = (dataPoint.Time.ToDateTime().Ticks - m_baseTicks) / (double)Ticks.PerMillisecond,
                        Flags = dataPoint.Quality.MeasurementQuality()
                    };
                }
            }

            public void Dispose()
            {
                m_archiveReader?.Dispose();
            }

            private static readonly Dictionary<string, string> s_archiveFileNames = new();

            private static string GetArchiveFileName(string instanceName)
            {
                instanceName = instanceName.ToLowerInvariant();

                lock (s_archiveFileNames)
                {
                    if (s_archiveFileNames.TryGetValue(instanceName, out string archiveFileName))
                        return archiveFileName;

                    CategorizedSettingsElementCollection settings = ConfigurationFile.Current.Settings[$"{instanceName}ArchiveFile"];
                    archiveFileName = settings?["FileName"]?.Value;

                    if (string.IsNullOrWhiteSpace(archiveFileName))
                        archiveFileName = instanceName.Equals("stat") ? @"Statistics\stat_archive.d" : $@"{instanceName}\{instanceName}_archive.d";

                    s_archiveFileNames[instanceName] = archiveFileName;

                    return archiveFileName;
                }
            }
        }

        // Fields
        private GrafanaDataSourceBase m_dataSource;
        private string m_defaultAPIPath;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the default API path string for this controller.
        /// </summary>
        protected virtual string DefaultAPIPath
        {
            get
            {
                if (!string.IsNullOrEmpty(m_defaultAPIPath))
                    return m_defaultAPIPath;

                string controllerName = GetType().Name.ToLowerInvariant();

                if (controllerName.EndsWith("controller") && controllerName.Length > 10)
                    controllerName = controllerName.Substring(0, controllerName.Length - 10);

                m_defaultAPIPath = $"/api/{controllerName}";

                return m_defaultAPIPath;
            }
        }

        /// <summary>
        /// Gets historian data source for this Grafana adapter.
        /// </summary>
        protected GrafanaDataSourceBase DataSource
        {
            get
            {
                if (m_dataSource is not null)
                    return m_dataSource;

                string uriPath = Request.RequestUri.PathAndQuery;
                string instanceName;

                if (uriPath.StartsWith(DefaultAPIPath, StringComparison.OrdinalIgnoreCase))
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

                Debug.Assert(!string.IsNullOrWhiteSpace(instanceName));

                //                                                   012345
                // Support optional version in instance name (e.g., "1.0-STAT")
                const string versionPattern = @"^(\d+\.\d+)-";

                Match match = Regex.Match(instanceName, versionPattern);
                int version = 0;

                if (match.Success)
                {
                    string decimalValue = match.Groups[1].Value;
                    instanceName = instanceName.Substring(decimalValue.Length + 1);
                    version = int.Parse(decimalValue.Split('.')[0]);
                }

                if (version is < 1 or > 2)
                    version = 2;

                if (version == 1)
                {
                    m_dataSource = new OH1DataSource(instanceName)
                    {
                        Metadata = GetAdapterInstance(TrendValueAPI.DefaultInstanceName)?.DataSource
                    };
                }
                else
                {
                    LocalOutputAdapter adapterInstance = GetAdapterInstance(instanceName);

                    if (adapterInstance is not null)
                    {
                        m_dataSource = new OH2DataSource
                        {
                            InstanceName = instanceName,
                            Metadata = adapterInstance.DataSource
                        };
                    }
                }

                return m_dataSource;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            if (m_dataSource is IDisposable disposable)
                disposable.Dispose();
        }

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
        public virtual Task<IEnumerable<TimeSeriesValues>> Query(QueryRequest request, CancellationToken cancellationToken)
        {
            if (request.targets.FirstOrDefault()?.target is null)
                return Task.FromResult(Enumerable.Empty<TimeSeriesValues>());

            return DataSource?.Query(request, cancellationToken) ?? Task.FromResult(Enumerable.Empty<TimeSeriesValues>());
        }

        /// <summary>
        /// Gets the data source value types, i.e., any type that has implemented <see cref="IDataSourceValue"/>,
        /// that have been loaded into the application domain.
        /// </summary>
        [HttpPost]
        public virtual IEnumerable<DataSourceValueType> GetValueTypes()
        {
            return DataSource?.GetValueTypes() ?? Enumerable.Empty<DataSourceValueType>();
        }

        /// <summary>
        /// Gets the table names that, at a minimum, contain all the fields that the value type has defined as
        /// required, see <see cref="IDataSourceValue.RequiredMetadataFieldNames"/>.
        /// </summary>
        /// <param name="request">Search request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        public virtual Task<IEnumerable<string>> GetValueTypeTables(SearchRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.GetValueTypeTables(request, cancellationToken) ?? Task.FromResult(Enumerable.Empty<string>());
        }

        /// <summary>
        /// Gets the field names for a given table.
        /// </summary>
        /// <param name="request">Search request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        public virtual Task<IEnumerable<FieldDescription>> GetValueTypeTableFields(SearchRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.GetValueTypeTableFields(request, cancellationToken) ?? Task.FromResult(Enumerable.Empty<FieldDescription>());
        }

        /// <summary>
        /// Gets the functions that are available for a given data source value type.
        /// </summary>
        /// <param name="request">Search request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>
        /// <see cref="SearchRequest.expression"/> is used to filter functions by group operation, specifically a
        /// value of "None", "Slice", or "Set" as defined in the <see cref="GroupOperations"/> enumeration. If all
        /// function descriptions are desired, regardless of group operation, an empty string can be provided.
        /// Combinations are also supported, e.g., "Slice,Set".
        /// </remarks>
        [HttpPost]
        public virtual Task<IEnumerable<FunctionDescription>> GetValueTypeFunctions(SearchRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.GetValueTypeFunctions(request, cancellationToken) ?? Task.FromResult(Enumerable.Empty<FunctionDescription>());
        }

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<string[]> Search(SearchRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.Search(request, cancellationToken) ?? Task.FromResult(Array.Empty<string>());
        }

        /// <summary>
        /// Reloads data source value types cache.
        /// </summary>
        /// <remarks>
        /// This function is used to support dynamic data source value type loading. Function only needs to be called
        /// when a new data source value is added to Grafana at run-time and end-user wants to use newly installed
        /// data source value type without restarting host.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public virtual void ReloadValueTypes()
        {
            DataSource?.ReloadDataSourceValueTypes();
        }

        /// <summary>
        /// Reloads Grafana functions cache.
        /// </summary>
        /// <remarks>
        /// This function is used to support dynamic loading for Grafana functions. Function only needs to be called
        /// when a new function is added to Grafana at run-time and end-user wants to use newly installed function
        /// without restarting host.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public virtual void ReloadGrafanaFunctions()
        {
            DataSource?.ReloadGrafanaFunctions();
        }

        /// <summary>
        /// Queries openHistorian for alarm state.
        /// </summary>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<IEnumerable<AlarmDeviceStateView>> GetAlarmState(CancellationToken cancellationToken)
        {
            return DataSource?.GetAlarmState(cancellationToken) ?? Task.FromResult(Enumerable.Empty<AlarmDeviceStateView>());
        }

        /// <summary>
        /// Queries openHistorian for device alarms.
        /// </summary>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<IEnumerable<AlarmState>> GetDeviceAlarms(CancellationToken cancellationToken)
        {
            return DataSource?.GetDeviceAlarms(cancellationToken) ?? Task.FromResult(Enumerable.Empty<AlarmState>());
        }

        /// <summary>
        /// Queries openHistorian for device groups.
        /// </summary>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<IEnumerable<DeviceGroup>> GetDeviceGroups(CancellationToken cancellationToken)
        {
            return DataSource?.GetDeviceGroups(cancellationToken) ?? Task.FromResult(Enumerable.Empty<DeviceGroup>());
        }

        /// <summary>
        /// Queries openHistorian for annotations in a time-range (e.g., Alarms).
        /// </summary>
        /// <param name="request">Annotation request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<List<AnnotationResponse>> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
        {
            return DataSource?.Annotations(request, cancellationToken) ?? Task.FromResult(new List<AnnotationResponse>());
        }

        #endregion

        #region [ Static ]

        private static readonly Regex s_intervalExpression = new(@"(?<Value>\d+\.?\d*)(?<Unit>\w+)", RegexOptions.Compiled);

        // Static Methods
        private static bool TryParseInterval(string interval, out TimeSpan timeSpan)
        {
            if (string.IsNullOrWhiteSpace(interval))
            {
                timeSpan = default;
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

            timeSpan = default;
            return false;
        }

        private static LocalOutputAdapter GetAdapterInstance(string instanceName)
        {
            if (string.IsNullOrWhiteSpace(instanceName))
                return null;

            return LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter adapterInstance) ? adapterInstance : null;
        }

        #endregion
    }
}
