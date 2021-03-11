//******************************************************************************************************
//  TransactionalEditTest.cs - Gbtc
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
//  12/2/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.IO.FileStructure.Media;
using NUnit.Framework;

namespace GSF.IO.FileStructure.Test
{
    [TestFixture()]
    public class TransactionalEditTest
    {
        private static readonly int BlockSize = 4096;
        private static readonly int BlockDataLength = BlockSize - FileStructureConstants.BlockFooterLength;

        [Test()]
        public void Test()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
            DiskIo stream = DiskIo.CreateMemoryFile(Globals.MemoryPool, BlockSize);
            _ = stream.LastCommittedHeader;
            //obtain a readonly copy of the file allocation table.
            FileHeaderBlock fat = stream.LastCommittedHeader;
            TestCreateNewFile(stream, fat);
            fat = stream.LastCommittedHeader;
            TestOpenExistingFile(stream, fat);
            fat = stream.LastCommittedHeader;
            TestRollback(stream, fat);
            fat = stream.LastCommittedHeader;
            TestVerifyRollback(stream, fat);
            Assert.IsTrue(true);
            stream.Dispose();
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }

        private static void TestCreateNewFile(DiskIo stream, FileHeaderBlock fat)
        {
            SubFileName id1 = SubFileName.CreateRandom();
            SubFileName id2 = SubFileName.CreateRandom();
            SubFileName id3 = SubFileName.CreateRandom();
            TransactionalEdit trans = new TransactionalEdit(stream);
            //create 3 files

            SubFileStream fs1 = trans.CreateFile(id1);
            SubFileStream fs2 = trans.CreateFile(id2);
            SubFileStream fs3 = trans.CreateFile(id3);
            if (fs1.SubFile.FileName != id1)
                throw new Exception();
            //write to the three files
            SubFileStreamTest.TestSingleByteWrite(fs1);
            SubFileStreamTest.TestCustomSizeWrite(fs2, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs3, BlockDataLength + 20);

            //read from them and verify content.
            SubFileStreamTest.TestCustomSizeRead(fs3, BlockDataLength + 20);
            SubFileStreamTest.TestCustomSizeRead(fs2, 5);
            SubFileStreamTest.TestSingleByteRead(fs1);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            trans.CommitAndDispose();
        }

        private static void TestOpenExistingFile(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream);
            //create 3 files

            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs1);
            SubFileStreamTest.TestCustomSizeRead(fs2, 5);
            SubFileStreamTest.TestCustomSizeRead(fs3, BlockDataLength + 20);

            //rewrite bad data.
            SubFileStreamTest.TestSingleByteWrite(fs2);
            SubFileStreamTest.TestCustomSizeWrite(fs3, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs1, BlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            trans.CommitAndDispose();
        }

        private static void TestRollback(DiskIo stream, FileHeaderBlock fat)
        {
            SubFileName id1 = SubFileName.CreateRandom();
            SubFileName id2 = SubFileName.CreateRandom();
            SubFileName id3 = SubFileName.CreateRandom();
            TransactionalEdit trans = new TransactionalEdit(stream);

            //create 3 files additional files
            SubFileStream fs21 = trans.CreateFile(id1);
            SubFileStream fs22 = trans.CreateFile(id2);
            SubFileStream fs23 = trans.CreateFile(id3);

            //open files
            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs2);
            SubFileStreamTest.TestCustomSizeRead(fs3, 5);
            SubFileStreamTest.TestCustomSizeRead(fs1, BlockDataLength + 20);

            //rewrite bad data.
            SubFileStreamTest.TestSingleByteWrite(fs3);
            SubFileStreamTest.TestCustomSizeWrite(fs1, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs2, BlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            fs21.Dispose();
            fs22.Dispose();
            fs23.Dispose();

            trans.RollbackAndDispose();
        }

        private static void TestVerifyRollback(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream);

            if (trans.Files.Count != 3)
                throw new Exception();

            //open files
            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs2);
            SubFileStreamTest.TestCustomSizeRead(fs3, 5);
            SubFileStreamTest.TestCustomSizeRead(fs1, BlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            trans.Dispose();
        }
    }
}