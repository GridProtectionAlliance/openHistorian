//******************************************************************************************************
//  PointIDFilter.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  11/9/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.IO;
using GSF.SortedTreeStore.Engine;
using openHistorian;

namespace GSF.SortedTreeStore.Filters
{
    public partial class PointIDFilter
    {
        /// <summary>
        /// Creates a filter that is a universe filter that will not filter any points.
        /// </summary>
        /// <returns></returns>
        public static KeyMatchFilterBase<TKey> CreateAllKeysValid<TKey>()
            where TKey : EngineKeyBase<TKey>, new()
        {
            return null;
        }

        /// <summary>
        /// Creates a filter from the list of points provided.
        /// </summary>
        /// <param name="listOfPointIDs">contains the list of pointIDs to include in the filter. List must support multiple enumerations</param>
        /// <returns></returns>
        public static KeyMatchFilterBase<TKey> CreateFromList<TKey>(IEnumerable<ulong> listOfPointIDs)
            where TKey : EngineKeyBase<TKey>, new()

        {
            KeyMatchFilterBase<TKey> filter;
            ulong maxValue = 0;
            if (listOfPointIDs.Any())
                maxValue = listOfPointIDs.Max();

            if (maxValue < 8 * 1024 * 64) //64KB of space, 524288
            {
                filter = new BitArrayFilter<TKey>(listOfPointIDs, maxValue);
            }
            else if (maxValue <= uint.MaxValue)
            {
                filter = new UIntHashSet<TKey>(listOfPointIDs, maxValue);
            }
            else
            {
                filter = new ULongHashSet<TKey>(listOfPointIDs, maxValue);
            }
            return filter;
        }

        /// <summary>
        /// Loads a <see cref="QueryFilterPointId"/> from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to load the filter from</param>
        /// <returns></returns>
        public static KeyMatchFilterBase<TKey> CreateFromStream<TKey>(BinaryStreamBase stream)
            where TKey : EngineKeyBase<TKey>, new()
        {
            KeyMatchFilterBase<TKey> filter;
            byte version = stream.ReadByte();
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
                        filter = new BitArrayFilter<TKey>(stream, count, maxValue);
                    }
                    else
                    {
                        filter = new UIntHashSet<TKey>(stream, count, maxValue);
                    }
                    break;
                case 2:
                    maxValue = stream.ReadUInt64();
                    count = stream.ReadInt32();
                    filter = new ULongHashSet<TKey>(stream, count, maxValue);
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
            return filter;
        }
    }
}
