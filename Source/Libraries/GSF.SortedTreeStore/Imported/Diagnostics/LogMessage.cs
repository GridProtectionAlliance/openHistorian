//******************************************************************************************************
//  LogMessage.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
using System.Text;
using GSF.IO;

namespace GSF.Diagnostics
{
    /// <summary>
    /// An individual log message.
    /// </summary>
    public sealed class LogMessage
    {
        /// <summary>
        /// Contains details about the <see cref="LogEventPublisher"/> that published this <see cref="LogMessage"/>.
        /// </summary>
        public readonly LogEventPublisherDetails EventPublisherDetails;

        /// <summary>
        /// The message stack that existed when the <see cref="LogPublisher"/> was originally constructed.
        /// </summary>
        public readonly LogStackMessages InitialStackMessages;

        /// <summary>
        /// The stack trace that existed when the <see cref="LogPublisher"/> was originally constructed.
        /// </summary>
        public readonly LogStackTrace InitialStackTrace;

        /// <summary>
        /// The message stack that existed when this <see cref="LogMessage"/> was published.
        /// </summary>
        public readonly LogStackMessages CurrentStackMessages;

        /// <summary>
        /// The stack trace that existed when this <see cref="LogMessage"/> was published.
        /// </summary>
        public readonly LogStackTrace CurrentStackTrace;

        /// <summary>
        /// The time that the message was created.
        /// </summary>
        public readonly DateTime UtcTime;

        /// <summary>
        /// The classification of the message
        /// </summary>
        public MessageClass Classification => LogMessageAttributes.Classification;

        /// <summary>
        /// The level associated with the message
        /// </summary>
        public MessageLevel Level => LogMessageAttributes.Level;

        /// <summary>
        /// The flags associated with the message
        /// </summary>
        public MessageFlags Flags => LogMessageAttributes.Flags;

        /// <summary>
        /// The suppression level assigned to this log message
        /// </summary>
        public MessageSuppression MessageSuppression => LogMessageAttributes.MessageSuppression;

        internal readonly LogMessageAttributes LogMessageAttributes;

        /// <summary>
        /// A specific message about the event giving more specifics about the actual message. 
        /// Typically, this will be up to 1 line of text. 
        /// Can be String.Empty.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// A long text field with the details of the message. 
        /// Can be String.Empty.
        /// </summary>
        public readonly string Details;

        /// <summary>
        /// An exception object if one is provided.
        /// Can be null. 
        /// Since the exception is not serialized to the disk, it will be null when loaded.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// A string representation of the exception. Can be String.Empty.
        /// If loaded from the disk, since exception objects cannot be serialized, 
        /// the <see cref="Exception"/> will be null and 
        /// this field will have the string representation of <see cref="Exception"/>
        /// </summary>
        public readonly string ExceptionString;

        /// <summary>
        /// Loads a log messages from the supplied stream
        /// </summary>
        /// <param name="stream">the stream to load the log message from.</param>
        /// <param name="saveHelper">A save helper that will condense objects</param>
        internal LogMessage(Stream stream, LogMessageSaveHelper saveHelper = null)
        {
            if (saveHelper == null)
                saveHelper = LogMessageSaveHelper.Create(true);

            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    EventPublisherDetails = saveHelper.LoadEventPublisherDetails(stream);
                    InitialStackMessages = saveHelper.LoadStackMessages(stream);
                    InitialStackTrace = saveHelper.LoadStackTrace(stream);
                    CurrentStackMessages = saveHelper.LoadStackMessages(stream);
                    CurrentStackTrace = saveHelper.LoadStackTrace(stream);
                    UtcTime = stream.ReadDateTime();
                    LogMessageAttributes = new LogMessageAttributes(stream);
                    Message = stream.ReadString();
                    Details = stream.ReadString();
                    Exception = null;
                    ExceptionString = stream.ReadString();
                    break;
                default:
                    throw new VersionNotFoundException();
            }
        }

