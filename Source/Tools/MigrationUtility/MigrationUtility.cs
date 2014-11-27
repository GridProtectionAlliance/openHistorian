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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GSF;
using GSF.Historian;
using GSF.IO;
using GSF.Snap.Services;
using GSF.Snap.Storage;
using openHistorian.Snap;
using openHistorian.Snap.Encoding;
using openHistorian.Utility;

namespace MigrationUtility
{
    public partial class MigrationUtility : Form
    {
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private readonly ManualResetEventSlim m_archiveReady;
        private bool m_operationStarted;
        private bool m_formClosing;

        public MigrationUtility()
        {
            InitializeComponent();

            m_key = new HistorianKey();
            m_value = new HistorianValue();
            m_archiveReady = new ManualResetEventSlim(false);
        }

        private void MigrationUtility_Load(object sender, EventArgs e)
        {
            // Add formatted names for ArchiveDirectoryMethod enumeration to combo-box
            comboBoxDirectoryNamingMode.Items.AddRange(
                Enum.GetValues(typeof(ArchiveDirectoryMethod)).Cast<ArchiveDirectoryMethod>().
                Select(method => (object)method.GetFormattedName()).ToArray());

            comboBoxDirectoryNamingMode.SelectedIndex = (int)ArchiveDirectoryMethod.YearThenMonth;
            radioButtonFastMigration.Checked = true;
        }

        private void MigrationUtility_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_formClosing = true;
            m_archiveReady.Set();
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

        private void checkBoxSourceIsSSD_CheckedChanged(object sender, EventArgs e)
        {
            labelSSDInfo.Visible = checkBoxSourceIsSSD.Checked;
        }

        private void textBoxSourceFiles_TextChanged(object sender, EventArgs e)
        {
            if (radioButtonLiveMigration.Checked || radioButtonCompareArchives.Checked)
                UpdateInstanceName(OpenGSFHistorianArchive(textBoxSourceFiles.Text, textBoxSourceOffloadedFiles.Text, textBoxInstanceName.Text, true));
            else
                EnableGoButton(Directory.Exists(textBoxSourceFiles.Text));
        }

        private void radioButtonLiveMigration_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonLiveMigration.Checked)
                return;

