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
using System.IO;
using System.Linq;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// Settings for <see cref="ArchiveInitializer{TKey,TValue}"/>.
    /// </summary>
    public class ArchiveInitializerSettings
    {
        private ArchiveDirectoryMethod m_directoryMethod;
        private bool m_isMemoryArchive = false;
        private string m_prefix = string.Empty;
        private List<string> m_writePath = new List<string>();
        private string m_fileExtension = ".d2i";
        private List<Guid> m_flags = new List<Guid>();
        private EncodingDefinition m_encodingMethod = SortedTree.FixedSizeNode;
        private long m_desiredRemainingSpace = 5 * 1024 * 1024 * 1024L; //5GB

        /// <summary>
        /// Gets the method that the directory structure will follow when writing a new file.
        /// </summary>
        public ArchiveDirectoryMethod DirectoryMethod
        {
            get
            {
                return m_directoryMethod;
            }
            set
            {
                m_directoryMethod = value;
            }
        }

        /// <summary>
        /// Gets if the archive file is a memory archive or a file archive.
        /// </summary>
        public bool IsMemoryArchive
        {
            get
            {
                return m_isMemoryArchive;
            }
            set
            {
                m_isMemoryArchive = value;
            }
        }

        /// <summary>
        /// Gets/Sets the file prefix. Can be String.Empty for no prefix.
        /// </summary>
        public string Prefix
        {
            get
            {
                return m_prefix;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    m_prefix = string.Empty;
                    return;
                }
                if (value.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    throw new ArgumentException("filename has invalid characters.", "value");
                m_prefix = value;
            }
        }

        /// <summary>
        /// The list of all available paths to write files to
        /// </summary>
        public List<string> WritePath
        {
            get
            {
                return m_writePath;
            }
        }

        /// <summary>
        /// The extension to name the file.
        /// </summary>
        public string FileExtension
        {
            get
            {
                return m_fileExtension;
            }
            set
            {
                m_fileExtension = PathHelpers.FormatExtension(value);
            }
        }

        /// <summary>
        /// The flags that will be added to any created archive files.
        /// </summary>
        public List<Guid> Flags
        {
            get
            {
                return m_flags;
            }
        }

        /// <summary>
        /// The encoding method that will be used to write files.
        /// </summary>
        public EncodingDefinition EncodingMethod
        {
            get
            {
                return m_encodingMethod;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                m_encodingMethod = value;
            }
        }

        /// <summary>
        /// The desired number of bytes to leave on the disk after a rollover has completed. 
        /// Otherwise, pick a different directory or throw an out of disk space exception.
        /// </summary>
        /// <remarks>
        /// Value must be between 100MB and 1TB
        /// </remarks>
        public long DesiredRemainingSpace
        {
            get
            {
                return m_desiredRemainingSpace;
            }
            set
            {
                if (value < 100 * 1024L * 1024L)
                {
                    m_desiredRemainingSpace = 100 * 1024L * 1024L;
                }
                else if (value > 1024 * 1024L * 1024L * 1024L)
                {
                    m_desiredRemainingSpace = 1024 * 1024L * 1024L * 1024L;
                }
                else
                {
                    m_desiredRemainingSpace = value;
                }
            }
        }

        /// <summary>
        /// Clones the <see cref="ArchiveInitializerSettings"/>
        /// </summary>
        /// <returns></returns>
        public ArchiveInitializerSettings Clone()
        {
            var other = (ArchiveInitializerSettings)MemberwiseClone();
            other.m_flags = m_flags.ToList();
            other.m_writePath = m_writePath.ToList();
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
            settings.IsMemoryArchive = true;
            settings.Flags.AddRange(flags);
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
            settings.IsMemoryArchive = false;
            settings.DirectoryMethod = directoryMethod;
            settings.FileExtension = extension;
            settings.Flags.AddRange(flags);
            settings.Prefix = prefix;
            settings.WritePath.AddRange(paths);
            settings.DesiredRemainingSpace = desiredRemainingSpace;
            settings.EncodingMethod = encodingMethod;
            return settings;
        }
    }
}