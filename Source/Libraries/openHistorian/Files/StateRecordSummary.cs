//******************************************************************************************************
//  StateRecordSummary.cs - Gbtc
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
//  06/14/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using TVA;
using TVA.Parsing;

namespace openHistorian.Files
{
    /// <summary>
    /// A class with just <see cref="StateRecord.CurrentData"/>. The <see cref="BinaryImage"/> of <see cref="MetadataRecordSummary"/> 
    /// is sent back as a reply to <see cref="Historian.Packets.PacketType11"/>.
    /// </summary>
    /// <seealso cref="StateRecord"/>
    /// <seealso cref="Historian.Packets.PacketType11"/>
    public class StateRecordSummary : ISupportBinaryImage
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 4          0-3        Int32      HistorianID                                                   *
        // * 16         4-19       Byte(16)   CurrentData                                                   *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="StateRecordSummary"/>.
        /// </summary>
        public const int ByteCount = 20;

        // Fields
        private int m_historianID;
        private StateRecordDataPoint m_currentData;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordSummary"/> class.
        /// </summary>
        /// <param name="record">A <see cref="StateRecord"/> object.</param>
        public StateRecordSummary(StateRecord record)
        {
            HistorianID = record.HistorianID;
            CurrentData = record.CurrentData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecordSummary"/> class.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="StateRecordSummary"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        public StateRecordSummary(byte[] binaryImage, int startIndex, int length)
        {
            Initialize(binaryImage, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Same as <see cref="StateRecord.HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive or -1.</exception>
        public int HistorianID
        {
            get
            {
                return m_historianID;
            }
            private set
            {
                if (value < 1 && value != -1)
                    throw new ArgumentException("Value must be positive or -1");

                m_historianID = value;
            }
        }

        /// <summary>
        /// Same as <see cref="StateRecord.CurrentData"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public StateRecordDataPoint CurrentData
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
        /// Gets the length of the <see cref="BinaryImage"/>.
        /// </summary>
        public int BinaryLength
        {
            get
            {
                return ByteCount;
            }
        }

        /// <summary>
        /// Gets the binary representation of <see cref="StateRecordSummary"/>.
        /// </summary>
        public byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_historianID), 0, image, 0, 4);
                Array.Copy(m_currentData.BinaryImage, 0, image, 4, StateRecordDataPoint.ByteCount);

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="StateRecordSummary"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="StateRecordSummary"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="StateRecordSummary"/>.</returns>
        public int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                HistorianID = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex);
                CurrentData = new StateRecordDataPoint(HistorianID, binaryImage, startIndex + 4, length - 4);

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
