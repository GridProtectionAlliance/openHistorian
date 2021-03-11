//******************************************************************************************************
//  BufferedArchiveStream'2.cs - Gbtc
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
//  10/25/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.Snap.Storage;
using GSF.Snap.Tree;

namespace GSF.Snap.Services.Reader
{
    public class BufferedArchiveStream<TKey, TValue>
        : IDisposable
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        public SortedTreeScannerBase<TKey, TValue> Scanner;
        private readonly ArchiveTableSummary<TKey, TValue> m_table;
        private SortedTreeTableReadSnapshot<TKey, TValue> m_snapshot;

        /// <summary>
        /// An index value that is used to disassociate the archive file. Passed to this class from the <see cref="SortedTreeEngineReaderSequential{TKey,TValue}"/>
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Creates the table reader.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="table"></param>
        public BufferedArchiveStream(int index, ArchiveTableSummary<TKey, TValue> table)
        {
            Index = index;
            m_table = table;
            m_snapshot = m_table.ActiveSnapshotInfo.CreateReadSnapshot();
            Scanner = m_snapshot.GetTreeScanner();
        }

        public bool CacheIsValid;
        public TKey CacheKey = new TKey();
        public TValue CacheValue = new TValue();

        public void UpdateCachedValue()
        {
            CacheIsValid = Scanner.Peek(CacheKey, CacheValue);
        }

        public void SkipToNextKeyAndUpdateCachedValue()
        {
            CacheIsValid = Scanner.Read(CacheKey, CacheValue);
            CacheIsValid = Scanner.Peek(CacheKey, CacheValue);
        }

        public void SeekToKeyAndUpdateCacheValue(TKey key)
        {
            Scanner.SeekToKey(key);
            CacheIsValid = Scanner.Peek(CacheKey, CacheValue);
        }

        public void Dispose()
        {
            if (m_snapshot != null)
            {
                m_snapshot.Dispose();
                m_snapshot = null;
            }
        }
    }
}
