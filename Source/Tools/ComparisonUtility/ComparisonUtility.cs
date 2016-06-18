//******************************************************************************************************
//  ComparisonUtility.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  06/15/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.IO;
using GSF.Windows.Forms;

namespace ComparisonUtility
{
    public partial class ComparisonUtility : Form
    {
        private bool m_formClosing;

        public ComparisonUtility()
        {
            InitializeComponent();
        }

        private void ComparisonUtility_Load(object sender, EventArgs e)
        {
            CategorizedSettingsElementCollection settings = ConfigurationFile.Current.Settings["systemSettings"];

            settings.Add("sourceHostAddress", textBoxSourceHistorianHostAddress.Text, "Host address of source historian", false, SettingScope.User);
            settings.Add("sourceDataPort", maskedTextBoxSourceHistorianDataPort.Text, "Data port of source historian", false, SettingScope.User);
            settings.Add("sourceMetaDataPort", maskedTextBoxSourceHistorianMetaDataPort.Text, "Meta-data port of source historian", false, SettingScope.User);
            settings.Add("sourceInstanceName", textBoxSourceHistorianInstanceName.Text, "Instance name of source historian", false, SettingScope.User);
            settings.Add("destinationHostAddress", textBoxDestinationHistorianHostAddress.Text, "Host address of destination historian", false, SettingScope.User);
            settings.Add("destinationDataPort", maskedTextBoxDestinationHistorianDataPort.Text, "Data port of destination historian", false, SettingScope.User);
            settings.Add("destinationMetaDataPort", maskedTextBoxDestinationHistorianMetaDataPort.Text, "Meta-data port of destination historian", false, SettingScope.User);
            settings.Add("destinationInstanceName", textBoxDestinationHistorianInstanceName.Text, "Instance name of destination historian", false, SettingScope.User);
            settings.Add("frameRate", maskedTextBoxFrameRate.Text, "Frame rate, in frames per second, used to estimate total data for timespan", false, SettingScope.User);
            settings.Add("metaDataTimeout", maskedTextBoxMetaDataTimeout.Text, "Meta-data retriever timeout", false, SettingScope.User);
            settings.Add("startTime", dateTimePickerSourceTime.Text, "Start of time range", false, SettingScope.User);
            settings.Add("endTime", dateTimePickerEndTime.Text, "End of time range", false, SettingScope.User);
            settings.Add("messageInterval", maskedTextBoxMessageInterval.Text, "Message display interval", false, SettingScope.User);

            textBoxSourceHistorianHostAddress.Text = settings["sourceHostAddress"].Value;
            maskedTextBoxSourceHistorianDataPort.Text = settings["sourceDataPort"].Value;
            maskedTextBoxSourceHistorianMetaDataPort.Text = settings["sourceMetaDataPort"].Value;
            textBoxSourceHistorianInstanceName.Text = settings["sourceInstanceName"].Value;
            textBoxDestinationHistorianHostAddress.Text = settings["destinationHostAddress"].Value;
            maskedTextBoxDestinationHistorianDataPort.Text = settings["destinationDataPort"].Value;
            maskedTextBoxDestinationHistorianMetaDataPort.Text = settings["destinationMetaDataPort"].Value;
            textBoxDestinationHistorianInstanceName.Text = settings["destinationInstanceName"].Value;
            maskedTextBoxFrameRate.Text = settings["frameRate"].Value;
            maskedTextBoxMetaDataTimeout.Text = settings["metaDataTimeout"].Value;
            dateTimePickerSourceTime.Text = settings["startTime"].Value;
            dateTimePickerEndTime.Text = settings["endTime"].Value;
            maskedTextBoxMessageInterval.Text = settings["messageInterval"].Value;

            this.RestoreLocation();
        }

