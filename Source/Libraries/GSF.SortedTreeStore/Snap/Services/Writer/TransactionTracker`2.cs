//******************************************************************************************************
//  TransactionTracker`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  03/07/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;

namespace GSF.Snap.Services.Writer
{
    /// <summary>
    /// Handles the transactions and any waits/notifications associated with transaction numbers.
    /// </summary>
    /// <typeparam name="TKey">The key</typeparam>
    /// <typeparam name="TValue">The value</typeparam>
    /// <remarks>
    /// Transaction IDs are long values, starting with zero. The reason behind this, even if 2 billion transactions
    /// could happen per second, it would still take over 100 years without an application restart to loop around. 
    /// Realistically a therotical peak would be 200 million transactions per second (An Interlocked.Increment).
    /// </remarks>
    public class TransactionTracker<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {

        /// <summary>
        /// An internal class created for each thread that is waiting for a transaction to committ.
        /// </summary>
        private class WaitForCommit : IDisposable
        {
            public long TransactionId { get; private set; }
            private ManualResetEvent m_resetEvent;
            public WaitForCommit(long transactionId)
            {
                TransactionId = transactionId;
                m_resetEvent = new ManualResetEvent(false);
            }

            public void Wait()
            {
                m_resetEvent.WaitOne();
            }

            public void Signal()
            {
                m_resetEvent.Set();
            }

            public void Dispose()
            {
                if (m_resetEvent != null)
                {
                    m_resetEvent.Dispose();
                    m_resetEvent = null;
                }
            }
        }

        private readonly object m_syncRoot;
        private long m_transactionSoftCommitted;
        private long m_transactionHardCommitted;
        private readonly PrebufferWriter<TKey, TValue> m_prebuffer;
        private readonly FirstStageWriter<TKey, TValue> m_firstStageWriter;
        private readonly List<WaitForCommit> m_waitingForSoftCommit;
        private readonly List<WaitForCommit> m_waitingForHardCommit;

        /// <summary>
        /// Creates a new transaction tracker that monitors the provided buffers.
        /// </summary>
        /// <param name="prebuffer"></param>
        /// <param name="firstStageWriter"></param>
        public TransactionTracker(PrebufferWriter<TKey, TValue> prebuffer, FirstStageWriter<TKey, TValue> firstStageWriter)
        {
            m_waitingForHardCommit = new List<WaitForCommit>();
            m_waitingForSoftCommit = new List<WaitForCommit>();

            m_syncRoot = new object();
            m_transactionSoftCommitted = 0;
            m_transactionHardCommitted = 0;
            m_prebuffer = prebuffer;
            m_firstStageWriter = firstStageWriter;
            m_firstStageWriter.RolloverComplete += TransactionSoftCommitted;
            m_firstStageWriter.SequenceNumberCommitted += TransactionHardCommitted;
        }

        /// <summary>
        /// Event handler.
        /// </summary>
        /// <param name="transactionId"></param>
        private void TransactionSoftCommitted(long transactionId)
        {
            lock (m_syncRoot)
            {
                m_transactionSoftCommitted = transactionId;
                for (int x = m_waitingForSoftCommit.Count - 1; x > 0; x--)
                {
                    WaitForCommit waiting = m_waitingForSoftCommit[x];
                    if (transactionId >= waiting.TransactionId)
                    {
                        waiting.Signal();
                        m_waitingForSoftCommit.RemoveAt(x);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler.
        /// </summary>
        /// <param name="transactionId"></param>
        private void TransactionHardCommitted(long transactionId)
        {
            lock (m_syncRoot)
            {
                m_transactionHardCommitted = transactionId;
                for (int x = m_waitingForHardCommit.Count - 1; x > 0; x--)
                {
                    WaitForCommit waiting = m_waitingForHardCommit[x];
                    if (transactionId >= waiting.TransactionId)
                    {
                        waiting.Signal();
                        m_waitingForHardCommit.RemoveAt(x);
                    }
                }
            }
        }

        /// <summary>
        /// Wait for the specified transaction to commit to memory.
        /// </summary>
        /// <param name="transactionId"></param>
        public void WaitForSoftCommit(long transactionId)
        {
            lock (m_syncRoot)
            {
                if (m_transactionSoftCommitted > transactionId)
                    return;
            }
            m_prebuffer.Commit(transactionId);

            using (WaitForCommit wait = new WaitForCommit(transactionId))
            {
                lock (m_syncRoot)
                {
                    if (m_transactionSoftCommitted > transactionId)
                        return;
                    m_waitingForSoftCommit.Add(wait);
                }
                wait.Wait();
            }
        }

        /// <summary>
        /// Waits for the specified transaction to commit to the disk.
        /// </summary>
        /// <param name="transactionId"></param>
        public void WaitForHardCommit(long transactionId)
        {
            bool triggerSoft = false;
            lock (m_syncRoot)
            {
                if (m_transactionHardCommitted > transactionId)
                    return;
                if (m_transactionSoftCommitted > transactionId)
                    triggerSoft = true;
            }
            if (triggerSoft)
                m_prebuffer.Commit(transactionId);
            m_firstStageWriter.Commit(transactionId);


            using (WaitForCommit wait = new WaitForCommit(transactionId))
            {
                lock (m_syncRoot)
                {
                    if (m_transactionHardCommitted > transactionId)
                        return;
                    m_waitingForHardCommit.Add(wait);
                }
                wait.Wait();
            }

        }
    }
}
