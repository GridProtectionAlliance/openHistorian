//******************************************************************************************************
//  ResourceEngineSettings.cs - Gbtc
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
//  7/4/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Data;
using System.IO;

namespace openHistorian.V2.Server.Database
{
    class ResourceEngineSettings
    {
        public PartitionInitializerSettings PartitionInitializerSettings;
        public List<string> ActivePartitionFileNames;
        public List<string> ProcessingPartitionFileNames;
        public List<string> PartitionsPendingDeletionsFileNames;
        public List<string> FinalPartitionsFileNames;

        public ResourceEngineSettings()
        {
            PartitionInitializerSettings = new PartitionInitializerSettings();
            ActivePartitionFileNames = new List<string>();
            ProcessingPartitionFileNames = new List<string>();
            PartitionsPendingDeletionsFileNames = new List<string>();
            FinalPartitionsFileNames = new List<string>();
        }

        public ResourceEngineSettings(BinaryReader reader)
        {
            Load(reader);
        }

        public void Load(BinaryReader reader)
        {
            switch (reader.ReadByte())
            {
                case 0:
                    PartitionInitializerSettings = new PartitionInitializerSettings(reader);
                    int count = reader.ReadInt32();
                    ActivePartitionFileNames = new List<string>(count);
                    while (count > 0)
                    {
                        count--;
                        ActivePartitionFileNames.Add(reader.ReadString());
                    }
                    count = reader.ReadInt32();
                    ProcessingPartitionFileNames = new List<string>(count);
                    while (count > 0)
                    {
                        count--;
                        ProcessingPartitionFileNames.Add(reader.ReadString());
                    }
                    count = reader.ReadInt32();
                    PartitionsPendingDeletionsFileNames = new List<string>(count);
                    while (count > 0)
                    {
                        count--;
                        PartitionsPendingDeletionsFileNames.Add(reader.ReadString());
                    }
                    count = reader.ReadInt32();
                    FinalPartitionsFileNames = new List<string>(count);
                    while (count > 0)
                    {
                        count--;
                        FinalPartitionsFileNames.Add(reader.ReadString());
                    }

                    break;
                default:
                    throw new VersionNotFoundException();
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write((byte)0);
            PartitionInitializerSettings.Save(writer);
            writer.Write(ActivePartitionFileNames.Count);
            foreach (var s in ActivePartitionFileNames)
            {
                writer.Write(s);
            }
            writer.Write(ProcessingPartitionFileNames.Count);
            foreach (var s in ProcessingPartitionFileNames)
            {
                writer.Write(s);
            }
            writer.Write(PartitionsPendingDeletionsFileNames.Count);
            foreach (var s in PartitionsPendingDeletionsFileNames)
            {
                writer.Write(s);
            }
            writer.Write(FinalPartitionsFileNames.Count);
            foreach (var s in FinalPartitionsFileNames)
            {
                writer.Write(s);
            }
        }
    }
}
