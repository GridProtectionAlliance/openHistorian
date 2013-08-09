//******************************************************************************************************
//  ArchiveFile_Editor.cs - Gbtc
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using openHistorian.Collections.Generic;
using openHistorian.FileStructure;

namespace openHistorian.Archive
{
    public partial class ArchiveTable<TKey, TValue>
    {
        /// <summary>
        /// A single instance editor that is used
        /// to modifiy an archive file.
        /// </summary>
        public class Editor : IDisposable
        {
            private bool m_disposed;
            private ArchiveTable<TKey, TValue> m_archiveFile;
            private readonly TransactionalEdit m_currentTransaction;
            private readonly SortedTreeContainerEdit<TKey, TValue> m_dataTree;

            internal Editor(ArchiveTable<TKey, TValue> archiveFile)
            {
                m_archiveFile = archiveFile;
                m_currentTransaction = m_archiveFile.m_fileStructure.BeginEdit();
                m_dataTree = new SortedTreeContainerEdit<TKey, TValue>(m_currentTransaction, archiveFile.m_fileName);
            }

            /// <summary>
            /// Commits the edits to the current archive file and disposes of this class.
            /// </summary>
            public void Commit()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_dataTree.GetKeyRange(m_archiveFile.m_firstKey, m_archiveFile.m_lastKey);
                m_dataTree.Dispose();
                m_currentTransaction.CommitAndDispose();
                InternalDispose();
            }

            /// <summary>
            /// Rolls back all edits that are made to the archive file and disposes of this class.
            /// </summary>
            public void Rollback()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_dataTree.Dispose();
                m_currentTransaction.RollbackAndDispose();
                InternalDispose();
            }

            /// <summary>
            /// Adds a single point to the archive file.
            /// </summary>
            /// <param name="key">the first 64 bits of the key</param>
            /// <param name="value">the first 64 bits of the value</param>
            public void AddPoint(TKey key, TValue value)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_dataTree.AddPoint(key, value);
            }

            public TreeScannerBase<TKey, TValue> GetRange()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_dataTree.GetDataRange();
            }

            public void AddPoints(TreeStream<TKey, TValue> scanner)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_dataTree.AddPoints(scanner);
            }

            /// <summary>
            /// Rollsback edits to the file.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    Rollback();
                }
            }

            private void InternalDispose()
            {
                m_disposed = true;
                m_archiveFile.m_activeEditor = null;
                m_archiveFile = null;
            }
        }
    }
}