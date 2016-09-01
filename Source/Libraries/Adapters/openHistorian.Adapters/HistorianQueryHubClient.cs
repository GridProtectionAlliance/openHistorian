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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private CancellationTokenSource m_cancellationTokenSource;
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
                        HistorianServer serverInstance = LocalOutputAdapter.ServerIntances.Values.FirstOrDefault();

                        if ((object)serverInstance == null)
                            throw new InvalidOperationException("Failed to access internal historian server instance.");

                        m_connection = SnapClient.Connect(serverInstance.Host);
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
                        m_cancellationTokenSource?.Dispose();
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
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public async Task<IEnumerable<TrendValue>> GetHistorianData(DateTime startTime, DateTime stopTime, long[] measurementIDs, Resolution resolution, int seriesLimit)
        {
            // Cancel any running query
            m_cancellationTokenSource?.Dispose(); // This will cancel pending operations
            m_cancellationTokenSource = new CancellationTokenSource();

            // Return full resolution data
            if (seriesLimit < 2)
                return await GetHistorianData(startTime, stopTime, measurementIDs.Select(id => (ulong)id), resolution, m_cancellationTokenSource.Token);

            // Reduce data-set to series limit
            List<TrendValue> trendValues = await GetHistorianData(startTime, stopTime, measurementIDs.Select(id => (ulong)id), resolution, m_cancellationTokenSource.Token);

            Dictionary<long, List<TrendValue>> seriesData = new Dictionary<long, List<TrendValue>>(trendValues.Count);
            Dictionary<long, long> pointCounts = new Dictionary<long, long>();
            Dictionary<long, long> intervals = new Dictionary<long, long>();
            List<TrendValue> seriesValues;
            long pointCount;

            // Count total measurements per point to calculate distribution intervals for each series
            foreach (TrendValue trendValue in trendValues)
                pointCounts[trendValue.ID] = pointCounts.GetOrAdd(trendValue.ID, 0L) + 1;

            foreach (long pointID in pointCounts.Keys)
                intervals[pointID] = (pointCounts[pointID] / seriesLimit) + 1;

            foreach (TrendValue trendValue in trendValues)
            {
                long pointID = trendValue.ID;

                seriesValues = seriesData.GetOrAdd(pointID, id => new List<TrendValue>());
                pointCount = pointCounts[pointID];

                if (pointCount++ % intervals[pointID] == 0)
                    seriesValues.Add(trendValue);

                pointCounts[pointID] = pointCount;
            }

            return seriesData.Values.SelectMany(measurementValues => measurementValues);
        }

        /// <summary>
        /// If the openHistorian adapter parameters get updated, e.g., listening port or instance name, this function can be called to refresh the values.
        /// </summary>
        public void RefreshConnectionParameters()
        {
            LoadConnectionParameters();
            Interlocked.Exchange(ref m_connection, null)?.Dispose();
        }

        private Task<List<TrendValue>> GetHistorianData(DateTime startTime, DateTime stopTime, IEnumerable<ulong> measurementIDs, Resolution resolution, CancellationToken cancellationToken)
        {
           return Task.Factory.StartNew(() =>
           {
               List<TrendValue> trendValues = new List<TrendValue>();
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
                   TimeSpan resolutionInterval = resolution.GetInterval();
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

               // Setup point ID selections
               if ((object)measurementIDs != null)
                   pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(measurementIDs);

               // Start stream reader for the provided time window and selected points
               ClientDatabaseBase<HistorianKey, HistorianValue> database = Database;

               lock (database)
               {
                   TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

                   while (stream.Read(key, value) && !cancellationToken.IsCancellationRequested)
                       trendValues.Add(new TrendValue
                       {
                           ID = (long)key.PointID,
                           Timestamp = GetUnixMilliseconds(key.TimestampAsDate.Ticks),
                           Value = value.AsSingle
                       });
               }

               return trendValues;
           },
           cancellationToken);
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

        private static double GetUnixMilliseconds(long ticks)
        {
            return new DateTime(ticks).Subtract(new DateTime(UnixTimeTag.BaseTicks)).TotalMilliseconds;
        }

        #endregion
    }
}
