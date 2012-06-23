//******************************************************************************************************
//  FileSystemSnapshotService.cs - Gbtc
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
//  10/14/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using openHistorian.V2.Unmanaged;
using MemoryStream = openHistorian.V2.IO.Unmanaged.MemoryStream;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// This class is responsible for managing the transactions that occur on the file system.
    /// Therefore, it keeps up with the latest snapshot of the file allocation table, 
    /// permits only a single concurrent edit of the archive system, and determines when a file
    /// can be deleted when there are no read or write transactions. It also containst the IO system.
    /// </summary>
    internal sealed class FileSystemSnapshotService : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// Determines if this object is currently being disposed so all read/write transactions will be properly aborted.
        /// </summary>
        bool m_disposing;
        /// <summary>
        /// Determines if this object has been disposed.
        /// </summary>
        bool m_disposed;

        /// <summary>
        /// Contains the disk IO subsystem for accessing the file.
        /// </summary>
        DiskIo m_diskIo;

        /// <summary>
        /// Contains the current snapshot of the file system.
        /// </summary>
        FileAllocationTable m_fileAllocationTable;

        /// <summary>
        /// This object will be used to synchronize access to aquire a transaction.
        /// </summary>
        object m_editTransactionSynchronizationObject = new object();

        /// <summary>
        /// Contains the current active transaction.  If this null, there is no active transaction.
        /// </summary>
        TransactionalEdit m_currentTransaction;

        /// <summary>
        /// Contains all of the transactions that are currently reading.
        /// </summary>
        List<TransactionalRead> m_readTransactions;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Opens an existing archive file system
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="isReadOnly">Determines if the file will be opened in read only mode.</param>
        private FileSystemSnapshotService(string fileName, bool isReadOnly)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, isReadOnly ? FileAccess.Read : FileAccess.ReadWrite);
            BufferedFileStream bufferedFileStream = new BufferedFileStream(fileStream);
            m_diskIo = new DiskIo(bufferedFileStream, 0);
            m_fileAllocationTable = FileAllocationTable.OpenHeader(m_diskIo);
            m_readTransactions = new List<TransactionalRead>();
        }

        /// <summary>
        /// Creates a new archive file for editing
        /// </summary>
        /// <param name="fileName">the file name</param>
        private FileSystemSnapshotService(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.CreateNew);
            BufferedFileStream bufferedFileStream = new BufferedFileStream(fileStream);
            m_diskIo = new DiskIo(bufferedFileStream, 0);
            FileAllocationTable.CreateFileAllocationTable(m_diskIo);
            m_fileAllocationTable = FileAllocationTable.OpenHeader(m_diskIo);
            m_readTransactions = new List<TransactionalRead>();
        }

        /// <summary>
        /// Creates a new archive file that is completely in memory
        ///  </summary>
        private FileSystemSnapshotService()
        {
            m_diskIo = new DiskIo(new MemoryStream(), 0);
            FileAllocationTable.CreateFileAllocationTable(m_diskIo);
            m_fileAllocationTable = FileAllocationTable.OpenHeader(m_diskIo);
            m_readTransactions = new List<TransactionalRead>();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// This will start a transactional edit on the file. 
        /// </summary>
        /// <param name="timeout">the amout of time in milliseconds to wait for a transaction before timing out and returning null.</param>
        /// <returns></returns>
        public TransactionalEdit BeginEditTransaction(int timeout = -1)
        {
            if (m_diskIo.IsReadOnly)
                throw new Exception("File has been opened in readonly mode");
            if (Monitor.TryEnter(m_editTransactionSynchronizationObject, timeout))
            {
                if (m_currentTransaction != null)
                    throw new Exception("A transaction has already been started");
                m_currentTransaction = new TransactionalEdit(m_diskIo, m_fileAllocationTable, OnEditTransactionDisposed, OnTransactionRolledBack, OnTransactionCommitted);
                return m_currentTransaction;
            }
            return null;
        }

        /// <summary>
        /// This will start a transactional read on the file. 
        /// </summary>
        /// <returns></returns>
        public TransactionalRead BeginReadTransaction()
        {
            TransactionalRead readTransaction = new TransactionalRead(m_diskIo, m_fileAllocationTable, OnReadTransactionDisposed);
            m_readTransactions.Add(readTransaction);
            return readTransaction;
        }

        void OnReadTransactionDisposed(TransactionalRead sender)
        {
            if (m_disposing)
                return;
            m_readTransactions.Remove(sender);
        }

        void OnEditTransactionDisposed(TransactionalEdit sender)
        {
            if (!m_disposing && m_currentTransaction != sender)
                throw new Exception("Only the current transaction can raise this.");
            m_currentTransaction = null;
            Monitor.Exit(m_editTransactionSynchronizationObject);
        }

        void OnTransactionCommitted(TransactionalEdit sender)
        {
            if (!m_disposing && m_currentTransaction != sender)
                throw new Exception("Only the current transaction can raise this.");
            m_fileAllocationTable = FileAllocationTable.OpenHeader(m_diskIo);
        }

        void OnTransactionRolledBack(TransactionalEdit sender)
        {
            if (!m_disposing && m_currentTransaction != sender)
                throw new Exception("Only the current transaction can raise this.");
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="FileSystemSnapshotService"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposing = true;
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (m_diskIo != null)
                    {
                        m_diskIo.Dispose();
                        m_diskIo = null;
                    }
                    if (m_currentTransaction != null)
                    {
                        m_currentTransaction.Dispose();
                        m_currentTransaction = null;
                    }
                    if (m_readTransactions != null)
                    {
                        foreach (var transaction in m_readTransactions)
                        {
                            transaction.Dispose();
                        }
                        m_readTransactions = null;
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion
        #region [ Static ]
        #region [ Methods ]

        /// <summary>
        /// Opens an existing file archive for unbuffered read/write operations.
        /// </summary>
        /// <param name="fileName">The path to the archive file.</param>
        /// <param name="isReadOnly">Determines if the file is going to be opened in readonly mode.</param>
        /// <remarks>
        /// Since buffering the data will be the responsibility of another layer, 
        /// allowing the OS to buffer the data decreases performance (slightly) and
        /// wastes system memory that can be better used by the historian's buffer.
        /// </remarks>
        public static FileSystemSnapshotService OpenFile(string fileName, bool isReadOnly)
        {
            return new FileSystemSnapshotService(fileName, isReadOnly);
        }

        /// <summary>
        /// Creates a new archive file at the given path
        /// </summary>
        /// <param name="fileName">The file name of the archive to write.  This file must not already exist.</param>
        /// <returns></returns>
        public static FileSystemSnapshotService CreateFile(string fileName)
        {
            return new FileSystemSnapshotService(fileName);
        }

        /// <summary>
        /// Creates a new archive file that resides completely in memory.
        /// </summary>
        /// <returns></returns>
        public static FileSystemSnapshotService CreateInMemory()
        {
            return new FileSystemSnapshotService();
        }

        #endregion
        #endregion
    }
}
