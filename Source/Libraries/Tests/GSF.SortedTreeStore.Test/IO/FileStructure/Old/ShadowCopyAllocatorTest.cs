////******************************************************************************************************
////  ShadowCopyAllocatorTest.cs - Gbtc
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
////  1/4/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using GSF;
//using GSF.IO.FileStructure.Media;
//using NUnit.Framework;

//namespace GSF.IO.FileStructure.Test
//{
//    [TestFixture()]
//    public class ShadowCopyAllocatorTest
//    {
//        private static readonly int BlockSize;
//        private static readonly int BlockDataLength;
//        private static readonly int AddressesPerBlock;
//        private static int AddressesPerBlockSquare;
//        private static readonly int FirstSingleIndirectBlockIndex;
//        private static readonly int FirstDoubleIndirectBlockIndex;
//        private static readonly int FirstTripleIndirectIndex;
//        private static int LastAddressableBlockIndex;

//        static ShadowCopyAllocatorTest()
//        {
//            BlockSize = 4096;
//            BlockDataLength = BlockSize - FileStructureConstants.BlockFooterLength;
//            AddressesPerBlock = BlockDataLength / 4; //rounds down
//            AddressesPerBlockSquare = AddressesPerBlock * AddressesPerBlock;
//            FirstSingleIndirectBlockIndex = 1;
//            FirstDoubleIndirectBlockIndex = (int)Math.Min(int.MaxValue, FirstSingleIndirectBlockIndex + (long)AddressesPerBlock);
//            FirstTripleIndirectIndex = (int)Math.Min(int.MaxValue, FirstDoubleIndirectBlockIndex + (long)AddressesPerBlock * (long)AddressesPerBlock);
//            LastAddressableBlockIndex = (int)Math.Min(int.MaxValue, FirstTripleIndirectIndex + (long)AddressesPerBlock * (long)AddressesPerBlock * (long)AddressesPerBlock - 1);
//        }

//        [Test()]
//        public void Test()
//        {
//            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
//            DiskIo stream = DiskIo.CreateMemoryFile(Globals.MemoryPool, BlockSize);
//            FileHeaderBlock header = stream.LastCommittedHeader;
//            header = header.CloneEditable();
//            header.CreateNewFile(SubFileName.CreateRandom());
//            header.CreateNewFile(SubFileName.CreateRandom());
//            header.CreateNewFile(SubFileName.CreateRandom());
//            stream.CommitChanges(header);
//            TestWrite(stream, 0);
//            TestWrite(stream, 1);
//            TestWrite(stream, 2);
//            TestWrite(stream, 0);
//            TestWrite(stream, 1);
//            TestWrite(stream, 2);
//            header = stream.LastCommittedHeader;
//            Assert.IsTrue(true);
//            stream.Dispose();
//            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
//        }

//        internal static void TestWrite(DiskIo stream, int FileNumber)
//        {
//            FileHeaderBlock header = stream.LastCommittedHeader;
//            header = header.CloneEditable();
//            SubFileMetaData node = header.Files[FileNumber];
//            IndexParser parse = new IndexParser(BlockSize, header.SnapshotSequenceNumber, stream, node);
//            ShadowCopyAllocator shadow = new ShadowCopyAllocator(BlockSize, stream, header, node, parse);

//            int nextPage = header.LastAllocatedBlock + 1;
//            byte[] block = new byte[BlockSize];

//            shadow.ShadowDataBlock(0);
//            PositionData pd = parse.GetPositionData(0);
//            if (node.DirectBlock != nextPage)
//                throw new Exception();
//            if (parse.DataClusterAddress != nextPage)
//                throw new Exception();


//            stream.WriteToNewBlock(parse.DataClusterAddress, BlockType.DataBlock, (int)(pd.VirtualPosition / BlockDataLength), node.FileIdNumber, header.SnapshotSequenceNumber, block);


//            //should do nothing since the page has already been allocated
//            shadow.ShadowDataBlock(1024);
//            pd = parse.GetPositionData(1024);
//            if (node.DirectBlock != nextPage)
//                throw new Exception();
//            if (parse.DataClusterAddress != nextPage)
//                throw new Exception();
//            stream.WriteToNewBlock(parse.DataClusterAddress, BlockType.DataBlock, (int)(pd.VirtualPosition / BlockDataLength), node.FileIdNumber, header.SnapshotSequenceNumber, block);


//            //Allocate in the 3th indirect block
//            shadow.ShadowDataBlock(FirstTripleIndirectIndex * (long)BlockDataLength);
//            pd = parse.GetPositionData(FirstTripleIndirectIndex * (long)BlockDataLength);
//            if (node.DirectBlock != nextPage)
//                throw new Exception();
//            if (parse.DataClusterAddress != nextPage + 1)
//                throw new Exception();
//            if (parse.FirstIndirectBlockAddress != nextPage + 4)
//                throw new Exception();
//            if (parse.SecondIndirectBlockAddress != nextPage + 3)
//                throw new Exception();
//            if (parse.ThirdIndirectBlockAddress != nextPage + 2)
//                throw new Exception();
//            stream.WriteToNewBlock(parse.DataClusterAddress, BlockType.DataBlock, (int)(pd.VirtualPosition / BlockDataLength), node.FileIdNumber, header.SnapshotSequenceNumber, block);

//            //if (node.DirectCluster != nextPage)
//            //    throw new Exception();
//            //if (parse.DataClusterAddress != nextPage + 1)
//            //    throw new Exception();
//            //if (parse.FirstIndirectBlockAddress != nextPage + 5)
//            //    throw new Exception();
//            //if (parse.SecondIndirectBlockAddress != nextPage + 4)
//            //    throw new Exception();
//            //if (parse.ThirdIndirectBlockAddress != nextPage + 3)
//            //    throw new Exception();
//            //if (parse.ForthIndirectBlockAddress != nextPage + 2)
//            //    throw new Exception();
//            stream.CommitChanges(header);
//        }
//    }
//}