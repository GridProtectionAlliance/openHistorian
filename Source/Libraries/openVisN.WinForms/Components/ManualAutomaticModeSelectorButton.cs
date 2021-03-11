//******************************************************************************************************
//  ManualAutomaticModeSelectorButton.cs - Gbtc
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
using System.ComponentModel;
using System.Windows.Forms;
using openVisN.Framework;

namespace openVisN.Components
{
    public partial class ManualAutomaticModeSelectorButton : UserControl
    {
        private VisualizationFramework m_frameworkCtrl;

        public ManualAutomaticModeSelectorButton()
        {
            InitializeComponent();
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
                        //if (m_frameworkCtrl != null)
                        //{
                        //    m_frameworkCtrl.Framework.RemoveSubscriber(this);
                        //}
                        //value.Framework.Updater.SynchronousNewQueryResults += m_framework_SynchronousNewQueryResults;

                        //value.Framework.AddSubscriber(this);
                    }
                }
                m_frameworkCtrl = value;
                if (m_frameworkCtrl.Framework.Updater.Mode == ExecutionMode.Automatic)
                {
                    btnStartStop.Text = "Stop";
                }
                else
                {
                    btnStartStop.Text = "Start";
                }
            }
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (m_frameworkCtrl.Framework.Updater.Mode == ExecutionMode.Automatic)
            {
                m_frameworkCtrl.Framework.Updater.SwitchToManual(true);
                btnStartStop.Text = "Start";
            }
            else
            {
                m_frameworkCtrl.Framework.Updater.SwitchToAutomatic(true);
                btnStartStop.Text = "Stop";
            }
        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_frameworkCtrl.Framework.Updater.PlaybackSpeed = 1;
        }

        private void xToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_frameworkCtrl.Framework.Updater.PlaybackSpeed = 2;
            //2x
        }

        private void xToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            m_frameworkCtrl.Framework.Updater.PlaybackSpeed = 3;
            //3x
        }

        private void xToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            m_frameworkCtrl.Framework.Updater.PlaybackSpeed = 4;
            //4x
        }

        private void xToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            m_frameworkCtrl.Framework.Updater.PlaybackSpeed = 5;
            //5x
        }

        private bool suspendUpdate;

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            suspendUpdate = true;
            toolCustomPlaybackSpeed.Text = m_frameworkCtrl.Framework.Updater.PlaybackSpeed.ToString();
            suspendUpdate = false;
        }

        private void toolCustomPlaybackSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            Save();
        }

        private void Save()
        {
            if (suspendUpdate)
                return;
            if (!double.TryParse(toolCustomPlaybackSpeed.Text, out double speed))
            {
                MessageBox.Show("Value is not a recgonized floating point number.");
                return;
            }
            m_frameworkCtrl.Framework.Updater.PlaybackSpeed = speed;
        }
    }
}