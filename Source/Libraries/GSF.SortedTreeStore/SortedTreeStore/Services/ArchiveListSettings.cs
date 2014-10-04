//******************************************************************************************************
//  ArchiveListSettings.cs - Gbtc
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
//  10/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GSF.IO;
using GSF.SortedTreeStore.Services.Writer;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Settings for <see cref="ArchiveList{TKey,TValue}"/>
    /// </summary>
    public class ArchiveListSettings
    {
        private ArchiveListLogSettings m_logSettings = new ArchiveListLogSettings();
        private List<string> m_importPaths = new List<string>();
        private List<string> m_importExtensions = new List<string>();


        /// <summary>
        /// The log settings to use for logging deletions.
        /// </summary>
        public ArchiveListLogSettings LogSettings
        {
            get
            {
                return m_logSettings;
            }
        }

        /// <summary>
        /// A set of all import paths to load upon initialization.
        /// Be sure to include all paths that existed last time the service
        /// restarted since the ArchiveListLog processes immediately upon 
        /// construction.
        /// </summary>
        public IEnumerable<string> ImportPaths
        {
            get
            {
                return m_importPaths;
            }
        }

        /// <summary>
        /// A set of all file extensions that will need to be loaded from each path.
        /// </summary>
        public IEnumerable<string> ImportExtensions
        {
            get
            {
                return m_importExtensions;
            }
        }

        /// <summary>
        /// Adds the supplied path to the list.
        /// </summary>
        /// <param name="path">the path to add.</param>
        public void AddPath(string path)
        {
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
            foreach (var p in paths)
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
            extension = PathHelpers.FormatExtension(extension);
            if (!m_importExtensions.Contains(extension, StringComparer.Create(CultureInfo.CurrentCulture, true)))
            {
                m_importExtensions.Add(extension);
            }
        }

        /// <summary>
        /// Creates a clone of this class.
        /// </summary>
        /// <returns></returns>
        public ArchiveListSettings Clone()
        {
            var obj = (ArchiveListSettings)MemberwiseClone();
            obj.m_logSettings = m_logSettings.Clone();
            obj.m_importPaths = m_importPaths.ToList();
            obj.m_importExtensions = m_importExtensions.ToList();
            return obj;
        }
    }
}
