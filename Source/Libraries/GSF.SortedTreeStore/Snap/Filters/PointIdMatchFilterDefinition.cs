//******************************************************************************************************
//  PointIdMatchFilterDefinition.cs - Gbtc
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
    public class PointIdMatchFilterDefinition
        : MatchFilterDefinitionBase
    {
        // {2034A3E3-F92E-4749-9306-B04DC36FD743}
        public static Guid FilterGuid = new Guid(0x2034a3e3, 0xf92e, 0x4749, 0x93, 0x06, 0xb0, 0x4d, 0xc3, 0x6f, 0xd7, 0x43);

        public override Guid FilterType => FilterGuid;

        public override MatchFilterBase<TKey, TValue> Create<TKey, TValue>(BinaryStreamBase stream)
        {
            MethodInfo method = typeof(PointIdMatchFilter).GetMethod("CreateFromStream", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(typeof(TKey), typeof(TValue));
            object rv = generic.Invoke(null, new[] { stream });
            return (MatchFilterBase<TKey, TValue>)rv;
        }
    }
}
