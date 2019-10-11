﻿//******************************************************************************************************
//  AdvancedServerDatabaseConfig.cs - Gbtc
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
//  11/25/2014 - J. Ritchie Carroll
//       Updated final staging file name to use database name as prefix instead of "stage(n)".
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.IO;
using GSF.Snap.Services.Writer;
using GSF.Snap.Storage;

namespace GSF.Snap.Services.Configuration
{
    /// <summary>
    /// Creates a configuration for the database to utilize.
    /// </summary>
    public class AdvancedServerDatabaseConfig<TKey, TValue>
        : IToServerDatabaseSettings
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private bool m_supportsWriting;
        private string m_databaseName;
        private string m_mainPath;
        private string m_finalFileExtension;
        private string m_intermediateFileExtension;
        private bool m_importAttachedPathsAtStartup;
        private List<string> m_importPaths;
        private List<string> m_finalWritePaths;
        private EncodingDefinition m_archiveEncodingMethod;
        private List<EncodingDefinition> m_streamingEncodingMethods;
        private long m_targetFileSize;
        private int m_stagingCount;
        private ArchiveDirectoryMethod m_directoryMethod;
        private int m_diskFlushInterval;
        private int m_cacheFlushInterval;

        /// <summary>
        /// Gets a database config.
        /// </summary>
        public AdvancedServerDatabaseConfig(string databaseName, string mainPath, bool supportsWriting)
        {
            m_supportsWriting = supportsWriting;
            m_databaseName = databaseName;
            m_mainPath = mainPath;
            m_intermediateFileExtension = ".d2i";
            m_finalFileExtension = ".d2";
            m_importAttachedPathsAtStartup = true;
            m_importPaths = new List<string>();
            m_finalWritePaths = new List<string>();
            m_archiveEncodingMethod = EncodingDefinition.FixedSizeCombinedEncoding;
            m_streamingEncodingMethods = new List<EncodingDefinition>();
            m_targetFileSize = 2 * 1024 * 1024 * 1024L;
            m_stagingCount = 3;
            m_directoryMethod = ArchiveDirectoryMethod.TopDirectoryOnly;
            m_diskFlushInterval = 10000;
            m_cacheFlushInterval = 100;
        }

