//******************************************************************************************************
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
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.IO;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Services.Writer;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Configuration
{
    /// <summary>
    /// Creates a configuration for the database to utilize.
    /// </summary>
    public class AdvancedServerDatabaseConfig<TKey, TValue>
        : IToServerDatabaseSettings
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private bool m_supportsWriting;
        private string m_databaseName;
        private string m_mainPath;
        private string m_finalFileExtension;
        private string m_intermediateFileExtension;
        private List<string> m_importPaths;
        private List<string> m_finalWritePaths;
        private EncodingDefinition m_archiveEncodingMethod;
        private List<EncodingDefinition> m_streamingEncodingMethods;
        private long m_targetFileSize;
        private int m_stagingCount;

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
            m_importPaths = new List<string>();
            m_finalWritePaths = new List<string>();
            m_archiveEncodingMethod = CreateFixedSizeCombinedEncoding.TypeGuid;
            m_streamingEncodingMethods = new List<EncodingDefinition>();
            m_targetFileSize = 2 * 1024 * 1024 * 1024L;
            m_stagingCount = 3;
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
                settings.StreamingEncodingMethods.Add(SortedTree.FixedSizeNode);
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
                settings.PrebufferWriter.RolloverInterval = 100;
                settings.PrebufferWriter.MaximumPointCount = 25000;
                settings.PrebufferWriter.RolloverPointCount = 25000;

                //10 seconds
                settings.FirstStageWriter.MaximumAllowedMb = 100; //about 10 million points
                settings.FirstStageWriter.RolloverSizeMb = 100; //about 10 million points
                settings.FirstStageWriter.RolloverInterval = 10000; //10 seconds
                settings.FirstStageWriter.EncodingMethod = ArchiveEncodingMethod;
                settings.FirstStageWriter.FinalSettings.ConfigureOnDisk(new string[] { m_mainPath }, 1024 * 1024 * 1024, ArchiveDirectoryMethod.TopDirectoryOnly, ArchiveEncodingMethod, "Stage1", intermediateFilePendingExtension, intermediateFileFinalExtension, FileFlags.Stage1);

                for (int stage = 2; stage < StagingCount; stage++)
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
                        //Final staging file
                        rollover.ArchiveSettings.ConfigureOnDisk(finalPaths, 5 * 1024L * 1024 * 1024,
                            ArchiveDirectoryMethod.YearMonth, ArchiveEncodingMethod, "stage" + stage,
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
            listSettings.AddPaths(ImportPaths);
            listSettings.AddPaths(FinalWritePaths);
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