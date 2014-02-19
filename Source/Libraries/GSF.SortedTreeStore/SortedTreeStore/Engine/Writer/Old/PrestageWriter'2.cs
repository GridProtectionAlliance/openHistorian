//******************************************************************************************************
//  PrestageWriter`2.cs - Gbtc
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
using GSF.Threading;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer.Old
{
    /// <summary>
    /// A collection of settings for <see cref="PrestageWriter{TKey,TValue}"/>.
    /// </summary>
    public struct PrestageSettings
    {
        /// <summary>
        /// The time interval in milliseconds after which automatic data commits occur.
        /// </summary>
        public int RolloverInterval;

        /// <summary>
        /// The maximum desired number of points in the prebuffer before a commit now is requested.
        /// </summary>
        public int RolloverPointCount;

        /// <summary>
        /// The maximum number of points before intentional delays are added to the input
        /// in an attempt to slow down the input.
        /// </summary>
        public int DelayOnPointCount;
    }

    /// <summary>
    /// Where uncommitted data is collected before it is 
    /// inserted into an archive file in a bulk operation.
    /// </summary>
    /// <remarks>
    /// This class is thread safe
    /// </remarks>
    public class PrestageWriter<TKey, TValue>
        : IDisposable
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        private bool m_disposed;
        private bool m_stopped;
        private readonly int m_rolloverInterval;
        private readonly int m_rolloverPointCount;
        private readonly int m_yieldThreadOnPointCount;
        private readonly int m_sleepThreadOnPointCount;
        private long m_sequenceId;
        private readonly object m_syncRoot;
        private PointStreamCache<TKey, TValue> m_processingQueue;
        private PointStreamCache<TKey, TValue> m_activeQueue;
        private ScheduledTask m_rolloverTask;
        private readonly Action<RolloverArgs<TKey, TValue>> m_onRollover;

        /// <summary>
        /// Creates a prestage writer.
        /// </summary>
        /// <param name="settings">the settings for this stage</param>
        /// <param name="onRollover">delegate to call when a file is done with this stage.</param>
        public PrestageWriter(PrestageSettings settings, Action<RolloverArgs<TKey, TValue>> onRollover)
        {
            if (settings.RolloverInterval < 10 || settings.RolloverInterval > 1000)
                throw new ArgumentOutOfRangeException("settings.RolloverInterval", "Must be between 10ms and 1000ms");
            if (settings.RolloverPointCount < 1)
                throw new ArgumentOutOfRangeException("settings.RolloverPointCount", "Cannot be less than 1");
            if (settings.DelayOnPointCount < settings.RolloverPointCount)
                throw new ArgumentOutOfRangeException("settings.DelayOnPointCount", "Must be greater than RolloverPointCount");

            m_sequenceId = 0;
            m_syncRoot = new object();
            m_activeQueue = new PointStreamCache<TKey, TValue>();
            m_processingQueue = new PointStreamCache<TKey, TValue>();
            m_activeQueue.ClearAndSetWriting();
            m_processingQueue.ClearAndSetWriting();
            m_onRollover = onRollover;
            m_rolloverInterval = settings.RolloverInterval;
            m_rolloverPointCount = settings.RolloverPointCount;
            m_yieldThreadOnPointCount = settings.DelayOnPointCount;
            m_sleepThreadOnPointCount = m_yieldThreadOnPointCount + (int)Math.Max(1000f, m_yieldThreadOnPointCount * 0.25f);
            m_rolloverTask = new ScheduledTask(ThreadingMode.DedicatedForeground);
            m_rolloverTask.OnEvent += ProcessRollover;
            m_rolloverTask.Start(m_rolloverInterval);
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
        /// Writes the provided key to the prestage
        /// </summary>
        /// <param name="key">the key to write</param>
        /// <param name="value">the value to write</param>
        /// <returns>the sequence number when this data will be committed</returns>
        /// <remarks>Calls to this function are thread safe</remarks>
        public long Write(TKey key, TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            long sequenceId;
            int pointCount;
            lock (m_syncRoot)
            {
                if (m_stopped)
                    throw new Exception("No new points can be added. Point queue has been stopped.");
                m_sequenceId++;
                m_activeQueue.Write(key, value);
                sequenceId = m_sequenceId;
                pointCount = m_activeQueue.Count;
                if (pointCount == m_rolloverPointCount)
                    m_rolloverTask.Start();
            }
            DelayIfNeeded(pointCount - 1);
            return sequenceId;
        }

        /// <summary>
        /// Writes the provided stream to the prestage buffer
        /// </summary>
        /// <param name="stream">the stream to write</param>
        /// <returns>the sequence number when this data will be committed</returns>
        /// <remarks>Calls to this function are thread safe</remarks>
        public long Write(TreeStream<TKey, TValue> stream)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            long sequenceId;
            int pointCountBefore;
            lock (m_syncRoot)
            {
                if (m_stopped)
                    throw new Exception("No new points can be added. Point queue has been stopped.");
                m_sequenceId++;
                pointCountBefore = m_activeQueue.Count;
                m_activeQueue.Write(stream);
                sequenceId = m_sequenceId;
                int pointCount = m_activeQueue.Count;
                if (pointCount >= m_rolloverPointCount && pointCountBefore < m_rolloverPointCount)
                    m_rolloverTask.Start();
            }
            DelayIfNeeded(pointCountBefore);
            return sequenceId;
        }

        /// <summary>
        /// Automatically delays the input if too many points are being written to the historian at once.
        /// </summary>
        /// <param name="pointCountBefore"></param>
        private void DelayIfNeeded(int pointCountBefore)
        {
            if (pointCountBefore >= m_sleepThreadOnPointCount)
                Thread.Sleep(1);
            else if (pointCountBefore >= m_yieldThreadOnPointCount)
                Thread.Sleep(0);
        }

        private void ProcessRollover(object sender, ScheduledTaskEventArgs e)
        {
            //the nature of how the ScheduledTask works 
            //gaurentees that this function will not be called concurrently

            //The worker can be disposed either via the Stop() method or 
            //the Dispose() method.  If via the dispose method, then
            //don't do any cleanup.
            if (m_disposed && e.IsDisposing)
                return;

            //go ahead and schedule the next rollover since nothing
            //will happen until this function exits anyway.
            //if the task is disposing, the following line does nothing.
            m_rolloverTask.Start(m_rolloverInterval);

            RolloverArgs<TKey, TValue> args;
            lock (m_syncRoot)
            {
                int count = m_activeQueue.Count;
                if (count == 0)
                    return;

                //Swap active and processing.
                PointStreamCache<TKey, TValue> stream = m_activeQueue;
                m_activeQueue = m_processingQueue;
                m_processingQueue = stream;

                m_activeQueue.ClearAndSetWriting();
                stream.SetReadingFromBeginning();

                args = new RolloverArgs<TKey, TValue>(stream, m_sequenceId);
            }

            m_onRollover(args);
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
                }
                finally
                {
                    m_activeQueue = null;
                    m_processingQueue = null;
                    m_rolloverTask = null;
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
    }
}