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
using openHistorian.V2.Collections;

namespace openHistorian.V2.FileStructure
{
    /// <summary>
    /// Contains the information that is in the header page of an archive file.  
    /// </summary>
    internal class FileHeaderBlock : SupportsReadonlyBase<FileHeaderBlock>
    {
        #region [ Members ]
        #region [ Enumerables ]
        /// <summary>
        /// Provides a convinent text description for what the MetaDataCode names are.
        /// </summary>
        enum MetaDataCodes : short
        {
            ListOfFiles = 1
        }
        #endregion
        #region [ Constants ]
        /// <summary>
        /// The file header bytes which equals: "openHistorian Archive 2.0\00"
        /// </summary>
        static byte[] s_fileAllocationTableHeaderBytes = new byte[] { 0x6F, 0x70, 0x65, 0x6E, 0x48, 0x69, 0x73, 0x74, 0x6F, 0x72, 0x69, 0x61, 0x6E, 0x20, 0x41, 0x72, 0x63, 0x68, 0x69, 0x76, 0x65, 0x20, 0x32, 0x2E, 0x30, 0x00 };
        const int FileAllocationTableVersion = 0;

        #endregion
        #region [ Fields ]

        /// <summary>
        /// The version number required to read the file system.
        /// </summary>
        int m_minimumReadVersion;
        /// <summary>
        /// The version number required to write to the file system.
        /// </summary>
        int m_minimumWriteVersion;
        /// <summary>
        /// The GUID for this archive file system.
        /// </summary>
        Guid m_archiveId;
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
        List<byte[]> m_unrecgonizedMetaDataTags;

        #endregion
        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new FileAllocationTable
        /// </summary>
        /// <param name="diskIo"></param>
        /// <param name="openMode"> </param>
        /// <param name="accessMode"> </param>
        public FileHeaderBlock(DiskIo diskIo, OpenMode openMode, AccessMode accessMode)
        {
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

            byte[] dataBytes = new byte[FileStructureConstants.BlockSize];
            MemoryStream stream = new MemoryStream(dataBytes);
            BinaryWriter dataWriter = new BinaryWriter(stream);

            dataWriter.Write(s_fileAllocationTableHeaderBytes);
            dataWriter.Write(m_minimumReadVersion);
            dataWriter.Write(m_minimumWriteVersion);
            dataWriter.Write(ArchiveId.ToByteArray());
            dataWriter.Write((byte)(0)); //IsOpenedForExclusiveEditing
            dataWriter.Write(SnapshotSequenceNumber);

            //Write meta data tags.
            //Currently only 1 is recgonized.  So unless there are unrecgonized tags, write 1.
            if (m_unrecgonizedMetaDataTags == null)
                dataWriter.Write((short)1); //meta data page count
            else
                dataWriter.Write((short)(1 + m_unrecgonizedMetaDataTags.Count)); //meta data page count

            byte fileCount = (byte)FileCount;

            dataWriter.Write((short)MetaDataCodes.ListOfFiles); //Meta Data Code
            dataWriter.Write((short)(fileCount * SubFileMetaData.SizeInBytes + 9)); //Meta data length
            dataWriter.Write(LastAllocatedBlock);
            dataWriter.Write(m_nextFileId);
            dataWriter.Write(fileCount);

            foreach (SubFileMetaData node in m_files)
            {
                if (node != null)
                    node.Save(dataWriter);
            }

            if (m_unrecgonizedMetaDataTags != null) //If there were tags that were not recgonized, simply copy them to the new archive file.
            {
                foreach (byte[] tag in m_unrecgonizedMetaDataTags)
                {
                    dataWriter.Write(tag);
                }
            }

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
            byte[] buffer = new byte[FileStructureConstants.BlockSize];
            diskIo.Read(0, BlockType.FileAllocationTable, 0, 0, int.MaxValue, buffer);

            int metaDataCount;

            MemoryStream stream = new MemoryStream(buffer);
            BinaryReader dataReader = new BinaryReader(stream);

            if (!dataReader.ReadBytes(26).SequenceEqual(s_fileAllocationTableHeaderBytes))
                throw new Exception("This file is not an archive file system, or the file is corrupt, or this file system major version is not recgonized by this version of the historian");

            m_minimumReadVersion = dataReader.ReadInt32();
            m_minimumWriteVersion = dataReader.ReadInt32();

            if (!CanRead)
                throw new Exception("The version of this file system is not recgonized");

            m_archiveId = new Guid(dataReader.ReadBytes(16));

            bool isOpenedForExclusiveEditing = (dataReader.ReadByte() != 0);
            if (isOpenedForExclusiveEditing)
                throw new Exception("The file was opened for exclusive editing.");

            m_snapshotSequenceNumber = dataReader.ReadInt32();

            //Process all of the meta data
            metaDataCount = dataReader.ReadInt16();
            for (; metaDataCount > 0; metaDataCount--)
            {
                int metaDataCode = dataReader.ReadInt16();
                int metaDataLength = dataReader.ReadInt16();
                if (metaDataLength + stream.Position > FileStructureConstants.BlockSize)
                    throw new Exception("File Allocation Tables larger than a block size is not supported");

                switch (metaDataCode)
                {
                    case 1:
                        if (metaDataLength < 9)
                            throw new Exception("The file format is corrupt");
                        m_lastAllocatedBlock = dataReader.ReadInt32();
                        m_nextFileId = dataReader.ReadInt32();
                        int fileCount = dataReader.ReadByte();
                        if (fileCount > 64)
                            throw new Exception("Only 64 features are supported per archive");
                        if (metaDataLength != fileCount * SubFileMetaData.SizeInBytes + 9)
                            throw new Exception("The file format is corrupt");

                        m_files = new ReadonlyList<SubFileMetaData>(fileCount);
                        for (int x = 0; x < fileCount; x++)
                        {
                            m_files.Add(new SubFileMetaData(dataReader, mode));
                        }
                        break;
                    default:
                        stream.Position -= 4;
                        if (m_unrecgonizedMetaDataTags == null)
                            m_unrecgonizedMetaDataTags = new List<byte[]>();
                        m_unrecgonizedMetaDataTags.Add(dataReader.ReadBytes(metaDataLength + 4));
                        break;
                }
            }
            if (!IsFileAllocationTableValid())
                throw new Exception("File System is invalid");
            IsReadOnly = (mode == AccessMode.ReadOnly);
            if (mode != AccessMode.ReadOnly)
                m_snapshotSequenceNumber++;
        }

        #endregion

    }
}
