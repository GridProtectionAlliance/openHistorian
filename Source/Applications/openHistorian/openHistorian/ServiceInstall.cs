//******************************************************************************************************
//  ServiceInstall.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/04/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.ComponentModel;
using System.Xml;
using GSF.TimeSeries;
#if !MONO
using GSF.ServiceProcess;
#endif

namespace openHistorian
{
    [RunInstaller(true)]
    public partial class ServiceInstall : InstallerBase
    {
        public ServiceInstall()
        {
            InitializeComponent();

#if !MONO
            // Reset the service failure count to zero after two minutes
            serviceInstallerEx.FailResetPeriod = 120;

            // Since .NET applications can gracefully fail the service as stopped with an error code, we execute actions on non-crash errors
            serviceInstallerEx.ExecuteActionsOnNonCrashErrors = true;

            // Define one recovery action to restart the service after failure, waiting two seconds before restart
            serviceInstallerEx.DefineRecoverAction(RecoverAction.Restart, 2000);

            // Subsequent recovery actions will require user intervention
            serviceInstallerEx.DefineRecoverAction(RecoverAction.None, 2000);
#endif
        }

        // Define the configuration file name to use for system settings
        protected override string ConfigurationName => "openHistorian.exe.Config";

        // Make sure the default company name and acronym are in the config file under system settings
        protected override void OnSystemSettingsLoaded(XmlDocument configurationFile, XmlNode systemSettingsNode)
        {
            XmlNode companyNameNode = systemSettingsNode.SelectSingleNode("add[@name = 'CompanyName']");
            XmlNode companyAcronymNode = systemSettingsNode.SelectSingleNode("add[@name = 'CompanyAcronym']");

            // Modify or add the CompanyName parameter.
            if (companyNameNode != null)
            {
                companyNameNode.Attributes["value"].Value = Context.Parameters["DP_CompanyName"];
            }
            else
            {
                companyNameNode = configurationFile.CreateNode(XmlNodeType.Element, "add", string.Empty);
                companyNameNode.Attributes.Append(CreateAttribute(configurationFile, "name", "CompanyName"));
                companyNameNode.Attributes.Append(CreateAttribute(configurationFile, "value", Context.Parameters["DP_CompanyName"]));
                companyNameNode.Attributes.Append(CreateAttribute(configurationFile, "description", "The name of the company who owns this instance of the openHistorian."));
                companyNameNode.Attributes.Append(CreateAttribute(configurationFile, "encrypted", "false"));
                systemSettingsNode.AppendChild(companyNameNode);
            }

            // Modify or add the CompanyAcronym parameter.
            if (companyAcronymNode != null)
            {
                companyAcronymNode.Attributes["value"].Value = Context.Parameters["DP_CompanyAcronym"];
            }
            else
            {
                companyAcronymNode = configurationFile.CreateNode(XmlNodeType.Element, "add", string.Empty);
                companyAcronymNode.Attributes.Append(CreateAttribute(configurationFile, "name", "CompanyAcronym"));
                companyAcronymNode.Attributes.Append(CreateAttribute(configurationFile, "value", Context.Parameters["DP_CompanyAcronym"]));
                companyAcronymNode.Attributes.Append(CreateAttribute(configurationFile, "description", "The acronym representing the company who owns this instance of the openHistorian."));
                companyAcronymNode.Attributes.Append(CreateAttribute(configurationFile, "encrypted", "false"));
                systemSettingsNode.AppendChild(companyAcronymNode);
            }
        }

        private XmlAttribute CreateAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute attribute = doc.CreateAttribute(name);
            attribute.Value = value;
            return attribute;
        }
    }
}