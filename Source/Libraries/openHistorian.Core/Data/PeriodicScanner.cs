//******************************************************************************************************
//  PeriodicScanner.cs - Gbtc
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
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace openHistorian.Data
{
    public class PeriodicScanner
    {
        private TimeSpan m_windowTolerance;

        private readonly List<ulong> m_downSampleRates;

        public PeriodicScanner(int samplesPerSecond)
            : this(samplesPerSecond, new TimeSpan(TimeSpan.TicksPerSecond / samplesPerSecond / 4))
        {
        }

        public PeriodicScanner(int samplesPerSecond, TimeSpan windowTolerance)
        {
            m_windowTolerance = windowTolerance;
            m_downSampleRates = new List<ulong>();
            CalculateDownSampleRates(samplesPerSecond);
        }

        public ulong SuggestSamplesPerDay(DateTime startTime, DateTime endTime, uint sampleCount)
        {
            double days = (endTime - startTime).TotalDays;

            ulong sampleRate = m_downSampleRates.FirstOrDefault((x) => sampleCount <= x * days);
            if (sampleRate == 0)
                sampleRate = m_downSampleRates.Last();

            return sampleRate;
        }

        public QueryFilterTimestamp GetParser(DateTime startTime, DateTime endTime, uint sampleCount)
        {
            return GetParser(startTime, endTime, SuggestSamplesPerDay(startTime, endTime, sampleCount));
        }

        public QueryFilterTimestamp GetParser(DateTime startTime, DateTime endTime, ulong samplesPerDay)
        {
            startTime = RoundDownToNearestSample(startTime, samplesPerDay);
            endTime = RoundUpToNearestSample(endTime, samplesPerDay);

            ulong interval = (TimeSpan.TicksPerDay / samplesPerDay);
            ulong bigInterval;

            uint count = 1;
            while (true)
            {
                if (samplesPerDay % count == 0)
                {
                    if (TimeSpan.TicksPerDay % (samplesPerDay / count) == 0)
                    {
                        bigInterval = TimeSpan.TicksPerDay / (samplesPerDay / count);
                        break;
                    }
                }
                count++;
            }
            return QueryFilterTimestamp.CreateFromIntervalData((ulong)startTime.Ticks, (ulong)endTime.Ticks, bigInterval, interval, (ulong)m_windowTolerance.Ticks);
        }

        private DateTime RoundDownToNearestSample(DateTime startTime, ulong samplesPerDay)
        {
            long interval = (long)(TimeSpan.TicksPerDay / samplesPerDay);
            if (interval * (long)samplesPerDay == TimeSpan.TicksPerDay)
            {
                return new DateTime(startTime.Ticks - startTime.Ticks % interval);
            }
            else
            {
                //Not exact, but close enough. 
                decimal interval2 = (decimal)TimeSpan.TicksPerDay / (decimal)samplesPerDay;

                long dateTicks = startTime.Ticks - startTime.Ticks % TimeSpan.TicksPerDay;
                long timeTicks = startTime.Ticks - dateTicks; //timeticks cannot be more than 864 billion.

                decimal overBy = timeTicks % interval2;

                long timeTicks2 = timeTicks - (long)overBy;

                DateTime rv = new DateTime(dateTicks + timeTicks2);
                return rv;
            }
        }

        private DateTime RoundUpToNearestSample(DateTime startTime, ulong samplesPerDay)
        {
            //ToDo: actually round up
            long interval = (long)(TimeSpan.TicksPerDay / samplesPerDay);
            if (interval * (long)samplesPerDay == TimeSpan.TicksPerDay)
            {
                return new DateTime(startTime.Ticks - startTime.Ticks % interval);
            }
            else
            {
                //Not exact, but close enough. 
                decimal interval2 = (decimal)TimeSpan.TicksPerDay / (decimal)samplesPerDay;

                long dateTicks = startTime.Ticks - startTime.Ticks % TimeSpan.TicksPerDay;
                long timeTicks = startTime.Ticks - dateTicks; //timeticks cannot be more than 864 billion.

                decimal overBy = timeTicks % interval2;

                long timeTicks2 = timeTicks - (long)overBy;

                DateTime rv = new DateTime(dateTicks + timeTicks2);
                return rv;
            }
        }

        private List<int> FactorNumber(int number)
        {
            List<int> factors = new List<int>();
            for (int x = 1; x * x <= number; x++)
            {
                if (number % x == 0)
                {
                    factors.Add(x);
                    if (x * x != number)
                        factors.Add(number / x);
                }
            }
            factors.Sort();
            return factors;
        }

        private void CalculateDownSampleRates(int samplesPerSecond)
        {
            m_downSampleRates.Add(1);
            m_downSampleRates.Add(2);
            m_downSampleRates.Add(3);
            m_downSampleRates.Add(4);
            m_downSampleRates.Add(6);
            m_downSampleRates.Add(8);
            m_downSampleRates.Add(12);
            m_downSampleRates.Add(24);
            m_downSampleRates.Add(24 * 2);
            m_downSampleRates.Add(24 * 3);
            m_downSampleRates.Add(24 * 4);
            m_downSampleRates.Add(24 * 5);
            m_downSampleRates.Add(24 * 6);
            m_downSampleRates.Add(24 * 10);
            m_downSampleRates.Add(24 * 12);
            m_downSampleRates.Add(24 * 15);
            m_downSampleRates.Add(24 * 20);
            m_downSampleRates.Add(24 * 30);
            m_downSampleRates.Add(24 * 60);
            m_downSampleRates.Add(24 * 60 * 2);
            m_downSampleRates.Add(24 * 60 * 3);
            m_downSampleRates.Add(24 * 60 * 4);
            m_downSampleRates.Add(24 * 60 * 5);
            m_downSampleRates.Add(24 * 60 * 6);
            m_downSampleRates.Add(24 * 60 * 10);
            m_downSampleRates.Add(24 * 60 * 12);
            m_downSampleRates.Add(24 * 60 * 15);
            m_downSampleRates.Add(24 * 60 * 20);
            m_downSampleRates.Add(24 * 60 * 30);
            m_downSampleRates.Add(24 * 60 * 60);

            List<int> factors = FactorNumber(samplesPerSecond);
            for (int x = 1; x < factors.Count; x++)
            {
                m_downSampleRates.Add(24uL * 60uL * 60uL * (ulong)factors[x]);
            }
        }
    }
}