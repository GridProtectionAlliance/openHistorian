//******************************************************************************************************
//  ArchiveList`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  07/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GSF.Collections;
using GSF.Diagnostics;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;
using GSF.Threading;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Manages the complete list of archive resources and the 
    /// associated reading and writing that goes along with it.
    /// </summary>
    public partial class ArchiveList<TKey, TValue>
        : ArchiveList
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {

        private bool m_disposed;

        private readonly object m_syncRoot;

        /// <summary>
        /// Contains the list of all archives.
        /// </summary>
        private readonly SortedList<Guid, ArchiveTableSummary<TKey, TValue>> m_fileSummaries;

        /// <summary>
        /// Contains all of the active snapshots of the archive lists
        /// This is used for determining when resources are no longer in use.
        /// </summary>
        private readonly WeakList<ArchiveListSnapshot<TKey, TValue>> m_allSnapshots;

        /// <summary>
        /// The log engine of the ArchiveList. This is where pending deletions or disposals are kept.
        /// </summary>
        private readonly ArchiveListLog m_listLog;

        /// <summary>
        /// The scheduled task for removing items.
        /// </summary>
        private readonly ScheduledTask m_processRemovals;
        private readonly List<SortedTreeTable<TKey, TValue>> m_filesToDelete;
        private readonly List<SortedTreeTable<TKey, TValue>> m_filesToDispose;
        private ArchiveListSettings m_settings;

        /// <summary>
        /// Creates an ArchiveList
        /// </summary>
        /// <param name="parent">The parent of this class</param>
        /// <param name="settings">The settings for the archive list. Null will revert to a default setting.</param>
        public ArchiveList(LogSource parent, ArchiveListSettings settings = null)
            : base(parent)
        {
            if (settings == null)
                settings = new ArchiveListSettings();
            m_settings = settings.CloneReadonly();
            m_settings.Validate();

            m_syncRoot = new object();
            m_fileSummaries = new SortedList<Guid, ArchiveTableSummary<TKey, TValue>>();
            m_allSnapshots = new WeakList<ArchiveListSnapshot<TKey, TValue>>();
            m_listLog = new ArchiveListLog(m_settings.LogSettings);
            m_filesToDelete = new List<SortedTreeTable<TKey, TValue>>();
            m_filesToDispose = new List<SortedTreeTable<TKey, TValue>>();
            m_processRemovals = new ScheduledTask(ThreadingMode.DedicatedBackground);
            m_processRemovals.Running += ProcessRemovals_Running;
            m_processRemovals.Disposing += ProcessRemovals_Disposing;
            m_processRemovals.UnhandledException += ProcessRemovals_UnhandledException;

            AttachFileOrPath(m_settings.ImportPaths);

            HashSet<Guid> files = new HashSet<Guid>(m_filesToDelete.Select(x => x.ArchiveId));
            m_listLog.ClearCompletedLogs(files);
        }


        /// <summary>
        /// Attaches the supplied paths or files.
        /// </summary>
        /// <param name="paths">the path to file names or directories to enumerate.</param>
        /// <returns></returns>
        public override void AttachFileOrPath(IEnumerable<string> paths)
        {
            var attachedFiles = new List<string>();
            foreach (string path in paths)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        attachedFiles.Add(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        foreach (var extension in m_settings.ImportExtensions)
                        {
                            attachedFiles.AddRange(Directory.GetFiles(path, "*" + extension, SearchOption.AllDirectories));
                        }
                    }
                    else
                    {
                        Log.Publish(VerboseLevel.Warning, "File or path does not exist", path);
                    }

                }
                catch (Exception ex)
                {
                    Log.Publish(VerboseLevel.Error, "Unknown error occured while attaching paths", "Path: " + path, null, ex);
                }

            }
            LoadFiles(attachedFiles);
        }

        /// <summary>
        /// Loads the specified files into the archive list.
        /// </summary>
        /// <param name="archiveFiles"></param>
        public override void LoadFiles(IEnumerable<string> archiveFiles)
        {
            if (m_disposed)
                throw new Exception("Object is disposing");

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
                        if (m_listLog.ShouldBeDeleted(table.ArchiveId))
                        {
                            Log.Publish(VerboseLevel.Warning, "File being deleted", "The supplied file is being deleted because it was part of a previous rollover that completed but the server crashed before it was properly deleted." + file);
                            table.BaseFile.Delete();
                        }
                        else
                        {
                            loadedFiles.Add(table);
                        }
                    }
                    if (Log.ShouldPublishInfo)
                        Log.Publish(VerboseLevel.Information, "Loading Files", "Successfully opened: " + file);
                }
                catch (Exception ex)
                {
                    Log.Publish(VerboseLevel.Warning, "Loading Files", "Skipping Failed File: " + file, null, ex);
                }
            }

            using (var edit = AcquireEditLock())
            {
                if (m_disposed)
                {
                    loadedFiles.ForEach(x => x.Dispose());
                    throw new Exception("Object is disposing");
                }

                foreach (var file in loadedFiles)
                {
                    try
                    {
                        edit.Add(file);
                    }
                    catch (Exception ex)
                    {
                        Log.Publish(VerboseLevel.Warning, "Attaching File", "File already attached: " + file.ArchiveId, file.BaseFile.FilePath, ex);
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
                if (m_disposed)
                    throw new Exception("Object is disposing");


                resources = new ArchiveListSnapshot<TKey, TValue>(ReleaseClientResources, UpdateSnapshot);
                m_allSnapshots.Add(resources);
            }

            if (Log.ShouldPublishDebugLow)
                Log.Publish(VerboseLevel.DebugLow, "Created a client resource");

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

            if (Log.ShouldPublishDebugLow)
                Log.Publish(VerboseLevel.DebugLow, "Removed a client resource");
        }

        /// <summary>
        /// Invoked by <see cref="ArchiveListSnapshot{TKey,TValue}.UpdateSnapshot"/>.
        /// </summary>
        /// <param name="transaction"></param>
        private void UpdateSnapshot(ArchiveListSnapshot<TKey, TValue> transaction)
        {
            lock (m_syncRoot)
            {
                transaction.Tables = new ArchiveTableSummary<TKey, TValue>[m_fileSummaries.Count];
                m_fileSummaries.Values.CopyTo(transaction.Tables, 0);
            }

            if (Log.ShouldPublishDebugLow)
                Log.Publish(VerboseLevel.DebugLow, "Refreshed a client snapshot");
        }

        #endregion

        /// <summary>
        /// Appends the status of the files in the ArchiveList to the provided <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="status"></param>
        public override void GetFullStatus(StringBuilder status)
        {
            lock (m_syncRoot)
            {
                status.AppendFormat("Files Pending Deletion: {0} Disposal: {1}\r\n", m_filesToDelete.Count, m_filesToDispose.Count);
                foreach (var file in m_filesToDelete)
                {
                    status.AppendFormat("Delete - {0}\r\n", file.BaseFile.FilePath);
                    status.AppendFormat("Is Being Used {0}\r\n", InternalIsFileBeingUsed(file));
                }

                foreach (var file in m_filesToDispose)
                {
                    status.AppendFormat("Dispose - {0} - {1}\r\n", file.FirstKey.ToString(), file.LastKey.ToString());
                    status.AppendFormat("Is Being Used {0}\r\n", InternalIsFileBeingUsed(file));
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
        /// Gets a complete list of all archive files
        /// </summary>
        public override List<ArchiveDetails> GetAllAttachedFiles()
        {
            var rv = new List<ArchiveDetails>();
            lock (m_syncRoot)
            {
                foreach (var file in m_fileSummaries.Values)
                {
                    rv.Add(ArchiveDetails.Create(file));
                }
                return rv;
            }
        }

        /// <summary>
        /// Returns an <see cref="IDisposable"/> class that can be used to edit the contents of this list.
        /// WARNING: Make changes quickly and dispose the returned class.  All calls to this class are blocked while
        /// editing this class.
        /// </summary>
        /// <returns></returns>
        new public ArchiveListEditor<TKey, TValue> AcquireEditLock()
        {
            if (Log.ShouldPublishDebugLow)
                Log.Publish(VerboseLevel.DebugLow, "Acquiring an edit lock");
            return new Editor(this);
        }


        /// <summary>
        /// Necessary to provide shadow method of <see cref="ArchiveList.AcquireEditLock"/>
        /// </summary>
        /// <returns></returns>
        protected override ArchiveListEditor InternalAcquireEditLock()
        {
            return AcquireEditLock();
        }


        /// <summary>
        /// Queues the supplied file as a file that needs to be deleted.
        /// MUST be called from a synchronized context.
        /// </summary>
        /// <param name="file"></param>
        void AddFileToDelete(SortedTreeTable<TKey, TValue> file)
        {
            if (file.BaseFile.IsMemoryFile)
            {
                AddFileToDispose(file);
                return;
            }
            if (!InternalIsFileBeingUsed(file))
            {
                file.BaseFile.Delete();
                return;
            }
            m_listLog.AddFileToDelete(file.ArchiveId);
            m_filesToDelete.Add(file);
            m_processRemovals.Start(1000);
        }
        /// <summary>
        /// Queues the supplied file as one that needs to be disposed when no longer in use.
        /// MUST be called from a synchronized context.
        /// </summary>
        /// <param name="file"></param>
        void AddFileToDispose(SortedTreeTable<TKey, TValue> file)
        {
            if (!InternalIsFileBeingUsed(file))
            {
                file.BaseFile.Dispose();
                return;
            }
            m_filesToDispose.Add(file);
            m_processRemovals.Start(1000);
        }

        /// <summary>
        /// Determines if the provided file is currently in use
        /// by any resource. 
        /// </summary>
        /// <param name="sortedTree"> file to search for.</param>
        /// <returns></returns>
        public bool IsFileBeingUsed(SortedTreeTable<TKey, TValue> sortedTree)
        {
            lock (m_syncRoot)
            {
                return InternalIsFileBeingUsed(sortedTree);
            }
        }

        /// <summary>
        /// Gets if the specified file is being. 
        /// MUST be called from a synchronized context.
        /// </summary>
        /// <param name="sortedTree"></param>
        /// <returns></returns>
        bool InternalIsFileBeingUsed(SortedTreeTable<TKey, TValue> sortedTree)
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

        private void ProcessRemovals_Running(object sender, EventArgs<ScheduledTaskRunningReason> eventArgs)
        {
            bool wasAFileDeleted = false;
            lock (m_syncRoot)
            {
                for (int x = m_filesToDelete.Count - 1; x >= 0; x--)
                {
                    var file = m_filesToDelete[x];
                    if (!InternalIsFileBeingUsed(file))
                    {
                        wasAFileDeleted = true;
                        file.BaseFile.Delete();
                        m_filesToDelete.RemoveAt(x);
                    }
                }

                for (int x = m_filesToDispose.Count - 1; x >= 0; x--)
                {
                    var file = m_filesToDispose[x];
                    if (!InternalIsFileBeingUsed(file))
                    {
                        file.BaseFile.Dispose();
                        m_filesToDispose.RemoveAt(x);
                    }
                }

                if (wasAFileDeleted)
                {
                    HashSet<Guid> files = new HashSet<Guid>(m_filesToDelete.Select(x => x.ArchiveId));
                    m_listLog.ClearCompletedLogs(files);
                }

                if (m_filesToDelete.Count > 0 || m_filesToDispose.Count > 0)
                    m_processRemovals.Start(1000);
            }
        }

        private void ProcessRemovals_Disposing(object sender, EventArgs eventArgs)
        {
            lock (m_syncRoot)
            {
                //ToDo: Kick all clients.
                m_filesToDelete.ForEach(x => x.BaseFile.Delete());
                m_filesToDelete.Clear();

                m_filesToDispose.ForEach(x => x.BaseFile.Dispose());
                m_filesToDispose.Clear();
            }
        }

        void ProcessRemovals_UnhandledException(object sender, EventArgs<Exception> e)
        {
            Log.Publish(VerboseLevel.Error, "Unknown error encountered while removing archive files.", null, null, e.Argument);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="LogSourceBase"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed && disposing)
            {
                if (Log.ShouldPublishDebugNormal)
                    Log.Publish(VerboseLevel.DebugNormal, "Disposing");

                ReleaseClientResources();
                m_processRemovals.Dispose();
                m_listLog.Dispose();
                lock (m_syncRoot)
                {
                    foreach (ArchiveTableSummary<TKey, TValue> f in m_fileSummaries.Values)
                    {
                        f.SortedTreeTable.BaseFile.Dispose();
                    }
                }
                m_disposed = true;
            }
            base.Dispose(disposing);
        }

        void ReleaseClientResources()
        {
            List<ArchiveListSnapshot<TKey, TValue>> tablesInUse = new List<ArchiveListSnapshot<TKey, TValue>>();

            lock (m_syncRoot)
            {
                tablesInUse.AddRange(m_allSnapshots);
            }

            tablesInUse.ForEach((x) => x.Engine_BeginDropConnection());
            tablesInUse.ForEach((x) => x.Engine_EndDropConnection());
        }
    }
}