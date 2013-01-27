//******************************************************************************************************
//  StageWriter.cs - Gbtc
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
using openHistorian.Archive;

namespace openHistorian.Engine.ArchiveWriters
{
    /// <summary>
    /// A collection of settings for <see cref="PrestageWriter"/>.
    /// </summary>
    internal struct StageWriterSettings
    {
        /// <summary>
        /// The time interval in milliseconds after which automatic data commits occur.
        /// </summary>
        public int RolloverInterval;
        /// <summary>
        /// The maximum desired number of points in the prebuffer before a commit now is requested.
        /// </summary>
        public long RolloverSize;

        /// <summary>
        /// The size that a file is permitted to get before entering a wait state to wait for a pending rollover
        /// to complete.
        /// </summary>
        public long MaximumAllowedSize;

        public StagingFile StagingFile;
    }

    /// <summary>
    /// Stage Zero is reponsible for getting archive data packaged into a user sortable transaction format.
    /// </summary>
    internal class StageWriter : IDisposable
    {
        public event Action<long> SequenceNumberCommitted;

        bool m_stopped;
        bool m_disposed;
        int m_rolloverInterval;
        long m_rolloverSize;
        long m_maximumAllowedSize;
        long m_lastCommitedSequenceNumber;
        long m_lastRolledOverSequenceNumber;
        ScheduledTask m_rolloverTask;
        Action<RolloverArgs> m_onRollover;
        object m_syncRoot;
        StagingFile m_stagingFile;
        ManualResetEvent m_rolloverComplete;

        public StageWriter(StageWriterSettings settings, Action<RolloverArgs> onRollover)
        {
            m_rolloverComplete = new ManualResetEvent(false);
            m_stagingFile = settings.StagingFile;
            m_rolloverInterval = settings.RolloverInterval;
            m_rolloverSize = settings.RolloverSize;
            m_maximumAllowedSize = settings.MaximumAllowedSize;
            m_syncRoot = new object();
            m_onRollover = onRollover;
            m_rolloverTask = new ScheduledTask(ThreadingMode.Foreground);
            m_rolloverTask.OnEvent += ProcessRollover;
            m_rolloverTask.Start(m_rolloverInterval);
        }

        /// <summary>
        /// Gets if this stage is backed by a physical file. 
        /// This means that any commits that occur are hard commits.
        /// </summary>
        public bool IsFileBacked
        {
            get
            {
                return m_stagingFile.IsFileBacked;
            }
        }

        public void AppendData(RolloverArgs args)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            long currentSize;
            ArchiveListRemovalStatus removedFile = null;
            //If there is data to write then write it to the current archive.
            lock (m_syncRoot)
            {
                if (m_stopped)
                    throw new Exception("No new points can be added. Point queue has been stopped.");

                if (args.File == null)
                    m_stagingFile.Append(args.CurrentStream);
                else
                    m_stagingFile.Combine(args.File);
                m_lastCommitedSequenceNumber = args.SequenceNumber;

                currentSize = m_stagingFile.Size;

                if (currentSize > m_rolloverSize)
                    m_rolloverTask.Start();

                if (currentSize > m_maximumAllowedSize)
                    m_rolloverComplete.Reset();
            }
            
            if (SequenceNumberCommitted != null)
                SequenceNumberCommitted(args.SequenceNumber);

            if (currentSize > m_maximumAllowedSize)
                m_rolloverComplete.WaitOne();
        }

        void ProcessRollover(object sender, ScheduledTaskEventArgs e)
        {
            //The worker can be disposed either via the Stop() method or 
            //the Dispose() method.  If via the dispose method, then
            //don't do any cleanup.
            if (m_disposed && e.IsDisposing)
                return;

            //go ahead and schedule the next rollover since nothing
            //will happen until this function exits anyway.
            //if the task is disposing, the following line does nothing.
            m_rolloverTask.Start(m_rolloverInterval);

            ArchiveFile file;
            long sequenceNumber;
            lock (m_syncRoot)
            {
                sequenceNumber = m_lastCommitedSequenceNumber;
                file = m_stagingFile.GetFileAndSetNull();
                m_rolloverComplete.Set();
            }

            if (file == null)
            {
                return;
            }

            RolloverArgs args = new RolloverArgs(file, null, sequenceNumber);
            m_onRollover(args);
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
    }
}
