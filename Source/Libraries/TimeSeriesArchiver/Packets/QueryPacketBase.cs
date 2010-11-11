//******************************************************************************************************
//  QueryPacketBase.cs - Gbtc
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
//  07/16/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using TVA;

namespace TimeSeriesArchiver.Packets
{
    /// <summary>
    /// Base class for a packet to be used for requesting information from a historian.
    /// </summary>
    public abstract class QueryPacketBase : PacketBase
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 2          0-1        Int16      TypeID (packet identifier)                                    *
        // * 4          2-5        Int32      RequestIDs.Count                                              *
        // * 4          6-9        Int32      RequestIDs[0]                                                 *
        // * 4          n1-n2      Int32      RequestIDs[RequestIDs.Count -1 ]                              *
        // **************************************************************************************************

        #region [ Members ]

        // Fields
        private List<int> m_requestIDs;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the query packet.
        /// </summary>
        /// <param name="packetID">Numeric identifier for the packet type.</param>
        protected QueryPacketBase(short packetID)
            : base(packetID)
        {
            m_requestIDs = new List<int>();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a list of historian identifiers whose information is being requested.
        /// </summary>
        /// <remarks>A singe entry with ID of -1 can be used to request information for all defined historian identifiers.</remarks>
        public IList<int> RequestIDs
        {
            get
            {
                return m_requestIDs;
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="BinaryImage"/>.
        /// </summary>
        public override int BinaryLength
        {
            get
            {
                return (2 + 4 + (m_requestIDs.Count * 4));
            }
        }

        /// <summary>
        /// Gets the binary representation of the query packet.
        /// </summary>
        public override byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[BinaryLength];

                Array.Copy(EndianOrder.LittleEndian.GetBytes(TypeID), 0, image, 0, 2);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_requestIDs.Count), 0, image, 2, 4);
                for (int i = 0; i < m_requestIDs.Count; i++)
                {
                    Array.Copy(EndianOrder.LittleEndian.GetBytes(m_requestIDs[i]), 0, image, 6 + (i * 4), 4);
                }

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes the query packet from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing the query packet.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing the query packet.</returns>
        public override int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= 6)
            {
                // Binary image has sufficient data.
                short packetID = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex);
                if (packetID != TypeID)
                    throw new ArgumentException(string.Format("Unexpected packet id '{0}' (expected '{1}')", packetID, TypeID));

                // Ensure that the binary image is complete
                int requestIDCount = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2);
                if (length < 6 + requestIDCount * 4)
                    return 0;

                // We have a binary image with the correct packet id.
                m_requestIDs.Clear();
                for (int i = 0; i < requestIDCount; i++)
                {
                    m_requestIDs.Add(EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 6 + (i * 4)));
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
        /// Extracts time-series data from the query packet type.
        /// </summary>
        /// <returns>A null reference.</returns>
        public override IEnumerable<IDataPoint> ExtractTimeSeriesData()
        {
            return null;
        }

        #endregion
    }
}