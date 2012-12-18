//******************************************************************************************************
//  DatabaseConfig.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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

namespace openHistorian.Engine
{
    /// <summary>
    /// Creates a configuration for the database to utilize.
    /// </summary>
    public class DatabaseConfig
    {
        public WriterOptions? Writer { get; set; }

        /// <summary>
        /// Gets all of the paths that are known by this historian.
        /// A path can be a file name or a folder.
        /// </summary>
        public PathList Paths { get; private set; }

        public DatabaseConfig(WriterOptions? writer, params string[] paths)
        {
            Paths = new PathList();
            Writer = writer;
            foreach (var path in paths)
            {
                Paths.AddPath(path, writer.HasValue);
            }
        }
        public DatabaseConfig()
        {
            Paths = new PathList();
        }
    }

    /// <summary>
    /// Configures how the historian is going to write to the database engine.
    /// </summary>
    public struct WriterOptions
    {
        /// <summary>
        /// IsValid means that the struct was created using one of the initialization functions.
        /// </summary>
        public bool IsValid { get; private set; }
        /// <summary>
        /// Determines if writes to this historian will only 
        /// occur in memory and never commited to the disk.
        /// </summary>
        public bool IsInMemoryOnly { get; private set; }
        /// <summary>
        /// Determines the number of seconds to allow the realtime queue 
        /// to buffer points before forcing them to be added to the historian.
        /// </summary>
        /// <remarks>
        /// This interval controls the maximum lag time between adding a point 
        /// to the historian and it becomes queryable by the user.
        /// 
        /// Setting to null allows the historian to auto configure this parameter.
        /// </remarks>
        public float? MemoryCommitInterval { get; private set; }
        /// <summary>
        /// Determines the number of seconds to allow memory buffers to manage newly 
        /// inserted points before forcing them to be committed to the disk. 
        /// </summary>
        /// <remarks>
        /// Setting this parameter too low can greatly increase the Disk IO. Only one disk
        /// commit can happen at a time per instance. Therefore, if the Disk IO becomes the bottleneck
        /// it will cause the historian to artificially increase this value.
        /// 
        /// Setting this parameter too large can increase the amount of data that will be lost
        /// in an unexpected crash and increase the amount of time it takes to shutdown the historian 
        /// as all these points must be committed to the disk before it will be shutdown.
        /// 
        /// Setting to null allows the historian to auto configure this parameter.
        /// </remarks>
        public float? DiskCommitInterval { get; private set; }
        /// <summary>
        /// Gives the historian an idea of the volume of data that it will be dealing with.
        /// </summary>
        /// <remarks>
        /// This value is used to estimate the volume of streaming data and how large
        /// archive files should be when initialized and autogrown.
        /// 
        /// Setting to null allows the historian to auto configure this parameter.
        /// </remarks>
        public float? OptimalPointsPerSecond { get; private set; }

        public static WriterOptions IsMemoryOnly(float commitInterval = 0.25f, float optimalPointsPerSecond = 8*30*10)
        {
            WriterOptions options = default(WriterOptions);
            options.IsValid = true;
            options.IsInMemoryOnly = true;
            options.MemoryCommitInterval = commitInterval;
            options.DiskCommitInterval = null;
            options.OptimalPointsPerSecond = optimalPointsPerSecond;
            return options;
        }

        public static WriterOptions IsFileBased(float memoryCommitInterval = 0.25f, float diskCommitInterval = 10f, float optimalPointsPerSecond = 8*30*10)
        {
            WriterOptions options = default(WriterOptions);
            options.IsValid = true;
            options.IsInMemoryOnly = false;
            options.MemoryCommitInterval = memoryCommitInterval;
            options.DiskCommitInterval = diskCommitInterval;
            options.OptimalPointsPerSecond = optimalPointsPerSecond;
            return options;
        }
    }

    public class PathList
    {
        List<string> m_paths = new List<string>();
        List<string> m_savePaths = new List<string>();

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
                if (File.Exists(path))
                    throw new Exception("Only directories can be written to, not files.");
                if (!Directory.Exists(path))
                    throw new ArgumentException("Could not locate the specified directory", "path");
            }
            else
            {
                if (!(File.Exists(path) || Directory.Exists(path)))
                    throw new ArgumentException("Could not locate the specified path", "path");
            }

            m_paths.Add(path);
            if (allowWritingToPath)
                m_savePaths.Add(path);
        }
        public void DropPath(string path, float waitTimeSeconds)
        {
            throw new NotImplementedException();
        }
        public void DropSavePath(string path, bool terminateActiveFiles)
        {
            throw new NotImplementedException();
        }
    }
}
