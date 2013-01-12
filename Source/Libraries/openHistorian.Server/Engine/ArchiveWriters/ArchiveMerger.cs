//******************************************************************************************************
//  ArchiveMerger.cs - Gbtc
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
//  7/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using openHistorian.Archive;
using openHistorian.Engine.Configuration;

//ToDo: clean up and implement this class.
namespace openHistorian.Engine.ArchiveWriters
{
    /// <summary>
    /// Performs the required rollovers by reading partitions from the data list
    /// and combining them into a file of a later generation.
    /// </summary>
    internal class ArchiveMerger : CommitWaitBase<ArchiveMerger.StateVariables>
    {
        /// <summary>
        /// The settings for the rollover
        /// </summary>
        ArchiveRolloverSettings m_settings;
        /// <summary>
        /// The initialization settings.
        /// </summary>
        ArchiveInitializer m_archiveInitializer;

        ArchiveList m_archiveList;

        Queue<KeyValuePair<ArchiveFile, long>> m_filesToProcess;

        Action<ArchiveFile, long> m_callbackFileComplete;

        Action<ArchiveListRemovalStatus> m_archivesPendingDeletion;

        internal class StateVariables : EventArgs
        {
            public Stopwatch FileAge = new Stopwatch();
            public long LastCommittedSequenceNumber = -1;
            public ArchiveFile ActiveFile = null;
        }

        /// <summary>
        /// Creates a new <see cref="ArchiveMerger"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="archiveList">The list used to attach newly created file.</param>
        /// <param name="callbackFileComplete">Once a file is complete with this layer, this callback is invoked</param>
        /// <param name="archivesPendingDeletion">Where to pass archive files that are pending deletion</param>
        public ArchiveMerger(ArchiveRolloverSettings settings, ArchiveList archiveList, Action<ArchiveFile, long> callbackFileComplete, Action<ArchiveListRemovalStatus> archivesPendingDeletion)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (archiveList == null)
                throw new ArgumentNullException("archiveList");
            if (callbackFileComplete == null)
                throw new ArgumentNullException("callbackFileComplete");
            if (archivesPendingDeletion == null)
                throw new ArgumentNullException("archivesPendingDeletion");
            m_archivesPendingDeletion = archivesPendingDeletion;
            m_filesToProcess = new Queue<KeyValuePair<ArchiveFile, long>>();
            m_callbackFileComplete = callbackFileComplete;
            m_settings = settings;
            m_archiveList = archiveList;
            m_archiveInitializer = new ArchiveInitializer(settings.Initializer);
        }



        /// <summary>
        /// This is executed by a dedicated thread and moves data from the point queue to the database.
        /// </summary>
        protected override void ProcessInsertingData(object sender, StateVariables state)
        {
            bool forcedQuit;
            bool forcedNewFile;
            long pendingSequenceNumber = -1;
            ArchiveFile fileToCombine = null;

            //Get state information for this run.
            lock (SyncRoot)
            {
                forcedQuit = ShouldQuit;
                forcedNewFile = ShouldRollover;
                ShouldRollover = false; //Reset the value saying that it has been done.
                if (m_filesToProcess.Count > 0)
                {
                    var kvp = m_filesToProcess.Dequeue();
                    fileToCombine = kvp.Key;
                    pendingSequenceNumber = kvp.Value;
                }
            }

            if (fileToCombine != null)
            {
                //Create a new file if need be
                if (state.ActiveFile == null)
                {
                    state.FileAge.Restart();
                    var newFile = m_archiveInitializer.CreateArchiveFile();
                    using (var edit = m_archiveList.AcquireEditLock())
                    {
                        //Create a new file.
                        edit.Add(newFile, true);
                    }
                    state.ActiveFile = newFile;
                }

                ArchiveFileSummary summary = new ArchiveFileSummary(fileToCombine);
                ArchiveListRemovalStatus oldArchiveRemovalStatus;

                using (var src = summary.ActiveSnapshotInfo.CreateReadSnapshot())
                {
                    using (var fileEditor = state.ActiveFile.BeginEdit())
                    {
                        var reader = src.GetTreeScanner();
                        reader.SeekToKey(0, 0);

                        ulong value1, value2, key1, key2;
                        while (reader.GetNextKey(out key1, out key2, out value1, out value2))
                        {
                            fileEditor.AddPoint(key1, key2, value1, value2);
                        }

                        fileEditor.Commit();
                        using (var editor = m_archiveList.AcquireEditLock())
                        {
                            editor.RenewSnapshot(state.ActiveFile);
                            editor.Remove(fileToCombine, out oldArchiveRemovalStatus);
                        }
                    }
                }

                m_archivesPendingDeletion(oldArchiveRemovalStatus);
                OnCommit(pendingSequenceNumber);
                state.LastCommittedSequenceNumber = pendingSequenceNumber;
            }

            bool fileTooBig = state.ActiveFile != null && (state.ActiveFile.FileSize >= m_settings.NewFileOnSize);
            TimeSpan waitForNewFile = (m_settings.NewFileOnInterval - state.FileAge.Elapsed);

            bool shouldRollOver = waitForNewFile.TotalMilliseconds < 1 || forcedNewFile || forcedQuit || fileTooBig;

            if (shouldRollOver)
            {
                state.FileAge.Reset();
                if (state.ActiveFile != null)
                    m_callbackFileComplete(state.ActiveFile, state.LastCommittedSequenceNumber);
                state.ActiveFile = null;
                OnRollover(state.LastCommittedSequenceNumber);
            }

            //Release any pending wait locks
            lock (SyncRoot)
            {
                if (m_filesToProcess.Count > 0)
                    AsyncProcess.RunWorker();
                else if (forcedQuit)
                {
                    OnThreadExited();
                    AsyncProcess.Dispose();
                }
                else
                {
                    TimeSpan newWaitTime = (m_settings.NewFileOnInterval - state.FileAge.Elapsed);
                    AsyncProcess.RunWorkerAfterDelay(newWaitTime);
                }
            }
        }

        /// <summary>
        /// Adds the following item to the queue, requesting that it get processed immediately.
        /// </summary>
        /// <param name="archiveFile"></param>
        /// <param name="sequenceId"></param>
        public void ProcessArchive(ArchiveFile archiveFile, long sequenceId)
        {
            lock (SyncRoot)
            {
                m_filesToProcess.Enqueue(new KeyValuePair<ArchiveFile, long>(archiveFile, sequenceId));
                OnNewData(sequenceId);
            }
            AsyncProcess.RunWorker();
        }

       
    }
}
