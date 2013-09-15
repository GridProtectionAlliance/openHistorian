//******************************************************************************************************
//  WriteProcessor.cs - Gbtc
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
using openHistorian.Collections.Generic;

namespace openHistorian.Engine.ArchiveWriters
{
    public partial class WriteProcessor<TKey, TValue> : IDisposable
        where TKey : class, new()
        where TValue : class, new()
    {
        private readonly ArchiveList<TKey, TValue> m_archiveList;

        private readonly PrestageWriter<TKey, TValue> m_prestage;
        private readonly StageWriter<TKey, TValue> m_stage0;
        private readonly StageWriter<TKey, TValue> m_stage1;
        private readonly StageWriter<TKey, TValue> m_stage2;

        public WriteProcessor(WriteProcessorSettings<TKey, TValue> settings, ArchiveList<TKey, TValue> list)
        {
            m_archiveList = list;
            m_stage2 = new StageWriter<TKey, TValue>(settings.Stage2, FinalizeArchiveFile);
            m_stage1 = new StageWriter<TKey, TValue>(settings.Stage1, m_stage2.AppendData);
            m_stage0 = new StageWriter<TKey, TValue>(settings.Stage0, m_stage1.AppendData);
            m_prestage = new PrestageWriter<TKey, TValue>(settings.Prestage, m_stage0.AppendData);
        }

        public long Write(TKey key, TValue value)
        {
            return m_prestage.Write(key, value);
        }

        public long Write(KeyValueStream<TKey, TValue> stream)
        {
            return m_prestage.Write(stream);
        }

        private void FinalizeArchiveFile(RolloverArgs<TKey, TValue> args)
        {
            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
            {
                edit.ReleaseEditLock(args.File);
            }
        }

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