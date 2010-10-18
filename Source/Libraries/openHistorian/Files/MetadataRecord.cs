//******************************************************************************************************
//  MetadataRecord.cs - Gbtc
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
//  05/03/2006 - Pinal C. Patel
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
using System.Text;
using TVA;
using TVA.Parsing;

namespace openHistorian.Files
{
    /// <summary>
    /// Represents a record in the <see cref="MetadataFile"/> that contains the various attributes associates to a <see cref="HistorianID"/>.
    /// </summary>
    /// <seealso cref="MetadataFile"/>
    /// <seealso cref="MetadataRecordAlarmFlags"/>
    /// <seealso cref="MetadataRecordGeneralFlags"/>
    /// <seealso cref="MetadataRecordSecurityFlags"/>
    /// <seealso cref="MetadataRecordAnalogFields"/>
    /// <seealso cref="MetadataRecordComposedFields"/>
    /// <seealso cref="MetadataRecordConstantFields"/>
    /// <seealso cref="MetadataRecordDigitalFields"/>
    /// <seealso cref="MetadataRecordSummary"/>
    public class MetadataRecord : ISupportBinaryImage, IComparable
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 512        0-511      Char(512)  Remarks                                                       *
        // * 512        512-1023   Char(512)  HardwareInfo                                                  *
        // * 512        1024-1535  Char(512)  EmailAddresses                                                *
        // * 80         1536-1615  Char(80)   Description                                                   *
        // * 80         1616-1695  Char(80)   CurrentData                                                   *
        // * 40         1696-1735  Char(40)   Name                                                          *
        // * 40         1736-1775  Char(40)   Synonym1                                                      *
        // * 40         1776-1815  Char(40)   Synonym2                                                      *
        // * 40         1816-1855  Char(40)   Synonym3                                                      *
        // * 40         1856-1895  Char(40)   PagerNumbers                                                  *
        // * 40         1896-1935  Char(40)   PhoneNumbers                                                  *
        // * 24         1936-1959  Char(24)   PlantCode                                                     *
        // * 24         1960-1983  Char(24)   System                                                        *
        // * 40         1984-2023  Char(40)   EmailTime                                                     *
        // * 40         2024-2063  Char(40)   [Spare string field 1]                                        *
        // * 40         2064-2103  Char(40)   [Spare string field 2]                                        *
        // * 4          2104-2107  Single     ScanRate                                                      *
        // * 4          2108-2111  Int32      UnitNumber                                                    *
        // * 4          2112-2115  Int32      SecurityFlags                                                 *
        // * 4          2116-2119  Int32      GeneralFlags                                                  *
        // * 4          2120-2123  Int32      AlarmFlags                                                    *
        // * 4          2124-2127  Int32      CompressionMinTime                                            *
        // * 4          2128-2131  Int32      CompressionMaxTime                                            *
        // * 4          2132-2135  Int32      SourceID                                                      *
        // * 4          2136-2139  Int32      [Spare 32-bit field 1]                                        *
        // * 4          2140-2143  Int32      [Spare 32-bit field 2]                                        *
        // * 4          2144-2147  Int32      [Spare 32-bit field 3]                                        *
        // * 4          2148-2151  Int32      [Spare 32-bit field 4]                                        *
        // * 512        2152-2663  Byte(512)  (Analog | Digital| Composed |Constant)Fields                  *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="MetadataRecord"/>.
        /// </summary>
        public const int ByteCount = 2664;

