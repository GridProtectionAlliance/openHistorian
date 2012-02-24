using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Historian.StorageSystem.BlockSorter
{
    class NodeTest
    {
        internal static void Test()
        {
            TestSingleLevelAddGet();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TestMulitLevel();
            sw.Stop();
            MessageBox.Show(sw.Elapsed.TotalMilliseconds.ToString());

            TestMulitLevelDeep();

        }

        static void TestMulitLevelDeep()
        {
            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            var bs = new BinaryStream(ms);
            var node = BPlusTree.Create(bs, 4096);

            DateTime origDate = DateTime.Now;
            DateTime date = origDate;
            TimeKey key;

            byte[] data = new byte[1];
            byte[] data2;
            for (int x = 0; x < 100000; x++)
            {
                key = new TimeKey(date);
                data[0] = (byte)rand.Next();
                date = date.AddTicks(data[0] + 1);

                node.AddData(key, data);
            }

            rand = new Random(seed);
            date = origDate;
            for (int x = 0; x < 100000; x++)
            {
                key = new TimeKey(date);
                data[0] = (byte)rand.Next();
                date = date.AddTicks(data[0] + 1);

                data2 = node.GetData(key);
                if (data2.Length != 1 || data[0] != data2[0])
                    throw new Exception();
            }

        }

        static void TestMulitLevel()
        {
            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            var bs = new BinaryStream(ms);
            var node = BPlusTree.Create(bs, 4096);

            DateTime origDate = DateTime.Now;
            DateTime date = origDate;
            TimeKey key;

            byte[] data = new byte[1];
            byte[] data2;
            for (int x = 0; x < 100000; x++)
            {
                key = new TimeKey(date);
                data[0] = (byte)rand.Next();
                date = date.AddTicks(data[0] + 1);

                node.AddData(key, data);
            }

            rand = new Random(seed);
            date = origDate;
            for (int x = 0; x < 100000; x++)
            {
                key = new TimeKey(date);
                data[0] = (byte)rand.Next();
                date = date.AddTicks(data[0] + 1);

                data2 = node.GetData(key);
                if (data2.Length!=1 || data[0] != data2[0])
                    throw new Exception();
            }

        }

        static void TestSingleLevelAddGet()
        {
            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            var bs = new BinaryStream(ms);
            var node = BPlusTree.Create(bs, 4096);

            DateTime origDate = DateTime.Now;
            DateTime date = origDate;
            TimeKey key;

            byte[] data = new byte[250];
            byte[] data2;
            for (int x = 0; x < LeafNode.CalculateMaximumChildren(4096); x++)
            {
                key = new TimeKey(date);
                date = date.AddTicks(rand.Next() + 1);
                rand.NextBytes(data);
                node.AddData(key, data);
            }

            rand = new Random(seed);
            date = origDate;
            for (int x = 0; x < LeafNode.CalculateMaximumChildren(4096); x++)
            {
                key = new TimeKey(date);
                date = date.AddTicks(rand.Next() + 1);
                rand.NextBytes(data);

                data2 = node.GetData(key);
                if (!data.SequenceEqual(data2))
                    throw new Exception();
            }

        }
    }
}