        private void ComparisonUtility_FormClosing(object sender, FormClosingEventArgs e)
        {
            CategorizedSettingsElementCollection settings = ConfigurationFile.Current.Settings["systemSettings"];

            m_formClosing = true;

            this.SaveLocation();

            settings["sourceHostAddress"].Value = textBoxSourceHistorianHostAddress.Text;
            settings["sourceDataPort"].Value = maskedTextBoxSourceHistorianDataPort.Text;
            settings["sourceMetaDataPort"].Value = maskedTextBoxSourceHistorianMetaDataPort.Text;
            settings["sourceInstanceName"].Value = textBoxSourceHistorianInstanceName.Text;
            settings["destinationHostAddress"].Value = textBoxDestinationHistorianHostAddress.Text;
            settings["destinationDataPort"].Value = maskedTextBoxDestinationHistorianDataPort.Text;
            settings["destinationMetaDataPort"].Value = maskedTextBoxDestinationHistorianMetaDataPort.Text;
            settings["destinationInstanceName"].Value = textBoxDestinationHistorianInstanceName.Text;
            settings["frameRate"].Value = maskedTextBoxFrameRate.Text;
            settings["metaDataTimeout"].Value = maskedTextBoxMetaDataTimeout.Text;
            settings["startTime"].Value = dateTimePickerSourceTime.Text;
            settings["endTime"].Value = dateTimePickerEndTime.Text;
            settings["messageInterval"].Value = maskedTextBoxMessageInterval.Text;

            ConfigurationFile.Current.Save();
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            buttonGo.Enabled = false;
            ClearUpdateMessages();
            UpdateProgressBar(0);
            SetProgressMaximum(100);

            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            parameters["sourceHostAddress"] = textBoxSourceHistorianHostAddress.Text;
            parameters["sourceDataPort"] = maskedTextBoxSourceHistorianDataPort.Text;
            parameters["sourceMetaDataPort"] = maskedTextBoxSourceHistorianMetaDataPort.Text;
            parameters["sourceInstanceName"] = textBoxSourceHistorianInstanceName.Text;
            parameters["destinationHostAddress"] = textBoxDestinationHistorianHostAddress.Text;
            parameters["destinationDataPort"] = maskedTextBoxDestinationHistorianDataPort.Text;
            parameters["destinationMetaDataPort"] = maskedTextBoxDestinationHistorianMetaDataPort.Text;
            parameters["destinationInstanceName"] = textBoxDestinationHistorianInstanceName.Text;
            parameters["frameRate"] = maskedTextBoxFrameRate.Text;
            parameters["metaDataTimeout"] = maskedTextBoxMetaDataTimeout.Text;
            parameters["startTime"] = dateTimePickerSourceTime.Text;
            parameters["endTime"] = dateTimePickerEndTime.Text;
            parameters["messageInterval"] = maskedTextBoxMessageInterval.Text;

            Thread operation = new Thread(CompareArchives);
            operation.IsBackground = true;
            operation.Start(parameters);
        }

