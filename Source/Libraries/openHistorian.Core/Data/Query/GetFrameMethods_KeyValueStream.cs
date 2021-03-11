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
using openHistorian.Snap;

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
    public static partial class GetFrameMethods
    {
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
        public static SortedList<DateTime, FrameData> GetFrames(this TreeStream<HistorianKey, HistorianValue> stream)
        {
            SortedList<DateTime, FrameDataConstructor> results = new SortedList<DateTime, FrameDataConstructor>();
            ulong lastTime = ulong.MinValue;
            FrameDataConstructor lastFrame = null;
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            while (stream.Read(key, value))
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