//******************************************************************************************************
//  MeasurementAPI.cs - Gbtc
//
//  Copyright © 2015, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/22/2015 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using GSF;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services.Reader;
using GSF.TimeSeries;
using openHistorian.Net;
using openHistorian.Snap;
using Database = GSF.Snap.Services.ClientDatabaseBase<openHistorian.Snap.HistorianKey, openHistorian.Snap.HistorianValue>;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Time resolutions.
    /// </summary>
    public enum Resolution
    {
        /// <summary>
        /// Query at full resolution.
        /// </summary>
        Full,
        /// <summary>
        /// Query at ten samples per second.
        /// </summary>
        TenPerSecond,
        /// <summary>
        /// Query at one sample per second.
        /// </summary>
        EverySecond,
        /// <summary>
        /// Query at one sample every ten seconds.
        /// </summary>
        Every10Seconds,
        /// <summary>
        /// Query at one sample every thirty seconds.
        /// </summary>
        Every30Seconds,
        /// <summary>
        /// Query at one sample every minute.
        /// </summary>
        EveryMinute,
        /// <summary>
        /// Query at one sample every ten minutes.
        /// </summary>
        Every10Minutes,
        /// <summary>
        /// Query at one sample every thirty minutes.
        /// </summary>
        Every30Minutes,
        /// <summary>
        /// Query at one sample every hour.
        /// </summary>
        EveryHour,
        /// <summary>
        /// Query at one sample every day.
        /// </summary>
        EveryDay,
        /// <summary>
        /// Query at one sample every month.
        /// </summary>
        EveryMonth
    }

    /// <summary>
    /// Represents an openHistorian connection.
    /// </summary>
    public class Connection : IDisposable
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Default openHistorian connection port.
        /// </summary>
        public const int DefaultHistorianPort = 38402;

        // Fields
        private readonly HistorianClient m_client;
        private readonly string m_instanceName;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new openHistorian <see cref="Connection"/>.
        /// </summary>
        /// <param name="historianServer">Historian server connection string, e.g., "localhost" or "myhistorian:38402"</param>
        /// <param name="instanceName">Instance name of historian, e.g., "PPA".</param>
        public Connection(string historianServer, string instanceName)
        {
            if (string.IsNullOrEmpty(historianServer))
                throw new ArgumentNullException(nameof(historianServer), "Missing historian server parameter");

            if (string.IsNullOrEmpty(instanceName))
                throw new ArgumentNullException(nameof(instanceName), "Missing historian instance name parameter");

            string[] parts = historianServer.Split(':');
            string hostName = parts[0];
            int port;

            if (parts.Length < 2 || !int.TryParse(parts[1], out port))
                port = DefaultHistorianPort;

            m_client = new HistorianClient(hostName, port);
            m_instanceName = instanceName;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets reference to <see cref="HistorianClient"/> instance of this <see cref="Connection"/>.
        /// </summary>
        public HistorianClient Client
        {
            get
            {
                return m_client;
            }
        }

        /// <summary>
        /// Gets instance name of this <see cref="Connection"/>.
        /// </summary>
        public string InstanceName
        {
            get
            {
                return m_instanceName;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="Connection"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Connection"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if ((object)m_client != null)
                            m_client.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Opens new database instance for this <see cref="Connection"/>.
        /// </summary>
        /// <param name="instanceName">Instance name of historian, e.g., "PPA" - or <c>null</c> to use default instance name associated with this connection.</param>
        public Database OpenDatabase(string instanceName = null)
        {
            return m_client.GetDatabase<HistorianKey, HistorianValue>(instanceName ?? m_instanceName);
        }

        #endregion
    }

    /// <summary>
    /// Defines openHistorian data query API functions.
    /// </summary>
    public static class MeasurementAPI
    {
        ///// <summary>
        ///// Establish measurement key cache based on received meta-data.
        ///// </summary>
        ///// <param name="metadata">Meta-data received from a subscription.</param>
        ///// <param name="instanceName">Instance name of the historian.</param>
        ///// <remarks>
        ///// If you want the <see cref="IMeasurement.Key"/> values that are returned from a <see cref="GetHistorianData"/> query
        ///// to have properly assigned Guid based signal IDs, i.e., <see cref="MeasurementKey.SignalID"/>, establish the
        ///// measurement key cache as soon as you have received meta-data from a GEP subscription query.
        ///// </remarks>

        /// <summary>
        /// Reads interpolated historian data from server.
        /// </summary>
        /// <param name="connection">openHistorian connection.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="interval">The sampling interval for the interpolated data.</param>
        /// <param name="measurementIDs">Comma separated list of measurement IDs to query.</param>
        /// <returns>Enumeration of <see cref="IMeasurement"/> values read for time range.</returns>
        /// <remarks>
        /// <example>
        /// <code>
        /// using (var connection = new Connection("127.0.0.1", "PPA"))
        ///     foreach(var measurement in GetInterpolatedData(connection, DateTime.UtcNow.AddMinutes(-1.0D), DateTime.UtcNow, TimeSpan.FromSeconds(2.0D), "7,5,15"))
        ///         Console.WriteLine("{0}:{1} @ {2} = {3}, quality: {4}", measurement.Key.Source, measurement.Key.ID, measurement.Timestamp, measurement.Value, measurement.StateFlags);
        /// </code>
        /// </example>
        /// </remarks>
        public static IEnumerable<IMeasurement> GetInterpolatedData(Connection connection, DateTime startTime, DateTime stopTime, TimeSpan interval, string measurementIDs)
        {
            if ((object)measurementIDs == null)
                throw new ArgumentNullException(nameof(measurementIDs));

            // TODO: This was based on code that aligns data and fills time - this function will
            // need to be re-worked to provide data on an interval based on slopes and time...
            Measurement[] values = measurementIDs.Split(',').Select(uint.Parse).Select(id => new Measurement() { Metadata = MeasurementKey.LookUpBySource(connection.InstanceName, id).Metadata }).ToArray();
            long tickInterval = interval.Ticks;
            long lastTimestamp = 0L;

            foreach (IMeasurement measurement in GetHistorianData(connection, startTime, stopTime, measurementIDs))
            {
                long timestamp = measurement.Timestamp;

                // Start a new row for each encountered new timestamp
                if (timestamp != lastTimestamp)
                {
                    if (lastTimestamp > 0)
                        foreach (IMeasurement value in values)
                            yield return value;

                    for (int i = 0; i < values.Length; i++)
                        values[i] = Measurement.Clone(values[i]);

                    if (lastTimestamp > 0 && timestamp > lastTimestamp)
                    {
                        long difference = timestamp - lastTimestamp;

                        if (difference > tickInterval)
                        {
                            long interpolated = lastTimestamp;

                            for (long i = 1; i < difference / tickInterval; i++)
                            {
                                interpolated = interpolated + tickInterval;

                                foreach (IMeasurement value in values)
                                    yield return value;
                            }
                        }
                    }

                    lastTimestamp = timestamp;
                }
            }
        }

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="connection">openHistorian connection.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Comma separated list of measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <returns>Enumeration of <see cref="IMeasurement"/> values read for time range.</returns>
        /// <remarks>
        /// <example>
        /// <code>
        /// using (var connection = new Connection("127.0.0.1", "PPA"))
        ///     foreach(var measurement in GetHistorianData(connection, DateTime.UtcNow.AddMinutes(-1.0D), DateTime.UtcNow))
        ///         Console.WriteLine("{0}:{1} @ {2} = {3}, quality: {4}", measurement.Key.Source, measurement.Key.ID, measurement.Timestamp, measurement.Value, measurement.StateFlags);
        /// </code>
        /// </example>
        /// </remarks>
        public static IEnumerable<IMeasurement> GetHistorianData(Connection connection, DateTime startTime, DateTime stopTime, string measurementIDs = null, Resolution resolution = Resolution.Full)
        {
            return GetHistorianData(connection, startTime, stopTime, measurementIDs?.Split(',').Select(ulong.Parse), resolution);
        }

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="connection">openHistorian connection.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Array of measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <returns>Enumeration of <see cref="IMeasurement"/> values read for time range.</returns>
        /// <remarks>
        /// <example>
        /// <code>
        /// using (var connection = new Connection("127.0.0.1", "PPA"))
        ///     foreach(var measurement in GetHistorianData(connection, DateTime.UtcNow.AddMinutes(-1.0D), DateTime.UtcNow))
        ///         Console.WriteLine("{0}:{1} @ {2} = {3}, quality: {4}", measurement.Key.Source, measurement.Key.ID, measurement.Timestamp, measurement.Value, measurement.StateFlags);
        /// </code>
        /// </example>
        /// </remarks>
        public static IEnumerable<IMeasurement> GetHistorianData(Connection connection, DateTime startTime, DateTime stopTime, IEnumerable<ulong> measurementIDs = null, Resolution resolution = Resolution.Full)
        {
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
            using (Database database = connection.OpenDatabase())
            {
                TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

                while (stream.Read(key, value))
                    yield return new Measurement()
                    {
                        Metadata = MeasurementKey.LookUpOrCreate(connection.InstanceName, (uint)key.PointID).Metadata,
                        Timestamp = key.TimestampAsDate,
                        Value = value.AsSingle,
                        StateFlags = (MeasurementStateFlags)value.Value3
                    };
            }
        }

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="connection">openHistorian connection.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementID">Measurement ID to test for data continuity.</param>
        /// <param name="resolution">Resolution for testing data.</param>
        /// <param name="expectedFullResolutionTicks">Expected number of ticks per interval at full resolution, e.g., 33,333 = 1/30 of a second representing a sampling interval of 30 times per second.</param>
        /// <returns>Enumeration of valid data ranges for specified time range.</returns>
        /// <remarks>
        /// 1 tick = 100 nanoseconds.
        /// </remarks>
        public static IEnumerable<Tuple<DateTime, DateTime>> GetContiguousDataRegions(Connection connection, DateTime startTime, DateTime stopTime, ulong measurementID, Resolution resolution, long expectedFullResolutionTicks = 333333)
        {
            // Setup time-range and point ID selections
            SeekFilterBase<HistorianKey> timeFilter;
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromPointID<HistorianKey, HistorianValue>(measurementID);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            TimeSpan interval, tolerance;

            // Set data scan resolution
            if (resolution == Resolution.Full)
            {
                interval = new TimeSpan(expectedFullResolutionTicks);
                timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);
            }
            else
            {
                interval = resolution.GetInterval();
                timeFilter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, interval, new TimeSpan(TimeSpan.TicksPerMillisecond));
            }

            // PMUs times may float a little - provide a one millisecond tolerance window above the standard interval
            tolerance = interval.Add(TimeSpan.FromMilliseconds(1.0D));

            DateTime lastStartTime = startTime;
            DateTime lastStopTime = startTime;
            DateTime nextExpectedTime = startTime;
            DateTime currentTime;
            long totalRegions = 0;

            // Start stream reader for the provided time window and selected points
            using (Database database = connection.OpenDatabase())
            {
                TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

                // Scan historian stream for given point over specified time range and data resolution
                while (stream.Read(key, value))
                {
                    currentTime = key.TimestampAsDate;

                    // See if current time was not expected time and gap is larger than resolution tolerance - could simply
                    // be user started with a time that was not aligned with desired resolution, hence the tolerance check
                    if (currentTime != nextExpectedTime && currentTime - nextExpectedTime > tolerance)
                    {
                        if (lastStartTime != lastStopTime)
                        {
                            // Detected a data gap, return last contiguous region
                            totalRegions++;
                            yield return new Tuple<DateTime, DateTime>(lastStartTime, lastStopTime);
                        }

                        // Move start time to current value
                        lastStartTime = currentTime;
                        lastStopTime = lastStartTime;
                        nextExpectedTime = lastStartTime + interval;
                    }
                    else
                    {
                        // Setup next expected timestamp
                        nextExpectedTime += interval;
                        lastStopTime = currentTime;
                    }
                }

                // If no data gaps were detected, return a single value for full region for where there was data
                if (totalRegions == 0 && lastStartTime != lastStopTime)
                    yield return new Tuple<DateTime, DateTime>(lastStartTime, lastStopTime);
            }
        }
    }

    /// <summary>
    /// Defines extension functions for the <see cref="Resolution"/> enumeration.
    /// </summary>
    public static class ResolutionExtensions
    {
        /// <summary>
        /// Gets <see cref="TimeSpan"/> for specified <paramref name="resolution"/>.
        /// </summary>
        /// <param name="resolution">Resolution to get interval for.</param>
        /// <returns><see cref="TimeSpan"/> for specified <paramref name="resolution"/>.</returns>
        public static TimeSpan GetInterval(this Resolution resolution)
        {
            switch (resolution)
            {
                case Resolution.Full:
                    return TimeSpan.Zero;
                case Resolution.TenPerSecond:
                    return new TimeSpan(TimeSpan.TicksPerMillisecond * 100);
                case Resolution.EverySecond:
                    return new TimeSpan(TimeSpan.TicksPerSecond * 1);
                case Resolution.Every10Seconds:
                    return new TimeSpan(TimeSpan.TicksPerSecond * 10);
                case Resolution.Every30Seconds:
                    return new TimeSpan(TimeSpan.TicksPerSecond * 30);
                case Resolution.EveryMinute:
                    return new TimeSpan(TimeSpan.TicksPerMinute * 1);
                case Resolution.Every10Minutes:
                    return new TimeSpan(TimeSpan.TicksPerMinute * 10);
                case Resolution.Every30Minutes:
                    return new TimeSpan(TimeSpan.TicksPerMinute * 30);
                case Resolution.EveryHour:
                    return new TimeSpan(TimeSpan.TicksPerHour * 1);
                case Resolution.EveryDay:
                    return new TimeSpan(TimeSpan.TicksPerDay);
                case Resolution.EveryMonth:
                    return new TimeSpan(TimeSpan.TicksPerDay * 30);
                default:
                    throw new Exception("Unknown resolution");
            }
        }
    }
}
