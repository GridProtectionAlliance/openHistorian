//******************************************************************************************************
//  CommitWaitHandles.cs - Gbtc
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
using System.Collections.Generic;
using System.Threading;

namespace openHistorian.Engine.ArchiveWriters
{
    public interface ISupportsWaitHandles : IDisposable
    {
        event Action<long> OnNewData;
        event Action<long> OnCommit;
        event Action<long> OnRollover;
        event Action OnThreadQuit;

        void ForceCommit();
        void ForceQuit();
        void ForceNewFile();
    }

    class CommitWaitHandles
    {
        ISupportsWaitHandles m_subClass;
        ManualResetEvent m_threadQuit;
        long m_lastCommitedSequenceNumber;
        long m_lastRolloverSequenceNumber;
        long m_latestSequenceId;
        bool m_threadHasQuit;

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

        object m_syncRoot;

        public CommitWaitHandles(ISupportsWaitHandles subClass)
        {
            m_subClass = subClass;
            m_threadQuit = new ManualResetEvent(false);
            m_syncRoot = new object();
            m_pendingCommitRequests = new List<WaitingForCommit>();
            m_lastCommitedSequenceNumber = -1;
            m_lastRolloverSequenceNumber = -1;
            m_latestSequenceId = -1;
            subClass.OnCommit += OnCommit;
            subClass.OnRollover += OnRollover;
            subClass.OnThreadQuit += OnThreadExited;
            subClass.OnNewData += OnNewData;
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
                m_subClass.ForceCommit();
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
                    m_subClass.ForceNewFile();
            }
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

        void OnCommit(long sequenceNumber)
        {
            lock (m_syncRoot)
            {
                m_lastCommitedSequenceNumber = sequenceNumber;
                ReleasePendingWaitLocks();
            }
        }

        void OnRollover(long sequenceNumber)
        {
            //Release any pending wait locks
            lock (m_syncRoot)
            {
                m_lastRolloverSequenceNumber = sequenceNumber;
                ReleasePendingWaitLocks();
            }
        }

        void OnNewData(long sequenceNumber)
        {
            lock (m_syncRoot)
            {
                m_latestSequenceId = sequenceNumber;
            }
        }

        void OnThreadExited()
        {
            lock (m_syncRoot)
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
}
