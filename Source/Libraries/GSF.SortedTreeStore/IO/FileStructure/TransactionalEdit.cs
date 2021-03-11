//******************************************************************************************************
//  TransactionalEdit.cs - Gbtc
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
//  12/2/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Diagnostics;
using GSF.Immutable;
using GSF.IO.FileStructure.Media;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// Provides the state information for a transaction on the file system.
    /// </summary>
    /// <remarks>Failing to call Commit or Rollback will inhibit additional transactions to be aquired</remarks>
    public sealed class TransactionalEdit
        : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(TransactionalEdit), MessageClass.Component);

        #region [ Members ]

        /// <summary>
        /// This delegate is called when the Commit function is called and all the data has been written to the underlying file system.
        /// the purpose of this delegate is to notify the calling class that this transaction is concluded since
        /// only one write transaction can be aquired at a time.
        /// </summary>
        private Action m_delHasBeenCommitted;

        /// <summary>
        /// This delegate is called when the RollBack function is called. This also occurs when the object is disposed.
        /// the purpose of this delegate is to notify the calling class that this transaction is concluded since
        /// only one write transaction can be aquired at a time.
        /// </summary>
        private Action m_delHasBeenRolledBack;

        /// <summary>
        /// Prevents duplicate calls to Dispose;
        /// </summary>
        private bool m_disposed;

        /// <summary>
        /// The underlying diskIO to do the read/writes against.
        /// </summary>
        private readonly DiskIo m_dataReader;

        /// <summary>
        /// The readonly snapshot of the archive file.
        /// </summary>
        private readonly FileHeaderBlock m_fileHeaderBlock;

        /// <summary>
        /// All files that have ever been opened.
        /// </summary>
        private readonly List<SubFileStream> m_openedFiles;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an editable copy of the transaction
        /// </summary>
        /// <param name="dataReader"> </param>
        /// <param name="delHasBeenRolledBack">the delegate to call when this transaction has been rolled back</param>
        /// <param name="delHasBeenCommitted">the delegate to call when this transaction has been committed</param>
        internal TransactionalEdit(DiskIo dataReader, Action delHasBeenRolledBack = null, Action delHasBeenCommitted = null)
        {
            if (dataReader is null)
                throw new ArgumentNullException("dataReader");

            m_openedFiles = new List<SubFileStream>();
            m_disposed = false;
            m_fileHeaderBlock = dataReader.LastCommittedHeader.CloneEditable();
            m_dataReader = dataReader;
            m_delHasBeenCommitted = delHasBeenCommitted;
            m_delHasBeenRolledBack = delHasBeenRolledBack;
        }

#if DEBUG
        ~TransactionalEdit()
        {
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
        }
#endif

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A list of all of the files in this collection.
        /// </summary>
        public ImmutableList<SubFileHeader> Files
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_fileHeaderBlock.Files;
            }
        }

        /// <summary>
        /// The guid for this archive type.
        /// </summary>
        public Guid ArchiveType
        {
            get => m_fileHeaderBlock.ArchiveType;
            set => m_fileHeaderBlock.ArchiveType = value;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates and Opens a new file on the current file system.
        /// </summary>
        /// <returns></returns>
        public SubFileStream CreateFile(SubFileName fileName)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_fileHeaderBlock.CreateNewFile(fileName);
            return OpenFile(fileName);
        }

        /// <summary>
        /// Opens a ArchiveFileStream that can be used to read/write to the file passed to this function.
        /// </summary>
        /// <param name="fileIndex">The index of the file to open.</param>
        /// <returns></returns>
        public SubFileStream OpenFile(int fileIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (fileIndex < 0 || fileIndex >= m_fileHeaderBlock.Files.Count)
                throw new ArgumentOutOfRangeException("fileIndex", "The file index provided could not be found in the header.");
            SubFileHeader subFile = m_fileHeaderBlock.Files[fileIndex];
            SubFileStream fileStream = new SubFileStream(m_dataReader, subFile, m_fileHeaderBlock, isReadOnly: false);
            m_openedFiles.Add(fileStream);
            return fileStream;
        }

        /// <summary>
        /// Opens a ArchiveFileStream that can be used to read/write to the file passed to this function.
        /// </summary>
        /// <returns></returns>
        public SubFileStream OpenFile(SubFileName fileName)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            for (int x = 0; x < Files.Count; x++)
            {
                SubFileHeader file = Files[x];
                if (file.FileName == fileName)
                {
                    return OpenFile(x);
                }
            }
            throw new Exception("File does not exist");
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

            foreach (SubFileStream file in m_openedFiles)
            {
                if (file != null && !file.IsDisposed)
                    throw new Exception("Not all files have been properly disposed of.");
            }
            try
            {
                //ToDo: First commit the data, then the file system.
                m_dataReader.CommitChanges(m_fileHeaderBlock);
                if (m_delHasBeenCommitted != null)
                    m_delHasBeenCommitted.Invoke();
            }
            finally
            {
                m_delHasBeenCommitted = null;
                m_delHasBeenRolledBack = null;
                m_disposed = true;
                GC.SuppressFinalize(this);
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

            foreach (SubFileStream file in m_openedFiles)
            {
                if (file != null && !file.IsDisposed)
                {
                    file.Dispose();
                }
            }
            try
            {
                m_dataReader.RollbackChanges();
                if (m_delHasBeenRolledBack != null)
                    m_delHasBeenRolledBack.Invoke();
            }
            finally
            {
                m_delHasBeenCommitted = null;
                m_delHasBeenRolledBack = null;
                m_disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="ReadSnapshot"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                RollbackAndDispose();
                GC.SuppressFinalize(this);
            }
        }

        #endregion


    }
}