////******************************************************************************************************
////  DiskIOMemoryStreamTest.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://opensource.org/licenses/MIT
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  1/1/2012 - Steven E. Chisholm
////       Generated original version of source code.
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace openHistorian.FileStructure
//{
//    [TestClass()]
//    public class DiskIOEnhanced2Test2
//    {
//        [TestMethod()]
//        public void Test()
//        {
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//            DiskIOEnhanced2Test.Test();
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//            Assert.IsTrue(true);
//        }
//    }

//    internal class DiskIOEnhanced2Test : DiskIoEnhanced
//    {
//        public static void Test()
//        {
//            TestAllReadStates();
//            TestNewMethods();
//        }
//        unsafe static void TestNewMethods()
//        {
//            DiskIoEnhanced disk = new DiskIoEnhanced();
//            IMemoryUnit buffer = disk.GetMemoryUnit();
//            if (disk.AquireBlockForRead(1, BlockType.DataBlock, 1, 2, 3, buffer) != IoReadState.ChecksumInvalidBecausePageIsNull)
//                throw new Exception("Outside Bounds");
//            disk.AquireBlockForWrite(1, buffer);

//            *(long*)buffer.Pointer = 23425;
//            *(long*)(buffer.Pointer + 20) = 23425;
//            disk.WriteBlock(BlockType.DataBlock, 1, 2, 3, buffer);

//            byte[] data = new byte[4096];

//            if (disk.AquireBlockForRead(1, BlockType.DataBlock, 1, 2, 3, buffer) != IoReadState.Valid)
//                throw new Exception();

//            if (disk.ReadBlock(1, BlockType.DataBlock, 1, 2, 3, data) != IoReadState.Valid)
//                throw new Exception();


//            for (int x = 0; x < ArchiveConstants.BlockSize; x++)
//            {
//                if (data[x] != buffer.Pointer[x])
//                    throw new Exception();
//            }
//            buffer.Dispose();
//            disk.Dispose();

//        }
//        static void TestAllReadStates()
//        {
//            DiskIOEnhanced2Test stream = new DiskIOEnhanced2Test();
//            for (int x = 0; x < 2; x++)
//            {
//                DiskIOEnhancedTest.TestAllReadStatesExceptInvalid(stream);
//                TestChecksumInvalid(stream);
//            }
//            stream.Dispose();
//        }
//        static void TestChecksumInvalid(DiskIOEnhanced2Test stream)
//        {
//            //IoReadState readState;
//            //int seed = (int)DateTime.Now.Ticks;
//            //byte[] buffer = GenerateRandomDataBlock(seed);
//            //int currentBlock = (int)(stream.FileSize / ArchiveConstants.BlockSize);

//            //stream.WriteBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);

//            //byte[] tmp = new byte[1];
//            ////stream.m_stream.Read(currentBlock * ArchiveConstants.BlockSize, tmp, 0, 1);
//            //tmp[0] = (byte)(tmp[0] + 1);
//            ////stream.m_stream.Write(currentBlock * ArchiveConstants.BlockSize, tmp, 0, 1);

//            //readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);
//            //if (readState != IoReadState.ChecksumInvalid)
//            //    throw new Exception();
//        }

//        public static byte[] GenerateRandomDataBlock(int seed)
//        {
//            Random rand = new Random(seed);
//            byte[] buffer = new byte[ArchiveConstants.BlockSize];
//            for (int x = 0; x < ArchiveConstants.DataBlockDataLength; x++)
//                buffer[x] = (byte)rand.Next();
//            return buffer;
//        }
//    }
//}

