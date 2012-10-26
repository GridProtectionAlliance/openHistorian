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

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Performs the required rollovers by reading partitions from the data list
    /// and combining them into a file of a later generation.
    /// </summary>
    public class ArchiveManagement
    {
        ArchiveRolloverSettings m_settings;

        ArchiveInitializer m_archiveInitializer;

        volatile bool m_disposed;

        ArchiveList m_archiveList;

        ArchiveFile m_activeFile;

        Thread m_insertThread;

        ManualResetEvent m_waitTimer;

        Stopwatch m_newFileCreationTime;

        ConcurrentQueue<ArchiveFile> m_filesToProcess;

        Action<ArchiveFile> m_callbackFileComplete;

        /// <summary>
        /// Creates a new <see cref="ArchiveManagement"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="archiveList">The list used to attach newly created file.</param>
        /// <param name="callbackFileComplete">Once a file is complete with this layer, this callback is invoked</param>
        public ArchiveManagement(ArchiveRolloverSettings settings, ArchiveList archiveList, Action<ArchiveFile> callbackFileComplete)
        {
            m_filesToProcess=new ConcurrentQueue<ArchiveFile>();
            m_callbackFileComplete = callbackFileComplete;
            m_settings = settings;

            m_archiveList = archiveList;
            m_archiveInitializer = new ArchiveInitializer(settings.Initializer);

            m_newFileCreationTime = new Stopwatch();

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
                if (m_filesToProcess.Count == 0)
                    m_waitTimer.WaitOne(1000);
                m_waitTimer.Reset();


                bool timeToCloseCurrentFile = m_activeFile != null &&
                                              ((m_newFileCreationTime.Elapsed >= m_settings.NewFileOnInterval) ||
                                              (m_activeFile.FileSize >= m_settings.NewFileOnSize));

                //Create a new file if need be
                if (timeToCloseCurrentFile)
                {
                    m_newFileCreationTime.Reset();
                    if (m_activeFile != null)
                        m_callbackFileComplete(m_activeFile);
                    m_activeFile = null;
                }

                ArchiveFile fileToCombine;
                if (m_filesToProcess.TryDequeue(out fileToCombine))
                {

                    //Create a new file if need be
                    if (m_activeFile == null)
                    {
                        m_newFileCreationTime.Start();
                        var newFile = m_archiveInitializer.CreateArchiveFile();
                        using (var edit = m_archiveList.AcquireEditLock())
                        {
                            //Create a new file.
                            edit.Add(newFile, true);
                        }
                        m_activeFile = newFile;
                    }

                    ArchiveFileSummary summary = new ArchiveFileSummary(fileToCombine);

                    using (var src = summary.ActiveSnapshot.OpenInstance())
                    {
                        using (var fileEditor = m_activeFile.BeginEdit())
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
                                editor.RenewSnapshot(m_activeFile);
                            }
                        }
                    }
                    using (var editor = m_archiveList.AcquireEditLock())
                    {
                        editor.RenewSnapshot(m_activeFile);
                        ArchiveListRemovalStatus status;
                        if (editor.Remove(fileToCombine, out status))
                        {
                            //ToDo: Do something with this removal status
                        }
                    }
                }
            }
        }

        public void ProcessArchive(ArchiveFile archiveFile)
        {
            m_filesToProcess.Enqueue(archiveFile);
            SignalProcessRollover();
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
