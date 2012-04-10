using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace openHistorian.V2.Unmanaged.Generic
{
    class NodeTestPageSize
    {
        internal static void Test()
        {
            TestMultiLevelLongRandomSpecific(1, 4096);
            //TestMultiLevelLongRandomSpecific(1, 512 * 1);
            //TestMultiLevelLongRandomSpecific(1, 512 * 2);
            //TestMultiLevelLongRandomSpecific(1, 512 * 4);
            //TestMultiLevelLongRandomSpecific(1, 512 * 8);
            //TestMultiLevelLongRandomSpecific(1, 512 * 16);
            //TestMultiLevelLongRandomSpecific(1, 512 * 32);
        }

        static void TestMultiLevelLongRandomSpecific(int seed, int nodeSize)
        {
            const int countTime = 1000;
            const int countKey = 1000;
            const int count = countTime * countKey;

            Random rand;
            rand = new Random(seed);

            var ms = new MemoryStream();
            ms.Position = 6000 * 4096;
            ms.Write(new byte[] { 1 }, 0, 1);
            var bs = new BinaryStream(ms);
            var tree = new BPlusTreeLongLong(bs, nodeSize);

            long key;
            long data;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < countTime; x++)
            {
                for (int y = 0; y < countKey; y++)
                {
                    key = (long)rand.Next() << 30 | rand.Next();
                    data = rand.Next();
                    tree.AddData(key, data);
                }
            }

            sw.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            rand = new Random(seed);

            for (int x = 0; x < countTime; x++)
            {
                for (int y = 0; y < countKey; y++)
                {
                    key = (long)rand.Next() << 30 | rand.Next();
                    data = rand.Next();

                    data2 = tree.GetData(key);
                    if (data2 != data)
                        throw new Exception();
                }
            }


            sw2.Stop();
            MessageBox.Show(nodeSize + " " + (count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
        }
   

    }
}
