//******************************************************************************************************
//  TimeTagBase.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  08/4/2009 - Josh L. Patterson
//       Edited Code Comments.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  03/12/2010 - Pinal C. Patel
//       Added the implementation of IFormattable interface.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

#region [ Contributor License Agreements ]

/**************************************************************************\
   Copyright © 2009 - J. Ritchie Carroll, Pinal C. Patel
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
using System.Runtime.Serialization;

namespace GSF
{
    /// <summary>
    /// Represents the base class for alternate timetag implementations.
    /// </summary>
    [Serializable]
    public abstract class TimeTagBase : ISerializable, IComparable, IComparable<TimeTagBase>, IComparable<DateTime>, IEquatable<TimeTagBase>, IEquatable<DateTime>, IFormattable
    {
        #region [ Members ]

        // Fields
        private readonly long m_baseDateOffsetTicks;
        private double m_seconds;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="TimeTagBase"/>, given number base time (in ticks) and seconds since base time.
        /// </summary>
        /// <param name="baseDateOffsetTicks">Ticks of timetag base.</param>
        /// <param name="seconds">Number of seconds since base time.</param>
        protected TimeTagBase(long baseDateOffsetTicks, double seconds)
        {
            m_baseDateOffsetTicks = baseDateOffsetTicks;
            Value = seconds;
        }

        /// <summary>
        /// Creates a new <see cref="TimeTagBase"/>, given standard .NET <see cref="DateTime"/>.
        /// </summary>
        /// <param name="baseDateOffsetTicks">Ticks of timetag base.</param>
        /// <param name="timestamp">Timestamp in <see cref="Ticks"/> used to create timetag from.</param>
        protected TimeTagBase(long baseDateOffsetTicks, Ticks timestamp)
        {
            // Zero base 100-nanosecond ticks from 1/1/1970 and convert to seconds.
            m_baseDateOffsetTicks = baseDateOffsetTicks;
            Value = (timestamp - m_baseDateOffsetTicks).ToSeconds();
        }

        /// <summary>
        /// Creates a new <see cref="TimeTagBase"/> from serialization parameters.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> with populated with data.</param>
        /// <param name="context">The source <see cref="StreamingContext"/> for this deserialization.</param>
        protected TimeTagBase(SerializationInfo info, StreamingContext context)
        {
            // Deserializes timetag
            m_baseDateOffsetTicks = info.GetInt64("baseDateOffsetTicks");
            m_seconds = info.GetDouble("seconds");
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets number of seconds (including any fractional seconds) since base time.
        /// </summary>
        public virtual double Value
        {
            get
            {
                return m_seconds;
            }
            set
            {
                m_seconds = value;
            }
        }

        /// <summary>
        /// Gets ticks representing the absolute minimum time of this timetag implementation.
        /// </summary>
        public virtual long BaseDateOffsetTicks
        {
            get
            {
                return m_baseDateOffsetTicks;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns standard .NET <see cref="DateTime"/> representation for timetag.
        /// </summary>
        /// <returns>A <see cref="DateTime"/>.</returns>
        public virtual DateTime ToDateTime()
        {
            // Converts m_seconds to 100-nanosecond ticks and add the base time offset.
            return new DateTime((long)(m_seconds * Ticks.PerSecond) + m_baseDateOffsetTicks);
        }

        /// <summary>
        /// Returns basic textual representation for timetag.
        /// </summary>
        /// <remarks>
        /// Format is "yyyy-MM-dd HH:mm:ss.fff" so that textual representation can be sorted in the
        /// correct chronological order.
        /// </remarks>
        /// <returns>A <see cref="string"/> value representing the timetag.</returns>
        public override string ToString()
        {
            return ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// Returns textual representation for timetag in the specified <paramref name="format"/>.
        /// </summary>
        /// <param name="format">Format of text output.</param>
        /// <returns><see cref="string"/> of textual representation for timetag.</returns>
        public virtual string ToString(string format)
        {
            return ToDateTime().ToString(format);
        }

        /// <summary>
        /// Returns textual representation for timetag using the specified <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns><see cref="string"/> of textual representation for timetag.</returns>
        public virtual string ToString(IFormatProvider provider)
        {
            return ToDateTime().ToString(provider);
        }

        /// <summary>
        /// Returns textual representation for timetag in the specified <paramref name="format"/> using 
        /// the specified <paramref name="provider"/>.
        /// </summary>
        /// <param name="format">Format of text output.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns><see cref="string"/> of textual representation for timetag.</returns>
        public virtual string ToString(string format, IFormatProvider provider)
        {
            return ToDateTime().ToString(format, provider);
        }

        /// <summary>
        /// Compares the <see cref="TimeTagBase"/> with another <see cref="TimeTagBase"/>.
        /// </summary>
        /// <param name="other">The <see cref="TimeTagBase"/> to compare with the current <see cref="TimeTagBase"/>.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(TimeTagBase other)
        {
            // Since compared time tags may not have the same base time, we compare using .NET date time.
            if (Equals(other, null))
                return 1;
            else
                return CompareTo(other.ToDateTime());
        }

        /// <summary>
        /// Compares the <see cref="TimeTagBase"/> with a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="other">The <see cref="DateTime"/> to compare with the current <see cref="TimeTagBase"/>.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(DateTime other)
        {
            return ToDateTime().CompareTo(other);
        }

        /// <summary>
        /// Compares the <see cref="TimeTagBase"/> with the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="TimeTagBase"/>.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
        /// <exception cref="ArgumentException"><see cref="Object"/> is not an <see cref="TimeTagBase"/> or a <see cref="DateTime"/>.</exception>
        public virtual int CompareTo(object obj)
        {
            TimeTagBase other = obj as TimeTagBase;

            if ((object)other != null)
                return CompareTo(other);

            if (obj is DateTime)
                return CompareTo((DateTime)obj);

            throw new ArgumentException("TimeTag can only be compared with other TimeTags or DateTimes");
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="TimeTagBase"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="TimeTagBase"/>.</param>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="TimeTagBase"/>;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException"><see cref="Object"/> is not an <see cref="TimeTagBase"/>.</exception>
        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Determines whether the specified <see cref="TimeTagBase"/> is equal to the current <see cref="TimeTagBase"/>.
        /// </summary>
        /// <param name="other">The <see cref="TimeTagBase"/> to compare with the current <see cref="TimeTagBase"/>.</param>
        /// <returns>
        /// true if the specified <see cref="TimeTagBase"/> is equal to the current <see cref="TimeTagBase"/>;
        /// otherwise, false.
        /// </returns>
        public bool Equals(TimeTagBase other)
        {
            return (CompareTo(other) == 0);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> is equal to the current <see cref="TimeTagBase"/>.
        /// </summary>
        /// <param name="other">The <see cref="DateTime"/> to compare with the current <see cref="TimeTagBase"/>.</param>
        /// <returns>
        /// true if the specified <see cref="DateTime"/> is equal to the current <see cref="TimeTagBase"/>;
        /// otherwise, false.
        /// </returns>
        public bool Equals(DateTime other)
        {
            return (CompareTo(other) == 0);
        }

        /// <summary>
        /// Serves as a hash function for the current <see cref="TimeTagBase"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="TimeTagBase"/>.</returns>
        /// <remarks>Hash code based on number of seconds timetag represents.</remarks>
        public override int GetHashCode()
        {
            return m_seconds.GetHashCode();
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination <see cref="StreamingContext"/> for this serialization.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serializes timetag.
            info.AddValue("baseDateOffsetTicks", m_baseDateOffsetTicks);
            info.AddValue("seconds", m_seconds);
        }

        #endregion

        #region [ Operators ]

        #region [ == Operators ]

        /// <summary>
        /// Returns true if <paramref name="value1"/> is equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator ==(TimeTagBase value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2) == 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator ==(DateTime value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2.ToDateTime()) == 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator ==(TimeTagBase value1, DateTime value2)
        {
            return (value1.CompareTo(value2) == 0);
        }

        #endregion

        #region [ != Operators ]

        /// <summary>
        /// Returns true if <paramref name="value1"/> is not equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is not equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator !=(TimeTagBase value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2) != 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is not equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is not equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator !=(DateTime value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2.ToDateTime()) != 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is not equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is not equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator !=(TimeTagBase value1, DateTime value2)
        {
            return (value1.CompareTo(value2) != 0);
        }

        #endregion

        #region [ <  Operators ]

        /// <summary>
        /// Returns true if <paramref name="value1"/> is less than <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is less than <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator <(TimeTagBase value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2) < 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is less than <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is less than <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator <(DateTime value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2.ToDateTime()) < 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is less than <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is less than <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator <(TimeTagBase value1, DateTime value2)
        {
            return (value1.CompareTo(value2) < 0);
        }

        #endregion

        #region [ <= Operators ]

        /// <summary>
        /// Returns true if <paramref name="value1"/> is less than or equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is less than or equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator <=(TimeTagBase value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2) <= 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is less than or equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is less than or equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator <=(DateTime value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2.ToDateTime()) <= 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is less than or equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is less than or equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator <=(TimeTagBase value1, DateTime value2)
        {
            return (value1.CompareTo(value2) <= 0);
        }

        #endregion

        #region [ >  Operators ]

        /// <summary>
        /// Returns true if <paramref name="value1"/> is greater than <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is greater than <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator >(TimeTagBase value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2) > 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is greater than <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is greater than <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator >(DateTime value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2.ToDateTime()) > 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is greater than <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is greater than <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator >(TimeTagBase value1, DateTime value2)
        {
            return (value1.CompareTo(value2) > 0);
        }

        #endregion

        #region [ >= Operators ]

        /// <summary>
        /// Returns true if <paramref name="value1"/> is greater than or equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is greater than or equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator >=(TimeTagBase value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2) >= 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is greater than or equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is greater than or equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator >=(DateTime value1, TimeTagBase value2)
        {
            return (value1.CompareTo(value2.ToDateTime()) >= 0);
        }

        /// <summary>
        /// Returns true if <paramref name="value1"/> is greater than or equal to <paramref name="value2"/>.
        /// </summary>
        /// <param name="value1">Value 1 in the comparision.</param>
        /// <param name="value2">Value 2 in the comparision.</param>
        /// <returns>true if <paramref name="value1"/> is greater than or equal to <paramref name="value2"/>; otherwise false.</returns>
        public static bool operator >=(TimeTagBase value1, DateTime value2)
        {
            return (value1.CompareTo(value2) >= 0);
        }

        #endregion

        #endregion
    }
}