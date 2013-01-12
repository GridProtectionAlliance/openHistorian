//******************************************************************************************************
//  DiskIoSession.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  6/15/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using openHistorian.IO.Unmanaged;

namespace openHistorian.FileStructure
{
    /// <summary>
    /// Provides a data IO session with the disk subsystem to perform basic read and write operations.
    /// </summary>
    unsafe sealed internal class DiskIoSession : IDisposable
    {
        /// <summary>
        /// Since exceptions are very expensive, this enum will be returned for basic
        /// I/O operations to let the reader know what to do with the data.  
        /// </summary>
        /// <remarks>There two overarching conditions.  Valid or not Valid.  
        /// If not valid, the reason why the page failed will be given.
        /// If a page is returned as valid, this does not mean that the 
        /// page being referenced is the correct page, it is up to the class
        /// to check the footer of the page to verify that the page being read
        /// is the correct page.</remarks>
        enum IoReadState
        {
            /// <summary>
            /// Indicates that the read completed sucessfully.
            /// </summary>
            Valid,
            /// <summary>
            /// The checksum failed to compute
            /// </summary>
            ChecksumInvalid,
            /// <summary>
            /// The page that was requested came from a newer version of the file.
            /// </summary>
            PageNewerThanSnapshotSequenceNumber,
            /// <summary>
            /// The page came from a different file.
            /// </summary>
            FileIdNumberDidNotMatch,
            /// <summary>
            /// The index value did not match that of the file.
            /// </summary>
            IndexNumberMissmatch,
            /// <summary>
            /// The page type requested did not match what was received
            /// </summary>
            BlockTypeMismatch
        }

        #region [ Members ]

        DiskIo m_diskIo;
        IBinaryStreamIoSession m_ioSession;
        bool m_disposed;
        bool m_isValid;
        bool m_pendingWriteComplete;
        bool m_blockSupportsWriting;
        int m_length;
        int m_blockIndex;
        int m_blockSize;
        byte* m_pointer;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new DiskIoSession that can be used to read from the disk subsystem.
        /// </summary>
        /// <param name="diskIo">owner of the disk</param>
        /// <param name="stream">the stream that the IoSession can be created from</param>
        public DiskIoSession(int blockSize, DiskIo diskIo, ISupportsBinaryStream stream)
        {
            if (diskIo == null)
                throw new ArgumentNullException("diskIo");
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (diskIo.IsDisposed)
                throw new ObjectDisposedException(diskIo.GetType().FullName);
            if (stream.IsDisposed)
                throw new ObjectDisposedException(stream.GetType().FullName);

            m_blockSize = blockSize;
            m_diskIo = diskIo;
            m_ioSession = stream.GetNextIoSession();
            m_isValid = false;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Returns true if this class is disposed
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Gets if the block in this IO Session is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return m_isValid && !IsDisposed && !m_ioSession.IsDisposed;
            }
        }

