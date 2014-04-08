//******************************************************************************************************
//  PrebufferWriter`2.cs - Gbtc
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
//  1/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;
using GSF.SortedTreeStore.Collection;
using GSF.Threading;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{
    /// <summary>
    /// A set of variables that are generated in the prebuffer stage that are provided to the onRollover 
    /// <see cref="Action"/> passed to the constructor of <see cref="PrebufferWriter{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The key</typeparam>
    /// <typeparam name="TValue">The value</typeparam>
    public class PrebufferRolloverArgs<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {

        /// <summary>
        /// The stream of points that need to be rolled over. 
        /// </summary>
        public SortedPointBuffer<TKey, TValue> Stream { get; private set; }

        /// <summary>
        /// The transaction id assoicated with the points in this buffer. 
        /// This is the id of the last point in this buffer.
        /// </summary>
        public long TransactionId { get; private set; }

        /// <summary>
        /// Creates a set of args
        /// </summary>
        /// <param name="stream">the stream to specify</param>
        /// <param name="transactionId">the number to specify</param>
        public PrebufferRolloverArgs(SortedPointBuffer<TKey, TValue> stream, long transactionId)
        {
            Stream = stream;
            TransactionId = transactionId;
        }
    }

    /// <summary>
    /// Where uncommitted data is collected before it is 
    /// inserted into an archive file in a bulk operation.
    /// </summary>
    /// <remarks>
    /// This class is thread safe
    /// </remarks>
    public class PrebufferWriter<TKey, TValue>
        : IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        
        /// <summary>
        /// An event handler that will raise any exceptions that go unhandled in the rollover process.
        /// </summary>
        public event EventHandler<EventArgs<Exception>> UnhandledException;

        /// <summary>
        /// Specifies that this class has been disposed.
        /// </summary>
        private bool m_disposed;

        /// <summary>
        /// Specifies that the prebuffer has been requested to stop processing data. 
        /// This occurs when gracefully shutting down the Engine, 
        /// allowing for all points to be rolled over and written to the underlying disk.
        /// </summary>
        private bool m_stopped;

        /// <summary>
        /// The interval after which data is rolled over.
        /// </summary>
        private readonly int m_rolloverInterval;

        /// <summary>
        /// The point sequence number assigned to points when they are added to the prebuffer.
        /// </summary>
        private long m_latestTransactionId;

        /// <summary>
        /// The Transaction Id that is currently being processed by the rollover thread.
        /// Its possible that it has not completed rolling over yet.
        /// </summary>
        private long m_currentTransactionIdRollingOver;

        private readonly object m_syncRoot;
        private readonly Action<PrebufferRolloverArgs<TKey, TValue>> m_onRollover;
        private ScheduledTask m_rolloverTask;
        ManualResetEvent m_waitForRolloverToComplete;
        private SortedPointBuffer<TKey, TValue> m_processingQueue;
        private SortedPointBuffer<TKey, TValue> m_activeQueue;

        /// <summary>
        /// Creates a prestage writer.
        /// </summary>
        /// <param name="rolloverInterval">the maximum interval to wait before progressing to the next state</param>
        /// <param name="onRollover">delegate to call when a file is done with this stage.</param>
        public PrebufferWriter(int rolloverInterval, Action<PrebufferRolloverArgs<TKey, TValue>> onRollover)
        {
            if (rolloverInterval < 10 || rolloverInterval > 1000)
                throw new ArgumentOutOfRangeException("rolloverInterval", "Must be between 10ms and 1000ms");
            if (onRollover == null)
                throw new ArgumentNullException("onRollover");

            m_latestTransactionId = 0;
            m_syncRoot = new object();
            m_activeQueue = new SortedPointBuffer<TKey, TValue>(10000);
            m_processingQueue = new SortedPointBuffer<TKey, TValue>(10000);
            m_activeQueue.Clear();
            m_processingQueue.Clear();
            m_onRollover = onRollover;
            m_rolloverInterval = rolloverInterval;
            m_waitForRolloverToComplete = new ManualResetEvent(false);
            m_rolloverTask = new ScheduledTask(ThreadingMode.DedicatedForeground, ThreadPriority.AboveNormal);
            m_rolloverTask.Running += m_rolloverTask_Running;
            m_rolloverTask.UnhandledException += m_rolloverTask_UnhandledException;
            m_rolloverTask.Start(m_rolloverInterval);
        }

        /// <summary>
        /// Gets the latest transaction id which is a sequential counter 
        /// based on the number of insert operations that have occured.
        /// </summary>
        public long LatestTransactionId
        {
            get
            {
                lock (m_syncRoot)
                {
                    return m_latestTransactionId;
                }
            }
        }

        /// <summary>
        /// Triggers a rollover if the provided transaction id has not yet been triggered.
        /// </summary>
        /// <param name="transactionId"></param>
        public void Commit(long transactionId)
        {
            lock (m_syncRoot)
            {
                if (transactionId > m_currentTransactionIdRollingOver)
                {
                    m_rolloverTask.Start();
                    m_waitForRolloverToComplete.Reset();
                }
            }
        }

        /// <summary>
        /// Writes the provided key/value to the prebuffer
        /// </summary>
        /// <param name="key">the key to write</param>
        /// <param name="value">the value to write</param>
        /// <returns>the transaction id identifying this point</returns>
        /// <remarks>Calls to this function are thread safe</remarks>
        public long Write(TKey key, TValue value)
        {
        TryAgain:

            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            long sequenceId;
            lock (m_syncRoot)
            {
                if (m_stopped)
                    throw new Exception("No new points can be added. Point queue has been stopped.");

                if (!m_activeQueue.TryEnqueue(key, value))
                {
                    m_rolloverTask.Start();
                    m_waitForRolloverToComplete.Reset();
                    goto TryAgainAfterWait;
                }
                m_latestTransactionId++;
                sequenceId = m_latestTransactionId;
            }
            return sequenceId;


        TryAgainAfterWait:
            m_waitForRolloverToComplete.WaitOne();
            goto TryAgain;
        }

        /// <summary>
        /// Processes the rollover of this file.
        /// </summary>
        private void m_rolloverTask_Running(object sender, EventArgs<ScheduledTaskRunningReason> e)
        {
            //the nature of how the ScheduledTask works 
            //gaurentees that this function will not be called concurrently

            //The worker can be disposed either via the Stop() method or 
            //the Dispose() method.  If via the dispose method, then
            //don't do any cleanup.
            if (m_disposed && e.Argument == ScheduledTaskRunningReason.Disposing)
            {
                m_waitForRolloverToComplete.Set();
                return;
            }

            //go ahead and schedule the next rollover since nothing
            //will happen until this function exits anyway.
            //if the task is disposing, the following line does nothing.
            m_rolloverTask.Start(m_rolloverInterval);

            PrebufferRolloverArgs<TKey, TValue> args;
            lock (m_syncRoot)
            {
                int count = m_activeQueue.Count;
                if (count == 0)
                {
                    m_waitForRolloverToComplete.Set();
                    return;
                }

                //Swap active and processing.
                SortedPointBuffer<TKey, TValue> stream = m_activeQueue;
                m_activeQueue = m_processingQueue;
                m_processingQueue = stream;

                m_activeQueue.Clear();

                stream.Sort();
                args = new PrebufferRolloverArgs<TKey, TValue>(stream, m_latestTransactionId);
                m_waitForRolloverToComplete.Set();
                m_currentTransactionIdRollingOver = m_latestTransactionId;

            }
            m_onRollover(args);
        }

        /// <summary>
        /// Stop all writing to this class.
        /// Once stopped, it cannot be resumed.
        /// All data is then immediately flushed to the output.
        /// This method calls Dispose()
        /// </summary>
        /// <returns>the transaction number of the last point that written</returns>
        public long Stop()
        {
            lock (m_syncRoot)
            {
                m_stopped = true;
            }
            m_rolloverTask.Dispose();
            m_rolloverTask = null;
            Dispose();
            return m_latestTransactionId;
        }

        /// <summary>
        /// Disposes the underlying queues contained in this class. 
        /// This method is not thread safe.
        /// It is assumed this will be called after <see cref="Stop"/>.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                try
                {
                    if (m_rolloverTask != null)
                        m_rolloverTask.Dispose();
                    if (m_processingQueue != null)
                        m_processingQueue.Dispose();
                    if (m_activeQueue != null)
                        m_activeQueue.Dispose();
                    if (m_waitForRolloverToComplete != null)
                        m_waitForRolloverToComplete.Dispose();
                }
                finally
                {
                    m_activeQueue = null;
                    m_processingQueue = null;
                    m_rolloverTask = null;
                    m_waitForRolloverToComplete = null;
                }
            }
        }
        
        void m_rolloverTask_UnhandledException(object sender, EventArgs<Exception> e)
        {

            if (UnhandledException != null)
                UnhandledException(sender, e);
        }

    }
}