//******************************************************************************************************
//  MainForm.cs - Gbtc
//
//  Copyright © 2019, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/11/2019 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF;
using GSF.Collections;
using GSF.ComponentModel;
using GSF.Diagnostics;
using GSF.IO;
using GSF.TimeSeries;
using GSF.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

// ReSharper disable AccessToModifiedClosure
// ReSharper disable PossibleMultipleEnumeration
namespace DataExtractor
{
    public partial class MainForm : Form
    {
        #region [ Members ]

        // Fields
        private Metadata m_metadata;
        private readonly LogPublisher m_log;
        private readonly GraphData m_graphData;
        private Settings m_settings;
        private bool m_formLoaded;
        private bool m_prefiltering;
        private bool m_exporting;
        private bool m_formClosing;

        #endregion

        #region [ Constructors ]
        public MainForm()
        {
            InitializeComponent();

            dataGridViewDevices.AutoGenerateColumns = true;

            // Save string format of select count label in its tag
            labelSelectCount.Tag = labelSelectCount.Text;
            labelSelectCount.Text = "";

            // Save string format of filter expression text box in its tag
            textBoxFilterExpression.Tag = textBoxFilterExpression.Text;
            textBoxFilterExpression.Text = "";

            // Create a new log publisher instance
            m_log = Logger.CreatePublisher(typeof(MainForm), MessageClass.Application);

            m_graphData = new GraphData();
        }

        #endregion

        #region [ Properties ]

        private int SelectedDeviceCount => m_metadata.Devices.Count(device => device.Selected);

        #endregion

        #region [ Methods ]

        // Form Event Handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Load current settings registering a symbolic reference to this form instance for use by default value expressions
                m_settings = new Settings(new Dictionary<string, object> {{ "Form", this }}.RegisterSymbols());

                // Restore last window size/location
                this.RestoreLayout();

