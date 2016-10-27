//******************************************************************************************************
//  LogSuppressionEngine.cs - Gbtc
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
using GSF.Threading;

namespace GSF.Diagnostics
{
    /// <summary>
    /// The suppression engine associated with an individual <see cref="LogEventPublisherDetails"/>.
    /// </summary>
    internal class LogSuppressionEngine
    {
        private readonly object m_syncRoot;
        private RateLimiter m_normal;
        private RateLimiter m_low;
        private RateLimiter m_med;

        private double m_messagesPerSecond;
        private int m_burstLimit;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messagesPerSecond">The number of messages per second that can be generated. Must be greater than zero and less than 1 billion</param>
        /// <param name="burstLimit">The maximum number of messages that can be burst at one time.</param>
        public LogSuppressionEngine(double messagesPerSecond, int burstLimit)
        {
            m_syncRoot = new object();
            m_normal = new RateLimiter();
            m_low = new RateLimiter();
            m_med = new RateLimiter();

            if (Math.Abs(m_messagesPerSecond - messagesPerSecond) > 0.00001 || m_burstLimit != burstLimit)
            {
                m_messagesPerSecond = messagesPerSecond;
                m_burstLimit = burstLimit;
                m_normal.UpdateLimits(messagesPerSecond, burstLimit);
                m_low.UpdateLimits(messagesPerSecond * 2, burstLimit * 2);
                m_med.UpdateLimits(messagesPerSecond * 4, burstLimit * 4);
            }
        }

        /// <summary>
        /// Increments the publish count for this type of message.
        /// </summary>
        /// <returns>The suppression code assigned to this message.</returns>
        public MessageSuppression IncrementPublishCount()
        {
            lock (m_syncRoot)
            {
                if (m_normal.TryTakeToken())
                    return MessageSuppression.None;
                if (m_low.TryTakeToken())
                    return MessageSuppression.Standard;
                if (m_med.TryTakeToken())
                    return MessageSuppression.Heavy;
                return MessageSuppression.Severe;
            }
        }

    }

}