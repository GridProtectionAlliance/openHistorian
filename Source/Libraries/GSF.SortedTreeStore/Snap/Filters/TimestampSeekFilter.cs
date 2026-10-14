//******************************************************************************************************
//  TimestampSeekFilter.cs - Gbtc
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
//  11/09/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Data;
using System.Runtime.CompilerServices;
using GSF.IO;
using GSF.Snap.Types;

namespace GSF.Snap.Filters;

public partial class TimestampSeekFilter
{
    /// <summary>
    /// Creates a nearest-sample filter over a set of date ranges
    /// </summary>
    /// <param name="firstTime">the first time if the query (inclusive)</param>
    /// <param name="lastTime">the last time of the query (inclusive)</param>
    /// <param name="interval">the interval between samples</param>
    /// <param name="pointIDs">the point IDs to include in the filter</param>
    /// <param name="maximumDistance">the optional maximum distance to search for the nearest key</param>
    /// <param name="searchMode">the mode to use when searching for the nearest key</param>
    /// <returns>A seek filter to find the nearest key for the specified interval data.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the interval is less than or equal to zero or the maximumDistance is less than zero.
    /// </exception>
    public static SeekFilterBase<TKey> CreateNearestFromIntervalData<TKey>(DateTime firstTime, DateTime lastTime,
        TimeSpan interval, System.Collections.Generic.IEnumerable<ulong> pointIDs, TimeSpan? maximumDistance = null,
        TimestampSearchMode searchMode = TimestampSearchMode.Nearest)
        where TKey : TimestampPointIDBase<TKey>, new()
    {
        if (interval.Ticks <= 0)
            throw new ArgumentOutOfRangeException(nameof(interval));
        
        if (maximumDistance is { Ticks: < 0 })
            throw new ArgumentOutOfRangeException(nameof(maximumDistance));

        return new NearestTimestampSeekFilter<TKey>(
            (ulong)firstTime.Ticks,
            (ulong)lastTime.Ticks,
            (ulong)interval.Ticks,
            pointIDs,
            maximumDistance is null ? null : (ulong?)maximumDistance.Value.Ticks,
            searchMode);
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
    /// Creates a filter over a set of date ranges (Similar to down sampled queries)
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
    /// Creates a filter over a set of date ranges (Similar to down sampled queries)
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
    private static SeekFilterBase<TKey> CreateFromStream<TKey>(BinaryStreamBase stream)
        where TKey : TimestampPointIDBase<TKey>, new()
    {
        byte version = stream.ReadUInt8();
        
        return version switch
        {
            0 => null,
            1 => new FixedRange<TKey>(stream),
            2 => new IntervalRanges<TKey>(stream),
            _ => throw new VersionNotFoundException("Unknown Version")
        };
    }
}