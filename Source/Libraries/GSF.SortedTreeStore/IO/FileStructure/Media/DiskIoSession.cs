//******************************************************************************************************
//  DiskIoSession.cs - Gbtc
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
//  6/15/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Data;
using GSF.Diagnostics;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure.Media
{
    /// <summary>
    /// Provides a data IO session with the disk subsystem to perform basic read and write operations.
    /// </summary>
    internal unsafe class DiskIoSession : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(DiskIoSession), MessageClass.Component);

        #region [ Members ]

        private DiskIo m_diskIo;
        private readonly BlockArguments m_args;

        private readonly bool m_isReadOnly;
        private readonly int m_blockSize;
        private readonly ushort m_fileIdNumber;
        private readonly uint m_snapshotSequenceNumber;
        private readonly uint m_lastReadonlyBlock;

        private BinaryStreamIoSessionBase m_diskMediumIoSession;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new DiskIoSession that can be used to read from the disk subsystem.
        /// </summary>
        /// <param name="diskIo">owner of the disk</param>
        /// <param name="ioSession">the base ioSession to use for this io session</param>
        /// <param name="file">The file that will be read from this diskIoSession</param>
        public DiskIoSession(DiskIo diskIo, BinaryStreamIoSessionBase ioSession, FileHeaderBlock header, SubFileHeader file)
        {
            if (diskIo is null)
                throw new ArgumentNullException("diskIo");
            if (diskIo.IsDisposed)
                throw new ObjectDisposedException(diskIo.GetType().FullName);
            if (ioSession is null)
                throw new ArgumentNullException("ioSession");
            if (file is null)
                throw new ArgumentNullException("file");

            m_args = new BlockArguments();
            m_lastReadonlyBlock = diskIo.LastReadonlyBlock;
            m_diskMediumIoSession = ioSession;
            m_snapshotSequenceNumber = header.SnapshotSequenceNumber;
            m_fileIdNumber = file.FileIdNumber;
            m_isReadOnly = file.IsReadOnly || diskIo.IsReadOnly;
            m_blockSize = diskIo.BlockSize;
            m_diskIo = diskIo;
            IsValid = false;
            IsDisposed = false;
        }

#if DEBUG
        ~DiskIoSession()
        {
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
        }
