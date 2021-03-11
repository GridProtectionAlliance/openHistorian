//******************************************************************************************************
//  ReferenceSignalSelectionComboBox.cs - Gbtc
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
using System.Windows.Forms;
using openVisN.Framework;

namespace openVisN.Components
{
    public partial class ReferenceSignalSelectionComboBox : UserControl, ISubscriber
    {
        private MetadataBase m_activeSignal;

        public ReferenceSignalSelectionComboBox()
        {
            InitializeComponent();
            comboBox1.DisplayMember = "DisplayName";
        }

        private VisualizationFramework m_frameworkCtrl;

        private class SignalWrapper
        {
            public readonly SignalGroup Signal;

            public SignalWrapper(SignalGroup signal)
            {
                Signal = signal;
            }

            public string DisplayName => Signal.SignalGroupName;
        }

        public void Initialize(SubscriptionFramework framework)
        {
            //m_frameworkCtrl = framework;
            comboBox1.Items.Clear();
            foreach (SignalGroup signal in framework.AllSignalGroups)
            {
                MetadataBase s = signal.TryGetSignal("Voltage Angle");
                if (s!=null)

                comboBox1.Items.Add(new SignalWrapper(signal));
            }
            comboBox1.Sorted = true;
        }

        public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
        {
            if (m_activeSignal != null)
                activeSignals.Add(m_activeSignal);
        }

        [
            Bindable(true),
            Browsable(true),
            Category("Framework"),
            Description("The framework component that this control will use."),
            DefaultValue(null)
        ]
        public VisualizationFramework Framework
        {
            get => m_frameworkCtrl;
            set
            {
                if (!DesignMode)
                {
                    if (!ReferenceEquals(m_frameworkCtrl, value))
                    {
                        if (m_frameworkCtrl != null)
                        {
                            m_frameworkCtrl.Framework.AddSubscriber(this);
                        }
                        value.Framework.AddSubscriber(this);
                    }
                }
                m_frameworkCtrl = value;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SignalWrapper item = (SignalWrapper)comboBox1.SelectedItem;
            m_activeSignal = item.Signal.TryGetSignal("Voltage Angle");
            m_frameworkCtrl.Framework.SetAngleReference(m_activeSignal);
        }
    }
}