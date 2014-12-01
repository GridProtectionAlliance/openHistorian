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
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF;
using GSF.IO;
using GSF.Snap;
using GSF.Snap.Services;
using GSF.Snap.Storage;
using openHistorian.Snap;
using openHistorian.Snap.Encoding;
using openHistorian.Utility;

namespace MigrationUtility
{
    public partial class MigrationUtility : Form
    {
        private class DataPoint
        {
            public ulong Timestamp;
            public ulong PointID;
            public ulong Value;
            public ulong Flags;
        }

        private readonly ManualResetEventSlim m_archiveReady;
        private bool m_operationStarted;
        private bool m_formClosing;
        private long m_pointCount;

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
            textBoxMaxThreads.Text = Environment.ProcessorCount.ToString();
            radioButtonFastMigration.Checked = true;
        }

        private void MigrationUtility_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_formClosing = true;
            m_archiveReady.Set();
        }

        private void MigrationUtility_FormClosed(object sender, FormClosedEventArgs e)
        {
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
            bool duplicatesIgnored = checkBoxIgnoreDuplicateKeys.Checked;
            labelDuplicatesIgnored.Visible = duplicatesIgnored;
            labelDuplicatesSaved.Visible = !duplicatesIgnored;
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

                if (!m_archiveReady.Wait(300000))
                {
                    ShowUpdateMessage("Still initializing source historian after 5 minutes...");
                    m_archiveReady.Wait();
                }

                bool ignoreDuplicates = parameters["ignoreDuplicates"].ParseBoolean();
                string instanceName = parameters["instanceName"];
                DataPoint point = new DataPoint();
                long migratedPoints = 0;

                SetProgressMaximum(100);

                Ticks readStartTime = DateTime.UtcNow.Ticks;

                using (SnapDBEngine engine = new SnapDBEngine(this,
                    instanceName,
                    parameters["destinationFilesLocation"],
                    parameters["targetFileSize"],
                    parameters["directoryNamingMethod"]))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> clientDatabase = engine.GetClientDatabase(instanceName))
                {
                    while (ReadNextGSFHistorianPoint(point))
                    {
                        engine.WriteSnapDBData(clientDatabase, point, ignoreDuplicates);
                        migratedPoints++;

                        if (migratedPoints % 1000000 == 0)
                        {
                            ShowUpdateMessage("{0}Migrated {1:#,##0} points so far averaging {2:#,##0} points per second...{0}", Environment.NewLine, migratedPoints, migratedPoints / (DateTime.UtcNow.Ticks - readStartTime).ToSeconds());

                            if (m_pointCount > 0)
                                UpdateProgressBar((int)((migratedPoints / (double)m_pointCount) * 100.0D));
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
                        engine.FlushSnapDB(clientDatabase);
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

                if ((object)parameters == null)
                    throw new ArgumentNullException("state", "Could not interpret thread state as parameters dictionary");

                ClearUpdateMessages();

                ShowUpdateMessage("Scanning source files...");

                if (!Directory.Exists(parameters["sourceFilesLocation"]))
                    throw new DirectoryNotFoundException(string.Format("Source directory \"{0}\" not found.", parameters["sourceFilesLocation"]));

                IEnumerable<string> sourceFiles = Directory.EnumerateFiles(parameters["sourceFilesLocation"], "*.d", SearchOption.TopDirectoryOnly);

                if (Directory.Exists(parameters["sourceFilesOffloadLocation"]))
                    sourceFiles = sourceFiles.Concat(Directory.EnumerateFiles(parameters["sourceFilesOffloadLocation"], "*.d", SearchOption.TopDirectoryOnly));

                int maxThreads, methodIndex;

                if (!int.TryParse(parameters["maxThreads"], out maxThreads))
                    maxThreads = Environment.ProcessorCount;

                if (!int.TryParse(parameters["directoryNamingMethod"], out methodIndex) || !Enum.IsDefined(typeof(ArchiveDirectoryMethod), methodIndex))
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

                Parallel.ForEach(sourceFileNames, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, (sourceFileName, loopState) =>
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
                        using (GSFHistorianStream stream = new GSFHistorianStream(this, sourceFileName, instanceName))
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

                    UpdateProgressBar(++processedFiles);
                    totalProcessedPoints += migratedPoints;

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

                // Start calculating total number of source points
                m_pointCount = 0;
                ThreadPool.QueueUserWorkItem(CalculateSourcePointCount, new[] { parameters["sourceFilesLocation"], parameters["sourceFilesOffloadLocation"] });

                int maxThreads;

                if (!int.TryParse(parameters["maxThreads"], out maxThreads))
                    maxThreads = Environment.ProcessorCount;

                string[] sourceFileNames = sourceFiles.ToArray();
                string instanceName = parameters["instanceName"];
                long comparedPoints = 0;
                long validPoints = 0;
                long invalidPoints = 0;
                long missingPoints = 0;
                long valueErrors = 0;
                long flagErrors = 0;

                SetProgressMaximum(100);

                Parallel.ForEach(sourceFileNames, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, (sourceFileName, loopState) =>
                {
                    ShowUpdateMessage("Comparing \"{0}\"...", FilePath.GetFileName(sourceFileName));

                    DataPoint sourcePoint = new DataPoint();
                    DataPoint destinationPoint = new DataPoint();
                    Ticks readStartTime = DateTime.UtcNow.Ticks;
                    bool updateProgress;

                    using (SnapDBEngine engine = new SnapDBEngine(this,
                        instanceName,
                        parameters["destinationFilesLocation"],
                        parameters["targetFileSize"],
                        parameters["directoryNamingMethod"]))
                    using (GSFHistorianStream sourceStream = new GSFHistorianStream(this, sourceFileName, instanceName))
                    using (ClientDatabaseBase<HistorianKey, HistorianValue> clientDatabase = engine.GetClientDatabase(sourceStream.InstanceName))
                    {
                        TreeStream<HistorianKey, HistorianValue> destinationStream = null;

                        try
                        {
                            while (true)
                            {
                                if (sourceStream.ReadNext(sourcePoint))
                                {
                                    if ((object)destinationStream == null)
                                    {
                                        destinationStream = engine.ScanToSnapDBPoint(clientDatabase, sourcePoint.Timestamp, sourcePoint.PointID, destinationPoint);
                                    }
                                    else if (!engine.ReadNextSnapDBPoint(destinationStream, destinationPoint))
                                    {
                                        ShowUpdateMessage("*** Compare Failed: Destination Read Was Short ***");
                                        break;
                                    }
                                }
                                else
                                {
                                    // Finished with source read
                                    break;
                                }

                                if (sourcePoint.PointID == destinationPoint.PointID && sourcePoint.Timestamp / Ticks.PerMillisecond == destinationPoint.Timestamp / Ticks.PerMillisecond)
                                {
                                    if (sourcePoint.Value == destinationPoint.Value)
                                    {
                                        if (sourcePoint.Flags == destinationPoint.Flags)
                                        {
                                            Interlocked.Increment(ref validPoints);
                                        }
                                        else
                                        {
                                            Interlocked.Increment(ref flagErrors);
                                            Interlocked.Increment(ref invalidPoints);
                                        }
                                    }
                                    else
                                    {
                                        Interlocked.Increment(ref valueErrors);
                                        Interlocked.Increment(ref invalidPoints);
                                    }
                                }
                                else
                                {
                                    Interlocked.Increment(ref missingPoints);

                                    if ((object)destinationStream != null)
                                        destinationStream.Dispose();

                                    destinationStream = engine.ScanToSnapDBPoint(clientDatabase, sourcePoint.Timestamp, sourcePoint.PointID, destinationPoint);

                                    while (sourcePoint.PointID != destinationPoint.PointID || sourcePoint.Timestamp / Ticks.PerMillisecond != destinationPoint.Timestamp / Ticks.PerMillisecond)
                                    {
                                        Interlocked.Increment(ref missingPoints);
                                        if (!sourceStream.ReadNext(sourcePoint))
                                            break;
                                    }
                                }

                                Interlocked.Increment(ref comparedPoints);
                                updateProgress = false;

                                if (comparedPoints % 5000000 == 0)
                                {
                                    ShowUpdateMessage("{0}*** Compared {1:#,##0} points so far averaging {2:#,##0} points per second ***{0}", Environment.NewLine, comparedPoints, comparedPoints / (DateTime.UtcNow.Ticks - readStartTime).ToSeconds());
                                    updateProgress = true;
                                }
                                else if ((validPoints > 0 && validPoints % 1000000 == 0) || (invalidPoints > 0 && invalidPoints % 1000000 == 0) || (missingPoints > 0 && missingPoints % 1000000 == 0))
                                {
                                    ShowUpdateMessage("{0}Found {1:#,##0} valid, {2:#,##0} invalid and {3:#,##0} missing points during compare so far...{0}" +
                                                      "     Value Errors: {4:#,##0}{0}" +
                                                      "      Flag Errors: {5:#,##0}{0}",
                                                      Environment.NewLine,
                                                      validPoints,
                                                      invalidPoints,
                                                      missingPoints,
                                                      valueErrors,
                                                      flagErrors);

                                    updateProgress = true;
                                }

                                if (updateProgress && m_pointCount > 0)
                                    UpdateProgressBar((int)((comparedPoints / (double)m_pointCount) * 100.0D));
                            }

                        }
                        finally
                        {
                            if ((object)destinationStream != null)
                                destinationStream.Dispose();
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
                        "   Source point count: {5:#,##0}{0}" +
                        "{0}Migrated data conversion {6:##0.000}% accurate",
                        Environment.NewLine,
                        comparedPoints,
                        validPoints,
                        invalidPoints,
                        missingPoints,
                        m_pointCount,
                        Math.Truncate(validPoints / (double)(comparedPoints + missingPoints) * 100000.0D) / 1000.0D);
                }

                //UpdateInstanceName(
                //OpenGSFHistorianArchive(
                //    parameters["sourceFilesLocation"],
                //    parameters["sourceFilesOffloadLocation"],
                //    parameters["instanceName"],
                //    operationName: "Compare"));

                //if (!m_archiveReady.Wait(300000))
                //{
                //    ShowUpdateMessage("Still initializing source historian after 5 minutes...");
                //    m_archiveReady.Wait();
                //}

                //OpenSnapDBEngine(
                //    parameters["instanceName"],
                //    parameters["destinationFilesLocation"],
                //    parameters["targetFileSize"],
                //    parameters["directoryNamingMethod"],
                //    true);

                //DataPoint sourcePoint = new DataPoint();
                //DataPoint destinationPoint = new DataPoint();
                //long comparedPoints = 0;
                //long validPoints = 0;
                //long invalidPoints = 0;
                //long missingPoints = 0;
                //long valueErrors = 0;
                //long flagErrors = 0;
                //bool updateProgress;
                //Ticks readStartTime = DateTime.UtcNow.Ticks;

                //SetProgressMaximum(100);

                //while (true)
                //{
                //    if (ReadNextGSFHistorianPoint(sourcePoint))
                //    {
                //        if (!ReadNextSnapDBPoint(destinationPoint))
                //        {
                //            ShowUpdateMessage("*** Compare Failed: Destination Read Was Short ***");
                //            break;
                //        }
                //    }
                //    else
                //    {
                //        // Finished with source read
                //        break;
                //    }

                //    if (sourcePoint.PointID == destinationPoint.PointID && sourcePoint.Timestamp / Ticks.PerMillisecond == destinationPoint.Timestamp / Ticks.PerMillisecond)
                //    {
                //        if (sourcePoint.Value == destinationPoint.Value)
                //        {
                //            if (sourcePoint.Flags == destinationPoint.Flags)
                //            {
                //                validPoints++;
                //            }
                //            else
                //            {
                //                flagErrors++;
                //                invalidPoints++;
                //            }
                //        }
                //        else
                //        {
                //            valueErrors++;
                //            invalidPoints++;
                //        }
                //    }
                //    else
                //    {
                //        missingPoints++;
                //        ScanToSnapDBPoint(sourcePoint.Timestamp, sourcePoint.PointID, destinationPoint);

                //        while (sourcePoint.PointID != destinationPoint.PointID || sourcePoint.Timestamp / Ticks.PerMillisecond != destinationPoint.Timestamp / Ticks.PerMillisecond)
                //        {
                //            missingPoints++;
                //            if (!ReadNextGSFHistorianPoint(sourcePoint))
                //                break;
                //        }
                //    }

                //    comparedPoints++;
                //    updateProgress = false;

                //    if (comparedPoints % 5000000 == 0)
                //    {
                //        ShowUpdateMessage("{0}*** Compared {1:#,##0} points so far averaging {2:#,##0} points per second ***{0}", Environment.NewLine, comparedPoints, comparedPoints / (DateTime.UtcNow.Ticks - readStartTime).ToSeconds());
                //        updateProgress = true;
                //    }
                //    else if ((validPoints > 0 && validPoints % 1000000 == 0) || (invalidPoints > 0 && invalidPoints % 1000000 == 0) || (missingPoints > 0 && missingPoints % 1000000 == 0))
                //    {
                //        ShowUpdateMessage("{0}Found {1:#,##0} valid, {2:#,##0} invalid and {3:#,##0} missing points during compare so far...{0}" +
                //                          "     Value Errors: {4:#,##0}{0}" +
                //                          "      Flag Errors: {5:#,##0}{0}",
                //                          Environment.NewLine,
                //                          validPoints,
                //                          invalidPoints,
                //                          missingPoints,
                //                          valueErrors,
                //                          flagErrors);

                //        updateProgress = true;
                //    }

                //    if (updateProgress && m_pointCount > 0)
                //        UpdateProgressBar((int)((comparedPoints / (double)m_pointCount) * 100.0D));

                //    if (m_formClosing)
                //        break;
                //}

                //if (m_formClosing)
                //{
                //    ShowUpdateMessage("Compare canceled.");
                //    UpdateProgressBar(0);
                //}
                //else
                //{
                //    ShowUpdateMessage("*** Compare Complete ***");
                //    ShowUpdateMessage("Total compare time {0}", (DateTime.UtcNow.Ticks - operationStartTime).ToElapsedTimeString(3));
                //    UpdateProgressBar(100);
                //}

                //ShowUpdateMessage("{0}" +
                //    "Total points compared: {1:#,##0}{0}" +
                //    "         Valid points: {2:#,##0}{0}" +
                //    "       Invalid points: {3:#,##0}{0}" +
                //    "       Missing points: {4:#,##0}{0}" +
                //    "   Source point count: {5:#,##0}{0}" +
                //    "{0}Migrated data conversion {6:##0.000}% accurate",
                //    Environment.NewLine,
                //    comparedPoints,
                //    validPoints,
                //    invalidPoints,
                //    missingPoints,
                //    m_pointCount,
                //    Math.Truncate(validPoints / (double)(comparedPoints + missingPoints) * 100000.0D) / 1000.0D);
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("Failure during compare: {0}", ex.Message);
            }
            finally
            {
                m_operationStarted = false;
                //CloseGSFHistorianArchive();
                //CloseSnapDBEngine();
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
            GSF.Diagnostics.Logger.SetLoggingPath(FilePath.GetAbsolutePath(""));
        }
    }
}
