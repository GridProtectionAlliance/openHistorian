//******************************************************************************************************
//  FileSystemSnapshotServiceTest.cs - Gbtc
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
//  10/14/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;

namespace openHistorian.Core.StorageSystem.File
{
    internal class FileSystemSnapshotServiceTest
    {
        internal static void Test()
        {
            //string file = Path.GetTempFileName();
            //System.IO.File.Delete(file);
            try
            {
                //using (FileSystemSnapshotService service = FileSystemSnapshotService.CreateFile(file))
                using (FileSystemSnapshotService service = FileSystemSnapshotService.CreateInMemory())
                {
                    using (TransactionalEdit edit = service.BeginEditTransaction())
                    {
                        ArchiveFileStream fs = edit.CreateFile(Guid.NewGuid(), 1);
                        fs.WriteByte(1);
                        edit.Commit();
                    }
                    using (TransactionalRead read = service.BeginReadTransaction())
                    {
                        ArchiveFileStream f1 = read.OpenFile(0);
                        if (f1.ReadByte() != 1)
                            throw new Exception();
                        using (TransactionalEdit edit = service.BeginEditTransaction())
                        {
                            ArchiveFileStream f2 = edit.OpenFile(0);
                            if (f2.ReadByte() != 1)
                                throw new Exception();
                            f2.WriteByte(3);
                        } //rollback should be issued;
                        if (f1.ReadByte() != 0)
                            throw new Exception();

                        using (TransactionalRead read2 = service.BeginReadTransaction())
                        {
                            ArchiveFileStream f2 = read2.OpenFile(0);
                            if (f2.ReadByte() != 1)
                                throw new Exception();
                            if (f2.ReadByte() != 0)
                                throw new Exception();
                        }


                    }
                    using (TransactionalEdit edit = service.BeginEditTransaction())
                    {
                        ArchiveFileStream f2 = edit.OpenFile(0);
                        f2.WriteByte(13);
                        f2.WriteByte(23);
                        ArchiveFileStream f3 = edit.OpenOrigionalFile(0);
                        if (f3.ReadByte() != 1)
                            throw new Exception();
                        if (f3.ReadByte() != 0)
                            throw new Exception();
                        edit.Rollback();
                    } //rollback should be issued;
                }
            }
            finally
            {
                //System.IO.File.Delete(file);
            }
        }

    }
}
