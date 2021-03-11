//******************************************************************************************************
//  SetTimeWindowCalendarControl.cs - Gbtc
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
    public partial class SetTimeWindowCalendarControl : UserControl, ISubscriber
    {
        private VisualizationFramework m_frameworkCtrl;

        public SetTimeWindowCalendarControl()
        {
            InitializeComponent();
        }

        private bool m_suspendEvent;

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
                            m_frameworkCtrl.Framework.RemoveSubscriber(this);
                        }
                        value.Framework.Updater.SynchronousNewQueryResults += m_framework_SynchronousNewQueryResults;

                        value.Framework.AddSubscriber(this);
                    }
                }
                m_frameworkCtrl = value;
            }
        }

        public void Initialize(SubscriptionFramework framework)
        {
        }

        public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
        {
        }

        private void m_framework_SynchronousNewQueryResults(object sender, QueryResultsEventArgs e)
        {
            m_suspendEvent = true;
            monthCalendar1.SetSelectionRange(e.StartTime.Date, e.EndTime.Date);
            m_suspendEvent = false;
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (m_suspendEvent)
                return;
            m_frameworkCtrl.Framework.ChangeDateRange(e.Start, e.End.Date.AddDays(1));
        }
    }
}