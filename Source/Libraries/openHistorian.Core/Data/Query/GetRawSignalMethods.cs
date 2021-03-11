//******************************************************************************************************
//  GetRawSignalMethods.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using GSF.Snap;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using openHistorian.Snap;

namespace openHistorian.Data.Query
{
    public class RawSignalTimeValue
    {
        public SortedList<DateTime, HistorianValueStruct> Signals = new SortedList<DateTime, HistorianValueStruct>();
    }

    /// <summary>
    /// Queries a historian database for a set of signals. 
    /// </summary>
    public static class GetRawSignalMethods
    {
        /// <summary>
        /// Queries the provided signals within a the provided time window [Inclusive]
        /// </summary>
        /// <param name="database"></param>
        /// <param name="startTime">the lower bound of the time</param>
        /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
        /// <param name="signals">an IEnumerable of all of the signals to query as part of the results set.</param>
        /// <returns></returns>
        public static Dictionary<ulong, RawSignalTimeValue> GetRawSignals(this ClientDatabaseBase<HistorianKey, HistorianValue> database, DateTime startTime, DateTime endTime, IEnumerable<ulong> signals)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            Dictionary<ulong, RawSignalTimeValue> results = signals.ToDictionary((x) => x, (x) => new RawSignalTimeValue());

            TreeStream<HistorianKey, HistorianValue> stream = database.Read((ulong)startTime.Ticks, (ulong)endTime.Ticks, signals);

            while (stream.Read(key, value))
            {
                results[key.PointID].Signals.Add(key.TimestampAsDate, value.ToStruct());
            }
            return results;
        }


    }
}