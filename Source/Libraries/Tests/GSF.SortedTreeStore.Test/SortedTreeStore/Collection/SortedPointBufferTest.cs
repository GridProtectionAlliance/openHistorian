//******************************************************************************************************
//  SortedPointBufferTest.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  2/5/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian.Collections;

namespace GSF.SortedTreeStore.Collection.Test
{
    [TestFixture]
    public class SortedPointBufferTest
    {
        [Test]
        public void Test()
        {

            const int MaxCount = 1000;
            Stopwatch sw = new Stopwatch();
            var buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(MaxCount);

            var key = new HistorianKey();
            var value = new HistorianValue();
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
        public void TestBenchmark()
        {


            const int MaxCount = 10000;
            Stopwatch sw = new Stopwatch();
            var buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(MaxCount);

            var key = new HistorianKey();
            var value = new HistorianValue();
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

        }

        [Test]
        public void TestBenchmark2()
        {
            const int MaxCount = 10000;
            Stopwatch sw = new Stopwatch();
            var buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(MaxCount);

            var key = new HistorianKey();
            var value = new HistorianValue();

            for (int i = 0; i < 10; i++)
            {
                Random r = new Random(1);
                buffer.IsReadingMode = false;
                for (int x = 0; x < MaxCount; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                sw.Reset();
                sw.Start();
                buffer.IsReadingMode = true;
                sw.Stop();

                System.Console.WriteLine(sw.ElapsedMilliseconds);
                System.Console.WriteLine(MaxCount / sw.Elapsed.TotalSeconds / 1000000);

            }
        }

        [Test]
        public void TestBenchmarkResort()
        {
            const int MaxCount = 10000;
            Stopwatch sw = new Stopwatch();
            var buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(MaxCount);

            var key = new HistorianKey();
            var value = new HistorianValue();

            for (int i = 0; i < 10; i++)
            {
                buffer.IsReadingMode = false;
                for (int x = 0; x < MaxCount; x++)
                {
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                sw.Reset();
                sw.Start();
                buffer.IsReadingMode = true;
                sw.Stop();

                System.Console.WriteLine(sw.ElapsedMilliseconds);
                System.Console.WriteLine(MaxCount / sw.Elapsed.TotalSeconds / 1000000);

            }
        }


        [Test]
        public void TestBenchmark3()
        {

            int Count = 64;
            Stopwatch sw = new Stopwatch();
            var buffer = new SortedPointBuffer<HistorianKey, HistorianValue>(1024 * 1024);

            var key = new HistorianKey();
            var value = new HistorianValue();

            for (int i = 0; i < 15; i++)
            {
                Random r = new Random(1);
                buffer.IsReadingMode = false;
                for (int x = 0; x < Count; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                sw.Reset();
                sw.Start();
                buffer.IsReadingMode = true;
                sw.Stop();
                System.Console.WriteLine(Count.ToString() + "\t" + (Count / sw.Elapsed.TotalSeconds / 1000000).ToString());
                Count *= 2;

            }
        }

    }
}
