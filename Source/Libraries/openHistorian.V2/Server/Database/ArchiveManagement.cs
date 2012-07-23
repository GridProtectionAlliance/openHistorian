//******************************************************************************************************
//  ArchiveManagement.cs - Gbtc
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Performs the required rollovers by reading partitions from the data list
    /// and combining them into a file of a later generation.
    /// </summary>
    class ArchiveManagement
    {
        NewArchiveCriteria m_newArchiveCriteria;

        ArchiveInitializer m_archiveInitializer;

        volatile bool m_disposed;

        ArchiveList m_archiveList;

        ArchiveFile m_activeFile;

        Thread m_insertThread;

        ManualResetEvent m_waitTimer;

        int m_commitCount;

        Stopwatch m_lastCommitTime;

        int m_rolloverGenerationNumber;

        /// <summary>
        /// Creates a new <see cref="ArchiveManagement"/>.
        /// </summary>
        /// <param name="archiveInitializer">Used to create a new partition.</param>
        /// <param name="archiveList">The list used to attach newly created file.</param>
        /// <param name="newArchiveCriteria"></param>
        public ArchiveManagement(ArchiveInitializer archiveInitializer, ArchiveList archiveList, NewArchiveCriteria newArchiveCriteria, int rolloverGenerationNumber)
        {
            m_rolloverGenerationNumber = rolloverGenerationNumber;
            m_archiveList = archiveList;
            m_newArchiveCriteria = newArchiveCriteria;
            m_archiveInitializer = archiveInitializer;

            m_lastCommitTime = Stopwatch.StartNew();

            m_waitTimer = new ManualResetEvent(false);
            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
        }

        /// <summary>
        /// This is executed by a dedicated thread and moves data from the point queue to the database.
        /// </summary>
        void ProcessInsertingData()
        {
            while (!m_disposed)
            {
                m_waitTimer.WaitOne(1000);
                m_waitTimer.Reset();

                List<ArchiveFileSummary> partitionsToRollOver = new List<ArchiveFileSummary>();

                using (var edit = m_archiveList.AcquireEditLock())
                {
                    foreach (var file in edit.Partitions)
                    {
                        if (file.Generation == m_rolloverGenerationNumber && !file.IsEditLocked)
                        {
                            partitionsToRollOver.Add(file.Summary);
                            file.IsEditLocked = true;
                        }
                    }
                }

                if (partitionsToRollOver.Count > 0)
                {
                    if (m_activeFile == null ||
                        (m_newArchiveCriteria.IsCommitCountValid && m_commitCount >= m_newArchiveCriteria.CommitCount) ||
                        (m_newArchiveCriteria.IsIntervalValid && m_lastCommitTime.Elapsed >= m_newArchiveCriteria.Interval) ||
                        (m_newArchiveCriteria.IsPartitionSizeValid && m_activeFile.FileSize >= m_newArchiveCriteria.PartitionSize))
                    {
                        m_commitCount = 0;
                        m_lastCommitTime.Restart();
                        var newFile = m_archiveInitializer.CreatePartition(m_rolloverGenerationNumber + 1);
                        using (var edit = m_archiveList.AcquireEditLock())
                        {
                            //Create a new file.
                            if (m_activeFile != null)
                            {
                                edit.ReleaseEditLock(m_activeFile);
                            }
                            edit.Add(newFile, new ArchiveFileStateInformation(false, true, 0));
                        }
                        m_activeFile = newFile;
                    }
                    m_activeFile.BeginEdit();

                    foreach (var source in partitionsToRollOver)
                    {
                        using (var src = source.ActiveSnapshot.OpenInstance())
                        {
                            m_activeFile.BeginEdit();

                            var reader = src.GetDataRange();
                            reader.SeekToKey(0, 0);

                            ulong value1, value2, key1, key2;
                            while (reader.GetNextKey(out key1, out key2, out value1, out value2))
                            {
                                m_activeFile.AddPoint(key1, key2, value1, value2);
                            }

                            m_activeFile.CommitEdit();
                            using (var editor = m_archiveList.AcquireEditLock())
                            {
                                editor.RenewSnapshot(m_activeFile);
                            }
                        }
                        m_commitCount++;
                    }
                    m_activeFile.CommitEdit();
                    using (var editor = m_archiveList.AcquireEditLock())
                    {
                        editor.RenewSnapshot(m_activeFile);
                        foreach (var source in partitionsToRollOver)
                        {
                            ArchiveListRemovalStatus status;
                            if (editor.Remove(source.ArchiveFileFile, out status))
                            {
                                //ToDo: Do something with this removal status
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves data from the queue and inserts it into Generation 0's Archive.
        /// </summary>
        public void SignalProcessRollover()
        {
            m_waitTimer.Set();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                SignalProcessRollover();
            }
        }
    }
}
