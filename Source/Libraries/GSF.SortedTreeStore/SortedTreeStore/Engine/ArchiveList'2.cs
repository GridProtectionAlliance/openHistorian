//******************************************************************************************************
//  ArchiveList`2.cs - Gbtc
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
using System.Text;
using GSF.Threading;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine
{
    /// <summary>
    /// Manages the complete list of archive resources and the 
    /// associated reading and writing that goes along with it.
    /// </summary>
    public partial class ArchiveList<TKey, TValue>
        : IDisposable
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        private bool m_disposed;
        private readonly object m_syncRoot = new object();

        /// <summary>
        /// Contains the list of all archives.
        /// </summary>
        private readonly List<ArchiveTableSummary<TKey, TValue>> m_fileSummaries;

        private readonly List<SortedTreeTable<TKey, TValue>> m_lockedFiles;

        /// <summary>
        /// Contains all of the active snapshots of the archive lists
        /// This is used for determining when resources are no longer in use.
        /// </summary>
        private readonly List<ArchiveListSnapshot<TKey, TValue>> m_allSnapshots;
        /// <summary>
        /// The scheduled task for removing items.
        /// </summary>
        private readonly ScheduledTask m_processRemovals;

        private readonly List<ArchiveListRemovalStatus<TKey, TValue>> m_filesToDelete;
        private readonly List<ArchiveListRemovalStatus<TKey, TValue>> m_filesToDispose;

        /// <summary>
        /// Creates an ArchiveList
        /// </summary>
        public ArchiveList()
        {
            m_filesToDelete = new List<ArchiveListRemovalStatus<TKey, TValue>>();
            m_filesToDispose = new List<ArchiveListRemovalStatus<TKey, TValue>>();
            m_processRemovals = new ScheduledTask(ThreadingMode.DedicatedBackground);
            m_processRemovals.OnRunWorker += m_processRemovals_OnRunWorker;
            m_processRemovals.OnDispose += m_processRemovals_OnDispose;
            m_processRemovals.OnException += m_processRemovals_OnException;
            m_lockedFiles = new List<SortedTreeTable<TKey, TValue>>();
            m_fileSummaries = new List<ArchiveTableSummary<TKey, TValue>>();
            m_allSnapshots = new List<ArchiveListSnapshot<TKey, TValue>>();
        }

        /// <summary>
        /// Creates an ArchiveList including all of the provided files.
        /// </summary>
        /// <param name="archiveFiles"></param>
        public ArchiveList(IEnumerable<string> archiveFiles)
            : this()
        {
            foreach (string file in archiveFiles)
            {
                try
                {
                    SortedTreeFile sortedTreeFile = SortedTreeFile.OpenFile(file, isReadOnly: true);
                    var table = sortedTreeFile.OpenTable<TKey, TValue>();
                    if (table == null)
                    {
                        sortedTreeFile.Dispose();
                        //archiveFile.Delete(); //ToDo: Consider the consequences of deleting a file.
                    }
                    else
                    {
                        ArchiveTableSummary<TKey, TValue> archiveTableSummary = new ArchiveTableSummary<TKey, TValue>(table);
                        m_fileSummaries.Add(archiveTableSummary);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        #region [ Resource Locks ]

        /// <summary>
        /// Creates an object that can be used to get updated snapshots from this <see cref="ArchiveList{TKey,TValue}"/>.
        /// Client must call <see cref="IDisposable.Dispose"/> method when finished with these resources as they will not 
        /// automatically be reclaimed by the garbage collector. Class will not be initiallized until calling <see cref="ArchiveListSnapshot{TKey,TValue}.UpdateSnapshot"/>.
        /// </summary>
        /// <returns></returns>
        public ArchiveListSnapshot<TKey, TValue> CreateNewClientResources()
        {
            lock (m_syncRoot)
            {
                ArchiveListSnapshot<TKey, TValue> resources = new ArchiveListSnapshot<TKey, TValue>(ReleaseClientResources, AcquireSnapshot);
                m_allSnapshots.Add(resources);
                return resources;
            }
        }

        /// <summary>
        /// Invoked by <see cref="ArchiveListSnapshot{TKey,TValue}.Dispose"/> method.
        /// </summary>
        /// <param name="archiveLists"></param>
        private void ReleaseClientResources(ArchiveListSnapshot<TKey, TValue> archiveLists)
        {
            lock (m_syncRoot)
            {
                m_allSnapshots.Remove(archiveLists);
            }
        }

        /// <summary>
        /// Invoked by <see cref="ArchiveListSnapshot{TKey,TValue}.UpdateSnapshot"/>.
        /// </summary>
        /// <param name="transaction"></param>
        private void AcquireSnapshot(ArchiveListSnapshot<TKey, TValue> transaction)
        {
            lock (m_syncRoot)
            {
                transaction.Tables = m_fileSummaries.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// Appends the status of the files in the ArchiveList to the provided <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="status"></param>
        public void GetFullStatus(StringBuilder status)
        {
            lock (m_syncRoot)
            {
                status.AppendFormat("Files Pending Deletion: {0} Disposal: {1}\r\n", m_filesToDelete.Count, m_filesToDispose.Count);
                foreach (var file in m_filesToDelete)
                {
                    status.AppendFormat("Delete - {0}\r\n", file.SortedTree.BaseFile.FileName);
                    status.AppendFormat("Is Being Used {0}\r\n", file.IsBeingUsed);
                }

                foreach (var file in m_filesToDispose)
                {
                    status.AppendFormat("Dispose - {0} - {1}\r\n", file.SortedTree.FirstKey.ToString(), file.SortedTree.LastKey.ToString());
                    status.AppendFormat("Is Being Used {0}\r\n", file.IsBeingUsed);
                }

                status.AppendFormat("Files In Archive: {0} \r\n", m_fileSummaries.Count);
                foreach (var file in m_fileSummaries)
                {
                    if (file.IsEmpty)
                        status.AppendFormat("Empty File - Name:{0}\r\n", file.SortedTreeTable.BaseFile.FileName);
                    else
                        status.AppendFormat("{0} - {1} Name:{2}\r\n", file.FirstKey.ToString(), file.LastKey.ToString(), file.SortedTreeTable.BaseFile.FileName);

                }
            }
        }

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
        /// <param name="sortedTreehe partition to search for.</param>
        /// <returns></returns>
        public bool IsPartitionBeingUsed(SortedTreeTable<TKey, TValue> sortedTree)
        {
            lock (m_syncRoot)
            {
                foreach (ArchiveListSnapshot<TKey, TValue> resource in m_allSnapshots)
                {
                    ArchiveTableSummary<TKey, TValue>[] tables = resource.Tables;
                    if (tables != null)
                    {
                        for (int x = 0; x < tables.Length; x++)
                        {
                            ArchiveTableSummary<TKey, TValue> summary = tables[x];
                            if (summary != null && summary.SortedTreeTable == sortedTree)
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
                using (Editor edit = AcquireEditLock())
                {
                    foreach (ArchiveTableSummary<TKey, TValue> f in edit.ArchiveFiles)
                    {
                        f.SortedTreeTable.BaseFile.Dispose();
                    }
                }
                m_processRemovals.Dispose();
                m_disposed = true;
            }
        }

        private void m_processRemovals_OnRunWorker(object sender, ScheduledTaskEventArgs e)
        {
            lock (m_syncRoot)
            {
                for (int x = m_filesToDelete.Count - 1; x >= 0; x--)
                {
                    ArchiveListRemovalStatus<TKey, TValue> file = m_filesToDelete[x];
                    if (!file.IsBeingUsed)
                    {
                        file.SortedTree.BaseFile.Delete();
                        m_filesToDelete.RemoveAt(x);
                    }
                }

                for (int x = m_filesToDispose.Count - 1; x >= 0; x--)
                {
                    ArchiveListRemovalStatus<TKey, TValue> file = m_filesToDispose[x];
                    if (!file.IsBeingUsed)
                    {
                        file.SortedTree.BaseFile.Dispose();
                        m_filesToDispose.RemoveAt(x);
                    }
                }

                if (m_filesToDelete.Count > 0 || m_filesToDispose.Count > 0)
                    m_processRemovals.Start(1000);
            }
        }

        private void m_processRemovals_OnDispose(object sender, ScheduledTaskEventArgs e)
        {
            lock (m_syncRoot)
            {
                m_filesToDelete.ForEach(x => x.SortedTree.BaseFile.Delete());
                m_filesToDelete.Clear();

                m_filesToDispose.ForEach(x => x.SortedTree.BaseFile.Dispose());
                m_filesToDispose.Clear();
            }
        }

        void m_processRemovals_OnException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.EventLog.WriteEntry("MyEventSource", (e.ExceptionObject).ToString(), System.Diagnostics.EventLogEntryType.Error);
        }
    }
}