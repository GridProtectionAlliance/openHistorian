//******************************************************************************************************
//  VisualizationFramework.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  12/15/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using openVisN.Framework;

namespace openVisN.Components
{
    public partial class VisualizationFramework : Component
    {
        private readonly SubscriptionFramework m_framework;
        private string m_server = string.Empty;
        private string m_database = string.Empty;
        private int m_port;
        private bool m_useNetworkHistorian;
        private bool m_enabled;
        private List<ISubscriber> m_pendingSubscribers = new List<ISubscriber>();

        private string[] m_localDirectories = new string[0];

        public VisualizationFramework()
        {
            InitializeComponent();

            m_framework = new SubscriptionFramework();
            Disposed += VisualizationFramework_Disposed;
        }

        private void VisualizationFramework_Disposed(object sender, EventArgs e)
        {
            m_framework.Dispose();
        }

        public VisualizationFramework(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            m_framework = new SubscriptionFramework();
            Disposed += VisualizationFramework_Disposed;
        }

        public void Start()
        {
            if (UseNetworkHistorian)
            {
                m_framework.Start(Server, Port, Database);
            }
            else
            {
                m_framework.Start(Paths);
            }
        }

        public void Stop()
        {
        }

        public SubscriptionFramework Framework => m_framework;

        [
            Bindable(true),
            Browsable(true),
            Category("Communications"),
            Description("Determines whether the framework is connected to the historian"),
            DefaultValue(false)
        ]
        public bool Enabled
        {
            get => m_enabled;
            set => m_enabled = value;
        }

        [
            Bindable(true),
            Browsable(true),
            Category("Communications"),
            Description("When using a local historian, these are the paths that it will use to connect."),
            DefaultValue("")
        ]
        public string[] Paths
        {
            get => m_localDirectories;
            set => m_localDirectories = value;
        }

        [
            Bindable(true),
            Browsable(true),
            Category("Communications"),
            Description("Determines whether to connect remotely to a historian, or create a local one"),
            DefaultValue(true)
        ]
        public bool UseNetworkHistorian
        {
            get => m_useNetworkHistorian;
            set => m_useNetworkHistorian = value;
        }

        [
            Browsable(true),
            Bindable(true),
            Description("Contains the server hostname or IP address that contains the historian."),
            Category("Communications"),
            DefaultValue("")
        ]
        public string Server
        {
            get => m_server;
            set => m_server = value;
        }

        [
            Browsable(true),
            Bindable(true),
            Description("Contains the port to communicate to the historian on."),
            Category("Communications"),
            DefaultValue(0)
        ]
        public int Port
        {
            get => m_port;
            set => m_port = value;
        }

        [
           Browsable(true),
           Bindable(true),
           Description("Contains the database instance name of the historian"),
           Category("Communications"),
           DefaultValue("")
       ]
        public string Database
        {
            get => m_database;
            set => m_database = value;
        }
    }
}