        /// <summary>
        /// Determines if the current block can be written to.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                CheckIsValid();
                return !m_pendingWriteComplete;
            }
        }

        /// <summary>
        /// Gets if the block has began a write cycle that is pending completion.
        /// </summary>
        public bool IsPendingWriteComplete
        {
            get
            {
                CheckIsValid();
                return m_pendingWriteComplete;
            }
        }

        /// <summary>
        /// Gets the number of bytes valid in this block.
        /// </summary>
        public int Length
        {
            get
            {
                CheckIsValid();
                return m_length;
            }
        }
        /// <summary>
        /// Gets the indexed page of this block.
        /// </summary>
        public int BlockIndex
        {
            get
            {
                CheckIsValid();
                return m_blockIndex;
            }
        }
        /// <summary>
        /// Gets a pointer to the block
        /// </summary>
        public byte* Pointer
        {
            get
            {
                CheckIsValid();
                return m_pointer;
            }
        }

        /// <summary>
        /// Gets a managed pointer to the block
        /// </summary>
        public IntPtr IntPtr
        {
            get
            {
                CheckIsValid();
                return (IntPtr)m_pointer;
            }
        }

        public int BlockSize
        {
            get
            {
                return m_blockSize;
            }
        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Navigates to a block that will be written to. 
        /// This class does not check if overwriting an existing block. So be careful not to corrupt the file.
        /// </summary>
        /// <param name="blockIndex">the zero based index of the block to write to.</param>
        /// <remarks>This function will increase the size of the file if the block excedes the current size of the file.</remarks>
        public void BeginWriteToNewBlock(int blockIndex)
        {
            if (BreakOnIO)
                Debugger.Break();
            WriteCount++;
            ReadCount++;
            CheckIsDisposed();
            if (m_diskIo.IsReadOnly)
                throw new ReadOnlyException("File system is read only");
            if (m_pendingWriteComplete)
                throw new Exception("A pending write operation must first complete before starting another one");
            if ((blockIndex > 10 && blockIndex <= m_diskIo.LastReadonlyBlock))
                throw new ArgumentOutOfRangeException("blockIndex", "Cannot write to committed blocks");

            m_isValid = false;

            //If the file is not large enough to write to this block, autogrow the file.
            if ((long)(blockIndex + 1) * m_blockSize > m_diskIo.FileSize)
            {
                m_diskIo.SetFileLength(0, blockIndex + 1);
            }

            m_isValid = false;
            ReadBlock(blockIndex, true);

            //if (!IsBlockNull(m_pointer))
            //    throw new ArgumentException("Block is not null", "blockIndex");

            m_isValid = true;
            m_pendingWriteComplete = true;
        }

        /// <summary>
        /// Navigates to a block that will be written to.
        /// This block must currently exist and have the correct parameters passed to this function
        /// In order to allow this block to be modified.
        /// </summary>
        /// <param name="blockIndex">the index value of this block</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number of this write</param>
        /// <returns></returns>
        public void BeginWriteToExistingBlock(int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            if (BreakOnIO)
                Debugger.Break();
            WriteCount++;
            ReadCount++;
            CheckIsDisposed();

            if (m_diskIo.IsReadOnly)
                throw new ReadOnlyException("File system is read only");
            if (m_pendingWriteComplete)
                throw new Exception("A pending write operation must first complete before starting another one");

            if ((blockIndex > 10 && blockIndex <= m_diskIo.LastReadonlyBlock))
                throw new ArgumentOutOfRangeException("blockIndex", "Cannot write to committed blocks");

            m_isValid = false;
            ReadBlock(blockIndex, true);

            IoReadState readState = IsFooterValid(m_pointer, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);

            if (readState != IoReadState.Valid)
                throw new Exception("Read Error: " + readState.ToString());

            m_pendingWriteComplete = true;
            m_isValid = true;
        }

        /// <summary>
        /// Completes the write operation by calculating the checksum for the block.
        /// After this call, the block is still valid, but cannot be written to until BeginWrite is executed.
        /// </summary>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number of this write</param>
        public void EndWrite(BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            if (BreakOnIO)
                Debugger.Break();
            CheckIsDisposed();

            if (!m_pendingWriteComplete)
                throw new Exception("A write operation has not started yet. Begin the write operation first.");

            WriteFooterData(m_pointer, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
            m_pendingWriteComplete = false;
        }


        /// <summary>
        /// Navigates to a block that will be only read and not modified.
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number of this write</param>
        /// <returns></returns>
        public void Read(int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            if (BreakOnIO)
                Debugger.Break();
            ReadCount++;
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_diskIo.IsDisposed)
                throw new ObjectDisposedException(typeof(DiskIo).FullName);
            if (m_pendingWriteComplete)
                throw new Exception("A pending write operation must first complete before starting another one");

            m_isValid = false;
            ReadBlock(blockIndex, false);

            IoReadState readState = IsFooterValid(m_pointer, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
            if (readState != IoReadState.Valid)
                throw new Exception("Read Error: " + readState.ToString());

            m_isValid = true;
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="DiskIoSession"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_ioSession != null)
                    {
                        m_ioSession.Dispose();
                        m_ioSession = null;
                    }
                    m_diskIo = null;
                }
                finally
                {
                    m_isValid = false;
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Releases any lock that has been aquired on the data so it can be collected if need be.
        /// </summary>
        public void Clear()
        {
            CheckIsDisposed();
            m_isValid = false;
            m_ioSession.Clear();
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Checks 3 flags and throws the correct exceptions if this class is invalid or disposed.
        /// </summary>
        void CheckIsValid()
        {
            if (!m_isValid)
                throw new InvalidDataException();
            CheckIsDisposed();
        }

        /// <summary>
        /// Checks 2 flags and throws the correct exceptions if this class is disposed.
        /// </summary>
        void CheckIsDisposed()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_diskIo.IsDisposed)
                throw new ObjectDisposedException(typeof(DiskIo).FullName);
            if (m_ioSession.IsDisposed)
                throw new ObjectDisposedException(m_ioSession.GetType().FullName);
        }

        /// <summary>
        /// Tries to read data from the following file
        /// </summary>
        /// <param name="blockIndex">the block where to write the data</param>
        /// <param name="requestWriteAccess">true if reading data from this block for the purpose of writing to it later</param>
        void ReadBlock(int blockIndex, bool requestWriteAccess)
        {
            IntPtr pointerToFirstByte;
            int validLength;
            long positionOfFirstByte;
            bool supportsWriting;
            long position = (long)blockIndex * m_blockSize;

            m_ioSession.GetBlock(position, requestWriteAccess, out pointerToFirstByte, out positionOfFirstByte, out validLength, out supportsWriting);
            int offsetOfPosition = (int)(position - positionOfFirstByte);

            if (validLength - offsetOfPosition < m_blockSize)
                throw new Exception("stream is not lining up on page boundries");

            m_blockIndex = blockIndex;
            m_pointer = (byte*)(pointerToFirstByte + offsetOfPosition);
            m_length = m_blockSize;
            m_blockSupportsWriting = supportsWriting;
        }

        #endregion

        #region [ Static ]


        static internal long ReadCount;
        static internal long WriteCount;
        internal static bool BreakOnIO=false;
        

        ///// <summary>
        ///// Computes the custom checksum of the data.
        ///// </summary>
        ///// <param name="data">the data to compute the checksum for.</param>
        ///// <returns></returns>
        //static long ComputeChecksum(byte* data)
        //{
        //    ChecksumCount += 1;
        //    return 0;

        //    long a = 1; //Maximum size for A is 20 bits in length
        //    long b = 0; //Maximum size for B is 31 bits in length
        //    long c = 0; //Maximum size for C is 42 bits in length
        //    for (int x = 0; x < ArchiveConstants.BlockSize - 8; x++)
        //    {
        //        a += data[x];
        //        b += a;
        //        c += b;
        //    }
        //    //Since only 13 bits of C will remain, xor all 42 bits of C into the first 13 bits.
        //    c = c ^ (c >> 13) ^ (c >> 26) ^ (c >> 39);
        //    return (c << 51) ^ (b << 20) ^ a;
        //}

        /// <summary>
        /// Determines if the footer data for the following page is valid.
        /// </summary>
        /// <param name="data">the block data to check</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <param name="fileIdNumber">the file number this block is associated with</param>
        /// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        /// <returns>State information about the state of the footer data</returns>
        IoReadState IsFooterValid(byte* data, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            if (data[m_blockSize - 4] == 1)
            {
                if (data[m_blockSize - 32] != (byte)blockType)
                    return IoReadState.BlockTypeMismatch;
                if (*(int*)(data + m_blockSize - 28) != indexValue)
                    return IoReadState.IndexNumberMissmatch;
                if ((uint)*(int*)(data + m_blockSize - 20) > snapshotSequenceNumber) //Note: Convert to uint so negative numbers also fall in this category
                    return IoReadState.PageNewerThanSnapshotSequenceNumber;
                if (*(int*)(data + m_blockSize - 24) != fileIdNumber)
                    return IoReadState.FileIdNumberDidNotMatch;
                return IoReadState.Valid;
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
        void WriteFooterData(byte* data, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            if (indexValue < 0 | fileIdNumber < 0 | snapshotSequenceNumber < 0)
                throw new Exception();

            data[m_blockSize - 4] = 1; //Write checksum is good
            data[m_blockSize - 3] = 1; //Write checksum needs to be updated

            data[m_blockSize - 32] = (byte)blockType;
            *(int*)(data + m_blockSize - 28) = indexValue;
            *(int*)(data + m_blockSize - 24) = fileIdNumber;
            *(int*)(data + m_blockSize - 20) = snapshotSequenceNumber;
        }

        #endregion

    }
}
