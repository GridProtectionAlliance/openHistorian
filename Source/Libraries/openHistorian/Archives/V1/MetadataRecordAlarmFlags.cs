//******************************************************************************************************
//  MetadataRecordAlarmFlags.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  02/22/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using TVA;

namespace openHistorian.Archives.V1
{
    /// <summary>
    /// Defines which data <see cref="Quality"/> should trigger an alarm notification.
    /// </summary>
    /// <seealso cref="MetadataRecord"/>
    public class MetadataRecordAlarmFlags
    {
        #region [ Members ]

        // Fields
        private int m_value;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.Unknown"/> should trigger an alarm notification.
        /// </summary>
        public bool Unknown
        {
            get
            {
                return m_value.CheckBits(Bits.Bit00);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit00) : m_value.ClearBits(Bits.Bit00);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.DeletedFromProcessing"/> should trigger an alarm notification.
        /// </summary>
        public bool DeletedFromProcessing
        {
            get
            {
                return m_value.CheckBits(Bits.Bit01);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit01) : m_value.ClearBits(Bits.Bit01);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.CouldNotCalculate"/> should trigger an alarm notification.
        /// </summary>
        public bool CouldNotCalculate
        {
            get
            {
                return m_value.CheckBits(Bits.Bit02);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit02) : m_value.ClearBits(Bits.Bit02);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.FrontEndHardwareError"/> should trigger an alarm notification.
        /// </summary>
        public bool FrontEndHardwareError
        {
            get
            {
                return m_value.CheckBits(Bits.Bit03);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit03) : m_value.ClearBits(Bits.Bit03);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.SensorReadError"/> should trigger an alarm notification.
        /// </summary>
        public bool SensorReadError
        {
            get
            {
                return m_value.CheckBits(Bits.Bit04);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit04) : m_value.ClearBits(Bits.Bit04);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.OpenThermocouple"/> should trigger an alarm notification.
        /// </summary>
        public bool OpenThermocouple
        {
            get
            {
                return m_value.CheckBits(Bits.Bit05);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit05) : m_value.ClearBits(Bits.Bit05);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.InputCountsOutOfSensorRange"/> should trigger an alarm notification.
        /// </summary>
        public bool InputCountsOutOfSensorRange
        {
            get
            {
                return m_value.CheckBits(Bits.Bit06);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit06) : m_value.ClearBits(Bits.Bit06);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.UnreasonableHigh"/> should trigger an alarm notification.
        /// </summary>
        public bool UnreasonableHigh
        {
            get
            {
                return m_value.CheckBits(Bits.Bit07);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit07) : m_value.ClearBits(Bits.Bit07);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.UnreasonableLow"/> should trigger an alarm notification.
        /// </summary>
        public bool UnreasonableLow
        {
            get
            {
                return m_value.CheckBits(Bits.Bit08);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit08) : m_value.ClearBits(Bits.Bit08);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.Old"/> should trigger an alarm notification.
        /// </summary>
        public bool Old
        {
            get
            {
                return m_value.CheckBits(Bits.Bit09);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit09) : m_value.ClearBits(Bits.Bit09);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.SuspectValueAboveHiHiLimit"/> should trigger an alarm notification.
        /// </summary>
        public bool SuspectValueAboveHiHiLimit
        {
            get
            {
                return m_value.CheckBits(Bits.Bit10);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit10) : m_value.ClearBits(Bits.Bit10);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.SuspectValueBelowLoLoLimit"/> should trigger an alarm notification.
        /// </summary>
        public bool SuspectValueBelowLoLoLimit
        {
            get
            {
                return m_value.CheckBits(Bits.Bit11);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit11) : m_value.ClearBits(Bits.Bit11);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.SuspectValueAboveHiLimit"/> should trigger an alarm notification.
        /// </summary>
        public bool SuspectValueAboveHiLimit
        {
            get
            {
                return m_value.CheckBits(Bits.Bit12);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit12) : m_value.ClearBits(Bits.Bit12);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.SuspectValueBelowLoLimit"/> should trigger an alarm notification.
        /// </summary>
        public bool SuspectValueBelowLoLimit
        {
            get
            {
                return m_value.CheckBits(Bits.Bit13);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit13) : m_value.ClearBits(Bits.Bit13);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.SuspectData"/> should trigger an alarm notification.
        /// </summary>
        public bool SuspectData
        {
            get
            {
                return m_value.CheckBits(Bits.Bit14);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit14) : m_value.ClearBits(Bits.Bit14);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.DigitalSuspectAlarm"/> should trigger an alarm notification.
        /// </summary>
        public bool DigitalSuspectAlarm
        {
            get
            {
                return m_value.CheckBits(Bits.Bit15);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit15) : m_value.ClearBits(Bits.Bit15);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.InsertedValueAboveHiHiLimit"/> should trigger an alarm notification.
        /// </summary>
        public bool InsertedValueAboveHiHiLimit
        {
            get
            {
                return m_value.CheckBits(Bits.Bit16);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit16) : m_value.ClearBits(Bits.Bit16);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.InsertedValueBelowLoLoLimit"/> should trigger an alarm notification.
        /// </summary>
        public bool InsertedValueBelowLoLoLimit
        {
            get
            {
                return m_value.CheckBits(Bits.Bit17);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit17) : m_value.ClearBits(Bits.Bit17);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.InsertedValueAboveHiLimit"/> should trigger an alarm notification.
        /// </summary>
        public bool InsertedValueAboveHiLimit
        {
            get
            {
                return m_value.CheckBits(Bits.Bit18);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit18) : m_value.ClearBits(Bits.Bit18);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.InsertedValueBelowLoLimit"/> should trigger an alarm notification.
        /// </summary>
        public bool InsertedValueBelowLoLimit
        {
            get
            {
                return m_value.CheckBits(Bits.Bit19);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit19) : m_value.ClearBits(Bits.Bit19);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.InsertedValue"/> should trigger an alarm notification.
        /// </summary>
        public bool InsertedValue
        {
            get
            {
                return m_value.CheckBits(Bits.Bit20);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit20) : m_value.ClearBits(Bits.Bit20);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.DigitalInsertedStatusInAlarm"/> should trigger an alarm notification.
        /// </summary>
        public bool DigitalInsertedStatusInAlarm
        {
            get
            {
                return m_value.CheckBits(Bits.Bit21);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit21) : m_value.ClearBits(Bits.Bit21);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.LogicalAlarm"/> should trigger an alarm notification.
        /// </summary>
        public bool LogicalAlarm
        {
            get
            {
                return m_value.CheckBits(Bits.Bit22);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit22) : m_value.ClearBits(Bits.Bit22);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.ValueAboveHiHiAlarm"/> should trigger an alarm notification.
        /// </summary>
        public bool ValueAboveHiHiAlarm
        {
            get
            {
                return m_value.CheckBits(Bits.Bit23);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit23) : m_value.ClearBits(Bits.Bit23);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.ValueBelowLoLoAlarm"/> should trigger an alarm notification.
        /// </summary>
        public bool ValueBelowLoLoAlarm
        {
            get
            {
                return m_value.CheckBits(Bits.Bit24);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit24) : m_value.ClearBits(Bits.Bit24);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.ValueAboveHiAlarm"/> should trigger an alarm notification.
        /// </summary>
        public bool ValueAboveHiAlarm
        {
            get
            {
                return m_value.CheckBits(Bits.Bit25);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit25) : m_value.ClearBits(Bits.Bit25);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.ValueBelowLoAlarm"/> should trigger an alarm notification.
        /// </summary>
        public bool ValueBelowLoAlarm
        {
            get
            {
                return m_value.CheckBits(Bits.Bit26);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit26) : m_value.ClearBits(Bits.Bit26);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.DeletedFromAlarmChecks"/> should trigger an alarm notification.
        /// </summary>
        public bool DeletedFromAlarmChecks
        {
            get
            {
                return m_value.CheckBits(Bits.Bit27);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit27) : m_value.ClearBits(Bits.Bit27);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.InhibitedByCutoutPoint"/> should trigger an alarm notification.
        /// </summary>
        public bool InhibitedByCutoutPoint
        {
            get
            {
                return m_value.CheckBits(Bits.Bit28);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit28) : m_value.ClearBits(Bits.Bit28);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether a data <see cref="Quality"/> of <see cref="Quality.Good"/> should trigger an alarm notification.
        /// </summary>
        public bool Good
        {
            get
            {
                return m_value.CheckBits(Bits.Bit29);
            }
            set
            {
                m_value = value ? m_value.SetBits(Bits.Bit29) : m_value.ClearBits(Bits.Bit29);
            }
        }

        /// <summary>
        /// Gets or sets the 32-bit integer value used for defining which data <see cref="Quality"/> should trigger an alarm notification.
        /// </summary>
        public int Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        #endregion
    }
}
