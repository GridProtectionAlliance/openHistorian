//******************************************************************************************************
//  SortedTreeEngineBase`2.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Server.Reader;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Server
{
    /// <summary>
    /// Represents a single historian database.
    /// </summary>
    public abstract class SortedTreeEngineBase<TKey, TValue> 
        : SortedTreeEngineBase, IDatabaseReader<TKey,TValue> 
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
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