            labelTargetFileSize.Enabled = true;
            textBoxTargetFileSize.Enabled = true;
            labelGigabytes.Enabled = true;
        }

        private void radioButtonFastMigration_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonFastMigration.Checked)
                return;

            // Fast migrations do a file-to-file conversion
            labelTargetFileSize.Enabled = false;
            textBoxTargetFileSize.Enabled = false;
            labelGigabytes.Enabled = false;
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            m_operationStarted = true;
            buttonGo.Enabled = false;

            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            parameters["sourceFilesLocation"] = textBoxSourceFiles.Text;
            parameters["sourceFilesOffloadLocation"] = textBoxSourceOffloadedFiles.Text;
            parameters["instanceName"] = textBoxInstanceName.Text;
            parameters["destinationFilesLocation"] = textBoxDestinationFiles.Text;
            parameters["targetFileSize"] = textBoxTargetFileSize.Text;
            parameters["directoryNamingMethod"] = comboBoxDirectoryNamingMode.SelectedIndex.ToString();
            parameters["sourceIsSSD"] = checkBoxSourceIsSSD.Checked.ToString();

            Thread operation =
                radioButtonCompareArchives.Checked ? new Thread(CompareArchives) :
                radioButtonLiveMigration.Checked ? new Thread(LiveMigration) : new Thread(FastMigration);

            operation.IsBackground = true;
            operation.Priority = ThreadPriority.Highest;
            operation.Start(parameters);
        }

        private void LiveMigration(object state)
        {
            try
            {
                Ticks operationStartTime = DateTime.UtcNow.Ticks;
                Dictionary<string, string> parameters = state as Dictionary<string, string>;

                if ((object)parameters == null)
                    throw new ArgumentNullException("state", "Could not interpret thread state as parameters dictionary");

                ClearUpdateMessages();

                UpdateInstanceName(
                OpenGSFHistorianArchive(
                    parameters["sourceFilesLocation"],
                    parameters["sourceFilesOffloadLocation"],
                    parameters["instanceName"]));
                if (!m_archiveReady.Wait(5000))
                    throw new TimeoutException("Failed waiting on source archive to initialize");

                OpenSnapDBEngine(
                    parameters["instanceName"],
                    parameters["destinationFilesLocation"],
                    parameters["targetFileSize"],
                    parameters["directoryNamingMethod"]);

                long migratedPoints = 0;
                Ticks readStartTime = DateTime.UtcNow.Ticks;

                foreach (IDataPoint point in ReadGSFHistorianData())
                {
                    WriteSnapDBData(point);
                    migratedPoints++;

                    if (migratedPoints % 1000000 == 0)
                        ShowUpdateMessage("{0}Migrated {1:#,##0} points so far averaging {2:#,##0} points per second...{0}", Environment.NewLine, migratedPoints, migratedPoints / (DateTime.UtcNow.Ticks - readStartTime).ToSeconds());

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
                    ShowUpdateMessage("*** Migration Complete ***");
                    ShowUpdateMessage("Total migration time {0}", (DateTime.UtcNow.Ticks - operationStartTime).ToElapsedTimeString(3));
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

        private void FastMigration(object state)
        {
            try
            {
                Ticks operationStartTime = DateTime.UtcNow.Ticks;
                Dictionary<string, string> parameters = state as Dictionary<string, string>;

                if ((object)parameters == null)
                    throw new ArgumentNullException("state", "Could not interpret thread state as parameters dictionary");

                ClearUpdateMessages();

                ShowUpdateMessage("Scanning source files...");

                if (!Directory.Exists(parameters["sourceFilesLocation"]))
                    throw new DirectoryNotFoundException(string.Format("Source directory \"{0}\" not found.", parameters["sourceFilesLocation"]));

                IEnumerable<string> sourceFiles = Directory.EnumerateFiles(parameters["sourceFilesLocation"], "*.d", SearchOption.TopDirectoryOnly);

                if (Directory.Exists(parameters["sourceFilesOffloadLocation"]))
                    sourceFiles = sourceFiles.Concat(Directory.EnumerateFiles(parameters["sourceFilesOffloadLocation"], "*.d", SearchOption.TopDirectoryOnly));

                int methodIndex;

                if (!int.TryParse(parameters["directoryNamingMethod"], out methodIndex))
                    methodIndex = (int)ArchiveDirectoryMethod.YearThenMonth;

                ArchiveDirectoryMethod method = (ArchiveDirectoryMethod)methodIndex;
                HistorianFileEncoding encoder = new HistorianFileEncoding();
                string destinationPath = parameters["destinationFilesLocation"];
                string instanceName = parameters["instanceName"];
                string completeFileName, pendingFileName;
                bool sourceIsSSD = parameters["sourceIsSSD"].ParseBoolean();
                long fileConversionStartTime, migratedPoints, readTime, sortTime, writeTime;
                Ticks totalTime;

                foreach (string sourceFile in sourceFiles)
                {
                    ShowUpdateMessage("Migrating \"{0}\"...", FilePath.GetFileName(sourceFile));

                    fileConversionStartTime = DateTime.UtcNow.Ticks;

                    if (sourceIsSSD)
                    {
                        // Option 1: Use time-sorted data reader - reads data sorted from source file directly into SnapDB.
                        // This is faster than option 2 for reading source data from a SSD.
                        using (GSFHistorianStream stream = new GSFHistorianStream(this, sourceFile, instanceName))
                        {
                            completeFileName = GetDestinationFileName(stream.ArchiveFile, sourceFile, instanceName, destinationPath, method);
                            pendingFileName = Path.Combine(FilePath.GetDirectoryName(completeFileName), FilePath.GetFileNameWithoutExtension(completeFileName) + ".~d2i");
                            SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.Create(pendingFileName, completeFileName, 4096, null, encoder.EncodingMethod, stream);
                            migratedPoints = stream.Total;
                        }

                        totalTime = DateTime.UtcNow.Ticks - fileConversionStartTime;

                        ShowUpdateMessage(
                            "{0}Migrated {1:#,##0} points for last file in {2} at {3:#,##0} points per second.{0}",
                            Environment.NewLine,
                            migratedPoints,
                            totalTime.ToElapsedTimeString(3),
                            migratedPoints / totalTime.ToSeconds());
                    }
                    else
                    {
                        // Option 2: Use raw file reader - reads data unsorted from source file into memory, sorts data then writes to SnapDB.
                        // This is faster than option 1 for reading source data from a spinning disk.
                        migratedPoints = ConvertArchiveFile.ConvertVersion1File(sourceFile, GetDestinationFileName(sourceFile, instanceName, destinationPath, method), encoder.EncodingMethod, out readTime, out sortTime, out writeTime);

                        totalTime = DateTime.UtcNow.Ticks - fileConversionStartTime;

                        ShowUpdateMessage(
                            "{0}Migrated {1:#,##0} points for last file in {2} at {3:#,##0} points per second.{0}{0}" +
                            "    Historian read time: {4}: {5:00.00%}{0}" +
                            "      Data sorting time: {6}: {7:00.00%}{0}" +
                            "      SnapDB write time: {8}: {9:00.00%}{0}",
                            Environment.NewLine,
                            migratedPoints,
                            totalTime.ToElapsedTimeString(3),
                            migratedPoints / totalTime.ToSeconds(),
                            new Ticks(readTime).ToElapsedTimeString(3),
                            readTime / (double)totalTime,
                            new Ticks(sortTime).ToElapsedTimeString(3),
                            sortTime / (double)totalTime,
                            new Ticks(writeTime).ToElapsedTimeString(3),
                            writeTime / (double)totalTime);
                    }

                    if (m_formClosing)
                        break;
                }

                if (m_formClosing)
                {
                    ShowUpdateMessage("Migration canceled.");
                }
                else
                {
                    ShowUpdateMessage("*** Migration Complete ***");
                    ShowUpdateMessage("Total migration time {0}", (DateTime.UtcNow.Ticks - operationStartTime).ToElapsedTimeString(3));
                }
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Failure during migration: {0}", ex.Message);
            }
            finally
            {
                EnableGoButton(false);
            }
        }

        private void CompareArchives(object state)
        {
            try
            {
                Ticks operationStartTime = DateTime.UtcNow.Ticks;
                Dictionary<string, string> parameters = state as Dictionary<string, string>;

                if ((object)parameters == null)
                    throw new ArgumentNullException("state", "Could not interpret thread state as parameters dictionary");

                ClearUpdateMessages();

                UpdateInstanceName(
                OpenGSFHistorianArchive(
                    parameters["sourceFilesLocation"],
                    parameters["sourceFilesOffloadLocation"],
                    parameters["instanceName"],
                    operationName: "Compare"));

                if (!m_archiveReady.Wait(5000))
                    throw new TimeoutException("Failed waiting on source archive to initialize");
                OpenSnapDBEngine(
                    parameters["instanceName"],
                    parameters["destinationFilesLocation"],
                    parameters["targetFileSize"],
                    parameters["directoryNamingMethod"],
                    true);

                IDataPoint destinationPoint;
                long comparedPoints = 0;
                long validPoints = 0;
                long invalidPoints = 0;
                long missingPoints = 0;
                Ticks readStartTime = DateTime.UtcNow.Ticks;

                foreach (IDataPoint sourcePoint in ReadGSFHistorianData())
                {
                    destinationPoint = ReadSnapDBValue(sourcePoint.Time.ToDateTime().Ticks, sourcePoint.HistorianID);

                    if ((object)destinationPoint != null)
                    {
                        if (sourcePoint.Value == destinationPoint.Value && sourcePoint.Quality == destinationPoint.Quality)
                            validPoints++;
                        else
                            invalidPoints++;
                    }
                    else
                    {
                        missingPoints++;
                    }

                    comparedPoints++;

                    if (comparedPoints % 100000 == 0)
                        ShowUpdateMessage("{0}*** Compared {1:#,##0} points so far averaging {2:#,##0} points per second ***{0}", Environment.NewLine, comparedPoints, comparedPoints / (DateTime.UtcNow.Ticks - readStartTime).ToSeconds());
                    else if ((validPoints > 0 && validPoints % 20000 == 0) || (invalidPoints > 0 && invalidPoints % 20000 == 0) || (missingPoints > 0 && missingPoints % 20000 == 0))
                        ShowUpdateMessage("{0}Found {1:#,##0} valid, {2:#,##0} invalid and {3:#,##0} missing points during compare so far...{0}", Environment.NewLine, validPoints, invalidPoints, missingPoints);

                    if (m_formClosing)
                        break;
                }

                if (m_formClosing)
                {
                    ShowUpdateMessage("Compare canceled.");
                }
                else
                {
                    ShowUpdateMessage("*** Compare Complete ***");
                    ShowUpdateMessage("Total compare time {0}", (DateTime.UtcNow.Ticks - operationStartTime).ToElapsedTimeString(3));
                }

                ShowUpdateMessage("{0}" +
                    "Total points compared: {1:#,##0}{0}" +
                    "         Valid points: {2:#,##0}{0}" +
                    "       Invalid points: {3:#,##0}{0}" +
                    "       Missing points: {4:#,##0}{0}" +
                    "{0}Migrated data conversion {5:##0.000%} accurate",
                    Environment.NewLine,
                    comparedPoints,
                    validPoints,
                    invalidPoints,
                    missingPoints,
                    validPoints / (double)comparedPoints);
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Failure during compare: {0}", ex.Message);
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

        private void UpdateInstanceName(string instanceName)
        {
            if (m_formClosing)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateInstanceName), instanceName);
            }
            else
            {
                textBoxInstanceName.Text = instanceName;
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
                StringBuilder outputText = new StringBuilder();

                outputText.AppendFormat(message, args);
                outputText.AppendLine();

                lock (textBoxMessageOutput)
                    textBoxMessageOutput.AppendText(outputText.ToString());
            }
        }

        static MigrationUtility()
        {
            // Set default logging path
            GSF.Diagnostics.Logger.SetLoggingPath(FilePath.GetAbsolutePath(""));
        }
    }
}
