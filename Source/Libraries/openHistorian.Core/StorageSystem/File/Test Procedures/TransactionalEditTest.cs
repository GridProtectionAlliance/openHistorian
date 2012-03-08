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

namespace openHistorian.Core.StorageSystem.File
{
    public class TransactionalEditTest
    {
        public static void Test()
        {
            DiskIoMemoryStream stream = new DiskIoMemoryStream();
            FileAllocationTable fat = FileAllocationTable.CreateFileAllocationTable(stream);
            //obtain a readonly copy of the file allocation table.
            fat = FileAllocationTable.OpenHeader(stream);
            TestCreateNewFile(stream, fat);
            fat = FileAllocationTable.OpenHeader(stream);
            TestOpenExistingFile(stream, fat);
            fat = FileAllocationTable.OpenHeader(stream);
            TestRollback(stream, fat);
            fat = FileAllocationTable.OpenHeader(stream);
            TestVerifyRollback(stream, fat);
        }
        static void TestCreateNewFile(DiskIoBase stream, FileAllocationTable fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);
            //create 3 files

            ArchiveFileStream fs1 = trans.CreateFile(id, 1234);
            ArchiveFileStream fs2 = trans.CreateFile(id, 1234);
            ArchiveFileStream fs3 = trans.CreateFile(id, 1234);
            if (fs1.File.FileExtension != id)
                throw new Exception();
            if (fs1.File.FileFlags != 1234)
                throw new Exception();
            //write to the three files
            ArchiveFileStreamTest.TestSingleByteWrite(fs1);
            ArchiveFileStreamTest.TestCustomSizeWrite(fs2, 5);
            ArchiveFileStreamTest.TestCustomSizeWrite(fs3, ArchiveConstants.DataBlockDataLength + 20);

            //read from them and verify content.
            ArchiveFileStreamTest.TestCustomSizeRead(fs3, ArchiveConstants.DataBlockDataLength + 20);
            ArchiveFileStreamTest.TestCustomSizeRead(fs2, 5);
            ArchiveFileStreamTest.TestSingleByteRead(fs1);

            trans.Commit();
        }

        static void TestOpenExistingFile(DiskIoBase stream, FileAllocationTable fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);
            //create 3 files

            ArchiveFileStream fs1 = trans.OpenFile(0);
            ArchiveFileStream fs2 = trans.OpenFile(1);
            ArchiveFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            ArchiveFileStreamTest.TestSingleByteRead(fs1);
            ArchiveFileStreamTest.TestCustomSizeRead(fs2, 5);
            ArchiveFileStreamTest.TestCustomSizeRead(fs3, ArchiveConstants.DataBlockDataLength + 20);

            //rewrite bad data.
            ArchiveFileStreamTest.TestSingleByteWrite(fs2);
            ArchiveFileStreamTest.TestCustomSizeWrite(fs3, 5);
            ArchiveFileStreamTest.TestCustomSizeWrite(fs1, ArchiveConstants.DataBlockDataLength + 20);

            //verify origional still in tact.
            ArchiveFileStream fs11 = trans.OpenOrigionalFile(0);
            ArchiveFileStream fs12 = trans.OpenOrigionalFile(1);
            ArchiveFileStream fs13 = trans.OpenOrigionalFile(2);

            ArchiveFileStreamTest.TestSingleByteRead(fs11);
            ArchiveFileStreamTest.TestCustomSizeRead(fs12, 5);
            ArchiveFileStreamTest.TestCustomSizeRead(fs13, ArchiveConstants.DataBlockDataLength + 20);
            trans.Commit();
        }

        static void TestRollback(DiskIoBase stream, FileAllocationTable fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);

            //create 3 files additional files
            ArchiveFileStream fs21 = trans.CreateFile(id, 1234);
            ArchiveFileStream fs22 = trans.CreateFile(id, 1234);
            ArchiveFileStream fs23 = trans.CreateFile(id, 1234);

            //open files
            ArchiveFileStream fs1 = trans.OpenFile(0);
            ArchiveFileStream fs2 = trans.OpenFile(1);
            ArchiveFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            ArchiveFileStreamTest.TestSingleByteRead(fs2);
            ArchiveFileStreamTest.TestCustomSizeRead(fs3, 5);
            ArchiveFileStreamTest.TestCustomSizeRead(fs1, ArchiveConstants.DataBlockDataLength + 20);

            //rewrite bad data.
            ArchiveFileStreamTest.TestSingleByteWrite(fs3);
            ArchiveFileStreamTest.TestCustomSizeWrite(fs1, 5);
            ArchiveFileStreamTest.TestCustomSizeWrite(fs2, ArchiveConstants.DataBlockDataLength + 20);

            trans.Rollback();
        }

        static void TestVerifyRollback(DiskIoBase stream, FileAllocationTable fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);

            if (trans.Files.Count != 3)
                throw new Exception();

            //open files
            ArchiveFileStream fs1 = trans.OpenFile(0);
            ArchiveFileStream fs2 = trans.OpenFile(1);
            ArchiveFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            ArchiveFileStreamTest.TestSingleByteRead(fs2);
            ArchiveFileStreamTest.TestCustomSizeRead(fs3, 5);
            ArchiveFileStreamTest.TestCustomSizeRead(fs1, ArchiveConstants.DataBlockDataLength + 20);
            trans.Dispose();
        }
    }
}
