//******************************************************************************************************
//  Ticks.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/12/2004 - J. Ritchie Carroll
//       Initial version of source generated.
//  08/04/2009 - Josh L. Patterson
//       Edited Code Comments.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//  02/16/2014 - Steven E. Chisholm
//       In-lined a large portion of the smaller methods. Also fixed errors 
//       in Equals(object) and CompareTo(object)
//  02/18/2014 - J. Ritchie Carroll
//       Added sub-second distribution functions for common use.
//
//******************************************************************************************************

#region [ Contributor License Agreements ]

/**************************************************************************\
   Copyright © 2009 - J. Ritchie Carroll
   All rights reserved.
  
   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:
  
      * Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.
       
      * Redistributions in binary form must reproduce the above
        copyright notice, this list of conditions and the following
        disclaimer in the documentation and/or other materials provided
        with the distribution.
  
   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER "AS IS" AND ANY
   EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
   IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
   PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
   OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 \**************************************************************************/

#endregion

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using GSF.Units;

namespace GSF
{
    #region [ Enumerations ]

    /// <summary>
    /// Time intervals enumeration used by <see cref="Ticks.BaselinedTimestamp"/> method.
    /// </summary>
    public enum BaselineTimeInterval
    {
        /// <summary>
        /// Baseline timestamp to the second (i.e., starting at zero milliseconds).
        /// </summary>
        Second,
        /// <summary>
        /// Baseline timestamp to the minute (i.e., starting at zero seconds and milliseconds).
        /// </summary>
        Minute,
        /// <summary>
        /// Baseline timestamp to the hour (i.e., starting at zero minutes, seconds and milliseconds).
        /// </summary>
        Hour,
        /// <summary>
        /// Baseline timestamp to the day (i.e., starting at zero hours, minutes, seconds and milliseconds).
        /// </summary>
        Day,
        /// <summary>
        /// Baseline timestamp to the month (i.e., starting at day one, zero hours, minutes, seconds and milliseconds).
        /// </summary>
        Month,
        /// <summary>
        /// Baseline timestamp to the year (i.e., starting at month one, day one, zero hours, minutes, seconds and milliseconds).
        /// </summary>
        Year
    }

    #endregion

    /// <summary>
    /// Represents an instant in time, or time period, as a 64-bit signed integer with a value that is expressed as the number
    /// of 100-nanosecond intervals that have elapsed since 12:00:00 midnight, January 1, 0001.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Ticks"/> can represent an "instant in time" and therefore can be used exactly like a <see cref="DateTime"/>.
    /// The difference between <see cref="Ticks"/> and <see cref="DateTime"/> is that <see cref="Ticks"/> is essentially a long
    /// integer (i.e., an <see cref="Int64"/>) which represents the number of ticks that have elapsed since 12:00:00 midnight,
    /// January 1, 0001 with each tick having a resolution of 100-nanoseconds. You would use <see cref="Ticks"/> in places where
    /// you needed to directly represent time in high-resolution, i.e., time with sub-second accuracy, using an object that will
    /// act like a long integer but handle time conversions. <see cref="Ticks"/> can also represent a "time period" (e.g., the
    /// number of ticks elapsed since a process started) and thus can also be used like a <see cref="TimeSpan"/>; when used in
    /// this manner the <see cref="Ticks.ToElapsedTimeString()"/> method can be used to convert the <see cref="Ticks"/> value
    /// into a handy textual representation of elapsed years, days, hours, minutes and seconds or sub-seconds.
    /// </para>
    /// <para>
    /// This class behaves just like an <see cref="Int64"/> representing a time in ticks; it is implicitly castable to and from
    /// an <see cref="Int64"/> and therefore can be generally used "as" an Int64 directly. It is also implicitly castable to and
    /// from a <see cref="DateTime"/>, a <see cref="TimeSpan"/>, an <see cref="NtpTimeTag"/> and a <see cref="UnixTimeTag"/>.
    /// </para>
    /// </remarks>
    // ReSharper disable RedundantNameQualifier
    [Serializable]
    public struct Ticks : IComparable, IFormattable, IConvertible, IComparable<Ticks>, IComparable<Int64>, IComparable<DateTime>, IComparable<TimeSpan>, IEquatable<Ticks>, IEquatable<Int64>, IEquatable<DateTime>, IEquatable<TimeSpan>
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Number of 100-nanosecond ticks in one second.
        /// </summary>
        public const long PerSecond = 10000000L;

        /// <summary>
        /// Number of 100-nanosecond ticks in one millisecond.
        /// </summary>
        public const long PerMillisecond = (long)(Ticks.PerSecond * SI.Milli);

        /// <summary>
        /// Number of 100-nanosecond ticks in one microsecond.
        /// </summary>
        public const long PerMicrosecond = (long)(Ticks.PerSecond * SI.Micro);

        /// <summary>
        /// Number of 100-nanosecond ticks in one minute.
        /// </summary>
        public const long PerMinute = 60L * Ticks.PerSecond;

        /// <summary>
        /// Number of 100-nanosecond ticks in one hour.
        /// </summary>
        public const long PerHour = 60L * Ticks.PerMinute;

        /// <summary>
        /// Number of 100-nanosecond ticks in one day.
        /// </summary>
        public const long PerDay = 24L * Ticks.PerHour;

        // Fields

