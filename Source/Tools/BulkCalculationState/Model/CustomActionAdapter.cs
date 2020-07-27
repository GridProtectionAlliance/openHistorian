//******************************************************************************************************
//  CustomActionAdapter.cs - Gbtc
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
//  03/30/2017 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel;
using GSF.Data.Model;

namespace BulkCalculationState.Model
{
    public class CustomActionAdapter : IComparable<CustomActionAdapter>
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

        public int LoadOrder { get; set; }

        public bool Enabled { get; set; }

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy { get; set; }

        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn { get; set; }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy { get; set; }

        public int CompareTo(CustomActionAdapter other) =>
            string.Compare(AdapterName, other.AdapterName, StringComparison.OrdinalIgnoreCase);
    }

    public class FilteredActionAdapter : IComparable<FilteredActionAdapter>
    {
        public string AdapterName { get; set; }

        public bool Enabled { get; set; }

        public void SyncEnabledAdapterStates() => Adapters.ForEach(adapter => adapter.Enabled = Enabled);

        public List<CustomActionAdapter> Adapters = new List<CustomActionAdapter>();
        
        public int CompareTo(FilteredActionAdapter other) => 
            string.Compare(AdapterName, other.AdapterName, StringComparison.OrdinalIgnoreCase);
    }
}
