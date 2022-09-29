//******************************************************************************************************
//  GlobalSettings.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/19/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;

namespace openHistorian
{
    public class GlobalSettings
    {
        public string CompanyName { get; set; }

        public string CompanyAcronym { get; set; }

        public Guid NodeID { get; set; }

        public string SubscriptionConnectionString { get; set; }

        public string ApplicationName { get; set; }

        public string ApplicationDescription { get; set; }

        public string ApplicationKeywords { get; set; }

        public string DateFormat { get; set; }

        public string TimeFormat { get; set; }

        public string DateTimeFormat { get; set; }

        public string PasswordRequirementsRegex { get; set; }

        public string PasswordRequirementsError { get; set; }

        public string BootstrapTheme { get; set; }

        public string WebRootPath { get; set; }

        public bool GrafanaServerInstalled { get; set; }

        public string GrafanaServerPath { get; set; }

        public bool MASInstalled { get; set; }
        
        public string MASVersion { get; set; }

        public bool PMUConnectionTesterInstalled { get; set; }
        
        public string PMUConnectionTesterVersion { get; set; }

        public bool StreamSplitterInstalled { get; set; }
        
        public string StreamSplitterVersion { get; set; }

        public string DefaultCorsOrigins { get; set; }

        public string DefaultCorsHeaders { get; set; }

        public string DefaultCorsMethods { get; set; }

        public bool DefaultCorsSupportsCredentials { get; set; }

        public int NominalFrequency { get; set; }

        public double DefaultCalculationLagTime { get; set; }

        public double DefaultCalculationLeadTime { get; set; }

        public int DefaultCalculationFramesPerSecond { get; set; }

        public string SystemName { get; set; }

        public bool HasOscEvents { get; set; }

        public string OscDashboard { get; set; }
    }
}
