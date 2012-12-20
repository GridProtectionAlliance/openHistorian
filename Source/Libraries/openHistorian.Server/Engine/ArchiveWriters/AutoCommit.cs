//******************************************************************************************************
//  AutoCommit.cs - Gbtc
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
//  11/17/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using openHistorian.Archive;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine.ArchiveWriters
{
    internal class AutoCommit : IDisposable
    {
        List<ArchiveListRemovalStatus> m_pendingDispose;
        object m_syncRoot;
        ConcurrentWriterAutoCommit m_concurrentWriterAutoCommit;
        ConcurrentArchiveMerger[] m_concurrentArchiveMerger;
        ArchiveList m_archiveList;
        bool m_disposed;

        public AutoCommit(DatabaseSettings settings, ArchiveList archiveList)
        {
            m_archiveList = archiveList;
            m_syncRoot = new object();
            m_pendingDispose = new List<ArchiveListRemovalStatus>();
            m_concurrentArchiveMerger = new ConcurrentArchiveMerger[settings.ArchiveRollovers.Count];

            ConcurrentArchiveMerger previousMerger = null;
            for (int x = settings.ArchiveRollovers.Count - 1; x >= 0; x--) //Go in reverse order since there is chaining that occurs
            {
                var managementSettings = settings.ArchiveRollovers[x];
                if (previousMerger == null)
                {
                    m_concurrentArchiveMerger[x] = new ConcurrentArchiveMerger(managementSettings, m_archiveList, FinalizeArchiveFile, ProcessRemoval);
                    previousMerger = m_concurrentArchiveMerger[x];
                }
                else
                {
                    m_concurrentArchiveMerger[x] = new ConcurrentArchiveMerger(managementSettings, m_archiveList, previousMerger.ProcessArchive, ProcessRemoval);
                    previousMerger = m_concurrentArchiveMerger[x];
                }
            }
            if (previousMerger == null)
            {
                m_concurrentWriterAutoCommit = new ConcurrentWriterAutoCommit(settings.ArchiveWriter, m_archiveList, FinalizeArchiveFile);
            }
            else
            {
                m_concurrentWriterAutoCommit = new ConcurrentWriterAutoCommit(settings.ArchiveWriter, m_archiveList, previousMerger.ProcessArchive);
            }
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
                if (m_concurrentWriterAutoCommit != null)
                    m_concurrentWriterAutoCommit.Dispose();
                foreach (var management in m_concurrentArchiveMerger)
                {
                    management.Dispose();
                }
            }
        }

        public long WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            return m_concurrentWriterAutoCommit.WriteData(key1, key2, value1, value2);
        }

        void FinalizeArchiveFile(ArchiveFile archive, long sequenceId)
        {
            using (var edit = m_archiveList.AcquireEditLock())
            {
                edit.ReleaseEditLock(archive);
            }
        }
        void ProcessRemoval(ArchiveListRemovalStatus removalStatus)
        {
            lock (m_syncRoot)
            {
                if (!removalStatus.IsBeingUsed)
                {
                    removalStatus.Archive.Dispose();
                }
                else
                {
                    m_pendingDispose.Add(removalStatus);
                }
                for (int x = m_pendingDispose.Count - 1; x >= 0; x--)
                {
                    var status = m_pendingDispose[x];
                    if (!status.IsBeingUsed)
                    {
                        status.Archive.Dispose();
                        m_pendingDispose.RemoveAt(x);
                    }
                }
            }
        }

        public bool IsCommitted(long transactionId)
        {
            return m_concurrentWriterAutoCommit.IsCommitted(transactionId);
        }

        public bool IsDiskCommitted(long transactionId)
        {
            return m_concurrentArchiveMerger.Last().IsCommitted(transactionId);
        }

        public bool WaitForCommitted(long transactionId)
        {
            return m_concurrentWriterAutoCommit.WaitForCommit(transactionId, false);
        }

        public bool WaitForDiskCommitted(long transactionId)
        {
            return m_concurrentArchiveMerger.Last().WaitForCommit(transactionId, false);
        }

        public void Commit()
        {
            m_concurrentWriterAutoCommit.Commit();
        }

        public void CommitToDisk()
        {
            m_concurrentWriterAutoCommit.CommitAndRollover();
            for (int x = 0; x < m_concurrentArchiveMerger.Length; x++)
            {
                if (x == m_concurrentArchiveMerger.Length - 1)
                {
                    m_concurrentArchiveMerger[x].Commit();
                }
                else
                {
                    m_concurrentArchiveMerger[x].CommitAndRollover();
                }
            }
        }

        public long LastCommittedTransactionId
        {
            get
            {
                return m_concurrentWriterAutoCommit.LastCommittedSequenceNumber;
            }
        }
        public long LastDiskCommittedTransactionId
        {
            get
            {
                return m_concurrentArchiveMerger.Last().LastCommittedSequenceNumber;
            }
        }
        public long CurrentTransactionId
        {
            get
            {
                return m_concurrentWriterAutoCommit.CurrentSequenceNumber;
            }
        }

    }
}
