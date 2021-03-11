//******************************************************************************************************
//  MigrationUtility.GSFHistorianReader.Designer.cs - Gbtc
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
using System.Threading;
using GSF;
using GSF.Historian;
using GSF.Historian.Files;
using GSF.IO;
using GSF.Snap.Services;

namespace ComparisonUtility
{
    // GSF Historian Engine Code
    partial class MigrationUtility
    {
        private static Dictionary<string, int> s_maximumPointID = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private ArchiveReader m_archiveReader;
        private IEnumerator<IDataPoint> m_enumerator;
        private string m_operationName;
        private int m_maxPointID;

        private string OpenGSFHistorianArchive(string sourceFilesLocation, string sourceFilesOffloadLocation, string instanceName, bool reopen = false, string operationName = "Migration")
        {
            if (!string.IsNullOrEmpty(operationName))
                m_operationName = operationName;

            if ((object)m_archiveReader is null)
            {
                m_archiveReader = new ArchiveReader();
                m_archiveReader.RolloverStart += m_archiveReader_RolloverStart;
                m_archiveReader.RolloverComplete += m_archiveReader_RolloverComplete;
                m_archiveReader.HistoricFileListBuildStart += m_archiveReader_HistoricFileListBuildStart;
                m_archiveReader.HistoricFileListBuildComplete += m_archiveReader_HistoricFileListBuildComplete;
                m_archiveReader.HistoricFileListBuildException += m_archiveReader_HistoricFileListBuildException;
                m_archiveReader.DataReadException += m_archiveReader_DataReadException;
            }

            if (!string.IsNullOrEmpty(sourceFilesLocation) && Directory.Exists(sourceFilesLocation) && (reopen || m_archiveReader.StateFile is null || !m_archiveReader.StateFile.IsOpen))
            {
                // Specified directory is a valid one.
                try
                {
                    m_archiveReady.Reset();
                    string[] matches = Directory.GetFiles(sourceFilesLocation, "*_archive.d");

                    // Open the active archive
                    if (matches.Length > 0)
                    {
                        m_archiveReader.Open(matches[0], sourceFilesOffloadLocation);
                        m_enumerator = null;

                        // Find maximum point ID
                        m_maxPointID = FindMaximumPointID(m_archiveReader.MetadataFile);

                        string archiveName = FilePath.GetFileName(m_archiveReader.FileName);
                        instanceName = archiveName.Substring(0, archiveName.IndexOf("_"));

                        ShowUpdateMessage("[GSFHistorian] Archive reader opened for \"{0}\" historian.", instanceName);

                        // Start calculating total number of source points
                        m_pointCount = 0;
                        ThreadPool.QueueUserWorkItem(CalculateSourcePointCount, new[] { sourceFilesLocation, sourceFilesOffloadLocation });
                    }
                }
                catch (Exception ex)
                {
                    ShowUpdateMessage("[GSFHistorian] Error attempting to open archive: {0}", ex.Message);
                }
            }

            return instanceName;
        }

        private void CloseGSFHistorianArchive()
        {
            if ((object)m_archiveReader != null)
            {
                m_archiveReader.RolloverStart -= m_archiveReader_RolloverStart;
                m_archiveReader.RolloverComplete -= m_archiveReader_RolloverComplete;
                m_archiveReader.HistoricFileListBuildStart -= m_archiveReader_HistoricFileListBuildStart;
                m_archiveReader.HistoricFileListBuildComplete -= m_archiveReader_HistoricFileListBuildComplete;
                m_archiveReader.HistoricFileListBuildException -= m_archiveReader_HistoricFileListBuildException;
                m_archiveReader.DataReadException -= m_archiveReader_DataReadException;
                m_archiveReader.Dispose();
                m_archiveReader = null;
            }

            m_enumerator = null;

            ShowUpdateMessage("[GSFHistorian] Archive reader closed.");
        }

