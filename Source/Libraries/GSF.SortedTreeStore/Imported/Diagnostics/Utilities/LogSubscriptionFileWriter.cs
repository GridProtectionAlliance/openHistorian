//******************************************************************************************************
//  LogSubscriptionFileWriter.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using GSF.IO;
using GSF.Threading;

namespace GSF.Diagnostics
{
    /// <summary>
    /// A log subscription that will write messages to a file
    /// </summary>
    public class LogSubscriptionFileWriter
        : IDisposable
    {
        public event Action<string> NewFileComplete;

        private readonly object m_syncRoot;
        private readonly ConcurrentQueue<LogMessage> m_messageQueue;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly LogSubscriber m_subscriber;
        private readonly int m_maxQueue;
        private string m_path;
        private VerboseLevel m_verbose;
        private LogFileWriter m_writer;
        private int m_maxFileCount;
        private Stopwatch m_fileAge = new Stopwatch();
        private ScheduledTask m_flushTask;

        /// <summary>
        /// Creates a LogFileWriter that initially queues message
        /// </summary>
        /// <param name="messageLimit">the number of messages to maintain</param>
        public LogSubscriptionFileWriter(int messageLimit)
        {
            m_syncRoot = new object();
            m_maxQueue = messageLimit;
            m_messageQueue = new ConcurrentQueue<LogMessage>();

            m_flushTask = new ScheduledTask();
            m_flushTask.Running += m_flushTask_Running;

            m_verbose = VerboseLevel.High;
            m_subscriber = Logger.CreateSubscriber(m_verbose);
            m_subscriber.SubscribeToAll(m_verbose);
            m_subscriber.NewLogMessage += SubscriberNewLogMessage;
        }

        private void m_flushTask_Running(object sender, EventArgs<ScheduledTaskRunningReason> e)
        {
            LogMessage message;
            bool hasWrittenLogMessage = false;
            while (m_messageQueue.TryDequeue(out message))
            {
                WriteLogMessage(message);
                hasWrittenLogMessage = true;
            }

            if (hasWrittenLogMessage)
                m_writer?.Flush();
        }

        /// <summary>
        /// Gets or sets verbosity level for this <see cref="LogSubscriptionFileWriter"/>.
        /// </summary>
        public VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            set
            {
                m_verbose = value;
                m_subscriber.SubscribeToAll(m_verbose);
            }
        }

        public void SetLoggingFileCount(int maxFileCount)
        {
            lock (m_syncRoot)
            {
                m_maxFileCount = maxFileCount;
            }
        }

        /// <summary>
        /// Sets the path of the log files.
        /// </summary>
        /// <param name="logDirectory"></param>
        public void SetPath(string logDirectory)
        {
            PathHelpers.ValidatePathName(logDirectory);
            lock (m_syncRoot)
            {
                m_path = logDirectory;
                m_flushTask.Start(1000);
            }
        }

        /// <summary>
        /// Sets log file writer path and optionally its verbosity level.
        /// </summary>
        public void SetPath(string logDirectory, VerboseLevel level)
        {
            PathHelpers.ValidatePathName(logDirectory);
            lock (m_syncRoot)
            {
                Verbose = level;
                m_path = logDirectory;
                m_flushTask.Start(1000);
            }
        }

        private void SubscriberNewLogMessage(LogMessage logMessage)
        {
            lock (m_syncRoot)
            {
                if (m_path == null)
                {
                    LogMessage message;
                    if (m_messageQueue.Count >= m_maxQueue)
                        m_messageQueue.TryDequeue(out message);
                    m_messageQueue.Enqueue(logMessage);
                }
                else
                {
                    m_messageQueue.Enqueue(logMessage);
                    m_flushTask.Start(1000);
                }
            }
        }

        private void WriteLogMessage(LogMessage log)
        {
            if (m_writer != null && (m_writer.LogCount >= 10000 || m_fileAge.Elapsed.TotalHours > 12))
            {
                string fileName = m_writer.FileName;
                m_writer.Dispose();
                m_writer = null;
                OnNewFileComplete(fileName);
            }

            if (m_writer == null)
            {
                m_fileAge.Restart();
                try
                {
                    if (m_maxFileCount > 0)
                    {
                        string[] files = Directory.GetFiles(m_path, "*.logz");
                        if (files.Length > m_maxFileCount)
                        {
                            Array.Sort(files);

                            for (int x = 0; x < files.Length - m_maxFileCount; x++)
                            {
                                File.Delete(files[x]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                var fileName = Path.Combine(m_path, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-ffffff") + ".logz");
                m_writer = new LogFileWriter(fileName);
            }

            m_writer.Write(log, false);
        }

        public void Dispose()
        {
            m_flushTask.Dispose();
            lock (m_syncRoot)
            {
                if (m_writer != null)
                {
                    string fileName = m_writer.FileName;
                    m_writer.Dispose();
                    m_writer = null;

                    OnNewFileComplete(fileName);
                }
            }
        }

        private void OnNewFileComplete(string fileName)
        {
            var e = NewFileComplete;
            if (e != null)
            {
                try
                {
                    e(fileName);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
