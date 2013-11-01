//******************************************************************************************************
//  KeyParserSecondary.cs - Gbtc
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
using System.Data;
using System.Linq;
using GSF.IO;

namespace openHistorian
{
    /// <summary>
    /// A class that is used to filter point results based on the PointID number.
    /// </summary>
    public abstract partial class QueryFilterPointId
    {
        private ulong m_maxValue;

        /// <summary>
        /// Creates a filter that is a universe filter that will not filter any points.
        /// </summary>
        /// <returns></returns>
        public static QueryFilterPointId CreateAllKeysValid()
        {
            return Universe.Instance;
        }

        /// <summary>
        /// Creates a filter from the list of points provided.
        /// </summary>
        /// <param name="listOfPointIDs">contains the list of pointIDs to include in the filter. List must support multiple enumerations</param>
        /// <returns></returns>
        public static QueryFilterPointId CreateFromList(IEnumerable<ulong> listOfPointIDs)
        {
            QueryFilterPointId filter;
            ulong maxValue = 0;
            if (listOfPointIDs.Any())
                maxValue = listOfPointIDs.Max();

            if (maxValue < 8 * 1024 * 64) //64KB of space, 524288
            {
                filter = new BitArrayFilter(listOfPointIDs, maxValue);
            }
            else if (maxValue <= uint.MaxValue)
            {
                filter = new UIntHashSet(listOfPointIDs);
            }
            else
            {
                filter = new ULongHashSet(listOfPointIDs);
            }
            filter.m_maxValue = maxValue;
            return filter;
        }

        /// <summary>
        /// Loads a <see cref="QueryFilterPointId"/> from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to load the filter from</param>
        /// <returns></returns>
        public static QueryFilterPointId CreateFromStream(BinaryStreamBase stream)
        {
            QueryFilterPointId filter;
            byte version = stream.ReadByte();
            ulong maxValue;
            int count;
            switch (version)
            {
                case 0:
                    return Universe.Instance;
                case 1:
                    maxValue = stream.ReadUInt64();
                    count = stream.ReadInt32();
                    if (maxValue < 8 * 1024 * 64) //64KB of space, 524288
                    {
                        filter = new BitArrayFilter(stream, count, maxValue);
                    }
                    else
                    {
                        filter = new UIntHashSet(stream, count);
                    }
                    break;
                case 2:
                    maxValue = stream.ReadUInt64();
                    count = stream.ReadInt32();
                    filter = new ULongHashSet(stream, count);
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
            filter.m_maxValue = maxValue;
            return filter;
        }

        /// <summary>
        /// Gets if this filter is the universe filter. 
        /// </summary> 
        /// <remarks>
        /// This can be used to reduce function calls as <see cref="ContainsPointID"/> always
        /// returns true on a universe filter.
        /// </remarks>
        public bool IsUniverseFilter
        {
            get
            {
                return (this as Universe) != null;
            }
        }

        /// <summary>
        /// Determines if a pointID is contained in the filter
        /// </summary>
        /// <param name="pointID">the point to check for.</param>
        /// <returns></returns>
        public abstract bool ContainsPointID(ulong pointID);

        /// <summary>
        /// Serializes the filter to a stream
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        protected abstract void WriteToStream(BinaryStreamBase stream);

        /// <summary>
        /// The number of points in this filter. Used to serialize to the disk.
        /// </summary>
        protected abstract int Count
        {
            get;
        }

        /// <summary>
        /// Serializes the filter to a stream
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public void Save(BinaryStreamBase stream)
        {
            if (this is Universe)
            {
                stream.Write((byte)0); //No data stored
            }
            else if (this is BitArrayFilter)
            {
                stream.Write((byte)1); //Stored as array of uint[]
                stream.Write(m_maxValue);
                stream.Write(Count);
                WriteToStream(stream);
            }
            else if (this is UIntHashSet)
            {
                stream.Write((byte)1); //Stored as array of uint[]
                stream.Write(m_maxValue);
                stream.Write(Count);
                WriteToStream(stream);
            }
            else if (this is ULongHashSet)
            {
                stream.Write((byte)2); //Stored as array of ulong[]
                stream.Write(m_maxValue);
                stream.Write(Count);
                WriteToStream(stream);
            }
            else
            {
                throw new NotSupportedException("The provided inherited class cannot be serialized");
            }
        }

    }
}