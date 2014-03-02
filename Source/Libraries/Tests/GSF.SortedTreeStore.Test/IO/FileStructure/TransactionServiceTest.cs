//******************************************************************************************************
//  TransactionServiceTest.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  10/14/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF;
using GSF.IO.Unmanaged;
using NUnit.Framework;

namespace GSF.IO.FileStructure.Test
{
    [TestFixture()]
    public class TransactionServiceTest
    {
        private static int BlockSize = 4096;

        [Test()]
        public void Test()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
            //string file = Path.GetTempFileName();
            //System.IO.File.Delete(file);
            try
            {
                //using (FileSystemSnapshotService service = FileSystemSnapshotService.CreateFile(file))
                using (TransactionService service = TransactionService.CreateInMemory(BlockSize))
                {
                    using (TransactionalEdit edit = service.BeginEditTransaction())
                    {
                        SubFileStream fs = edit.CreateFile(SubFileName.CreateRandom());
                        BinaryStream bs = new BinaryStream(fs);
                        bs.Write((byte)1);
                        bs.Dispose();
                        fs.Dispose();
                        edit.CommitAndDispose();
                    }
                    {
                        TransactionalRead read = service.GetCurrentSnapshot();
                        SubFileStream f1 = read.OpenFile(0);
                        BinaryStream bs1 = new BinaryStream(f1);
                        if (bs1.ReadUInt8() != 1)
                            throw new Exception();

                        using (TransactionalEdit edit = service.BeginEditTransaction())
                        {
                            SubFileStream f2 = edit.OpenFile(0);
                            BinaryStream bs2 = new BinaryStream(f2);
                            if (bs2.ReadUInt8() != 1)
                                throw new Exception();
                            bs2.Write((byte)3);
                            bs2.Dispose();
                        } //rollback should be issued;
                        if (bs1.ReadUInt8() != 0)
                            throw new Exception();
                        bs1.Dispose();

                        {
                            TransactionalRead read2 = service.GetCurrentSnapshot();
                            SubFileStream f2 = read2.OpenFile(0);
                            BinaryStream bs2 = new BinaryStream(f2);
                            if (bs2.ReadUInt8() != 1)
                                throw new Exception();
                            if (bs2.ReadUInt8() != 0)
                                throw new Exception();
                            bs2.Dispose();
                        }
                    }
                    using (TransactionalEdit edit = service.BeginEditTransaction())
                    {
                        SubFileStream f2 = edit.OpenFile(0);
                        BinaryStream bs2 = new BinaryStream(f2);
                        bs2.Write((byte)13);
                        bs2.Write((byte)23);
                        bs2.Dispose();
                        edit.RollbackAndDispose();
                    } //rollback should be issued;
                }
            }
            finally
            {
                //System.IO.File.Delete(file);
            }

            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
            Assert.IsTrue(true);
        }
    }
}