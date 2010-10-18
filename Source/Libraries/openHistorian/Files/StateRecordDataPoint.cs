//******************************************************************************************************
//  StateRecordDataPoint.cs - Gbtc
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
//  05/04/2009 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using TVA;

namespace openHistorian.Files
{
    /// <summary>
    /// Represents time-series data stored in <see cref="StateFile"/>.
    /// </summary>
    /// <seealso cref="StateFile"/>
    /// <seealso cref="StateRecord"/>
    /// <seealso cref="StateRecordSummary"/>
    public class StateRecordDataPoint : ArchiveDataPoint
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 8          0-7        Double     Time                                                          *
        // * 4          8-11       Int32      Flags (Quality, TimeZoneIndex & DaylightSavingsTime)          *
        // * 4          12-15      Single     Value                                                         *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="StateRecordDataPoint"/>.
        /// </summary>
        public new const int ByteCount = 16;

        /// <summary>
        /// Specifies the bit-mask for <see cref="TimeZoneIndex"/> stored in <see cref="ArchiveDataPoint.Flags"/>.
        /// </summary>
        [CLSCompliant(false)]
        protected const Bits TziMask = Bits.Bit05 | Bits.Bit06 | Bits.Bit07 | Bits.Bit08 | Bits.Bit09 | Bits.Bit10;

        /// <summary>
        /// Specifies the bit-mask for <see cref="DaylightSavingsTime"/> stored in <see cref="ArchiveDataPoint.Flags"/>.
        /// </summary>
        [CLSCompliant(false)]
        protected const Bits DstMask = Bits.Bit11;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordDataPoint"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="StateRecordDataPoint"/>.</param>
        public StateRecordDataPoint(int historianID)
            : base(historianID)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordDataPoint"/> class.
        /// </summary>
        /// <param name="dataPoint">A time-series data point.</param>
        public StateRecordDataPoint(IDataPoint dataPoint)
            : base(dataPoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordDataPoint"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="StateRecordDataPoint"/>.</param>
        /// <param name="time"><see cref="TimeTag"/> of <see cref="StateRecordDataPoint"/>.</param>
        /// <param name="value">Floating-point value of <see cref="StateRecordDataPoint"/>.</param>
        /// <param name="quality"><see cref="Quality"/> of <see cref="StateRecordDataPoint"/>.</param>
        public StateRecordDataPoint(int historianID, TimeTag time, float value, Quality quality)
            : base(historianID, time, value, quality)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordDataPoint"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="StateRecordDataPoint"/>.</param>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="StateRecordDataPoint"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        public StateRecordDataPoint(int historianID, byte[] binaryImage, int startIndex, int length)
            : this(historianID)
        {
            Initialize(binaryImage, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the 0-based index of the time-zone for the <see cref="Time"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive or zero.</exception>
        public short TimeZoneIndex
        {
            get
            {
                return (short)(Flags.GetMaskedValue(TziMask) >> 5);
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be positive or zero");

                Flags = Flags.SetMaskedValue(TziMask, value << 5);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether daylight savings time is in effect.
        /// </summary>
        public bool DaylightSavingsTime
        {
            get
            {
                return Flags.CheckBits(DstMask);
            }
            set
            {
                Flags = value ? Flags.SetBits(DstMask) : Flags.ClearBits(DstMask);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of <see cref="StateRecordDataPoint"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 01/01/1995 and 01/19/2063.</exception>
        public override TimeTag Time
        {
            get
            {
                return base.Time;
            }
            set
            {
                int flags = base.Flags; // Save existing flags.
                base.Time = value;      // Update base time (modifies flags).
                base.Flags = flags;     // Restore saved flags.
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
        /// Gets the binary representation of <see cref="StateRecordDataPoint"/>.
        /// </summary>
        public override byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(EndianOrder.LittleEndian.GetBytes(Time.Value), 0, image, 0, 8);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(Flags), 0, image, 8, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(Value), 0, image, 12, 4);

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="StateRecordDataPoint"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="StateRecordDataPoint"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="StateRecordDataPoint"/>.</returns>
        public override int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                Time = new TimeTag(EndianOrder.LittleEndian.ToDouble(binaryImage, startIndex));
                Flags = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 8);
                Value = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 12);

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
