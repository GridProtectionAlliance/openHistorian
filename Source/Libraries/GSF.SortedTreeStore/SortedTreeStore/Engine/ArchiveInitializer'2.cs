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
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine
{
    /// <summary>
    /// Creates new archive files based on user settings.
    /// </summary>
    public class ArchiveInitializer<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        private string m_prefix;
        private bool m_isMemoryArchive;
        private List<string> m_paths;
        private long m_requiredFreeSpaceForNewFile;
        private long m_initialSize;
        private Guid m_compressionMethod;

        private ArchiveInitializer()
        {
        }

        public static ArchiveInitializer<TKey, TValue> CreateInMemory(Guid compressionMethod)
        {
            ArchiveInitializer<TKey, TValue> settings = new ArchiveInitializer<TKey, TValue>();
            settings.m_isMemoryArchive = true;
            settings.m_compressionMethod = compressionMethod;
            return settings;
        }

        public static ArchiveInitializer<TKey, TValue> CreateOnDisk(List<string> paths, Guid compressionMethod, string prefix)
        {
            ArchiveInitializer<TKey, TValue> settings = new ArchiveInitializer<TKey, TValue>();
            settings.m_prefix = prefix;
            settings.m_isMemoryArchive = false;
            settings.m_paths = paths.ToList();
            settings.m_requiredFreeSpaceForNewFile = 1024 * 1024;
            settings.m_initialSize = 1024 * 1024;
            settings.m_compressionMethod = compressionMethod;
            return settings;
        }

        /// <summary>
        /// Gets if the <see cref="HistorianArchiveFile"/> that is created with this initializer is file backed.
        /// </summary>
        public bool IsFileBacked
        {
            get
            {
                return !m_isMemoryArchive;
            }
        }

        /// <summary>
        /// Creates a new <see cref="HistorianArchiveFile"/> based on the settings passed to this class.
        /// Once created, it is up to he caller to make sure that this class is properly disposed of.
        /// </summary>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> CreateArchiveFile()
        {
            if (m_isMemoryArchive)
            {
                SortedTreeFile af = SortedTreeFile.CreateInMemory();
                return af.OpenOrCreateTable<TKey, TValue>(m_compressionMethod);
            }
            else
            {
                string fileName = CreateArchiveName();
                SortedTreeFile af = SortedTreeFile.CreateFile(fileName);
                return af.OpenOrCreateTable<TKey, TValue>(m_compressionMethod);
            }
        }

        /// <summary>
        /// Creates a new random file in one of the provided folders in a round robin fashion.
        /// </summary>
        /// <returns></returns>
        private string CreateArchiveName()
        {
            long requiredFreeSpace = Math.Min(m_requiredFreeSpaceForNewFile, m_initialSize);
            foreach (string path in m_paths)
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