//******************************************************************************************************
//  StreamFilterBase'2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  2/2/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.SortedTreeStore.Filters
{
    public abstract class StreamFilterBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()

    {
        //protected TKey CurrentKey;
        //protected TValue CurrentValue;
       
        public long PointsFiltered;

        public abstract bool StopReading(TKey key, TValue value);

        ///// <summary>
        ///// Used to maintain the relationship that a stream's <see cref="CurrentKey"/> and <see cref="CurrentValue"/>
        ///// are always the same instance. Can also help having to copy the Key and Value parameters if in a nested stream reading case.
        ///// </summary>
        ///// <param name="key">the instance to use for <see cref="CurrentKey"/></param>
        ///// <param name="value">the instance to use for <see cref="CurrentValue"/></param>
        ///// <remarks>
        ///// Be weary of calling this function too often as assignment of classes in .NET requires a function call.
        ///// </remarks>
        //public void SetKeyValueReferences(TKey key, TValue value)
        //{
        //    CurrentKey = key;
        //    CurrentValue = value;
        //}
    }
}
