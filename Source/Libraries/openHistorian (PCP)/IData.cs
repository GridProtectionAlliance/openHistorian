//******************************************************************************************************
//  IData.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  02/24/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  03/12/2010 - Pinal C. Patel
//       Updated to inherit from IComparable and IFormattable interfaces.
//
//******************************************************************************************************

using openHistorian.Archives;
using TVA.Parsing;

namespace openHistorian
{
    #region [ Enumerations ]

    /// <summary>
    /// Indicates the quality of time-series data.
    /// </summary>
    public enum Quality
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// DeletedFromProcessing
        /// </summary>
        DeletedFromProcessing,
        /// <summary>
        /// CouldNotCalculate
        /// </summary>
        CouldNotCalculate,
        /// <summary>
        /// FrontEndHardwareError
        /// </summary>
        FrontEndHardwareError,
        /// <summary>
        /// SensorReadError
        /// </summary>
        SensorReadError,
        /// <summary>
        /// OpenThermocouple
        /// </summary>
        OpenThermocouple,
        /// <summary>
        /// InputCountsOutOfSensorRange
        /// </summary>
        InputCountsOutOfSensorRange,
        /// <summary>
        /// UnreasonableHigh
        /// </summary>
        UnreasonableHigh,
        /// <summary>
        /// UnreasonableLow
        /// </summary>
        UnreasonableLow,
        /// <summary>
        /// Old
        /// </summary>
        Old,
        /// <summary>
        /// SuspectValueAboveHiHiLimit
        /// </summary>
        SuspectValueAboveHiHiLimit,
        /// <summary>
        /// SuspectValueBelowLoLoLimit
        /// </summary>
        SuspectValueBelowLoLoLimit,
        /// <summary>
        /// SuspectValueAboveHiLimit
        /// </summary>
        SuspectValueAboveHiLimit,
        /// <summary>
        /// SuspectValueBelowLoLimit
        /// </summary>
        SuspectValueBelowLoLimit,
        /// <summary>
        /// SuspectData
        /// </summary>
        SuspectData,
        /// <summary>
        /// DigitalSuspectAlarm
        /// </summary>
        DigitalSuspectAlarm,
        /// <summary>
        /// InsertedValueAboveHiHiLimit
        /// </summary>
        InsertedValueAboveHiHiLimit,
        /// <summary>
        /// InsertedValueBelowLoLoLimit
        /// </summary>
        InsertedValueBelowLoLoLimit,
        /// <summary>
        /// InsertedValueAboveHiLimit
        /// </summary>
        InsertedValueAboveHiLimit,
        /// <summary>
        /// InsertedValueBelowLoLimit
        /// </summary>
        InsertedValueBelowLoLimit,
        /// <summary>
        /// InsertedValue
        /// </summary>
        InsertedValue,
        /// <summary>
        /// DigitalInsertedStatusInAlarm
        /// </summary>
        DigitalInsertedStatusInAlarm,
        /// <summary>
        /// LogicalAlarm
        /// </summary>
        LogicalAlarm,
        /// <summary>
        /// ValueAboveHiHiAlarm
        /// </summary>
        ValueAboveHiHiAlarm,
        /// <summary>
        /// ValueBelowLoLoAlarm
        /// </summary>
        ValueBelowLoLoAlarm,
        /// <summary>
        /// ValueAboveHiAlarm
        /// </summary>
        ValueAboveHiAlarm,
        /// <summary>
        /// ValueBelowLoAlarm
        /// </summary>
        ValueBelowLoAlarm,
        /// <summary>
        /// DeletedFromAlarmChecks
        /// </summary>
        DeletedFromAlarmChecks,
        /// <summary>
        /// InhibitedByCutoutPoint
        /// </summary>
        InhibitedByCutoutPoint,
        /// <summary>
        /// Good
        /// </summary>
        Good
    }

    #endregion

    /// <summary>
    /// Represents time-series data stored in an <see cref="IDataArchive"/>.
    /// </summary>
    /// <seealso cref="TimeTag"/>
    /// <seealso cref="Quality"/>
    public interface IData : ISupportBinaryImage
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="DataKey"/> of the event.
        /// </summary>
        DataKey Key { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of the event.
        /// </summary>
        TimeTag Time { get; set; }

        /// <summary>
        /// Gets or sets the value <see cref="object"/> of the event.
        /// </summary>
        float Value { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Quality"/> of the event's <see cref="Value"/>.
        /// </summary>
        Quality Quality { get; set; }

        #endregion
    }
}
