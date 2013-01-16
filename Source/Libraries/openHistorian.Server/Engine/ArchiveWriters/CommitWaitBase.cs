//******************************************************************************************************
//  CommitWaitBase.cs - Gbtc
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
using System.Threading;
using GSF.Threading;

//ToDo: clean up and implement this class.
namespace openHistorian.Engine.ArchiveWriters
{

    internal abstract class CommitWaitBase<T> : IDisposable
        where T : new()
    {

        protected ScheduledTask AsyncProcess;
        ManualResetEvent m_threadQuit;
        long m_lastCommitedSequenceNumber;
        long m_lastRolloverSequenceNumber;
        long m_latestSequenceId;
        bool m_threadHasQuit;
        bool m_disposed;
        T m_state;
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

        List<WaitingForCommit> m_pendingCommitRequests;

        protected readonly object SyncRoot;

        protected CommitWaitBase()
        {
            m_state = new T();
            AsyncProcess = new ScheduledTask(ProcessInsertingData, ProcessInsertingData);
            m_threadQuit = new ManualResetEvent(false);
            SyncRoot = new object();
            m_pendingCommitRequests = new List<WaitingForCommit>();
            m_lastCommitedSequenceNumber = -1;
            m_lastRolloverSequenceNumber = -1;
            m_latestSequenceId = -1;
        }

        void ProcessInsertingData()
        {
            ProcessInsertingData(m_state);
            ReleasePendingWaitLocks();
        }

        protected abstract void ProcessInsertingData(T state);

        protected bool ShouldQuit { get; private set; }

        protected bool ShouldRollover { get; set; }

        /// <summary>
        /// Gets the latest sequence number that this class knows about.
        /// </summary>
        public long CurrentSequenceNumber
        {
            get
            {
                lock (SyncRoot)
                {
                    return m_latestSequenceId;
                }
            }
        }

        /// <summary>
        /// Gets the last sequence number that has processed a commit stage.
        /// </summary>
        public long LastCommittedSequenceNumber
        {
            get
            {
                lock (SyncRoot)
                {
                    return m_lastCommitedSequenceNumber;
                }
            }
        }

        /// <summary>
        /// Gets the last sequence number that has processed the rollover stage.
        /// </summary>
        public long LastRolloverSequenceNumber
        {
            get
            {
                lock (SyncRoot)
                {
                    return m_lastRolloverSequenceNumber;
                }
            }
        }

        /// <summary>
        /// Determines if the following sequence number has been committed.
        /// </summary>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        public bool IsCommitted(long sequenceNumber)
        {
            lock (SyncRoot)
            {
                return (sequenceNumber <= m_lastCommitedSequenceNumber);
            }
        }

        /// <summary>
        /// Waits the provided sequence number to commit.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number to wait for</param>
        /// <param name="startImediately">Determines if a commit should be signaled.</param>
        /// <returns>true if the sequence number was committed. False if there was a timeout or the object disposed.</returns>
        public bool WaitForCommit(long sequenceNumber, bool startImediately)
        {
            WaitingForCommit waiting;
            lock (SyncRoot)
            {
                if (sequenceNumber <= m_lastCommitedSequenceNumber)
                    return true;
                if (m_threadHasQuit)
                    return false;
                waiting = new WaitingForCommit(sequenceNumber, false);
                m_pendingCommitRequests.Add(waiting);
            }
            if (startImediately)
                AsyncProcess.Start();

            waiting.Wait.WaitOne();
            return waiting.Successful;
        }

        /// <summary>
        /// Waits the provided sequence number to commit and rollover to the next stage.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number to wait for</param>
        /// <param name="startImediately">Determines if a commit should be signaled.</param>
        /// <returns>true if the sequence number was committed. False if there was a timeout or the object disposed.</returns>
        public bool WaitForRollover(long sequenceNumber, bool startImediately)
        {
            WaitingForCommit waiting;
            lock (SyncRoot)
            {
                if (sequenceNumber <= m_lastRolloverSequenceNumber)
                    return true;
                if (m_threadHasQuit)
                    return false;
                waiting = new WaitingForCommit(sequenceNumber, true);
                m_pendingCommitRequests.Add(waiting);
                if (startImediately)
                {
                    ShouldRollover = true;
                    AsyncProcess.Start();
                }

            }
            waiting.Wait.WaitOne();
            return waiting.Successful;
        }

        /// <summary>
        /// Forces a commit of the latest sequence number and waits for it to complete.
        /// </summary>
        public void Commit()
        {
            long sequenceId;
            lock (SyncRoot)
            {
                sequenceId = m_latestSequenceId;
            }

            WaitForCommit(sequenceId, true);
        }

        /// <summary>
        /// Forces a rollover of the latest sequence number and waits for it to complete.
        /// </summary>
        public void CommitAndRollover()
        {
            long sequenceId;
            lock (SyncRoot)
            {
                sequenceId = m_latestSequenceId;
            }

            WaitForRollover(sequenceId, true);
        }

        /// <summary>
        /// Method is to be called by 
        /// </summary>
        /// <param name="sequenceNumber"></param>
        protected void OnCommit(long sequenceNumber)
        {
            lock (SyncRoot)
            {
                m_lastCommitedSequenceNumber = sequenceNumber;
            }
        }

        protected void OnRollover(long sequenceNumber)
        {
            //Release any pending wait locks
            lock (SyncRoot)
            {
                m_lastRolloverSequenceNumber = sequenceNumber;
            }
        }

        protected void OnNewData(long sequenceNumber)
        {
            lock (SyncRoot)
            {
                m_latestSequenceId = sequenceNumber;
            }
        }

        protected void OnThreadExited()
        {
            lock (SyncRoot)
            {
                //If we need to quit, then signal all remaining waits as unsuccessful.
                foreach (var pending in m_pendingCommitRequests)
                {
                    pending.Successful = false;
                    pending.Wait.Set();
                }
                //m_pendingCommitRequests = null;
                m_threadHasQuit = true;
                m_threadQuit.Set();
            }
        }

        void ReleasePendingWaitLocks()
        {
            
            lock (SyncRoot)
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
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                lock (SyncRoot)
                {
                    m_disposed = true;
                    ShouldQuit = true;
                    AsyncProcess.Dispose();
                }
            }
        }
    }
}
