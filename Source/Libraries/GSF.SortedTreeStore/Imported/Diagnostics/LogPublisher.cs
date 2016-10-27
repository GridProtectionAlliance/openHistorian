//******************************************************************************************************
//  LogPublisher.cs - Gbtc
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

// ReSharper disable InconsistentlySynchronizedField

namespace GSF.Diagnostics
{
    /// <summary>
    /// A publisher of log messages. 
    /// </summary>
    /// <remarks>
    /// <see cref="InitialStackMessages"/> and <see cref="InitialStackTrace"/> can be modified so messages that are generated 
    /// with this instance will have this data appended to the log message.
    /// 
    /// The user can either call one of the <see cref="Publish"/> overloads to lazily publish a message, or they
    /// can register a message with <see cref="RegisterEvent"/> so calling this message will incur little overhead.
    /// If registering an event, the user can check <see cref="LogEventPublisher.HasSubscribers"/> to determine if the log message can be skipped altogether. 
    /// Registering events also allows the user to specify the auto-suppression algorithm and the depth of the stack trace that will be recorded on a message being raised.
    /// 
    /// </remarks>
    public class LogPublisher
    {
        private readonly LogPublisherInternal m_publisherInstance;
        private readonly MessageClass m_classification;

        /// <summary>
        /// The stack messages that existed when this publisher was created. This can be modified by the user of this publisher.
        /// Any messages that get published by this class will automatically have this data added to the log message.
        /// </summary>
        public LogStackMessages InitialStackMessages;

        /// <summary>
        /// The stack trace that existed when this publisher was created. This can be modified by the user of this publisher.
        /// Any messages that get published by this class will automatically have this data added to the log message.
        /// </summary>
        public LogStackTrace InitialStackTrace;

        internal LogPublisher(LogPublisherInternal publisherInstance, MessageClass classification)
        {
            m_publisherInstance = publisherInstance;
            m_classification = classification;
            InitialStackMessages = Logger.GetStackMessages();
            InitialStackTrace = new LogStackTrace(true, 1, 10);
        }

        /// <summary>
        /// Initializes an <see cref="LogEventPublisher"/> with the provided values.
        /// </summary>
        /// <param name="level">the level of the message and associated flags if any</param>
        /// <param name="eventName">the name of the event.</param>
        /// <returns></returns>
        public LogEventPublisher RegisterEvent(MessageLevel level, string eventName)
        {
            LogMessageAttributes flag = new LogMessageAttributes(m_classification, level, MessageSuppression.None, MessageFlags.None);
            LogEventPublisherInternal publisher = m_publisherInstance.RegisterEvent(flag, eventName);
            return new LogEventPublisher(this, publisher);
        }

        /// <summary>
        /// Initializes an <see cref="LogEventPublisher"/> with the provided values.
        /// </summary>
        /// <param name="level">the level of the message</param>
        /// <param name="flags">associated flags</param>
        /// <param name="eventName">the name of the event.</param>
        /// <returns></returns>
        public LogEventPublisher RegisterEvent(MessageLevel level, MessageFlags flags, string eventName)
        {
            LogMessageAttributes flag = new LogMessageAttributes(m_classification, level, MessageSuppression.None, flags);
            LogEventPublisherInternal publisher = m_publisherInstance.RegisterEvent(flag, eventName);
            return new LogEventPublisher(this, publisher);
        }

        /// <summary>
        /// Initializes an <see cref="LogEventPublisher"/> with the provided values.
        /// </summary>
        /// <param name="level">the level of the message</param>
        /// <param name="eventName"></param>
        /// <param name="stackTraceDepth"></param>
        /// <param name="messagesPerSecond"></param>
        /// <param name="burstLimit"></param>
        /// <returns></returns>
        public LogEventPublisher RegisterEvent(MessageLevel level, string eventName, int stackTraceDepth, MessageRate messagesPerSecond, int burstLimit)
        {
            LogMessageAttributes flag = new LogMessageAttributes(m_classification, level, MessageSuppression.None, MessageFlags.None);
            LogEventPublisherInternal publisher = m_publisherInstance.RegisterEvent(flag, eventName, stackTraceDepth, messagesPerSecond, burstLimit);
            return new LogEventPublisher(this, publisher);
        }

        /// <summary>
        /// Initializes an <see cref="LogEventPublisher"/> with the provided values.
        /// </summary>
        /// <param name="level">the level of the message</param>
        /// <param name="flags">associated flags</param>
        /// <param name="eventName"></param>
        /// <param name="stackTraceDepth"></param>
        /// <param name="messagesPerSecond"></param>
        /// <param name="burstLimit"></param>
        /// <returns></returns>
        public LogEventPublisher RegisterEvent(MessageLevel level, MessageFlags flags, string eventName, int stackTraceDepth, MessageRate messagesPerSecond, int burstLimit)
        {
            LogMessageAttributes flag = new LogMessageAttributes(m_classification, level, MessageSuppression.None, flags);
            LogEventPublisherInternal publisher = m_publisherInstance.RegisterEvent(flag, eventName, stackTraceDepth, messagesPerSecond, burstLimit);
            return new LogEventPublisher(this, publisher);
        }

        /// <summary>
        /// Gets the full name of the type.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_publisherInstance.TypeFullName + " (" + m_publisherInstance.AssemblyFullName + ")";
        }

        /// <summary>
        /// Raises a log message with the provided data.
        /// </summary>
        /// <param name="level">the level of the message</param>
        /// <param name="eventName">A short name about what this message is detailing. Typically this will be a few words.</param>
        /// <param name="message"> A longer message than <see cref="eventName"/> giving more specifics about the actual message. 
        /// Typically, this will be up to 1 line of text.</param>
        /// <param name="details">A long text field with the details of the message.</param>
        /// <param name="exception">An exception object if one is provided.</param>
        public void Publish(MessageLevel level, string eventName, string message = null, string details = null, Exception exception = null)
        {
            LogMessageAttributes flag = new LogMessageAttributes(m_classification, level, MessageSuppression.None, MessageFlags.None);
            m_publisherInstance.RegisterEvent(flag, eventName).Publish(message, details, exception, InitialStackMessages, InitialStackTrace);
        }

        /// <summary>
        /// Raises a log message with the provided data.
        /// </summary>
        /// <param name="level">the level of the message</param>
        /// <param name="flags">associated flags</param>
        /// <param name="eventName">A short name about what this message is detailing. Typically this will be a few words.</param>
        /// <param name="message"> A longer message than <see cref="eventName"/> giving more specifics about the actual message. 
        /// Typically, this will be up to 1 line of text.</param>
        /// <param name="details">A long text field with the details of the message.</param>
        /// <param name="exception">An exception object if one is provided.</param>
        public void Publish(MessageLevel level, MessageFlags flags, string eventName, string message = null, string details = null, Exception exception = null)
        {
            LogMessageAttributes flag = new LogMessageAttributes(m_classification, level, MessageSuppression.None, flags);
            m_publisherInstance.RegisterEvent(flag, eventName).Publish(message, details, exception, InitialStackMessages, InitialStackTrace);
        }

    }
}