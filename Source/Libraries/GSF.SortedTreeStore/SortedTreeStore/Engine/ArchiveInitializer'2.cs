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
using GSF.SortedTreeStore.Types;

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
        /// <param name="uniqueFileId">a guid that will be the unique identifier of this file. If Guid.Empty one will be generated in the constructor</param>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> CreateArchiveFile(Guid uniqueFileId = default(Guid))
        {
            if (m_isMemoryArchive)
            {
                SortedTreeFile af = SortedTreeFile.CreateInMemory(4096, uniqueFileId);
                return af.OpenOrCreateTable<TKey, TValue>(m_encodingMethod);
            }
            else
            {
                string fileName = CreateArchiveName();
                SortedTreeFile af = SortedTreeFile.CreateFile(fileName, 4096, uniqueFileId);
                return af.OpenOrCreateTable<TKey, TValue>(m_encodingMethod);
            }
        }

        /// <summary>
        /// Creates a new <see cref="SortedTreeTable{TKey,TValue}"/> based on the settings passed to this class.
        /// Once created, it is up to he caller to make sure that this class is properly disposed of.
        /// </summary>
        /// <param name="startKey">the first key in the archive file</param>
        /// <param name="endKey">the last key in the archive file</param>
        /// <param name="uniqueFileId">a guid that will be the unique identifier of this file. If Guid.Empty one will be generated in the constructor</param>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> CreateArchiveFile(TKey startKey, TKey endKey, Guid uniqueFileId = default(Guid))
        {
            if (m_isMemoryArchive)
            {
                SortedTreeFile af = SortedTreeFile.CreateInMemory(4096, uniqueFileId);
                return af.OpenOrCreateTable<TKey, TValue>(m_encodingMethod);
            }
            else
            {
                string fileName = CreateArchiveName(startKey, endKey);
                SortedTreeFile af = SortedTreeFile.CreateFile(fileName, 4096, uniqueFileId);
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

        /// <summary>
        /// Creates a new random file in one of the provided folders in a round robin fashion.
        /// </summary>
        /// <returns></returns>
        private string CreateArchiveName(TKey startKey, TKey endKey)
        {
            IHasTimestampField startTime = startKey as IHasTimestampField;
            IHasTimestampField endTime = endKey as IHasTimestampField;
            DateTime startDate;
            DateTime endDate;

            if (startTime == null || endTime == null)
                return CreateArchiveName();

            if (!startTime.TryGetDateTime(out startDate) || !endTime.TryGetDateTime(out endDate))
                return CreateArchiveName();


            long requiredFreeSpace = Math.Min(m_requiredFreeSpaceForNewFile, m_initialSize);
            long freeSpace, totalSpace;
            if (WinApi.GetAvailableFreeSpace(m_path, out freeSpace, out totalSpace))
            {
                if (freeSpace >= requiredFreeSpace)
                {
                    return Path.Combine(m_path, DateTime.Now.Ticks.ToString() + "-" + m_prefix + "-" + startDate.ToString("yyyy-MM-dd HH.mm.ss.fff") + "_to_" + startDate.ToString("yyyy-MM-dd HH.mm.ss.fff") + ".d2");
                }
            }
            throw new Exception("Out of free space");
        }
    }
}