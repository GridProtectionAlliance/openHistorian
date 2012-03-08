//******************************************************************************************************
//  ArchiveFileStreamTest.cs - Gbtc
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
//  12/10/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace openHistorian.Core.StorageSystem.File
{
    /// <summary>
    /// Provides a stream that converts the virtual addresses of the internal feature files to physical address
    /// Also provides a way to copy on write to support the versioning file system.
    /// </summary>
    internal class ArchiveFileStreamTest
    {
        public static void Test()
        {
            DiskIoMemoryStream stream = new DiskIoMemoryStream();
            TestReadAndWrites(stream);
            TestReadAndWritesWithCommit(stream);
            TestReadAndWritesToDifferentFilesWithCommit(stream);
            TestBinaryStream(stream);

        }
        static void TestBinaryStream(DiskIoBase stream)
        {
            FileAllocationTable header = FileAllocationTable.CreateFileAllocationTable(stream);
            FileMetaData node = header.CreateNewFile(Guid.NewGuid());
            header.CreateNewFile(Guid.NewGuid());
            header.CreateNewFile(Guid.NewGuid());

            ArchiveFileStream ds = new ArchiveFileStream(stream, node, header, false);
            BinaryStreamTest.Test(ds);
        }

        static void TestReadAndWrites(DiskIoBase stream)
        {
            FileAllocationTable header = FileAllocationTable.CreateFileAllocationTable(stream);
            FileMetaData node = header.CreateNewFile(Guid.NewGuid());
            header.CreateNewFile(Guid.NewGuid());
            header.CreateNewFile(Guid.NewGuid());

            ArchiveFileStream ds = new ArchiveFileStream(stream, node, header, false);
            TestSingleByteWrite(ds);
            TestSingleByteRead(ds);

            TestCustomSizeWrite(ds, 5);
            TestCustomSizeRead(ds, 5);

            TestCustomSizeWrite(ds, ArchiveConstants.DataBlockDataLength + 20);
            TestCustomSizeRead(ds, ArchiveConstants.DataBlockDataLength + 20);
            header.WriteToFileSystem(stream);
        }

        static void TestReadAndWritesWithCommit(DiskIoBase stream)
        {
            FileAllocationTable header;
            FileMetaData node;
            ArchiveFileStream ds, ds1, ds2;
            //Open The File For Editing
            header = FileAllocationTable.OpenHeader(stream).CreateEditableCopy(true);
            node = header.Files[0];
            ds = new ArchiveFileStream(stream, node, header, false);
            TestSingleByteWrite(ds);
            header.WriteToFileSystem(stream);

            header = FileAllocationTable.OpenHeader(stream);
            node = header.Files[0];
            ds1 = ds = new ArchiveFileStream(stream, node, header, true);
            TestSingleByteRead(ds);

            //Open The File For Editing
            header = FileAllocationTable.OpenHeader(stream).CreateEditableCopy(true);
            node = header.Files[0];
            ds = new ArchiveFileStream(stream, node, header, false);
            TestCustomSizeWrite(ds, 5);
            header.WriteToFileSystem(stream);

            header = FileAllocationTable.OpenHeader(stream);
            node = header.Files[0];
            ds2 = ds = new ArchiveFileStream(stream, node, header, true);
            TestCustomSizeRead(ds, 5);

            //Open The File For Editing
            header = FileAllocationTable.OpenHeader(stream).CreateEditableCopy(true);
            node = header.Files[0];
            ds = new ArchiveFileStream(stream, node, header, false);
            TestCustomSizeWrite(ds, ArchiveConstants.DataBlockDataLength + 20);
            header.WriteToFileSystem(stream);

            header = FileAllocationTable.OpenHeader(stream);
            node = header.Files[0];
            ds = new ArchiveFileStream(stream, node, header, true);
            TestCustomSizeRead(ds, ArchiveConstants.DataBlockDataLength + 20);

            //check old versions of the file
            TestSingleByteRead(ds1);
            TestCustomSizeRead(ds2, 5);
        }

        static void TestReadAndWritesToDifferentFilesWithCommit(DiskIoBase stream)
        {
            FileAllocationTable header;

            ArchiveFileStream ds;
            //Open The File For Editing
            header = FileAllocationTable.OpenHeader(stream).CreateEditableCopy(true);
            ds = new ArchiveFileStream(stream, header.Files[0], header, false);
            TestSingleByteWrite(ds);
            ds = new ArchiveFileStream(stream, header.Files[1], header, false);
            TestCustomSizeWrite(ds, 5);
            ds = new ArchiveFileStream(stream, header.Files[2], header, false);
            TestCustomSizeWrite(ds, ArchiveConstants.DataBlockDataLength + 20);
            header.WriteToFileSystem(stream);

            header = FileAllocationTable.OpenHeader(stream);
            ds = new ArchiveFileStream(stream, header.Files[0], header, true);
            TestSingleByteRead(ds);
            ds = new ArchiveFileStream(stream, header.Files[1], header, true);
            TestCustomSizeRead(ds, 5);
            ds = new ArchiveFileStream(stream, header.Files[2], header, true);
            TestCustomSizeRead(ds, ArchiveConstants.DataBlockDataLength + 20);

        }


        internal static void TestSingleByteWrite(ArchiveFileStream ds)
        {
            ds.Position = 0;
            for (int x = 0; x < 10000; x++)
            {
                ds.WriteByte((byte)x);
            }
            ds.Flush();
        }
        internal static void TestSingleByteRead(ArchiveFileStream ds)
        {
            ds.Position = 0;
            for (int x = 0; x < 10000; x++)
            {
                if ((byte)x != ds.ReadByte())
                    throw new Exception();
            }
        }

        internal static void TestCustomSizeWrite(ArchiveFileStream ds, int length)
        {
            ds.Position = 0;
            byte[] buffer = new byte[length];

            Random rand = new Random(length);
            for (int x = 0; x < 1000; x++)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)rand.Next();
                }
                ds.Write(buffer, 0, buffer.Length);
            }
            ds.Flush();
        }

        internal static void TestCustomSizeRead(ArchiveFileStream ds, int length)
        {
            byte[] buffer = new byte[length];
            ds.Position = 0;
            Random rand2 = new Random(length);
            for (int x = 0; x < 1000; x++)
            {
                ds.Read(buffer, 0, buffer.Length);
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != (byte)rand2.Next())
                        throw new Exception();
                }
            }
            ds.Flush();
        }

    }
}
