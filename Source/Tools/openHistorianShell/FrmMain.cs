using System;
using System.Windows.Forms;
using openHistorian;

namespace openHistorianShell
{
    public partial class FrmMain : Form
    {
        HistorianServer m_server;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            GSF.Globals.MemoryPool.SetMaximumBufferSize(long.Parse(TxtMaxMB.Text) * 1024 * 1024);
            HistorianDatabaseInstance db1 = new HistorianDatabaseInstance();
            db1.Paths = TxtArchivePath.Lines;
            db1.IsNetworkHosted = true;
            db1.ConnectionString = "port=" + TxtLocalPort.Text;
            m_server = new HistorianServer(db1);
            BtnStart.Enabled = false;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            TxtMaxMB.Text = (GSF.Globals.MemoryPool.MaximumBufferSize / 1024 / 1024).ToString();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_server.Dispose();
        }
    }
}
