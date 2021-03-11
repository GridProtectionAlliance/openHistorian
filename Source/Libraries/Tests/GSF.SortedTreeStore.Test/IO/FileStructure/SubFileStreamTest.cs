//******************************************************************************************************
//  SubFileStreamTest.cs - Gbtc
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
//  12/10/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.IO.FileStructure.Media;
using GSF.IO.Test;
using GSF.IO.Unmanaged;
using NUnit.Framework;

namespace GSF.IO.FileStructure.Test
{
    /// <summary>
    /// Provides a stream that converts the virtual addresses of the internal feature files to physical address
    /// Also provides a way to copy on write to support the versioning file system.
    /// </summary>
    [TestFixture()]
    public class SubFileStreamTest
    {
        private static readonly int BlockSize = 4096;
        private static readonly int BlockDataLength = BlockSize - FileStructureConstants.BlockFooterLength;

        [Test()]
        public void Test()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);

            DiskIo stream = DiskIo.CreateMemoryFile(Globals.MemoryPool, BlockSize);
            TestReadAndWrites(stream);

            TestReadAndWritesWithCommit(stream);
            TestReadAndWritesToDifferentFilesWithCommit(stream);
            TestBinaryStream(stream);
            stream.Dispose();
            Assert.IsTrue(true);
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }

        private static void TestBinaryStream(DiskIo stream)
        {
            FileHeaderBlock header = stream.LastCommittedHeader;
            header = header.CloneEditable();
            SubFileHeader node = header.CreateNewFile(SubFileName.CreateRandom());
            header.CreateNewFile(SubFileName.CreateRandom());
            header.CreateNewFile(SubFileName.CreateRandom());

            SubFileStream ds = new SubFileStream(stream, node, header, false);
            BinaryStreamTest.Test(ds);
        }

        private static void TestReadAndWrites(DiskIo stream)
        {
            FileHeaderBlock header = stream.LastCommittedHeader;
            header = header.CloneEditable();
            SubFileHeader node = header.CreateNewFile(SubFileName.CreateRandom());
            header.CreateNewFile(SubFileName.CreateRandom());
            header.CreateNewFile(SubFileName.CreateRandom());

            SubFileStream ds = new SubFileStream(stream, node, header, false);
            TestSingleByteWrite(ds);
            TestSingleByteRead(ds);

            TestCustomSizeWrite(ds, 5);
            TestCustomSizeRead(ds, 5);

            TestCustomSizeWrite(ds, BlockDataLength + 20);
            TestCustomSizeRead(ds, BlockDataLength + 20);
            stream.CommitChanges(header);
        }

        private static void TestReadAndWritesWithCommit(DiskIo stream)
        {
            FileHeaderBlock header;
            SubFileHeader node;
            SubFileStream ds, ds1, ds2;
            //Open The File For Editing
            header = stream.LastCommittedHeader.CloneEditable();
            node = header.Files[0];
            ds = new SubFileStream(stream, node, header, false);
            TestSingleByteWrite(ds);
            stream.CommitChanges(header);

            header = stream.LastCommittedHeader;
            node = header.Files[0];
            ds1 = ds = new SubFileStream(stream, node, header, true);
            TestSingleByteRead(ds);

            //Open The File For Editing
            header = stream.LastCommittedHeader.CloneEditable();
            node = header.Files[0];
            ds = new SubFileStream(stream, node, header, false);
            TestCustomSizeWrite(ds, 5);
            stream.CommitChanges(header);

            header = stream.LastCommittedHeader;
            node = header.Files[0];
            ds2 = ds = new SubFileStream(stream, node, header, true);
            TestCustomSizeRead(ds, 5);

            //Open The File For Editing
            header = stream.LastCommittedHeader.CloneEditable();
            node = header.Files[0];
            ds = new SubFileStream(stream, node, header, false);
            TestCustomSizeWrite(ds, BlockDataLength + 20);
            stream.CommitChanges(header);

            header = stream.LastCommittedHeader;
            node = header.Files[0];
            ds = new SubFileStream(stream, node, header, true);
            TestCustomSizeRead(ds, BlockDataLength + 20);

            //check old versions of the file
            TestSingleByteRead(ds1);
            TestCustomSizeRead(ds2, 5);
        }

        private static void TestReadAndWritesToDifferentFilesWithCommit(DiskIo stream)
        {
            FileHeaderBlock header;

            SubFileStream ds;
            //Open The File For Editing
            header = stream.LastCommittedHeader.CloneEditable();
            ds = new SubFileStream(stream, header.Files[0], header, false);
            TestSingleByteWrite(ds);
            ds = new SubFileStream(stream, header.Files[1], header, false);
            TestCustomSizeWrite(ds, 5);
            ds = new SubFileStream(stream, header.Files[2], header, false);
            TestCustomSizeWrite(ds, BlockDataLength + 20);
            stream.CommitChanges(header);

            header = stream.LastCommittedHeader;
            ds = new SubFileStream(stream, header.Files[0], header, true);
            TestSingleByteRead(ds);
            ds = new SubFileStream(stream, header.Files[1], header, true);
            TestCustomSizeRead(ds, 5);
            ds = new SubFileStream(stream, header.Files[2], header, true);
            TestCustomSizeRead(ds, BlockDataLength + 20);
        }


        internal static void TestSingleByteWrite(SubFileStream ds)
        {
            using (BinaryStream bs = new BinaryStream(ds))
            {
                bs.Position = 0;
                for (int x = 0; x < 10000; x++)
                {
                    bs.Write((byte)x);
                }
            }
        }

        internal static void TestSingleByteRead(SubFileStream ds)
        {
            using (BinaryStream bs = new BinaryStream(ds))
            {
                bs.Position = 0;
                for (int x = 0; x < 10000; x++)
                {
                    if ((byte)x != bs.ReadUInt8())
                        throw new Exception();
                }
            }
        }

        internal static void TestCustomSizeWrite(SubFileStream ds, int length)
        {
            using (BinaryStream bs = new BinaryStream(ds))
            {
                Random r = new Random(length);

                bs.Position = 0;
                byte[] buffer = new byte[25];

                for (int x = 0; x < 1000; x++)
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = (byte)r.Next();
                    }
                    bs.Write(buffer, 0, r.Next(25));
                }
            }
        }

        internal static void TestCustomSizeRead(SubFileStream ds, int seed)
        {
            using (BinaryStream bs = new BinaryStream(ds))
            {
                Random r = new Random(seed);

                byte[] buffer = new byte[25];
                byte[] buffer2 = new byte[25];
                bs.Position = 0;
                for (int x = 0; x < 1000; x++)
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = (byte)r.Next();
                    }
                    int length = r.Next(25);
                    bs.ReadAll(buffer2, 0, length);

                    for (int i = 0; i < length; i++)
                    {
                        if (buffer[i] != buffer2[i])
                            throw new Exception();
                    }
                }
            }
        }
    }
}