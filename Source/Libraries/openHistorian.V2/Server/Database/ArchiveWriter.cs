//******************************************************************************************************
//  ArchiveWriter.cs - Gbtc
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
using System.Diagnostics;
using System.Threading;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Responsible for getting data into the database. This class will prebuffer
    /// points and commit them in bulk operations.
    /// </summary>
    public class ArchiveWriter : IDisposable
    {
        //ToDo: In the event that gobs of points are added, it might be quicker to presort the values.
        //ToDo: Build in some kind of auto slowdown method if the disk is getting bogged down.

        ArchiveWriterSettings m_settings;

        volatile bool m_disposed;

        ArchiveList m_archiveList;

        ArchiveFile m_activeFile;

        PointQueue m_pointQueue;

        Thread m_insertThread;

        ManualResetEvent m_waitTimer;

        Stopwatch m_newFileCreationTime;

        Action<ArchiveFile> m_callbackFileComplete;

        /// <summary>
        /// Creates a new <see cref="ArchiveWriter"/>.
        /// </summary>
        /// <param name="settings">The settings for this class.</param>
        /// <param name="archiveList">The list used to attach newly created file.</param>
        /// <param name="callbackFileComplete">Once a file is complete with this layer, this callback is invoked</param>
        public ArchiveWriter(ArchiveWriterSettings settings, ArchiveList archiveList, Action<ArchiveFile> callbackFileComplete)
        {
            m_callbackFileComplete = callbackFileComplete;

            m_settings = settings;

            m_archiveList = archiveList;

            m_pointQueue = new PointQueue();

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
                m_waitTimer.WaitOne((int)m_settings.CommitOnInterval.TotalMilliseconds);
                m_waitTimer.Reset();

                bool timeToCloseCurrentFile = m_activeFile != null && (m_newFileCreationTime.Elapsed >= m_settings.NewFileOnInterval);
                BinaryStream stream;
                int pointCount;
                m_pointQueue.GetPointBlock(out stream, out pointCount);

                if (timeToCloseCurrentFile)
                {
                    m_newFileCreationTime.Reset(); 
                    if (m_activeFile != null)
                        m_callbackFileComplete(m_activeFile);
                    m_activeFile = null;
                }

                if (pointCount > 0) //If there is data to write
                {
                    if (m_activeFile == null) //Create a new file
                    {
                        var newFile = new ArchiveFile();
                        using (var edit = m_archiveList.AcquireEditLock())
                        {
                            //Add the newly created file.
                            edit.Add(newFile, true);
                        }
                        m_newFileCreationTime.Start();
                        m_activeFile = newFile;
                    }

                    using (var fileEditor = m_activeFile.BeginEdit())
                    {
                        while (pointCount > 0)
                        {
                            pointCount--;

                            ulong time = stream.ReadUInt64();
                            ulong id = stream.ReadUInt64();
                            ulong flags = stream.ReadUInt64();
                            ulong value = stream.ReadUInt64();

                            fileEditor.AddPoint(time, id, flags, value);
                        }
                        fileEditor.Commit();
                    }
                    using (var editor = m_archiveList.AcquireEditLock())
                    {
                        editor.RenewSnapshot(m_activeFile); //updates the current read transaction of this archive file.
                    }
                }
            }
        }

        /// <summary>
        /// Adds data to the input queue that will be committed at the user defined interval
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public void WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            m_pointQueue.WriteData(key1, key2, value1, value2);
        }

        /// <summary>
        /// Moves data from the queue and inserts it into the current archive
        /// ToDo: This function has a good use case, I just don't know what it is yet or should be called. Maybe a Flush or something like that
        /// </summary>
        public void SignalInitialInsert()
        {
            m_waitTimer.Set();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                SignalInitialInsert();
            }
        }



    }
}
