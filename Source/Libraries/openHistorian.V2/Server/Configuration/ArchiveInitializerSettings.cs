//******************************************************************************************************
//  PartitionInitializerGenerationSettings.cs - Gbtc
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
using openHistorian.V2.Server.Configuration;

namespace openHistorian.V2.Server.Database
{
    public class ArchiveInitializerSettings : SupportsReadonlyAutoBase<ArchiveInitializerSettings>
    {
        bool m_isMemoryArchive;
        FolderListSettings m_savePath;
        long m_initialSize;
        long m_autoGrowthSize;
        long m_requiredFreeSpaceForNewFile;
        long m_requiredFreeSpaceForAutoGrowth;
        
        public ArchiveInitializerSettings()
        {
            m_savePath = new FolderListSettings();
        }

        public bool IsMemoryArchive
        {
            get
            {
                return m_isMemoryArchive;
            }
            set
            {
                TestForEditable();
                m_isMemoryArchive = value;
            }
        }

        public FolderListSettings SavePaths
        {
            get
            {
                return m_savePath;
            }
            set
            {
                TestForEditable();
                m_savePath = value;
            }
        }

        public long InitialSize
        {
            get
            {
                return m_initialSize;
            }
            set
            {
                TestForEditable();
                m_initialSize = value;
            }
        }

        /// <summary>
        /// Get/Set the number of bytes an archive will 
        /// auto-grow by on each allocation
        /// </summary>
        public long AutoGrowthSize
        {
            get
            {
                return m_autoGrowthSize;
            }
            set
            {
                TestForEditable();
                m_autoGrowthSize = value;
            }
        }

        /// <summary>
        /// Get/Set the required free space in a folder path in order for a new file to
        /// be created in this path.  
        /// </summary>
        public long RequiredFreeSpaceForNewFile
        {
            get
            {
                return m_requiredFreeSpaceForNewFile;
            }
            set
            {
                TestForEditable();
                m_requiredFreeSpaceForNewFile = value;
            }
        }

        /// <summary>
        /// Get/Set the free space point where the file no longer grows freely. 
        /// </summary>
        public long RequiredFreeSpaceForAutoGrowth
        {
            get
            {
                return m_requiredFreeSpaceForAutoGrowth;
            }
            set
            {
                TestForEditable();
                m_requiredFreeSpaceForAutoGrowth = value;
            }
        }
    }
}
