//******************************************************************************************************
//  NearestTimestampSeekFilter.cs - Gbtc
//
//  Copyright © 2026, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/21/2026 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GSF.IO;
using GSF.Snap.Types;

namespace GSF.Snap.Filters;

/// <summary>
/// Selects the nearest eligible sample per point ID around regularly spaced target timestamps.
/// Requires timestamp-first key ordering, then point ID (as used by HistorianKey).
/// </summary>
/// <remarks>
/// Targets start at firstTime and include lastTime when it falls on the grid. Samples must lie
/// within the query range and maximumDistance of the target. Equal distances prefer the earlier
/// timestamp, then the smaller key. Original keys and values are returned without interpolation.
/// Windows cannot overlap except at their midpoint, which belongs to the earlier target.
/// Consequently, results are sequential and a source sample is never returned twice. AtOrAfter
/// mode instead selects the first eligible sample at or after each target, using the same
/// distance limit. Missing IDs produce no result. Both endpoints must support the selected mode.
/// </remarks>
public sealed class NearestTimestampSeekFilter<TKey> : SeekFilterBase<TKey> where TKey : SnapTypeBase<TKey>, new()
{
    private readonly ulong m_first;
    private readonly ulong m_firstTarget;
    private readonly bool m_singleTarget;
    private readonly ulong m_last;
    private readonly ulong m_interval;
    private readonly ulong m_distance;
    private readonly ulong[] m_pointIDs;
    private bool m_started;
    private bool m_finished;

    /// <summary>
    /// Creates a nearest-sample query. All time quantities are in timestamp ticks.
    /// </summary>
    /// <param name="firstTime">Inclusive query start and first target.</param>
    /// <param name="lastTime">Inclusive query end.</param>
    /// <param name="interval">Positive target spacing.</param>
    /// <param name="pointIDs">Explicit set of IDs to sample.</param>
    /// <param name="maximumDistance">Search radius, at most half the interval; defaults to half.</param>
    /// <param name="searchMode">Search both sides, or only at/after each target using the same distance limit.</param>
    public NearestTimestampSeekFilter(ulong firstTime, ulong lastTime, ulong interval,
        IEnumerable<ulong> pointIDs, ulong? maximumDistance = null, TimestampSearchMode searchMode = TimestampSearchMode.Nearest)
    {
        if (new TKey() is not TimestampPointIDBase<TKey>)
            throw new NotSupportedException("Nearest sampling requires timestamp/point-ID keys.");

        if (firstTime > lastTime)
            throw new ArgumentOutOfRangeException(nameof(lastTime));

        if (interval == 0)
            throw new ArgumentOutOfRangeException(nameof(interval));

        if (pointIDs == null)
            throw new ArgumentNullException(nameof(pointIDs));

        ValidateSearchMode(searchMode);

        SearchMode = searchMode;
        m_first = firstTime;
        m_firstTarget = firstTime;
        m_last = lastTime;
        m_interval = interval;
        m_distance = maximumDistance ?? interval / 2;

        if (m_distance > interval / 2)
            throw new ArgumentOutOfRangeException(nameof(maximumDistance), "Search windows must not overlap.");

        m_pointIDs = pointIDs.Distinct().OrderBy(id => id).ToArray();
        StartOfRange = new TKey();
        EndOfRange = new TKey();
        StartOfFrame = new TKey();
        EndOfFrame = new TKey();

        SetTimestamp(StartOfRange, firstTime, false);
        SetTimestamp(EndOfRange, lastTime, true);

        Reset();
    }

    /// <summary>
    /// Finds one nearest sample for an ID, searching on both sides of a timestamp.
    /// </summary>
    /// <param name="timestamp">Target timestamp.</param>
    /// <param name="pointID">ID to find.</param>
    /// <param name="maximumDistance">Maximum distance in timestamp ticks, inclusive.</param>
    /// <param name="searchMode">Search both sides, or only at/after the target.</param>
    public NearestTimestampSeekFilter(ulong timestamp, ulong pointID, ulong maximumDistance, TimestampSearchMode searchMode = TimestampSearchMode.Nearest)
        : this(searchMode == TimestampSearchMode.AtOrAfter ? timestamp : timestamp - Math.Min(timestamp, maximumDistance),
            timestamp + Math.Min(ulong.MaxValue - timestamp, maximumDistance),
            ulong.MaxValue, [ pointID ], 0, searchMode)
    {
        m_distance = maximumDistance;
        m_firstTarget = timestamp;
        m_singleTarget = true;
        Reset();
    }

    /// <inheritdoc/>
    public override Guid FilterType => NearestTimestampSeekFilterDefinition.FilterGuid;

