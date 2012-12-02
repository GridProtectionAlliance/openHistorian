using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.V2.Collections;

namespace openHistorian.V2.UnmanagedMemory.Test
{
    [TestFixture()]
    public class BitArrayTest
    {
        //ToDo: Add another test procedure to properly test for FindNextBit function.
        [Test()]
        public void BitArray()
        {
            Random rand = new Random();
            int seed = rand.Next();

            TestSequential(rand.Next(30));
            TestSequentialInv(rand.Next(30));
            TestSequential(rand.Next(100000) + 10);
            TestSequentialInv(rand.Next(100000) + 10);
            TestRandom(seed);
            Assert.IsTrue(true);
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

        static void TestSequentialInv(int count)
        {
            BitArray array = new BitArray(count, false);
            for (int x = 0; x < count; x++)
            {
                if (array.GetBit(x))
                    throw new Exception("each bit should be cleared");
            }

            array = new BitArray(count, true);
            for (int x = 0; x < count; x++)
            {
                if (!array.GetBit(x))
                    throw new Exception("each bit should be set");
            }
            for (int x = 0; x < count; x++)
            {
                array.ClearBit(x);
                if (array.GetBit(x))
                    throw new Exception("each bit should be cleared");
                array.SetBit(x);
                if (!array.GetBit(x))
                    throw new Exception("each bit should be cleared");
                array.ClearBit(x);

                if (array.FindSetBit() != (x == count - 1 ? -1 : x + 1))
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
  

    }
}
