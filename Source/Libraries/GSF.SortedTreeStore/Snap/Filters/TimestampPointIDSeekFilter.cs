//******************************************************************************************************
//  TimestampPointIDSeekFilter.cs - Gbtc
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
//  11/26/2014 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;
using GSF.IO;
using GSF.Snap.Types;

namespace GSF.Snap.Filters
{
    /// <summary>
    /// Represents a seek filter for a specific timestamp and point ID.
    /// </summary>
    public static class TimestampPointIDSeekFilter
    {
        /// <summary>
        /// Creates a filter for the specified timestamp and point ID.
        /// </summary>
        /// <param name="timestamp">The specific timestamp to find.</param>
        /// <param name="pointID">The specific point ID to find.</param>
        /// <returns>Seek filter to find specific key.</returns>
        public static SeekFilterBase<TKey> FindKey<TKey>(ulong timestamp, ulong pointID)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            return new SeekToKey<TKey>(timestamp, pointID);
        }

        /// <summary>
        /// Loads a <see cref="SeekFilterBase{TKey}"/> from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to load the filter from</param>
        /// <returns>Seek filter to find specific key.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static SeekFilterBase<TKey> CreateFromStream<TKey>(BinaryStreamBase stream)
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            return new SeekToKey<TKey>(stream);
        }

        private class SeekToKey<TKey>
            : SeekFilterBase<TKey>
            where TKey : TimestampPointIDBase<TKey>, new()
        {
            private readonly TKey m_keyToFind;
            private bool m_isEndReached;

            private SeekToKey()
            {
                m_keyToFind = new TKey();
                StartOfFrame = new TKey();
                EndOfFrame = new TKey();
                StartOfRange = StartOfFrame;
                EndOfRange = EndOfFrame;
            }

            /// <summary>
            /// Creates a filter by reading from the stream.
            /// </summary>
            /// <param name="stream">the stream to read from</param>
            public SeekToKey(BinaryStreamBase stream)
                : this()
            {
                m_keyToFind.Timestamp = stream.ReadUInt64();
                m_keyToFind.PointID = stream.ReadUInt64();
                m_keyToFind.CopyTo(StartOfRange);
                m_keyToFind.CopyTo(EndOfRange);
            }

            /// <summary>
            /// Creates a filter for the key.
            /// </summary>
            /// <param name="timestamp">The specific timestamp to find.</param>
            /// <param name="pointID">The specific point ID to find.</param>
            public SeekToKey(ulong timestamp, ulong pointID)
                : this()
            {
                m_keyToFind.Timestamp = timestamp;
                m_keyToFind.PointID = pointID;
                m_keyToFind.CopyTo(StartOfRange);
                m_keyToFind.CopyTo(EndOfRange);
            }

            /// <summary>
            /// Gets the next search window.
            /// </summary>
            /// <returns>true if window exists, false if finished.</returns>
            public override bool NextWindow()
            {
                if (m_isEndReached)
                    return false;

                m_isEndReached = true;
                return true;
            }

            /// <summary>
            /// Resets the iterative nature of the filter. 
            /// </summary>
            public override void Reset()
            {
                m_isEndReached = false;
            }

            /// <summary>
            /// Serializes the filter to a stream
            /// </summary>
            /// <param name="stream">the stream to write to</param>
            public override void Save(BinaryStreamBase stream)
            {
                stream.Write(m_keyToFind.Timestamp);
                stream.Write(m_keyToFind.PointID);
            }

            public override Guid FilterType => TimestampPointIDSeekFilterDefinition.FilterGuid;
        }
    }
}
