//******************************************************************************************************
//  MigrationUtility.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Snap.Services;
using GSF.Snap.Storage;
using openHistorian.Snap;
using openHistorian.Snap.Encoding;
using openHistorian.Utility;

namespace ComparisonUtility
{
    public partial class MigrationUtility : Form
    {
        private class DataPoint
        {
            private ulong m_timestamp;

            public ulong Timestamp
            {
                get => m_timestamp / Ticks.PerMillisecond * Ticks.PerMillisecond;
                set => m_timestamp = value / Ticks.PerMillisecond * Ticks.PerMillisecond;
            }

            public float ValueAsSingle
            {
                get => BitConvert.ToSingle(Value);
                set => Value = BitConvert.ToUInt64(value);
            }

            public ulong PointID;
            public ulong Value;
            public ulong Flags;

            public void Clone(DataPoint destination)
            {
                destination.m_timestamp = m_timestamp;
                destination.PointID = PointID;
                destination.Value = Value;
                destination.Flags = Flags;
            }
        }

        private readonly ManualResetEventSlim m_archiveReady;
        private bool m_operationStarted;
        private bool m_formClosing;
        private long m_pointCount;
        private int m_defaultMaxThreads;

        public MigrationUtility()
        {
            InitializeComponent();
            m_archiveReady = new ManualResetEventSlim(false);
        }

        private void MigrationUtility_Load(object sender, EventArgs e)
        {
            // Add formatted names for ArchiveDirectoryMethod enumeration to combo-box
            comboBoxDirectoryNamingMode.Items.AddRange(
                Enum.GetValues(typeof(ArchiveDirectoryMethod)).Cast<ArchiveDirectoryMethod>().
                Select(method => (object)method.GetFormattedName()).ToArray());

            comboBoxDirectoryNamingMode.SelectedIndex = (int)ArchiveDirectoryMethod.YearThenMonth;

            if (Environment.ProcessorCount > 1)
                m_defaultMaxThreads = Environment.ProcessorCount / 2;
            else
                m_defaultMaxThreads = 1;

            textBoxMaxThreads.Text = m_defaultMaxThreads.ToString();
            radioButtonFastMigration.Checked = true;
        }

        private void MigrationUtility_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_formClosing = true;
            m_archiveReady.Set();
        }

