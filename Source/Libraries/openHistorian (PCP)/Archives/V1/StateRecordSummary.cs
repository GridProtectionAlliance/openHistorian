//******************************************************************************************************
//  StateRecordSummary.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  06/14/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System;
using openHistorian.Adapters.Native.Packets;
using TVA;
using TVA.Parsing;

namespace openHistorian.Archives.V1
{
    /// <summary>
    /// A class with just <see cref="StateRecord.CurrentData"/>. The generated binary image of <see cref="MetadataRecordSummary"/> 
    /// is sent back as a reply to <see cref="PacketType11"/>.
    /// </summary>
    /// <seealso cref="StateRecord"/>
    /// <seealso cref="PacketType11"/>
    public class StateRecordSummary : ISupportBinaryImage
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 4          0-3        Int32      Key                                                   *
        // * 16         4-19       Byte(16)   CurrentData                                                   *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="StateRecordSummary"/>.
        /// </summary>
        public const int ByteCount = 20;

        // Fields
        private int m_key;
        private IData m_currentData;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordSummary"/> class.
        /// </summary>
        /// <param name="record">A <see cref="StateRecord"/> object.</param>
        public StateRecordSummary(StateRecord record)
        {
            Key = record.Key;
            CurrentData = record.CurrentData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordSummary"/> class.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="StateRecordSummary"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public StateRecordSummary(byte[] buffer, int startIndex, int length)
        {
            ParseBinaryImage(buffer, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Same as <see cref="StateRecord.Key"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive or -1.</exception>
        public int Key
        {
            get
            {
                return m_key;
            }
            private set
            {
                if (value < 1 && value != -1)
                    throw new ArgumentException("Value must be positive or -1");

                m_key = value;
            }
        }

        /// <summary>
        /// Same as <see cref="StateRecord.CurrentData"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public IData CurrentData
        {
            get
            {
                return m_currentData;
            }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_currentData = value;
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="StateRecordSummary"/>.
        /// </summary>
        public int BinaryLength
        {
            get
            {
                return ByteCount;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="StateRecordSummary"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="StateRecordSummary"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="StateRecordSummary"/>.</returns>
        public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                Key = EndianOrder.LittleEndian.ToInt32(buffer, startIndex);
                CurrentData = new StateRecordData(Key, buffer, startIndex + 4, length - 4);

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="StateRecordSummary"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        public int GenerateBinaryImage(byte[] buffer, int startIndex)
        {
            int length = BinaryLength;

            buffer.ValidateParameters(startIndex, length);

            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_key), 0, buffer, startIndex, 4);
            m_currentData.GenerateBinaryImage(buffer, startIndex + 4);

            return length;
        }

        #endregion
    }
}
