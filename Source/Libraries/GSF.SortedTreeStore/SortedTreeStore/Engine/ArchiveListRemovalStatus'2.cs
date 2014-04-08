//******************************************************************************************************
//  ArchiveListRemovalStatus.cs - Gbtc
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
//  7/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine
{
    /// <summary>
    /// The return value of <see cref="ArchiveList{TKey,TValue}.Editor.TryRemove"/> provided to the calling
    /// function to determine when a resource is no longer being used.
    /// </summary>
    public class ArchiveListRemovalStatus<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private bool m_isBeingUsed;
        private readonly SortedTreeTable<TKey, TValue> m_sortedTree;
        private ArchiveList<TKey, TValue> m_collection;

        public ArchiveListRemovalStatus(SortedTreeTable<TKey, TValue> sortedTree, ArchiveList<TKey, TValue> collection)
        {
            m_sortedTree = sortedTree;
            m_collection = collection;
            m_isBeingUsed = true;
        }

        /// <summary>
        /// Checks on the status of the removed <see cref="SortedTree"/> to determine if it is still being used
        /// by a <see cref="ArchiveList{TKey,TValue}"/>'s <see cref="ArchiveListSnapshot{TKey,TValue}"/>.
        /// </summary>
        public bool IsBeingUsed
        {
            get
            {
                if (m_isBeingUsed)
                    m_isBeingUsed = m_collection.IsPartitionBeingUsed(m_sortedTree);
                if (!m_isBeingUsed)
                    m_collection = null;
                return m_isBeingUsed;
            }
        }

        /// <summary>
        /// The <see cref="HistorianArchiveFile"/> that was removed from the <see cref="ArchiveList{TKey,TValue}"/>.
        /// </summary>
        public SortedTreeTable<TKey, TValue> SortedTree
        {
            get
            {
                return m_sortedTree;
            }
        }
    }
}