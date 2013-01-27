//******************************************************************************************************
//  ArchiveInitializer.cs - Gbtc
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
//  7/24/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GSF;
using openHistorian.Archive;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine
{
    /// <summary>
    /// Creates new archive files based on user settings.
    /// </summary>
    internal class ArchiveInitializer
    {
        string m_prefix;
        bool m_isMemoryArchive;
        List<string> m_paths;
        long m_requiredFreeSpaceForNewFile;
        long m_initialSize;
        CompressionMethod m_method;

        private ArchiveInitializer()
        {
        }

        public ArchiveInitializer(ArchiveInitializerSettings settings)
        {
            m_prefix = "";
            m_isMemoryArchive = settings.IsMemoryArchive;
            m_paths = settings.Paths.ToList();
            m_requiredFreeSpaceForNewFile = settings.RequiredFreeSpaceForNewFile;
            m_initialSize = settings.InitialSize;
            m_method = CompressionMethod.None;
        }

        public static ArchiveInitializer CreateInMemory(CompressionMethod method)
        {
            var settings = new ArchiveInitializer();
            settings.m_isMemoryArchive = true;
            settings.m_method = method;
            return settings;

        }

        public static ArchiveInitializer CreateOnDisk(List<string> paths, CompressionMethod method, string prefix)
        {
            var settings = new ArchiveInitializer();
            settings.m_prefix = prefix;
            settings.m_isMemoryArchive = false;
            settings.m_paths = paths.ToList();
            settings.m_requiredFreeSpaceForNewFile = 1024 * 1024;
            settings.m_initialSize = 1024 * 1024;
            settings.m_method = method;
            return settings;
        }

        /// <summary>
        /// Gets if the <see cref="ArchiveFile"/> that is created with this initializer is file backed.
        /// </summary>
        public bool IsFileBacked
        {
            get
            {
                return !m_isMemoryArchive;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ArchiveFile"/> based on the settings passed to this class.
        /// Once created, it is up to he caller to make sure that this class is properly disposed of.
        /// </summary>
        /// <returns></returns>
        public ArchiveFile CreateArchiveFile()
        {
            if (m_isMemoryArchive)
            {
                return ArchiveFile.CreateInMemory(m_method);
            }
            else
            {
                var fileName = CreateArchiveName();
                var file = ArchiveFile.CreateFile(fileName,m_method);
                return file;
            }
        }

        /// <summary>
        /// Creates a new random file in one of the provided folders in a round robin fashion.
        /// </summary>
        /// <returns></returns>
        string CreateArchiveName()
        {
            long requiredFreeSpace = Math.Min(m_requiredFreeSpaceForNewFile, m_initialSize);
            foreach (var path in m_paths)
            {
                long freeSpace, totalSpace;
                if (WinApi.GetAvailableFreeSpace(path, out freeSpace, out totalSpace))
                {
                    if (freeSpace >= requiredFreeSpace)
                    {
                        return Path.Combine(path, DateTime.Now.Ticks.ToString() + "-" + m_prefix + "-" + Guid.NewGuid().ToString() + ".d2");
                    }
                }
            }
            throw new Exception("Out of free space");
        }

    }
}
