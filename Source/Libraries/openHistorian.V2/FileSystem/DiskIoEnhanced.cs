//******************************************************************************************************
//  DiskIoEnhanced.cs - Gbtc
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
//  3/24/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.Unmanaged;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// Abstract class for the basic Disk IO functions.
    /// </summary>
    unsafe internal class DiskIoEnhanced : IDisposable
    {

        /// <summary>
        /// Contains basic information about a page of memory
        /// </summary>
        private class MemoryUnit : IMemoryUnit
        {
            DiskIoEnhanced m_diskIo;
            bool m_isValid;
            bool m_isReadOnly;
            int m_length;
            uint m_blockAddress;
            public byte* m_pointer;

            public bool IsValid
            {
                get
                {
                    return m_isValid;
                }
                set
                {
                    m_isValid = value;
                }
            }
            public bool IsReadOnly
            {
                get
                {
                    if (!m_isValid)
                        throw new Exception("Value is invalid");
                    return m_isReadOnly;
                }
                set
                {
                    m_isReadOnly = value;
                }
            }
            public int Length
            {
                get
                {
                    if (!m_isValid)
                        throw new Exception("Value is invalid");
                    return m_length;
                }
                set
                {
                    m_length = value;
                }
            }
            public uint BlockIndex
            {
                get
                {
                    if (!m_isValid)
                        throw new Exception("Value is invalid");
                    return m_blockAddress;
                }
                set
                {
                    m_blockAddress = value;
                }
            }
            public byte* Pointer
            {
                get
                {
                    if (!m_isValid)
                        throw new Exception("Value is invalid");
                    return m_pointer;
                }
                set
                {
                    m_pointer = value;
                }
            }

            public MemoryUnit(DiskIoEnhanced diskIo)
            {
                m_diskIo = diskIo;
                m_isValid = false;
                m_diskIo.RegisterBlock(this);
            }

            ~MemoryUnit()
            {
                m_diskIo.UnregisterBlock(this);
                //Debugger.Break();
                //Debug.Assert(false, "Memory object failed to properly be disposed of.");
            }

            public void Dispose()
            {
                m_diskIo.UnregisterBlock(this);
                GC.SuppressFinalize(this);
            }
        }

        int m_allocatedBlocksCount;

        protected MemoryStream m_stream;
        ISupportsBinaryStream m_stream2;

        public DiskIoEnhanced()
        {
            m_allocatedBlocksCount = 0;
            m_stream = new MemoryStream();
            m_stream2 = m_stream;
        }

        public IMemoryUnit GetMemoryUnit()
        {
            return new MemoryUnit(this);
        }
        void RegisterBlock(MemoryUnit page)
        {
            //Do Nothing
        }
        void UnregisterBlock(MemoryUnit page)
        {
            //Do Nothing
        }

        /// <summary>
        /// Writes a specific block of data to the disk system.
        /// </summary>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number of this write</param>
        /// <param name="buffer">the data to write. It must be equal to <see cref="ArchiveConstants.BlockSize"/>.</param>
        public void WriteBlock(BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber, IMemoryUnit buffer)
        {
            MemoryUnit data = (MemoryUnit)buffer;
            if (!data.IsValid)
                throw new Exception("Buffer is not defined");
            if (data.IsReadOnly)
                throw new Exception("Buffer was opened as read only");
            //ToDo: Consider reloading the origional data if a buffer was modified when it wasn't supposed to be
            //This is actually a pretty serious bug.

            if (IsReadOnly)
                throw new Exception("File system is read only");

            //If the file is not large enought to set this block, autogrow the file.
            if ((long)(data.BlockIndex + 1) * ArchiveConstants.BlockSize > FileSize)
            {
                SetFileLength(0, data.BlockIndex + 1);
            }

            WriteFooterData(buffer.Pointer, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
        }

        public void AquireBlockForWrite(uint blockIndex, IMemoryUnit buffer)
        {
            if (IsReadOnly)
                throw new Exception("File system is read only");
            MemoryUnit data = (MemoryUnit)buffer;
            data.IsValid = false;
            ReadForWrite(blockIndex, data);
            data.IsValid = true;
        }
        public IoReadState AquireBlockForWrite(uint blockIndex, BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber, IMemoryUnit buffer)
        {
            MemoryUnit data = (MemoryUnit)buffer;
            data.IsValid = false;
            IoReadState readState = ReadBlock(blockIndex, data);

            if (readState != IoReadState.Valid)
                return readState;

            readState = IsFooterValid(data.m_pointer, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);

            if (readState != IoReadState.Valid)
                return readState;
            data.IsReadOnly = false;
            data.IsValid = true;
            return readState;
        }

        public IoReadState AquireBlockForRead(uint blockIndex, BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber, IMemoryUnit buffer)
        {
            MemoryUnit data = (MemoryUnit)buffer;
            data.IsValid = false;
            IoReadState readState = ReadBlock(blockIndex, data);

            if (readState != IoReadState.Valid)
                return readState;

            readState = IsFooterValid(data.m_pointer, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);

            if (readState != IoReadState.Valid)
                return readState;

            data.IsValid = true;

            return readState;
        }

        #region [ Abstract Methods/Properties ]

        /// <summary>
        /// Writes the following data to the stream
        /// </summary>
        /// <param name="blockIndex">the block where to write the data</param>
        /// <param name="data">the data to write</param>
        protected void WriteBlock(uint blockIndex, byte[] data)
        {
            m_stream.Write(blockIndex * ArchiveConstants.BlockSize, data, 0, data.Length);
        }

        /// <summary>
        /// Resizes the file to the requested size
        /// </summary>
        /// <param name="requestedSize">The size to resize to</param>
        /// <returns>The actual size of the file after the resize</returns>
        protected long SetFileLength(long requestedSize)
        {
            m_stream.Position = requestedSize - 1;
            m_stream.Read(requestedSize - 1, new byte[1], 0, 1);
            m_allocatedBlocksCount = (int)(requestedSize / ArchiveConstants.BlockSize);
            return m_allocatedBlocksCount * ArchiveConstants.BlockSize;
        }

        /// <summary>
        /// Tries to read data from the following file
        /// </summary>
        /// <param name="blockIndex">the block where to write the data</param>
        /// <param name="data">the data to write</param>
        /// <returns>A status whether the read was sucessful. See <see cref="IoReadState"/>.</returns>
        protected IoReadState ReadBlock(uint blockIndex, byte[] data)
        {
            if (blockIndex > m_allocatedBlocksCount)
                return IoReadState.ReadPastThenEndOfTheFile;

            m_stream.Read(blockIndex * ArchiveConstants.BlockSize, data, 0, data.Length);
            return IoReadState.Valid;
        }

        /// <summary>
        /// Tries to read data from the following file
        /// </summary>
        /// <param name="blockIndex">the block where to write the data</param>
        /// <param name="memory">the data to write</param>
        /// <returns>A status whether the read was sucessful. See <see cref="IoReadState"/>.</returns>
        IoReadState ReadBlock(uint blockIndex, MemoryUnit memory)
        {

            if (blockIndex > m_allocatedBlocksCount)
                return IoReadState.ReadPastThenEndOfTheFile;

            IntPtr ptr;
            int length;
            long pos;
            bool supportsWriting;

            m_stream2.GetBlock(0,blockIndex * ArchiveConstants.BlockSize, false, out ptr, out pos, out length, out supportsWriting);

            int cur = (int)(blockIndex * ArchiveConstants.BlockSize - pos);
            //m_stream2.GetCurrentBlock(blockIndex * ArchiveConstants.BlockSize, false, out ptr, out first, out last, out cur);

            if (length - cur < ArchiveConstants.BlockSize)
                throw new Exception("memory is not lining up on page boundries");

            byte* data = (byte*)(ptr + cur);

            memory.BlockIndex = blockIndex;
            memory.Pointer = data;
            memory.Length = ArchiveConstants.BlockSize;
            memory.IsReadOnly = true;

            return IoReadState.Valid;
        }

        /// <summary>
        /// Tries to read data from the following file
        /// </summary>
        /// <param name="blockIndex">the block where to write the data</param>
        /// <param name="memory">the data to write</param>
        /// <returns>A status whether the read was sucessful. See <see cref="IoReadState"/>.</returns>
        void ReadForWrite(uint blockIndex, MemoryUnit memory)
        {
            IntPtr ptr;
            int length;
            long pos;
            bool supportsWriting;

            m_stream2.GetBlock(0, blockIndex * ArchiveConstants.BlockSize, false, out ptr, out pos, out length, out supportsWriting);

            int cur = (int)(blockIndex * ArchiveConstants.BlockSize - pos);

            if (length - cur < ArchiveConstants.BlockSize)
                throw new Exception("memory is not lining up on page boundries");

            byte* data = (byte*)(ptr + cur);


            memory.BlockIndex = blockIndex;
            memory.Pointer = data;
            memory.Length = ArchiveConstants.BlockSize;
            memory.IsReadOnly = false;
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        /// Gets the current size of the file.
        /// </summary>
        public long FileSize
        {
            get
            {
                return m_allocatedBlocksCount * (long)ArchiveConstants.BlockSize;
            }
        }

        #endregion

        /// <summary>
        /// The calculated checksum for a page of all zeros.
        /// </summary>
        const long EmptyChecksum = 6845471437889732609;

        /// <summary>
        /// Checks how many times the checksum was computed.  This is used to see IO amplification.
        /// It is currently a debug term that will soon disappear.
        /// </summary>
        static internal long ChecksumCount;

        /// <summary>
        /// Computes the custom checksum of the data.
        /// </summary>
        /// <param name="data">the data to compute the checksum for.</param>
        /// <returns></returns>
        static long ComputeChecksum(byte* data)
        {
            ChecksumCount += 1;
            // return 0;

            long a = 1; //Maximum size for A is 20 bits in length
            long b = 0; //Maximum size for B is 31 bits in length
            long c = 0; //Maximum size for C is 42 bits in length
            for (int x = 0; x < ArchiveConstants.BlockSize - 8; x++)
            {
                a += data[x];
                b += a;
                c += b;
            }
            //Since only 13 bits of C will remain, xor all 42 bits of C into the first 13 bits.
            c = c ^ (c >> 13) ^ (c >> 26) ^ (c >> 39);
            return (c << 51) ^ (b << 20) ^ a;
        }

        /// <summary>
        /// Determines if the footer data for the following page is valid.
        /// </summary>
        /// <param name="data">the block data to check</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        /// <returns>State information about the state of the footer data</returns>
        static IoReadState IsFooterValid(byte* data, BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber)
        {
            long checksum = ComputeChecksum(data);
            long checksumInData = *(long*)(data + ArchiveConstants.BlockSize - 8);

            if (checksum == checksumInData)
            {
                if (data[ArchiveConstants.BlockSize - 21] != (byte)blockType)
                    return IoReadState.BlockTypeMismatch;
                if (*(uint*)(data + ArchiveConstants.BlockSize - 20) != indexValue)
                    return IoReadState.IndexNumberMissmatch;
                if (*(uint*)(data + ArchiveConstants.BlockSize - 12) > snapshotSequenceNumber)
                    return IoReadState.PageNewerThanSnapshotSequenceNumber;
                if (*(uint*)(data + ArchiveConstants.BlockSize - 16) != fileIdNumber)
                    return IoReadState.FileIdNumberDidNotMatch;

                return IoReadState.Valid;
            }
            if ((checksumInData == 0) && (checksum == EmptyChecksum))
            {
                return IoReadState.ChecksumInvalidBecausePageIsNull;
            }
            return IoReadState.ChecksumInvalid;
        }

        /// <summary>
        /// Writes the following footer data to the block.
        /// </summary>
        /// <param name="data">the block data to write to</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        /// <returns></returns>
        static void WriteFooterData(byte* data, BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber)
        {
            data[ArchiveConstants.BlockSize - 21] = (byte)blockType;
            *(uint*)(data + ArchiveConstants.BlockSize - 20) = indexValue;
            *(uint*)(data + ArchiveConstants.BlockSize - 16) = fileIdNumber;
            *(uint*)(data + ArchiveConstants.BlockSize - 12) = snapshotSequenceNumber;

            long checksum = ComputeChecksum(data);
            *(long*)(data + ArchiveConstants.BlockSize - 8) = checksum;
        }


        #region [ OldMethods ]

        /// <summary>
        /// Writes a specific block of data to the disk system.
        /// </summary>
        /// <param name="blockIndex">the index of the block to write to.</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number of this write</param>
        /// <param name="data">the data to write. It must be equal to <see cref="ArchiveConstants.BlockSize"/>.</param>
        public void WriteBlock(uint blockIndex, BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber, byte[] data)
        {
            if (IsReadOnly)
                throw new Exception("File system is read only");
            if (data.Length != ArchiveConstants.BlockSize)
                throw new Exception("All page IOs must be performed one page at a time.");

            //If the file is not large enought to set this block, autogrow the file.
            if ((long)(blockIndex + 1) * ArchiveConstants.BlockSize > FileSize)
            {
                SetFileLength(0, blockIndex + 1);
            }

            WriteFooterData(data, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);

            WriteBlock(blockIndex, data);
        }

        /// <summary>
        /// Reads a specific block of data to the disk system.
        /// </summary>
        /// <param name="blockIndex">the index of the block to read from.</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        /// <param name="data">the place to store the data. It must be equal to <see cref="ArchiveConstants.BlockSize"/>.</param>
        /// <returns>
        /// The result of the read operation. See <see cref="IoReadState"/> for details.
        /// </returns>
        public IoReadState ReadBlock(uint blockIndex, BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber, byte[] data)
        {
            if (data.Length != ArchiveConstants.BlockSize)
                throw new Exception("All page IOs must be performed one page at a time.");
            if ((long)(blockIndex + 1) * ArchiveConstants.BlockSize > FileSize)
                return IoReadState.ReadPastThenEndOfTheFile;

            IoReadState readState = ReadBlock(blockIndex, data);
            if (readState != IoReadState.Valid)
                return readState;

            return IsFooterValid(data, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
        }

        /// <summary>
        /// This will resize the file to the provided size in bytes;
        /// If resizing smaller than the allocated space, this number is 
        /// increased to the allocated space.  
        /// If file size is not a multiple of the page size, it is rounded up to
        /// the nearest page boundry
        /// </summary>
        /// <param name="size">The number of bytes to make the file.</param>
        /// <param name="nextUnallocatedBlock">the next free block.  
        /// This value is used to ensure that the archive file is not 
        /// reduced beyond this limit causing data coruption</param>
        /// <returns>The size that the file is after this call</returns>
        /// <remarks>Passing 0 to this function will effectively trim out 
        /// all of the free space in this file.</remarks>
        public long SetFileLength(long size, uint nextUnallocatedBlock)
        {
            if (nextUnallocatedBlock * ArchiveConstants.BlockSize > size)
            {
                //if shrinking beyond the allocated space, 
                //adjust the size exactly to the allocated space.
                size = nextUnallocatedBlock * ArchiveConstants.BlockSize;
            }
            else
            {
                long remainder = (size % ArchiveConstants.BlockSize);
                //if there will be a fragmented page remaining
                if (remainder != 0)
                {
                    //if the requested size is not a multiple of the page size
                    //round up to the nearest page
                    size = size + ArchiveConstants.BlockSize - remainder;
                }
            }
            return SetFileLength(size);
        }

        /// <summary>
        /// Computes the custom checksum of the data.
        /// </summary>
        /// <param name="data">the data to compute the checksum for.</param>
        /// <returns></returns>
        static long ComputeChecksum(byte[] data)
        {
            ChecksumCount += 1;
            // return 0;
            if (data.Length != 4096)
            {
                throw new Exception("This checksum is only valid for a length of 4096 bytes");
            }

            long a = 1; //Maximum size for A is 20 bits in length
            long b = 0; //Maximum size for B is 31 bits in length
            long c = 0; //Maximum size for C is 42 bits in length
            for (int x = 0; x < ArchiveConstants.BlockSize - 8; x++)
            {
                a += data[x];
                b += a;
                c += b;
            }
            //Since only 13 bits of C will remain, xor all 42 bits of C into the first 13 bits.
            c = c ^ (c >> 13) ^ (c >> 26) ^ (c >> 39);
            return (c << 51) ^ (b << 20) ^ a;
        }

        /// <summary>
        /// Determines if the footer data for the following page is valid.
        /// </summary>
        /// <param name="data">the block data to check</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        /// <returns>State information about the state of the footer data</returns>
        static IoReadState IsFooterValid(byte[] data, BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber)
        {
            long checksum = ComputeChecksum(data);
            long checksumInData = BitConverter.ToInt64(data, ArchiveConstants.BlockSize - 8);
            if (checksum == checksumInData)
            {
                if (data[ArchiveConstants.BlockSize - 21] != (byte)blockType)
                    return IoReadState.BlockTypeMismatch;
                if (BitConverter.ToUInt32(data, ArchiveConstants.BlockSize - 20) != indexValue)
                    return IoReadState.IndexNumberMissmatch;
                if (BitConverter.ToUInt32(data, ArchiveConstants.BlockSize - 12) > snapshotSequenceNumber)
                    return IoReadState.PageNewerThanSnapshotSequenceNumber;
                if (BitConverter.ToUInt32(data, ArchiveConstants.BlockSize - 16) != fileIdNumber)
                    return IoReadState.FileIdNumberDidNotMatch;
                return IoReadState.Valid;
            }
            if ((checksumInData == 0) && (checksum == EmptyChecksum))
            {
                return IoReadState.ChecksumInvalidBecausePageIsNull;
            }
            return IoReadState.ChecksumInvalid;
        }

        /// <summary>
        /// Writes the following footer data to the block.
        /// </summary>
        /// <param name="data">the block data to write to</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        /// <returns></returns>
        static void WriteFooterData(byte[] data, BlockType blockType, uint indexValue, uint fileIdNumber, uint snapshotSequenceNumber)
        {
            data[ArchiveConstants.BlockSize - 21] = (byte)blockType;
            data[ArchiveConstants.BlockSize - 20] = (byte)(indexValue);
            data[ArchiveConstants.BlockSize - 19] = (byte)(indexValue >> 8);
            data[ArchiveConstants.BlockSize - 18] = (byte)(indexValue >> 16);
            data[ArchiveConstants.BlockSize - 17] = (byte)(indexValue >> 24);
            data[ArchiveConstants.BlockSize - 16] = (byte)(fileIdNumber);
            data[ArchiveConstants.BlockSize - 15] = (byte)(fileIdNumber >> 8);
            data[ArchiveConstants.BlockSize - 14] = (byte)(fileIdNumber >> 16);
            data[ArchiveConstants.BlockSize - 13] = (byte)(fileIdNumber >> 24);
            data[ArchiveConstants.BlockSize - 12] = (byte)(snapshotSequenceNumber);
            data[ArchiveConstants.BlockSize - 11] = (byte)(snapshotSequenceNumber >> 8);
            data[ArchiveConstants.BlockSize - 10] = (byte)(snapshotSequenceNumber >> 16);
            data[ArchiveConstants.BlockSize - 9] = (byte)(snapshotSequenceNumber >> 24);

            long checksum = ComputeChecksum(data);
            data[ArchiveConstants.BlockSize - 8] = (byte)(checksum);
            data[ArchiveConstants.BlockSize - 7] = (byte)(checksum >> 8);
            data[ArchiveConstants.BlockSize - 6] = (byte)(checksum >> 16);
            data[ArchiveConstants.BlockSize - 5] = (byte)(checksum >> 24);
            data[ArchiveConstants.BlockSize - 4] = (byte)(checksum >> 32);
            data[ArchiveConstants.BlockSize - 3] = (byte)(checksum >> 40);
            data[ArchiveConstants.BlockSize - 2] = (byte)(checksum >> 48);
            data[ArchiveConstants.BlockSize - 1] = (byte)(checksum >> 56);
        }

        public void Dispose()
        {
            m_stream.Dispose();
        }

        #endregion

    }
}
