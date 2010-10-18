//******************************************************************************************************
//  IDataPoint.cs - Gbtc
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
//  02/24/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  03/12/2010 - Pinal C. Patel
//       Updated to inherit from IComparable and IFormattable interfaces.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
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
    /// Defines time-series data warehoused by a historian.
    /// </summary>
    /// <seealso cref="TimeTag"/>
    /// <seealso cref="Quality"/>
    public interface IDataPoint : ISupportBinaryImage, IComparable, IFormattable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the historian identifier of the time-series data point.
        /// </summary>
        int HistorianID { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of the time-series data point.
        /// </summary>
        TimeTag Time { get; set; }

        /// <summary>
        /// Gets or sets the value of the time-series data point.
        /// </summary>
        float Value { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Quality"/> of the time-series data point.
        /// </summary>
        Quality Quality { get; set; }

        #endregion
    }
}
