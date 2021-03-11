//******************************************************************************************************
//  SeekableTreeStream'2.cs - Gbtc
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
//  03/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace GSF.Snap
{

    /// <summary>
    /// Provides ability to seek and stream KeyValues.
    /// </summary>
    /// <typeparam name="TKey">The key of the pair</typeparam>
    /// <typeparam name="TValue">The value of the pair</typeparam>
    public abstract class SeekableTreeStream<TKey, TValue>
        : TreeStream<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {

        /// <summary>
        /// Seeks the stream to the first value greater than or equal to <see cref="key"/>
        /// </summary>
        /// <param name="key">the key to seek to.</param>
        public abstract void SeekToKey(TKey key);
    }
}