        private void MigrationUtility_FormClosed(object sender, FormClosedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);
            Environment.Exit(0);
        }

        private void buttonOpenSourceFilesLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = "Find source location for openHistorian 1.0 / DatAWare archive files";

            if (Directory.Exists(textBoxSourceFiles.Text))
                folderBrowserDialog.SelectedPath = textBoxSourceFiles.Text;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                textBoxSourceFiles.Text = folderBrowserDialog.SelectedPath;
        }

        private void buttonOpenSourceOffloadedFilesLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = "Find offload location for openHistorian 1.0 / DatAWare archive files";

            if (Directory.Exists(textBoxSourceOffloadedFiles.Text))
                folderBrowserDialog.SelectedPath = textBoxSourceOffloadedFiles.Text;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                textBoxSourceOffloadedFiles.Text = folderBrowserDialog.SelectedPath;
        }

        private void buttonOpenDestinationFilesLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = "Find destination location for new openHistorian 2.0 archive files";

            if (Directory.Exists(textBoxDestinationFiles.Text))
                folderBrowserDialog.SelectedPath = textBoxDestinationFiles.Text;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                textBoxDestinationFiles.Text = folderBrowserDialog.SelectedPath;
        }

        private void checkBoxIgnoreDuplicateKeys_CheckedChanged(object sender, EventArgs e)
        {
            labelDuplicatesIgnored.Visible = checkBoxIgnoreDuplicateKeys.Checked;
        }

        private void textBoxSourceFiles_TextChanged(object sender, EventArgs e)
        {
            EnableGoButton(Directory.Exists(textBoxSourceFiles.Text) && Directory.Exists(textBoxDestinationFiles.Text));
        }

        private void textBoxDestinationFiles_TextChanged(object sender, EventArgs e)
        {
            EnableGoButton(Directory.Exists(textBoxSourceFiles.Text) && Directory.Exists(textBoxDestinationFiles.Text));
        }

        private void radioButtonLiveMigration_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonLiveMigration.Checked)
                return;

            labelTargetFileSize.Visible = true;
            textBoxTargetFileSize.Visible = true;
            labelGigabytes.Visible = true;

            labelMaxParallelism.Visible = false;
            textBoxMaxThreads.Visible = false;
            labelThreads.Visible = false;
        }

        private void radioButtonFastMigration_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonFastMigration.Checked)
                return;

            labelMaxParallelism.Visible = true;
            textBoxMaxThreads.Visible = true;
            labelThreads.Visible = true;

            labelTargetFileSize.Visible = false;
            textBoxTargetFileSize.Visible = false;
            labelGigabytes.Visible = false;
        }

        private void radioButtonCompareArchives_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonCompareArchives.Checked)
                return;

            labelMaxParallelism.Visible = true;
            textBoxMaxThreads.Visible = true;
            labelThreads.Visible = true;

            labelTargetFileSize.Visible = false;
            textBoxTargetFileSize.Visible = false;
            labelGigabytes.Visible = false;

            EnableGoButton(Directory.Exists(textBoxSourceFiles.Text) && Directory.Exists(textBoxDestinationFiles.Text));
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
            parameters["maxThreads"] = textBoxMaxThreads.Text;
            parameters["directoryNamingMethod"] = comboBoxDirectoryNamingMode.SelectedIndex.ToString();
            parameters["ignoreDuplicates"] = checkBoxIgnoreDuplicateKeys.Checked.ToString();

            Thread operation =
                radioButtonCompareArchives.Checked ? new Thread(CompareArchives) :
                radioButtonLiveMigration.Checked ? new Thread(LiveMigration) : new Thread(FastMigration);

            operation.IsBackground = true;
            operation.Start(parameters);
        }

        private void LiveMigration(object state)
        {
            try
            {
                const int MessageInterval = 1000000;
                Ticks operationStartTime = DateTime.UtcNow.Ticks;
                Dictionary<string, string> parameters = state as Dictionary<string, string>;

                if (parameters is null)
                    throw new ArgumentNullException("state", "Could not interpret thread state as parameters dictionary");

                ClearUpdateMessages();

                string instanceName = OpenGSFHistorianArchive(
                    parameters["sourceFilesLocation"],
                    parameters["sourceFilesOffloadLocation"],
                    parameters["instanceName"]);

                UpdateInstanceName(instanceName);

                if (!m_archiveReady.Wait(300000))
                {
                    ShowUpdateMessage("Still initializing source historian after 5 minutes...");
                    m_archiveReady.Wait();
                }

                bool ignoreDuplicates = parameters["ignoreDuplicates"].ParseBoolean();
                DataPoint point = new DataPoint();
                long migratedPoints = 0;
                long displayMessageCount = MessageInterval;

                SetProgressMaximum(100);

                Ticks readStartTime = DateTime.UtcNow.Ticks;

                using (SnapDBEngine engine = new SnapDBEngine(this,
                    instanceName,
                    parameters["destinationFilesLocation"],
                    parameters["targetFileSize"],
                    parameters["directoryNamingMethod"]))
                using (SnapDBClient client = new SnapDBClient(engine, instanceName))
                {
                    while (ReadNextGSFHistorianPoint(point))
                    {
                        client.WriteSnapDBData(point, ignoreDuplicates);
                        migratedPoints++;

                        if (migratedPoints == displayMessageCount)
                        {
                            ShowUpdateMessage("{0}Migrated {1:#,##0} points so far averaging {2:#,##0} points per second...{0}", Environment.NewLine, migratedPoints, migratedPoints / (DateTime.UtcNow.Ticks - readStartTime).ToSeconds());

                            if (m_pointCount > 0)
                                UpdateProgressBar((int)(migratedPoints / (double)m_pointCount * 100.0D));

                            displayMessageCount += MessageInterval;
                        }

                        if (m_formClosing)
                            break;
                    }

                    if (m_formClosing)
                    {
                        ShowUpdateMessage("Migration canceled.");
                        UpdateProgressBar(0);
                    }
                    else
                    {
                        client.FlushSnapDB();
                        ShowUpdateMessage("*** Migration Complete ***");
                        ShowUpdateMessage("Total migration time {0}", (DateTime.UtcNow.Ticks - operationStartTime).ToElapsedTimeString(3));
                        UpdateProgressBar(100);
                    }
                }

            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Failure during migration: {0}", ex.Message);
            }
            finally
            {
                m_operationStarted = false;
                CloseGSFHistorianArchive();
            }
        }

        private void FastMigration(object state)
        {
            try
            {
                long operationStartTime = DateTime.UtcNow.Ticks;
                Dictionary<string, string> parameters = state as Dictionary<string, string>;

                if (parameters is null)
                    throw new ArgumentNullException("state", "Could not interpret thread state as parameters dictionary");

                ClearUpdateMessages();

                ShowUpdateMessage("Scanning source files...");

                if (!Directory.Exists(parameters["sourceFilesLocation"]))
                    throw new DirectoryNotFoundException(string.Format("Source directory \"{0}\" not found.", parameters["sourceFilesLocation"]));

                IEnumerable<string> sourceFiles = Directory.EnumerateFiles(parameters["sourceFilesLocation"], "*.d", SearchOption.TopDirectoryOnly);

                if (Directory.Exists(parameters["sourceFilesOffloadLocation"]))
                    sourceFiles = sourceFiles.Concat(Directory.EnumerateFiles(parameters["sourceFilesOffloadLocation"], "*.d", SearchOption.TopDirectoryOnly));

                if (!int.TryParse(parameters["maxThreads"], out int maxThreads))
                    maxThreads = m_defaultMaxThreads;

                if (!int.TryParse(parameters["directoryNamingMethod"], out int methodIndex) || !Enum.IsDefined(typeof(ArchiveDirectoryMethod), methodIndex))
                    methodIndex = (int)ArchiveDirectoryMethod.YearThenMonth;

                ArchiveDirectoryMethod method = (ArchiveDirectoryMethod)methodIndex;
                HistorianFileEncoding encoder = new HistorianFileEncoding();
                string[] sourceFileNames = sourceFiles.ToArray();
                string destinationPath = parameters["destinationFilesLocation"];
                string instanceName = parameters["instanceName"];
                bool ignoreDuplicates = parameters["ignoreDuplicates"].ParseBoolean();
                long totalProcessedPoints = 0;
                int processedFiles = 0;

                SetProgressMaximum(sourceFileNames.Length);

                using (StreamWriter duplicateDataOutput = File.CreateText(FilePath.GetAbsolutePath("DuplicateData.txt")))
                {
                    Parallel.ForEach(sourceFileNames, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = maxThreads
                    },
                    (sourceFileName, loopState) =>
                    {
                        ShowUpdateMessage("Migrating \"{0}\"...", FilePath.GetFileName(sourceFileName));

                        long fileConversionStartTime = DateTime.UtcNow.Ticks;
                        long migratedPoints;

                        if (ignoreDuplicates)
                        {
                            // Migrate using SortedTreeFileSimpleWriter.CreateNonSequential() with raw unsorted historian file read
                            migratedPoints = ConvertArchiveFile.ConvertVersion1FileIgnoreDuplicates(
                                sourceFileName,
                                GetDestinationFileName(sourceFileName, instanceName, destinationPath, method),
                                encoder.EncodingMethod);
                        }
                        else
                        {
                            // Migrate using SortedTreeFileSimpleWriter.Create() with API sorted historian file read with duplicate handling
                            using (GSFHistorianStream stream = new GSFHistorianStream(this, sourceFileName, instanceName, duplicateDataOutput))
                            {
                                string completeFileName = GetDestinationFileName(stream.ArchiveFile, sourceFileName, instanceName, destinationPath, method);
                                string pendingFileName = Path.Combine(FilePath.GetDirectoryName(completeFileName), FilePath.GetFileNameWithoutExtension(completeFileName) + ".~d2i");

                                SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.Create(pendingFileName, completeFileName, 4096, null, encoder.EncodingMethod, stream);

                                migratedPoints = stream.PointCount;
                            }
                        }

                        Ticks totalTime = DateTime.UtcNow.Ticks - fileConversionStartTime;

                        ShowUpdateMessage(
                            "{0}Migrated {1:#,##0} points for last file in {2} at {3:#,##0} points per second.{0}",
                            Environment.NewLine,
                            migratedPoints,
                            totalTime.ToElapsedTimeString(3),
                            migratedPoints / totalTime.ToSeconds());

                        Interlocked.Increment(ref processedFiles);
                        Interlocked.Add(ref totalProcessedPoints, migratedPoints);

                        UpdateProgressBar(processedFiles);

                        if (m_formClosing)
                            loopState.Break();
                    });
                }

                if (m_formClosing)
                {
                    ShowUpdateMessage("Migration canceled.");
                    UpdateProgressBar(0);
                }
                else
                {
                    Ticks totalTime = DateTime.UtcNow.Ticks - operationStartTime;
                    ShowUpdateMessage("*** Migration Complete ***");
                    ShowUpdateMessage("Total migration time {0} at {1:#,##0} points per second.", totalTime.ToElapsedTimeString(3), totalProcessedPoints / totalTime.ToSeconds());
                    UpdateProgressBar(sourceFileNames.Length);
                }
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Failure during migration: {0}", ex.Message);
            }
            finally
            {
                m_operationStarted = false;
            }
        }

        private void CompareArchives(object state)
        {
            try
            {
                const int MessageInterval = 1000000;
                Ticks operationStartTime = DateTime.UtcNow.Ticks;
                Dictionary<string, string> parameters = state as Dictionary<string, string>;

                if (parameters is null)
                    throw new ArgumentNullException("state", "Could not interpret thread state as parameters dictionary");

                ClearUpdateMessages();

                ShowUpdateMessage("Scanning source files...");

                if (!Directory.Exists(parameters["sourceFilesLocation"]))
                    throw new DirectoryNotFoundException(string.Format("Source directory \"{0}\" not found.", parameters["sourceFilesLocation"]));

                IEnumerable<string> sourceFiles = Directory.EnumerateFiles(parameters["sourceFilesLocation"], "*.d", SearchOption.TopDirectoryOnly);

                if (Directory.Exists(parameters["sourceFilesOffloadLocation"]))
                    sourceFiles = sourceFiles.Concat(Directory.EnumerateFiles(parameters["sourceFilesOffloadLocation"], "*.d", SearchOption.TopDirectoryOnly));

                // Start calculating total number of source points
                m_pointCount = 0;
                ThreadPool.QueueUserWorkItem(CalculateSourcePointCount, new[] { parameters["sourceFilesLocation"], parameters["sourceFilesOffloadLocation"] });

                if (!int.TryParse(parameters["maxThreads"], out int maxThreads))
                    maxThreads = m_defaultMaxThreads;

                string[] sourceFileNames = sourceFiles.ToArray();
                string instanceName = parameters["instanceName"];
                bool ignoreDuplicates = parameters["ignoreDuplicates"].ParseBoolean();
                long comparedPoints = 0;
                long validPoints = 0;
                long invalidPoints = 0;
                long missingPoints = 0;
                long duplicatePoints = 0;
                long resyncs = 0;
                long displayMessageCount = MessageInterval;

                SetProgressMaximum(100);

                using (SnapDBEngine engine = new SnapDBEngine(this,
                    instanceName,
                    parameters["destinationFilesLocation"],
                    parameters["targetFileSize"],
                    parameters["directoryNamingMethod"]))
                using (StreamWriter missingDataOutput = File.CreateText(FilePath.GetAbsolutePath("MissingData.txt")))
                {
                    Parallel.ForEach(sourceFileNames, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = maxThreads
                    },
                    (sourceFileName, loopState) =>
                    {
                        ShowUpdateMessage("Comparing \"{0}\"...", FilePath.GetFileName(sourceFileName));

                        DataPoint sourcePoint = new DataPoint();
                        DataPoint destinationPoint = new DataPoint();
                        DataPoint lastPoint = new DataPoint();
                        Ticks readStartTime = DateTime.UtcNow.Ticks;
                        bool updateProgress, resync, readInitialized = false;

                        using (GSFHistorianStream sourceStream = new GSFHistorianStream(this, sourceFileName, instanceName))
                        using (SnapDBClient client = new SnapDBClient(engine, sourceStream.InstanceName))
                        {
                            while (true)
                            {
                                if (sourceStream.ReadNext(sourcePoint))
                                {
                                    if (ignoreDuplicates)
                                    {
                                        bool success = true;

                                        while (success && sourcePoint.PointID == lastPoint.PointID && sourcePoint.Timestamp == lastPoint.Timestamp)
                                        {
                                            Interlocked.Increment(ref duplicatePoints);
                                            success = sourceStream.ReadNext(sourcePoint);
                                        }

                                        // Finished with source read
                                        if (!success)
                                            break;
                                    }

                                    if (readInitialized)
                                    {
                                        if (!client.ReadNextSnapDBPoint(destinationPoint))
                                        {
                                            ShowUpdateMessage("*** Compare for \"{0}\" Failed: Destination Read Was Short ***", FilePath.GetFileName(sourceFileName));
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        readInitialized = client.ScanToSnapDBPoint(sourcePoint.Timestamp, sourcePoint.PointID, destinationPoint);
                                    }
                                }
                                else
                                {
                                    // Finished with source read
                                    break;
                                }

                                resync = false;

                                do
                                {
                                    if (resync)
                                        Interlocked.Increment(ref resyncs);

                                    // See if source and destination points match
                                    if (sourcePoint.PointID == destinationPoint.PointID && sourcePoint.Timestamp == destinationPoint.Timestamp)
                                    {
                                        if (sourcePoint.Value == destinationPoint.Value)
                                        {
                                            if (sourcePoint.Flags == destinationPoint.Flags)
                                                Interlocked.Increment(ref validPoints);
                                            else
                                                Interlocked.Increment(ref invalidPoints);
                                        }
                                        else
                                        {
                                            Interlocked.Increment(ref invalidPoints);
                                        }

                                        resync = false;
                                    }
                                    else
                                    {
                                        // Attempt to resynchronize readers by rescanning to point if we didn't find point and are not resynchronizing already
                                        resync = !resync && client.ScanToSnapDBPoint(sourcePoint.Timestamp, sourcePoint.PointID, destinationPoint);

                                        if (!resync)
                                        {
                                            Interlocked.Increment(ref missingPoints);

                                            lock (missingDataOutput)
                                                missingDataOutput.WriteLine("[{0:00000}@{1:yyyy-MM-dd HH:mm:ss.fff}] = {2}({3})", sourcePoint.PointID, new DateTime((long)sourcePoint.Timestamp, DateTimeKind.Utc), sourcePoint.ValueAsSingle, sourcePoint.Flags);
                                        }
                                    }
                                }
                                while (resync);

                                // Update last point
                                if (ignoreDuplicates)
                                    sourcePoint.Clone(lastPoint);

                                updateProgress = false;

                                if (Interlocked.Increment(ref comparedPoints) == displayMessageCount)
                                {
                                    if (comparedPoints % (5 * MessageInterval) == 0)
                                        ShowUpdateMessage("{0}*** Compared {1:#,##0} points so far averaging {2:#,##0} points per second ***{0}",
                                                            Environment.NewLine,
                                                            comparedPoints,
                                                            comparedPoints / (DateTime.UtcNow.Ticks - readStartTime).ToSeconds());
                                    else
                                        ShowUpdateMessage("{0}Found {1:#,##0} valid, {2:#,##0} invalid and {3:#,##0} missing points during compare so far...{0}",
                                                            Environment.NewLine,
                                                            validPoints,
                                                            invalidPoints,
                                                            missingPoints);

                                    updateProgress = true;
                                    displayMessageCount += MessageInterval;
                                }

                                // Note that point count used here is estimated
                                if (updateProgress && m_pointCount > 0)
                                    UpdateProgressBar((int)(comparedPoints / (double)m_pointCount * 100.0D));
                            }
                        }

                        if (m_formClosing)
                            loopState.Break();
                    });

                    if (m_formClosing)
                    {
                        ShowUpdateMessage("Migration canceled.");
                        UpdateProgressBar(0);
                    }
                    else
                    {
                        Ticks totalTime = DateTime.UtcNow.Ticks - operationStartTime;
                        ShowUpdateMessage("*** Compare Complete ***");
                        ShowUpdateMessage("Total compare time {0} at {1:#,##0} points per second.", totalTime.ToElapsedTimeString(3), comparedPoints / totalTime.ToSeconds());
                        UpdateProgressBar(100);

                        ShowUpdateMessage("{0}" +
                            "Total points compared: {1:#,##0}{0}" +
                            "         Valid points: {2:#,##0}{0}" +
                            "       Invalid points: {3:#,##0}{0}" +
                            "       Missing points: {4:#,##0}{0}" +
                            "     Duplicate points: {5:#,##0}{0}" +
                            "   Resynchronizations: {6:#,##0}{0}" +
                            "   Source point count: {7:#,##0}{0}" +
                            "{0}Migrated data conversion {8:##0.000}% accurate",
                            Environment.NewLine,
                            comparedPoints,
                            validPoints,
                            invalidPoints,
                            missingPoints,
                            duplicatePoints,
                            resyncs,
                            comparedPoints + missingPoints,
                            Math.Truncate(validPoints / (double)(comparedPoints + missingPoints) * 100000.0D) / 1000.0D);

                        if (ignoreDuplicates && invalidPoints > 0 && duplicatePoints >= invalidPoints)
                            ShowUpdateMessage(
                                "{0}Note: Since duplicated source data was being ignored and duplicate points outnumber (or are equal to) " +
                                "invalid points, the invalid data is likely an artifact of comparing a duplicated source record that was " +
                                "not archived into the destination.{0}",
                                Environment.NewLine);
                    }
                }

            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Failure during compare: {0}", ex.Message);
            }
            finally
            {
                m_operationStarted = false;
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
                if (!m_operationStarted)
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
            GSF.Diagnostics.Logger.FileWriter.SetPath(FilePath.GetAbsolutePath(""), VerboseLevel.Ultra);
        }
    }
}
