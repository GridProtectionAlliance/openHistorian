//******************************************************************************************************
//  ArchiveFileSummary.cs - Gbtc
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
//  5/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine
{
    /// <summary>
    /// Contains an immutable class of the current partition
    /// along with its most recent snapshot.
    /// </summary>
    public class ArchiveTableSummary<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        #region [ Members ]

        private readonly TKey m_firstKey;
        private readonly TKey m_lastKey;
        private readonly SortedTreeTable<TKey, TValue> m_sortedTreeTable;
        private readonly SortedTreeTableSnapshotInfo<TKey, TValue> m_activeSnapshotInfo;
        private readonly SortedTreeTypeMethodsBase<TKey> m_keyMethods;

        #endregion

        #region [ Constructors ]

        public ArchiveTableSummary(SortedTreeTable<TKey, TValue> file)
        {
            m_keyMethods = new TKey().CreateValueMethods();
            m_sortedTreeTable = file;
            m_activeSnapshotInfo = file.AcquireReadSnapshot();
            m_firstKey = file.FirstKey;
            m_lastKey = file.LastKey;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="HistorianArchiveFile"/> that this class represents.
        /// </summary>
        public SortedTreeTable<TKey, TValue> SortedTreeTable
        {
            get
            {
                return m_sortedTreeTable;
            }
        }

        /// <summary>
        /// Gets the first key contained in this partition.
        /// </summary>
        public TKey FirstKey
        {
            get
            {
                return m_firstKey;
            }
        }

        /// <summary>
        /// Gets the last key contained in this partition.
        /// </summary>
        public TKey LastKey
        {
            get
            {
                return m_lastKey;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return m_keyMethods.IsGreaterThan(FirstKey, LastKey);
            }
        }

        /// <summary>
        /// Gets the most recent <see cref="ArchiveSnapshotInfo"/> of this class when it was instanced.
        /// </summary>
        public SortedTreeTableSnapshotInfo<TKey, TValue> ActiveSnapshotInfo
        {
            get
            {
                return m_activeSnapshotInfo;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Determines if this partition might contain data for the keys provided.
        /// </summary>
        /// <param name="startKey">the first key searching for</param>
        /// <param name="stopKey">the last key searching for</param>
        /// <returns></returns>
        public bool Contains(TKey startKey, TKey stopKey)
        {
            //If the archive file is empty, it will always be searched.  
            //Since this will likely never happen and has little performance 
            //implications, I have decided not to include logic that would exclude this case.
            return !(m_keyMethods.IsGreaterThan(startKey, LastKey) || m_keyMethods.IsLessThan(stopKey, FirstKey));
        }

        #endregion
    }
}