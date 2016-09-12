//******************************************************************************************************
//  HistorianQueryHubClient.cs - Gbtc
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
//  06/07/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using GSF;
using GSF.Collections;
using GSF.Data;
using GSF.Data.Model;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Web.Hubs;
using openHistorian.Model;
using openHistorian.Net;
using openHistorian.Snap;
using CancellationToken = GSF.Threading.CancellationToken;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a client instance of a SignalR Hub for historian data queries.
    /// </summary>
    public class HistorianQueryHubClient : HubClientBase
    {
        #region [ Members ]

        // Fields
        private SnapClient m_connection;
        private ClientDatabaseBase<HistorianKey, HistorianValue> m_database;
        private CancellationToken m_cancellationToken;
        private bool m_disposed;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets historian connection instance, creating a new one if needed.
        /// </summary>
        private SnapClient Connection
        {
            get
            {
                if ((object)m_connection == null)
                {
                    try
                    {
                        HistorianServer serverInstance;

                        if (LocalOutputAdapter.ServerInstances.TryGetValue(InstanceName, out serverInstance))
                        {
                            if ((object)serverInstance == null)
                                throw new InvalidOperationException("Failed to access internal historian server instance.");

                            m_connection = SnapClient.Connect(serverInstance.Host);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogException(new InvalidOperationException($"Failed to connect to historian: {ex.Message}", ex));
                    }
                }

                return m_connection;
            }
        }

        private ClientDatabaseBase<HistorianKey, HistorianValue> Database
        {
            get
            {
                if ((object)m_database == null)
                {
                    try
                    {
                        SnapClient connection = Connection;

                        if ((object)connection != null)
                            m_database = Connection.GetDatabase<HistorianKey, HistorianValue>(InstanceName);
                    }
                    catch (Exception ex)
                    {
                        LogException(new InvalidOperationException($"Failed to access historian database instance \"{InstanceName}\": {ex.Message}", ex));
                    }
                }

                return m_database;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HistorianQueryHubClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        m_database?.Dispose();
                        m_connection?.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <param name="forceLimit">Flag that determines if series limit should be strictly enforced.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public IEnumerable<TrendValue> GetHistorianData(DateTime startTime, DateTime stopTime, ulong[] measurementIDs, Resolution resolution, int seriesLimit, bool forceLimit)
        {
            // Cancel any running query
            CancellationToken cancellationToken = new CancellationToken();
            Interlocked.Exchange(ref m_cancellationToken, cancellationToken)?.Cancel();

            TimeSpan resolutionInterval = resolution.GetInterval();
            SeekFilterBase<HistorianKey> timeFilter;
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = null;
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            // Set data scan resolution
            if (resolution == Resolution.Full)
            {
                timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);
            }
            else
            {
                BaselineTimeInterval interval = BaselineTimeInterval.Second;

                if (resolutionInterval.Ticks < Ticks.PerMinute)
                    interval = BaselineTimeInterval.Second;
                else if (resolutionInterval.Ticks < Ticks.PerHour)
                    interval = BaselineTimeInterval.Minute;
                else if (resolutionInterval.Ticks == Ticks.PerHour)
                    interval = BaselineTimeInterval.Hour;

                startTime = startTime.BaselinedTimestamp(interval);
                stopTime = stopTime.BaselinedTimestamp(interval);

                timeFilter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, resolutionInterval, new TimeSpan(TimeSpan.TicksPerMillisecond));
            }

            Dictionary<ulong, DataRow> metadata = LocalOutputAdapter.ServerMetaData[InstanceName];

            // Setup point ID selections
            if ((object)measurementIDs != null)
                pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(measurementIDs);
            else
                measurementIDs = metadata.Keys.ToArray();

            // Start stream reader for the provided time window and selected points
            ClientDatabaseBase<HistorianKey, HistorianValue> database = Database;

            if ((object)database == null)
                yield break;

            Dictionary<ulong, long> pointCounts = new Dictionary<ulong, long>(measurementIDs.Length);
            Dictionary<ulong, long> intervals = new Dictionary<ulong, long>(measurementIDs.Length);
            Dictionary<ulong, ulong> lastTimes = new Dictionary<ulong, ulong>(measurementIDs.Length);
            double range = (stopTime - startTime).TotalSeconds;
            long estimatedPointCount = (long)(range / resolutionInterval.TotalSeconds.NotZero(1.0D));
            ulong pointID, timestamp, resolutionSpan = (ulong)resolutionInterval.Ticks, baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;
            long pointCount;
            DataRow row;

            if (resolutionSpan <= 1UL)
                resolutionSpan = Ticks.PerSecond;

            if (seriesLimit < 1)
                seriesLimit = 1;

            // Estimate total measurement counts per point so decimation intervals for each series can be calculated
            foreach (ulong measurementID in measurementIDs)
            {
                if (resolution == Resolution.Full)
                    pointCounts[measurementID] = metadata.TryGetValue(measurementID, out row) ? (long)(int.Parse(row["FramesPerSecond"].ToString()) * range) : 2;
                else
                    pointCounts[measurementID] = estimatedPointCount;
            }

            foreach (ulong measurementID in pointCounts.Keys)
                intervals[measurementID] = (pointCounts[measurementID] / seriesLimit).NotZero(1L);

            lock (database)
            {
                TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

                while (stream.Read(key, value) && !cancellationToken.Cancelled)
                {
                    pointID = key.PointID;
                    timestamp = key.Timestamp;
                    pointCount = pointCounts[pointID];

                    if (pointCount++ % intervals[pointID] == 0 || (!forceLimit && timestamp - lastTimes.GetOrAdd(pointID, 0UL) > resolutionSpan))
                        yield return new TrendValue
                        {
                            ID = (long)pointID,
                            Timestamp = (timestamp - baseTicks) / (double)Ticks.PerMillisecond,
                            Value = value.AsSingle
                        };

                    pointCounts[pointID] = pointCount;
                    lastTimes[pointID] = timestamp;
                }
            }
        }

        /// <summary>
        /// If the openHistorian adapter parameters get updated, e.g., listening port or instance name, this function can be called to refresh the values.
        /// </summary>
        public void RefreshConnectionParameters()
        {
            LoadConnectionParameters();
            Interlocked.Exchange(ref m_connection, null)?.Dispose();
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static string s_instanceName;
        private static int s_portNumber;

        // Static Constructor
        static HistorianQueryHubClient()
        {
            LoadConnectionParameters();
        }

        // Static Properties

        /// <summary>
        /// Gets configured instance name for historian.
        /// </summary>
        public static string InstanceName => s_instanceName;

        /// <summary>
        /// Gets configured port number for historian.
        /// </summary>
        public static int PortNumber => s_portNumber;

        // Static Methods

        private static void LoadConnectionParameters()
        {
            try
            {
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<IaonOutputAdapter> operations = new TableOperations<IaonOutputAdapter>(connection);
                    IaonOutputAdapter record = operations.QueryRecords(limit: 1, restriction: new RecordRestriction
                    {
                        FilterExpression = "TypeName = 'openHistorian.Adapters.LocalOutputAdapter'"
                    })
                    .FirstOrDefault();

                    if ((object)record == null)
                        throw new NullReferenceException("Primary openHistorian adapter instance not found.");

                    Dictionary<string, string> settings = record.ConnectionString.ParseKeyValuePairs();
                    string setting;

                    if (!settings.TryGetValue("port", out setting) || !int.TryParse(setting, out s_portNumber))
                        s_portNumber = Adapters.Connection.DefaultHistorianPort;

                    if (!settings.TryGetValue("instanceName", out s_instanceName) || string.IsNullOrWhiteSpace(s_instanceName))
                        s_instanceName = record.AdapterName ?? "PPA";
                }
            }
            catch
            {
                s_instanceName = "PPA";
                s_portNumber = Adapters.Connection.DefaultHistorianPort;
            }
        }

        #endregion
    }
}