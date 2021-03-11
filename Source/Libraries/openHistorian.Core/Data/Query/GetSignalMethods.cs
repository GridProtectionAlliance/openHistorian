//******************************************************************************************************
//  GetSignalMethods.cs - Gbtc
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

using System.Collections.Generic;
using System.Linq;
using GSF.Snap;
using GSF.Snap.Services.Reader;
using GSF.Snap.Filters;
using openHistorian.Data.Types;
using openHistorian.Snap;

namespace openHistorian.Data.Query
{
    /// <summary>
    /// Queries a historian database for a set of signals. 
    /// </summary>
    public static class GetSignalMethods
    {
        /// <summary>
        /// Queries all of the signals at the given time.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="time">the time to query</param>
        /// <returns></returns>
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong time)
        {
            return database.GetSignals(time, time);
        }

        /// <summary>
        /// Queries all of the signals within a the provided time window [Inclusive]
        /// </summary>
        /// <param name="database"></param>
        /// <param name="startTime">the lower bound of the time</param>
        /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
        /// <returns></returns>
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue hvalue = new HistorianValue();
            Dictionary<ulong, SignalDataBase> results = new Dictionary<ulong, SignalDataBase>();

            TreeStream<HistorianKey, HistorianValue> stream = database.Read(startTime, endTime);
            ulong time, point, quality, value;
            while (stream.Read(key, hvalue))
            {
                time = key.Timestamp;
                point = key.PointID;
                _ = hvalue.Value3;
                value = hvalue.Value1;
                results.AddSignal(time, point, value);
            }
            foreach (SignalDataBase signal in results.Values)
                signal.Completed();
            return results;
        }

        /// <summary>
        /// Queries the provided signals within a the provided time window [Inclusive]
        /// </summary>
        /// <param name="database"></param>
        /// <param name="startTime">the lower bound of the time</param>
        /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
        /// <param name="signals">an IEnumerable of all of the signals to query as part of the results set.</param>
        /// <returns></returns>
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ulong> signals)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue hvalue = new HistorianValue();
            Dictionary<ulong, SignalDataBase> results = signals.ToDictionary((x) => x, (x) => (SignalDataBase)new SignalDataUnknown());

            TreeStream<HistorianKey, HistorianValue> stream = database.Read(startTime, endTime, signals);
            ulong time, point, quality, value;
            while (stream.Read(key, hvalue))
            {
                time = key.Timestamp;
                point = key.PointID;
                quality = hvalue.Value3;
                value = hvalue.Value1;
                results.AddSignalIfExists(time, point, value);
            }
            foreach (SignalDataBase signal in results.Values)
                signal.Completed();
            return results;
        }

        /// <summary>
        /// Queries the provided signals within a the provided time window [Inclusive]
        /// This method will strong type the signals, but all signals must be of the same type for this to work.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="startTime">the lower bound of the time</param>
        /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
        /// <param name="signals">an IEnumerable of all of the signals to query as part of the results set.</param>
        /// <param name="conversion">a single conversion method to use for all signals</param>
        /// <returns></returns>
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ulong> signals, TypeBase conversion)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue hvalue = new HistorianValue();
            Dictionary<ulong, SignalDataBase> results = signals.ToDictionary((x) => x, (x) => (SignalDataBase)new SignalData(conversion));

            TreeStream<HistorianKey, HistorianValue> stream = database.Read(startTime, endTime, signals);
            ulong time, point, quality, value;
            while (stream.Read(key, hvalue))
            {
                time = key.Timestamp;
                point = key.PointID;
                quality = hvalue.Value3;
                value = hvalue.Value1;
                results.AddSignalIfExists(time, point, value);
            }
            foreach (SignalDataBase signal in results.Values)
                signal.Completed();
            return results;
        }

        /// <summary>
        /// Queries the provided signals within a the provided time window [Inclusive].
        /// With this method, the signals will be strong typed and therefore can be converted.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="startTime">the lower bound of the time</param>
        /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
        /// <param name="signals">an IEnumerable of all of the signals to query as part of the results set.</param>
        /// <returns></returns>
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ISignalWithType> signals)
        {
            return database.GetSignals(TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime), signals, SortedTreeEngineReaderOptions.Default);
        }

        /// <summary>
        /// Queries the provided signals within a the time described by the <see cref="QueryFilterTimestamp"/>.
        /// With this method, the signals will be strong typed and therefore can be converted.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="timestamps">a <see cref="QueryFilterTimestamp"/> that describes how a signal will be parsed</param>
        /// <param name="signals">an IEnumerable of all of the signals to query as part of the results set.</param>
        /// <param name="readerOptions">The options that will be used when querying this data.</param>
        /// <returns></returns>
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, IEnumerable<ISignalWithType> signals, SortedTreeEngineReaderOptions readerOptions)
        {
            Dictionary<ulong, SignalDataBase> results = new Dictionary<ulong, SignalDataBase>();

            foreach (ISignalWithType pt in signals)
            {
                if (pt.HistorianId.HasValue)
                {
                    if (!results.ContainsKey(pt.HistorianId.Value))
                    {
                        results.Add(pt.HistorianId.Value, new SignalData(pt.Functions));
                    }
                }
            }

            HistorianKey key = new HistorianKey();
            HistorianValue hvalue = new HistorianValue();
            MatchFilterBase<HistorianKey, HistorianValue> keyParser = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(signals.Where((x) => x.HistorianId.HasValue).Select((x) => x.HistorianId.Value));
            TreeStream<HistorianKey, HistorianValue> stream = database.Read(readerOptions, timestamps, keyParser);
            ulong time, point, quality, value;
            while (stream.Read(key, hvalue))
            {
                time = key.Timestamp;
                point = key.PointID;
                quality = hvalue.Value3;
                value = hvalue.Value1;
                results.AddSignalIfExists(time, point, value);
            }

            foreach (SignalDataBase signal in results.Values)
            {
                signal.Completed();
            }
            return results;
        }

        /// <summary>
        /// Adds the following signal to the dictionary. If the signal is
        /// not part of the dictionary, it is added automatically.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="time"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        private static void AddSignal(this Dictionary<ulong, SignalDataBase> results, ulong time, ulong point, ulong value)
        {
            if (!results.TryGetValue(point, out SignalDataBase signalData))
            {
                signalData = new SignalDataUnknown();
                results.Add(point, signalData);
            }
            signalData.AddDataRaw(time, value);
        }

        /// <summary>
        /// Adds the provided signal to the dictionary unless the signal is not
        /// already part of the dictionary.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="time"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        private static void AddSignalIfExists(this Dictionary<ulong, SignalDataBase> results, ulong time, ulong point, ulong value)
        {
            if (results.TryGetValue(point, out SignalDataBase signalData))
                signalData.AddDataRaw(time, value);
        }
    }
}