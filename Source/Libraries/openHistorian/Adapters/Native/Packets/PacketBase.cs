//******************************************************************************************************
//  PacketBase.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  07/27/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Archives;
using TVA.Parsing;

namespace openHistorian.Adapters.Native.Packets
{
    /// <summary>
    /// Base class for a binary packet received by a historian.
    /// </summary>
    /// <seealso cref="IPacket"/>
    public abstract class PacketBase : IPacket
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of the packet.
        /// </summary>
        /// <remarks>A value of -1 indicates that the binary image of the packet is of variable length.</remarks>
        public const int ByteCount = -1;

        // Fields
        private IDataArchive m_archive;
        private Func<IEnumerable<byte[]>> m_processHandler;
        private Func<IEnumerable<byte[]>> m_preProcessHandler;
        private ICommonHeader<short> m_commonHeader;
        private short m_typeID;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the packet.
        /// </summary>
        /// <param name="packetID">Numeric identifier for the packet type.</param>
        protected PacketBase(short packetID)
        {
            m_typeID = packetID;
        }

        #endregion

        #region [ Properties ]

        #region [ Abstract ]

        /// <summary>
        /// Gets the length of the packet's binary representation.
        /// </summary>
        public abstract int BinaryLength { get; }

        #endregion

        /// <summary>
        /// Gets or sets the current <see cref="IDataArchive"/>.
        /// </summary>
        public IDataArchive Archive
        {
            get
            {
                return m_archive;
            }
            set
            {
                m_archive = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Delegate"/> that processes the packet.
        /// </summary>
        /// <remarks>
        /// <see cref="Func{TResult}"/> returns an <see cref="IEnumerable{T}"/> object containing the binary data to be sent back to the packet sender.
        /// </remarks>
        public Func<IEnumerable<byte[]>> ProcessHandler
        {
            get
            {
                return m_processHandler;
            }
            set
            {
                m_processHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Delegate"/> that pre-processes the packet.
        /// </summary>
        /// <remarks>
        /// <see cref="Func{TResult}"/> returns an <see cref="IEnumerable{T}"/> object containing the binary data to be sent back to the packet sender.
        /// </remarks>
        public Func<IEnumerable<byte[]>> PreProcessHandler
        {
            get
            {
                return m_preProcessHandler;
            }
            set
            {
                m_preProcessHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PacketCommonHeader"/> for the packet.
        /// </summary>
        public ICommonHeader<short> CommonHeader
        {
            get
            {
                return m_commonHeader;
            }
            set
            {
                m_commonHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets the numeric identifier for the packet type.
        /// </summary>
        public short TypeID
        {
            get
            {
                return m_typeID;
            }
        }

        #endregion

        #region [ Methods ]

        #region [ Abstract ]

        /// <summary>
        /// When overridden in a derived class, , initializes packet by parsing the specified <paramref name="buffer"/> containing a binary image.
        /// </summary>
        /// <param name="buffer">Buffer containing binary image to parse.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start parsing.</param>
        /// <param name="length">Valid number of bytes within <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>The number of bytes used for initialization in the <paramref name="buffer"/> (i.e., the number of bytes parsed).</returns>
        /// <remarks>
        /// Implementors should validate <paramref name="startIndex"/> and <paramref name="length"/> against <paramref name="buffer"/> length.
        /// The <see cref="TVA.BufferExtensions.ValidateParameters"/> method can be used to perform this validation.
        /// </remarks>
        public abstract int ParseBinaryImage(byte[] buffer, int startIndex, int length);

        /// <summary>
        /// When overridden in a derived class,  generates binary image of the packet and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <remarks>
        /// Implementors should validate <paramref name="startIndex"/> and <see cref="BinaryLength"/> against <paramref name="buffer"/> length.
        /// The <see cref="TVA.BufferExtensions.ValidateParameters"/> method can be used to perform this validation.
        /// </remarks>
        public abstract int GenerateBinaryImage(byte[] buffer, int startIndex);

        #endregion

        /// <summary>
        /// Extracts time-series data from the packet.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IData"/>s if the packet contains time-series data; otherwise null.</returns>
        public abstract IEnumerable<IData> ExtractTimeSeriesData();

        #endregion
    }
}