        /// <summary>
        /// Time value stored in ticks.
        /// </summary>
        public readonly long Value;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="Ticks"/>.
        /// </summary>
        /// <param name="value">New time value in ticks.</param>
        public Ticks(long value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new <see cref="Ticks"/>.
        /// </summary>
        /// <param name="value">New time value as a <see cref="DateTime"/>.</param>
        public Ticks(DateTime value)
        {
            Value = value.Ticks;
        }

        /// <summary>
        /// Creates a new <see cref="Ticks"/>.
        /// </summary>
        /// <param name="value">New time value as a <see cref="TimeSpan"/>.</param>
        public Ticks(TimeSpan value)
        {
            Value = value.Ticks;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the <see cref="Ticks"/> value in equivalent number of seconds.
        /// </summary>
        /// <returns>Value of <see cref="Ticks"/> in seconds.</returns>
        /// <remarks>
        /// If <see cref="Ticks"/> value represents an instant in time, returned value will represent the number of seconds
        /// that have elapsed since 12:00:00 midnight, January 1, 0001.
        /// </remarks>
        public double ToSeconds()
        {
            return Value / (double)Ticks.PerSecond;
        }

        /// <summary>
        /// Gets the <see cref="Ticks"/> value in equivalent number of milliseconds.
        /// </summary>
        /// <returns>Value of <see cref="Ticks"/> in milliseconds.</returns>
        /// <remarks>
        /// If <see cref="Ticks"/> value represents an instant in time, returned value will represent the number of milliseconds
        /// that have elapsed since 12:00:00 midnight, January 1, 0001.
        /// </remarks>
        public double ToMilliseconds()
        {
            return Value / (double)Ticks.PerMillisecond;
        }

        /// <summary>
        /// Gets the <see cref="Ticks"/> value in equivalent number of microseconds.
        /// </summary>
        /// <returns>Value of <see cref="Ticks"/> in microseconds.</returns>
        /// <remarks>
        /// If <see cref="Ticks"/> value represents an instant in time, returned value will represent the number of microseconds
        /// that have elapsed since 12:00:00 midnight, January 1, 0001.
        /// </remarks>
        public double ToMicroseconds()
        {
            return Value / (double)Ticks.PerMicrosecond;
        }

        /// <summary>
        /// Determines if time, represented by <see cref="Ticks"/> value in UTC time, is valid by comparing it to
        /// the system clock.
        /// </summary>
        /// <param name="lagTime">The allowed lag time, in seconds, before assuming time is too old to be valid.</param>
        /// <param name="leadTime">The allowed lead time, in seconds, before assuming time is too advanced to be valid.</param>
        /// <returns>True, if UTC time represented by <see cref="Ticks"/> value, is within the specified range.</returns>
        /// <remarks>
        /// Time, represented by <see cref="Ticks"/> value, is considered valid if it exists within the specified
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> range of system clock time in UTC. Note
        /// that <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero, but can be set
        /// to sub-second intervals.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero, but can be less than one.
        /// </exception>
        public bool UtcTimeIsValid(double lagTime, double leadTime)
        {
            return TimeIsValid(DateTime.UtcNow.Ticks, lagTime, leadTime);
        }

        /// <summary>
        /// Determines if time, represented by <see cref="Ticks"/> value in UTC time, is valid by comparing it to
        /// the system clock.
        /// </summary>
        /// <param name="lagTime">The allowed lag time, in ticks, before assuming time is too old to be valid.</param>
        /// <param name="leadTime">The allowed lead time, in ticks, before assuming time is too advanced to be valid.</param>
        /// <returns>True, if UTC time represented by <see cref="Ticks"/> value, is within the specified range.</returns>
        /// <remarks>
        /// Time, represented by <see cref="Ticks"/> value, is considered valid if it exists within the specified
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> range of system clock time in UTC. Note
        /// that <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero.
        /// </exception>
        public bool UtcTimeIsValid(Ticks lagTime, Ticks leadTime)
        {
            return TimeIsValid(DateTime.UtcNow.Ticks, lagTime, leadTime);
        }

        /// <summary>
        /// Determines if time, represented by <see cref="Ticks"/> value in local time, is valid by comparing it to
        /// the system clock.
        /// </summary>
        /// <param name="lagTime">The allowed lag time, in seconds, before assuming time is too old to be valid.</param>
        /// <param name="leadTime">The allowed lead time, in seconds, before assuming time is too advanced to be valid.</param>
        /// <returns>True, if local time represented by <see cref="Ticks"/> value, is within the specified range.</returns>
        /// <remarks>
        /// Time, represented by <see cref="Ticks"/> value, is considered valid if it exists within the specified
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> range of local system clock time. Note
        /// that <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero, but can be set
        /// to sub-second intervals.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero, but can be less than one.
        /// </exception>
        public bool LocalTimeIsValid(double lagTime, double leadTime)
        {
            return TimeIsValid(DateTime.Now.Ticks, lagTime, leadTime);
        }

        /// <summary>
        /// Determines if time, represented by <see cref="Ticks"/> value in local time, is valid by comparing it to
        /// the system clock.
        /// </summary>
        /// <param name="lagTime">The allowed lag time, in ticks, before assuming time is too old to be valid.</param>
        /// <param name="leadTime">The allowed lead time, in ticks, before assuming time is too advanced to be valid.</param>
        /// <returns>True, if local time represented by <see cref="Ticks"/> value, is within the specified range.</returns>
        /// <remarks>
        /// Time, represented by <see cref="Ticks"/> value, is considered valid if it exists within the specified
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> range of local system clock time. Note
        /// that <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero.
        /// </exception>
        public bool LocalTimeIsValid(Ticks lagTime, Ticks leadTime)
        {
            return TimeIsValid(DateTime.Now.Ticks, lagTime, leadTime);
        }

        /// <summary>
        /// Determines if time, represented by <see cref="Ticks"/> value, is valid by comparing it to the specified
        /// current time.
        /// </summary>
        /// <param name="currentTime">Specified current time (e.g., could be DateTime.Now.Ticks).</param>
        /// <param name="lagTime">The allowed lag time, in seconds, before assuming time is too old to be valid.</param>
        /// <param name="leadTime">The allowed lead time, in seconds, before assuming time is too advanced to be valid.</param>
        /// <returns>True, if time represented by <see cref="Ticks"/> value, is within the specified range.</returns>
        /// <remarks>
        /// Time, represented by <see cref="Ticks"/> value, is considered valid if it exists within the specified
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> range of <paramref name="currentTime"/>. Note
        /// that <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero, but can be set
        /// to sub-second intervals.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero, but can be less than one.
        /// </exception>
        public bool TimeIsValid(Ticks currentTime, double lagTime, double leadTime)
        {
            if (lagTime <= 0)
                throw new ArgumentOutOfRangeException("lagTime", "lagTime must be greater than zero, but it can be less than one");

            if (leadTime <= 0)
                throw new ArgumentOutOfRangeException("leadTime", "leadTime must be greater than zero, but it can be less than one");

            double distance = (currentTime.Value - Value) / (double)Ticks.PerSecond;

            return (distance >= -leadTime && distance <= lagTime);
        }

        /// <summary>
        /// Determines if time, represented by <see cref="Ticks"/> value, is valid by comparing it to the specified
        /// current time.
        /// </summary>
        /// <param name="currentTime">Specified current time (e.g., could be DateTime.Now.Ticks).</param>
        /// <param name="lagTime">The allowed lag time, in ticks, before assuming time is too old to be valid.</param>
        /// <param name="leadTime">The allowed lead time, in ticks, before assuming time is too advanced to be valid.</param>
        /// <returns>True, if time represented by <see cref="Ticks"/> value, is within the specified range.</returns>
        /// <remarks>
        /// Time, represented by <see cref="Ticks"/> value, is considered valid if it exists within the specified
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> range of <paramref name="currentTime"/>. Note
        /// that <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lagTime"/> and <paramref name="leadTime"/> must be greater than zero.
        /// </exception>
        public bool TimeIsValid(Ticks currentTime, Ticks lagTime, Ticks leadTime)
        {
            if (lagTime.Value <= 0)
                throw new ArgumentOutOfRangeException("lagTime", "lagTime must be greater than zero");

            if (leadTime.Value <= 0)
                throw new ArgumentOutOfRangeException("leadTime", "leadTime must be greater than zero");

            long distance = (currentTime.Value - Value);

            return (distance >= -leadTime.Value && distance <= lagTime.Value);
        }

        /// <summary>
        /// Gets the distance, in 100-nanoseconds intervals, beyond the top of the second in the timestamp
        /// represented by the <see cref="Ticks"/>.
        /// </summary>
        /// <returns>
        /// Number of 100-nanoseconds intervals <see cref="Ticks"/> value is from the top of the second.
        /// </returns>
        public Ticks DistanceBeyondSecond()
        {
            // Calculate the number of ticks past the top of the second
            return Value - (Value - Value % Ticks.PerSecond);
        }

        /// <summary>
        /// Creates a new <see cref="Ticks"/> value that represents a base-lined timestamp, in 100-nanoseconds
        /// intervals, that begins at the beginning of the specified time interval.
        /// </summary>
        /// <param name="interval">
        /// <see cref="BaselineTimeInterval"/> to which <see cref="Ticks"/> timestamp should be base-lined.
        /// </param>
        /// <returns>
        /// A new <see cref="Ticks"/> value that represents a base-lined timestamp, in 100-nanoseconds intervals,
        /// that begins at the specified <see cref="BaselineTimeInterval"/>.
        /// </returns>
        /// <remarks>
        /// Base-lining to the <see cref="BaselineTimeInterval.Second"/> would return the <see cref="Ticks"/>
        /// value starting at zero milliseconds.<br/>
        /// Base-lining to the <see cref="BaselineTimeInterval.Minute"/> would return the <see cref="Ticks"/>
        /// value starting at zero seconds and milliseconds.<br/>
        /// Base-lining to the <see cref="BaselineTimeInterval.Hour"/> would return the <see cref="Ticks"/>
        /// value starting at zero minutes, seconds and milliseconds.<br/>
        /// Base-lining to the <see cref="BaselineTimeInterval.Day"/> would return the <see cref="Ticks"/>
        /// value starting at zero hours, minutes, seconds and milliseconds.<br/>
        /// Base-lining to the <see cref="BaselineTimeInterval.Month"/> would return the <see cref="Ticks"/>
        /// value starting at day one, zero hours, minutes, seconds and milliseconds.<br/>
        /// Base-lining to the <see cref="BaselineTimeInterval.Year"/> would return the <see cref="Ticks"/>
        /// value starting at month one, day one, zero hours, minutes, seconds and milliseconds.
        /// </remarks>
        public Ticks BaselinedTimestamp(BaselineTimeInterval interval)
        {
            switch (interval)
            {
                case BaselineTimeInterval.Second:
                    return Value - Value % Ticks.PerSecond;
                case BaselineTimeInterval.Minute:
                    return Value - Value % Ticks.PerMinute;
                case BaselineTimeInterval.Hour:
                    return Value - Value % Ticks.PerHour;
                case BaselineTimeInterval.Day:
                    return Value - Value % Ticks.PerDay;
                case BaselineTimeInterval.Month:
                    DateTime toMonth = new DateTime(Value);
                    return new DateTime(toMonth.Year, toMonth.Month, 1, 0, 0, 0, 0).Ticks;
                case BaselineTimeInterval.Year:
                    return new DateTime((new DateTime(Value)).Year, 1, 1, 0, 0, 0, 0).Ticks;
                default:
                    return this;
            }
        }

        /// <summary>
        /// Converts the value of the <see cref="Ticks"/> value to its equivalent <see cref="DateTime"/> string representation.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> string representation of the <see cref="Ticks"/> value.</returns>
        public override string ToString()
        {
            return ((DateTime)this).ToString();
        }

        /// <summary>
        /// Converts the <see cref="Ticks"/> value to its equivalent string representation, using
        /// the specified <see cref="DateTime"/> format.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <returns>
        /// The string representation of the value of this instance as specified by format.
        /// </returns>
        public string ToString(string format)
        {
            return ((DateTime)this).ToString(format);
        }

        /// <summary>
        /// Converts the <see cref="Ticks"/> value to its equivalent string representation, using
        /// the specified culture-specific <see cref="DateTime"/> format information.
        /// </summary>
        /// <param name="provider">
        /// A <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// The string representation of the value of this instance as specified by provider.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            return ((DateTime)this).ToString(provider);
        }

        /// <summary>
        /// Converts the <see cref="Ticks"/> value to its equivalent string representation, using
        /// specified format and culture-specific <see cref="DateTime"/> format information.
        /// </summary>
        /// <param name="format">A format specification.</param>
        /// <param name="provider">
        /// A <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        /// The string representation of the value of this instance as specified by format and provider.
        /// </returns>
        public string ToString(string format, IFormatProvider provider)
        {
            return ((DateTime)this).ToString(format, provider);
        }

        /// <summary>
        /// Converts the <see cref="Ticks"/> value into a textual representation of years, days, hours,
        /// minutes and seconds.
        /// </summary>
        /// <remarks>
        /// Note that this ToElapsedTimeString overload will not display fractional seconds. To allow
        /// display of fractional seconds, or completely remove second resolution from the textual
        /// representation, use the <see cref="ToElapsedTimeString(int,double)"/> overload instead.
        /// </remarks>
        /// <returns>
        /// The string representation of the value of this instance, consisting of the number of
        /// years, days, hours, minutes and seconds represented by this value.
        /// </returns>
        public string ToElapsedTimeString()
        {
            return Time.ToElapsedTimeString(ToSeconds(), 0);
        }

        /// <summary>
        /// Converts the <see cref="Ticks"/> value into a textual representation of years, days, hours,
        /// minutes and seconds with the specified number of fractional digits.
        /// </summary>
        /// <param name="secondPrecision">Number of fractional digits to display for seconds.</param>
        /// <param name="minimumSubSecondResolution">
        /// Minimum sub-second resolution to display. Defaults to <see cref="SI.Milli"/>.
        /// </param>
        /// <remarks>
        /// Set <paramref name="secondPrecision"/> to -1 to suppress seconds display, this will
        /// force minimum resolution of time display to minutes.
        /// </remarks>
        /// <returns>
        /// The string representation of the value of this instance, consisting of the number of
        /// years, days, hours, minutes and seconds represented by this value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minimumSubSecondResolution"/> is not less than or equal to <see cref="SI.Milli"/> or
        /// <paramref name="minimumSubSecondResolution"/> is not defined in <see cref="SI.Factors"/> array.
        /// </exception>
        public string ToElapsedTimeString(int secondPrecision, double minimumSubSecondResolution = SI.Milli)
        {
            return Time.ToElapsedTimeString(ToSeconds(), secondPrecision, null, minimumSubSecondResolution);
        }

        /// <summary>
        /// Converts the <see cref="Ticks"/> value into a textual representation of years, days, hours,
        /// minutes and seconds with the specified number of fractional digits given string array of
        /// time names.
        /// </summary>
        /// <param name="secondPrecision">Number of fractional digits to display for seconds.</param>
        /// <param name="timeNames">Time names array to use during textual conversion.</param>
        /// <param name="minimumSubSecondResolution">
        /// Minimum sub-second resolution to display. Defaults to <see cref="SI.Milli"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// Set <paramref name="secondPrecision"/> to -1 to suppress seconds display, this will
        /// force minimum resolution of time display to minutes.
        /// </para>
        /// <para>
        /// <paramref name="timeNames"/> array needs one string entry for each of the following names:<br/>
        /// " year", " years", " day", " days", " hour", " hours", " minute", " minutes", " second", " seconds", "less than ".
        /// </para>
        /// </remarks>
        /// <returns>
        /// The string representation of the value of this instance, consisting of the number of
        /// years, days, hours, minutes and seconds represented by this value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minimumSubSecondResolution"/> is not less than or equal to <see cref="SI.Milli"/> or
        /// <paramref name="minimumSubSecondResolution"/> is not defined in <see cref="SI.Factors"/> array.
        /// </exception>
        public string ToElapsedTimeString(int secondPrecision, string[] timeNames, double minimumSubSecondResolution = SI.Milli)
        {
            return Time.ToElapsedTimeString(ToSeconds(), secondPrecision, timeNames, minimumSubSecondResolution);
        }

        #region [ Numeric Interface Implementations ]

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An object to compare, or null.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value. Returns less than zero
        /// if this instance is less than value, zero if this instance is equal to value, or greater than zero
        /// if this instance is greater than value.
        /// </returns>
        /// <exception cref="ArgumentException">value is not an <see cref="Int64"/> or <see cref="Ticks"/>.</exception>
        public int CompareTo(object value)
        {
            if ((object)value == null)
                return 1;

            long num;

            if (value is long)
                num = (long)value;
            else if (value is Ticks)
                num = ((Ticks)value).Value;
            else if (value is DateTime)
                num = ((DateTime)value).Ticks;
            else if (value is TimeSpan)
                num = ((TimeSpan)value).Ticks;
            else
                throw new ArgumentException("Argument must be an Int64 or a Ticks");

            return (Value < num ? -1 : (Value > num ? 1 : 0));
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Ticks"/> and returns an indication of their
        /// relative values.
        /// </summary>
        /// <param name="value">A <see cref="Ticks"/> to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value. Returns less than zero
        /// if this instance is less than value, zero if this instance is equal to value, or greater than zero
        /// if this instance is greater than value.
        /// </returns>
        public int CompareTo(Ticks value)
        {
            return CompareTo(value.Value);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="DateTime"/> and returns an indication of their
        /// relative values.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value. Returns less than zero
        /// if this instance is less than value, zero if this instance is equal to value, or greater than zero
        /// if this instance is greater than value.
        /// </returns>
        public int CompareTo(DateTime value)
        {
            return CompareTo(value.Ticks);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="TimeSpan"/> and returns an indication of their
        /// relative values.
        /// </summary>
        /// <param name="value">A <see cref="TimeSpan"/> to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value. Returns less than zero
        /// if this instance is less than value, zero if this instance is equal to value, or greater than zero
        /// if this instance is greater than value.
        /// </returns>
        public int CompareTo(TimeSpan value)
        {
            return CompareTo(value.Ticks);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Int64"/> and returns an indication of their
        /// relative values.
        /// </summary>
        /// <param name="value">An <see cref="Int64"/> to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value. Returns less than zero
        /// if this instance is less than value, zero if this instance is equal to value, or greater than zero
        /// if this instance is greater than value.
        /// </returns>
        public int CompareTo(long value)
        {
            return (Value < value ? -1 : (Value > value ? 1 : 0));
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare, or null.</param>
        /// <returns>
        /// True if obj is an instance of <see cref="Int64"/> or <see cref="Ticks"/> and equals the value of this instance;
        /// otherwise, False.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is long)
                return Value == (long)obj;
            if (obj is Ticks)
                return Value == ((Ticks)obj).Value;
            if (obj is DateTime)
                return Value == ((DateTime)obj).Ticks;
            if (obj is TimeSpan)
                return Value == ((TimeSpan)obj).Ticks;

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Ticks"/> value.
        /// </summary>
        /// <param name="obj">A <see cref="Ticks"/> value to compare to this instance.</param>
        /// <returns>
        /// True if obj has the same value as this instance; otherwise, False.
        /// </returns>
        public bool Equals(Ticks obj)
        {
            return (Value == obj.Value);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="obj">A <see cref="DateTime"/> value to compare to this instance.</param>
        /// <returns>
        /// True if obj has the same value as this instance; otherwise, False.
        /// </returns>
        public bool Equals(DateTime obj)
        {
            return (Value == obj.Ticks);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="TimeSpan"/> value.
        /// </summary>
        /// <param name="obj">A <see cref="TimeSpan"/> value to compare to this instance.</param>
        /// <returns>
        /// True if obj has the same value as this instance; otherwise, False.
        /// </returns>
        public bool Equals(TimeSpan obj)
        {
            return (Value == obj.Ticks);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Int64"/> value.
        /// </summary>
        /// <param name="obj">An <see cref="Int64"/> value to compare to this instance.</param>
        /// <returns>
        /// True if obj has the same value as this instance; otherwise, False.
        /// </returns>
        public bool Equals(long obj)
        {
            return (Value == obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns the <see cref="TypeCode"/> for value type <see cref="Int64"/>.
        /// </summary>
        /// <returns>The enumerated constant, <see cref="TypeCode.Int64"/>.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Int64;
        }

        #region [ Explicit IConvertible Implementation ]

        // These are explicitly implemented on the native System.Int64 implementations, so we do the same...

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(Value, provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Value, provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Value, provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Value, provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Value, provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value, provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Value, provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value, provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Value;
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value, provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Value, provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Value, provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(Value, provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Value, provider);
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.ChangeType(Value, type, provider);
        }

        #endregion

        #endregion

        #endregion

        #region [ Operators ]

        #region [ Comparison Operators ]

        /// <summary>
        /// Compares the two values for equality.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="bool"/> value representing the result.</returns>
        public static bool operator ==(Ticks value1, Ticks value2)
        {
            return value1.Value == value2.Value;
        }

        /// <summary>
        /// Compares the two values for inequality.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="bool"/> value representing the result.</returns>
        public static bool operator !=(Ticks value1, Ticks value2)
        {
            return value1.Value != value2.Value;
        }

        /// <summary>
        /// Returns true if left value is less than right value.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="bool"/> value representing the result.</returns>
        public static bool operator <(Ticks value1, Ticks value2)
        {
            return value1.Value < value2.Value;
        }

        /// <summary>
        /// Returns true if left value is less or equal to than right value.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="bool"/> value representing the result.</returns>
        public static bool operator <=(Ticks value1, Ticks value2)
        {
            return value1.Value <= value2.Value;
        }

        /// <summary>
        /// Returns true if left value is greater than right value.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="bool"/> value representing the result.</returns>
        public static bool operator >(Ticks value1, Ticks value2)
        {
            return value1.Value > value2.Value;
        }

        /// <summary>
        /// Returns true if left value is greater than or equal to right value.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="bool"/> value representing the result.</returns>
        public static bool operator >=(Ticks value1, Ticks value2)
        {
            return value1.Value >= value2.Value;
        }

        #endregion

        #region [ Type Conversion Operators ]

        /// <summary>
        /// Implicitly converts value, represented in ticks, to a <see cref="Ticks"/>.
        /// </summary>
        /// <param name="value"><see cref="Int64"/> value to convert.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static implicit operator Ticks(Int64 value)
        {
            return new Ticks(value);
        }

        /// <summary>
        /// Implicitly converts value, represented as a <see cref="DateTime"/>, to a <see cref="Ticks"/>.
        /// </summary>
        /// <param name="value"><see cref="DateTime"/> value to convert.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static implicit operator Ticks(DateTime value)
        {
            return new Ticks(value);
        }

        /// <summary>
        /// Implicitly converts value, represented as a <see cref="TimeSpan"/>, to a <see cref="Ticks"/>.
        /// </summary>
        /// <param name="value"><see cref="TimeSpan"/> value to convert.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static implicit operator Ticks(TimeSpan value)
        {
            return new Ticks(value);
        }

        /// <summary>
        /// Implicitly converts value, represented as a <see cref="TimeTagBase"/>, to a <see cref="Ticks"/>.
        /// </summary>
        /// <param name="value"><see cref="TimeTagBase"/> value to convert.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static implicit operator Ticks(TimeTagBase value)
        {
            return new Ticks(value.ToDateTime());
        }

        /// <summary>
        /// Implicitly converts <see cref="Ticks"/>, represented in ticks, to an <see cref="Int64"/>.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to convert.</param>
        /// <returns><see cref="Int64"/> value representing the result.</returns>
        public static implicit operator Int64(Ticks value)
        {
            return value.Value;
        }

        /// <summary>
        /// Implicitly converts <see cref="Ticks"/>, represented in ticks, to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to convert.</param>
        /// <returns><see cref="DateTime"/> value representing the result.</returns>
        public static implicit operator DateTime(Ticks value)
        {
            return new DateTime(value.Value);
        }

        /// <summary>
        /// Implicitly converts <see cref="Ticks"/>, represented in ticks, to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to convert.</param>
        /// <returns><see cref="TimeSpan"/> value representing the result.</returns>
        public static implicit operator TimeSpan(Ticks value)
        {
            return new TimeSpan(value.Value);
        }

        /// <summary>
        /// Implicitly converts <see cref="Ticks"/>, represented in ticks, to an <see cref="NtpTimeTag"/>.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to convert.</param>
        /// <returns><see cref="NtpTimeTag"/> value representing the result.</returns>
        public static implicit operator NtpTimeTag(Ticks value)
        {
            return new NtpTimeTag(new DateTime(value.Value));
        }

        /// <summary>
        /// Implicitly converts <see cref="Ticks"/>, represented in ticks, to a <see cref="UnixTimeTag"/>.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to convert.</param>
        /// <returns><see cref="UnixTimeTag"/> value representing the result.</returns>
        public static implicit operator UnixTimeTag(Ticks value)
        {
            return new UnixTimeTag(new DateTime(value.Value));
        }

        #endregion

        #region [ Boolean and Bitwise Operators ]

        /// <summary>
        /// Returns true if value is not zero.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to evaluate.</param>
        /// <returns><see cref="bool"/> value representing the result.</returns>
        public static bool operator true(Ticks value)
        {
            return (value.Value != 0);
        }

        /// <summary>
        /// Returns true if value is equal to zero.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to evaluate.</param>
        /// <returns><see cref="bool"/> value representing the result.</returns>
        public static bool operator false(Ticks value)
        {
            return (value.Value == 0);
        }

        /// <summary>
        /// Returns bitwise complement of value.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to evaluate.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator ~(Ticks value)
        {
            return new Ticks(~value.Value);
        }

        /// <summary>
        /// Returns logical bitwise AND of values.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator &(Ticks value1, Ticks value2)
        {
            return new Ticks(value1.Value & value2.Value);
        }

        /// <summary>
        /// Returns logical bitwise OR of values.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator |(Ticks value1, Ticks value2)
        {
            return new Ticks(value1.Value | value2.Value);
        }

        /// <summary>
        /// Returns logical bitwise exclusive-OR of values.
        /// </summary>
        /// <param name="value1"><see cref="Ticks"/> left hand operand.</param>
        /// <param name="value2"><see cref="Ticks"/> right hand operand.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator ^(Ticks value1, Ticks value2)
        {
            return new Ticks(value1.Value ^ value2.Value);
        }

        /// <summary>
        /// Returns value after right shifts of first value by the number of bits specified by second value.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to shift.</param>
        /// <param name="shifts"><see cref="int"/> number of bits to shift.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator >>(Ticks value, int shifts)
        {
            return new Ticks((value.Value >> shifts));
        }

        /// <summary>
        /// Returns value after left shifts of first value by the number of bits specified by second value.
        /// </summary>
        /// <param name="value"><see cref="Ticks"/> value to shift.</param>
        /// <param name="shifts"><see cref="int"/> number of bits to shift.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator <<(Ticks value, int shifts)
        {
            return new Ticks((value.Value << shifts));
        }

        #endregion

        #region [ Arithmetic Operators ]

        /// <summary>
        /// Returns computed remainder after dividing first value by the second.
        /// </summary>
        /// <param name="value1">Left hand <see cref="Ticks"/> operand.</param>
        /// <param name="value2">Right hand <see cref="Ticks"/> operand.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator %(Ticks value1, Ticks value2)
        {
            return new Ticks(value1.Value % value2.Value);
        }

        /// <summary>
        /// Returns computed sum of values.
        /// </summary>
        /// <param name="value1">Left hand <see cref="Ticks"/> operand.</param>
        /// <param name="value2">Right hand <see cref="Ticks"/> operand.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator +(Ticks value1, Ticks value2)
        {
            return new Ticks(value1.Value + value2.Value);
        }

        /// <summary>
        /// Returns computed difference of values.
        /// </summary>
        /// <param name="value1">Left hand <see cref="Ticks"/> operand.</param>
        /// <param name="value2">Right hand <see cref="Ticks"/> operand.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator -(Ticks value1, Ticks value2)
        {
            return new Ticks(value1.Value - value2.Value);
        }

        /// <summary>
        /// Returns computed product of values.
        /// </summary>
        /// <param name="value1">Left hand <see cref="Ticks"/> operand.</param>
        /// <param name="value2">Right hand <see cref="Ticks"/> operand.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator *(Ticks value1, Ticks value2)
        {
            return new Ticks(value1.Value * value2.Value);
        }

        // Integer division operators

        /// <summary>
        /// Returns computed division of values.
        /// </summary>
        /// <param name="value1">Left hand <see cref="Ticks"/> operand.</param>
        /// <param name="value2">Right hand <see cref="Ticks"/> operand.</param>
        /// <returns><see cref="Ticks"/> value representing the result.</returns>
        public static Ticks operator /(Ticks value1, Ticks value2)
        {
            return new Ticks(value1.Value / value2.Value);
        }

        // C# doesn't expose an exponent operator but some other .NET languages do,
        // so we expose the operator via its native special IL function name

        /// <summary>
        /// Returns result of first value raised to power of second value.
        /// </summary>
        /// <param name="value1">Left hand <see cref="Ticks"/> operand.</param>
        /// <param name="value2">Right hand <see cref="Ticks"/> operand.</param>
        /// <returns><see cref="double"/> value representing the result.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced), SpecialName]
        public static double op_Exponent(Ticks value1, Ticks value2)
        {
            return Math.Pow((double)value1.Value, (double)value2.Value);
        }

        #endregion

        #endregion

        #region [ Static ]

        // Static Fields

        /// <summary>
        /// Represents the largest possible value of a <see cref="Ticks"/>. This field is constant.
        /// </summary>
        public static readonly Ticks MaxValue = (Ticks)long.MaxValue;

        /// <summary>
        /// Represents the smallest possible value of a <see cref="Ticks"/>. This field is constant.
        /// </summary>
        public static readonly Ticks MinValue = (Ticks)long.MinValue;

        // Static Methods

        /// <summary>
        /// Converts <paramref name="value"/>, in 100-nanosecond tick intervals, to seconds.
        /// </summary>
        /// <param name="value">Number of ticks to convert to seconds.</param>
        /// <returns>Number seconds represented by specified <paramref name="value"/> in ticks.</returns>
        /// <remarks>
        /// If <paramref name="value"/> represents an instant in time, returned value will represent the number of seconds
        /// that have elapsed since 12:00:00 midnight, January 1, 0001.
        /// </remarks>
        public static double ToSeconds(Ticks value)
        {
            return value / (double)Ticks.PerSecond;
        }

        /// <summary>
        /// Converts <paramref name="value"/>, in 100-nanosecond tick intervals, to milliseconds.
        /// </summary>
        /// <param name="value">Number of ticks to convert to milliseconds.</param>
        /// <returns>Number milliseconds represented by specified <paramref name="value"/> in ticks.</returns>
        /// <remarks>
        /// If <paramref name="value"/> represents an instant in time, returned value will represent the number of milliseconds
        /// that have elapsed since 12:00:00 midnight, January 1, 0001.
        /// </remarks>
        public static double ToMilliseconds(Ticks value)
        {
            return value / (double)Ticks.PerMillisecond;
        }

        /// <summary>
        /// Converts <paramref name="value"/>, in 100-nanosecond tick intervals, to microseconds.
        /// </summary>
        /// <param name="value">Number of ticks to convert to microseconds.</param>
        /// <returns>Number microseconds represented by specified <paramref name="value"/> in ticks.</returns>
        /// <remarks>
        /// If <paramref name="value"/> represents an instant in time, returned value will represent the number of microseconds
        /// that have elapsed since 12:00:00 midnight, January 1, 0001.
        /// </remarks>
        public static double ToMicroseconds(Ticks value)
        {
            return value / (double)Ticks.PerMicrosecond;
        }

        /// <summary>
        /// Creates a new <see cref="Ticks"/> from the specified <paramref name="value"/> in seconds.
        /// </summary>
        /// <param name="value">New <see cref="Ticks"/> value in seconds.</param>
        /// <returns>New <see cref="Ticks"/> object from the specified <paramref name="value"/> in seconds.</returns>
        public static Ticks FromSeconds(double value)
        {
            return new Ticks((long)(value * Ticks.PerSecond));
        }

        /// <summary>
        /// Creates a new <see cref="Ticks"/> from the specified <paramref name="value"/> in milliseconds.
        /// </summary>
        /// <param name="value">New <see cref="Ticks"/> value in milliseconds.</param>
        /// <returns>New <see cref="Ticks"/> object from the specified <paramref name="value"/> in milliseconds.</returns>
        public static Ticks FromMilliseconds(double value)
        {
            return new Ticks((long)(value * Ticks.PerMillisecond));
        }

        /// <summary>
        /// Creates a new <see cref="Ticks"/> from the specified <paramref name="value"/> in microseconds.
        /// </summary>
        /// <param name="value">New <see cref="Ticks"/> value in microseconds.</param>
        /// <returns>New <see cref="Ticks"/> object from the specified <paramref name="value"/> in microseconds.</returns>
        public static Ticks FromMicroseconds(double value)
        {
            return new Ticks((long)(value * Ticks.PerMicrosecond));
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Ticks"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <returns>
        /// A <see cref="Ticks"/> equivalent to the number contained in s.
        /// </returns>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="OverflowException">
        /// s represents a number less than <see cref="Ticks.MinValue"/> or greater than <see cref="Ticks.MaxValue"/>.
        /// </exception>
        /// <exception cref="FormatException">s is not in the correct format.</exception>
        public static Ticks Parse(string s)
        {
            return (Ticks)long.Parse(s);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style to its <see cref="Ticks"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the permitted format of s.
        /// </param>
        /// <returns>
        /// A <see cref="Ticks"/> equivalent to the number contained in s.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// style is not a System.Globalization.NumberStyles value. -or- style is not a combination of
        /// System.Globalization.NumberStyles.AllowHexSpecifier and System.Globalization.NumberStyles.HexNumber values.
        /// </exception>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="OverflowException">
        /// s represents a number less than <see cref="Ticks.MinValue"/> or greater than <see cref="Ticks.MaxValue"/>.
        /// </exception>
        /// <exception cref="FormatException">s is not in a format compliant with style.</exception>
        public static Ticks Parse(string s, NumberStyles style)
        {
            return (Ticks)long.Parse(s, style);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified culture-specific format to its <see cref="Ticks"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="provider">
        /// A <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information about s.
        /// </param>
        /// <returns>
        /// A <see cref="Ticks"/> equivalent to the number contained in s.
        /// </returns>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="OverflowException">
        /// s represents a number less than <see cref="Ticks.MinValue"/> or greater than <see cref="Ticks.MaxValue"/>.
        /// </exception>
        /// <exception cref="FormatException">s is not in the correct format.</exception>
        public static Ticks Parse(string s, IFormatProvider provider)
        {
            return (Ticks)long.Parse(s, provider);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its <see cref="Ticks"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the permitted format of s.
        /// </param>
        /// <param name="provider">
        /// A <see cref="System.IFormatProvider"/> that supplies culture-specific formatting information about s.
        /// </param>
        /// <returns>
        /// A <see cref="Ticks"/> equivalent to the number contained in s.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// style is not a System.Globalization.NumberStyles value. -or- style is not a combination of
        /// System.Globalization.NumberStyles.AllowHexSpecifier and System.Globalization.NumberStyles.HexNumber values.
        /// </exception>
        /// <exception cref="ArgumentNullException">s is null.</exception>
        /// <exception cref="OverflowException">
        /// s represents a number less than <see cref="Ticks.MinValue"/> or greater than <see cref="Ticks.MaxValue"/>.
        /// </exception>
        /// <exception cref="FormatException">s is not in a format compliant with style.</exception>
        public static Ticks Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            return (Ticks)long.Parse(s, style, provider);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Ticks"/> equivalent. A return value
        /// indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="Ticks"/> value equivalent to the number contained in s,
        /// if the conversion succeeded, or zero if the conversion failed. The conversion fails if the s parameter is null,
        /// is not of the correct format, or represents a number less than <see cref="Ticks.MinValue"/> or greater than <see cref="Ticks.MaxValue"/>.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if s was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string s, out Ticks result)
        {
            long parseResult;
            bool parseResponse;

            parseResponse = long.TryParse(s, out parseResult);
            result = parseResult;

            return parseResponse;
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its
        /// <see cref="Ticks"/> equivalent. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the permitted format of s.
        /// </param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="Ticks"/> value equivalent to the number contained in s,
        /// if the conversion succeeded, or zero if the conversion failed. The conversion fails if the s parameter is null,
        /// is not in a format compliant with style, or represents a number less than <see cref="Ticks.MinValue"/> or
        /// greater than <see cref="Ticks.MaxValue"/>. This parameter is passed uninitialized.
        /// </param>
        /// <param name="provider">
        /// A <see cref="System.IFormatProvider"/> object that supplies culture-specific formatting information about s.
        /// </param>
        /// <returns>true if s was converted successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentException">
        /// style is not a System.Globalization.NumberStyles value. -or- style is not a combination of
        /// System.Globalization.NumberStyles.AllowHexSpecifier and System.Globalization.NumberStyles.HexNumber values.
        /// </exception>
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out Ticks result)
        {
            long parseResult;
            bool parseResponse;

            parseResponse = long.TryParse(s, style, provider, out parseResult);
            result = parseResult;

            return parseResponse;
        }

        /// <summary>
        /// Gets a sub-second time distribution in <see cref="Ticks"/> for the specified <paramref name="samplesPerSecond"/>.
        /// </summary>
        /// <param name="samplesPerSecond">Samples per second.</param>
        /// <returns>Array of sub-second time distribution in <see cref="Ticks"/>.</returns>
        public static Ticks[] SubsecondDistribution(int samplesPerSecond)
        {
            Ticks[] sampleTimes = new Ticks[samplesPerSecond];
            double sampleFraction = 1.0D / samplesPerSecond;

            for (int i = 0; i < samplesPerSecond; i++)
                sampleTimes[i] = new Ticks((long)(Ticks.PerSecond * (i * sampleFraction)));

            return sampleTimes;
        }

        /// <summary>
        /// Gets a sub-second time distribution in milliseconds for the specified <paramref name="samplesPerSecond"/>.
        /// </summary>
        /// <param name="samplesPerSecond">Samples per second.</param>
        /// <returns>Array of sub-second time distribution in milliseconds.</returns>
        public static int[] MillisecondDistribution(int samplesPerSecond)
        {
            return SubsecondDistribution(samplesPerSecond).Select(ticks => (int)ticks.ToMilliseconds()).ToArray();
        }

        /// <summary>
        /// Gets a sub-second time distribution in microseconds for the specified <paramref name="samplesPerSecond"/>.
        /// </summary>
        /// <param name="samplesPerSecond">Samples per second.</param>
        /// <returns>Array of sub-second time distribution in microseconds.</returns>
        public static int[] MicrosecondDistribution(int samplesPerSecond)
        {
            return SubsecondDistribution(samplesPerSecond).Select(ticks => (int)ticks.ToMicroseconds()).ToArray();
        }

        /// <summary>
        /// Returns a floor-aligned sub-second distribution timestamp for given <paramref name="timestamp"/>.
        /// </summary>
        /// <param name="timestamp">Timestamp to align.</param>
        /// <param name="samplesPerSecond">Samples per second to use for distribution.</param>
        /// <param name="timeResolution">Defines the time resolution to use when aligning <paramref name="timestamp"/> to its proper distribution timestamp.</param>
        /// <returns>A floor-aligned sub-second distribution timestamp for given <paramref name="timestamp"/>.</returns>
        /// <remarks>
        /// Time resolution value is typically a power of 10 based on the number of ticks per the desired resolution. The following table defines
        /// common resolutions and their respective <paramref name="timeResolution"/> values:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Resolution</term>
        ///         <description>Time Resolution Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Seconds</term>
        ///         <description><see cref="Ticks.PerSecond"/></description>
        ///     </item>
        ///     <item>
        ///         <term>Milliseconds</term>
        ///         <description><see cref="Ticks.PerMillisecond"/></description>
        ///     </item>
        ///     <item>
        ///         <term>Microseconds</term>
        ///         <description><see cref="Ticks.PerMicrosecond"/></description>
        ///     </item>
        ///     <item>
        ///         <term>Ticks</term>
        ///         <description>0</description>
        ///     </item>
        /// </list>
        /// If source timestamps have variation, i.e., they are not aligned within common distributions, the <paramref name="timeResolution"/>
        /// can be adjusted to include slack to accommodate the variance. When including slack in the time resolution, the value will depend
        /// on the <paramref name="samplesPerSecond"/>, for example: you could use 330,000 for 30 samples per second, 160,000 for 60 samples
        /// per second, and 80,000 for 120 samples per second. Actual slack value may need to be more or less depending on the size of the
        /// source timestamp variation.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ticks AlignToSubsecondDistribution(Ticks timestamp, int samplesPerSecond, long timeResolution)
        {
            // Calculate destination ticks for this frame
            long ticks = timestamp.Value, baseTicks, ticksBeyondSecond, frameIndex, destinationTicks, nextDestinationTicks;

            // Baseline timestamp to the top of the second
            baseTicks = ticks - ticks % Ticks.PerSecond;

            // Remove the seconds from ticks
            ticksBeyondSecond = ticks - baseTicks;

            // Calculate a frame index between 0 and m_framesPerSecond-1, corresponding to ticks
            // rounded down to the nearest frame
            frameIndex = (long)(ticksBeyondSecond / (Ticks.PerSecond / (double)samplesPerSecond));

            // Calculate the timestamp of the nearest frame rounded up
            nextDestinationTicks = (frameIndex + 1) * Ticks.PerSecond / samplesPerSecond;

            // Determine whether the desired frame is the nearest
            // frame rounded down or the nearest frame rounded up
            if (timeResolution <= 1)
            {
                if (nextDestinationTicks <= ticksBeyondSecond)
                    destinationTicks = nextDestinationTicks;
                else
                    destinationTicks = frameIndex * Ticks.PerSecond / samplesPerSecond;
            }
            else
            {
                // If, after translating nextDestinationTicks to the time resolution, it is less than
                // or equal to ticks, nextDestinationTicks corresponds to the desired frame
                if ((nextDestinationTicks / timeResolution) * timeResolution <= ticksBeyondSecond)
                    destinationTicks = nextDestinationTicks;
                else
                    destinationTicks = frameIndex * Ticks.PerSecond / samplesPerSecond;
            }

            // Recover the seconds that were removed
            destinationTicks += baseTicks;

            return new Ticks(destinationTicks);
        }

        /// <summary>
        /// Returns a floor-aligned millisecond distribution timestamp for given <paramref name="timestamp"/>.
        /// </summary>
        /// <param name="timestamp">Timestamp to align.</param>
        /// <param name="samplesPerSecond">Samples per second to use for distribution.</param>
        /// <returns>A floor-aligned millisecond distribution timestamp for given <paramref name="timestamp"/>.</returns>
        public static Ticks AlignToMillisecondDistribution(Ticks timestamp, int samplesPerSecond)
        {
            return AlignToSubsecondDistribution(timestamp, samplesPerSecond, Ticks.PerMillisecond);
        }

        /// <summary>
        /// Returns a floor-aligned microsecond distribution timestamp for given <paramref name="timestamp"/>.
        /// </summary>
        /// <param name="timestamp">Timestamp to align.</param>
        /// <param name="samplesPerSecond">Samples per second to use for distribution.</param>
        /// <returns>A floor-aligned microsecond distribution timestamp for given <paramref name="timestamp"/>.</returns>
        public static Ticks AlignToMicrosecondDistribution(Ticks timestamp, int samplesPerSecond)
        {
            return AlignToSubsecondDistribution(timestamp, samplesPerSecond, Ticks.PerMicrosecond);
        }

        #endregion
    }
}