//******************************************************************************************************
//  BitArrayTest.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  3/20/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Linq;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.Collections.Test
{
    [TestFixture()]
    public class BitArrayTest
    {
        [Test()]
        public void BitArray()
        {
            MemoryPoolTest.TestMemoryLeak();
            Random rand = new Random();
            int seed = rand.Next();

            TestSequential(rand.Next(30));
            TestSequentialInv(rand.Next(30));
            TestSequential(rand.Next(100000) + 10);
            TestSequentialInv(rand.Next(100000) + 10);
            TestRandom(seed);
            Assert.IsTrue(true);
            MemoryPoolTest.TestMemoryLeak();
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
            MemoryPoolTest.TestMemoryLeak();
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
            MemoryPoolTest.TestMemoryLeak();
        }
      
        [Test]
        public void FindBits()
        {
            BitArray arraySet = new BitArray(true, 15);
            BitArray arrayClear = new BitArray(false, 15);

            if (arraySet.FindClearedBit() != -1)
                throw new Exception();
            if (arrayClear.FindSetBit() != -1)
                throw new Exception();

            int count = 0;

            foreach (int set in arraySet.GetAllSetBits())
            {
                if (set != count)
                    throw new Exception();
                count++;
            }

            count = 0;
            foreach (int set in arrayClear.GetAllClearedBits())
            {
                if (set != count)
                    throw new Exception();
                count++;
            }


            if (arrayClear.GetAllSetBits().Count() != 0)
                throw new Exception();


            if (arraySet.GetAllClearedBits().Count() != 0)
                throw new Exception();

            arraySet.EnsureCapacity(300);
            if (!arraySet.AreAllBitsSet(62, 200))
                throw new Exception();

            if (arraySet.ClearCount != 0)
                throw new Exception();

        }

        [Test]
        public void GetSetBits()
        {
            BitArray array = new BitArray(false, 15);
         
            for (int x = 0; x < 15; x++)
            {
                if (array[x])
                    throw new Exception();
            }

            for (int x = 0; x < 15; x++)
            {
                if (array.GetBitUnchecked(x))
                    throw new Exception();
            }

            array[1] = true;
            if(array.TrySetBit(1))
                throw new Exception();

            if (!array.TrySetBit(2))
                throw new Exception();

            if (array.TrySetBit(2))
                throw new Exception();

            if (!array.TryClearBit(2))
                throw new Exception();

            if (array.TryClearBit(2))
                throw new Exception();

            //Here, bit 1 is set. 

            if (!array.AreAllBitsSet(1,1))
                throw new Exception();

            if (array.AreAllBitsSet(1,3))
                throw new Exception();

            if (!array.AreAllBitsCleared(2,8))
                throw new Exception();

            if (array.AreAllBitsCleared(0, 8))
                throw new Exception();


            array.SetBits(1,8);
            if (!array.AreAllBitsSet(1,8))
                throw new Exception();

            array.ClearBits(1, 8);
            if (!array.AreAllBitsCleared(1, 8))
                throw new Exception();


            array.EnsureCapacity(62);

            if (!array.AreAllBitsCleared(1, 8))
                throw new Exception();

            array.EnsureCapacity(1000);
            if (!array.AreAllBitsCleared(62, 500))
                throw new Exception();
        }
    }
}