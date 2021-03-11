//*****************************************************************************************************
//  TreeStreamExtensions.cs - Gbtc
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
//  09/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************


namespace GSF.Snap
{
    /// <summary>
    /// 
    /// </summary>
    public static class TreeStreamExtensions
    {
        /// <summary>
        /// Parses an entire stream to count the number of points. Notice, this will
        /// enumerate the list and the list will have to be reset to be enumerated again.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="stream">The stream to enumerate</param>
        /// <returns>the number of items in the stream.</returns>
        public static long Count<TKey,TValue>(this TreeStream<TKey,TValue> stream)
            where TKey : class, new()
            where TValue : class, new()
        {
            TKey key = new TKey();
            TValue value = new TValue();
            long cnt=0;
            while (stream.Read(key, value))
                cnt++;
            return cnt;
        }

    }
}
