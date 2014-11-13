using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF.Snap.Storage;
using openHistorian.Collections;

namespace ArchiveTools
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void BtnFindCorruptFiles_Click(object sender, EventArgs e)
        {
            DataTable resultsTable = new DataTable();
            resultsTable.Columns.Add("File", typeof(string));
            resultsTable.Columns.Add("ID", typeof(string));
            StringBuilder sb = new StringBuilder();

            using (var dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                dlg.Filter = "Open Historian 2.0 File|*.d2";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in dlg.FileNames)
                    {
                        sb.Clear();
                        try
                        {
                            using (var file = SortedTreeFile.OpenFile(fileName, true))
                            {
                                sb.AppendFormat("ID: {0} ", file.Snapshot.Header.ArchiveId);
                                sb.AppendFormat("Commit Number: {0} ", file.Snapshot.Header.SnapshotSequenceNumber);
                                var table = file.OpenTable<HistorianKey, HistorianValue>();
                                if (table == null)
                                {
                                    sb.Append("ERROR - No Historian Table ");
                                }
                                else
                                {
                                    try
                                    {
                                        if ((long)table.FirstKey.Timestamp < DateTime.MinValue.Ticks || (long)table.FirstKey.Timestamp > DateTime.MaxValue.Ticks)
                                        {
                                            sb.Append("Start Time: Invalid ");
                                        }
                                        else
                                        {
                                            sb.AppendFormat("Start Time: {0} ", table.FirstKey.TimestampAsDate.ToString());
                                        }

                                        if ((long)table.LastKey.Timestamp < DateTime.MinValue.Ticks || (long)table.LastKey.Timestamp > DateTime.MaxValue.Ticks)
                                        {
                                            sb.Append("End Time: Invalid ");
                                        }
                                        else
                                        {
                                            sb.AppendFormat("End Time: {0} ", table.LastKey.TimestampAsDate.ToString());
                                        }
                                    }
                                    finally
                                    {
                                        table.Dispose();
                                    }
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            sb.AppendFormat("File Header Corrupt: {0} ", ex.ToString());
                            throw;
                        }

                        resultsTable.Rows.Add(fileName, sb.ToString());
                    }

                    var win = new FrmDisplayFileMetaData(resultsTable);
                    win.Show();
                }
            }
        }
    }
}
