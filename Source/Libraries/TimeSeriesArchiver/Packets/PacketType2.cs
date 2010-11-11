//******************************************************************************************************
//  PacketType2.cs - Gbtc
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
//  07/27/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/23/2009 - Pinal C. Patel
//       Edited code comments.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using TimeSeriesArchiver.Files;
using TVA;

namespace TimeSeriesArchiver.Packets
{
    /// <summary>
    /// Represents a packet to be used for sending single time (expanded format) series data point to a historian for archival.
    /// </summary>
    public class PacketType2 : PacketBase
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 2          0-1        Int16      TypeID (packet identifier)                                    *
        // * 4          2-5        Int32      HistorianID                                                   *
        // * 2          6-7        Int16      Year                                                          *
        // * 1          8          Byte       Month                                                         *
        // * 1          9          Byte       Day                                                           *
        // * 1          10         Byte       Hour                                                          *
        // * 1          11         Byte       Minute                                                        *
        // * 1          12         Byte       Second                                                        *
        // * 1          13         Byte       Quality                                                       *
        // * 2          14-15      Int16      Milliseconds                                                  *
        // * 2          16-17      Int16      GmtOffset                                                     *
        // * 4          18-21      Single     Value                                                         *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="PacketType2"/>.
        /// </summary>
        public new const int ByteCount = 22;

