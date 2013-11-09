//******************************************************************************************************
//  HistorianDataReaderBase.cs - Gbtc
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
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.SortedTreeStore;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;

namespace openHistorian
{
    /// <summary>
    /// Creates a session that can read data from the historian.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class HistorianDataReaderBase<TKey, TValue>
        : IDisposable
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, new()
    {
        /// <summary>
        /// Reads data from the historian with the provided filters.
        /// </summary>
        /// <param name="timestampFilter">filters for the timestamp</param>
        /// <param name="pointIdFilter">filters for the pointId</param>
        /// <param name="readerOptions">options for the reader, such as automatic timeouts.</param>
        /// <returns></returns>
        public abstract TreeStream<TKey, TValue> Read(QueryFilterTimestamp timestampFilter, QueryFilterPointId pointIdFilter, DataReaderOptions readerOptions);

        /// <summary>
        /// Closes this reader
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();
    }
}