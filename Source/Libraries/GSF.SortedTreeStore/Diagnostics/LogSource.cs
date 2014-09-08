//******************************************************************************************************
//  LogSource.cs - Gbtc
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using GSF.Collections;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Identifies the source of log messages, along with its owning hirearchy if it exists.
    /// </summary>
    public class LogSource
    {
        /// <summary>
        /// A custom log message
        /// </summary>
        private class CustomSourceDetails
            : ILogSourceDetails
        {
            private string m_detailMessage;
            public CustomSourceDetails(string detailMessage)
            {
                m_detailMessage = detailMessage;
            }
            public string GetSourceDetails()
            {
                return m_detailMessage;
            }
        }

        private WeakReference m_source;

        /// <summary>
        /// A list of all subscribers to this publisher. Can be null.
        /// </summary>
        private WeakList<LogSubscriber> m_subscribers;

        /// <summary>
        /// A link to the log's parent so a stack trace can be computed. 
        /// <see cref="Root"/> if the parent supplied was null.
        /// </summary>
        public LogSource Parent { get; private set; }

        /// <summary>
        /// The name of the source type.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets the type of the log source.
        /// </summary>
        public LogType LogType { get; private set; }

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

        private VerboseLevel m_objectVerbose;
        private VerboseLevel m_verbose;
        private bool m_verboseIsValid;

        /// <summary>
        /// Gets the verbose level that this source should report on.
        /// </summary>
        internal VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            private set
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
        internal void Subscribe(LogSubscriber subscriber)
        {
            if (m_subscribers == null)
            {
                Interlocked.CompareExchange(ref m_subscribers, new WeakList<LogSubscriber>(), null);
            }
            m_subscribers.Add(subscriber);
            Logger.RefreshVerbose();
        }

        /// <summary>
        /// Removes the subscription to this publisher
        /// and all children.
        /// </summary>
        /// <param name="subscriber">the subscriber to remove.</param>
        internal void Unsubscribe(LogSubscriber subscriber)
        {
            if (m_subscribers == null)
                return;
            m_subscribers.Remove(subscriber);
            Logger.RefreshVerbose();
        }

        /// <summary>
        /// The object reference. <see cref="SourceCollectedObject"/> if the source
        /// has been collected.
        /// </summary>
        public object Source
        {
            get
            {
                return m_source.Target ?? SourceCollectedObject;
            }
        }

        /// <summary>
        /// Gets additional metadata about the source. 
        /// </summary>
        /// <returns></returns>
        public string GetDetails()
        {
            ILogSourceDetails details = Source as ILogSourceDetails;
            if ((object)details == null)
                return string.Empty;
            try
            {
                return details.GetSourceDetails();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Called by the static constructor to make the root object
        /// </summary>
        private LogSource()
        {
            Parent = null;
            LogType = LogType.Root;
            m_source = new WeakReference(RootObject);
            TypeName = GetType().FullName;
            CalculateVerbose();
            Logger.Register(this);
        }

        /// <summary>
        /// A source to report log details.
        /// </summary>
        /// <param name="source">The source object of the message. Cannot be null</param>
        /// <param name="parent">The parent source object. May be null.</param>
        /// <param name="logType">The type of the log. If null, the type of <see cref="source"/> is looked up.</param>
        public LogSource(object source, LogSource parent = null, LogType logType = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (parent == null)
                parent = Root;
            if (logType == null)
                logType = LogType.Lookup(source.GetType());

            Parent = parent;
            LogType = logType;
            m_source = new WeakReference(source);
            TypeName = source.GetType().FullName;
            CalculateVerbose();
            Logger.Register(this);
        }

        /// <summary>
        /// Gets a string representation of this source
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        public void AppendString(StringBuilder sb, bool stackTrace)
        {
            sb.Append("   at: ");
            sb.AppendLine(TypeName);
            if (stackTrace)
            {
                string details = GetDetails();
                if (details != string.Empty)
                {
                    sb.Append("      // ");
                    sb.AppendLine(details);
                }
                if (Parent != null)
                {
                    Parent.AppendString(sb, true);
                }
            }
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
        public void Publish(VerboseLevel level, string eventName, string message = null, string details = null, Exception exception = null)
        {
            if ((level & m_verbose) == 0)
                return;
            if (level == VerboseLevel.All)
                throw new InvalidEnumArgumentException("level", -1, typeof(VerboseLevel));
            if (eventName == null)
                throw new ArgumentNullException("eventName");

            if (message == null)
                message = string.Empty;
            if (details == null)
                details = string.Empty;

            var logMessage = new LogMessage(level, this, LogType, eventName, message, details, exception);
            Logger.RaiseLogMessage(logMessage);
        }


        #region [ Static ]

        // Strong references to these source details, as LogSource only maintains weak references.
        private static CustomSourceDetails RootObject;
        private static CustomSourceDetails SourceCollectedObject;

        /// <summary>
        /// Represents the root of all sources.
        /// </summary>
        public static LogSource Root { get; private set; }

        static LogSource()
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
            RootObject = new CustomSourceDetails("Root Object");
            SourceCollectedObject = new CustomSourceDetails("Source was garbage collected");
            Root = new LogSource();
        }


        #endregion
    }
}