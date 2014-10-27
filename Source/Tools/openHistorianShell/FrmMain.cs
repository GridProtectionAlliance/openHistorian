using System;
using System.Windows.Forms;
using GSF.SortedTreeStore.Services.Configuration;
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

            var settings = new HistorianServerDatabaseConfig("DB", TxtArchivePath.Text, true);
            m_server = new HistorianServer(settings, int.Parse(TxtLocalPort.Text));
            BtnStart.Enabled = false;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            TxtMaxMB.Text = (GSF.Globals.MemoryPool.MaximumPoolSize / 1024 / 1024).ToString();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                m_server.Dispose();
            }
            catch (Exception)
            {

            }
        }
    }
}
