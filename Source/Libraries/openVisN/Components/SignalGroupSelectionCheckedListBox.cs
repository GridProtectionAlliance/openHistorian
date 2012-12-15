using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace openVisN.Components
{
    public partial class SignalGroupSelectionCheckedListBox : UserControl, ISubscriber
    {
        SubscriptionFramework m_framework;
        
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

        public SignalGroupSelectionCheckedListBox()
        {
            InitializeComponent();
            chkAllSignals.DisplayMember = "DisplayName";
        }

        public void Initialize(SubscriptionFramework framework)
        {
            m_framework = framework;
            chkAllSignals.Items.Clear();
            foreach (var signal in framework.AllSignalGroups)
            {
                chkAllSignals.Items.Add(new SignalWrapper(signal));
            }
            chkAllSignals.Sorted = true;
        }
    }
}
