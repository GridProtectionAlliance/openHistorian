////******************************************************************************************************
////  ArchiveWriter.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  5/29/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading;
//using openHistorian.IO.Unmanaged;
//using openHistorian.Server.Configuration;
//using openHistorian.Server.Database.Archive;

//namespace openHistorian.Server.Database
//{
//    /// <summary>
//    /// Responsible for getting data into the database. This class will prebuffer
//    /// points and commit them in bulk operations.
//    /// </summary>
//    public class ArchiveWriter : IDisposable
//    {
//        /// <summary>
//        /// Provides a way to block a thread until data has been committed to the archive writer.
//        /// </summary>
//        class WaitingForCommit
//        {
//            /// <summary>
//            /// The wait handle to signaled
//            /// </summary>
//            public ManualResetEvent Wait { get; private set; }
//            /// <summary>
//            /// The desired sequence number to wait for.
//            /// </summary>
//            public long SequenceNumberToWaitFor { get; private set; }
//            /// <summary>
//            /// Determines if the wait is for a commit, or for a successful rollover.
//            /// </summary>
//            public bool WaitForRollover { get; private set; }
//            /// <summary>
//            /// Value will be set to true if the condition was met.
//            /// False will only occur if the writer is closed before conditions were met.
//            /// </summary>
//            public bool Successful { get; set; }

//            public WaitingForCommit(long seqenceNumber, bool waitForRollover)
//            {
//                Successful = false;
//                WaitForRollover = waitForRollover;
//                SequenceNumberToWaitFor = seqenceNumber;
//                Wait = new ManualResetEvent(false);
//            }
//        }

//        //ToDo: In the event that gobs of points are added, it might be quicker to presort the values.
//        //ToDo: Build in some kind of auto slowdown method if the disk is getting bogged down.

//        ArchiveWriterSettings m_settings;

//        long m_lastCommitedSequenceNumber;
//        long m_lastRolloverSequenceNumber;
//        bool m_disposed;
//        bool m_forceNewFile;
//        bool m_forceQuit;
//        bool m_threadHasQuit;

//        ArchiveList m_archiveList;

//        ArchiveFile m_activeFile;

//        PointQueue m_pointQueue;

//        Thread m_insertThread;

//        ManualResetEvent m_waitTimer;

//        Action<ArchiveFile> m_callbackFileComplete;

//        object m_syncRoot;

//        List<WaitingForCommit> m_pendingCommitRequests;

//        /// <summary>
//        /// Creates a new <see cref="ArchiveWriter"/>.
//        /// </summary>
//        /// <param name="settings">The settings for this class.</param>
//        /// <param name="archiveList">The list used to attach newly created file.</param>
//        /// <param name="callbackFileComplete">Once a file is complete with this layer, this callback is invoked</param>
//        public ArchiveWriter(ArchiveWriterSettings settings, ArchiveList archiveList, Action<ArchiveFile> callbackFileComplete)
//        {
//            m_pendingCommitRequests = new List<WaitingForCommit>();
//            m_syncRoot = new object();
//            m_lastCommitedSequenceNumber = -1;
//            m_lastRolloverSequenceNumber = -1;

//            m_callbackFileComplete = callbackFileComplete;

//            m_settings = settings;

//            m_archiveList = archiveList;

//            m_pointQueue = new PointQueue();

//            m_waitTimer = new ManualResetEvent(false);
//            m_insertThread = new Thread(ProcessInsertingData);
//            m_insertThread.Start();
//        }

//        /// <summary>
//        /// This is executed by a dedicated thread and moves data from the point queue to the database.
//        /// </summary>
//        void ProcessInsertingData()
//        {
//            Stopwatch newFileCreationTime = new Stopwatch();
//            Stopwatch lastCommitTime = new Stopwatch();

//            bool forcedQuit = false;
//            while (!forcedQuit)
//            {
//                long pendingSequenceNumber;

