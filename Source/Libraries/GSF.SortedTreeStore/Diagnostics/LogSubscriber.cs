//******************************************************************************************************
//  LogSubscriber.cs - Gbtc
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
//  4/11/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Handles or subscribes to log events.
    /// </summary>
    public class LogSubscriber
    {
        /// <summary>
        /// Event handler for the logs that are raised
        /// </summary>
        public event LogEventHandler Log;

        private ConcurrentQueue<LogMessage> m_pendingMessages;

        VerboseLevel m_verbose;

        /// <summary>
        /// Creates a Log Subscriber
        /// </summary>
        public LogSubscriber()
        {
            m_pendingMessages = new ConcurrentQueue<LogMessage>();
            Logger.Register(this);
        }

        public void Subscribe(LogSource source)
        {
            source.Subscribe(this);
        }

        public void Unsubscribe(LogSource source)
        {
            source.Unsubscribe(this);
        }

        public void Subscribe(LogType type)
        {
            type.Subscribe(this);
        }

        public void Unsubscribe(LogType type)
        {
            type.Unsubscribe(this);
        }

        /// <summary>
        /// Gets/Sets the verbose level of this subscriber
        /// </summary>
        public VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            set
            {
                if (m_verbose != value)
                {
                    m_verbose = value;
                    Logger.RefreshVerbose();
                }
            }
        }

        /// <summary>
        /// Messages to raise
        /// </summary>
        /// <param name="log"></param>
        void ProcessMessage(LogMessage log)
        {
            if ((log.Level & m_verbose) == 0)
                return;

            try
            {
                if (Log != null)
                    Log(log);
            }
            catch (Exception ex)
            {
                ex = ex;
            }
        }

        private LogMessage m_tempMessage;
        private bool m_setByType;
        private bool m_setByObject;
        private bool m_pending;

        internal void BeginLogMessage()
        {
            m_tempMessage = null;
            m_setByType = false;
            m_setByObject = false;
            m_pending = true;
        }

        internal void SetMessageFromType(LogMessage log)
        {
            if (!m_pending)
                throw new Exception("Must call BeginLogMessage first");
            m_setByType = true;
            m_tempMessage = log;
        }

        internal void SetMessageFromObject(LogMessage log)
        {
            m_setByObject = true;
            m_tempMessage = log;
        }

        internal void EndLogMessage()
        {
            if (m_tempMessage != null)
            {
                ProcessMessage(m_tempMessage);
                //m_pendingMessages.Enqueue(m_tempMessage);
                m_tempMessage = null;
            }
            m_pending = false;
        }


    }
}