//******************************************************************************************************
//  ArchiveListSettings.cs - Gbtc
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
//  10/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using GSF.Immutable;
using GSF.IO;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Settings for <see cref="ArchiveList{TKey,TValue}"/>
    /// </summary>
    public class ArchiveListSettings
        : SettingsBase<ArchiveListSettings>
    {
        private readonly ImmutableList<string> m_importPaths;
        private readonly ImmutableList<string> m_importExtensions;
        private readonly ArchiveListLogSettings m_logSettings;

        /// <summary>
        /// Creates a new instance of <see cref="ArchiveListSettings"/>
        /// </summary>
        public ArchiveListSettings()
        {
            m_importPaths = new ImmutableList<string>(x =>
            {
                PathHelpers.ValidatePathName(x);
                return x;
            });
            m_importExtensions = new ImmutableList<string>(PathHelpers.FormatExtension);
            m_logSettings = new ArchiveListLogSettings();
        }

        /// <summary>
        /// The log settings to use for logging deletions.
        /// </summary>
        public ArchiveListLogSettings LogSettings => m_logSettings;

        /// <summary>
        /// A set of all import paths to load upon initialization.
        /// Be sure to include all paths that existed last time the service
        /// restarted since the ArchiveListLog processes immediately upon 
        /// construction.
        /// </summary>
        public IEnumerable<string> ImportPaths => m_importPaths;

        /// <summary>
        /// A set of all file extensions that will need to be loaded from each path.
        /// </summary>
        public IEnumerable<string> ImportExtensions => m_importExtensions;

        /// <summary>
        /// Adds the supplied path to the list.
        /// </summary>
        /// <param name="path">the path to add.</param>
        public void AddPath(string path)
        {
            TestForEditable();
            PathHelpers.ValidatePathName(path);
            if (!m_importPaths.Contains(path, StringComparer.Create(CultureInfo.CurrentCulture, true)))
            {
                m_importPaths.Add(path);
            }
        }

        /// <summary>
        /// Adds the supplied paths to the list.
        /// </summary>
        /// <param name="paths">the paths to add.</param>
        public void AddPaths(IEnumerable<string> paths)
        {
            TestForEditable();
            foreach (string p in paths)
            {
                AddPath(p);
            }
        }

        /// <summary>
        /// Adds the supplied extension to the list.
        /// </summary>
        /// <param name="extension">the extension to add.</param>
        public void AddExtension(string extension)
        {
            TestForEditable();
            extension = PathHelpers.FormatExtension(extension);
            if (!m_importExtensions.Contains(extension, StringComparer.Create(CultureInfo.CurrentCulture, true)))
            {
                m_importExtensions.Add(extension);
            }
        }

        public override void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(m_importPaths.Count);
            foreach (string path in m_importPaths)
            {
                stream.Write(path);
            }
            stream.Write(m_importExtensions.Count);
            foreach (string extensions in m_importExtensions)
            {
                stream.Write(extensions);
            }
            m_logSettings.Save(stream);
        }

        public override void Load(Stream stream)
        {
            TestForEditable();
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    int cnt = stream.ReadInt32();
                    m_importPaths.Clear();
                    while (cnt > 0)
                    {
                        cnt--;
                        m_importPaths.Add(stream.ReadString());
                    }
                    cnt = stream.ReadInt32();
                    m_importPaths.Clear();
                    while (cnt > 0)
                    {
                        cnt--;
                        m_importPaths.Add(stream.ReadString());
                    }
                    m_logSettings.Load(stream);
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version Code: " + version);

            }
        }

        public override void Validate()
        {
            if (m_importPaths.Count>0 && m_importExtensions.Count == 0)
                throw new Exception("Path specified but no extension specified.");
            m_logSettings.Validate();

           
        }
    }
}
