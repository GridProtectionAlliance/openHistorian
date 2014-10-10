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


        #region [ IToServerDatabaseSettings ]

        ServerDatabaseSettings IToServerDatabaseSettings.ToServerDatabaseSettings()
        {
            var settings = new ServerDatabaseSettings();
            settings.DatabaseName = m_databaseName;
            ToWriteProcessorSettings(settings.WriteProcessor);
            ToArchiveListSettings(settings.ArchiveList);
            settings.RolloverLog.LogPath = m_mainPath;
            settings.KeyType = new TKey().GenericTypeGuid;
            settings.ValueType = new TValue().GenericTypeGuid;
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
                settings.IsEnabled = true;
                //settings.PrebufferWriter.RolloverInterval = 100;
                //settings.PrebufferWriter.MaximumPointCount = 5000;
                //settings.PrebufferWriter.RolloverPointCount = 2000;

                settings.FirstStageWriter.MaximumAllowedMb = 100;
                settings.FirstStageWriter.RolloverSizeMb = 100;
                settings.FirstStageWriter.RolloverInterval = 1000;
                settings.FirstStageWriter.StagingFileSettings.InitialSettings.ConfigureInMemory(SortedTree.FixedSizeNode, FileFlags.Stage0);//.StagingFile.Encoding = databaseConfig.ArchiveEncodingMethod;
                //settings.FirstStageWriter.StagingFileSettings.FinalSettings.ConfigureInMemory(ArchiveEncodingMethod, FileFlags.Stage1);//.StagingFile.Encoding = databaseConfig.ArchiveEncodingMethod;
                settings.FirstStageWriter.StagingFileSettings.FinalSettings.ConfigureOnDisk(new string[] { m_mainPath }, 1024 * 1024 * 1024, ArchiveDirectoryMethod.TopDirectoryOnly, ArchiveEncodingMethod, "Stage1", intermediateFilePendingExtension, FileFlags.Stage1);//.StagingFile.Encoding = databaseConfig.ArchiveEncodingMethod;
                settings.FirstStageWriter.StagingFileSettings.FinalFileExtension = intermediateFileFinalExtension;

                var rollover = new CombineFilesSettings();
                rollover.ArchiveSettings.ConfigureOnDisk(new String[] { m_mainPath }, 1024 * 1024 * 1024, ArchiveDirectoryMethod.TopDirectoryOnly, ArchiveEncodingMethod, "stage2", intermediateFilePendingExtension, FileFlags.Stage2);
                rollover.FinalFileExtension = intermediateFileFinalExtension;
                rollover.LogPath = m_mainPath;
                rollover.ExecuteTimer = 1000;
                rollover.CombineOnFileCount = 10;
                rollover.CombineOnFileSize = 100 * 1024 * 1024;
                rollover.MatchFlag = FileFlags.Stage1;
                settings.StagingRollovers.Add(rollover);

                List<string> finalPaths = new List<string>();
                if (FinalWritePaths.Count > 0)
                {
                    finalPaths.AddRange(FinalWritePaths);
                }
                else
                {
                    finalPaths.Add(m_mainPath);
                }

                rollover = new CombineFilesSettings();
                rollover.ArchiveSettings.ConfigureOnDisk(finalPaths, 5 * 1024L * 1024 * 1024, ArchiveDirectoryMethod.Year, ArchiveEncodingMethod, "stage3", finalFilePendingExtension, FileFlags.Stage3);
                rollover.FinalFileExtension = finalFileFinalExtension;
                rollover.LogPath = m_mainPath;
                rollover.ExecuteTimer = 1000;
                rollover.CombineOnFileCount = 10;
                rollover.CombineOnFileSize = 1000 * 1024 * 1024;
                rollover.MatchFlag = FileFlags.Stage2;
                settings.StagingRollovers.Add(rollover);

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