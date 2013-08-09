//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using GSF;
//using GSF.IO.Unmanaged;
//using NUnit.Framework;
//using openHistorian.Archive;

//namespace openHistorian.Collections
//{
//    [TestFixture]
//    public class SortedTree64Test
//    {
//        bool m_supress;

//        [Test]
//        public void TestRandom()
//        {
//            int count = 100000;
//            var random = new Random(1);
//            using (var bs = new BinaryStream())
//            {
//                bs.Write(0);
//                SortedList<uint, uint> list = new SortedList<uint, uint>();
//                SortedTree64 tree = SortedTree64.Create(bs, 4096, CompressionMethod.None);

//                for (uint x = 0; x < count; x++)
//                {
//                    uint rand = (uint)random.Next();
//                    list.Add(rand, x * 2);
//                    tree.Add(rand, x * 2);
//                    if (tree.Get(rand) != x * 2)
//                        throw new Exception();

//                    if ((x & 4095) == 0)
//                        random = new Random(random.Next());

//                }

//                random = new Random(1);

//                for (uint x = 0; x < count; x++)
//                {
//                    uint rand = (uint)random.Next();
//                    if (tree.Get(rand) != x * 2)
//                        throw new Exception();

//                    if ((x & 4095) == 0)
//                        random = new Random(random.Next());
//                }

//                var scan = tree.GetTreeScanner();
//                scan.SeekToKey(0);

//                foreach (var kvp in list)
//                {
//                    if (!scan.Read())
//                        throw new Exception();
//                    if (scan.CurrentKVP.Key != kvp.Key)
//                        throw new Exception();
//                    if (scan.CurrentKVP.Value != kvp.Value)
//                        throw new Exception();
//                }
//                if (scan.Read())
//                    throw new Exception();
//            }
//        }

//        [Test]
//        public void BenchmarkRandom()
//        {
//            Console.WriteLine("Random\tCount\tRead\tWrite");

//            //m_supress = true;
//            //TestSortedListRandom(100000);
//            //m_supress = false;

//            //TestSortedListRandom(100);
//            //TestSortedListRandom(1000);
//            //TestSortedListRandom(10000);
//            //TestSortedListRandom(100000);
//            ////TestSortedList(1000000);
//            ////TestSortedList(10000000);

//            m_supress = true;
//            TestSortedTreeRandom(100000);
//            m_supress = false;

//            TestSortedTreeRandom(100);
//            TestSortedTreeRandom(1000);
//            TestSortedTreeRandom(10000);
//            TestSortedTreeRandom(100000);
//            //TestSortedTree(1000000);
//            //TestSortedTree(10000000);
//        }

//        public void TestSortedTreeRandom(uint count)
//        {
//            GC.Collect();
//            var random = new Random(1);
//            Stopwatch swWrite = new Stopwatch();
//            Stopwatch swRead = new Stopwatch();
//            Stopwatch swScan = new Stopwatch();
//            using (var bs = new BinaryStream())
//            {
//                bs.Write(0);

//                SortedTree64 tree = SortedTree64.Create(bs, 4096, CompressionMethod.None);
//                tree.SkipIntermediateSaves = true;

//                swWrite.Start();
//                for (uint x = 0; x < count; x++)
//                {
//                    tree.Add((uint)random.Next(), x * 2);

//                    if ((x & 4095) == 0)
//                        random = new Random(random.Next());
//                }
//                swWrite.Stop();

//                random = new Random(1);

//                swRead.Start();
//                for (uint x = 0; x < count; x++)
//                {
//                    tree.Get((uint)random.Next());

//                    if ((x & 4095) == 0)
//                        random = new Random(random.Next());
//                }
//                swRead.Stop();

//                swScan.Start();
//                var scan = tree.GetTreeScanner();
//                scan.SeekToKey(0);
//                while (scan.Read())
//                    ;
//                swScan.Stop();

//            }
//            if (m_supress)
//                return;

//            Console.Write(count.ToString("Tree\t0\t"));
//            Console.Write((count / swRead.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.Write((count / swWrite.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.Write((count / swScan.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.WriteLine();
//        }

//        public void TestSortedListRandom(uint count)
//        {
//            var random = new Random(1);
//            Stopwatch swWrite = new Stopwatch();
//            Stopwatch swRead = new Stopwatch();
//            Stopwatch swScan = new Stopwatch();

//            SortedList<uint, uint> tree = new SortedList<uint, uint>();

//            swWrite.Start();
//            for (uint x = 0; x < count; x++)
//            {
//                tree.Add((uint)random.Next(), x * 2);

