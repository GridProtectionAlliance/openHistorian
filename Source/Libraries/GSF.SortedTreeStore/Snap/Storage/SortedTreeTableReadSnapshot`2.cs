//******************************************************************************************************
//  SortedTreeTableReadSnapshot`2.cs - Gbtc
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
//  05/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.IO.Unmanaged;
using GSF.Snap.Tree;
using GSF.IO.FileStructure;

namespace GSF.Snap.Storage
{
    /// <summary>
    /// Provides a user with a read-only instance of an archive.
    /// This class is not thread safe.
    /// </summary>
    public class SortedTreeTableReadSnapshot<TKey, TValue>
        : IDisposable
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        #region [ Members ]

        private SubFileStream m_subStream;
        private BinaryStream m_binaryStream;
        private SortedTree<TKey, TValue> m_tree;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        internal SortedTreeTableReadSnapshot(ReadSnapshot currentTransaction, SubFileName fileName)
        {
            try
            {
                m_subStream = currentTransaction.OpenFile(fileName);
                m_binaryStream = new BinaryStream(m_subStream);
                m_tree = SortedTree<TKey, TValue>.Open(m_binaryStream);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if this read snapshot has been disposed.
        /// </summary>
        public bool IsDisposed => m_disposed;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets a reader that can be used to parse an archive file.
        /// </summary>
        /// <returns></returns>
        public SortedTreeScannerBase<TKey, TValue> GetTreeScanner()
        {
            return m_tree.CreateTreeScanner();
        }
        /// <summary>
        /// Returns the lower and upper bounds of the tree
        /// </summary>
        /// <param name="lowerBounds">the first key in the tree</param>
        /// <param name="upperBounds">the last key in the tree</param>
        /// <remarks>
        /// If the tree is empty, lowerBounds will be greater than upperBounds</remarks>
        public void GetKeyRange(TKey lowerBounds, TKey upperBounds)
        {
            m_tree.GetKeyRange(lowerBounds, upperBounds);
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
                    if (m_binaryStream != null)
                    {
                        m_binaryStream.Dispose();
                    }
                    if (m_subStream != null)
                    {
                        m_subStream.Dispose();
                    }
                }
                finally
                {
                    m_subStream = null;
                    m_binaryStream = null;
                    m_tree = null;
                    m_disposed = true;
                }
            }
        }

        #endregion
    }
}