//******************************************************************************************************
//  NearestTimestampReader.cs - Gbtc
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
using System.Linq;
using System.Threading;
using GSF.Snap.Filters;

namespace GSF.Snap.Services.Reader;

// Uses the existing reader's snapshot, match filter, timeout and cancellation lifetime.
internal sealed class NearestTimestampReader<TKey, TValue>
    where TKey : SnapTypeBase<TKey>, new()
    where TValue : SnapTypeBase<TValue>, new()
{
    private sealed class Candidate
    {
        public readonly TKey Key = new();
        public readonly TValue Value = new();
        public bool Found;
        public ulong Distance;
    }

    private readonly NearestTimestampSeekFilter<TKey> m_filter;
    private readonly SeekFilterBase<TKey> m_bounds;
    private readonly List<BufferedArchiveStream<TKey, TValue>> m_tables;
    private readonly MatchFilterBase<TKey, TValue> m_match;
    private readonly Func<bool> m_cancelled;
    private readonly SortedTreeEngineReaderOptions m_options;
    private readonly Dictionary<ulong, Candidate> m_candidates;
    private readonly Dictionary<BufferedArchiveStream<TKey, TValue>, Dictionary<ulong, Candidate>> m_tableCandidates;
    private Dictionary<ulong, Candidate> m_activeCandidates;
    private readonly Dictionary<ulong, Candidate> m_windowCandidates = new();
    private readonly Queue<Candidate> m_results = new();
    private readonly TKey m_target = new();
    private readonly TKey m_lower = new();
    private readonly TKey m_upper = new();
    private long m_scanned;
    private long m_seeks;
    private long m_returned;
    private bool m_stopped;
    private int m_missing;
    private ulong m_farthest;
    private readonly Func<TKey, TValue, bool> m_forwardVisitor;
    private readonly Func<TKey, TValue, bool> m_backwardVisitor;

    internal NearestTimestampReader(NearestTimestampSeekFilter<TKey> filter, SeekFilterBase<TKey> bounds,
        List<BufferedArchiveStream<TKey, TValue>> tables, MatchFilterBase<TKey, TValue> match,
        SortedTreeEngineReaderOptions options, Func<bool> cancelled)
    {
        m_filter = filter;
        m_bounds = bounds;
        m_tables = tables;
        m_match = match;
        m_options = options;
        m_cancelled = cancelled;
        m_candidates = filter.PointIDs.ToDictionary(id => id, id => new Candidate());
        m_tableCandidates = tables.ToDictionary(table => table, loadTableCandidates);
        m_forwardVisitor = (key, value) => Visit(key, value, false);
        m_backwardVisitor = (key, value) => Visit(key, value, true);
        m_bounds.Reset();
        return;

        Dictionary<ulong, Candidate> loadTableCandidates(BufferedArchiveStream<TKey, TValue> table)
        {
            return m_candidates.Where(pair => !options.UsePointIDIndex || table.MayContainPointID(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }

    internal bool Read(TKey key, TValue value)
    {
        if (m_tables.Count == 0 || m_stopped || m_cancelled() || (m_options.MaxReturnedCount > 0 && m_returned >= m_options.MaxReturnedCount))
            return false;

        while (m_results.Count == 0)
        {
            if (!m_bounds.NextWindow() || m_cancelled())
                return false;

            m_bounds.StartOfFrame.CopyTo(m_lower);
            m_bounds.EndOfFrame.CopyTo(m_upper);
                
            if (m_lower.IsLessThan(m_bounds.StartOfRange))
                m_bounds.StartOfRange.CopyTo(m_lower);
                
            if (m_upper.IsGreaterThan(m_bounds.EndOfRange))
                m_bounds.EndOfRange.CopyTo(m_upper);
                
            if (m_lower.IsGreaterThan(m_upper))
                continue;

            m_missing = m_candidates.Count;
                
            foreach (Candidate candidate in m_candidates.Values)
                candidate.Found = false;
                
            NearestTimestampSeekFilter<TKey>.SetTimestamp(m_target, m_filter.TargetTimestamp, false);

            foreach (BufferedArchiveStream<TKey, TValue> table in m_tables)
            {
                m_activeCandidates = m_tableCandidates[table];
                    
                if (m_activeCandidates.Count == 0 || !table.ContainsRange(m_lower, m_upper))
                    continue;
                    
                if (m_options.UseTimeBucketBloomFilter && table.HasTimeBucketBloomIndex)
                {
                    m_windowCandidates.Clear();
                        
                    ulong lowerTime = NearestTimestampSeekFilter<TKey>.TimeKey(m_lower).Timestamp;
                    ulong upperTime = NearestTimestampSeekFilter<TKey>.TimeKey(m_upper).Timestamp;
                        
                    foreach (KeyValuePair<ulong, Candidate> pair in m_activeCandidates)
                    {
                        if (m_cancelled())
                            return false;
                            
                        if (table.MayContainPointID(pair.Key, lowerTime, upperTime))
                            m_windowCandidates.Add(pair.Key, pair.Value);
                    }

                    m_activeCandidates = m_windowCandidates;
                        
                    if (m_activeCandidates.Count == 0)
                        continue;
                }

                m_missing = 0;
                m_farthest = 0;
                    
                foreach (Candidate candidate in m_activeCandidates.Values)
                {
                    if (!candidate.Found)
                        m_missing++;
                    else if (candidate.Distance > m_farthest)
                        m_farthest = candidate.Distance;
                }

                if (!CanSeek())
                    return false;
                    
                table.Scanner.VisitForward(m_target.IsLessThan(m_lower) ? m_lower : m_target,
                    m_forwardVisitor);
                    
                if (m_stopped || m_cancelled())
                    return false;
                    
                // An exact match for every requested ID cannot be improved by a preceding
                // timestamp. Continue checking other archives forwards for same-time key ties.
                if (m_filter.SearchMode == TimestampSearchMode.AtOrAfter || (m_missing == 0 && m_farthest == 0))
                    continue;
                    
                if (!CanSeek())
                    return false;
                    
                table.Scanner.VisitBackward(m_target, m_backwardVisitor);
                    
                if (m_stopped || m_cancelled())
                    return false;
            }
                
            foreach (Candidate candidate in m_candidates.Values.Where(c => c.Found).OrderBy(c => c.Key, Comparer<TKey>.Create((a, b) => a.CompareTo(b))))
                m_results.Enqueue(candidate);
        }

            
        Candidate result = m_results.Dequeue();
            
        result.Key.CopyTo(key);
        result.Value.CopyTo(value);
            
        m_returned++;
            
        return true;
    }

    private bool CanSeek()
    {
        if (m_cancelled() || (m_options.MaxSeekCount > 0 && m_seeks >= m_options.MaxSeekCount))
        {
            m_stopped = true;
            return false;
        }
            
        m_seeks++;
            
        Interlocked.Increment(ref Stats.SeeksRequested);
            
        return true;
    }

    private bool Visit(TKey key, TValue value, bool backwards)
    {
        if (m_cancelled() || (m_options.MaxScanCount > 0 && m_scanned >= m_options.MaxScanCount))
        {
            m_stopped = true;
            return false;
        }
            
        m_scanned++;
            
        Interlocked.Increment(ref Stats.PointsScanned);
            
        if (backwards ? key.IsLessThan(m_lower) : key.IsGreaterThan(m_upper))
            return false;
            
        if (key.IsLessThan(m_lower) || key.IsGreaterThan(m_upper))
            return true;

        ulong timestamp = NearestTimestampSeekFilter<TKey>.TimeKey(key).Timestamp;
        ulong target = m_filter.TargetTimestamp;
        ulong distance = timestamp >= target ? timestamp - target : target - timestamp;
            
        // Once every ID has a better candidate than any remaining timestamp, the rest
        // of this direction (potentially hours of dense data) is skipped.
        if (m_missing == 0 && distance > m_farthest)
            return false;
            
        ulong idValue = NearestTimestampSeekFilter<TKey>.TimeKey(key).PointID;

        if (!m_activeCandidates.TryGetValue(idValue, out Candidate candidate) || (candidate.Found && distance > candidate.Distance) || !m_match.Contains(key, value))
            return true;

        if (candidate.Found && distance >= candidate.Distance && (distance != candidate.Distance || !key.IsLessThan(candidate.Key)))
            return true;
            
        bool refreshFarthest = !candidate.Found || candidate.Distance == m_farthest;
            
        if (!candidate.Found)
            m_missing--;
            
        key.CopyTo(candidate.Key);
        value.CopyTo(candidate.Value);
            
        candidate.Distance = distance;
        candidate.Found = true;
            
        if (m_missing == 0 && refreshFarthest)
            m_farthest = m_activeCandidates.Values.Max(c => c.Distance);
            
        return true;
    }
}