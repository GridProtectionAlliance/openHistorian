//******************************************************************************************************
//  ServerDatabaseConfig.cs - Gbtc
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
//  12/9/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using GSF.IO.FileStructure.Media;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
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
    public class ServerDatabaseConfig
    {
        /// <summary>
        /// Gets the write mode for the server.
        /// </summary>
        public WriterMode WriterMode { get; set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets the main path for this database. 
        /// </summary>
        public string MainPath { get; set; }

        /// <summary>
        /// Gets all of the paths that are known by this historian.
        /// A path can be a file name or a folder.
        /// </summary>
        public List<string> ImportPaths { get; private set; }

        /// <summary>
        /// Gets the default encoding methods for storing files.
        /// </summary>
        public EncodingDefinition EncodingMethod { get; set; }

        /// <summary>
        /// Gets the type of the key componenet
        /// </summary>
        public Guid KeyType { get; set; }

        /// <summary>
        /// Gets the type of the value componenent.
        /// </summary>
        public Guid ValueType { get; set; }

        /// <summary>
        /// Gets a database config.
        /// </summary>
        public ServerDatabaseConfig()
        {
            WriterMode = WriterMode.None;
            DatabaseName = string.Empty;
            MainPath = string.Empty;
            ImportPaths = new List<string>();
            EncodingMethod = CreateFixedSizeCombinedEncoding.TypeGuid;
            KeyType = Guid.Empty;
            ValueType = Guid.Empty;
        }

        public static ServerDatabaseConfig Create<TKey, TValue>(string databaseName, WriterMode writeMode, EncodingDefinition typeGuid, string path)
        {
            var server = new ServerDatabaseConfig();
            server.DatabaseName = databaseName;
            server.WriterMode = writeMode;
            server.EncodingMethod = typeGuid;
            server.MainPath = path;
            return server;
        }
    }

    //public class PathList
    //{
    //    private readonly List<string> m_paths = new List<string>();
    //    private readonly List<string> m_savePaths = new List<string>();

    //    public IEnumerable<string> GetPaths()
    //    {
    //        return m_paths;
    //    }

    //    public IEnumerable<string> GetSavePaths()
    //    {
    //        return m_savePaths;
    //    }

    //    public void AddPath(string path, bool allowWritingToPath)
    //    {
    //        if (allowWritingToPath)
    //        {
    //            if (System.IO.File.Exists(path))
    //                throw new Exception("Only directories can be written to, not files.");
    //            if (!Directory.Exists(path))
    //                throw new ArgumentException("Could not locate the specified directory", "path");
    //        }
    //        else
    //        {
    //            if (!(System.IO.File.Exists(path) || Directory.Exists(path)))
    //                throw new ArgumentException("Could not locate the specified path", "path");
    //        }

    //        m_paths.Add(path);
    //        if (allowWritingToPath)
    //            m_savePaths.Add(path);
    //    }

    //}
}