        // Fields
        private int m_historianID;
        private string m_remarks;
        private string m_hardwareInfo;
        private string m_emailAddresses;
        private string m_description;
        private string m_currentData;
        private string m_name;
        private string m_synonym1;
        private string m_synonym2;
        private string m_synonym3;
        private string m_pagerNumbers;
        private string m_phoneNumbers;
        private string m_plantCode;
        private string m_system;
        private string m_emailTime;
        private float m_scanRate;
        private int m_unitNumber;
        private MetadataRecordSecurityFlags m_securityFlags;
        private MetadataRecordGeneralFlags m_generalFlags;
        private MetadataRecordAlarmFlags m_alarmFlags;
        private int m_compressionMinTime;
        private int m_compressionMaxTime;
        private int m_sourceID;
        private MetadataRecordAnalogFields m_analogFields;
        private MetadataRecordDigitalFields m_digitalFields;
        private MetadataRecordComposedFields m_composedFields;
        private MetadataRecordConstantFields m_constantFields;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataRecord"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="MetadataRecord"/>.</param>
        public MetadataRecord(int historianID)
        {
            m_historianID = historianID;
            m_remarks = string.Empty;
            m_hardwareInfo = string.Empty;
            m_emailAddresses = string.Empty;
            m_description = string.Empty;
            m_currentData = string.Empty;
            m_name = string.Empty;
            m_synonym1 = string.Empty;
            m_synonym2 = string.Empty;
            m_synonym3 = string.Empty;
            m_pagerNumbers = string.Empty;
            m_phoneNumbers = string.Empty;
            m_plantCode = string.Empty;
            m_system = string.Empty;
            m_emailTime = string.Empty;
            m_securityFlags = new MetadataRecordSecurityFlags();
            m_generalFlags = new MetadataRecordGeneralFlags();
            m_alarmFlags = new MetadataRecordAlarmFlags();
            m_analogFields = new MetadataRecordAnalogFields();
            m_digitalFields = new MetadataRecordDigitalFields();
            m_composedFields = new MetadataRecordComposedFields();
            m_constantFields = new MetadataRecordConstantFields();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataRecord"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="MetadataRecord"/>.</param>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="MetadataRecord"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        public MetadataRecord(int historianID, byte[] binaryImage, int startIndex, int length)
            : this(historianID)
        {
            Initialize(binaryImage, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets any remarks associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="Remarks"/> is 512 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string Remarks
        {
            get
            {
                return m_remarks;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_remarks = value.TruncateRight(512);
            }
        }

        /// <summary>
        /// Gets or sets hardware information associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="HardwareInfo"/> is 512 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string HardwareInfo
        {
            get
            {
                return m_hardwareInfo;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_hardwareInfo = value.TruncateRight(512);
            }
        }

        /// <summary>
        /// Gets or sets a comma-seperated list of email addresses that will receive alarm notification email messages based 
        /// on the <see cref="AlarmFlags"/> settings for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="AlarmEmails"/> is 512 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string AlarmEmails
        {
            get
            {
                return m_emailAddresses;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_emailAddresses = value.TruncateRight(512);
            }
        }

        /// <summary>
        /// Gets or sets the description associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="Description"/> is 80 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_description = value.TruncateRight(80);
            }
        }

        /// <summary>
        /// Gets or sets the time, value and quality of the most current data received for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="CurrentData"/> is 80 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string CurrentData
        {
            get
            {
                return m_currentData;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_currentData = value.TruncateRight(80);
            }
        }

        /// <summary>
        /// Gets or sets a alpha-numeric name for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="Name"/> is 40 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_name = value.TruncateRight(40);
            }
        }

        /// <summary>
        /// Gets or sets an alternate <see cref="Name"/> for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="Synonym1"/> is 40 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string Synonym1
        {
            get
            {
                return m_synonym1;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_synonym1 = value.TruncateRight(40);
            }
        }

        /// <summary>
        /// Gets or sets an alternate <see cref="Name"/> for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="Synonym2"/> is 40 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string Synonym2
        {
            get
            {
                return m_synonym2;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_synonym2 = value.TruncateRight(40);
            }
        }

        /// <summary>
        /// Gets or sets an alternate <see cref="Name"/> for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="Synonym3"/> is 40 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string Synonym3
        {
            get
            {
                return m_synonym3;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_synonym3 = value.TruncateRight(40);
            }
        }

        /// <summary>
        /// Gets or sets a comma-seperated list of pager numbers that will receive alarm notification text messages based 
        /// on the <see cref="AlarmFlags"/> settings for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="AlarmPagers"/> is 40 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string AlarmPagers
        {
            get
            {
                return m_pagerNumbers;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_pagerNumbers = value.TruncateRight(40);
            }
        }

        /// <summary>
        /// Gets or sets a comma-seperated list of phone numbers that will receive alarm notification voice messages based 
        /// on the <see cref="AlarmFlags"/> settings for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="AlarmPhones"/> is 40 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string AlarmPhones
        {
            get
            {
                return m_phoneNumbers;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_phoneNumbers = value.TruncateRight(40);
            }
        }
        
        /// <summary>
        /// Gets or sets the name of the plant to which the <see cref="HistorianID"/> is associated.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="PlantCode"/> is 24 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string PlantCode
        {
            get
            {
                return m_plantCode;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_plantCode = value.TruncateRight(24);
            }
        }

        /// <summary>
        /// Gets or sets the alpha-numeric system identifier for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="SystemName"/> is 24 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string SystemName
        {
            get
            {
                return m_system;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_system = value.TruncateRight(24);
            }
        }

        /// <summary>
        /// Gets or sets the data and time when an alarm notification is sent based on the <see cref="AlarmFlags"/> settings for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// Maximum length for <see cref="EmailTime"/> is 40 characters.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value being assigned is a null string.</exception>
        public string EmailTime
        {
            get
            {
                return m_emailTime;
            }
            set
            {
                m_emailTime = value.TruncateRight(40);
            }
        }

        /// <summary>
        /// Gets or sets the rate at which the source device scans and sends data for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="ScanRate"/> is used by data aquisition components for polling data from the actual device.
        /// </remarks>
        public float ScanRate
        {
            get
            {
                return m_scanRate;
            }
            set
            {
                m_scanRate = value;
            }
        }

        /// <summary>
        /// Gets or sets the unit (i.e. generator) to which the <see cref="HistorianID"/> is associated.
        /// </summary>
        public int UnitNumber
        {
            get
            {
                return m_unitNumber;
            }
            set
            {
                m_unitNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordSecurityFlags"/> associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public MetadataRecordSecurityFlags SecurityFlags
        {
            get
            {
                return m_securityFlags;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_securityFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags"/> associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public MetadataRecordGeneralFlags GeneralFlags
        {
            get
            {
                return m_generalFlags;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_generalFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAlarmFlags"/> associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public MetadataRecordAlarmFlags AlarmFlags
        {
            get
            {
                return m_alarmFlags;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_alarmFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowable time (in seconds) between archived data for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CompressionMinTime"/> is useful for limiting archived data for noisy <see cref="HistorianID"/>s.
        /// </remarks>
        public int CompressionMinTime
        {
            get
            {
                return m_compressionMinTime;
            }
            set
            {
                m_compressionMinTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum time (in seconds) after which data is to be archived for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CompressionMaxTime"/> ensures that archived data exist every "n" seconds for the <see cref="HistorianID"/>, 
        /// which would otherwise be omitted due to compression.
        /// </remarks>
        public int CompressionMaxTime
        {
            get
            {
                return m_compressionMaxTime;
            }
            set
            {
                m_compressionMaxTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the numeric identifier of the data source for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="SourceID"/> is used for the determination of "global time" when that client option is in effect.  
        /// When "global time" is in effect, the historian returns the current data time for a <see cref="HistorianID"/> 
        /// based on the latest time received for all <see cref="HistorianID"/>s with the same <see cref="SourceID"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">The value being assigned is not positive or zero.</exception>
        public int SourceID
        {
            get
            {
                return m_sourceID;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be positive or zero");

                m_sourceID = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields"/> associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public MetadataRecordAnalogFields AnalogFields
        {
            get
            {
                return m_analogFields;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_analogFields = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordDigitalFields"/> associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public MetadataRecordDigitalFields DigitalFields
        {
            get
            {
                return m_digitalFields;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_digitalFields = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordComposedFields"/> associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public MetadataRecordComposedFields ComposedFields
        {
            get
            {
                return m_composedFields;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_composedFields = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordConstantFields"/> associated with the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public MetadataRecordConstantFields ConstantFields
        {
            get
            {
                return m_constantFields;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_constantFields = value;
            }
        }

        /// <summary>
        /// Gets the historian identifier of <see cref="MetadataRecord"/>.
        /// </summary>
        public int HistorianID
        {
            get
            {
                return m_historianID;
            }
        }

        /// <summary>
        /// Gets the <see cref="MetadataRecordSummary"/> object for <see cref="MetadataRecord"/>.
        /// </summary>
        public MetadataRecordSummary Summary
        {
            get
            {
                return new MetadataRecordSummary(this);
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
        /// Gets the binary representation of <see cref="MetadataRecord"/>.
        /// </summary>
        public byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[ByteCount];

                // Construct the binary IP buffer for this event
                Array.Copy(Encoding.ASCII.GetBytes(m_remarks.PadRight(512)), 0, image, 0, 512);
                Array.Copy(Encoding.ASCII.GetBytes(m_hardwareInfo.PadRight(512)), 0, image, 512, 512);
                Array.Copy(Encoding.ASCII.GetBytes(m_emailAddresses.PadRight(512)), 0, image, 1024, 512);
                Array.Copy(Encoding.ASCII.GetBytes(m_description.PadRight(80)), 0, image, 1536, 80);
                Array.Copy(Encoding.ASCII.GetBytes(m_currentData.PadRight(80)), 0, image, 1616, 80);
                Array.Copy(Encoding.ASCII.GetBytes(m_name.PadRight(40)), 0, image, 1696, 40);
                Array.Copy(Encoding.ASCII.GetBytes(m_synonym1.PadRight(40)), 0, image, 1736, 40);
                Array.Copy(Encoding.ASCII.GetBytes(m_synonym2.PadRight(40)), 0, image, 1776, 40);
                Array.Copy(Encoding.ASCII.GetBytes(m_synonym3.PadRight(40)), 0, image, 1816, 40);
                Array.Copy(Encoding.ASCII.GetBytes(m_pagerNumbers.PadRight(40)), 0, image, 1856, 40);
                Array.Copy(Encoding.ASCII.GetBytes(m_phoneNumbers.PadRight(40)), 0, image, 1896, 40);
                Array.Copy(Encoding.ASCII.GetBytes(m_plantCode.PadRight(24)), 0, image, 1936, 24);
                Array.Copy(Encoding.ASCII.GetBytes(m_system.PadRight(24)), 0, image, 1960, 24);
                Array.Copy(Encoding.ASCII.GetBytes(m_emailTime.PadRight(40)), 0, image, 1984, 40);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_scanRate), 0, image, 2104, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_unitNumber), 0, image, 2108, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_securityFlags.Value), 0, image, 2112, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_generalFlags.Value), 0, image, 2116, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_alarmFlags.Value), 0, image, 2120, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_compressionMinTime), 0, image, 2124, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_compressionMaxTime), 0, image, 2128, 4);
                Array.Copy(EndianOrder.LittleEndian.GetBytes(m_sourceID), 0, image, 2132, 4);
                switch (m_generalFlags.DataType)
                {
                    case DataType.Analog:
                        Array.Copy(m_analogFields.BinaryImage, 0, image, 2152, MetadataRecordAnalogFields.ByteCount);
                        break;
                    case DataType.Digital:
                        Array.Copy(m_digitalFields.BinaryImage, 0, image, 2152, MetadataRecordDigitalFields.ByteCount);
                        break;
                    case DataType.Composed:
                        Array.Copy(m_composedFields.BinaryImage, 0, image, 2152, MetadataRecordComposedFields.ByteCount);
                        break;
                    case DataType.Constant:
                        Array.Copy(m_constantFields.BinaryImage, 0, image, 2152, MetadataRecordConstantFields.ByteCount);
                        break;
                }

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="MetadataRecord"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="MetadataRecord"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="MetadataRecord"/>.</returns>
        public int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                Remarks = Encoding.ASCII.GetString(binaryImage, startIndex, 512).Trim();
                HardwareInfo = Encoding.ASCII.GetString(binaryImage, startIndex + 512, 512).Trim();
                AlarmEmails = Encoding.ASCII.GetString(binaryImage, startIndex + 1024, 512).Trim();
                Description = Encoding.ASCII.GetString(binaryImage, startIndex + 1536, 80).Trim();
                CurrentData = Encoding.ASCII.GetString(binaryImage, startIndex + 1616, 80).Trim();
                Name = Encoding.ASCII.GetString(binaryImage, startIndex + 1696, 40).Trim();
                Synonym1 = Encoding.ASCII.GetString(binaryImage, startIndex + 1736, 40).Trim();
                Synonym2 = Encoding.ASCII.GetString(binaryImage, startIndex + 1776, 40).Trim();
                Synonym3 = Encoding.ASCII.GetString(binaryImage, startIndex + 1816, 40).Trim();
                AlarmPagers = Encoding.ASCII.GetString(binaryImage, startIndex + 1856, 40).Trim();
                AlarmPhones = Encoding.ASCII.GetString(binaryImage, startIndex + 1896, 40).Trim();
                PlantCode = Encoding.ASCII.GetString(binaryImage, startIndex + 1936, 24).Trim();
                SystemName = Encoding.ASCII.GetString(binaryImage, startIndex + 1960, 24).Trim();
                EmailTime = Encoding.ASCII.GetString(binaryImage, startIndex + 1984, 40).Trim();
                ScanRate = EndianOrder.LittleEndian.ToSingle(binaryImage, startIndex + 2104);
                UnitNumber = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2108);
                SecurityFlags.Value = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2112);
                GeneralFlags.Value = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2116);
                AlarmFlags.Value = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2120);
                CompressionMinTime = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2124);
                CompressionMaxTime = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2128);
                SourceID = EndianOrder.LittleEndian.ToInt32(binaryImage, startIndex + 2132);
                switch (GeneralFlags.DataType)
                {
                    case DataType.Analog:
                        m_analogFields.Initialize(binaryImage, startIndex + 2152, length - 2152);
                        break;
                    case DataType.Digital:
                        m_digitalFields.Initialize(binaryImage, startIndex + 2152, length - 2152);
                        break;
                    case DataType.Composed:
                        m_composedFields.Initialize(binaryImage, startIndex + 2152, length - 2152);
                        break;
                    case DataType.Constant:
                        m_constantFields.Initialize(binaryImage, startIndex + 2152, length - 2152);
                        break;
                }

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Compares the current <see cref="MetadataRecord"/> object to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="MetadataRecord"/> object is to be compared.</param>
        /// <returns>
        /// Negative value if the current <see cref="MetadataRecord"/> object is less than <paramref name="obj"/>, 
        /// Zero if the current <see cref="MetadataRecord"/> object is equal to <paramref name="obj"/>, 
        /// Positive value if the current <see cref="MetadataRecord"/> object is greater than <paramref name="obj"/>.
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            MetadataRecord other = obj as MetadataRecord;
            if (other == null)
                return 1;
            else
                return m_historianID.CompareTo(other.HistorianID);
        }

        /// <summary>
        /// Determines whether the current <see cref="MetadataRecord"/> object is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="MetadataRecord"/> object is to be compared for equality.</param>
        /// <returns>true if the current <see cref="MetadataRecord"/> object is equal to <paramref name="obj"/>; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Returns the text representation of <see cref="MetadataRecord"/> object.
        /// </summary>
        /// <returns>A <see cref="string"/> value.</returns>
        public override string ToString()
        {
            return string.Format("ID={0}; Name={1}", m_historianID, m_name);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="MetadataRecord"/> object.
        /// </summary>
        /// <returns>A 32-bit signed integer value.</returns>
        public override int GetHashCode()
        {
            return m_historianID.GetHashCode();
        }

        #endregion
    }
}