        /// <summary>
        /// Creates a log message
        /// </summary>
        internal LogMessage(LogEventPublisherDetails eventPublisherDetails, LogStackMessages initialStackMessages, LogStackTrace initialStackTrace, LogStackMessages currentStackMessages, LogStackTrace currentStackTrace, LogMessageAttributes flags, string message, string details, Exception exception)
        {
            if (eventPublisherDetails == null)
                throw new ArgumentNullException(nameof(eventPublisherDetails));

            if (exception != null)
            {
                ExceptionString = exception.ToString();
            }
            else
            {
                ExceptionString = string.Empty;
            }

            EventPublisherDetails = eventPublisherDetails;
            InitialStackMessages = initialStackMessages ?? LogStackMessages.Empty;
            InitialStackTrace = initialStackTrace ?? LogStackTrace.Empty;
            CurrentStackMessages = currentStackMessages ?? LogStackMessages.Empty;
            CurrentStackTrace = currentStackTrace ?? LogStackTrace.Empty;
            UtcTime = DateTime.UtcNow;
            LogMessageAttributes = flags;
            Message = message ?? string.Empty;
            Details = details ?? string.Empty;
            Exception = exception;
        }

        public override string ToString()
        {
            return GetMessage();
        }

        /// <summary>
        /// Writes the log data to the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="saveHelper"></param>
        internal void Save(Stream stream, LogMessageSaveHelper saveHelper = null)
        {
            if (saveHelper == null)
                saveHelper = LogMessageSaveHelper.Create(true);

            stream.Write((byte)1);
            saveHelper.SaveEventPublisherDetails(stream, EventPublisherDetails);
            saveHelper.SaveStackMessages(stream, InitialStackMessages);
            saveHelper.SaveStackTrace(stream, InitialStackTrace);
            saveHelper.SaveStackMessages(stream, CurrentStackMessages);
            saveHelper.SaveStackTrace(stream, CurrentStackTrace);
            stream.Write(UtcTime);
            LogMessageAttributes.Save(stream);
            stream.Write(Message);
            stream.Write(Details);
            stream.Write(ExceptionString);
        }

        /// <summary>
        /// Gets the details of the message.
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Time: ");
            sb.Append(UtcTime.ToLocalTime());
            sb.Append(" - ");
            sb.Append(Classification.ToString());
            sb.Append(" - ");
            sb.Append(Level.ToString());
            sb.Append(" - ");
            sb.Append(Flags.ToString());
            sb.Append(" - ");
            sb.Append(MessageSuppression.ToString());
            sb.AppendLine();
            sb.Append("Event Name: ");
            sb.AppendLine(EventPublisherDetails.EventName);
            if (Message.Length > 0)
            {
                sb.Append("Message: ");
                sb.AppendLine(Message);
            }
            if (Details.Length > 0)
            {
                sb.Append("Details: ");
                sb.AppendLine(Details);
            }
            if (ExceptionString.Length > 0)
            {
                sb.AppendLine("Exception: ");
                sb.AppendLine(ExceptionString);
            }
            if (EventPublisherDetails.TypeName.Length > 0)
            {
                sb.AppendLine("Message Type: " + EventPublisherDetails.TypeName);
            }
            if (EventPublisherDetails.AssemblyName.Length > 0)
            {
                sb.AppendLine("Message Assembly: " + EventPublisherDetails.AssemblyName);
            }

            if (!ReferenceEquals(InitialStackMessages, LogStackMessages.Empty))
            {
                sb.AppendLine();
                sb.AppendLine("Initial Stack Messages: ");
                sb.AppendLine(InitialStackMessages.ToString());
            }
            if (!ReferenceEquals(InitialStackTrace, LogStackTrace.Empty))
            {
                sb.AppendLine();
                sb.AppendLine("Initial Stack Trace: ");
                sb.AppendLine(InitialStackTrace.ToString());
            }
            if (!ReferenceEquals(CurrentStackMessages, LogStackMessages.Empty))
            {
                sb.AppendLine();
                sb.AppendLine("Current Stack Messages: ");
                sb.AppendLine(CurrentStackMessages.ToString());
            }
            if (!ReferenceEquals(CurrentStackTrace, LogStackTrace.Empty))
            {
                sb.AppendLine();
                sb.AppendLine("Current Stack Trace: ");
                sb.AppendLine(CurrentStackTrace.ToString());
            }

            return sb.ToString();
        }
    }
}