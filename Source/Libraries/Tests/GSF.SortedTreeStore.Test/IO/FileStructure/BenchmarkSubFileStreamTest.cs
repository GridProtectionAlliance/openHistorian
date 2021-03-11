//******************************************************************************************************
//  BenchmarkSubFileStreamTest.cs - Gbtc
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
//  12/3/2011 - Steven E. Chisholm
//       Generated original version of source code.
//     
//******************************************************************************************************


using GSF.IO.Test;
using GSF.IO.Unmanaged;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.IO.FileStructure.Test
{
    [TestFixture]
    internal class BenchmarkSubFileStreamTest
    {
        [Test]
        public void TestSubFileStream()
        {
            const int BlockSize = 256;
            MemoryPoolTest.TestMemoryLeak();
            //string file = Path.GetTempFileName();
            //System.IO.File.Delete(file);
            try
            {
                //using (FileSystemSnapshotService service = FileSystemSnapshotService.CreateFile(file))
                using (TransactionalFileStructure service = TransactionalFileStructure.CreateInMemory(BlockSize))
                {
                    using (TransactionalEdit edit = service.BeginEdit())
                    {
                        SubFileStream fs = edit.CreateFile(SubFileName.Empty);
                        BinaryStream bs = new BinaryStream(fs);

                        for (int x = 0; x < 20000000; x++)
                            bs.Write(1L);

                        bs.Position = 0;

                        BinaryStreamBenchmark.Run(bs, false);

                        bs.Dispose();
                        fs.Dispose();
                        edit.CommitAndDispose();
                    }
                }
            }
            finally
            {
                //System.IO.File.Delete(file);
            }

            MemoryPoolTest.TestMemoryLeak();
        }
    }
}