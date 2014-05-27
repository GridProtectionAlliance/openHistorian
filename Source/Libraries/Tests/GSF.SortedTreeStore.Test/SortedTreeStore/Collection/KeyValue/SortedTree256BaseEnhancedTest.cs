//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Text;
//using GSF;
//using openHistorian.Collections;

//namespace openHistorian.Collections
//{
//    internal static class SortedTree256BaseEnhancedTest
//    {
//        public static void BenchmarkTreeScanner(Func<SortedTree256> initializer, uint pointCount)
//        {
//            int seed = 1235213452;
//            double[] times = new double[11];

//            SortedTree256 tree = null;
//            for (int x = 0; x < 11; x++)
//            {
//                tree = initializer();
//                times[x] = AddSequential(tree, 1324 + pointCount, pointCount);
//            }
//            ReportAverage(times, pointCount, "Sequentially Add " + pointCount.ToString("N0") + " points.");

//            for (int x = 0; x < 11; x++)
//            {
//                tree = initializer();
//                times[x] = AddReverseSequential(tree, 1324 + pointCount, pointCount);
//            }
//            ReportAverage(times, pointCount, "Reverse Sequentially Add " + pointCount.ToString("N0") + " points.");

//            for (int x = 0; x < 11; x++)
//            {
//                tree = initializer();
//                times[x] = AddRandom(tree, 1324, pointCount);
//            }
//            ReportAverage(times, pointCount, "Random Add " + pointCount.ToString("N0") + " points.");

//        }

//        static void ReportAverage(double[] times, uint pointCount, string message)
//        {
//            Array.Sort(times);
//            double sum = 0;
//            for (int x = 3; x < 8; x++)
//                sum += times[x];
//            sum = sum / 5;

//            double pps = pointCount / sum / 1000000;

//            Console.WriteLine(message + " (" + (sum * 1000).ToString("0.0") + "ms, " + pps.ToString("0.00") + " MPPS)");

//        }

//        static double AddSequential(SortedTree256 tree, ulong firstPoint, ulong count)
//        {
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            var scan = new ScannerSequential(firstPoint, count);
//            tree.Add(scan);
//            sw.Stop();
//            return sw.Elapsed.TotalSeconds;
//        }

//        static double AddReverseSequential(SortedTree256 tree, ulong lastPoint, ulong count)
//        {
//            Stopwatch sw = new Stopwatch();
//            sw.Start();

//            for (ulong x = lastPoint; x > lastPoint - count; x--)
//            {
//                tree.Add(x, x, x, x);
//            }
//            sw.Stop();
//            return sw.Elapsed.TotalSeconds;
//        }

//        static double AddRandom(SortedTree256 tree, int seed, uint count)
//        {
//            Random r = new Random(seed);
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (uint x = 0; x < count; x++)
//            {
//                ulong value = (ulong)r.Next();
//                tree.Add(value, x, value, value);
//            }
//            sw.Stop();
//            return sw.Elapsed.TotalSeconds;
//        }

//        class ScannerSequential : Stream256Base
//        {
//            ulong m_first;
//            ulong m_count;
//            public ScannerSequential(ulong first, ulong count)
//            {
//                m_first = first;
//                m_count = count;
//            }

//            public override bool Read()
//            {
//                if (m_count <= 0)
//                {
//                    Key1 = 0;
//                    Key2 = 0;
//                    Value1 = 0;
//                    Value2 = 0;
//                    return false;
//                }
//                m_count--;
//                Key1 = m_first;
//                Key2 = m_first;
//                Value1 = m_first;
//                Value2 = m_first;
//                m_first++;
//                return true;
//            }

//        }


//    }
//}

