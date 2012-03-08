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
using System.Collections.Generic;

namespace openHistorian.Core.StorageSystem.File
{
    /// <summary>
    /// Aquires a snapshot of the file system to browse in an isolated mannor.  
    /// This is read only and will also block the main file from being deleted. 
    /// Therefore it is important to release this lock so the file can be deleted after a rollover.
    /// </summary>
    public class TransactionalRead : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// This event is raised when the read transaction has been disposed. 
        /// The purpose of this event is to notify the <see cref="FileSystemSnapshotService"/>
        /// that this transaction is no longer executing.
        /// </summary>
        internal event EventHandler HasBeenDisposed;

        /// <summary>
        /// Prevents duplicate calls to Dispose;
        /// </summary>
        bool m_disposed;
        /// <summary>
        /// Maintains a list of all of the files that have been opened 
        /// so they can be properly disposed of when the transaction ends.
        /// </summary>
        List<ArchiveFileStream> m_openedFiles;

        /// <summary>
        /// The readonly snapshot of the archive file.
        /// </summary>
        FileAllocationTable m_fileAllocationTable;

        /// <summary>
        /// The underlying diskIO to do the reads against.
        /// </summary>
        DiskIoBase m_dataReader;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a readonly copy of a transaction.
        /// </summary>
        /// <param name="dataReader"> </param>
        /// <param name="fileAllocationTable">This parameter must be in a read only mode.
        /// This is to ensure that the value is not modified after it has been passed to this class.</param>
        internal TransactionalRead(DiskIoBase dataReader, FileAllocationTable fileAllocationTable)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");
            if (fileAllocationTable == null)
                throw new ArgumentNullException("fileAllocationTable");
            if (!fileAllocationTable.IsReadOnly)
                throw new ArgumentException("The file passed to this procedure must be read only.", "fileAllocationTable");
            m_openedFiles = new List<ArchiveFileStream>();
            m_fileAllocationTable = fileAllocationTable;
            m_dataReader = dataReader;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="TransactionalRead"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~TransactionalRead()
        {
            Dispose(false);
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
            if (fileIndex < 0 || fileIndex >= m_fileAllocationTable.Files.Count)
                throw new ArgumentOutOfRangeException("fileIndex", "The file index provided could not be found in the header.");

            ArchiveFileStream archiveFileStream = new ArchiveFileStream(m_dataReader, m_fileAllocationTable.Files[fileIndex], m_fileAllocationTable, true);
            if (!archiveFileStream.IsReadOnly)
                throw new Exception("Stream is supposed to be readonly.");
            m_openedFiles.Add(archiveFileStream);
            return archiveFileStream;
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="TransactionalRead"/> object.
        /// This also closes all ArchiveFileStream objects that were opened in this transaction.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="TransactionalRead"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (m_openedFiles != null)
                    {
                        for (int x = 0; x < m_openedFiles.Count; x++)
                        {
                            ArchiveFileStream file = m_openedFiles[x];
                            if (file != null)
                            {
                                file.Dispose();
                                m_openedFiles[x] = null;
                            }
                        }
                        m_openedFiles = null;
                    }

                    if (HasBeenDisposed != null)
                        HasBeenDisposed(this, EventArgs.Empty);

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

        #endregion
    }
}
