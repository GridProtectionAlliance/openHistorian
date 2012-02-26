using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace openHistorian.Core.StorageSystem.Generic
{
    class NodeTest
    {

        internal static void Test()
        {
            TestSingleLevelAddGet();
            TestMultiLevel();
            //TestMulitLevelDeep();
        }
        
        static void TestMultiLevel()
        {
            const int count = 100000;

            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            var bs = new BinaryStream(ms);
            var tree = new BPlusTree<TreeTypeLong, TreeTypeLong>(bs, 4096);

            long origKey = seed;
            TreeTypeLong key = origKey;
            TreeTypeLong data=1;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < count; x++)
            {
                key.Value += rand.Next() + 1;
                data.Value = rand.Next();
                tree.AddData(key, data);
            }

            sw.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            rand = new Random(seed);
            key = origKey;
            for (int x = 0; x < count; x++)
            {
                key.Value += rand.Next() + 1;
                data.Value = rand.Next();

                data2 = tree.GetData(key).Value;
                if (data2 != data.Value)
                    throw new Exception();
            }

            sw2.Stop();
            MessageBox.Show((count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
        }
        static void TestSingleLevelAddGet()
        {
            const int count = 1000;

            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);

            var ms = new PooledMemoryStream();
            var bs = new BinaryStream(ms);
            var tree = new BPlusTree<TreeTypeLong, TreeTypeLong>(bs, 4096);

            long origKey = seed;
            long key = origKey;
            long data;
            long data2;

            for (int x = 0; x < count; x++)
            {
                key += rand.Next() + 1;
                data = rand.Next();
                tree.AddData(key, data);
            }


            rand = new Random(seed);
            key = origKey;
            for (int x = 0; x < count; x++)
            {
                key += rand.Next() + 1;
                data = rand.Next();

                data2 = tree.GetData(key).Value;
                if (data2 != data)
                    throw new Exception();
            }

        }

    }
}
