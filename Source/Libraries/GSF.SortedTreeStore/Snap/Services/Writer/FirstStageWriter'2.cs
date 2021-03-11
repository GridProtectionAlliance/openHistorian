//******************************************************************************************************
//  FirstStageWriter`2.cs - Gbtc
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
//  02/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GSF.Diagnostics;
using GSF.Snap.Services.Reader;
using GSF.Snap.Storage;
using GSF.Threading;

namespace GSF.Snap.Services.Writer
{
    /// <summary>
    /// Handles how data is initially taken from prestage chunks and serialized to the disk.
    /// </summary>
    public class FirstStageWriter<TKey, TValue>
        : DisposableLoggingClassBase
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
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

        private readonly FirstStageWriterSettings m_settings;
        private bool m_stopped;
        private bool m_disposed;
        private readonly AtomicInt64 m_lastCommitedSequenceNumber = new AtomicInt64();
        private readonly AtomicInt64 m_lastRolledOverSequenceNumber = new AtomicInt64();

        private readonly ScheduledTask m_rolloverTask;
        private readonly object m_syncRoot;
        private readonly SafeManualResetEvent m_rolloverComplete;
        private readonly ArchiveList<TKey, TValue> m_list;
        private List<SortedTreeTable<TKey, TValue>> m_pendingTables1;
        private List<SortedTreeTable<TKey, TValue>> m_pendingTables2;
        private List<SortedTreeTable<TKey, TValue>> m_pendingTables3;
        private readonly SimplifiedArchiveInitializer<TKey, TValue> m_createNextStageFile;

        /// <summary>
        /// Creates a stage writer.
        /// </summary>
        public FirstStageWriter(FirstStageWriterSettings settings, ArchiveList<TKey, TValue> list)
            : base(MessageClass.Framework)
        {
            if (settings is null)
                throw new ArgumentNullException("settings");
            m_settings = settings.CloneReadonly();
            m_settings.Validate();
            m_createNextStageFile = new SimplifiedArchiveInitializer<TKey, TValue>(m_settings.FinalSettings);
            m_rolloverComplete = new SafeManualResetEvent(false);
            m_list = list;
            m_pendingTables1 = new List<SortedTreeTable<TKey, TValue>>();
            m_pendingTables2 = new List<SortedTreeTable<TKey, TValue>>();
            m_pendingTables3 = new List<SortedTreeTable<TKey, TValue>>();
            m_syncRoot = new object();
            m_rolloverTask = new ScheduledTask(ThreadingMode.DedicatedForeground, ThreadPriority.Normal);
            m_rolloverTask.Running += RolloverTask_Running;
            m_rolloverTask.UnhandledException += OnProcessException;
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
                Log.Publish(MessageLevel.Info, "No new points can be added. Point queue has been stopped. Data in rollover will be lost");
                return;
            }
            if (m_disposed)
            {
                Log.Publish(MessageLevel.Info, "First stage writer has been disposed. Data in rollover will be lost");
                return;
            }

            SortedTreeFile file = SortedTreeFile.CreateInMemory(4096);
            SortedTreeTable<TKey, TValue> table = file.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
            using (SortedTreeTableEditor<TKey, TValue> edit = table.BeginEdit())
            {
                edit.AddPoints(args.Stream);
                edit.Commit();
            }

