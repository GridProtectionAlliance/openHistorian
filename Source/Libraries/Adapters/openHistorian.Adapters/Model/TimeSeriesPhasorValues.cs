//******************************************************************************************************
//  TimeSeriesPhasorValues.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/06/2017 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Generic;

namespace openHistorian.Adapters.Model
{
    /// <summary>
    /// Defines a Grafana time-series values.
    /// </summary>
    /// <remarks>
    /// This structure is serialized and returned to Grafana via JSON.
    /// </remarks>
    public class TimeSeriesPhasorValues
    {
        /// <summary>
        /// Data point index for value.
        /// </summary>
        public const int Value = 0;

        /// <summary>
        /// Data point index for time.
        /// </summary>
        public const int Time = 1;

        /// <summary>
        /// Defines a Grafana time-series value point source.
        /// </summary>
        public string target;

        /// <summary>
        /// Defines a Grafana time-series underlying point tag.
        /// </summary>
        public string pointtag;

        /// <summary>
        /// Defines a Grafana time-series underlying device latitude.
        /// </summary>
        public string latitude;

        /// <summary>
        /// Defines a Grafana time-series underlying device longitude.
        /// </summary>
        public string longitude;

        /// <summary>
        /// Defines a Grafana time-series underlying device latitude.
        /// </summary>
        public string tolatitude;

        /// <summary>
        /// Defines a Grafana time-series underlying device longitude.
        /// </summary>
        public string tolongitude;

        /// <summary>
        /// Defines a Grafana time-series underlying pointtag of the down line device.
        /// </summary>
        public string todevicepointtag;

        /// <summary>
        /// Defines a Grafana time-series underlying pointtag of the down line device.
        /// </summary>
        public string powerpointtag;

        /// <summary>
        /// Defines a Grafana time-series underlying pointtag of the down line device.
        /// </summary>
        public string anglepointtag;

        /// <summary>
        /// Defines a Grafana time-series underlying pointtag of the down line device.
        /// </summary>
        public string magpointtag;

        /// <summary>
        /// Defines a Grafana time-series underlying pointtag of the down line device.
        /// </summary>
        public string toanglepointtag;

        /// <summary>
        /// Defines a Grafana time-series phasor magnitude value data.
        /// </summary>
        /// <remarks>
        /// "datapoints":[
        ///       [622,1450754160000],
        ///       [365,1450754220000]
        /// ]
        /// </remarks>
        public List<double[]> magdatapoints;

        /// <summary>
        /// Defines a Grafana time-series phasor last magnitude value.
        /// </summary>
        public double magvalue;


        /// <summary>
        /// Defines a Grafana time-series phasor angle value data.
        /// </summary>
        /// <remarks>
        /// "datapoints":[
        ///       [622,1450754160000],
        ///       [365,1450754220000]
        /// ]
        /// </remarks>
        public List<double[]> angledatapoints;


        /// <summary>
        /// Defines a Grafana time-series phasor last angle value.
        /// </summary>
        public double anglevalue;

        /// <summary>
        /// Defines a Grafana time-series phasor last angle value.
        /// </summary>
        public double maxanglevalue;
        /// <summary>
        /// Defines a Grafana time-series phasor last angle value.
        /// </summary>
        public double minanglevalue;


        /// <summary>
        /// Defines a Grafana time-series phasor line power value data.
        /// </summary>
        /// <remarks>
        /// "datapoints":[
        ///       [622,1450754160000],
        ///       [365,1450754220000]
        /// ]
        /// </remarks>
        public List<double[]> powerdatapoints;


        /// <summary>
        /// Defines a Grafana time-series phasor last power value.
        /// </summary>
        public double powervalue;

    }
}
