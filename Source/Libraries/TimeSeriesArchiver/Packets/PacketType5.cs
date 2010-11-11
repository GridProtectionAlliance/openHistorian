//******************************************************************************************************
//  PacketType5.cs - Gbtc
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
//  08/02/2007 - Pinal C. Patel
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
    /// Represents a packet that can be used to query the status of a historian.
    /// </summary>
    public class PacketType5 : PacketBase
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 2          0-1        Int16      TypeID (packet identifier)                                    *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="PacketType5"/>.
        /// </summary>
        public new const int ByteCount = 2;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType5"/> class.
        /// </summary>
        public PacketType5()
            : base(5)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType5"/> class.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="PacketType5"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        public PacketType5(byte[] binaryImage, int startIndex, int length)
            : this()
        {
            Initialize(binaryImage, startIndex, length);
        }
        #endregion

        #region [ Properties ]

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
        /// Gets the binary representation of <see cref="PacketType5"/>.
        /// </summary>
        public override byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(EndianOrder.LittleEndian.GetBytes(TypeID), 0, image, 0, 2);

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="PacketType5"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="PacketType5"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="PacketType5"/>.</returns>
        public override int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                short packetID = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex);
                if (packetID != TypeID)
                    throw new ArgumentException(string.Format("Unexpected packet id '{0}' (expected '{1}')", packetID, TypeID));

                // We have a binary image with the correct packet id and that all this packet type will contain.
                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Extracts time-series data from <see cref="PacketType5"/>.
        /// </summary>
        /// <returns>A null reference.</returns>
        public override IEnumerable<IDataPoint> ExtractTimeSeriesData()
        {
            return null;
        }

        #endregion
    }
}