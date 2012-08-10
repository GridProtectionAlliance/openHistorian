//******************************************************************************************************
//  FileManagementSettings.cs - Gbtc
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

using System;
using openHistorian.V2.Collections;
using openHistorian.V2.Server.Database;

namespace openHistorian.V2.Server.Configuration
{
    public class ArchiveManagementSettings : SupportsReadonlyBase<ArchiveManagementSettings>
    {
        string m_sourceName;
        string m_destinationName;
        ArchiveInitializerSettings m_initializer;

        int? m_newFileOnCommitCount;
        TimeSpan? m_newFileOnInterval;
        long? m_newFileOnSize;

        public string SourceName
        {
            get
            {
                return m_sourceName;
            }
            set
            {
                TestForEditable();
                m_sourceName = value;
            }
        }

        public string DestinationName
        {
            get
            {
                return m_destinationName;
            }
            set
            {
                TestForEditable();
                m_destinationName = value;
            }
        }

        public ArchiveInitializerSettings Initializer
        {
            get
            {
                return m_initializer;
            }
            set
            {
                TestForEditable();
                m_initializer = value;
            }
        }

        public int? NewFileOnCommitCount
        {
            get
            {
                return m_newFileOnCommitCount;
            }
            set
            {
                TestForEditable();
                m_newFileOnCommitCount = value;
            }
        }

        public TimeSpan? NewFileOnInterval
        {
            get
            {
                return m_newFileOnInterval;
            }
            set
            {
                TestForEditable();
                m_newFileOnInterval = value;
            }
        }

        public long? NewFileOnSize
        {
            get
            {
                return m_newFileOnSize;
            }
            set
            {
                TestForEditable();
                m_newFileOnSize = value;
            }
        }

        protected override void SetInternalMembersAsReadOnly()
        {
             m_initializer.IsReadOnly = true;
        }

        protected override void SetInternalMembersAsEditable()
        {
             m_initializer = m_initializer.EditableClone();
        }

      
    }
}
