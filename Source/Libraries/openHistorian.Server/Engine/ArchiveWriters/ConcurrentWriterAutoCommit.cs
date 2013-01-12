//******************************************************************************************************
//  ConcurrentWriterAutoCommit.cs - Gbtc
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
//  5/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using openHistorian.Engine.Configuration;
using openHistorian.IO.Unmanaged;
using openHistorian.Archive;

namespace openHistorian.Engine.ArchiveWriters
{
    /// <summary>
    /// Responsible for getting data into the database. This class will prebuffer
    /// points and commit them in bulk operations.
    /// </summary>
    internal partial class ConcurrentWriterAutoCommit : IDisposable
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

        //ToDo: In the event that gobs of points are added, it might be quicker to presort the values.
        //ToDo: Build in some kind of auto slowdown method if the disk is getting bogged down.

        ArchiveWriterSettings m_settings;

        long m_lastCommitedSequenceNumber;
        long m_lastRolloverSequenceNumber;
        bool m_disposed;
        bool m_forceNewFile;
        bool m_forceQuit;
        bool m_forceCommit;
        bool m_threadHasQuit;

        ArchiveList m_archiveList;

        ConcurrentPointQueue m_concurrentPointQueue;

        Thread m_insertThread;

        ManualResetEvent m_waitTimer;

        Action<ArchiveFile,long> m_callbackFileComplete;

        object m_syncRoot;

        List<WaitingForCommit> m_pendingCommitRequests;

        /// <summary>
        /// Creates a new <see cref="ConcurrentWriterAutoCommit"/>.
        /// </summary>
        /// <param name="settings">The settings for this class.</param>
        /// <param name="archiveList">The list used to attach newly created file.</param>
        /// <param name="callbackFileComplete">Once a file is complete with this layer, this callback is invoked</param>
        public ConcurrentWriterAutoCommit(ArchiveWriterSettings settings, ArchiveList archiveList, Action<ArchiveFile,long> callbackFileComplete)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (archiveList == null)
                throw new ArgumentNullException("archiveList");
            if (callbackFileComplete == null)
                throw new ArgumentNullException("callbackFileComplete");

            m_pendingCommitRequests = new List<WaitingForCommit>();
            m_syncRoot = new object();
            m_lastCommitedSequenceNumber = -1;
            m_lastRolloverSequenceNumber = -1;

            m_callbackFileComplete = callbackFileComplete;

            m_settings = settings;

            m_archiveList = archiveList;

            m_concurrentPointQueue = new ConcurrentPointQueue();

            m_waitTimer = new ManualResetEvent(false);
            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
        }

        /// <summary>
        /// This is executed by a dedicated thread and moves data from the point queue to the database.
        /// </summary>
        void ProcessInsertingData()
        {
            ActiveFile activeFile = new ActiveFile(m_archiveList, m_callbackFileComplete);
            bool forcedQuit = false;
            while (!forcedQuit)
            {
                BinaryStream stream;
                int pointCount;
                long pendingSequenceNumber;
                bool forcedNewFile = false;
                bool forcedCommit = false;
                bool shouldCommit;
                bool shouldRollOver;

                if (m_waitTimer.WaitOne(1)) //implied memory barrior
                    m_waitTimer.Reset();


                if (m_forceCommit || m_forceQuit || m_forceNewFile) //reduces the lock contention.
                {
                    lock (m_syncRoot)
                    {
                        forcedNewFile = m_forceNewFile;
                        forcedQuit = m_forceQuit;
                        forcedCommit = m_forceCommit;

                        m_forceCommit = false;
                        m_forceNewFile = false;
                    }
                }

                m_concurrentPointQueue.GetPointBlock(out stream, out pointCount, out pendingSequenceNumber, forcedQuit);

                double waitForNewFile = (m_settings.NewFileOnInterval - activeFile.FileAge).TotalMilliseconds;
                double waitForNextCommitWindow = (m_settings.CommitOnInterval - activeFile.CommitAge).TotalMilliseconds;

                //If there is data to write then write it to the current archive.
                if (pointCount > 0)
                {
                    activeFile.CreateIfNotExists();
                    activeFile.Append(stream, pointCount);
                }

                shouldRollOver = waitForNewFile < 0 || forcedNewFile || forcedQuit;
                shouldCommit = waitForNextCommitWindow < 0 || forcedCommit || shouldRollOver;

                if (shouldRollOver)
                {
                    activeFile.RefreshAndRolloverFile(pendingSequenceNumber);
                }
                else
                {
                    if (shouldCommit)
                    {
                        activeFile.RefreshSnapshot();
                    }
                }

                //Release any pending wait locks
                if (shouldRollOver || shouldCommit)
                {
                    lock (m_syncRoot)
                    {
                        if (shouldRollOver)
                            m_lastRolloverSequenceNumber = pendingSequenceNumber;
                        if (shouldCommit)
                            m_lastCommitedSequenceNumber = pendingSequenceNumber;

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

        public long CurrentSequenceNumber
        {
            get
            {
                return m_concurrentPointQueue.SequenceId;
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

        /// <summary>
        /// Adds data to the input queue that will be committed at the user defined interval
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public long WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            return m_concurrentPointQueue.WriteData(key1, key2, value1, value2);
        }

        /// <summary>
        /// Moves data from the queue and inserts it into the current archive
        /// </summary>
        void SignalInitialInsert()
        {
            m_waitTimer.Set();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                StopExecution();
                m_concurrentPointQueue.Dispose();
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
                if (startImediately)
                    m_forceCommit = true;
            }
            if (startImediately)
                SignalInitialInsert();
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
                    m_forceCommit = true;
                }
            }
            if (startImediately)
                SignalInitialInsert();
            waiting.Wait.WaitOne();
            return waiting.Successful;
        }

        public void Commit()
        {
            long sequenceId = m_concurrentPointQueue.SequenceId;
            WaitForCommit(sequenceId, true);
        }

        public void CommitAndRollover()
        {
            long sequenceId = m_concurrentPointQueue.SequenceId;
            WaitForRollover(sequenceId, true);
        }

        public void StopExecution()
        {
            lock (m_syncRoot)
            {
                m_forceQuit = true;
            }
            SignalInitialInsert();
            m_insertThread.Join();
        }

        public void CommitNoWait()
        {
            lock (m_syncRoot)
            {
                m_forceCommit = true;
            }
            SignalInitialInsert();
        }

        public void CommitAndRolloverNoWait()
        {
            lock (m_syncRoot)
            {
                m_forceCommit = true;
                m_forceNewFile = true;
            }
            SignalInitialInsert();
        }

    }
}
