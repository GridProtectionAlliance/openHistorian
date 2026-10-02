//******************************************************************************************************
//  TimeBucketBloomFilterSettings.cs - Gbtc
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
//  09/22/2026 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;

namespace GSF.Snap.Storage;

/// <summary>Immutable options for optional time-bucket PointID Bloom sidecars.</summary>
public sealed class TimeBucketBloomFilterSettings
{
    /// <summary>
    /// Whether finalized archives should receive a Bloom sidecar.
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// Bucket width in seconds, from 1 through 86400.
    /// </summary>
    public int BucketDurationSeconds { get; }

    /// <summary>
    /// Target false-positive probability per bucket, from 0.0001 through 0.25.
    /// </summary>
    public double FalsePositiveProbability { get; }

    /// <summary>
    /// Maximum uncompressed index payload per archive in MiB (1 through 64).
    /// </summary>
    public int MaximumIndexSizeMiB { get; }

    /// <summary>Creates validated settings. Disabled by default; existing indexes remain readable.</summary>
    /// <param name="enabled">Whether new finalized files should receive Bloom indexes.</param>
    /// <param name="bucketDurationSeconds">Time bucket width in seconds, 1 through 86400.</param>
    /// <param name="falsePositiveProbability">Target per-bucket probability, 0.0001 through 0.25.</param>
    /// <param name="maximumIndexSizeMiB">Maximum uncompressed index payload per archive, 1 through 64 MiB.</param>
    public TimeBucketBloomFilterSettings(bool enabled = false, int bucketDurationSeconds = 900, double falsePositiveProbability = 0.01D, int maximumIndexSizeMiB = 64)
    {
        if (bucketDurationSeconds is < 1 or > 86400)
            throw new ArgumentOutOfRangeException(nameof(bucketDurationSeconds));

        if (double.IsNaN(falsePositiveProbability) || falsePositiveProbability < 0.0001D || falsePositiveProbability > 0.25D)
            throw new ArgumentOutOfRangeException(nameof(falsePositiveProbability));
        
        if (maximumIndexSizeMiB is < 1 or > 64)
            throw new ArgumentOutOfRangeException(nameof(maximumIndexSizeMiB));
        
        MaximumIndexSizeMiB = maximumIndexSizeMiB;
        Enabled = enabled;
        BucketDurationSeconds = bucketDurationSeconds;
        FalsePositiveProbability = falsePositiveProbability;
    }
}
