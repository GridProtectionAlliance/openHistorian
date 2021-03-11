//******************************************************************************************************
//  SeekFilterDefinitionBase.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  11/09/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.Snap.Filters;

namespace GSF.Snap.Definitions
{
    /// <summary>
    /// Has the ability to create a filter based on the key and the value.
    /// </summary>
    public abstract class SeekFilterDefinitionBase
    {
        /// <summary>
        /// The filter guid 
        /// </summary>
        public abstract Guid FilterType { get; }

        /// <summary>
        /// Determines if a Key/Value is contained in the filter
        /// </summary>
        /// <param name="stream">the value to check</param>
        /// <returns></returns>
        public abstract SeekFilterBase<TKey> Create<TKey>(BinaryStreamBase stream);

    }
}
