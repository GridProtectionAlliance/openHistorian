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
        : IDisposable
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : class, new()
    {

        ArchiveTableSummary<TKey, TValue> m_table;
        ArchiveTableReadSnapshot<TKey, TValue> m_snapshot;
        SeekableKeyValueStream<TKey, TValue> m_scanner;

        /// <summary>
        /// An index value that is used to disassociate the archive file. Passed to this class from the <see cref="ArchiveReaderSequential"/>
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// True if <see cref="CurrentKey"/> and <see cref="CurrentValue"/> contains valid data
        /// </summary>
        public bool IsValid { get; private set; }
        /// <summary>
        /// The current key. Only valid if <see cref="IsValid"/> is true.
        /// </summary>
        public TKey CurrentKey { get; private set; }
        /// <summary>
        /// The current value. Only valid if <see cref="IsValid"/> is true.
        /// </summary>
        public TValue CurrentValue { get; private set; }

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
            CurrentKey = m_scanner.CurrentKey;
            CurrentValue = m_scanner.CurrentValue;
            IsValid = false;
        }

        /// <summary>
        /// Seeks the provided time window and populates the first key/value.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="stopTime"></param>
        /// <returns>True if this time window is valid for this ArchiveTable. False otherwise. </returns>
        public bool SeekToTimeWindow(ulong startTime, ulong stopTime)
        {
            if (!m_table.Contains(startTime, stopTime))
            {
                IsValid = false;
                return false;
            }

            CurrentKey.SetMin();
            CurrentKey.Timestamp = startTime;
            m_scanner.SeekToKey(CurrentKey);
            if (m_scanner.Read())
            {
                if (CurrentKey.Timestamp > stopTime)
                {
                    IsValid = false;
                    return false;
                }
                IsValid = true;
                return true;
            }
            IsValid = false;
            return false;
        }

        /// <summary>
        /// Advances to the next key/value pair.
        /// </summary>
        /// <returns>True if the advance was successful, false otherwise</returns>
        public bool ReadNext()
        {
            IsValid = m_scanner.Read();
            return IsValid;
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
