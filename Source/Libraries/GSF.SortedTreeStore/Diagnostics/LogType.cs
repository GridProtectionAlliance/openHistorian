//******************************************************************************************************
//  LogType.cs - Gbtc
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
//  09/06/2014 - Steven E. Chisholm
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
    /// <summary>
    /// The type definition of a log source.
    /// </summary>
    public class LogType
    {
        public readonly string Name;
        public readonly string FullName;

        private readonly int m_depth;
        private readonly LogType m_parent;
        private WeakList<LogSubscriber> m_subscribers;
        private ConcurrentIndexedDictionary<string, LogType> m_subTypes;
        private VerboseLevel m_verbose;

        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishDebug { get; private set; }
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishInfo { get; private set; }
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishWarning { get; private set; }
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishError { get; private set; }
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishCritical { get; private set; }
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishFatal { get; private set; }

        /// <summary>
        /// Gets/Sets the verbose flags this log will report on.
        /// </summary>
        internal VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            set
            {
                ShouldPublishDebug = (value & VerboseLevel.Debug) != 0;
                ShouldPublishInfo = (value & VerboseLevel.Information) != 0;
                ShouldPublishWarning = (value & VerboseLevel.Warning) != 0;
                ShouldPublishError = (value & VerboseLevel.Error) != 0;
                ShouldPublishCritical = (value & VerboseLevel.Critical) != 0;
                ShouldPublishFatal = (value & VerboseLevel.Fatal) != 0;
                m_verbose = value;
            }
        }

        /// <summary>
        /// Calculates a new verbose value.
        /// </summary>
        private void CalculateVerbose(VerboseLevel parentVerboseLevel)
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

        private LogType(string name, LogType parent)
        {
            Name = name;
            m_parent = parent;
            if (parent == null)
            {
                m_depth = 0;
                FullName = "";
                Verbose = VerboseLevel.None;
            }
            else
            {
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
        }

        internal void Subscribe(LogSubscriber subscriber)
        {
            if (m_subscribers == null)
            {
                Interlocked.CompareExchange(ref m_subscribers, new WeakList<LogSubscriber>(), null);
            }
            m_subscribers.Add(subscriber);
            Logger.RefreshVerbose();
        }

        internal void Unsubscribe(LogSubscriber subscriber)
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
        internal void ProcessMessage(LogMessage msg)
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

        private LogType GetOrAddNode(string name)
        {
            if (m_subTypes == null)
            {
                Interlocked.CompareExchange(ref m_subTypes, new ConcurrentIndexedDictionary<string, LogType>(), null);
            }
            return m_subTypes.GetOrAdd(name, () => new LogType(name, this));
        }

        public override string ToString()
        {
            return FullName;
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
        public void Publish(VerboseLevel level, string eventName, string message = null, string details = null, Exception exception = null)
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

            var logMessage = new LogMessage(level, LogSource.Root, this, eventName, message, details, exception);
            Logger.RaiseLogMessage(logMessage);
        }

        #region [ Static ]

        /// <summary>
        /// Gets the root type for all types.
        /// </summary>
        public static LogType Root { get; private set; }

        static LogType()
        {
            Logger.Initialize();
        }

        /// <summary>
        /// Due to inter static dependencies, we must initialize 
        /// <see cref="Logger"/>, <see cref="LogType"/>, and <see cref="LogSource"/>
        /// in a controlled manner.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static void Initialize()
        {
            Root = new LogType("", null);
        }

        /// <summary>
        /// Looks up the type of the log source
        /// </summary>
        /// <param name="type">the type</param>
        /// <returns></returns>
        public static LogType Lookup(Type type)
        {
            string name = type.AssemblyQualifiedName;

            return Lookup(name);
        }

        /// <summary>
        /// Looks up the type of the log source
        /// </summary>
        /// <param name="name">the string name of the type</param>
        /// <returns></returns>
        public static LogType Lookup(string name)
        {
            int newLength = name.Length;
            int indexOfBracket = name.IndexOf('[');
            int indexOfComma = name.IndexOf(',');

            if (indexOfBracket >= 0)
            {
                newLength = Math.Min(indexOfBracket, newLength);
            }
            if (indexOfComma >= 0)
            {
                newLength = Math.Min(indexOfComma, newLength);
            }
            name = name.Substring(0, newLength).Trim();

            string[] parts = name.Split('.', '+');

            var current = Root;
            foreach (var s in parts)
            {
                current = current.GetOrAddNode(s);
            }
            return current;
        }

        /// <summary>
        /// Method should only be called from <see cref="Logger.RefreshVerbose"/>
        /// </summary>
        internal static void RefreshVerbose()
        {
            Root.CalculateVerbose(VerboseLevel.None);
        }

        #endregion

    }

}