                m_formLoaded = true;
            }
            catch (Exception ex)
            {
                m_log.Publish(MessageLevel.Error, "FormLoad", "Failed while loading settings", exception: ex);

            #if DEBUG
                throw;
            #endif
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_formClosing = true;

                // Save current window size/location
                this.SaveLayout();

                // Save any updates to current screen values
                m_settings.Save();
            }
            catch (Exception ex)
            {
                m_log.Publish(MessageLevel.Error, "FormClosing", "Failed while saving settings", exception: ex);

            #if DEBUG
                throw;
            #endif
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ShowUpdateMessage($"Loading meta-data from \"{m_settings.HostAddress}:{m_settings.Port}\"...");
                m_metadata = new Metadata(m_settings);
                tabControlOptions.TabIndex = 0;

                ShowUpdateMessage("Metadata loaded, extracting data types from meta-data from...");
                List<string> signals = m_metadata.Measurements.Select(row => row.SignalAcronym).Distinct().ToList();
                checkedListBoxDataTypes.Items.Clear();
                checkedListBoxDataTypes.Items.AddRange(signals.OrderByDescending(s => s).Cast<object>().ToArray());

                RefreshDevicesDataGrid();
                RefreshSelectedCount();

                ShowUpdateMessage("Ready for user data type selection.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to connect to \"{m_settings.HostAddress}:{m_settings.Port}\": {ex.Message}", "Connection Exception", MessageBoxButtons.OK);
            }
            finally
            {
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void buttonCancelPreFilter_Click(object sender, EventArgs e)
        {
            m_prefiltering = false;
        }

        private void buttonPreFilter_Click(object sender, EventArgs e)
        {
            m_prefiltering = true;
            SetButtonsEnabledState(false);
            ClearUpdateMessages();
            UpdateProgressBar(0);
            SetProgressBarMaximum(100);

            // Kick off a thread to start archive read
            new Thread(PreFilter) { IsBackground = true }.Start();
        }

        private void buttonShowGraph_Click(object sender, EventArgs e)
        {
            m_graphData.Show();
        }

        private void buttonExportCancel_Click(object sender, EventArgs e)
        {
            m_exporting = false;
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxExportFileName.Text))
            {
                MessageBox.Show("You must define an export file name before export.", "Error", MessageBoxButtons.OK);
            }
            else
            {
                m_exporting = true;
                SetButtonsEnabledState(false);
                ClearUpdateMessages();
                UpdateProgressBar(0);
                SetProgressBarMaximum(100);

                // Kick off a thread to start archive read
                new Thread(ExportData) {IsBackground = true}.Start();
            }
        }

        private void dataGridViewDevices_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn column = dataGridViewDevices.Columns[e.ColumnIndex];

            if (column.SortMode == DataGridViewColumnSortMode.NotSortable)
                return;

            SortOrder sortOrder = InvertSortOrder(column.HeaderCell);
            m_metadata.Devices.Sort(new DeviceDetailComparer(column.Name, sortOrder));

            // Refresh data grid
            RefreshDevicesDataGrid();
            dataGridViewDevices.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortOrder;
        }

        private void dataGridViewDevices_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex > m_metadata.Devices.Count)
                return;

            DataGridViewColumn column = dataGridViewDevices.Columns[e.ColumnIndex];

            if (column.Name.Equals("Selected", StringComparison.OrdinalIgnoreCase))
            {
                m_metadata.Devices[e.RowIndex].Selected = !m_metadata.Devices[e.RowIndex].Selected;
                dataGridViewDevices.RefreshEdit();
            }

            RefreshSelectedCount();
        }

        private void dataGridViewDevices_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dataGridViewDevices.ColumnCount; i++)
            {
                DataGridViewColumn column = dataGridViewDevices.Columns[i];
                column.ReadOnly = !column.Name.Equals("Selected", StringComparison.OrdinalIgnoreCase);
                column.Visible = !column.Name.Equals("UniqueID", StringComparison.OrdinalIgnoreCase);
                column.SortMode = DataGridViewColumnSortMode.Programmatic;
                column.Resizable = DataGridViewTriState.True;
            }

            dataGridViewDevices.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewDevices.Columns[0].Width = 80;
        }

        private void checkBoxSelectAllDevices_CheckedChanged(object sender, EventArgs e)
        {         
            bool selected = checkBoxSelectAllDevices.Checked;

            foreach (DeviceDetail device in m_metadata.Devices)
                device.Selected = selected;

            RefreshDevicesDataGrid();
            RefreshSelectedCount();
        }

        private void checkedListBoxDataTypes_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (!checkBoxExportFilePerDataType.Checked)
                RefreshFilterExpression(-1);
        }

        private void checkBoxExportFilePerDataType_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_formLoaded)
                return;

            RefreshFilterExpression(-1);
            FormElementChanged(sender, e);
        }

        private void textBoxExportFileName_TextChanged(object sender, EventArgs e)
        {
            if (!m_formLoaded)
                return;

            buttonExport.Enabled = !string.IsNullOrEmpty(textBoxExportFileName.Text);
            FormElementChanged(sender, e);
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxExportFileName.Text = saveFileDialog.FileName;
                textBoxExportFileName.SelectionStart = textBoxExportFileName.Text.Length;
                textBoxExportFileName.SelectionLength = 0;
            }
        }

        private void RefreshDevicesDataGrid()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshDevicesDataGrid));
            }
            else
            {
                dataGridViewDevices.DataSource = null;
                dataGridViewDevices.DataSource = m_metadata.Devices;
            }
        }

        private void RefreshSelectedCount()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshSelectedCount));
            }
            else
            {
                if (Visible && m_formLoaded)
                {
                    int selectedCount = SelectedDeviceCount;

                    labelSelectCount.Text = string.Format(labelSelectCount.Tag.ToString(), selectedCount);

                    if (selectedCount == m_metadata.Devices.Count && m_metadata.Devices.Count > 0)
                        checkBoxSelectAllDevices.Checked = true;
                    else if (selectedCount == 0)
                        checkBoxSelectAllDevices.Checked = false;

                    RefreshFilterExpression(selectedCount);

                    buttonPreFilter.Enabled = selectedCount > 0;
                }
            }
        }

        private void RefreshFilterExpression(int selectedCount)
        {
            if (selectedCount == -1)
                selectedCount = SelectedDeviceCount;

            textBoxFilterExpression.Text = GenerateFilterExpression(checkedListBoxDataTypes.CheckedItems.Cast<string>(), selectedCount);

            m_settings.Load();
        }

        private string GenerateFilterExpression(IEnumerable<string> signalTypes, int selectedDeviceCount)
        {
            StringBuilder filterExpression = new StringBuilder();

            if (selectedDeviceCount > 0)
                filterExpression.Append($"Device IN ({string.Join(", ", m_metadata.Devices.Where(device => device.Selected).Select(device => $"'{device.Name}'"))})");

            if (signalTypes.Any())
            {
                if (filterExpression.Length > 0)
                    filterExpression.Append(" AND ");

                filterExpression.Append($"SignalType IN ({string.Join(", ", signalTypes.Select(item => $"'{item}'"))})");
            }

            if (filterExpression.Length > 0)
                return string.Format(textBoxFilterExpression.Tag.ToString(), filterExpression);

            return "";
        }

        private SortOrder InvertSortOrder(DataGridViewColumnHeaderCell headerCell)
        {
            if (headerCell.SortGlyphDirection == SortOrder.None || headerCell.SortGlyphDirection == SortOrder.Descending)
            {
                headerCell.SortGlyphDirection = SortOrder.Ascending;
                return SortOrder.Ascending;
            }

            headerCell.SortGlyphDirection = SortOrder.Descending;
            return SortOrder.Descending;
        }

        // Form Element Accessors -- these functions allow access to form elements from non-UI threads

        private void FormElementChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(FormElementChanged), sender, e);
            }
            else
            {
                if (Visible && m_formLoaded)
                    m_settings?.UpdateProperties();
            }
        }

        private void ShowUpdateMessage(string message)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowUpdateMessage), message);
            }
            else
            {
                lock (textBoxMessageOutput)
                    textBoxMessageOutput.AppendText($"{message}{Environment.NewLine}");

                m_log.Publish(MessageLevel.Info, "StatusMessage", message);
            }
        }

        private void ClearUpdateMessages()
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(ClearUpdateMessages));
            }
            else
            {
                lock (textBoxMessageOutput)
                    textBoxMessageOutput.Text = "";
            }
        }

        private void SetButtonsEnabledState(bool enabled)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(SetButtonsEnabledState), enabled);
            }
            else
            {
                buttonConnect.Enabled = enabled;
                buttonPreFilter.Enabled = enabled && SelectedDeviceCount > 0;
                buttonCancelPreFilter.Visible = !enabled && m_prefiltering;
                buttonExport.Enabled = enabled && !string.IsNullOrEmpty(textBoxExportFileName.Text);
                buttonCancelExport.Visible = !enabled && m_exporting;
                buttonShowGraph.Visible = m_graphData.HasData;
                buttonSelectFile.Enabled = enabled;
            }
        }

        private void UpdateProgressBar(int value)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                if (value < progressBar.Minimum)
                    value = progressBar.Minimum;

                if (value > progressBar.Maximum)
                    progressBar.Maximum = value;

                progressBar.Value = value;
            }
        }

        private void SetProgressBarMaximum(int maximum)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
                BeginInvoke(new Action<int>(SetProgressBarMaximum), maximum);
            else
                progressBar.Maximum = maximum;
        }

        // Internal Functions

        private void PreFilter(object state)
        {
            const int MaxPoints = 50;

            try
            {
                double timeRange = (m_settings.EndTime - m_settings.StartTime).TotalSeconds;
                Dictionary<string, DeviceDetail> deviceMap = new Dictionary<string, DeviceDetail>(StringComparer.OrdinalIgnoreCase);
                Dictionary<Guid, DeviceDetail> signalMap = new Dictionary<Guid, DeviceDetail>();
                Dictionary<Guid, DeviceStats> deviceStats = new Dictionary<Guid, DeviceStats>();
                Dictionary<Guid, Tuple<Ticks, List<double>, List<double>>> plotValues = new Dictionary<Guid, Tuple<Ticks, List<double>, List<double>>>();
                bool readComplete = false;
                long receivedPoints = 0L;
                Ticks operationTime;
                Ticks operationStartTime;
                double pointInterval = timeRange / MaxPoints;

                void handleNewMeasurements(ICollection<IMeasurement> measurements)
                {
                    bool showMessage = receivedPoints + measurements.Count >= (receivedPoints / m_settings.MessageInterval + 1) * m_settings.MessageInterval;

                    receivedPoints += measurements.Count;

                    foreach (IMeasurement measurement in measurements)
                    {
                        Guid signalID = measurement.ID;

                        if (signalMap.TryGetValue(signalID, out DeviceDetail device) && deviceStats.TryGetValue(device.UniqueID, out DeviceStats stats))
                        {
                            stats.Total++;

                            if (!measurement.ValueQualityIsGood())
                                stats.BadDataCount++;

                            if (!measurement.TimestampQualityIsGood())
                                stats.BadTimeCount++;
                        }

                        Tuple<Ticks, List<double>, List<double>> plotData = plotValues.GetOrAdd(signalID, _ => new Tuple<Ticks, List<double>, List<double>>(measurement.Timestamp, new List<double>(new[] { (double)measurement.Timestamp }), new List<double>(new[] { measurement.AdjustedValue })));

                        if ((measurement.Timestamp - plotData.Item1).ToSeconds() > pointInterval)
                        {
                            plotData.Item2.Add(measurement.Timestamp);
                            plotData.Item3.Add(measurement.AdjustedValue);
                            plotValues[signalID] = new Tuple<Ticks, List<double>, List<double>>(measurement.Timestamp, plotData.Item2, plotData.Item3);
                        }
                    }

                    if (showMessage && measurements.Count > 0)
                    {
                        IMeasurement measurement = measurements.First();
                        ShowUpdateMessage($"{Environment.NewLine}{receivedPoints:N0} points read so far averaging {receivedPoints / (DateTime.UtcNow.Ticks - operationStartTime).ToSeconds():N0} points per second.");
                        UpdateProgressBar((int)((1.0D - new Ticks(m_settings.EndTime.Ticks - (long)measurement.Timestamp).ToSeconds() / timeRange) * 100.0D));
                    }
                }

                void readCompleted()
                {
                    readComplete = true;
                    ShowUpdateMessage("Data read completed.");

                    foreach (KeyValuePair<Guid, Tuple<Ticks, List<double>, List<double>>> plotData in plotValues)
                        m_graphData.PlotLine(plotData.Value.Item2, plotData.Value.Item3);
                }

                operationStartTime = DateTime.UtcNow.Ticks;

                foreach (DeviceDetail device in m_metadata.Devices.Where(d => d.Selected))
                    deviceMap[device.Name] = device;

                foreach (MeasurementDetail measurement in m_metadata.Measurements)
                {
                    if (deviceMap.TryGetValue(measurement.DeviceName, out DeviceDetail device))
                        signalMap[measurement.SignalID] = device;
                }

                foreach (DeviceDetail device in m_metadata.Devices.Where(d => d.Selected))
                    deviceStats[device.UniqueID] = new DeviceStats { Device = device };

                m_graphData.ClearPlots();

                using (new DataReceiver($"server={m_settings.HostAddress}; port={m_settings.Port}; interface=0.0.0.0", m_settings.FilterExpression, m_settings.StartTime, m_settings.EndTime)
                {
                    NewMeasurementsCallback = handleNewMeasurements,
                    StatusMessageCallback = ShowUpdateMessage,
                    ProcessExceptionCallback = ex => ShowUpdateMessage($"Error: {ex.Message}"),
                    ReadCompletedCallback = readCompleted
                })
                {
                    while (!m_formClosing && !readComplete && m_prefiltering)
                        Thread.Sleep(500);
                }

                long expectedPoints = (long)(m_settings.FrameRate * timeRange);

                foreach (DeviceStats stats in deviceStats.Values)
                {
                    DeviceDetail device = stats.Device;
                    stats.MissingDataCount = expectedPoints - stats.Total;
                    double badData, badTime, missingData;

                    if (stats.Total == 0)
                    {
                        badTime = 0.0D;
                        badData = 0.0D;
                        missingData = 1.0D;
                    }
                    else
                    {
                        badData = stats.BadDataCount / (double)stats.Total;
                        badTime = stats.BadTimeCount / (double)stats.Total;
                        missingData = stats.MissingDataCount / (double)stats.Total;

                        if (badData < 0.0D)
                            badData = 0.0D;

                        if (badTime < 0.0D)
                            badTime = 0.0D;

                        if (missingData < 0.0D)
                            missingData = 0.0D;
                    }

                    if (stats.BadDataCount / (double)stats.Total * 100.0D > m_settings.AcceptableBadData)
                    {
                        device.Selected = false;
                        ShowUpdateMessage($"Device \"{device.Name}\" unselected - too much bad data: {badData:0.00%}...");
                    }
                    else if (stats.BadTimeCount / (double)stats.Total * 100.0D > m_settings.AcceptableBadTime)
                    {
                        device.Selected = false;
                        ShowUpdateMessage($"Device \"{device.Name}\" unselected - too much bad data with bad time: {badTime:0.00%}...");
                    }
                    else if (stats.MissingDataCount / (double)stats.Total * 100.0D > m_settings.AcceptableMissingData)
                    {
                        device.Selected = false;
                        ShowUpdateMessage($"Device \"{device.Name}\" unselected - too much missing data: {missingData:0.00%}...");
                    }
                }

                RefreshSelectedCount();
                RefreshDevicesDataGrid();

                operationTime = DateTime.UtcNow.Ticks - operationStartTime;

                if (m_formClosing || !m_prefiltering)
                {
                    ShowUpdateMessage("*** Data Pre-filter Canceled ***");
                    UpdateProgressBar(0);
                }
                else
                {
                    ShowUpdateMessage("*** Data Pre-filter Complete ***");
                    UpdateProgressBar(100);
                }

                ShowUpdateMessage($"Total pre-filter processing time {operationTime.ToElapsedTimeString(3)} at {receivedPoints / operationTime.ToSeconds():N0} points per second.{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                ShowUpdateMessage($"!!! Failure during historian read: {ex.Message}");
                m_log.Publish(MessageLevel.Error, "HistorianDataRead", "Failed while reading data from the historian", exception: ex);
            }
            finally
            {
                SetButtonsEnabledState(true);
            }
        }

        private void ExportData(object state)
        {
            try
            {
                const int TargetBufferSize = 524288;

                Dictionary<string, DeviceDetail> deviceMap = new Dictionary<string, DeviceDetail>(StringComparer.OrdinalIgnoreCase);
                Dictionary<Guid, DeviceDetail> signalMap = new Dictionary<Guid, DeviceDetail>();
                Dictionary<Guid, DeviceStats> deviceStats = new Dictionary<Guid, DeviceStats>();

                string hostAddress = m_settings.HostAddress;
                int port = m_settings.Port;
                DateTime startTime = m_settings.StartTime;
                DateTime endTime = m_settings.EndTime;
                int frameRate = m_settings.FrameRate;
                bool alignTimestamps = m_settings.AlignTimestamps;
                bool missingAsNaN = m_settings.ExportMissingAsNaN;
                bool fillMissingTimestamps = alignTimestamps && m_settings.FillInMissingTimestamps;
                int messageInterval = m_settings.MessageInterval;
                bool exportFilePerDataType = m_settings.ExportFilePerDataType;
                string exportFileName = m_settings.ExportFileName;
                double timeRange = (endTime - startTime).TotalSeconds;

                Ticks operationStartTime;
                StringBuilder readBuffer = new StringBuilder(TargetBufferSize * 2);
                ManualResetEventSlim bufferReady = new ManualResetEventSlim(false);
                List<string> writeBuffer = new List<string>();
                object writeBufferLock = new object();

                TextWriter writer = null;
                bool readComplete = false;
                bool writeComplete = false;
                long receivedPoints = 0L;
                int exports = 0;
                int totalExports;

                Ticks[] subseconds = Ticks.SubsecondDistribution(frameRate);
                long interval = subseconds.Length > 1 ? subseconds[1].Value : Ticks.PerSecond;

                Dictionary<Guid, int> signalIDIndex = new Dictionary<Guid, int>();
                double[] values = {};
                int deviceCount = 0;
                int measurementCount = 0;
                long lastTimestamp = 0L;
                long timestamp = 0L;
                bool pastFirstRow = false;

                void bufferValues()
                {
                    // Write row values
                    readBuffer.Append(missingAsNaN ? string.Join(",", values) : string.Join(",", values.Select(val => double.IsNaN(val) ? "" : $"{val}")));

                    if (readBuffer.Length < TargetBufferSize)
                        return;

                    lock (writeBufferLock)
                        writeBuffer.Add(readBuffer.ToString());

                    readBuffer.Clear();
                    bufferReady.Set();
                }

                void writeData()
                {
                    string[] localBuffer;

                    while (writeBuffer.Count > 0 || !writeComplete)
                    {
                        bufferReady.Wait();
                        bufferReady.Reset();

                        lock (writeBufferLock)
                        {
                            localBuffer = writeBuffer.ToArray();
                            writeBuffer.Clear();
                        }

                        foreach (string buffer in localBuffer)
                            writer?.Write(buffer);
                    }
                }

                string getHeaders(HashSet<string> signalTypes)
                {
                    StringBuilder headers = new StringBuilder();

                    headers.AppendLine($"Data extraction from \"{hostAddress}:{port}\" exported on {DateTime.UtcNow.ToString(TimeTagBase.DefaultFormat)} UTC");
                    headers.AppendLine($"Export range: {startTime.ToString(TimeTagBase.DefaultFormat)} UTC to {endTime.ToString(TimeTagBase.DefaultFormat)} UTC");

                    StringBuilder deviceRow = new StringBuilder();
                    StringBuilder measurementRow = new StringBuilder();

                    measurementRow.Append("\"Timestamp\"");
                    signalIDIndex.Clear();

                    foreach (DeviceDetail device in m_metadata.Devices)
                    {
                        if (!device.Selected)
                            continue;

                        if (!m_metadata.Measurements.Any(m => string.Equals(m.DeviceName, device.Name, StringComparison.OrdinalIgnoreCase)))
                            continue;

                        deviceRow.Append($",{device.Name}");
                        deviceCount++;

                        foreach (MeasurementDetail measurement in m_metadata.Measurements.Where(m => string.Equals(m.DeviceName, device.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (!signalTypes.Contains(measurement.SignalAcronym))
                                continue;

                            deviceRow.Append(",");
                            measurementRow.Append($",{measurement.PointTag} [{measurement.SignalAcronym}]");
                            signalIDIndex.Add(measurement.SignalID, measurementCount++);
                        }
                    }

                    headers.AppendLine($"Device count: {deviceCount}");
                    headers.AppendLine($"Measurement count: {measurementCount}");
                    headers.AppendLine();

                    headers.AppendLine(deviceRow.ToString());
                    headers.Append(measurementRow);

                    values = new double[measurementCount];

                    for (int i = 0; i < values.Length; i++)
                        values[i] = double.NaN;

                    return headers.ToString();
                }

                void handleNewMeasurements(ICollection<IMeasurement> measurements)
                {
                    bool showMessage = receivedPoints + measurements.Count >= (receivedPoints / messageInterval + 1) * messageInterval;

                    receivedPoints += measurements.Count;

                    foreach (IMeasurement measurement in measurements)
                    {
                        if (signalMap.TryGetValue(measurement.Key.SignalID, out DeviceDetail device) && deviceStats.TryGetValue(device.UniqueID, out DeviceStats stats))
                        {
                            stats.Total++;

                            if (!measurement.ValueQualityIsGood())
                                stats.BadDataCount++;

                            if (!measurement.TimestampQualityIsGood())
                                stats.BadTimeCount++;
                        }

                        if (signalIDIndex.TryGetValue(measurement.ID, out int index))
                        {
                            if (alignTimestamps)
                                timestamp = Ticks.RoundToSubsecondDistribution(measurement.Timestamp, frameRate).Value;
                            else
                                timestamp = measurement.Timestamp;

                            // Start a new row for each encountered new timestamp
                            if (timestamp != lastTimestamp)
                            {
                                if (lastTimestamp > 0 && pastFirstRow)
                                    bufferValues();

                                for (int i = 0; i < values.Length; i++)
                                    values[i] = float.NaN;

                                // Handle any missing data rows
                                if (fillMissingTimestamps && lastTimestamp > 0 && timestamp > lastTimestamp)
                                {
                                    long difference = timestamp - lastTimestamp;

                                    if (difference > interval)
                                    {
                                        long interpolated = lastTimestamp;

                                        for (long i = 1; i < difference / interval; i++)
                                        {
                                            interpolated = Ticks.RoundToSubsecondDistribution(interpolated + interval, frameRate).Value;
                                            readBuffer.Append($"{Environment.NewLine}{new DateTime(interpolated, DateTimeKind.Utc).ToString(TimeTagBase.DefaultFormat)},");
                                            bufferValues();
                                        }
                                    }
                                }

                                readBuffer.Append($"{Environment.NewLine}{new DateTime(timestamp, DateTimeKind.Utc).ToString(TimeTagBase.DefaultFormat)},");
                                lastTimestamp = timestamp;
                                pastFirstRow = true;
                            }

                            // Save value to its column
                            values[index] = measurement.AdjustedValue;
                        }
                    }

                    if (showMessage && measurements.Count > 0)
                    {
                        IMeasurement measurement = measurements.First();
                        ShowUpdateMessage($"{Environment.NewLine}{receivedPoints:N0} points read so far averaging {receivedPoints / (DateTime.UtcNow.Ticks - operationStartTime).ToSeconds():N0} points per second.");

                        int exportFraction = (int)(100.0D * exports / totalExports);
                        double currentExportProgress = 1.0D - (endTime.Ticks - measurement.Timestamp).ToSeconds() / timeRange / totalExports;

                        UpdateProgressBar(exportFraction + (int)(currentExportProgress * 100.0D));
                    }
                }

                void flushBuffers()
                {
                    // Flush last row
                    if (timestamp > 0)
                        bufferValues();

                    // Flush remaining buffers
                    if (readBuffer.Length > 0)
                    {
                        lock (writeBufferLock)
                            writeBuffer.Add(readBuffer.ToString());
                    }
                }

                void readCompleted()
                {
                    readComplete = true;

                    if (++exports == totalExports)
                        ShowUpdateMessage($"Data read{(totalExports > 1 ? "s" : "" )} completed.");
                    else
                        ShowUpdateMessage($"{exports} of {totalExports} data reads completed.");
                }

                void exportData(HashSet<string> signalTypes, string suffix = null)
                {
                    string fileName;

                    if (string.IsNullOrEmpty(suffix))
                        fileName = exportFileName;
                    else
                        fileName = $"{FilePath.GetDirectoryName(exportFileName)}{FilePath.GetFileNameWithoutExtension(exportFileName)}_{suffix}{FilePath.GetExtension(exportFileName)}";

                    readComplete = false;
                    lastTimestamp = 0L;
                    measurementCount = 0;
                    pastFirstRow = false;

                    using (writer = File.CreateText(fileName))
                    {
                        writer.Write(getHeaders(signalTypes));
                        writeComplete = false;
                        bufferReady.Reset();

                        using (Task writeTask = Task.Factory.StartNew(writeData))
                        {
                            try
                            {
                                if (measurementCount > 0)
                                {
                                    ShowUpdateMessage($"Starting data read for {string.Join(", ", signalTypes)} signal type{(signalTypes.Count > 1 ? "s" : "")}...");

                                    using (new DataReceiver($"server={hostAddress}; port={port}; interface=0.0.0.0", GenerateFilterExpression(signalTypes, SelectedDeviceCount), startTime, endTime)
                                    {
                                        NewMeasurementsCallback = handleNewMeasurements,
                                        StatusMessageCallback = ShowUpdateMessage,
                                        ProcessExceptionCallback = ex => ShowUpdateMessage($"Error: {ex.Message}"),
                                        ReadCompletedCallback = readCompleted
                                    })
                                    {
                                        while (!m_formClosing && !readComplete && m_exporting)
                                            Thread.Sleep(500);
                                    }

                                    flushBuffers();
                                }
                            }
                            finally
                            {
                                writeComplete = true;
                                bufferReady.Set();
                            }

                            writeTask.Wait();
                        }
                    }
                }

                // Start export operations
                HashSet<string> selectedSignalTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (DeviceDetail device in m_metadata.Devices.Where(d => d.Selected))
                    deviceMap[device.Name] = device;

                foreach (MeasurementDetail measurement in m_metadata.Measurements)
                {
                    if (deviceMap.TryGetValue(measurement.DeviceName, out DeviceDetail device))
                        signalMap[measurement.SignalID] = device;
                }

                foreach (DeviceDetail device in m_metadata.Devices.Where(d => d.Selected))
                    deviceStats[device.UniqueID] = new DeviceStats { Device = device };

                if (checkedListBoxDataTypes.CheckedItems.Count > 0)
                    selectedSignalTypes.UnionWith(checkedListBoxDataTypes.CheckedItems.Cast<string>());
                else
                    selectedSignalTypes.UnionWith(checkedListBoxDataTypes.Items.Cast<string>());

                operationStartTime = DateTime.UtcNow.Ticks;

                if (exportFilePerDataType)
                {
                    totalExports = selectedSignalTypes.Count;

                    foreach (string signalType in selectedSignalTypes)
                        exportData(new HashSet<string>(new[] { signalType }, StringComparer.OrdinalIgnoreCase), signalType);
                }
                else
                {
                    totalExports = 1;
                    exportData(selectedSignalTypes);
                }

                long expectedPoints = (long)(frameRate * timeRange);

                // Show export summary information
                foreach (DeviceStats stats in deviceStats.Values)
                {
                    DeviceDetail device = stats.Device;
                    stats.MissingDataCount = expectedPoints - stats.Total;
                    double badData, badTime, missingData;

                    if (stats.Total == 0)
                    {
                        badTime = 0.0D;
                        badData = 0.0D;
                        missingData = 1.0D;
                    }
                    else
                    {
                        badData = stats.BadDataCount / (double)stats.Total;
                        badTime = stats.BadTimeCount / (double)stats.Total;
                        missingData = stats.MissingDataCount / (double)stats.Total;

                        if (badData < 0.0D)
                            badData = 0.0D;

                        if (badTime < 0.0D)
                            badTime = 0.0D;

                        if (missingData < 0.0D)
                            missingData = 0.0D;
                    }

                    ShowUpdateMessage($"Device \"{device.Name}\" bad data: {badData:0.00%}, bad time: {badTime:0.00%}, missing data: {missingData:0.00%}...");
                }

                Ticks operationTime = DateTime.UtcNow.Ticks - operationStartTime;

                if (m_formClosing || !m_exporting)
                {
                    ShowUpdateMessage("*** Data Export Canceled ***");
                    UpdateProgressBar(0);
                }
                else
                {
                    ShowUpdateMessage("*** Data Export Complete ***");
                    UpdateProgressBar(100);
                }

                ShowUpdateMessage($"Total export processing time {operationTime.ToElapsedTimeString(3)} at {receivedPoints / operationTime.ToSeconds():N0} points per second.{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                ShowUpdateMessage($"!!! Failure during historian read: {ex.Message}");
                m_log.Publish(MessageLevel.Error, "HistorianDataRead", "Failed while reading data from the historian", exception: ex);
            }
            finally
            {
                SetButtonsEnabledState(true);
            }
        }

        #endregion

        #region [ Static ]

        // Static Constructor
        static MainForm()
        {
            // Set default logging path
            Logger.FileWriter.SetPath(FilePath.GetAbsolutePath(""), VerboseLevel.Ultra);
        }

        #endregion
    }
}