        /// <summary>
        /// Gets the method of how the directory will be stored. Defaults to 
        /// top directory only.
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
        /// The name associated with the database.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return m_databaseName;
            }
        }

        /// <summary>
        /// The desired size of archive files
        /// </summary>
        public long TargetFileSize
        {
            get
            {
                return m_targetFileSize;
            }
            set
            {
                m_targetFileSize = value;
            }
        }

        /// <summary>
        /// The number of stages.
        /// </summary>
        public int StagingCount
        {
            get
            {
                return m_stagingCount;
            }
            set
            {
                m_stagingCount = value;
            }
        }


        /// <summary>
        /// The extension to use for the intermediate files
        /// </summary>
        public string IntermediateFileExtension
        {
            get
            {
                return m_intermediateFileExtension;
            }
            set
            {
                m_intermediateFileExtension = PathHelpers.FormatExtension(value);
            }
        }

        /// <summary>
        /// The extension to use for the final file
        /// </summary>
        public string FinalFileExtension
        {
            get
            {
                return m_finalFileExtension;
            }
            set
            {
                m_finalFileExtension = PathHelpers.FormatExtension(value);
            }
        }

        /// <summary>
        /// Determines whether the server should import attached paths at startup.
        /// </summary>
        public bool ImportAttachedPathsAtStartup
        {
            get
            {
                return m_importAttachedPathsAtStartup;
            }
            set
            {
                m_importAttachedPathsAtStartup = value;
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
                return m_importPaths;
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
                return m_finalWritePaths;
            }
        }

        /// <summary>
        /// Gets the default encoding methods for storing files.
        /// </summary>
        public EncodingDefinition ArchiveEncodingMethod
        {
            get
            {
                return m_archiveEncodingMethod;
            }
            set
            {
                m_archiveEncodingMethod = value;
            }
        }

        /// <summary>
        /// Gets the supported encoding methods for streaming data. This list is in a prioritized order.
        /// </summary>
        public List<EncodingDefinition> StreamingEncodingMethods
        {
            get
            {
                return m_streamingEncodingMethods;
            }
        }
        /// <summary>
        /// Gets if writing will be supported
        /// </summary>
        public bool SupportsWriting
        {
            get
            {
                return m_supportsWriting;
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
                return m_diskFlushInterval;
            }
            set
            {
                m_diskFlushInterval = value;
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
                return m_cacheFlushInterval;
            }
            set
            {
                m_cacheFlushInterval = value;
            }
        }

        #region [ IToServerDatabaseSettings ]

        public ServerDatabaseSettings ToServerDatabaseSettings()
        {
            var settings = new ServerDatabaseSettings();
            settings.DatabaseName = m_databaseName;
            if (m_supportsWriting)
                ToWriteProcessorSettings(settings.WriteProcessor);
            settings.SupportsWriting = m_supportsWriting;
            ToArchiveListSettings(settings.ArchiveList);
            settings.RolloverLog.LogPath = m_mainPath;
            settings.KeyType = new TKey().GenericTypeGuid;
            settings.ValueType = new TValue().GenericTypeGuid;
            if (m_streamingEncodingMethods.Count == 0)
            {
                settings.StreamingEncodingMethods.Add(EncodingDefinition.FixedSizeCombinedEncoding);
            }
            else
            {
                settings.StreamingEncodingMethods.AddRange(m_streamingEncodingMethods);
            }
            return settings;
        }

        private void ToWriteProcessorSettings(WriteProcessorSettings settings)
        {
            string intermediateFilePendingExtension;
            string intermediateFileFinalExtension;
            string finalFilePendingExtension;
            string finalFileFinalExtension;

            ValidateExtension(IntermediateFileExtension, out intermediateFilePendingExtension, out intermediateFileFinalExtension);
            ValidateExtension(FinalFileExtension, out finalFilePendingExtension, out finalFileFinalExtension);

            if (m_supportsWriting)
            {
                List<string> finalPaths = new List<string>();
                if (FinalWritePaths.Count > 0)
                {
                    finalPaths.AddRange(FinalWritePaths);
                }
                else
                {
                    finalPaths.Add(m_mainPath);
                }

                settings.IsEnabled = true;

                //0.1 seconds
                settings.PrebufferWriter.RolloverInterval = m_cacheFlushInterval;
                settings.PrebufferWriter.MaximumPointCount = 25000;
                settings.PrebufferWriter.RolloverPointCount = 25000;

                //10 seconds
                settings.FirstStageWriter.MaximumAllowedMb = 100; //about 10 million points
                settings.FirstStageWriter.RolloverSizeMb = 100; //about 10 million points
                settings.FirstStageWriter.RolloverInterval = m_diskFlushInterval; //10 seconds
                settings.FirstStageWriter.EncodingMethod = ArchiveEncodingMethod;
                settings.FirstStageWriter.FinalSettings.ConfigureOnDisk(new string[] { m_mainPath }, 1024 * 1024 * 1024, ArchiveDirectoryMethod.TopDirectoryOnly, ArchiveEncodingMethod, "stage1", intermediateFilePendingExtension, intermediateFileFinalExtension, FileFlags.Stage1, FileFlags.IntermediateFile);

                for (int stage = 2; stage <= StagingCount; stage++)
                {
                    int remainingStages = StagingCount - stage;

                    var rollover = new CombineFilesSettings();
                    if (remainingStages > 0)
                    {
                        rollover.ArchiveSettings.ConfigureOnDisk(new string[] { m_mainPath }, 1024 * 1024 * 1024,
                            ArchiveDirectoryMethod.TopDirectoryOnly, ArchiveEncodingMethod, "stage" + stage,
                            intermediateFilePendingExtension, intermediateFileFinalExtension, FileFlags.GetStage(stage), FileFlags.IntermediateFile);
                    }
                    else
                    {
                        // TODO: JRC - Must make desiredRemainingSpace configurable!!

                        //Final staging file
                        rollover.ArchiveSettings.ConfigureOnDisk(finalPaths, 5 * 1024L * 1024 * 1024,
                            m_directoryMethod, ArchiveEncodingMethod, m_databaseName.ToNonNullNorEmptyString("stage" + stage).RemoveInvalidFileNameCharacters(),
                            finalFilePendingExtension, finalFileFinalExtension, FileFlags.GetStage(stage));
                    }

                    rollover.LogPath = m_mainPath;
                    rollover.ExecuteTimer = 1000;
                    rollover.CombineOnFileCount = 60;
                    rollover.CombineOnFileSize = m_targetFileSize / (long)Math.Pow(30, remainingStages);
                    rollover.MatchFlag = FileFlags.GetStage(stage - 1);
                    settings.StagingRollovers.Add(rollover);
                }
            }
        }

        private void ToArchiveListSettings(ArchiveListSettings listSettings)
        {
            string intermediateFilePendingExtension;
            string intermediateFileFinalExtension;
            string finalFilePendingExtension;
            string finalFileFinalExtension;

            ValidateExtension(IntermediateFileExtension, out intermediateFilePendingExtension, out intermediateFileFinalExtension);
            ValidateExtension(FinalFileExtension, out finalFilePendingExtension, out finalFileFinalExtension);


            listSettings.AddExtension(intermediateFileFinalExtension);
            listSettings.AddExtension(finalFileFinalExtension);
            if (!string.IsNullOrWhiteSpace(m_mainPath))
                listSettings.AddPath(m_mainPath);
            if (ImportAttachedPathsAtStartup)
            {
                listSettings.AddPaths(ImportPaths);
                listSettings.AddPaths(FinalWritePaths);
            }
            listSettings.LogSettings.LogPath = m_mainPath;
        }

        private static void ValidateExtension(string extension, out string pending, out string final)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Cannot be null or whitespace", "extension");
            extension = extension.Trim();
            if (extension.Contains("."))
            {
                extension = extension.Substring(extension.IndexOf('.') + 1);
            }
            pending = ".~" + extension;
            final = "." + extension;
        }

        #endregion

    }
}