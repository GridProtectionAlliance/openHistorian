//******************************************************************************************************
//  MatchFilterBase`2.cs - Gbtc
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

namespace GSF.Snap.Filters
{
    /// <summary>
    /// Represents some kind of filter that does a match based on the key/value.
    /// </summary>
    /// <typeparam name="TKey">the key to match</typeparam>
    /// <typeparam name="TValue">the value to match</typeparam>
    public abstract class MatchFilterBase<TKey, TValue>
    {
        /// <summary>
        /// The filter guid 
        /// </summary>
        public abstract Guid FilterType { get; }

        /// <summary>
        /// Serializes the filter to a stream
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public abstract void Save(BinaryStreamBase stream);

        /// <summary>
        /// Determines if a Key/Value is contained in the filter
        /// </summary>
        /// <param name="key">the key to check</param>
        /// <param name="value">the value to check</param>
        /// <returns></returns>
        public abstract bool Contains(TKey key, TValue value);
    }
}
