//******************************************************************************************************
//  GetSignalMethods.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Linq;
using System.Collections.Generic;
using openHistorian.Data.Types;

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
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IHistorianDatabase database, ulong time)
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
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IHistorianDatabase database, ulong startTime, ulong endTime)
        {
            var results = new Dictionary<ulong, SignalDataBase>();

            using (var reader = database.OpenDataReader())
            {
                var stream = reader.Read(startTime, endTime);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    results.AddSignal(time, point, value);
                }
            }
            foreach (var signal in results.Values)
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
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IHistorianDatabase database, ulong startTime, ulong endTime, IEnumerable<ulong> signals)
        {
            var results = signals.ToDictionary((x) => x, (x) => (SignalDataBase)new SignalDataUnknown());

            using (var reader = database.OpenDataReader())
            {
                var stream = reader.Read(startTime, endTime, signals);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    results.AddSignalIfExists(time, point, value);
                }
            }
            foreach (var signal in results.Values)
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
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IHistorianDatabase database, ulong startTime, ulong endTime, IEnumerable<ulong> signals, TypeBase conversion)
        {
            var results = signals.ToDictionary((x) => x, (x) => (SignalDataBase)new SignalData(conversion));

            using (var reader = database.OpenDataReader())
            {
                var stream = reader.Read(startTime, endTime, signals);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    results.AddSignalIfExists(time, point, value);
                }
            }
            foreach (var signal in results.Values)
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
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IHistorianDatabase database, ulong startTime, ulong endTime, IEnumerable<ISignalWithType> signals)
        {
            return database.GetSignals(KeyParserPrimary.CreateFromRange(startTime, endTime), signals, DataReaderOptions.Default);
        }

        /// <summary>
        /// Queries the provided signals within a the time described by the <see cref="KeyParserPrimary"/>.
        /// With this method, the signals will be strong typed and therefore can be converted.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="timestamps">a <see cref="KeyParserPrimary"/> that describes how a signal will be parsed</param>
        /// <param name="signals">an IEnumerable of all of the signals to query as part of the results set.</param>
        /// <param name="readerOptions">The options that will be used when querying this data.</param>
        /// <returns></returns>
        public static Dictionary<ulong, SignalDataBase> GetSignals(this IHistorianDatabase database, KeyParserPrimary timestamps, IEnumerable<ISignalWithType> signals, DataReaderOptions readerOptions)
        {
            var results = new Dictionary<ulong, SignalDataBase>();

            foreach (var point in signals)
            {
                if (point.HistorianId.HasValue)
                {
                    if (!results.ContainsKey(point.HistorianId.Value))
                    {
                        results.Add(point.HistorianId.Value, new SignalData(point.Functions));
                    }
                }
            }

            using (var reader = database.OpenDataReader())
            {
                var keyParser = KeyParserSecondary.CreateFromList(signals.Where((x) => x.HistorianId.HasValue).Select((x) => x.HistorianId.Value));
                var stream = reader.Read(timestamps, keyParser, readerOptions);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    results.AddSignalIfExists(time, point, value);
                }
            }

            foreach (var signal in results.Values)
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
        static void AddSignal(this Dictionary<ulong, SignalDataBase> results, ulong time, ulong point, ulong value)
        {
            SignalDataBase signalData;
            if (!results.TryGetValue(point, out signalData))
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
        static void AddSignalIfExists(this Dictionary<ulong, SignalDataBase> results, ulong time, ulong point, ulong value)
        {
            SignalDataBase signalData;
            if (results.TryGetValue(point, out signalData))
                signalData.AddDataRaw(time, value);
        }

    }

}