            bool shouldWait = false;
            //If there is data to write then write it to the current archive.
            lock (m_syncRoot)
            {
                if (m_stopped)
                {
                    Log.Publish(MessageLevel.Info, "No new points can be added. Point queue has been stopped. Data in rollover will be lost");
                    table.Dispose();
                    return;
                }
                if (m_disposed)
                {
                    Log.Publish(MessageLevel.Info, "First stage writer has been disposed. Data in rollover will be lost");
                    table.Dispose();
                    return;
                }

                using (ArchiveListEditor<TKey, TValue> edit = m_list.AcquireEditLock())
                {
                    edit.Add(table);
                }
                m_pendingTables1.Add(table);

                if (m_pendingTables1.Count == 10)
                {
                    using (UnionTreeStream<TKey, TValue> reader = new UnionTreeStream<TKey, TValue>(m_pendingTables1.Select(x => new ArchiveTreeStreamWrapper<TKey, TValue>(x)), true))
                    {
                        SortedTreeFile file1 = SortedTreeFile.CreateInMemory(4096);
                        SortedTreeTable<TKey, TValue> table1 = file1.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
                        using (SortedTreeTableEditor<TKey, TValue> edit = table1.BeginEdit())
                        {
                            edit.AddPoints(reader);
                            edit.Commit();
                        }

                        using (ArchiveListEditor<TKey, TValue> edit = m_list.AcquireEditLock())
                        {
                            //Add the newly created file.
                            edit.Add(table1);

                            foreach (SortedTreeTable<TKey, TValue> table2 in m_pendingTables1)
                            {
                                edit.TryRemoveAndDelete(table2.ArchiveId);
                            }
                        }

                        m_pendingTables2.Add(table1);
                        m_pendingTables1.Clear();
                    }
                }

                if (m_pendingTables2.Count == 10)
                {
                    using (UnionTreeStream<TKey, TValue> reader = new UnionTreeStream<TKey, TValue>(m_pendingTables2.Select(x => new ArchiveTreeStreamWrapper<TKey, TValue>(x)), true))
                    {
                        SortedTreeFile file1 = SortedTreeFile.CreateInMemory(4096);
                        SortedTreeTable<TKey, TValue> table1 = file1.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
                        using (SortedTreeTableEditor<TKey, TValue> edit = table1.BeginEdit())
                        {
                            edit.AddPoints(reader);
                            edit.Commit();
                        }

                        using (ArchiveListEditor<TKey, TValue> edit = m_list.AcquireEditLock())
                        {
                            //Add the newly created file.
                            edit.Add(table1);

                            foreach (SortedTreeTable<TKey, TValue> table2 in m_pendingTables2)
                            {
                                edit.TryRemoveAndDelete(table2.ArchiveId);
                            }
                        }

                        m_pendingTables3.Add(table1);
                        m_pendingTables2.Clear();
                    }
                }

                m_lastCommitedSequenceNumber.Value = args.TransactionId;

                long currentSizeMb = (m_pendingTables1.Sum(x => x.BaseFile.ArchiveSize) + m_pendingTables2.Sum(x => x.BaseFile.ArchiveSize)) >> 20;
                if (currentSizeMb > m_settings.MaximumAllowedMb)
                {
                    shouldWait = true;
                    m_rolloverTask.Start();
                    m_rolloverComplete.Reset();
                }
                else if (currentSizeMb > m_settings.RolloverSizeMb)
                {
                    m_rolloverTask.Start();
                }
                else
                {
                    m_rolloverTask.Start(m_settings.RolloverInterval);
                }
            }

            if (SequenceNumberCommitted != null)
                SequenceNumberCommitted(args.TransactionId);

            if (shouldWait)
            {
                Log.Publish(MessageLevel.NA, MessageFlags.PerformanceIssue, "Queue is full", "Rollover task is taking a long time. A long pause on the inputs is about to occur.");
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
                Log.Publish(MessageLevel.Info, "Rollover thread is Disposing");

                m_rolloverComplete.Dispose();
                return;
            }

            List<SortedTreeTable<TKey, TValue>> pendingTables1;
            List<SortedTreeTable<TKey, TValue>> pendingTables2;
            List<SortedTreeTable<TKey, TValue>> pendingTables3;
            long sequenceNumber;
            lock (m_syncRoot)
            {
                pendingTables1 = m_pendingTables1;
                pendingTables2 = m_pendingTables2;
                pendingTables3 = m_pendingTables3;
                sequenceNumber = m_lastCommitedSequenceNumber;
                m_pendingTables1 = new List<SortedTreeTable<TKey, TValue>>();
                m_pendingTables2 = new List<SortedTreeTable<TKey, TValue>>();
                m_pendingTables3 = new List<SortedTreeTable<TKey, TValue>>();
                m_rolloverComplete.Set();
            }

