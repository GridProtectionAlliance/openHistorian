//******************************************************************************************************
//  ReportingMeasurements.cs - Gbtc
//
//  Copyright © 2019, Grid Protection Alliance.  All Rights Reserved.
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
//  10/08/2019 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class ReportMeasurements 
    {
        public ReportMeasurements()
        {
        }


        [Label("Device")]
        public string DeviceName
        {
            get;
            set;
        }

        [Label("Tag Name")]
        [Required]
        [StringLength(200)]
        public string PointTag
        {
            get;
            set;
        }


        [PrimaryKey(false)]
        [Label("Unique Signal ID")]
        [CSVExcludeField]
        public Guid SignalID
        {
            get;
            set;
        }


        [Label("Signal Reference")]
        [Required]
        [StringLength(200)]
        [CSVExcludeField]
        public string SignalReference
        {
            get;
            set;
        }

        [Label("Signal Type")]
        public string SignalType
        {
            get;
            set;
        }

       

        

        // Fields populated by historian data
        public double Mean
        {
            get; 
            set;
        }

        public double Min
        {
            get;
            set;
        }

        public double Max
        {
            get;
            set;
        }

        public double StandardDeviation
        {
            get; 
            set;
        }

        public int AlarmCount
        {
            get;
            set;
        }

        public double PercentInAlarm
        {
            get;
            set;
        }

    }
}