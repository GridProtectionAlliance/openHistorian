using System;
using NUnit.Framework;

namespace GSF.Collections.Test
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

        private static void TestSequential(int count)
        {
            BitArray array = new BitArray(true, count);
            for (int x = 0; x < count; x++)
            {
                if (!array.GetBit(x))
                    throw new Exception("each bit should be set");
            }

            array = new BitArray(false, count);
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

        private static void TestSequentialInv(int count)
        {
            BitArray array = new BitArray(false, count);
            for (int x = 0; x < count; x++)
            {
                if (array.GetBit(x))
                    throw new Exception("each bit should be cleared");
            }

            array = new BitArray(true, count);
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

        private static void TestRandom(int seed)
        {
            Random rand = new Random(seed);
            int count = rand.Next(1000000);

            bool[] tmp = new bool[count];
            BitArray array = new BitArray(false, count);
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

        [Test()]
        public void TestCounts()
        {
            BitArray bit = new BitArray(true, 1000);

            for (int x = 0; x < 1000; x++)
            {
                Assert.AreEqual(1000, bit.Count);
                Assert.AreEqual(1000, bit.SetCount);
                Assert.AreEqual(0, bit.ClearCount);
                bit[x] = true;
            }

            for (int x = 0; x < 1000; x++)
            {
                Assert.AreEqual(1000, bit.Count);
                Assert.AreEqual(1000 - x, bit.SetCount);
                Assert.AreEqual(x, bit.ClearCount);
                bit[x] = false;
            }


            for (int x = 0; x < 1000; x++)
            {
                Assert.AreEqual(1000, bit.Count);
                Assert.AreEqual(0, bit.SetCount);
                Assert.AreEqual(1000, bit.ClearCount);
                bit[x] = false;
            }

            for (int x = 0; x < 1000; x++)
            {
                Assert.AreEqual(1000, bit.Count);
                Assert.AreEqual(x, bit.SetCount);
                Assert.AreEqual(1000 - x, bit.ClearCount);
                bit[x] = true;
            }
        }
    }
}