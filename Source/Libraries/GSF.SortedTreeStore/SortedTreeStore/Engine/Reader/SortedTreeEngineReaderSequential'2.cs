//******************************************************************************************************
//  SortedTreeEngineReaderSequential'2.cs - Gbtc
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
//  10/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Reader
{
    /// <summary>
    /// A <see cref="SortedTreeEngineReaderBase{TKey,TValue}"/> that can read from a <see cref="ArchiveList{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class SortedTreeEngineReaderSequential<TKey, TValue>
        : SortedTreeEngineReaderBase<TKey, TValue>
        where TKey : EngineKeyBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private readonly ArchiveList<TKey, TValue> m_list;
        private readonly ArchiveListSnapshot<TKey, TValue> m_snapshot;

        public SortedTreeEngineReaderSequential(ArchiveList<TKey, TValue> list)
        {
            m_list = list;
            m_snapshot = m_list.CreateNewClientResources();
        }

        ///// <summary>
        ///// Reads data from the historian with the provided filters.
        ///// </summary>
        ///// <param name="timestampFilter">filters for the timestamp</param>
        ///// <param name="pointIdFilter">filters for the pointId</param>
        ///// <param name="readerOptions">options for the reader, such as automatic timeouts.</param>
        ///// <returns></returns>
        //public override TreeStream<TKey, TValue> Read(QueryFilterTimestamp timestampFilter, QueryFilterPointId pointIdFilter, SortedTreeEngineReaderOptions readerOptions)
        //{
        //    Stats.QueriesExecuted++;
        //    return new ReadStream(timestampFilter, pointIdFilter, m_snapshot, readerOptions);
        //}

        public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, KeySeekFilterBase<TKey> keySeekFilter,
                                        KeyMatchFilterBase<TKey> keyMatchFilter, ValueMatchFilterBase<TValue> valueMatchFilter)
        {
            if (readerOptions == null)
                readerOptions = SortedTreeEngineReaderOptions.Default;
            if (keySeekFilter == null)
                keySeekFilter = new KeySeekFilterUniverse<TKey>();
            if (keyMatchFilter == null)
                keyMatchFilter = new KeyMatchFilterUniverse<TKey>();
            if (valueMatchFilter == null)
                valueMatchFilter = new ValueMatchFilterUniverse<TValue>();

            Stats.QueriesExecuted++;
            return new SequentialReaderStream<TKey, TValue>(m_snapshot, readerOptions, keySeekFilter, keyMatchFilter, valueMatchFilter);
        }

        /// <summary>
        /// Closes the current reader.
        /// </summary>
        public override void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            m_snapshot.Dispose();
        }
    }
}