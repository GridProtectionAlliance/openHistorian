//******************************************************************************************************
//  ShadowCopyAllocatorTest.cs - Gbtc
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
    internal class ShadowCopyAllocatorTest
    {
        public static void Test()
        {
            DiskIoEnhanced stream = new DiskIoEnhanced();
            FileAllocationTable header = FileAllocationTable.CreateFileAllocationTable(stream);
            header.CreateNewFile(Guid.NewGuid());
            header.CreateNewFile(Guid.NewGuid());
            header.CreateNewFile(Guid.NewGuid());
            header.WriteToFileSystem(stream);
            TestWrite(stream, 0);
            TestWrite(stream, 1);
            TestWrite(stream, 2);
            TestWrite(stream, 0);
            TestWrite(stream, 1);
            TestWrite(stream, 2);
            header = FileAllocationTable.OpenHeader(stream);
        }

        public static void TestWrite(DiskIoEnhanced stream, int FileNumber)
        {
            FileAllocationTable header = FileAllocationTable.OpenHeader(stream);
            header = header.CreateEditableCopy(true);
            FileMetaData node = header.Files[FileNumber];
            IndexParser parse = new IndexParser(header.SnapshotSequenceNumber, stream, node);
            ShadowCopyAllocator shadow = new ShadowCopyAllocator(stream, header, node, parse);

            uint nextPage = header.NextUnallocatedBlock;


            shadow.ShadowDataBlock(0);
            PositionData pd = parse.GetPositionData(0);
            if (node.DirectCluster != nextPage)
                throw new Exception();
            if (parse.DataClusterAddress != nextPage)
                throw new Exception();
            stream.WriteBlock(parse.DataClusterAddress, BlockType.DataBlock, (uint)(pd.VirtualPosition / ArchiveConstants.DataBlockDataLength), node.FileIdNumber, header.SnapshotSequenceNumber, parse.BufferPool.Data.Block);


            //should do nothing since the page has already been allocated
            shadow.ShadowDataBlock(1024);
            pd = parse.GetPositionData(1024);
            if (node.DirectCluster != nextPage)
                throw new Exception();
            if (parse.DataClusterAddress != nextPage)
                throw new Exception();
            stream.WriteBlock(parse.DataClusterAddress, BlockType.DataBlock, (uint)(pd.VirtualPosition / ArchiveConstants.DataBlockDataLength), node.FileIdNumber, header.SnapshotSequenceNumber, parse.BufferPool.Data.Block);

            if (ArchiveConstants.BlockSize == 4096)
            {
                //Allocate in the 4th indirect block
                shadow.ShadowDataBlock(ArchiveConstants.FirstQuadrupleIndirectBlockIndex * (long)ArchiveConstants.DataBlockDataLength);
                pd = parse.GetPositionData(ArchiveConstants.FirstQuadrupleIndirectBlockIndex * (long)ArchiveConstants.DataBlockDataLength);
                if (node.DirectCluster != nextPage)
                    throw new Exception();
                if (parse.DataClusterAddress != nextPage + 1)
                    throw new Exception();
                if (parse.FirstIndirectBlockAddress != nextPage + 5)
                    throw new Exception();
                if (parse.SecondIndirectBlockAddress != nextPage + 4)
                    throw new Exception();
                if (parse.ThirdIndirectBlockAddress != nextPage + 3)
                    throw new Exception();
                if (parse.ForthIndirectBlockAddress != nextPage + 2)
                    throw new Exception();
                stream.WriteBlock(parse.DataClusterAddress, BlockType.DataBlock, (uint)(pd.VirtualPosition / ArchiveConstants.DataBlockDataLength), node.FileIdNumber, header.SnapshotSequenceNumber, parse.BufferPool.Data.Block);

            }
            else
            {
                //Allocate in the 3th indirect block
                shadow.ShadowDataBlock(ArchiveConstants.FirstTripleIndirectIndex * (long)ArchiveConstants.DataBlockDataLength);
                pd = parse.GetPositionData(ArchiveConstants.FirstTripleIndirectIndex * (long)ArchiveConstants.DataBlockDataLength);
                if (node.DirectCluster != nextPage)
                    throw new Exception();
                if (parse.DataClusterAddress != nextPage + 1)
                    throw new Exception();
                if (parse.FirstIndirectBlockAddress != nextPage + 4)
                    throw new Exception();
                if (parse.SecondIndirectBlockAddress != nextPage + 3)
                    throw new Exception();
                if (parse.ThirdIndirectBlockAddress != nextPage + 2)
                    throw new Exception();
                stream.WriteBlock(parse.DataClusterAddress, BlockType.DataBlock, (uint)(pd.VirtualPosition / ArchiveConstants.DataBlockDataLength), node.FileIdNumber, header.SnapshotSequenceNumber, parse.BufferPool.Data.Block);
            }
            //if (node.DirectCluster != nextPage)
            //    throw new Exception();
            //if (parse.DataClusterAddress != nextPage + 1)
            //    throw new Exception();
            //if (parse.FirstIndirectBlockAddress != nextPage + 5)
            //    throw new Exception();
            //if (parse.SecondIndirectBlockAddress != nextPage + 4)
            //    throw new Exception();
            //if (parse.ThirdIndirectBlockAddress != nextPage + 3)
            //    throw new Exception();
            //if (parse.ForthIndirectBlockAddress != nextPage + 2)
            //    throw new Exception();

            header.WriteToFileSystem(stream);
        }





    }
}
