//******************************************************************************************************
//  ArchiveList.cs - Gbtc
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
using System.Collections.Generic;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Manages the complete list of archive resources and the 
    /// associated reading and writing that goes along with it.
    /// </summary>
    public partial class ArchiveList : IDisposable
    {
        bool m_disposed;
        object m_syncRoot = new object();

        /// <summary>
        /// Contains the list of all archives.
        /// </summary>
        List<ArchiveFileSummary> m_fileSummaries;
        List<ArchiveFile> m_lockedFiles;

        /// <summary>
        /// Contains all of the active snapshots of the archive lists
        /// This is used for determining when resources are no longer in use.
        /// </summary>
        List<ArchiveListSnapshot> m_allSnapshots;

        public ArchiveList()
        {
            m_lockedFiles = new List<ArchiveFile>();
            m_fileSummaries = new List<ArchiveFileSummary>();
            m_allSnapshots = new List<ArchiveListSnapshot>();
        }

        public ArchiveList(IEnumerable<string> archiveFiles)
            : this()
        {
            foreach (var file in archiveFiles)
            {
                var archiveFile = ArchiveFile.OpenFile(file, AccessMode.ReadOnly);
                var archiveFileSummary = new ArchiveFileSummary(archiveFile);
                m_fileSummaries.Add(archiveFileSummary);
            }
        }

        #region [ Resource Locks ]

        /// <summary>
        /// Creates an object that can be used to get updated snapshots from this <see cref="ArchiveList"/>.
        /// Client must call <see cref="IDisposable.Dispose"/> method when finished with these resources as they will not 
        /// automatically be reclaimed by the garbage collector. Class will not be initiallized until calling <see cref="ArchiveListSnapshot.UpdateSnapshot"/>.
        /// </summary>
        /// <returns></returns>
        public ArchiveListSnapshot CreateNewClientResources()
        {
            lock (m_syncRoot)
            {
                var resources = new ArchiveListSnapshot(ReleaseClientResources, AcquireSnapshot);
                m_allSnapshots.Add(resources);
                return resources;
            }
        }

        /// <summary>
        /// Invoked by <see cref="ArchiveListSnapshot.Dispose"/> method.
        /// </summary>
        /// <param name="archiveLists"></param>
        void ReleaseClientResources(ArchiveListSnapshot archiveLists)
        {
            lock (m_syncRoot)
            {
                m_allSnapshots.Remove(archiveLists);
            }
        }

        /// <summary>
        /// Invoked by <see cref="ArchiveListSnapshot.UpdateSnapshot"/>.
        /// </summary>
        /// <param name="transaction"></param>
        void AcquireSnapshot(ArchiveListSnapshot transaction)
        {
            lock (m_syncRoot)
            {
                transaction.Tables = m_fileSummaries.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// Returns an <see cref="IDisposable"/> class that can be used to edit the contents of this resource.
        /// WARNING: Make changes quickly and dispose the returned class.  All calls to this class are blocked while
        /// editing this class.
        /// </summary>
        /// <returns></returns>
        public Editor AcquireEditLock()
        {
            return new Editor(this);
        }

        /// <summary>
        /// Determines if the provided partition file is currently in use
        /// by any resource. 
        /// </summary>
        /// <param name="archive">the partition to search for.</param>
        /// <returns></returns>
        public bool IsPartitionBeingUsed(ArchiveFile archive)
        {
            lock (m_syncRoot)
            {
                foreach (var resource in m_allSnapshots)
                {
                    var tables = resource.Tables;
                    if (tables != null)
                    {
                        for (int x = 0; x < tables.Length; x++)
                        {
                            var summary = tables[x];
                            if (summary != null && summary.ArchiveFileFile == archive)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                using (var edit = AcquireEditLock())
                {
                    foreach (var f in edit.ArchiveFiles)
                    {
                        f.ArchiveFileFile.Dispose();
                    }
                }
                m_disposed = true;
            }

        }
    }
}