            TKey startKey = new TKey();
            TKey endKey = new TKey();
            startKey.SetMax();
            endKey.SetMin();

            Log.Publish(MessageLevel.Info, "Pending Tables Report", "Pending Tables V1: " + pendingTables1.Count + " V2: " + pendingTables2.Count + " V3: " + pendingTables3.Count);

            List<ArchiveTableSummary<TKey, TValue>> summaryTables = new List<ArchiveTableSummary<TKey, TValue>>();
            foreach (SortedTreeTable<TKey, TValue> table in pendingTables1)
            {
                ArchiveTableSummary<TKey, TValue> summary = new ArchiveTableSummary<TKey, TValue>(table);
                if (!summary.IsEmpty)
                {
                    summaryTables.Add(summary);
                    if (startKey.IsGreaterThan(summary.FirstKey))
                        summary.FirstKey.CopyTo(startKey);
                    if (endKey.IsLessThan(summary.LastKey))
                        summary.LastKey.CopyTo(endKey);
                }
            }
            foreach (SortedTreeTable<TKey, TValue> table in pendingTables2)
            {
                ArchiveTableSummary<TKey, TValue> summary = new ArchiveTableSummary<TKey, TValue>(table);
                if (!summary.IsEmpty)
                {
                    summaryTables.Add(summary);
                    if (startKey.IsGreaterThan(summary.FirstKey))
                        summary.FirstKey.CopyTo(startKey);
                    if (endKey.IsLessThan(summary.LastKey))
                        summary.LastKey.CopyTo(endKey);
                }
            }
            foreach (SortedTreeTable<TKey, TValue> table in pendingTables3)
            {
                ArchiveTableSummary<TKey, TValue> summary = new ArchiveTableSummary<TKey, TValue>(table);
                if (!summary.IsEmpty)
                {
                    summaryTables.Add(summary);
                    if (startKey.IsGreaterThan(summary.FirstKey))
                        summary.FirstKey.CopyTo(startKey);
                    if (endKey.IsLessThan(summary.LastKey))
                        summary.LastKey.CopyTo(endKey);
                }
            }


            long size = summaryTables.Sum(x => x.SortedTreeTable.BaseFile.ArchiveSize);

            if (summaryTables.Count > 0)
            {
                using (UnionTreeStream<TKey, TValue> reader = new UnionTreeStream<TKey, TValue>(summaryTables.Select(x => new ArchiveTreeStreamWrapper<TKey, TValue>(x)), true))
                {
                    SortedTreeTable<TKey, TValue> newTable = m_createNextStageFile.CreateArchiveFile(startKey, endKey, size, reader, null);

                    using (ArchiveListEditor<TKey, TValue> edit = m_list.AcquireEditLock())
                    {
                        //Add the newly created file.
                        edit.Add(newTable);

                        foreach (SortedTreeTable<TKey, TValue> table in pendingTables1)
                        {
                            edit.TryRemoveAndDelete(table.ArchiveId);
                        }

                        foreach (SortedTreeTable<TKey, TValue> table in pendingTables2)
                        {
                            edit.TryRemoveAndDelete(table.ArchiveId);
                        }


                        foreach (SortedTreeTable<TKey, TValue> table in pendingTables3)
                        {
                            edit.TryRemoveAndDelete(table.ArchiveId);
                        }
                    }
                }
            }

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
            Log.Publish(MessageLevel.Info, "Stop() called", "Write is stopping");

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
                    Log.Publish(MessageLevel.Info, MessageFlags.BugReport, "Unhandled exception in the dispose process", null, null, ex);
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
            Log.Publish(MessageLevel.Critical, "Unhandled exception", "The worker thread threw an unhandled exception", null, e.Argument);
        }
    }
}