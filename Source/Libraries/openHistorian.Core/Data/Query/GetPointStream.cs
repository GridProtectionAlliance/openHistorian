//******************************************************************************************************
//  GetPointStream.cs - Gbtc
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
    /// A helper way to read data from a stream.
    /// </summary>
    [Obsolete("This will soon be removed")]
    public class PointStream
            : TreeStream<HistorianKey, HistorianValue>
    {
        IDatabaseReader<HistorianKey, HistorianValue> m_reader;
        readonly TreeStream<HistorianKey, HistorianValue> m_stream;

        public PointStream(IDatabaseReader<HistorianKey, HistorianValue> reader, TreeStream<HistorianKey, HistorianValue> stream)
        {
            m_stream = stream;
            m_reader = reader;
        }

        public HistorianKey CurrentKey = new HistorianKey();
        public HistorianValue CurrentValue = new HistorianValue();
        public bool IsValid;

        public bool Read()
        {
            return Read(CurrentKey, CurrentValue);
        }

        protected override bool ReadNext(HistorianKey key, HistorianValue value)
        {
            if (m_stream.Read(CurrentKey, CurrentValue))
            {
                IsValid = true;
                return true;
            }
            else
            {
                IsValid = false;
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_reader.Dispose();
                m_reader = null;
            }
        }
    }

    /// <summary>
    /// Queries a historian database for a set of signals. 
    /// </summary>
    public static partial class GetPointStreamExtensionMethods
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
        public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime)
        {
            return database.GetPointStream(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), null);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, params ulong[] points)
        {
            return database.GetPointStream(SortedTreeEngineReaderOptions.Default, timestamps, PointIdMatchFilter.CreateFromList<HistorianKey,HistorianValue>(points));
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <returns></returns>
        public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime, params ulong[] points)
        {
            return database.GetPointStream(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points));
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
        public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
        {
            return database.GetPointStream(SortedTreeEngineReaderOptions.Default, timestamps, points);
        }

        /// <summary>
        /// Gets frames from the historian as individual frames.
        /// </summary>
        /// <param name="database">the database to use</param>
        /// <param name="timestamps">the timestamps to query for</param>
        /// <param name="points">the points to query</param>
        /// <param name="options">A list of query options</param>
        /// <returns></returns>
        public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database,
            SortedTreeEngineReaderOptions options, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
        {
            return new PointStream(database, database.Read(options, timestamps, points));
        }



        class FrameDataConstructor
        {
            public readonly List<ulong> PointId = new List<ulong>();
            public readonly List<HistorianValueStruct> Values = new List<HistorianValueStruct>();
            public FrameData ToFrameData()
            {
                return new FrameData(PointId, Values);
            }
        }

        /// <summary>
        /// Gets concentrated frames from the provided stream
        /// </summary>
        /// <param name="stream">the database to use</param>
        /// <returns></returns>
        public static SortedList<DateTime, FrameData> GetPointStream(this TreeStream<HistorianKey, HistorianValue> stream)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            SortedList<DateTime, FrameDataConstructor> results = new SortedList<DateTime, FrameDataConstructor>();
            ulong lastTime = ulong.MinValue;
            FrameDataConstructor lastFrame = null;
            while (stream.Read(key,value))
            {
                if (lastFrame is null || key.Timestamp != lastTime)
                {
                    lastTime = key.Timestamp;
                    DateTime timestamp = new DateTime((long)lastTime);

                    if (!results.TryGetValue(timestamp, out lastFrame))
                    {
                        lastFrame = new FrameDataConstructor();
                        results.Add(timestamp, lastFrame);
                    }
                }
                lastFrame.PointId.Add(key.PointID);
                lastFrame.Values.Add(value.ToStruct());
            }
            List<FrameData> data = new List<FrameData>(results.Count);
            data.AddRange(results.Values.Select(x => x.ToFrameData()));
            return SortedListConstructor.Create(results.Keys, data);
        }


    }
}