//******************************************************************************************************
//  FileHeaderBlock.cs - Gbtc
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
//  12/3/2011 - Steven E. Chisholm
//       Generated original version of source code. That is capable of reading/writing header version 0
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using openHistorian.Collections;

namespace openHistorian.FileStructure
{
    /// <summary>
    /// Contains the information that is in the header page of an archive file.  
    /// </summary>
    internal class FileHeaderBlock : SupportsReadonlyBase<FileHeaderBlock>
    {
        #region [ Members ]

        /// <summary>
        /// The file header bytes which equals: "openHistorian Archive 2.0\00"
        /// </summary>
        static byte[] s_fileAllocationTableHeaderBytes = new byte[] { 0x6F, 0x70, 0x65, 0x6E, 0x48, 0x69, 0x73, 0x74, 0x6F, 0x72, 0x69, 0x61, 0x6E, 0x20, 0x41, 0x72, 0x63, 0x68, 0x69, 0x76, 0x65, 0x20, 0x32, 0x2E, 0x30, 0x00 };

        const short FileAllocationTableVersion = 0;

        /// <summary>
        /// The version number required to read the file system.
        /// </summary>
        short m_minimumReadVersion;
        /// <summary>
        /// The version number required to write to the file system.
        /// </summary>
        short m_minimumWriteVersion;
        /// <summary>
        /// The GUID for this archive file system.
        /// </summary>
        Guid m_archiveId;
        /// <summary>
        /// The GUID to represent the type of this archive file.
        /// </summary>
        Guid m_archiveType;
        /// <summary>
        /// This will be updated every time the file system has been modified. Initially, it will be one.
        /// </summary>
        int m_snapshotSequenceNumber;
        /// <summary>
        /// Since files are allocated sequentially, this value is the next file id that is not used.
        /// </summary>
        int m_nextFileId;
        /// <summary>
        /// Returns the last allocated block.
        /// </summary>
        int m_lastAllocatedBlock;
        /// <summary>
        /// Provides a list of all of the Features that are contained within the file.
        /// </summary>
        ReadonlyList<SubFileMetaData> m_files;
        /// <summary>
        /// Maintains any meta data tags that existed in the file header that were not recgonized by this version of the file so they can be saved back to the file.
        /// </summary>
        byte[] m_userData;

        int m_blockSize;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new FileAllocationTable
        /// </summary>
        /// <param name="diskIo"></param>
        /// <param name="openMode"> </param>
        /// <param name="accessMode"> </param>
        public FileHeaderBlock(int blockSize, DiskIo diskIo, OpenMode openMode, AccessMode accessMode)
        {
            m_blockSize = blockSize;
            if (openMode == OpenMode.Create)
            {
                CreateNewFileAllocationTable(diskIo, accessMode);
            }
            else
            {
                LoadFromBuffer(diskIo, accessMode);
            }
            IsReadOnly = (accessMode == AccessMode.ReadOnly);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if the file can be written to because enough features are recgonized by this current version to do it without corrupting the file system.
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return (m_minimumWriteVersion <= FileAllocationTableVersion);
            }
        }

        /// <summary>
        /// Determines if the archive file can be read
        /// </summary>
        public bool CanRead
        {
            get
            {
                return (m_minimumReadVersion <= FileAllocationTableVersion);
            }
        }

        /// <summary>
        /// The GUID number for this archive.
        /// </summary>
        public Guid ArchiveId
        {
            get
            {
                return m_archiveId;
            }
        }

        /// <summary>
        /// The GUID number for this archive.
        /// </summary>
        public Guid ArchiveType
        {
            get
            {
                return m_archiveType;
            }
            set
            {
                TestForEditable();
                m_archiveType = value;
            }
        }

        /// <summary>
        /// Maintains a sequential number that represents the version of the file.
        /// </summary>
        public int SnapshotSequenceNumber
        {
            get
            {
                return m_snapshotSequenceNumber;
            }
        }

        /// <summary>
        /// Represents the next block that has not been allocated.
        /// </summary>
        public int LastAllocatedBlock
        {
            get
            {
                return m_lastAllocatedBlock;
            }
        }

        /// <summary>
        /// Returns the number of files that are in this file system. 
        /// </summary>
        /// <returns></returns>
        public int FileCount
        {
            get
            {
                return m_files.Count;
            }
        }

        public ReadonlyList<SubFileMetaData> Files
        {
            get
            {
                return m_files;
            }
        }

        public byte[] UserData
        {
            get
            {
                if (IsReadOnly)
                    return (byte[])m_userData.Clone();
                else
                    return m_userData;
            }
            set
            {
                base.TestForEditable();
                if (value == null)
                    m_userData = new byte[] { };
                else
                {
                    if (m_files.Count * SubFileMetaData.SizeInBytes + 84 + 32 + value.Length > m_blockSize)
                        throw new Exception("User block is too big for the current file.");
                    m_userData = value;
                }
            }
        }


