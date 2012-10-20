//******************************************************************************************************
//  PartitionInitializerGenerationSettings.cs - Gbtc
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

using openHistorian.V2.Server.Configuration;
using openHistorian.V2.Collections;
namespace openHistorian.V2.Server.Database
{
    public class ArchiveInitializerSettings
    {
        public bool IsMemoryArchive { get; private set; }
        public ReadonlyList<string> Folders{ get; private set; }
        public long InitialSize { get; private set; }
        public long AutoGrowthSize { get; private set; }
        public long RequiredFreeSpaceForNewFile { get; private set; }
        public long RequiredFreeSpaceForAutoGrowth { get; private set; }

        public ArchiveInitializerSettings(ConfigNode node)
        {
            Folders = new ReadonlyList<string>();

            if (bool.Parse(node["IsMemoryArchive","false"]))
            {
                IsMemoryArchive = true;
            }
            else
            {
                IsMemoryArchive = false;
                InitialSize = long.Parse(node["InitialSize"]);
                AutoGrowthSize = long.Parse(node["AutoGrowthSize"]);
                RequiredFreeSpaceForNewFile = long.Parse(node["RequiredFreeSpaceForNewFile"]);
                RequiredFreeSpaceForAutoGrowth = long.Parse(node["RequiredFreeSpaceForAutoGrowth"]);
                foreach (var child in node.GetChildren("FolderList"))
                {
                    Folders.Add(child);
                }
            }
            Folders.IsReadOnly = true;
        }

    }
}
