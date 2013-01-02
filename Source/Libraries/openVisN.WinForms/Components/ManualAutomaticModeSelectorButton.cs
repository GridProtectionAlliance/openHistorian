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
    public partial class ManualAutomaticModeSelectorButton : UserControl
    {
        VisualizationFramework m_frameworkCtrl;

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
    }
}
