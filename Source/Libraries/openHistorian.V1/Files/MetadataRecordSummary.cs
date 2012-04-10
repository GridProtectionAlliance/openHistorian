//******************************************************************************************************
//  MetadataRecordSummary.cs - Gbtc
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
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  04/20/2009 - Pinal C. Patel
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
using TVA;
using TVA.Parsing;

namespace openHistorian.V1.Files
{
    /// <summary>
    /// A class with a subset of information defined in <see cref="MetadataRecord"/>. The generated binary image of 
    /// <see cref="MetadataRecordSummary"/> is sent back as a reply to <see cref="openHistorian.V1.Adapters.PacketType3"/> and 
    /// <see cref="openHistorian.V1.Adapters.PacketType4"/>.
    /// </summary>
    /// <seealso cref="MetadataRecord"/>
    public class MetadataRecordSummary : ISupportBinaryImage
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 4          0-3        Int32      HistorianID                                                   *
        // * 4          4-7        Single     ExceptionLimit                                                *
        // * 4          8-11       Int32      Enabled                                                       *
        // * 4          12-15      Single     HighWarning                                                   *
        // * 4          16-19      Single     LowWarning                                                    *
        // * 4          20-23      Single     HighAlarm                                                     *
        // * 4          24-27      Single     LowAlarm                                                      *
        // * 4          28-31      Single     HighRange                                                     *
        // * 4          32-35      Single     LowRange                                                      *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="MetadataRecordSummary"/>.
        /// </summary>
        public const int FixedLength = 36;

        // Fields
        private int m_historianID;
        private float m_exceptionLimit;
        private int m_enabled;
        private float m_highWarning;
        private float m_lowWarning;
        private float m_highAlarm;
        private float m_lowAlarm;
        private float m_highRange;
        private float m_lowRange;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataRecordSummary"/> class.
        /// </summary>
        /// <param name="record">A <see cref="MetadataRecord"/> object.</param>
        public MetadataRecordSummary(MetadataRecord record)
        {
            HistorianID = record.HistorianID;
            ExceptionLimit = record.AnalogFields.ExceptionLimit;
            Enabled = record.GeneralFlags.Enabled;
            HighWarning = record.AnalogFields.HighWarning;
            LowWarning = record.AnalogFields.LowWarning;
            HighAlarm = record.AnalogFields.HighAlarm;
            LowAlarm = record.AnalogFields.LowAlarm;
            HighRange = record.AnalogFields.HighRange;
            LowRange = record.AnalogFields.LowRange;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataRecordSummary"/> class.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="MetadataRecordSummary"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public MetadataRecordSummary(byte[] buffer, int startIndex, int length)
        {
            ParseBinaryImage(buffer, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Same as <see cref="MetadataRecord.HistorianID"/>.
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
        /// Same as <see cref="MetadataRecordAnalogFields.ExceptionLimit"/>.
        /// </summary>
        public float ExceptionLimit
        {
            get
            {
                return m_exceptionLimit;
            }
            private set
            {
                m_exceptionLimit = value;
            }
        }

        /// <summary>
        /// Same as <see cref="MetadataRecordGeneralFlags.Enabled"/>.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return Convert.ToBoolean(m_enabled);
            }
            private set
            {
                m_enabled = Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// Same as <see cref="MetadataRecordAnalogFields.HighWarning"/>.
        /// </summary>
        public float HighWarning
        {
            get
            {
                return m_highWarning;
            }
            private set
            {
                m_highWarning = value;
            }
        }

        /// <summary>
        /// Same as <see cref="MetadataRecordAnalogFields.LowWarning"/>.
        /// </summary>
        public float LowWarning
        {
            get
            {
                return m_lowWarning;
            }
            private set
            {
                m_lowWarning = value;
            }
        }

        /// <summary>
        /// Same as <see cref="MetadataRecordAnalogFields.HighAlarm"/>.
        /// </summary>
        public float HighAlarm
        {
            get
            {
                return m_highAlarm;
            }
            private set
            {
                m_highAlarm = value;
            }
        }

        /// <summary>
        /// Same as <see cref="MetadataRecordAnalogFields.LowAlarm"/>.
        /// </summary>
        public float LowAlarm
        {
            get
            {
                return m_lowAlarm;
            }
            private set
            {
                m_lowAlarm = value;
            }
        }

        /// <summary>
        /// Same as <see cref="MetadataRecordAnalogFields.HighRange"/>.
        /// </summary>
        public float HighRange
        {
            get
            {
                return m_highRange;
            }
            private set
            {
                m_highRange = value;
            }
        }

        /// <summary>
        /// Same as <see cref="MetadataRecordAnalogFields.LowRange"/>.
        /// </summary>
        public float LowRange
        {
            get
            {
                return m_lowRange;
            }
            private set
            {
                m_lowRange = value;
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="MetadataRecordSummary"/>.
        /// </summary>
        public int BinaryLength
        {
            get
            {
                return FixedLength;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="MetadataRecordSummary"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="MetadataRecordSummary"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="MetadataRecordSummary"/>.</returns>
        public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= FixedLength)
            {
                // Binary image has sufficient data.
                HistorianID = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 0);
                ExceptionLimit = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 4);
                Enabled = Convert.ToBoolean(EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 8));
                HighWarning = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 12);
                LowWarning = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 16);
                HighAlarm = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 20);
                LowAlarm = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 24);
                HighRange = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 28);
                LowRange = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 32);

                return FixedLength;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="MetadataRecordSummary"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        public virtual int GenerateBinaryImage(byte[] buffer, int startIndex)
        {
            int length = BinaryLength;

            buffer.ValidateParameters(startIndex, length);

            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_historianID), 0, buffer, startIndex, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_exceptionLimit), 0, buffer, startIndex + 4, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(Convert.ToInt32(m_enabled)), 0, buffer, startIndex + 8, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_highWarning), 0, buffer, startIndex + 12, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_lowWarning), 0, buffer, startIndex + 16, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_highAlarm), 0, buffer, startIndex + 20, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_lowAlarm), 0, buffer, startIndex + 24, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_highRange), 0, buffer, startIndex + 28, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_lowRange), 0, buffer, startIndex + 32, 4);

            return length;
        }

        #endregion
    }
}
