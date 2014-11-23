//******************************************************************************************************
//  MigrationUtility.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/21/2014 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GSF;
using GSF.Historian;
using GSF.Snap.Services;
using openHistorian.Snap;

namespace MigrationUtility
{
    public partial class MigrationUtility : Form
    {
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private bool m_formClosing;

        public MigrationUtility()
        {
            InitializeComponent();

            m_key = new HistorianKey();
            m_value = new HistorianValue();
        }

        private void MigrationUtility_Load(object sender, EventArgs e)
        {
            // Add formatted names for ArchiveDirectoryMethod enumeration to combo-box
            comboBoxDirectoryNamingMode.Items.AddRange(
                Enum.GetValues(typeof(ArchiveDirectoryMethod)).Cast<ArchiveDirectoryMethod>().
                Select(method => (object)method.GetFormattedName()).ToArray());

            comboBoxDirectoryNamingMode.SelectedIndex = (int)ArchiveDirectoryMethod.YearThenMonth;
        }

        private void MigrationUtility_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_formClosing = true;
            Environment.Exit(0);
        }

        private void buttonOpenSourceFilesLocation_Click(object sender, EventArgs e)
        {
            FolderBrowser.Description = "Find source location for openHistorian 1.0 / DatAWare archive files";

            if (FolderBrowser.ShowDialog(this) == DialogResult.OK)
                textBoxSourceFiles.Text = FolderBrowser.SelectedPath;
        }

        private void buttonOpenSourceOffloadedFilesLocation_Click(object sender, EventArgs e)
        {
            FolderBrowser.Description = "Find offload location for openHistorian 1.0 / DatAWare archive files";

            if (FolderBrowser.ShowDialog(this) == DialogResult.OK)
                textBoxSourceOffloadedFiles.Text = FolderBrowser.SelectedPath;
        }

        private void buttonOpenDestinationFilesLocation_Click(object sender, EventArgs e)
        {
            FolderBrowser.Description = "Find destination location for new openHistorian 2.0 archive files";

            if (FolderBrowser.ShowDialog(this) == DialogResult.OK)
                textBoxDestinationFiles.Text = FolderBrowser.SelectedPath;
        }

        private void textBoxSourceFiles_TextChanged(object sender, EventArgs e)
        {
            OpenGSFHistorianArchive(textBoxSourceFiles.Text, textBoxSourceOffloadedFiles.Text, true);
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            buttonGo.Enabled = false;

            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            parameters["sourceFilesLocation"] = textBoxSourceOffloadedFiles.Text;
            parameters["sourceFilesOffloadLocation"] = textBoxSourceOffloadedFiles.Text;
            parameters["instanceName"] = textBoxInstanceName.Text;
            parameters["destinationFilesLocation"] = textBoxDestinationFiles.Text;
            parameters["targetFileSize"] = textBoxTargetFileSize.Text;
            parameters["directoryNamingMethod"] = comboBoxDirectoryNamingMode.SelectedIndex.ToString();

            Thread migrateData = new Thread(MigrateData);
            migrateData.IsBackground = true;
            migrateData.Start(parameters);
        }

        private void MigrateData(object state)
        {
            try
            {
                Ticks startTime = DateTime.UtcNow.Ticks;
                Dictionary<string, string> parameters = state as Dictionary<string, string>;

                if ((object)parameters == null)
                    throw new ArgumentNullException("state", "Could not interpret thread state as parameters dictionary");

                ClearUpdateMessages();

                OpenGSFHistorianArchive(
                    parameters["sourceFilesLocation"],
                    parameters["sourceFilesOffloadLocation"]);

                OpenSnapDBEngine(
                    parameters["instanceName"],
                    parameters["destinationFilesLocation"],
                    parameters["targetFileSize"],
                    parameters["directoryNamingMethod"]);

                long migratedPoints = 0;

                foreach (IDataPoint point in ReadGSFHistorianData())
                {
                    WriteSnapDBData(point);
                    migratedPoints++;

                    if (migratedPoints % 500000 == 0)
                        ShowUpdateMessage("Migrated {0:#,##0} points so far...", migratedPoints);

                    if (m_formClosing)
                        break;
                }

                if (m_formClosing)
                {
                    ShowUpdateMessage("Migration canceled.");
                }
                else
                {
                    FlushSnapDB();
                    ShowUpdateMessage("Total migration time {0}", (DateTime.UtcNow.Ticks - startTime).ToElapsedTimeString(3));
                }
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Failure during migration: {0}", ex.Message);
            }
            finally
            {
                EnableGoButton(false);
                CloseGSFHistorianArchive();
                CloseSnapDBEngine();
            }
        }

        private void EnableGoButton(bool enabled)
        {
            if (m_formClosing)
                return;
                
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(EnableGoButton), enabled);
            }
            else
            {
                buttonGo.Enabled = enabled;
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
                textBoxMessageOutput.Text = "";
                Application.DoEvents();
            }
        }

        private void ShowUpdateMessage(string message, params object[] args)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, object[]>(ShowUpdateMessage), message, args);
            }
            else
            {
                StringBuilder outputText = new StringBuilder();

                outputText.AppendFormat(message, args);
                outputText.AppendLine();

                textBoxMessageOutput.AppendText(outputText.ToString());
                Application.DoEvents();
            }
        }
    }
}
