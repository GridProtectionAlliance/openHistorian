//******************************************************************************************************
//  DiskIo.cs - Gbtc
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
//  3/24/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
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
        #region [ Members ]

        private DiskMedium m_stream;
        private readonly int m_blockSize;
        private readonly bool m_isReadOnly;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        private DiskIo(DiskMedium stream, bool isReadOnly)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            m_isReadOnly = isReadOnly;
            m_blockSize = stream.BlockSize;
            m_stream = stream;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the number of bytes in a single block.
        /// </summary>
        public int BlockSize
        {
            get
            {
                return m_blockSize;
            }
        }

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
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

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
        public DiskIoSession CreateDiskIoSession(FileHeaderBlock header, SubFileMetaData file)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new DiskIoSession(this, m_stream.CreateIoSession(), header, file);
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
                    m_stream = null;
                    m_disposed = true;
                }
            }
        }

        #endregion

        public static DiskIo CreateMemoryFile(MemoryPool pool, int fileStructureBlockSize, Guid uniqueFileId = default(Guid), params Guid[] flags)
        {
            DiskMedium disk = DiskMedium.CreateMemoryFile(pool, fileStructureBlockSize, uniqueFileId, flags);
            return new DiskIo(disk, false);
        }

        public static DiskIo CreateFile(string fileName, MemoryPool pool, int fileStructureBlockSize, Guid uniqueFileId = default(Guid), params Guid[] flags)
        {
            //Exclusive opening to prevent duplicate opening.
            FileStream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
            DiskMedium disk = DiskMedium.CreateFile(fileStream, pool, fileStructureBlockSize, uniqueFileId, flags);
            return new DiskIo(disk, false);
        }

        public static DiskIo OpenFile(string fileName, MemoryPool pool, bool isReadOnly)
        {
            //Exclusive opening to prevent duplicate opening.
            FileStream fileStream = new FileStream(fileName, FileMode.Open, isReadOnly ? FileAccess.Read : FileAccess.ReadWrite, FileShare.None);
            int fileStructureBlockSize = FileHeaderBlock.SearchForBlockSize(fileStream);
            DiskMedium disk = DiskMedium.OpenFile(fileStream, pool, fileStructureBlockSize);
            return new DiskIo(disk, isReadOnly);
        }
    }
}