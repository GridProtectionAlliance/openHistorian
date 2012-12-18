//******************************************************************************************************
//  ArchiveInitializer.cs - Gbtc
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
using System.IO;
using openHistorian.Archive;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine
{
    /// <summary>
    /// Creates new archive files based on user settings.
    /// </summary>
    internal class ArchiveInitializer
    {
        ArchiveInitializerSettings m_settings;

        public ArchiveInitializer(ArchiveInitializerSettings settings)
        {
            m_settings = settings;
        }

        /// <summary>
        /// Creates a new <see cref="ArchiveFile"/> based on the settings passed to this class.
        /// Once created, it is up to he caller to make sure that this class is properly disposed of.
        /// </summary>
        /// <returns></returns>
        public ArchiveFile CreateArchiveFile()
        {
            if (m_settings.IsMemoryArchive)
            {
                return ArchiveFile.CreateInMemory();
            }
            else
            {
                var fileName = CreateArchiveName();
                var file = ArchiveFile.CreateFile(fileName);
                file.SetFileSize(m_settings.InitialSize, m_settings.AutoGrowthSize, m_settings.RequiredFreeSpaceForAutoGrowth);
                return file;
            }
        }

        /// <summary>
        /// Creates a new random file in one of the provided folders in a round robin fashion.
        /// </summary>
        /// <returns></returns>
        string CreateArchiveName()
        {
            long requiredFreeSpace = Math.Min(m_settings.RequiredFreeSpaceForNewFile, m_settings.InitialSize);
            foreach (var path in m_settings.Paths)
            {
                long freeSpace, totalSpace;
                if (WinApi.GetAvailableFreeSpace(path,out freeSpace, out totalSpace))
                {
                    if (freeSpace>=requiredFreeSpace)
                    {
                        return Path.Combine(path, Guid.NewGuid().ToString() + ".d2");
                    }
                }
            }
            throw new Exception("Out of free space");
        }

    }
}
