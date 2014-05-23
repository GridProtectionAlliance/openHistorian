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
using GSF.Collections;
using GSF.Diagnostics;
using GSF.Threading;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Manages the complete list of archive resources and the 
    /// associated reading and writing that goes along with it.
    /// </summary>
    public partial class ArchiveList<TKey, TValue>
        : IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        LogReporter m_logger;

        private bool m_disposed;

        private bool m_disposing;

        private readonly object m_syncRoot;

        /// <summary>
        /// Contains the list of all archives.
        /// </summary>
        private readonly SortedList<Guid, ArchiveTableSummary<TKey, TValue>> m_fileSummaries;

        /// <summary>
        /// Contains all archives that are locked.
        /// </summary>
        private readonly List<Guid> m_lockedFiles;

        /// <summary>
        /// Contains all of the active snapshots of the archive lists
        /// This is used for determining when resources are no longer in use.
        /// </summary>
        private readonly WeakList<ArchiveListSnapshot<TKey, TValue>> m_allSnapshots;

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
            m_syncRoot = new object();
            m_logger = Logger.Default.Register(this, "ArchiveList", "GSF.SortedTreeStore.Engine.ArchiveList");
            m_filesToDelete = new List<ArchiveListRemovalStatus<TKey, TValue>>();
            m_filesToDispose = new List<ArchiveListRemovalStatus<TKey, TValue>>();
            m_processRemovals = new ScheduledTask(ThreadingMode.DedicatedBackground);
            m_processRemovals.Running += ProcessRemovals_Running;
            m_processRemovals.Disposing += ProcessRemovals_Disposing;
            m_processRemovals.UnhandledException += ProcessRemovals_UnhandledException;
            m_lockedFiles = new List<Guid>();
            m_fileSummaries = new SortedList<Guid, ArchiveTableSummary<TKey, TValue>>();
            m_allSnapshots = new WeakList<ArchiveListSnapshot<TKey, TValue>>();
        }

        /// <summary>
        /// Loads the specified files into the archive list.
        /// </summary>
        /// <param name="archiveFiles"></param>
        public void LoadFiles(IEnumerable<string> archiveFiles)
        {
            var loadedFiles = new List<SortedTreeTable<TKey, TValue>>();

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
                        loadedFiles.Add(table);
                    }
                    if (m_logger.ReportInfo)
                        m_logger.LogMessage(VerboseLevel.Information, -1, "Loading Files", "Successfully opened: " + file);
                }
                catch (Exception ex)
                {
                    m_logger.LogMessage(VerboseLevel.Warning, -1, "Loading Files", "Skipping Failed File: " + file, null, ex);
                }
            }

            using (var edit = AcquireEditLock())
            {
                foreach (var file in loadedFiles)
                {
                    try
                    {
                        edit.Add(file, false);
                    }
                    catch (Exception ex)
                    {
                        m_logger.LogMessage(VerboseLevel.Warning, -1, "Attaching File", "File already attached: " + file.ArchiveId, null, ex);
                        file.BaseFile.Dispose();
                    }
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
            ArchiveListSnapshot<TKey, TValue> resources;

            lock (m_syncRoot)
            {
                if (m_disposing)
                    throw new Exception("Object is disposing");


                resources = new ArchiveListSnapshot<TKey, TValue>(ReleaseClientResources, AcquireSnapshot);
                m_allSnapshots.Add(resources);
            }

            if (m_logger.ReportDebug)
                m_logger.LogMessage(VerboseLevel.Debug, -1, "Created a client resource");

            return resources;

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
            if (m_logger.ReportDebug)
                m_logger.LogMessage(VerboseLevel.Debug, -1, "Removed a client resource");
        }

        /// <summary>
        /// Invoked by <see cref="ArchiveListSnapshot{TKey,TValue}.UpdateSnapshot"/>.
        /// </summary>
        /// <param name="transaction"></param>
        private void AcquireSnapshot(ArchiveListSnapshot<TKey, TValue> transaction)
        {
            lock (m_syncRoot)
            {
                transaction.Tables = new ArchiveTableSummary<TKey, TValue>[m_fileSummaries.Count];
                m_fileSummaries.Values.CopyTo(transaction.Tables, 0);
            }
            if (m_logger.ReportDebug)
                m_logger.LogMessage(VerboseLevel.Debug, -1, "Refreshed a client snapshot");
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
                    status.AppendFormat("Delete - {0}\r\n", file.SortedTree.BaseFile.FilePath);
                    status.AppendFormat("Is Being Used {0}\r\n", file.IsBeingUsed);
                }

                foreach (var file in m_filesToDispose)
                {
                    status.AppendFormat("Dispose - {0} - {1}\r\n", file.SortedTree.FirstKey.ToString(), file.SortedTree.LastKey.ToString());
                    status.AppendFormat("Is Being Used {0}\r\n", file.IsBeingUsed);
                }

                status.AppendFormat("Files In Archive: {0} \r\n", m_fileSummaries.Count);
                foreach (var file in m_fileSummaries.Values)
                {
                    if (file.IsEmpty)
                        status.AppendFormat("Empty File - Name:{0}\r\n", file.SortedTreeTable.BaseFile.FilePath);
                    else
                        status.AppendFormat("{0} - {1} Name:{2}\r\n", file.FirstKey.ToString(), file.LastKey.ToString(), file.SortedTreeTable.BaseFile.FilePath);

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
            if (m_logger.ReportDebug)
                m_logger.LogMessage(VerboseLevel.Debug, -1, "Acquiring a edit lock");
            return new Editor(this);
        }

        /// <summary>
        /// Determines if the provided partition file is currently in use
        /// by any resource. 
        /// </summary>
        /// <param name="sortedTree"> partition to search for.</param>
        /// <returns></returns>
        public bool IsPartitionBeingUsed(SortedTreeTable<TKey, TValue> sortedTree)
        {
            lock (m_syncRoot)
            {
                foreach (var snapshot in m_allSnapshots)
                {
                    ArchiveTableSummary<TKey, TValue>[] tables = snapshot.Tables;
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
                if (m_logger.ReportDebug)
                    m_logger.LogMessage(VerboseLevel.Debug, -1, "Disposing");
                ReleaseClientResources();
                m_processRemovals.Dispose();
                lock (m_syncRoot)
                {
                    foreach (ArchiveTableSummary<TKey, TValue> f in m_fileSummaries.Values)
                    {
                        f.SortedTreeTable.BaseFile.Dispose();
                    }
                }
                m_disposed = true;
            }
        }

        void ReleaseClientResources()
        {
            List<ArchiveListSnapshot<TKey, TValue>> tablesInUse = new List<ArchiveListSnapshot<TKey, TValue>>();

            lock (m_syncRoot)
            {
                m_disposing = true;
                tablesInUse.AddRange(m_allSnapshots);
            }

            tablesInUse.ForEach((x) => x.Engine_BeginDropConnection());
            tablesInUse.ForEach((x) => x.Engine_EndDropConnection());
        }

        private void ProcessRemovals_Running(object sender, EventArgs<ScheduledTaskRunningReason> eventArgs)
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

        private void ProcessRemovals_Disposing(object sender, EventArgs eventArgs)
        {
            lock (m_syncRoot)
            {
                m_filesToDelete.ForEach(x => x.SortedTree.BaseFile.Delete());
                m_filesToDelete.Clear();

                m_filesToDispose.ForEach(x => x.SortedTree.BaseFile.Dispose());
                m_filesToDispose.Clear();
            }
        }

        void ProcessRemovals_UnhandledException(object sender, EventArgs<Exception> e)
        {
            m_logger.LogMessage(VerboseLevel.Error, -1, "Unknown error encountered while removing archive files.", null, null, e.Argument);
        }
    }
}