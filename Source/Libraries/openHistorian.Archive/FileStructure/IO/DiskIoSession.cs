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
using GSF.IO.Unmanaged;
using openHistorian.FileStructure.IO;

namespace openHistorian.FileStructure
{
    /// <summary>
    /// Provides a data IO session with the disk subsystem to perform basic read and write operations.
    /// </summary>
    unsafe sealed internal class DiskIoSession : IDisposable
    {
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
        /// <param name="blockSize">The unit size of a block</param>
        /// <param name="diskIo">owner of the disk</param>
        /// <param name="stream">the stream that the IoSession can be created from</param>
        public DiskIoSession(int blockSize, DiskIo diskIo, DiskMediumBase stream)
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
            ReadBlock(blockIndex, true);
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

            IoReadState readState = Footer.IsFooterValid((IntPtr)m_pointer, m_blockSize, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
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

            Footer.WriteFooterData(m_pointer, m_blockSize, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
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

            IoReadState readState = Footer.IsFooterValid((IntPtr)m_pointer, m_blockSize, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
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
        internal static bool BreakOnIO = false;

        #endregion

    }
}
