using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataExtractionUtility.Properties;
using GSF.SortedTreeStore.Net;
using GSF.TimeSeries;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using openVisN;
using openHistorian;
using openHistorian.Data.Query;

namespace DataExtractionUtility
{
    public partial class FrmMain : Form
    {
        List<MeasurementRow> m_selectedMeasurements;
        MetaData m_meta;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            var res = Resolutions.GetAllResolutions();
            cmbResolution.Items.AddRange(res.ToArray());
            cmbResolution.SelectedIndex = 0;
            dateStartTime.Value = DateTime.Now.Date;
            dateStopTime.Value = dateStartTime.Value.AddDays(1);
        }

        private void BtnGetMetadata_Click(object sender, EventArgs e)
        {
            m_meta = new MetaData();
            var signals = m_meta.Measurements.Select((x) => x.SignalAcronym).Distinct().ToList();
            signals.Sort();
            ChkSignalType.Items.Clear();
            ChkSignalType.Items.AddRange(signals.ToArray());
        }

        private void ChkSignalType_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var types = ChkSignalType.CheckedItems.Cast<string>().ToList();
            if (e.NewValue == CheckState.Checked)
                types.Add((string)ChkSignalType.Items[e.Index]);
            else
                types.Remove((string)ChkSignalType.Items[e.Index]);

            m_selectedMeasurements = m_meta.Measurements.Where((x) => types.Contains(x.SignalAcronym)).ToList();

            lblPointCount.Text = string.Format("Point Count: {0}", m_selectedMeasurements.Count);
        }

        private void dateStartTime_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateStopTime_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            if (m_meta == null)
            {
                MessageBox.Show("Please download the metadata first.");
                return;
            }
            if (m_selectedMeasurements == null || m_selectedMeasurements.Count == 0)
            {
                MessageBox.Show("There are no measurements to extract");
                return;
            }
            DateTime startTime = dateStartTime.Value;
            DateTime stopTime = dateStopTime.Value;
            if (startTime > stopTime)
            {
                MessageBox.Show("Start and Stop times are invalid");
                return;
            }
            TimeSpan interval = Resolutions.GetInterval((string)cmbResolution.SelectedItem);


            HistorianClientOptions clientOptions = new HistorianClientOptions();
            clientOptions.DefaultDatabase = Settings.Default.HistorianInstanceName;
            clientOptions.NetworkPort = Settings.Default.HistorianStreamingPort;
            clientOptions.ServerNameOrIp = Settings.Default.ServerIP;
            using (HistorianClient<HistorianKey, HistorianValue> client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions))
            {

                QueryFilterTimestamp timeFilter;
                if (interval.Ticks != 0)
                    timeFilter = QueryFilterTimestamp.CreateFromIntervalData(startTime, stopTime, interval, new TimeSpan(TimeSpan.TicksPerMillisecond));
                else
                    timeFilter = QueryFilterTimestamp.CreateFromRange(startTime, stopTime);

                var points = m_selectedMeasurements.Select((x) => (ulong)x.PointID).ToArray();
                QueryFilterPointId pointFilter = QueryFilterPointId.CreateFromList(points);

                var database = client.GetDefaultDatabase();
                var frames = database.GetFrames(timeFilter, pointFilter).RoundToTolerance(1);
                using (var csvStream = new StreamWriter("C:\\temp\\file.csv"))
                {
                    //csvStream.AutoFlush = false;
                    csvStream.Write("Timestamp,");
                    foreach (var signal in m_selectedMeasurements)
                    {
                        csvStream.Write(signal.Description);
                        csvStream.Write(',');
                    }
                    csvStream.WriteLine();

                    foreach (var frame in frames)
                    {
                        csvStream.Write(frame.Key.ToString("MM/dd/yyyy hh:mm:ss.fffffff"));
                        csvStream.Write(',');

                        foreach (var signal in m_selectedMeasurements)
                        {
                            HistorianValueStruct value;
                            if (frame.Value.Points.TryGetValue((ulong)signal.PointID, out value))
                            {
                                csvStream.Write(value.AsSingle);
                            }
                            csvStream.Write(',');
                        }
                        csvStream.WriteLine();
                    }
                    csvStream.Flush();
                }
                database.Disconnect();
            }


        }


    }
}
