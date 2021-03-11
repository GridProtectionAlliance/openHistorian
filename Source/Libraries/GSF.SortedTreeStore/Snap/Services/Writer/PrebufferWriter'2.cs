//******************************************************************************************************
//  PrebufferWriter`2.cs - Gbtc
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
//  01/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//  09/13/2014 - Steven E. Chisholm
//       Improved the thread safety of the class and incorporated logging.     
//
//******************************************************************************************************

using System;
using System.Threading;
using GSF.Diagnostics;
using GSF.Snap.Collection;
using GSF.Threading;

namespace GSF.Snap.Services.Writer
{
    /// <summary>
    /// Where uncommitted data is collected before it is 
    /// inserted into an archive file in a bulk operation.
    /// </summary>
    /// <remarks>
    /// This class is thread safe
    /// </remarks>
    public class PrebufferWriter<TKey, TValue>
        : DisposableLoggingClassBase
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        /// <summary>
        /// Specifies that this class has been disposed.
        /// </summary>
        private bool m_disposed;
        /// <summary>
        /// Gets if the rollover thread is currently working.
        /// </summary>
        private bool m_currentlyRollingOverFullQueue;
        /// <summary>
        /// Specifies that the prebuffer has been requested to stop processing data. 
        /// This occurs when gracefully shutting down the Engine, 
        /// allowing for all points to be rolled over and written to the underlying disk.
        /// </summary>
        private bool m_stopped;

        private readonly PrebufferWriterSettings m_settings;

        /// <summary>
        /// The point sequence number assigned to points when they are added to the prebuffer.
        /// </summary>
        private readonly AtomicInt64 m_latestTransactionId = new AtomicInt64();

        /// <summary>
        /// The Transaction Id that is currently being processed by the rollover thread.
        /// Its possible that it has not completed rolling over yet.
        /// </summary>
        private AtomicInt64 m_currentTransactionIdRollingOver = new AtomicInt64();

        private readonly object m_syncRoot;
        private readonly Action<PrebufferRolloverArgs<TKey, TValue>> m_onRollover;
        private readonly SafeManualResetEvent m_waitForEmptyActiveQueue;
        private readonly ScheduledTask m_rolloverTask;
        private SortedPointBuffer<TKey, TValue> m_processingQueue;
        private SortedPointBuffer<TKey, TValue> m_activeQueue;
        private readonly LogEventPublisher m_performanceLog;

        /// <summary>
        /// Creates a prestage writer.
        /// </summary>
        /// <param name="settings">The settings to use for this prebuffer writer</param>
        /// <param name="onRollover">delegate to call when a file is done with this stage.</param>
        public PrebufferWriter(PrebufferWriterSettings settings, Action<PrebufferRolloverArgs<TKey, TValue>> onRollover)
            : base(MessageClass.Framework)
        {
            if (settings is null)
                throw new ArgumentNullException("settings");
            if (onRollover is null)
                throw new ArgumentNullException("onRollover");

            m_settings = settings.CloneReadonly();
            m_settings.Validate();

            m_performanceLog = Log.RegisterEvent(MessageLevel.Info, MessageFlags.PerformanceIssue, "Queue is full", 0, MessageRate.PerSecond(1), 1);
            m_currentlyRollingOverFullQueue = false;
            m_latestTransactionId.Value = 0;
            m_syncRoot = new object();
            m_activeQueue = new SortedPointBuffer<TKey, TValue>(m_settings.MaximumPointCount, true);
            m_processingQueue = new SortedPointBuffer<TKey, TValue>(m_settings.MaximumPointCount, true);
            m_activeQueue.IsReadingMode = false;
            m_processingQueue.IsReadingMode = false;
            m_onRollover = onRollover;
            m_waitForEmptyActiveQueue = new SafeManualResetEvent(false);
            m_rolloverTask = new ScheduledTask(ThreadingMode.DedicatedForeground, ThreadPriority.AboveNormal);
            m_rolloverTask.Running += m_rolloverTask_Running;
            m_rolloverTask.UnhandledException += OnProcessException;
        }

        /// <summary>
        /// Gets the latest transaction id which is a sequential counter 
        /// based on the number of insert operations that have occured.
        /// </summary>
        public long LatestTransactionId => m_latestTransactionId;