        private bool ReadNextGSFHistorianPoint(DataPoint point)
        {
            if ((object)m_enumerator is null)
            {
                // We want data for all possible point IDs
                IEnumerable<int> historianIDs = Enumerable.Range(1, m_maxPointID);
                m_enumerator = m_archiveReader.ReadData(historianIDs).GetEnumerator();
            }

            if (m_enumerator.MoveNext())
            {
                IDataPoint archivePoint = m_enumerator.Current;

                point.Timestamp = (ulong)archivePoint.Time.ToDateTime().Ticks;
                point.PointID = (ulong)archivePoint.HistorianID;
                point.ValueAsSingle = archivePoint.Value;
                point.Flags = (ulong)archivePoint.Quality;

                return true;
            }

            return false;
        }

        private string GetDestinationFileName(string sourceFileName, string instanceName, string destinationPath, ArchiveDirectoryMethod method)
        {
            using (ArchiveFile file = OpenArchiveFile(sourceFileName, ref instanceName))
            {
                return GetDestinationFileName(file, sourceFileName, instanceName, destinationPath, method);
            }
        }

        private string GetDestinationFileName(ArchiveFile file, string sourceFileName, string instanceName, string destinationPath, ArchiveDirectoryMethod method)
        {
            string destinationFileName = FilePath.GetFileNameWithoutExtension(sourceFileName) + ".d2";
            string archiveFileName = FilePath.GetFileName(sourceFileName);
            string archiveInstanceName = archiveFileName.Substring(0, archiveFileName.LastIndexOf("_archive", StringComparison.OrdinalIgnoreCase));
            DateTime startTime, endTime;

            // Use source archive instance name for destination database instance name if not specified
            if (string.IsNullOrEmpty(instanceName))
                instanceName = archiveInstanceName;

            startTime = file.Fat.FileStartTime.ToDateTime();
            endTime = file.Fat.FileEndTime.ToDateTime();
            destinationFileName = instanceName.ToLower() + "-" + startTime.ToString("yyyy-MM-dd HH.mm.ss.fff") + "_to_" + endTime.ToString("yyyy-MM-dd HH.mm.ss.fff") + "-" + DateTime.UtcNow.Ticks + ".d2";

            switch (method)
            {
                case ArchiveDirectoryMethod.Year:
                    destinationPath = Path.Combine(destinationPath, string.Format("{0}\\", startTime.Year));
                    break;
                case ArchiveDirectoryMethod.YearMonth:
                    destinationPath = Path.Combine(destinationPath, string.Format("{0}{1:00}\\", startTime.Year, startTime.Month));
                    break;
                case ArchiveDirectoryMethod.YearThenMonth:
                    destinationPath = Path.Combine(destinationPath, string.Format("{0}\\{1:00}\\", startTime.Year, startTime.Month));
                    break;
            }

            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            return Path.Combine(destinationPath, destinationFileName);
        }

        private static ArchiveFile OpenArchiveFile(string sourceFileName, ref string instanceName)
        {
            const string MetadataFileName = "{0}{1}_dbase.dat";
            const string MetadataFile2Name = "{0}{1}_dbase.dat2";
            const string StateFileName = "{0}{1}_startup.dat";
            const string IntercomFileName = "{0}scratch.dat";

            ArchiveFile file;
            string archiveLocation = FilePath.GetDirectoryName(sourceFileName);
            string archiveFileName = FilePath.GetFileName(sourceFileName);
            string archiveInstanceName = archiveFileName.Substring(0, archiveFileName.LastIndexOf("_archive", StringComparison.OrdinalIgnoreCase));

            // Use source archive instance name for destination database instance name if not specified
            if (string.IsNullOrEmpty(instanceName))
                instanceName = archiveInstanceName;

            // See if new file format exists
            MetadataFileLegacyMode legacyMode;

            string metadatFileName = string.Format(MetadataFile2Name, archiveLocation, archiveInstanceName);

            if (File.Exists(metadatFileName))
            {
                legacyMode = MetadataFileLegacyMode.Disabled;
            }
            else
            {
                metadatFileName = string.Format(MetadataFileName, archiveLocation, archiveInstanceName);
                legacyMode = MetadataFileLegacyMode.Enabled;
            }

            file = new ArchiveFile
            {
                FileName = sourceFileName,
                FileAccessMode = FileAccess.Read,
                MonitorNewArchiveFiles = false,
                PersistSettings = false,
                StateFile = new StateFile
                {
                    FileAccessMode = FileAccess.Read,
                    FileName = string.Format(StateFileName, archiveLocation, archiveInstanceName)
                },
                IntercomFile = new IntercomFile
                {
                    FileAccessMode = FileAccess.Read,
                    FileName = string.Format(IntercomFileName, archiveLocation)
                },
                MetadataFile = new MetadataFile
                {
                    FileAccessMode = FileAccess.Read,
                    LegacyMode = legacyMode,
                    FileName = metadatFileName,
                    LoadOnOpen = true
                }
            };

            file.Open();

            return file;
        }

