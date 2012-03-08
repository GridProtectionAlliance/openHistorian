//******************************************************************************************************
//  ArchiveFileStream.cs - Gbtc
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
using System.IO;

namespace openHistorian.Core.StorageSystem.File
{
    /// <summary>
    ///Provides a file stream that can be used to open a file and does all of the background work 
    ///required to translate virtual position data into physical ones.
    /// </summary>
    public class ArchiveFileStream : Stream, ISupportsBinaryStream
    {

        #region [ Members ]

        /// <summary>
        /// Determines if the file stream has been disposed.
        /// </summary>
        private bool m_disposed;
        /// <summary>
        /// Determines if the filestream can be written to.
        /// </summary>
        bool m_isReadOnly;
        /// <summary>
        /// Determines if the current block has been written to and thus is dirty.
        /// </summary>
        bool m_isBlockDirty;
        /// <summary>
        /// This address is used to determine if the block being referenced is an old block or a new one. 
        /// Any addresses greater than or equal to this are new blocks for this transaction. Values before this are old.
        /// </summary>
        uint m_newBlocksStartAtThisAddress;
        /// <summary>
        /// The FileAllocationTable
        /// </summary>
        FileAllocationTable m_fileAllocationTable;
        /// <summary>
        /// The Disk Subsystem.
        /// </summary>
        DiskIoBase m_dataReader;
        /// <summary>
        /// The file used by the stream.
        /// </summary>
        FileMetaData m_file;
        /// <summary>
        /// The translation information for the most recent block looked up.
        /// </summary>
        PositionData m_positionBlock = default(PositionData);
        /// <summary>
        /// The position in the file that is being referenced.
        /// </summary>
        long m_position;
        /// <summary>
        /// Used to convert physical addresses into virtual addresses.
        /// </summary>
        FileAddressTranslation m_addressTranslation;

        #endregion

        #region [ Constructors ]
        /// <summary>
        /// Creates an ArchiveFileStream
        /// </summary>
        /// <param name="dataReader">The location to read from.</param>
        /// <param name="file">The file to read.</param>
        /// <param name="fileAllocationTable">The FileAllocationTable</param>
        /// <param name="openReadOnly">Determines if the file stream allows writing.</param>
        internal ArchiveFileStream(DiskIoBase dataReader, FileMetaData file, FileAllocationTable fileAllocationTable, bool openReadOnly)
        {
            m_isReadOnly = openReadOnly;
            m_isBlockDirty = false;
            m_newBlocksStartAtThisAddress = fileAllocationTable.NextUnallocatedBlock;
            m_fileAllocationTable = fileAllocationTable;
            m_dataReader = dataReader;
            m_file = file;
            m_addressTranslation = new FileAddressTranslation(file, dataReader, m_fileAllocationTable, openReadOnly);
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// The buffer that contains the most recent copy of a data block.
        /// </summary>
        IndexBufferPool.Buffer Buffer
        {
            get
            {
                return m_addressTranslation.DataBuffer;
            }
        }
        /// <summary>
        /// Returns the file that was used to make this stream.
        /// </summary>
        public FileMetaData File
        {
            get
            {
                return m_file;
            }
        }

        /// <summary>
        /// Determines if the file system has been disposed yet.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Determines if the file can be read.  Always True.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Determines if the file can be seeked. Always True.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Determines if the file is Read Only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
        }

