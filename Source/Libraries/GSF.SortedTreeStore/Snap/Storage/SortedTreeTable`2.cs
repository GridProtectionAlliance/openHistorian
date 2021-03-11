//******************************************************************************************************
//  SortedTreeTable`2.cs - Gbtc
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
//  05/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.IO.FileStructure;

namespace GSF.Snap.Storage
{
    /// <summary>
    /// Represents an individual table contained within the file. 
    /// </summary>
    public partial class SortedTreeTable<TKey, TValue>
        : IDisposable
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        #region [ Members ]

        private readonly SubFileName m_fileName;
        private readonly TKey m_firstKey;
        private readonly TKey m_lastKey;
        private readonly TransactionalFileStructure m_fileStructure;
        private Editor m_activeEditor;
        private bool m_disposed;

        /// <summary>
        /// Gets the archive file where this table exists.
        /// </summary>
        public SortedTreeFile BaseFile { get; private set; }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a SortedTreeTable
        /// </summary>
        /// <param name="fileStructure"></param>
        /// <param name="fileName"></param>
        /// <param name="baseFile"></param>
        internal SortedTreeTable(TransactionalFileStructure fileStructure, SubFileName fileName, SortedTreeFile baseFile)
        {
            BaseFile = baseFile;
            m_fileName = fileName;
            m_fileStructure = fileStructure;
            m_firstKey = new TKey();
            m_lastKey = new TKey();
            using (SortedTreeTableReadSnapshot<TKey, TValue> snapshot = BeginRead())
            {
                snapshot.GetKeyRange(m_firstKey, m_lastKey);
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if the archive file has been disposed. 
        /// </summary>
        public bool IsDisposed => m_disposed;

        /// <summary>
        /// The first key.  Note: Values only update on commit.
        /// </summary>
        public TKey FirstKey => m_firstKey;

        /// <summary>
        /// The last key.  Note: Values only update on commit.
        /// </summary>
        public TKey LastKey => m_lastKey;


        public Guid ArchiveId => BaseFile.Snapshot.Header.ArchiveId;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// Acquires a read snapshot of the current archive file.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Once the snapshot has been acquired, any future commits
        /// will not effect this snapshot. The snapshot has a tiny footprint
        /// and allows an unlimited number of reads that can be created.
        /// </remarks>
        public SortedTreeTableSnapshotInfo<TKey, TValue> AcquireReadSnapshot()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new SortedTreeTableSnapshotInfo<TKey, TValue>(m_fileStructure, m_fileName);
        }

        /// <summary>
        /// Allows the user to get a read snapshot on the table.
        /// </summary>
        /// <returns></returns>
        public SortedTreeTableReadSnapshot<TKey, TValue> BeginRead()
        {
            return AcquireReadSnapshot().CreateReadSnapshot();
        }

        /// <summary>
        /// Begins an edit of the current archive table.
        /// </summary>
        /// <remarks>
        /// Concurrent editing of a file is not supported. Subsequent calls will
        /// throw an exception rather than blocking. This is to encourage
        /// proper synchronization at a higher layer. 
        /// Wrap the return value of this function in a Using block so the dispose
        /// method is always called. 
        /// </remarks>
        /// <example>
        /// using (ArchiveFile.ArchiveFileEditor editor = archiveFile.BeginEdit())
        /// {
        ///     editor.AddPoint(key, value);
        ///     editor.AddPoint(key, value);
        ///     editor.Commit();
        /// }
        /// </example>
        public SortedTreeTableEditor<TKey, TValue> BeginEdit()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_activeEditor != null)
                throw new Exception("Only one concurrent edit is supported");
            m_activeEditor = new Editor(this);
            return m_activeEditor;
        }

        /// <summary>
        /// Closes the archive file. If there is a current transaction, 
        /// that transaction is immediately rolled back and disposed.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                if (m_activeEditor != null)
                    m_activeEditor.Dispose();
                m_fileStructure.Dispose();
                m_disposed = true;
            }
        }

        ///// <summary>
        ///// Closes and deletes the Archive File. Also calls dispose.
        ///// If this is a memory archive, it will release the memory space to the buffer pool.
        ///// </summary>
        //public void Delete()
        //{
        //    Dispose();
        //    if (m_fileName != string.Empty)
        //    {
        //        File.Delete(m_fileName);
        //    }
        //}
       

        #endregion
    }
}