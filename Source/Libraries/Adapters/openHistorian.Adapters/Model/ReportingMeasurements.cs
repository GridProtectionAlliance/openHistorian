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
using GSF.TimeSeries;

namespace openHistorian.Model
{

    
    public class ReportMeasurements 
    {
        

        public ReportMeasurements()
        {}

        public ReportMeasurements(ActiveMeasurement activemeasurement)
        {
            this.ID = activemeasurement.ID;
            this.Source = activemeasurement.Source;
            this.PointID = activemeasurement.PointID;
            this.SignalID = (Guid)activemeasurement.SignalID;
            this.PointTag = activemeasurement.PointTag;
            this.SignalReference = activemeasurement.SignalReference;
            this.FramesPerSecond = activemeasurement.FramesPerSecond;
           

            this.Mean = 0;
            this.Min = 0;
            this.StandardDeviation = 0;
            this.NumberOfAlarms = 0;
            this.PercentAlarms = 0;
            this.TimeInAlarm = 0;
        }

        [Searchable]
        [CSVExcludeField]
        public string ID
        {  get; set;   }



        [PrimaryKey(false)]
        [Label("Unique Signal ID")]
        [CSVExcludeField]
        public Guid SignalID
        {
            get;
            set;
        }

        [Label("Tag Name")]
        [Required]
        [StringLength(200)]
        [Searchable]
        public string PointTag
        {
            get;
            set;
        }

       
        [Label("Signal Reference")]
        [Required]
        [StringLength(200)]
        [Searchable]
        [CSVExcludeField]
        public string SignalReference
        {
            get;
            set;
        }

        [Label("Frames per Second")]
        [CSVExcludeField]
        public int? FramesPerSecond
        {
            get;
            set;
        }

        [NonRecordField]
        public string Source
        { get; set; }

        [NonRecordField]
        public ulong PointID
        { get; set; }

        // Fields populated by historian data
        public double Mean
        { get; set; }

        public double Min
        { get; set; }

        public double Max
        { get; set; }

        public double StandardDeviation
        { get; set; }

        public double NumberOfAlarms
        { get; set; }

        public double PercentAlarms
        { get; set; }

        public double TimeInAlarm
        { get; set; }

    }
}