using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NPlot;
using openHistorian.Data.Query;
using openVisN;
using openVisN.Framework;
using PlotSurface2D = NPlot.Windows.PlotSurface2D;

namespace frameworkVisN
{
    public partial class FrmMain : Form, ISubscriber
    {
        private double minX, maxX, minY, maxY;
        private PlotSurface2D LastPlotInteraction;

        private static class ColorWheel
        {
            private static int index;
            private static readonly List<Pen> m_pens;

            static ColorWheel()
            {
                m_pens = new List<Pen>();
                m_pens.Add(new Pen(Color.Red, 2));
                m_pens.Add(new Pen(Color.LimeGreen, 2));
                m_pens.Add(new Pen(Color.Cyan, 2));
                m_pens.Add(new Pen(Color.Purple, 2));
                m_pens.Add(new Pen(Color.Brown, 2));
                m_pens.Add(new Pen(Color.Orange, 2));
                m_pens.Add(new Pen(Color.Magenta, 2));
                m_pens.Add(new Pen(Color.Blue, 2));
                m_pens.Add(new Pen(Color.Black, 2));
                m_pens.Add(new Pen(Color.Gray, 2));
                m_pens.Add(new Pen(Color.DarkGreen, 2));
            }

            public static Pen GetPen()
            {
                return m_pens[index++ % m_pens.Count];
            }

            public static void Reset()
            {
                index = 0;
            }
        }

        private class SignalWrapper
        {
            public readonly SignalGroup Signal;

            public SignalWrapper(SignalGroup signal)
            {
                Signal = signal;
            }

            public string DisplayName => Signal.SignalGroupName;
        }

        private readonly SubscriptionFramework m_framework;
        private readonly List<Guid> m_frequencySignals = new List<Guid>();
        private readonly List<Guid> m_voltageAngleSignals = new List<Guid>();

        public FrmMain()
        {
            InitializeComponent();
            m_framework = new SubscriptionFramework(new string[] {@"H:\August 2012.d2"});
            m_framework.AddSubscriber(this);
            m_framework.Updater.SynchronousNewQueryResults += m_framework_NewQueryResults;
            ChkAllSignals.DisplayMember = "DisplayName";
        }

        private void m_framework_NewQueryResults(object sender, QueryResultsEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<QueryResultsEventArgs>(m_framework_NewQueryResults), sender, e);
                return;
            }

            long points = 0;
            foreach (SignalDataBase pt in e.Results.Values)
                points += pt.Count;

            LblPointCount.Text = "Point Count: " + points.ToString();

            PlotChart(e, PlotFrequency, m_frequencySignals, ReferenceEquals(PlotFrequency, LastPlotInteraction));
            PlotChart(e, PlotVoltageAngle, m_voltageAngleSignals, ReferenceEquals(PlotVoltageAngle, LastPlotInteraction));
            LastPlotInteraction = null;
        }

        private void PlotChart(QueryResultsEventArgs e, PlotSurface2D plot, List<Guid> signals, bool cacheAxis)
        {
            if (cacheAxis)
            {
                maxX = plot.XAxis1.WorldMax;
                minX = plot.XAxis1.WorldMin;
                maxY = plot.YAxis1.WorldMax;
                minY = plot.YAxis1.WorldMin;

                foreach (IDrawable drawing in plot.Drawables.ToArray())
                    plot.Remove(drawing, false);

                ColorWheel.Reset();
                foreach (Guid freq in signals)
                {
                    SignalDataBase data = e.Results[freq];

                    List<double> y = new List<double>(data.Count);
                    List<double> x = new List<double>(data.Count);

                    for (int i = 0; i < data.Count; i++)
                    {
                        data.GetData(i, out ulong time, out double value);

                        x.Add(time);
                        y.Add(value);
                    }

                    LinePlot lines = new LinePlot(y, x);
                    lines.Pen = ColorWheel.GetPen();

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

                plot.AddInteraction(new PlotSurface2D.Interactions.HorizontalDrag());
                plot.AddInteraction(new PlotSurface2D.Interactions.VerticalDrag());
                plot.AddInteraction(new PlotSurface2D.Interactions.AxisDrag(false));

                ColorWheel.Reset();
                foreach (Guid freq in signals)
                {
                    SignalDataBase data = e.Results[freq];

                    List<double> y = new List<double>(data.Count);
                    List<double> x = new List<double>(data.Count);

                    for (int i = 0; i < data.Count; i++)
                    {
                        data.GetData(i, out ulong time, out double value);

                        x.Add(time);
                        y.Add(value);
                    }

                    LinePlot lines = new LinePlot(y, x);
                    lines.Pen = ColorWheel.GetPen();

                    plot.Add(lines);
                }
                plot.Refresh();
            }
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            LastPlotInteraction = null;
            m_framework.ChangeDateRange(new DateTime(2012, 8, 2), new DateTime(2012, 8, 2, 1, 0, 0));
        }

        public void Initialize(SubscriptionFramework framework)
        {
            ChkAllSignals.Items.Clear();
            foreach (SignalGroup signal in framework.AllSignalGroups)
            {
                ChkAllSignals.Items.Add(new SignalWrapper(signal));
            }
            ChkAllSignals.Sorted = true;
        }


        public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
        {
            MetadataBase signalReference = null;
            m_frequencySignals.Clear();
            m_voltageAngleSignals.Clear();
            foreach (SignalGroup group in currentlyActiveGroups)
            {
                SinglePhasorTerminal calc = group as SinglePhasorTerminal;
                if (calc != null)
                {
                    if (signalReference is null)
                    {
                        signalReference = calc.VoltageAngle;
                        m_framework.SetAngleReference(signalReference);
                    }

                    m_frequencySignals.Add(calc.VoltageAngleReference.SignalId);
                    activeSignals.Add(calc.VoltageAngleReference);
                    //m_frequencySignals.Add(calc.Frequency.SignalId);
                    //activeSignals.Add(calc.Frequency);
                    m_voltageAngleSignals.Add(calc.VoltageMagnitudePu.SignalId);
                    activeSignals.Add(calc.VoltageMagnitudePu);
                    //m_voltageAngleSignals.Add(calc.VoltageAngle.SignalId);
                    //activeSignals.Add(calc.VoltageAngle);
                }
            }
        }

        private void ChkAllSignals_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SignalWrapper item = (SignalWrapper)ChkAllSignals.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                m_framework.ActivateSignalGroup(item.Signal);
            }
            else
            {
                m_framework.DeactivateSignalGroup(item.Signal);
            }
        }

        private void PlotFrequency_InteractionOccured(object sender)
        {
            if (!(sender is PlotSurface2D.Interactions.HorizontalDrag || sender is PlotSurface2D.Interactions.AxisDrag))
                return;
            DateTime minDate, maxDate;
            minDate = new DateTime((long)PlotFrequency.XAxis1.WorldMin);
            maxDate = new DateTime((long)PlotFrequency.XAxis1.WorldMax);
            LastPlotInteraction = PlotFrequency;
            m_framework.ChangeDateRange(minDate, maxDate);
        }

        private void PlotVoltageAngle_InteractionOccured(object sender)
        {
            if (!(sender is PlotSurface2D.Interactions.HorizontalDrag || sender is PlotSurface2D.Interactions.AxisDrag))
                return;
            DateTime minDate, maxDate;
            minDate = new DateTime((long)PlotVoltageAngle.XAxis1.WorldMin);
            maxDate = new DateTime((long)PlotVoltageAngle.XAxis1.WorldMax);
            LastPlotInteraction = PlotVoltageAngle;
            m_framework.ChangeDateRange(minDate, maxDate);
        }
    }
}