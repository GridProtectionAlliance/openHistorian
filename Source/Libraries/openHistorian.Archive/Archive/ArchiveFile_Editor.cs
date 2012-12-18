//******************************************************************************************************
//  ArchiveFile_Editor.cs - Gbtc
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
using openHistorian.FileStructure;

namespace openHistorian.Archive
{
    public partial class ArchiveFile
    {
        /// <summary>
        /// A single instance editor that is used
        /// to modifiy an archive file.
        /// </summary>
        public class Editor : IDisposable
        {
            bool m_disposed;
            ArchiveFile m_archiveFile;
            TransactionalEdit m_currentTransaction;
            SortedTreeContainerEdit m_dataTree;

            internal Editor(ArchiveFile archiveFile)
            {
                m_archiveFile = archiveFile;
                m_currentTransaction = m_archiveFile.m_fileStructure.BeginEdit();
                m_dataTree = new SortedTreeContainerEdit(m_currentTransaction, PointDataFile, 1);
            }

            /// <summary>
            /// Commits the edits to the current archive file and disposes of this class.
            /// </summary>
            public void Commit()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_archiveFile.m_firstKey = m_dataTree.FirstKey;
                m_archiveFile.m_lastKey = m_dataTree.LastKey;
                m_currentTransaction.UserData = m_archiveFile.SaveUserData();
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
            /// <param name="key1">the first 64 bits of the key</param>
            /// <param name="key2">the last 64 bits of the key</param>
            /// <param name="value1">the first 64 bits of the value</param>
            /// <param name="value2">the last 64 bits of the value</param>
            public void AddPoint(ulong key1, ulong key2, ulong value1, ulong value2)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_dataTree.AddPoint(key1, key2, value1, value2);
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

            void InternalDispose()
            {
                m_disposed = true;
                m_archiveFile.m_activeEditor = null;
                m_archiveFile = null;
            }
        }
    }
}
