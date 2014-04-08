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
//  2/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;
using GSF.Threading;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{
    /// <summary>
    /// Handles how data is initially taken from prestage chunks and serialized to the disk.
    /// </summary>
    public class FirstStageWriter<TKey, TValue> : IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        /// <summary>
        /// An event handler that will raise any exceptions that go unhandled in the rollover process.
        /// </summary>
        public event EventHandler<EventArgs<Exception>> UnhandledException;
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
        private readonly int m_rolloverInterval;
        private readonly long m_rolloverSize;
        private long m_lastCommitedSequenceNumber;
        private long m_lastRolledOverSequenceNumber;
        private ScheduledTask m_rolloverTask;
        private readonly object m_syncRoot;
        private IncrementalStagingFile<TKey, TValue> m_activeStagingFile;
        private IncrementalStagingFile<TKey, TValue> m_workingStagingFile;
        private readonly ManualResetEvent m_rolloverComplete;
        long m_maximumAllowedSize;

        /// <summary>
        /// Creates a stage writer.
        /// </summary>
        public FirstStageWriter(IncrementalStagingFile<TKey, TValue> incrementalStagingFile, int rolloverInterval)
        {
            m_rolloverSize = 200 * 1024 * 1024;
            m_maximumAllowedSize = 300 * 1024 * 1024;
            m_rolloverComplete = new ManualResetEvent(false);
            m_activeStagingFile = incrementalStagingFile;
            m_workingStagingFile = incrementalStagingFile.Clone();
            m_rolloverInterval = rolloverInterval;
            m_syncRoot = new object();
            m_rolloverTask = new ScheduledTask(ThreadingMode.DedicatedForeground, ThreadPriority.Normal);
            m_rolloverTask.Running += m_rolloverTask_Running;
            m_rolloverTask.UnhandledException += m_rolloverTask_UnhandledException;
            m_rolloverTask.Start(m_rolloverInterval);
        }


        /// <summary>
        /// Appends this data to this stage. Also queues up for deletion if necessary.
        /// </summary>
        /// <param name="args">arguments handed to this class from either the 
        /// PrestageWriter or another StageWriter of a previous generation</param>
        public void AppendData(PrebufferRolloverArgs<TKey, TValue> args)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            long currentSize;

            //If there is data to write then write it to the current archive.
            lock (m_syncRoot)
            {
                if (m_stopped)
                    throw new Exception("No new points can be added. Point queue has been stopped.");

                m_activeStagingFile.Append(args.Stream);
                m_lastCommitedSequenceNumber = args.TransactionId;

                currentSize = m_activeStagingFile.Size;

                if (currentSize > m_rolloverSize)
                    m_rolloverTask.Start();

                if (currentSize > m_maximumAllowedSize)
                    m_rolloverComplete.Reset();
            }

            if (SequenceNumberCommitted != null)
                SequenceNumberCommitted(args.TransactionId);

            if (currentSize > m_maximumAllowedSize)
                m_rolloverComplete.WaitOne();
        }

        private void m_rolloverTask_Running(object sender, EventArgs<ScheduledTaskRunningReason> e)
        {
            //The worker can be disposed either via the Stop() method or 
            //the Dispose() method.  If via the dispose method, then
            //don't do any cleanup.
            if (m_disposed && e.Argument == ScheduledTaskRunningReason.Disposing)
                return;

            //go ahead and schedule the next rollover since nothing
            //will happen until this function exits anyway.
            //if the task is disposing, the following line does nothing.
            m_rolloverTask.Start(m_rolloverInterval);

            long sequenceNumber;
            lock (m_syncRoot)
            {
                var tmp = m_activeStagingFile;
                m_activeStagingFile = m_workingStagingFile;
                m_workingStagingFile = tmp;
                sequenceNumber = m_lastCommitedSequenceNumber;
            }

            m_workingStagingFile.DumpToDisk();
            m_rolloverComplete.Set();

            m_lastRolledOverSequenceNumber = sequenceNumber;
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
            lock (m_syncRoot)
            {
                m_stopped = true;
            }
            m_rolloverTask.Dispose();
            m_rolloverTask = null;
            Dispose();
            return m_lastCommitedSequenceNumber;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                if (m_rolloverTask != null)
                    m_rolloverTask.Dispose();
                m_rolloverTask = null;
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
                if (sequenceId > m_lastRolledOverSequenceNumber)
                    m_rolloverTask.Start();
            }
        }

        void m_rolloverTask_UnhandledException(object sender, EventArgs<Exception> e)
        {
            if (UnhandledException != null)
                UnhandledException(sender, e);
        }

        void OnRolloverComplete(long obj)
        {
            Action<long> handler = RolloverComplete;
            if (handler != null)
                handler(obj);
        }
    }
}