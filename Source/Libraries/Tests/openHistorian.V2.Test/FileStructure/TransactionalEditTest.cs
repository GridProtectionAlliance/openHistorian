//******************************************************************************************************
//  TransactionalEditTest.cs - Gbtc
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
//  12/2/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.FileStructure.Test
{
    [TestClass()]
    public class TransactionalEditTest
    {
        [TestMethod()]
        public void Test()
        {
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
            DiskIo stream = new DiskIo(new MemoryStream(), 0);
            FileHeaderBlock fat = new FileHeaderBlock(stream, OpenMode.Create, AccessMode.ReadWrite);
            //obtain a readonly copy of the file allocation table.
            fat = new FileHeaderBlock(stream, OpenMode.Open, AccessMode.ReadOnly);
            TestCreateNewFile(stream, fat);
            fat = new FileHeaderBlock(stream, OpenMode.Open, AccessMode.ReadOnly);
            TestOpenExistingFile(stream, fat);
            fat = new FileHeaderBlock(stream, OpenMode.Open, AccessMode.ReadOnly);
            TestRollback(stream, fat);
            fat = new FileHeaderBlock(stream, OpenMode.Open, AccessMode.ReadOnly);
            TestVerifyRollback(stream, fat);
            Assert.IsTrue(true);
            stream.Dispose();
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
        }
        static void TestCreateNewFile(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);
            //create 3 files

            SubFileStream fs1 = trans.CreateFile(id, 1234);
            SubFileStream fs2 = trans.CreateFile(id, 1234);
            SubFileStream fs3 = trans.CreateFile(id, 1234);
            if (fs1.SubFile.FileExtension != id)
                throw new Exception();
            if (fs1.SubFile.FileFlags != 1234)
                throw new Exception();
            //write to the three files
            SubFileStreamTest.TestSingleByteWrite(fs1);
            SubFileStreamTest.TestCustomSizeWrite(fs2, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs3, FileStructureConstants.DataBlockDataLength + 20);

            //read from them and verify content.
            SubFileStreamTest.TestCustomSizeRead(fs3, FileStructureConstants.DataBlockDataLength + 20);
            SubFileStreamTest.TestCustomSizeRead(fs2, 5);
            SubFileStreamTest.TestSingleByteRead(fs1);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            trans.CommitAndDispose();
        }

        static void TestOpenExistingFile(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);
            //create 3 files

            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs1);
            SubFileStreamTest.TestCustomSizeRead(fs2, 5);
            SubFileStreamTest.TestCustomSizeRead(fs3, FileStructureConstants.DataBlockDataLength + 20);

            //rewrite bad data.
            SubFileStreamTest.TestSingleByteWrite(fs2);
            SubFileStreamTest.TestCustomSizeWrite(fs3, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs1, FileStructureConstants.DataBlockDataLength + 20);

            //verify origional still in tact.
            SubFileStream fs11 = trans.OpenOrigionalFile(0);
            SubFileStream fs12 = trans.OpenOrigionalFile(1);
            SubFileStream fs13 = trans.OpenOrigionalFile(2);

            SubFileStreamTest.TestSingleByteRead(fs11);
            SubFileStreamTest.TestCustomSizeRead(fs12, 5);
            SubFileStreamTest.TestCustomSizeRead(fs13, FileStructureConstants.DataBlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            fs11.Dispose();
            fs12.Dispose();
            fs13.Dispose();

            trans.CommitAndDispose();
        }

        static void TestRollback(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);

            //create 3 files additional files
            SubFileStream fs21 = trans.CreateFile(id, 1234);
            SubFileStream fs22 = trans.CreateFile(id, 1234);
            SubFileStream fs23 = trans.CreateFile(id, 1234);

            //open files
            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs2);
            SubFileStreamTest.TestCustomSizeRead(fs3, 5);
            SubFileStreamTest.TestCustomSizeRead(fs1, FileStructureConstants.DataBlockDataLength + 20);

            //rewrite bad data.
            SubFileStreamTest.TestSingleByteWrite(fs3);
            SubFileStreamTest.TestCustomSizeWrite(fs1, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs2, FileStructureConstants.DataBlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            fs21.Dispose();
            fs22.Dispose();
            fs23.Dispose();

            trans.RollbackAndDispose();
        }

        static void TestVerifyRollback(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);

            if (trans.Files.Count != 3)
                throw new Exception();

            //open files
            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs2);
            SubFileStreamTest.TestCustomSizeRead(fs3, 5);
            SubFileStreamTest.TestCustomSizeRead(fs1, FileStructureConstants.DataBlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            trans.Dispose();
        }
    }
}
