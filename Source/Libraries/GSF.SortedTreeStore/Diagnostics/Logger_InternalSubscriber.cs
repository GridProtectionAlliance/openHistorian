//******************************************************************************************************
//  Logger_InternalSubscriber.cs - Gbtc
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
//  9/12/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GSF.Diagnostics
{
    public static partial class Logger
    {
        /// <summary>
        /// This class is to help prevent improper use of <see cref="LogMessage"/>
        /// </summary>
        private class InternalSubscriber
            : LogSubscriber
        {
            //ToDo: Use this concurrent queue to raise messages on their own thread.
            //ToDo: Possibly with a ScheduledTask.
            private ConcurrentQueue<InternalMessage> m_pendingMessages;
            private List<LogType> m_ignoreList;

            /// <summary>
            /// Creates a Log Subscriber
            /// </summary>
            public InternalSubscriber()
            {
                m_ignoreList = new List<LogType>();
                m_pendingMessages = new ConcurrentQueue<InternalMessage>();
                Logger.Register(this);
            }

            public override void Subscribe(LogSource source)
            {
                InternalSource src = source as InternalSource;
                if (src == null)
                    throw new ArgumentNullException("source", "Cannot be null and must be of type InternalSource");
                src.Subscribe(this);
            }

            public override void Unsubscribe(LogSource source)
            {
                InternalSource src = source as InternalSource;
                if (src == null)
                    throw new ArgumentNullException("source", "Cannot be null and must be of type InternalSource");
                src.Unsubscribe(this);
            }

            public override void Subscribe(LogType type)
            {
                InternalType typ = type as InternalType;
                if (typ == null)
                    throw new ArgumentNullException("type", "Cannot be null and must be of type InternalType");
                typ.Subscribe(this);
            }

            public override void Unsubscribe(LogType type)
            {
                InternalType typ = type as InternalType;
                if (typ == null)
                    throw new ArgumentNullException("type", "Cannot be null and must be of type InternalType");
                typ.Unsubscribe(this);
            }

            public override void AddIgnored(LogType type)
            {
                lock (m_ignoreList)
                {
                    m_ignoreList.Add(type);
                }
            }

            public override void RemoveIgnored(LogType type)
            {
                lock (m_ignoreList)
                {
                    m_ignoreList.Remove(type);
                }
            }

            protected override void OnVerboseChanged()
            {
                Logger.RefreshVerbose();
            }

            /// <summary>
            /// Messages to raise
            /// </summary>
            /// <param name="log"></param>
            void ProcessMessage(InternalMessage log)
            {
                OnLog(log);
            }

            private InternalMessage m_tempMessage;
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

            internal void SetMessageFromType(InternalMessage log)
            {
                if (!m_pending)
                    throw new Exception("Must call BeginLogMessage first");
                m_setByType = true;
                m_tempMessage = log;
            }

            internal void SetMessageFromObject(InternalMessage log)
            {
                m_setByObject = true;
                m_tempMessage = log;
            }

            internal void EndLogMessage()
            {
                if (m_tempMessage != null)
                {
                    if (m_ignoreList.Count == 0)
                    {
                        ProcessMessage(m_tempMessage);
                    }
                    else
                    {
                        bool suppress = false;
                        lock (m_ignoreList)
                        {
                            foreach (var type in m_ignoreList)
                            {
                                if (m_tempMessage.Type.IsChildOf(type))
                                {
                                    suppress = true;
                                    break;
                                }
                            }
                        }
                        if (!suppress)
                        {
                            ProcessMessage(m_tempMessage);
                        }
                    }

                    //m_pendingMessages.Enqueue(m_tempMessage);
                    m_tempMessage = null;
                }
                m_pending = false;
            }
        }
    }
}
