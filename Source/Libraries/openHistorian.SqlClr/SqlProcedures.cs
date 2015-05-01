//******************************************************************************************************
//  SqlProcedures.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
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
public class SqlProcedures
{
    // No need to make this a structure since it will just get boxed when passed to FillRow function anyway...
    private class HistorianMeasurement
    {
        public readonly ulong ChannelID;
        public readonly DateTime Time;
        public readonly float Value;

        public HistorianMeasurement(ulong channelID, DateTime time, float value)
        {
            ChannelID = channelID;
            Time = time;
            Value = value;
        }
    }

    /// <summary>
    /// Queries data from the openHistorian.
    /// </summary>
    /// <param name="historianServer">Historian server host IP or DNS name. Can be optionally suffixed with port number, e.g.: historian:38402.</param>
    /// <param name="instanceName">Instance name of the historian.</param>
    /// <param name="startTime">Start time of desired data range.</param>
    /// <param name="stopTime">End time of desired data range.</param>
    /// <param name="channelIDs">Comma separated list of channel ID values; set to <c>null</c>to retrieve all points.</param>
    /// <returns>
    /// Enumerable historian data results for specified time range and points.
    /// </returns>
    [SqlFunction(
        DataAccess = DataAccessKind.Read,
        FillRowMethodName = "GetHistorianData_FillRow",
        TableDefinition = "ChannelID bigint, Time datetime2, Value real")
    ]
    public static IEnumerable GetHistorianData(SqlString historianServer, SqlString instanceName, DateTime startTime, DateTime stopTime, SqlString channelIDs)
    {
        const int DefaultHistorianPort = 38402;

        if (historianServer == SqlString.Null || string.IsNullOrEmpty(historianServer.Value))
            throw new ArgumentException("Missing historian server parameter", "historianServer");

        if (instanceName == SqlString.Null || string.IsNullOrEmpty(instanceName.Value))
            throw new ArgumentException("Missing historian instance name parameter", "instanceName");

        if (startTime > stopTime)
            throw new ArgumentException("Invalid time range specified", "startTime");

        string[] parts = historianServer.Value.Split(':');
        string hostName = parts[0];
        int port;

        if (parts.Length < 2 || !int.TryParse(parts[1], out port))
            port = DefaultHistorianPort;

        using (HistorianClient client = new HistorianClient(hostName, port))
        using (ClientDatabaseBase<HistorianKey, HistorianValue> reader = client.GetDatabase<HistorianKey, HistorianValue>(instanceName.Value))
        {
            SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = null;
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            if (channelIDs != SqlString.Null && !string.IsNullOrEmpty(channelIDs.Value))
                pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(channelIDs.Value.Split(',').Select(ulong.Parse));

            // Start stream reader for the provided time window and selected points
            TreeStream<HistorianKey, HistorianValue> stream = reader.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

            while (stream.Read(key, value))
                yield return new HistorianMeasurement(key.PointID, key.TimestampAsDate, value.AsSingle);
        }
    }

    /// <summary>
    /// Used to fill table columns with enumerable data returned from <see cref="GetHistorianData"/>.
    /// </summary>
    /// <param name="source">Source data, i.e., a HistorianMeasurement.</param>
    /// <param name="channelID">Channel ID</param>
    /// <param name="time">Timestamp</param>
    /// <param name="value">Floating point value</param>
    public static void GetHistorianData_FillRow(object source, out SqlInt64 channelID, out DateTime time, out SqlSingle value)
    {
        HistorianMeasurement measurement = source as HistorianMeasurement;

        if ((object)measurement == null)
            throw new InvalidOperationException("FillRow source is not a HistorianMeasurement");

        channelID = (long)measurement.ChannelID;
        time = measurement.Time;
        value = measurement.Value;
    }
}
