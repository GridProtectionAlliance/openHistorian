//******************************************************************************************************
//  LogReporter.cs - Gbtc
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

namespace GSF.Diagnostics
{
    /// <summary>
    /// Used to report logging events for a single source.
    /// </summary>
    public class LogReporter
    {
        /// <summary>
        /// The logger associated with this reporter
        /// </summary>
        public readonly Logger Logger;

        /// <summary>
        /// The source details
        /// </summary>
        public readonly LogSource LogSource;

        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ReportDebug;
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ReportInfo;
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ReportWarning;
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ReportError;
        /// <summary>
        /// Set by the subscribers to the log. Allows for the source to skip logging this entry
        /// as there are no subscribers to this verbose level.
        /// </summary>
        public bool ReportFatal;


        VerboseLevel m_verbose;

        /// <summary>
        /// Gets/Sets the verbose flags this log will report on.
        /// </summary>
        public VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            set
            {
                ReportDebug = (value & VerboseLevel.Debug) != 0;
                ReportInfo = (value & VerboseLevel.Information) != 0;
                ReportWarning = (value & VerboseLevel.Warning) != 0;
                ReportError = (value & VerboseLevel.Error) != 0;
                ReportFatal = (value & VerboseLevel.Fatal) != 0;
                m_verbose = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        /// <param name="classification"></param>
        /// <param name="getDetails"></param>
        /// <param name="parent"></param>
        internal LogReporter(Logger logger, object sender, string name, string classification, Func<string> getDetails = null, LogSource parent = null)
        {
            Logger = logger;
            LogSource = new LogSource(sender, name, classification, getDetails, parent);
        }

        /// <summary>
        /// Raises a log message with the provided data.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="eventId"></param>
        /// <param name="eventCode"></param>
        /// <param name="message"></param>
        /// <param name="values"></param>
        /// <param name="exception"></param>
        public void LogMessage(VerboseLevel level, int eventId, string eventCode, string message = null, string[] values = null, Exception exception = null)
        {
            if (message == null)
                message = string.Empty;
            
            var logMessage = new LogMessage();
            logMessage.Level = level;
            logMessage.Source = LogSource;
            logMessage.EventID = eventId;
            logMessage.EventCode = eventCode;
            logMessage.Message = message;
            logMessage.Values = values;
            logMessage.Exception = exception;
            Logger.RaiseMessage(logMessage);
        }

    }
}