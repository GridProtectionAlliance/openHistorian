//******************************************************************************************************
//  TimestampSeekFilter_IntervalRanges.cs - Gbtc
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
        private class IntervalRanges<TKey>
            : SeekFilterBase<TKey>
            where TKey : TimestampBase<TKey>, new()
        {

            private ulong m_start;
            private ulong m_current;
            private ulong m_mainInterval;
            private ulong m_subInterval;
            private uint m_count;
            private uint m_subIntervalPerMainInterval;
            private ulong m_tolerance;
            private ulong m_stop;

            private IntervalRanges()
            {
                StartOfFrame = new TKey();
                EndOfFrame = new TKey();
                StartOfRange = new TKey();
                EndOfRange = new TKey();
            }

            /// <summary>
            /// Creates a filter by reading from the stream.
            /// </summary>
            /// <param name="stream">the stream to read from</param>
            public IntervalRanges(BinaryStreamBase stream)
                : this()
            {
                ulong start = stream.ReadUInt64();
                ulong stop = stream.ReadUInt64();
                ulong mainInterval = stream.ReadUInt64();
                ulong subInterval = stream.ReadUInt64();
                ulong tolerance = stream.ReadUInt64();
                Initialize(start, stop, mainInterval, subInterval, tolerance);
            }

            /// <summary>
            /// Creates a filter over a set of date ranges (Similiar to downsampled queries)
            /// </summary>
            /// <param name="firstTime">the first time if the query (inclusive)</param>
            /// <param name="lastTime">the last time of the query (inclusive if contained in the intervals)</param>
            /// <param name="mainInterval">the smallest interval that is exact</param>
            /// <param name="subInterval">the interval that will be parsed. Possible to be rounded</param>
            /// <param name="tolerance">the width of every window</param>
            /// <returns>A <see cref="KeySeekFilterBase{TKey}"/> that will be able to do this parsing</returns>
            /// <remarks>
            /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
            ///               MainInterval = 0.1 seconds. SubInterval = 0.0333333 seconds.
            ///               Tolerance = 0.001 seconds.
            /// </remarks>
            public IntervalRanges(ulong firstTime, ulong lastTime, ulong mainInterval, ulong subInterval, ulong tolerance)
                : this()
            {
                Initialize(firstTime, lastTime, mainInterval, subInterval, tolerance);
            }

            private void Initialize(ulong start, ulong stop, ulong mainInterval, ulong subInterval, ulong tolerance)
            {
                if (start > stop)
                    throw new ArgumentOutOfRangeException("start", "start must be before stop");
                if (mainInterval < subInterval)
                    throw new ArgumentOutOfRangeException("mainInterval", "must be larger than the subinterval");
                if (tolerance > subInterval)
                    throw new ArgumentOutOfRangeException("tolerance", "must be smaller than the subinterval");

                m_start = start;
                m_stop = stop;

                StartOfRange.SetMin();
                StartOfRange.Timestamp = m_start;
                EndOfRange.SetMax();
                EndOfRange.Timestamp = m_stop;

                m_current = start;
                m_mainInterval = mainInterval;
                m_subInterval = subInterval;
                m_subIntervalPerMainInterval = (uint)Math.Round(mainInterval / (double)subInterval);
                m_tolerance = tolerance;
                m_count = 0;
            }

            /// <summary>
            /// Gets the next search window.
            /// </summary>
            /// <returns>true if window exists, false if finished.</returns>
            public override bool NextWindow()
            {
                checked
                {
                    ulong middle = m_current + m_subInterval * m_count;
                    startOfWindow = middle - m_tolerance;
                    endOfWindow = middle + m_tolerance;

                    if (startOfWindow > m_stop)
                    {
                        startOfWindow = 0;
                        endOfWindow = 0;
                        return false;
                    }

                    if (m_count + 1 == m_subIntervalPerMainInterval)
                    {
                        m_current += m_mainInterval;
                        m_count = 0;
                    }
                    else
                    {
                        m_count += 1;
                    }
                    return true;
                }
            }

            private ulong startOfWindow 
            {
                get => StartOfFrame.Timestamp;
                set
                {
                    StartOfFrame.SetMin();
                    StartOfFrame.Timestamp = value;
                }
            }

            private ulong endOfWindow
            {
                get => EndOfFrame.Timestamp;
                set
                {
                    EndOfFrame.SetMax();
                    EndOfFrame.Timestamp = value;
                }
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
                m_current = m_start;
                m_count = 0;
            }

            /// <summary>
            /// Serializes the filter to a stream
            /// </summary>
            /// <param name="stream">the stream to write to</param>
            public override void Save(BinaryStreamBase stream)
            {
                stream.Write((byte)2); //Stored with interval data
                stream.Write(m_start);
                stream.Write(m_stop);
                stream.Write(m_mainInterval);
                stream.Write(m_subInterval);
                stream.Write(m_tolerance);
            }

            public override Guid FilterType => TimestampSeekFilterDefinition.FilterGuid;
        }

    }
}
