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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine.ArchiveWriters
{
    class WriteProcessor : IDisposable
    {
        ArchiveList m_archiveList;
        object m_syncRoot;

        PrestageWriter m_prestage;
        StageWriter m_stage0;
        StageWriter m_stage1;
        StageWriter m_stage2;

        public WriteProcessor(DatabaseSettings settings, ArchiveList archiveList)
        {
            m_syncRoot = new object();
            m_archiveList = archiveList;

            var prestageSettings = new PrestageSettings()
                {
                    DelayOnPointCount = 20 * 1000,
                    RolloverPointCount = 10 * 1000,
                    RolloverInterval = 100
                };

            var stage0Settings = new StageWriterSettings()
                {
                    RolloverInterval = 1000,
                    RolloverSize = 1 * 1000 * 1000,
                    StagingFile = new StagingFile(archiveList, new ArchiveInitializer(settings.Stage0.Initializer))
                };

            var stage1Settings = new StageWriterSettings()
            {
                RolloverInterval = 1000,
                RolloverSize = 1 * 1000 * 1000,
                StagingFile = new StagingFile(archiveList, new ArchiveInitializer(settings.Stage1.Initializer))
            };

            var stage2Settings = new StageWriterSettings()
            {
                RolloverInterval = 1000,
                RolloverSize = 1 * 1000 * 1000,
                StagingFile = new StagingFile(archiveList, new ArchiveInitializer(settings.Stage2.Initializer))
            };

            m_stage2 = new StageWriter(stage2Settings, FinalizeArchiveFile);
            m_stage1 = new StageWriter(stage1Settings, m_stage2.AppendData);
            m_stage0 = new StageWriter(stage0Settings, m_stage1.AppendData);
            m_prestage = new PrestageWriter(prestageSettings, m_stage0.AppendData);
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
            throw new NotImplementedException();
        }

       
    }
}
