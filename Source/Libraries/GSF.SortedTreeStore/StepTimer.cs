//******************************************************************************************************
//  StepTimer.cs - Gbtc
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
//  12/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GSF
{
    public static class StepTimer
    {
        public interface ITimer
        {
            void Stop(int loopCount = 1);
        }

        private class RunCount : ITimer
        {
            public readonly List<double> RunResults = new List<double>();
            public readonly Stopwatch SW = new Stopwatch();

            public void Stop(int loopCount = 1)
            {
                SW.Stop();
                RunResults.Add(SW.Elapsed.TotalSeconds / loopCount);
            }
        }

        private static readonly SortedList<string, RunCount> AllStopwatches;

        static StepTimer()
        {
            AllStopwatches = new SortedList<string, RunCount>();
        }

        public static ITimer Start(string name, bool runGC = false)
        {
            if (!AllStopwatches.ContainsKey(name))
            {
                AllStopwatches.Add(name, new RunCount());
            }
            if (runGC)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            RunCount sw = AllStopwatches[name];
            sw.SW.Restart();
            return sw;
        }

        public static void Reset()
        {
            AllStopwatches.Clear();
        }

        public static double GetAverage(string Name)
        {
            RunCount kvp = AllStopwatches[Name];
            kvp.RunResults.Sort();
            return kvp.RunResults[kvp.RunResults.Count >> 1];
        }

        public static double GetNanoSeconds(string Name, int loopCount)
        {
            RunCount kvp = AllStopwatches[Name];
            kvp.RunResults.Sort();
            return kvp.RunResults[kvp.RunResults.Count >> 1] * 1000000000.0 / loopCount;
        }

        public static double GetSlowest(string Name)
        {
            RunCount kvp = AllStopwatches[Name];
            kvp.RunResults.Sort();
            return kvp.RunResults[(int)(kvp.RunResults.Count * 0.9)];
        }

        public static string GetResults()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, RunCount> kvp in AllStopwatches)
            {
                kvp.Value.RunResults.Sort();
                double rate = kvp.Value.RunResults[kvp.Value.RunResults.Count >> 1];
                sb.Append(kvp.Key + '\t' + (rate / 1000000).ToString("0.00"));
            }
            return sb.ToString();
        }

        public static string Time(int internalLoopCount, Action del)
        {
            Stopwatch sw = new Stopwatch();

            int innerLoopCount = 1;

            //prime loop
            del();
            Thread.Sleep(1);
            del();
            Thread.Sleep(1);
            del();

            //Build an inner loop that takes at least 3 ms to complete.
            while (TimeLoop(sw, del, innerLoopCount) < 3)
            {
                innerLoopCount *= 2;
            }

            List<double> list = new List<double>();

            for (int x = 0; x < 100; x++)
            {
                if (x % 10 == 0)
                    Thread.Sleep(1);
                list.Add(TimeLoop(sw, del, innerLoopCount));
            }

            list.Sort();
            return (list[list.Count >> 2] * 1000000.0 / innerLoopCount / internalLoopCount).ToString("0.0");
        }

        private static double TimeLoop(Stopwatch sw, Action del, int loopCount)
        {
            sw.Restart();

            for (int x = 0; x < loopCount; x++)
            {
                del();
            }

            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        public static string Time(int internalLoopCount, Action<Stopwatch> del)
        {
            Stopwatch sw = new Stopwatch();

            int innerLoopCount = 1;

            //prime loop
            del(sw);
            Thread.Sleep(1);
            del(sw);
            Thread.Sleep(1);
            del(sw);

            //Build an inner loop that takes at least 3 ms to complete.
            while (TimeLoop(sw, del, innerLoopCount) < 3)
            {
                innerLoopCount *= 2;
            }

            List<double> list = new List<double>();

            for (int x = 0; x < 100; x++)
            {
                if (x % 10 == 0)
                    Thread.Sleep(1);
                list.Add(TimeLoop(sw, del, innerLoopCount));
            }

            list.Sort();
            return (list[list.Count >> 2] * 1000000.0 / innerLoopCount / internalLoopCount).ToString("0.0");
        }

        private static double TimeLoop(Stopwatch sw, Action<Stopwatch> del, int loopCount)
        {
            sw.Reset();

            for (int x = 0; x < loopCount; x++)
            {
                del(sw);
            }

            return sw.Elapsed.TotalMilliseconds;
        }
    }
}