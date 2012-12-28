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
    public partial class ReferenceSignalSelectionComboBox : UserControl, ISubscriber
    {
        MetadataBase m_activeSignal;

        public ReferenceSignalSelectionComboBox()
        {
            InitializeComponent();
            comboBox1.DisplayMember = "DisplayName";
        }

        VisualizationFramework m_frameworkCtrl;

        class SignalWrapper
        {
            public SignalGroup Signal;

            public SignalWrapper(SignalGroup signal)
            {
                Signal = signal;
            }

            public string DisplayName
            {
                get
                {
                    return Signal.SignalGroupName;
                }
            }
        }

        public void Initialize(SubscriptionFramework framework)
        {
            //m_frameworkCtrl = framework;
            comboBox1.Items.Clear();
            foreach (var signal in framework.AllSignalGroups)
            {
                comboBox1.Items.Add(new SignalWrapper(signal));
            }
            comboBox1.Sorted = true;
            
        }

        public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
        {
            if (m_activeSignal!=null)
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
