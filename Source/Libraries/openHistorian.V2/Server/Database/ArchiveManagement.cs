//******************************************************************************************************
//  ArchiveManagement.cs - Gbtc
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
//  7/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Performs the required rollovers by reading partitions from the data list
    /// and combining them into a file of a later generation.
    /// </summary>
    public class ArchiveManagement
    {
        /// <summary>
        /// Provides a way to block a thread until data has been committed to the archive writer.
        /// </summary>
        class WaitingForCommit
        {
            /// <summary>
            /// The wait handle to signaled
            /// </summary>
            public ManualResetEvent Wait { get; private set; }
            /// <summary>
            /// The desired sequence number to wait for.
            /// </summary>
            public long SequenceNumberToWaitFor { get; private set; }
            /// <summary>
            /// Determines if the wait is for a commit, or for a successful rollover.
            /// </summary>
            public bool WaitForRollover { get; private set; }
            /// <summary>
            /// Value will be set to true if the condition was met.
            /// False will only occur if the writer is closed before conditions were met.
            /// </summary>
            public bool Successful { get; set; }

            public WaitingForCommit(long seqenceNumber, bool waitForRollover)
            {
                Successful = false;
                WaitForRollover = waitForRollover;
                SequenceNumberToWaitFor = seqenceNumber;
                Wait = new ManualResetEvent(false);
            }
        }

        ArchiveRolloverSettings m_settings;

        ArchiveInitializer m_archiveInitializer;

        bool m_disposed;

        ArchiveList m_archiveList;

        Thread m_insertThread;

        ManualResetEvent m_waitTimer;

        ConcurrentQueue<KeyValuePair<ArchiveFile, long>> m_filesToProcess;
        Action<ArchiveFile, long> m_callbackFileComplete;

        bool m_forceNewFile;
        bool m_forceQuit;
        object m_syncRoot;
        long m_lastCommitedSequenceNumber;
        long m_lastRolloverSequenceNumber;
        List<WaitingForCommit> m_pendingCommitRequests;
        bool m_threadHasQuit;
        Action<ArchiveListRemovalStatus> m_archivesPendingDeletion;
        long m_latestSequenceId;

        /// <summary>
        /// Creates a new <see cref="ArchiveManagement"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="archiveList">The list used to attach newly created file.</param>
        /// <param name="callbackFileComplete">Once a file is complete with this layer, this callback is invoked</param>
        public ArchiveManagement(ArchiveRolloverSettings settings, ArchiveList archiveList, Action<ArchiveFile, long> callbackFileComplete, Action<ArchiveListRemovalStatus> archivesPendingDeletion)
        {
            m_archivesPendingDeletion = archivesPendingDeletion;
            m_pendingCommitRequests = new List<WaitingForCommit>();
            m_syncRoot = new object();
            m_lastCommitedSequenceNumber = -1;
            m_lastRolloverSequenceNumber = -1;
            m_latestSequenceId = -1;


            m_filesToProcess = new ConcurrentQueue<KeyValuePair<ArchiveFile, long>>();
            m_callbackFileComplete = callbackFileComplete;
            m_settings = settings;

            m_archiveList = archiveList;
            m_archiveInitializer = new ArchiveInitializer(settings.Initializer);

            m_waitTimer = new ManualResetEvent(false);
            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
        }

        /// <summary>
        /// This is executed by a dedicated thread and moves data from the point queue to the database.
        /// </summary>
        void ProcessInsertingData()
        {
            Stopwatch fileAge = new Stopwatch();
            ArchiveFile activeFile = null;

            bool forcedQuit = false;
            while (!forcedQuit)
            {
                if (m_filesToProcess.Count == 0)
                {
                    if (m_waitTimer.WaitOne(10))
                        m_waitTimer.Reset();
                }

                bool shouldRollOver;
                bool forcedNewFile = false;
                if (m_forceQuit || m_forceNewFile) //reduces the lock contention.
                {
                    lock (m_syncRoot)
                    {
                        forcedNewFile = m_forceNewFile;
                        forcedQuit = m_forceQuit;
                        m_forceNewFile = false;
                    }
                }

                KeyValuePair<ArchiveFile, long> nextJob;
                if (m_filesToProcess.TryDequeue(out nextJob))
                {
                    ArchiveFile fileToCombine = nextJob.Key;
                    long pendingSequenceNumber = nextJob.Value;

                    //Create a new file if need be
                    if (activeFile == null)
                    {
                        fileAge.Start();
                        var newFile = m_archiveInitializer.CreateArchiveFile();
                        using (var edit = m_archiveList.AcquireEditLock())
                        {
                            //Create a new file.
                            edit.Add(newFile, true);
                        }
                        activeFile = newFile;
                    }

                    ArchiveFileSummary summary = new ArchiveFileSummary(fileToCombine);
                    ArchiveListRemovalStatus oldArchiveRemovalStatus;

                    using (var src = summary.ActiveSnapshot.OpenInstance())
                    {
                        using (var fileEditor = activeFile.BeginEdit())
                        {
                            var reader = src.GetDataRange();
                            reader.SeekToKey(0, 0);

                            ulong value1, value2, key1, key2;
                            while (reader.GetNextKey(out key1, out key2, out value1, out value2))
                            {
                                fileEditor.AddPoint(key1, key2, value1, value2);
                            }

                            fileEditor.Commit();
                            using (var editor = m_archiveList.AcquireEditLock())
                            {
                                editor.RenewSnapshot(activeFile);
                                editor.Remove(fileToCombine, out oldArchiveRemovalStatus);
                            }
                        }
                    }
                    m_archivesPendingDeletion(oldArchiveRemovalStatus);
                    lock (m_syncRoot)
                    {
                        m_lastCommitedSequenceNumber = pendingSequenceNumber;
                    }
                }


                bool fileTooBig = activeFile != null && (activeFile.FileSize >= m_settings.NewFileOnSize);
                double waitForNewFile = (m_settings.NewFileOnInterval - fileAge.Elapsed).TotalMilliseconds;

                shouldRollOver = waitForNewFile < 0 || forcedNewFile || forcedQuit || fileTooBig;

                if (shouldRollOver)
                {
                    //Create a new file if need be
                    if (fileTooBig)
                    {
                        fileAge.Reset();
                        if (activeFile != null)
                            m_callbackFileComplete(activeFile, m_lastCommitedSequenceNumber);
                        activeFile = null;
                    }
                }

                //Release any pending wait locks
                lock (m_syncRoot)
                {
                    if (shouldRollOver)
                        m_lastRolloverSequenceNumber = m_lastCommitedSequenceNumber;

                    ReleasePendingWaitLocks();

                    //If we need to quit, then signal all remaining waits as unsuccessful.
                    if (forcedQuit)
                    {
                        m_threadHasQuit = forcedQuit;
                        foreach (var pending in m_pendingCommitRequests)
                        {
                            pending.Successful = false;
                            pending.Wait.Set();
                        }
                        m_pendingCommitRequests = null;
                    }
                }
            }
        }

        /// <summary>
        /// Helper function for <see cref="ProcessInsertingData"/>. Only to be called within a
        /// lock of <see cref="m_syncRoot"/>.
        /// </summary>
        void ReleasePendingWaitLocks()
        {
            int x = m_pendingCommitRequests.Count - 1;
            while (x >= 0)
            {
                var pending = m_pendingCommitRequests[x];
                if (pending.WaitForRollover)
                {
                    if (pending.SequenceNumberToWaitFor <= m_lastRolloverSequenceNumber)
                    {
                        pending.Successful = true;
                        pending.Wait.Set();
                        m_pendingCommitRequests.RemoveAt(x);
                    }
                }
                else
                {
                    if (pending.SequenceNumberToWaitFor <= m_lastCommitedSequenceNumber)
                    {
                        pending.Successful = true;
                        pending.Wait.Set();
                        m_pendingCommitRequests.RemoveAt(x);
                    }
                }
                x--;
            }
        }

        public long LastCommittedSequenceNumber
        {
            get
            {
                lock (m_syncRoot)
                {
                    return m_lastCommitedSequenceNumber;
                }
            }
        }
        public long LastRolloverSequenceNumber
        {
            get
            {
                lock (m_syncRoot)
                {
                    return m_lastRolloverSequenceNumber;
                }
            }
        }

        public void ProcessArchive(ArchiveFile archiveFile, long sequenceId)
        {
            m_filesToProcess.Enqueue(new KeyValuePair<ArchiveFile, long>(archiveFile, sequenceId));
            lock (m_syncRoot)
            {
                m_latestSequenceId = sequenceId;
            }
            SignalProcessRollover();
        }

        /// <summary>
        /// Moves data from the queue and inserts it into Generation 0's Archive.
        /// </summary>
        public void SignalProcessRollover()
        {
            m_waitTimer.Set();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                StopExecution();
                m_disposed = true;
            }
        }

        public bool IsCommitted(long sequenceNumber)
        {
            lock (m_syncRoot)
            {
                return (sequenceNumber <= m_lastCommitedSequenceNumber);
            }
        }

        public bool WaitForCommit(long sequenceId, bool startImediately)
        {
            WaitingForCommit waiting;
            lock (m_syncRoot)
            {
                if (sequenceId <= m_lastCommitedSequenceNumber)
                    return true;
                if (m_threadHasQuit)
                    return false;
                waiting = new WaitingForCommit(sequenceId, false);
                m_pendingCommitRequests.Add(waiting);
            }
            if (startImediately)
                SignalProcessRollover();
            waiting.Wait.WaitOne();
            return waiting.Successful;
        }

        public bool WaitForRollover(long sequenceId, bool startImediately)
        {
            WaitingForCommit waiting;
            lock (m_syncRoot)
            {
                if (sequenceId <= m_lastRolloverSequenceNumber)
                    return true;
                if (m_threadHasQuit)
                    return false;
                waiting = new WaitingForCommit(sequenceId, true);
                m_pendingCommitRequests.Add(waiting);
                if (startImediately)
                {
                    m_forceNewFile = true;
                }
            }
            if (startImediately)
                SignalProcessRollover();
            waiting.Wait.WaitOne();
            return waiting.Successful;
        }

        public void Commit()
        {
            long sequenceId;
            lock (m_syncRoot)
            {
                sequenceId = m_latestSequenceId;
            }

            WaitForCommit(sequenceId, true);
        }

        public void CommitAndRollover()
        {
            long sequenceId;
            lock (m_syncRoot)
            {
                sequenceId = m_latestSequenceId;
            }

            WaitForRollover(sequenceId, true);
        }

        public void StopExecution()
        {
            lock (m_syncRoot)
            {
                m_forceQuit = true;
            }
            SignalProcessRollover();
            m_insertThread.Join();
        }

        public void CommitNoWait()
        {
            SignalProcessRollover();
        }

        public void CommitAndRolloverNoWait()
        {
            lock (m_syncRoot)
            {
                m_forceNewFile = true;
            }
            SignalProcessRollover();
        }
    }
}
