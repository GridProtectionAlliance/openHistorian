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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using GSF;
using GSF.ComponentModel;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Windows.Forms;

namespace DataExtractor
{
    public partial class MainForm : Form
    {
        #region [ Members ]

        // Fields
        private Metadata m_metadata;
        private readonly LogPublisher m_log;
        private Settings m_settings;
        private bool m_formClosing;

        #endregion

        #region [ Constructors ]
        public MainForm()
        {
            InitializeComponent();

            dataGridViewDevices.AutoGenerateColumns = true;

            // Create a new log publisher instance
            m_log = Logger.CreatePublisher(typeof(MainForm), MessageClass.Application);
        }

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
            //try
            //{
                Cursor.Current = Cursors.WaitCursor;

                ShowUpdateMessage($"Loading meta-data from \"{m_settings.HostAddress}:{m_settings.Port}\"...");
                m_metadata = new Metadata(m_settings);
                tabControlOptions.TabIndex = 0;

                // Unique data type list
                ShowUpdateMessage("Metadata loaded, extracting data types from meta-data from...");
                List<string> signals = m_metadata.Measurements.Select(row => row.SignalAcronym).Distinct().ToList();
                checkedListBoxDataTypes.Items.Clear();
                checkedListBoxDataTypes.Items.AddRange(signals.OrderByDescending(s => s).Cast<object>().ToArray());

                RefreshDevicesDataGrid();

                ShowUpdateMessage("Ready for user data type selection.");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this, $"Failed to connect to \"{m_settings.HostAddress}:{m_settings.Port}\": {ex.Message}", "Connection Exception", MessageBoxButtons.OK);
            //}
            //finally
            //{
                Cursor.Current = Cursors.Arrow;
            //}
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            if (m_settings.AlignTimestamps || m_settings.ExportMissingAsNaN)
            {
                SetGoButtonEnabledState(false);
                ClearUpdateMessages();
                UpdateProgressBar(0);
                SetProgressBarMaximum(100);

                // Kick off a thread to start archive read
                new Thread(ReadArchive) { IsBackground = true }.Start();
            }
            else
            {
                MessageBox.Show("You must pick at least one algorithm to execute.", "Error", MessageBoxButtons.OK);
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
        }

        private void RefreshDevicesDataGrid()
        {
            dataGridViewDevices.DataSource = null;
            dataGridViewDevices.DataSource = m_metadata.Devices;
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
                if (Visible)
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

        private void SetGoButtonEnabledState(bool enabled)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
                BeginInvoke(new Action<bool>(SetGoButtonEnabledState), enabled);
            else
                buttonGo.Enabled = enabled;
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

        private void ReadArchive(object state)
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
                SetGoButtonEnabledState(true);
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
