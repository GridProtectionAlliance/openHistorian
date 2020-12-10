//******************************************************************************************************
//  IaonOutputAdapter.cs - Gbtc
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
//  11/06/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class IaonOutputAdapter : IIaonAdapter
    {
        public Guid NodeID { get; set; }

        [PrimaryKey(true)]
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        public string AdapterName { get; set; }

        [Required]
        public string AssemblyName { get; set;  }

        [Required]
        [StringLength(200)]
        public string TypeName { get; set; }

        public string ConnectionString { get; set; }
    }
}
