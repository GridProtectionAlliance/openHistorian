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
using openHistorian.Archive;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine.ArchiveWriters
{
    internal class WriteProcessorSettings
    {
        public PrestageSettings Prestage;
        public StageWriterSettings Stage0;
        public StageWriterSettings Stage1;
        public StageWriterSettings Stage2;

        private WriteProcessorSettings()
        {

        }

        public static WriteProcessorSettings CreateFromSettings(DatabaseConfig config, ArchiveList list)
        {
            if (config.Writer.Value.IsInMemoryOnly)
                return CreateInMemory(list);
            else
                return CreateOnDisk(list, config.Paths.GetSavePaths().ToList());
        }

        public static WriteProcessorSettings CreateInMemory(ArchiveList list)
        {
            var settings = new WriteProcessorSettings();

            settings.Prestage = new PrestageSettings()
                {
                    DelayOnPointCount = 20 * 1000,
                    RolloverPointCount = 10 * 1000,
                    RolloverInterval = 100
                };

            settings.Stage0 = new StageWriterSettings()
                {
                    RolloverInterval = 1000,
                    RolloverSize = 1 * 1000 * 1000,
                    MaximumAllowedSize = 10 * 1000 * 1000,
                    StagingFile = new StagingFile(list, ArchiveInitializer.CreateInMemory(CompressionMethod.None))
                };

            settings.Stage1 = new StageWriterSettings()
                {
                    RolloverInterval = 10 * 1000,
                    RolloverSize = 10 * 1000 * 1000,
                    MaximumAllowedSize = 100 * 1000 * 1000,
                    StagingFile = new StagingFile(list, ArchiveInitializer.CreateInMemory(CompressionMethod.None))
                };

            settings.Stage2 = new StageWriterSettings()
                {
                    RolloverInterval = 100 * 1000,
                    RolloverSize = 100 * 1000 * 1000,
                    MaximumAllowedSize = 100 * 1000 * 1000,
                    StagingFile = new StagingFile(list, ArchiveInitializer.CreateInMemory(CompressionMethod.TimeSeriesEncoded2))
                };
            return settings;
        }

        public static WriteProcessorSettings CreateOnDisk(ArchiveList list, List<string> paths)
        {
            var settings = new WriteProcessorSettings();

            settings.Prestage = new PrestageSettings()
                {
                    DelayOnPointCount = 20 * 1000,
                    RolloverPointCount = 10 * 1000,
                    RolloverInterval = 100
                };

            settings.Stage0 = new StageWriterSettings()
                {
                    RolloverInterval = 1000,
                    RolloverSize = 1 * 1000 * 1000,
                    MaximumAllowedSize = 10 * 1000 * 1000,
                    StagingFile = new StagingFile(list, ArchiveInitializer.CreateInMemory(CompressionMethod.None))
                };

            settings.Stage1 = new StageWriterSettings()
                {
                    RolloverInterval = 10 * 1000,
                    RolloverSize = 10 * 1000 * 1000,
                    MaximumAllowedSize = 100 * 1000 * 1000,
                    StagingFile = new StagingFile(list, ArchiveInitializer.CreateOnDisk(paths, CompressionMethod.None, "Stage1"))
                };

            settings.Stage2 = new StageWriterSettings()
                {
                    RolloverInterval = 100 * 1000,
                    RolloverSize = 100 * 1000 * 1000,
                    MaximumAllowedSize = 1000 * 1000 * 1000,
                    StagingFile = new StagingFile(list, ArchiveInitializer.CreateOnDisk(paths, CompressionMethod.TimeSeriesEncoded2, "Stage2"))
                };
            return settings;
        }


    }
}