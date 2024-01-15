//******************************************************************************************************
//  DataSourcePhasorValueGroup.cs - Gbtc
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

// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System.Collections.Generic;
using GrafanaAdapters.DataSources.BuiltIn;

namespace openHistorian.Model
{
    /// <summary>
    /// Defines a class that represents an enumeration of <see cref="DataSourceValue"/> for a given target.
    /// </summary>
    /// <remarks>
    /// This is a group construct keyed on <see cref="Target"/> for data source value enumerations.
    /// </remarks>
    public class DataSourcePhasorValueGroup
    {
        /// <summary>
        /// Defines the name of phasor.
        /// </summary>
        public string Target;

        /// <summary>
        /// Defines the ActivePhasors defined ID for the phasor.
        /// </summary>
        public int PhasorID;

        /// <summary>
        /// Defines the ActivePhasors defined latitude for the phasor device location.
        /// </summary>
        public string Latitude;

        /// <summary>
        /// Defines the ActivePhasors defined longitude for the phasor device location.
        /// </summary>
        public string Longitude;

        /// <summary>
        /// Defines the ActivePhasors defined latitude for the phasor device location.
        /// </summary>
        public string ToLatitude;

        /// <summary>
        /// Defines the ActivePhasors defined longitude for the phasor device location.
        /// </summary>
        public string ToLongitude;

        /// <summary>
        /// Defines the ActivePhasors defined destination phasor tag for the phasor.
        /// </summary>
        public string DestinationPhasorLabel;

        /// <summary>
        /// Defines the ActivePhasors defined destination phasor tag for the phasor.
        /// </summary>
        public string ReferenceAngle;

        /// <summary>
        /// Query target, e.g., a point-tag, representative of all <see cref="MagnitudeSource"/> values.
        /// </summary>
        public string MagnitudeTarget;

        /// <summary>
        /// Query target, e.g., a point-tag, representative of all <see cref="AngleSource"/> values.
        /// </summary>
        public string AngleTarget;
        /// <summary>
        /// Query target, e.g., a point-tag, representative of all <see cref="AngleSource"/> values.
        /// </summary>
        public string ToAngleTarget;

        /// <summary>
        /// Query target, e.g., a point-tag, representative of all <see cref="PowerSource"/> values.
        /// </summary>
        public string PowerTarget;


        /// <summary>
        /// Data source magnitude values enumerable.
        /// </summary>
        public IEnumerable<DataSourceValue> MagnitudeSource;

        /// <summary>
        /// Data source angle values enumerable.
        /// </summary>
        public IEnumerable<DataSourceValue> AngleSource;

        /// <summary>
        /// Data source angle values enumerable.
        /// </summary>
        public IEnumerable<DataSourceValue> PowerSource;

    }
}
