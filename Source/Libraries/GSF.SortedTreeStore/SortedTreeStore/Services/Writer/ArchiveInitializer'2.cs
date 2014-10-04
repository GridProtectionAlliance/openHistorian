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
//  07/24/2012 - Steven E. Chisholm
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

namespace GSF.SortedTreeStore.Services.Writer
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
        /// <param name="estimatedSize">The estimated size of the file. -1 to ignore this feature and write to the first available directory.</param>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> CreateArchiveFile(long estimatedSize = -1)
        {
            if (m_settings.IsMemoryArchive)
            {
                SortedTreeFile af = SortedTreeFile.CreateInMemory(blockSize: 4096, flags: m_settings.Flags.ToArray());
                return af.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
            }
            else
            {
                string fileName = CreateArchiveName(GetPathWithEnoughSpace(estimatedSize));
                SortedTreeFile af = SortedTreeFile.CreateFile(fileName, blockSize: 4096, flags: m_settings.Flags.ToArray());
                return af.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
            }
        }

        /// <summary>
        /// Creates a new <see cref="SortedTreeTable{TKey,TValue}"/> based on the settings passed to this class.
        /// Once created, it is up to he caller to make sure that this class is properly disposed of.
        /// </summary>
        /// <param name="startKey">the first key in the archive file</param>
        /// <param name="endKey">the last key in the archive file</param>
        /// <param name="estimatedSize">The estimated size of the file. -1 to ignore this feature and write to the first available directory.</param>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> CreateArchiveFile(TKey startKey, TKey endKey, long estimatedSize = -1)
        {
            if (m_settings.IsMemoryArchive)
            {
                SortedTreeFile af = SortedTreeFile.CreateInMemory(blockSize: 4096, flags: m_settings.Flags.ToArray());
                return af.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
            }
            else
            {
                string fileName = CreateArchiveName(GetPathWithEnoughSpace(estimatedSize), startKey, endKey);
                SortedTreeFile af = SortedTreeFile.CreateFile(fileName, blockSize: 4096, flags: m_settings.Flags.ToArray());
                return af.OpenOrCreateTable<TKey, TValue>(m_settings.EncodingMethod);
            }
        }

        /// <summary>
        /// Creates a new random file in one of the provided folders in a round robin fashion.
        /// </summary>
        /// <returns></returns>
        private string CreateArchiveName(string path)
        {
            path = GetPath(path, DateTime.Now);
            return Path.Combine(path, DateTime.Now.Ticks.ToString() + "-" + m_settings.Prefix + "-" + Guid.NewGuid().ToString() + m_settings.FileExtension);
        }

        /// <summary>
        /// Creates a new random file in one of the provided folders in a round robin fashion.
        /// </summary>
        /// <returns></returns>
        private string CreateArchiveName(string path, TKey startKey, TKey endKey)
        {
            IHasTimestampField startTime = startKey as IHasTimestampField;
            IHasTimestampField endTime = endKey as IHasTimestampField;
            DateTime startDate;
            DateTime endDate;

            if (startTime == null || endTime == null)
                return CreateArchiveName(path);

            if (!startTime.TryGetDateTime(out startDate) || !endTime.TryGetDateTime(out endDate))
                return CreateArchiveName(path);

            path = GetPath(path, startDate);
            return Path.Combine(path, DateTime.Now.Ticks.ToString() + "-" + m_settings.Prefix + "-" + startDate.ToString("yyyy-MM-dd HH.mm.ss.fff") + "_to_" + endDate.ToString("yyyy-MM-dd HH.mm.ss.fff") + m_settings.FileExtension);
        }

        private string GetPath(string rootPath, DateTime time)
        {
            switch (m_settings.DirectoryMethod)
            {
                case ArchiveDirectoryMethod.TopDirectoryOnly:
                    break;
                case ArchiveDirectoryMethod.Year:
                    rootPath = Path.Combine(rootPath, time.Year.ToString());
                    break;
                case ArchiveDirectoryMethod.YearMonth:
                    rootPath = Path.Combine(rootPath, time.Year.ToString() + time.Month.ToString("00"));
                    break;
                case ArchiveDirectoryMethod.YearThenMonth:
                    rootPath = Path.Combine(rootPath, time.Year.ToString() + '\\' + time.Month.ToString("00"));
                    break;
            }
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);
            return rootPath;
        }

        string GetPathWithEnoughSpace(long estimatedSize)
        {
            if (estimatedSize < 0)
                return m_settings.WritePath.First();
            long remainingSpace = m_settings.DesiredRemainingSpace;
            foreach (string path in m_settings.WritePath)
            {
                long freeSpace;
                long totalSpace;
                WinApi.GetAvailableFreeSpace(path, out freeSpace, out totalSpace);
                if (freeSpace - estimatedSize > remainingSpace)
                    return path;
            }
            throw new Exception("Out of free space");
        }


    }
}