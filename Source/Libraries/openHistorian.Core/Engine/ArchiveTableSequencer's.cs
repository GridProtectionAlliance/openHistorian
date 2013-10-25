//******************************************************************************************************
//  ArchiveTableSequencer'2.cs - Gbtc
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

using System.Collections.Generic;
using openHistorian.Collections;

namespace openHistorian.Engine
{
    public class ArchiveTableSequencer<TKey, TValue>
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : HistorianValueBase<TValue>, new()
    {

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

        List<ArchiveTablePointEnumerator<TKey, TValue>> m_validFiles;

        public ArchiveTableSequencer()
        {
            CurrentKey = new TKey();
            CurrentValue = new TValue();
            m_validFiles = new List<ArchiveTablePointEnumerator<TKey, TValue>>();
            IsValid = false;
        }

        public void PrepareNextList(List<ArchiveTablePointEnumerator<TKey, TValue>> tables, ulong startTime, ulong stopTime)
        {
            Stats.SeeksRequested++;
            m_validFiles.Clear();
            IsValid = false;
            foreach (var table in tables)
            {
                if (table.SeekToTimeWindow(startTime, stopTime))
                    m_validFiles.Add(table);
            }
        }

        public bool ReadNext()
        {
            TryAgain:

            //ToDo: Discard any duplicate data
            if (m_validFiles.Count == 0)
            {
                IsValid = false;
                return false;
            }
            else if (m_validFiles.Count == 1)
            {
                if (!m_validFiles[0].IsValid)
                {
                    IsValid = false;
                    return false;
                }
                m_validFiles[0].CurrentKey.CopyTo(CurrentKey);
                m_validFiles[0].CurrentValue.CopyTo(CurrentValue);
                m_validFiles[0].ReadNext();
                IsValid = true;
                return true;
            }
            else
            {
                int nextValue = -1;
                TKey key = null;
                for (int x = 0; x < m_validFiles.Count; x++)
                {
                    if (m_validFiles[x].IsValid)
                    {
                        if (nextValue == -1)
                        {
                            key = m_validFiles[x].CurrentKey;
                            nextValue = x;
                        }
                        else
                        {
                            //No NullReferenceException due to state machine.
                            if (key.IsGreaterThan(m_validFiles[x].CurrentKey))
                            {
                                key = m_validFiles[x].CurrentKey;
                                nextValue = x;
                            }
                        }
                    }
                }

                if (nextValue == -1)
                {
                    IsValid = false;
                    return false;
                }
                
                if (IsValid && m_validFiles[nextValue].CurrentKey.IsEqualTo(CurrentKey))
                {
                    m_validFiles[nextValue].ReadNext();
                    goto TryAgain;
                }

                m_validFiles[nextValue].CurrentKey.CopyTo(CurrentKey);
                m_validFiles[nextValue].CurrentValue.CopyTo(CurrentValue);
                m_validFiles[nextValue].ReadNext();
                IsValid = true;
                return true;
            }
        }
    }
}
