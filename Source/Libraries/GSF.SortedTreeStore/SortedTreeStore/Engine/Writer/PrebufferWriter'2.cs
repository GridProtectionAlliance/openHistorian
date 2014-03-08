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
        public PointBuffer<TKey, TValue> Stream { get; private set; }
        /// <summary>
        /// The sequence number assoicated with the points in this buffer. T
        /// This is the id of the last point in this buffer.
        /// </summary>
        public long SequenceNumber { get; private set; }

        /// <summary>
        /// Creates a set of args
        /// </summary>
        /// <param name="stream">the stream to specify</param>
        /// <param name="sequenceNumber">the number to specify</param>
        public PrebufferRolloverArgs(PointBuffer<TKey, TValue> stream, long sequenceNumber)
        {
            Stream = stream;
            SequenceNumber = sequenceNumber;
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
        /// Occurs after a rollover operation has completed and provides the sequence number associated with
        /// the rollover.
        /// </summary>
        public event Action<long> RolloverComplete;

        /// <summary>
        /// An event handler that will raise any exceptions that go unhandled in the rollover process.
        /// </summary>
        public event UnhandledExceptionEventHandler Exception;

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
        private long m_sequenceId;

        /// <summary>
        /// The sequenceId that is currently being processed by the rollover thread.
        /// </summary>
        private long m_rollingOverCurrentSequenceId;

        private readonly object m_syncRoot;
        private readonly Action<PrebufferRolloverArgs<TKey, TValue>> m_onRollover;
        private ScheduledTask m_rolloverTask;
        private ManualResetEvent m_waitForRolloverToComplete = new ManualResetEvent(false);
        private PointBuffer<TKey, TValue> m_processingQueue;
        private PointBuffer<TKey, TValue> m_activeQueue;

        /// <summary>
        /// Creates a prestage writer.
        /// </summary>
        /// <param name="rolloverInterval">the maximum interval to wait before progressing to the next state</param>
        /// <param name="onRollover">delegate to call when a file is done with this stage.</param>
        public PrebufferWriter(int rolloverInterval, Action<PrebufferRolloverArgs<TKey, TValue>> onRollover)
        {
            if (rolloverInterval < 10 || rolloverInterval > 1000)
                throw new ArgumentOutOfRangeException("rolloverInterval", "Must be between 10ms and 1000ms");

            m_sequenceId = 0;
            m_syncRoot = new object();
            m_activeQueue = new PointBuffer<TKey, TValue>(10000);
            m_processingQueue = new PointBuffer<TKey, TValue>(10000);
            m_activeQueue.Clear();
            m_processingQueue.Clear();
            m_onRollover = onRollover;
            m_rolloverInterval = rolloverInterval;
            m_rolloverTask = new ScheduledTask(ThreadingMode.DedicatedForeground, ThreadPriority.AboveNormal);
            m_rolloverTask.OnEvent += ProcessRollover;
            m_rolloverTask.OnException += OnException;
            m_rolloverTask.Start(m_rolloverInterval);
        }

        void OnException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledExceptionEventHandler handler = Exception;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Gets the latest seqence id which is a sequential counter 
        /// based on the number of insert operations have occured.
        /// </summary>
        public long SequenceId
        {
            get
            {
                lock (m_syncRoot)
                {
                    return m_sequenceId;
                }
            }
        }

        /// <summary>
        /// Triggers a rollover if the provided sequence id has not yet been committed.
        /// </summary>
        /// <param name="sequenceId"></param>
        public void Commit(long sequenceId)
        {
            lock (m_syncRoot)
            {
                if (sequenceId > m_rollingOverCurrentSequenceId)
                    m_rolloverTask.Start();
            }
        }

        /// <summary>
        /// Writes the provided key/value to the prebuffer
        /// </summary>
        /// <param name="key">the key to write</param>
        /// <param name="value">the value to write</param>
        /// <returns>the sequence number identifying this point</returns>
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
                    goto TryAgainWithWait;
                }
                m_sequenceId++;
                sequenceId = m_sequenceId;
            }
            return sequenceId;


        TryAgainWithWait:
            m_waitForRolloverToComplete.WaitOne();
            goto TryAgain;
        }

        private void ProcessRollover(object sender, ScheduledTaskEventArgs e)
        {
            //the nature of how the ScheduledTask works 
            //gaurentees that this function will not be called concurrently

            //The worker can be disposed either via the Stop() method or 
            //the Dispose() method.  If via the dispose method, then
            //don't do any cleanup.
            if (m_disposed && e.IsDisposing)
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
                PointBuffer<TKey, TValue> stream = m_activeQueue;
                m_activeQueue = m_processingQueue;
                m_processingQueue = stream;

                m_activeQueue.Clear();

                args = new PrebufferRolloverArgs<TKey, TValue>(stream, m_sequenceId);
                m_waitForRolloverToComplete.Set();
                m_rollingOverCurrentSequenceId = m_sequenceId;

            }
            m_onRollover(args);
            OnRolloverComplete(args.SequenceNumber);
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

        /// <summary>
        /// Stop all writing to this class.
        /// Once stopped, it cannot be resumed.
        /// All data is then immediately flushed to the output.
        /// This method calls Dispose()
        /// </summary>
        /// <returns>the sequence number of the last point that needs to be written to the computer.</returns>
        public long Stop()
        {
            lock (m_syncRoot)
            {
                m_stopped = true;
            }
            m_rolloverTask.Dispose();
            m_rolloverTask = null;
            Dispose();
            return m_sequenceId;
        }

        void OnRolloverComplete(long obj)
        {
            Action<long> handler = RolloverComplete;
            if (handler != null)
                handler(obj);
        }

    }
}