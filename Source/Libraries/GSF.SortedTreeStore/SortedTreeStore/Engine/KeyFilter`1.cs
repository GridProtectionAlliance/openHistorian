//******************************************************************************************************
//  KeyFilter`1.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  11/9/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.SortedTreeStore.Engine.Reader;
using GSF.SortedTreeStore.Filters;

namespace GSF.SortedTreeStore.Engine
{
    /// <summary>
    /// Filters that apply for the keys
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class KeyFilter<TKey>
    {
        /// <summary>
        /// A filter that has a seek like characteristic. Specify null to not apply a filter.
        /// </summary>
        public KeySeekFilterBase<TKey> KeySeekFilter;
        /// <summary>
        /// A fitler that looks at the key on a 1 by 1 basis for applying the filter. Specify null for no filter.
        /// </summary>
        public KeyMatchFilterBase<TKey> KeyMatchFilter;

        /// <summary>
        /// Unions the provided two classes
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static KeyFilter<TKey> operator |(KeyFilter<TKey> left, KeyFilter<TKey> right)
        {
            if (left.KeySeekFilter != null && right.KeySeekFilter != null)
                throw new Exception("Class is overdefined. Only 1 class can have KeySeekFilter defined");
            if (left.KeyMatchFilter != null && right.KeyMatchFilter != null)
                throw new Exception("Class is overdefined. Only 1 class can have KeyMatchFilter defined");

            var rv = new KeyFilter<TKey>();
            rv.KeySeekFilter = left.KeySeekFilter;
            rv.KeyMatchFilter = left.KeyMatchFilter;

            if (rv.KeySeekFilter == null)
                rv.KeySeekFilter = right.KeySeekFilter;
            if (rv.KeyMatchFilter == null)
                rv.KeyMatchFilter = right.KeyMatchFilter;
            return rv;
        }

    }
}
