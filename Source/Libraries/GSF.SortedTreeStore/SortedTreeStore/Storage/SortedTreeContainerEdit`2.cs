//******************************************************************************************************
//  SortedTreeContainerEdit`2.cs - Gbtc
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
//  5/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Tree;
using GSF.IO.FileStructure;

namespace GSF.SortedTreeStore.Storage
{
    /// <summary>
    /// Encapsolates the ArchiveFileStream, BinaryStream, and BasicTree for a certain tree.
    /// </summary>
    internal class SortedTreeContainerEdit<TKey, TValue>
        : IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        #region [ Members ]

        private SubFileStream m_subStream;
        private BinaryStream m_binaryStream1;
        private BinaryStream m_binaryStream2;
        private SortedTree<TKey, TValue> m_tree;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public SortedTreeContainerEdit(TransactionalEdit currentTransaction, SubFileName fileName)
        {
            m_subStream = currentTransaction.OpenFile(fileName);
            m_binaryStream1 = new BinaryStream(m_subStream);
            m_binaryStream2 = new BinaryStream(m_subStream);
            m_tree = SortedTree<TKey, TValue>.Open(m_binaryStream1, m_binaryStream2);
            m_tree.AutoFlush = false;
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

        #endregion

        #region [ Methods ]

        public void GetKeyRange(TKey firstKey, TKey lastKey)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_tree.GetKeyRange(firstKey, lastKey);
        }

        public void AddPoint(TKey key, TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_tree.TryAdd(key, value);
        }

        public void AddPoints(TreeStream<TKey, TValue> stream)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_tree.TryAddRange(stream);
        }

        public SortedTreeScannerBase<TKey, TValue> GetDataRange()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return m_tree.CreateTreeScanner();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
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
                    if (m_binaryStream2 != null)
                    {
                        m_binaryStream2.Dispose();
                        m_binaryStream2 = null;
                    }
                    if (m_subStream != null)
                    {
                        m_subStream.Dispose();
                        m_subStream = null;
                    }
                }
                finally
                {
                    m_tree = null;
                    m_disposed = true;
                }
            }
        }

        #endregion
    }
}