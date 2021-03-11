//******************************************************************************************************
//  SimplifiedFileWriterTest.cs - Gbtc
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
//  10/16/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using GSF.IO.Unmanaged;
using GSF.Snap.Storage;
using NUnit.Framework;

namespace GSF.IO.FileStructure
{
    [TestFixture]
    public class SimplifiedFileWriterTest
    {
        [Test]
        public void Test1()
        {
            File.Delete(@"C:\Temp\fileTemp.~d2i");
            File.Delete(@"C:\Temp\fileTemp.d2i");
            using (SimplifiedFileWriter writer = new SimplifiedFileWriter(@"C:\Temp\fileTemp.~d2i", @"C:\Temp\fileTemp.d2i", 4096, FileFlags.ManualRollover))
            {
                using (ISupportsBinaryStream file = writer.CreateFile(SubFileName.CreateRandom()))
                using (BinaryStream bs = new BinaryStream(file))
                {
                    bs.Write(1);
                }

                writer.Commit();
            }


            using (TransactionalFileStructure reader = TransactionalFileStructure.OpenFile(@"C:\Temp\fileTemp.d2i", true))
            {
                using (SubFileStream file = reader.Snapshot.OpenFile(0))
                using (BinaryStream bs = new BinaryStream(file))
                {
                    if (bs.ReadInt32() != 1)
                        throw new Exception();
                }
            }
        }

        [Test]
        public void TestOneFileBig()
        {
            Random r = new Random(1);

            File.Delete(@"C:\Temp\fileTemp.~d2i");
            File.Delete(@"C:\Temp\fileTemp.d2i");
            using (SimplifiedFileWriter writer = new SimplifiedFileWriter(@"C:\Temp\fileTemp.~d2i", @"C:\Temp\fileTemp.d2i", 4096, FileFlags.ManualRollover))
            {
                using (ISupportsBinaryStream file = writer.CreateFile(SubFileName.CreateRandom()))
                using (BinaryStream bs = new BinaryStream(file))
                {
                    bs.Write((byte)1);
                    for (int x = 0; x < 100000; x++)
                    {
                        bs.Write(r.NextDouble());
                    }
                }

                writer.Commit();
            }

            r = new Random(1);
            using (TransactionalFileStructure reader = TransactionalFileStructure.OpenFile(@"C:\Temp\fileTemp.d2i", true))
            {
                using (SubFileStream file = reader.Snapshot.OpenFile(0))
                using (BinaryStream bs = new BinaryStream(file))
                {
                    if (bs.ReadUInt8() != 1)
                        throw new Exception();

                    for (int x = 0; x < 100000; x++)
                    {
                        if (bs.ReadDouble() != r.NextDouble())
                            throw new Exception();
                    }

                }
            }
        }

        [Test]
        public void TestMultipleFiles()
        {
            Random r = new Random(1);

            File.Delete(@"C:\Temp\fileTemp.~d2i");
            File.Delete(@"C:\Temp\fileTemp.d2i");
            using (SimplifiedFileWriter writer = new SimplifiedFileWriter(@"C:\Temp\fileTemp.~d2i", @"C:\Temp\fileTemp.d2i", 4096, FileFlags.ManualRollover))
            {
                for (int i = 0; i < 10; i++)
                {
                    using (ISupportsBinaryStream file = writer.CreateFile(SubFileName.CreateRandom()))
                    using (BinaryStream bs = new BinaryStream(file))
                    {
                        bs.Write((byte)1);
                        for (int x = 0; x < 100000; x++)
                        {
                            bs.Write(r.NextDouble());
                        }
                    }
                }
                writer.Commit();
            }

            r = new Random(1);
            using (TransactionalFileStructure reader = TransactionalFileStructure.OpenFile(@"C:\Temp\fileTemp.d2i", true))
            {
                for (int i = 0; i < 10; i++)
                {
                    using (SubFileStream file = reader.Snapshot.OpenFile(i))
                    using (BinaryStream bs = new BinaryStream(file))
                    {
                        if (bs.ReadUInt8() != 1)
                            throw new Exception();

                        for (int x = 0; x < 100000; x++)
                        {
                            if (bs.ReadDouble() != r.NextDouble())
                                throw new Exception();
                        }
                    }
                }
            }

        }



    }
}
