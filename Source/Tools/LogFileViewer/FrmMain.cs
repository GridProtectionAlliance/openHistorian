using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using GSF.Diagnostics;
using GSF.IO;

namespace LogFileViewer
{
    public partial class FrmMain : Form
    {
        private List<LogMessageSerializable> m_messages;
        private List<IMessageMatch> m_filters;

        public FrmMain()
        {
            m_filters = new List<IMessageMatch>();
            m_messages = new List<LogMessageSerializable>();
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dgvResults, new object[] { true });
        }

        private void RefreshFilters()
        {
            var dt = new DataTable();
            dt.Columns.Add("Object", typeof(LogMessageSerializable));
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
                if (m_filters.All(x => x.IsIncluded(message)))
                {
                    dt.Rows.Add(message, message.UtcTime, message.Level.ToString(),
                        message.Type, message.EventName, message.Message,
                        message.Details, message.Exception, message.Source);
                }
            }

            dgvResults.DataSource = dt;
            dgvResults.Columns["Object"].Visible = false;
            foreach (DataGridViewColumn dc in dgvResults.Columns)
            {
                if (dc.ValueType == typeof(DateTime))
                {
                    dc.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss.fff";
                }
            }
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
                RefreshFilters();
            }
        }

        private void dgvResults_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                LogMessageSerializable item = (LogMessageSerializable)dgvResults.Rows[e.RowIndex].Cells["Object"].Value;

                switch (dgvResults.Columns[e.ColumnIndex].Name)
                {
                    case "Type":
                        MakeMenu(e, new MatchType(item));
                        break;
                    case "EventName":
                        MakeMenu(e, new MatchEventName(item));
                        break;
                    case "Time":
                        MakeMenu(e, new MatchTimestamp(item));
                        break;
                    case "Level":
                        MakeMenu(e, new MatchVerbose(item));
                        break;
                    default:
                        return;
                }
            }
        }

        private void MakeMenu(DataGridViewCellMouseEventArgs e, IMessageMatch filter)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ShowImageMargin = false;
            foreach (var item in filter.GetMenuButtons())
            {
                var button = new ToolStripButton(item.Item1);
                button.Click += (send1, e1) =>
                    {
                        item.Item2();
                        LstFilters.Items.Add(filter);
                        m_filters.Add(filter);
                        RefreshFilters();
                    };
                menu.Items.Add(button);
            }

            menu.Width = 150;
            menu.Show(dgvResults, dgvResults.PointToClient(Cursor.Position));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (LstFilters.SelectedItem != null)
            {
                m_filters.Remove((IMessageMatch)LstFilters.SelectedItem);
                LstFilters.Items.Remove(LstFilters.SelectedItem);
                RefreshFilters();
            }
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (LstFilters.SelectedItem != null)
            {
                ((IMessageMatch)LstFilters.SelectedItem).ToggleResult();
                LstFilters.DisplayMember = string.Empty;
                LstFilters.DisplayMember = "Description";
                RefreshFilters();
            }

        }

        private void dgvResults_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.RowIndex >= 0)
            {
                LogMessageSerializable item = (LogMessageSerializable)dgvResults.Rows[e.RowIndex].Cells["Object"].Value;
                FrmShowError win = new FrmShowError();
                win.TxtMessage.Text = item.ToString();
                win.Show();
                win.TxtMessage.Select(0,0);
            }
        }
    }
}
