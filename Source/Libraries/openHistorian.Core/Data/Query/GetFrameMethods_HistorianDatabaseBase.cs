//******************************************************************************************************
//  GetFrameMethods.cs - Gbtc
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
//  8/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using GSF.Collections;
using GSF.Snap;
using GSF.Snap.Services.Reader;
using GSF.Snap.Filters;
using openHistorian.Snap;

namespace openHistorian.Data.Query
{


    /// <summary>
    /// Queries a historian database for a set of signals. 
    /// </summary>
    public static partial class GetFrameMethods
    {
        ///// <summary>
        ///// Gets frames from the historian as individual frames.
        ///// </summary>
        ///// <param name="database">the database to use</param>
        ///// <returns></returns>
        //public static SortedList<DateTime, FrameData> GetFrames(this SortedTreeEngineBase<HistorianKey, HistorianValue> database, DateTime timestamp)
        //{
        //    return database.GetFrames(SortedTreeEngineReaderOptions.Default, TimestampFilter.CreateFromRange<HistorianKey>(timestamp, timestamp), PointIDFilter.CreateAllKeysValid<HistorianKey>(), null);
        //}

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime)
        {
            return database.GetFrames(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), null);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, params ulong[] points)
        {
            return database.GetFrames(SortedTreeEngineReaderOptions.Default, timestamps, PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points));
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime, params ulong[] points)
        {
            return database.GetFrames(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), PointIdMatchFilter.CreateFromList<HistorianKey,HistorianValue>(points));
        }

        ///// <summary>
        ///// Gets frames from the historian as individual frames.
        ///// </summary>
        ///// <param name="database">the database to use</param>
        ///// <returns></returns>
        //public static SortedList<DateTime, FrameData> GetFrames(this SortedTreeEngineBase<HistorianKey, HistorianValue> database)
        //{
        //    return database.GetFrames(QueryFilterTimestamp.CreateAllKeysValid(), QueryFilterPointId.CreateAllKeysValid(), SortedTreeEngineReaderOptions.Default);
        //}

        ///// <summary>
        ///// Gets frames from the historian as individual frames.
        ///// </summary>
        ///// <param name="database">the database to use</param>
        ///// <param name="timestamps">the timestamps to query for</param>
        ///// <returns></returns>
        //public static SortedList<DateTime, FrameData> GetFrames(this SortedTreeEngineBase<HistorianKey, HistorianValue> database, QueryFilterTimestamp timestamps)
        //{
        //    return database.GetFrames(timestamps, QueryFilterPointId.CreateAllKeysValid(), SortedTreeEngineReaderOptions.Default);
        //}

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <param name="timestamps">the timestamps to query for</param>
        /// <param name="points">the points to query</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
        {
            return database.GetFrames(SortedTreeEngineReaderOptions.Default, timestamps, points);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <param name="timestamps">the timestamps to query for</param>
        /// <param name="points">the points to query</param>
        /// <param name="options">A list of query options</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database,
            SortedTreeEngineReaderOptions options, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey,HistorianValue> points)
        {
            return database.Read(options, timestamps, points).GetFrames();
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

            foreach (KeyValuePair<DateTime, FrameData> items in origional)
            {
                DateTime roundedDate = items.Key.Round(tolerance);
                if (!buckets.TryGetValue(roundedDate, out List<FrameData> frames))
                {
                    frames = new List<FrameData>();
                    buckets.Add(roundedDate, frames);
                }
                frames.Add(items.Value);
            }

            foreach (KeyValuePair<DateTime, List<FrameData>> bucket in buckets)
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

                    List<EnumerableHelper> allFrames = new List<EnumerableHelper>();

                    foreach (FrameData frame in bucket.Value)
                    {
                        allFrames.Add(new EnumerableHelper(frame));
                    }

                    while (true)
                    {
                        EnumerableHelper lowestKey = null;

                        foreach (EnumerableHelper item in allFrames)
                            lowestKey = Min(lowestKey, item);

                        if (lowestKey is null)
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
            long delta = origional.Ticks % tolerance.Ticks;
            if (delta >= tolerance.Ticks >> 1)
                return new DateTime(origional.Ticks - delta + tolerance.Ticks);
            return new DateTime(origional.Ticks - delta);
        }

        static EnumerableHelper Min(EnumerableHelper left, EnumerableHelper right)
        {
            if (left is null)
                return right;
            if (right is null)
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
            readonly IEnumerator<KeyValuePair<ulong, HistorianValueStruct>> m_enumerator;
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