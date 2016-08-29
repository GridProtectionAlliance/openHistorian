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
using GSF;
using GSF.Collections;
using GSF.Data;
using GSF.Data.Model;
using GSF.TimeSeries;
using GSF.Web.Hubs;
using openHistorian.Model;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a client instance of a SignalR Hub for historian data queries.
    /// </summary>
    public class HistorianQueryHubClient : HubClientBase
    {
        #region [ Members ]

        // Fields
        private Connection m_connection;
        private bool m_disposed;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets historian connection instance, creating a new one if needed.
        /// </summary>
        public Connection Connection
        {
            get
            {
                if ((object)m_connection == null)
                {
                    try
                    {
                        m_connection = new Connection($"127.0.0.1:{s_portNumber}", s_instanceName);
                    }
                    catch (Exception ex)
                    {
                        LogException(new InvalidOperationException($"Failed to connect to historian: {ex.Message}", ex));
                    }
                }

                return m_connection;
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
                        m_connection?.Dispose();
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
        /// <returns>Enumeration of <see cref="MeasurementValue"/> instances read for time range.</returns>
        public IEnumerable<MeasurementValue> GetHistorianData(DateTime startTime, DateTime stopTime, long[] measurementIDs, Resolution resolution, int seriesLimit)
        {
            Connection connection = Connection;
            
            lock (connection)
            {
                if (seriesLimit < 2)
                {
                    // Return full resolution data
                    return MeasurementAPI.GetHistorianData(connection, startTime, stopTime, measurementIDs.Select(id => (ulong)id), resolution).Select(measurement => new MeasurementValue
                    {
                        ID = measurement.ID,
                        Timestamp = GetUnixMilliseconds(measurement.Timestamp),
                        Value = measurement.AdjustedValue
                    });
                }

                // Reduce data-set to series limit
                IMeasurement[] measurements = MeasurementAPI.GetHistorianData(connection, startTime, stopTime, measurementIDs.Select(id => (ulong)id), resolution).ToArray();

                Dictionary<ulong, List<MeasurementValue>> data = new Dictionary<ulong, List<MeasurementValue>>(measurements.Length);
                Dictionary<ulong, long> pointCounts = new Dictionary<ulong, long>();
                Dictionary<ulong, long> intervals = new Dictionary<ulong, long>();
                List<MeasurementValue> values;
                long pointCount;

                // Count total measurements per point to calculate distribution intervals for each series
                foreach (IMeasurement measurement in measurements)
                    pointCounts[measurement.Key.ID] = pointCounts.GetOrAdd(measurement.Key.ID, 0L) + 1;

                foreach (ulong pointID in pointCounts.Keys)
                    intervals[pointID] = (pointCounts[pointID] / seriesLimit) + 1;

                foreach (IMeasurement measurement in measurements)
                {
                    ulong pointID = measurement.Key.ID;

                    values = data.GetOrAdd(pointID, id => new List<MeasurementValue>());
                    pointCount = pointCounts[pointID];

                    if (pointCount++ % intervals[pointID] == 0)
                        values.Add(new MeasurementValue
                        {
                            ID = measurement.ID,
                            Timestamp = GetUnixMilliseconds(measurement.Timestamp),
                            Value = measurement.AdjustedValue
                        });

                    pointCounts[pointID] = pointCount;
                }

                return data.Values.SelectMany(measurementValues => measurementValues);
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
                        s_portNumber = Connection.DefaultHistorianPort;

                    if (!settings.TryGetValue("instanceName", out s_instanceName) || string.IsNullOrWhiteSpace(s_instanceName))
                        s_instanceName = record.AdapterName ?? "PPA";
                }
            }
            catch
            {
                s_instanceName = "PPA";
                s_portNumber = Connection.DefaultHistorianPort;
            }
        }

        private static double GetUnixMilliseconds(long ticks)
        {
            return new DateTime(ticks).Subtract(new DateTime(UnixTimeTag.BaseTicks)).TotalMilliseconds;
        }

        #endregion
    }
}
