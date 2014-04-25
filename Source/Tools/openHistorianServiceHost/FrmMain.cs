using System;
using System.Threading;
using System.Windows.Forms;
using GSF.SortedTreeStore.Client;
using GSF.SortedTreeStore.Server;
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
            SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
            clientOptions.IsReadOnly = true;
            clientOptions.NetworkPort = 54996;
            clientOptions.ServerNameOrIp = "127.0.0.1";

            using (HistorianClient client = new HistorianClient(clientOptions))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDefaultDatabase<HistorianKey, HistorianValue>())
            {
                using (SortedTreeTable<HistorianKey, HistorianValue> file = SortedTreeFile.OpenFile(@"H:\OGE 2009.d2", isReadOnly: true).OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode))
                {
                    using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> read = file.BeginRead())
                    {
                        SortedTreeScannerBase<HistorianKey, HistorianValue> scan = read.GetTreeScanner();
                        scan.SeekToStart();
                        ulong key1, key2, value1, value2;
                        long count = 0;
                        while (scan.Read())
                        {
                            count++;
                            database.Write(scan.CurrentKey, scan.CurrentValue);
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