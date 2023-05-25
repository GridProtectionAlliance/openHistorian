//******************************************************************************************************
//  EventMarker.cs - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
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
//  05/24/2023 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

// ReSharper disable CheckNamespace
#pragma warning disable 1591

using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model
{
    public class EventMarker
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        public int? ParentID { get; set; }

        [Label("Event Source")]
        [Required]
        [StringLength(200)]
        [Searchable]
        public string Source { get; set; }

        [Label("Start Time")]
        public DateTime? StartTime { get; set; }

        [Label("Stop Time")]
        public DateTime? StopTime { get; set; }

        [Searchable]
        public string Notes { get; set; }
    }
}