        /// <summary>
        /// Determines if the file can be written to.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return m_isReadOnly;
            }
        }

        /// <summary>
        /// Returns the last addressable location.  This is not the amount of space requred by this file since blocks can remain unassigned.
        /// </summary>
        public override long Length
        {
            get { return (long)m_file.LastAllocatedCluster * (long)ArchiveConstants.BlockSize; }
        }

        /// <summary>
        /// Get/Set the position being referenced.
        /// </summary>
        public override long Position
        {
            get
            {
                return m_position;
            }
            set
            {
                //ToDo: Test the upper bound.
                if (value < 0)
                    throw new Exception("The position must be positive");
                m_position = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Flushes any dirty blocks to the DiskIO system.
        /// </summary>
        public override void Flush()
        {
            if (m_isBlockDirty)
            {
                if (IsReadOnly)
                    throw new Exception();
                if (m_positionBlock.PhysicalBlockIndex < m_newBlocksStartAtThisAddress)
                    throw new Exception("Programming Error: A write attempt has been made to the committed data blocks.");

                uint indexValue = (uint)(m_positionBlock.VirtualPosition / ArchiveConstants.DataBlockDataLength);
                uint fileIdNumber = m_file.FileIdNumber;
                uint snapshotSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;
                m_dataReader.WriteBlock(m_positionBlock.PhysicalBlockIndex, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber, Buffer.Block);
                Buffer.Address = m_positionBlock.PhysicalBlockIndex;
                m_isBlockDirty = false;
            }
        }
        /// <summary>
        /// requests the virtual/physical translation data for the current position.
        /// </summary>
        /// <remarks>This only looks up the block if the old one does not contain the address.</remarks>
        private void LookupPosition()
        {
            if (!m_positionBlock.Containts(m_position))
            {
                Flush();
                m_positionBlock = m_addressTranslation.VirtualToPhysical(m_position);
                if (m_positionBlock.PhysicalBlockIndex != 0)
                {
                    uint indexValue = (uint)(m_positionBlock.VirtualPosition / ArchiveConstants.DataBlockDataLength);
                    uint fileIdNumber = m_file.FileIdNumber;
                    uint snapshotSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;

                    if (Buffer.Address != m_positionBlock.PhysicalBlockIndex)
                    {
                        IoReadState readState = m_dataReader.ReadBlock(m_positionBlock.PhysicalBlockIndex, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber, Buffer.Block);
                        Buffer.Address = m_positionBlock.PhysicalBlockIndex;
                        if (readState != IoReadState.Valid)
                            throw new Exception("Error Reading File " + readState.ToString());
                    }
                }
                else
                {
                    throw new Exception("Block is null");
                    //Array.Clear(m_tempBlockBuffer, 0, m_tempBlockBuffer.Length);
                }
            }
        }
        /// <summary>
        /// Looks up the position data and prepares the current block to be written to.
        /// </summary>
        private void PrepareBlockForWrite()
        {
            if (!m_positionBlock.Containts(m_position) || m_positionBlock.PhysicalBlockIndex < m_newBlocksStartAtThisAddress)
            {
                Flush();
                m_positionBlock = m_addressTranslation.VirtualToShadowPagePhysical(m_position);
                if (m_positionBlock.PhysicalBlockIndex != 0)
                {
                    uint indexValue = (uint)(m_positionBlock.VirtualPosition / ArchiveConstants.DataBlockDataLength);
                    uint featureSequenceNumber = m_file.FileIdNumber;
                    uint revisionSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;
                    if (Buffer.Address != m_positionBlock.PhysicalBlockIndex)
                    {
                        IoReadState readState = m_dataReader.ReadBlock(m_positionBlock.PhysicalBlockIndex, BlockType.DataBlock, indexValue, featureSequenceNumber, revisionSequenceNumber, Buffer.Block);
                        Buffer.Address = m_positionBlock.PhysicalBlockIndex;
                        if (readState != IoReadState.Valid)
                            throw new Exception("Error Reading File " + readState.ToString());
                    }
                }
                else
                {
                    throw new Exception("Failure to shadow copy the page.");
                    //Array.Clear(m_tempPageBuffer, 0, m_tempPageBuffer.Length);
                }
            }
        }
        /// <summary>
        /// Reads one byte from the file.
        /// </summary>
        /// <returns></returns>
        public override int ReadByte()
        {
            //Todo: If reached the end of the stream, do some kind of end of stream exception.

            LookupPosition();
            long availableLength = m_positionBlock.ValidLength(m_position);

            // determine if there is enough bytes in the block to read the entire block
            int value = Buffer.Block[m_positionBlock.Offset(m_position)];
            m_position += 1;
            return value;
        }

        /// <summary>
        /// Reads bytes from the file.
        /// </summary>
        /// <param name="buffer">The place to put the data.</param>
        /// <param name="offset">The starting position.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            //Todo: If reached the end of the stream, do some kind of end of stream exception.
            LookupPosition();
            long availableLength = m_positionBlock.ValidLength(m_position);

            // determine if there is enough bytes in the block to read the entire block
            if (availableLength >= count)
            {
                Array.Copy(Buffer.Block, m_positionBlock.Offset(m_position), buffer, offset, count);
                m_position += count;
            }
            else
            {
                Array.Copy(Buffer.Block, m_positionBlock.Offset(m_position), buffer, offset, availableLength);
                m_position += availableLength;
                Read(buffer, offset + (int)availableLength, count - (int)availableLength);
            }
            return count;
        }
        /// <summary>
        /// Seeks the file.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    throw new Exception();
            }
            return m_position;
        }

        /// <summary>
        /// Setting the length of a file is not supported.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the following byte to the archive file.
        /// </summary>
        /// <param name="value"></param>
        public override void WriteByte(byte value)
        {
            if (IsReadOnly)
                throw new Exception("File is read only");
            PrepareBlockForWrite();
            m_isBlockDirty = true;

            Buffer.Block[m_positionBlock.Offset(m_position)] = value;
            m_position++;
        }

        int ISupportsBinaryStream.Read(long position, byte[] data, int start, int count)
        {
            Position = position;
            return Read(data, start, count);
        }

        void ISupportsBinaryStream.Write(long position, byte[] data, int start, int count)
        {
            Position = position;
            Write(data, start, count);
        }

        void ISupportsBinaryStream.GetCurrentBlock(long position, bool isWriting, out byte[] buffer, out int firstIndex, out int lastIndex, out int curentPosition)
        {
            if (isWriting && IsReadOnly)
                throw new Exception("File is read only");
            Position = position;
            PrepareBlockForWrite();
            if (isWriting)
                m_isBlockDirty = true;
            firstIndex = 0;
            lastIndex = (int)m_positionBlock.Length - 1;
            curentPosition = m_positionBlock.Offset(m_position);
            buffer = Buffer.Block;
        }

        /// <summary>
        /// Writes the following data to the stream.
        /// </summary>
        /// <param name="buffer">the data to write</param>
        /// <param name="offset">starting position</param>
        /// <param name="count">length of the write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (IsReadOnly)
                throw new Exception("File is read only");
            PrepareBlockForWrite();
            m_isBlockDirty = true;
            long availableLength = m_positionBlock.ValidLength(m_position);

            // determine if there is enough bytes in the block to read the entire block
            if (availableLength >= count)
            {
                Array.Copy(buffer, offset, Buffer.Block, m_positionBlock.Offset(m_position), count);
                m_position += count;
            }
            else
            {
                Array.Copy(buffer, offset, Buffer.Block, m_positionBlock.Offset(m_position), availableLength);
                m_position += availableLength;
                Write(buffer, offset + (int)availableLength, count - (int)availableLength);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ArchiveFileStream"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    Flush();
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        #endregion







    }
}
