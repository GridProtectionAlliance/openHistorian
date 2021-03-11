//******************************************************************************************************
//  HistorianFunctions.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  05/01/2015 - J. Ritchie Carroll
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Linq;
using GSF;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using Microsoft.SqlServer.Server;
using openHistorian.Net;
using openHistorian.Snap;

/// <summary>
/// openHistorian SQL CLR procedure used to query historian data.
/// </summary>
// ReSharper disable once CheckNamespace
public class HistorianFunctions
{
    /// <summary>
    /// Defines a floating-point measurement value.
    /// </summary>
    // No need to make this a structure since it will just get boxed when passed to FillRow function anyway...
    public class Measurement
    {
        /// <summary>
        /// Measurement ID.
        /// </summary>
        public readonly ulong ID;

        /// <summary>
        /// Measurement timestamp.
        /// </summary>
        public readonly DateTime Time;

        /// <summary>
        /// Measurement value.
        /// </summary>
        public readonly float Value;

        /// <summary>
        /// Creates a new <see cref="Measurement"/>.
        /// </summary>
        /// <param name="id">Measurement ID.</param>
        /// <param name="time">Measurement timestamp.</param>
        /// <param name="value">Measurement value.</param>
        public Measurement(ulong id, DateTime time, float value)
        {
            ID = id;
            Time = time;
            Value = value;
        }
    }

    /// <summary>
    /// Queries measurement data from the openHistorian.
    /// </summary>
    /// <param name="historianServer">Historian server host IP or DNS name. Can be optionally suffixed with port number, e.g.: historian:38402.</param>
    /// <param name="instanceName">Instance name of the historian.</param>
    /// <param name="startTime">Start time of desired data range.</param>
    /// <param name="stopTime">End time of desired data range.</param>
    /// <param name="measurementIDs">Comma separated list of measurement ID values; set to <c>null</c>to retrieve values for all measurements.</param>
    /// <returns>
    /// Enumerable historian data results for specified time range and points.
    /// </returns>
    [SqlFunction(
        DataAccess = DataAccessKind.Read,
        FillRowMethodName = "GetHistorianData_FillRow",
        TableDefinition = "[ID] bigint, [Time] datetime2, [Value] real")
    ]
    public static IEnumerable GetHistorianData(SqlString historianServer, SqlString instanceName, DateTime startTime, DateTime stopTime, [SqlFacet(MaxSize = -1)] SqlString measurementIDs)
    {
        return GetHistorianDataSampled(historianServer, instanceName, startTime, stopTime, TimeSpan.Zero, measurementIDs);
    }

    /// <summary>
    /// Queries measurement data from the openHistorian.
    /// </summary>
    /// <param name="historianServer">Historian server host IP or DNS name. Can be optionally suffixed with port number, e.g.: historian:38402.</param>
    /// <param name="instanceName">Instance name of the historian.</param>
    /// <param name="startTime">Start time of desired data range.</param>
    /// <param name="stopTime">End time of desired data range.</param>
    /// <param name="interval">Interval of data points.</param>
    /// <param name="measurementIDs">Comma separated list of measurement ID values; set to <c>null</c>to retrieve values for all measurements.</param>
    /// <returns>
    /// Enumerable historian data results for specified time range and points.
    /// </returns>
    [SqlFunction(
        DataAccess = DataAccessKind.Read,
        FillRowMethodName = "GetHistorianData_FillRow",
        TableDefinition = "[ID] bigint, [Time] datetime2, [Value] real")
    ]
    public static IEnumerable GetHistorianDataSampled(SqlString historianServer, SqlString instanceName, DateTime startTime, DateTime stopTime, TimeSpan interval, [SqlFacet(MaxSize = -1)] SqlString measurementIDs)
    {
        const int DefaultHistorianPort = 38402;

        if (historianServer.IsNull || string.IsNullOrEmpty(historianServer.Value))
            throw new ArgumentNullException("historianServer", "Missing historian server parameter");

        if (instanceName.IsNull || string.IsNullOrEmpty(instanceName.Value))
            throw new ArgumentNullException("instanceName", "Missing historian instance name parameter");

        if (startTime > stopTime)
            throw new ArgumentException("Invalid time range specified", "startTime");

        string[] parts = historianServer.Value.Split(':');
        string hostName = parts[0];

        if (parts.Length < 2 || !int.TryParse(parts[1], out int port))
            port = DefaultHistorianPort;

        using (HistorianClient client = new HistorianClient(hostName, port))
        using (ClientDatabaseBase<HistorianKey, HistorianValue> reader = client.GetDatabase<HistorianKey, HistorianValue>(instanceName.Value))
        {
            SeekFilterBase<HistorianKey> timeFilter = interval.Ticks == 0 ? TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime) :
                                            TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, interval, new TimeSpan(TimeSpan.TicksPerMillisecond));

            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = null;
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            if (!measurementIDs.IsNull && !string.IsNullOrEmpty(measurementIDs.Value))
                pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(measurementIDs.Value.Split(',').Select(ulong.Parse));

