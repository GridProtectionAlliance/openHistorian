//******************************************************************************************************
//  ArchiveList`2_Editor.cs - Gbtc
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using GSF.SortedTreeStore.Storage;

namespace GSF.SortedTreeStore.Engine
{
    public partial class ArchiveList<TKey, TValue>
    {
        /// <summary>
        /// Provides a way to edit an <see cref="ArchiveList{TKey,TValue}"/> since all edits must be atomic.
        /// WARNING: Instancing this class on an <see cref="ArchiveList{TKey,TValue}"/> will lock the class
        /// until <see cref="Dispose"/> is called. Therefore, keep locks to a minimum and always
        /// use a Using block.
        /// </summary>
        public class Editor : IDisposable
        {
            private bool m_disposed;
            private ArchiveList<TKey, TValue> m_collection;
            private ReadOnlyCollection<ArchiveTableSummary<TKey, TValue>> m_archiveFiles;

            /// <summary>
            /// Creates an editor for the ArchiveList
            /// </summary>
            /// <param name="collection"></param>
            public Editor(ArchiveList<TKey, TValue> collection)
            {
                m_collection = collection;
                m_archiveFiles = new ReadOnlyCollection<ArchiveTableSummary<TKey, TValue>>(collection.m_fileSummaries);
                Monitor.Enter(m_collection.m_syncRoot);
            }

            /// <summary>
            /// Represents a readonly list of all of the partitions. 
            /// To edit the partitions, call <see cref="Add"/>, <see cref="Remove"/>, or <see cref="RenewSnapshot"/>.
            /// </summary>
            public ReadOnlyCollection<ArchiveTableSummary<TKey, TValue>> ArchiveFiles
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_archiveFiles;
                }
            }

            /// <summary>
            /// Releases an edit lock that was placed on an archive file.
            /// </summary>
            /// <param name="sortedTree/param>
            public void ReleaseEditLock(SortedTreeTable<TKey, TValue> sortedTree)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_collection.m_lockedFiles.Remove(sortedTree);
            }

            /// <summary>
            /// Renews the snapshot of the partition file. This will acquire the latest 
            /// read transaction so all new snapshots will use this later version.
            /// </summary>
            /// <param name="sortedTree">file to update the snapshot on.</param>
            /// <returns></returns>
            public bool RenewSnapshot(SortedTreeTable<TKey, TValue> sortedTree)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                List<ArchiveTableSummary<TKey, TValue>> partitions = m_collection.m_fileSummaries;
                for (int x = 0; x < partitions.Count; x++)
                {
                    if (partitions[x].SortedTreeTable == sortedTree)
                    {
                        partitions[x] = new ArchiveTableSummary<TKey, TValue>(sortedTree);
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Adds an archive file to the list with the given state information.
            /// </summary>
            /// <param name="sortedTree">archive table to add</param>
            /// <param name="isLocked">the item added contains a write lock on the file.</param>
            public void Add(SortedTreeTable<TKey, TValue> sortedTree, bool isLocked)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                ArchiveTableSummary<TKey, TValue> summary = new ArchiveTableSummary<TKey, TValue>(sortedTree);
                m_collection.m_fileSummaries.Add(summary);
                if (isLocked)
                    m_collection.m_lockedFiles.Add(sortedTree);
            }

            /// <summary>
            /// Removes the first occurnace of <see cref="archive"/> from <see cref="ArchiveList{TKey,TValue}"/>.
            /// </summary>
            /// <param name="sortedTree">the partition to remove</param>
            /// <param name="listRemovalStatus">A <see cref="ArchiveListRemovalStatus{TKey,TValue}"/> that can be used to determine
            /// when this resource is no longer being used and can be closed as a result.  
            /// Closing prematurely can cause erratic behaviour which may result in 
            /// data coruption and the application crashing.  Value is null if no item can be found.</param>
            /// <returns>True if the item was removed, False otherwise.</returns>
            /// <exception cref="Exception">Thrown if <see cref="archive"/> is not in this list.</exception>
            public bool Remove(SortedTreeTable<TKey, TValue> sortedTree, out ArchiveListRemovalStatus<TKey, TValue> listRemovalStatus)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                List<ArchiveTableSummary<TKey, TValue>> partitions = m_collection.m_fileSummaries;
                for (int x = 0; x < partitions.Count; x++)
                {
                    if (partitions[x].SortedTreeTable == sortedTree)
                    {
                        listRemovalStatus = new ArchiveListRemovalStatus<TKey, TValue>(partitions[x].SortedTreeTable, m_collection);
                        partitions.RemoveAt(x);
                        m_collection.m_lockedFiles.Remove(sortedTree);
                        return true;
                    }
                }
                listRemovalStatus = null;
                return false;
            }

            /// <summary>
            /// Removes the supplied file from the <see cref="ArchiveList{TKey,TValue}"/> and queues it for deletion.
            /// </summary>
            /// <param name="sortedTree">file to remove and delete.</param>
            /// <returns></returns>
            public bool RemoveAndDelete(SortedTreeTable<TKey, TValue> sortedTree)
            {
                ArchiveListRemovalStatus<TKey, TValue> status;
                if (Remove(sortedTree, out status))
                {
                    if (!status.IsBeingUsed)
                    {
                        status.SortedTree.BaseFile.Delete();
                        return true;
                    }
                    status.SortedTree.BaseFile.EmptyFile();
                    m_collection.m_filesToDelete.Add(status);
                    m_collection.m_processRemovals.Start(1000);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Releases the lock on the <see cref="ArchiveList{TKey,TValue}"/>.
            /// </summary>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    Monitor.Exit(m_collection.m_syncRoot);
                    m_archiveFiles = null;
                    m_collection = null;
                    m_disposed = true;
                }
            }
        }
    }
}