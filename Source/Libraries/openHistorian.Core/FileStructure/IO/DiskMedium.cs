//******************************************************************************************************
//  DiskMedium.cs - Gbtc
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
//  2/22/2013 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Threading;
using GSF.IO.Unmanaged;
using GSF.UnmanagedMemory;

namespace openHistorian.FileStructure.IO
{
    /// <summary>
    /// Provides read/write access to all of the different types of disk types
    /// to use to store the file structure.
    /// </summary>
    internal class DiskMedium : IDisposable
    {
        #region [ Members ]

        private FileHeaderBlock m_header;
        private IDiskMedium m_disk;
        private readonly int m_blockSize;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        private DiskMedium(IDiskMedium disk, FileHeaderBlock header)
        {
            m_header = header;
            m_disk = disk;
            m_blockSize = header.BlockSize;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the current number of bytes used by the file system. 
        /// This is only intended to be an approximate figure. 
        /// </summary>
        public long Length
        {
            get
            {
                return m_disk.Length;
            }
        }

        /// <summary>
        /// Gets the most recent committed header from the archive file.
        /// </summary>
        public FileHeaderBlock Header
        {
            get
            {
                return m_header;
            }
        }

        /// <summary>
        /// Gets the number of bytes in the file structure block size.
        /// </summary>
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
        /// Occurs when rolling back a transaction. This will free up
        /// any temporary space allocated for the change. 
        /// </summary>
        public void RollbackChanges()
        {
            m_disk.RollbackChanges();
        }

        /// <summary>
        /// Occurs when committing the following data to the disk.
        /// This will copy any pending data to the disk in a manner that
        /// will protect against corruption.
        /// </summary>
        /// <param name="header"></param>
        public void CommitChanges(FileHeaderBlock header)
        {
            header.IsReadOnly = true;
            m_disk.FlushWithHeader(header);
            Thread.MemoryBarrier();
            m_header = header;
        }

        public BinaryStreamIoSessionBase CreateIoSession()
        {
            return m_disk.CreateIoSession();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                m_disk.Dispose();
                m_disk = null;
            }
        }

        #endregion

        #region [ Static ]

        public static DiskMedium CreateMemoryFile(MemoryPool pool, int fileStructureBlockSize)
        {
            FileHeaderBlock header = FileHeaderBlock.CreateNew(fileStructureBlockSize);
            MemoryPoolFile disk = new MemoryPoolFile(pool);
            return new DiskMedium(disk, header);
        }

        public static DiskMedium CreateFile(FileStream stream, MemoryPool pool, int fileStructureBlockSize)
        {
            FileHeaderBlock header = FileHeaderBlock.CreateNew(fileStructureBlockSize);
            BufferedFile disk = new BufferedFile(stream, pool, header, true);
            return new DiskMedium(disk, header);
        }

        public static DiskMedium OpenFile(FileStream stream, MemoryPool pool, int fileStructureBlockSize)
        {
            byte[] buffer = new byte[fileStructureBlockSize];
            stream.Position = 0;
            stream.Read(buffer, 0, fileStructureBlockSize);
            FileHeaderBlock header = FileHeaderBlock.Open(buffer);
            BufferedFile disk = new BufferedFile(stream, pool, header, false);
            return new DiskMedium(disk, header);
        }

        #endregion
    }
}