#endif

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Returns true if this class is disposed
        /// </summary>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets if the block in this IO Session is valid.
        /// </summary>
        public bool IsValid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of bytes valid in this block.
        /// </summary>
        public int Length
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the indexed page of this block.
        /// </summary>
        public uint BlockIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a pointer to the block
        /// </summary>
        public byte* Pointer
        {
            get;
            private set;
        }

        public byte BlockType
        {
            get;
            private set;
        }

        public uint IndexValue
        {
            get;
            private set;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Navigates to a block that will be written to. 
        /// This class does not check if overwriting an existing block. So be careful not to corrupt the file.
        /// </summary>
        /// <param name="blockIndex">the index value of this block</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <remarks>This function will increase the size of the file if the block excedes the current size of the file.</remarks>
        public void WriteToNewBlock(uint blockIndex, BlockType blockType, uint indexValue)
        {
            BlockIndex = blockIndex;
            BlockType = (byte)blockType;
            IndexValue = indexValue;

            WriteCount++;
            ReadCount++;
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_diskIo.IsDisposed)
                throw new ObjectDisposedException(typeof(DiskIo).FullName);
            if (m_isReadOnly)
                throw new ReadOnlyException("The subfile used for this io session is read only.");
            if (blockIndex > 10 && blockIndex <= m_lastReadonlyBlock)
                throw new ArgumentOutOfRangeException("blockIndex", "Cannot write to committed blocks");

            IsValid = true;

            BlockIndex = blockIndex;
            ReadBlock(true);
            ClearFooterData();
            WriteFooterData();
        }

        /// <summary>
        /// Navigates to a block that will be written to.
        /// This block must currently exist and have the correct parameters passed to this function
        /// In order to allow this block to be modified.
        /// </summary>
        /// <param name="blockIndex">the index value of this block</param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <returns></returns>
        public void WriteToExistingBlock(uint blockIndex, BlockType blockType, uint indexValue)
        {
            BlockIndex = blockIndex;
            BlockType = (byte)blockType;
            IndexValue = indexValue;

            WriteCount++;
            ReadCount++;
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_diskIo.IsDisposed)
                throw new ObjectDisposedException(typeof(DiskIo).FullName);
            if (m_isReadOnly)
                throw new ReadOnlyException("The subfile used for this io session is read only.");
            if (blockIndex > 10 && blockIndex <= m_lastReadonlyBlock)
                throw new ArgumentOutOfRangeException("blockIndex", "Cannot write to committed blocks");

            IsValid = true;

            ReadBlock(true);

            IoReadState readState = IsFooterCurrentSnapshotAndValid();
            if (readState != IoReadState.Valid)
            {
                IsValid = false;
                throw new Exception("Read Error: " + readState.ToString());
            }
        }

        /// <summary>
        /// Navigates to a block that will be only read and not modified.
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <returns></returns>
        public void Read(uint blockIndex, BlockType blockType, uint indexValue)
        {
            BlockIndex = blockIndex;
            BlockType = (byte)blockType;
            IndexValue = indexValue;

            ReadCount++;
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_diskIo.IsDisposed)
                throw new ObjectDisposedException(typeof(DiskIo).FullName);

            IsValid = true;

            ReadBlock(false);

            IoReadState readState = IsFooterValid();
            if (readState != IoReadState.Valid)
            {
                IsValid = false;
                throw new Exception("Read Error: " + readState.ToString());
            }
        }

        /// <summary>
        /// Navigates to a block that will be only read and not modified.
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="blockType">the type of this block.</param>
        /// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        /// <returns></returns>
        public void ReadOld(uint blockIndex, BlockType blockType, uint indexValue)
        {
            BlockIndex = blockIndex;
            BlockType = (byte)blockType;
            IndexValue = indexValue;

            ReadCount++;
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_diskIo.IsDisposed)
                throw new ObjectDisposedException(typeof(DiskIo).FullName);

            IsValid = true;

            ReadBlock(false);

            IoReadState readState = IsFooterValidFromOldBlock();
            if (readState != IoReadState.Valid)
            {
                IsValid = false;
                throw new Exception("Read Error: " + readState.ToString());
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="DiskIoSession"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                try
                {
                    if (m_diskMediumIoSession != null)
                    {
                        m_diskMediumIoSession.Dispose();
                        m_diskMediumIoSession = null;
                    }
                    m_diskIo = null;
                }
                finally
                {
                    GC.SuppressFinalize(this);
                    IsValid = false;
                    IsDisposed = true; // Prevent duplicate dispose.
                }
            }
        }

        public void Clear()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_diskIo.IsDisposed)
                throw new ObjectDisposedException(typeof(DiskIo).FullName);
            m_args.Length = 0;
            IsValid = false;
            m_diskMediumIoSession.Clear();
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Tries to read data from the following file
        /// </summary>
        /// <param name="requestWriteAccess">true if reading data from this block for the purpose of writing to it later</param>
        private void ReadBlock(bool requestWriteAccess)
        {
            long position = BlockIndex * m_blockSize;

            if (position >= m_args.FirstPosition && position < m_args.FirstPosition + m_args.Length && (m_args.SupportsWriting || !requestWriteAccess))
            {
                Pointer = (byte*)m_args.FirstPointer;
                CachedLookups++;
            }
            else
            {
                m_args.Position = position;
                m_args.IsWriting = requestWriteAccess;
                m_diskMediumIoSession.GetBlock(m_args);
                Pointer = (byte*)m_args.FirstPointer;
                Lookups++;
            }

            int offsetOfPosition = (int)(position - m_args.FirstPosition);
            if (m_args.Length - offsetOfPosition < m_blockSize)
                throw new Exception("stream is not lining up on page boundries");

            Pointer += offsetOfPosition;
            Length = m_blockSize - FileStructureConstants.BlockFooterLength;
        }

        private IoReadState IsFooterValidFromOldBlock()
        {
            byte* lpdata = Pointer + m_blockSize - 32;
            int checksumState = lpdata[28];
            if (checksumState == Footer.ChecksumIsNotValid)
                return IoReadState.ChecksumInvalid;
            if (checksumState == Footer.ChecksumIsValid || checksumState == Footer.ChecksumMustBeRecomputed)
            {
                if (lpdata[0] != BlockType)
                    return IoReadState.BlockTypeMismatch;
                if (*(uint*)(lpdata + 4) != IndexValue)
                    return IoReadState.IndexNumberMissmatch;
                if (*(uint*)(lpdata + 8) >= m_snapshotSequenceNumber)
                    return IoReadState.PageNewerThanSnapshotSequenceNumber;
                if (*(ushort*)(lpdata + 2) != m_fileIdNumber)
                    return IoReadState.FileIdNumberDidNotMatch;
                return IoReadState.Valid;
            }
            throw new Exception("Checksum was not computed properly.");
        }

        private IoReadState IsFooterValid()
        {
            byte* lpdata = Pointer + m_blockSize - 32;
            int checksumState = lpdata[28];
            if (checksumState == Footer.ChecksumIsNotValid)
                return IoReadState.ChecksumInvalid;
            if (checksumState == Footer.ChecksumIsValid || checksumState == Footer.ChecksumMustBeRecomputed)
            {
                if (lpdata[0] != BlockType)
                    return IoReadState.BlockTypeMismatch;
                if (*(uint*)(lpdata + 4) != IndexValue)
                    return IoReadState.IndexNumberMissmatch;
                if (*(uint*)(lpdata + 8) > m_snapshotSequenceNumber)
                    return IoReadState.PageNewerThanSnapshotSequenceNumber;
                if (*(ushort*)(lpdata + 2) != m_fileIdNumber)
                    return IoReadState.FileIdNumberDidNotMatch;
                return IoReadState.Valid;
            }
            throw new Exception("Checksum was not computed properly.");
        }

        private IoReadState IsFooterCurrentSnapshotAndValid()
        {
            byte* lpdata = Pointer + m_blockSize - 32;
            int checksumState = lpdata[28];
            if (checksumState == Footer.ChecksumIsNotValid)
                return IoReadState.ChecksumInvalid;
            if (checksumState == Footer.ChecksumIsValid || checksumState == Footer.ChecksumMustBeRecomputed)
            {
                if (lpdata[0] != BlockType)
                    return IoReadState.BlockTypeMismatch;
                if (*(uint*)(lpdata + 4) != IndexValue)
                    return IoReadState.IndexNumberMissmatch;
                if (*(uint*)(lpdata + 8) != m_snapshotSequenceNumber)
                    return IoReadState.PageNewerThanSnapshotSequenceNumber;
                if (*(ushort*)(lpdata + 2) != m_fileIdNumber)
                    return IoReadState.FileIdNumberDidNotMatch;
                return IoReadState.Valid;
            }
            throw new Exception("Checksum was not computed properly.");
        }

        private void WriteFooterData()
        {
            byte* data = Pointer + m_blockSize - 32;

            data[28] = Footer.ChecksumMustBeRecomputed;
            data[0] = BlockType;
            *(uint*)(data + 4) = IndexValue;
            *(ushort*)(data + 2) = m_fileIdNumber;
            *(uint*)(data + 8) = m_snapshotSequenceNumber;
        }

        private void ClearFooterData()
        {
            long* ptr = (long*)(Pointer + m_blockSize - 32);
            ptr[0] = 0;
            ptr[1] = 0;
            ptr[2] = 0;
            ptr[3] = 0; //ToDo: Consider if this should not be done. This will clear the footer status page.
        }

        #endregion

        #region [ Static ]

        internal static long ReadCount;
        internal static long WriteCount;
        public static long CachedLookups;
        public static long Lookups;

        #endregion
    }
}