//******************************************************************************************************
//  BulkCalculationState.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
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
//  02/25/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using BulkCalculationState.Model;
using GSF;
using GSF.Collections;
using GSF.ComponentModel;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Threading;
using GSF.Windows.Forms;

namespace BulkCalculationState
{
    public partial class BulkCalculationState : Form
    {
        private const string SourceApp = "openHistorian";

        private Settings m_settings;
        private readonly LogPublisher m_log;
        private bool m_formLoaded;
        private volatile bool m_formClosing;
        private string m_sourcePath;
        private AdoDataConnection m_connection;
        private TableOperations<CustomActionAdapter> m_actionAdapterTable;
        private List<CustomActionAdapter> m_actionAdapters;
        private List<FilteredActionAdapter> m_filteredActionAdapters;
        private ShortSynchronizedOperation m_updateTotals;
        private Process m_consoleProcess;
        private string m_consoleOutput;
        private bool m_updatingSelectAllStates;
        private bool m_reconnecting;

        public BulkCalculationState()
        {
            InitializeComponent();

            // Create a new log publisher instance
            m_log = Logger.CreatePublisher(typeof(BulkCalculationState), MessageClass.Application);
        }

        private void BulkCalculationState_Load(object sender, EventArgs e)
        {
            try
            {
                // Load current settings registering a symbolic reference to this form instance for use by default value expressions
                m_settings = new Settings(new Dictionary<string, object> { { "Form", this } }.RegisterSymbols());

                // Restore last window size/location
                this.RestoreLayout();

                m_sourcePath = FilePath.GetAbsolutePath("");
                string configFile = $@"{m_sourcePath}\{SourceApp}.exe.config";

                if (!File.Exists(configFile))
                {
                    m_sourcePath = $@"C:\Program Files\{SourceApp}";
                    configFile = $@"{m_sourcePath}\{SourceApp}.exe.config";
                }

                if (!File.Exists(configFile))
                    throw new FileNotFoundException($"Config file for {SourceApp} application \"{configFile}\" was not found.");

                XDocument serviceConfig = XDocument.Load(configFile);

                string connectionString = serviceConfig
                    .Descendants("systemSettings")
                    .SelectMany(systemSettings => systemSettings.Elements("add"))
                    .Where(element => "ConnectionString".Equals((string)element.Attribute("name"), StringComparison.OrdinalIgnoreCase))
                    .Select(element => (string)element.Attribute("value"))
                    .FirstOrDefault();

                string dataProviderString = serviceConfig
                    .Descendants("systemSettings")
                    .SelectMany(systemSettings => systemSettings.Elements("add"))
                    .Where(element => "DataProviderString".Equals((string)element.Attribute("name"), StringComparison.OrdinalIgnoreCase))
                    .Select(element => (string)element.Attribute("value"))
                    .FirstOrDefault();

                m_connection = new AdoDataConnection(connectionString, dataProviderString);
                m_actionAdapterTable = new TableOperations<CustomActionAdapter>(m_connection);
                RefreshActionAdapters();

                m_updateTotals = new ShortSynchronizedOperation(UpdateTotals);
                SyncCheckedListBox();
                ConnectConsole();

                m_formLoaded = true;
            }
            catch (Exception ex)
            {
                m_log.Publish(MessageLevel.Error, "FormLoad", "Failed while loading settings", exception: ex);

            #if DEBUG
                throw;
            #else
                MessageBox.Show(this, $"Failed during initialization: {ex.Message}", "Initialization Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            #endif
            }
        }

        private void ConnectConsole()
        {
            lock (m_settings)
            {
                Invoke(new Action(() => { buttonEnableSelected.Enabled = false; }));

                if (!(m_consoleOutput is null))
                {
                    m_consoleProcess.OutputDataReceived -= m_consoleProcess_OutputDataReceived;
                    m_consoleProcess.Close();
                    m_consoleProcess.Dispose();
                }
            
                m_consoleOutput = "";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = $@"{m_sourcePath}\{SourceApp}Console.exe"
                };

                // Pre-start console process for quick update responses
                m_consoleProcess = new Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                m_consoleProcess.OutputDataReceived += m_consoleProcess_OutputDataReceived;
                m_consoleProcess.Start();
                m_consoleProcess.BeginOutputReadLine();
            }
        }

        private void ClearConsoleOutput()
        {
            lock (m_settings)
                m_consoleOutput = "";
        }

        private void m_consoleProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            const int MaxOutput = 8196;

            Debug.WriteLine(e.Data);

            lock (m_settings)
            {
                string output = m_consoleOutput + e.Data;

                if (output.Length > MaxOutput)
                    output = output.TruncateLeft(MaxOutput);

                m_consoleOutput = output;
            }

