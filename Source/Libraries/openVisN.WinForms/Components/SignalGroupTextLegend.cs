using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using openVisN.Framework;

namespace openVisN.Components
{
    public partial class SignalGroupTextLegend : UserControl, ISubscriber
    {
        ColorWheel m_colorWheel;
        VisualizationFramework m_frameworkCtrl;
        List<string> m_allGroups = new List<string>();

        public SignalGroupTextLegend()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
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
            get
            {
                return m_colorWheel;
            }
            set
            {
                m_colorWheel = value;
            }
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
            get
            {
                return m_frameworkCtrl;
            }
            set
            {
                if (!DesignMode)
                {
                    if (!object.ReferenceEquals(m_frameworkCtrl, value))
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
            //chkAllSignals.Items.Clear();
            //foreach (var signal in framework.AllSignalGroups)
            //{
            //    chkAllSignals.Items.Add(new SignalWrapper(signal));
            //}
            //chkAllSignals.Sorted = true;
        }

        public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
        {
            m_allGroups.Clear();
            foreach (var group in currentlyActiveGroups)
            {
                m_allGroups.Add(group.SignalGroupName);
            }



        }

        List<LegendEntry> m_legendData = new List<LegendEntry>();
        class LegendEntry
        {
            public string DisplayName;
            public Pen Pen;
            public LegendEntry(string displayName, Pen pen)
            {
                DisplayName = displayName;
                Pen = pen;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawImage(e.Graphics, new Rectangle(0, 0, Width, Height));
            base.OnPaint(e);
        }

        public void DrawImage(Graphics g, Rectangle r)
        {
            for (int x = 0; x <= m_legendData.Count - 1; x++)
            {
                LegendEntry Entry = m_legendData[x];
                g.DrawLine(Entry.Pen, r.X + 10, r.Y + 25 * x + 13, r.X + 30, r.Y + 25 * x + 13);
                g.DrawString(Entry.DisplayName, new Font("Segoe UI", 13.25f), Brushes.Black, r.X + 40, r.Y + 25 * x + 5);
            }
        }

        void m_framework_SynchronousNewQueryResults(object sender, QueryResultsEventArgs e)
        {
            m_legendData.Clear();
            for (int x = 0; x < m_allGroups.Count; x++)
            {
                m_legendData.Add(new LegendEntry(m_allGroups[x], m_colorWheel.TryGetPen(x)));
            }
            Refresh();

        }
    }
}
