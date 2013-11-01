//******************************************************************************************************
//  QueryFilterPointId_BitArray.cs - Gbtc
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
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Collections;
using GSF.IO;

namespace openHistorian
{
    /// <summary>
    /// A class that is used to filter point results based on the PointID number.
    /// </summary>
    public abstract partial class QueryFilterPointId
    {
      
        /// <summary>
        /// A filter that uses a <see cref="BitArray"/> to set true and false values
        /// </summary>
        private class BitArrayFilter 
            : QueryFilterPointId
        {
            private readonly BitArray m_points;
            private ulong m_maxValue;

            /// <summary>
            /// Creates a new filter backed by a <see cref="BitArray"/>.
            /// </summary>
            /// <param name="stream">The the stream to load from.</param>
            /// <param name="pointCount">the number of points in the stream.</param>
            /// <param name="maxValue">the maximum value stored in the bit array. Cannot be larger than int.MaxValue-1</param>
            public BitArrayFilter(BinaryStreamBase stream, int pointCount, ulong maxValue)
            {
                if (maxValue >= int.MaxValue)
                    throw new ArgumentOutOfRangeException("maxValue","Cannot be larger than int.MaxValue-1");

                m_maxValue = maxValue;
                m_points = new BitArray(false, (int)maxValue + 1);
                while (pointCount > 0)
                {
                    //Since a bitarray cannot have more than 32bit 
                    m_points.SetBit((int)stream.ReadUInt32()); 
                    pointCount--;
                }
            }

            /// <summary>
            /// Creates a bit array filter from <see cref="points"/>
            /// </summary>
            /// <param name="points">the points to use.</param>
            /// <param name="maxValue">the maximum value stored in the bit array. Cannot be larger than int.MaxValue-1</param>
            public BitArrayFilter(IEnumerable<ulong> points, ulong maxValue)
            {
                m_maxValue = maxValue;
                m_points = new BitArray(false, (int)maxValue + 1);
                foreach (ulong pt in points)
                {
                    m_points.SetBit((int)pt);
                }
            }

            /// <summary>
            /// Determines if a pointID is contained in the filter
            /// </summary>
            /// <param name="pointID">the point to check for.</param>
            /// <returns></returns>
            public override bool ContainsPointID(ulong pointID)
            {
                return pointID <= m_maxValue && m_points[(int)pointID];
            }

            /// <summary>
            /// Serializes the filter to a stream
            /// </summary>
            /// <param name="stream">the stream to write to</param>
            protected override void WriteToStream(BinaryStreamBase stream)
            {
                foreach (int x in m_points.GetAllSetBits())
                {
                    stream.Write((uint)x);
                }
            }

            /// <summary>
            /// The number of points in this filter. Used to serialize to the disk.
            /// </summary>
            protected override int Count
            {
                get
                {
                    return m_points.SetCount;
                }
            }
        }

    }
}