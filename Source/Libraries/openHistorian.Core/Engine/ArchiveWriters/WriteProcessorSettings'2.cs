//******************************************************************************************************
//  WriteProcessorSettings.cs - Gbtc
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

using System.Collections.Generic;
using System.Linq;
using openHistorian.Collections.Generic;

namespace openHistorian.Engine.ArchiveWriters
{
    public class WriteProcessorSettings<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        public PrestageSettings Prestage;
        public StageWriterSettings<TKey, TValue> Stage0;
        public StageWriterSettings<TKey, TValue> Stage1;
        public StageWriterSettings<TKey, TValue> Stage2;

        private WriteProcessorSettings()
        {
        }

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

            settings.Prestage = new PrestageSettings()
            {
                DelayOnPointCount = 20 * 1000,
                RolloverPointCount = 10 * 1000,
                RolloverInterval = 100
            };

            settings.Stage0 = new StageWriterSettings<TKey, TValue>()
            {
                RolloverInterval = 1000,
                RolloverSize = 1 * 1000 * 1000,
                MaximumAllowedSize = 10 * 1000 * 1000,
                StagingFile = new StagingFile<TKey, TValue>(list, ArchiveInitializer<TKey, TValue>.CreateInMemory(CreateFixedSizeNode.TypeGuid))
            };

            settings.Stage1 = new StageWriterSettings<TKey, TValue>()
            {
                RolloverInterval = 10 * 1000,
                RolloverSize = 10 * 1000 * 1000,
                MaximumAllowedSize = 100 * 1000 * 1000,
                StagingFile = new StagingFile<TKey, TValue>(list, ArchiveInitializer<TKey, TValue>.CreateInMemory(CreateFixedSizeNode.TypeGuid))
            };

            settings.Stage2 = new StageWriterSettings<TKey, TValue>()
            {
                RolloverInterval = 100 * 1000,
                RolloverSize = 100 * 1000 * 1000,
                MaximumAllowedSize = 100 * 1000 * 1000,
                StagingFile = new StagingFile<TKey, TValue>(list, ArchiveInitializer<TKey, TValue>.CreateInMemory(config.CompressionMethod))
            };
            return settings;
        }

        static WriteProcessorSettings<TKey, TValue> CreateOnDisk(DatabaseConfig config, ArchiveList<TKey, TValue> list)
        {
            var paths = config.Paths.GetSavePaths().ToList();
            WriteProcessorSettings<TKey, TValue> settings = new WriteProcessorSettings<TKey, TValue>();

            settings.Prestage = new PrestageSettings()
            {
                DelayOnPointCount = 20 * 1000,
                RolloverPointCount = 10 * 1000,
                RolloverInterval = 100
            };

            settings.Stage0 = new StageWriterSettings<TKey, TValue>()
            {
                RolloverInterval = 1000,
                RolloverSize = 1 * 1000 * 1000,
                MaximumAllowedSize = 10 * 1000 * 1000,
                StagingFile = new StagingFile<TKey, TValue>(list, ArchiveInitializer<TKey, TValue>.CreateInMemory(CreateFixedSizeNode.TypeGuid))
            };

            settings.Stage1 = new StageWriterSettings<TKey, TValue>()
            {
                RolloverInterval = 60 * 1000,
                RolloverSize = 50 * 1000 * 1000,
                MaximumAllowedSize = 200 * 1000 * 1000,
                //StagingFile = new StagingFile(list, ArchiveInitializer.CreateInMemory(CompressionMethod.None))
                StagingFile = new StagingFile<TKey, TValue>(list, ArchiveInitializer<TKey, TValue>.CreateOnDisk(paths, CreateFixedSizeNode.TypeGuid, "Stage1"))
            };

            settings.Stage2 = new StageWriterSettings<TKey, TValue>()
            {
                RolloverInterval = 15 * 60 * 1000,
                RolloverSize = 1000 * 1000 * 1000,
                MaximumAllowedSize = 2000 * 1000 * 1000,
                StagingFile = new StagingFile<TKey, TValue>(list, ArchiveInitializer<TKey, TValue>.CreateOnDisk(paths, config.CompressionMethod, "Stage2"))
            };
            return settings;
        }
    }
}