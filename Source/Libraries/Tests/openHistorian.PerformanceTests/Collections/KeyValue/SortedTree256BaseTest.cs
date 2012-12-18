using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace openHistorian.Collections.KeyValue
{
    internal static class SortedTree256BaseTest
    {
        public static void BenchmarkTree(Func<SortedTree256Base> initializer, uint pointCount)
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
            //return;
            for (int x = 0; x < 11; x++)
            {
                times[x] = GetSequential(tree, 1324 + pointCount, pointCount);
            }
            ReportAverage(times, pointCount, "Sequentially Get " + pointCount.ToString("N0") + " points.");

            for (int x = 0; x < 11; x++)
            {
                times[x] = ScanTree(tree);
            }
            ReportAverage(times, pointCount, "Scan " + pointCount.ToString("N0") + " points.");

            for (int x = 0; x < 11; x++)
            {
                tree = initializer();
                times[x] = AddReverseSequential(tree, 1324 + pointCount, pointCount);
            }
            ReportAverage(times, pointCount, "Reverse Sequentially Add " + pointCount.ToString("N0") + " points.");

            for (int x = 0; x < 11; x++)
            {
                times[x] = GetReverseSequential(tree, 1324 + pointCount, pointCount);
            }
            ReportAverage(times, pointCount, "Reverse Sequentially Get " + pointCount.ToString("N0") + " points.");

            for (int x = 0; x < 11; x++)
            {
                times[x] = ScanTree(tree);
            }
            ReportAverage(times, pointCount, "Scan " + pointCount.ToString("N0") + " points.");

            for (int x = 0; x < 11; x++)
            {
                tree = initializer();
                times[x] = AddRandom(tree, 1324, pointCount);
            }
            ReportAverage(times, pointCount, "Random Add " + pointCount.ToString("N0") + " points.");

            for (int x = 0; x < 11; x++)
            {
                times[x] = GetRandom(tree, 1324, pointCount);
            }
            ReportAverage(times, pointCount, "Random Get " + pointCount.ToString("N0") + " points.");
            
            for (int x = 0; x < 11; x++)
            {
                times[x] = ScanTree(tree);
            }
            ReportAverage(times, pointCount, "Scan " + pointCount.ToString("N0") + " points.");
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
            for (ulong x = firstPoint; x < firstPoint + count; x++)
            {
                tree.Add(x, x, x, x);
            }
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

        static double GetSequential(SortedTree256Base tree, ulong firstPoint, ulong count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ulong v1, v2;
            for (ulong x = firstPoint; x < firstPoint + count; x++)
            {
                tree.Get(x, x, out v1, out v2);
            }
            sw.Stop();
            return sw.Elapsed.TotalSeconds;
        }

        static double ScanTree(SortedTree256Base tree)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ulong v1, v2, v3, v4;
            var scan = tree.GetDataRange();
            scan.SeekToKey(0, 0);
            while(scan.GetNextKey(out v1, out v2, out v3, out v4))
                ;
            sw.Stop();
            return sw.Elapsed.TotalSeconds;
        }

        static double GetReverseSequential(SortedTree256Base tree, ulong lastPoint, ulong count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ulong v1, v2;
            for (ulong x = lastPoint; x > lastPoint - count; x--)
            {
                tree.Get(x, x, out v1, out v2);
            }
            sw.Stop();
            return sw.Elapsed.TotalSeconds;
        }

        static double GetRandom(SortedTree256Base tree, int seed, uint count)
        {
            Random r = new Random(seed);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ulong v1, v2;
            for (uint x = 0; x < count; x++)
            {
                ulong value = (ulong)r.Next();
                tree.Get(value, x, out v1, out v2);
            }
            sw.Stop();
            return sw.Elapsed.TotalSeconds;
        }



    }
}