        private void CompareArchives(object state)
        {
            try
            {
                Ticks operationStartTime = DateTime.UtcNow.Ticks;
                Dictionary<string, string> parameters = state as Dictionary<string, string>;

                if ((object)parameters == null)
                    throw new ArgumentNullException(nameof(state), "Could not interpret thread state as parameters dictionary");

                string sourceHostAddress = parameters["sourceHostAddress"];
                int sourceDataPort = int.Parse(parameters["sourceDataPort"]);
                int sourceMetadataPort = int.Parse(parameters["sourceMetaDataPort"]);
                string sourceInstanceName = parameters["sourceInstanceName"];
                string destinationHostAddress = parameters["destinationHostAddress"];
                int destinationDataPort = int.Parse(parameters["destinationDataPort"]);
                int destinationMetaDataPort = int.Parse(parameters["destinationMetaDataPort"]);
                string destinationInstanceName = parameters["destinationInstanceName"];
                int frameRate = int.Parse(parameters["frameRate"]);
                int metaDataTimeout = int.Parse(parameters["metaDataTimeout"]) * 1000;
                ulong startTime = (ulong)DateTime.Parse(parameters["startTime"]).Ticks;
                ulong endTime = (ulong)DateTime.Parse(parameters["endTime"]).Ticks;
                int messageInterval = int.Parse(parameters["messageInterval"]);

                ShowUpdateMessage("Loading source connection metadata...");
                List<Metadata> sourceMetadata = Metadata.Query(sourceHostAddress, sourceMetadataPort, metaDataTimeout);

                ShowUpdateMessage("Loading destination connection metadata...");
                List<Metadata> destinationMetadata = Metadata.Query(destinationHostAddress, destinationMetaDataPort, metaDataTimeout);

                Ticks totalTime = DateTime.UtcNow.Ticks - operationStartTime;
                ShowUpdateMessage("*** Metadata Load Complete ***");
                ShowUpdateMessage($"Total metadata load time {totalTime.ToElapsedTimeString(3)}...");

                operationStartTime = DateTime.UtcNow.Ticks;

                Dictionary<ulong, ulong> sourcePointMappings = new Dictionary<ulong, ulong>();
                Dictionary<ulong, ulong> destinationPointMappings = new Dictionary<ulong, ulong>();

                Func<string, string> rootTagName = tagName =>
                {
                    int lastBangIndex = tagName.LastIndexOf('!');
                    return lastBangIndex > -1 ? tagName.Substring(lastBangIndex + 1).Trim() : tagName.Trim();
                };

                using (StreamWriter writer = new StreamWriter(FilePath.GetAbsolutePath("Metadata.txt")))
                {
                    writer.WriteLine($"Meta-data dump for archive comparison spanning {new DateTime((long)startTime):yyyy-MM-dd HH:mm:ss} to {new DateTime((long)endTime):yyyy-MM-dd HH:mm:ss}:");
                    writer.WriteLine();
                    writer.WriteLine($"     Source Meta-data: {sourceMetadata.Count:N0} records");
                    writer.WriteLine($"Destination Meta-data: {destinationMetadata.Count:N0} records");

                    string lastDeviceName = "";

                    // Create point ID cross reference dictionaries
                    foreach (Metadata sourceRecord in sourceMetadata.OrderBy(record => record.DeviceName))
                    {
                        ulong sourcePointID = sourceRecord.PointID;
                        Metadata destinationRecord = destinationMetadata.FirstOrDefault(record => rootTagName(sourceRecord.PointTag).Equals(rootTagName(record.PointTag), StringComparison.OrdinalIgnoreCase));
                        ulong destinationPointID = destinationRecord?.PointID ?? 0;
                        sourcePointMappings[destinationPointID] = sourcePointID;
                        destinationPointMappings[sourcePointID] = destinationPointID;

                        if (!sourceRecord.DeviceName.Equals(lastDeviceName, StringComparison.OrdinalIgnoreCase))
                        {
                            lastDeviceName = sourceRecord.DeviceName;
                            writer.WriteLine();
                            writer.WriteLine($"Measurements for device \"{lastDeviceName}\":");
                            writer.WriteLine();
                        }

                        writer.WriteLine($"Source \"{sourceRecord.PointTag}\" [{sourcePointID}] = Destination \"{destinationRecord?.PointTag}\" [{destinationPointID}]");
                    }
                }

                double timespan = new Ticks((long)(endTime - startTime)).ToSeconds();
                long comparedPoints = 0;
                long validPoints = 0;
                long invalidPoints = 0;
                long missingPoints = 0;
                long duplicatePoints = 0;
                long displayMessageCount = messageInterval;

                const int SourcePoint = 0;
                const int DestinationPoint = 1;

                Dictionary<ulong, DataPoint[]> dataBlock = new Dictionary<ulong, DataPoint[]>();
                DataPoint sourcePoint = new DataPoint();
                DataPoint destinationPoint = new DataPoint();
                Ticks readStartTime = DateTime.UtcNow.Ticks;

                using (SnapDBClient sourceClient = new SnapDBClient(sourceHostAddress, sourceDataPort, sourceInstanceName, startTime, endTime, frameRate, sourcePointMappings.Values))
                using (SnapDBClient destinationClient = new SnapDBClient(destinationHostAddress, destinationDataPort, destinationInstanceName, startTime, endTime, frameRate, destinationPointMappings.Values))
                {
                    // Scan to first record in source
                    if (!sourceClient.ReadNext(sourcePoint))
                        throw new InvalidOperationException("No data for specified time range in source connection!");

                    // Scan to first record in destination
                    if (!destinationClient.ReadNext(destinationPoint))
                        throw new InvalidOperationException("No data for specified time range in destination connection!");

                    while (true)
                    {
                        // Compare timestamps of current records
                        int timeComparison = DataPoint.CompareTimestamps(sourcePoint.Timestamp, destinationPoint.Timestamp, frameRate);
                        bool readSuccess = true;

                        // If timestamps do not match, synchronize starting times of source and destination datasets
                        while (readSuccess && timeComparison != 0)
                        {
                            if (timeComparison < 0)
                            {
                                // Destination has no data where source begins, scan source forward to match destination start time
                                do
                                {
                                    missingPoints++;

                                    if (!sourceClient.ReadNext(sourcePoint))
                                    {
                                        readSuccess = false;
                                        break;
                                    }
                                        
                                    timeComparison = DataPoint.CompareTimestamps(sourcePoint.Timestamp, destinationPoint.Timestamp, frameRate);
                                }
                                while (timeComparison < 0);
                            }
                            else // timeComparison > 0
                            {
                                // Source has no data where destination begins, scan destination forward to match source start time
                                do
                                {
                                    missingPoints++;

                                    if (!destinationClient.ReadNext(destinationPoint))
                                    {
                                        readSuccess = false;
                                        break;
                                    }

                                    timeComparison = DataPoint.CompareTimestamps(sourcePoint.Timestamp, destinationPoint.Timestamp, frameRate);
                                }
                                while (timeComparison > 0);
                            }
                        }

                        // Finished with data read
                        if (!readSuccess)
                        {
                            ShowUpdateMessage("*** End of data read encountered ***");
                            break;
                        }

                        // Read all time adjusted points for the current timestamp into a single block
                        ulong currentTimestamp = DataPoint.RoundTimestamp(sourcePoint.Timestamp, frameRate);
                        dataBlock.Clear();

                        // Load source data for current timestamp
                        do
                        {
                            if (!sourceClient.ReadNext(sourcePoint))
                            {
                                readSuccess = false;
                                break;
                            }

                            timeComparison = DataPoint.CompareTimestamps(sourcePoint.Timestamp, currentTimestamp, frameRate);

                            if (timeComparison == 0)
                            {
                                DataPoint[] points = dataBlock.GetOrAdd(sourcePoint.PointID, id => new DataPoint[2]);

                                if ((object)points[SourcePoint] != null)
                                    duplicatePoints++;

                                points[SourcePoint] = sourcePoint.Clone();
                            }
                        }
                        while (timeComparison == 0);

                        // Finished with data read
                        if (!readSuccess)
                        {
                            ShowUpdateMessage("*** End of data read encountered ***");
                            break;
                        }

                        // Load destination data for current timestamp
                        do
                        {
                            if (!destinationClient.ReadNext(destinationPoint))
                            {
                                readSuccess = false;
                                break;
                            }

                            timeComparison = DataPoint.CompareTimestamps(destinationPoint.Timestamp, currentTimestamp, frameRate);

                            if (timeComparison == 0)
                            {
                                DataPoint[] points = dataBlock.GetOrAdd(sourcePointMappings[destinationPoint.PointID], id => new DataPoint[2]);

                                if ((object)points[DestinationPoint] != null)
                                    duplicatePoints++;

                                points[DestinationPoint] = destinationPoint.Clone();
                            }
                        }
                        while (timeComparison == 0);

                        // Finished with data read - destination is short of source read
                        if (!readSuccess)
                        {
                            ShowUpdateMessage("*** End of data read encountered: destination read was short of data available in source ***");
                            break;
                        }

                        // Analyze data block
                        foreach (DataPoint[] points in dataBlock.Values)
                        {
                            if ((object)points[SourcePoint] == null && (object)points[DestinationPoint] == null)
                                continue;

                            if ((object)points[SourcePoint] == null || (object)points[DestinationPoint] == null)
                            {
                                missingPoints++;
                            }
                            else
                            {
                                if (points[SourcePoint].Value == points[DestinationPoint].Value)
                                {
                                    if (points[SourcePoint].Flags == points[DestinationPoint].Flags)
                                        validPoints++;
                                    else
                                        invalidPoints++;
                                }
                                else
                                {
                                    invalidPoints++;
                                }
                            }

                            if (comparedPoints++ == displayMessageCount)
                            {
                                if (comparedPoints % (5 * messageInterval) == 0)
                                    ShowUpdateMessage($"{Environment.NewLine}*** Compared {comparedPoints:#,##0} points so far averaging {comparedPoints / (DateTime.UtcNow.Ticks - readStartTime).ToSeconds():#,##0} points per second ***{Environment.NewLine}");
                                else
                                    ShowUpdateMessage($"{Environment.NewLine}Found {validPoints:#,##0} valid, {invalidPoints:#,##0} invalid and {missingPoints:#,##0} missing points during compare so far...{Environment.NewLine}");

                                displayMessageCount += messageInterval;

                                UpdateProgressBar((int)((1.0D - new Ticks((long)(endTime - sourcePoint.Timestamp)).ToSeconds() / timespan) * 100.0D));
                            }
                        }
                    }

                    if (m_formClosing)
                    {
                        ShowUpdateMessage("Comparison canceled.");
                        UpdateProgressBar(0);
                    }
                    else
                    {
                        totalTime = DateTime.UtcNow.Ticks - operationStartTime;
                        ShowUpdateMessage("*** Compare Complete ***");
                        ShowUpdateMessage($"Total compare time {totalTime.ToElapsedTimeString(3)} at {comparedPoints / totalTime.ToSeconds():#,##0} points per second.");
                        UpdateProgressBar(100);

                        ShowUpdateMessage(
                            $"{Environment.NewLine}" +
                            $"     Meta-data points: {sourceMetadata.Count}{Environment.NewLine}" +
                            $"    Time-span covered: {timespan:#,##0} seconds: {Ticks.FromSeconds(timespan).ToElapsedTimeString(2)}{Environment.NewLine}" +
                            $"      Points compared: {comparedPoints:#,##0}{Environment.NewLine}" +
                            $"         Valid points: {validPoints:#,##0}{Environment.NewLine}" +
                            $"       Invalid points: {invalidPoints:#,##0}{Environment.NewLine}" +
                            $"       Missing points: {missingPoints:#,##0}{Environment.NewLine}" +
                            $"     Duplicate points: {duplicatePoints:#,##0}{Environment.NewLine}" +
                            $"   Source point count: {comparedPoints + missingPoints:#,##0}{Environment.NewLine}" +
                            $"{Environment.NewLine}Data comparison {Math.Truncate(validPoints / (double)(comparedPoints + missingPoints) * 100000.0D) / 1000.0D:##0.000}% accurate");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowUpdateMessage($"Failure during historian comparison: {ex.Message}");
            }
            finally
            {
                EnableGoButton(true);
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

        internal void ShowUpdateMessage(string message)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowUpdateMessage), message);
            }
            else
            {
                StringBuilder outputText = new StringBuilder();

                outputText.AppendLine(message);
                outputText.AppendLine();

                lock (textBoxMessageOutput)
                    textBoxMessageOutput.AppendText(outputText.ToString());
            }
        }

        static ComparisonUtility()
        {
            // Set default logging path
            GSF.Diagnostics.Logger.SetLoggingPath(FilePath.GetAbsolutePath(""));
        }
    }
}
