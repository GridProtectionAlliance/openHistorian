//******************************************************************************************************
//  FileHeaderBlock.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using GSF.Collections;
using GSF.IO.FileStructure.Media;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// Contains the information that is in the header page of an archive file.  
    /// </summary>
    public class FileHeaderBlock 
        : SupportsReadonlyBase<FileHeaderBlock>
    {
        #region [ Members ]

        /// <summary>
        /// The file header bytes which equals: "openHistorian 2.0 Archive\00"
        /// </summary>
        static readonly byte[] FileAllocationTableHeaderBytes = Encoding.ASCII.GetBytes("openHistorian 2.0 Archive\0");

        const short FileAllocationReadTableVersion = 0;
        const short FileAllocationWriteTableVersion = 1;

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
        uint m_snapshotSequenceNumber;

        /// <summary>
        /// Since files are allocated sequentially, this value is the next file id that is not used.
        /// </summary>
        ushort m_nextFileId;

        /// <summary>
        /// Returns the last allocated block.
        /// </summary>
        uint m_lastAllocatedBlock;

        /// <summary>
        /// Provides a list of all of the Features that are contained within the file.
        /// </summary>
        ReadonlyList<SubFileMetaData> m_files;

        DateTime m_creationTime;

        DateTime m_lastModifiedTime;

        ReadonlyList<Guid> m_flags;

        /// <summary>
        /// Maintains any meta data tags that existed in the file header that were not recgonized by this version of the file so they can be saved back to the file.
        /// </summary>
        byte[] m_userData;

        int m_blockSize;

        #endregion

        #region [ Constructors ]

        FileHeaderBlock()
        {
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The number of bytes per block for the file structure.
        /// </summary>
        public int BlockSize
        {
            get
            {
                return m_blockSize;
            }
        }

        /// <summary>
        /// Determines if the file can be written to because enough features are recgonized by this current version to do it without corrupting the file system.
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return (m_minimumWriteVersion <= FileAllocationWriteTableVersion);
            }
        }

        /// <summary>
        /// Determines if the archive file can be read
        /// </summary>
        public bool CanRead
        {
            get
            {
                return (m_minimumReadVersion <= FileAllocationWriteTableVersion);
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
        public uint SnapshotSequenceNumber
        {
            get
            {
                return m_snapshotSequenceNumber;
            }
        }

        /// <summary>
        /// Represents the last block that has been allocated
        /// </summary>
        public uint LastAllocatedBlock
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

        /// <summary>
        /// Gets the size of each data block (block size - overhead)
        /// </summary>
        public int DataBlockSize
        {
            get
            {
                return m_blockSize - FileStructureConstants.BlockFooterLength;
            }
        }

        /// <summary>
        /// Gets the total number of bytes consumed in the data space.
        /// </summary>
        public long DataSpace
        {
            get
            {
                long dataBlocks = m_files.Sum(file => file.DataBlockCount);
                return m_blockSize * dataBlocks;
            }
        }

        /// <summary>
        /// Gets the total number of bytes consumed (including shadow copied data)
        /// </summary>
        public long TotalSize
        {
            get
            {
                long allBlocks = m_files.Sum(file => file.TotalBlockCount);
                allBlocks += 10; //The header overhead.
                return m_blockSize * allBlocks;
            }
        }

        /// <summary>
        /// Gets the UTC time when the header for this file was created.
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                return m_creationTime;
            }
        }

        /// <summary>
        /// Gets the UTC time when this file as last committed to the disk.
        /// </summary>
        public DateTime LastModifiedTime
        {
            get
            {
                return m_lastModifiedTime;
            }
        }

        /// <summary>
        /// User definable flags to associate with archive files.
        /// </summary>
        public ReadonlyList<Guid> Flags
        {
            get
            {
                return m_flags;
            }
        }

        /// <summary>
        /// A list of all of the files in this collection.
        /// </summary>
        public ReadonlyList<SubFileMetaData> Files
        {
            get
            {
                return m_files;
            }
        }

        /// <summary>
        /// Custom user data that can be added to a file.
        /// </summary>
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
            FileHeaderBlock clone = base.CloneEditable();
            clone.m_lastModifiedTime = DateTime.UtcNow;
            clone.m_snapshotSequenceNumber++;
            return clone;
        }

        /// <summary>
        /// Requests that member fields be set to readonly. 
        /// </summary>
        protected override void SetMembersAsReadOnly()
        {
            m_files.IsReadOnly = true;
            m_flags.IsReadOnly = true;
        }

        /// <summary>
        /// Request that member fields be cloned and marked as editable.
        /// </summary>
        protected override void CloneMembersAsEditable()
        {
            if (!CanWrite)
                throw new Exception("This file cannot be modified because the file system version is not recgonized");
            m_flags = m_flags.CloneEditable();
            m_files = m_files.CloneEditable();
            m_userData = (byte[])m_userData.Clone();
        }

        /// <summary>
        /// Allocates a sequential number of blocks at the end of the file and returns the starting address of the allocation
        /// </summary>
        /// <param name="count">the number of blocks to allocate</param>
        /// <returns>the address of the first block of the allocation </returns>
        public uint AllocateFreeBlocks(int count)
        {
            if (count <= 0)
                throw new ArgumentException("the value 0 is not valid", "count");
            TestForEditable();
            uint blockAddress = m_lastAllocatedBlock + 1;
            m_lastAllocatedBlock += (uint)count;
            return blockAddress;
        }

        /// <summary>
        /// Creates a new file on the file system and returns the <see cref="SubFileMetaData"/> associated with the new file.
        /// </summary>
        /// <param name="fileName">Represents the nature of the data that will be stored in this file.</param>
        /// <returns></returns>
        /// <remarks>A file system only supports 64 files. This is a fundamental limitation and cannot be changed easily.</remarks>
        public SubFileMetaData CreateNewFile(SubFileName fileName)
        {
            base.TestForEditable();
            if (!CanWrite)
                throw new Exception("Writing to this file type is not supported");
            if (IsReadOnly)
                throw new Exception("File is read only");
            if (m_files.Count >= 64)
                throw new OverflowException("Only 64 files per file system is supported");
            if (ContainsSubFile(fileName))
                throw new DuplicateNameException("Name already exists");

            SubFileMetaData node = new SubFileMetaData(m_nextFileId, fileName, isImmutable: false);
            m_nextFileId++;
            m_files.Add(node);
            return node;
        }

        /// <summary>
        /// Determines if the file contains the subfile
        /// </summary>
        /// <param name="fileName">the subfile to look for</param>
        /// <returns>true if contained, false otherwise</returns>
        public bool ContainsSubFile(SubFileName fileName)
        {
            return m_files.Any(file => file.FileName == fileName);
        }

        /// <summary>
        /// Checks all of the information in the header file 
        /// to verify if it is valid.
        /// </summary>
        private bool IsFileAllocationTableValid()
        {
            if (ArchiveId == Guid.Empty) return false;
            if (LastAllocatedBlock < 9) return false;
            if (m_minimumReadVersion < 0) return false;
            if (m_minimumWriteVersion < 0) return false;
            return true;
        }

        /// <summary>
        /// This will return a byte array of data that can be written to an archive file.
        /// </summary>
        public byte[] GetBytes()
        {
            if (!IsFileAllocationTableValid())
                throw new InvalidOperationException("File Allocation Table is invalid");

            byte[] dataBytes = new byte[m_blockSize];
            MemoryStream stream = new MemoryStream(dataBytes);
            BinaryWriter dataWriter = new BinaryWriter(stream);

            dataWriter.Write(FileAllocationTableHeaderBytes);

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
            dataWriter.Write(m_creationTime.Ticks);
            dataWriter.Write(m_lastModifiedTime.Ticks);
            dataWriter.Write(m_flags.Count);
            foreach (var flag in m_flags)
            {
                dataWriter.Write(flag.ToByteArray());
            }

            if (stream.Position + 32 > dataBytes.Length)
                throw new Exception("the file size exceedes the allowable size.");

            WriteFooterData(dataBytes);
            return dataBytes;
        }

        /// <summary>
        /// This procedure will attempt to read all of the data out of the file allocation table
        /// If the file allocation table is corrupt, an error will be generated.
        /// </summary>
        /// <param name="buffer">the block that contains the buffer data.</param>
        private void LoadFromBuffer(byte[] buffer)
        {
            ValidateBlock(buffer);

            m_blockSize = buffer.Length;
            MemoryStream stream = new MemoryStream(buffer);
            BinaryReader dataReader = new BinaryReader(stream);

            if (!dataReader.ReadBytes(26).SequenceEqual(FileAllocationTableHeaderBytes))
                throw new Exception("This file is not an archive file system, or the file is corrupt, or this file system major version is not recgonized by this version of the historian");

            char endian = (char)dataReader.ReadByte();
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

            m_snapshotSequenceNumber = dataReader.ReadUInt32();
            m_lastAllocatedBlock = dataReader.ReadUInt32();
            m_nextFileId = dataReader.ReadUInt16();
            int fileCount = dataReader.ReadInt32();

            //ToDo: check based on block length
            if (fileCount > 64)
                throw new Exception("Only 64 features are supported per archive");

            m_files = new ReadonlyList<SubFileMetaData>(fileCount);
            for (int x = 0; x < fileCount; x++)
            {
                m_files.Add(new SubFileMetaData(dataReader, isImmutable: true));
            }

            //ToDo: check based on block length
            int userSpaceLength = dataReader.ReadInt32();
            m_userData = dataReader.ReadBytes(userSpaceLength);

            if (m_minimumWriteVersion == 1)
            {
                m_creationTime = new DateTime(dataReader.ReadInt64());
                m_lastModifiedTime = new DateTime(dataReader.ReadInt64());
                int flagCount = dataReader.ReadInt32();
                m_flags = new ReadonlyList<Guid>(flagCount);
                while (flagCount > 0)
                {
                    flagCount--;
                    m_flags.Add(new Guid(dataReader.ReadBytes(16)));
                }
            }
            else
            {
                m_creationTime = DateTime.MinValue;
                m_lastModifiedTime = DateTime.MinValue;
                m_flags = new ReadonlyList<Guid>();
            }


            if (!IsFileAllocationTableValid())
                throw new Exception("File System is invalid");
            IsReadOnly = true;
        }

        private unsafe void ValidateBlock(byte[] buffer)
        {
            long checksum1;
            int checksum2;
            fixed (byte* lpData = buffer)
            {
                Footer.ComputeChecksum((IntPtr)lpData, out checksum1, out checksum2, buffer.Length - 16);

                long checksumInData1 = *(long*)(lpData + buffer.Length - 16);
                int checksumInData2 = *(int*)(lpData + buffer.Length - 8);
                if (checksum1 != checksumInData1 || checksum2 != checksumInData2)
                {
                    throw new Exception("Checksum on header file is invalid");
                }
                if (lpData[buffer.Length - 32] != (byte)BlockType.FileAllocationTable)
                    throw new Exception("IoReadState.BlockTypeMismatch");
                if (*(int*)(lpData + buffer.Length - 28) != 0)
                    throw new Exception("IoReadState.IndexNumberMissmatch");
                if (*(int*)(lpData + buffer.Length - 24) != 0)
                    throw new Exception("IoReadState.FileIdNumberDidNotMatch");
            }
        }

        private unsafe void WriteFooterData(byte[] buffer)
        {
            fixed (byte* lpData = buffer)
            {
                long checksum1;
                int checksum2;

                lpData[buffer.Length - 32] = (byte)BlockType.FileAllocationTable;
                *(int*)(lpData + buffer.Length - 28) = 0;
                *(int*)(lpData + buffer.Length - 24) = 0;
                *(int*)(lpData + buffer.Length - 20) = 0;

                Footer.ComputeChecksum((IntPtr)lpData, out checksum1, out checksum2, buffer.Length - 16);
                *(long*)(lpData + buffer.Length - 16) = checksum1;
                *(int*)(lpData + buffer.Length - 8) = checksum2;
                *(int*)(lpData + buffer.Length - 4) = 0;
            }
        }

