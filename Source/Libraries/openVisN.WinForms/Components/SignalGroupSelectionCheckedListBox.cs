//******************************************************************************************************
//  SignalGroupSelectionCheckedListBox.cs - Gbtc
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


using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using openVisN.Framework;

namespace openVisN.Components
{
    public partial class SignalGroupSelectionCheckedListBox : UserControl, ISubscriber
    {
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

        public SignalGroupSelectionCheckedListBox()
        {
            InitializeComponent();
            chkAllSignals.DisplayMember = "DisplayName";
        }


        public void Initialize(SubscriptionFramework framework)
        {
            //m_frameworkCtrl = framework;
            chkAllSignals.Items.Clear();
            foreach (SignalGroup signal in framework.AllSignalGroups)
            {
                chkAllSignals.Items.Add(new SignalWrapper(signal));
            }
            chkAllSignals.Sorted = true;
        }

        public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
        {
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

        private void chkAllSignals_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SignalWrapper item = (SignalWrapper)chkAllSignals.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                m_frameworkCtrl.Framework.ActivateSignalGroup(item.Signal);
            }
            else
            {
                m_frameworkCtrl.Framework.DeactivateSignalGroup(item.Signal);
            }
        }
    }
}