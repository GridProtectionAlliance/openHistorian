//******************************************************************************************************
//  MemoryPoolStreamPerformanceTest.cs - Gbtc
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
//  5/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using NUnit.Framework;
using openHistorian;

namespace GSF.IO.Unmanaged.Test
{
    [TestFixture]
    public class MemoryPoolStreamPerformanceTest
    {
        [Test]
        public void TestBlocksPerSecond()
        {
            //UnmanagedMemory.Memory.UseLargePages = true;
            DebugStopwatch sw = new DebugStopwatch();
            using (MemoryPoolStream ms = new MemoryPoolStream())
            {
                using (BinaryStreamIoSessionBase io = ms.CreateIoSession())
                {
                    BlockArguments args = new BlockArguments();
                    args.Position = ms.BlockSize * 2000L - 1;
                    args.IsWriting = true;

                    io.GetBlock(args);

                    double sec = sw.TimeEvent(() =>
                        {
                            for (int y = 0; y < 100; y++)
                                for (int x = 0; x < 2000; x++)
                                {
                                    args.Position = (long)x * ms.BlockSize;
                                    io.GetBlock(args);
                                }
                        });

                    System.Console.WriteLine("Get Blocks: " + (200000 / sec / 1000000).ToString("0.00 Million Per Second"));
                }
            }


        }
    }
}

