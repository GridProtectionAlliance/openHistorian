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
using GSF.Collections.Generic;
using openHistorian.Collections;
using openHistorian.Collections.Generic;
using openHistorian.Data.Types;

namespace openHistorian.Data.Query
{
    public class FrameData
    {
        public FrameData()
        {
            Points = new SortedList<ulong, HistorianValueStruct>();
        }
        public FrameData(List<ulong> pointId, List<HistorianValueStruct> values)
        {
            Points = SortedListConstructor.Create(pointId, values);
        }

        public SortedList<ulong, HistorianValueStruct> Points;
    }

    /// <summary>
    /// Queries a historian database for a set of signals. 
    /// </summary>
    public static class GetFrameMethods
    {
        class FrameDataConstructor
        {
            public List<ulong> PointId = new List<ulong>();
            public List<HistorianValueStruct> Values = new List<HistorianValueStruct>();
            public FrameData ToFrameData()
            {
                return new FrameData(PointId,Values);
            }
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this HistorianDatabaseBase<HistorianKey, HistorianValue> database, DateTime timestamp)
        {
            return database.GetFrames(QueryFilterTimestamp.CreateFromRange(timestamp, timestamp), QueryFilterPointId.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this HistorianDatabaseBase<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime)
        {
            return database.GetFrames(QueryFilterTimestamp.CreateFromRange(startTime, stopTime), QueryFilterPointId.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this HistorianDatabaseBase<HistorianKey, HistorianValue> database, QueryFilterTimestamp timestamps, params ulong[] points)
        {
            return database.GetFrames(timestamps, QueryFilterPointId.CreateFromList(points), DataReaderOptions.Default);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this HistorianDatabaseBase<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime, params ulong[] points)
        {
            return database.GetFrames(QueryFilterTimestamp.CreateFromRange(startTime, stopTime), QueryFilterPointId.CreateFromList(points), DataReaderOptions.Default);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this HistorianDatabaseBase<HistorianKey, HistorianValue> database)
        {
            return database.GetFrames(QueryFilterTimestamp.CreateAllKeysValid(), QueryFilterPointId.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <param name="timestamps">the timestamps to query for</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this HistorianDatabaseBase<HistorianKey, HistorianValue> database, QueryFilterTimestamp timestamps)
        {
            return database.GetFrames(timestamps, QueryFilterPointId.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <param name="timestamps">the timestamps to query for</param>
        /// <param name="points">the points to query</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this HistorianDatabaseBase<HistorianKey, HistorianValue> database, QueryFilterTimestamp timestamps, QueryFilterPointId points)
        {
            return database.GetFrames(timestamps, points, DataReaderOptions.Default);
        }

        ///// <summary>
        ///// Gets frames from the historian as individual frames.
        ///// </summary>
        ///// <param name="database">the database to use</param>
        ///// <param name="timestamps">the timestamps to query for</param>
        ///// <param name="points">the points to query</param>
        ///// <param name="options">A list of query options</param>
        ///// <returns></returns>
        //public static SortedList<DateTime, FrameData> GetFrames(this IHistorianDatabase<HistorianKey, HistorianValue> database, QueryFilterTimestamp timestamps, QueryFilterPointId points, DataReaderOptions options)
        //{
        //    SortedList<DateTime, FrameData> results = new SortedList<DateTime, FrameData>();
        //    using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
        //    {
        //        ulong lastTime = ulong.MinValue;
        //        FrameData lastFrame = null;
        //        TreeStream<HistorianKey, HistorianValue> stream = reader.Read(timestamps, points, options);
        //        while (stream.Read())
        //        {
        //            if (lastFrame == null || stream.CurrentKey.Timestamp != lastTime)
        //            {
        //                lastTime = stream.CurrentKey.Timestamp;
        //                DateTime timestamp = new DateTime((long)lastTime);

        //                if (!results.TryGetValue(timestamp, out lastFrame))
        //                {
        //                    lastFrame = new FrameData();
        //                    results.Add(timestamp, lastFrame);
        //                }
        //            }
        //            lastFrame.Points.Add(stream.CurrentKey.PointID, stream.CurrentValue.ToStruct());
        //        }
        //    }
        //    return results;
        //}

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <param name="timestamps">the timestamps to query for</param>
        /// <param name="points">the points to query</param>
        /// <param name="options">A list of query options</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this HistorianDatabaseBase<HistorianKey, HistorianValue> database, QueryFilterTimestamp timestamps, QueryFilterPointId points, DataReaderOptions options)
        {
            SortedList<DateTime, FrameDataConstructor> results = new SortedList<DateTime, FrameDataConstructor>();
            using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
            {
                ulong lastTime = ulong.MinValue;
                FrameDataConstructor lastFrame = null;
                KeyValueStream<HistorianKey, HistorianValue> stream = reader.Read(timestamps, points, options);
                while (stream.Read())
                {
                    if (lastFrame == null || stream.CurrentKey.Timestamp != lastTime)
                    {
                        lastTime = stream.CurrentKey.Timestamp;
                        DateTime timestamp = new DateTime((long)lastTime);

                        if (!results.TryGetValue(timestamp, out lastFrame))
                        {
                            lastFrame = new FrameDataConstructor();
                            results.Add(timestamp, lastFrame);
                        }
                    }
                    lastFrame.PointId.Add(stream.CurrentKey.PointID);
                    lastFrame.Values.Add(stream.CurrentValue.ToStruct());
                }
            }
            List<FrameData> data = new List<FrameData>(results.Count);
            data.AddRange(results.Values.Select(x => x.ToFrameData()));
            return SortedListConstructor.Create(results.Keys, data);
        }

        /// <summary>
        /// Rounds the frame to the nearest level of specified tolerance.
        /// </summary>
        /// <param name="origional">the frame to round</param>
        /// <param name="toleranceMilliseconds">the timespan in milliseconds.</param>
        /// <returns>A new frame that is rounded.</returns>
        public static SortedList<DateTime, FrameData> RoundToTolerance(this SortedList<DateTime, FrameData> origional, int toleranceMilliseconds)
        {
            return origional.RoundToTolerance(new TimeSpan(TimeSpan.TicksPerMillisecond * toleranceMilliseconds));
        }

        /// <summary>
        /// Rounds the frame to the nearest level of specified tolerance.
        /// </summary>
        /// <param name="origional">the frame to round</param>
        /// <param name="tolerance">the timespan to round on.</param>
        /// <returns>A new frame that is rounded.</returns>
        public static SortedList<DateTime, FrameData> RoundToTolerance(this SortedList<DateTime, FrameData> origional, TimeSpan tolerance)
        {
            SortedList<DateTime, FrameData> results = new SortedList<DateTime, FrameData>();

            SortedList<DateTime, List<FrameData>> buckets = new SortedList<DateTime, List<FrameData>>();

            foreach (var items in origional)
            {
                DateTime roundedDate = items.Key.Round(tolerance);
                List<FrameData> frames;
                if (!buckets.TryGetValue(roundedDate, out frames))
                {
                    frames = new List<FrameData>();
                    buckets.Add(roundedDate, frames);
                }
                frames.Add(items.Value);
            }

            foreach (var bucket in buckets)
            {
                if (bucket.Value.Count == 1)
                {
                    results.Add(bucket.Key, bucket.Value[0]);
                }
                else
                {
                    int count = bucket.Value.Sum(x => x.Points.Count);
                    List<ulong> keys = new List<ulong>(count);
                    List<HistorianValueStruct> values = new List<HistorianValueStruct>(count);

                    FrameData tempFrame = new FrameData();
                    tempFrame.Points = new SortedList<ulong, HistorianValueStruct>();

                    var allFrames = new List<EnumerableHelper>();

                    foreach (var frame in bucket.Value)
                    {
                        allFrames.Add(new EnumerableHelper(frame));
                    }

                    while (true)
                    {
                        EnumerableHelper lowestKey = null;

                        foreach (var item in allFrames)
                            lowestKey = Min(lowestKey, item);

                        if (lowestKey == null)
                            break;

                        keys.Add(lowestKey.PointId);
                        values.Add(lowestKey.Value);

                        //tempFrame.Points.Add(lowestKey.PointId, lowestKey.Value);
                        lowestKey.Read();
                    }
                    tempFrame.Points = SortedListConstructor.Create(keys, values);
                    results.Add(bucket.Key, tempFrame);
                }
            }
            return results;
        }

        static DateTime Round(this DateTime origional, TimeSpan tolerance)
        {
            long delta = (origional.Ticks % tolerance.Ticks);
            if (delta >= (tolerance.Ticks >> 1))
                return new DateTime(origional.Ticks - delta + tolerance.Ticks);
            return new DateTime(origional.Ticks - delta);
        }

        static EnumerableHelper Min(EnumerableHelper left, EnumerableHelper right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;
            if (!left.IsValid && !right.IsValid)
                return null;
            if (!left.IsValid)
                return right;
            if (!right.IsValid)
                return left;
            if (left.PointId < right.PointId)
                return left;
            return right;
        }

        class EnumerableHelper
        {
            public bool IsValid;
            public ulong PointId;
            public HistorianValueStruct Value;
            IEnumerator<KeyValuePair<ulong, HistorianValueStruct>> m_enumerator;
            public EnumerableHelper(FrameData frame)
            {
                m_enumerator = frame.Points.GetEnumerator();
                IsValid = true;
                Read();
            }

            public void Read()
            {
                if (IsValid && m_enumerator.MoveNext())
                {
                    IsValid = true;
                    PointId = m_enumerator.Current.Key;
                    Value = m_enumerator.Current.Value;
                }
                else
                    IsValid = false;
            }

        }

    }
}