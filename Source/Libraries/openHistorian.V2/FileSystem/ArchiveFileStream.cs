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
//  6/1/2012 - Steven E. Chisholm
//       Removed the inheritance from System.IO.Stream since it wasn't used.
//       And prevented concurrent access to this class
//
//******************************************************************************************************

using System;
using openHistorian.V2.IO;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    ///Provides a file stream that can be used to open a file and does all of the background work 
    ///required to translate virtual position data into physical ones.
    /// </summary>
    unsafe public partial class ArchiveFileStream : ISupportsBinaryStream
    {
        #region [ Members ]

        IoSession m_ioStream;

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
        int m_newBlocksStartAtThisAddress;
        /// <summary>
        /// The FileAllocationTable
        /// </summary>
        FileAllocationTable m_fileAllocationTable;
        /// <summary>
        /// The Disk Subsystem.
        /// </summary>
        DiskIo m_dataReader;
        /// <summary>
        /// The file used by the stream.
        /// </summary>
        FileMetaData m_file;
        /// <summary>
        /// The translation information for the most recent block looked up.
        /// </summary>
        PositionData m_positionBlock = default(PositionData);
        /// <summary>
        /// Used to convert physical addresses into virtual addresses.
        /// </summary>
        FileAddressTranslation m_addressTranslation;
        /// <summary>
        /// Contains the read/write buffer.
        /// </summary>
        DiskIoSession m_buffer;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an ArchiveFileStream
        /// </summary>
        /// <param name="dataReader">The location to read from.</param>
        /// <param name="file">The file to read.</param>
        /// <param name="fileAllocationTable">The FileAllocationTable</param>
        /// <param name="openReadOnly">Determines if the file stream allows writing.</param>
        internal ArchiveFileStream(DiskIo dataReader, FileMetaData file, FileAllocationTable fileAllocationTable, bool openReadOnly)
        {
            m_isReadOnly = openReadOnly;
            m_isBlockDirty = false;
            m_newBlocksStartAtThisAddress = fileAllocationTable.LastAllocatedBlock+1;
            m_fileAllocationTable = fileAllocationTable;
            m_dataReader = dataReader;
            m_file = file;
            m_addressTranslation = new FileAddressTranslation(file, dataReader, m_fileAllocationTable, openReadOnly);
            m_buffer = dataReader.CreateDiskIoSession();
        }
        ~ArchiveFileStream()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
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

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Flushes any dirty blocks to the DiskIO system.
        /// </summary>
        public void Flush()
        {
            if (m_isBlockDirty)
            {
                if (m_isReadOnly)
                    throw new Exception();
                if (m_positionBlock.PhysicalBlockIndex < m_newBlocksStartAtThisAddress)
                    throw new Exception("Programming Error: A write attempt has been made to the committed data blocks.");

                int indexValue = (int)(m_positionBlock.VirtualPosition / ArchiveConstants.DataBlockDataLength);
                int fileIdNumber = m_file.FileIdNumber;
                int snapshotSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;
                m_buffer.EndWrite(BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber);
                m_isBlockDirty = false;
            }
        }

        /// <summary>
        /// Looks up the position data and prepares the current block to be written to.
        /// </summary>
        private void PrepareBlockForWrite(long position)
        {
            if (!m_positionBlock.Containts(position) || m_positionBlock.PhysicalBlockIndex < m_newBlocksStartAtThisAddress || !m_buffer.IsValid || m_buffer.IsReadOnly)
            {
                Flush();
                m_positionBlock = m_addressTranslation.VirtualToShadowPagePhysical(position);
                if (m_positionBlock.PhysicalBlockIndex != 0)
                {
                    int indexValue = (int)(m_positionBlock.VirtualPosition / ArchiveConstants.DataBlockDataLength);
                    int featureSequenceNumber = m_file.FileIdNumber;
                    int revisionSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;
                    if (!m_buffer.IsValid || m_buffer.IsReadOnly || m_buffer.BlockIndex != m_positionBlock.PhysicalBlockIndex)
                    {
                        m_buffer.BeginWriteToExistingBlock(m_positionBlock.PhysicalBlockIndex, BlockType.DataBlock, indexValue, featureSequenceNumber, revisionSequenceNumber);
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
        /// Looks up the position data and prepares the current block to be written to.
        /// </summary>
        private void PrepareBlockForRead(long position)
        {
            if (!m_positionBlock.Containts(position) || !m_buffer.IsValid)
            {
                Flush();
                m_positionBlock = m_addressTranslation.VirtualToPhysical(position);
                if (m_positionBlock.PhysicalBlockIndex != 0)
                {
                    int indexValue = (int)(m_positionBlock.VirtualPosition / ArchiveConstants.DataBlockDataLength);
                    int featureSequenceNumber = m_file.FileIdNumber;
                    int revisionSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;
                    if (!m_buffer.IsValid || m_buffer.BlockIndex != m_positionBlock.PhysicalBlockIndex)
                    {
                        m_buffer.Read(m_positionBlock.PhysicalBlockIndex, BlockType.DataBlock, indexValue, featureSequenceNumber, revisionSequenceNumber);
                    }
                }
                else
                {
                    throw new Exception("Failure to shadow copy the page.");
                    //Array.Clear(m_tempPageBuffer, 0, m_tempPageBuffer.Length);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ArchiveFileStream"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
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
            if (isWriting)
            {
                if (m_isReadOnly)
                    throw new Exception("File is read only");

                PrepareBlockForWrite(position);

                m_isBlockDirty = true;

                firstPosition = m_positionBlock.VirtualPosition;
                length = (int)m_positionBlock.Length;
                firstPointer = (IntPtr)m_buffer.Pointer;
                supportsWriting = m_isBlockDirty;
            }
            else
            {
                PrepareBlockForRead(position);
                firstPosition = m_positionBlock.VirtualPosition;
                length = (int)m_positionBlock.Length;
                firstPointer = (IntPtr)m_buffer.Pointer;
                supportsWriting = m_isBlockDirty;
            }
        }

        IBinaryStreamIoSession ISupportsBinaryStream.GetNextIoSession()
        {
            if (RemainingSupportedIoSessions == 0)
                throw new Exception("There are not any remaining IO Sessions");
            m_ioStream = new IoSession(this);
            return m_ioStream;
        }

        public IBinaryStream CreateBinaryStream()
        {
            return new BinaryStream(this);
        }
    }
}
