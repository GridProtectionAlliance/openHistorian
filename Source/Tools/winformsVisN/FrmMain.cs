using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GSF.Snap;
using NPlot;
using openHistorian.Data.Query;
using openVisN;
using openVisN.Framework;
using winformsVisN.Properties;

namespace winformsVisN
{
    public partial class FrmMain : Form
    {
        private readonly string EventsFile;
        private readonly string SignalGroupFile;
        private readonly string SignalMetaData;
        private readonly List<string> ArchiveFiles;

        public FrmMain()
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
            //FrmEvents.DefaultFile = EventsFile;

            //InitializeComponent();

            //visualizationFramework1.Paths = ArchiveFiles.ToArray();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            visualizationFramework1.Start();
            visualizationFramework1.Framework.ChangeDateRange(new DateTime(2009, 2, 2), new DateTime(2009, 2, 2, 1, 0, 0));
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
                sb.Append("" + (Stats.PointsScanned / sw1.Elapsed.TotalSeconds / 1000000).ToString("0.0 M/s").PadRight(9));
                sb.Append("| Points: " + Stats.PointsReturned.ToString().PadRight(9));
                sb.Append("" + (Stats.PointsReturned / sw1.Elapsed.TotalSeconds / 1000000).ToString("0.0 M/s").PadRight(9));
                sb.Append("| Seek: " + Stats.SeeksRequested.ToString().PadRight(8));
                sb.Append("" + (Stats.SeeksRequested / sw1.Elapsed.TotalSeconds / 1000).ToString("0 K/s").PadRight(9));
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
            FrmEvents win = new FrmEvents(visualizationFramework1);
            win.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(StepTimer.GetResultsPercent());
            StepTimer.Reset();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            QueryResultsEventArgs results = m_lastResults;
            if (results is null)
            {
                MessageBox.Show("No query has been executed");
                return;
            }
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "CVS File|*.csv";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter fs = new StreamWriter(dlg.FileName, false);
                    fs.AutoFlush = false;
                    SortedList<DateTime, List<double?>> list = new SortedList<DateTime, List<double?>>();

                    int column = 0;
                    foreach (SignalDataBase value in results.Results.Values)
                    {
                        for (int x = 0; x < value.Count; x++)
                        {
                            value.GetData(x, out ulong date, out double v);
                            DateTime d = new DateTime((long)date);
                            if (!list.ContainsKey(d))
                            {
                                list.Add(d, new List<double?>());
                                for (int k = 0; k < column; k++)
                                    list[d].Add(null);
                            }
                            list[d].Add(v);
                        }
                        foreach (List<double?> lst in list.Values)
                        {
                            while (lst.Count <= column)
                                lst.Add(null);
                        }
                        column++;
                    }

                    List<string> name = new List<string>();
                    Dictionary<Guid, MetadataBase> lookup = visualizationFramework1.Framework.AllSignals.ToDictionary((meta) => meta.UniqueId);

                    foreach (Guid value in results.Results.Keys)
                    {
                        if (!lookup.ContainsKey(value))
                        {
                            name.Add(value.ToString());
                        }
                        else if (lookup[value].Name == "")
                        {
                            name.Add(value.ToString());
                        }
                        else
                        {
                            name.Add(lookup[value].Name);
                        }
                    }


                    fs.Write("Date");
                    name.ForEach((x) => fs.Write("," + x));
                    fs.WriteLine();
                    foreach (KeyValuePair<DateTime, List<double?>> kvp in list)
                    {
                        fs.Write(kvp.Key.ToString("yyyy.MM.dd HH:mm:ss:fff"));

                        foreach (double? val in kvp.Value)
                        {
                            fs.Write(",");
                            if (val.HasValue)
                                fs.Write(val.ToString());
                        }
                        fs.WriteLine();
                    }
                    fs.Flush();
                    fs.Close();
                    Process.Start(dlg.FileName);
                }
            }
        }
    }
}