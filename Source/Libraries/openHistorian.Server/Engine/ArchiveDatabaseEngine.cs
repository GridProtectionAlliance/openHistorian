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
using openHistorian.Engine.ArchiveWriters;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class ArchiveDatabaseEngine : IHistorianDatabase
    {
        List<ArchiveListRemovalStatus> m_pendingDispose;
        AutoCommit m_archiveWriter;
        ArchiveList m_archiveList;
        volatile bool m_disposed;

        public ArchiveDatabaseEngine(WriterOptions? writer, params string[] paths)
            : this(new DatabaseConfig(writer, paths))
        {

        }

        public ArchiveDatabaseEngine(DatabaseConfig settings)
            : this(new DatabaseSettings(settings))
        {

        }

        internal ArchiveDatabaseEngine(DatabaseSettings settings)
        {
            m_pendingDispose = new List<ArchiveListRemovalStatus>();
            m_archiveList = new ArchiveList(settings.AttachedFiles);

            if (settings.ArchiveWriter != null)
            {
                m_archiveWriter = new AutoCommit(settings, m_archiveList);
            }
        }

        /// <summary>
        /// The most recent transaction that is available to be queried.
        /// </summary>
        public long LastCommittedTransactionId
        {
            get
            {
                return m_archiveWriter.LastCommittedTransactionId;
            }
        }

        /// <summary>
        /// The most recent transaction id that has been committed to a perminent storage system.
        /// </summary>
        public long LastDiskCommittedTransactionId
        {
            get
            {
                return m_archiveWriter.LastDiskCommittedTransactionId;
            }
        }

        /// <summary>
        /// The transaction of the most recently inserted data.
        /// </summary>
        public long CurrentTransactionId
        {
            get
            {
                return m_archiveWriter.CurrentTransactionId;
            }
        }

        /// <summary>
        /// Determines if this database is currently online.
        /// </summary>
        public bool IsOnline
        {
            get
            {
                return true;
            }
        }

        public void Write(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_archiveWriter == null)
                throw new Exception("Writing is not configured on this historian");
            m_archiveWriter.WriteData(key1, key2, value1, value2);
        }

        public void Write(IPointStream points)
        {
            ulong key1, key2, value1, value2;
            while (points.Read(out key1, out key2, out value1, out value2))
                Write(key1, key2, value1, value2);
        }

        public long WriteBulk(IPointStream points)
        {
            Write(points);
            return CurrentTransactionId;
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

        public void SoftCommit()
        {
            m_archiveWriter.Commit();
        }

        public void HardCommit()
        {
            m_archiveWriter.CommitToDisk();
        }

        /// <summary>
        /// Disconnects from the current database. 
        /// </summary>
        public void Disconnect()
        {
            //Does nothing
        }

        /// <summary>
        /// Opens a stream connection that can be used to read 
        /// and write data to the current historian database.
        /// </summary>
        /// <returns></returns>
        public IHistorianDataReader OpenDataReader()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new ArchiveReader(m_archiveList);
        }

        /// <summary>
        /// Talks the historian database offline
        /// </summary>
        /// <param name="waitTimeSeconds">the maximum number of seconds to wait before terminating all client connections.</param>
        public void TakeOffline(float waitTimeSeconds = 0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Brings this database online.
        /// </summary>
        public void BringOnline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shuts down this database.
        /// </summary>
        /// <param name="waitTimeSeconds"></param>
        public void Shutdown(float waitTimeSeconds = 0)
        {
            throw new NotImplementedException();
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
                if (m_archiveWriter != null)
                    m_archiveWriter.Dispose();

                m_archiveList.Dispose();

                foreach (var status in m_pendingDispose)
                {
                    status.Archive.Dispose();
                }
            }
        }

    }
}
