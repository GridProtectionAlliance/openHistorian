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
using System.Text;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Identifies the source of log messages, along with its owning hirearchy if it exists.
    /// </summary>
    public abstract class LogSource
    {
        private WeakReference m_source;

        private VerboseLevel m_verbose;

        /// <summary>
        /// Creates a <see cref="LogSource"/>
        /// </summary>
        /// <param name="source"></param>
        protected LogSource(object source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            m_source = new WeakReference(source);
        }

        /// <summary>
        /// A link to the log's parent so a stack trace can be computed. 
        /// <see cref="Root"/> if the parent supplied was null.
        /// </summary>
        public LogSource Parent { get; protected set; }

        /// <summary>
        /// The name of the source type.
        /// </summary>
        public string TypeName
        {
            get
            {
                return LogType.FullName;
            }
        }

        /// <summary>
        /// Gets the type of the log source.
        /// </summary>
        public LogType LogType { get; protected set; }

        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishDebugLow { get; private set; }
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishDebugNormal { get; private set; }
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishDebugHigh { get; private set; }
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
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishBugReport { get; private set; }
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ShouldPublishPerformanceIssue { get; private set; }

        /// <summary>
        /// Gets the verbose level that this source should report on.
        /// </summary>
        public VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            protected set
            {
                ShouldPublishDebugLow = (value & VerboseLevel.DebugLow) != 0;
                ShouldPublishDebugNormal = (value & VerboseLevel.DebugNormal) != 0;
                ShouldPublishDebugHigh = (value & VerboseLevel.DebugHigh) != 0;
                ShouldPublishInfo = (value & VerboseLevel.Information) != 0;
                ShouldPublishWarning = (value & VerboseLevel.Warning) != 0;
                ShouldPublishError = (value & VerboseLevel.Error) != 0;
                ShouldPublishCritical = (value & VerboseLevel.Critical) != 0;
                ShouldPublishFatal = (value & VerboseLevel.Fatal) != 0;
                ShouldPublishBugReport = (value & VerboseLevel.BugReport) != 0;
                ShouldPublishPerformanceIssue = (value & VerboseLevel.PerformanceIssue) != 0;
                m_verbose = value;
            }
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
        /// Gets a string representation of this source
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        public void AppendString(StringBuilder sb, bool stackTrace)
        {
            if (ReferenceEquals(this, Logger.RootSource))
            {
                return;
            }

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
        /// Raises a log message with the provided data.
        /// </summary>
        /// <param name="level">The verbose level associated with the message</param>
        /// <param name="eventName">A short name about what this message is detailing. Typically this will be a few words.</param>
        /// <param name="message"> A longer message than <see cref="eventName"/> giving more specifics about the actual message. 
        /// Typically, this will be up to 1 line of text.</param>
        /// <param name="details">A long text field with the details of the message.</param>
        /// <param name="exception">An exception object if one is provided.</param>
        public abstract void Publish(VerboseLevel level, string eventName, string message = null, string details = null, Exception exception = null);

        /// <summary>
        /// A custom log message
        /// </summary>
        protected class CustomSourceDetails
            : ILogSourceDetails
        {
            private string m_detailMessage;
            /// <summary>
            /// Creates a <see cref="CustomSourceDetails"/>
            /// </summary>
            /// <param name="detailMessage">The message to return when <see cref="GetSourceDetails"/> is called </param>
            public CustomSourceDetails(string detailMessage)
            {
                m_detailMessage = detailMessage;
            }

            /// <summary>
            /// The source message
            /// </summary>
            /// <returns></returns>
            public string GetSourceDetails()
            {
                return m_detailMessage;
            }
        }

        // Strong references to these source details, as LogSource only maintains weak references.
        protected static CustomSourceDetails RootObject;
        protected static CustomSourceDetails SourceCollectedObject;

        static LogSource()
        {
            RootObject = new CustomSourceDetails("Root Object");
            SourceCollectedObject = new CustomSourceDetails("Source was garbage collected");
        }

    }
}