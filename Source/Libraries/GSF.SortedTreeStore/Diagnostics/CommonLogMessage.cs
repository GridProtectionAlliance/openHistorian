//******************************************************************************************************
//  CommonLogMessage.cs - Gbtc
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
//  9/20/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Diagnostics;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Certain log messages that can be raised many times can use this helper class to
    /// handle how often the log message can be raised
    /// </summary>
    public class CommonLogMessage
    {
        private LogSource m_source;
        private Stopwatch m_timeSinceLastPublish;
        private TimeSpan m_maximumPublishInterval;

        /// <summary>
        /// Creates a <see cref="CommonLogMessage"/>
        /// </summary>
        /// <param name="log"></param>
        /// <param name="maximumPublishInterval"></param>
        public CommonLogMessage(LogSource log, TimeSpan maximumPublishInterval)
        {
            m_source = log;
            m_maximumPublishInterval = maximumPublishInterval;
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
            bool shouldPublish = false;
            if (m_timeSinceLastPublish == null)
            {
                shouldPublish = true;
                m_timeSinceLastPublish = new Stopwatch();
                m_timeSinceLastPublish.Restart();
            }

            if (shouldPublish || m_timeSinceLastPublish.Elapsed > m_maximumPublishInterval)
            {
                m_timeSinceLastPublish.Restart();
                m_source.Publish(level, eventName, message, details, exception);
            }
        }
    }
}
