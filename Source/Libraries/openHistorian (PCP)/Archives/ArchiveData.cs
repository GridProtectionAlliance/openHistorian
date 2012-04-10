//******************************************************************************************************
//  ArchiveData.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
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
//
//******************************************************************************************************

using System;
using TVA;

namespace openHistorian.Archives
{
    /// <summary>
    /// Represents time-series data stored in an <see cref="IDataArchive"/>.
    /// </summary>
    public class ArchiveData : Data
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
        /// Specifies the number of bytes in the binary image of <see cref="ArchiveData"/>.
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
        private int m_flags;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveData"/> class.
        /// </summary>
        /// <param name="key">Historian identifier of <see cref="ArchiveData"/>.</param>
        public ArchiveData(int key)
            : base(key)
        {
            this.Time = TimeTag.MinValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveData"/> class.
        /// </summary>
        /// <param name="dataPoint">A time-series data point.</param>
        public ArchiveData(IData dataPoint)
            : this(dataPoint.Key, dataPoint.Time, dataPoint.Value, dataPoint.Quality)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveData"/> class.
        /// </summary>
        /// <param name="key">Historian identifier of <see cref="ArchiveData"/>.</param>
        /// <param name="time"><see cref="TimeTag"/> of <see cref="ArchiveData"/>.</param>
        /// <param name="value">Floating-point value of <see cref="ArchiveData"/>.</param>
        /// <param name="quality"><see cref="Quality"/> of <see cref="ArchiveData"/>.</param>
        public ArchiveData(int key, TimeTag time, float value, Quality quality)
            : this(key)
        {
            this.Time = time;
            this.Value = value;
            this.Quality = quality;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveData"/> class.
        /// </summary>
        /// <param name="key">Historian identifier of <see cref="ArchiveData"/>.</param>
        /// <param name="buffer">Binary image to be used for initializing <see cref="ArchiveData"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public ArchiveData(int key, byte[] buffer, int startIndex, int length)
            : this(key)
        {
            ParseBinaryImage(buffer, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of <see cref="ArchiveData"/>.
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
                if (value < TimeTag.MinValue || value > TimeTag.MaxValue)
                    throw new ArgumentException("Value must between 01/01/1995 and 01/19/2063");

                base.Time = value;
                Flags = Flags.SetMaskedValue(MillisecondMask, base.Time.ToDateTime().Millisecond << 5);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Quality"/> of <see cref="ArchiveData"/>.
        /// </summary>
        public override Quality Quality
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
        /// Gets or sets the 32-bit word used for storing data of <see cref="ArchiveData"/>.
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

        /// <summary>
        /// Gets the length of the <see cref="ArchiveData"/>.
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
        /// Initializes <see cref="ArchiveData"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="ArchiveData"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="ArchiveData"/>.</returns>
        public override int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                Flags = EndianOrder.LittleEndian.ToInt16(buffer, startIndex + 4);
                Value = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 6);
                Time = new TimeTag(EndianOrder.LittleEndian.ToInt32(buffer, startIndex) +               // Seconds
                                    ((double)(m_flags.GetMaskedValue(MillisecondMask) >> 5) / 1000));   // Milliseconds

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="ArchiveData"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
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

            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes((int)Time.Value), 0, buffer, startIndex, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes((short)m_flags), 0, buffer, startIndex + 4, 2);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(Value), 0, buffer, startIndex + 6, 4);

            return length;
        }

        #endregion
    }
}
