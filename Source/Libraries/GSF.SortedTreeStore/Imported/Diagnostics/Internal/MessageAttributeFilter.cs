//******************************************************************************************************
//  MessageAttributeFilter.cs - Gbtc
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

namespace GSF.Diagnostics
{
    /// <summary>
    /// A subscription that filters which messages to sort by based on their Classification and Flags.
    /// </summary>
    internal class MessageAttributeFilter
        : IEquatable<MessageAttributeFilter>
    {
        private MessageLevelFlags m_componentLevel;
        private MessageSuppressionFlags m_componentMessageSuppression;
        private MessageFlags m_componentFlags;

        private MessageLevelFlags m_frameworkLevel;
        private MessageSuppressionFlags m_frameworkMessageSuppression;
        private MessageFlags m_frameworkFlags;

        private MessageLevelFlags m_applicationLevel;
        private MessageSuppressionFlags m_applicationMessageSuppression;
        private MessageFlags m_applicationFlags;

        private MessageAttributeFilter(VerboseLevel verbose)
        {
            m_componentLevel = MessageLevelFlags.Error | MessageLevelFlags.Critical;
            m_componentMessageSuppression = MessageSuppressionFlags.None;
            m_componentFlags = MessageFlags.BugReport | MessageFlags.SystemHealth;

            m_frameworkLevel = MessageLevelFlags.Error | MessageLevelFlags.Critical;
            m_frameworkMessageSuppression = MessageSuppressionFlags.None;
            m_frameworkFlags = MessageFlags.BugReport | MessageFlags.SystemHealth;

            m_applicationLevel = MessageLevelFlags.Error | MessageLevelFlags.Critical;
            m_applicationMessageSuppression = MessageSuppressionFlags.None;
            m_applicationFlags = MessageFlags.BugReport | MessageFlags.SystemHealth;

            if (verbose >= VerboseLevel.Low)
            {
                m_componentLevel |= MessageLevelFlags.Error;
                m_componentFlags |= MessageFlags.None;

                m_frameworkLevel |= MessageLevelFlags.Error;
                m_frameworkFlags |= MessageFlags.None;

                m_applicationLevel |= MessageLevelFlags.Warning;
                m_applicationFlags |= MessageFlags.None;
            }
            if (verbose >= VerboseLevel.Medium)
            {
                m_componentLevel |= MessageLevelFlags.Warning;
                m_componentFlags |= MessageFlags.SecurityMessage;

                m_frameworkLevel |= MessageLevelFlags.Warning;
                m_frameworkFlags |= MessageFlags.SecurityMessage;

                m_applicationLevel |= MessageLevelFlags.Info;
                m_applicationFlags |= MessageFlags.SecurityMessage;
            }
            if (verbose >= VerboseLevel.High)
            {
                m_componentLevel |= MessageLevelFlags.Info;
                m_componentFlags |= MessageFlags.UsageIssue;

                m_frameworkLevel |= MessageLevelFlags.Info;
                m_frameworkFlags |= MessageFlags.UsageIssue;

                m_applicationLevel |= MessageLevelFlags.Debug;
                m_applicationFlags |= MessageFlags.UsageIssue;
            }
            if (verbose >= VerboseLevel.Ultra)
            {
                m_componentLevel |= MessageLevelFlags.Debug;
                m_componentFlags |= MessageFlags.PerformanceIssue;

                m_frameworkLevel |= MessageLevelFlags.Debug;
                m_frameworkFlags |= MessageFlags.PerformanceIssue;

                m_applicationLevel |= MessageLevelFlags.Debug;
                m_applicationFlags |= MessageFlags.PerformanceIssue;
            }
            if (verbose >= VerboseLevel.All)
            {
                m_componentLevel |= MessageLevelFlags.Debug;
                m_componentFlags |= MessageFlags.PerformanceIssue;

                m_frameworkLevel |= MessageLevelFlags.Debug;
                m_frameworkFlags |= MessageFlags.PerformanceIssue;

                m_applicationLevel |= MessageLevelFlags.Debug;
                m_applicationFlags |= MessageFlags.PerformanceIssue;
            }

            if (verbose >= VerboseLevel.All)
            {
                m_componentMessageSuppression = MessageSuppressionFlags.Standard | MessageSuppressionFlags.Heavy | MessageSuppressionFlags.Severe;
                m_frameworkMessageSuppression = MessageSuppressionFlags.Standard | MessageSuppressionFlags.Heavy | MessageSuppressionFlags.Severe;
                m_applicationMessageSuppression = MessageSuppressionFlags.Standard | MessageSuppressionFlags.Heavy | MessageSuppressionFlags.Severe;
            }
        }

        public MessageAttributeFilter()
        {

        }

        /// <summary>
        /// Gets if this subscription includes the provided message.
        /// </summary>
        /// <returns></returns>
        public bool IsSubscribedTo(LogMessageAttributes flags)
        {
            switch (flags.Classification)
            {
                case MessageClass.Component:
                    if (flags.MessageSuppression != MessageSuppression.None)
                    {
                        if ((1 << ((int)flags.MessageSuppression - 1) & (int)m_componentMessageSuppression) == 0)
                            return false;
                    }
                    if (flags.Level == MessageLevel.NA)
                        return (flags.Flags & m_componentFlags) != 0;
                    return ((1 << ((int)flags.Level - 1)) & (int)m_componentLevel) != 0 || (flags.Flags & m_componentFlags) != 0;
                case MessageClass.Framework:
                    if (flags.MessageSuppression != MessageSuppression.None)
                    {
                        if ((1 << ((int)flags.MessageSuppression - 1) & (int)m_frameworkMessageSuppression) == 0)
                            return false;
                    }
                    if (flags.Level == MessageLevel.NA)
                        return (flags.Flags & m_frameworkFlags) != 0;
                    return ((1 << ((int)flags.Level - 1)) & (int)m_frameworkLevel) != 0 || (flags.Flags & m_frameworkFlags) != 0;
                case MessageClass.Application:
                    if (flags.MessageSuppression != MessageSuppression.None)
                    {
                        if ((1 << ((int)flags.MessageSuppression - 1) & (int)m_applicationMessageSuppression) == 0)
                            return false;
                    }
                    if (flags.Level == MessageLevel.NA)
                        return (flags.Flags & m_applicationFlags) != 0;
                    return ((1 << ((int)flags.Level - 1)) & (int)m_applicationLevel) != 0 || (flags.Flags & m_applicationFlags) != 0;
                default:
                    return false;
            }
        }

        public bool Equals(MessageAttributeFilter other)
        {
            return other != null && m_componentMessageSuppression == other.m_componentMessageSuppression && m_componentLevel == other.m_componentLevel && m_componentFlags == other.m_componentFlags
                && m_frameworkMessageSuppression == other.m_frameworkMessageSuppression && m_frameworkLevel == other.m_frameworkLevel && m_frameworkFlags == other.m_frameworkFlags
                && m_applicationMessageSuppression == other.m_applicationMessageSuppression && m_applicationLevel == other.m_applicationLevel && m_applicationFlags == other.m_applicationFlags;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MessageAttributeFilter);
        }

