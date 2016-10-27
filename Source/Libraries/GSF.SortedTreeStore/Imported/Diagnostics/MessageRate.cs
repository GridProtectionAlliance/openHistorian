//******************************************************************************************************
//  MessageRate.cs - Gbtc
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

namespace GSF.Diagnostics
{
    /// <summary>
    /// Defines a message rate for the message suppression algorithm
    /// </summary>
    public struct MessageRate
    {
        /// <summary>
        /// The rate in messages per second. (add 1 to this time, so the default is 1 message per second)
        /// </summary>
        private readonly double m_rate;

        private MessageRate(double rate)
        {
            m_rate = rate - 1;
        }

        /// <summary>
        /// Gets the default message rate. Which is 1 message per second.
        /// </summary>
        public static readonly MessageRate Default = new MessageRate(1);

        /// <summary>
        /// As messages per second.
        /// </summary>
        /// <param name="messagesPerSecond">the number of messages to allow per second</param>
        /// <returns></returns>
        public static MessageRate PerSecond(double messagesPerSecond)
        {
            return new MessageRate(messagesPerSecond);
        }
        /// <summary>
        /// As messages per minute.
        /// </summary>
        /// <param name="messagesPerMinute">the number of messages to allow per minute</param>
        /// <returns></returns>
        public static MessageRate PerMinute(double messagesPerMinute)
        {
            return new MessageRate(messagesPerMinute * (1.0 / 60.0));
        }
        /// <summary>
        /// As messages per hour.
        /// </summary>
        /// <param name="messagesPerHour">the number of messages to allow per hour</param>
        /// <returns></returns>
        public static MessageRate PerHour(double messagesPerHour)
        {
            return new MessageRate(messagesPerHour * (1.0 / (60.0 * 60.0)));
        }
        /// <summary>
        /// As messages per day.
        /// </summary>
        /// <param name="messagesPerDay">the number of messages to allow per day</param>
        /// <returns></returns>
        public static MessageRate PerDay(double messagesPerDay)
        {
            return new MessageRate(messagesPerDay * (1.0 / (60.0 * 60.0 * 24.0)));
        }

        /// <summary>
        /// As a minimum timespan between each message.
        /// </summary>
        /// <param name="seperation">the number of seconds between messages</param>
        /// <returns></returns>
        public static MessageRate EveryFewSeconds(double seperation)
        {
            return PerSecond(1.0 / seperation);
        }

        /// <summary>
        /// As a minimum timespan between each message.
        /// </summary>
        /// <param name="seperation">the number of Minutes between messages</param>
        /// <returns></returns>
        public static MessageRate EveryFewMinutes(double seperation)
        {
            return PerMinute(1.0 / seperation);
        }

        /// <summary>
        /// As a minimum timespan between each message.
        /// </summary>
        /// <param name="seperation">the number of Hours between messages</param>
        /// <returns></returns>
        public static MessageRate EveryFewHours(double seperation)
        {
            return PerHour(1.0 / seperation);
        }

        /// <summary>
        /// As a minimum timespan between each message.
        /// </summary>
        /// <param name="seperation">the number of Days between messages</param>
        /// <returns></returns>
        public static MessageRate EveryFewDays(double seperation)
        {
            return PerDay(1.0 / seperation);
        }

        /// <summary>
        /// Implicitly convert the message rate to a rate per second.
        /// </summary>
        /// <param name="rate">the item to convert.</param>
        /// <returns></returns>
        public static implicit operator double(MessageRate rate)
        {
            return rate.m_rate + 1;
        }


    }
}
