//******************************************************************************************************
//  PacketBase.cs - Gbtc
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
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  11/30/2011 - J. Ritchie Carroll
//       Modified to support buffer optimized ISupportBinaryImage.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V1.Files;
using TVA.Parsing;

namespace openHistorian.V1.Adapters
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
        public const int FixedLength = -1;

        // Fields
        private ArchiveFile m_archive;
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

        /// <summary>
        /// Gets the length of the packet's binary representation.
        /// </summary>
        public abstract int BinaryLength
        {
            get;
        }

        /// <summary>
        /// Gets or sets the current <see cref="IArchive"/>.
        /// </summary>
        public ArchiveFile Archive
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

        /// <summary>
        /// Initializes packet by parsing the specified <paramref name="buffer"/> containing a binary image.
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
        /// Generates binary image of the packet and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <remarks>
        /// Implementors should validate <paramref name="startIndex"/> and <see cref="BinaryLength"/> against <paramref name="buffer"/> length.
        /// The <see cref="TVA.BufferExtensions.ValidateParameters"/> method can be used to perform this validation.
        /// </remarks>
        public abstract int GenerateBinaryImage(byte[] buffer, int startIndex);

        /// <summary>
        /// Extracts time-series data from the packet.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IDataPoint"/>s if the packet contains time-series data; otherwise null.</returns>
        public abstract IEnumerable<IDataPoint> ExtractTimeSeriesData();

        #endregion
    }
}