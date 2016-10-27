//******************************************************************************************************
//  ShortTime.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Diagnostics;
using System;

namespace GSF
{
    internal static class ShortTimeFunctions
    {
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;

        private static readonly long Frequency;
        private static readonly bool IsHighResolution;

        private static readonly double TickFrequency;
        private static readonly double MillisecondFrequency;

        private static readonly double SecondsPerCount;
        private static readonly double MillisecondsPerCount;
        private static readonly double MicrosecondsPerCount;
        private static readonly double TicksPerCount;

        private static readonly double CountsPerSecond;
        private static readonly double CountsPerMillisecond;
        private static readonly double CountsPerMicrosecond;
        private static readonly double CountsPerTick;

        static ShortTimeFunctions()
        {
            Frequency = Stopwatch.Frequency;

            decimal countsPerSecond = Frequency;
            CountsPerSecond = (double)countsPerSecond;
            CountsPerMillisecond = (double)(countsPerSecond / 1000);
            CountsPerMicrosecond = (double)(countsPerSecond / 1000000);
            CountsPerTick = (double)(countsPerSecond / 10000000);

            decimal secondsPerCount = (decimal)1 / (decimal)Frequency;
            SecondsPerCount = (double)secondsPerCount;
            MillisecondsPerCount = (double)(secondsPerCount * 1000);
            MicrosecondsPerCount = (double)(secondsPerCount * 1000000);
            TicksPerCount = (double)(secondsPerCount * 10000000);

            IsHighResolution = Stopwatch.IsHighResolution;

        }

        public static long Now()
        {
            return Stopwatch.GetTimestamp();
        }

        public static long AddSeconds(long time, double value)
        {
            return time + (long)(CountsPerSecond * value);
        }

        public static long AddMilliseconds(long time, double value)
        {
            return time + (long)(CountsPerMillisecond * value);
        }

        public static long AddMicroseconds(long time, double value)
        {
            return time + (long)(CountsPerMicrosecond * value);
        }

        public static long AddTicks(long time, double value)
        {
            return time + (long)(CountsPerTick * value);
        }

        public static double ElapsedSeconds(long a, long b)
        {
            return ((b - a) * SecondsPerCount);
        }

        public static double ElapsedMilliseconds(long a, long b)
        {
            return ((b - a) * MillisecondsPerCount);
        }

        public static double ElapsedMicroseconds(long a, long b)
        {
            return ((b - a) * MicrosecondsPerCount);
        }

        public static double ElapsedTicks(long a, long b)
        {
            return ((b - a) * TicksPerCount);
        }
    }

    /// <summary>
    /// Represents a high resolution time that is very granular but may drift 
    /// if trying to acurately measure long time durations (Such as hours). 
    /// This time is not adjusted with changes to the system clock.
    /// Typical clock drifts by about 2-3 ms per minute as apposed to 0.4ms per minute for 
    /// standard DateTime.
    /// </summary>
    /// <remarks>
    /// Call times are about 40+ million calls per second.
    /// </remarks>
    public struct ShortTime
    {
        private readonly long m_time;

        private ShortTime(long time)
        {
            m_time = time;
        }

        /// <summary>
        /// Calculates the approximate <see cref="DateTime"/> represented by this time. 
        /// </summary>
        /// <returns></returns>
        public DateTime UtcTime
        {
            get
            {
                return DateTime.UtcNow - Elapsed();
            }
        }

        public double ElapsedSeconds()
        {
            return ElapsedSeconds(Now);
        }

        public double ElapsedMilliseconds()
        {
            return ElapsedMilliseconds(Now);
        }

        public double ElapsedMicroseconds()
        {
            return ElapsedMicroseconds(Now);
        }

        public double ElapsedTicks()
        {
            return ElapsedTicks(Now);
        }

        /// <summary>
        /// Gets the time that has elapsed since the creation of this time.
        /// </summary>
        /// <returns></returns>
        public TimeSpan Elapsed()
        {
            return Elapsed(Now);
        }

        public double ElapsedSeconds(ShortTime futureTime)
        {
            return ShortTimeFunctions.ElapsedSeconds(m_time, futureTime.m_time);
        }
        public double ElapsedMilliseconds(ShortTime futureTime)
        {
            return ShortTimeFunctions.ElapsedMilliseconds(m_time, futureTime.m_time);
        }
        public double ElapsedMicroseconds(ShortTime futureTime)
        {
            return ShortTimeFunctions.ElapsedMicroseconds(m_time, futureTime.m_time);
        }
        public double ElapsedTicks(ShortTime futureTime)
        {
            return ShortTimeFunctions.ElapsedTicks(m_time, futureTime.m_time);
        }

        public TimeSpan Elapsed(ShortTime futureTime)
        {
            return new TimeSpan((long)ShortTimeFunctions.ElapsedTicks(m_time, futureTime.m_time));
        }

        public ShortTime AddMilliseconds(double duration)
        {
            return new ShortTime(ShortTimeFunctions.AddMilliseconds(m_time, duration));
        }
        public ShortTime AddSeconds(double duration)
        {
            return new ShortTime(ShortTimeFunctions.AddSeconds(m_time, duration));
        }
        public ShortTime AddTicks(double duration)
        {
            return new ShortTime(ShortTimeFunctions.AddTicks(m_time, duration));
        }
        public ShortTime Add(TimeSpan duration)
        {
            return new ShortTime(ShortTimeFunctions.AddTicks(m_time, duration.Ticks));
        }

        public static ShortTime Now
        {
            get
            {
                return new ShortTime(ShortTimeFunctions.Now());
            }
        }

        public static bool operator <(ShortTime a, ShortTime b)
        {
            return a.m_time - b.m_time < 0; //Accounts for overflows.
        }

        public static bool operator >(ShortTime a, ShortTime b)
        {
            return a.m_time - b.m_time > 0; //Accounts for overflows.
        }

        public static bool operator <=(ShortTime a, ShortTime b)
        {
            return a.m_time - b.m_time <= 0; //Accounts for overflows.
        }

        public static bool operator >=(ShortTime a, ShortTime b)
        {
            return a.m_time - b.m_time >= 0; //Accounts for overflows.
        }

        public static TimeSpan operator -(ShortTime a, ShortTime b)
        {
            return b.Elapsed(a);
        }

        public override string ToString()
        {
            return UtcTime.ToString();
        }
    }
}