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
    public partial class SetTimeWindowCalendarControl : UserControl, ISubscriber
    {
        VisualizationFramework m_frameworkCtrl;
        public SetTimeWindowCalendarControl()
        {
            InitializeComponent();
        }

        bool m_suspendEvent;

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
        }

        public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
        {
        }

        void m_framework_SynchronousNewQueryResults(object sender, QueryResultsEventArgs e)
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
