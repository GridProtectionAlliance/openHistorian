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
using GSF.ComponentModel;
using GSF.Diagnostics;
using GSF.IO;
using GSF.TimeSeries;
using GSF.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DataExtractor
{
    public partial class MainForm : Form
    {
        #region [ Members ]

        // Fields
        private Metadata m_metadata;
        private readonly LogPublisher m_log;
        private Settings m_settings;
        private bool m_formLoaded;
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

        private void buttonPreFilter_Click(object sender, EventArgs e)
        {
            SetButtonsEnabledState(false);
            ClearUpdateMessages();
            UpdateProgressBar(0);
            SetProgressBarMaximum(100);

            // Kick off a thread to start archive read
            new Thread(PreFilter) { IsBackground = true }.Start();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxExportFileName.Text))
            {
                SetButtonsEnabledState(false);
                ClearUpdateMessages();
                UpdateProgressBar(0);
                SetProgressBarMaximum(100);

                // Kick off a thread to start archive read
                new Thread(ExportData) { IsBackground = true }.Start();
            }
            else
            {
                MessageBox.Show("You must define an export file name before export.", "Error", MessageBoxButtons.OK);
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
            buttonExport.Enabled = !string.IsNullOrEmpty(textBoxExportFileName.Text);
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
            dataGridViewDevices.DataSource = null;
            dataGridViewDevices.DataSource = m_metadata.Devices;
        }

        private void RefreshSelectedCount()
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

        private void RefreshFilterExpression(int selectedCount)
        {
            StringBuilder filterExpression = new StringBuilder();

            if (selectedCount == -1)
                selectedCount = SelectedDeviceCount;

            if (selectedCount > 0)
                filterExpression.Append($"Device IN ({string.Join(", ", m_metadata.Devices.Where(device => device.Selected).Select(device => $"'{device.Name}'"))})");

            if (!checkBoxExportFilePerDataType.Checked && checkedListBoxDataTypes.CheckedItems.Count > 0)
            {
                if (filterExpression.Length > 0)
                    filterExpression.Append(" AND ");

                filterExpression.Append($"SignalType IN ({string.Join(", ", checkedListBoxDataTypes.CheckedItems.Cast<string>().Select(item => $"'{item}'"))})");
            }

            if (filterExpression.Length > 0)
                textBoxFilterExpression.Text = string.Format(textBoxFilterExpression.Tag.ToString(), filterExpression);
            else
                textBoxFilterExpression.Text = "";
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
                buttonExport.Enabled = enabled && !string.IsNullOrEmpty(textBoxExportFileName.Text);
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
            try
            {
                double timeRange = (m_settings.EndTime - m_settings.StartTime).TotalSeconds;
                Dictionary<string, DeviceDetail> deviceMap = new Dictionary<string, DeviceDetail>(StringComparer.OrdinalIgnoreCase);
                Dictionary<Guid, DeviceDetail> signalMap = new Dictionary<Guid, DeviceDetail>();
                Dictionary<DeviceDetail, DeviceStats> deviceStats = new Dictionary<DeviceDetail, DeviceStats>();
                bool readComplete = false;
                long receivedPoints = 0L;
                Ticks operationTime;
                Ticks operationStartTime;

                void handleNewMeasurements(ICollection<IMeasurement> measurements)
                {
                    bool showMessage = (receivedPoints + measurements.Count >= (receivedPoints / m_settings.MessageInterval + 1) * m_settings.MessageInterval);

                    receivedPoints += measurements.Count;

                    foreach (IMeasurement measurement in measurements)
                    {
                        if (signalMap.TryGetValue(measurement.Key.SignalID, out DeviceDetail device) && deviceStats.TryGetValue(device, out DeviceStats stats))
                        {
                            stats.Total++;

                            if (!measurement.ValueQualityIsGood())
                                stats.BadDataCount++;

                            if (!measurement.TimestampQualityIsGood())
                                stats.BadTimeCount++;
                        }
                    }

                    if (showMessage && measurements.Count > 0)
                    {
                        IMeasurement measurement = measurements.First();
                        ShowUpdateMessage($"{Environment.NewLine}{receivedPoints:N0} points read so far averaging {receivedPoints / (DateTime.UtcNow.Ticks - operationStartTime).ToSeconds():N0} points per second.");
                        UpdateProgressBar((int)((1.0D - new Ticks(m_settings.EndTime.Ticks - (long)measurement.Timestamp).ToSeconds() / timeRange) * 100.0D));
                    }
                }

                void readCompleted(string message)
                {
                    readComplete = true;
                    ShowUpdateMessage(message);
                }

                operationStartTime = DateTime.UtcNow.Ticks;

                foreach (DeviceDetail device in m_metadata.Devices)
                    deviceMap[device.Name] = device;

                foreach (MeasurementDetail measurement in m_metadata.Measurements)
                {
                    if (deviceMap.TryGetValue(measurement.DeviceName, out DeviceDetail device))
                        signalMap[measurement.SignalID] = device;
                }

                foreach (DeviceDetail device in m_metadata.Devices)
                    deviceStats[device] = new DeviceStats();

                using (DataReceiver receiver = new DataReceiver($"server={m_settings.HostAddress}; port={m_settings.Port}; interface=0.0.0.0", m_settings.FilterExpression, m_settings.StartTime, m_settings.EndTime, handleNewMeasurements, ShowUpdateMessage, ex => ShowUpdateMessage($"Error: {ex.Message}"), readCompleted))
                {
                    while (!m_formClosing && !readComplete)
                        Thread.Sleep(1000);
                }

                operationTime = DateTime.UtcNow.Ticks - operationStartTime;

                if (m_formClosing)
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
                HashSet<ulong> pointIDList;
                double timeRange = (m_settings.EndTime - m_settings.StartTime).TotalSeconds;
                long scannedPoints = 0L;
                long processedPoints = 0L;
                long processedDataBlocks = 0L;
                long duplicatePoints = 0L;
                Ticks operationTime;
                Ticks operationStartTime;
                //DataPoint point = new DataPoint();
                DateTime firstTimestamp = new DateTime(0L);
                DateTime lastTimestamp = new DateTime(0L);
                long lastRowIndex;
                
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
