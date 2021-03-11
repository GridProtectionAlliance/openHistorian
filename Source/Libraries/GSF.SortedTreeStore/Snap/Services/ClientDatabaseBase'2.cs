//******************************************************************************************************
//  ClientDatabaseBase`2.cs - Gbtc
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
//  12/08/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using GSF.Snap.Filters;
using GSF.Snap.Services.Reader;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Represents a single historian database.
    /// </summary>
    public abstract class ClientDatabaseBase<TKey, TValue>
        : ClientDatabaseBase, IDatabaseReader<TKey, TValue> 
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
        public abstract TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter);

        /// <summary>
        /// Writes the tree stream to the database. 
        /// </summary>
        /// <param name="stream">all of the key/value pairs to add to the database.</param>
        public abstract void Write(TreeStream<TKey, TValue> stream);

        /// <summary>
        /// Writes an individual key/value to the sorted tree store.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void Write(TKey key, TValue value);
      
    }
}