        #endregion

        #region [ Methods ]

        /// <summary>
        /// Clones the object, while incrementing the sequence number.
        /// </summary>
        /// <returns></returns>
        public override FileHeaderBlock CloneEditable()
        {
            var clone = base.CloneEditable();
            clone.m_snapshotSequenceNumber++;
            return clone;
        }

        protected override void SetMembersAsReadOnly()
        {
            m_files.IsReadOnly = true;
        }

        protected override void CloneMembersAsEditable()
        {
            if (!CanWrite)
                throw new Exception("This file cannot be modified because the file system version is not recgonized");
            m_files = m_files.CloneEditable();
            m_userData = (byte[])m_userData.Clone();
        }

        /// <summary>
        /// Allocates a sequential number of blocks at the end of the file and returns the starting address of the allocation
        /// </summary>
        /// <param name="count">the number of blocks to allocate</param>
        /// <returns>the address of the first block of the allocation </returns>
        public int AllocateFreeBlocks(int count)
        {
            if (count <= 0)
                throw new ArgumentException("the value 0 is not valid", "count");
            TestForEditable();
            int blockAddress = m_lastAllocatedBlock + 1;
            m_lastAllocatedBlock += count;
            return blockAddress;
        }

        /// <summary>
        /// Creates a new file on the file system and returns the <see cref="SubFileMetaData"/> associated with the new file.
        /// </summary>
        /// <param name="fileExtention">Represents the nature of the data that will be stored in this file.</param>
        /// <returns></returns>
        /// <remarks>A file system only supports 64 files. This is a fundamental limitation and cannot be changed easily.</remarks>
        public SubFileMetaData CreateNewFile(Guid fileExtention)
        {
            if (!CanWrite)
                throw new Exception("Writing to this file type is not supported");
            if (IsReadOnly)
                throw new Exception("File is read only");
            if (m_files.Count >= 64)
                throw new OverflowException("Only 64 files per file system is supported");

            SubFileMetaData node = new SubFileMetaData(m_nextFileId, fileExtention, AccessMode.ReadWrite);
            m_nextFileId++;
            m_files.Add(node);
            return node;
        }

        /// <summary>
        /// Saves the file allocation table to the disk
        /// </summary>
        /// <param name="diskIo">The place to store the file allocation table.</param>
        public void WriteToFileSystem(DiskIo diskIo)
        {
            if (!CanWrite)
                throw new Exception("Writing is not supported to this file system type.");

            byte[] data = GetBytes();
            diskIo.WriteToExistingBlock(0, BlockType.FileAllocationTable, 0, 0, m_snapshotSequenceNumber, data);
            diskIo.WriteToExistingBlock(1, BlockType.FileAllocationTable, 0, 0, m_snapshotSequenceNumber, data);
            diskIo.WriteToExistingBlock(((m_snapshotSequenceNumber & 7) + 2), BlockType.FileAllocationTable, 0, 0, m_snapshotSequenceNumber, data);
        }

        /// <summary>
        /// Checks all of the information in the header file 
        /// to verify if it is valid.
        /// </summary>
        bool IsFileAllocationTableValid()
        {
            if (ArchiveId == Guid.Empty) return false;
            if (LastAllocatedBlock < 9) return false;
            if (m_minimumReadVersion < 0) return false;
            if (m_minimumWriteVersion < 0) return false;
            return true;
        }

        /// <summary>
        /// Creates a new file system and writes the file allocation table to the provided disk.
        /// </summary>
        /// <param name="diskIo"></param>
        /// <param name="mode"></param>
        void CreateNewFileAllocationTable(DiskIo diskIo, AccessMode mode)
        {
            m_minimumReadVersion = FileAllocationTableVersion;
            m_minimumWriteVersion = FileAllocationTableVersion;
            m_archiveId = Guid.NewGuid();
            m_snapshotSequenceNumber = 1;
            m_nextFileId = 0;
            m_lastAllocatedBlock = 9;
            m_files = new ReadonlyList<SubFileMetaData>();
            IsReadOnly = (mode == AccessMode.ReadOnly);
            m_userData = new byte[] { };
            m_archiveType = Guid.Empty;

            byte[] data = GetBytes();

            //write the file header to the first 10 pages of the file.
            for (int x = 0; x < 10; x++)
            {
                diskIo.WriteToNewBlock(x, BlockType.FileAllocationTable, 0, 0, m_snapshotSequenceNumber, data);
            }

        }

