//******************************************************************************************************
//  StreamEncodingBaseDefinition.cs - Gbtc
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
//  08/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using GSF.Snap.Encoding;

namespace GSF.Snap.Definitions
{
    /// <summary>
    /// A base class that allows for generically constructing any number of <see cref="T:openHistorian.Collections.Generic.TreeNodeBase`2"/> implementations.
    /// </summary>
    public abstract class StreamEncodingBaseDefinition
        : CombinedEncodingDictionaryType
    {

        /// <summary>
        /// Creates a compressed Stream
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public abstract StreamEncodingBase<TKey, TValue> Create<TKey, TValue>()
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new();
    }
}