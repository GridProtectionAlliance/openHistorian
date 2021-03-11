//******************************************************************************************************
//  FileHeaderBlock.cs - Gbtc
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
//  12/03/2011 - Steven E. Chisholm
//       Generated original version of source code. That is capable of reading/writing header version 0
//  10/11/2014 - Steven E. Chisholm
//       Added header version 2
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using GSF.Immutable;
using GSF.IO.FileStructure.Media;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// Contains the information that is in the header page of an archive file.  
    /// </summary>
    public class FileHeaderBlock
        : ImmutableObjectBase<FileHeaderBlock>
    {
        #region [ Members ]

        /// <summary>
        /// The file header bytes which equals: "openHistorian 2.0 Archive\00"
        /// </summary>
        private static readonly byte[] FileAllocationTableHeaderBytes = Encoding.ASCII.GetBytes("openHistorian 2.0 Archive\0");

        private const short FileAllocationReadTableVersion = 2;
        private const short FileAllocationWriteTableVersion = 2;
        private const short FileAllocationHeaderVersion = 2;

        /// <summary>
        /// The size of the block.
        /// </summary>
        private int m_blockSize;

        /// <summary>
        /// The version number required to read the file system.
        /// </summary>
        private short m_minimumReadVersion;

        /// <summary>
        /// The version number required to write to the file system.
        /// </summary>
        private short m_minimumWriteVersion;

        /// <summary>
        /// The version of the header.
        /// </summary>
        private short m_headerVersion;

        /// <summary>
        /// Gets if this file uses the simplifed file format.
        /// </summary>
        private bool m_isSimplifiedFileFormat;

        /// <summary>
        /// Gets the number of times the file header is repeated
        /// </summary>
        private byte m_headerBlockCount;

        /// <summary>
        /// Returns the last allocated block.
        /// </summary>
        private uint m_lastAllocatedBlock;

        /// <summary>
        /// This will be updated every time the file system has been modified. Initially, it will be one.
        /// </summary>
        private uint m_snapshotSequenceNumber;

        /// <summary>
        /// Since files are allocated sequentially, this value is the next file id that is not used.
        /// </summary>
        private ushort m_nextFileId;

        /// <summary>
        /// The GUID for this archive file system.
        /// </summary>
        private Guid m_archiveId;

        /// <summary>
        /// The GUID to represent the type of this archive file.
        /// </summary>
        private Guid m_archiveType;

        /// <summary>
        /// Provides a list of all of the Features that are contained within the file.
        /// </summary>
        private ImmutableList<SubFileHeader> m_files;

        /// <summary>
        /// A set of file flags describing this file.
        /// </summary>
        private ImmutableList<Guid> m_flags;

        private Dictionary<short, byte[]> m_unknownAttributes;

        private Dictionary<Guid, byte[]> m_userAttributes;

        #endregion

        #region [ Constructors ]

            private FileHeaderBlock()
        {
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The number of bytes per block for the file structure.
        /// </summary>
        public int BlockSize => m_blockSize;

        /// <summary>
        /// Determines if the file can be written to because enough features are recgonized by this current version to do it without corrupting the file system.
        /// </summary>
        public bool CanWrite =>
            //ToDo: Support changing files that are from an older version.
            m_minimumWriteVersion == FileAllocationWriteTableVersion;

        /// <summary>
        /// Determines if the archive file can be read
        /// </summary>
        public bool CanRead => m_minimumReadVersion <= FileAllocationWriteTableVersion;

        /// <summary>
        /// The GUID number for this archive.
        /// </summary>
        public Guid ArchiveId => m_archiveId;

        /// <summary>
        /// The GUID number for this archive.
        /// </summary>
        public Guid ArchiveType
        {
            get => m_archiveType;
            set
            {
                TestForEditable();
                m_archiveType = value;
            }
        }

        /// <summary>
        /// Gets if this file uses the simplifed file format.
        /// </summary>
        public bool IsSimplifiedFileFormat => m_isSimplifiedFileFormat;

        /// <summary>
        /// Gets the number of times the file header exists in the archive file.
        /// </summary>
        public byte HeaderBlockCount => m_headerBlockCount;

        /// <summary>
        /// Maintains a sequential number that represents the version of the file.
        /// </summary>
        public uint SnapshotSequenceNumber => m_snapshotSequenceNumber;

        /// <summary>
        /// Represents the last block that has been allocated
        /// </summary>
        public uint LastAllocatedBlock => m_lastAllocatedBlock;

        /// <summary>
        /// Returns the number of files that are in this file system. 
        /// </summary>
        /// <returns></returns>
        public int FileCount => m_files.Count;

        /// <summary>
        /// Gets the size of each data block (block size - overhead)
        /// </summary>
        public int DataBlockSize => m_blockSize - FileStructureConstants.BlockFooterLength;

        /// <summary>
        /// User definable flags to associate with archive files.
        /// </summary>
        public ImmutableList<Guid> Flags => m_flags;

        /// <summary>
        /// A list of all of the files in this collection.
        /// </summary>
        public ImmutableList<SubFileHeader> Files => m_files;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// Clones the object, while incrementing the sequence number.
        /// </summary>
        /// <returns></returns>
        public override FileHeaderBlock CloneEditable()
        {
            FileHeaderBlock clone = base.CloneEditable();
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

            if (m_userAttributes != null)
            {
                m_userAttributes = new Dictionary<Guid, byte[]>(m_userAttributes);
            }
            if (m_unknownAttributes != null)
            {
                m_unknownAttributes = new Dictionary<short, byte[]>(m_unknownAttributes);
            }
        }

        /// <summary>
        /// Allocates a sequential number of blocks at the end of the file and returns the starting address of the allocation
        /// </summary>
        /// <param name="count">the number of blocks to allocate</param>
        /// <returns>the address of the first block of the allocation </returns>
        public uint AllocateFreeBlocks(uint count)
        {
            TestForEditable();
            uint blockAddress = m_lastAllocatedBlock + 1;
            m_lastAllocatedBlock += count;
            return blockAddress;
        }

        /// <summary>
        /// Creates a new file on the file system and returns the <see cref="SubFileHeader"/> associated with the new file.
        /// </summary>
        /// <param name="fileName">Represents the nature of the data that will be stored in this file.</param>
        /// <returns></returns>
        /// <remarks>A file system only supports 64 files. This is a fundamental limitation and cannot be changed easily.</remarks>
        public SubFileHeader CreateNewFile(SubFileName fileName)
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

            SubFileHeader node = new SubFileHeader(m_nextFileId, fileName, isImmutable: false, isSimplified: IsSimplifiedFileFormat);
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
            if (m_headerVersion < 0) return false;
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
            dataWriter.Write((byte)BitMath.CountBitsSet((uint)(m_blockSize - 1)));

            dataWriter.Write(FileAllocationReadTableVersion);
            dataWriter.Write(FileAllocationWriteTableVersion);
            dataWriter.Write(FileAllocationHeaderVersion);
            dataWriter.Write((byte)(m_isSimplifiedFileFormat ? 2 : 1));
            dataWriter.Write(m_headerBlockCount);
            dataWriter.Write(m_lastAllocatedBlock);
            dataWriter.Write(m_snapshotSequenceNumber);
            dataWriter.Write(m_nextFileId);
            dataWriter.Write(m_archiveId.ToByteArray());
            dataWriter.Write(m_archiveType.ToByteArray());
            dataWriter.Write((short)m_files.Count);
            foreach (SubFileHeader node in m_files)
            {
                node.Save(dataWriter);
            }

            //Metadata Flags
            if (m_flags.Count > 0)
            {
                Encoding7Bit.WriteInt15(dataWriter.Write, (short)FileHeaderAttributes.FileFlags);
                Encoding7Bit.WriteInt15(dataWriter.Write, (short)(m_flags.Count * 16));
                foreach (Guid flag in m_flags)
                {
                    dataWriter.Write(GuidExtensions.ToLittleEndianBytes(flag));
                }
            }

            if (m_unknownAttributes != null)
            {
                foreach (KeyValuePair<short, byte[]> md in m_unknownAttributes)
                {
                    Encoding7Bit.WriteInt15(dataWriter.Write, md.Key);
                    Encoding7Bit.WriteInt15(dataWriter.Write, (short)md.Value.Length);
                    dataWriter.Write(md.Value);
                }
            }

            if (m_userAttributes != null)
            {
                foreach (KeyValuePair<Guid, byte[]> md in m_userAttributes)
                {
                    Encoding7Bit.WriteInt15(dataWriter.Write, (short)FileHeaderAttributes.UserAttributes);
                    dataWriter.Write(GuidExtensions.ToLittleEndianBytes(md.Key));
                    Encoding7Bit.WriteInt15(dataWriter.Write, (short)md.Value.Length);
                    dataWriter.Write(md.Value);
                }
            }

            Encoding7Bit.WriteInt15(dataWriter.Write, (short)FileHeaderAttributes.EndOfAttributes);
            Encoding7Bit.WriteInt15(dataWriter.Write, 0);

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

            m_headerVersion = m_minimumWriteVersion;
            if (m_headerVersion < 0)
            {
                throw new Exception("Header version not supported");
            }
            if (m_headerVersion == 0 || m_headerVersion == 1)
            {
                m_isSimplifiedFileFormat = false;
                m_headerBlockCount = 10;
                LoadHeaderV0V1(dataReader);
                return;
            }

            m_headerVersion = dataReader.ReadInt16();
            byte fileMode = dataReader.ReadByte();
            if (fileMode == 1)
            {
                m_isSimplifiedFileFormat = false;
            }
            else if (fileMode == 2)
            {
                m_isSimplifiedFileFormat = true;
            }
            else
            {
                throw new Exception("Unknown File Mode");
            }

            m_headerBlockCount = dataReader.ReadByte();
            m_lastAllocatedBlock = dataReader.ReadUInt32();
            m_snapshotSequenceNumber = dataReader.ReadUInt32();
            m_nextFileId = dataReader.ReadUInt16();
            m_archiveId = new Guid(dataReader.ReadBytes(16));
            m_archiveType = new Guid(dataReader.ReadBytes(16));

            int fileCount = dataReader.ReadInt16();
            //ToDo: check based on block length
            if (fileCount > 64)
                throw new Exception("Only 64 features are supported per archive");

            m_files = new ImmutableList<SubFileHeader>(fileCount);
            for (int x = 0; x < fileCount; x++)
            {
                m_files.Add(new SubFileHeader(dataReader, isImmutable: true, isSimplified: m_isSimplifiedFileFormat));
            }

            m_flags = new ImmutableList<Guid>();

            FileHeaderAttributes tag = (FileHeaderAttributes)Encoding7Bit.ReadInt15(dataReader.ReadByte);
            while (tag != FileHeaderAttributes.EndOfAttributes)
            {
                short dataLen;
                switch (tag)
                {
                    case FileHeaderAttributes.FileFlags:
                        dataLen = Encoding7Bit.ReadInt15(dataReader.ReadByte);
                        while (dataLen > 0)
                        {
                            dataLen -= 16;
                            m_flags.Add(GuidExtensions.ToLittleEndianGuid(dataReader.ReadBytes(16)));
                        }
                        break;
                    case FileHeaderAttributes.UserAttributes:
                        Guid flag = GuidExtensions.ToLittleEndianGuid(dataReader.ReadBytes(16));
                        dataLen = Encoding7Bit.ReadInt15(dataReader.ReadByte);
                        AddUserAttribute(flag, dataReader.ReadBytes(dataLen));
                        break;
                    default:
                        dataLen = Encoding7Bit.ReadInt15(dataReader.ReadByte);
                        AddUnknownAttribute((byte)tag, dataReader.ReadBytes(dataLen));
                        break;
                }
                tag = (FileHeaderAttributes)dataReader.ReadByte();
            }

            if (!IsFileAllocationTableValid())
                throw new Exception("File System is invalid");
            IsReadOnly = true;
        }

        private void AddUserAttribute(Guid id, byte[] data)
        {
            if (m_userAttributes is null)
                m_userAttributes = new Dictionary<Guid, byte[]>();
            if (!m_userAttributes.ContainsKey(id))
                m_userAttributes.Add(id, data);
            else
                m_userAttributes[id] = data;
        }

        private void AddUnknownAttribute(byte id, byte[] data)
        {
            if (m_unknownAttributes is null)
                m_unknownAttributes = new Dictionary<short, byte[]>();
            if (!m_unknownAttributes.ContainsKey(id))
                m_unknownAttributes.Add(id, data);
            else
                m_unknownAttributes[id] = data;
        }


        private void LoadHeaderV0V1(BinaryReader dataReader)
        {
            m_archiveId = new Guid(dataReader.ReadBytes(16));
            m_archiveType = new Guid(dataReader.ReadBytes(16));

            m_snapshotSequenceNumber = dataReader.ReadUInt32();
            m_lastAllocatedBlock = dataReader.ReadUInt32();
            m_nextFileId = dataReader.ReadUInt16();
            int fileCount = dataReader.ReadInt32();

            //ToDo: check based on block length
            if (fileCount > 64)
                throw new Exception("Only 64 features are supported per archive");

            m_files = new ImmutableList<SubFileHeader>(fileCount);
            for (int x = 0; x < fileCount; x++)
            {
                m_files.Add(new SubFileHeader(dataReader, isImmutable: true, isSimplified: false));
            }

            //ToDo: check based on block length
            int userSpaceLength = dataReader.ReadInt32();
            dataReader.ReadBytes(userSpaceLength);

            if (m_minimumWriteVersion == 1)
            {
                new DateTime(dataReader.ReadInt64());
                new DateTime(dataReader.ReadInt64());
                int flagCount = dataReader.ReadInt32();
                m_flags = new ImmutableList<Guid>(flagCount);
                while (flagCount > 0)
                {
                    flagCount--;
                    m_flags.Add(new Guid(dataReader.ReadBytes(16)));
                }
            }
            else
                m_flags = new ImmutableList<Guid>();


            if (!IsFileAllocationTableValid())
                throw new Exception("File System is invalid");
            IsReadOnly = true;
        }

        private unsafe void ValidateBlock(byte[] buffer)
        {
            fixed (byte* lpData = buffer)
            {
                Footer.ComputeChecksum((IntPtr)lpData, out long checksum1, out int checksum2, buffer.Length - 16);

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
                lpData[buffer.Length - 32] = (byte)BlockType.FileAllocationTable;
                *(int*)(lpData + buffer.Length - 28) = 0;
                *(int*)(lpData + buffer.Length - 24) = 0;
                *(int*)(lpData + buffer.Length - 20) = 0;

                Footer.ComputeChecksum((IntPtr)lpData, out long checksum1, out int checksum2, buffer.Length - 16);
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
        /// <param name="flags">Flags to write to the file</param>
        /// <returns></returns>
        public static FileHeaderBlock CreateNew(int blockSize, params Guid[] flags)
        {
            FileHeaderBlock header = new FileHeaderBlock();
            header.m_blockSize = blockSize;
            header.m_minimumReadVersion = FileAllocationReadTableVersion;
            header.m_minimumWriteVersion = FileAllocationWriteTableVersion;
            header.m_headerVersion = FileAllocationHeaderVersion;
            header.m_headerBlockCount = 10;
            header.m_isSimplifiedFileFormat = false;
            header.m_archiveId = Guid.NewGuid();
            header.m_snapshotSequenceNumber = 1;
            header.m_nextFileId = 0;
            header.m_lastAllocatedBlock = 9;
            header.m_files = new ImmutableList<SubFileHeader>();
            header.m_flags = new ImmutableList<Guid>();
            header.m_archiveType = Guid.Empty;
            foreach (Guid f in flags)
            {
                header.Flags.Add(f);
            }

            header.IsReadOnly = true;
            return header;
        }
        /// <summary>
        /// Creates a new file header.
        /// </summary>
        /// <param name="blockSize">The block size to make the header</param>
        /// <param name="flags">Flags to write to the file</param>
        /// <returns></returns>
        public static FileHeaderBlock CreateNewSimplified(int blockSize, params Guid[] flags)
        {
            FileHeaderBlock header = new FileHeaderBlock();
            header.m_blockSize = blockSize;
            header.m_minimumReadVersion = FileAllocationReadTableVersion;
            header.m_minimumWriteVersion = FileAllocationWriteTableVersion;
            header.m_headerVersion = FileAllocationHeaderVersion;
            header.m_headerBlockCount = 1;
            header.m_isSimplifiedFileFormat = true;
            header.m_archiveId = Guid.NewGuid();
            header.m_snapshotSequenceNumber = 1;
            header.m_nextFileId = 0;
            header.m_lastAllocatedBlock = 0;
            header.m_files = new ImmutableList<SubFileHeader>();
            header.m_flags = new ImmutableList<Guid>();
            header.m_archiveType = Guid.Empty;
            foreach (Guid f in flags)
            {
                header.Flags.Add(f);
            }

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