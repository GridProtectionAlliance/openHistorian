//******************************************************************************************************
//  ServiceHost.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
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
//  09/02/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF;
using GSF.Configuration;
using GSF.IO;
using GSF.IO.Unmanaged;
using GSF.TimeSeries;
using GSF.Units;

namespace openHistorian
{
    public class ServiceHost : ServiceHostBase
    {
        public ServiceHost()
        {
            ServiceName = "openHistorian";
        }

        protected override void ServiceStartingHandler(object sender, EventArgs<string[]> e)
        {
            // Handle base class service starting procedures
            base.ServiceStartingHandler(sender, e);

            // Make sure openHistorian specific default service settings exist
            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];

            systemSettings.Add("CompanyName", "Grid Protection Alliance", "The name of the company who owns this instance of the openHistorian.");
            systemSettings.Add("CompanyAcronym", "GPA", "The acronym representing the company who owns this instance of the openHistorian.");
            systemSettings.Add("MemoryPoolSize", "0.0", "The fixed memory pool size. Leave at zero for dynamically calculated setting.");
            systemSettings.Add("MemoryPoolTargetUtilization", "Low", "The target utilization level for the memory pool. One of 'Low', 'Medium', or 'High'.");

            // Set maximum buffer size
            double memoryPoolSize = systemSettings["MemoryPoolSize"].ValueAs(0.0D);

            if (memoryPoolSize > 0.0D)
                GSF.Globals.MemoryPool.SetMaximumBufferSize((long)memoryPoolSize * SI2.Giga);

            TargetUtilizationLevels targetLevel;

            if (!Enum.TryParse(systemSettings["MemoryPoolTargetUtilization"].Value, false, out targetLevel))
                targetLevel = TargetUtilizationLevels.High;

            GSF.Globals.MemoryPool.SetTargetUtilizationLevel(targetLevel);

            // Set default logging path
            GSF.Diagnostics.Logger.SetLoggingPath(FilePath.GetAbsolutePath(""));
        }
    }
}