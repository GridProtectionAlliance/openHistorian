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
using System.IO;
using System.Threading;
using GSF.SortedTreeStore.Storage;

namespace GSF.SortedTreeStore.Server
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

            /// <summary>
            /// Creates an editor for the ArchiveList
            /// </summary>
            /// <param name="collection"></param>
            public Editor(ArchiveList<TKey, TValue> collection)
            {
                m_collection = collection;
                Monitor.Enter(m_collection.m_syncRoot);
            }

            /// <summary>
            /// Attempts to acquire an edit lock on a file. 
            /// </summary>
            /// <param name="archiveId"></param>
            /// <returns></returns>
            public bool TryAcquireEditLock(Guid archiveId)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (m_collection.m_lockedFiles.Contains(archiveId))
                {
                    return false;
                }
                m_collection.m_lockedFiles.Add(archiveId);
                return true;
            }

            /// <summary>
            /// Releases an edit lock that was placed on an archive file.
            /// </summary>
            /// <param name="archiveId">the ID of the archive snapshot to renew</param>
            public void ReleaseEditLock(Guid archiveId)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_collection.m_lockedFiles.Remove(archiveId);
            }

            /// <summary>
            /// Renews the snapshot of the archive file. This will acquire the latest 
            /// read transaction so all new snapshots will use this later version.
            /// </summary>
            /// <param name="archiveId">the ID of the archive snapshot to renew</param>
            /// <returns></returns>
            public void RenewSnapshot(Guid archiveId)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_collection.m_fileSummaries[archiveId] = new ArchiveTableSummary<TKey, TValue>(m_collection.m_fileSummaries[archiveId].SortedTreeTable);
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
                m_collection.m_fileSummaries.Add(sortedTree.ArchiveId, summary);
                if (isLocked)
                    m_collection.m_lockedFiles.Add(sortedTree.ArchiveId);
            }

            /// <summary>
            /// Returns true if the archive list contains the provided file.
            /// </summary>
            /// <param name="fileId"></param>
            /// <returns></returns>
            public bool Contains(Guid fileId)
            {
                return m_collection.m_fileSummaries.ContainsKey(fileId);
            }

            /// <summary>
            /// Removes the <see cref="archiveId"/> from <see cref="ArchiveList{TKey,TValue}"/>.
            /// </summary>
            /// <param name="archiveId">the archive to remove</param>
            /// <param name="listRemovalStatus">A <see cref="ArchiveListRemovalStatus{TKey,TValue}"/> that can be used to determine
            /// when this resource is no longer being used and can be closed as a result.  
            /// Closing prematurely can cause erratic behaviour which may result in 
            /// data coruption and the application crashing.  Value is null if function returns false.</param>
            /// <returns>True if the item was removed, False otherwise.</returns>
            /// <remarks>
            /// Also unlocks the archive file.
            /// </remarks>
            public bool TryRemove(Guid archiveId, out ArchiveListRemovalStatus<TKey, TValue> listRemovalStatus)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                var partitions = m_collection.m_fileSummaries;
                if (!partitions.ContainsKey(archiveId))
                {
                    listRemovalStatus = null;
                    return false;
                }

                var tree = partitions[archiveId].SortedTreeTable;
                partitions.Remove(archiveId);
                listRemovalStatus = new ArchiveListRemovalStatus<TKey, TValue>(tree, m_collection);
                m_collection.m_lockedFiles.Remove(archiveId);
                return true;
            }

            /// <summary>
            /// Removes the supplied file from the <see cref="ArchiveList{TKey,TValue}"/> and queues it for deletion.
            /// </summary>
            /// <param name="sortedTree">file to remove and delete.</param>
            /// <returns>true if deleted, false otherwise</returns>
            public bool TryRemoveAndDelete(Guid sortedTree)
            {
                ArchiveListRemovalStatus<TKey, TValue> status;
                if (TryRemove(sortedTree, out status))
                {
                    if (!status.IsBeingUsed)
                    {
                        status.SortedTree.BaseFile.Delete();
                        return true;
                    }
                    string fileName = status.SortedTree.BaseFile.FilePath;
                    if (fileName != string.Empty)
                    {
                        string newFileName = Path.ChangeExtension(fileName, "~d2");
                        File.WriteAllBytes(newFileName, new byte[0]);
                    }
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
                    m_disposed = true;
                    Monitor.Exit(m_collection.m_syncRoot);
                    m_collection = null;
                }
            }

        }
    }
}