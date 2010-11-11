//******************************************************************************************************
//  MetadataRecordComposedFields.cs - Gbtc
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
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using TVA;
using TVA.Parsing;

namespace TimeSeriesArchiver.Files
{
    /// <summary>
    /// Defines specific fields for <see cref="MetadataRecord"/>s that are of type <see cref="DataType.Composed"/>.
    /// </summary>
    /// <seealso cref="MetadataRecord"/>
    public class MetadataRecordComposedFields : ISupportBinaryImage
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 4           0-3         Single      HighAlarm                                                  *
        // * 4           4-7         Single      LowAlarm                                                   *
        // * 4           8-11        Single      HighRange                                                  *
        // * 4           12-15       Single      LowRange                                                   *
        // * 4           16-19       Single      LowWarning                                                 *
        // * 4           20-23       Single      HighWarning                                                *
        // * 4           24-27       Int32       DisplayDigits                                              *
        // * 48          28-75       Int32(12)   InputPointers                                              *
        // * 24          76-99       Char(24)    EngineeringUnits                                           *
        // * 256         100-355     Char(256)   Equation                                                   *
        // * 4           356-359     Single      CompressionLimit                                           *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="MetadataRecordComposedFields"/>.
        /// </summary>
        public const int ByteCount = 360;

        // Fields
        private float m_highAlarm;
        private float m_lowAlarm;
        private float m_highRange;
        private float m_lowRange;
        private float m_lowWarning;
        private float m_highWarning;
        private int m_displayDigits;
        private List<int> m_inputPointers;
        private string m_engineeringUnits;
        private string m_equation;
        private float m_compressionLimit;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataRecordComposedFields"/> class.
        /// </summary>
        internal MetadataRecordComposedFields()
        {
            m_engineeringUnits = string.Empty;
            m_equation = string.Empty;
            m_inputPointers = new List<int>();
            for (int i = 0; i < 12; i++)
            {
                m_inputPointers.Add(default(int));
            }
        }

        #endregion

        #region [ Properties ]

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
        /// Gets or sets the historian identifiers being used in the <see cref="Equation"/>.
        /// </summary>
        public IList<int> InputPointers
        {
            get
            {
                return m_inputPointers.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets the engineering units of archived data values for the <see cref="MetadataRecord"/>.
        /// </summary>
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
        /// Gets or sets the mathematical equation used for calculating the data value for the <see cref="MetadataRecord"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public string Equation
        {
            get
            {
                return m_equation;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_equation = value.TruncateRight(256);
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
        /// Gets the binary representation of <see cref="MetadataRecordComposedFields"/>.
        /// </summary>
        public byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_highAlarm), 0, image, 0, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_lowAlarm), 0, image, 4, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_highRange), 0, image, 8, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_lowRange), 0, image, 12, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_lowWarning), 0, image, 16, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_highWarning), 0, image, 20, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_displayDigits), 0, image, 24, 4);
                for (int i = 0; i < m_inputPointers.Count; i++)
                {
                    Array.Copy(EndianOrder.LittleEndian.GetBytes(m_inputPointers[i]), 0, image, (28 + (i * 4)), 4);
                }
                Array.Copy(Encoding.ASCII.GetBytes(m_engineeringUnits.PadRight(24)), 0, image, 76, 24);
                Array.Copy(Encoding.ASCII.GetBytes(m_equation.PadRight(256)), 0, image, 100, 256);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_compressionLimit), 0, image, 356, 4);

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="MetadataRecordComposedFields"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="MetadataRecordComposedFields"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="MetadataRecordComposedFields"/>.</returns>
        public int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                HighAlarm = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex);
                LowAlarm = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 4);
                HighRange = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 8);
                LowRange = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 12);
                LowWarning = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 16);
                HighWarning = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 20);
                DisplayDigits = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 24);
                for (int i = 0; i < m_inputPointers.Count; i++)
                {
                    m_inputPointers[i] = EndianOrder.LittleEndian.ToInt32(binaryImage, (startIndex + 28 + (i * 4)));
                }
                EngineeringUnits = Encoding.ASCII.GetString(binaryImage, startIndex + 76, 24).Trim();
                Equation = Encoding.ASCII.GetString(binaryImage, startIndex + 100, 256).Trim();
                CompressionLimit = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 356);

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
