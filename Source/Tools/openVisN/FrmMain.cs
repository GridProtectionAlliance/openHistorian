using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using openHistorian.Data.Query;
using openVisN.Framework;
using GSF.IO;
using GSF.Reflection;
using GSF.Snap;

namespace openVisN
{
    public partial class FrmMain : Form
    {
        private long m_pointCount;
        private readonly Stopwatch m_sw1 = new Stopwatch();
        private readonly Stopwatch m_sw2 = new Stopwatch();
        private readonly SettingsManagement m_settings;

        public FrmMain()
        {
            m_settings = new SettingsManagement(Path.Combine(FilePath.GetApplicationDataFolder(), "config.xml"));
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Version version = AssemblyInfo.EntryAssembly.Version;
            Text = string.Format(Text, $"v{version.Major}.{version.Minor}.{version.Build}");
        }

        private void Updater_NewQueryResults(object sender, QueryResultsEventArgs e)
        {
            long count = 0;

            foreach (SignalDataBase r in e.Results.Values)
                count += r.Count;

            m_pointCount = count;
        }

        private void UpdaterOnBeforeExecuteQuery(object sender, EventArgs eventArgs)
        {
            m_sw1.Reset();
            m_sw2.Reset();

            m_sw1.Start();
            Stats.Clear();
        }

        private void UpdaterOnAfterExecuteQuery(object sender, EventArgs eventArgs)
        {
            try
            {
                m_sw2.Stop();
                StringBuilder sb = new StringBuilder();

                sb.Append("Scanned: " + Stats.PointsScanned.ToString().PadRight(9));
                sb.Append("" + (Stats.PointsScanned / m_sw1.Elapsed.TotalSeconds / 1000000).ToString("0.0 M/s").PadRight(9));
                sb.Append("| Points: " + Stats.PointsReturned.ToString().PadRight(9));
                sb.Append("" + (Stats.PointsReturned / m_sw1.Elapsed.TotalSeconds / 1000000).ToString("0.0 M/s").PadRight(9));
                sb.Append("| Seek: " + Stats.SeeksRequested.ToString().PadRight(8));
                sb.Append("" + (Stats.SeeksRequested / m_sw1.Elapsed.TotalSeconds / 1000).ToString("0 K/s").PadRight(9));
                sb.Append("| Calculated: " + (m_pointCount - Stats.PointsReturned).ToString().PadRight(7));
                sb.Append("| Queries Per Second: " + (1 / m_sw1.Elapsed.TotalSeconds).ToString("0.0").PadRight(9));            

                this.BeginInvoke(new Action(() => lblStatus.Text = sb.ToString()));
            }
            catch
            {
            }
        }

        private void UpdaterOnAfterQuery(object sender, EventArgs eventArgs)
        {
            m_sw1.Stop();
            m_sw2.Start();
        }

        //private void BtnEvents_Click(object sender, EventArgs e)
        //{
        //    FrmEvents win = new FrmEvents(visualizationFramework1);
        //    win.Show();
        //}

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            m_settings.Edit();
            visualizationFramework1.Framework.ReLoadSignalsAndSignalGroups();
            signalGroupSelectionCheckedListBox1.Initialize(visualizationFramework1.Framework);
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            visualizationFramework1.Server = m_settings.ServerIP;
            visualizationFramework1.Port = int.Parse(m_settings.HistorianPort);
            visualizationFramework1.UseNetworkHistorian = true;
            visualizationFramework1.Database = m_settings.HistorianDatabase;

            //visualizationFramework1.UseNetworkHistorian = false;
            //visualizationFramework1.Paths = new string[] { @"\\172.21.4.212\c$\Program Files\openHistorian\Archive" };
            //visualizationFramework1.Paths = new string[] { @"C:\TempArchive\635513871062094134-stage3-2014-11-12 17.29.34.833_to_2014-11-12 19.05.02.900.d2" };
            //visualizationFramework1.Paths = new string[] { @"C:\TempArchive\635514895342106848-Stage1-2014-11-13 20.32.03.133_to_2014-11-13 20.32.14.166.d2i" };
                        
            visualizationFramework1.Start();
            DateTime start = DateTime.UtcNow.AddMinutes(-5);
            visualizationFramework1.Framework.ChangeDateRange(start, start.AddMinutes(5));
            visualizationFramework1.Framework.Updater.BeforeExecuteQuery += UpdaterOnBeforeExecuteQuery;
            visualizationFramework1.Framework.Updater.AfterQuery += UpdaterOnAfterQuery;
            visualizationFramework1.Framework.Updater.AfterExecuteQuery += UpdaterOnAfterExecuteQuery;
            visualizationFramework1.Framework.Updater.NewQueryResults += Updater_NewQueryResults;
            BtnConnect.Enabled = false;

            signalGroupSelectionCheckedListBox1.Enabled = true;
            manualAutomaticModeSelectorButton1.Enabled = true;
        }
    }
}
