//******************************************************************************************************
//  ArchiveDatabaseEngine.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class ArchiveDatabaseEngine
    {
        //ToDo: The dispose pattern has to be handled properly. Architect that solution.
        ArchiveWriter m_archiveWriter;
        ArchiveList m_archiveList;
        ArchiveManagement[] m_archiveManagement;

        public ArchiveDatabaseEngine(DatabaseSettings settings)
        {
            m_archiveList = new ArchiveList(settings.AttachedFiles);
            m_archiveManagement = new ArchiveManagement[settings.ArchiveRollovers.Count];

            ArchiveManagement previousManagement = null;
            for (int x = settings.ArchiveRollovers.Count - 1; x >= 0; x-- ) //Go in reverse order since there is chaining that occurs
            {
                var managementSettings = settings.ArchiveRollovers[x];
                if (previousManagement == null)
                {
                    m_archiveManagement[x] = new ArchiveManagement(managementSettings, m_archiveList, FinalizeArchiveFile);
                    previousManagement = m_archiveManagement[x];
                }
                else
                {
                    m_archiveManagement[x] = new ArchiveManagement(managementSettings, m_archiveList, previousManagement.ProcessArchive);
                    previousManagement = m_archiveManagement[x];
                }
            }
            if (previousManagement == null)
            {
                m_archiveWriter = new ArchiveWriter(settings.ArchiveWriter, m_archiveList, FinalizeArchiveFile);
            }
            else
            {
                m_archiveWriter = new ArchiveWriter(settings.ArchiveWriter, m_archiveList, previousManagement.ProcessArchive);
            }
        }

        public void WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            m_archiveWriter.WriteData(key1, key2, value1, value2);
        }

        void FinalizeArchiveFile(ArchiveFile archive)
        {
            using (var edit = m_archiveList.AcquireEditLock())
            {
                edit.ReleaseEditLock(archive);
            }
        }

    }
}
