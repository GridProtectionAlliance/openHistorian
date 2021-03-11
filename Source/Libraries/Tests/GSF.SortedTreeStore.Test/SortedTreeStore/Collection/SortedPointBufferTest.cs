//******************************************************************************************************
//  SortedPointBufferTest.cs - Gbtc
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
//  2/5/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using openHistorian.Snap;

namespace GSF.Snap.Collection.Test
{
    [TestFixture]
    public class SortedPointBufferTest
    {
        [Test]
        public void Test()
        {
            const int MaxCount = 1000;
            Stopwatch sw = new Stopwatch();
            SortedPointBuffer<HistorianKey, HistorianValue> buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(MaxCount, true);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            Random r = new Random(1);

            for (int x = 0; x < MaxCount; x++)
            {
                key.Timestamp = (ulong)r.Next();
                key.PointID = (ulong)x;

                buffer.TryEnqueue(key, value);
            }

            sw.Start();
            buffer.IsReadingMode = true;
            sw.Stop();

            System.Console.WriteLine(sw.ElapsedMilliseconds);
            System.Console.WriteLine(MaxCount / sw.Elapsed.TotalSeconds / 1000000);

            for (int x = 0; x < MaxCount; x++)
            {
                buffer.ReadSorted(x, key, value);
                System.Console.WriteLine(key.Timestamp.ToString() + "\t" + key.PointID.ToString());
            }
        }


        [Test]
        public void BenchmarkRandomData()
        {
            for (int x = 16; x < 1000 * 1000; x *= 2)
            {
                BenchmarkRandomData(x);
            }
        }

        public void BenchmarkRandomData(int pointCount)
        {
            Stopwatch sw = new Stopwatch();
            SortedPointBuffer<HistorianKey, HistorianValue> buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(pointCount, true);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            List<double> times = new List<double>();
            for (int cnt = 0; cnt < 10; cnt++)
            {
                Random r = new Random(1);
                buffer.IsReadingMode = false;
                for (int x = 0; x < pointCount; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                sw.Restart();
                buffer.IsReadingMode = true;
                sw.Stop();
                times.Add(sw.Elapsed.TotalSeconds);
            }
            times.Sort();
            System.Console.WriteLine("{0} points {1}ms {2} Million/second ", pointCount, times[5] * 1000, pointCount / times[5] / 1000000);
        }

        [Test]
        public void BenchmarkRandomDataRead()
        {
            for (int x = 16; x < 1000 * 1000; x *= 2)
            {
                BenchmarkRandomDataRead(x);
            }
        }

        public void BenchmarkRandomDataRead(int pointCount)
        {
            Stopwatch sw = new Stopwatch();
            SortedPointBuffer<HistorianKey, HistorianValue> buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(pointCount, true);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            List<double> times = new List<double>();
            for (int cnt = 0; cnt < 10; cnt++)
            {
                Random r = new Random(1);
                buffer.IsReadingMode = false;
                for (int x = 0; x < pointCount; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                buffer.IsReadingMode = true;
                sw.Restart();
                while (buffer.Read(key, value))
                {
                }
                sw.Stop();
                times.Add(sw.Elapsed.TotalSeconds);
            }
            times.Sort();
            System.Console.WriteLine("{0} points {1}ms {2} Million/second ", pointCount, times[5] * 1000, pointCount / times[5] / 1000000);
        }

        [Test]
        public void BenchmarkSortedData()
        {
            for (int x = 16; x < 1000 * 1000; x *= 2)
            {
                BenchmarkSortedData(x);
            }
        }

        public void BenchmarkSortedData(int pointCount)
        {
            Stopwatch sw = new Stopwatch();
            SortedPointBuffer<HistorianKey, HistorianValue> buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(pointCount, true);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            List<double> times = new List<double>();
            for (int cnt = 0; cnt < 10; cnt++)
            {
                buffer.IsReadingMode = false;
                for (int x = 0; x < pointCount; x++)
                {
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }

                sw.Restart();
                buffer.IsReadingMode = true;
                sw.Stop();
                times.Add(sw.Elapsed.TotalSeconds);
            }
            times.Sort();
            System.Console.WriteLine("{0} points {1}ms {2} Million/second ", pointCount, times[5] * 1000, pointCount / times[5] / 1000000);
        }

        [Test]
        public void BenchmarkSortedDataRead()
        {
            for (int x = 16; x < 1000 * 1000; x *= 2)
            {
                BenchmarkSortedDataRead(x);
            }
        }

        public void BenchmarkSortedDataRead(int pointCount)
        {
            Stopwatch sw = new Stopwatch();
            SortedPointBuffer<HistorianKey, HistorianValue> buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(pointCount, true);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            List<double> times = new List<double>();
            for (int cnt = 0; cnt < 10; cnt++)
            {
                buffer.IsReadingMode = false;
                for (int x = 0; x < pointCount; x++)
                {
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                buffer.IsReadingMode = true;
                sw.Restart();
                while (buffer.Read(key, value))
                {
                }
                sw.Stop();
                times.Add(sw.Elapsed.TotalSeconds);
            }
            times.Sort();
            System.Console.WriteLine("{0} points {1}ms {2} Million/second ", pointCount, times[5] * 1000, pointCount / times[5] / 1000000);
        }
    }
}
