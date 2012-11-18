//******************************************************************************************************
//  ArchiveMerger.cs - Gbtc
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database.ArchiveWriters
{
    /// <summary>
    /// Performs the required rollovers by reading partitions from the data list
    /// and combining them into a file of a later generation.
    /// </summary>
    public class ArchiveMerger : IDisposable
    {
        ArchiveRolloverSettings m_settings;

        ArchiveInitializer m_archiveInitializer;

        bool m_disposed;

        ArchiveList m_archiveList;

        Action<ArchiveFile, long> m_callbackFileComplete;

        long m_lastCommitedSequenceNumber;
        long m_lastRolloverSequenceNumber;
        Action<ArchiveListRemovalStatus> m_archivesPendingDeletion;
        ArchiveFile activeFile;

        /// <summary>
        /// Creates a new <see cref="ConcurrentArchiveMerger"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="archiveList">The list used to attach newly created file.</param>
        /// <param name="callbackFileComplete">Once a file is complete with this layer, this callback is invoked</param>
        /// <param name="archivesPendingDeletion">Where to pass archive files that are pending deletion</param>
        public ArchiveMerger(ArchiveRolloverSettings settings, ArchiveList archiveList, Action<ArchiveFile, long> callbackFileComplete, Action<ArchiveListRemovalStatus> archivesPendingDeletion)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (archiveList == null)
                throw new ArgumentNullException("archiveList");
            if (callbackFileComplete == null)
                throw new ArgumentNullException("callbackFileComplete");
            if (archivesPendingDeletion == null)
                throw new ArgumentNullException("archivesPendingDeletion");

            m_archivesPendingDeletion = archivesPendingDeletion;
            m_lastCommitedSequenceNumber = -1;
            m_lastRolloverSequenceNumber = -1;

            m_callbackFileComplete = callbackFileComplete;
            m_settings = settings;

            m_archiveList = archiveList;
            m_archiveInitializer = new ArchiveInitializer(settings.Initializer);

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

        public void Commit(ArchiveFile archiveFile, long sequenceId)
        {
            ArchiveFile fileToCombine = archiveFile;
            long pendingSequenceNumber = sequenceId;

            //Create a new file if need be
            if (activeFile == null)
            {
                var newFile = m_archiveInitializer.CreateArchiveFile();
                using (var edit = m_archiveList.AcquireEditLock())
                {
                    //Create a new file.
                    edit.Add(newFile, true);
                }
                activeFile = newFile;
            }

            ArchiveFileSummary summary = new ArchiveFileSummary(fileToCombine);
            ArchiveListRemovalStatus oldArchiveRemovalStatus;

            using (var src = summary.ActiveSnapshot.OpenInstance())
            {
                using (var fileEditor = activeFile.BeginEdit())
                {
                    var reader = src.GetDataRange();
                    reader.SeekToKey(0, 0);

                    ulong value1, value2, key1, key2;
                    while (reader.GetNextKey(out key1, out key2, out value1, out value2))
                    {
                        fileEditor.AddPoint(key1, key2, value1, value2);
                    }

                    fileEditor.Commit();
                    using (var editor = m_archiveList.AcquireEditLock())
                    {
                        editor.RenewSnapshot(activeFile);
                        editor.Remove(fileToCombine, out oldArchiveRemovalStatus);
                    }
                }
            }
            m_archivesPendingDeletion(oldArchiveRemovalStatus);
            m_lastCommitedSequenceNumber = pendingSequenceNumber;
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                Rollover();
                m_disposed = true;
            }
        }

        public bool IsCommitted(long sequenceNumber)
        {
            return (sequenceNumber <= m_lastCommitedSequenceNumber);
        }

        public void Rollover()
        {
            if (activeFile != null)
                m_callbackFileComplete(activeFile, m_lastCommitedSequenceNumber);
            activeFile = null;
            m_lastRolloverSequenceNumber = m_lastCommitedSequenceNumber;
        }



    }
}
