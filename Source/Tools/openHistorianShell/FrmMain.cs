using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GSF.Diagnostics;
using openHistorian.Net;

namespace openHistorianShell
{
    public partial class FrmMain : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();

        HistorianServer m_server;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            GSF.Globals.MemoryPool.SetMaximumBufferSize(long.Parse(TxtMaxMB.Text) * 1024 * 1024);

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig(txtDbName.Text, TxtArchivePath.Text, true);
            m_server = new HistorianServer(settings, int.Parse(TxtLocalPort.Text));
            BtnStart.Enabled = false;

            AllocConsole();
            Logger.Console.Verbose = VerboseLevel.High;
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
