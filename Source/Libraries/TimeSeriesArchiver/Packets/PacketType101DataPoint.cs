//******************************************************************************************************
//  PacketType101DataPoint.cs - Gbtc
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
//  06/04/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/21/2009 - Pinal C. Patel
//       Added overloaded constructor that takes in IMeasurement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using TimeSeriesArchiver.Files;
using TimeSeriesFramework;
using TVA;

namespace TimeSeriesArchiver.Packets
{
    /// <summary>
    /// Represents time-series data transmitted in <see cref="PacketType101"/>.
    /// </summary>
    /// <seealso cref="PacketType101"/>
    public class PacketType101DataPoint : ArchiveDataPoint
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 4          0-3        Int32      HistorianID                                                   *
        // * 4          4-7        Int32      Time                                                          *
        // * 2          8-9        Int16      Flags (Quality & Milliseconds)                                *
        // * 4          10-13      Single     Value                                                         *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="PacketType101DataPoint"/>.
        /// </summary>
        public new const int ByteCount = 14;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101DataPoint"/> class.
        /// </summary>
        /// <param name="dataPoint">A time-series data point.</param>
        public PacketType101DataPoint(IDataPoint dataPoint)
            : base(dataPoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101DataPoint"/> class.
        /// </summary>
        /// <param name="measurement">Object that implements the <see cref="IMeasurement"/> interface.</param>
        [CLSCompliant(false)]
        public PacketType101DataPoint(IMeasurement measurement)
            : base(measurement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101DataPoint"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="PacketType101DataPoint"/>.</param>
        /// <param name="time"><see cref="TimeTag"/> of <see cref="PacketType101DataPoint"/>.</param>
        /// <param name="value">Floating-point value of <see cref="PacketType101DataPoint"/>.</param>
        /// <param name="quality"><see cref="Quality"/> of <see cref="PacketType101DataPoint"/>.</param>
        public PacketType101DataPoint(int historianID, TimeTag time, float value, Quality quality)
            : base(historianID, time, value, quality)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101DataPoint"/> class.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="PacketType101DataPoint"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        public PacketType101DataPoint(byte[] binaryImage, int startIndex, int length)
            : base(1, binaryImage, startIndex, length)
        {
        }

        #endregion

        #region [ Methods ]

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
        /// Gets the binary representation of <see cref="PacketType101DataPoint"/>.
        /// </summary>
        public override byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(EndianOrder.LittleEndian.GetBytes(HistorianID), 0, image, 0, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes((int)Time.Value), 0, image, 4, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes((short)Flags), 0, image, 8, 2);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(Value), 0, image, 10, 4);

                return image;
            }
        }

        /// <summary>
        /// Initializes <see cref="PacketType101DataPoint"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="PacketType101DataPoint"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="PacketType101DataPoint"/>.</returns>
        public override int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                HistorianID = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex);
                Flags = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex + 8);
                Value = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 10);
                Time = new TimeTag(EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 4) +  // Seconds
                                   ((double)(Flags.GetMaskedValue(MillisecondMask) >> 5) / 1000));  // Milliseconds

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        #endregion
    }
}
