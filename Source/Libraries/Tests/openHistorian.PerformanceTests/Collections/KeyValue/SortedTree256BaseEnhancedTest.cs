using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using openHistorian.Collections.KeyValue;

namespace openHistorian.Collections.KeyValue
{
    internal static class SortedTree256BaseEnhancedTest
    {
        public static void BenchmarkTreeScanner(Func<SortedTree256Base> initializer, uint pointCount)
        {
            int seed = 1235213452;
            double[] times = new double[11];

            SortedTree256Base tree = null;
            for (int x = 0; x < 11; x++)
            {
                tree = initializer();
                times[x] = AddSequential(tree, 1324 + pointCount, pointCount);
            }
            ReportAverage(times, pointCount, "Sequentially Add " + pointCount.ToString("N0") + " points.");

            for (int x = 0; x < 11; x++)
            {
                tree = initializer();
                times[x] = AddReverseSequential(tree, 1324 + pointCount, pointCount);
            }
            ReportAverage(times, pointCount, "Reverse Sequentially Add " + pointCount.ToString("N0") + " points.");

            for (int x = 0; x < 11; x++)
            {
                tree = initializer();
                times[x] = AddRandom(tree, 1324, pointCount);
            }
            ReportAverage(times, pointCount, "Random Add " + pointCount.ToString("N0") + " points.");

        }

        static void ReportAverage(double[] times, uint pointCount, string message)
        {
            Array.Sort(times);
            double sum = 0;
            for (int x = 3; x < 8; x++)
                sum += times[x];
            sum = sum / 5;

            double pps = pointCount / sum / 1000000;

            Console.WriteLine(message + " (" + (sum * 1000).ToString("0.0") + "ms, " + pps.ToString("0.00") + " MPPS)");

        }

        static double AddSequential(SortedTree256Base tree, ulong firstPoint, ulong count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var scan = new ScannerSequential(firstPoint, count);
            tree.Add(scan);
            sw.Stop();
            return sw.Elapsed.TotalSeconds;
        }

        static double AddReverseSequential(SortedTree256Base tree, ulong lastPoint, ulong count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            for (ulong x = lastPoint; x > lastPoint - count; x--)
            {
                tree.Add(x, x, x, x);
            }
            sw.Stop();
            return sw.Elapsed.TotalSeconds;
        }

        static double AddRandom(SortedTree256Base tree, int seed, uint count)
        {
            Random r = new Random(seed);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (uint x = 0; x < count; x++)
            {
                ulong value = (ulong)r.Next();
                tree.Add(value, x, value, value);
            }
            sw.Stop();
            return sw.Elapsed.TotalSeconds;
        }

        class ScannerSequential : ITreeScanner256
        {
            ulong m_first;
            ulong m_count;
            public ScannerSequential(ulong first, ulong count)
            {
                m_first = first;
                m_count = count;
            }

            public bool GetNextKey(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
            {
                if (m_count <= 0)
                {
                    key1 = 0;
                    key2 = 0;
                    value1 = 0;
                    value2 = 0;
                    return false;
                }
                m_count--;
                key1 = m_first;
                key2 = m_first;
                value1 = m_first;
                value2 = m_first;
                m_first++;
                return true;
            }

            public void SeekToKey(ulong key1, ulong key2)
            {
                throw new NotImplementedException();
            }
        }


    }
}
