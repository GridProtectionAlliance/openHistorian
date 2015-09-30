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

namespace GSF.Diagnostics
{
    /// <summary>
    /// The type associated with a log message.
    /// </summary>
    public abstract class LogType
    {
        /// <summary>
        /// Gets the name of the type (or namespace)
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the full name of the type
        /// </summary>
        public string FullName { get; protected set; }

        private VerboseLevel m_verbose;

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
        /// Gets the full name of the type.
        /// </summary>
        /// <returns></returns>
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
        public abstract void Publish(VerboseLevel level, string eventName, string message = null, string details = null, Exception exception = null);

        /// <summary>
        /// Gets if this type is a child of the specified parent.
        /// </summary>
        /// <param name="parentType"></param>
        /// <returns>
        /// True if it is a child. Also true if the object and <see cref="parentType"/> are the same object.
        /// </returns>
        public abstract bool IsChildOf(LogType parentType);

    }

}
