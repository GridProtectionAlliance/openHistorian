//******************************************************************************************************
//  TimestampSeekFilter_FixedRange.cs - Gbtc
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
using GSF.IO;
using GSF.Snap.Types;

namespace GSF.Snap.Filters
{
    public partial class TimestampSeekFilter
    {
        private class FixedRange<TKey>
            : SeekFilterBase<TKey>
            where TKey : TimestampBase<TKey>, new()
        {
            private bool m_isEndReached;

            private readonly ulong m_start;
            private readonly ulong m_stop;

            private FixedRange()
            {
                StartOfFrame = new TKey();
                EndOfFrame = new TKey();
                StartOfRange = StartOfFrame;
                EndOfRange = EndOfFrame;
            }

            /// <summary>
            /// Creates a filter by reading from the stream.
            /// </summary>
            /// <param name="stream">the stream to read from</param>
            public FixedRange(BinaryStreamBase stream)
                : this()
            {
                m_start = stream.ReadUInt64();
                m_stop = stream.ReadUInt64();
                StartOfRange.SetMin();
                StartOfRange.Timestamp = m_start;
                EndOfRange.SetMax();
                EndOfRange.Timestamp = m_stop;
            }

            /// <summary>
            /// Creates a filter from the boundary
            /// </summary>
            /// <param name="firstTime">the start of the only window.</param>
            /// <param name="lastTime">the stop of the only window.</param>
            public FixedRange(ulong firstTime, ulong lastTime)
                : this()
            {
                m_start = firstTime;
                m_stop = lastTime;
                StartOfRange.SetMin();
                StartOfRange.Timestamp = m_start;
                EndOfRange.SetMax();
                EndOfRange.Timestamp = m_stop;
            }

            /// <summary>
            /// Gets the next search window.
            /// </summary>
            /// <returns>true if window exists, false if finished.</returns>
            public override bool NextWindow()
            {
                if (m_isEndReached)
                {
                    return false;
                }
                StartOfRange.SetMin();
                StartOfRange.Timestamp = m_start;
                EndOfRange.SetMax();
                EndOfRange.Timestamp = m_stop;
                m_isEndReached = true;
                return true;
            }

            /// <summary>
            /// Resets the iterative nature of the filter. 
            /// </summary>
            /// <remarks>
            /// Since a time filter is a set of date ranges, this will reset the frame so a
            /// call to <see cref="NextWindow"/> will return the first window of the sequence.
            /// </remarks>
            public override void Reset()
            {
                m_isEndReached = false;
                StartOfRange.SetMin();
                StartOfRange.Timestamp = m_start;
                EndOfRange.SetMax();
                EndOfRange.Timestamp = m_stop;
            }

            /// <summary>
            /// Serializes the filter to a stream
            /// </summary>
            /// <param name="stream">the stream to write to</param>
            public override void Save(BinaryStreamBase stream)
            {
                stream.Write((byte)1); //stored as start/stop
                stream.Write(m_start);
                stream.Write(m_stop);
            }

            public override Guid FilterType => TimestampSeekFilterDefinition.FilterGuid;
        }

    }
}
