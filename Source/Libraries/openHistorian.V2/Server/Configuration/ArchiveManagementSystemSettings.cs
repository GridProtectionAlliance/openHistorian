//******************************************************************************************************
//  ArchiveManagementSystemSettings.cs - Gbtc
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
//  7/24/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using openHistorian.V2.Collections;

namespace openHistorian.V2.Server.Configuration
{
    public class ArchiveManagementSystemSettings : SupportsReadonlyBase<ArchiveManagementSystemSettings>
    {
        ArchiveWriterSettings m_archiveWriter;
        ReadonlyList<ArchiveManagementSettings> m_archiveManagers;
        ArchiveListSettings m_archiveList;

        public ArchiveManagementSystemSettings()
        {
            m_archiveWriter = new ArchiveWriterSettings();
            m_archiveManagers = new ReadonlyList<ArchiveManagementSettings>();
            m_archiveList = new ArchiveListSettings();
        }

        public ArchiveWriterSettings ArchiveWriter
        {
            get
            {
                return m_archiveWriter;
            }
            set
            {
                TestForEditable();
                m_archiveWriter = value;
            }
        }
        public ReadonlyList<ArchiveManagementSettings> ArchiveManagers
        {
            get
            {
                return m_archiveManagers;
            }
            set
            {
                TestForEditable();
                m_archiveManagers = value;
            }
        }
        public ArchiveListSettings ArchiveList
        {
            get
            {
                return m_archiveList;
            }
            set
            {
                TestForEditable();
                m_archiveList = value;
            }
        }

        protected override void SetInternalMembersAsReadOnly()
        {
            m_archiveWriter.IsReadOnly = true;
            m_archiveManagers.IsReadOnly = true;
            m_archiveList.IsReadOnly = true;
        }

        protected override void SetInternalMembersAsEditable()
        {
            m_archiveWriter = m_archiveWriter.EditableClone();
            m_archiveManagers = m_archiveManagers.EditableClone();
            m_archiveList = m_archiveList.EditableClone();
        }

    }

}
