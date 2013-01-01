using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winformsVisN
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            visualizationFramework1.Start();
            visualizationFramework1.Framework.ChangeDateRange(new DateTime(2012, 8, 2), new DateTime(2012, 8, 2, 1, 0, 0));

        }

        private void BtnEvents_Click(object sender, EventArgs e)
        {
            var win = new FrmEvents(visualizationFramework1);
            win.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(NPlot.StepTimer.GetResultsPercent());
            NPlot.StepTimer.Reset();

        }

    }
}
