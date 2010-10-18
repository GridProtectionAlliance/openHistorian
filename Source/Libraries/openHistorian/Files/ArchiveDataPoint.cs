//******************************************************************************************************
//  ArchiveDataPoint.cs - Gbtc
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
//  02/23/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/10/2009 - Pinal C. Patel
//       Added contructor that takes in IMeasurement.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  03/15/2010 - Pinal C. Patel
//       Implemented IFormattable.ToString() overloads.
//  04/20/2010 - J. Ritchie Carroll
//       Added construction overload for IMeasurement that accepts specific quality.
//  09/16/2010 - J. Ritchie Carroll
//       Modified formatted time to include milliseconds.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Globalization;
using TimeSeriesFramework;
using TVA;

namespace openHistorian.Files
{
    /// <summary>
    /// Represents time-series data stored in <see cref="ArchiveFile"/>.
    /// </summary>
    /// <seealso cref="ArchiveFile"/>
    /// <seealso cref="ArchiveFileAllocationTable"/>
    /// <seealso cref="ArchiveDataBlock"/>
    /// <seealso cref="ArchiveDataBlockPointer"/>
    public class ArchiveDataPoint : IDataPoint
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 4          0-3        Int32      Time                                                          *
        // * 2          4-5        Int16      Flags (Quality & Milliseconds)                                *
        // * 4          6-9        Single     Value                                                         *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="ArchiveDataPoint"/>.
        /// </summary>
        public const int ByteCount = 10;

        /// <summary>
        /// Specifies the bit-mask for <see cref="Quality"/> stored in <see cref="Flags"/>.
        /// </summary>
        [CLSCompliant(false)]
        protected const Bits QualityMask = Bits.Bit00 | Bits.Bit01 | Bits.Bit02 | Bits.Bit03 | Bits.Bit04;

        /// <summary>
        /// Specifies the bit-mask for <see cref="TimeTag"/> milliseconds stored in <see cref="Flags"/>.
        /// </summary>
        [CLSCompliant(false)]
        protected const Bits MillisecondMask = Bits.Bit05 | Bits.Bit06 | Bits.Bit07 | Bits.Bit08 | Bits.Bit09 | Bits.Bit10 | Bits.Bit11 | Bits.Bit12 | Bits.Bit13 | Bits.Bit14 | Bits.Bit15;