//                if ((x & 4095) == 0)
//                    random = new Random(random.Next());
//            }
//            swWrite.Stop();

//            random = new Random(1);

//            swRead.Start();
//            for (uint x = 0; x < count; x++)
//            {
//                uint l = tree[(uint)random.Next()];

//                if ((x & 4095) == 0)
//                    random = new Random(random.Next());
//            }
//            swRead.Stop();

//            swScan.Start();
//            foreach (var kvp in tree)
//                ;
//            swScan.Stop();

//            if (m_supress)
//                return;

//            //var SB = new StringBuilder();
//            Console.Write(count.ToString("List\t0\t"));
//            Console.Write((count / swRead.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.Write((count / swWrite.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.Write((count / swScan.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.WriteLine();
//            //Clipboard.SetText(SB.ToString());
//        }

//        [Test]
//        public void BenchmarkSequential()
//        {
//            //Console.WriteLine("Sequential\tCount\tRead\tWrite");

//            //m_supress = true;
//            //TestSortedList(100000);
//            //m_supress = false;

//            //TestSortedList(100);
//            //TestSortedList(1000);
//            //TestSortedList(10000);
//            //TestSortedList(100000);
//            ////TestSortedList(1000000);
//            ////TestSortedList(10000000);

//            m_supress = true;
//            //TestSortedTree(100000);
//            m_supress = false;

//            //TestSortedTree(100);
//            //TestSortedTree(1000);
//            TestSortedTree(10000);
//            //TestSortedTree(100000);
//            //TestSortedTree(1000000);
//            //TestSortedTree(10000000);
//        }

//        public void TestSortedTree(uint count, bool clip = false)
//        {
//            StepTimer.Reset();
//            for (int cnt = 0; cnt < 1000; cnt++)
//            {
//                GC.Collect();
//                GC.WaitForPendingFinalizers();

//                using (var bs = new BinaryStream())
//                {
//                    bs.Write(0);

//                    SortedTree64 tree = SortedTree64.Create(bs, 4096, CompressionMethod.None);
//                    tree.SkipIntermediateSaves = true;

//                    var timer = StepTimer.Start("Write");
//                    for (uint x = 0; x < count; x++)
//                    {
//                        tree.Add(x, x * 2);
//                    }
//                    timer.Stop();
//                    timer = StepTimer.Start("Read");
//                    for (uint x = 0; x < count; x++)
//                    {
//                        tree.Get(x);
//                    }
//                    timer.Stop();
//                    timer = StepTimer.Start("Scan");
//                    var scan = tree.GetTreeScanner();
//                    scan.SeekToKey(0);
//                    //scan.SeekToKey(0);
//                    //var read = scan.GetReadDelegate;
//                    long sum = 0;
//                    var KVP = scan.CurrentKey;
//                    while (scan.Read())
//                    {
//                        //sum += KVP.Key;// +KVP.Value;
//                        //sum += scan.CurrentKey.Key + scan.CurrentKey.Value;
//                    }
//                    timer.Stop();
//                }
//            }
//            if (m_supress)
//                return;


//            var SB = new StringBuilder();
//            //Console.Write(count.ToString("Tree\t0\t"));
//            SB.Append((count / StepTimer.GetAverage("Read") / 1000000).ToString("0.000\t"));
//            SB.Append((count / StepTimer.GetAverage("Write") / 1000000).ToString("0.000\t"));
//            SB.Append((count / StepTimer.GetAverage("Scan") / 1000000).ToString("0.000"));
//            if (clip)
//                Clipboard.SetText(SB.ToString());
//            Console.WriteLine(SB.ToString());

//        }

//        public void TestSortedList(uint count)
//        {
//            Stopwatch swWrite = new Stopwatch();
//            Stopwatch swRead = new Stopwatch();
//            Stopwatch swScan = new Stopwatch();

//            SortedList<uint, uint> tree = new SortedList<uint, uint>();

//            swWrite.Start();
//            for (uint x = 0; x < count; x++)
//            {
//                tree.Add(x, x * 2);
//            }
//            swWrite.Stop();
//            swRead.Start();
//            for (uint x = 0; x < count; x++)
//            {
//                uint l = tree[x];
//            }
//            swRead.Stop();

//            swScan.Start();
//            foreach (var kvp in tree)
//                ;
//            swScan.Stop();

//            if (m_supress)
//                return;

//            Console.Write(count.ToString("List\t0\t"));
//            Console.Write((count / swRead.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.Write((count / swWrite.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.Write((count / swScan.Elapsed.TotalSeconds / 1000000).ToString("0.000\t"));
//            Console.WriteLine();
//        }


//    }
//}

