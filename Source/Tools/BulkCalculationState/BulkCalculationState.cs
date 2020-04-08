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
//  02/25/2020 - rcarroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using BulkCalculationState.Model;
using GSF.Data;
using GSF.Data.Model;
using GSF.IO;
using GSF.Threading;

namespace BulkCalculationState
{
    public partial class BulkCalculationState : Form
    {
        private const string SourceApp = "openHistorian";

        private string m_sourcePath;
        private AdoDataConnection m_connection;
        private TableOperations<CustomActionAdapter> m_actionAdapterTable;
        private List<CustomActionAdapter> m_actionAdapters;
        private ShortSynchronizedOperation m_updateTotals;

        public BulkCalculationState() => InitializeComponent();

        private void BulkCalculationState_Load(object sender, EventArgs e)
        {
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
            m_actionAdapters = m_actionAdapterTable.QueryRecordsWhere("TypeName = 'DynamicCalculator.DynamicCalculator'").ToList();

            checkedListBoxDevices.DataSource = m_actionAdapters;
            checkedListBoxDevices.DisplayMember = "AdapterName";
            checkedListBoxDevices.ValueMember = "Enabled";

            m_updateTotals = new ShortSynchronizedOperation(UpdateTotals);

            SyncCheckedListBox();
        }

        private void checkedListBoxDevices_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            m_updateTotals?.RunOnceAsync();
        }

        private void checkBoxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = checkBoxSelectAll.Checked;

            foreach (CustomActionAdapter actionAdapter in m_actionAdapters)
                actionAdapter.Enabled = enabled;

            SyncCheckedListBox();
        }

        private void buttonEnableSelected_Click(object sender, EventArgs e)
        {
            SyncDataSource();

            foreach (CustomActionAdapter actionAdapter in m_actionAdapters)
                m_actionAdapterTable.UpdateRecord(actionAdapter);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false, 
                RedirectStandardInput = true, 
                FileName = $@"{m_sourcePath}\{SourceApp}Console.exe"
            };

            Process process = new Process { StartInfo = startInfo };

            process.Start();
            process.StandardInput.WriteLine("ReloadConfig");
            process.StandardInput.WriteLine("Exit");

            process.WaitForExit();
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            SyncDataSource();

            string search = textBoxSearch.Text;
            bool enabled = checkBoxSelect.Checked;

            foreach (CustomActionAdapter actionAdapter in m_actionAdapters)
            {
                if (actionAdapter.AdapterName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    actionAdapter.Enabled = enabled;
            }

            SyncCheckedListBox();
        }

        private void SyncCheckedListBox()
        {
            for (int i = 0; i < checkedListBoxDevices.Items.Count; i++)
            {
                if (checkedListBoxDevices.Items[i] is CustomActionAdapter actionAdapter)
                    checkedListBoxDevices.SetItemChecked(i, actionAdapter.Enabled);
            }

            m_updateTotals?.RunOnceAsync();
        }

        private void SyncDataSource()
        {
            for (int i = 0; i < checkedListBoxDevices.Items.Count; i++)
            {
                if (checkedListBoxDevices.Items[i] is CustomActionAdapter actionAdapter)
                    actionAdapter.Enabled = checkedListBoxDevices.GetItemChecked(i);
            }

            m_updateTotals?.RunOnceAsync();
        }

        private void UpdateTotals()
        {
            BeginInvoke(new Action(() =>
            {
                labelTotal.Text = string.Format(labelTotal.Tag.ToString(), checkedListBoxDevices.Items.Count);
                labelSelected.Text = string.Format(labelSelected.Tag.ToString(), checkedListBoxDevices.CheckedItems.Count);
            }));
        }
    }
}
