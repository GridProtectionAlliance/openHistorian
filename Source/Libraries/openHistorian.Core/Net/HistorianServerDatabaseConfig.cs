//******************************************************************************************************
//  HistorianServerDatabaseConfig.cs - Gbtc
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
//  10/05/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Snap;
using GSF.Snap.Definitions;
using GSF.Snap.Encoding;
using GSF.Snap.Services;
using GSF.Snap.Services.Configuration;
using GSF.Snap.Tree;
using openHistorian.Snap;
using openHistorian.Snap.Definitions;

namespace openHistorian.Net
{
    /// <summary>
    /// Creates a configuration for the database to utilize.
    /// </summary>
    public class HistorianServerDatabaseConfig
        : IToServerDatabaseSettings
    {
        private AdvancedServerDatabaseConfig<HistorianKey, HistorianValue> m_config;

        /// <summary>
        /// Gets a database config.
        /// </summary>
        public HistorianServerDatabaseConfig(string databaseName, string mainPath, bool supportsWriting)
        {
            m_config = new AdvancedServerDatabaseConfig<HistorianKey, HistorianValue>(databaseName, mainPath, supportsWriting);
            m_config.ArchiveEncodingMethod = HistorianFileEncodingDefinition.TypeGuid;
            m_config.StreamingEncodingMethods.Add(HistorianStreamEncodingDefinition.TypeGuid);
            m_config.StreamingEncodingMethods.Add(EncodingDefinition.FixedSizeCombinedEncoding);
            m_config.IntermediateFileExtension = ".d2i";
            m_config.FinalFileExtension = ".d2";
            m_config.DirectoryMethod = ArchiveDirectoryMethod.YearThenMonth;
        }

        /// <summary>
        /// Specify how archive files will be written into the final directory.
        /// </summary>
        public ArchiveDirectoryMethod DirectoryMethod
        {
            get
            {
                return m_config.DirectoryMethod;
            }
            set
            {
                m_config.DirectoryMethod = value;
            }
        }

        /// <summary>
        /// The desired size of archive files
        /// </summary>
        /// <remarks>Must be between 100MB and 1TB</remarks>
        public long TargetFileSize
        {
            get
            {
                return m_config.TargetFileSize;
            }
            set
            {
                if (value < 100 * 1024 * 1024)
                {
                    throw new ArgumentOutOfRangeException("value", "Target size must be between 100MB and 1TB");
                }
                if (value > 1 * 1024 * 1024 * 1024 * 1024L)
                {
                    throw new ArgumentOutOfRangeException("value", "Target size must be between 100MB and 1TB");
                }
                m_config.TargetFileSize = value;
            }
        }

        /// <summary>
        /// The number of milliseconds before data is automatically flushed to the disk.
        /// </summary>
        /// <remarks>
        /// Must be between 1,000 ms and 60,000 ms.
        /// </remarks>
        public int DiskFlushInterval
        {
            get
            {
                return m_config.DiskFlushInterval;
            }
            set
            {
                if (value < 1000 || value > 60000)
                    throw new ArgumentOutOfRangeException("value", "Must be between 1,000 ms and 60,000 ms.");
                m_config.DiskFlushInterval = value;
            }
        }

        /// <summary>
        /// The number of milliseconds before data is taken from it's cache and put in the
        /// memory file.
        /// </summary>
        /// <remarks>
        /// Must be between 1 and 1,000
        /// </remarks>
        public int CacheFlushInterval
        {
            get
            {
                return m_config.CacheFlushInterval;
            }
            set
            {
                if (value < 1 || value > 1000)
                    throw new ArgumentOutOfRangeException("value", "Must be between 1 and 1,000");
                m_config.CacheFlushInterval = value;
            }
        }

        /// <summary>
        /// The number of stages to progress through before writing the final file.
        /// </summary>
        /// <remarks>
        /// This defaults to 3 stages which allows files up to 10 hours of data to be combined
        /// into a single archive file. If <see cref="TargetFileSize"/> is large and files of this
        /// size are not being created, increase this to 4. 
        /// 
        /// Valid settings are 3 or 4.
        /// </remarks>
        public int StagingCount
        {
            get
            {
                return m_config.StagingCount;
            }
            set
            {
                if (value < 3 || value > 4)
                    throw new ArgumentOutOfRangeException("value", "StagingCount must be 3 or 4");
                m_config.StagingCount = value;
            }
        }

        /// <summary>
        /// The name associated with the database.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return m_config.DatabaseName;
            }
        }

        /// <summary>
        /// Gets all of the paths that are known by this historian.
        /// A path can be a file name or a folder.
        /// </summary>
        public List<string> ImportPaths
        {
            get
            {
                return m_config.ImportPaths;
            }
        }

        /// <summary>
        /// The list of directories where final files can be placed written. 
        /// If nothing is specified, the main directory is used.
        /// </summary>
        public List<string> FinalWritePaths
        {
            get
            {
                return m_config.FinalWritePaths;
            }
        }

        /// <summary>
        /// Creates a <see cref="ServerDatabaseSettings"/> configuration that can be used for <see cref="SnapServerDatabase{TKey,TValue}"/>
        /// </summary>
        /// <returns></returns>
        public ServerDatabaseSettings ToServerDatabaseSettings()
        {
            return m_config.ToServerDatabaseSettings();
        }
    }
}