using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace openHistorian.V2.StorageSystem.Specialized
{
    class NodeTest
    {

        internal static void Test()
        {
            TestSingleLevelAddGet();
            TestMultiLevel();
            //TestSortedList(1);
            TestMultiLevelLongRandom(1);
            TestScan(1);
        }
        static void TestMultiLevelLongRandom(int seed)
        {
            const int countTime = 1000;
            const int countKey = 1000;
            const int count = countTime * countKey;

            Random rand;
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            ms.Position = 6000 * 4096;
            ms.Write(new byte[] { 1 }, 0, 1);
            var bs = new BinaryStream(ms);
            var tree = new BPlusTree<TimeKeyPair.KeyType, TreeTypeLong>(bs, 4096);

            TimeKeyPair.KeyType origKey;
            origKey.Time = DateTime.Now;//new DateTime(DateTime.Now.Ticks);
            origKey.Key = seed;
            TimeKeyPair.KeyType key = origKey;
            long data;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < countTime; x++)
            {
                key.Time = new DateTime((long)rand.Next() << 30 | rand.Next());
                for (int y = 0; y < countKey; y++)
                {
                    key.Key = ((long)rand.Next() << 32) | rand.Next();
                    //key.Key += rand.Next() + 1;
                    //key.Time = key.Time.AddTicks(rand.Next() + 1);
                    data = rand.Next();
                    tree.AddData(key, data);
                }
            }

            sw.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            rand = new Random(seed);
            key = origKey;

            for (int x = 0; x < countTime; x++)
            {
                key.Time = new DateTime((long)rand.Next() << 30 | rand.Next());
                for (int y = 0; y < countKey; y++)
                {
                    key.Key = ((long)rand.Next() << 32) | rand.Next();
                    //key.Key += rand.Next() + 1;
                    //key.Time = key.Time.AddTicks(rand.Next() + 1);
                    data = rand.Next();

                    data2 = tree.GetData(key).Value;
                    if (data2 != data)
                        throw new Exception();
                }
            }


            sw2.Stop();
            MessageBox.Show((count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
        }

        static void TestMultiLevel()
        {
            const int count = 100000;

            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            ms.Position = 1000 * 4096;
            ms.Write(new byte[] { 1 }, 0, 1);
            var bs = new BinaryStream(ms);
            var tree = new BPlusTree<TimeKeyPair.KeyType, TreeTypeLong>(bs, 4096);

            TimeKeyPair.KeyType origKey;
            origKey.Time = new DateTime(DateTime.Now.Ticks);
            origKey.Key = seed;
            TimeKeyPair.KeyType key = origKey;
            long data;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < count; x++)
            {
                key.Key += rand.Next() + 1;
                key.Time = key.Time.AddTicks(rand.Next() + 1);
                data = rand.Next();
                tree.AddData(key, data);
            }

            sw.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            rand = new Random(seed);
            key = origKey;
            for (int x = 0; x < count; x++)
            {
                key.Key += rand.Next() + 1;
                key.Time = key.Time.AddTicks(rand.Next() + 1);
                data = rand.Next();

                data2 = tree.GetData(key).Value;
                if (data2 != data)
                    throw new Exception();
            }

            sw2.Stop();
            //MessageBox.Show((count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
            //    (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
        }
        static void TestSingleLevelAddGet()
        {
            const int count = 1000;

            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            var bs = new BinaryStream(ms);
            var tree = new BPlusTree<TimeKeyPair.KeyType, TreeTypeLong>(bs, 4096);

            TimeKeyPair.KeyType origKey;
            origKey.Time = new DateTime(DateTime.Now.Ticks);
            origKey.Key = seed;
            TimeKeyPair.KeyType key = origKey;
            long data;
            long data2;

            for (int x = 0; x < count; x++)
            {
                key.Key += rand.Next() + 1;
                key.Time = key.Time.AddTicks(rand.Next() + 1);
                data = rand.Next();
                tree.AddData(key, data);
            }

            rand = new Random(seed);
            key = origKey;
            for (int x = 0; x < count; x++)
            {
                key.Key += rand.Next() + 1;
                key.Time = key.Time.AddTicks(rand.Next() + 1);
                data = rand.Next();

                data2 = tree.GetData(key).Value;
                if (data2 != data)
                    throw new Exception();
            }

        }

        static void TestScan(int seed)
        {
            const int countTime = 1000;
            const int countKey = 1000;
            const int count = countTime * countKey;

            Random rand;
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            ms.Position = 6000 * 4096;
            ms.Write(new byte[] { 1 }, 0, 1);
            var bs = new BinaryStream(ms);
            var tree = new BPlusTree<TimeKeyPair.KeyType, TreeTypeLong>(bs, 4096);

            TimeKeyPair.KeyType origKey;
            origKey.Time = new DateTime(DateTime.Now.Ticks);
            origKey.Key = seed;
            TimeKeyPair.KeyType key = origKey;
            long data;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < countTime; x++)
            {
                for (int y = 0; y < countKey; y++)
                {
                    key.Time = key.Time.AddTicks(rand.Next() + 1);
                    key.Key += rand.Next() + 1;
                    data = rand.Next();
                    tree.AddData(key, data);
                }
            }

            sw.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            rand = new Random(seed);
            key = origKey;

            TimeKeyPair.KeyType start = default(TimeKeyPair.KeyType);
            TimeKeyPair.KeyType stop = default(TimeKeyPair.KeyType);
            start.Time = DateTime.MinValue;
            stop.Time = DateTime.MaxValue;

            int cnt = 0;
            var scan = tree.ExecuteScan(start, stop);
            while (scan.Next())
            {
                TimeKeyPair.KeyType keyResult = scan.GetKey();
                long valueResult = scan.GetValue().Value;

                key.Time = key.Time.AddTicks(rand.Next() + 1);
                key.Key += rand.Next() + 1;
                data = rand.Next();

                if (key.Time != keyResult.Time)
                    throw new Exception();

                if (key.Key != keyResult.Key)
                    throw new Exception();

                if (valueResult != data)
                    throw new Exception();

                cnt++;
                if (cnt == 86)
                    cnt = cnt;
            }

            sw2.Stop();
            MessageBox.Show((count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
        }

    }
}
