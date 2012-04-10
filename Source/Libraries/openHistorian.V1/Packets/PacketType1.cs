//******************************************************************************************************
//  PacketType1.cs - Gbtc
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
//  11/30/2011 - J. Ritchie Carroll
//       Modified to support buffer optimized ISupportBinaryImage.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using openHistorian.V1.Files;
using TimeSeriesFramework;
using TVA;

namespace openHistorian.V1.Adapters
{
    /// <summary>
    /// Represents a packet to be used for sending single time-series data point to a historian for archival.
    /// </summary>
    public class PacketType1 : PacketBase
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 2          0-1        Int16      TypeID (packet identifier)                                    *
        // * 4          2-5        Int32      HistorianID                                                   *
        // * 8          6-13       Double     Time                                                          *
        // * 4          14-17      Int32      Quality                                                       *
        // * 4          18-21      Single     Value                                                         *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="PacketType1"/>.
        /// </summary>
        public new const int FixedLength = 22;

        // Fields
        private int m_historianID;
        private TimeTag m_time;
        private Quality m_quality;
        private float m_value;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType1"/> class.
        /// </summary>
        public PacketType1()
            : base(1)
        {
            Time = TimeTag.MinValue;
            ProcessHandler = Process;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType1"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier.</param>
        public PacketType1(int historianID)
            : this()
        {
            HistorianID = historianID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType1"/> class.
        /// </summary>
        /// <param name="dataPoint">Object that implements the <see cref="IDataPoint"/> interface.</param>
        public PacketType1(IDataPoint dataPoint)
            : this()
        {
            HistorianID = dataPoint.HistorianID;
            Time = dataPoint.Time;
            Value = dataPoint.Value;
            Quality = (Quality)dataPoint.Flags;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType1"/> class.
        /// </summary>
        /// <param name="measurement">Object that implements the <see cref="IMeasurement"/> interface.</param>
        [CLSCompliant(false)]
        public PacketType1(IMeasurement measurement)
            : this()
        {
            HistorianID = (int)measurement.Key.ID;
            Time = new TimeTag((DateTime)measurement.Timestamp);
            Value = (float)measurement.AdjustedValue;
            Quality = measurement.HistorianQuality();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType1"/> class.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="PacketType1"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public PacketType1(byte[] buffer, int startIndex, int length)
            : this()
        {
            ParseBinaryImage(buffer, startIndex, length);
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
                    throw new ArgumentException("Value must be positive");

                m_historianID = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of the time-series data.
        /// </summary>
        /// /// <exception cref="ArgumentException">The value being assigned is not between 01/01/1995 and 01/19/2063.</exception>
        public TimeTag Time
        {
            get
            {
                return m_time;
            }
            set
            {
                if (value.CompareTo(TimeTag.MinValue) < 0 || value.CompareTo(TimeTag.MaxValue) > 0)
                    throw new ArgumentException("Value must between 01/01/1995 and 01/19/2063");

                m_time = value;
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
        /// Gets the length of the <see cref="PacketType1"/>.
        /// </summary>
        public override int BinaryLength
        {
            get
            {
                return FixedLength;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="PacketType1"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="PacketType1"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="PacketType1"/>.</returns>
        public override int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= FixedLength)
            {
                // Binary image has sufficient data.
                short packetID = EndianOrder.LittleEndian.ToInt16(buffer, startIndex);
                if (packetID != TypeID)
                    throw new ArgumentException(string.Format("Unexpected packet id '{0}' (expected '{1}')", packetID, TypeID));

                // We have a binary image with the correct packet id.
                HistorianID = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 2);
                Time = new TimeTag(EndianOrder.LittleEndian.ToDouble(buffer, startIndex + 6));
                Quality = (Quality)(EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 14));
                Value = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 18);

                // We'll send an "ACK" to the sender if this is the last packet in the transmission.
                if (length == FixedLength)
                    PreProcessHandler = PreProcess;

                return FixedLength;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="PacketType1"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        public override int GenerateBinaryImage(byte[] buffer, int startIndex)
        {
            int length = BinaryLength;

            buffer.ValidateParameters(startIndex, length);

            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(TypeID), 0, buffer, startIndex, 2);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_historianID), 0, buffer, startIndex + 2, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_time.Value), 0, buffer, startIndex + 6, 8);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes((int)m_quality), 0, buffer, startIndex + 14, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_value), 0, buffer, startIndex + 18, 4);

            return length;
        }

        /// <summary>
        /// Extracts time-series data from <see cref="PacketType1"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> object of <see cref="ArchiveDataPoint"/>s.</returns>
        public override IEnumerable<IDataPoint> ExtractTimeSeriesData()
        {
            return new ArchiveDataPoint[] { new ArchiveDataPoint(m_historianID, m_time, m_value, m_quality) };
        }

        /// <summary>
        /// Processes <see cref="PacketType1"/>.
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
        /// Pre-processes <see cref="PacketType1"/>.
        /// </summary>
        /// <returns>A <see cref="byte"/> array for "ACK".</returns>
        protected virtual IEnumerable<byte[]> PreProcess()
        {
            return new byte[][] { Encoding.ASCII.GetBytes("ACK") };
        }

        #endregion
    }
}