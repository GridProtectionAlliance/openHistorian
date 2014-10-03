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
    /// Specifies the directory structure to follow when writing archive files to the disk.
    /// </summary>
    public enum ArchiveDirectoryMethod
    {
        /// <summary>
        /// Writes all files in the top directory
        /// </summary>
        TopDirectoryOnly,
        /// <summary>
        /// Writes all files based on the starting year
        /// </summary>
        Year,
        /// <summary>
        /// Writes all files based on 'YearMonth'
        /// </summary>
        YearMonth,
        /// <summary>
        /// Writes all files based on 'Year\Month'
        /// </summary>
        YearThenMonth
    }

    /// <summary>
    /// Settings for <see cref="ArchiveInitializer{TKey,TValue}"/>.
    /// </summary>
    public class ArchiveInitializerSettings
    {
        public ArchiveDirectoryMethod DirectoryMethod;
        public bool IsMemoryArchive = false;
        public string Prefix = string.Empty;
        public List<string> WritePath = new List<string>();
        public string FileExtension = ".d2i";
        public List<Guid> Flags = new List<Guid>();
        public EncodingDefinition EncodingMethod = SortedTree.FixedSizeNode;
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
        /// <param name="paths">the paths to place the files.</param>
        /// <param name="desiredRemainingSpace">The desired free space to leave on the disk before moving to another disk.</param>
        /// <param name="directoryMethod">the method for storing files in a directory.</param>
        /// <param name="encodingMethod">the encoding method to use for the archive file.</param>
        /// <param name="prefix">the prefix to affix to the files created.</param>
        /// <param name="extension">the extension file name</param>
        /// <param name="flags">flags to include in the archive that is created.</param>
        /// <returns></returns>
        public static ArchiveInitializerSettings CreateOnDisk(IEnumerable<string> paths, long desiredRemainingSpace, ArchiveDirectoryMethod directoryMethod, EncodingDefinition encodingMethod, string prefix, string extension, params Guid[] flags)
        {
            ArchiveInitializerSettings settings = new ArchiveInitializerSettings();
            settings.DirectoryMethod = directoryMethod;
            settings.FileExtension = extension;
            settings.Flags.AddRange(flags);
            settings.Prefix = prefix;
            settings.IsMemoryArchive = false;
            settings.WritePath.AddRange(paths);
            settings.DesiredRemainingSpace = desiredRemainingSpace;
            settings.EncodingMethod = encodingMethod;
            return settings;
        }
    }
}