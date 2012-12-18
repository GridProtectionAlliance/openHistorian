//******************************************************************************************************
//  ArchiveList_Editor.cs - Gbtc
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
//  7/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.ObjectModel;
using System.Threading;
using openHistorian.Archive;

namespace openHistorian.Engine
{
    internal partial class ArchiveList
    {
        /// <summary>
        /// Provides a way to edit an <see cref="ArchiveList"/> since all edits must be atomic.
        /// WARNING: Instancing this class on an <see cref="ArchiveList"/> will lock the class
        /// until <see cref="Dispose"/> is called. Therefore, keep locks to a minimum and always
        /// use a Using block.
        /// </summary>
        public class Editor : IDisposable
        {
            bool m_disposed;
            ArchiveList m_collection;
            ReadOnlyCollection<ArchiveFileSummary> m_archiveFiles;

            public Editor(ArchiveList collection)
            {
                m_collection = collection;
                m_archiveFiles = new ReadOnlyCollection<ArchiveFileSummary>(collection.m_fileSummaries);
                Monitor.Enter(m_collection.m_syncRoot);
            }

            /// <summary>
            /// Represents a readonly list of all of the partitions. 
            /// To edit the partitions, call <see cref="Add"/>, <see cref="Remove"/>, or <see cref="RenewSnapshot"/>.
            /// </summary>
            public ReadOnlyCollection<ArchiveFileSummary> ArchiveFiles
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
            /// <param name="archive"></param>
            public void ReleaseEditLock(ArchiveFile archive)
            {
                
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_collection.m_lockedFiles.Remove(archive);
            }

            /// <summary>
            /// Renews the snapshot of the partition file. This will acquire the latest 
            /// read transaction so all new snapshots will use this later version.
            /// </summary>
            /// <param name="archive">the file to update the snapshot on.</param>
            /// <returns></returns>
            public bool RenewSnapshot(ArchiveFile archive)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                var partitions = m_collection.m_fileSummaries;
                for (int x = 0; x < partitions.Count; x++)
                {
                    if (partitions[x].ArchiveFileFile == archive)
                    {
                        partitions[x] = new ArchiveFileSummary(archive);
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Adds an archive file to the list with the given state information.
            /// </summary>
            /// <param name="archive"></param>
            /// <param name="isLocked"></param>
            public void Add(ArchiveFile archive, bool isLocked)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                var summary = new ArchiveFileSummary(archive);
                m_collection.m_fileSummaries.Add(summary);
                if (isLocked)
                    m_collection.m_lockedFiles.Add(archive);
            }

            /// <summary>
            /// Removes the first occurnace of <see cref="archive"/> from <see cref="ArchiveList"/>.
            /// </summary>
            /// <param name="archive">The partition to remove</param>
            /// <param name="listRemovalStatus">A <see cref="ArchiveListRemovalStatus"/> that can be used to determine
            /// when this resource is no longer being used and can be closed as a result.  
            /// Closing prematurely can cause erratic behaviour which may result in 
            /// data coruption and the application crashing.  Value is null if no item can be found.</param>
            /// <returns>True if the item was removed, False otherwise.</returns>
            /// <exception cref="Exception">Thrown if <see cref="archive"/> is not in this list.</exception>
            public bool Remove(ArchiveFile archive, out ArchiveListRemovalStatus listRemovalStatus)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                var partitions = m_collection.m_fileSummaries;
                for (int x = 0; x < partitions.Count; x++)
                {
                    if (partitions[x].ArchiveFileFile == archive)
                    {
                        listRemovalStatus = new ArchiveListRemovalStatus(partitions[x].ArchiveFileFile, m_collection);
                        partitions.RemoveAt(x);
                        m_collection.m_lockedFiles.Remove(archive);
                        return true;
                    }
                }
                listRemovalStatus = null;
                return false;
            }

            /// <summary>
            /// Releases the lock on the <see cref="ArchiveList"/>.
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
