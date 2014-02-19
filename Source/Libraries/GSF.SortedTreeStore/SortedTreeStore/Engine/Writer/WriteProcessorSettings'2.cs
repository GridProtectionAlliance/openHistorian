//******************************************************************************************************
//  WriteProcessorSettings`2.cs - Gbtc
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
//  1/21/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System.Linq;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{
    /// <summary>
    /// Responsible for the settings that are used for writing
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class WriteProcessorSettings<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        /// <summary>
        /// The settings for the precommitted stage
        /// </summary>
        public PrestageSettings Prestage;
        /// <summary>
        /// The initial committed stage (usually in memory)
        /// </summary>
        public FirstStageWriterSettings<TKey, TValue> Stage0;
        
        /// <summary>
        /// The first stange of data writen to the disk. Usually uncompressed since random inserts are still fast.
        /// </summary>
        public CombineFilesSettings<TKey, TValue> Stage1;

        /// <summary>
        /// The first stange of data writen to the disk. Usually uncompressed since random inserts are still fast.
        /// </summary>
        public CombineFilesSettings<TKey, TValue> Stage2;

        /// <summary>
        /// Forces the class to only be constructed via static methods.
        /// </summary>
        private WriteProcessorSettings()
        {
        }

        /// <summary>
        /// Converts a database config into the processor settings that will be used by the archivewriter.
        /// </summary>
        /// <param name="config">the config base to use</param>
        /// <param name="list">where to </param>
        /// <returns></returns>
        public static WriteProcessorSettings<TKey, TValue> CreateFromSettings(DatabaseConfig config, ArchiveList<TKey, TValue> list)
        {
            if (config.WriterMode == WriterMode.InMemory)
                return CreateInMemory(config, list);
            else
                return CreateOnDisk(config, list);
        }

        static WriteProcessorSettings<TKey, TValue> CreateInMemory(DatabaseConfig config, ArchiveList<TKey, TValue> list)
        {
            WriteProcessorSettings<TKey, TValue> settings = new WriteProcessorSettings<TKey, TValue>();

            settings.Prestage = new PrestageSettings
            {
                DelayOnPointCount = 20 * 1000,
                RolloverPointCount = 10 * 1000,
                RolloverInterval = 100
            };


            settings.Stage0 = new FirstStageWriterSettings<TKey, TValue>
            {
                RolloverInterval = 1000,
                RolloverSize = 1 * 1000 * 1000,
                MaximumAllowedSize = 10 * 1000 * 1000,
                TempFile = new TempFile<TKey, TValue>(list, ArchiveInitializer<TKey, TValue>.CreateInMemory(SortedTree.FixedSizeNode), ArchiveInitializer<TKey, TValue>.CreateInMemory(config.CompressionMethod))
            };

            settings.Stage1 = new CombineFilesSettings<TKey, TValue>
                {
                    ArchiveList = list,
                    TargetSize = 100 * 1014 * 1024,
                    CreateNextStageFile = ArchiveInitializer<TKey, TValue>.CreateInMemory(config.CompressionMethod),
                    NameMatch = "-Stage1-"
                };

            settings.Stage2 = new CombineFilesSettings<TKey, TValue>
            {
                ArchiveList = list,
                TargetSize = 100 * 1014 * 1024,
                CreateNextStageFile = ArchiveInitializer<TKey, TValue>.CreateInMemory(config.CompressionMethod),
                NameMatch = "-Stage2-"
            };

            return settings;
        }

        static WriteProcessorSettings<TKey, TValue> CreateOnDisk(DatabaseConfig config, ArchiveList<TKey, TValue> list)
        {
            var paths = config.Paths.GetSavePaths().ToList();
            WriteProcessorSettings<TKey, TValue> settings = new WriteProcessorSettings<TKey, TValue>();

            settings.Prestage = new PrestageSettings
            {
                DelayOnPointCount = 20 * 1000,
                RolloverPointCount = 10 * 1000,
                RolloverInterval = 100
            };

            settings.Stage0 = new FirstStageWriterSettings<TKey, TValue>
            {
                RolloverInterval = 10000,
                RolloverSize = 100 * 1000 * 1000,
                MaximumAllowedSize = 200 * 1000 * 1000,
                TempFile = new TempFile<TKey, TValue>(list, ArchiveInitializer<TKey, TValue>.CreateInMemory(SortedTree.FixedSizeNode), ArchiveInitializer<TKey, TValue>.CreateOnDisk(paths, config.CompressionMethod, "Stage1"))
            };

            settings.Stage1 = new CombineFilesSettings<TKey, TValue>
            {
                ArchiveList = list,
                TargetSize = 50 * 1014 * 1024,
                CreateNextStageFile = ArchiveInitializer<TKey, TValue>.CreateOnDisk(paths, config.CompressionMethod, "Stage2"),
                NameMatch = "-Stage1-"
            };

            settings.Stage2 = new CombineFilesSettings<TKey, TValue>
            {
                ArchiveList = list,
                TargetSize = 1000 * 1014 * 1024,
                CreateNextStageFile = ArchiveInitializer<TKey, TValue>.CreateOnDisk(paths, config.CompressionMethod, "Stage3"),
                NameMatch = "-Stage2-"
            };

            return settings;
        }
    }
}