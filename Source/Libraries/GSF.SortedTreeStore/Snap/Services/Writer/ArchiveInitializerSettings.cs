//******************************************************************************************************
//  ArchiveInitializerSettings.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/01/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using GSF.Immutable;
using GSF.IO;

namespace GSF.Snap.Services.Writer
{
    /// <summary>
    /// Settings for <see cref="ArchiveInitializer{TKey,TValue}"/>.
    /// </summary>
    public class ArchiveInitializerSettings
        : SettingsBase<ArchiveInitializerSettings>
    {
        private ArchiveDirectoryMethod m_directoryMethod;
        private bool m_isMemoryArchive;
        private string m_prefix;
        private string m_fileExtension;
        private long m_desiredRemainingSpace;
        private EncodingDefinition m_encodingMethod;
        private ImmutableList<string> m_writePath;
        private ImmutableList<Guid> m_flags;

        /// <summary>
        /// Creates a new <see cref="ArchiveInitializerSettings"/>
        /// </summary>
        public ArchiveInitializerSettings()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_directoryMethod = ArchiveDirectoryMethod.TopDirectoryOnly;
            m_isMemoryArchive = false;
            m_prefix = string.Empty;
            m_fileExtension = ".d2i";
            m_desiredRemainingSpace = 5 * 1024 * 1024 * 1024L; //5GB
            m_encodingMethod = EncodingDefinition.FixedSizeCombinedEncoding;
            m_writePath = new ImmutableList<string>((x) =>
            {
                PathHelpers.ValidatePathName(x);
                return x;
            });
            m_flags = new ImmutableList<Guid>();
        }

        /// <summary>
        /// Gets the method that the directory structure will follow when writing a new file.
        /// </summary>
        public ArchiveDirectoryMethod DirectoryMethod
        {
            get => m_directoryMethod;
            set
            {
                TestForEditable();
                m_directoryMethod = value;
            }
        }

        /// <summary>
        /// Gets if the archive file is a memory archive or a file archive.
        /// </summary>
        public bool IsMemoryArchive
        {
            get => m_isMemoryArchive;
            set
            {
                TestForEditable();
                m_isMemoryArchive = value;
            }
        }

        /// <summary>
        /// Gets/Sets the file prefix. Can be String.Empty for no prefix.
        /// </summary>
        public string Prefix
        {
            get => m_prefix;
            set
            {
                TestForEditable();
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
        public ImmutableList<string> WritePath => m_writePath;

        /// <summary>
        /// The extension to name the file.
        /// </summary>
        public string FileExtension
        {
            get => m_fileExtension;
            set
            {
                TestForEditable();
                m_fileExtension = PathHelpers.FormatExtension(value);
            }
        }

        /// <summary>
        /// The flags that will be added to any created archive files.
        /// </summary>
        public ImmutableList<Guid> Flags => m_flags;

        /// <summary>
        /// The encoding method that will be used to write files.
        /// </summary>
        public EncodingDefinition EncodingMethod
        {
            get => m_encodingMethod;
            set
            {
                TestForEditable();
                if (value is null)
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
            get => m_desiredRemainingSpace;
            set
            {
                TestForEditable();
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
        /// Creates a <see cref="ArchiveInitializer{TKey,TValue}"/> that will reside in memory.
        /// </summary>
        /// <param name="encodingMethod">the encoding method to use for the archive file.</param>
        /// <param name="flags">flags to include in the archive that is created.</param>
        /// <returns></returns>
        public void ConfigureInMemory(EncodingDefinition encodingMethod, params Guid[] flags)
        {
            TestForEditable();
            Initialize();
            IsMemoryArchive = true;
            Flags.AddRange(flags);
            EncodingMethod = encodingMethod;
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
        public void ConfigureOnDisk(IEnumerable<string> paths, long desiredRemainingSpace, ArchiveDirectoryMethod directoryMethod, EncodingDefinition encodingMethod, string prefix, string extension, params Guid[] flags)
        {
            TestForEditable();
            Initialize();
            IsMemoryArchive = false;
            DirectoryMethod = directoryMethod;
            FileExtension = extension;
            Flags.AddRange(flags);
            Prefix = prefix;
            WritePath.AddRange(paths);
            DesiredRemainingSpace = desiredRemainingSpace;
            EncodingMethod = encodingMethod;
        }

        public override void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write((int)m_directoryMethod);
            stream.Write(m_isMemoryArchive);
            stream.Write(m_prefix);
            stream.Write(m_fileExtension);
            stream.Write(m_desiredRemainingSpace);
            m_encodingMethod.Save(stream);
            stream.Write(m_writePath.Count);
            foreach (string path in m_writePath)
            {
                stream.Write(path);
            }
            stream.Write(m_flags.Count);
            foreach (Guid flag in m_flags)
            {
                stream.Write(flag);
            }
        }

        public override void Load(Stream stream)
        {
            TestForEditable();
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    m_directoryMethod = (ArchiveDirectoryMethod)stream.ReadInt32();
                    m_isMemoryArchive = stream.ReadBoolean();
                    m_prefix = stream.ReadString();
                    m_fileExtension = stream.ReadString();
                    m_desiredRemainingSpace = stream.ReadInt64();
                    m_encodingMethod = new EncodingDefinition(stream);
                    int cnt = stream.ReadInt32();
                    m_writePath.Clear();
                    while (cnt > 0)
                    {
                        cnt--;
                        m_writePath.Add(stream.ReadString());
                    }
                    cnt = stream.ReadInt32();
                    m_flags.Clear();
                    while (cnt > 0)
                    {
                        cnt--;
                        m_flags.Add(stream.ReadGuid());
                    }
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version Code: " + version);

            }
        }

        public override void Validate()
        {
            if (IsMemoryArchive)
            {
                return;
            }
            if (WritePath.Count == 0)
                throw new Exception("Missing write paths.");
            foreach (string path in WritePath)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

    }
}