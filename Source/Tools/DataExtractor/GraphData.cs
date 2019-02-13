//******************************************************************************************************
//  GraphData.cs - Gbtc
//
//  Copyright © 2019, Grid Protection Alliance.  All Rights Reserved.
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
//  02/12/2019 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using NPlot;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PlotSurface2D = NPlot.Windows.PlotSurface2D;

namespace DataExtractor
{
    public partial class GraphData : Form
    {
        private bool m_hasData;

        public GraphData()
        {
            InitializeComponent();
        }

        private void GraphData_Load(object sender, EventArgs e)
        {
            plot.AddInteraction(new PlotSurface2D.Interactions.HorizontalDrag());
            plot.AddInteraction(new PlotSurface2D.Interactions.VerticalDrag());
            plot.AddInteraction(new PlotSurface2D.Interactions.AxisDrag(false));
        }

        public void ClearPlots()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(ClearPlots));
            }
            else
            {
                plot.Clear();
                m_hasData = false;
            }
        }

        public void PlotLine(IList<double> times, IList<double> values)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IList<double>, IList<double>>(PlotLine), times, values);
            }
            else
            {
                plot.Add(new LinePlot(values, times));
                m_hasData = true;
            }
        }

        public void RefreshPlots()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshPlots));
            }
            else
            {
                plot.Refresh();
            }
        }

        public bool HasData => m_hasData;
    }
}
