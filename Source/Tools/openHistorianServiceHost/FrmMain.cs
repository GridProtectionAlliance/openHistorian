using System;
using System.Threading;
using System.Windows.Forms;
using GSF.Snap;
using GSF.Snap.Services;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using openHistorian.Net;
using openHistorian.Snap;

namespace openHistorianServiceHost
{
    public partial class FrmMain : Form
    {
        private HistorianHost m_host;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            m_host = new HistorianHost();
        }

        private void BtnStartStream_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(StartStream);
        }

        private void StartStream(object args)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (HistorianClient client = new HistorianClient("127.0.0.1", 54996))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(string.Empty))
            {
                using (SortedTreeTable<HistorianKey, HistorianValue> file = SortedTreeFile.OpenFile(@"H:\OGE 2009.d2", isReadOnly: true).OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
                {
                    using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> read = file.BeginRead())
                    {
                        SortedTreeScannerBase<HistorianKey, HistorianValue> scan = read.GetTreeScanner();
                        scan.SeekToStart();
                        long count = 0;
                        while (scan.Read(key, value))
                        {
                            count++;
                            database.Write(key, value);
                            if (count % 10 == 1)
                                Thread.Sleep(1);
                        }
                    }
                }
            }
        }

        private void btnOpenClient_Click(object sender, EventArgs e)
        {
            FrmClientApp frm = new FrmClientApp();
            frm.Show();
        }
    }
}