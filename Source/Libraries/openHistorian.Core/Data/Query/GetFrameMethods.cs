//******************************************************************************************************
//  GetFrameMethods.cs - Gbtc
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
//  8/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using openHistorian.Collections;
using openHistorian.Collections.Generic;
using openHistorian.Data.Types;

namespace openHistorian.Data.Query
{
    public class FrameData
    {
        public SortedList<ulong, HistorianValueStruct> Points = new SortedList<ulong, HistorianValueStruct>();
    }

    /// <summary>
    /// Queries a historian database for a set of signals. 
    /// </summary>
    public static class GetFrameMethods
    {
        /// <summary>
        /// Queries all of the signals at the given time.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="time">the time to query</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this IHistorianDatabase<HistorianKey, HistorianValue> database, ulong time)
        {
            return database.GetFrames(time, time);
        }

        /// <summary>
        /// Queries all of the signals within a the provided time window [Inclusive]
        /// </summary>
        /// <param name="database"></param>
        /// <param name="startTime">the lower bound of the time</param>
        /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this IHistorianDatabase<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime)
        {
            SortedList<DateTime, FrameData> results = new SortedList<DateTime, FrameData>();

            using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
            {
                TreeStream<HistorianKey, HistorianValue> stream = reader.Read(startTime, endTime);
                while (stream.Read())
                {
                    DateTime timestamp = new DateTime((long)stream.CurrentKey.Timestamp);
                    FrameData frame;
                    if (!results.TryGetValue(timestamp, out frame))
                    {
                        frame = new FrameData();
                        results.Add(timestamp, frame);
                    }
                    frame.Points.Add(stream.CurrentKey.PointID, stream.CurrentValue.ToStruct());
                }
            }
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
        public static SortedList<DateTime, FrameData> GetFrames(this IHistorianDatabase<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ulong> signals)
        {
            SortedList<DateTime,FrameData> results = new SortedList<DateTime, FrameData>();

            using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
            {
                TreeStream<HistorianKey, HistorianValue> stream = reader.Read(startTime, endTime, signals);
                while (stream.Read())
                {
                    DateTime timestamp = new DateTime((long)stream.CurrentKey.Timestamp);
                    FrameData frame;
                    if (!results.TryGetValue(timestamp, out frame))
                    {
                        frame = new FrameData();
                        results.Add(timestamp,frame);
                    }
                    frame.Points.Add(stream.CurrentKey.PointID,stream.CurrentValue.ToStruct());
                }
            }
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
        public static SortedList<DateTime, FrameData> GetFrames(this IHistorianDatabase<HistorianKey, HistorianValue> database, QueryFilterTimestamp timestamps, IEnumerable<ulong> signals)
        {
            SortedList<DateTime, FrameData> results = new SortedList<DateTime, FrameData>();

            using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
            {
                TreeStream<HistorianKey, HistorianValue> stream = reader.Read(timestamps, signals);
                while (stream.Read())
                {
                    DateTime timestamp = new DateTime((long)stream.CurrentKey.Timestamp);
                    FrameData frame;
                    if (!results.TryGetValue(timestamp, out frame))
                    {
                        frame = new FrameData();
                        results.Add(timestamp, frame);
                    }
                    frame.Points.Add(stream.CurrentKey.PointID, stream.CurrentValue.ToStruct());
                }
            }
            return results;
        }
        
    }
}