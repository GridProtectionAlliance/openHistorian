//******************************************************************************************************
//  SignalPlot.cs - Gbtc
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
using NPlot;
using openHistorian.Data.Query;
using openVisN.Framework;
using PlotSurface2D = NPlot.Windows.PlotSurface2D;

namespace openVisN.Components
{
    public partial class SignalPlots : UserControl, ISubscriber
    {
        private bool m_ignoreNextUpdate;
        private readonly object m_token = new object();
        private double m_scalingFactor = 1;
        private string m_signalTypeToPlot = "";
        private string m_plotTitle = "";
        private ColorWheel m_colorWheel;

        private readonly List<KeyValuePair<int, Guid>> m_signals = new List<KeyValuePair<int, Guid>>();

        private VisualizationFramework m_frameworkCtrl;

        private readonly PlotSurface2D.Interactions.HorizontalDrag HorizontalDrag = new PlotSurface2D.Interactions.HorizontalDrag();
        private readonly PlotSurface2D.Interactions.VerticalDrag VerticalDrag = new PlotSurface2D.Interactions.VerticalDrag();
        private readonly PlotSurface2D.Interactions.AxisDragX AxisDragX = new PlotSurface2D.Interactions.AxisDragX(true);
        private readonly PlotSurface2D.Interactions.AxisDragY AxisDragY = new PlotSurface2D.Interactions.AxisDragY(true);
        private readonly PlotSurface2D.Interactions.MouseWheelZoom MouseWheelZoom = new PlotSurface2D.Interactions.MouseWheelZoom();

        public SignalPlots()
        {
            InitializeComponent();
            plotSurface2D1.InteractionOccured += PlotSurface2D1_InteractionOccured;
            plotSurface2D1.DateTimeToolTip = true;
        }

        public void Initialize(SubscriptionFramework framework)
        {
            //chkAllSignals.Items.Clear();
            //foreach (var signal in framework.AllSignalGroups)
            //{
            //    chkAllSignals.Items.Add(new SignalWrapper(signal));
            //}
            //chkAllSignals.Sorted = true;
        }

        public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
        {
            //MetadataBase signalReference = null;
            m_signals.Clear();
            int index = 0;
            foreach (SignalGroup group in currentlyActiveGroups)
            {
                MetadataBase signal = group.TryGetSignal(m_signalTypeToPlot);
                if (signal != null)
                {
                    m_signals.Add(new KeyValuePair<int, Guid>(index, signal.UniqueId));
                    activeSignals.Add(signal);
                }
                index++;
            }
        }

        private void PlotSurface2D1_InteractionOccured(object sender)
        {
            if (m_frameworkCtrl.Framework.Updater.Mode == ExecutionMode.Automatic)
            {
                if (!(sender is PlotSurface2D.Interactions.AxisDragX || sender is PlotSurface2D.Interactions.MouseWheelZoom))
                    return;
                DateTime minDate, maxDate;
                minDate = new DateTime((long)plotSurface2D1.XAxis1.WorldMin);
                maxDate = new DateTime((long)plotSurface2D1.XAxis1.WorldMax);
                m_ignoreNextUpdate = true;
                m_frameworkCtrl.Framework.Updater.SetWindowDuration(maxDate - minDate, m_token);
            }
            else
            {
                if (!(sender is PlotSurface2D.Interactions.HorizontalDrag || sender is PlotSurface2D.Interactions.AxisDragX || sender is PlotSurface2D.Interactions.MouseWheelZoom))
                    return;
                DateTime minDate, maxDate;
                minDate = new DateTime((long)plotSurface2D1.XAxis1.WorldMin);
                maxDate = new DateTime((long)plotSurface2D1.XAxis1.WorldMax);
                m_frameworkCtrl.Framework.ChangeDateRange(minDate, maxDate, m_token);
            }
        }


        private void m_framework_ParallelWithControlLockNewQueryResults(object sender, QueryResultsEventArgs e)
        {
            //Debug Code
            //if (InvokeRequired)
            //    throw new Exception();
            try
            {
                PlotChart(e, plotSurface2D1, m_signals, m_ignoreNextUpdate |= ReferenceEquals(m_token, e.RequestedToken));
            }
            catch (Exception)
            {
            }
            m_ignoreNextUpdate = false;
        }

