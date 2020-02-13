//******************************************************************************************************
//  SignalType.cs - Gbtc
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
//  01/22/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

#pragma warning disable 1591

namespace MAS.Model
{
    public class SignalType
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        [Searchable]
        public string Name { get; set; }

        [Required]
        [StringLength(4)]
        [Searchable]
        public string Acronym { get; set; }

        [Required]
        [StringLength(2)]
        public string Suffix { get; set; }

        [Required]
        [StringLength(2)]
        public string Abbreviation { get; set; }

        [Required]
        [StringLength(200)]
        [Searchable]
        public string LongAcronym { get; set; }

        [Required]
        [StringLength(10)]
        public string Source { get; set; }

        [StringLength(10)]
        public string EngineeringUnits { get; set; }
    }
}