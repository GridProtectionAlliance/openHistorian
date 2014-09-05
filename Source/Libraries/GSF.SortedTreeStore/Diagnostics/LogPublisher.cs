//******************************************************************************************************
//  LogPublisher.cs - Gbtc
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

namespace GSF.Diagnostics
{
    /// <summary>
    /// Used to publish logging events.
    /// </summary>
    public class LogPublisher
    {
        /// <summary>
        /// The logger associated with this reporter
        /// </summary>
        public readonly Logger Logger;

        /// <summary>
        /// The source details
        /// </summary>
        public readonly LogPublisherDetails LogPublisherDetails;

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

        VerboseLevel m_verbose;

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
        /// A source to report log details.
        /// </summary>
        /// <param name="logger">The <see cref="Logger"/> that this reporter will write to.</param>
        /// <param name="source">The source object of the message.</param>
        /// <param name="parent">The parent source object. Can be null.</param>
        internal LogPublisher(Logger logger, object source, LogPublisherDetails parent)
        {
            Logger = logger;
            LogPublisherDetails = new LogPublisherDetails(source, parent, logger);
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

            var logMessage = new LogMessage(level, LogPublisherDetails, eventName, message, details, exception);
            Logger.RaiseMessage(logMessage);
        }

    }
}