//******************************************************************************************************
//  FileMetaDataTest.cs - Gbtc
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
//  11/23/2011 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;

namespace openHistorian.Core.StorageSystem.File
{
    internal class FileMetaDataTest
    {

        internal static void Test()
        {
            Random rand = new Random();
            uint fileIDNumber = (uint)rand.Next(int.MaxValue);
            Guid fileExtension = Guid.NewGuid();
            uint fileFlags = (uint)rand.Next(int.MaxValue);
            uint allocatedBlocks = (uint)rand.Next(int.MaxValue);
            uint dataBlock1 = (uint)rand.Next(int.MaxValue);
            uint singleRedirect = (uint)rand.Next(int.MaxValue);
            uint doubleRedirect = (uint)rand.Next(int.MaxValue);
            uint tripleRedirect = (uint)rand.Next(int.MaxValue);
            uint quadrupleRedirect = (uint)rand.Next(int.MaxValue);

            Guid gid = Guid.NewGuid();
            FileMetaData node = FileMetaData.CreateFileMetaData(fileIDNumber, fileExtension);
            node.FileFlags = fileFlags;
            node.LastAllocatedCluster = allocatedBlocks;
            node.DirectCluster = dataBlock1;
            node.SingleIndirectCluster = singleRedirect;
            node.DoubleIndirectCluster = doubleRedirect;
            node.TripleIndirectCluster = tripleRedirect;
            node.QuadrupleIndirectCluster = quadrupleRedirect;
            FileMetaData node2 = saveItem(node);

            if (node2.FileIdNumber != fileIDNumber) throw new Exception();
            if (node2.FileExtension != fileExtension) throw new Exception();
            if (node2.FileFlags != fileFlags) throw new Exception();
            if (node2.LastAllocatedCluster != allocatedBlocks) throw new Exception();
            if (node2.DirectCluster != dataBlock1) throw new Exception();
            if (node2.SingleIndirectCluster != singleRedirect) throw new Exception();
            if (node2.DoubleIndirectCluster != doubleRedirect) throw new Exception();
            if (node2.TripleIndirectCluster != tripleRedirect) throw new Exception();
            if (node2.QuadrupleIndirectCluster != quadrupleRedirect) throw new Exception();

        }

        private static FileMetaData saveItem(FileMetaData node)
        {
            //Serialize the header
            MemoryStream stream = new MemoryStream();
            node.Save(new BinaryWriter(stream));

            stream.Position = 0;
            //load the header
            FileMetaData node2 = FileMetaData.OpenFileMetaData(new BinaryReader(stream));

            CheckEqual(node2, node);

            FileMetaData node3 = node2.CreateEditableCopy();

            CheckEqual(node2, node3);
            return node3;

        }
        internal static void CheckEqual(FileMetaData RO, FileMetaData RW)
        {
            if (!RO.AreEqual(RW)) throw new Exception();
        }
    }
}
