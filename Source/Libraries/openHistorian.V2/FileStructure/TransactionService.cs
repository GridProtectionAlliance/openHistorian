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
using System.IO;
using System.Threading;
using openHistorian.V2.Unmanaged;
using MemoryStream = openHistorian.V2.IO.Unmanaged.MemoryStream;

namespace openHistorian.V2.FileStructure
{
    /// <summary>
    /// This class is responsible for managing the transactions that occur on the file system.
    /// Therefore, it keeps up with the latest snapshot of the file allocation table, 
    /// permits only a single concurrent edit of the archive system, and determines when a file
    /// can be deleted when there are no read or write transactions. It also containst the IO system.
    /// </summary>
    internal sealed class TransactionService : IDisposable
    {
        #region [ Members ]

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
        FileHeaderBlock m_fileHeaderBlock;

        /// <summary>
        /// Contains the current active transaction.  If this null, there is no active transaction.
        /// </summary>
        TransactionalEdit m_currentTransaction;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new archive file that is completely in memory
        ///  </summary>
        public TransactionService()
        {
            m_diskIo = new DiskIo(new MemoryStream(), 0);
            m_fileHeaderBlock = new FileHeaderBlock(m_diskIo, OpenMode.Create, AccessMode.ReadOnly);
        }

        /// <summary>
        /// Opens/Creates and archive file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="openMode"></param>
        /// <param name="accessMode"></param>
        public TransactionService(string fileName, OpenMode openMode, AccessMode accessMode)
        {
            if (openMode == OpenMode.Create)
            {
                FileStream fileStream = new FileStream(fileName, FileMode.CreateNew);
                BufferedFileStream bufferedFileStream = new BufferedFileStream(fileStream);
                m_diskIo = new DiskIo(bufferedFileStream, 0);
                m_fileHeaderBlock = new FileHeaderBlock(m_diskIo, OpenMode.Create, accessMode);
            }
            else
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Open, (accessMode == AccessMode.ReadOnly) ? FileAccess.Read : FileAccess.ReadWrite);
                BufferedFileStream bufferedFileStream = new BufferedFileStream(fileStream);
                m_diskIo = new DiskIo(bufferedFileStream, 0);
                m_fileHeaderBlock = new FileHeaderBlock(m_diskIo, OpenMode.Open, accessMode);
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// This will start a transactional edit on the file. 
        /// </summary>
        /// <returns></returns>
        public TransactionalEdit BeginEditTransaction()
        {
            if (m_diskIo.IsReadOnly)
                throw new Exception("File has been opened in readonly mode");
            var transaction = new TransactionalEdit(m_diskIo, m_fileHeaderBlock, OnTransactionRolledBack, OnTransactionCommitted);
            Interlocked.CompareExchange(ref m_currentTransaction, transaction, null);
            if (m_currentTransaction != transaction)
                throw new Exception("Only one edit transaction can exist at one time.");
            return m_currentTransaction;
        }

        /// <summary>
        /// This will start a transactional read on the file. 
        /// </summary>
        /// <returns></returns>
        public TransactionalRead BeginReadTransaction()
        {
            TransactionalRead readTransaction = new TransactionalRead(m_diskIo, m_fileHeaderBlock);
            return readTransaction;
        }

        void OnTransactionCommitted()
        {
            m_fileHeaderBlock = new FileHeaderBlock(m_diskIo, OpenMode.Open, AccessMode.ReadOnly);
            m_currentTransaction = null;
        }

        void OnTransactionRolledBack()
        {
            m_currentTransaction = null;

        }

        /// <summary>
        /// Releases all the resources used by the <see cref="TransactionService"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_currentTransaction != null)
                    {
                        m_currentTransaction.Dispose();
                        m_currentTransaction = null;
                    }
                    if (m_diskIo != null)
                    {
                        m_diskIo.Dispose();
                        m_diskIo = null;
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
