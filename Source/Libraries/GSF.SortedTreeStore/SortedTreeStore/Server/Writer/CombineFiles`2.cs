//******************************************************************************************************
//  CombineFiles`2.cs - Gbtc
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
//  2/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using GSF.SortedTreeStore.Server.Reader;
using GSF.Threading;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Server.Writer
{
    /// <summary>
    /// A collection of settings for <see cref="CombineFiles{TKey,TValue}"/>.
    /// </summary>
    public class CombineFilesSettings<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        public string LogPath;
        public long TargetSize;
        public Guid MatchFlag;
        public ArchiveList<TKey, TValue> ArchiveList;
        public ArchiveInitializer<TKey, TValue> CreateNextStageFile;
    }

    /// <summary>
    /// Represents a series of stages that an archive file progresses through
    /// in order to properly condition the data.
    /// </summary>
    public class CombineFiles<TKey, TValue> : IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        public event EventHandler<EventArgs<Exception>> UnhandledException;

        /// <summary>
        /// Execute every 10 seconds
        /// </summary>
        public const int ExecuteInterval = 10000;
        private object m_syncRoot;
        private long m_targetSize;
        private bool m_stopped;
        private bool m_disposed;
        private string m_logPath;
        private ScheduledTask m_rolloverTask;
        private readonly ManualResetEvent m_rolloverComplete;

        Guid m_matchFlag;
        ArchiveInitializer<TKey, TValue> m_createNextStageFile;
        ArchiveList<TKey, TValue> m_archiveList;

        /// <summary>
        /// Creates a stage writer.
        /// </summary>
        /// <param name="settings">the settings for this stage</param>
        public CombineFiles(CombineFilesSettings<TKey, TValue> settings)
        {
            m_logPath = settings.LogPath;
            m_targetSize = settings.TargetSize;
            m_archiveList = settings.ArchiveList;
            m_matchFlag = settings.MatchFlag;
            m_createNextStageFile = settings.CreateNextStageFile;

            m_rolloverComplete = new ManualResetEvent(false);
            m_syncRoot = new object();
            m_rolloverTask = new ScheduledTask(ThreadingMode.DedicatedForeground, ThreadPriority.BelowNormal);
            m_rolloverTask.Running += OnExecute;
            m_rolloverTask.UnhandledException += OnException;

            m_rolloverTask.Start(ExecuteInterval);
        }

        void OnException(object sender, EventArgs<Exception> e)
        {
            if (UnhandledException != null)
                UnhandledException(sender, e);
        }

        private void OnExecute(object sender, EventArgs<ScheduledTaskRunningReason> e)
        {
            //The worker can be disposed either via the Stop() method or 
            //the Dispose() method.  If via the dispose method, then
            //don't do any cleanup.
            if (m_disposed && e.Argument == ScheduledTaskRunningReason.Disposing)
                return;

            //go ahead and schedule the next rollover since nothing
            //will happen until this function exits anyway.
            //if the task is disposing, the following line does nothing.
            m_rolloverTask.Start(ExecuteInterval);

            lock (m_syncRoot)
            {
                if (m_stopped)
                    return;

                using (var resource = m_archiveList.CreateNewClientResources())
                {
                    resource.UpdateSnapshot();

                    List<ArchiveTableSummary<TKey, TValue>> list = new List<ArchiveTableSummary<TKey, TValue>>();
                    List<Guid> listIds = new List<Guid>();

                    for (int x = 0; x < resource.Tables.Length; x++)
                    {
                        var table = resource.Tables[x];
                        if (table.SortedTreeTable.BaseFile.Snapshot.Header.Flags.Contains(m_matchFlag))
                        {
                            list.Add(table);
                            listIds.Add(table.FileId);
                        }
                        else
                        {
                            resource.Tables[x] = null;
                        }
                    }

                    bool shouldRollover = list.Count >= 30;

                    long size = 0;

                    for (int x = 0; x < list.Count; x++)
                    {
                        size += list[x].SortedTreeTable.BaseFile.ArchiveSize;
                        if (size > m_targetSize)
                        {
                            if (x != list.Count - 1)//If not the last entry
                                list.RemoveRange(x + 1, list.Count - x - 1);
                            break;
                        }
                    }
                    if (size > m_targetSize)
                        shouldRollover = true;

                    if (shouldRollover)
                    {
                        Guid newFileId = Guid.NewGuid();
                        string fileName = Path.Combine(m_logPath, newFileId.ToString() + ".RolloverLog");

                        RolloverLog<TKey, TValue> log = new RolloverLog<TKey, TValue>(fileName, listIds, newFileId, m_archiveList);

                        var reader = new UnionReader<TKey, TValue>(list);
                        var dest = m_createNextStageFile.CreateArchiveFile(log.StartKey, log.EndKey, newFileId);

                        try
                        {
                            using (var edit = dest.BeginEdit())
                            {
                                edit.AddPoints(reader);
                                edit.Commit();
                            }

                            resource.Dispose();

                            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
                            {
                                //Add the newly created file.
                                edit.Add(dest, isLocked: true);

                                foreach (var table in list)
                                {
                                    edit.TryRemoveAndDelete(table.FileId);
                                }
                            }
                        }
                        finally
                        {
                            reader.Cancel();
                        }

                        log.DeleteLog();

                    }

                    resource.Dispose();

                }

                m_rolloverComplete.Set();
            }
        }


        /// <summary>
        /// Stop all writing to this class.
        /// Once stopped, it cannot be resumed.
        /// All data is then immediately flushed to the output.
        /// This method calls Dispose()
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            lock (m_syncRoot)
            {
                m_stopped = true;
            }
            m_rolloverTask.Dispose();
            m_rolloverTask = null;
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                if (m_rolloverTask != null)
                    m_rolloverTask.Dispose();
                m_rolloverTask = null;
            }
        }
    }
}