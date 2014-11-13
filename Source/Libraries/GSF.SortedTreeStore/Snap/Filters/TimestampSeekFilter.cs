//******************************************************************************************************
//  TimestampSeekFilter.cs - Gbtc
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
//  11/09/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using GSF.IO;
using GSF.Snap.Types;

namespace GSF.Snap.Filters
{
    public partial class TimestampSeekFilter
        : CreateSeekFilterBase
    {

        // {0F0F9478-DC42-4EEF-9F26-231A942EF1FA}
        public static Guid FilterGuid = new Guid(0x0f0f9478, 0xdc42, 0x4eef, 0x9f, 0x26, 0x23, 0x1a, 0x94, 0x2e, 0xf1, 0xfa);

        public override Guid FilterType
        {
            get
            {
                return FilterGuid;
            }
        }

        public override SeekFilterBase<TKey> Create<TKey>(BinaryStreamBase stream)
        {
            MethodInfo method = typeof(TimestampSeekFilter).GetMethod("CreateFromStream", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = method.MakeGenericMethod(typeof(TKey));
            var rv = generic.Invoke(this, new[] { stream });
            return (SeekFilterBase<TKey>)rv;
        }

        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <returns></returns>
        public static SeekFilterBase<TKey> CreateFromRange<TKey>(DateTime firstTime, DateTime lastTime)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            return new FixedRange<TKey>((ulong)firstTime.Ticks, (ulong)lastTime.Ticks);
        }

        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <returns></returns>
        public static SeekFilterBase<TKey> CreateFromRange<TKey>(ulong firstTime, ulong lastTime)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            return new FixedRange<TKey>(firstTime, lastTime);
        }

        /// <summary>
        /// Creates a filter over a set of date ranges (Similiar to downsampled queries)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive if contained in the intervals)</param>
        /// <param name="mainInterval">the smallest interval that is exact</param>
        /// <param name="subInterval">the interval that will be parsed. Possible to be rounded</param>
        /// <param name="tolerance">the width of every window</param>
        /// <returns>A <see cref="KeySeekFilterBase{TKey}"/> that will be able to do this parsing</returns>
        /// <remarks>
        /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
        ///               MainInterval = 0.1 seconds. SubInterval = 0.0333333 seconds.
        ///               Tolerance = 0.001 seconds.
        /// </remarks>
        public static SeekFilterBase<TKey> CreateFromIntervalData<TKey>(ulong firstTime, ulong lastTime, ulong mainInterval, ulong subInterval, ulong tolerance)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            return new IntervalRanges<TKey>(firstTime, lastTime, mainInterval, subInterval, tolerance);
        }

        /// <summary>
        /// Creates a filter over a set of date ranges (Similiar to downsampled queries)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive if contained in the intervals)</param>
        /// <param name="interval">the exact interval</param>
        /// <param name="tolerance">the width of every window</param>
        /// <returns>A <see cref="KeySeekFilterBase{TKey}"/> that will be able to do this parsing</returns>
        /// <remarks>
        /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
        ///               MainInterval = 0.1 seconds. SubInterval = 0.0333333 seconds.
        ///               Tolerance = 0.001 seconds.
        /// </remarks>
        public static SeekFilterBase<TKey> CreateFromIntervalData<TKey>(ulong firstTime, ulong lastTime, ulong interval, ulong tolerance)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            return new IntervalRanges<TKey>(firstTime, lastTime, interval, interval, tolerance);
        }

        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <param name="mainInterval">the smallest interval that is exact</param>
        /// <param name="subInterval">the interval that will be parsed. Possible to be rounded</param>
        /// <param name="tolerance">the width of every window</param>
        /// <returns>A <see cref="KeySeekFilterBase{TKey}"/> that will be able to do this parsing</returns>
        /// <remarks>
        /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
        ///               MainInterval = 0.1 seconds. SubInterval = 0.0333333 seconds.
        ///               Tolerance = 0.001 seconds.
        /// </remarks>
        public static SeekFilterBase<TKey> CreateFromIntervalData<TKey>(DateTime firstTime, DateTime lastTime, TimeSpan mainInterval, TimeSpan subInterval, TimeSpan tolerance)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            return new IntervalRanges<TKey>((ulong)firstTime.Ticks, (ulong)lastTime.Ticks, (ulong)mainInterval.Ticks, (ulong)subInterval.Ticks, (ulong)tolerance.Ticks);
        }

        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <param name="interval">the exact interval to do the scan</param>
        /// <param name="tolerance">the width of every window</param>
        /// <returns>A <see cref="KeySeekFilterBase{TKey}"/> that will be able to do this parsing</returns>
        /// <remarks>
        /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
        ///               Interval = 0.1 seconds.
        ///               Tolerance = 0.001 seconds.
        /// </remarks>
        public static SeekFilterBase<TKey> CreateFromIntervalData<TKey>(DateTime firstTime, DateTime lastTime, TimeSpan interval, TimeSpan tolerance)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            return new IntervalRanges<TKey>((ulong)firstTime.Ticks, (ulong)lastTime.Ticks, (ulong)interval.Ticks, (ulong)interval.Ticks, (ulong)tolerance.Ticks);
        }

        /// <summary>
        /// Loads a <see cref="KeySeekFilterBase{TKey}"/> from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to load the filter from</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        SeekFilterBase<TKey> CreateFromStream<TKey>(BinaryStreamBase stream)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            byte version = stream.ReadUInt8();
            switch (version)
            {
                case 0:
                    return null;
                case 1:
                    return new FixedRange<TKey>(stream);
                case 2:
                    return new IntervalRanges<TKey>(stream);
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
        }


    }
}
