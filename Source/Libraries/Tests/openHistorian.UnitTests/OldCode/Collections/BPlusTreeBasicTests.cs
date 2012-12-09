//using System;
//using System.Diagnostics;
//using System.Windows.Forms;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using openHistorian.Collections.BPlusTreeTypes;
//using openHistorian.Collections.Specialized;
//using openHistorian.IO.Unmanaged;

//namespace openHistorian.Collections
//{
//    //[TestClass()]
//    public class BPlusTreeBasicTests
//    {
//        //[TestMethod()]
//        public void Test()
//        {
//            TestMultiLevelLongRandom(1);
//            TestMultiLevelLongRandomSpecific(1);
//            TestScan(1);
//        }

//        static void TestMultiLevelLongRandomSpecific(int seed)
//        {
//            const int countTime = 1000;
//            const int countKey = 1000;
//            const int count = countTime * countKey;

//            Random rand;
//            rand = new Random(seed);

//            var ms = new MemoryStream();
//            ms.Position = 6000 * 4096;
//            ms.Write(new byte[] { 1 }, 0, 1);
//            var bs = new BinaryStream(ms);
//            var tree = new BPlusTreeLongLong(bs, 4096);

//            long key;
//            long data;
//            long data2;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();

//            for (int x = 0; x < countTime; x++)
//            {
//                for (int y = 0; y < countKey; y++)
//                {
//                    key = (long)rand.Next() << 30 | rand.Next();
//                    data = rand.Next();
//                    tree.Add(key, data);
//                }
//            }

//            sw.Stop();

//            Stopwatch sw2 = new Stopwatch();
//            sw2.Start();

//            rand = new Random(seed);

//            for (int x = 0; x < countTime; x++)
//            {
//                for (int y = 0; y < countKey; y++)
//                {
//                    key = (long)rand.Next() << 30 | rand.Next();
//                    data = rand.Next();

//                    data2 = tree.Get(key);
//                    if (data2 != data)
//                        throw new Exception();
//                }
//            }


//            sw2.Stop();
//            MessageBox.Show((count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
//                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
//        }

//        static void TestMultiLevelLongRandom(int seed)
//        {
//            const int countTime = 1000;
//            const int countKey = 1000;
//            const int count = countTime * countKey;

//            Random rand;
//            rand = new Random(seed);

//            var ms = new MemoryStream();
//            ms.Position = 6000 * 4096;
//            ms.Write(new byte[] { 1 }, 0, 1);
//            var bs = new BinaryStream(ms);
//            var tree = new BPlusTree<Long, Long>(bs, 4096);

//            Long key;
//            long data;
//            long data2;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();

//            for (int x = 0; x < countTime; x++)
//            {
//                for (int y = 0; y < countKey; y++)
//                {
//                    key = (long)rand.Next() << 30 | rand.Next();
//                    data = rand.Next();
//                    tree.Add(key, data);
//                }
//            }

//            sw.Stop();

//            Stopwatch sw2 = new Stopwatch();
//            sw2.Start();

//            rand = new Random(seed);

//            for (int x = 0; x < countTime; x++)
//            {
//                for (int y = 0; y < countKey; y++)
//                {
//                    key = (long)rand.Next() << 30 | rand.Next();
//                    data = rand.Next();

//                    data2 = tree.Get(key).Value;
//                    if (data2 != data)
//                        throw new Exception();
//                }
//            }


//            sw2.Stop();
//            MessageBox.Show((count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
//                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
//        }

//        static void TestScan(int seed)
//        {
//            const int countTime = 1000;
//            const int countKey = 1000;
//            const int count = countTime * countKey;

//            Random rand;
//            rand = new Random(seed);

//            var ms = new MemoryStream();
//            ms.Position = 6000 * 4096;
//            ms.Write(new byte[] { 1 }, 0, 1);
//            var bs = new BinaryStream(ms);
//            var tree = new BPlusTree<DateTimeLong, Long>(bs, 4096);

//            DateTimeLong origKey;
//            origKey.Time = new DateTime(DateTime.Now.Ticks);
//            origKey.Key = seed;
//            DateTimeLong key = origKey;
//            long data;
//            long data2;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();

//            for (int x = 0; x < countTime; x++)
//            {
//                for (int y = 0; y < countKey; y++)
//                {
//                    key.Time = key.Time.AddTicks(rand.Next() + 1);
//                    key.Key += rand.Next() + 1;
//                    data = rand.Next();
//                    tree.Add(key, data);
//                }
//            }

//            sw.Stop();

//            Stopwatch sw2 = new Stopwatch();
//            sw2.Start();

//            rand = new Random(seed);
//            key = origKey;

//            DateTimeLong start = default(DateTimeLong);
//            DateTimeLong stop = default(DateTimeLong);
//            start.Time = DateTime.MinValue;
//            stop.Time = DateTime.MaxValue;

//            int cnt = 0;
//            var scan = tree.GetRange(start, stop);
//            foreach (var kvp in scan)
//            {
//                DateTimeLong keyResult = kvp.Key;
//                long valueResult = kvp.Value.Value;

//                key.Time = key.Time.AddTicks(rand.Next() + 1);
//                key.Key += rand.Next() + 1;
//                data = rand.Next();

//                if (key.Time != keyResult.Time)
//                    throw new Exception();

//                if (key.Key != keyResult.Key)
//                    throw new Exception();

//                if (valueResult != data)
//                    throw new Exception();

//                cnt++;
//                if (cnt == 86)
//                    cnt = cnt;
//            }

//            sw2.Stop();
//            MessageBox.Show((count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
//                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
//        }

//    }
//}
