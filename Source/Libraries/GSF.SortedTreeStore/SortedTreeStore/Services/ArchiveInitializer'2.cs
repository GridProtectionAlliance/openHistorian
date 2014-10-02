//******************************************************************************************************
//  ArchiveInitializer`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
using System.Linq;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Types;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Creates new archive files based on user settings.
    /// </summary>
    public class ArchiveInitializer<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private ArchiveInitializerSettings m_settings;

        /// <summary>
        /// Creates a <see cref="ArchiveInitializer{TKey,TValue}"/>
        /// </summary>
        /// <param name="settings"></param>
        public ArchiveInitializer(ArchiveInitializerSettings settings)
        {
            m_settings = settings;
        }

        /// <summary>
        /// Creates a new <see cref="SortedTreeTable{TKey,TValue}"/> based on the settings passed to this class.
        /// Once created, it is up to he caller to make sure that this class is properly disposed of.
        /// </summary>
        /// <param name="uniqueFileId">a guid that will be the unique identifier of this file. If Guid.Empty one will be generated in the constructor</param>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> CreateArchiveFile(Guid uniqueFileId = default(Guid))
        {
            if (m_settings.IsMemoryArchive)
            {
                SortedTreeFile af = SortedTreeFile.CreateInMemory(4096, uniqueFileId, m_settings.Flags.ToArray());
                return af.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
            }
            else
            {
                string fileName = CreateArchiveName();
                SortedTreeFile af = SortedTreeFile.CreateFile(fileName, 4096, uniqueFileId, m_settings.Flags.ToArray());
                return af.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
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
            if (m_settings.IsMemoryArchive)
            {
                SortedTreeFile af = SortedTreeFile.CreateInMemory(4096, uniqueFileId, m_settings.Flags.ToArray());
                return af.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
            }
            else
            {
                string fileName = CreateArchiveName(startKey, endKey);
                SortedTreeFile af = SortedTreeFile.CreateFile(fileName, 4096, uniqueFileId, m_settings.Flags.ToArray());
                return af.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
            }
        }

        /// <summary>
        /// Creates a new random file in one of the provided folders in a round robin fashion.
        /// </summary>
        /// <returns></returns>
        private string CreateArchiveName()
        {
            long requiredFreeSpace = m_settings.RequiredFreeSpaceForNewFile;
            long freeSpace, totalSpace;
            if (WinApi.GetAvailableFreeSpace(m_settings.WritePath.First(), out freeSpace, out totalSpace))
            {
                if (freeSpace >= requiredFreeSpace)
                {
                    return Path.Combine(m_settings.WritePath.First(), DateTime.Now.Ticks.ToString() + "-" + m_settings.Prefix + "-" + Guid.NewGuid().ToString() + m_settings.FileExtension);
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


            long requiredFreeSpace = m_settings.RequiredFreeSpaceForNewFile;
            long freeSpace, totalSpace;
            if (WinApi.GetAvailableFreeSpace(m_settings.WritePath.First(), out freeSpace, out totalSpace))
            {
                if (freeSpace >= requiredFreeSpace)
                {
                    return Path.Combine(m_settings.WritePath.First(), DateTime.Now.Ticks.ToString() + "-" + m_settings.Prefix + "-" + startDate.ToString("yyyy-MM-dd HH.mm.ss.fff") + "_to_" + startDate.ToString("yyyy-MM-dd HH.mm.ss.fff") + m_settings.FileExtension);
                }
            }
            throw new Exception("Out of free space");
        }

    }
}