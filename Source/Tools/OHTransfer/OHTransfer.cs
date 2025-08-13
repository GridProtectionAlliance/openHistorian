//******************************************************************************************************
//  OHTransfer.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  08/12/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable AccessToModifiedClosure
// ReSharper disable AccessToDisposedClosure

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GSF;
using GSF.Configuration;
using GSF.IO;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Snap.Storage;
using GSF.Threading;
using GSF.Units;
using GSF.Units.EE;
using GSF.Windows.Forms;
using openHistorian.Net;
using openHistorian.Snap;
using openHistorian.Snap.Definitions;
using CancellationToken = System.Threading.CancellationToken;

namespace OHTransfer
{
    public partial class OHTransfer : Form
    {
        private CancellationTokenSource m_cancellationTokenSource;
        private bool m_formClosing;

        public OHTransfer()
        {
            InitializeComponent();

            DateTime nowFromTopOfHour = DateTime.UtcNow.BaselinedTimestamp(BaselineTimeInterval.Hour);
            dateTimePickerFrom.Value = nowFromTopOfHour.AddDays(-30);
            dateTimePickerTo.Value = nowFromTopOfHour;
        }

        private void OHTransfer_Load(object sender, EventArgs e)
        {
            CategorizedSettingsElementCollection settings = ConfigurationFile.Current.Settings["systemSettings"];

            settings.Add("sourceFiles", textBoxSourceFiles.Text, "openHistorian 2.0 source files location", false, SettingScope.User);
            settings.Add("destinationFiles", textBoxDestinationFiles.Text, "openHistorian 2.0 destination files location", false, SettingScope.User);
            settings.Add("sourceCSVMeasurements", textBoxSourceCSVMeasurements.Text, "openHistorian 2.0 source CSV measurements export file", false, SettingScope.User);
            settings.Add("destinationCSVMeasurements", textBoxDestinationCSVMeasurements.Text, "openHistorian 2.0 destination CSV measurements export file", false, SettingScope.User);
            settings.Add("fromDate", dateTimePickerFrom.Value.ToString(dateTimePickerFrom.CustomFormat), "Start date for transfer", false, SettingScope.User);
            settings.Add("toDate", dateTimePickerTo.Value.ToString(dateTimePickerTo.CustomFormat), "End date for transfer", false, SettingScope.User);

            textBoxSourceFiles.Text = settings["sourceFiles"].Value;
            textBoxDestinationFiles.Text = settings["destinationFiles"].Value;
            textBoxSourceCSVMeasurements.Text = settings["sourceCSVMeasurements"].Value;
            textBoxDestinationCSVMeasurements.Text = settings["destinationCSVMeasurements"].Value;
            dateTimePickerFrom.Value = DateTime.ParseExact(settings["fromDate"].Value, dateTimePickerFrom.CustomFormat, null, DateTimeStyles.AssumeUniversal);
            dateTimePickerTo.Value = DateTime.ParseExact(settings["toDate"].Value, dateTimePickerTo.CustomFormat, null, DateTimeStyles.AssumeUniversal);

            this.RestoreLocation();
        }

        private void OHTransfer_FormClosing(object sender, FormClosingEventArgs e)
        {
            CategorizedSettingsElementCollection settings = ConfigurationFile.Current.Settings["systemSettings"];

            m_formClosing = true;

            this.SaveLocation();

            settings["sourceFiles"].Value = textBoxSourceFiles.Text;
            settings["destinationFiles"].Value = textBoxDestinationFiles.Text;
            settings["sourceCSVMeasurements"].Value = textBoxSourceCSVMeasurements.Text;
            settings["destinationCSVMeasurements"].Value = textBoxDestinationCSVMeasurements.Text;
            settings["fromDate"].Value = dateTimePickerFrom.Value.ToString(dateTimePickerFrom.CustomFormat);
            settings["toDate"].Value = dateTimePickerTo.Value.ToString(dateTimePickerTo.CustomFormat);

            ConfigurationFile.Current.Save();

            m_cancellationTokenSource?.Cancel();
            Thread.Sleep(500);
        }

        private void buttonOpenSourceFilesLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = "Find source location for existing openHistorian 2.0 archive files";

            if (Directory.Exists(textBoxSourceFiles.Text))
                folderBrowserDialog.SelectedPath = textBoxSourceFiles.Text;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                textBoxSourceFiles.Text = folderBrowserDialog.SelectedPath;

            if (!textBoxSourceFiles.Text.EndsWith(Path.DirectorySeparatorChar.ToString()))
                textBoxSourceFiles.Text += Path.DirectorySeparatorChar;
        }

        private void buttonOpenDestinationFilesLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = "Find destination location for new openHistorian 2.0 archive files";

            if (Directory.Exists(textBoxDestinationFiles.Text))
                folderBrowserDialog.SelectedPath = textBoxDestinationFiles.Text;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                textBoxDestinationFiles.Text = folderBrowserDialog.SelectedPath;

            if (!textBoxDestinationFiles.Text.EndsWith(Path.DirectorySeparatorChar.ToString()))
                textBoxDestinationFiles.Text += Path.DirectorySeparatorChar;

            if (!Directory.Exists(textBoxDestinationFiles.Text))
                Directory.CreateDirectory(textBoxDestinationFiles.Text);
        }

        private void buttonOpenSourceCSVMeasurementsFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Find source openHistorian 2.0 CSV measurements export file";
            openFileDialog.FileName = File.Exists(textBoxSourceCSVMeasurements.Text) ? textBoxSourceCSVMeasurements.Text : string.Empty;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                textBoxSourceCSVMeasurements.Text = openFileDialog.FileName;
        }

        private void buttonOpenDestinationCSVMeasurementsFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Find destination openHistorian 2.0 CSV measurements export file";
            openFileDialog.FileName = File.Exists(textBoxDestinationCSVMeasurements.Text) ? textBoxDestinationCSVMeasurements.Text : string.Empty;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                textBoxDestinationCSVMeasurements.Text = openFileDialog.FileName;
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            ClearUpdateMessages();
            UpdateProgressBar(0);

            if (string.IsNullOrWhiteSpace(textBoxSourceFiles.Text))
            {
                MessageBox.Show("Please select a source location for existing openHistorian 2.0 archive files.", "Missing Source Location", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxDestinationFiles.Text))
            {
                MessageBox.Show("Please select a destination location for new openHistorian 2.0 archive files.", "Missing Destination Location", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxSourceCSVMeasurements.Text))
            {
                MessageBox.Show("Please select a source openHistorian 2.0 CSV measurements export file.", "Missing Source File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxDestinationCSVMeasurements.Text))
            {
                MessageBox.Show("Please select a destination openHistorian 2.0 CSV measurements export file.", "Missing Destination File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(textBoxDestinationFiles.Text))
            {
                MessageBox.Show("The specified destination location does not exist.", "Invalid Destination Location", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(textBoxSourceCSVMeasurements.Text))
            {
                MessageBox.Show("The specified source openHistorian 2.0 CSV measurements export file does not exist.", "Invalid Source File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(textBoxDestinationCSVMeasurements.Text))
            {
                MessageBox.Show("The specified destination openHistorian 2.0 CSV measurements export file does not exist.", "Invalid Destination File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime startTime = dateTimePickerFrom.Value;
            DateTime endTime = dateTimePickerTo.Value;

            if (endTime <= startTime)
            {
                MessageBox.Show("The end time must be greater than the start time.", "Invalid Time Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long processStartTime = DateTime.UtcNow.Ticks;
            string[] csvLines;

            // Read CSV measurements from source file
            ShowUpdateMessage("Reading source CSV measurements file: {0}...", textBoxSourceCSVMeasurements.Text);

            try
            {
                csvLines = File.ReadAllLines(textBoxSourceCSVMeasurements.Text);
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Error reading source CSV measurements file: {0}", ex.Message);
                return;
            }

            // Parse ID and SignalReference columns from CSV lines into a dictionary using header row
            (Dictionary<ulong, SignalReference> sourceCSVData, string deviceName) = ParseCSVMeasurements(csvLines);

            if (sourceCSVData.Count == 0)
            {
                ShowUpdateMessage("No valid measurements found in source CSV file -- operation cancelled.");
                return;
            }

            // Read CSV measurements from destination file
            ShowUpdateMessage("Reading destination CSV measurements file: {0}...", textBoxDestinationCSVMeasurements.Text);

            try
            {
                csvLines = File.ReadAllLines(textBoxDestinationCSVMeasurements.Text);
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Error reading destination CSV measurements file: {0}", ex.Message);
                return;
            }

            // Parse ID and SignalReference columns from CSV lines ulong a dictionary using header row
            (Dictionary<ulong, SignalReference> destinationCSVData, _) = ParseCSVMeasurements(csvLines);

            if (destinationCSVData.Count == 0)
            {
                ShowUpdateMessage("No valid measurements found in destination CSV file -- operation cancelled.");
                return;
            }

            // Find measurements by SignalReference.Kind + SignalReference.Index in source CSV that also exist in destination CSV
            // where the result is source CSV historian ID to destination CSV historian ID mapping
            Dictionary<ulong, ulong> historianIDMapping = new();

            // Pre-index destination signals for O(1) lookup by match key
            Dictionary<(SignalKind Kind, int Index), ulong> signalReferenceIDMap = [];
            Dictionary<(SignalKind Kind, int Index, string Acronym), ulong> calculatedSignalReferenceIDMap = [];

            foreach (KeyValuePair<ulong, SignalReference> destinationEntry in destinationCSVData)
            {
                SignalReference destinationSignal = destinationEntry.Value;

                if (destinationSignal.Kind == SignalKind.Calculation)
                    calculatedSignalReferenceIDMap[(destinationSignal.Kind, destinationSignal.Index, destinationSignal.Acronym ?? string.Empty)] = destinationEntry.Key;
                else
                    signalReferenceIDMap[(destinationSignal.Kind, destinationSignal.Index)] = destinationEntry.Key;
            }

            foreach (KeyValuePair<ulong, SignalReference> sourceEntry in sourceCSVData)
            {
                SignalReference sourceSignal = sourceEntry.Value;

                if (sourceSignal.Kind == SignalKind.Calculation)
                {
                    if (calculatedSignalReferenceIDMap.TryGetValue((sourceSignal.Kind, sourceSignal.Index, sourceSignal.Acronym ?? string.Empty), out ulong destID))
                        historianIDMapping[sourceEntry.Key] = destID;
                }
                else
                {
                    if (signalReferenceIDMap.TryGetValue((sourceSignal.Kind, sourceSignal.Index), out ulong destID))
                        historianIDMapping[sourceEntry.Key] = destID;
                }
            }

            if (historianIDMapping.Count == 0)
            {
                ShowUpdateMessage("No matching measurements found between source and destination CSV files -- operation cancelled.");
                return;
            }

            ShowUpdateMessage("Found {0:N0} matching measurements between source and destination CSV files.", historianIDMapping.Count);

            string sourcePath = textBoxSourceFiles.Text.Trim();
            string destinationPath = textBoxDestinationFiles.Text.Trim();
            bool processCompleted = false;

            m_cancellationTokenSource = new CancellationTokenSource();
            ICancellationToken monitorCancellationToken = null;

            new Thread(() =>
            {
                try
                {
                    EnableGoButton(false);

                    CancellationToken cancellationToken = m_cancellationTokenSource.Token;
                    
                    int totalDays = (int)(endTime - startTime).TotalDays;
                    SetProgressMaximum(totalDays);

                    const string InstanceName = "PPA";

                    // Establish archive information for this historian instance
                    HistorianServerDatabaseConfig archiveInfo = new(InstanceName, sourcePath, false)
                    {
                        TargetFileSize = (long)(1.5D * SI.Giga),
                        DirectoryMethod = ArchiveDirectoryMethod.YearThenMonth
                    };

                    HistorianServer server = new(archiveInfo);
                    SnapClient client = SnapClient.Connect(server.Host);
                    ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(InstanceName);
                    Dictionary<Guid, ArchiveDetails> attachedFiles = database.GetAllAttachedFiles().ToDictionary(file => file.Id);

                    using ArchiveList<HistorianKey, HistorianValue> archiveList = new();
                    archiveList.LoadFiles(attachedFiles.Values.OrderBy(file => file.EndTime).Select(file => file.FileName));

                    SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime);
                    MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(historianIDMapping.Keys);

                    using WorkerThreadSynchronization workerThreadSynchronization = new();
                    using SequentialStreamReader reader = new(archiveList, SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter, workerThreadSynchronization);

                    long lastStatusMessage = DateTime.UtcNow.Ticks;

                    void readingMonitor()
                    {
                        if (cancellationToken.IsCancellationRequested || processCompleted)
                        {
                            reader.CancelReader();
                            return;
                        }

                        workerThreadSynchronization.RequestCallback(() =>
                        {
                            if (DateTime.UtcNow.Ticks - lastStatusMessage < Ticks.PerSecond * 4)
                                return;

                            lastStatusMessage = DateTime.UtcNow.Ticks;

                            if (Stats.PointsReturned == 0L)
                            {
                                ShowUpdateMessage("\r\nScanned {0:N0} points so far, scanning at {1:N0} points per second...", Stats.PointsScanned, (Stats.PointsReturned + Stats.PointsScanned) / ((DateTime.UtcNow.Ticks - processStartTime) / Ticks.PerSecond));
                            }
                            else
                            {
                                ShowUpdateMessage("\r\nProcessed {0:N0} points so far, scanning at {1:N0} points per second:", Stats.PointsReturned, (Stats.PointsReturned + Stats.PointsScanned) / ((DateTime.UtcNow.Ticks - processStartTime) / Ticks.PerSecond));

                                double processedDays = (reader.LastKey.TimestampAsDate - startTime).TotalDays;

                                UpdateProgressBar((int)processedDays);
                                ShowUpdateMessage("        {0:0.00%} complete...", processedDays / totalDays);
                            }
                        });

                        Application.DoEvents();
                        monitorCancellationToken = new Action(readingMonitor).DelayAndExecute(2000);
                    }

                    monitorCancellationToken = new Action(readingMonitor).DelayAndExecute(2000);

                    string completeFileName = Path.Combine(destinationPath, $"{deviceName}-Exported.d2");
                    string pendingFileName = Path.Combine(destinationPath, $"{FilePath.GetFileNameWithoutExtension(completeFileName)}.~d2i");

                    if (File.Exists(pendingFileName))
                        File.Delete(pendingFileName);

                    if (File.Exists(completeFileName))
                        File.Delete(completeFileName);

                    SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.Create(pendingFileName, completeFileName, 4096, null, HistorianFileEncodingDefinition.TypeGuid, reader, FileFlags.GetStage(3));

                    Ticks processDuration = DateTime.UtcNow.Ticks - processStartTime;
                    processCompleted = true;

                    ShowUpdateMessage("Data transfer completed in {0}", processDuration.ToElapsedTimeString(2));
                    UpdateProgressBar(totalDays);
                }
                catch (Exception ex)
                {
                    ShowUpdateMessage("Error during transfer: {0}", ex.Message);
                }
                finally
                {
                    monitorCancellationToken?.Cancel();
                    EnableGoButton(true);
                }
            })
            {
                IsBackground = true
            }
            .Start();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            m_cancellationTokenSource.Cancel();
            ShowUpdateMessage("Transfer operation cancelled by user.");
        }

        private (Dictionary<ulong, SignalReference> csvData, string deviceName) ParseCSVMeasurements(string[] csvLines)
        {
            Dictionary<ulong, SignalReference> csvData = new();

            if (csvLines is null || csvLines.Length <= 1)
                return (csvData, "");

            // Parse header to find column indices using robust CSV parser
            string[] headers = ParseCSVLine(csvLines[0]);

            int deviceIndex = Array.FindIndex(headers, header => header.Trim().Equals("Device", StringComparison.OrdinalIgnoreCase));

            if (deviceIndex < 0)
            {
                ShowUpdateMessage("CSV header must contain 'Device' column.");
                return (csvData, "");
            }

            // Validate that device names in all rows are the same
            string firstDeviceName = null;

            for (int i = 1; i < csvLines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(csvLines[i]))
                    continue;

                string[] columns = ParseCSVLine(csvLines[i]);

                if (columns.Length <= deviceIndex)
                    continue;

                string deviceName = columns[deviceIndex].Trim();
                    
                if (string.IsNullOrEmpty(firstDeviceName))
                {
                    firstDeviceName = deviceName;
                }
                else if (!string.Equals(firstDeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
                {
                    ShowUpdateMessage("All rows in CSV file must have the same 'Device' name. Export is designed to transfer one device at a time.");
                    return (csvData, "");
                }
            }

            int idIndex = Array.FindIndex(headers, header => header.Trim().Equals("ID", StringComparison.OrdinalIgnoreCase));
            int signalRefIndex = Array.FindIndex(headers, header => header.Trim().Equals("SignalReference", StringComparison.OrdinalIgnoreCase));

            if (idIndex < 0 || signalRefIndex < 0)
            {
                ShowUpdateMessage("CSV header must contain 'ID' and 'SignalReference' columns.");
                return (csvData, "");
            }

            int maxColumnIndex = Math.Max(Math.Max(deviceIndex, idIndex), signalRefIndex);
            int calcIndex = 1;

            // Parse data rows using robust CSV parser
            for (int i = 1; i < csvLines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(csvLines[i]))
                    continue;

                string[] columns = ParseCSVLine(csvLines[i]);

                if (columns.Length <= maxColumnIndex)
                    continue;

                string idValue = columns[idIndex].Trim();

                if (string.IsNullOrEmpty(idValue))
                    continue;

                string[] parts = idValue.Split(':');

                if (parts.Length != 2 || !ulong.TryParse(parts[1], out ulong id))
                {
                    ShowUpdateMessage("Invalid ID format in row {0}: {1}. Expected format is 'Source:ID'.", i + 1, idValue);
                    continue;
                }

                string signalReferenceValue = columns[signalRefIndex].Trim();

                if (string.IsNullOrEmpty(signalReferenceValue))
                    continue;

                SignalReference signalReference = new(signalReferenceValue);

                if (signalReference.Kind == SignalKind.Unknown)
                {
                    parts = signalReferenceValue.Split('-');

                    if (parts.Length > 1)
                    {
                        // Track matching acronyms for Calculation signals
                        signalReference.Acronym = parts[parts.Length - 1].Trim();
                        signalReference.Kind = SignalKind.Calculation;
                        signalReference.Index = calcIndex++;
                    }
                    else
                    {
                        continue; // Skip invalid SignalReference
                    }
                }

                csvData[id] = signalReference;
            }

            return (csvData, firstDeviceName ?? "");
        }

        private static string[] ParseCSVLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return [];

            List<string> fields = [];
            StringBuilder current = new();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // Check for escaped quote ("")
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            current.Append('"');
                            i++; // Skip the next quote
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else
                {
                    switch (c)
                    {
                        case ',':
                            fields.Add(current.ToString());
                            current.Clear();
                            break;
                        case '"':
                            inQuotes = true;
                            break;
                        default:
                            current.Append(c);
                            break;
                    }
                }
            }

            fields.Add(current.ToString());

            return fields.ToArray();
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
                buttonGo.Visible = enabled;
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

        private void SetProgressMaximum(int maximum)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(SetProgressMaximum), maximum);
            }
            else
            {
                progressBar.Maximum = maximum;
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
                StringBuilder outputText = new();

                outputText.AppendFormat(message, args);
                outputText.AppendLine();

                lock (textBoxMessageOutput)
                    textBoxMessageOutput.AppendText(outputText.ToString());
            }
        }
    }
}
