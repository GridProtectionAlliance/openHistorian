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
using System.Data;
using GSF.IO;

namespace openHistorian
{
    public abstract class QueryFilterTimestamp
    {
        public static QueryFilterTimestamp CreateAllKeysValid()
        {
            return new UniverseRange();
        }
        
        public static QueryFilterTimestamp CreateFromRange(DateTime firstTime, DateTime lastTime)
        {
            return new FixedRange((ulong)firstTime.Ticks, (ulong)lastTime.Ticks);
        }

        public static QueryFilterTimestamp CreateFromRange(ulong firstTime, ulong lastTime)
        {
            return new FixedRange(firstTime, lastTime);
        }

        public static QueryFilterTimestamp CreateFromIntervalData(ulong firstTime, ulong lastTime, ulong mainInterval, ulong subInterval, ulong tolerance)
        {
            return new IntervalRanges(firstTime, lastTime, mainInterval, subInterval, tolerance);
        }

        public static QueryFilterTimestamp CreateFromStream(BinaryStreamBase stream)
        {
            byte version = stream.ReadByte();
            switch (version)
            {
                case 0:
                    return new UniverseRange();
                case 1:
                    return new FixedRange(stream);
                case 2:
                    return new IntervalRanges(stream);
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
        }

        public abstract ulong FirstTime
        {
            get;
        }

        public abstract ulong LastTime
        {
            get;
        }

        public abstract void Reset();

        public abstract bool GetNextWindow(out ulong startOfWindow, out ulong endOfWindow);

        public void Save(BinaryStreamBase stream)
        {
            if (this is UniverseRange)
            {
                stream.Write((byte)0); //No data stored
            }
            else if (this is FixedRange)
            {
                stream.Write((byte)1); //stored as start/stop
                WriteToStream(stream);
            }
            else if (this is IntervalRanges)
            {
                stream.Write((byte)2); //Stored with interval data
                WriteToStream(stream);
            }
            else
            {
                throw new NotSupportedException("The provided inherited class cannot be serialized");
            }
        }

        protected abstract void WriteToStream(BinaryStreamBase stream);

        private class UniverseRange : QueryFilterTimestamp
        {
            private bool m_isEndReached;

            public override ulong FirstTime
            {
                get
                {
                    return ulong.MinValue;
                }
            }

            public override ulong LastTime
            {
                get
                {
                    return ulong.MaxValue;
                }
            }

            public override void Reset()
            {
                m_isEndReached = false;
            }

            public override bool GetNextWindow(out ulong startOfWindow, out ulong endOfWindow)
            {
                if (m_isEndReached)
                {
                    startOfWindow = 0;
                    endOfWindow = 0;
                    return false;
                }
                startOfWindow = ulong.MinValue;
                endOfWindow = ulong.MaxValue;
                m_isEndReached = true;
                return true;
            }

            protected override void WriteToStream(BinaryStreamBase stream)
            {
            }
        }

        private class FixedRange : QueryFilterTimestamp
        {
            private bool m_isEndReached;
            private readonly ulong m_start;
            private readonly ulong m_stop;

            public FixedRange(BinaryStreamBase stream)
            {
                m_start = stream.ReadUInt64();
                m_stop = stream.ReadUInt64();
            }

            public FixedRange(ulong firstTime, ulong lastTime)
            {
                m_start = firstTime;
                m_stop = lastTime;
            }

            public override bool GetNextWindow(out ulong startOfWindow, out ulong endOfWindow)
            {
                if (m_isEndReached)
                {
                    startOfWindow = 0;
                    endOfWindow = 0;
                    return false;
                }
                startOfWindow = m_start;
                endOfWindow = m_stop;
                m_isEndReached = true;
                return true;
            }

            public override ulong FirstTime
            {
                get
                {
                    return m_start;
                }
            }

            public override ulong LastTime
            {
                get
                {
                    return m_stop;
                }
            }

            public override void Reset()
            {
                m_isEndReached = false;
            }

            protected override void WriteToStream(BinaryStreamBase stream)
            {
                stream.Write(m_start);
                stream.Write(m_stop);
            }
        }

        private class IntervalRanges : QueryFilterTimestamp
        {
            private ulong m_start;
            private ulong m_current;
            private ulong m_mainInterval;
            private ulong m_subInterval;
            private uint m_count;
            private uint m_subIntervalPerMainInterval;
            private ulong m_tolerance;
            private ulong m_stop;

            public IntervalRanges(BinaryStreamBase stream)
            {
                ulong start = stream.ReadUInt64();
                ulong stop = stream.ReadUInt64();
                ulong mainInterval = stream.ReadUInt64();
                ulong subInterval = stream.ReadUInt64();
                ulong tolerance = stream.ReadUInt64();
                Initialize(start, stop, mainInterval, subInterval, tolerance);
            }

            public IntervalRanges(ulong start, ulong stop, ulong mainInterval, ulong subInterval, ulong tolerance)
            {
                Initialize(start, stop, mainInterval, subInterval, tolerance);
            }

            private void Initialize(ulong start, ulong stop, ulong mainInterval, ulong subInterval, ulong tolerance)
            {
                if (start > stop)
                    throw new Exception();
                if (mainInterval < subInterval)
                    throw new Exception();
                if (tolerance >= subInterval)
                    throw new Exception();

                m_start = start;
                m_stop = stop;
                m_current = start;
                m_mainInterval = mainInterval;
                m_subInterval = subInterval;
                m_subIntervalPerMainInterval = (uint)Math.Round((double)mainInterval / (double)subInterval);
                m_tolerance = tolerance;
                m_count = 0;
            }

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

            public override ulong FirstTime
            {
                get
                {
                    return m_start;
                }
            }

            public override ulong LastTime
            {
                get
                {
                    return m_stop;
                }
            }

            public override void Reset()
            {
                m_current = m_start;
                m_count = 0;
            }

            protected override void WriteToStream(BinaryStreamBase stream)
            {
                stream.Write(m_start);
                stream.Write(m_stop);
                stream.Write(m_mainInterval);
                stream.Write(m_subInterval);
                stream.Write(m_tolerance);
            }
        }
    }
}