using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GSF.Snap;
using openHistorian.Data.Query;
using openHistorianServiceHost.Properties;
using openVisN.Framework;

namespace openHistorianServiceHost
{
    public partial class FrmClientApp : Form
    {
        private string EventsFile;
        private readonly string SignalGroupFile;
        private readonly string SignalMetaData;
        private readonly List<string> ArchiveFiles;

        public FrmClientApp()
        {
            if (!Settings.Default.Upgraded)
            {
                Settings.Default.Upgraded = true;
                Settings.Default.Upgrade();
                Settings.Default.Save();
            }
            if (!File.Exists(Settings.Default.Configini))
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "Config File|*.d2ini";
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    Settings.Default.Configini = dlg.FileName;
                    Settings.Default.Save();
                }
            }

            string[] lines = File.ReadAllLines(Settings.Default.Configini);
            if (lines.Length < 4)
                throw new Exception("Must have at least 4 lines.");

            if (!File.Exists(lines[0]) ||
                !File.Exists(lines[1]) ||
                !File.Exists(lines[2]) ||
                !File.Exists(lines[3]))
                throw new Exception("some files in the config file do not exist.");

            EventsFile = lines[0];
            SignalGroupFile = lines[1];
            SignalMetaData = lines[2];
            ArchiveFiles = new List<string>();
            for (int x = 3; x < lines.Length; x++)
            {
                string l = lines[x];
                if (l.Length > 0)
                {
                    if (!File.Exists(lines[0]))
                        throw new Exception("some files in the config file do not exist.");

                    ArchiveFiles.Add(l);
                }
            }
            throw new NotImplementedException();
            //AllSignals.DefaultPath = SignalMetaData;
            //AllSignalGroups.DefaultPath = SignalGroupFile;

            //InitializeComponent();

            //visualizationFramework1.Server = "127.0.0.1";
            //visualizationFramework1.Port = 54996;
            //visualizationFramework1.UseNetworkHistorian = true;
            //visualizationFramework1.Paths = ArchiveFiles.ToArray();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            visualizationFramework1.Start();
            visualizationFramework1.Framework.ChangeDateRange(new DateTime(2009, 1, 1), new DateTime(2009, 2, 2));
            visualizationFramework1.Framework.Updater.BeforeExecuteQuery += UpdaterOnBeforeExecuteQuery;
            visualizationFramework1.Framework.Updater.AfterQuery += UpdaterOnAfterQuery;
            visualizationFramework1.Framework.Updater.AfterExecuteQuery += UpdaterOnAfterExecuteQuery;
            visualizationFramework1.Framework.Updater.NewQueryResults += Updater_NewQueryResults;
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

                sb.Append("Scanned: " + Stats.PointsScanned.ToString().PadRight(9));
                sb.Append("" + (Stats.PointsScanned / sw1.Elapsed.TotalSeconds / 100000).ToString("0.0 M/s").PadRight(9));
                sb.Append("| Points: " + Stats.PointsReturned.ToString().PadRight(9));
                sb.Append("" + (Stats.PointsReturned / sw1.Elapsed.TotalSeconds / 100000).ToString("0.0 M/s").PadRight(9));
                sb.Append("| Seek: " + Stats.SeeksRequested.ToString().PadRight(8));
                sb.Append("" + (Stats.SeeksRequested / sw1.Elapsed.TotalSeconds / 100).ToString("0 K/s").PadRight(9));
                sb.Append("| Calculated: " + (pointCount - Stats.PointsReturned).ToString().PadRight(7));
                sb.Append("| Queries Per Second: " + (1 / sw1.Elapsed.TotalSeconds).ToString("0.0").PadRight(9));

                //if (pointCount < Stats.PointsReturned)
                //{
                //    pointCount = pointCount;
                //}


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
            //var win = new FrmEvents(visualizationFramework1);
            //win.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Clipboard.SetText(NPlot.StepTimer.GetResultsPercent());
            //NPlot.StepTimer.Reset();
        }
    }
}