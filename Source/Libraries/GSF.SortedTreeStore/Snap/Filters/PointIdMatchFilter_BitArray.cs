//******************************************************************************************************
//  PointIDFilter_BitArray.cs - Gbtc
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
using System.Collections.Generic;
using GSF.Collections;
using GSF.IO;
using GSF.Snap.Types;

namespace GSF.Snap.Filters
{
    public partial class PointIdMatchFilter
    {
        /// <summary>
        /// A filter that uses a <see cref="BitArray"/> to set true and false values
        /// </summary>
        public class BitArrayFilter<TKey, TValue>
            : MatchFilterBase<TKey, TValue>
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            private readonly BitArray m_points;

            public ulong MaxValue = ulong.MaxValue;
            public ulong MinValue = ulong.MinValue;

            /// <summary>
            /// Creates a new filter backed by a <see cref="BitArray"/>.
            /// </summary>
            /// <param name="stream">The the stream to load from.</param>
            /// <param name="pointCount">the number of points in the stream.</param>
            /// <param name="maxValue">the maximum value stored in the bit array. Cannot be larger than int.MaxValue-1</param>
            public BitArrayFilter(BinaryStreamBase stream, int pointCount, ulong maxValue)
            {
                if (maxValue >= int.MaxValue)
                    throw new ArgumentOutOfRangeException("maxValue", "Cannot be larger than int.MaxValue-1");

                MaxValue = maxValue;
                m_points = new BitArray(false, (int)maxValue + 1);
                while (pointCount > 0)
                {
                    //Since a bitarray cannot have more than 32bit 
                    m_points.SetBit((int)stream.ReadUInt32());
                    pointCount--;
                }

                foreach (int point in m_points.GetAllSetBits())
                {
                    MinValue = (ulong)point;
                    break;
                }
            }

            /// <summary>
            /// Creates a bit array filter from <see cref="points"/>
            /// </summary>
            /// <param name="points">the points to use.</param>
            /// <param name="maxValue">the maximum value stored in the bit array. Cannot be larger than int.MaxValue-1</param>
            public BitArrayFilter(IEnumerable<ulong> points, ulong maxValue)
            {
                MaxValue = maxValue;
                m_points = new BitArray(false, (int)maxValue + 1);
                foreach (ulong pt in points)
                {
                    m_points.SetBit((int)pt);
                }

                foreach (int point in m_points.GetAllSetBits())
                {
                    MinValue = (ulong)point;
                    break;
                }

            }

            public override Guid FilterType => PointIdMatchFilterDefinition.FilterGuid;

            public override void Save(BinaryStreamBase stream)
            {
                stream.Write((byte)1); //Stored as array of uint[]
                stream.Write(MaxValue);
                stream.Write(m_points.SetCount);
                foreach (int x in m_points.GetAllSetBits())
                {
                    stream.Write((uint)x);
                }
            }

            public override bool Contains(TKey key, TValue value)
            {
                int point = (int)key.PointID;
                return key.PointID <= MaxValue && m_points.GetBitUnchecked(point);
            }
        }
    }
}
