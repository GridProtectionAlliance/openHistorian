//******************************************************************************************************
//  FileAllocationTable.cs - Gbtc
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// Contains the information that is in the header page of an archive file.  
    /// </summary>
    internal class FileAllocationTable
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
        /// Determines if the class is immutable
        /// </summary>
        bool m_isReadOnly;
        /// <summary>
        /// This flag is marked as true if while opening the file, the primary file allocation table was corrupt and a backup had to be used.
        /// </summary>
        bool m_isTableCompromised;
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
        /// Determines if the file is currently opened for exclusive editing.
        /// If this file was able to be opened for write access, 
        /// this parameter will suggest that the file was not closed correctly.
        /// </summary>
        /// <remarks>This flag will usually be set by a utility that is doing some kind of defragment on the file.
        /// The file will be locked exclusively by the process.</remarks>
        bool m_isOpenedForExclusiveEditing;
        /// <summary>
        ///This will be updated every time the file system has been modified. Initially, it will be zero.
        /// </summary>
        int m_snapshotSequenceNumber;
        /// <summary>
        /// Since files are allocated sequentially, this value is the next file id that is not used.
        /// </summary>
        int m_nextFileId;
        /// <summary>
        /// Returns the next unallocated free block.
        /// </summary>
        int m_nextUnallocatedBlock;
        /// <summary>
        /// Provides a list of all of the Features that are contained within the file.
        /// </summary>
        List<FileMetaData> m_files;
        /// <summary>
        /// This readonly collection is provided so the user can read the files without able to modify the collection.
        /// </summary>
        ReadOnlyCollection<FileMetaData> m_readOnlyFiles;
        /// <summary>
        /// Maintains any meta data tags that existed in the file header that were not recgonized by this version of the file so they can be saved back to the file.
        /// </summary>
        List<byte[]> m_unrecgonizedMetaDataTags;

        #endregion
        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a readonly FileAllocationTable from the byte array passed to the class.
        /// </summary>
        /// <param name="dataToLoad"></param>
        FileAllocationTable(byte[] dataToLoad)
        {
            m_isReadOnly = true;
            LoadFromBuffer(dataToLoad);
            m_readOnlyFiles = new ReadOnlyCollection<FileMetaData>(m_files);
        }

        /// <summary>
        /// Creates a new FileAllocationTable and writes this data to the passed file system.
        /// </summary>
        /// <param name="diskIo"></param>
        FileAllocationTable(DiskIo diskIo)
        {
            m_isReadOnly = false;
            CreateNewFileAllocationTable(diskIo);
            m_readOnlyFiles = new ReadOnlyCollection<FileMetaData>(m_files);
        }

        /// <summary>
        /// Clones an existing FileAllocationTable for editable access
        /// </summary>
        /// <param name="fileAllocationTable"></param>
        /// <param name="incrementSnapshotSequenceNumber"></param>
        FileAllocationTable(FileAllocationTable fileAllocationTable, bool incrementSnapshotSequenceNumber)
        {
            if (!fileAllocationTable.CanWrite)
                throw new Exception("This file cannot be modified because the file system version is not recgonized");
            m_isReadOnly = false;
            m_archiveId = fileAllocationTable.m_archiveId;
            m_files = new List<FileMetaData>(fileAllocationTable.Files.Count);
            foreach (FileMetaData t in fileAllocationTable.Files)
            {
                m_files.Add(t.CreateEditableCopy());
            }
            m_snapshotSequenceNumber = fileAllocationTable.m_snapshotSequenceNumber;
            if (incrementSnapshotSequenceNumber)
                m_snapshotSequenceNumber++;
            m_isOpenedForExclusiveEditing = fileAllocationTable.m_isOpenedForExclusiveEditing;
            m_minimumReadVersion = fileAllocationTable.m_minimumReadVersion;
            m_minimumWriteVersion = fileAllocationTable.m_minimumWriteVersion;
            m_nextUnallocatedBlock = fileAllocationTable.m_nextUnallocatedBlock;
            m_nextFileId = fileAllocationTable.m_nextFileId;
            m_unrecgonizedMetaDataTags = fileAllocationTable.m_unrecgonizedMetaDataTags;
            m_isTableCompromised = fileAllocationTable.IsTableCompromised;
            m_readOnlyFiles = new ReadOnlyCollection<FileMetaData>(m_files);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if the class is immutable
        /// </summary>
        internal bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
        }

        /// <summary>
        /// Determines if the file can be written to because enough features are recgonized by this current version to do it without corrupting the file system.
        /// </summary>
        internal bool CanWrite
        {
            get
            {
                return (m_minimumWriteVersion <= FileAllocationTableVersion);
            }
        }

        /// <summary>
        /// Determines if the archive file can be read
        /// </summary>
        bool CanRead
        {
            get
            {
                return (m_minimumReadVersion <= FileAllocationTableVersion);
            }
        }

        /// <summary>
        /// The GUID number for this archive.
        /// </summary>
        internal Guid ArchiveId
        {
            get
            {
                return m_archiveId;
            }
        }

        /// <summary>
        /// Set this flag if an action is going to performed on the file that causes multiple user access to no longer remain isolated.
        /// </summary>
        /// <remarks>This is here for future support and is currently not being used.</remarks>
        bool IsOpenedForExclusiveEditing
        {
            get
            {
                return m_isOpenedForExclusiveEditing;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("class is read only");
                m_isOpenedForExclusiveEditing = value;
            }
        }

        /// <summary>
        /// Maintains a sequential number that represents the version of the file.
        /// </summary>
        internal int SnapshotSequenceNumber
        {
            get
            {
                return m_snapshotSequenceNumber;
            }
        }

        /// <summary>
        /// Represents the next block that has not been allocated.
        /// </summary>
        internal int NextUnallocatedBlock
        {
            get
            {
                return m_nextUnallocatedBlock;
            }
        }

        /// <summary>
        /// Returns the number of files that are in this file system. 
        /// </summary>
        /// <returns></returns>
        internal int FileCount
        {
            get
            {
                return m_files.Count;
            }
        }

        /// <summary>
        /// Returns the <see cref="FileMetaData"/> for all of the files of the file system.
        /// </summary>
        internal ReadOnlyCollection<FileMetaData> Files
        {
            get
            {
                return m_readOnlyFiles;
            }
        }

        /// <summary>
        /// This flag is marked as true if while opening the file system, the primary file allocation table was corrupt and a backup had to be used.
        /// </summary>
        internal bool IsTableCompromised
        {
            get
            {
                return m_isTableCompromised;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Allocates a sequential number of blocks at the end of the file and returns the starting address of the allocation
        /// </summary>
        /// <param name="count">the number of blocks to allocate</param>
        /// <returns>the address of the first block of the allocation </returns>
        public int AllocateFreeBlocks(int count)
        {
            if (count == 0)
                throw new ArgumentException("the value 0 is not valid", "count");
            if (IsReadOnly)
                throw new Exception("class is read only");
            int blockAddress = m_nextUnallocatedBlock;
            m_nextUnallocatedBlock += count;
            return blockAddress;
        }

        /// <summary>
        /// Creates an editable copy of this class.
        /// </summary>
        /// <param name="incrementSnapshotSequenceNumber"></param>
        /// <returns></returns>
        internal FileAllocationTable CreateEditableCopy(bool incrementSnapshotSequenceNumber)
        {
            FileAllocationTable table = new FileAllocationTable(this, incrementSnapshotSequenceNumber);
            if (table.IsReadOnly)
                throw new Exception();
            return table;
        }

        /// <summary>
        /// Creates a new file on the file system and returns the <see cref="FileMetaData"/> associated with the new file.
        /// </summary>
        /// <param name="fileExtention">Represents the nature of the data that will be stored in this file.</param>
        /// <returns></returns>
        /// <remarks>A file system only supports 64 files. This is a fundamental limitation and cannot be changed easily.</remarks>
        internal FileMetaData CreateNewFile(Guid fileExtention)
        {
            if (IsReadOnly)
                throw new Exception("File is read only");
            if (m_files.Count >= 64)
                throw new OverflowException("Only 64 files per file system is supported");

            FileMetaData node = FileMetaData.CreateFileMetaData(m_nextFileId, fileExtention);
            m_nextFileId++;
            m_files.Add(node);
            return node;
        }

        /// <summary>
        /// Checks all of the information in the header file 
        /// to verify if it is valid.
        /// </summary>
        bool IsFileAllocationTableValid()
        {
            if (ArchiveId == Guid.Empty) return false;
            if (NextUnallocatedBlock < 10) return false;
            if (m_minimumReadVersion < 0) return false;
            if (m_minimumWriteVersion < 0) return false;
            return true;
        }

        /// <summary>
        /// Creates a new file system and writes the file allocation table to the provided disk.
        /// </summary>
        /// <param name="diskIo"></param>
        void CreateNewFileAllocationTable(DiskIo diskIo)
        {
            m_minimumReadVersion = FileAllocationTableVersion;
            m_minimumWriteVersion = FileAllocationTableVersion;
            m_archiveId = Guid.NewGuid();
            m_isOpenedForExclusiveEditing = false;
            m_snapshotSequenceNumber = 0;
            m_nextFileId = 0;
            m_nextUnallocatedBlock = 10;
            m_files = new List<FileMetaData>();
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

            byte[] dataBytes = new byte[ArchiveConstants.BlockSize];
            MemoryStream stream = new MemoryStream(dataBytes);
            BinaryWriter dataWriter = new BinaryWriter(stream);

            dataWriter.Write(s_fileAllocationTableHeaderBytes);
            dataWriter.Write(m_minimumReadVersion);
            dataWriter.Write(m_minimumWriteVersion);
            dataWriter.Write(ArchiveId.ToByteArray());
            dataWriter.Write((byte)(IsOpenedForExclusiveEditing ? 1 : 0));
            dataWriter.Write(SnapshotSequenceNumber);

            //Write meta data tags.
            //Currently only 1 is recgonized.  So unless there are unrecgonized tags, write 1.
            if (m_unrecgonizedMetaDataTags == null)
                dataWriter.Write((short)1); //meta data page count
            else
                dataWriter.Write((short)(1 + m_unrecgonizedMetaDataTags.Count)); //meta data page count

            byte fileCount = (byte)FileCount;

            dataWriter.Write((short)MetaDataCodes.ListOfFiles); //Meta Data Code
            dataWriter.Write((short)(fileCount * FileMetaData.SizeInBytes + 9)); //Meta data length
            dataWriter.Write(NextUnallocatedBlock);
            dataWriter.Write(m_nextFileId);
            dataWriter.Write(fileCount);

            foreach (FileMetaData node in m_files)
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
        /// <param name="buffer">the block that contains the buffer data.</param>
        void LoadFromBuffer(byte[] buffer)
        {
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
            m_isOpenedForExclusiveEditing = (dataReader.ReadByte() != 0);
            m_snapshotSequenceNumber = dataReader.ReadInt32();

            //Process all of the meta data
            metaDataCount = dataReader.ReadInt16();
            for (; metaDataCount > 0; metaDataCount--)
            {
                int metaDataCode = dataReader.ReadInt16();
                int metaDataLength = dataReader.ReadInt16();
                if (metaDataLength + stream.Position > ArchiveConstants.BlockSize)
                    throw new Exception("File Allocation Tables larger than a block size is not supported");

                switch (metaDataCode)
                {
                    case 1:
                        if (metaDataLength < 9)
                            throw new Exception("The file format is corrupt");
                        m_nextUnallocatedBlock = dataReader.ReadInt32();
                        m_nextFileId = dataReader.ReadInt32();
                        int fileCount = dataReader.ReadByte();
                        if (fileCount > 64)
                            throw new Exception("Only 64 features are supported per archive");
                        if (metaDataLength != fileCount * FileMetaData.SizeInBytes + 9)
                            throw new Exception("The file format is corrupt");

                        m_files = new List<FileMetaData>(fileCount);
                        for (int x = 0; x < fileCount; x++)
                        {
                            m_files.Add(FileMetaData.OpenFileMetaData(dataReader));
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
        }
        /// <summary>
        /// Saves the file allocation table to the disk
        /// </summary>
        /// <param name="diskIo">The place to store the file allocation table.</param>
        public void WriteToFileSystem(DiskIo diskIo)
        {
            byte[] data = GetBytes();
            diskIo.WriteToExistingBlock(0, BlockType.FileAllocationTable, 0, 0, m_snapshotSequenceNumber, data);
            diskIo.WriteToExistingBlock(1, BlockType.FileAllocationTable, 0, 0, m_snapshotSequenceNumber, data);
            diskIo.WriteToExistingBlock(((m_snapshotSequenceNumber & 7) + 2), BlockType.FileAllocationTable, 0, 0, m_snapshotSequenceNumber, data);
        }

        public bool AreEqual(FileAllocationTable other)
        {
            if (other == null)
                return false;
            if (m_isTableCompromised != other.m_isTableCompromised) return false;
            if (m_minimumReadVersion != other.m_minimumReadVersion) return false;
            if (m_minimumWriteVersion != other.m_minimumWriteVersion) return false;
            if (m_archiveId != other.m_archiveId) return false;
            if (m_isOpenedForExclusiveEditing != other.m_isOpenedForExclusiveEditing) return false;
            if (m_snapshotSequenceNumber != other.m_snapshotSequenceNumber) return false;
            if (m_nextFileId != other.m_nextFileId) return false;
            if (m_nextUnallocatedBlock != other.m_nextUnallocatedBlock) return false;
            //compare files.
            if (m_files == null)
            {
                if (other.m_files != null) return false;
            }
            else
            {
                if (other.m_files == null) return false;
                if (m_files.Count != other.m_files.Count) return false;
                for (int x = 0; x < m_files.Count; x++)
                {
                    FileMetaData file = m_files[x];
                    FileMetaData fileOther = other.m_files[x];

                    if (file == null)
                    {
                        if (fileOther != null) return false;
                    }
                    else
                    {
                        if (fileOther == null) return false;
                        if (!file.AreEqual(fileOther)) return false;
                    }
                }
            }
            //compare unrecgonized meta data
            if (m_unrecgonizedMetaDataTags == null)
            {
                if (other.m_unrecgonizedMetaDataTags != null) return false;
            }
            else
            {
                if (other.m_unrecgonizedMetaDataTags == null) return false;
                if (m_unrecgonizedMetaDataTags.Count != other.m_unrecgonizedMetaDataTags.Count) return false;
                for (int x = 0; x < m_unrecgonizedMetaDataTags.Count; x++)
                {
                    byte[] file = m_unrecgonizedMetaDataTags[x];
                    byte[] fileOther = other.m_unrecgonizedMetaDataTags[x];

                    if (file == null)
                    {
                        if (fileOther != null) return false;
                    }
                    else
                    {
                        if (fileOther == null) return false;

                        if (file.Length != fileOther.Length) return false;

                        if (!file.SequenceEqual(fileOther))
                            return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region [ Static ]

        #region [ Methods ]

        /// <summary>
        /// This will open an existing archive header that is read only.
        /// </summary>
        /// <returns></returns>
        public static FileAllocationTable OpenHeader(DiskIo diskIo)
        {
            Exception openException;
            byte[] blockBytes = new byte[ArchiveConstants.BlockSize];

            FileAllocationTable fat0;
            FileAllocationTable fat1;
            FileAllocationTable fat2;
            FileAllocationTable fat3;
            FileAllocationTable fat4;
            FileAllocationTable fat5;
            FileAllocationTable fat6;
            FileAllocationTable fat7;
            FileAllocationTable fat8;
            FileAllocationTable fat9;
            FileAllocationTable latestHeader;

            //Attempt to open and return the first header of the file.
            fat0 = TryOpenFileAllocationTable(0, diskIo, blockBytes, out openException);
            if (fat0 != null)
            {
                if (!fat0.IsReadOnly)
                    throw new Exception();
                fat0.m_isTableCompromised = false;
                return fat0;
            }
            //Attempt to open and return the second header of the file if the first is corrupt.
            fat1 = TryOpenFileAllocationTable(1, diskIo, blockBytes, out openException);
            if (fat1 != null)
            {
                if (!fat1.IsReadOnly)
                    throw new Exception();
                fat1.m_isTableCompromised = true;
                return fat1;
            }
            //Read the next 8 headers of the file. All must not be corrupt. Return the header with the largest FileChangeSequenceNumber.
            //If any of the pages are corrupt, a seperate action must be taken because there is a chance that the user must revert to
            //an older version of the file, which will result in losing data.  
            //If all pages are corrupt, the file is basically unrecoverable unless the user can guess the missing inode table data.

            fat2 = TryOpenFileAllocationTable(2, diskIo, blockBytes, out openException);
            if (fat2 == null)
                throw new Exception("File header is corrupt", openException);

            fat3 = TryOpenFileAllocationTable(3, diskIo, blockBytes, out openException);
            if (fat3 == null)
                throw new Exception("File header is corrupt", openException);

            fat4 = TryOpenFileAllocationTable(4, diskIo, blockBytes, out openException);
            if (fat4 == null)
                throw new Exception("File header is corrupt", openException);

            fat5 = TryOpenFileAllocationTable(5, diskIo, blockBytes, out openException);
            if (fat5 == null)
                throw new Exception("File header is corrupt", openException);

            fat6 = TryOpenFileAllocationTable(6, diskIo, blockBytes, out openException);
            if (fat6 == null)
                throw new Exception("File header is corrupt", openException);

            fat7 = TryOpenFileAllocationTable(7, diskIo, blockBytes, out openException);
            if (fat7 == null)
                throw new Exception("File header is corrupt", openException);

            fat8 = TryOpenFileAllocationTable(8, diskIo, blockBytes, out openException);
            if (fat8 == null)
                throw new Exception("File header is corrupt", openException);

            fat9 = TryOpenFileAllocationTable(9, diskIo, blockBytes, out openException);
            if (fat9 == null)
                throw new Exception("File header is corrupt", openException);

            latestHeader = GetLatestFileAllocationTable(fat2, fat3);
            latestHeader = GetLatestFileAllocationTable(latestHeader, fat4);
            latestHeader = GetLatestFileAllocationTable(latestHeader, fat5);
            latestHeader = GetLatestFileAllocationTable(latestHeader, fat6);
            latestHeader = GetLatestFileAllocationTable(latestHeader, fat7);
            latestHeader = GetLatestFileAllocationTable(latestHeader, fat8);
            latestHeader = GetLatestFileAllocationTable(latestHeader, fat9);

            latestHeader.m_isTableCompromised = true;

            if (!latestHeader.IsReadOnly)
                throw new Exception();
            return latestHeader;
        }

        /// <summary>
        /// Makes an attempt to open a file allocation table from the data buffer. 
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="diskIo"></param>
        /// <param name="tempBuffer"></param>
        /// <param name="error">an output parameter for the error if one was encountered.</param>
        /// <returns>null if there was an error and puts the exception in the error parameter.</returns>
        static FileAllocationTable TryOpenFileAllocationTable(int blockIndex, DiskIo diskIo, byte[] tempBuffer, out Exception error)
        {
            error = null;

            diskIo.Read(blockIndex, BlockType.FileAllocationTable, 0, 0, int.MaxValue, tempBuffer);
            try
            {
                return new FileAllocationTable(tempBuffer);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return null;
        }

        /// <summary>
        /// Returns the file allocation table that has the most recent snapshot sequence number
        /// </summary>
        /// <param name="fat1"></param>
        /// <param name="fat2"></param>
        /// <returns></returns>
        static FileAllocationTable GetLatestFileAllocationTable(FileAllocationTable fat1, FileAllocationTable fat2)
        {
            if (fat1 == null)
                return fat2;
            if (fat2 == null)
                return fat1;
            if (fat1.SnapshotSequenceNumber > fat2.SnapshotSequenceNumber)
                return fat1;
            return fat2;
        }

        /// <summary>
        /// This will create a new File Allocation Table that is editable and writes the data to the File System.
        /// </summary>
        /// <returns></returns>
        public static FileAllocationTable CreateFileAllocationTable(DiskIo file)
        {
            FileAllocationTable table = new FileAllocationTable(file);
            if (table.IsReadOnly)
                throw new Exception();
            return table;
        }

        #endregion
        #endregion
    }
}
