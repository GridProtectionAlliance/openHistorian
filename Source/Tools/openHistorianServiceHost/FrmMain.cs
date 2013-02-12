using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using openHistorian;
using openHistorian.Archive;

namespace openHistorianServiceHost
{
    public partial class FrmMain : Form
    {
        HistorianHost m_host;
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

        void StartStream(object args)
        {
            var clientOptions = new HistorianClientOptions();
            clientOptions.IsReadOnly = true;
            clientOptions.NetworkPort = 54996;
            clientOptions.ServerNameOrIp = "127.0.0.1";

            using (var client = new HistorianClient(clientOptions))
            {
                var database = client.GetDatabase();

                using (ArchiveFile file = ArchiveFile.OpenFile(@"H:\OGE 2009.d2", AccessMode.ReadOnly))
                {
                    using (var read = file.BeginRead())
                    {
                        var scan = read.GetTreeScanner();
                        scan.SeekToKey(0, 0);
                        ulong key1, key2, value1, value2;
                        long count = 0;
                        while (scan.Read(out key1, out key2, out value1, out value2))
                        {
                            count++;
                            database.Write(key1, key2, value1, value2);
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
            var frm = new FrmClientApp();
            frm.Show();
        }
    }
}