//                double waitForNewFile = (m_settings.NewFileOnInterval - newFileCreationTime.Elapsed).TotalMilliseconds;
//                double waitForNextCommitWindow = (m_settings.CommitOnInterval - lastCommitTime.Elapsed).TotalMilliseconds;
//                double waitTime = Math.Min(waitForNewFile, waitForNextCommitWindow);
//                if (waitForNewFile < waitForNextCommitWindow)
//                {
//                    waitTime = waitTime;
//                }
//                if (waitTime > 1)
//                {
//                    m_waitTimer.WaitOne((int)Math.Ceiling(waitTime));
//                    m_waitTimer.Reset();
//                }
//                lastCommitTime.Restart();

//                bool forcedNewFile;

//                lock (m_syncRoot)
//                {
//                    forcedNewFile = m_forceNewFile;
//                    forcedQuit = m_forceQuit;

//                    m_forceNewFile = false;
//                }

//                BinaryStream stream;
//                int pointCount;

//                if (forcedQuit)
//                {
//                    m_pointQueue.GetPointBlockAndStop(out stream, out pointCount, out pendingSequenceNumber);
//                }
//                else
//                {
//                    m_pointQueue.GetPointBlock(out stream, out pointCount, out pendingSequenceNumber);
//                }

//                //If there is data to write then write it to the current archive.
//                if (pointCount > 0)
//                {
//                    //Create a new file if the current file is null
//                    //Also start the new archive file timer
//                    if (m_activeFile == null)
//                    {
//                        var newFile = new ArchiveFile();
//                        using (var edit = m_archiveList.AcquireEditLock())
//                        {
//                            //Add the newly created file.
//                            edit.Add(newFile, true);
//                        }
//                        newFileCreationTime.Start();
//                        m_activeFile = newFile;
//                    }

//                    //Write data to the current archive file.
//                    using (var fileEditor = m_activeFile.BeginEdit())
//                    {
//                        while (pointCount > 0)
//                        {
//                            pointCount--;

//                            ulong time = stream.ReadUInt64();
//                            ulong id = stream.ReadUInt64();
//                            ulong flags = stream.ReadUInt64();
//                            ulong value = stream.ReadUInt64();

//                            fileEditor.AddPoint(time, id, flags, value);
//                        }
//                        fileEditor.Commit();
//                    }

//                    //Refresh the read snapshot on the archive file
//                    using (var editor = m_archiveList.AcquireEditLock())
//                    {
//                        editor.RenewSnapshot(m_activeFile); //updates the current read transaction of this archive file.
//                    }
//                }

//                bool timeToCloseCurrentFile = ((m_settings.NewFileOnInterval - newFileCreationTime.Elapsed).TotalMilliseconds < 5);
//                if (timeToCloseCurrentFile || forcedNewFile || forcedQuit)
//                {
//                    newFileCreationTime.Reset();
//                    if (m_activeFile != null)
//                        m_callbackFileComplete(m_activeFile);
//                    m_activeFile = null;
//                }

//                //Release any pending wait locks
//                lock (m_syncRoot)
//                {
//                    if (m_activeFile == null)
//                        m_lastRolloverSequenceNumber = pendingSequenceNumber;
//                    m_lastCommitedSequenceNumber = pendingSequenceNumber;

//                    int x = m_pendingCommitRequests.Count - 1;
//                    while (x >= 0)
//                    {
//                        var pending = m_pendingCommitRequests[x];
//                        if (pending.WaitForRollover)
//                        {
//                            if (pending.SequenceNumberToWaitFor <= m_lastRolloverSequenceNumber)
//                            {
//                                pending.Successful = true;
//                                pending.Wait.Set();
//                                m_pendingCommitRequests.RemoveAt(x);
//                            }
//                        }
//                        else
//                        {
//                            if (pending.SequenceNumberToWaitFor <= m_lastCommitedSequenceNumber)
//                            {
//                                pending.Successful = true;
//                                pending.Wait.Set();
//                                m_pendingCommitRequests.RemoveAt(x);
//                            }
//                        }
//                        x--;
//                    }
//                    //If we need to quit, then signal all remaining waits as unsuccessful.
//                    if (forcedQuit)
//                    {
//                        m_threadHasQuit = forcedQuit;
//                        foreach (var pending in m_pendingCommitRequests)
//                        {
//                            pending.Successful = false;
//                            pending.Wait.Set();
//                        }
//                        m_pendingCommitRequests = null;
//                    }
//                }
//            }
//        }

