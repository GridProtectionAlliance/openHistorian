//******************************************************************************************************
//  WriteProcessor`2.cs - Gbtc
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
//  1/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{
    /// <summary>
    /// Houses all of the write operations for the historian
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class WriteProcessor<TKey, TValue>
        : IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private readonly ArchiveList<TKey, TValue> m_archiveList;

        private readonly PrestageWriter<TKey, TValue> m_prestage;
        private readonly FirstStageWriter<TKey, TValue> m_stage0;
        readonly CombineFiles<TKey, TValue> m_stage1;
        readonly CombineFiles<TKey, TValue> m_stage2;
        /// <summary>
        /// Creates a new class
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="list"></param>
        public WriteProcessor(WriteProcessorSettings<TKey, TValue> settings, ArchiveList<TKey, TValue> list)
        {
            m_archiveList = list;
            m_stage2 = new CombineFiles<TKey, TValue>(settings.Stage2);
            m_stage1 = new CombineFiles<TKey, TValue>(settings.Stage1);
            m_stage0 = new FirstStageWriter<TKey, TValue>(settings.Stage0);
            m_prestage = new PrestageWriter<TKey, TValue>(settings.Prestage, m_stage0.AppendData);
        }

        /// <summary>
        /// Writes the provided key/value to the historian.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>the transaction code so this write can be tracked.</returns>
        public long Write(TKey key, TValue value)
        {
            return m_prestage.Write(key, value);
        }

        /// <summary>
        /// Writes the provided stream to the historian.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>the transaction code so this write can be tracked.</returns>
        public long Write(TreeStream<TKey, TValue> stream)
        {
            return m_prestage.Write(stream);
        }

        /// <summary>
        /// Stops the execution of the historian in a orderly mannor.
        /// </summary>
        public void Dispose()
        {
            m_prestage.Stop();
            m_stage0.Stop();
            m_stage1.Stop();
            m_stage2.Stop();
        }

        public void SoftCommit()
        {
            throw new NotImplementedException();
        }

        public void HardCommit()
        {
            throw new NotImplementedException();
        }
    }
}