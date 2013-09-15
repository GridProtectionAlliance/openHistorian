//******************************************************************************************************
//  QueryFilterPointId_UIntHashSet.cs - Gbtc
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

using System.Collections.Generic;
using System.Linq;
using GSF.IO;

namespace openHistorian
{
    /// <summary>
    /// A class that is used to filter point results based on the PointID number.
    /// </summary>
    public abstract partial class QueryFilterPointId
    {
        /// <summary>
        /// A filter that can fit in a 32bit value that is in a HashSet.
        /// </summary>
        private class UIntHashSet
            : QueryFilterPointId
        {
            private readonly HashSet<uint> m_points;

            /// <summary>
            /// Creates a filter from the stream.
            /// </summary>
            /// <param name="stream">The the stream to load from.</param>
            /// <param name="pointCount">the number of points in the stream.</param>
            public UIntHashSet(BinaryStreamBase stream, int pointCount)
            {
                m_points = new HashSet<uint>();
                while (pointCount > 0)
                {
                    m_points.Add(stream.ReadUInt32());
                    pointCount--;
                }
            }

            /// <summary>
            /// Creates a filter from the stream.
            /// </summary>
            /// <param name="points">the points to use.</param>
            public UIntHashSet(IEnumerable<ulong> points)
            {
                m_points = new HashSet<uint>();
                m_points.UnionWith(points.Select(x => (uint)x));
            }


            /// <summary>
            /// Determines if a pointID is contained in the filter
            /// </summary>
            /// <param name="pointID">the point to check for.</param>
            /// <returns></returns>
            public override bool ContainsPointID(ulong pointID)
            {
                return pointID <= uint.MaxValue && m_points.Contains((uint)pointID);
            }

            /// <summary>
            /// Serializes the filter to a stream
            /// </summary>
            /// <param name="stream">the stream to write to</param>
            protected override void WriteToStream(BinaryStreamBase stream)
            {
                foreach (uint x in m_points)
                {
                    stream.Write(x);
                }
            }

            /// <summary>
            /// The number of points in this filter. Used to serialize to the disk.
            /// </summary>
            protected override int Count
            {
                get
                {
                    return m_points.Count;
                }
            }
        }

    }
}