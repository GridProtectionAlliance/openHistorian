//******************************************************************************************************
//  TimestampPointIDSeekFilterDefinition.cs - Gbtc
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
//  11/26/2014 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Reflection;
using GSF.IO;
using GSF.Snap.Definitions;

namespace GSF.Snap.Filters
{
    /// <summary>
    /// Represents a seek filter definition for the <see cref="TimestampPointIDSeekFilter"/> methods.
    /// </summary>
    public class TimestampPointIDSeekFilterDefinition
        : SeekFilterDefinitionBase
    {
        /// <summary>
        /// Guid for the <see cref="TimestampPointIDSeekFilterDefinition"/>.
        /// </summary>
        // {8E0A841F-03C3-4B55-87CB-9F9A732F86DC}
        public static Guid FilterGuid = new Guid(0x8E0A841F, 0x03C3, 0x4B55, 0x87, 0xCB, 0x9F, 0x9A, 0x73, 0x2F, 0x86, 0xDC);

        /// <summary>
        /// Gets the filter type Guid for the <see cref="TimestampPointIDSeekFilterDefinition"/>.
        /// </summary>
        public override Guid FilterType => FilterGuid;

        /// <summary>
        /// Creates a new seek filter for the <see cref="TimestampPointIDSeekFilterDefinition"/>.
        /// </summary>
        /// <typeparam name="TKey">Type of key/</typeparam>
        /// <param name="stream">Binary stream to create seek filter from.</param>
        /// <returns>New seek filter based on information in binary stream.</returns>
        public override SeekFilterBase<TKey> Create<TKey>(BinaryStreamBase stream)
        {
            MethodInfo method = typeof(TimestampPointIDSeekFilter).GetMethod("CreateFromStream", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(typeof(TKey));
            object rv = generic.Invoke(null, new[] { stream });
            return (SeekFilterBase<TKey>)rv;
        }
    }
}
