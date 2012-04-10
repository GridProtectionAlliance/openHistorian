//******************************************************************************************************
//  MetadataRecordAnalogFields.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
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
//
//******************************************************************************************************

using System;
using System.Text;
using TVA;
using TVA.Parsing;

namespace openHistorian.Archives.V1
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
        /// Gets the length of the <see cref="MetadataRecordAnalogFields"/>.
        /// </summary>
        public int BinaryLength
        {
            get
            {
                return ByteCount;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="MetadataRecordAnalogFields"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="MetadataRecordAnalogFields"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="MetadataRecordAnalogFields"/>.</returns>
        public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= ByteCount)
            {
                // Binary image has sufficient data.
                EngineeringUnits = Encoding.ASCII.GetString(buffer, startIndex, 24).Trim();
                HighAlarm = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 24);
                LowAlarm = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 28);
                HighRange = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 32);
                LowRange = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 36);
                HighWarning = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 40);
                LowWarning = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 44);
                ExceptionLimit = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 48);
                CompressionLimit = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 52);
                AlarmDelay = EndianOrder.LittleEndian.ToSingle(buffer, startIndex + 56);
                DisplayDigits = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 60);

                return ByteCount;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="MetadataRecordAnalogFields"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
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

            Buffer.BlockCopy(Encoding.ASCII.GetBytes(m_engineeringUnits.PadRight(24)), 0, buffer, startIndex, 24);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_highAlarm), 0, buffer, startIndex + 24, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_lowAlarm), 0, buffer, startIndex + 28, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_highRange), 0, buffer, startIndex + 32, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_lowRange), 0, buffer, startIndex + 36, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_highWarning), 0, buffer, startIndex + 40, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_lowWarning), 0, buffer, startIndex + 44, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_exceptionLimit), 0, buffer, startIndex + 48, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_compressionLimit), 0, buffer, startIndex + 52, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_alarmDelay), 0, buffer, startIndex + 56, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_displayDigits), 0, buffer, startIndex + 60, 4);

            return length;
        }

        #endregion
    }
}
