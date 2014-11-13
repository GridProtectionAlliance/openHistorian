//******************************************************************************************************
//  PointIdMatchFilter.cs - Gbtc
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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GSF.IO;
using GSF.Snap.Definitions;
using GSF.Snap.Encoding;
using GSF.Snap.Types;

namespace GSF.Snap.Filters
{
    public partial class PointIdMatchFilter
        : MatchFilterBaseDefinition
    {
        // {2034A3E3-F92E-4749-9306-B04DC36FD743}
        public static Guid FilterGuid = new Guid(0x2034a3e3, 0xf92e, 0x4749, 0x93, 0x06, 0xb0, 0x4d, 0xc3, 0x6f, 0xd7, 0x43);

        public override Guid FilterType
        {
            get
            {
                return FilterGuid;
            }
        }

        public override MatchFilterBase<TKey, TValue> Create<TKey, TValue>(BinaryStreamBase stream)
        {
            MethodInfo method = typeof(PointIdMatchFilter).GetMethod("CreateFromStream", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = method.MakeGenericMethod(typeof(TKey), typeof(TValue));
            var rv = generic.Invoke(this, new[] { stream });
            return (MatchFilterBase<TKey, TValue>)rv;
        }

        /// <summary>
        /// Creates a filter from the list of points provided.
        /// </summary>
        /// <param name="listOfPointIDs">contains the list of pointIDs to include in the filter. List must support multiple enumerations</param>
        /// <returns></returns>
        public static MatchFilterBase<TKey, TValue> CreateFromList<TKey, TValue>(IEnumerable<ulong> listOfPointIDs)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            MatchFilterBase<TKey, TValue> filter;
            ulong maxValue = 0;
            if (listOfPointIDs.Any())
                maxValue = listOfPointIDs.Max();

            if (maxValue < 8 * 1024 * 64) //64KB of space, 524288
            {
                filter = new BitArrayFilter<TKey, TValue>(listOfPointIDs, maxValue);
            }
            else if (maxValue <= uint.MaxValue)
            {
                filter = new UIntHashSet<TKey, TValue>(listOfPointIDs, maxValue);
            }
            else
            {
                filter = new ULongHashSet<TKey, TValue>(listOfPointIDs, maxValue);
            }
            return filter;
        }

        /// <summary>
        /// Loads a <see cref="QueryFilterPointId"/> from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to load the filter from</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        MatchFilterBase<TKey, TValue> CreateFromStream<TKey, TValue>(BinaryStreamBase stream)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            MatchFilterBase<TKey, TValue> filter;
            byte version = stream.ReadUInt8();
            ulong maxValue;
            int count;
            switch (version)
            {
                case 0:
                    return null;
                case 1:
                    maxValue = stream.ReadUInt64();
                    count = stream.ReadInt32();
                    if (maxValue < 8 * 1024 * 64) //64KB of space, 524288
                    {
                        filter = new BitArrayFilter<TKey, TValue>(stream, count, maxValue);
                    }
                    else
                    {
                        filter = new UIntHashSet<TKey, TValue>(stream, count, maxValue);
                    }
                    break;
                case 2:
                    maxValue = stream.ReadUInt64();
                    count = stream.ReadInt32();
                    filter = new ULongHashSet<TKey, TValue>(stream, count, maxValue);
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
            return filter;
        }

    }
}
