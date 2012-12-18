//******************************************************************************************************
//  WriterManualCommit.cs - Gbtc
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
//  5/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using openHistorian.Engine.Configuration;
using openHistorian.IO.Unmanaged;
using openHistorian.Archive;

namespace openHistorian.Engine.ArchiveWriters
{
    /// <summary>
    /// Responsible for getting data into the database. This class will prebuffer
    /// points and commit them in bulk operations.
    /// </summary>
    internal partial class WriterManualCommit : IDisposable
    {

        ArchiveWriterSettings m_settings;

        long m_lastCommitedSequenceNumber;
        long m_lastRolloverSequenceNumber;
        bool m_disposed;

        ArchiveList m_archiveList;

        Action<ArchiveFile, long> m_callbackFileComplete;

        long m_currentSequenceId;

        ActiveFile activeFile;

        /// <summary>
        /// Creates a new <see cref="ConcurrentWriterAutoCommit"/>.
        /// </summary>
        /// <param name="settings">The settings for this class.</param>
        /// <param name="archiveList">The list used to attach newly created file.</param>
        /// <param name="callbackFileComplete">Once a file is complete with this layer, this callback is invoked</param>
        public WriterManualCommit(ArchiveWriterSettings settings, ArchiveList archiveList, Action<ArchiveFile, long> callbackFileComplete)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (archiveList == null)
                throw new ArgumentNullException("archiveList");
            if (callbackFileComplete == null)
                throw new ArgumentNullException("callbackFileComplete");

            m_currentSequenceId = 0;
            m_lastCommitedSequenceNumber = -1;
            m_lastRolloverSequenceNumber = -1;

            m_callbackFileComplete = callbackFileComplete;

            m_settings = settings;

            m_archiveList = archiveList;

            activeFile = new ActiveFile(m_archiveList, m_callbackFileComplete);
        }

        public long CurrentSequenceNumber
        {
            get
            {
                return m_currentSequenceId;
            }
        }
        public long LastCommittedSequenceNumber
        {
            get
            {
                return m_lastCommitedSequenceNumber;
            }
        }
        public long LastRolloverSequenceNumber
        {
            get
            {
                return m_lastRolloverSequenceNumber;
            }
        }

        /// <summary>
        /// Adds data to the input queue that will be committed at the user defined interval
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public long WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            m_currentSequenceId++;
            activeFile.CreateIfNotExists();
            activeFile.Append(key1, key2, value1, value2);
            return m_currentSequenceId;
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                CommitAndRollover();
                m_disposed = true;
            }
        }

        public bool IsCommitted(long sequenceNumber)
        {
            return (sequenceNumber <= m_lastCommitedSequenceNumber);
        }

        public void Commit()
        {
            activeFile.RefreshSnapshot();
            m_lastCommitedSequenceNumber = m_currentSequenceId;
        }

        public void CommitAndRollover()
        {
            activeFile.RefreshAndRolloverFile(m_currentSequenceId);
            m_lastRolloverSequenceNumber = m_currentSequenceId;
            m_lastCommitedSequenceNumber = m_currentSequenceId;
        }

    }
}
