//******************************************************************************************************
//  TimestampSeekFilterDefinition.cs - Gbtc
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
using System.Reflection;
using GSF.IO;
using GSF.Snap.Definitions;

namespace GSF.Snap.Filters
{
    public class TimestampSeekFilterDefinition
        : SeekFilterDefinitionBase
    {

        // {0F0F9478-DC42-4EEF-9F26-231A942EF1FA}
        public static Guid FilterGuid = new Guid(0x0f0f9478, 0xdc42, 0x4eef, 0x9f, 0x26, 0x23, 0x1a, 0x94, 0x2e, 0xf1, 0xfa);

        public override Guid FilterType => FilterGuid;

        public override SeekFilterBase<TKey> Create<TKey>(BinaryStreamBase stream)
        {
            MethodInfo method = typeof(TimestampSeekFilter).GetMethod("CreateFromStream", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(typeof(TKey));
            object rv = generic.Invoke(null, new[] { stream });
            return (SeekFilterBase<TKey>)rv;
        }

    }
}
