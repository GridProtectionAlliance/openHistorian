//******************************************************************************************************
//  PacketType101Data.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  06/04/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/21/2009 - Pinal C. Patel
//       Added overloaded constructor that takes in IMeasurement.
//
//******************************************************************************************************

using System;
using openHistorian.Archives;
using TVA;

namespace openHistorian.Adapters.Native.Packets
{
    /// <summary>
    /// Represents time-series data transmitted in <see cref="PacketType101"/>.
    /// </summary>
    /// <seealso cref="PacketType101"/>
    internal class PacketType101Data : ArchiveData
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 4          0-3        Int32      Key                                                   *
        // * 4          4-7        Int32      Time                                                          *
        // * 2          8-9        Int16      Flags (Quality & Milliseconds)                                *
        // * 4          10-13      Single     Value                                                         *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="PacketType101Data"/>.
        /// </summary>
        public new const int ByteCount = 14;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101Data"/> class.
        /// </summary>
        /// <param name="dataPoint">A time-series data point.</param>
        public PacketType101Data(IData dataPoint)
            : base(dataPoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101Data"/> class.
        /// </summary>
        /// <param name="key">Historian identifier of <see cref="PacketType101Data"/>.</param>
        /// <param name="time"><see cref="TimeTag"/> of <see cref="PacketType101Data"/>.</param>
        /// <param name="value">Floating-point value of <see cref="PacketType101Data"/>.</param>
        /// <param name="quality"><see cref="Quality"/> of <see cref="PacketType101Data"/>.</param>
        public PacketType101Data(int key, TimeTag time, float value, Quality quality)
            : base(key, time, value, quality)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101Data"/> class.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="PacketType101Data"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        public PacketType101Data(byte[] binaryImage, int startIndex, int length)
            : base(1, binaryImage, startIndex, length)
        {
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the length of the <see cref="PacketType101Data"/>.
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
        /// Initializes <see cref="PacketType101Data"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="PacketType101Data"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="PacketType101Data"/>.</returns>
        public override int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                Key = EndianOrder.LittleEndian.ToInt32(buffer, startIndex);
                Flags = EndianOrder.LittleEndian.ToInt16(buffer, startIndex + 8);
                Value = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 10);
                Time = new TimeTag(EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 4) +       // Seconds
                                   ((double)(Flags.GetMaskedValue(MillisecondMask) >> 5) / 1000));  // Milliseconds

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="PacketType101Data"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
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

            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes((int)Key), 0, buffer, startIndex, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes((int)Time.Value), 0, buffer, startIndex + 4, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes((short)Flags), 0, buffer, startIndex + 8, 2);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(Value), 0, buffer, startIndex + 10, 4);

            return length;
        }

        #endregion
    }
}
