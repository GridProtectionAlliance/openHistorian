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
using System.Runtime.InteropServices;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    ///Provides a file stream that can be used to open a file and does all of the background work 
    ///required to translate virtual position data into physical ones.
    /// </summary>
    unsafe public class ArchiveFileStream : Stream, ISupportsBinaryStream
    {


        // Nested Types
        class IoSession : IBinaryStreamIoSession
        {
            bool m_disposed;
            ArchiveFileStream m_stream;

            public IoSession(ArchiveFileStream stream)
            {
                m_stream = stream;
            }

            /// <summary>
            /// Releases the unmanaged resources before the <see cref="IoSession"/> object is reclaimed by <see cref="GC"/>.
            /// </summary>
            ~IoSession()
            {
                Dispose(false);
            }

            /// <summary>
            /// Releases all the resources used by the <see cref="IoSession"/> object.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="IoSession"/> object and optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            protected virtual void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        // This will be done regardless of whether the object is finalized or disposed.
                        m_stream.m_ioStream = null;
                        if (disposing)
                        {
                            // This will be done only when the object is disposed by calling Dispose().
                        }
                    }
                    finally
                    {
                        m_disposed = true;  // Prevent duplicate dispose.
                    }
                }
            }

            public void GetBlock(long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
            {
                m_stream.GetBlock(position, isWriting, out firstPointer, out firstPosition, out length, out supportsWriting);
            }


            public void Clear()
            {
                
            }
        }

        #region [ Members ]

        IoSession m_ioStream;

        public event EventHandler StreamDisposed;

        /// <summary>
        /// Determines if the file stream has been disposed.
        /// </summary>
        bool m_disposed;
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
        DiskIoEnhanced m_dataReader;
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
        /// <summary>
        /// Contains the read/write buffer.
        /// </summary>
        IMemoryUnit m_buffer;

        #endregion

        #region [ Constructors ]
        /// <summary>
        /// Creates an ArchiveFileStream
        /// </summary>
        /// <param name="dataReader">The location to read from.</param>
        /// <param name="file">The file to read.</param>
        /// <param name="fileAllocationTable">The FileAllocationTable</param>
        /// <param name="openReadOnly">Determines if the file stream allows writing.</param>
        internal ArchiveFileStream(DiskIoEnhanced dataReader, FileMetaData file, FileAllocationTable fileAllocationTable, bool openReadOnly)
        {
            m_isReadOnly = openReadOnly;
            m_isBlockDirty = false;
            m_newBlocksStartAtThisAddress = fileAllocationTable.NextUnallocatedBlock;
            m_fileAllocationTable = fileAllocationTable;
            m_dataReader = dataReader;
            m_file = file;
            m_addressTranslation = new FileAddressTranslation(file, dataReader, m_fileAllocationTable, openReadOnly);
            m_buffer = dataReader.GetMemoryUnit();
        }
        #endregion

        #region [ Properties ]

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
                m_dataReader.WriteBlock(BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber, m_buffer);
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

                    if (!m_buffer.IsValid || m_buffer.BlockIndex != m_positionBlock.PhysicalBlockIndex)
                    {
                        IoReadState readState = m_dataReader.AquireBlockForRead(m_positionBlock.PhysicalBlockIndex, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber, m_buffer);
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
            if (!m_positionBlock.Containts(m_position) || m_positionBlock.PhysicalBlockIndex < m_newBlocksStartAtThisAddress || !m_buffer.IsValid || m_buffer.IsReadOnly)
            {
                Flush();
                m_positionBlock = m_addressTranslation.VirtualToShadowPagePhysical(m_position);
                if (m_positionBlock.PhysicalBlockIndex != 0)
                {
                    uint indexValue = (uint)(m_positionBlock.VirtualPosition / ArchiveConstants.DataBlockDataLength);
                    uint featureSequenceNumber = m_file.FileIdNumber;
                    uint revisionSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;
                    if (!m_buffer.IsValid || m_buffer.IsReadOnly || m_buffer.BlockIndex != m_positionBlock.PhysicalBlockIndex)
                    {
                        m_dataReader.AquireBlockForWrite(m_positionBlock.PhysicalBlockIndex, m_buffer);
                        //IoReadState readState = m_dataReader.AquireBlockForWrite(m_positionBlock.PhysicalBlockIndex, BlockType.DataBlock, indexValue, featureSequenceNumber, revisionSequenceNumber, m_buffer);
                        //if (readState != IoReadState.Valid)
                        //    throw new Exception("Error Reading File " + readState.ToString());
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
            int value = m_buffer.Pointer[m_positionBlock.Offset(m_position)];
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
                Marshal.Copy((IntPtr)m_buffer.Pointer + m_positionBlock.Offset(m_position), buffer, offset, count);
                //Array.Copy(m_buffer.Block, m_positionBlock.Offset(m_position), buffer, offset, count);
                m_position += count;
            }
            else
            {
                Marshal.Copy((IntPtr)m_buffer.Pointer + m_positionBlock.Offset(m_position), buffer, offset, (int)availableLength);
                //Array.Copy(m_buffer.Block, m_positionBlock.Offset(m_position), buffer, offset, availableLength);
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

            m_buffer.Pointer[m_positionBlock.Offset(m_position)] = value;
            m_position++;
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
                Marshal.Copy(buffer, offset, (IntPtr)m_buffer.Pointer + m_positionBlock.Offset(m_position), count);
                //Array.Copy(buffer, offset, m_buffer.Block, m_positionBlock.Offset(m_position), count);
                m_position += count;
            }
            else
            {
                Marshal.Copy(buffer, offset, (IntPtr)m_buffer.Pointer + m_positionBlock.Offset(m_position), (int)availableLength);
                //Array.Copy(buffer, offset, m_buffer.Block, m_positionBlock.Offset(m_position), availableLength);
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
                if (StreamDisposed != null)
                    StreamDisposed.Invoke(this, EventArgs.Empty);
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

        public int RemainingSupportedIoSessions
        {
            get
            {
                if (m_ioStream == null)
                    return 1;
                return 0;
            }
        }

        void GetBlock(long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
        {
            if (isWriting && IsReadOnly)
                throw new Exception("File is read only");
            Position = position;
            PrepareBlockForWrite();
            if (isWriting)
                m_isBlockDirty = true;
            firstPosition = m_positionBlock.VirtualPosition;
            length = (int)m_positionBlock.Length;
            firstPointer = (IntPtr)m_buffer.Pointer;
            supportsWriting = m_isBlockDirty;
        }

        IBinaryStreamIoSession ISupportsBinaryStream.GetNextIoSession()
        {
            if (RemainingSupportedIoSessions == 0)
                throw new Exception("There are not any remaining IO Sessions");
            m_ioStream = new IoSession(this);
            return m_ioStream;
        }
    }
}
