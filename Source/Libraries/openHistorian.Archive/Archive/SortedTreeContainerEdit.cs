//******************************************************************************************************
//  SortedTreeContainerEdit.cs - Gbtc
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
//  5/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using openHistorian.Collections.KeyValue;
using openHistorian.FileStructure;
using openHistorian.IO.Unmanaged;

namespace openHistorian.Archive
{
    /// <summary>
    /// Encapsolates the ArchiveFileStream, BinaryStream, and BasicTree for a certain tree.
    /// </summary>
    internal class SortedTreeContainerEdit : IDisposable
    {

        #region [ Members ]

        SubFileStream m_subStream;
        BinaryStream m_binaryStream;
        SortedTree256Base m_tree;
        bool m_disposed;

        #endregion

        #region [ Constructors ]

        public SortedTreeContainerEdit(TransactionalEdit currentTransaction, Guid fileNumber, int flags)
        {
            m_subStream = currentTransaction.OpenFile(fileNumber, flags);
            m_binaryStream = new BinaryStream(m_subStream);
            m_tree = SortedTree256Initializer.Open(m_binaryStream);
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

        public ulong FirstKey
        {
            get
            {
                return m_tree.FirstKey;
            }
        }

        public ulong LastKey
        {
            get
            {
                return m_tree.LastKey;
            }
        }

        #endregion

        #region [ Methods ]

        public void AddPoint(ulong date, ulong pointId, ulong value1, ulong value2)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_tree.Add(date, pointId, value1, value2);
        }

        public void AddPoints(ITreeScanner256 scanner)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_tree.Add(scanner);
        }

        public ITreeScanner256 GetDataRange()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return m_tree.GetDataRange();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if(m_tree != null)
                    {
                        m_tree.Save();
                        m_tree = null;
                    }
                    if (m_binaryStream != null)
                    {
                        m_binaryStream.Dispose();
                        m_binaryStream = null;
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
