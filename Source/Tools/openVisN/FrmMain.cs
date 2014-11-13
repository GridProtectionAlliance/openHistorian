using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using GSF.Snap;
using GSF.TimeSeries;
using GSF.TimeSeries.Transport;
using NPlot;
using openHistorian;
using openHistorian.Data.Query;
using openVisN;
using openVisN.Framework;
using openVisN.Library;
using openVisN.Properties;
using ServerCommand = GSF.TimeSeries.Transport.ServerCommand;

namespace openVisN
{
    public partial class FrmMain : Form
    {

        SettingsManagement m_settings;
        public FrmMain()
        {
            m_settings = new SettingsManagement(Path.GetFullPath("config.xml"));

            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
          
        }

        private void Updater_NewQueryResults(object sender, QueryResultsEventArgs e)
        {
            m_lastResults = e;
            long pointCount = 0;
            foreach (SignalDataBase r in e.Results.Values)
                pointCount += r.Count;
            this.pointCount = pointCount;
        }

        private QueryResultsEventArgs m_lastResults;
        private long pointCount;
        private readonly Stopwatch sw1 = new Stopwatch();
        private readonly Stopwatch sw2 = new Stopwatch();

        private void UpdaterOnBeforeExecuteQuery(object sender, EventArgs eventArgs)
        {
            sw1.Reset();
            sw2.Reset();

            sw1.Start();
            Stats.Clear();
        }

        private void UpdaterOnAfterExecuteQuery(object sender, EventArgs eventArgs)
        {
            try
            {
                sw2.Stop();
                StringBuilder sb = new StringBuilder();

                sb.Append(("Scanned: " + Stats.PointsScanned.ToString().PadRight(9)));
                sb.Append(("" + (Stats.PointsScanned / sw1.Elapsed.TotalSeconds / 1000000).ToString("0.0 M/s").PadRight(9)));
                sb.Append(("| Points: " + Stats.PointsReturned.ToString().PadRight(9)));
                sb.Append(("" + (Stats.PointsReturned / sw1.Elapsed.TotalSeconds / 1000000).ToString("0.0 M/s").PadRight(9)));
                sb.Append(("| Seek: " + Stats.SeeksRequested.ToString().PadRight(8)));
                sb.Append(("" + (Stats.SeeksRequested / sw1.Elapsed.TotalSeconds / 1000).ToString("0 K/s").PadRight(9)));
                sb.Append(("| Calculated: " + (pointCount - Stats.PointsReturned).ToString().PadRight(7)));
                sb.Append(("| Queries Per Second: " + (1 / sw1.Elapsed.TotalSeconds).ToString("0.0").PadRight(9)));

                if (pointCount < Stats.PointsReturned)
                {
                    pointCount = pointCount;
                }


                this.Invoke(new Action(() => lblStatus.Text = sb.ToString()));
            }
            catch (Exception)
            {
            }
        }

        private void UpdaterOnAfterQuery(object sender, EventArgs eventArgs)
        {
            sw1.Stop();
            sw2.Start();
        }


        private void BtnEvents_Click(object sender, EventArgs e)
        {
            FrmEvents win = new FrmEvents(visualizationFramework1);
            win.Show();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {

        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            m_settings.Edit();
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            visualizationFramework1.Server = m_settings.ServerIP;
            visualizationFramework1.Port = int.Parse(m_settings.HistorianPort);
            visualizationFramework1.UseNetworkHistorian = true;
            visualizationFramework1.Database = m_settings.HistorianDatabase;
            visualizationFramework1.Start();
            DateTime start = DateTime.UtcNow.AddMinutes(-5);
            visualizationFramework1.Framework.ChangeDateRange(start, start.AddMinutes(5));
            visualizationFramework1.Framework.Updater.BeforeExecuteQuery += UpdaterOnBeforeExecuteQuery;
            visualizationFramework1.Framework.Updater.AfterQuery += UpdaterOnAfterQuery;
            visualizationFramework1.Framework.Updater.AfterExecuteQuery += UpdaterOnAfterExecuteQuery;
            visualizationFramework1.Framework.Updater.NewQueryResults += Updater_NewQueryResults;
            BtnConnect.Enabled = false;
        }
       
    }
}
