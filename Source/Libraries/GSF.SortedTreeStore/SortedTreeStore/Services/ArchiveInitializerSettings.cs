//******************************************************************************************************
//  ArchiveInitializerSettings.cs - Gbtc
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
//  10/1/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Settings for <see cref="ArchiveInitializer{TKey,TValue}"/>.
    /// </summary>
    public class ArchiveInitializerSettings
    {
        public bool IsMemoryArchive = false;
        public string Prefix = string.Empty;
        public List<string> WritePath = new List<string>();
        public string FileExtension = ".d2i";
        public List<Guid> Flags = new List<Guid>();
        public EncodingDefinition EncodingMethod = SortedTree.FixedSizeNode;
        public long RequiredFreeSpaceForNewFile;
        public long DesiredRemainingSpace;

        /// <summary>
        /// Clones the <see cref="ArchiveInitializerSettings"/>
        /// </summary>
        /// <returns></returns>
        public ArchiveInitializerSettings Clone()
        {
            var other = (ArchiveInitializerSettings)MemberwiseClone();
            return other;
        }

        /// <summary>
        /// Creates a <see cref="ArchiveInitializer{TKey,TValue}"/> that will reside in memory.
        /// </summary>
        /// <param name="encodingMethod">the encoding method to use for the archive file.</param>
        /// <param name="flags">flags to include in the archive that is created.</param>
        /// <returns></returns>
        public static ArchiveInitializerSettings CreateInMemory(EncodingDefinition encodingMethod, params Guid[] flags)
        {
            ArchiveInitializerSettings settings = new ArchiveInitializerSettings();
            settings.Flags.AddRange(flags);
            settings.IsMemoryArchive = true;
            settings.EncodingMethod = encodingMethod;
            return settings;
        }

        /// <summary>
        /// Creates a <see cref="ArchiveInitializer{TKey,TValue}"/> that will reside on the disk.
        /// </summary>
        /// <param name="path">the path to place the files.</param>
        /// <param name="encodingMethod">the encoding method to use for the archive file.</param>
        /// <param name="prefix">the prefix to affix to the files created.</param>
        /// <param name="extension">the extension file name</param>
        /// <param name="flags">flags to include in the archive that is created.</param>
        /// <returns></returns>
        public static ArchiveInitializerSettings CreateOnDisk(string path, EncodingDefinition encodingMethod, string prefix, string extension, params Guid[] flags)
        {
            ArchiveInitializerSettings settings = new ArchiveInitializerSettings();
            settings.FileExtension = extension;
            settings.Flags.AddRange(flags);
            settings.Prefix = prefix;
            settings.IsMemoryArchive = false;
            settings.WritePath.Add(path);
            settings.RequiredFreeSpaceForNewFile = 1024 * 1024;
            settings.DesiredRemainingSpace = 1024 * 1024;
            settings.EncodingMethod = encodingMethod;
            return settings;
        }
    }
}