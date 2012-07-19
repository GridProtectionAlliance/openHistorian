//******************************************************************************************************
//  DataManagement.cs - Gbtc
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
using openHistorian.V2.Server.Database.Partitions;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Performs the required rollovers by reading partitions from the data list
    /// and combining them into a file of a later generation.
    /// </summary>
    class DataManagement
    {
        NewPartitionCriteria m_newPartitionCriteria;

        PartitionInitializer m_partitionInitializer;

        volatile bool m_disposed;

        DataList m_dataList;

        PartitionFile m_activeFile;

        Thread m_insertThread;

        ManualResetEvent m_waitTimer;

        int m_commitCount;

        Stopwatch m_lastCommitTime;

        int m_rolloverGenerationNumber;

        /// <summary>
        /// Creates a new <see cref="DataManagement"/>.
        /// </summary>
        /// <param name="partitionInitializer">Used to create a new partition.</param>
        /// <param name="dataList">The list used to attach newly created file.</param>
        /// <param name="newPartitionCriteria"></param>
        public DataManagement(PartitionInitializer partitionInitializer, DataList dataList, NewPartitionCriteria newPartitionCriteria, int rolloverGenerationNumber)
        {
            m_rolloverGenerationNumber = rolloverGenerationNumber;
            m_dataList = dataList;
            m_newPartitionCriteria = newPartitionCriteria;
            m_partitionInitializer = partitionInitializer;

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

                List<PartitionSummary> partitionsToRollOver = new List<PartitionSummary>();

                using (var edit = m_dataList.AcquireEditLock())
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
                        (m_newPartitionCriteria.IsCommitCountValid && m_commitCount >= m_newPartitionCriteria.CommitCount) ||
                        (m_newPartitionCriteria.IsIntervalValid && m_lastCommitTime.Elapsed >= m_newPartitionCriteria.Interval) ||
                        (m_newPartitionCriteria.IsPartitionSizeValid && m_activeFile.FileSize >= m_newPartitionCriteria.PartitionSize))
                    {
                        m_commitCount = 0;
                        m_lastCommitTime.Restart();
                        var newFile = m_partitionInitializer.CreatePartition(m_rolloverGenerationNumber + 1);
                        using (var edit = m_dataList.AcquireEditLock())
                        {
                            //Create a new file.
                            if (m_activeFile != null)
                            {
                                edit.ReleaseEditLock(m_activeFile);
                            }
                            edit.Add(newFile, new PartitionStateInformation(false, true, 0));
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
                            using (var editor = m_dataList.AcquireEditLock())
                            {
                                editor.RenewSnapshot(m_activeFile);
                            }
                        }
                        m_commitCount++;
                    }
                    m_activeFile.CommitEdit();
                    using (var editor = m_dataList.AcquireEditLock())
                    {
                        editor.RenewSnapshot(m_activeFile);
                        foreach (var source in partitionsToRollOver)
                        {
                            DataListRemovalStatus status;
                            if (editor.Remove(source.PartitionFileFile, out status))
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
