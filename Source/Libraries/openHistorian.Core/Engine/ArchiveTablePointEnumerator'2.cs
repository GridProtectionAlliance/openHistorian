//******************************************************************************************************
//  ArchiveTablePointEnumerator'2.cs - Gbtc
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
//  10/25/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian.Engine
{
    public class ArchiveTablePointEnumerator<TKey, TValue>
        : SeekableKeyValueStream<TKey, TValue>, IDisposable
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {

        ArchiveTableSummary<TKey, TValue> m_table;
        ArchiveTableReadSnapshot<TKey, TValue> m_snapshot;
        SeekableKeyValueStream<TKey, TValue> m_scanner;

        /// <summary>
        /// An index value that is used to disassociate the archive file. Passed to this class from the <see cref="ArchiveReaderSequential"/>
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Creates the table reader.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="table"></param>
        public ArchiveTablePointEnumerator(int index, ArchiveTableSummary<TKey, TValue> table)
        {
            Index = index;
            m_table = table;
            m_snapshot = m_table.ActiveSnapshotInfo.CreateReadSnapshot();
            m_scanner = m_snapshot.GetTreeScanner();
            SetKeyValueReferences(m_scanner.CurrentKey, m_scanner.CurrentValue);
            IsValid = false;
        }

        public override bool Read()
        {
            IsValid = m_scanner.Read();
            return IsValid;
        }
        public override void SeekToKey(TKey key)
        {
            m_scanner.SeekToKey(key);
            IsValid = false;
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
