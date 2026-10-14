//******************************************************************************************************
//  TimestampSearchMode.cs - Gbtc
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

namespace GSF.Snap.Filters;

/// <summary>Controls which side of a target timestamp is searched for a sample.</summary>
public enum TimestampSearchMode : byte
{
    /// <summary>Search both sides and choose the closest eligible sample, preferring the earlier timestamp on ties.</summary>
    Nearest = 0,

    /// <summary>Choose the first eligible sample at or after the target; never return a preceding sample.</summary>
    AtOrAfter = 1
}