using System;
using System.Threading;
using System.Windows.Forms;
using openHistorian;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

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
            HistorianClientOptions clientOptions = new HistorianClientOptions();
            clientOptions.IsReadOnly = true;
            clientOptions.NetworkPort = 54996;
            clientOptions.ServerNameOrIp = "127.0.0.1";

            using (HistorianClient<HistorianKey, HistorianValue> client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions))
            {
                HistorianDatabaseBase<HistorianKey, HistorianValue> database = client.GetDefaultDatabase();

                using (ArchiveTable<HistorianKey, HistorianValue> file = ArchiveFile.OpenFile(@"H:\OGE 2009.d2", isReadOnly: true).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
                {
                    using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> read = file.BeginRead())
                    {
                        TreeScannerBase<HistorianKey, HistorianValue> scan = read.GetTreeScanner();
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
                database.Disconnect();
            }
        }

        private void btnOpenClient_Click(object sender, EventArgs e)
        {
            FrmClientApp frm = new FrmClientApp();
            frm.Show();
        }
    }
}