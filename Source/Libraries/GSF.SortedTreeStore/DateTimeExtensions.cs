//******************************************************************************************************
//  DateTimeExtensions.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  8/29/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF
{
    /// <summary>
    /// Helper methods for type <see cref="DateTime"/>
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Rounds the supplied datetime down to the nearest day.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime RoundDownToNearestDay(this DateTime value)
        {
            return new DateTime(value.Ticks - value.Ticks % TimeSpan.TicksPerDay, value.Kind);
        }

        /// <summary>
        /// Rounds the supplied datetime down to the nearest hour.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime RoundDownToNearestHour(this DateTime value)
        {
            return new DateTime(value.Ticks - value.Ticks % TimeSpan.TicksPerHour, value.Kind);
        }

        /// <summary>
        /// Rounds the supplied datetime down to the nearest minute.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime RoundDownToNearestMinute(this DateTime value)
        {
            return new DateTime(value.Ticks - value.Ticks % TimeSpan.TicksPerMinute, value.Kind);
        }

        /// <summary>
        /// Rounds the supplied datetime down to the nearest second.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime RoundDownToNearestSecond(this DateTime value)
        {
            return new DateTime(value.Ticks - value.Ticks % TimeSpan.TicksPerSecond, value.Kind);
        }

        /// <summary>
        /// Rounds the supplied datetime down to the nearest millisecond.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime RoundDownToNearestMillisecond(this DateTime value)
        {
            return new DateTime(value.Ticks - value.Ticks % TimeSpan.TicksPerMillisecond, value.Kind);
        }
    }
}
