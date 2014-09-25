//******************************************************************************************************
//  SortedTreeTable`2_Editor.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Tree;
using GSF.IO.FileStructure;

namespace GSF.SortedTreeStore.Storage
{
    public partial class SortedTreeTable<TKey, TValue>
    {
        /// <summary>
        /// A single instance editor that is used
        /// to modifiy an archive file.
        /// </summary>
        public class Editor : IDisposable
        {
            private bool m_disposed;
            private SortedTreeTable<TKey, TValue> m_sortedTreeFile;
            private readonly TransactionalEdit m_currentTransaction;
            private SubFileStream m_subStream;
            private BinaryStream m_binaryStream1;
            private SortedTree<TKey, TValue> m_tree;

            internal Editor(SortedTreeTable<TKey, TValue> sortedTreeFile)
            {
                m_sortedTreeFile = sortedTreeFile;
                m_currentTransaction = m_sortedTreeFile.m_fileStructure.BeginEdit();
                m_subStream = m_currentTransaction.OpenFile(sortedTreeFile.m_fileName);
                m_binaryStream1 = new BinaryStream(m_subStream);
                m_tree = SortedTree<TKey, TValue>.Open(m_binaryStream1);
                m_tree.AutoFlush = false;
            }

            /// <summary>
            /// Commits the edits to the current archive file and disposes of this class.
            /// </summary>
            public void Commit()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                GetKeyRange(m_sortedTreeFile.m_firstKey, m_sortedTreeFile.m_lastKey);

                if (m_tree != null)
                {
                    m_tree.Flush();
                    m_tree = null;
                }
                if (m_binaryStream1 != null)
                {
                    m_binaryStream1.Dispose();
                    m_binaryStream1 = null;
                }
                if (m_subStream != null)
                {
                    m_subStream.Dispose();
                    m_subStream = null;
                }

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
               
                if (m_tree != null)
                {
                    m_tree.Flush();
                    m_tree = null;
                }
                if (m_binaryStream1 != null)
                {
                    m_binaryStream1.Dispose();
                    m_binaryStream1 = null;
                }
                if (m_subStream != null)
                {
                    m_subStream.Dispose();
                    m_subStream = null;
                }

                m_currentTransaction.RollbackAndDispose();
                InternalDispose();
            }

            public void GetKeyRange(TKey firstKey, TKey lastKey)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_tree.GetKeyRange(firstKey, lastKey);
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
                m_tree.TryAdd(key, value);
            }

            /// <summary>
            /// Adds all of the points to this archive file.
            /// </summary>
            /// <param name="stream"></param>
            public void AddPoints(TreeStream<TKey, TValue> stream)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_tree.TryAddRange(stream);
            }

            /// <summary>
            /// Opens a tree scanner for this archive file
            /// </summary>
            /// <returns></returns>
            public SortedTreeScannerBase<TKey, TValue> GetRange()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_tree.CreateTreeScanner();
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
                m_sortedTreeFile.m_activeEditor = null;
                m_sortedTreeFile = null;
            }
        }
    }
}