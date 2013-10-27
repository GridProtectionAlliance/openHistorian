//******************************************************************************************************
//  KeyParserPrimary.cs - Gbtc
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
using GSF.IO;

namespace openHistorian
{
    public abstract partial class QueryFilterTimestamp
    {

        /// <summary>
        /// Creates a filter over a set of date ranges (Similiar to downsampled queries)
        /// </summary>
        private class IntervalRanges 
            : QueryFilterTimestamp
        {
            private ulong m_start;
            private ulong m_current;
            private ulong m_mainInterval;
            private ulong m_subInterval;
            private uint m_count;
            private uint m_subIntervalPerMainInterval;
            private ulong m_tolerance;
            private ulong m_stop;

            /// <summary>
            /// Creates a filter by reading from the stream.
            /// </summary>
            /// <param name="stream">the stream to read from</param>
            public IntervalRanges(BinaryStreamBase stream)
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
            /// <returns>A <see cref="QueryFilterTimestamp"/> that will be able to do this parsing</returns>
            /// <remarks>
            /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
            ///               MainInterval = 0.1 seconds. SubInterval = 0.0333333 seconds.
            ///               Tolerance = 0.001 seconds.
            /// </remarks>
            public IntervalRanges(ulong firstTime, ulong lastTime, ulong mainInterval, ulong subInterval, ulong tolerance)
            {
                Initialize(firstTime, lastTime, mainInterval, subInterval, tolerance);
            }

            private void Initialize(ulong start, ulong stop, ulong mainInterval, ulong subInterval, ulong tolerance)
            {
                if (start > stop)
                    throw new ArgumentOutOfRangeException("start","start must be before stop");
                if (mainInterval < subInterval)
                    throw new ArgumentOutOfRangeException("mainInterval","must be larger than the subinterval");
                if (tolerance >= subInterval)
                    throw new ArgumentOutOfRangeException("tolerance","must be smaller than the subinterval");

                m_start = start;
                m_stop = stop;
                m_current = start;
                m_mainInterval = mainInterval;
                m_subInterval = subInterval;
                m_subIntervalPerMainInterval = (uint)Math.Round((double)mainInterval / (double)subInterval);
                m_tolerance = tolerance;
                m_count = 0;
            }

            /// <summary>
            /// Gets the next search window.
            /// </summary>
            /// <param name="startOfWindow">the start of the window to search</param>
            /// <param name="endOfWindow">the end of the window to search</param>
            /// <returns>true if window exists, false if finished.</returns>
            public override bool GetNextWindow(out ulong startOfWindow, out ulong endOfWindow)
            {
                checked
                {
                    ulong middle = m_current + (m_subInterval * m_count);
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

            /// <summary>
            /// Resets the iterative nature of the filter. 
            /// </summary>
            /// <remarks>
            /// Since a time filter is a set of date ranges, this will reset the frame so a
            /// call to <see cref="GetNextWindow"/> will return the first window of the sequence.
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
            protected override void WriteToStream(BinaryStreamBase stream)
            {
                stream.Write(m_start);
                stream.Write(m_stop);
                stream.Write(m_mainInterval);
                stream.Write(m_subInterval);
                stream.Write(m_tolerance);
            }


            /// <summary>
            /// Gets the first time that might be accessed by this filter.
            /// </summary>
            public override ulong FirstTime
            {
                get
                {
                    return m_start;
                }
            }

            /// <summary>
            /// Gets the last time that might be accessed by this filter.
            /// </summary>
            public override ulong LastTime
            {
                get
                {
                    return m_stop;
                }
            }

        }
    }
}