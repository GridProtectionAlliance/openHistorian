//******************************************************************************************************
//  DatabaseConfig.cs - Gbtc
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
//  12/9/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Tree.TreeNodes;

namespace GSF.SortedTreeStore.Engine
{
    public enum WriterMode
    {
        /// <summary>
        /// Tells the historian that it will not support writing.
        /// </summary>
        None,
        /// <summary>
        /// Tells the historian that any new data written to it will only percist in memory.
        /// </summary>
        InMemory,
        /// <summary>
        /// Tell the historian that new data will be stored on the disk.
        /// </summary>
        OnDisk
    }

    /// <summary>
    /// Creates a configuration for the database to utilize.
    /// </summary>
    public class DatabaseConfig
    {
        public WriterMode WriterMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets all of the paths that are known by this historian.
        /// A path can be a file name or a folder.
        /// </summary>
        public PathList Paths
        {
            get;
            private set;
        }
        public Guid CompressionMethod { get; set; }

        public DatabaseConfig(WriterMode writerMode, params string[] paths)
        {
            CompressionMethod = SortedTree.FixedSizeNode;
            if (writerMode == WriterMode.OnDisk)
                CompressionMethod = CreateHistorianCompressionTs.TypeGuid;

            Paths = new PathList();
            WriterMode = writerMode;
            foreach (string path in paths)
            {
                Paths.AddPath(path, writerMode == WriterMode.OnDisk);
            }
        }

        public List<string> GetAttachedFiles()
        {
            var attachedFiles = new List<string>();

            foreach (string path in Paths.GetPaths())
            {
                if (System.IO.File.Exists(path))
                {
                    attachedFiles.Add(path);
                }
                else if (Directory.Exists(path))
                {
                    attachedFiles.AddRange(Directory.GetFiles(path, "*.d2", SearchOption.TopDirectoryOnly));
                }
            }

            return attachedFiles;
        }
    }

    public class PathList
    {
        private readonly List<string> m_paths = new List<string>();
        private readonly List<string> m_savePaths = new List<string>();

        public IEnumerable<string> GetPaths()
        {
            return m_paths;
        }

        public IEnumerable<string> GetSavePaths()
        {
            return m_savePaths;
        }

        public void AddPath(string path, bool allowWritingToPath)
        {
            if (allowWritingToPath)
            {
                if (System.IO.File.Exists(path))
                    throw new Exception("Only directories can be written to, not files.");
                if (!Directory.Exists(path))
                    throw new ArgumentException("Could not locate the specified directory", "path");
            }
            else
            {
                if (!(System.IO.File.Exists(path) || Directory.Exists(path)))
                    throw new ArgumentException("Could not locate the specified path", "path");
            }

            m_paths.Add(path);
            if (allowWritingToPath)
                m_savePaths.Add(path);
        }

    }
}