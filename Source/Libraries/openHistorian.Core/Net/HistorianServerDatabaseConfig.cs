//******************************************************************************************************
//  HistorianServerDatabaseConfig.cs - Gbtc
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
//  10/05/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//  10/15/2019 - J. Ritchie Carroll
//       Added DesiredRemainingSpace property for configurable target disk remaining space for
//       final staging files / general code cleanup.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Snap;
using GSF.Snap.Services;
using GSF.Snap.Services.Configuration;
using GSF.Units;
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
        private readonly AdvancedServerDatabaseConfig<HistorianKey, HistorianValue> m_config;

        /// <summary>
        /// Gets a database config.
        /// </summary>
        public HistorianServerDatabaseConfig(string databaseName, string mainPath, bool supportsWriting)
        {
            m_config = new AdvancedServerDatabaseConfig<HistorianKey, HistorianValue>(databaseName, mainPath, supportsWriting)
            {
                ArchiveEncodingMethod = HistorianFileEncodingDefinition.TypeGuid
            };
            
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
            get => m_config.DirectoryMethod;
            set => m_config.DirectoryMethod = value;
        }

        /// <summary>
        /// Gets or sets the desired size of the final stage archive files.
        /// </summary>
        /// <remarks>Must be between 100MB and 1TB</remarks>
        public long TargetFileSize
        {
            get => m_config.TargetFileSize;
            set
            {
                if (value < 100 * SI2.Mega)
                    throw new ArgumentOutOfRangeException(nameof(value), "Target file size must be between 100MB and 1TB");

                if (value > SI2.Tera)
                    throw new ArgumentOutOfRangeException(nameof(value), "Target file size must be between 100MB and 1TB");

                m_config.TargetFileSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the desired remaining drive space, in bytes, for final stage files.
        /// </summary>
        /// <remarks>Must be between 100MB and 1TB</remarks>
        public long DesiredRemainingSpace
        {
            get => m_config.DesiredRemainingSpace;
            set
            {
                if (value < 100 * SI2.Mega)
                    throw new ArgumentOutOfRangeException(nameof(value), "Desired remaining space must be between 100MB and 1TB");

                if (value > SI2.Tera)
                    throw new ArgumentOutOfRangeException(nameof(value), "Desired remaining space must be between 100MB and 1TB");

                m_config.DesiredRemainingSpace = value;
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
            get => m_config.DiskFlushInterval;
            set
            {
                if (value < 1000 || value > 60000)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be between 1,000 ms and 60,000 ms.");

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
            get => m_config.CacheFlushInterval;
            set
            {
                if (value < 1 || value > 1000)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be between 1 and 1,000");

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
            get => m_config.StagingCount;
            set
            {
                if (value < 3 || value > 4)
                    throw new ArgumentOutOfRangeException(nameof(value), "StagingCount must be 3 or 4");

                m_config.StagingCount = value;
            }
        }

        /// <summary>
        /// The name associated with the database.
        /// </summary>
        public string DatabaseName => m_config.DatabaseName;

        /// <summary>
        /// Determines whether the historian should import attached paths at startup.
        /// </summary>
        public bool ImportAttachedPathsAtStartup
        {
            get => m_config.ImportAttachedPathsAtStartup;
            set => m_config.ImportAttachedPathsAtStartup = value;
        }

        /// <summary>
        /// Gets all of the paths that are known by this historian.
        /// A path can be a file name or a folder.
        /// </summary>
        public List<string> ImportPaths => m_config.ImportPaths;

        /// <summary>
        /// The list of directories where final files can be placed written. 
        /// If nothing is specified, the main directory is used.
        /// </summary>
        public List<string> FinalWritePaths => m_config.FinalWritePaths;

        /// <summary>
        /// Creates a <see cref="ServerDatabaseSettings"/> configuration that can be used for <see cref="SnapServerDatabase{TKey,TValue}"/>
        /// </summary>
        /// <returns></returns>
        public ServerDatabaseSettings ToServerDatabaseSettings() => m_config.ToServerDatabaseSettings();
    }
}