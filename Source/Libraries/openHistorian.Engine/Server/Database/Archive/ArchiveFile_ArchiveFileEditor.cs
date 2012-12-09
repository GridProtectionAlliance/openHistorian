//******************************************************************************************************
//  ArchiveFile_ArchiveFileEditor.cs - Gbtc
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.FileStructure;

namespace openHistorian.Server.Database.Archive
{
    public partial class ArchiveFile
    {
        public class ArchiveFileEditor : IDisposable
        {
            bool m_disposed;
            ArchiveFile m_archiveFile;
            TransactionalEdit m_currentTransaction;
            SortedTreeContainerEdit m_dataTree;

            /// <summary>
            /// To only be called by <see cref="ArchiveFileEditor"/>
            /// </summary>
            /// <param name="archiveFile"></param>
            public ArchiveFileEditor(ArchiveFile archiveFile)
            {
                m_archiveFile = archiveFile;
                m_currentTransaction = m_archiveFile.m_fileStructure.BeginEdit();
                m_dataTree = new SortedTreeContainerEdit(m_currentTransaction, s_pointDataFile, 1);
            }

            /// <summary>
            /// Commits the edits to the current archive file.
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
            /// Rolls back all edits that are made to the archive file.
            /// </summary>
            public void Rollback()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_dataTree.Dispose();
                m_currentTransaction.RollbackAndDispose();
                InternalDispose();
            }

            public void AddPoint(ulong date, ulong pointId, ulong value1, ulong value2)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_dataTree.AddPoint(date, pointId, value1, value2);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
