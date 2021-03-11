//******************************************************************************************************
//  IndexMapperTest.cs - Gbtc
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
//  1/4/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.IO.FileStructure.Test
{
    [TestFixture()]
    public class IndexMapperTest
    {
        private static readonly uint BlockSize;
        private static readonly uint BlockDataLength;
        private static readonly uint AddressesPerBlock;

        static IndexMapperTest()
        {
            BlockSize = 256;
            BlockDataLength = BlockSize - FileStructureConstants.BlockFooterLength;
            AddressesPerBlock = BlockDataLength / 4; //rounds down
        }

        //[Test()]
        //public void Test()
        //{
        //    Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
        //    //Class tested to approximately 1.7 million calculations per second at an inode depth of 4
        //    //That's 4kb * 1.7 million/sec or 6.8GB/sec of data.
        //    //TestSpeed();

        //    TestMethod1();
        //    Assert.IsTrue(true);
        //    Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
        //}

        [Test()]
        public void Benchmark()
        {
            MemoryPoolTest.TestMemoryLeak();
            Benchmark(0, "Direct\t");
            Benchmark(AddressesPerBlock - 1, "Single\t");
            Benchmark(AddressesPerBlock * AddressesPerBlock - 1, "Double\t");
            Benchmark(AddressesPerBlock * AddressesPerBlock * AddressesPerBlock - 1, "Triple\t");
            Benchmark(AddressesPerBlock * AddressesPerBlock * AddressesPerBlock * AddressesPerBlock - 1, "Last\t");
            MemoryPoolTest.TestMemoryLeak();
        }

        public void Benchmark(uint page, string text)
        {
            Stopwatch sw = new Stopwatch();

            IndexMapper map = new IndexMapper((int)BlockSize);

            System.Console.WriteLine(text +
                              StepTimer.Time(10, () =>
                              {
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                                  map.MapPosition(page);
                              }));
        }

        [Test]
        public static void TestMethod1()
        {
            MemoryPoolTest.TestMemoryLeak();
            int blockSize = 128;
            IndexMapper map = new IndexMapper(blockSize);
            CheckValues check = new CheckValues(blockSize);

            uint lastAddress = (uint)Math.Min(uint.MaxValue, check.BlocksPerPage * (long)check.BlocksPerPage * check.BlocksPerPage * check.BlocksPerPage - 1);

            //this line is to shortcut so the test is less comprehensive.
            for (uint x = 0; x <= lastAddress; x++)
            {
                map.MapPosition(x);
                check.Check(map, x);
            }
            MemoryPoolTest.TestMemoryLeak();
        }

        private class CheckValues
        {
            public int FirstRedirectOffset;
            public int SecondRedirectOffset;
            public int ThirdRedirectOffset;
            public int FourthRedirectOffset;
            public readonly int BlocksPerPage;

            public int BaseVirtualAddressIndexValue;

            public CheckValues(int blockSize)
            {
                BlocksPerPage = (blockSize - FileStructureConstants.BlockFooterLength) / 4;
            }

            public void Check(IndexMapper map, uint address)
            {
                if (FirstRedirectOffset != map.FirstIndirectOffset)
                    throw new Exception();
                if (SecondRedirectOffset != map.SecondIndirectOffset)
                    throw new Exception();
                if (ThirdRedirectOffset != map.ThirdIndirectOffset)
                    throw new Exception();

                if (BaseVirtualAddressIndexValue != map.BaseVirtualAddressIndexValue)
                    throw new Exception();
                if (address != map.BaseVirtualAddressIndexValue)
                    throw new Exception();

                Increment();
            }

            private void Increment()
            {
                BaseVirtualAddressIndexValue++;
                FourthRedirectOffset++;
                if (FourthRedirectOffset == BlocksPerPage)
                {
                    FourthRedirectOffset = 0;
                    ThirdRedirectOffset++;
                    if (ThirdRedirectOffset == BlocksPerPage)
                    {
                        ThirdRedirectOffset = 0;
                        SecondRedirectOffset++;
                        if (SecondRedirectOffset == BlocksPerPage)
                        {
                            SecondRedirectOffset = 0;
                            FirstRedirectOffset++;
                        }
                    }
                }
            }
        }
    }
}