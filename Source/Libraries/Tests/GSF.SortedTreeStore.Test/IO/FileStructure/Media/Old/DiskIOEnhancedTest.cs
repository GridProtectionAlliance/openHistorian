////******************************************************************************************************
////  DiskIOTest.cs - Gbtc
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
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace openHistorian.FileStructure
//{
//    internal class DiskIOEnhancedTest
//    {
//        internal static void TestAllReadStatesExceptInvalid(DiskIoEnhanced stream)
//        {
//            TestReadPastTheEndOfTheFile(stream);
//            TestChecksumInvalidBecausePageIsNull(stream);
//            TestPageNewerThanSnapshotSequenceNumber(stream);
//            TestFileIDNumberDidNotMatch(stream);
//            TestIndexNumberMissmatch(stream);
//            TestBlockTypeMismatch(stream);
//            TestResize(stream);
//        }

//        static void TestResize(DiskIoEnhanced stream)
//        {
//            //IoReadState readState;
//            //long oldFileSize = stream.FileSize;
//            //int seed = (int)DateTime.Now.Ticks;
//            //byte[] buffer = GenerateRandomDataBlock(seed);
//            //int currentBlock = (int)(stream.FileSize / ArchiveConstants.BlockSize);

//            //using (var buffer = stream.GetMemoryUnit())
//            //{


//            //    stream.WriteBlock(currentBlock, BlockType.IndexIndirect, 1, 2, 3, buffer);
//            //    stream.SetFileLength(0, currentBlock + 1);
//            //    readState = stream.ReadBlock(currentBlock, BlockType.IndexIndirect, 1, 2, 3, buffer);
//            //    if (readState != IoReadState.Valid)
//            //        throw new Exception();

//            //}
//            //stream.SetFileLength(0, currentBlock);

//            //readState = stream.ReadBlock(currentBlock, BlockType.IndexIndirect, 1, 2, 3, buffer);
//            //if (readState != IoReadState.ChecksumInvalidBecausePageIsNull)
//            //    throw new Exception();

//            //if (stream.FileSize != oldFileSize)
//            //    throw new Exception();

//            //stream.SetFileLength(oldFileSize, 0);
//            //if (stream.FileSize != oldFileSize)
//            //    throw new Exception();

//            //stream.SetFileLength(oldFileSize + 1, 0);
//            //if (stream.FileSize - ArchiveConstants.BlockSize != oldFileSize)
//            //    throw new Exception();
//        }

//        static void TestBlockTypeMismatch(DiskIoEnhanced stream)
//        {
//            IoReadState readState;
//            int seed = (int)DateTime.Now.Ticks;
//            byte[] buffer = GenerateRandomDataBlock(seed);
//            int currentBlock = (int)(stream.FileSize / ArchiveConstants.BlockSize);


//            stream.WriteBlock(currentBlock, BlockType.IndexIndirect, 1, 2, 3, buffer);
//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);

//            if (readState != IoReadState.BlockTypeMismatch)
//                throw new Exception();

//            readState = stream.ReadBlock(currentBlock, BlockType.DataBlock, 1, 2, 3, buffer);
//            if (readState != IoReadState.BlockTypeMismatch)
//                throw new Exception();


//        }

//        static void TestIndexNumberMissmatch(DiskIoEnhanced stream)
//        {
//            IoReadState readState;
//            int seed = (int)DateTime.Now.Ticks;
//            byte[] buffer = GenerateRandomDataBlock(seed);
//            int currentBlock = (int)(stream.FileSize / ArchiveConstants.BlockSize);

//            stream.WriteBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);
//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 0, 2, 3, buffer);

//            if (readState != IoReadState.IndexNumberMissmatch)
//                throw new Exception();

//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 3, 2, 3, buffer);
//            if (readState != IoReadState.IndexNumberMissmatch)
//                throw new Exception();
//        }

//        static void TestFileIDNumberDidNotMatch(DiskIoEnhanced stream)
//        {
//            IoReadState readState;
//            int seed = (int)DateTime.Now.Ticks;
//            byte[] buffer = GenerateRandomDataBlock(seed);
//            int currentBlock = (int)(stream.FileSize / ArchiveConstants.BlockSize);

//            stream.WriteBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);
//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 3, 3, buffer);

//            if (readState != IoReadState.FileIdNumberDidNotMatch)
//                throw new Exception();

//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 1, 3, buffer);
//            if (readState != IoReadState.FileIdNumberDidNotMatch)
//                throw new Exception();
//        }

//        static void TestPageNewerThanSnapshotSequenceNumber(DiskIoEnhanced stream)
//        {
//            //Writing sequence number 3, reading both 2 and 5.  2 should fail, 5 should not.
//            IoReadState readState;
//            int seed = (int)DateTime.Now.Ticks;
//            byte[] buffer = GenerateRandomDataBlock(seed);
//            int currentBlock = (int)(stream.FileSize / ArchiveConstants.BlockSize);

//            stream.WriteBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);
//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 2, buffer);

//            if (readState != IoReadState.PageNewerThanSnapshotSequenceNumber)
//                throw new Exception();

//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 5, buffer);
//            if (readState != IoReadState.Valid)
//                throw new Exception();
//        }

//        static void TestChecksumInvalidBecausePageIsNull(DiskIoEnhanced stream)
//        {
//            IoReadState readState;
//            int seed = (int)DateTime.Now.Ticks;
//            byte[] buffer = GenerateRandomDataBlock(seed);
//            int currentBlock = (int)(stream.FileSize / ArchiveConstants.BlockSize);

//            stream.WriteBlock(currentBlock + 1, BlockType.FileAllocationTable, 1, 2, 3, buffer);

//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);
//            if (readState != IoReadState.ChecksumInvalidBecausePageIsNull)
//                throw new Exception();
//        }
//        static void TestReadPastTheEndOfTheFile(DiskIoEnhanced stream)
//        {
//            //IO Status was changed to remove this flag.  Instead it returns a null page.

//            IoReadState readState;
//            int seed = (int)DateTime.Now.Ticks;
//            byte[] buffer = GenerateRandomDataBlock(seed);
//            int currentBlock = (int)(stream.FileSize / ArchiveConstants.BlockSize);

//            //Testing IOReadState.ReadPastThenEndOfTheFile
//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);
//            if (readState != IoReadState.ChecksumInvalidBecausePageIsNull)
//                throw new Exception();
//            stream.WriteBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);

//            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);
//            if (readState != IoReadState.Valid)
//                throw new Exception();

//            readState = stream.ReadBlock(currentBlock + 1, BlockType.FileAllocationTable, 1, 2, 3, buffer);
//            if (readState != IoReadState.ChecksumInvalidBecausePageIsNull)
//                throw new Exception();
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

