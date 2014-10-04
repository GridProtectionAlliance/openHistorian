//******************************************************************************************************
//  ServerDatabaseConfigExtensions.cs - Gbtc
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
using System.Linq;
using GSF.SortedTreeStore.Services.Writer;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    public static class ServerDatabaseConfigExtensions
    {

        public static ServerDatabaseSettings ToServerDatabaseSettings(this ServerDatabaseConfig databaseConfig)
        {
            var settings = new ServerDatabaseSettings();
            settings.DatabaseName = databaseConfig.DatabaseName;
            settings.WriteProcessor = databaseConfig.ToWriteProcessorSettings();
            settings.ArchiveList = databaseConfig.ToArchiveListSettings();
            settings.RolloverLog.LogPath = databaseConfig.MainPath;
            return settings;
        }

        /// <summary>
        /// Creates a new <see cref="WriteProcessorSettings"/> from the settings in <see cref="ServerDatabaseConfig"/>
        /// </summary>
        public static WriteProcessorSettings ToWriteProcessorSettings(this ServerDatabaseConfig databaseConfig)
        {
            string intermediateFilePendingExtension;
            string intermediateFileFinalExtension;
            string finalFilePendingExtension;
            string finalFileFinalExtension;

            ValidateExtension(databaseConfig.IntermediateFileExtension, out intermediateFilePendingExtension, out intermediateFileFinalExtension);
            ValidateExtension(databaseConfig.FinalFileExtension, out finalFilePendingExtension, out finalFileFinalExtension);

            if (databaseConfig.WriterMode == WriterMode.InMemory)
            {
                var settings = new WriteProcessorSettings();
                settings.FirstStageWriter.StagingFileSettings.InitialSettings = ArchiveInitializerSettings.CreateInMemory(SortedTree.FixedSizeNode, FileFlags.Stage0);
                settings.FirstStageWriter.StagingFileSettings.FinalSettings = ArchiveInitializerSettings.CreateInMemory(databaseConfig.ArchiveEncodingMethod, FileFlags.Stage1);

                settings.Stage1Rollover.ArchiveSettings = ArchiveInitializerSettings.CreateInMemory(databaseConfig.ArchiveEncodingMethod, FileFlags.Stage2);
                settings.Stage1Rollover.FinalFileExtension = intermediateFileFinalExtension;
                settings.Stage1Rollover.LogPath = databaseConfig.MainPath;
                settings.Stage1Rollover.CombineOnFileCount = 100;
                settings.Stage1Rollover.CombineOnFileSize = 100 * 1024 * 1024;
                settings.Stage1Rollover.MatchFlag = FileFlags.Stage1;

                settings.Stage2Rollover.ArchiveSettings = ArchiveInitializerSettings.CreateInMemory(databaseConfig.ArchiveEncodingMethod, FileFlags.Stage3);
                settings.Stage2Rollover.FinalFileExtension = finalFileFinalExtension;
                settings.Stage2Rollover.LogPath = databaseConfig.MainPath;
                settings.Stage2Rollover.CombineOnFileCount = 100;
                settings.Stage2Rollover.CombineOnFileSize = 1000 * 1024 * 1024;
                settings.Stage2Rollover.MatchFlag = FileFlags.Stage2;

                return settings;
            }
            if (databaseConfig.WriterMode == WriterMode.OnDisk)
            {
                var settings = new WriteProcessorSettings();
                settings.FirstStageWriter.StagingFileSettings.InitialSettings = ArchiveInitializerSettings.CreateInMemory(SortedTree.FixedSizeNode, FileFlags.Stage0);//.StagingFile.Encoding = databaseConfig.ArchiveEncodingMethod;
                settings.FirstStageWriter.StagingFileSettings.FinalSettings = ArchiveInitializerSettings.CreateOnDisk(new string[] { databaseConfig.MainPath }, 1024 * 1024 * 1024, ArchiveDirectoryMethod.TopDirectoryOnly, databaseConfig.ArchiveEncodingMethod, "Stage1", intermediateFilePendingExtension, FileFlags.Stage1);//.StagingFile.Encoding = databaseConfig.ArchiveEncodingMethod;
                settings.FirstStageWriter.StagingFileSettings.FinalFileExtension = intermediateFileFinalExtension;

                settings.Stage1Rollover.ArchiveSettings = ArchiveInitializerSettings.CreateOnDisk(new String[] { databaseConfig.MainPath }, 1024 * 1024 * 1024, ArchiveDirectoryMethod.TopDirectoryOnly, databaseConfig.ArchiveEncodingMethod, "stage2", intermediateFilePendingExtension, FileFlags.Stage2);
                settings.Stage1Rollover.FinalFileExtension = intermediateFileFinalExtension;
                settings.Stage1Rollover.LogPath = databaseConfig.MainPath;
                settings.Stage1Rollover.ExecuteTimer = 1000;
                settings.Stage1Rollover.CombineOnFileCount = 3;
                settings.Stage1Rollover.CombineOnFileSize = 100 * 1024 * 1024;
                settings.Stage1Rollover.MatchFlag = FileFlags.Stage1;

                List<string> finalPaths = new List<string>();
                if (databaseConfig.FinalWritePaths.Count > 0)
                {
                    finalPaths.AddRange(databaseConfig.FinalWritePaths);
                }
                else
                {
                    finalPaths.Add(databaseConfig.MainPath);
                }

                settings.Stage2Rollover.ArchiveSettings = ArchiveInitializerSettings.CreateOnDisk(finalPaths, 5 * 1024L * 1024 * 1024, ArchiveDirectoryMethod.Year, databaseConfig.ArchiveEncodingMethod, "stage3", finalFilePendingExtension, FileFlags.Stage3);
                settings.Stage2Rollover.FinalFileExtension = finalFileFinalExtension;
                settings.Stage2Rollover.LogPath = databaseConfig.MainPath;
                settings.Stage2Rollover.ExecuteTimer = 1000;
                settings.Stage2Rollover.CombineOnFileCount = 3;
                settings.Stage2Rollover.CombineOnFileSize = 1000 * 1024 * 1024;
                settings.Stage2Rollover.MatchFlag = FileFlags.Stage2;
                return settings;
            }

            return null;
        }

        internal static ArchiveListSettings ToArchiveListSettings(this ServerDatabaseConfig databaseConfig)
        {
            string intermediateFilePendingExtension;
            string intermediateFileFinalExtension;
            string finalFilePendingExtension;
            string finalFileFinalExtension;

            ValidateExtension(databaseConfig.IntermediateFileExtension, out intermediateFilePendingExtension, out intermediateFileFinalExtension);
            ValidateExtension(databaseConfig.FinalFileExtension, out finalFilePendingExtension, out finalFileFinalExtension);


            var listSettings = new ArchiveListSettings();
            listSettings.AddExtension(intermediateFileFinalExtension);
            listSettings.AddExtension(finalFileFinalExtension);
            listSettings.AddPath(databaseConfig.MainPath);
            listSettings.AddPaths(databaseConfig.ImportPaths);
            listSettings.AddPaths(databaseConfig.FinalWritePaths);
            listSettings.LogSettings.LogPath = databaseConfig.MainPath;
            return listSettings;
        }

        private static void ValidateExtension(string extension, out string pending, out string final)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Cannot be null or whitespace", "extension");
            extension = extension.Trim();
            if (extension.Contains('.'))
            {
                extension = extension.Substring(extension.IndexOf('.') + 1);
            }
            pending = ".~" + extension;
            final = "." + extension;
        }
    }
}
