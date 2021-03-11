//******************************************************************************************************
//  ArchiveTableSummary`2.cs - Gbtc
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
//  05/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.Snap.Storage;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Contains an immutable class of the current table
    /// along with its most recent snapshot.
    /// </summary>
    public class ArchiveTableSummary<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        #region [ Members ]

        private readonly TKey m_firstKey;
        private readonly TKey m_lastKey;
        private readonly SortedTreeTable<TKey, TValue> m_sortedTreeTable;
        private readonly SortedTreeTableSnapshotInfo<TKey, TValue> m_activeSnapshotInfo;
        private readonly Guid m_fileId;
        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a snapshot summary of a table.
        /// </summary>
        /// <param name="table">the table to take the read snapshot of.</param>
        public ArchiveTableSummary(SortedTreeTable<TKey, TValue> table)
        {
            m_firstKey = new TKey();
            m_lastKey = new TKey();
            m_sortedTreeTable = table;
            m_activeSnapshotInfo = table.AcquireReadSnapshot();
            table.FirstKey.CopyTo(m_firstKey);
            table.LastKey.CopyTo(m_lastKey);
            m_fileId = m_sortedTreeTable.BaseFile.Snapshot.Header.ArchiveId;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the ID for this file.
        /// </summary>
        public Guid FileId => m_fileId;

        /// <summary>
        /// Gets the <see cref="SortedTreeTable{TKey, TValue}"/> that this class represents.
        /// </summary>
        public SortedTreeTable<TKey, TValue> SortedTreeTable => m_sortedTreeTable;

        /// <summary>
        /// Gets the first key contained in this partition.
        /// </summary>
        public TKey FirstKey => m_firstKey;

        /// <summary>
        /// Gets the last key contained in this partition.
        /// </summary>
        public TKey LastKey => m_lastKey;

        /// <summary>
        /// Gets if this table is empty.
        /// </summary>
        public bool IsEmpty => FirstKey.IsGreaterThan(LastKey);

        /// <summary>
        /// Gets the most recent <see cref="SortedTreeTableSnapshotInfo{TKey,TValue}"/> of this class when it was created.
        /// </summary>
        public SortedTreeTableSnapshotInfo<TKey, TValue> ActiveSnapshotInfo => m_activeSnapshotInfo;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// Determines if this table might contain data for the keys provided.
        /// </summary>
        /// <param name="startKey">the first key searching for</param>
        /// <param name="stopKey">the last key searching for</param>
        /// <returns></returns>
        public bool Contains(TKey startKey, TKey stopKey)
        {
            //If the archive file is empty, it will always be searched.  
            //Since this will likely never happen and has little performance 
            //implications, I have decided not to include logic that would exclude this case.
            return !(startKey.IsGreaterThan(LastKey) || stopKey.IsLessThan(FirstKey));
        }

        #endregion
    }
}