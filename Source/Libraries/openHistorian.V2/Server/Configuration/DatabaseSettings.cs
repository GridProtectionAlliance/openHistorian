//******************************************************************************************************
//  DatabaseSettings.cs - Gbtc
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
//  7/24/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;

namespace openHistorian.V2.Server.Configuration
{
    public class DatabaseSettings
    {
        public ArchiveWriterSettings ArchiveWriter;
        public List<ArchiveRolloverSettings> ArchiveRollovers;
        public List<string> AttachedFiles { get; set; }

        public DatabaseSettings()
        {
            ArchiveRollovers = new List<ArchiveRolloverSettings>();
            AttachedFiles = new List<string>();
        }

        public DatabaseSettings(IDatabaseConfig config)
            : this()
        {
            if (config == null)
                throw new ArgumentNullException("config");
            SetupWriters(config);
            LoadFiles(config);
        }


        void LoadFiles(IDatabaseConfig config)
        {
            foreach (string path in config.Paths.GetPaths())
            {
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.Directory) != 0)
                {
                    AttachedFiles.AddRange(Directory.GetFiles(path, "*.d2", SearchOption.TopDirectoryOnly));
                }
                else if (File.Exists(path))
                {
                    AttachedFiles.Add(path);
                }
            }
        }

        void SetupWriters(IDatabaseConfig config)
        {
            if (!config.Writer.HasValue)
                return;
            if (!config.Writer.Value.IsValid)
                throw new ArgumentException("The writer has not been defined", "config");
            var options = config.Writer.Value;

            if (options.IsInMemoryOnly)
            {
                TimeSpan commitInterval = new TimeSpan((long)(options.MemoryCommitInterval.Value * TimeSpan.TicksPerSecond));
                if (commitInterval.TotalMilliseconds < 1)
                    commitInterval = new TimeSpan(0, 0, 0, 0, 1);
                TimeSpan rollover1 = new TimeSpan(commitInterval.Ticks * 100);

                ArchiveWriter = new ArchiveWriterSettings();
                ArchiveWriter.CommitOnInterval = commitInterval;
                ArchiveWriter.NewFileOnInterval = rollover1;

                ArchiveRolloverSettings rollover = new ArchiveRolloverSettings();
                rollover.Initializer.IsMemoryArchive = true;
                rollover.NewFileOnSize = 1024 * 1024;
                ArchiveRollovers.Add(rollover);

                rollover = new ArchiveRolloverSettings();
                rollover.Initializer.IsMemoryArchive = true;
                rollover.NewFileOnSize = 10 * 1024 * 1024;
                ArchiveRollovers.Add(rollover);
            }
            else
            {
                TimeSpan commitInterval = new TimeSpan((long)(options.MemoryCommitInterval.Value * TimeSpan.TicksPerSecond));
                if (commitInterval.TotalMilliseconds < 1)
                    commitInterval = new TimeSpan(0, 0, 0, 0, 1);

                TimeSpan rolloverIntermediate = new TimeSpan(commitInterval.Ticks * 100);
                TimeSpan rolloverDisk = new TimeSpan((long)(options.DiskCommitInterval.Value * TimeSpan.TicksPerSecond));

                if (rolloverIntermediate < rolloverDisk)
                {
                    //Generate an intermediate rollover archive.
                    ArchiveWriter = new ArchiveWriterSettings();
                    ArchiveWriter.CommitOnInterval = commitInterval;
                    ArchiveWriter.NewFileOnInterval = rolloverIntermediate;

                    ArchiveRolloverSettings rollover = new ArchiveRolloverSettings();
                    rollover.Initializer.IsMemoryArchive = true;
                    rollover.NewFileOnInterval = rolloverDisk;
                    ArchiveRollovers.Add(rollover);

                    rollover = new ArchiveRolloverSettings();
                    rollover.Initializer.IsMemoryArchive = false;
                    rollover.Initializer.Paths.AddRange(config.Paths.GetSavePaths());
                    rollover.NewFileOnSize = 1024 * 1024 * 1024; //1GB

                    ArchiveRollovers.Add(rollover);
                }
                else
                {
                    //No need for an intermediate memory file
                    ArchiveWriter = new ArchiveWriterSettings();
                    ArchiveWriter.CommitOnInterval = commitInterval;
                    ArchiveWriter.NewFileOnInterval = rolloverIntermediate;

                    ArchiveRolloverSettings rollover = new ArchiveRolloverSettings();
                    rollover.Initializer.IsMemoryArchive = false;
                    rollover.Initializer.Paths.AddRange(config.Paths.GetSavePaths());
                    rollover.NewFileOnSize = 1024 * 1024 * 1024; //1GB

                    ArchiveRollovers.Add(rollover);
                }
            }


        }




    }

}
