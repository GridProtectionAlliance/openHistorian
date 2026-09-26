//******************************************************************************************************
//  NearestTimestampSeekFilterDefinition.cs - Gbtc
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
using System.Reflection;
using GSF.IO;
using GSF.Snap.Definitions;

namespace GSF.Snap.Filters;

/// <summary>
/// Wire definition for nearest timestamp sampling.
/// </summary>
public sealed class NearestTimestampSeekFilterDefinition : SeekFilterDefinitionBase
{
    /// <summary>
    /// Stable identifier for the nearest-sample wire format.
    /// </summary>
    public static readonly Guid FilterGuid = new("f906dfb8-692b-4a28-98ed-ff78ae74e4b5");

    /// <inheritdoc/>
    public override Guid FilterType => FilterGuid;

    /// <inheritdoc/>
    public override SeekFilterBase<TKey> Create<TKey>(BinaryStreamBase stream)
    {
        Type filterType = typeof(NearestTimestampSeekFilter<>).MakeGenericType(typeof(TKey));
        
        MethodInfo method = filterType.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic) ?? 
                                throw new InvalidOperationException("Nearest filter loader is missing.");
        
        return (SeekFilterBase<TKey>)(method.Invoke(null, [stream]) ?? 
                   throw new InvalidOperationException("Nearest filter loader returned no filter."));
    }
}