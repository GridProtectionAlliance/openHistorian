using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged
{
    static class BitArrayTest
    {
        public static void Test()
        {
            Random rand = new Random();
            int seed = rand.Next();

            TestSequential(0);
            TestSequential(rand.Next(100000) + 10);
            TestRandom(seed);
            Benchmark();
        }
        static void TestSequential(int count)
        {
            BitArray array = new BitArray(count, true);
            for (int x = 0; x < count; x++)
            {
                if (!array.GetBit(x))
                    throw new Exception("each bit should be set");
            }

            array = new BitArray(count,false);
            for (int x = 0; x < count; x++)
            {
                if (array.GetBit(x))
                    throw new Exception("each bit should be cleared");
            }
            for (int x = 0; x < count; x++)
            {
                array.SetBit(x);
                if (!array.GetBit(x))
                    throw new Exception("each bit should be cleared");
                array.ClearBit(x);
                if (array.GetBit(x))
                    throw new Exception("each bit should be cleared");
                array.SetBit(x);

                if (array.FindClearedBit() != (x == count - 1 ? -1 : x + 1))
                    throw new Exception();
            }

        }

        static void TestRandom(int seed)
        {
            Random rand = new Random(seed);
            int count = rand.Next(1000000);

            bool[] tmp = new bool[count];
            BitArray array = new BitArray(count,false);
            for (int x = 0; x < count << 1; x++)
            {
                int index = rand.Next(count);
                array.SetBit(index);
                tmp[index] = true;
            }
            for (int x = 0; x < count; x++)
            {
                if (tmp[x] != array.GetBit(x))
                    throw new Exception();
            }
        }

        static void Benchmark()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            Stopwatch sw3 = new Stopwatch();
            Stopwatch sw4 = new Stopwatch();
            Stopwatch sw5 = new Stopwatch();
            Stopwatch sw6 = new Stopwatch();

            const int count = 20 * 1024 * 1024;
            //20 million, That's like 120GB
            BitArray array = new BitArray(count,false);

            sw1.Start();
            for (int x = 0; x<count; x++)
            {
                array.SetBit(x);
            }
            sw1.Stop();

            sw2.Start();
            for (int x = 0; x < count; x++)
            {
                array.SetBit(x);
            }
            sw2.Stop();

            sw3.Start();
            for (int x = 0; x < count; x++)
            {
                array.ClearBit(x);
            }
            sw3.Stop();

            sw4.Start();
            for (int x = 0; x < count; x++)
            {
                array.ClearBit(x);
            }
            sw4.Stop();

            sw5.Start();
            for (int x = 0; x < count; x++)
            {
                array.GetBit(x);
            }
            sw5.Stop();

            for (int x = 0; x < count-1; x++)
            {
                array.SetBit(x);
            }
            
            sw6.Start();
            for (int x = 0; x < count; x++)
            {
                array.FindClearedBit();
            }
            sw6.Stop();

            //MessageBox.Show((count / sw1.Elapsed.TotalSeconds / 1000000).ToString());
            //MessageBox.Show((count / sw2.Elapsed.TotalSeconds / 1000000).ToString());
            //MessageBox.Show((count / sw3.Elapsed.TotalSeconds / 1000000).ToString());
            //MessageBox.Show((count / sw4.Elapsed.TotalSeconds / 1000000).ToString());
            //MessageBox.Show((count / sw5.Elapsed.TotalSeconds / 1000000).ToString());
            //MessageBox.Show((count / sw6.Elapsed.TotalSeconds / 1000000).ToString());
            


        }


    }
}
