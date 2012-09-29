//******************************************************************************************************
//  IndexMapperTest.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace openHistorian.V2.FileStructure.Test
{
    [TestClass()]
    public class IndexMapperTest
    {
        [TestMethod()]
        public void Test()
        {
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
            //Class tested to approximately 1.7 million calculations per second at an inode depth of 4
            //That's 4kb * 1.7 million/sec or 6.8GB/sec of data.
            //TestSpeed();

            TestMethod1();
            Assert.IsTrue(true);
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
        }


        public static void TestSpeed()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            IndexMapper map = new IndexMapper();
            long page = (long)FileStructureConstants.LastAddressableBlockIndex * FileStructureConstants.DataBlockDataLength;

            for (int x = 0; x < 10000000; x++)
            {
                page += FileStructureConstants.DataBlockDataLength;
                map.SetPosition(page);
            }

            sw.Stop();
            System.Windows.Forms.MessageBox.Show(((int.MaxValue / FileStructureConstants.DataBlockDataLength) / sw.Elapsed.TotalSeconds).ToString());
        }

        public static void TestMethod1()
        {
            IndexMapper map = new IndexMapper();
            CheckValues check = new CheckValues();
            long lastAddress = Math.Min((long)int.MaxValue * FileStructureConstants.DataBlockDataLength, FileStructureConstants.LastAddressableBlockIndex * (long)FileStructureConstants.DataBlockDataLength);

            //this line is to shortcut so the test is less comprehensive.
            lastAddress = (long)FileStructureConstants.DataBlockDataLength * (long)FileStructureConstants.FirstTripleIndirectIndex + 100;

            for (long x = 0; x < lastAddress; x += FileStructureConstants.DataBlockDataLength)
            {
                map.SetPosition(x);
                check.Check(map, x);
            }
        }

        private class CheckValues
        {
            public int RedirectNumber = 0;
            public int FirstRedirectOffset = -1;
            public int SecondRedirectOffset = -1;
            public int ThirdRedirectOffset = -1;
            public uint BaseVirtualAddressIndexValue;
            public long BaseVirtualAddress;

            public void Check(IndexMapper map, long address)
            {

                if (RedirectNumber != map.IndirectNumber)
                    throw new Exception();
                if (FirstRedirectOffset != map.FirstIndirectOffset)
                    throw new Exception();
                if (SecondRedirectOffset != map.SecondIndirectOffset)
                    throw new Exception();
                if (ThirdRedirectOffset != map.ThirdIndirectOffset)
                    throw new Exception();

                if (BaseVirtualAddressIndexValue != map.BaseVirtualAddressIndexValue)
                    throw new Exception();
                if (BaseVirtualAddress != map.BaseVirtualAddress)
                    throw new Exception();
                Increment();
            }

            void Increment()
            {
                BaseVirtualAddressIndexValue++;
                BaseVirtualAddress += FileStructureConstants.DataBlockDataLength;
                switch (RedirectNumber)
                {
                    case 3:
                        ThirdRedirectOffset += 4;
                        if (ThirdRedirectOffset == FileStructureConstants.AddressesPerBlock * 4)
                        {
                            SecondRedirectOffset += 4;
                            ThirdRedirectOffset = 0;
                        }
                        if (SecondRedirectOffset == FileStructureConstants.AddressesPerBlock * 4)
                        {
                            FirstRedirectOffset += 4;
                            SecondRedirectOffset = 0;
                        }
                        if (FirstRedirectOffset == FileStructureConstants.AddressesPerBlock * 4)
                        {
                            RedirectNumber = 4;
                            FirstRedirectOffset = 0;
                            SecondRedirectOffset = 0;
                            ThirdRedirectOffset = 0;
                        }
                        break;
                    case 2:
                        SecondRedirectOffset += 4;
                        if (SecondRedirectOffset == FileStructureConstants.AddressesPerBlock * 4)
                        {
                            FirstRedirectOffset += 4;
                            SecondRedirectOffset = 0;
                        }
                        if (FirstRedirectOffset == FileStructureConstants.AddressesPerBlock * 4)
                        {
                            RedirectNumber = 3;
                            FirstRedirectOffset = 0;
                            SecondRedirectOffset = 0;
                            ThirdRedirectOffset = 0;
                        }
                        break;
                    case 1:
                        FirstRedirectOffset += 4;
                        if (FirstRedirectOffset == FileStructureConstants.AddressesPerBlock * 4)
                        {
                            RedirectNumber = 2;
                            FirstRedirectOffset = 0;
                            SecondRedirectOffset = 0;
                        }
                        break;
                    case 0:
                        RedirectNumber = 1;
                        FirstRedirectOffset = 0;
                        break;
                }
            }
        }
    }
}
