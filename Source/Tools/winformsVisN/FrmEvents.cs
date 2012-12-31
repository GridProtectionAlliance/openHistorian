using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using openVisN.Components;

namespace winformsVisN
{
    public partial class FrmEvents : Form
    {
        VisualizationFramework m_framework;

        public FrmEvents(VisualizationFramework framework)
        {
            InitializeComponent();
            m_framework = framework;
        }

        private void FrmEvents_Load(object sender, EventArgs e)
        {
            var DT = new DataTable();
            DT.Columns.Add("Time", typeof(DateTime));
            DT.Columns.Add("Location", typeof(string));
            DT.Columns.Add("Cause", typeof(string));

            var lines = File.ReadAllLines(@"C:\Unison\GPA\Demo\Events.txt");
            for (int x = 1; x < lines.Length; x++)
            {
                DT.Rows.Add(lines[x].Split('\t'));
            }

            dataGridView1.DataSource = DT;
            dataGridView1.Columns["Time"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns["Time"].Width = 150;
            dataGridView1.Columns["Location"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns["Location"].FillWeight = 10;
            dataGridView1.Columns["Cause"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns["Cause"].FillWeight = 10;

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dataGridView1, new object[] { true });

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DateTime date = (DateTime)dataGridView1[0, e.RowIndex].Value;
                date = date.ToUniversalTime();
                m_framework.Framework.ChangeDateRange(date.AddMinutes(-2), date.AddMinutes(2));
            }
        }
    }
}