            if (!buttonEnableSelected.Enabled && m_consoleOutput.Contains("Remote client connected"))
            {
                ClearConsoleOutput();
                BeginInvoke(new Action(() => { buttonEnableSelected.Enabled = true; }));
                m_reconnecting = false;
            }
        }

        private void BulkCalculationState_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_formClosing = true;

                // Save current window size/location
                this.SaveLayout();

                // Save any updates to current screen values
                m_settings.Save();

                // Close associated console process
                m_consoleProcess.Close();
            }
            catch (Exception ex)
            {
                m_log.Publish(MessageLevel.Error, "FormClosing", "Failed while saving settings", exception: ex);

            #if DEBUG
                throw;
            #endif
            }
        }

        private void BulkCalculationState_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_settings?.Dispose();
        }

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

        private bool FilterApplied => checkBoxGroupByPrefix.Checked || !string.IsNullOrWhiteSpace(textBoxFilter.Text);

        private void RefreshActionAdapters()
        {
            string filter = textBoxFilter.Text.ToLowerInvariant();

            lock (m_actionAdapterTable)
            {
                m_actionAdapters = m_actionAdapterTable.QueryRecordsWhere("TypeName = 'DynamicCalculator.DynamicCalculator'").ToList();

                Dictionary<string, FilteredActionAdapter> filteredAdapters = new Dictionary<string, FilteredActionAdapter>(StringComparer.OrdinalIgnoreCase);

                foreach (CustomActionAdapter actionAdapter in m_actionAdapters)
                {
                    string name = actionAdapter.AdapterName;

                    if (checkBoxGroupByPrefix.Checked)
                    {
                        int dash = name.IndexOf('-');
                        name = dash < 0 ? name : name.Substring(0, dash);

                        if (name.EndsWith("CALC", StringComparison.OrdinalIgnoreCase) && name.Length > 4)
                            name = name.Substring(0, name.Length - 4);
                    }

                    if (!string.IsNullOrWhiteSpace(filter) && !name.ToLowerInvariant().Contains(filter))
                        continue;

                    FilteredActionAdapter filteredAdapter = filteredAdapters.GetOrAdd(name, _ => new FilteredActionAdapter { AdapterName = name });
                    filteredAdapter.Adapters.Add(actionAdapter);
                }

                m_filteredActionAdapters = filteredAdapters.Values.ToList();

                foreach (FilteredActionAdapter filteredActionAdapter in m_filteredActionAdapters)
                {
                    filteredActionAdapter.Enabled = filteredActionAdapter.Adapters.All(adapter => adapter.Enabled);

                    if (FilterApplied)
                        filteredActionAdapter.SyncEnabledAdapterStates();
                }
            }

            BeginInvoke(new Action(() =>
            {
                lock (m_actionAdapterTable)
                {
                    UseWaitCursor = true;

                    // Force full refresh of list
                    checkedListBoxDevices.DataSource = null;
                    checkedListBoxDevices.DataSource = FilterApplied ? (object)m_filteredActionAdapters : m_actionAdapters;
                    checkedListBoxDevices.DisplayMember = "AdapterName";
                    checkedListBoxDevices.ValueMember = "Enabled";
                    SyncCheckedListBox();

                    UseWaitCursor = false;
                }
            }));
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            if (!m_formLoaded || m_formClosing)
                return;

            RefreshActionAdapters();
            FormElementChanged(sender, e);
        }

        private void checkBoxGroupByPrefix_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_formLoaded || m_formClosing)
                return;

            RefreshActionAdapters();
            FormElementChanged(sender, e);
        }

        private void checkedListBoxDevices_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!m_formLoaded || m_formClosing)
                return;

            object item = checkedListBoxDevices.Items[e.Index];
            bool state = e.NewValue == CheckState.Checked;

            if (item is CustomActionAdapter actionAdapter)
                actionAdapter.Enabled = state;

            if (item is FilteredActionAdapter filteredActionAdapter)
            {
                filteredActionAdapter.Enabled = state;

                if (FilterApplied)
                    filteredActionAdapter.SyncEnabledAdapterStates();
            }

            UpdateSelectAllStates();
        }

        private void SelectAllCheckedChanged(object sender, EventArgs e)
        {
            if (m_updatingSelectAllStates || !m_formLoaded || m_formClosing)
                return;

            bool enabled = radioButtonSelectAll.Checked;

            lock (m_actionAdapterTable)
            {
                if (FilterApplied)
                {
                    foreach (FilteredActionAdapter actionAdapter in m_filteredActionAdapters)
                        actionAdapter.Enabled = enabled;
                }
                else
                {
                    foreach (CustomActionAdapter actionAdapter in m_actionAdapters)
                        actionAdapter.Enabled = enabled;
                }
            }

            SyncCheckedListBox();
        }

        private void buttonEnableSelected_Click(object sender, EventArgs e)
        {
            if (!m_formLoaded || m_formClosing)
                return;

            buttonEnableSelected.Enabled = false;
            buttonEnableSelected.Text = "Working...";
            UseWaitCursor = true;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    SyncDataSource();

                    lock (m_actionAdapterTable)
                    {
                        foreach (CustomActionAdapter actionAdapter in m_actionAdapters)
                            m_actionAdapterTable.UpdateRecord(actionAdapter);
                    }

                    ClearConsoleOutput();
                    m_consoleProcess.StandardInput.WriteLine("ReloadConfig");

                    const int SleepInterval = 500;
                    const int MaxSleeps = 20; // 10 seconds
                    int sleepCount = 0;

                    while (!m_consoleOutput.Contains("System configuration was successfully reloaded.") && sleepCount++ < MaxSleeps && !m_reconnecting)
                        Thread.Sleep(SleepInterval);

                    if (sleepCount > MaxSleeps)
                        throw new TimeoutException("Timeout while waiting on reload configuration response");
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() => MessageBox.Show(this, $"Error updating calculation states: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
                finally
                {
                    BeginInvoke(new Action(() =>
                    {
                        UseWaitCursor = false;
                        buttonEnableSelected.Text = buttonEnableSelected.Tag.ToString();

                        if (!m_reconnecting)
                            buttonEnableSelected.Enabled = true;
                    }));
                }
            });
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (!m_formLoaded || m_formClosing)
                return;

            SyncDataSource();

            string search = textBoxSearch.Text;
            bool enabled = checkBoxSelect.Checked;

            lock (m_actionAdapterTable)
            {
                if (FilterApplied)
                {
                    foreach (FilteredActionAdapter actionAdapter in m_filteredActionAdapters)
                    {
                        if (actionAdapter.AdapterName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                            actionAdapter.Enabled = enabled;

                        actionAdapter.SyncEnabledAdapterStates();
                    }
                }
                else
                {
                    foreach (CustomActionAdapter actionAdapter in m_actionAdapters)
                    {
                        if (actionAdapter.AdapterName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                            actionAdapter.Enabled = enabled;
                    }
                }
            }

            SyncCheckedListBox();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (!m_formLoaded || m_formClosing)
                return;

            RefreshActionAdapters();
        }

        private void buttonReconnect_Click(object sender, EventArgs e)
        {
            m_reconnecting = true;
            ConnectConsole();
        }

        private void UpdateSelectAllStates()
        {
            try
            {
                // Prevent unwanted check change event on radio buttons - this code is just making UI match current selections, i.e., all or none states
                m_updatingSelectAllStates = true;
                
                if (FilterApplied)
                {
                    radioButtonSelectAll.Checked = m_filteredActionAdapters.All(adapter => adapter.Enabled);

                    if (!radioButtonSelectAll.Checked)
                        radioButtonUnselectAll.Checked = m_filteredActionAdapters.All(adapter => !adapter.Enabled);
                }
                else
                {
                    radioButtonSelectAll.Checked = m_actionAdapters.All(adapter => adapter.Enabled);

                    if (!radioButtonSelectAll.Checked)
                        radioButtonUnselectAll.Checked = m_actionAdapters.All(adapter => !adapter.Enabled);
                }
            }
            finally
            {
                m_updatingSelectAllStates = false;
            }

            m_updateTotals?.RunOnceAsync();
        }

        private void SyncCheckedListBox()
        {
            for (int i = 0; i < checkedListBoxDevices.Items.Count; i++)
            {
                if (checkedListBoxDevices.Items[i] is CustomActionAdapter actionAdapter)
                    checkedListBoxDevices.SetItemChecked(i, actionAdapter.Enabled);

                if (checkedListBoxDevices.Items[i] is FilteredActionAdapter filteredActionAdapter)
                    checkedListBoxDevices.SetItemChecked(i, filteredActionAdapter.Enabled);
            }

            UpdateSelectAllStates();
        }

        private void SyncDataSource()
        {
            for (int i = 0; i < checkedListBoxDevices.Items.Count; i++)
            {
                if (checkedListBoxDevices.Items[i] is CustomActionAdapter actionAdapter)
                    actionAdapter.Enabled = checkedListBoxDevices.GetItemChecked(i);

                if (checkedListBoxDevices.Items[i] is FilteredActionAdapter filteredActionAdapter)
                {
                    filteredActionAdapter.Enabled = checkedListBoxDevices.GetItemChecked(i);

                    if (FilterApplied)
                        filteredActionAdapter.SyncEnabledAdapterStates();
                }
            }

            m_updateTotals?.RunOnceAsync();
        }

        private void UpdateTotals()
        {
            BeginInvoke(new Action(() =>
            {
                labelTotal.Text = string.Format(labelTotal.Tag.ToString(), m_actionAdapters.Count);
                labelSelected.Text = string.Format(labelSelected.Tag.ToString(), m_actionAdapters.Count(adapter => adapter.Enabled));
            }));
        }
    }
}
