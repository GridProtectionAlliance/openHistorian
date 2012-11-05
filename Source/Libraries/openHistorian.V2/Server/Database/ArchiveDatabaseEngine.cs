//******************************************************************************************************
//  ArchiveDatabaseEngine.cs - Gbtc
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class ArchiveDatabaseEngine
    {
        List<ArchiveListRemovalStatus> m_pendingDispose;
        ArchiveWriter m_archiveWriter;
        ArchiveList m_archiveList;
        ArchiveManagement[] m_archiveManagement;
        volatile bool m_disposed;
        object m_syncRoot;

        public ArchiveDatabaseEngine(DatabaseSettings settings)
        {
            m_syncRoot = new object();
            m_pendingDispose = new List<ArchiveListRemovalStatus>();
            m_archiveList = new ArchiveList(settings.AttachedFiles);
            m_archiveManagement = new ArchiveManagement[settings.ArchiveRollovers.Count];

            if (settings.ArchiveWriter != null)
            {
                ArchiveManagement previousManagement = null;
                for (int x = settings.ArchiveRollovers.Count - 1; x >= 0; x--) //Go in reverse order since there is chaining that occurs
                {
                    var managementSettings = settings.ArchiveRollovers[x];
                    if (previousManagement == null)
                    {
                        m_archiveManagement[x] = new ArchiveManagement(managementSettings, m_archiveList, FinalizeArchiveFile, ProcessRemoval);
                        previousManagement = m_archiveManagement[x];
                    }
                    else
                    {
                        m_archiveManagement[x] = new ArchiveManagement(managementSettings, m_archiveList, previousManagement.ProcessArchive, ProcessRemoval);
                        previousManagement = m_archiveManagement[x];
                    }
                }
                if (previousManagement == null)
                {
                    m_archiveWriter = new ArchiveWriter(settings.ArchiveWriter, m_archiveList, FinalizeArchiveFile);
                }
                else
                {
                    m_archiveWriter = new ArchiveWriter(settings.ArchiveWriter, m_archiveList, previousManagement.ProcessArchive);
                }
            }
        }

        public void WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_archiveWriter == null)
                throw new Exception("Writing is not configured on this historian");
            m_archiveWriter.WriteData(key1, key2, value1, value2);
        }

        /// <summary>
        /// Creates a reader that supports queires where only one
        /// can be executed at a time. To support concurrent queries, simply
        /// call this class again. Be sure to call Dispose() when finished with this class.
        /// </summary>
        /// <returns></returns>
        public ArchiveReader CreateReader()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new ArchiveReader(m_archiveList);
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                if (m_archiveWriter != null)
                    m_archiveWriter.Dispose();
                foreach (var management in m_archiveManagement)
                {
                    management.Dispose();
                }
                m_archiveList.Dispose();

                foreach (var status in m_pendingDispose)
                {
                    status.Archive.Dispose();
                }
            }
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
            return m_archiveWriter.IsCommitted(transactionId);
        }

        public bool IsDiskCommitted(long transactionId)
        {
            return m_archiveManagement.Last().IsCommitted(transactionId);
        }

        public bool WaitForCommitted(long transactionId)
        {
            return m_archiveWriter.WaitForCommit(transactionId, false);
        }

        public bool WaitForDiskCommitted(long transactionId)
        {
            return m_archiveManagement.Last().WaitForCommit(transactionId, false);
        }

        public void Commit()
        {
            m_archiveWriter.Commit();
        }

        public void CommitToDisk()
        {
            m_archiveWriter.CommitAndRollover();
            for (int x = 0; x < m_archiveManagement.Length; x++)
            {
                if (x == m_archiveManagement.Length - 1)
                {
                    m_archiveManagement[x].Commit();
                }
                else
                {
                    m_archiveManagement[x].CommitAndRollover();
                }
            }
        }

        public long LastCommittedTransactionId
        {
            get
            {
                return m_archiveWriter.LastCommittedSequenceNumber;
            }
        }
        public long LastDiskCommittedTransactionId
        {
            get
            {
                return m_archiveManagement.Last().LastCommittedSequenceNumber;
            }
        }
        public long CurrentTransactionId
        {
            get
            {
                return m_archiveWriter.CurrentSequenceNumber;
            }
        }

    }
}
