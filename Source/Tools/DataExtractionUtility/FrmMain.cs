using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DataExtractionUtility.Properties;
using GSF;
using GSF.IO;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using openHistorian.Net;
using openHistorian.Snap;
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
            List<string> res = Resolutions.GetAllResolutions();
            cmbResolution.Items.AddRange(res.ToArray());
            cmbResolution.SelectedIndex = 0;
            dateStartTime.Value = DateTime.Now.Date;
            dateStopTime.Value = dateStartTime.Value.AddDays(1);
        }

        private void BtnGetMetadata_Click(object sender, EventArgs e)
        {
            m_meta = new MetaData();
            List<string> signals = m_meta.Measurements.Select((x) => x.SignalAcronym).Distinct().ToList();
            signals.Sort();
            ChkSignalType.Items.Clear();
            ChkSignalType.Items.AddRange(signals.ToArray());
        }

        private void ChkSignalType_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            List<string> types = ChkSignalType.CheckedItems.Cast<string>().ToList();
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

        //private void BtnExport_Click(object sender, EventArgs e)
        //{
        //    Settings.Default.Save();
        //    if (m_meta is null)
        //    {
        //        MessageBox.Show("Please download the metadata first.");
        //        return;
        //    }
        //    if (m_selectedMeasurements is null || m_selectedMeasurements.Count == 0)
        //    {
        //        MessageBox.Show("There are no measurements to extract");
        //        return;
        //    }
        //    DateTime startTime = dateStartTime.Value;
        //    DateTime stopTime = dateStopTime.Value;
        //    if (startTime > stopTime)
        //    {
        //        MessageBox.Show("Start and Stop times are invalid");
        //        return;
        //    }
        //    TimeSpan interval = Resolutions.GetInterval((string)cmbResolution.SelectedItem);


        //    HistorianClientOptions clientOptions = new HistorianClientOptions();
        //    clientOptions.DefaultDatabase = Settings.Default.HistorianInstanceName;
        //    clientOptions.NetworkPort = Settings.Default.HistorianStreamingPort;
        //    clientOptions.ServerNameOrIp = Settings.Default.ServerIP;
        //    using (HistorianClient client = new HistorianClient(clientOptions))
        //    {
        //        KeySeekFilterBase<HistorianKey> timeFilter;
        //        if (interval.Ticks != 0)
        //            timeFilter = TimestampFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, interval, new TimeSpan(TimeSpan.TicksPerMillisecond));
        //        else
        //            timeFilter = TimestampFilter.CreateFromRange<HistorianKey>(startTime, stopTime);

        //        var points = m_selectedMeasurements.Select((x) => (ulong)x.PointID).ToArray();
        //        var pointFilter = PointIDFilter.CreateFromList<HistorianKey>(points);

        //        var database = client.GetDefaultDatabase();
        //        var frames = database.GetFrames(timeFilter, pointFilter).RoundToTolerance(1);
        //        using (var csvStream = new StreamWriter("C:\\temp\\file.csv"))
        //        {
        //            //csvStream.AutoFlush = false;
        //            csvStream.Write("Timestamp,");
        //            foreach (var signal in m_selectedMeasurements)
        //            {
        //                csvStream.Write(signal.Description);
        //                csvStream.Write(',');
        //            }
        //            csvStream.WriteLine();

        //            foreach (var frame in frames)
        //            {
        //                csvStream.Write(frame.Key.ToString("MM/dd/yyyy hh:mm:ss.fffffff"));
        //                csvStream.Write(',');

        //                foreach (var signal in m_selectedMeasurements)
        //                {
        //                    HistorianValueStruct value;
        //                    if (frame.Value.Points.TryGetValue((ulong)signal.PointID, out value))
        //                    {
        //                        csvStream.Write(value.AsSingle);
        //                    }
        //                    csvStream.Write(',');
        //                }
        //                csvStream.WriteLine();
        //            }
        //            csvStream.Flush();
        //        }
        //        database.Disconnect();
        //    }


        //}

        //private void BtnExport_Click(object sender, EventArgs e)
        //{
        //    Settings.Default.Save();
        //    if (m_meta is null)
        //    {
        //        MessageBox.Show("Please download the metadata first.");
        //        return;
        //    }
        //    if (m_selectedMeasurements is null || m_selectedMeasurements.Count == 0)
        //    {
        //        MessageBox.Show("There are no measurements to extract");
        //        return;
        //    }
        //    DateTime startTime = dateStartTime.Value;
        //    DateTime stopTime = dateStopTime.Value;
        //    if (startTime > stopTime)
        //    {
        //        MessageBox.Show("Start and Stop times are invalid");
        //        return;
        //    }
        //    TimeSpan interval = Resolutions.GetInterval((string)cmbResolution.SelectedItem);


        //    HistorianClientOptions clientOptions = new HistorianClientOptions();
        //    clientOptions.DefaultDatabase = Settings.Default.HistorianInstanceName;
        //    clientOptions.NetworkPort = Settings.Default.HistorianStreamingPort;
        //    clientOptions.ServerNameOrIp = Settings.Default.ServerIP;
        //    using (HistorianClient client = new HistorianClient(clientOptions))
        //    {
        //        KeySeekFilterBase<HistorianKey> timeFilter;
        //        if (interval.Ticks != 0)
        //            timeFilter = TimestampFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, interval, new TimeSpan(TimeSpan.TicksPerMillisecond));
        //        else
        //            timeFilter = TimestampFilter.CreateFromRange<HistorianKey>(startTime, stopTime);

        //        var points = m_selectedMeasurements.Select((x) => (ulong)x.PointID).ToArray();
        //        var pointFilter = PointIDFilter.CreateFromList<HistorianKey>(points);

        //        var database = client.GetDefaultDatabase();

        //        using (var frameReader = database.GetPointStream(timeFilter, pointFilter).GetFrameReader())
        //        using (var csvStream = new StreamWriter("C:\\temp\\file.csv"))
        //        {
        //            var ultraStream = new UltraStreamWriter(csvStream);
        //            //csvStream.AutoFlush = false;
        //            csvStream.Write("Timestamp,");
        //            foreach (var signal in m_selectedMeasurements)
        //            {
        //                csvStream.Write(signal.Description);
        //                csvStream.Write(',');
        //            }
        //            csvStream.WriteLine();

        //            while (frameReader.Read())
        //            {
        //                csvStream.Write(frameReader.FrameTime.ToString("MM/dd/yyyy hh:mm:ss.fffffff"));
        //                csvStream.Write(',');

        //                foreach (var signal in m_selectedMeasurements)
        //                {
        //                    HistorianValueStruct value;
        //                    if (frameReader.Frame.TryGetValue((ulong)signal.PointID, out value))
        //                    {
        //                        ultraStream.Write(value.AsSingle);
        //                        //csvStream.Write(value.AsSingle);
        //                    }
        //                    //csvStream.Write(',');
        //                    ultraStream.Write(',');
        //                }
        //                ultraStream.Flush();
        //                csvStream.WriteLine();
        //            }

        //            csvStream.Flush();
        //        }
        //        database.Disconnect();
        //    }
        //}

        //private void BtnExport_Click(object sender, EventArgs e)
        //{
        //    Settings.Default.Save();
        //    if (m_meta is null)
        //    {
        //        MessageBox.Show("Please download the metadata first.");
        //        return;
        //    }
        //    if (m_selectedMeasurements is null || m_selectedMeasurements.Count == 0)
        //    {
        //        MessageBox.Show("There are no measurements to extract");
        //        return;
        //    }
        //    DateTime startTime = dateStartTime.Value;
        //    DateTime stopTime = dateStopTime.Value;
        //    if (startTime > stopTime)
        //    {
        //        MessageBox.Show("Start and Stop times are invalid");
        //        return;
        //    }
        //    TimeSpan interval = Resolutions.GetInterval((string)cmbResolution.SelectedItem);


        //    HistorianClientOptions clientOptions = new HistorianClientOptions();
        //    clientOptions.DefaultDatabase = Settings.Default.HistorianInstanceName;
        //    clientOptions.NetworkPort = Settings.Default.HistorianStreamingPort;
        //    clientOptions.ServerNameOrIp = Settings.Default.ServerIP;
        //    using (HistorianClient client = new HistorianClient(clientOptions))
        //    {
        //        m_readIndex = 0;
        //        m_fillMeasurements.Clear();
        //        m_measurementsInOrder.Clear();
        //        foreach (var signal in m_selectedMeasurements)
        //        {
        //            var m = new Measurements();
        //            m_fillMeasurements.Add((ulong)signal.PointID, m);
        //            m_measurementsInOrder.Add(m);
        //        }

        //        KeySeekFilterBase<HistorianKey> timeFilter;
        //        if (interval.Ticks != 0)
        //            timeFilter = TimestampFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, interval, new TimeSpan(TimeSpan.TicksPerMillisecond));
        //        else
        //            timeFilter = TimestampFilter.CreateFromRange<HistorianKey>(startTime, stopTime);

        //        var points = m_selectedMeasurements.Select((x) => (ulong)x.PointID).ToArray();
        //        var pointFilter = PointIDFilter.CreateFromList<HistorianKey>(points);

        //        var database = client.GetDefaultDatabase();

        //        using (var fillAdapter = database.GetPointStream(timeFilter, pointFilter).GetFillAdapter())
        //        using (var csvStream = new StreamWriter("C:\\temp\\file.csv"))
        //        {
        //            var ultraStream = new UltraStreamWriter(csvStream);
        //            //csvStream.AutoFlush = false;
        //            csvStream.Write("Timestamp,");
        //            foreach (var signal in m_selectedMeasurements)
        //            {
        //                csvStream.Write(signal.Description);
        //                csvStream.Write(',');
        //            }
        //            csvStream.WriteLine();

        //            m_readIndex++;
        //            while (fillAdapter.Fill(FillData))
        //            {
        //                csvStream.Write(fillAdapter.FrameTime.ToString("MM/dd/yyyy hh:mm:ss.fffffff"));
        //                csvStream.Write(',');

        //                foreach (var signal in m_measurementsInOrder)
        //                {
        //                    if (signal.ReadNumber == m_readIndex)
        //                    {
        //                        ultraStream.Write(signal.Value);
        //                    }
        //                    ultraStream.Write(',');
        //                }
        //                ultraStream.Flush();
        //                csvStream.WriteLine();
        //                m_readIndex++;

        //            }

        //            csvStream.Flush();
        //        }
        //        database.Disconnect();
        //    }
        //}

        private void BtnExport_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            if (m_meta is null)
            {
                MessageBox.Show("Please download the metadata first.");
                return;
            }
            if (m_selectedMeasurements is null || m_selectedMeasurements.Count == 0)
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

            BtnExport.Tag = BtnExport.Text;
            BtnExport.Text = "Exporting...";
            BtnExport.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            TimeSpan interval = Resolutions.GetInterval((string)cmbResolution.SelectedItem);
            Thread workerThread = new Thread(start =>
            {
                long processingStartTime = DateTime.UtcNow.Ticks;

                using (HistorianClient client = new HistorianClient(TxtServerIP.Text,int.Parse(TxtHistorianPort.Text)))
                {
                    m_readIndex = 0;
                    m_fillMeasurements.Clear();
                    m_measurementsInOrder.Clear();

                    foreach (MeasurementRow signal in m_selectedMeasurements)
                    {
                        Measurements m = new Measurements();
                        m_fillMeasurements.Add((ulong)signal.PointID, m);
                        m_measurementsInOrder.Add(m);
                    }

                    SeekFilterBase<HistorianKey> timeFilter;
                    if (interval.Ticks != 0)
                        timeFilter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, interval, new TimeSpan(TimeSpan.TicksPerMillisecond));
                    else
                        timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);

                    ulong[] points = m_selectedMeasurements.Select((x) => (ulong)x.PointID).ToArray();
                    MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points);

                    using (ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(TxtHistorianInstance.Text))
                    {
                        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Export.csv");

                        using (DataFillAdapter fillAdapter = database.GetPointStream(timeFilter, pointFilter).GetFillAdapter())
                        using (StreamWriter csvStream = new StreamWriter(fileName))
                        {
                            UltraStreamWriter ultraStream = new UltraStreamWriter(csvStream);
                            //csvStream.AutoFlush = false;
                            csvStream.Write("Timestamp,");
                            foreach (MeasurementRow signal in m_selectedMeasurements)
                            {
                                csvStream.Write(signal.Description);
                                csvStream.Write(',');
                            }
                            csvStream.WriteLine();

                            m_readIndex++;
                            while (fillAdapter.Fill(FillData))
                            {
                                csvStream.Write(fillAdapter.FrameTime.ToString("MM/dd/yyyy hh:mm:ss.fffffff"));
                                csvStream.Write(',');

                                foreach (Measurements signal in m_measurementsInOrder)
                                {
                                    if (signal.ReadNumber == m_readIndex)
                                    {
                                        ultraStream.Write(signal.Value);
                                    }
                                    ultraStream.Write(',');
                                }
                                ultraStream.Flush();
                                csvStream.WriteLine();
                                m_readIndex++;
                            }

                            csvStream.Flush();
                        }
                    }
                }

                Ticks runtime = DateTime.UtcNow.Ticks - processingStartTime;

                BeginInvoke(new Action<Ticks>(r => MessageBox.Show(r.ToElapsedTimeString(2), "Processing Time", MessageBoxButtons.OK, MessageBoxIcon.Information)), runtime);
                BeginInvoke(new Action(RestoreExportButton));
            });


            workerThread.Start();
        }

        private void RestoreExportButton()
        {
            Cursor.Current = Cursors.Default;
            BtnExport.Text = BtnExport.Tag.ToString();
            BtnExport.Enabled = true;
        }

        int m_readIndex;
        public Dictionary<ulong, Measurements> m_fillMeasurements = new Dictionary<ulong, Measurements>();
        public List<Measurements> m_measurementsInOrder = new List<Measurements>();
        public class Measurements
        {
            public int ReadNumber;
            public float Value;
        }

        void FillData(ulong pointId, HistorianValue key)
        {
            if (m_fillMeasurements.TryGetValue(pointId, out Measurements m))
            {
                m.ReadNumber = m_readIndex;
                m.Value = key.AsSingle;
            }
        }


    }
}