#endregion

        #region [ Static ] 

        /// <summary>
        /// Looks in the contents of a file for the block size of the file.
        /// </summary>
        /// <param name="stream">the stream to look</param>
        /// <returns>the number of bytes in a block. Always a power of 2.</returns>
        public static int SearchForBlockSize(Stream stream)
        {
            long oldPosition = stream.Position;
            stream.Position = 0;

            if (!stream.ReadBytes(26).SequenceEqual(FileAllocationTableHeaderBytes))
                throw new Exception("This file is not an archive file system, or the file is corrupt, or this file system major version is not recgonized by this version of the historian");

            char endian = (char)stream.ReadNextByte();
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

            byte blockSizePower = stream.ReadNextByte();
            if (blockSizePower > 30 || blockSizePower < 5) //Stored as 2^n power. 
                throw new Exception("Block size of this file is not supported");
            int blockSize = 1 << blockSizePower;
            stream.Position = oldPosition;
            return blockSize;
        }


        /// <summary>
        /// Creates a new file header.
        /// </summary>
        /// <param name="blockSize">The block size to make the header</param>
        /// <param name="uniqueFileId">a guid that will be the unique identifier of this file. If Guid.Empty one will be generated in the constructor</param>
        /// <param name="flags">Flags to write to the file</param>
        /// <returns></returns>
        public static FileHeaderBlock CreateNew(int blockSize, Guid uniqueFileId = default(Guid), params Guid[] flags)
        {
            FileHeaderBlock header = new FileHeaderBlock();
            header.m_blockSize = blockSize;
            header.m_minimumReadVersion = FileAllocationReadTableVersion;
            header.m_minimumWriteVersion = FileAllocationWriteTableVersion;
            if (uniqueFileId == Guid.Empty)
            {
                header.m_archiveId = Guid.NewGuid();
            }
            else
            {
                header.m_archiveId = uniqueFileId;
            }
            header.m_snapshotSequenceNumber = 1;
            header.m_nextFileId = 0;
            header.m_lastAllocatedBlock = 9;
            header.m_files = new ReadonlyList<SubFileMetaData>();
            header.m_flags = new ReadonlyList<Guid>();
            header.m_userData = new byte[] { };
            header.m_archiveType = Guid.Empty;
            header.m_creationTime = DateTime.UtcNow;
            header.m_lastModifiedTime = header.m_creationTime;
            foreach (var f in flags)
                header.Flags.Add(f);

            header.IsReadOnly = true;
            return header;
        }

        /// <summary>
        /// Opens a file header
        /// </summary>
        /// <param name="data">The block of data to be loaded. The length of this block must be equal to the
        /// block size of a partition.</param>
        /// <returns></returns>
        public static FileHeaderBlock Open(byte[] data)
        {
            FileHeaderBlock header = new FileHeaderBlock();
            header.LoadFromBuffer(data);
            header.IsReadOnly = true;
            return header;
        }

        #endregion

    }
}