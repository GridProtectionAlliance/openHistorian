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
using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Server.Database.Archive;
using openHistorian.V2.Server.Database.ArchiveWriters;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class ArchiveDatabaseEngine
    {
        List<ArchiveListRemovalStatus> m_pendingDispose;
        IArchiveWriter m_archiveWriter;
        ArchiveList m_archiveList;
        volatile bool m_disposed;

        public ArchiveDatabaseEngine(DatabaseSettings settings)
        {
            m_pendingDispose = new List<ArchiveListRemovalStatus>();
            m_archiveList = new ArchiveList(settings.AttachedFiles);

            if (settings.ArchiveWriter != null)
            {
                if (settings.ArchiveWriter.AutoCommit)
                    m_archiveWriter = new AutoCommit(settings, m_archiveList);
                else
                    m_archiveWriter = new ManualCommit(settings, m_archiveList);
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

                m_archiveList.Dispose();

                foreach (var status in m_pendingDispose)
                {
                    status.Archive.Dispose();
                }
            }
        }

        public bool IsCommitted(long transactionId)
        {
            return m_archiveWriter.IsCommitted(transactionId);
        }

        public bool IsDiskCommitted(long transactionId)
        {
            return m_archiveWriter.IsDiskCommitted(transactionId);
        }

        public bool WaitForCommitted(long transactionId)
        {
            return m_archiveWriter.WaitForCommitted(transactionId);
        }

        public bool WaitForDiskCommitted(long transactionId)
        {
            return m_archiveWriter.WaitForDiskCommitted(transactionId);
        }

        public void Commit()
        {
            m_archiveWriter.Commit();
        }

        public void CommitToDisk()
        {
            m_archiveWriter.CommitToDisk();
        }

        public long LastCommittedTransactionId
        {
            get
            {
                return m_archiveWriter.LastCommittedTransactionId;
            }
        }
        public long LastDiskCommittedTransactionId
        {
            get
            {
                return m_archiveWriter.LastDiskCommittedTransactionId;
            }
        }
        public long CurrentTransactionId
        {
            get
            {
                return m_archiveWriter.CurrentTransactionId;
            }
        }

    }
}
