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
using GrafanaAdapters.DataSourceValueTypes;
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
using GSF.Web.Security;
using openHistorian.Snap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Http;
using DataQualityMonitoring;
using AlarmState = GrafanaAdapters.Model.Database.AlarmState;
using CancellationToken = System.Threading.CancellationToken;
using Timer = System.Timers.Timer;

// ReSharper disable CompareOfFloatsByEqualityOperator
namespace openHistorian.Adapters;

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
    internal sealed class OH2DataSource : GrafanaDataSourceBase
    {
        private readonly ulong m_baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;

        public OH2DataSource()
        {
            MaximumSearchTargetsPerRequest = s_maximumSearchTargetsPerRequest;
            MaximumAnnotationsPerRequest = s_maximumAnnotationsPerRequest;
        }

        /// <summary>
        /// Starts a query that will read data source values, given a set of point IDs and targets, over a time range.
        /// </summary>
        /// <param name="queryParameters">Parameters that define the query.</param>
        /// <param name="targetMap">Set of IDs with associated targets to query.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        /// <returns>Queried data source data in terms of value and time.</returns>
        protected override async IAsyncEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, OrderedDictionary<ulong, (string, string)> targetMap, [EnumeratorCancellation] CancellationToken cancellationToken)
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
                (string, string) id = targetMap[pointID];

                if (includePeaks)
                {
                    if (peak.MinTimestamp > 0UL)
                    {
                        yield return new DataSourceValue
                        {
                            ID = id,
                            Value = peak.Min,
                            Time = (peak.MinTimestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                            Flags = (MeasurementStateFlags)peak.MinFlags
                        };
                    }

                    if (peak.MaxTimestamp != peak.MinTimestamp)
                    {
                        yield return new DataSourceValue
                        {
                            ID = id,
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
                        ID = id,
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
        private static readonly long s_baseTicks = UnixTimeTag.BaseTicks.Value;

        public OH1DataSource(string instanceName)
        {
            m_archiveReader = new ArchiveReader();
            m_archiveReader.DataReadException += (_, args) => Logger.SwallowException(args.Argument);
            m_archiveReader.Open(GetArchiveFileName(instanceName));

            InstanceName = instanceName;

            MaximumSearchTargetsPerRequest = s_maximumSearchTargetsPerRequest;
            MaximumAnnotationsPerRequest = s_maximumAnnotationsPerRequest;
        }

        protected override async IAsyncEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, OrderedDictionary<ulong, (string, string)> targetMap, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            DateTime startTime = queryParameters.StartTime;
            DateTime stopTime = queryParameters.StopTime;
            Dictionary<ulong, (Ticks, int)> lastAlarmValues = [];

            // Check if any of the points being trended are alarm measurements
            foreach (ulong pointID in targetMap.Keys.Where(IsAlarmMeasurement))
            {
                // Query first alarm state in reverse time order to get last state before query range, note that
                // when start time is greater than end time, API assumes query is to be processed in reverse
                IDataPoint dataPoint = m_archiveReader.ReadData((int)pointID, startTime.AddMilliseconds(-1.0D), startTime.AddDays(-s_reverseAlarmSearchLimit), false).FirstOrDefault();

                // If no data point is found in search limit, skip to next alarm point
                if (dataPoint is null)
                    continue;

                // Report any prior alarm value at the start of query
                yield return new DataSourceValue
                {
                    ID = targetMap[pointID],
                    Value = dataPoint.Value,
                    Time = (startTime.Ticks - s_baseTicks) / (double)Ticks.PerMillisecond,
                    Flags = MeasurementStateFlags.Normal
                };

                lastAlarmValues[pointID] = (startTime.Ticks, dataPoint.Value > 0.0D ? 1 : 0);
            }

            // Query historian for data points over the specified time range
            await foreach (IDataPoint dataPoint in m_archiveReader.ReadData(targetMap.Keys.Select(pointID => (int)pointID), startTime, stopTime, false).ToAsyncEnumerable().WithCancellation(cancellationToken))
            {
                ulong pointID = (ulong)dataPoint.HistorianID;
                long pointTime = dataPoint.Time.ToDateTime().Ticks;

                yield return new DataSourceValue
                {
                    ID = targetMap[pointID],
                    Value = dataPoint.Value,
                    Time = (pointTime - s_baseTicks) / (double)Ticks.PerMillisecond,
                    Flags = dataPoint.Quality.MeasurementQuality()
                };

                if (!IsAlarmMeasurement(pointID))
                    continue;

                if (!lastAlarmValues.TryGetValue(pointID, out (Ticks time, int) last) || pointTime > last.time)
                    lastAlarmValues[pointID] = (pointTime, dataPoint.Value > 0.0D ? 1 : 0);

                // If data point time matches one in alarm measurement buffer, remove any matching point ID that was already recorded in historian.
                // Since matching values found in the archive will already have been trended, they are no longer needed in the memory buffer.
                if (s_alarmMeasurementBuffer.TryGetValue(pointTime, out ConcurrentDictionary<ulong, int> measurementBuffer))
                    measurementBuffer.TryRemove(pointID, out _);
            }

            if (s_alarmMeasurementBuffer.Count == 0)
                yield break;

            // Report any alarm change states that occurred during the query range but are not yet available in the historian.
            // This real-time operation helps ensure any alarm measurements that were published in the query range get trended
            // even when the historian has not yet finished recording them.
            foreach (KeyValuePair<Ticks, ConcurrentDictionary<ulong, int>> timeBufferPair in s_alarmMeasurementBuffer)
            {
                long alarmTime = timeBufferPair.Key;
                ConcurrentDictionary<ulong, int> measurementBuffer = timeBufferPair.Value;

                // If no more alarm measurements exist for this time, remove it from the buffer
                if (measurementBuffer.Count == 0)
                {
                    // This is safe, enumeration operates over a snapshot of the collection
                    s_alarmMeasurementBuffer.TryRemove(alarmTime, out _);
                    continue;
                }

                // Ignore any alarm measurements that are outside the query time range
                if (alarmTime < startTime.Ticks || alarmTime > stopTime.Ticks)
                    continue;

                foreach (KeyValuePair<ulong, int> idValuePair in measurementBuffer)
                {
                    ulong pointID = idValuePair.Key;
                    int value = idValuePair.Value;

                    // Ignore any alarm measurements that are not in the query target map
                    if (!targetMap.TryGetValue(pointID, out (string, string) id))
                        continue;
                    
                    yield return new DataSourceValue
                    {
                        ID = id,
                        Value = value,
                        Time = (alarmTime - s_baseTicks) / (double)Ticks.PerMillisecond,
                        Flags = MeasurementStateFlags.Normal
                    };

                    if (!lastAlarmValues.TryGetValue(pointID, out (Ticks time, int) last) || alarmTime > last.time)
                        lastAlarmValues[pointID] = (alarmTime, value);
                }
            }

            // Report any ongoing alarm values at the end of the query range
            foreach (KeyValuePair<ulong, (Ticks, int)> idTimeValuePair in lastAlarmValues)
            {
                ulong pointID = idTimeValuePair.Key;
                (Ticks lastTime, int lastValue) = idTimeValuePair.Value;

                // Ignore last point if trended time is already at the end of the query range
                if (lastTime >= stopTime.Ticks)
                    continue;
                
                if (!targetMap.TryGetValue(pointID, out (string, string) id))
                    continue;
                
                yield return new DataSourceValue
                {
                    ID = id,
                    Value = lastValue,
                    Time = (stopTime.Ticks - s_baseTicks) / (double)Ticks.PerMillisecond,
                    Flags = MeasurementStateFlags.Normal
                };
            }
        }

        public void Dispose()
        {
            m_archiveReader?.Dispose();
        }

        private static readonly ConcurrentDictionary<string, string> s_archiveFileNames = new();

        private static string GetArchiveFileName(string instanceName)
        {
            instanceName = instanceName.ToLowerInvariant();

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

    // Fields
    private GrafanaDataSourceBase m_dataSource;

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets the default API path string for this controller.
    /// </summary>
    protected virtual string DefaultAPIPath
    {
        get
        {
            if (!string.IsNullOrEmpty(s_defaultAPIPath))
                return s_defaultAPIPath;

            string controllerName = GetType().Name.ToLowerInvariant();

            if (controllerName.EndsWith("controller") && controllerName.Length > 10)
                controllerName = controllerName.Substring(0, controllerName.Length - 10);

            s_defaultAPIPath = $"/api/{controllerName}";

            return s_defaultAPIPath;
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
    /// Gets the data source value types, i.e., any type that has implemented <see cref="IDataSourceValueType"/>,
    /// that have been loaded into the application domain.
    /// </summary>
    [HttpPost]
    public virtual IEnumerable<DataSourceValueType> GetValueTypes()
    {
        return DataSource?.GetValueTypes() ?? Enumerable.Empty<DataSourceValueType>();
    }

    /// <summary>
    /// Gets the table names that, at a minimum, contain all the fields that the value type has defined as
    /// required, see <see cref="IDataSourceValueType.RequiredMetadataFieldNames"/>.
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
    [AuthorizeControllerRole("Administrator")]
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
    [AuthorizeControllerRole("Administrator")]
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

    // Static Fields
    private static readonly Regex s_intervalExpression = new(@"(?<Value>\d+\.?\d*)(?<Unit>\w+)", RegexOptions.Compiled);
    private static readonly int s_maximumSearchTargetsPerRequest;
    private static readonly int s_maximumAnnotationsPerRequest;
    private static readonly double s_reverseAlarmSearchLimit;
    private static readonly double s_alarmMeasurementBufferSize;
    private static readonly ConcurrentDictionary<Ticks, ConcurrentDictionary<ulong, int>> s_alarmMeasurementBuffer;
    private static readonly Timer s_alarmBufferCurtailmentTimer;
    private static HashSet<ulong> s_alarmMeasurements;

    // Static Constructor
    static GrafanaController()
    {
        const int DefaultMaximumSearchTargetsPerRequest = 200;
        const int DefaultMaximumAnnotationsPerRequest = 100;
        const double DefaultReverseAlarmSearchLimit = 1.0D;
        const double DefaultAlarmMeasurementBufferSize = 3.0D;

        try
        {
            // Make sure Grafana specific default threshold settings exist
            CategorizedSettingsElementCollection thresholdSettings = ConfigurationFile.Current.Settings["thresholdSettings"];

            // Make sure needed settings exist
            thresholdSettings.Add("GrafanaMaximumSearchTargets", DefaultMaximumSearchTargetsPerRequest, "Defines maximum number of search targets to return during a Grafana search query.");
            thresholdSettings.Add("GrafanaMaximumAnnotations", DefaultMaximumAnnotationsPerRequest, "Defines maximum number of annotations to return during a Grafana annotation query.");
            thresholdSettings.Add("ReverseAlarmSearchLimit", DefaultReverseAlarmSearchLimit, "Defines the maximum time, in floating-point days, to execute a reverse order query to find last alarm change state.");
            thresholdSettings.Add("AlarmMeasurementBufferSize", DefaultAlarmMeasurementBufferSize, "Defines the maximum time, in floating-point seconds, to buffer latest alarm measurements relative to local clock.");

            // Get settings as currently defined in configuration file
            s_maximumSearchTargetsPerRequest = thresholdSettings["GrafanaMaximumSearchTargets"].ValueAs(DefaultMaximumSearchTargetsPerRequest);
            s_maximumAnnotationsPerRequest = thresholdSettings["GrafanaMaximumAnnotations"].ValueAs(DefaultMaximumAnnotationsPerRequest);
            s_reverseAlarmSearchLimit = thresholdSettings["ReverseAlarmSearchLimit"].ValueAs(DefaultReverseAlarmSearchLimit);
            s_alarmMeasurementBufferSize = thresholdSettings["AlarmMeasurementBufferSize"].ValueAs(DefaultAlarmMeasurementBufferSize);
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex);

            s_maximumSearchTargetsPerRequest = DefaultMaximumSearchTargetsPerRequest;
            s_maximumAnnotationsPerRequest = DefaultMaximumAnnotationsPerRequest;
        }

        s_alarmMeasurementBuffer = [];
        
        s_alarmBufferCurtailmentTimer = new Timer(1000.0D)
        {
            AutoReset = true,
            Enabled = false
        };

        s_alarmBufferCurtailmentTimer.Elapsed += AlarmBufferCurtailmentTimer_Elapsed;
    }

    // Static Methods

    private static bool IsAlarmMeasurement(ulong pointID)
    {
        // If alarm adapter is not available yet, then no point can be determined to be an alarm measurement
        if (AlarmAdapter.Default is null)
            return false;

        // If alarm measurement map has already been initialized, check if pointID is in the set
        if (s_alarmMeasurements is not null)
            return s_alarmMeasurements.Contains(pointID);

        // Initialize alarm measurement map - multiple threads may try to initialize at the same time,
        // if alarm measurements is not null here, another thread has already initialized it, so just
        // check if pointID is in the set
        if (Interlocked.CompareExchange(ref s_alarmMeasurements, GetAlarmMeasurements(), null) is not null)
            return s_alarmMeasurements.Contains(pointID);
        
        // Attach to alarm engine inputs updated event to automatically handle alarm configuration changes
        AlarmAdapter.Default.InputMeasurementKeysUpdated += (_, _) =>
        {
            Interlocked.Exchange(ref s_alarmMeasurements, GetAlarmMeasurements());
        };

        // Attach to alarm engine new measurements event to buffer recent alarm measurements
        AlarmAdapter.Default.NewMeasurements += (_, args) =>
        {
            ICollection<IMeasurement> measurements = args.Argument;

            if (measurements is null)
                return;

            foreach (IMeasurement measurement in measurements)
            {
                if (!s_alarmMeasurements.Contains(measurement.Key.ID))
                    continue;

                // Add alarm measurement to buffer
                ConcurrentDictionary<ulong, int> measurementBuffer = s_alarmMeasurementBuffer.GetOrAdd(measurement.Timestamp, _ => new ConcurrentDictionary<ulong, int>());
                measurementBuffer[measurement.Key.ID] = measurement.AdjustedValue > 0.0D ? 1 : 0;
            }
        };

        return s_alarmMeasurements.Contains(pointID);
    }

    private static HashSet<ulong> GetAlarmMeasurements()
    {
        MeasurementKey[] alarmInputMeasurements = AlarmAdapter.Default.InputMeasurementKeys;

        if (alarmInputMeasurements is null || alarmInputMeasurements.Length == 0)
            return [];

        HashSet<ulong> alarmMeasurements = [];

        foreach (ICollection<Alarm> alarms in alarmInputMeasurements.Select(key => AlarmAdapter.Default.GetAlarmStatus(key.SignalID)))
        {
            if (alarms is not { Count: > 0 })
                continue;

            foreach (Alarm alarm in alarms)
            {
                if (alarm.AssociatedMeasurementID is null)
                    continue;

                MeasurementKey alarmOutputMeasurement = MeasurementKey.LookUpBySignalID(alarm.AssociatedMeasurementID.Value);

                if (alarmOutputMeasurement != MeasurementKey.Undefined)
                    alarmMeasurements.Add(alarmOutputMeasurement.ID);
            }
        }

        return alarmMeasurements;
    }

    private static void AlarmBufferCurtailmentTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        // Get buffer window expiration time based on configured size relative to local clock
        long bufferExpirationTime = DateTime.UtcNow.AddSeconds(-s_alarmMeasurementBufferSize).Ticks;

        // Remove any alarm measurements older than the defined buffer window size
        foreach (KeyValuePair<Ticks, ConcurrentDictionary<ulong, int>> kvp in s_alarmMeasurementBuffer)
        {
            if (kvp.Key < bufferExpirationTime)
                s_alarmMeasurementBuffer.TryRemove(kvp.Key, out _);
        }
    }

    private static bool TryParseInterval(string interval, out TimeSpan timeSpan)
    {
        if (string.IsNullOrWhiteSpace(interval))
        {
            timeSpan = TimeSpan.Zero;
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

        timeSpan = TimeSpan.Zero;
        return false;
    }

    private static LocalOutputAdapter GetAdapterInstance(string instanceName)
    {
        if (string.IsNullOrWhiteSpace(instanceName))
            return null;

        return LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter adapterInstance) ? adapterInstance : null;
    }

    private static string s_defaultAPIPath;

    #endregion
}