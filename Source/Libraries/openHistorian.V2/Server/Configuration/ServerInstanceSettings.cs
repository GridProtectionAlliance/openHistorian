//******************************************************************************************************
//  ServerInstanceSettings.cs - Gbtc
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

using System.Collections.Generic;
using System.Data;
using System.IO;
using openHistorian.V2.Collections;

namespace openHistorian.V2.Server.Configuration
{
    public class ServerInstanceSettings
    {
        public ReadonlySortedList<string, ArchiveManagementSystemSettings> Databases;

        public ServerInstanceSettings(ConfigNode node)
        {
            Databases = new ReadonlySortedList<string, ArchiveManagementSystemSettings>();
            foreach (var instances in node.GetChildren("Instance"))
            {
                var ams = new ArchiveManagementSystemSettings(instances);
                Databases.Add(ams.Name.ToLower(), ams);
            }
            Databases.IsReadOnly = true;
        }

    }
}
