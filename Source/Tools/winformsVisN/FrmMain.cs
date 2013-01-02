using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using winformsVisN.Properties;

namespace winformsVisN
{
    public partial class FrmMain : Form
    {
        string EventsFile;
        string SignalGroupFile;
        string SignalMetaData;
        List<string> ArchiveFiles;

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
                using (var dlg = new OpenFileDialog())
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
            ArchiveFiles=new List<string>();
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

            openVisN.Library.AllSignals.DefaultPath = SignalMetaData;
            openVisN.Library.AllSignalGroups.DefaultPath = SignalGroupFile;
            FrmEvents.DefaultFile = EventsFile;

            InitializeComponent();

            visualizationFramework1.Paths = ArchiveFiles.ToArray();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            visualizationFramework1.Start();
            visualizationFramework1.Framework.ChangeDateRange(new DateTime(2009, 2, 2), new DateTime(2009, 2, 2, 1, 0, 0));

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