        /// <summary>
        /// Triggers a rollover if the provided transaction id has not yet been triggered.
        /// This method does not block
        /// </summary>
        /// <param name="transactionId">the transaction id to execute the commit on.</param>
        public void Commit(long transactionId)
        {
            lock (m_syncRoot)
            {
                if (m_stopped)
                {
                    if (m_disposed)
                    {
                        Log.Publish(MessageLevel.Warning, "Disposed Object", "A call to Commit() occured after this class disposed");
                        return;
                    }
                    else
                    {
                        Log.Publish(MessageLevel.Warning, "Writer Stopped", "A call to Commit() occured after this class was stopped");
                        return;
                    }
                }
                if (transactionId > m_currentTransactionIdRollingOver)
                {
                    m_rolloverTask.Start();
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
            bool currentlyWorking;
            lock (m_syncRoot)
            {
                if (m_disposed)
                {
                    Log.Publish(MessageLevel.Warning, "Disposed Object", "A call to Write(TKey,TValue) occured after this class disposed");
                    return m_latestTransactionId;
                }
                if (m_stopped)
                {
                    Log.Publish(MessageLevel.Warning, "Writer Stopped", "A call to Write(TKey,TValue) occured after this class was stopped");
                    return m_latestTransactionId;
                }

                if (m_activeQueue.TryEnqueue(key, value))
                {
                    if (m_activeQueue.Count == 1)
                    {
                        m_rolloverTask.Start(m_settings.RolloverInterval);
                    }
                    if (m_activeQueue.Count == m_settings.RolloverPointCount)
                    {
                        m_rolloverTask.Start();
                    }
                    m_latestTransactionId.Value++;
                    return m_latestTransactionId;
                }
                currentlyWorking = m_currentlyRollingOverFullQueue;
                m_rolloverTask.Start();
                m_waitForEmptyActiveQueue.Reset();
            }

            if (currentlyWorking)
                m_performanceLog.Publish("Input Queue is processing at 100%, A long pause on the inputs is about to occur.");

            m_waitForEmptyActiveQueue.WaitOne();
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
                Log.Publish(MessageLevel.Info, "Rollover thread is Disposing");

                m_waitForEmptyActiveQueue.Dispose();
                return;
            }

            lock (m_syncRoot)
            {
                int count = m_activeQueue.Count;
                if (count == 0)
                {
                    m_waitForEmptyActiveQueue.Set();
                    return;
                }

                //Swap active and processing.
                SortedPointBuffer<TKey, TValue> swap = m_activeQueue;
                m_activeQueue = m_processingQueue;
                m_processingQueue = swap;
                m_activeQueue.IsReadingMode = false; //Should do nothing, but just to be sure.

                m_waitForEmptyActiveQueue.Set();
                m_currentTransactionIdRollingOver = m_latestTransactionId;
                m_currentlyRollingOverFullQueue = m_processingQueue.IsFull;
            }

            //ToDo: The current inner loop for inserting random data is the sorting process here. 
            //ToDo:  If the current speed isn't fast enough, this can be multithreaded to improve
            //ToDo:  the insert performance. However, at this time, the added complexity is
            //ToDo:  not worth it since write speeds are already blazing fast.
            try
            {
                m_processingQueue.IsReadingMode = true; //Very CPU intensive. This does a sort on the incoming measurements. Profiling shows that about 33% of the time is spent sorting elements.
                PrebufferRolloverArgs<TKey, TValue> args = new PrebufferRolloverArgs<TKey, TValue>(m_processingQueue, m_currentTransactionIdRollingOver);
                m_onRollover(args);
                m_processingQueue.IsReadingMode = false; //Clears the queue
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Critical, "Rollover process unhandled exception", "The rollover process threw an unhandled exception. There is likely data loss that will result from this exception", null, ex);
            }
            m_currentlyRollingOverFullQueue = false;

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
            Log.Publish(MessageLevel.Info, "Stop() called", "Write is stopping");

            lock (m_syncRoot)
            {
                m_stopped = true;
            }
            m_rolloverTask.Dispose(); //This method block until the worker runs one last time
            Dispose();
            return m_latestTransactionId;
        }

        /// <summary>
        /// Disposes the underlying queues contained in this class. 
        /// This method is not thread safe.
        /// It is assumed this will be called after <see cref="Stop"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed && disposing)
            {
                lock (m_syncRoot)
                {
                    if (m_disposed) //Prevents concurrent calls.
                        return;
                    m_stopped = true;
                    m_disposed = true;
                }
                try
                {
                    m_rolloverTask.Dispose();
                    m_waitForEmptyActiveQueue.Dispose();
                    m_processingQueue.Dispose();
                    m_activeQueue.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Publish(MessageLevel.Info, MessageFlags.BugReport, "Unhandled exception in the dispose process", null, null, ex);
                }
            }
            base.Dispose(disposing);
        }

        private void OnProcessException(object sender, EventArgs<Exception> e)
        {
            Log.Publish(MessageLevel.Critical, "Unhandled exception", "The worker thread threw an unhandled exception", null, e.Argument);
        }
    }
}