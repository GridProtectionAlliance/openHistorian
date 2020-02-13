//******************************************************************************************************
//  Common.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/12/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using GSF.TimeSeries;

namespace MAS
{
    /// <summary>
    /// Defines common functions.
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Gets derived quality flags from a set of source measurements.
        /// </summary>
        /// <param name="measurements">Source measurements.</param>
        /// <returns>Derived quality flags.</returns>
        public static MeasurementStateFlags DerivedQualityFlags(IEnumerable<IMeasurement> measurements)
        {
            MeasurementStateFlags derivedQuality = MeasurementStateFlags.Normal;
            MeasurementStateFlags[] stateFlags = measurements.Select(measurement => measurement.StateFlags).ToArray();

            bool dataQualityIsBad(MeasurementStateFlags flags) => (flags & MeasurementStateFlags.BadData) > 0;
            bool timeQualityIsBad(MeasurementStateFlags flags) => (flags & MeasurementStateFlags.BadTime) > 0;

            if (stateFlags.Any(dataQualityIsBad))
                derivedQuality |= MeasurementStateFlags.BadData;

            if (stateFlags.Any(timeQualityIsBad))
                derivedQuality |= MeasurementStateFlags.BadTime;

            return derivedQuality;
        }

        /// <summary>
        /// Gets derived quality flags from a set of value and time quality vectors.
        /// </summary>
        /// <param name="valueQualities">Boolean vector where flag determines if value quality is good.</param>
        /// <param name="timeQualities">Boolean vector where flag determines if time quality is good.</param>
        /// <returns>Derived quality flags.</returns>
        public static MeasurementStateFlags DerivedQualityFlags(IEnumerable<bool> valueQualities, IEnumerable<bool> timeQualities)
        {
            MeasurementStateFlags derivedQuality = MeasurementStateFlags.Normal;

            bool qualityIsBad(bool qualityIsGood) => !qualityIsGood;

            if (valueQualities.Any(qualityIsBad))
                derivedQuality |= MeasurementStateFlags.BadData;

            if (timeQualities.Any(qualityIsBad))
                derivedQuality |= MeasurementStateFlags.BadTime;

            return derivedQuality;
        }

        /// <summary>
        /// Gets derived quality flags from specified value and time quality.
        /// </summary>
        /// <param name="valueQualityIsGood">Flag that determines if value quality is good.</param>
        /// <param name="timeQualityIsGood">Flag that determines if time quality is good.</param>
        /// <returns>Derived quality flags.</returns>
        public static MeasurementStateFlags DerivedQualityFlags(bool valueQualityIsGood, bool timeQualityIsGood)
        {
            MeasurementStateFlags derivedQuality = MeasurementStateFlags.Normal;

            if (!valueQualityIsGood)
                derivedQuality |= MeasurementStateFlags.BadData;

            if (!timeQualityIsGood)
                derivedQuality |= MeasurementStateFlags.BadTime;

            return derivedQuality;
        }

        /// <summary>
        /// Gets a single column of data from a two dimensional data window.
        /// </summary>
        /// <param name="dataWindow">Target data window.</param>
        /// <param name="columnIndex">Index of column to return.</param>
        /// <returns>All values from <paramref name="columnIndex"/> in <paramref name="dataWindow"/>.</returns>
        public static IMeasurement[] GetDataColumn(this IMeasurement[,] dataWindow, int columnIndex)
        {
            if (dataWindow == null)
                throw new ArgumentNullException(nameof(dataWindow));

            if (dataWindow.Rank != 2)
                throw new ArgumentException("Data window must be two dimensional", nameof(dataWindow));

            if (columnIndex < 0 || columnIndex >= dataWindow.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(columnIndex));

            IEnumerable<IMeasurement> getDataColumn()
            {
                for (int i = 0; i < dataWindow.GetLength(1); i++)
                    yield return dataWindow[columnIndex, i];
            }

            return getDataColumn().ToArray();
        }
    }
}
