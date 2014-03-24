using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian.Collections;

namespace GSF.SortedTreeStore.Collection
{
    [TestFixture]
    public class SortedPointBuffer_Test
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
            buffer.Sort();
            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(MaxCount / sw.Elapsed.TotalSeconds / 1000000);

            for (int x = 0; x < MaxCount; x++)
            {
                buffer.ReadSorted(x, key, value);
                Console.WriteLine(key.Timestamp.ToString() + "\t" + key.PointID.ToString());
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
            buffer.Sort();
            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(MaxCount / sw.Elapsed.TotalSeconds / 1000000);

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
                buffer.Clear();
                for (int x = 0; x < MaxCount; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                sw.Reset();
                sw.Start();
                buffer.Sort();
                sw.Stop();

                Console.WriteLine(sw.ElapsedMilliseconds);
                Console.WriteLine(MaxCount / sw.Elapsed.TotalSeconds / 1000000);

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
                buffer.Clear();
                for (int x = 0; x < MaxCount; x++)
                {
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                sw.Reset();
                sw.Start();
                buffer.Sort();
                sw.Stop();

                Console.WriteLine(sw.ElapsedMilliseconds);
                Console.WriteLine(MaxCount / sw.Elapsed.TotalSeconds / 1000000);

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
                buffer.Clear();
                for (int x = 0; x < Count; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)x;

                    buffer.TryEnqueue(key, value);
                }
                sw.Reset();
                sw.Start();
                buffer.Sort();
                sw.Stop();
                Console.WriteLine(Count.ToString() + "\t" + (Count / sw.Elapsed.TotalSeconds / 1000000).ToString());
                Count *= 2;

            }
        }

    }
}
