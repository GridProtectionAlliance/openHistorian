//******************************************************************************************************
//  MetadataRecordAnalogFields.cs - Gbtc
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
    /// Defines specific fields for <see cref="MetadataRecord"/>s that are of type <see cref="DataType.Analog"/>.
    /// </summary>
    /// <seealso cref="MetadataRecord"/>
    public class MetadataRecordAnalogFields : ISupportBinaryImage
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 24         0-23       Char(24)   EngineeringUnits                                              *
        // * 4          24-27      Single     HighAlarm                                                     *
        // * 4          28-31      Single     LowAlarm                                                      *
        // * 4          32-35      Single     HighRange                                                     *
        // * 4          36-39      Single     LowRange                                                      *
        // * 4          40-43      Single     HighWarning                                                   *
        // * 4          44-47      Single     LowWarning                                                    *
        // * 4          48-51      Single     ExceptionLimit                                                *
        // * 4          52-55      Single     CompressionLimit                                              *
        // * 4          56-59      Single     AlarmDelay                                                    *
        // * 4          60-63      Int32      DisplayDigits                                                 *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="MetadataRecordAnalogFields"/>.
        /// </summary>
        public const int ByteCount = 64;

        // Fields
        private string m_engineeringUnits;
        private float m_highAlarm;
        private float m_lowAlarm;
        private float m_highRange;
        private float m_lowRange;
        private float m_highWarning;
        private float m_lowWarning;
        private float m_exceptionLimit;
        private float m_compressionLimit;
        private float m_alarmDelay;
        private int m_displayDigits;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataRecordAnalogFields"/> class.
        /// </summary>
        internal MetadataRecordAnalogFields()
        {
            m_engineeringUnits = string.Empty;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the engineering units of archived data values for the <see cref="MetadataRecord"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="EngineeringUnits"/> is 24 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public string EngineeringUnits
        {
            get
            {
                return m_engineeringUnits;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_engineeringUnits = value.TruncateRight(24);
            }
        }

        /// <summary>
        /// Gets or sets the value above which archived data for the <see cref="MetadataRecord"/> is assigned a quality of <see cref="Quality.ValueAboveHiHiAlarm"/>.
        /// </summary>
        public float HighAlarm
        {
            get
            {
                return m_highAlarm;
            }
            set
            {
                m_highAlarm = value;
            }
        }

        /// <summary>
        /// Gets or sets the value above which archived data for the <see cref="MetadataRecord"/> is assigned a quality of <see cref="Quality.ValueBelowLoLoAlarm"/>.
        /// </summary>
        public float LowAlarm
        {
            get
            {
                return m_lowAlarm;
            }
            set
            {
                m_lowAlarm = value;
            }
        }

        /// <summary>
        /// Gets or sets the value above which archived data for the <see cref="MetadataRecord"/> is assigned a quality of <see cref="Quality.UnreasonableHigh"/>.
        /// </summary>
        public float HighRange
        {
            get
            {
                return m_highRange;
            }
            set
            {
                m_highRange = value;
            }
        }

        /// <summary>
        /// Gets or sets the value above which archived data for the <see cref="MetadataRecord"/> is assigned a quality of <see cref="Quality.UnreasonableLow"/>.
        /// </summary>
        public float LowRange
        {
            get
            {
                return m_lowRange;
            }
            set
            {
                m_lowRange = value;
            }
        }

        /// <summary>
        /// Gets or sets the value above which archived data for the <see cref="MetadataRecord"/> is assigned a quality of <see cref="Quality.ValueAboveHiAlarm"/>.
        /// </summary>
        public float HighWarning
        {
            get
            {
                return m_highWarning;
            }
            set
            {
                m_highWarning = value;
            }
        }

        /// <summary>
        /// Gets or sets the value above which archived data for the <see cref="MetadataRecord"/> is assigned a quality of <see cref="Quality.ValueBelowLoAlarm"/>.
        /// </summary>
        public float LowWarning
        {
            get
            {
                return m_lowWarning;
            }
            set
            {
                m_lowWarning = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount, expressed in <see cref="EngineeringUnits"/>,  by which data values for the <see cref="MetadataRecord"/> must change before being sent for archival by the data aquisition source.
        /// </summary>
        public float ExceptionLimit
        {
            get
            {
                return m_exceptionLimit;
            }
            set
            {
                m_exceptionLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount, expressed in <see cref="EngineeringUnits"/>, by which data values for the <see cref="MetadataRecord"/>  must changed before being archived.
        /// </summary>
        public float CompressionLimit
        {
            get
            {
                return m_compressionLimit;
            }
            set
            {
                m_compressionLimit = value;
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
        /// Gets or sets the number of digits after the decimal point to be displayed for the <see cref="MetadataRecord"/>.
        /// </summary>
        public int DisplayDigits
        {
            get
            {
                return m_displayDigits;
            }
            set
            {
                m_displayDigits = value;
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
        /// Gets the binary representation of <see cref="MetadataRecordAnalogFields"/>.
        /// </summary>
        public byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(Encoding.ASCII.GetBytes(m_engineeringUnits.PadRight(24)), 0, image, 0, 24);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_highAlarm), 0, image, 24, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_lowAlarm), 0, image, 28, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_highRange), 0, image, 32, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_lowRange), 0, image, 36, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_highWarning), 0, image, 40, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_lowWarning), 0, image, 44, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_exceptionLimit), 0, image, 48, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_compressionLimit), 0, image, 52, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_alarmDelay), 0, image, 56, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_displayDigits), 0, image, 60, 4);

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="MetadataRecordAnalogFields"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="MetadataRecordAnalogFields"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="MetadataRecordAnalogFields"/>.</returns>
        public int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                EngineeringUnits = Encoding.ASCII.GetString(binaryImage, startIndex, 24).Trim();
                HighAlarm = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 24);
                LowAlarm = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 28);
                HighRange = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 32);
                LowRange = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 36);
                HighWarning = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 40);
                LowWarning = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 44);
                ExceptionLimit = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 48);
                CompressionLimit = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 52);
                AlarmDelay = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 56);
                DisplayDigits = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 60);

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
