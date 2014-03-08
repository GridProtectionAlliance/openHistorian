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
using System.IO;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine
{
    /// <summary>
    /// Creates new archive files based on user settings.
    /// </summary>
    public class ArchiveInitializer<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private string m_prefix;
        private string m_path;
        private bool m_isMemoryArchive;
        private long m_requiredFreeSpaceForNewFile;
        private long m_initialSize;
        private EncodingDefinition m_encodingMethod;

        private ArchiveInitializer()
        {
        }

        public static ArchiveInitializer<TKey, TValue> CreateInMemory(EncodingDefinition compressionMethod)
        {
            ArchiveInitializer<TKey, TValue> settings = new ArchiveInitializer<TKey, TValue>();
            settings.m_isMemoryArchive = true;
            settings.m_encodingMethod = compressionMethod;
            return settings;
        }

        public static ArchiveInitializer<TKey, TValue> CreateOnDisk(string path, EncodingDefinition compressionMethod, string prefix)
        {
            ArchiveInitializer<TKey, TValue> settings = new ArchiveInitializer<TKey, TValue>();
            settings.m_prefix = prefix;
            settings.m_isMemoryArchive = false;
            settings.m_path = path;
            settings.m_requiredFreeSpaceForNewFile = 1024 * 1024;
            settings.m_initialSize = 1024 * 1024;
            settings.m_encodingMethod = compressionMethod;
            return settings;
        }

        /// <summary>
        /// Creates a new <see cref="SortedTreeTable{TKey,TValue}"/> based on the settings passed to this class.
        /// Once created, it is up to he caller to make sure that this class is properly disposed of.
        /// </summary>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> CreateArchiveFile()
        {
            if (m_isMemoryArchive)
            {
                SortedTreeFile af = SortedTreeFile.CreateInMemory();
                return af.OpenOrCreateTable<TKey, TValue>(m_encodingMethod);
            }
            else
            {
                string fileName = CreateArchiveName();
                SortedTreeFile af = SortedTreeFile.CreateFile(fileName);
                return af.OpenOrCreateTable<TKey, TValue>(m_encodingMethod);
            }
        }

        /// <summary>
        /// Creates a new random file in one of the provided folders in a round robin fashion.
        /// </summary>
        /// <returns></returns>
        private string CreateArchiveName()
        {
            long requiredFreeSpace = Math.Min(m_requiredFreeSpaceForNewFile, m_initialSize);
            long freeSpace, totalSpace;
            if (WinApi.GetAvailableFreeSpace(m_path, out freeSpace, out totalSpace))
            {
                if (freeSpace >= requiredFreeSpace)
                {
                    return Path.Combine(m_path, DateTime.Now.Ticks.ToString() + "-" + m_prefix + "-" + Guid.NewGuid().ToString() + ".d2");
                }
            }
            throw new Exception("Out of free space");
        }
    }
}