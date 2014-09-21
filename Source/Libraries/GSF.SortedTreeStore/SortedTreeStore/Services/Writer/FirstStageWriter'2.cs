//******************************************************************************************************
//  FirstStageWriter`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  02/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;
using GSF.Diagnostics;
using GSF.Threading;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// Handles how data is initially taken from prestage chunks and serialized to the disk.
    /// </summary>
    public class FirstStageWriter<TKey, TValue>
        : LogSourceBase
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        /// <summary>
        /// Event that notifies that a certain sequence number has been committed.
        /// </summary>
        public event Action<long> SequenceNumberCommitted;

        /// <summary>
        /// Occurs after a rollover operation has completed and provides the sequence number associated with
        /// the rollover.
        /// </summary>
        public event Action<long> RolloverComplete;

        private bool m_stopped;
        private bool m_disposed;
        private int m_rolloverInterval;
        private int m_rolloverSizeMb;
        private int m_maximumAllowedMb;
        private readonly AtomicInt64 m_lastCommitedSequenceNumber = new AtomicInt64();
        private readonly AtomicInt64 m_lastRolledOverSequenceNumber = new AtomicInt64();

        private ScheduledTask m_rolloverTask;
        private readonly object m_syncRoot;
        private IncrementalStagingFile<TKey, TValue> m_activeStagingFile;
        private IncrementalStagingFile<TKey, TValue> m_workingStagingFile;
        private readonly SafeManualResetEvent m_rolloverComplete;

        /// <summary>
        /// Creates a stage writer.
        /// </summary>
        public FirstStageWriter(IncrementalStagingFile<TKey, TValue> incrementalStagingFile, FirstStageWriterSettings settings, LogSource parent)
            : base(parent)
        {
            if (incrementalStagingFile == null)
                throw new ArgumentNullException("incrementalStagingFile");
            if (settings == null)
                throw new ArgumentNullException("settings");

            m_rolloverSizeMb = settings.RolloverSizeMb;
            m_maximumAllowedMb = settings.MaximumAllowedMb;
            m_rolloverInterval = settings.RolloverInterval;

            m_rolloverComplete = new SafeManualResetEvent(false);
            m_activeStagingFile = incrementalStagingFile;
            m_workingStagingFile = incrementalStagingFile.Clone();

            m_syncRoot = new object();
            m_rolloverTask = new ScheduledTask(ThreadingMode.DedicatedForeground, ThreadPriority.Normal);
            m_rolloverTask.Running += RolloverTask_Running;
            m_rolloverTask.UnhandledException += OnProcessException;
        }

        /// <summary>
        /// The number of milliseconds before data is flushed to the disk. 
        /// </summary>
        /// <remarks>
        /// Must be between 1,000 ms and 60,000 ms.
        /// </remarks>
        public int RolloverInterval
        {
            get
            {
                return m_rolloverInterval;
            }
            set
            {
                if (value < 1000 || value > 60000)
                    throw new ArgumentOutOfRangeException("value", "Must be between 1000ms and 60000ms");
                m_rolloverInterval = value;
            }
        }

        /// <summary>
        /// The size at which a rollover will be signaled
        /// </summary>
        /// <remarks>
        /// Must be at least 1MB. Upper Limit should be Memory Constrained, but not larger than 1024MB.
        /// </remarks>
        public int RolloverSizeMb
        {
            get
            {
                return m_rolloverSizeMb;
            }
            set
            {
                if (value < 1 || value > 1024)
                    throw new ArgumentOutOfRangeException("value", "Must be between 1 and 1024MB");
                m_rolloverSizeMb = value;
            }
        }

        /// <summary>
        /// The size after which the incoming write queue will pause
        /// to wait for rollovers to complete.
        /// </summary>
        /// <remarks>
        /// It is recommended to make this value larger than <see cref="RolloverSizeMb"/>.
        /// If this value is smaller than <see cref="RolloverSizeMb"/> then <see cref="RolloverSizeMb"/> will be used.
        /// Must be at least 1MB. Upper Limit should be Memory Constrained, but not larger than 1024MB.
        /// </remarks>
        public int MaximumAllowedMb
        {
            get
            {
                return Math.Max(m_rolloverSizeMb, m_maximumAllowedMb);
            }
            set
            {
                if (value < 1 || value > 1024)
                    throw new ArgumentOutOfRangeException("value", "Must be between 1 and 1024MB");
                m_maximumAllowedMb = value;
            }
        }

        /// <summary>
        /// Gets the settings for this class
        /// </summary>
        /// <returns></returns>
        public FirstStageWriterSettings GetSettings()
        {
            return new FirstStageWriterSettings
            {
                RolloverInterval = m_rolloverInterval,
                RolloverSizeMb = m_rolloverSizeMb,
                MaximumAllowedMb = m_maximumAllowedMb
            };
        }

        /// <summary>
        /// Appends this data to this stage. Also queues up for deletion if necessary.
        /// </summary>
        /// <param name="args">arguments handed to this class from either the 
        /// PrestageWriter or another StageWriter of a previous generation</param>
        /// <remarks>
        /// This method must be called in a single threaded manner.
        /// </remarks>
        public void AppendData(PrebufferRolloverArgs<TKey, TValue> args)
        {
            if (m_stopped)
            {
                if (Log.ShouldPublishInfo)
                    Log.Publish(VerboseLevel.Information, "No new points can be added. Point queue has been stopped. Data in rollover will be lost");
                return;
            }
            if (m_disposed)
            {
                if (Log.ShouldPublishInfo)
                    Log.Publish(VerboseLevel.Information, "First stage writer has been disposed. Data in rollover will be lost");
                return;
            }

            bool shouldWait = false;
            //If there is data to write then write it to the current archive.
            lock (m_syncRoot)
            {
                if (m_stopped)
                {
                    if (Log.ShouldPublishInfo)
                        Log.Publish(VerboseLevel.Information, "No new points can be added. Point queue has been stopped. Data in rollover will be lost");
                    return;
                }
                if (m_disposed)
                {
                    if (Log.ShouldPublishInfo)
                        Log.Publish(VerboseLevel.Information, "First stage writer has been disposed. Data in rollover will be lost");
                    return;
                }

                m_activeStagingFile.Append(args.Stream);
                m_lastCommitedSequenceNumber.Value = args.TransactionId;

                long currentSizeMb = m_activeStagingFile.Size >> 20;
                if (currentSizeMb > MaximumAllowedMb)
                {
                    shouldWait = true;
                    m_rolloverTask.Start();
                    m_rolloverComplete.Reset();
                }
                else if (currentSizeMb > RolloverSizeMb)
                {
                    m_rolloverTask.Start();
                }
                else
                {
                    m_rolloverTask.Start(m_rolloverInterval);
                }
            }

            if (SequenceNumberCommitted != null)
                SequenceNumberCommitted(args.TransactionId);

            if (shouldWait)
            {
                Log.Publish(VerboseLevel.PerformanceIssue, "Queue is full", "Rollover task is taking a long time. A long pause on the inputs is about to occur.");
                m_rolloverComplete.WaitOne();
            }
        }

        private void RolloverTask_Running(object sender, EventArgs<ScheduledTaskRunningReason> e)
        {
            //The worker can be disposed either via the Stop() method or 
            //the Dispose() method.  If via the dispose method, then
            //don't do any cleanup.
            if (m_disposed && e.Argument == ScheduledTaskRunningReason.Disposing)
            {
                if (Log.ShouldPublishInfo)
                    Log.Publish(VerboseLevel.Information, "Rollover thread is Disposing");

                m_rolloverComplete.Dispose();
                return;
            }

            long sequenceNumber;
            lock (m_syncRoot)
            {
                var swap = m_activeStagingFile;
                m_activeStagingFile = m_workingStagingFile;
                m_workingStagingFile = swap;
                sequenceNumber = m_lastCommitedSequenceNumber;
                m_rolloverComplete.Set();
            }

            m_workingStagingFile.DumpToDisk();
            m_lastRolledOverSequenceNumber.Value = sequenceNumber;

            if (RolloverComplete != null)
                RolloverComplete(sequenceNumber);
        }

        /// <summary>
        /// Stop all writing to this class.
        /// Once stopped, it cannot be resumed.
        /// All data is then immediately flushed to the output.
        /// This method calls Dispose()
        /// </summary>
        /// <returns></returns>
        public long Stop()
        {
            if (Log.ShouldPublishInfo)
                Log.Publish(VerboseLevel.Information, "Stop() called", "Write is stopping");

            lock (m_syncRoot)
            {
                m_stopped = true;
            }
            m_rolloverTask.Dispose();
            Dispose();
            return m_lastCommitedSequenceNumber;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FirstStageWriter{TKey,TValue}"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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
                    m_rolloverComplete.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Publish(VerboseLevel.BugReport, "Unhandled exception in the dispose process", null, null, ex);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Triggers a rollover if the provided sequence id has not yet been committed.
        /// </summary>
        /// <param name="sequenceId"></param>
        public void Commit(long sequenceId)
        {
            lock (m_syncRoot)
            {
                if (sequenceId > m_lastRolledOverSequenceNumber)
                    m_rolloverTask.Start();
            }
        }

        private void OnProcessException(object sender, EventArgs<Exception> e)
        {
            if (Log.ShouldPublishCritical)
                Log.Publish(VerboseLevel.Critical, "Unhandled exception", "The worker thread threw an unhandled exception", null, e.Argument);
        }
    }
}