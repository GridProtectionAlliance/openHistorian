//******************************************************************************************************
//  Device.cs - Gbtc
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
//  01/21/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

#pragma warning disable 1591

using GSF.ComponentModel;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MAS.Model
{
    [PrimaryLabel("Acronym")]
    public class Device
    {
        // This magical attribute means you never have to lookup or provide
        // a NodeID for new Device records...
        [DefaultValueExpression("Global.NodeID")]
        public Guid NodeID { get; set; }

        [Label("Local Device ID")]
        [PrimaryKey(true)]
        public int ID { get; set; }

        public int? ParentID { get; set; }

        [Label("Unique Device ID")]
        [DefaultValueExpression("Guid.NewGuid()")]
        public Guid UniqueID { get; set; }

        [Required]
        [StringLength(200)]
        [AcronymValidation]
        [Searchable]
        public string Acronym { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [Label("Folder Name")]
        [StringLength(20)]
        public string OriginalSource { get; set; }

        [Label("Is Concentrator")]
        public bool IsConcentrator { get; set; }

        [Required]
        [Label("Company")]
        [DefaultValueExpression("Connection.ExecuteScalar(typeof(int), (object)null, 'SELECT ID FROM Company WHERE Acronym = {0}', Global.CompanyAcronym)", Cached = true)]
        public int? CompanyID { get; set; }

        [Label("Historian")]
        public int? HistorianID { get; set; }

        [Label("Access ID")]
        public int AccessID { get; set; }

        [Label("Vendor Device")]
        public int? VendorDeviceID { get; set; }

        [Label("Protocol")]
        public int? ProtocolID { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        [Label("Interconnection")]
        [InitialValueScript("1")]
        public int? InterconnectionID { get; set; }

        [Label("Connection String")]
        public string ConnectionString { get; set; }

        [StringLength(200)]
        public string TimeZone { get; set; }

        [Label("Frames Per Second")]
        [DefaultValue(30)]
        public int? FramesPerSecond { get; set; }

        public long TimeAdjustmentTicks { get; set; }

        [DefaultValue(5.0D)]
        public double DataLossInterval { get; set; }

        [DefaultValue(10)]
        public int AllowedParsingExceptions { get; set; }

        [DefaultValue(5.0D)]
        public double ParsingExceptionWindow { get; set; }

        [DefaultValue(5.0D)]
        public double DelayedConnectionInterval { get; set; }

        [DefaultValue(true)]
        public bool AllowUseOfCachedConfiguration { get; set; }

        [DefaultValue(true)]
        public bool AutoStartDataParsingSequence { get; set; }

        public bool SkipDisableRealTimeData { get; set; }

        [DefaultValue(100000)]
        public int MeasurementReportingInterval { get; set; }

        [Label("Connect On Demand")]
        [DefaultValue(true)]
        public bool ConnectOnDemand { get; set; }

        [Label("Contacts")]
        public string ContactList { get; set; }

        public int? MeasuredLines { get; set; }

        public int LoadOrder { get; set; }

        public bool Enabled { get; set; }

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy { get; set; }

        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn { get; set; }

        [Required]
        [StringLength(50)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy { get; set; }
    }
}