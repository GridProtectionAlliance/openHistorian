//******************************************************************************************************
//  ArchiveFileReadOnlySnapshotInstance.cs - Gbtc
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

namespace openHistorian.Archive
{
    /// <summary>
    /// Provides a user with a read-only instance of an archive.
    /// This class is not thread safe.
    /// </summary>
    public class ArchiveFileReadOnlySnapshotInstance : IDisposable
    {

        //Since there is currently only one BasicTree per partition, this is basically a pass through class.
        
        #region [ Members ]
        

        bool m_disposed;
        SortedTreeContainer m_dataTree;

        #endregion

        #region [ Constructors ]

        public ArchiveFileReadOnlySnapshotInstance(TransactionalRead currentTransaction)
        {
            m_dataTree = new SortedTreeContainer(currentTransaction, ArchiveFile.s_pointDataFile, 1);
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

        public ulong FirstKey
        {
            get
            {
                return m_dataTree.FirstKey;
            }
        }

        public ulong LastKey
        {
            get
            {
                return m_dataTree.LastKey;
            }
        }

        public ITreeScanner256 GetDataRange()
        {
            return m_dataTree.GetDataRange();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_dataTree != null)
                    {
                        m_dataTree.Dispose();
                        m_dataTree = null;
                    }
                }
                finally
                {
                    m_disposed = true;
                }
            }
        }
        #endregion
    
    }
}
