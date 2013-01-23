//******************************************************************************************************
//  PrestageWriter.cs - Gbtc
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
using System.Linq;
using System.Text;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine.ArchiveWriters
{
    internal class WriteProcessor : IDisposable
    {
        ArchiveList m_archiveList;
        object m_syncRoot;

        PrestageWriter m_prestage;
        StageWriter m_stage0;
        StageWriter m_stage1;
        StageWriter m_stage2;

        public WriteProcessor(WriteProcessorSettings settings, ArchiveList list)
        {
            m_archiveList = list;
            m_syncRoot = new object();

            m_stage2 = new StageWriter(settings.Stage2, FinalizeArchiveFile);
            m_stage1 = new StageWriter(settings.Stage1, m_stage2.AppendData);
            m_stage0 = new StageWriter(settings.Stage0, m_stage1.AppendData);
            m_prestage = new PrestageWriter(settings.Prestage, m_stage0.AppendData);
        }

        public void Write(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            m_prestage.Write(key1, key2, value1, value2);
        }

        void FinalizeArchiveFile(RolloverArgs args)
        {
            using (var edit = m_archiveList.AcquireEditLock())
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


    }
}
