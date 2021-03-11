//******************************************************************************************************
//  DebugHost.cs - Gbtc
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
//  05/15/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF.TimeSeries;

namespace openHistorian
{
    public class DebugHost : DebugHostBase
    {
        public DebugHost(ServiceHost host)
        {
            this.ServiceHost = host;
            InitializeComponent();
        }

        protected override string ServiceClientName => "openHistorianConsole.exe";

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugHost));
            this.SuspendLayout();
            // 
            // DebugHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(344, 73);
            this.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            this.Name = "DebugHost";
            this.ResumeLayout(false);

        }
    }
}