            // Start stream reader for the provided time window and selected points
            using (TreeStream<HistorianKey, HistorianValue> stream = reader.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter))
            {
                while (stream.Read(key, value))
                    yield return new Measurement(key.PointID, key.TimestampAsDate, value.AsSingle);
            }
        }
    }

    /// <summary>
    /// Used to fill table columns with enumerable data returned from <see cref="GetHistorianData"/>.
    /// </summary>
    /// <param name="source">Source data, i.e., a <see cref="Measurement"/>.</param>
    /// <param name="id">Measurement ID</param>
    /// <param name="time">Measurement timestamp</param>
    /// <param name="value">Measurement value</param>
    public static void GetHistorianData_FillRow(object source, out SqlInt64 id, out DateTime time, out SqlSingle value)
    {
        Measurement measurement = source as Measurement;

        if (measurement is null)
            throw new InvalidOperationException("FillRow source is not a Measurement");

        id = (long)measurement.ID;
        time = measurement.Time;
        value = measurement.Value;
    }

    /// <summary>
    /// Returns the unsigned high-double-word (SqlInt32) from a quad-word (SqlInt64).
    /// </summary>
    /// <param name="quadWord">8-byte, 64-bit integer value.</param>
    /// <returns>The high-order double-word of the specified 64-bit integer value.</returns>
    /// <remarks>
    /// On little-endian architectures (e.g., Intel platforms), this will be the word value
    /// whose in-memory representation is the same as the right-most, most-significant-word
    /// of the integer value.
    /// </remarks>
    public static SqlInt32 HighDoubleWord(SqlInt64 quadWord)
    {
        if (quadWord.IsNull)
            throw new ArgumentNullException("quadWord");

        return (int)((ulong)quadWord.Value).HighDoubleWord();
    }

    /// <summary>
    /// Returns the low-double-word (SqlInt32) from a quad-word (SqlInt64).
    /// </summary>
    /// <param name="quadWord">8-byte, 64-bit integer value.</param>
    /// <returns>The low-order double-word of the specified 64-bit integer value.</returns>
    /// <remarks>
    /// On little-endian architectures (e.g., Intel platforms), this will be the word value
    /// whose in-memory representation is the same as the left-most, least-significant-word
    /// of the integer value.
    /// </remarks>
    public static SqlInt32 LowDoubleWord(SqlInt64 quadWord)
    {
        if (quadWord.IsNull)
            throw new ArgumentNullException("quadWord");

        return (int)((ulong)quadWord.Value).LowDoubleWord();
    }

    /// <summary>
    /// Makes a quad-word (SqlInt64) from two double-words (SqlInt32).
    /// </summary>
    /// <param name="high">High double-word.</param>
    /// <param name="low">Low double-word.</param>
    /// <returns>A 64-bit quad-word made from the two specified 32-bit double-words.</returns>
    public static SqlInt64 MakeQuadWord(SqlInt32 high, SqlInt32 low)
    {
        if (high.IsNull)
            throw new ArgumentNullException("high");

        if (low.IsNull)
            throw new ArgumentNullException("low");

        return (long)Word.MakeQuadWord((uint)high.Value, (uint)low.Value);
    }
}
