//******************************************************************************************************
//  Logger_InternalType.cs - Gbtc
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
using System.ComponentModel;
using System.Threading;
using GSF.Collections;

namespace GSF.Diagnostics
{
    public static partial class Logger
    {
        /// <summary>
        /// This class is to help prevent improper use of <see cref="LogType"/>
        /// </summary>
        private class InternalType
            : LogType
        {
            private readonly int m_depth;
            protected readonly InternalType m_parent;
            protected WeakList<InternalSubscriber> m_subscribers;
            protected ConcurrentIndexedDictionary<string, InternalType> m_subTypes;

            /// <summary>
            /// Creates a root type.
            /// </summary>
            public InternalType()
            {
                Name = "";
                FullName = "";
                m_parent = null;
                m_depth = 0;
                Verbose = VerboseLevel.None;
            }

            public InternalType(string name, InternalType parent)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException("name", "cannot be null or an empty string");
                if (parent == null)
                    throw new ArgumentNullException("parent");

                Name = name;
                m_parent = parent;
                m_depth = parent.m_depth + 1;
                if (m_depth > 1)
                {
                    FullName = m_parent.FullName + "." + Name;
                }
                else
                {
                    FullName = Name;
                }
                Verbose = parent.Verbose;
            }

            internal void Subscribe(InternalSubscriber subscriber)
            {
                if (m_subscribers == null)
                {
                    Interlocked.CompareExchange(ref m_subscribers, new WeakList<InternalSubscriber>(), null);
                }
                m_subscribers.Add(subscriber);
                Logger.RefreshVerbose();
            }

            internal void Unsubscribe(InternalSubscriber subscriber)
            {
                if (m_subscribers == null)
                    return;
                m_subscribers.Remove(subscriber);
                Logger.RefreshVerbose();
            }

            /// <summary>
            /// Sends the specified message to all subscribers of this type and all of its
            /// parent's subscribers
            /// </summary>
            /// <param name="msg">the log message</param>
            internal void ProcessMessage(InternalMessage msg)
            {
                var logType = this;
                while (logType != null)
                {
                    if (logType.m_subscribers != null)
                    {
                        foreach (var subscriber in logType.m_subscribers)
                        {
                            if ((subscriber.Verbose & msg.Level) > 0)
                            {
                                subscriber.SetMessageFromType(msg);
                            }
                        }
                    }
                    logType = logType.m_parent;
                }
            }

            /// <summary>
            /// Raises a log message with the provided data.
            /// </summary>
            /// <param name="level">The verbose level associated with the message</param>
            /// <param name="eventName">A short name about what this message is detailing. Typically this will be a few words.</param>
            /// <param name="message"> A longer message than <see cref="eventName"/> giving more specifics about the actual message. 
            /// Typically, this will be up to 1 line of text.</param>
            /// <param name="details">A long text field with the details of the message.</param>
            /// <param name="exception">An exception object if one is provided.</param>
            public override void Publish(VerboseLevel level, string eventName, string message = null, string details = null, Exception exception = null)
            {
                if ((level & Verbose) == 0)
                    return;
                if (level == VerboseLevel.All)
                    throw new InvalidEnumArgumentException("level", -1, typeof(VerboseLevel));
                if (eventName == null)
                    throw new ArgumentNullException("eventName");

                if (message == null)
                    message = string.Empty;
                if (details == null)
                    details = string.Empty;

                var logMessage = new InternalMessage(level, InternalRootSource, this, eventName, message, details, exception);
                Logger.RaiseLogMessage(logMessage);
            }

            public void CalculateVerbose(VerboseLevel parentVerboseLevel)
            {
                if (m_subscribers != null)
                {
                    foreach (var sub in m_subscribers)
                    {
                        parentVerboseLevel |= sub.Verbose;
                    }
                }
                Verbose = parentVerboseLevel;
                if (m_subTypes != null)
                {
                    for (int x = 0; x < m_subTypes.Count; x++)
                    {
                        m_subTypes[x].CalculateVerbose(parentVerboseLevel);
                    }
                }
            }

            public InternalType GetOrAddNode(string name)
            {
                if (m_subTypes == null)
                {
                    Interlocked.CompareExchange(ref m_subTypes, new ConcurrentIndexedDictionary<string, InternalType>(), null);
                }
                return m_subTypes.GetOrAdd(name, () => new InternalType(name, this));
            }
        }
    }
}