    /// <summary>The current target timestamp after NextWindow succeeds.</summary>
    public ulong TargetTimestamp { get; private set; }

    /// <summary>Gets whether samples are selected on both sides or only at/after the target.</summary>
    public TimestampSearchMode SearchMode { get; }

    internal IEnumerable<ulong> PointIDs => m_pointIDs;

    internal static TimestampPointIDBase<TKey> TimeKey(TKey key) => (TimestampPointIDBase<TKey>)(object)key;

    internal static void SetTimestamp(TKey key, ulong timestamp, bool maximum)
    {
        if (maximum)
            key.SetMax();
        else
            key.SetMin();

        TimeKey(key).Timestamp = timestamp;
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        m_started = false;
        m_finished = false;
        TargetTimestamp = m_firstTarget;
        StartOfRange.CopyTo(StartOfFrame);
        EndOfRange.CopyTo(EndOfFrame);
    }

    /// <inheritdoc/>
    public override bool NextWindow()
    {
        if (m_finished || m_pointIDs.Length == 0)
            return false;

        if (m_started)
        {
            if (m_singleTarget || m_last - TargetTimestamp < m_interval)
            {
                m_finished = true;
                return false;
            }

            TargetTimestamp += m_interval;
        }

        ulong lower = SearchMode == TimestampSearchMode.AtOrAfter ? TargetTimestamp : TargetTimestamp - Math.Min(m_distance, TargetTimestamp - m_first);
        ulong upper = TargetTimestamp + Math.Min(m_distance, m_last - TargetTimestamp);

        // When the interval is even, the shared midpoint belongs to the previous target.
        if (SearchMode == TimestampSearchMode.Nearest && m_started && m_distance == m_interval - m_distance)
            lower++;

        SetTimestamp(StartOfFrame, lower, false);
        SetTimestamp(EndOfFrame, upper, true);

        m_started = true;

        return true;
    }

    /// <inheritdoc/>
    public override void Save(BinaryStreamBase stream)
    {
        // Keep the original payload for nearest queries; older readers reject the new version
        // for directional queries rather than silently changing their meaning.
        stream.Write((byte)(SearchMode == TimestampSearchMode.Nearest ? 1 : 2));
        stream.Write((byte)(m_singleTarget ? 1 : 0));

        if (SearchMode != TimestampSearchMode.Nearest)
            stream.Write((byte)SearchMode);

        if (m_singleTarget)
        {
            stream.Write(m_firstTarget);
            stream.Write(m_pointIDs[0]);
            stream.Write(m_distance);
            return;
        }

        stream.Write(m_first);
        stream.Write(m_last);
        stream.Write(m_interval);
        stream.Write(m_distance);
        stream.Write(m_pointIDs.Length);

        foreach (ulong id in m_pointIDs)
            stream.Write(id);
    }

    private static void ValidateSearchMode(TimestampSearchMode searchMode)
    {
        if (searchMode != TimestampSearchMode.Nearest && searchMode != TimestampSearchMode.AtOrAfter)
            throw new ArgumentOutOfRangeException(nameof(searchMode));
    }
    internal static NearestTimestampSeekFilter<TKey> Load(BinaryStreamBase stream)
    {
        byte version = stream.ReadUInt8();

        if (version != 1 && version != 2)
            throw new InvalidDataException("Unknown nearest-timestamp filter version.");

        byte mode = stream.ReadUInt8();
        TimestampSearchMode searchMode = version == 1 ? TimestampSearchMode.Nearest : (TimestampSearchMode)stream.ReadUInt8();

        if (searchMode != TimestampSearchMode.Nearest && searchMode != TimestampSearchMode.AtOrAfter)
            throw new InvalidDataException("Unknown timestamp search mode.");

        if (mode == 1)
            return new NearestTimestampSeekFilter<TKey>(stream.ReadUInt64(), stream.ReadUInt64(), stream.ReadUInt64(), searchMode);

        if (mode != 0)
            throw new InvalidDataException("Unknown nearest-timestamp filter mode.");

        ulong first = stream.ReadUInt64();
        ulong last = stream.ReadUInt64();
        ulong interval = stream.ReadUInt64();
        ulong distance = stream.ReadUInt64();
        int count = stream.ReadInt32();

        if (count < 0)
            throw new InvalidDataException("Negative point-ID count.");

        // Read incrementally: do not allocate an attacker-controlled array before reading payload.
        List<ulong> ids = [];

        for (int i = 0; i < count; i++)
            ids.Add(stream.ReadUInt64());

        return new NearestTimestampSeekFilter<TKey>(first, last, interval, ids, distance, searchMode);
    }
}