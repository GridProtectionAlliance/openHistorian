//******************************************************************************************************
//  LogFileWriter.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  11/12/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using GSF.IO;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Creates log files 
    /// </summary>
    public class LogFileWriter : IDisposable
    {
        private Queue<LogMessageSerializable> m_messageQueue;
        private object m_syncRoot;
        private string m_path;
        private FileStream m_stream;
        private LogSubscriber m_subscriber;
        private int m_logCount;
        private int m_maxQueue;

        /// <summary>
        /// Creates a LogFileWriter that initially queues message
        /// </summary>
        /// <param name="messageLimit">the number of messages to maintain</param>
        public LogFileWriter(int messageLimit)
        {
            m_logCount = 0;
            m_syncRoot = new object();
            m_maxQueue = messageLimit;
            m_messageQueue = new Queue<LogMessageSerializable>(messageLimit);
            m_subscriber = Logger.CreateSubscriber();
            m_subscriber.Subscribe(Logger.RootSource);
            m_subscriber.Subscribe(Logger.RootType);
            m_subscriber.Verbose = VerboseLevel.All ^ VerboseLevel.DebugLow;
            m_subscriber.Log += m_subscriber_Log;
        }

        /// <summary>
        /// writes log files to the provided directory.
        /// </summary>
        /// <param name="logDirectory"></param>
        public LogFileWriter(string logDirectory)
            : this(0)
        {
            SetPath(logDirectory);
        }

        ~LogFileWriter()
        {
            Dispose();
        }

        /// <summary>
        /// Gets or sets verbosity level for this <see cref="LogFileWriter"/>.
        /// </summary>
        public VerboseLevel Verbose
        {
            get
            {
                return m_subscriber.Verbose;
            }
            set
            {
                m_subscriber.Verbose = value;
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
                while (m_messageQueue.Count > 0)
                {
                    WriteLogMessage(m_messageQueue.Dequeue());
                }
            }
        }

        void m_subscriber_Log(LogMessage logMessage)
        {
            lock (m_syncRoot)
            {
                if (m_path == null)
                {
                    if (m_messageQueue.Count == m_maxQueue)
                        m_messageQueue.Dequeue();
                    m_messageQueue.Enqueue(new LogMessageSerializable(logMessage));
                }
                else
                {
                    WriteLogMessage(new LogMessageSerializable(logMessage));
                }
            }
        }

        void WriteLogMessage(LogMessageSerializable log)
        {
            if (m_logCount == 5000)
            {
                m_stream.Write(false);
                m_stream.Dispose();
                m_stream = null;
                m_logCount = 0;
            }
            if (m_stream == null)
            {
                m_stream = new FileStream(Path.Combine(m_path, DateTime.UtcNow.Ticks.ToString() + ".logbin"), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
            }
            m_stream.Write(true);
            log.Save(m_stream);
            m_logCount++;

        }

        public void Dispose()
        {
            lock (m_syncRoot)
            {
                if (m_stream != null)
                {
                    try
                    {
                        m_stream.Write(false);
                        m_stream.Dispose();

                    }
                    catch (Exception)
                    {

                    }
                    m_stream = null;
                    m_logCount = 0;
                }
            }
        }
    }
}
