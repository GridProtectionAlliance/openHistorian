//******************************************************************************************************
//  IDatabaseReader`2.cs - Gbtc
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
//  03/01/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.Snap.Filters;
using GSF.Snap.Services.Reader;

namespace GSF.Snap
{

    /// <summary>
    /// An interface that is necessary for many of the transformations inside the SortedTreeStore to function.
    /// </summary>
    /// <typeparam name="TKey">A key type supported by the SortedTreeStore</typeparam>
    /// <typeparam name="TValue">A value type supported by the SortedTreeStore</typeparam>
    public interface IDatabaseReader<TKey, TValue> : IDisposable
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        /// <summary>
        /// Reads data from the SortedTreeEngine with the provided read options and server side filters.
        /// </summary>
        /// <param name="readerOptions">read options supplied to the reader. Can be null.</param>
        /// <param name="keySeekFilter">a seek based filter to follow. Can be null.</param>
        /// <param name="keyMatchFilter">a match based filer to follow. Can be null.</param>
        /// <returns>A stream that will read the specified data.</returns>
        TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter);

    }
}