        private static int FindMaximumPointID(MetadataFile metadata)
        {
            lock (s_maximumPointID)
            {
                int maxPointID;

                if (s_maximumPointID.TryGetValue(metadata.FileName, out maxPointID))
                    return maxPointID;

                MetadataRecord definition;

                for (int i = 1; i <= metadata.RecordsOnDisk; i++)
                {
                    definition = metadata.Read(i);

                    if (definition.GeneralFlags.Enabled && definition.HistorianID > maxPointID)
                        maxPointID = definition.HistorianID;
                }

                s_maximumPointID.Add(metadata.FileName, maxPointID);

                return maxPointID;
            }
        }

        private void m_archiveReader_HistoricFileListBuildStart(object sender, EventArgs e)
        {
            ShowUpdateMessage("[GSFHistorian] Building list of historic archive files...");
            ShowUpdateMessage("{0}{0}{1} will begin when historic archive file list enumeration has completed.{0}{0}", Environment.NewLine, m_operationName);
        }

        private void m_archiveReader_HistoricFileListBuildComplete(object sender, EventArgs e)
        {
            ShowUpdateMessage("[GSFHistorian] Completed building list of historic archive files.");
            m_archiveReady.Set();
        }

        private void m_archiveReader_HistoricFileListBuildException(object sender, EventArgs<Exception> e)
        {
            ShowUpdateMessage("[GSFHistorian] {0}", e.Argument.Message);
        }

        private void m_archiveReader_DataReadException(object sender, EventArgs<Exception> e)
        {
            ShowUpdateMessage("[GSFHistorian] Exception encountered during data read: {0}", e.Argument.Message);
        }

        private void m_archiveReader_RolloverStart(object sender, EventArgs e)
        {
            ShowUpdateMessage("[GSFHistorian] {0} Paused: active archive rollover in progress...", m_operationName);
        }

        private void m_archiveReader_RolloverComplete(object sender, EventArgs e)
        {
            ShowUpdateMessage("[GSFHistorian] {0} Resumed: active archive rollover complete.", m_operationName);
        }

        private void CalculateSourcePointCount(object state)
        {
            try
            {
                string[] parameters = state as string[];

                if ((object)parameters is null || parameters.Length != 2)
                    return;

                if (!Directory.Exists(parameters[0]))
                    return;

                IEnumerable<string> sourceFileNames = Directory.EnumerateFiles(parameters[0], "*.d", SearchOption.TopDirectoryOnly);

                if (Directory.Exists(parameters[1]))
                    sourceFileNames = sourceFileNames.Concat(Directory.EnumerateFiles(parameters[1], "*.d", SearchOption.TopDirectoryOnly));

                long pointCount = 0;
                string instanceName = null;

                foreach (string sourceFileName in sourceFileNames)
                {
                    using (ArchiveFile file = OpenArchiveFile(sourceFileName, ref instanceName))
                    {
                        pointCount += file.Fat.DataPointsArchived;
                    }
                }

                m_pointCount = pointCount;
            }
            catch (Exception ex)
            {
                ShowUpdateMessage("[GSFHistorian] Exception encountered while attempting to calculate point count: {0}", ex.Message);
            }
        }
    }
}
