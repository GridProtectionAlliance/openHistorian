//******************************************************************************************************
//  TimeTag.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  -----------------------------------------------------------------------------------------------------
//  05/03/2006 - J. Ritchie Carroll
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  07/12/2006 - J. Ritchie Carroll
//       Modified class to be derived from new "TimeTagBase" class.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/02/2009 - Pinal C. Patel
//       Added Parse() static method to allow conversion of string to TimeTag.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/25/2009 - Pinal C. Patel
//       Added overloaded constructor that take ticks.
//       Added Now and UtcNow static properties for ease-of-use.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using TVA;

namespace openHistorian.V1
{
    /// <summary>
    /// Represents a historian time tag as number of seconds from the <see cref="BaseDate"/>.
    /// </summary>
    public class TimeTag : TimeTagBase, IComparable<TimeTag>
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTag"/> class.
        /// </summary>
        /// <param name="ticks">Number of ticks since the <see cref="BaseDate"/>.</param>
        public TimeTag(long ticks)
            : base(BaseDate.Ticks, Ticks.ToSeconds(ticks))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTag"/> class.
        /// </summary>
        /// <param name="seconds">Number of seconds since the <see cref="BaseDate"/>.</param>
        public TimeTag(double seconds)
            : base(BaseDate.Ticks, seconds)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTag"/> class.
        /// </summary>
        /// <param name="timestamp"><see cref="DateTime"/> value on or past the <see cref="BaseDate"/>.</param>
        public TimeTag(DateTime timestamp)
            : base(BaseDate.Ticks, timestamp)
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns the default text representation of <see cref="TimeTag"/>.
        /// </summary>
        /// <returns><see cref="string"/> that represents <see cref="TimeTag"/>.</returns>
        public override string ToString()
        {
            return ToString("dd-MMM-yyyy HH:mm:ss.fff");
        }

        /// <summary>
        /// Compares this time tag instance to another time tag instance and returns an integer that indicates whether the value of this instance
        /// is less than, equal to, or greater than the value of the other time tag.
        /// </summary>
        /// <param name="other">A <see cref="TimeTag"/> instance to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and the other value.</returns>
        /// <remarks>
        /// Time tags are compared by their value.
        /// </remarks>
        public int CompareTo(TimeTag other)
        {
            double tVal = Value;
            double oVal = other.Value;
            return tVal < oVal ? -1 : tVal > oVal ? 1 : 0;
        }

        #endregion

        #region [ Static ]

        // Static Fields

        /// <summary>
        /// Represents the smallest possible value of <see cref="TimeTag"/>.
        /// </summary>
        public static readonly TimeTag MinValue;

        /// <summary>
        /// Represents the largest possible value of <see cref="TimeTag"/>.
        /// </summary>
        public static readonly TimeTag MaxValue;

        /// <summary>
        /// Represents the base <see cref="DateTime"/> (01/01/1995) for <see cref="TimeTag"/>.
        /// </summary>
        public static readonly DateTime BaseDate;

        // Static Constructor

        static TimeTag()
        {
            BaseDate = new DateTime(1995, 1, 1, 0, 0, 0);
            MinValue = new TimeTag(0.0);
            MaxValue = new TimeTag(2147483647.999);
        }

        // Static Properties

        /// <summary>
        /// Gets a <see cref="TimeTag"/> object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        public static TimeTag Now
        {
            get
            {
                return new TimeTag(PrecisionTimer.Now.Ticks - BaseDate.Ticks);
            }
        }

        /// <summary>
        /// Gets a <see cref="TimeTag"/> object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public static TimeTag UtcNow
        {
            get
            {
                return new TimeTag(PrecisionTimer.UtcNow.Ticks - BaseDate.Ticks);
            }
        }

        // Static Methods

        /// <summary>
        /// Implicitly converts value, represented as a <see cref="TimeTag"/>, to a <see cref="Ticks"/>.
        /// </summary>
        /// <param name="value"><see cref="TimeTag"/> value to convert.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static implicit operator Ticks(TimeTag value)
        {
            return new Ticks(value.ToDateTime());
        }

        /// <summary>
        /// Implicitly converts value, represented as a <see cref="Ticks"/>, to a <see cref="TimeTag"/>.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to convert.</param>
        /// <returns><see cref="TimeTag"/> value representing the result.</returns>
        public static implicit operator TimeTag(Ticks value)
        {
            return new TimeTag((long)value);
        }

        /// <summary>
        /// Converts the specified string representation of a date and time to its <see cref="TimeTag"/> equivalent.
        /// </summary>
        /// <param name="timetag">A string containing the date and time to convert.</param>
        /// <returns>A <see cref="TimeTag"/> object.</returns>
        /// <remarks>
        /// <paramref name="timetag"/> can be specified in one of the following format:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Time Format</term>
        ///         <description>Format Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>12-30-2000 23:59:59</term>
        ///         <description>Absolute date and time.</description>
        ///     </item>
        ///     <item>
        ///         <term>*</term>
        ///         <description>Evaluates to <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>*-20s</term>
        ///         <description>Evaluates to 20 seconds before <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>*-10m</term>
        ///         <description>Evaluates to 10 minutes before <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>*-1h</term>
        ///         <description>Evaluates to 1 hour before <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>*-1d</term>
        ///         <description>Evaluates to 1 day before <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public static TimeTag Parse(string timetag)
        {
            DateTime dateTime;
            timetag = timetag.ToLower();
            if (timetag.Contains("*"))
            {
                // Relative time is specified.
                // Examples:
                // 1) * (Now)
                // 2) *-20s (20 seconds ago)
                // 3) *-10m (10 minutes ago)
                // 4) *-1h (1 hour ago)
                // 5) *-1d (1 day ago)
                dateTime = DateTime.UtcNow;
                if (timetag.Length > 1)
                {
                    string unit = timetag.Substring(timetag.Length - 1);
                    int adjustment = int.Parse(timetag.Substring(1, timetag.Length - 2));
                    switch (unit)
                    {
                        case "s":
                            dateTime = dateTime.AddSeconds(adjustment);
                            break;
                        case "m":
                            dateTime = dateTime.AddMinutes(adjustment);
                            break;
                        case "h":
                            dateTime = dateTime.AddHours(adjustment);
                            break;
                        case "d":
                            dateTime = dateTime.AddDays(adjustment);
                            break;
                    }
                }
            }
            else
            {
                // Absolute time is specified.
                dateTime = DateTime.Parse(timetag);
            }

            return new TimeTag(dateTime);
        }

        #endregion
    }
}
