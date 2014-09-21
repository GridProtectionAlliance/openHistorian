//******************************************************************************************************
//  Logger_InternalSource.cs - Gbtc
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
using System.Runtime.CompilerServices;
using System.Threading;
using GSF.Collections;

namespace GSF.Diagnostics
{
    public static partial class Logger
    {
        /// <summary>
        /// This class is to help prevent improper use of <see cref="LogSource"/>
        /// </summary>
        private class InternalSource
            : LogSource
        {
            /// <summary>
            /// A list of all subscribers to this publisher. Can be null.
            /// </summary>
            private WeakList<InternalSubscriber> m_subscribers;

            private VerboseLevel m_objectVerbose;
            private bool m_verboseIsValid;

            /// <summary>
            /// Creates a root source
            /// </summary>
            public InternalSource(InternalType rootType)
                : base(RootObject)
            {
                base.Parent = null;
                base.LogType = rootType;
                Logger.Register(this);
            }

            /// <summary>
            /// A source to report log details.
            /// </summary>
            /// <param name="source">The source object of the message. Cannot be null</param>
            /// <param name="parent">The parent source object. May be null.</param>
            /// <param name="logType">The type of the log. If null, the type of <see cref="source"/> is looked up.</param>
            public InternalSource(object source, InternalSource parent, InternalType logType)
                : base(source)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");
                if (logType == null)
                    throw new ArgumentNullException("logType");

                base.Parent = parent;
                base.LogType = logType;
                CalculateVerbose();
                Logger.Register(this);
            }

            private new InternalSource Parent
            {
                get
                {
                    return base.Parent as InternalSource;
                }
            }

            private new InternalType LogType
            {
                get
                {
                    return base.LogType as InternalType;
                }
            }


            /// <summary>
            /// Invalidates the current verbose cache so it can be recalculated
            /// </summary>
            /// <remarks>
            /// All <see cref="LogSource"/>s must be invalidated before recomputing
            /// the verbose level.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void BeginRecalculateVerbose()
            {
                m_verboseIsValid = false;
            }

            /// <summary>
            /// Refreshes the current verbose cache.
            /// </summary>
            /// <remarks>
            /// <see cref="BeginRecalculateVerbose"/> must be called
            /// on all sources before it can be ended.
            /// It is assumed that <see cref="LogType"/> has already 
            /// had its verbose level recalculated. If this is false
            /// the verbose level may not be corrent.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void EndRecalculateVerbose()
            {
                if (!m_verboseIsValid)
                    CalculateVerbose();
            }

            /// <summary>
            /// Calculates a new verbose value.
            /// </summary>
            private void CalculateVerbose()
            {
                VerboseLevel level = VerboseLevel.None;
                if (m_subscribers != null)
                {
                    foreach (var sub in m_subscribers)
                    {
                        level |= sub.Verbose;
                    }
                }
                if (Parent != null)
                {
                    if (!Parent.m_verboseIsValid)
                        Parent.CalculateVerbose();
                    level |= Parent.m_objectVerbose;
                }
                m_objectVerbose = level;
                Verbose = level | LogType.Verbose;
                m_verboseIsValid = true;
            }


            /// <summary>
            /// Subscribes to logs that are generated from this publisher
            /// and all children.
            /// </summary>
            /// <param name="subscriber">the subscriber listening to the events.</param>
            internal void Subscribe(InternalSubscriber subscriber)
            {
                if (m_subscribers == null)
                {
                    Interlocked.CompareExchange(ref m_subscribers, new WeakList<InternalSubscriber>(), null);
                }
                m_subscribers.Add(subscriber);
                Logger.RefreshVerbose();
            }

            /// <summary>
            /// Removes the subscription to this publisher
            /// and all children.
            /// </summary>
            /// <param name="subscriber">the subscriber to remove.</param>
            internal void Unsubscribe(InternalSubscriber subscriber)
            {
                if (m_subscribers == null)
                    return;
                m_subscribers.Remove(subscriber);
                Logger.RefreshVerbose();
            }

            /// <summary>
            /// Raises a log message to all subscribers of the message.
            /// </summary>
            /// <param name="msg"></param>
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
                                subscriber.SetMessageFromObject(msg);
                            }
                        }
                    }
                    logType = logType.Parent;
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

                var logMessage = new InternalMessage(level, this, LogType, eventName, message, details, exception);
                Logger.RaiseLogMessage(logMessage);
            }
        }
    }
}
