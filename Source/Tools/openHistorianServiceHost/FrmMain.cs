using System;
using System.Threading;
using System.Windows.Forms;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Services.Net;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Net;
using openHistorian;
using GSF.SortedTreeStore.Storage;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Tree.TreeNodes;

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

            NetworkClientConfig clientConfig = new NetworkClientConfig();
            clientConfig.IsReadOnly = true;
            clientConfig.NetworkPort = 54996;
            clientConfig.ServerNameOrIp = "127.0.0.1";

            using (HistorianClient client = new HistorianClient(clientConfig))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(string.Empty))
            {
                using (SortedTreeTable<HistorianKey, HistorianValue> file = SortedTreeFile.OpenFile(@"H:\OGE 2009.d2", isReadOnly: true).OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode))
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
                            if ((count % 10) == 1)
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