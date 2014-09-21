//******************************************************************************************************
//  LogMessage.cs - Gbtc
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
using System.Text;

namespace GSF.Diagnostics
{
    /// <summary>
    /// An individual log message.
    /// </summary>
    public abstract class LogMessage
    {
        /// <summary>
        /// The time that the message was created.
        /// </summary>
        public readonly DateTime UtcTime;
        /// <summary>
        /// The verbose level associated with the message
        /// </summary>
        public readonly VerboseLevel Level;
        /// <summary>
        /// The source of the log message
        /// </summary>
        public readonly LogSource Source;
        /// <summary>
        /// The type of the log message. A message will be raised on this type, not on Source.Type
        /// </summary>
        public readonly LogType Type;
        /// <summary>
        /// A short name about what this message is detailing. Typically this will be a few words.
        /// </summary>
        public readonly string EventName;
        /// <summary>
        /// A longer message than <see cref="EventName"/> giving more specifics about the actual message. 
        /// Typically, this will be up to 1 line of text. Can be String.Empty.
        /// </summary>
        public readonly string Message;
        /// <summary>
        /// A long text field with the details of the message. Can be String.Empty.
        /// </summary>
        public readonly string Details;
        /// <summary>
        /// An exception object if one is provided.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// Creates a log message
        /// </summary>
        /// <param name="level"></param>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="details"></param>
        /// <param name="exception"></param>
        protected LogMessage(VerboseLevel level, LogSource source, LogType type, string eventName, string message, string details, Exception exception)
        {
            if (level == VerboseLevel.None || level == VerboseLevel.All)
                throw new InvalidEnumArgumentException("level", (int)level, typeof(VerboseLevel));
            if (type == null)
                throw new ArgumentNullException("type");
            if (source == null)
                throw new ArgumentNullException("source");
            if (eventName == null)
                throw new ArgumentNullException("eventName");
            if (message == null)
                throw new ArgumentNullException("message");
            if (details == null)
                throw new ArgumentNullException("details");

            UtcTime = DateTime.UtcNow;
            Level = level;
            Source = source;
            Type = type;
            EventName = eventName;
            Message = message;
            Details = details;
            Exception = exception;
        }

        /// <summary>
        /// Gets the details of the message.
        /// </summary>
        /// <param name="details">True to get more details, such as the call stack.</param>
        /// <returns></returns>
        public string GetMessage(bool details)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Time: ");
            sb.Append(UtcTime.ToLocalTime().ToString());
            switch (Level)
            {
                case VerboseLevel.DebugLow:
                    sb.Append(" Level: Debug (Low)");
                    break;
                case VerboseLevel.DebugNormal:
                    sb.Append(" Level: Debug (Normal)");
                    break;
                case VerboseLevel.DebugHigh:
                    sb.Append(" Level: Debug (High)");
                    break;
                case VerboseLevel.Information:
                    sb.Append(" Level: Information ");
                    break;
                case VerboseLevel.Warning:
                    sb.Append(" Level: Warning ");
                    break;
                case VerboseLevel.Error:
                    sb.Append(" Level: Error ");
                    break;
                case VerboseLevel.Critical:
                    sb.Append(" Level: Critical ");
                    break;
                case VerboseLevel.Fatal:
                    sb.Append(" Level: Fatal ");
                    break;
                case VerboseLevel.BugReport:
                    sb.Append(" Level: Bug Report");
                    break;
                case VerboseLevel.PerformanceIssue:
                    sb.Append(" Level: Performance Issue");
                    break;
                default:
                    sb.Append(" Level: Unknown ");
                    break;
            }

            sb.AppendLine();
            sb.Append("Event Name: ");
            sb.AppendLine(EventName);

            if (!string.IsNullOrWhiteSpace(Message))
            {
                sb.Append("Message: ");
                sb.AppendLine(Message);
            }
            if (!string.IsNullOrWhiteSpace(Details))
            {
                sb.Append("Details: ");
                sb.AppendLine(Details);
            }
            if (Exception != null)
            {
                sb.AppendLine("Exception: ");
                sb.AppendLine(Exception.ToString());
            }

            sb.AppendLine("Message Source: ");
            Source.AppendString(sb, details);

            return sb.ToString();
        }
    }
}