        /// <summary>
        /// This will return a byte array of data that can be written to an archive file.
        /// </summary>
        byte[] GetBytes()
        {
            if (!IsFileAllocationTableValid())
                throw new InvalidOperationException("File Allocation Table is invalid");

            byte[] dataBytes = new byte[m_blockSize];
            MemoryStream stream = new MemoryStream(dataBytes);
            BinaryWriter dataWriter = new BinaryWriter(stream);

            dataWriter.Write(s_fileAllocationTableHeaderBytes);

            if (BitConverter.IsLittleEndian)
            {
                dataWriter.Write('L');
            }
            else
            {
                dataWriter.Write('B');
            }
            dataWriter.Write((byte)(BitMath.CountBitsSet((uint)(m_blockSize - 1))));

            dataWriter.Write(m_minimumReadVersion);
            dataWriter.Write(m_minimumWriteVersion);
            dataWriter.Write(ArchiveId.ToByteArray());
            dataWriter.Write(ArchiveType.ToByteArray());
            dataWriter.Write(SnapshotSequenceNumber);
            dataWriter.Write(LastAllocatedBlock);
            dataWriter.Write(m_nextFileId);
            dataWriter.Write(m_files.Count);
            foreach (SubFileMetaData node in m_files)
            {
                node.Save(dataWriter);
            }
            dataWriter.Write(m_userData.Length);
            dataWriter.Write(m_userData);

            if (stream.Position + 32 > dataBytes.Length)
                throw new Exception("the file size exceedes the allowable size.");

            return dataBytes;
        }

        /// <summary>
        /// This procedure will attempt to read all of the data out of the file allocation table
        /// If the file allocation table is corrupt, an error will be generated.
        /// </summary>
        /// <param name="diskIo">the block that contains the buffer data.</param>
        /// <param name="mode"></param>
        void LoadFromBuffer(DiskIo diskIo, AccessMode mode)
        {
            byte[] buffer = new byte[m_blockSize];
            diskIo.Read(0, BlockType.FileAllocationTable, 0, 0, int.MaxValue, buffer);

            MemoryStream stream = new MemoryStream(buffer);
            BinaryReader dataReader = new BinaryReader(stream);

            if (!dataReader.ReadBytes(26).SequenceEqual(s_fileAllocationTableHeaderBytes))
                throw new Exception("This file is not an archive file system, or the file is corrupt, or this file system major version is not recgonized by this version of the historian");

            char endian = dataReader.ReadChar();
            if (BitConverter.IsLittleEndian)
            {
                if (endian != 'L')
                    throw new Exception("This archive file was not writen with a little endian processor");
            }
            else
            {
                if (endian != 'B')
                    throw new Exception("This archive file was not writen with a big endian processor");
            }

            byte blockSizePower = dataReader.ReadByte();
            if (blockSizePower > 30 || blockSizePower < 5)
                throw new Exception("Block size of this file is not supported");
            int blockSize = 1 << blockSizePower;

            if (m_blockSize != blockSize)
                throw new Exception("Block size is unexpected");

            m_minimumReadVersion = dataReader.ReadInt16();
            m_minimumWriteVersion = dataReader.ReadInt16();

            if (!CanRead)
                throw new Exception("The version of this file system is not recgonized");

            m_archiveId = new Guid(dataReader.ReadBytes(16));
            m_archiveType = new Guid(dataReader.ReadBytes(16));

            m_snapshotSequenceNumber = dataReader.ReadInt32();
            m_lastAllocatedBlock = dataReader.ReadInt32();
            m_nextFileId = dataReader.ReadInt32();
            int fileCount = dataReader.ReadInt32();

            //ToDo: check based on block
            if (fileCount > 64)
                throw new Exception("Only 64 features are supported per archive");
            
            m_files = new ReadonlyList<SubFileMetaData>(fileCount);
            for (int x = 0; x < fileCount; x++)
            {
                m_files.Add(new SubFileMetaData(dataReader, mode));
            }

            //ToDo: check based on block length
            int userSpaceLength = dataReader.ReadInt32();
            m_userData = dataReader.ReadBytes(userSpaceLength);

            if (!IsFileAllocationTableValid())
                throw new Exception("File System is invalid");
            IsReadOnly = (mode == AccessMode.ReadOnly);
            if (mode != AccessMode.ReadOnly)
                m_snapshotSequenceNumber++;
        }

        public static int SearchForBlockSize(FileStream stream)
        {
            var oldPosition = stream.Position;
            BinaryReader dataReader = new BinaryReader(stream);
            stream.Position = 0;
            
            if (!dataReader.ReadBytes(26).SequenceEqual(s_fileAllocationTableHeaderBytes))
                throw new Exception("This file is not an archive file system, or the file is corrupt, or this file system major version is not recgonized by this version of the historian");

            char endian = dataReader.ReadChar();
            if (BitConverter.IsLittleEndian)
            {
                if (endian != 'L')
                    throw new Exception("This archive file was not writen with a little endian processor");
            }
            else
            {
                if (endian != 'B')
                    throw new Exception("This archive file was not writen with a big endian processor");
            }

            byte blockSizePower = dataReader.ReadByte();
            if (blockSizePower > 30 || blockSizePower < 5)
                throw new Exception("Block size of this file is not supported");
            int blockSize = 1 << blockSizePower;
            stream.Position = oldPosition;
            return blockSize;
        }

        #endregion

    }
}
