//******************************************************************************************************
//  DiskIo.cs - Gbtc
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
//  3/24/2012 - Steven E. Chisholm
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
    /// The IO system that the entire file structure uses to acomplish it's IO operations.
    /// This class hands data one block at a time to requesting classes
    /// and is responsible for checking the footer data of the file for corruption.
    /// </summary>
    internal sealed class DiskIo : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(DiskIo), MessageClass.Component);

        #region [ Members ]

        private DiskMedium m_stream;
        private readonly int m_blockSize;
        private readonly bool m_isReadOnly;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        private DiskIo(DiskMedium stream, bool isReadOnly)
        {
            if (stream is null)
                throw new ArgumentNullException("stream");
            m_isReadOnly = isReadOnly;
            m_blockSize = stream.BlockSize;
            m_stream = stream;
        }

#if DEBUG
        ~DiskIo()
        {
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
        }
#endif

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the number of bytes in a single block.
        /// </summary>
        public int BlockSize => m_blockSize;

        /// <summary>
        /// Gets if the disk supports writing.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_isReadOnly;
            }
        }

        /// <summary>
        /// Gets if the class has been disposed.
        /// </summary>
        public bool IsDisposed => m_disposed;

        /// <summary>
        /// Gets the current size of the file.
        /// </summary>
        public long FileSize
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_stream.Length;
            }
        }

        /// <summary>
        /// Returns the last block that is readonly.
        /// </summary>
        public uint LastReadonlyBlock
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_stream.Header.LastAllocatedBlock;
            }
        }

        /// <summary>
        /// Gets the file header that was the last header to be committed to the disk.
        /// </summary>
        public FileHeaderBlock LastCommittedHeader
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_stream.Header;
            }
        }

        public string FileName => m_stream.FileName;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// Occurs when rolling back a transaction. This will free up
        /// any temporary space allocated for the change. 
        /// </summary>
        public void RollbackChanges()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_isReadOnly)
                throw new ReadOnlyException();
            m_stream.RollbackChanges();
        }

        /// <summary>
        /// Occurs when committing the following data to the disk.
        /// This will copy any pending data to the disk in a manner that
        /// will protect against corruption.
        /// </summary>
        /// <param name="header"></param>
        public void CommitChanges(FileHeaderBlock header)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_isReadOnly)
                throw new ReadOnlyException();
            m_stream.CommitChanges(header);
        }

        /// <summary>
        /// Creates a <see cref="DiskIoSession"/> that can be used to perform basic read/write functions.
        /// </summary>
        /// <returns></returns>
        public DiskIoSession CreateDiskIoSession(FileHeaderBlock header, SubFileHeader file)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new DiskIoSession(this, m_stream.CreateIoSession(), header, file);
        }

        /// <summary>
        /// Changes the extension of the current file.
        /// </summary>
        /// <param name="extension">the new extension</param>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        public void ChangeExtension(string extension, bool isReadOnly, bool isSharingEnabled)
        {
            m_stream.ChangeExtension(extension, isReadOnly, isSharingEnabled);
        }

        /// <summary>
        /// Reopens the file with different permissions.
        /// </summary>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        public void ChangeShareMode(bool isReadOnly, bool isSharingEnabled)
        {
            m_stream.ChangeShareMode(isReadOnly, isSharingEnabled);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DiskIo"/> object and optionally releases the managed resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_stream != null)
                    {
                        m_stream.Dispose();
                    }
                }
                finally
                {
                    GC.SuppressFinalize(this);
                    m_stream = null;
                    m_disposed = true;
                }
            }
        }

        #endregion

        public static DiskIo CreateMemoryFile(MemoryPool pool, int fileStructureBlockSize, params Guid[] flags)
        {
            DiskMedium disk = DiskMedium.CreateMemoryFile(pool, fileStructureBlockSize, flags);
            return new DiskIo(disk, false);
        }

        public static DiskIo CreateFile(string fileName, MemoryPool pool, int fileStructureBlockSize, params Guid[] flags)
        {
            //Exclusive opening to prevent duplicate opening.
            CustomFileStream fileStream = CustomFileStream.CreateFile(fileName, pool.PageSize, fileStructureBlockSize);
            DiskMedium disk = DiskMedium.CreateFile(fileStream, pool, fileStructureBlockSize, flags);
            return new DiskIo(disk, false);
        }

        public static DiskIo OpenFile(string fileName, MemoryPool pool, bool isReadOnly)
        {
            CustomFileStream fileStream = CustomFileStream.OpenFile(fileName, pool.PageSize, out int fileStructureBlockSize, isReadOnly, true);
            DiskMedium disk = DiskMedium.OpenFile(fileStream, pool, fileStructureBlockSize);
            return new DiskIo(disk, isReadOnly);
        }
    }
}