        public override int GetHashCode()
        {
            return (int)m_componentMessageSuppression ^ (int)m_componentLevel ^ (int)m_componentFlags
                ^ (int)m_frameworkMessageSuppression ^ (int)m_frameworkLevel ^ (int)m_frameworkFlags
                ^ (int)m_applicationMessageSuppression ^ (int)m_applicationLevel ^ (int)m_applicationFlags;
        }

        /// <summary>
        /// Does a Union of the two subscriptions.
        /// </summary>
        /// 
        /// <returns></returns>
        public void Append(MessageAttributeFilter a)
        {
            m_componentMessageSuppression = m_componentMessageSuppression | a.m_componentMessageSuppression;
            m_componentLevel = m_componentLevel | a.m_componentLevel;
            m_componentFlags = m_componentFlags | a.m_componentFlags;

            m_frameworkMessageSuppression = m_frameworkMessageSuppression | a.m_frameworkMessageSuppression;
            m_frameworkLevel = m_frameworkLevel | a.m_frameworkLevel;
            m_frameworkFlags = m_frameworkFlags | a.m_frameworkFlags;

            m_applicationMessageSuppression = m_applicationMessageSuppression | a.m_applicationMessageSuppression;
            m_applicationLevel = m_applicationLevel | a.m_applicationLevel;
            m_applicationFlags = m_applicationFlags | a.m_applicationFlags;
        }

        /// <summary>
        /// Removes subscription A from this subscription if possible.
        /// </summary>
        /// <returns></returns>
        public void Remove(MessageAttributeFilter a)
        {
            m_componentMessageSuppression = m_componentMessageSuppression & ~a.m_componentMessageSuppression;
            m_componentLevel = m_componentLevel & ~a.m_componentLevel;
            m_componentFlags = m_componentFlags & ~a.m_componentFlags;

            m_frameworkMessageSuppression = m_frameworkMessageSuppression & ~a.m_frameworkMessageSuppression;
            m_frameworkLevel = m_frameworkLevel & ~a.m_frameworkLevel;
            m_frameworkFlags = m_frameworkFlags & ~a.m_frameworkFlags;

            m_applicationMessageSuppression = m_applicationMessageSuppression & ~a.m_applicationMessageSuppression;
            m_applicationLevel = m_applicationLevel & ~a.m_applicationLevel;
            m_applicationFlags = m_applicationFlags & ~a.m_applicationFlags;
        }

        public static MessageAttributeFilter Create(VerboseLevel level)
        {
            if (level == VerboseLevel.None)
                return null;
            return new MessageAttributeFilter(level);
        }
    }
}