        private void PlotChart(QueryResultsEventArgs e, PlotSurface2D plot, List<KeyValuePair<int, Guid>> signals, bool cacheAxis)
        {
            if (cacheAxis)
            {
                double minX, maxX, minY, maxY;

                plot.Title = m_plotTitle;
                maxX = plot.XAxis1.WorldMax;
                minX = plot.XAxis1.WorldMin;
                maxY = plot.YAxis1.WorldMax;
                minY = plot.YAxis1.WorldMin;

                foreach (IDrawable drawing in plot.Drawables.ToArray())
                    plot.Remove(drawing, false);

                foreach (KeyValuePair<int, Guid> freq in signals)
                {
                    SignalDataBase data = e.Results[freq.Value];

                    List<double> y = new List<double>(data.Count);
                    List<double> x = new List<double>(data.Count);

                    for (int i = 0; i < data.Count; i++)
                    {
                        data.GetData(i, out ulong time, out double value);

                        x.Add(time);
                        y.Add(value * m_scalingFactor);
                    }

                    LinePlot lines = new LinePlot(y, x);
                    lines.Pen = m_colorWheel.TryGetPen(freq.Key);

                    plot.Add(lines);
                }

                plot.XAxis1.WorldMax = maxX;
                plot.XAxis1.WorldMin = minX;
                plot.YAxis1.WorldMax = maxY;
                plot.YAxis1.WorldMin = minY;

                plot.Refresh();
            }
            else
            {
                plot.Clear();
                plot.Title = m_plotTitle;
                AddInteractions();

                foreach (KeyValuePair<int, Guid> freq in signals)
                {
                    SignalDataBase data = e.Results[freq.Value];

                    List<double> y = new List<double>(data.Count);
                    List<double> x = new List<double>(data.Count);

                    for (int i = 0; i < data.Count; i++)
                    {
                        data.GetData(i, out ulong time, out double value);

                        x.Add(time);
                        y.Add(value * m_scalingFactor);
                    }

                    LinePlot lines = new LinePlot(y, x);
                    lines.Pen = m_colorWheel.TryGetPen(freq.Key);

                    plot.Add(lines);
                }

                if (plot.XAxis1 != null)
                {
                    plot.XAxis1.WorldMax = e.EndTime.Ticks;
                    plot.XAxis1.WorldMin = e.StartTime.Ticks;
                }
                plot.Refresh();
            }
        }


        private void AddInteractions()
        {
            plotSurface2D1.AddInteraction(HorizontalDrag);
            plotSurface2D1.AddInteraction(VerticalDrag);
            plotSurface2D1.AddInteraction(AxisDragX);
            plotSurface2D1.AddInteraction(AxisDragY);
            plotSurface2D1.AddInteraction(MouseWheelZoom);
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
                            m_frameworkCtrl.Framework.RemoveSubscriber(this);
                        }
                        value.Framework.Updater.ParallelWithControlLockNewQueryResults += m_framework_ParallelWithControlLockNewQueryResults;
                        value.Framework.Updater.ExecutionModeChanged += UpdaterOnExecutionModeChanged;

                        value.Framework.AddSubscriber(this);
                    }
                }
                m_frameworkCtrl = value;
            }
        }

        private void UpdaterOnExecutionModeChanged(object sender, ExecutionModeEventArgs executionMode)
        {
            if (executionMode.Mode == ExecutionMode.Manual)
            {
                MouseWheelZoom.ZoomLocation = PlotSurface2D.Interactions.MouseWheelZoom.Location.MouseLocation;
                AxisDragX.ZoomLocation = PlotSurface2D.Interactions.AxisDragX.Location.MouseLocation;
            }
            else
            {
                MouseWheelZoom.ZoomLocation = PlotSurface2D.Interactions.MouseWheelZoom.Location.FrontOfGraph;
                AxisDragX.ZoomLocation = PlotSurface2D.Interactions.AxisDragX.Location.FrontOfGraph;
            }
        }

        [
            Bindable(true),
            Browsable(true),
            Category("Framework"),
            Description("The framework component that this control will use."),
            DefaultValue("")
        ]
        public string SignalTypeToPlot
        {
            get => m_signalTypeToPlot;
            set => m_signalTypeToPlot = value;
        }

        [
            Bindable(true),
            Browsable(true),
            Category("Framework"),
            Description("The framework component that this control will use."),
            DefaultValue("")
        ]
        public string PlotTitle
        {
            get => m_plotTitle;
            set
            {
                m_plotTitle = value;
                plotSurface2D1.Title = value;
            }
        }

        [
            Bindable(true),
            Browsable(true),
            Category("Framework"),
            Description("The framework component that this control will use."),
            DefaultValue(1.0)
        ]
        public double ScalingFactor
        {
            get => m_scalingFactor;
            set => m_scalingFactor = value;
        }

        [
            Bindable(true),
            Browsable(true),
            Category("Framework"),
            Description("The framework component that this control will use."),
            DefaultValue(null)
        ]
        public ColorWheel Colors
        {
            get => m_colorWheel;
            set => m_colorWheel = value;
        }
    }
}