        // Fields
        private int m_historianID;
        private short m_year;
        private short m_month;
        private short m_day;
        private short m_hour;
        private short m_minute;
        private short m_second;
        private Quality m_quality;
        private short m_millisecond;
        private short m_gmtOffset;
        private float m_value;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType2"/> class.
        /// </summary>
        public PacketType2()
            : base(2)
        {
            ProcessHandler = Process;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType2"/> class.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="PacketType2"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        public PacketType2(byte[] binaryImage, int startIndex, int length)
            : this()
        {
            Initialize(binaryImage, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the historian identifier of the time-series data.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive.</exception>
        public int HistorianID
        {
            get
            {
                return m_historianID;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Value must be positive.");

                m_historianID = value;
            }
        }

        /// <summary>
        /// Gets or sets the year-part of the time.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 1995 and 2063.</exception>
        public short Year
        {
            get
            {
                return m_year;
            }
            set
            {
                if (value < TimeTag.MinValue.ToDateTime().Year || value > TimeTag.MaxValue.ToDateTime().Year)
                    throw new ArgumentException("Value must 1995 and 2063.");

                m_year = value;
            }
        }

        /// <summary>
        /// Gets or sets the month-part of the time.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 1 and 12.</exception>
        public short Month
        {
            get
            {
                return m_month;
            }
            set
            {
                if (value < 1 || value > 12)
                    throw new ArgumentException("Value must be between 1 and 12.");

                m_month = value;
            }
        }

        /// <summary>
        /// Gets or sets the day-part of the time.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 1 and 31.</exception>
        public short Day
        {
            get
            {
                return m_day;
            }
            set
            {
                if (value < 1 || value > 31)
                    throw new ArgumentException("Value must be between 1 and 31.");

                m_day = value;
            }
        }

        /// <summary>
        /// Gets or sets the hour-part of the time.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 0 and 23.</exception>
        public short Hour
        {
            get
            {
                return m_hour;
            }
            set
            {
                if (value < 0 || value > 23)
                    throw new ArgumentException("Value must be between 0 and 23.");

                m_hour = value;
            }
        }

        /// <summary>
        /// Gets or sets the minute-part of the time.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 0 and 59.</exception>
        public short Minute
        {
            get
            {
                return m_minute;
            }
            set
            {
                if (value < 0 || value > 59)
                    throw new ArgumentException("Value must be between 0 and 59.");

                m_minute = value;
            }
        }

        /// <summary>
        /// Gets or sets the second-part of the time.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 0 and 59.</exception>
        public short Second
        {
            get
            {
                return m_second;
            }
            set
            {
                if (value < 0 || value > 59)
                    throw new ArgumentException("Value must be between 0 and 59.");

                m_second = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Quality"/> of the time-series data.
        /// </summary>
        public Quality Quality
        {
            get
            {
                return m_quality;
            }
            set
            {
                m_quality = value;
            }
        }

        /// <summary>
        /// Gets or sets the millisecond-part of the time.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 0 and 999.</exception>
        public short Millisecond
        {
            get
            {
                return m_millisecond;
            }
            set
            {
                if (value < 0 || value > 999)
                    throw new ArgumentException("Value must be between 0 and 999.");

                m_millisecond = value;
            }
        }

        /// <summary>
        /// Gets or sets the difference, in hours, between the local time and Greenwich Mean Time (Universal Coordinated Time).
        /// </summary>
        public short GmtOffset
        {
            get
            {
                return m_gmtOffset;
            }
            set
            {
                m_gmtOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the time-series data.
        /// </summary>
        public float Value
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
        /// Gets the length of the <see cref="BinaryImage"/>.
        /// </summary>
        public override int BinaryLength
        {
            get
            {
                return ByteCount;
            }
        }

        /// <summary>
        /// Gets the binary representation of <see cref="PacketType2"/>.
        /// </summary>
        public override byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(EndianOrder.LittleEndian.GetBytes(TypeID), 0, image, 0, 2);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_historianID), 0, image, 2, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_year), 0, image, 6, 2);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_month), 0, image, 8, 1);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_day), 0, image, 9, 1);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_hour), 0, image, 10, 1);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_minute), 0, image, 11, 1);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_second), 0, image, 12, 1);
                Array.Copy(EndianOrder.LittleEndian.GetBytes((int)m_quality), 0, image, 13, 1);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_millisecond), 0, image, 14, 2);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_gmtOffset), 0, image, 16, 2);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_value), 0, image, 18, 4);

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="PacketType2"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="PacketType2"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="PacketType2"/>.</returns>
        public override int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                short packetID = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex);
                if (packetID != TypeID)
                    throw new ArgumentException(string.Format("Unexpected packet id '{0}' (expected '{1}')", packetID, TypeID));

                // We have a binary image with the correct packet id.
                HistorianID = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2);
                Year = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex + 6);
                Month = Convert.ToInt16(binaryImage[startIndex + 8]);
                Day = Convert.ToInt16(binaryImage[startIndex + 9]);
                Hour = Convert.ToInt16(binaryImage[startIndex + 10]);
                Minute = Convert.ToInt16(binaryImage[startIndex + 11]);
                Second = Convert.ToInt16(binaryImage[startIndex + 12]);
                Quality = (Quality)(binaryImage[startIndex + 13]);
                Millisecond = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex + 14);
                GmtOffset = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex + 16);
                Value = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 18);

                // We'll send an "ACK" to the sender if this is the last packet in the transmission.
                if (length == ByteCount)
                    PreProcessHandler = PreProcess;

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Extracts time-series data from <see cref="PacketType2"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> object of <see cref="ArchiveDataPoint"/>s.</returns>
        public override IEnumerable<IDataPoint> ExtractTimeSeriesData()
        {
            DateTime timestamp = new DateTime(m_year, m_month, m_day, m_hour + m_gmtOffset, m_minute, m_second, m_millisecond, DateTimeKind.Utc);

            return new ArchiveDataPoint[] { new ArchiveDataPoint(m_historianID, new TimeTag(timestamp), m_value, m_quality) };
        }

        /// <summary>
        /// Processes <see cref="PacketType2"/>.
        /// </summary>
        /// <returns>A null reference.</returns>
        protected virtual IEnumerable<byte[]> Process()
        {
            if (Archive != null)
            {
                foreach (IDataPoint dataPoint in ExtractTimeSeriesData())
                {
                    Archive.WriteData(dataPoint);
                }
            }

            return null;
        }

        /// <summary>
        /// Pre-processes <see cref="PacketType2"/>.
        /// </summary>
        /// <returns>A <see cref="byte"/> array for "ACK".</returns>
        protected virtual IEnumerable<byte[]> PreProcess()
        {
            return new byte[][] { Encoding.ASCII.GetBytes("ACK") };
        }

        #endregion
    }
}