using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using openVisN;
using openVisN.Calculations;

namespace frameworkVisN
{
    public partial class FrmMain : Form
    {
        SubscriptionFramework m_framework;

        public FrmMain()
        {
            InitializeComponent();
            m_framework = new SubscriptionFramework(new string[] { @"P:\August 2012.d2" });
            m_framework.AddSubscriber(ChkSelectSignalGroups);
            m_framework.AddSubscriber(new TextSubscriber());
            m_framework.NewQueryResults += m_framework_NewQueryResults;
        }

        void m_framework_NewQueryResults(object sender, QueryResultsEventArgs e)
        {
            long points = 0;
            foreach (var signal in e.Results.GetAllPoints())
            {
                points += e.Results.GetSignal(signal).Data.Count;
            }
            MessageBox.Show("Points Read: " + points.ToString());
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            m_framework.ChangeDateRange(new DateTime(2012,8,2),new DateTime(2012,8,2,1,0,0));
        }

        public class TextSubscriber : ISubscriber
        {
            public void Initialize(SubscriptionFramework framework)
            {
                
            }

            public void GetAllDesiredSignals(HashSet<MetadataBase> activeSignals, HashSet<SignalGroup> currentlyActiveGroups)
            {
                foreach (var group in currentlyActiveGroups)
                {
                    SinglePhasorTerminal calc = group as SinglePhasorTerminal;
                    if (calc != null)
                    {
                        activeSignals.Add(calc.VoltageAngle);
                        activeSignals.Add(calc.Watt);
                    }
                }
                
            }
        }

    }
}
