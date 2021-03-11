//******************************************************************************************************
//  SortedTreeTableSnapshotInfo`2.cs - Gbtc
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

using GSF.IO.FileStructure;

namespace GSF.Snap.Storage
{
    /// <summary>
    /// Aquires a read transaction on the current archive partition. This will allow all user created
    /// transactions to have snapshot isolation of the entire data set.
    /// </summary>
    public class SortedTreeTableSnapshotInfo<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        #region [ Members ]

        private readonly TransactionalFileStructure m_fileStructure;
        private readonly ReadSnapshot m_currentTransaction;
        private readonly SubFileName m_fileName;

        #endregion

        #region [ Constructors ]

        internal SortedTreeTableSnapshotInfo(TransactionalFileStructure fileStructure, SubFileName fileName)
        {
            m_fileName = fileName;
            m_fileStructure = fileStructure;
            m_currentTransaction = m_fileStructure.Snapshot;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens an instance of the archive file to allow for concurrent reading of a snapshot.
        /// </summary>
        /// <returns></returns>
        public SortedTreeTableReadSnapshot<TKey, TValue> CreateReadSnapshot()
        {
            return new SortedTreeTableReadSnapshot<TKey, TValue>(m_currentTransaction, m_fileName);
        }

        #endregion
    }
}