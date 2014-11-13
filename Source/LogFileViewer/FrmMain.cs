using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using GSF.Diagnostics;
using GSF.IO;

namespace LogFileViewer
{
    public partial class FrmMain : Form
    {
        private List<LogMessageSerializable> m_messages;
        public FrmMain()
        {
            m_messages = new List<LogMessageSerializable>();
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dgvResults, new object[] { true });
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Log File|*.LogBin";
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_messages.Clear();
                    foreach (var file in dlg.FileNames)
                    {
                        using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            try
                            {
                                while (fs.ReadBoolean())
                                {
                                    m_messages.Add(new LogMessageSerializable(fs));
                                }
                            }
                            catch (Exception)
                            {


                            }
                        }
                    }
                }

                var dt = new DataTable();
                dt.Columns.Add("Time", typeof(DateTime));
                dt.Columns.Add("Level", typeof(string));
                dt.Columns.Add("Type", typeof(string));
                dt.Columns.Add("EventName", typeof(string));
                dt.Columns.Add("Message", typeof(string));
                dt.Columns.Add("Details", typeof(string));
                dt.Columns.Add("Exception", typeof(string));
                dt.Columns.Add("Source", typeof(string));

                foreach (var message in m_messages)
                {
                    dt.Rows.Add(message.UtcTime, message.Level.ToString(),
                        message.Type, message.EventName, message.Message,
                        message.Details, message.Exception, message.Source);
                }

                dgvResults.DataSource = dt;

                foreach (DataGridViewColumn dc in dgvResults.Columns)
                {
                    if (dc.ValueType == typeof(DateTime))
                    {
                        dc.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss.fff";
                    }
                }
            }
        }
    }
}
