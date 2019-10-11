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
    [RootQueryRestriction("ID LIKE {0}", "PPA:%")]
    public class SNRMeasurment: ActiveMeasurement
    {
        [NonRecordField]
        public double Mean
        { get; set; }

        [NonRecordField]
        public double Min
        { get; set; }

        [NonRecordField]
        public double Max
        { get; set; }

        [NonRecordField]
        public double StandardDeviation
        { get; set; }

        [NonRecordField]
        public double NumAlarms
        { get; set; }

        [NonRecordField]
        public double PercentAlarms
        { get; set; }

        [NonRecordField]
        public double TimeAlarms
        { get; set; }

        [NonRecordField]
        public string OriginalTag
        {
            get
            {
                if (this.UnbalanceFlag == 0)
                {
                    return this.PointTag.Substring(0, this.PointTag.Length - 4);
                }
                else
                {
                    return this.PointTag.Substring(0, this.PointTag.Length - 5);
                }
            }
        }

        // 0 -> SNR
        // 1 -> I
        // 2 -> V
        public int UnbalanceFlag
        { get; set; }
    }

    [RootQueryRestriction("ID LIKE {0} AND SignalReference LIKE {1}", "PPA:%")]
    public class ReportMeasurements : ActiveMeasurement
    {

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

        public string OriginalTag
        {
            get; set;
        }
    }
}