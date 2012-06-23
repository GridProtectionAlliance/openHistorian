//******************************************************************************************************
//  TransactionalRead.cs - Gbtc
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
//  12/2/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.ObjectModel;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// Aquires a snapshot of the file system to browse in an isolated mannor.  
    /// This is read only and will also block the main file from being deleted. 
    /// Therefore it is important to release this lock so the file can be deleted after a rollover.
    /// </summary>
    public sealed class TransactionalRead : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// This delegate is called when the read transaction has been disposed. 
        /// The purpose of this event is to notify the <see cref="FileSystemSnapshotService"/>
        /// that this transaction is no longer executing.
        /// </summary>
        Action<TransactionalRead> m_delHasBeenDisposed;

        /// <summary>
        /// Prevents duplicate calls to Dispose;
        /// </summary>
        bool m_disposed;

        /// <summary>
        /// The readonly snapshot of the archive file.
        /// </summary>
        FileAllocationTable m_fileAllocationTable;

        /// <summary>
        /// The underlying diskIO to do the reads against.
        /// </summary>
        DiskIo m_dataReader;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a readonly copy of a transaction.
        /// </summary>
        /// <param name="dataReader"> </param>
        /// <param name="fileAllocationTable">This parameter must be in a read only mode.
        ///  This is to ensure that the value is not modified after it has been passed to this class.</param>
        /// <param name="delHasBeenDisposed">A delegate to call when this transaction is transaction has been disposed</param>
        internal TransactionalRead(DiskIo dataReader, FileAllocationTable fileAllocationTable, Action<TransactionalRead> delHasBeenDisposed = null)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");
            if (fileAllocationTable == null)
                throw new ArgumentNullException("fileAllocationTable");
            if (!fileAllocationTable.IsReadOnly)
                throw new ArgumentException("The file passed to this procedure must be read only.", "fileAllocationTable");
            m_fileAllocationTable = fileAllocationTable;
            m_dataReader = dataReader;
            m_delHasBeenDisposed = delHasBeenDisposed;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A list of all of the files in this collection.
        /// </summary>
        public ReadOnlyCollection<FileMetaData> Files
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_fileAllocationTable.Files;
            }
        }

        /// <summary>
        /// Determines if this object has been disposed
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
        /// Opens a ArchiveFileStream that can be used to read the file passed to this function.
        /// </summary>
        /// <param name="fileIndex">The index of the file to open.</param>
        /// <returns></returns>
        public ArchiveFileStream OpenFile(int fileIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (fileIndex < 0 || fileIndex >= m_fileAllocationTable.Files.Count)
                throw new ArgumentOutOfRangeException("fileIndex", "The file index provided could not be found in the header.");

            return new ArchiveFileStream(m_dataReader, m_fileAllocationTable.Files[fileIndex], m_fileAllocationTable, true);
        }

        /// <summary>
        /// Opens a ArchiveFileStream that can be used to read/write to the file passed to this function.
        /// </summary>
        /// <returns></returns>
        public ArchiveFileStream OpenFile(Guid fileExtension, int fileFlags)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            for (int x = 0; x < Files.Count; x++)
            {
                var file = Files[x];
                if (file.FileExtension == fileExtension && file.FileFlags == fileFlags)
                {
                    return OpenFile(x);
                }
            }
            throw new Exception("File does not exist");
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="TransactionalRead"/> object.
        /// This also closes all ArchiveFileStream objects that were opened in this transaction.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_delHasBeenDisposed != null)
                    {
                        m_delHasBeenDisposed.Invoke(this);
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion
    }
}
