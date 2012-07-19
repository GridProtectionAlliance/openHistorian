//******************************************************************************************************
//  PartitionFile.cs - Gbtc
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.IO;
using openHistorian.V2.Collections.KeyValue;
using openHistorian.V2.FileSystem;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Server.Database.Partitions
{
    /// <summary>
    /// Represents a individual self-contained archive file. 
    /// This is one of many files that are part of a given <see cref="DatabaseEngine"/>.
    /// </summary>
    public class PartitionFile : IDisposable
    {
        #region [ Members ]

        static Guid s_pointDataFile = new Guid("{29D7CCC2-A474-11E1-885A-B52D6288709B}");

        string m_fileName;
        ulong m_firstKey;
        ulong m_lastKey;
        bool m_disposed;
        VirtualFileSystem m_fileSystem;
        TransactionalEdit m_currentTransaction;
        BasicTreeContainerEdit m_dataTree;

        #endregion

        #region [ Constructors ]

        public PartitionFile()
        {
            m_fileName = string.Empty;
            m_fileSystem = new VirtualFileSystem();
            InitializeNewFile();
        }

        public PartitionFile(string file, OpenMode openMode, AccessMode accessMode)
        {
            m_fileName = file;
            m_fileSystem = new VirtualFileSystem(file, openMode, accessMode);
            if (openMode == OpenMode.Create)
            {
                InitializeNewFile();
            }
            else
            {
                using (var snapshot = CreateSnapshot().OpenInstance())
                {
                    m_firstKey = snapshot.FirstKey;
                    m_lastKey = snapshot.LastKey;
                }
            }
        }

        #endregion

        #region [ Properties ]

        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// The first key.  Note: Values only update on commit.
        /// </summary>
        public ulong FirstKey
        {
            get
            {
                return m_firstKey;
            }
        }

        /// <summary>
        /// The last key.  Note: Values only update on commit.
        /// </summary>
        public ulong LastKey
        {
            get
            {
                return m_lastKey;
            }
        }

        /// <summary>
        /// Returns the name of the file.  Returns <see cref="String.Empty"/> if this is a memory archive.
        /// </summary>
        public string FileName
        {
            get
            {
                return m_fileName;
            }
        }

        public long FileSize
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region [ Methods ]

        void InitializeNewFile()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            using (var trans = m_fileSystem.BeginEdit())
            {
                using (var fs = trans.CreateFile(s_pointDataFile, 1))
                using (var bs = new BinaryStream(fs))
                {
                    var tree = new BasicTree(bs, ArchiveConstants.DataBlockDataLength);
                    m_firstKey = tree.FirstKey;
                    m_lastKey = tree.LastKey;
                }
                trans.CommitAndDispose();
            }
        }

        /// <summary>
        /// Aquires a snapshot of the current file system.
        /// </summary>
        /// <returns></returns>
        public PartitionSnapshot CreateSnapshot()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new PartitionSnapshot(m_fileSystem);
        }

        /// <summary>
        /// Begins an edit of the current archive file.
        /// </summary>
        public void BeginEdit()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_currentTransaction = m_fileSystem.BeginEdit();
            m_dataTree = new BasicTreeContainerEdit(m_currentTransaction, s_pointDataFile, 1);

        }
        /// <summary>
        /// Commits the edits to the current archive file.
        /// </summary>
        public void CommitEdit()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_firstKey = m_dataTree.FirstKey;
            m_lastKey = m_dataTree.LastKey;
            m_dataTree.Dispose();
            m_currentTransaction.CommitAndDispose();
        }

        /// <summary>
        /// Rolls back all edits that are made to the archive file.
        /// </summary>
        public void RollbackEdit()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_dataTree.Dispose();
            m_currentTransaction.RollbackAndDispose();
        }

        public void AddPoint(ulong date, ulong pointId, ulong value1, ulong value2)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_dataTree.AddPoint(date, pointId, value1, value2);
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_fileSystem.Dispose();
                m_disposed = true;
            }

        }
        
        /// <summary>
        /// Closes and deletes the partition. Also calls dispose.
        /// If this is a memory archive, it will release the memory space to the buffer pool.
        /// </summary>
        public void Delete()
        {
            Dispose();
            if (m_fileName != string.Empty)
            {
                File.Delete(m_fileName);
            }
        }

        #endregion

    }
}
