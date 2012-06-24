//  TransactionalEdit.cs - Gbtc
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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// Provides the state information for a transaction on the file system.
    /// </summary>
    /// <remarks>Failing to call Commit or Rollback will inhibit additional transactions to be aquired</remarks>
    public sealed class TransactionalEdit : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// This delegate is called when the edit transaction has been disposed. 
        /// The purpose of this delegate is to notify the <see cref="FileSystemSnapshotService"/>
        /// that this transaction is no longer executing and everything has been properly disposed of.
        /// </summary>
        Action<TransactionalEdit> m_delHasBeenDisposed;

        /// <summary>
        /// This delegate is called when the Commit function is called and all the data has been written to the underlying file system.
        /// the purpose of this delegate is to notify the calling class that this transaction is concluded since
        /// only one write transaction can be aquired at a time.
        /// </summary>
        Action<TransactionalEdit> m_delHasBeenCommitted;

        /// <summary>
        /// This delegate is called when the RollBack function is called. This also occurs when the object is disposed.
        /// the purpose of this delegate is to notify the calling class that this transaction is concluded since
        /// only one write transaction can be aquired at a time.
        /// </summary>
        Action<TransactionalEdit> m_delHasBeenRolledBack;

        /// <summary>
        /// Prevents duplicate calls to Dispose;
        /// </summary>
        bool m_disposed;

        /// <summary>
        /// Deteremins if the transaction is complete.  This is to 
        /// prevent duplicate calls to Rollback or Commit.
        /// </summary>
        bool m_transactionComplete;

        /// <summary>
        /// This provides a snapshot of the origional file system incase the owner 
        /// of this transaction wants to view the origional version of the file system.
        /// </summary>
        TransactionalRead m_transactionalRead;

        /// <summary>
        /// The underlying diskIO to do the read/writes against.
        /// </summary>
        DiskIo m_dataReader;

        /// <summary>
        /// The readonly snapshot of the archive file.
        /// </summary>
        FileAllocationTable m_fileAllocationTable;

        /// <summary>
        /// All files that have ever been opened.
        /// </summary>
        List<ArchiveFileStream> m_openedFiles;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an editable copy of the transaction
        /// </summary>
        /// <param name="dataReader"> </param>
        /// <param name="fileAllocationTable">This parameter must be in a read only mode.
        /// This is to ensure that the value is not modified after it has been passed to this class.
        /// This will be converted into an editable version within the constructor of this class</param>
        /// <param name="delHasBeenDisposed">the delegate to call when this transaction has been disposed</param>
        /// <param name="delHasBeenRolledBack">the delegate to call when this transaction has been rolled back</param>
        /// <param name="delHasBeenCommitted">the delegate to call when this transaction has been committed</param>
        internal TransactionalEdit(DiskIo dataReader, FileAllocationTable fileAllocationTable, Action<TransactionalEdit> delHasBeenDisposed = null, Action<TransactionalEdit> delHasBeenRolledBack= null, Action<TransactionalEdit> delHasBeenCommitted = null)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");
            if (fileAllocationTable == null)
                throw new ArgumentNullException("fileAllocationTable");
            if (!fileAllocationTable.IsReadOnly)
                throw new ArgumentException("The file passed to this procedure must be read only.", "fileAllocationTable");

            m_openedFiles = new List<ArchiveFileStream>();
            m_disposed = false;
            m_transactionComplete = false;
            m_transactionalRead = new TransactionalRead(dataReader, fileAllocationTable);
            m_fileAllocationTable = fileAllocationTable.CloneEditableCopy();
            m_dataReader = dataReader;
            m_delHasBeenDisposed = delHasBeenDisposed;
            m_delHasBeenCommitted = delHasBeenCommitted;
            m_delHasBeenRolledBack = delHasBeenRolledBack;
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
        /// A list of all of the files in this snapshot before they were edited.
        /// </summary>
        public ReadOnlyCollection<FileMetaData> OrigionalFiles
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_transactionalRead.Files;
            }
        }

        #endregion

        #region [ Methods ]
        /// <summary>
        /// Creates and Opens a new file on the current file system.
        /// </summary>
        /// <param name="fileExtension">The extension to use for the file.</param>
        /// <param name="fileFlags">Flags that can be used to differentiate between 
        /// files with common extensions. These are not required.</param>
        /// <returns></returns>
        public ArchiveFileStream CreateFile(Guid fileExtension, int fileFlags)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            FileMetaData file = m_fileAllocationTable.CreateNewFile(fileExtension);
            file.FileFlags = fileFlags;
            return OpenFile(file);
        }

        /// <summary>
        /// Opens a ArchiveFileStream that can be used to read/write to the file passed to this function.
        /// </summary>
        /// <param name="fileIndex">The index of the file to open.</param>
        /// <returns></returns>
        public ArchiveFileStream OpenFile(int fileIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (fileIndex < 0 || fileIndex >= m_fileAllocationTable.Files.Count)
                throw new ArgumentOutOfRangeException("fileIndex", "The file index provided could not be found in the header.");
            FileMetaData file = m_fileAllocationTable.Files[fileIndex];
            var fileStream = new ArchiveFileStream(m_dataReader, file, m_fileAllocationTable, AccessMode.ReadWrite);
            m_openedFiles.Add(fileStream);
            return fileStream;
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
        /// Opens a ArchiveFileStream that can be used to read/write to the file passed to this function.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <returns></returns>
        public ArchiveFileStream OpenFile(FileMetaData file)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (file == null)
                throw new ArgumentNullException("file");
            for (int x = 0; x < m_fileAllocationTable.Files.Count; x++)
            {
                if (ReferenceEquals(file, m_fileAllocationTable.Files[x]))
                    return OpenFile(x);
            }
            throw new Exception("The file provided does not belong in the file allocation table");
        }

        /// <summary>
        /// Opens a ArchiveFileStream that can be used to read one of the origional files that existed when this transaction started.
        /// </summary>
        /// <param name="fileIndex">The index of the file to open.</param>
        /// <returns></returns>
        public ArchiveFileStream OpenOrigionalFile(int fileIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return m_transactionalRead.OpenFile(fileIndex);
        }

        /// <summary>
        /// This will cause the transaction to be written to the database.
        /// Also Calls Dispose()
        /// </summary>
        /// <remarks>Duplicate calls to this function, or subsequent calls to RollbackTransaction will throw an exception</remarks>
        public void CommitAndDispose()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_transactionComplete)
                throw new Exception("Duplicate call to Commit/Rollback Transaction");
            foreach (var file in m_openedFiles)
            {
                if (file != null && !file.IsDisposed)
                    throw new Exception("Not all files have been properly disposed of.");
            }
            try
            {
                m_fileAllocationTable.WriteToFileSystem(m_dataReader);
                if (m_delHasBeenCommitted != null)
                    m_delHasBeenCommitted.Invoke(this);
            }
            finally
            {
                m_transactionComplete = true;
                Dispose();
            }
        }

        /// <summary>
        /// This will rollback the transaction by not writing the table of contents to the file.
        /// </summary>
        /// <remarks>Duplicate calls to this function, or subsequent calls to CommitTransaction will throw an exception</remarks>
        public void RollbackAndDispose()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_transactionComplete)
                throw new Exception("Duplicate call to Commit/Rollback Transaction");

            foreach (var file in m_openedFiles)
            {
                if (file != null && !file.IsDisposed)
                {
                    file.Dispose();
                }
            }
            try
            {
                if (m_delHasBeenRolledBack != null)
                    m_delHasBeenRolledBack.Invoke(this);
            }
            finally
            {
                m_transactionComplete = true;
                Dispose();
            }
        }

        /// <summary>
        /// Sets the length of the file system to the length passed, but rounds it up to the nearest block.
        /// </summary>
        /// <param name="size">The desired size, specifying a value less than the current allocated size of the file system 
        /// will cause it to shrink to the current size.</param>
        /// <returns></returns>
        public long SetFileLength(long size)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_disposed)
                throw new Exception("Duplicate call to Commit/Rollback Transaction");
            return m_dataReader.SetFileLength(size, m_fileAllocationTable.LastAllocatedBlock + 1);
        }

        /// <summary>
        /// Computes the amount of free space in the file system.  This takes into consideration the pending edits on the file.
        /// </summary>
        /// <returns></returns>
        public long FreeSpace
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_dataReader.FileSize - (m_fileAllocationTable.LastAllocatedBlock + 1) * ArchiveConstants.BlockSize;
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="TransactionalRead"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (!m_transactionComplete)
                        RollbackAndDispose();

                    if (m_transactionalRead != null)
                    {
                        m_transactionalRead.Dispose();
                        m_transactionalRead = null;
                    }

                    if (m_delHasBeenDisposed != null)
                    {
                        m_delHasBeenDisposed(this);
                        m_delHasBeenDisposed = null;
                    }

                }
                finally
                {
                    m_delHasBeenCommitted = null;
                    m_delHasBeenRolledBack = null;
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion
    }
}
