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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.File
{
    internal class IndexMapperTest
    {
        public static void Test()
        {
            //Class tested to approximately 1.7 million calculations per second at an inode depth of 4
            //That's 4kb * 1.7 million/sec or 6.8GB/sec of data.
            //TestSpeed();

            TestMethod1(1);
        }

        public static void TestSpeed()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            IndexMapper map = new IndexMapper(1);
            long page = (long)ArchiveConstants.LastTripleIndirectBlockIndex * ArchiveConstants.DataBlockDataLength;

            for (int x = 0; x < 10000000; x++)
            {
                page += ArchiveConstants.DataBlockDataLength;
                map.SetPosition(page);
            }

            sw.Stop();
            System.Windows.Forms.MessageBox.Show(((int.MaxValue / ArchiveConstants.DataBlockDataLength) / sw.Elapsed.TotalSeconds).ToString());
        }

        public static void TestMethod1(uint pagesPerBlock)
        {
            IndexMapper map = new IndexMapper(pagesPerBlock);
            CheckValues check = new CheckValues();
            long lastAddress = Math.Min((long)uint.MaxValue * ArchiveConstants.DataBlockDataLength, ArchiveConstants.MaxFileSystemSize);

            //this line is to shortcut so the test is less comprehensive.
            lastAddress = (long)ArchiveConstants.DataBlockDataLength * (long)ArchiveConstants.LastDoubleIndirectBlockIndex + 100;

            for (long x = 0; x < lastAddress; x += ArchiveConstants.DataBlockDataLength)
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
            public int ForthRedirectOffset = -1;
            public uint FirstRedirectBaseIndex;
            public uint SecondRedirectBaseIndex;
            public uint ThirdRedirectBaseIndex;
            public uint ForthRedirectBaseIndex;
            public long Length;
            public uint BaseVirtualAddressIndexValue;
            public long BaseVirtualAddress;

            public void Check(IndexMapper map, long address)
            {
                Length = ArchiveConstants.DataBlockDataLength;

                if (RedirectNumber != map.IndirectNumber)
                    throw new Exception();
                if (FirstRedirectOffset != map.FirstIndirectOffset)
                    throw new Exception();
                if (SecondRedirectOffset != map.SecondIndirectOffset)
                    throw new Exception();
                if (ThirdRedirectOffset != map.ThirdIndirectOffset)
                    throw new Exception();
                if (ForthRedirectOffset != map.ForthIndirectOffset)
                    throw new Exception();

                if (BaseVirtualAddressIndexValue != map.BaseVirtualAddressIndexValue)
                    throw new Exception();
                if (BaseVirtualAddress != map.BaseVirtualAddress)
                    throw new Exception();
                if (Length != map.Length)
                    throw new Exception();

                FirstRedirectBaseIndex = map.FirstIndirectBaseIndex;
                SecondRedirectBaseIndex = map.SecondIndirectBaseIndex;
                ThirdRedirectBaseIndex = map.ThirdIndirectBaseIndex;
                ForthRedirectBaseIndex = map.ForthIndirectBaseIndex;


                Increment();
            }

            public void Increment()
            {
                BaseVirtualAddressIndexValue++;
                BaseVirtualAddress += ArchiveConstants.DataBlockDataLength;
                switch (RedirectNumber)
                {
                    case 0:
                        RedirectNumber = 1;
                        FirstRedirectOffset = 0;
                        break;
                    case 1:
                        FirstRedirectOffset += 4;
                        if (FirstRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            RedirectNumber = 2;
                            FirstRedirectOffset = 0;
                            SecondRedirectOffset = 0;
                        }
                        break;
                    case 2:
                        SecondRedirectOffset += 4;
                        if (SecondRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            FirstRedirectOffset += 4;
                            SecondRedirectOffset = 0;
                        }
                        if (FirstRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            RedirectNumber = 3;
                            FirstRedirectOffset = 0;
                            SecondRedirectOffset = 0;
                            ThirdRedirectOffset = 0;
                        }
                        break;
                    case 3:
                        ThirdRedirectOffset += 4;
                        if (ThirdRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            SecondRedirectOffset += 4;
                            ThirdRedirectOffset = 0;
                        }
                        if (SecondRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            FirstRedirectOffset += 4;
                            SecondRedirectOffset = 0;
                        }
                        if (FirstRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            RedirectNumber = 4;
                            FirstRedirectOffset = 0;
                            SecondRedirectOffset = 0;
                            ThirdRedirectOffset = 0;
                            ForthRedirectOffset = 0;
                        }
                        break;
                    case 4:
                        ForthRedirectOffset += 4;
                        if (ForthRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            ThirdRedirectOffset += 4;
                            ForthRedirectOffset = 0;
                        }
                        if (ThirdRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            SecondRedirectOffset += 4;
                            ThirdRedirectOffset = 0;
                        }
                        if (SecondRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            FirstRedirectOffset += 4;
                            SecondRedirectOffset = 0;
                        }
                        if (FirstRedirectOffset == ArchiveConstants.AddressesPerBlock * 4)
                        {
                            throw new Exception();
                        }
                        break;
                }
            }
        }
    }
}