        // Fields
        private int m_historianID;
        private TimeTag m_time;
        private float m_value;
        private int m_flags;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataPoint"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="ArchiveDataPoint"/>.</param>
        public ArchiveDataPoint(int historianID)
        {
            m_time = TimeTag.MinValue;
            this.HistorianID = historianID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataPoint"/> class.
        /// </summary>
        /// <param name="dataPoint">A time-series data point.</param>
        public ArchiveDataPoint(IDataPoint dataPoint)
            : this(dataPoint.HistorianID, dataPoint.Time, dataPoint.Value, dataPoint.Quality)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataPoint"/> class from a <see cref="IMeasurement"/> value.
        /// </summary>
        /// <param name="measurement">Object that implements the <see cref="IMeasurement"/> interface.</param>
        [CLSCompliant(false)]
        public ArchiveDataPoint(IMeasurement measurement)
            : this(measurement, measurement.IsDiscarded ? Quality.DeletedFromProcessing : (measurement.ValueQualityIsGood ? (measurement.TimestampQualityIsGood ? Quality.Good : Quality.Old) : Quality.SuspectData))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataPoint"/> class from a <see cref="IMeasurement"/> value.
        /// </summary>
        /// <param name="measurement">Object that implements the <see cref="IMeasurement"/> interface.</param>
        /// <param name="quality">Specific <see cref="Quality"/> value to apply to new <see cref="ArchiveDataPoint"/>.</param>
        [CLSCompliant(false)]
        public ArchiveDataPoint(IMeasurement measurement, Quality quality)
            : this((int)measurement.ID)
        {
            this.Time = new TimeTag((DateTime)measurement.Timestamp);
            this.Value = (float)measurement.AdjustedValue;
            this.Quality = quality;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataPoint"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="ArchiveDataPoint"/>.</param>
        /// <param name="time"><see cref="TimeTag"/> of <see cref="ArchiveDataPoint"/>.</param>
        /// <param name="value">Floating-point value of <see cref="ArchiveDataPoint"/>.</param>
        /// <param name="quality"><see cref="Quality"/> of <see cref="ArchiveDataPoint"/>.</param>
        public ArchiveDataPoint(int historianID, TimeTag time, float value, Quality quality)
            : this(historianID)
        {
            this.Time = time;
            this.Value = value;
            this.Quality = quality;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataPoint"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="ArchiveDataPoint"/>.</param>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="ArchiveDataPoint"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        public ArchiveDataPoint(int historianID, byte[] binaryImage, int startIndex, int length)
            : this(historianID)
        {
            Initialize(binaryImage, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the historian identifier of <see cref="ArchiveDataPoint"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive or -1.</exception>
        public int HistorianID
        {
            get
            {
                return m_historianID;
            }
            set
            {
                if (value < 1 && value != -1)
                    throw new ArgumentException("Value must be positive or -1");

                m_historianID = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of <see cref="ArchiveDataPoint"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 01/01/1995 and 01/19/2063.</exception>
        public virtual TimeTag Time
        {
            get
            {
                return m_time;
            }
            set
            {
                if (value < TimeTag.MinValue || value > TimeTag.MaxValue)
                    throw new ArgumentException("Value must between 01/01/1995 and 01/19/2063");

                m_time = value;
                Flags = Flags.SetMaskedValue(MillisecondMask, m_time.ToDateTime().Millisecond << 5);
            }
        }

        /// <summary>
        /// Gets or sets the floating-point value of <see cref="ArchiveDataPoint"/>.
        /// </summary>
        public virtual float Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Quality"/> of <see cref="ArchiveDataPoint"/>.
        /// </summary>
        public virtual Quality Quality
        {
            get
            {
                return (Quality)Flags.GetMaskedValue(QualityMask);
            }
            set
            {
                Flags = Flags.SetMaskedValue(QualityMask, (int)value);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether <see cref="ArchiveDataPoint"/> contains any data.
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                return ((m_time == TimeTag.MinValue) &&
                        (m_value == default(float)) &&
                        (Quality == Quality.Unknown));
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="BinaryImage"/>.
        /// </summary>
        public virtual int BinaryLength
        {
            get
            {
                return ByteCount;
            }
        }

        /// <summary>
        /// Gets the binary representation of <see cref="ArchiveDataPoint"/>.
        /// </summary>
        public virtual byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(EndianOrder.LittleEndian.GetBytes((int)m_time.Value), 0, image, 0, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes((short)m_flags), 0, image, 4, 2);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_value), 0, image, 6, 4);

                return image;
            }
        }

        /// <summary>
        /// Gets or sets the 32-bit word used for storing data of <see cref="ArchiveDataPoint"/>.
        /// </summary>
        protected virtual int Flags
        {
            get
            {
                return m_flags;
            }
            set
            {
                m_flags = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="ArchiveDataPoint"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="ArchiveDataPoint"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="ArchiveDataPoint"/>.</returns>
        public virtual int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                Flags = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex + 4);
                Value = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 6);
                Time = new TimeTag(EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex) +          // Seconds
                                      ((double)(m_flags.GetMaskedValue(MillisecondMask) >> 5) / 1000)); // Milliseconds

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Compares the current <see cref="ArchiveDataPoint"/> object to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="ArchiveDataPoint"/> object is to be compared.</param>
        /// <returns>
        /// Negative value if the current <see cref="ArchiveDataPoint"/> object is less than <paramref name="obj"/>, 
        /// Zero if the current <see cref="ArchiveDataPoint"/> object is equal to <paramref name="obj"/>, 
        /// Positive value if the current <see cref="ArchiveDataPoint"/> object is greater than <paramref name="obj"/>.
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            ArchiveDataPoint other = obj as ArchiveDataPoint;
            if (other == null)
            {
                return 1;
            }
            else
            {
                int result = m_historianID.CompareTo(other.HistorianID);
                if (result != 0)
                    return result;
                else
                    return m_time.CompareTo(other.Time);
            }
        }

        /// <summary>
        /// Determines whether the current <see cref="ArchiveDataPoint"/> object is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="ArchiveDataPoint"/> object is to be compared for equality.</param>
        /// <returns>true if the current <see cref="ArchiveDataPoint"/> object is equal to <paramref name="obj"/>; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Returns the text representation of <see cref="ArchiveDataPoint"/> object.
        /// </summary>
        /// <returns>A <see cref="string"/> value.</returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Returns the text representation of <see cref="ArchiveDataPoint"/> object in the specified <paramref name="format"/>.
        /// </summary>
        /// <param name="format">Format of text output (I for ID, T for Time, V for Value, Q for Quality).</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public virtual string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Returns the text representation of <see cref="ArchiveDataPoint"/> object using the specified <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public virtual string ToString(IFormatProvider provider)
        {
            return ToString(null, provider);
        }

        /// <summary>
        /// Returns the text representation of <see cref="ArchiveDataPoint"/> object in the specified <paramref name="format"/> 
        /// using the specified <paramref name="provider"/>.
        /// </summary>
        /// <param name="format">Format of text output (I for ID, T for Time, V for Value, Q for Quality).</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public virtual string ToString(string format, IFormatProvider provider)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            switch (format)
            {
                case "I":
                    return m_historianID.ToString(provider);
                case "T":
                    return m_time.ToString();
                case "V":
                    return m_value.ToString(provider);
                case "Q":
                    return Quality.ToString();
                default:
                    return string.Format("ID={0}; Time={1}; Value={2}; Quality={3}",
                                         m_historianID.ToString(provider), m_time.ToString(), m_value.ToString(provider), Quality.ToString());
            }
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="ArchiveDataPoint"/> object.
        /// </summary>
        /// <returns>A 32-bit signed integer value.</returns>
        public override int GetHashCode()
        {
            return m_historianID.GetHashCode();
        }

        #endregion
    }
}
