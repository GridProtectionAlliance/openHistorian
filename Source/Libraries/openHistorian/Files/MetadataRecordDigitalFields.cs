//******************************************************************************************************
//  MetadataRecordDigitalFields.cs - Gbtc
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
//  02/21/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  01/23/2008 - Pinal C. Patel
//       Added AlarmDelay property to expose delay in sending alarm notification.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Text;
using TVA;
using TVA.Parsing;

namespace openHistorian.Files
{
    /// <summary>
    /// Defines specific fields for <see cref="MetadataRecord"/>s that are of type <see cref="DataType.Digital"/>.
    /// </summary>
    /// <seealso cref="MetadataRecord"/>
    public class MetadataRecordDigitalFields : ISupportBinaryImage
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 24         0-23       Char(24)   SetDescription                                                *
        // * 24         24-47      Char(24)   ClearDescription                                              *
        // * 4          48-51      Int32      AlarmState                                                    *
        // * 4          52-55      Single     AlarmDelay                                                    *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="MetadataRecordDigitalFields"/>.
        /// </summary>
        public const int ByteCount = 56;

        // Fields
        private string m_setDescription;
        private string m_clearDescription;
        private int m_alarmState;
        private float m_alarmDelay;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataRecordDigitalFields"/> class.
        /// </summary>
        internal MetadataRecordDigitalFields()
        {
            m_setDescription = string.Empty;
            m_clearDescription = string.Empty;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the text description of the data value for the <see cref="MetadataRecord"/> when it is 1.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="SetDescription"/> is 24 characters.
        /// </remarks>
        public string SetDescription
        {
            get
            {
                return m_setDescription;
            }
            set
            {
                m_setDescription = value.TruncateRight(24);
            }
        }

        /// <summary>
        /// Gets or sets the text description of the data value for the <see cref="MetadataRecord"/> when it is 0.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="ClearDescription"/> is 24 characters.
        /// </remarks>
        public string ClearDescription
        {
            get
            {
                return m_clearDescription;
            }
            set
            {
                m_clearDescription = value.TruncateRight(24);
            }
        }

        /// <summary>
        /// Gets or sets the value (0 or 1) that indicates alarm state for the <see cref="MetadataRecord"/>.
        /// </summary>
        /// <remarks>A value of -1 indicates no alarm state.</remarks>
        /// <exception cref="ArgumentException">The value being assigned is not -1, 0 or 1.</exception>
        public int AlarmState
        {
            get
            {
                return m_alarmState;
            }
            set
            {
                if (value < -1 || value > 1)
                    throw new ArgumentException("Value must be either -1, 0 or 1");

                m_alarmState = value;
            }
        }

        /// <summary>
        /// Gets or sets the time (in seconds) to wait before consecutive alarm notifications are sent for the <see cref="MetadataRecord"/>.
        /// </summary>
        public float AlarmDelay
        {
            get
            {
                return m_alarmDelay;
            }
            set
            {
                m_alarmDelay = value;
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
        /// Gets the binary representation of <see cref="MetadataRecordDigitalFields"/>.
        /// </summary>
        public byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(Encoding.ASCII.GetBytes(m_setDescription.PadRight(24)), 0, image, 0, 24);
                Array.Copy(Encoding.ASCII.GetBytes(m_clearDescription.PadRight(24)), 0, image, 24, 24);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_alarmState), 0, image, 48, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_alarmDelay), 0, image, 52, 4);

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="MetadataRecordDigitalFields"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="MetadataRecordDigitalFields"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="MetadataRecordDigitalFields"/>.</returns>
        public int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                SetDescription = Encoding.ASCII.GetString(binaryImage, startIndex, 24).Trim();
                ClearDescription = Encoding.ASCII.GetString(binaryImage, startIndex + 24, 24).Trim();
                AlarmState = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 48);
                AlarmDelay = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 52);

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