//        public long LastCommittedSequenceNumber
//        {
//            get
//            {
//                lock (m_syncRoot)
//                {
//                    return m_lastCommitedSequenceNumber;
//                }
//            }
//        }
//        public long LastRolloverSequenceNumber
//        {
//            get
//            {
//                lock (m_syncRoot)
//                {
//                    return m_lastRolloverSequenceNumber;
//                }
//            }
//        }

//        /// <summary>
//        /// Adds data to the input queue that will be committed at the user defined interval
//        /// </summary>
//        /// <param name="key1"></param>
//        /// <param name="key2"></param>
//        /// <param name="value1"></param>
//        /// <param name="value2"></param>
//        public long WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
//        {
//            return m_pointQueue.WriteData(key1, key2, value1, value2);
//        }

//        /// <summary>
//        /// Moves data from the queue and inserts it into the current archive
//        /// </summary>
//        void SignalInitialInsert()
//        {
//            m_waitTimer.Set();
//        }

//        public void Dispose()
//        {
//            if (!m_disposed)
//            {
//                StopExecution();
//                m_pointQueue.Dispose();
//                m_disposed = true;
//            }
//        }

//        public bool IsCommitted(long sequenceNumber)
//        {
//            lock (m_syncRoot)
//            {
//                return (sequenceNumber <= m_lastCommitedSequenceNumber);
//            }
//        }

//        public bool WaitForCommit(long sequenceId, bool startImediately)
//        {
//            WaitingForCommit waiting;
//            lock (m_syncRoot)
//            {
//                if (sequenceId <= m_lastCommitedSequenceNumber)
//                    return true;
//                if (m_threadHasQuit)
//                    return false;
//                waiting = new WaitingForCommit(sequenceId, false);
//                m_pendingCommitRequests.Add(waiting);
//            }
//            if (startImediately)
//                SignalInitialInsert();
//            waiting.Wait.WaitOne();
//            return waiting.Successful;
//        }

//        public bool WaitForRollover(long sequenceId, bool startImediately)
//        {
//            WaitingForCommit waiting;
//            lock (m_syncRoot)
//            {
//                if (sequenceId <= m_lastRolloverSequenceNumber)
//                    return true;
//                if (m_threadHasQuit)
//                    return false;
//                waiting = new WaitingForCommit(sequenceId, true);
//                m_pendingCommitRequests.Add(waiting);
//                if (startImediately)
//                    m_forceNewFile = true;
//            }
//            if (startImediately)
//                SignalInitialInsert();
//            waiting.Wait.WaitOne();
//            return waiting.Successful;
//        }

//        public void Commit()
//        {
//            long sequenceId = m_pointQueue.SequenceId;
//            WaitForCommit(sequenceId, true);
//        }

//        public void CommitAndRollover()
//        {
//            long sequenceId = m_pointQueue.SequenceId;
//            WaitForRollover(sequenceId, true);
//        }

//        public void StopExecution()
//        {
//            lock (m_syncRoot)
//            {
//                m_forceQuit = true;
//            }
//            SignalInitialInsert();
//            m_insertThread.Join();
//        }

//        public void CommitNoWait()
//        {
//            SignalInitialInsert();
//        }

//        public void CommitAndRolloverNoWait()
//        {
//            lock (m_syncRoot)
//            {
//                m_forceNewFile = true;
//            }
//            SignalInitialInsert();
//        }



//    }
//}
