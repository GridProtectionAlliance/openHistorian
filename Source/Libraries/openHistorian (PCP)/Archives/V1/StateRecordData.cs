//******************************************************************************************************
//  StateRecordData.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  05/04/2009 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System;
using TVA;

namespace openHistorian.Archives.V1
{
    /// <summary>
    /// Represents time-series data stored in <see cref="StateFile"/>.
    /// </summary>
    /// <seealso cref="StateFile"/>
    /// <seealso cref="StateRecord"/>
    /// <seealso cref="StateRecordSummary"/>
    internal class StateRecordData : ArchiveData
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
        /// Specifies the number of bytes in the binary image of <see cref="StateRecordData"/>.
        /// </summary>
        public new const int ByteCount = 16;

        /// <summary>
        /// Specifies the bit-mask for <see cref="TimeZoneIndex"/> stored in <see cref="ArchiveData.Flags"/>.
        /// </summary>
        protected const Bits TziMask = Bits.Bit05 | Bits.Bit06 | Bits.Bit07 | Bits.Bit08 | Bits.Bit09 | Bits.Bit10;

        /// <summary>
        /// Specifies the bit-mask for <see cref="DaylightSavingsTime"/> stored in <see cref="ArchiveData.Flags"/>.
        /// </summary>
        protected const Bits DstMask = Bits.Bit11;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordData"/> class.
        /// </summary>
        /// <param name="key">Historian identifier of <see cref="StateRecordData"/>.</param>
        public StateRecordData(int key)
            : base(key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordData"/> class.
        /// </summary>
        /// <param name="dataPoint">A time-series data point.</param>
        public StateRecordData(IData dataPoint)
            : base(dataPoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordData"/> class.
        /// </summary>
        /// <param name="key">Historian identifier of <see cref="StateRecordData"/>.</param>
        /// <param name="time"><see cref="TimeTag"/> of <see cref="StateRecordData"/>.</param>
        /// <param name="value">Floating-point value of <see cref="StateRecordData"/>.</param>
        /// <param name="quality"><see cref="Quality"/> of <see cref="StateRecordData"/>.</param>
        public StateRecordData(int key, TimeTag time, float value, Quality quality)
            : base(key, time, value, quality)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordData"/> class.
        /// </summary>
        /// <param name="key">Historian identifier of <see cref="StateRecordData"/>.</param>
        /// <param name="buffer">Binary image to be used for initializing <see cref="StateRecordData"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public StateRecordData(int key, byte[] buffer, int startIndex, int length)
            : this(key)
        {
            ParseBinaryImage(buffer, startIndex, length);
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
        /// Gets or sets the <see cref="TimeTag"/> of <see cref="StateRecordData"/>.
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
        /// Gets the length of the <see cref="StateRecordData"/>.
        /// </summary>
        public override int BinaryLength
        {
            get
            {
                return ByteCount;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="StateRecordData"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="StateRecordData"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="StateRecordData"/>.</returns>
        public override int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                Time = new TimeTag(EndianOrder.LittleEndian.ToDouble(buffer, startIndex));
                Flags = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 8);
                Value = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 12);

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="StateRecordData"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
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

            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(Time.Value), 0, buffer, startIndex, 8);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(Flags), 0, buffer, startIndex + 8, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(Value), 0, buffer, startIndex + 12, 4);

            return length;
        }

        #endregion
    }
}
