using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GSF.Snap;
using GSF.Snap.Services.Reader;
using NPlot;
using openHistorian.Data.Query;
using openHistorian.Data.Types;
using GSF.Snap.Services;
using openHistorian.Net;
using openHistorian.Snap;
using openHistorian.Snap.Definitions;
using PlotSurface2D = NPlot.Windows.PlotSurface2D;

namespace simpleVisN
{
    public partial class FrmMain : Form
    {
        private HistorianServer m_archiveFile;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
        }

        private void btnConvertHistorianFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlgOpen = new OpenFileDialog())
            {
                dlgOpen.Filter = "openHistorian 1.0 file|*.d";
                if (dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    using (SaveFileDialog dlgSave = new SaveFileDialog())
                    {
                        dlgSave.Filter = "openHistorian 2.0 file|*.d2";
                        if (dlgSave.ShowDialog() == DialogResult.OK)
                        {
                            openHistorian.Utility.ConvertArchiveFile.ConvertVersion1FileIgnoreDuplicates(dlgOpen.FileName, dlgSave.FileName, HistorianFileEncodingDefinition.TypeGuid);
                            MessageBox.Show("Done!");
                        }
                    }
                }
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlgOpen = new OpenFileDialog())
            {
                dlgOpen.Filter = "openHistorian 2.0 file|*.d2";
                if (dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    HistorianServerDatabaseConfig db = new HistorianServerDatabaseConfig("", "", false);
                    db.ImportPaths.AddRange(dlgOpen.FileNames);
                    m_archiveFile = new HistorianServer(db);
                }
            }
            BuildListOfAllPoints();
        }

        private void BuildListOfAllPoints()
        {
            HashSet<ulong> keys = new HashSet<ulong>();
            SnapClient client = SnapClient.Connect(m_archiveFile.Host);
            ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("");
            TreeStream<HistorianKey, HistorianValue> scanner = db.Read(0, ulong.MaxValue);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            while (scanner.Read(key, value))
            {
                keys.Add(key.PointID);
            }
            List<ulong> AllKeys = keys.ToList();
            AllKeys.Sort();

            chkAllPoints.Items.Clear();
            AllKeys.ForEach((x) => chkAllPoints.Items.Add(x));
            db.Dispose();
            client.Dispose();
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            List<ulong> keys = new List<ulong>(chkAllPoints.CheckedItems.OfType<ulong>());

            plot.Clear();

            plot.AddInteraction(new PlotSurface2D.Interactions.HorizontalDrag());
            plot.AddInteraction(new PlotSurface2D.Interactions.VerticalDrag());
            plot.AddInteraction(new PlotSurface2D.Interactions.AxisDrag(false));

            if (keys.Count == 0)
                return;
            SnapClient client = SnapClient.Connect(m_archiveFile.Host);
            ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("");
            
            Dictionary<ulong, SignalDataBase> results = db.GetSignals(0, ulong.MaxValue, keys, TypeSingle.Instance);

            foreach (ulong point in keys)
            {
                List<double> y = new List<double>();
                List<double> x = new List<double>();
                SignalDataBase data = results[point];

                for (int i = 0; i < data.Count; i++)
                {
                    data.GetData(i, out ulong time, out double value);

                    x.Add(time);
                    y.Add(value);
                }

                LinePlot lines = new LinePlot(y, x);

                plot.Add(lines);
            }

            plot.Refresh();
            db.Dispose();
            client.Dispose();

        }
    }
}