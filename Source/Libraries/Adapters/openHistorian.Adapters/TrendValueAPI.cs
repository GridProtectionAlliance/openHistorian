//******************************************************************************************************
//  TrendValueAPI.cs - Gbtc
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
//  09/13/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GSF;
using GSF.Collections;
using GSF.Data;
using GSF.Data.Model;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Threading;
using openHistorian.Model;
using openHistorian.Snap;
using CancellationToken = GSF.Threading.CancellationToken;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines an API for publishing <see cref="TrendValue"/> instances from the openHistorian.
    /// </summary>
    public static class TrendValueAPI
    {
        private static string s_defaultInstanceName;
        private static int s_portNumber;

        static TrendValueAPI()
        {
            LoadConnectionParameters();
        }

        /// <summary>
        /// Gets configured instance name for historian.
        /// </summary>
        public static string DefaultInstanceName => s_defaultInstanceName;

        /// <summary>
        /// Gets configured port number for historian.
        /// </summary>
        public static int PortNumber => s_portNumber;

        internal static void LoadConnectionParameters()
        {
            try
            {
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<IaonOutputAdapter> operations = new TableOperations<IaonOutputAdapter>(connection);
                    IaonOutputAdapter record = operations.QueryRecordWhere("TypeName = {0}", typeof(LocalOutputAdapter).FullName);

                    if (record is null)
                        throw new NullReferenceException("Primary openHistorian adapter instance not found.");

                    Dictionary<string, string> settings = record.ConnectionString.ParseKeyValuePairs();

                    if (!settings.TryGetValue("port", out string setting) || !int.TryParse(setting, out s_portNumber))
                        s_portNumber = Connection.DefaultHistorianPort;

                    if (!settings.TryGetValue("instanceName", out s_defaultInstanceName) || string.IsNullOrWhiteSpace(s_defaultInstanceName))
                        s_defaultInstanceName = record.AdapterName ?? "PPA";
                }
            }
            catch
            {
                s_defaultInstanceName = "PPA";
                s_portNumber = Connection.DefaultHistorianPort;
            }
        }

        /// <summary>
        /// Gets loaded historian adapter instance names.
        /// </summary>
        /// <returns>Historian adapter instance names.</returns>
        public static IEnumerable<string> GetInstanceNames() => LocalOutputAdapter.Instances.Keys;

        /// <summary>
        /// Estimates a decent plot resolution for given time range.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs being queried - or <c>null</c> for all available points.</param>
        /// <returns>Plot resolution for given time range.</returns>
        public static Resolution EstimatePlotResolution(string instanceName, DateTime startTime, DateTime stopTime, IEnumerable<ulong> measurementIDs)
        {
            Dictionary<ulong, DataRow> metadata = LocalOutputAdapter.Instances[instanceName].Measurements;
            DataRow row;

            long range = (stopTime - startTime).Ticks;

            if (range <= TimeSpan.TicksPerHour && !measurementIDs.Any(measurementID => (metadata.TryGetValue(measurementID, out row) ? int.Parse(row["FramesPerSecond"].ToString()) : 1) > 1))
                return Resolution.Full;

            if (range <= Ticks.PerMinute)
                return Resolution.Full;

            if (range <= Ticks.PerMinute * 5L)
                return Resolution.TenPerSecond;

            if (range <= Ticks.PerMinute * 30L)
                return Resolution.EverySecond;

            if (range <= Ticks.PerHour * 3L)
                return Resolution.Every10Seconds;

            if (range <= Ticks.PerHour * 8L)
                return Resolution.Every30Seconds;

            if (range <= Ticks.PerDay)
                return Resolution.EveryMinute;

            if (range <= Ticks.PerDay * 7L)
                return Resolution.Every10Minutes;

            if (range <= Ticks.PerDay * 21L)
                return Resolution.Every30Minutes;

            return Resolution.EveryHour;
        }

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="server">The server to use for the query.</param>
        /// <param name="instanceName">Name of the archive to be queried.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <param name="forceLimit">Flag that determines if series limit should be strictly enforced.</param>
        /// <param name="cancellationToken">Cancellation token for query.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public static IEnumerable<TrendValue> GetHistorianData(SnapServer server, string instanceName, DateTime startTime, DateTime stopTime, ulong[] measurementIDs, Resolution resolution, int seriesLimit, bool forceLimit, ICancellationToken cancellationToken = null)
        {
            if (cancellationToken is null)
                cancellationToken = new CancellationToken();

            if (server is null)
                yield break;

            // Setting series limit to zero requests full resolution data, which overrides provided parameter
            if (seriesLimit < 1)
            {
                resolution = Resolution.Full;
                forceLimit = false;
            }

            TimeSpan resolutionInterval = resolution.GetInterval();
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = null;
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            bool subFullResolution = false;

            // Set data scan resolution
            if (resolution != Resolution.Full)
            {
                subFullResolution = true;

                BaselineTimeInterval interval = BaselineTimeInterval.Second;

                if (resolutionInterval.Ticks < Ticks.PerMinute)
                    interval = BaselineTimeInterval.Second;
                else if (resolutionInterval.Ticks < Ticks.PerHour)
                    interval = BaselineTimeInterval.Minute;
                else if (resolutionInterval.Ticks == Ticks.PerHour)
                    interval = BaselineTimeInterval.Hour;

                startTime = startTime.BaselinedTimestamp(interval);
                stopTime = stopTime.BaselinedTimestamp(interval);
            }

            SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);

            Dictionary<ulong, DataRow> metadata = null;

            using (SnapClient connection = SnapClient.Connect(server))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName))
            {
                if (database is null)
                    yield break;

                if (LocalOutputAdapter.Instances.TryGetValue(database.Info?.DatabaseName ?? DefaultInstanceName, out LocalOutputAdapter historianAdapter))
                    metadata = historianAdapter?.Measurements;

                if (metadata is null)
                    yield break;

                // Setup point ID selections
                if (measurementIDs != null)
                    pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(measurementIDs);
                else
                    measurementIDs = metadata.Keys.ToArray();

                Dictionary<ulong, long> pointCounts = new Dictionary<ulong, long>(measurementIDs.Length);
                Dictionary<ulong, ulong> lastTimes = new Dictionary<ulong, ulong>(measurementIDs.Length);
                Dictionary<ulong, Tuple<float, float>> extremes = new Dictionary<ulong, Tuple<float, float>>(measurementIDs.Length);
                ulong pointID, timestamp, resolutionSpan = (ulong)resolutionInterval.Ticks, baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;
                long pointCount;
                float pointValue, min = 0.0F, max = 0.0F;

                foreach (ulong measurementID in measurementIDs)
                    pointCounts[measurementID] = 0L;

                // Start stream reader for the provided time window and selected points
                using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter))
                {
                    while (stream.Read(key, value) && !cancellationToken.IsCancelled)
                    {
                        pointID = key.PointID;
                        timestamp = key.Timestamp;
                        pointCount = pointCounts[pointID];
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
                            pointCount++;

                            if (forceLimit && pointCount > seriesLimit)
                                break;

                            yield return new TrendValue
                            {
                                ID = (long)pointID,
                                Timestamp = (timestamp - baseTicks) / (double)Ticks.PerMillisecond,
                                Value = pointValue
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

                        pointCounts[pointID] = pointCount;
                    }
                }
            }
        }
    }
}
