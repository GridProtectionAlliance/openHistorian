//******************************************************************************************************
//  PacketType101.cs - Gbtc
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
//  09/23/2009 - Pinal C. Patel
//       Edited code comments.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using openHistorian.Archives;
using TVA;

namespace openHistorian.Adapters.Native.Packets
{
    /// <summary>
    /// Represents a packet that can be used to send multiple time-series data points to a historian for archival.
    /// </summary>
    /// <seealso cref="PacketType101Data"/>
    public class PacketType101 : PacketBase
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 2          0-1        Int16      TypeID (packet identifier)                                    *
        // * 4          2-5        Int32      Data.Count                                                    *
        // * 14         6-19       Byte[]     Data[0]                                                       *
        // * 14         n1-n2      Byte[]     Data[Data.Count - 1]                                          *
        // **************************************************************************************************

        #region [ Members ]

        // Fields
        private List<IData> m_data;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101"/> class.
        /// </summary>
        public PacketType101()
            : base(101)
        {
            m_data = new List<IData>();
            ProcessHandler = Process;
            PreProcessHandler = PreProcess;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101"/> class.
        /// </summary>
        /// <param name="dataPoints">A collection of time-series data points.</param>
        public PacketType101(IEnumerable<IData> dataPoints)
            : this()
        {
            if (dataPoints == null)
                throw new ArgumentNullException("value");

            foreach (IData dataPoint in dataPoints)
            {
                m_data.Add(new PacketType101Data(dataPoint));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType101"/> class.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="PacketType101"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public PacketType101(byte[] buffer, int startIndex, int length)
            : this()
        {
            ParseBinaryImage(buffer, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the time-series data in <see cref="PacketType101"/>.
        /// </summary>
        public IList<IData> Data
        {
            get
            {
                return m_data;
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="PacketType101"/>.
        /// </summary>
        public override int BinaryLength
        {
            get
            {
                return (2 + 4 + (m_data.Count * PacketType101Data.ByteCount));
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="PacketType101"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="PacketType101"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="PacketType101"/>.</returns>
        public override int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= 6)
            {
                // Binary image has sufficient data.
                short packetID = EndianOrder.LittleEndian.ToInt16(buffer, startIndex);
                if (packetID != TypeID)
                    throw new ArgumentException(string.Format("Unexpected packet id '{0}' (expected '{1}')", packetID, TypeID));

                // Ensure that the binary image is complete
                int dataCount = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 2);
                if (length < 6 + dataCount * PacketType101Data.ByteCount)
                    return 0;

                // We have a binary image with the correct packet id.
                int offset;
                m_data.Clear();
                for (int i = 0; i < dataCount; i++)
                {
                    offset = startIndex + 6 + (i * PacketType101Data.ByteCount);
                    m_data.Add(new PacketType101Data(buffer, offset, length - offset));
                }

                return BinaryLength;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="PacketType101"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
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
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_data.Count), 0, buffer, startIndex + 2, 4);
            for (int i = 0; i < m_data.Count; i++)
            {
                m_data[i].GenerateBinaryImage(buffer, startIndex + 6 + (i * PacketType101Data.ByteCount));
            }

            return length;
        }

        /// <summary>
        /// Extracts time-series data from <see cref="PacketType101"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> object of <see cref="ArchiveData"/>s.</returns>
        public override IEnumerable<IData> ExtractTimeSeriesData()
        {
            List<IData> data = new List<IData>();
            foreach (IData dataPoint in m_data)
            {
                data.Add(new ArchiveData(dataPoint));
            }
            return data;
        }

        /// <summary>
        /// Processes <see cref="PacketType101"/>.
        /// </summary>
        /// <returns>A null reference.</returns>
        protected virtual IEnumerable<byte[]> Process()
        {
            if (Archive != null)
            {
                foreach (IData dataPoint in ExtractTimeSeriesData())
                {
                    Archive.WriteData(dataPoint);
                }
            }

            return null;
        }

        /// <summary>
        /// Pre-processes <see cref="PacketType101"/>.
        /// </summary>
        /// <returns>A <see cref="byte"/> array for "ACK".</returns>
        protected virtual IEnumerable<byte[]> PreProcess()
        {
            return new byte[][] { Encoding.ASCII.GetBytes("ACK") };
        }

        #endregion
    }
}
