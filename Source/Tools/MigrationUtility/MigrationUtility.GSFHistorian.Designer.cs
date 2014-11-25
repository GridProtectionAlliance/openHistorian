//******************************************************************************************************
//  MigrationUtility.GSFHistorianReader.Designer.cs - Gbtc
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
using GSF;
using GSF.Historian;
using GSF.Historian.Files;
using GSF.IO;
using GSF.Snap.Services;

namespace MigrationUtility
{
    // GSF Historian Engine Code
    partial class MigrationUtility
    {
        private ArchiveReader m_archiveReader;
        private string m_operationName;
        private int m_maxPointID;

        private string OpenGSFHistorianArchive(string sourceFilesLocation, string sourceFilesOffloadLocation, string instanceName, bool reopen = false, string operationName = "Migration")
        {
            if ((object)m_archiveReader == null)
            {
                m_archiveReader = new ArchiveReader();
                m_archiveReader.RolloverStart += m_archiveReader_RolloverStart;
                m_archiveReader.RolloverComplete += m_archiveReader_RolloverComplete;
                m_archiveReader.HistoricFileListBuildStart += m_archiveReader_HistoricFileListBuildStart;
                m_archiveReader.HistoricFileListBuildComplete += m_archiveReader_HistoricFileListBuildComplete;
                m_archiveReader.HistoricFileListBuildException += m_archiveReader_HistoricFileListBuildException;
                m_archiveReader.DataReadException += m_archiveReader_DataReadException;
            }

            if (!string.IsNullOrEmpty(sourceFilesLocation) && Directory.Exists(sourceFilesLocation) && (reopen || m_archiveReader.StateFile == null || !m_archiveReader.StateFile.IsOpen))
            {
                // Specified directory is a valid one.
                try
                {
                    string[] matches = Directory.GetFiles(sourceFilesLocation, "*_archive.d");

                    // Open the active archive
                    if (matches.Length > 0)
                    {
                        m_archiveReader.Open(matches[0], sourceFilesOffloadLocation);

                        // Find maximum point ID
                        m_maxPointID = FindMaximumPointID(m_archiveReader.MetadataFile);

                        string archiveName = FilePath.GetFileName(m_archiveReader.FileName);
                        instanceName = archiveName.Substring(0, archiveName.IndexOf("_"));

                        ShowUpdateMessage("[GSFHistorian] Archive reader opened for \"{0}\" historian.", instanceName);
                    }
                }
                catch (Exception ex)
                {
                    ShowUpdateMessage("[GSFHistorian] Error attempting to open archive: {0}", ex.Message);
                }
            }

            m_operationName = operationName;

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

            ShowUpdateMessage("[GSFHistorian] Archive reader closed.");
        }

        private IEnumerable<IDataPoint> ReadGSFHistorianData()
        {
            // We want data for all possible point IDs
            IEnumerable<int> historianIDs = Enumerable.Range(1, m_maxPointID);

            foreach (IDataPoint point in m_archiveReader.ReadData(historianIDs, false))
                yield return point;
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
            destinationFileName = DateTime.Now.Ticks.ToString() + "-" + instanceName + "-" + startTime.ToString("yyyy-MM-dd HH.mm.ss.fff") + "_to_" + endTime.ToString("yyyy-MM-dd HH.mm.ss.fff") + ".d2";

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

        private static ArchiveFile OpenArchiveFile(string sourceFileName, string instanceName)
        {
            const string MetadataFileName = "{0}{1}_dbase.dat";
            const string StateFileName = "{0}{1}_startup.dat";
            const string IntercomFileName = "{0}scratch.dat";

            ArchiveFile file;
            string archiveLocation = FilePath.GetDirectoryName(sourceFileName);
            string archiveFileName = FilePath.GetFileName(sourceFileName);
            string archiveInstanceName = archiveFileName.Substring(0, archiveFileName.LastIndexOf("_archive", StringComparison.OrdinalIgnoreCase));

            // Use source archive instance name for destination database instance name if not specified
            if (string.IsNullOrEmpty(instanceName))
                instanceName = archiveInstanceName;

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
                    FileName = string.Format(MetadataFileName, archiveLocation, archiveInstanceName),
                }
            };

            file.Open();

            return file;
        }

        private static int FindMaximumPointID(MetadataFile metadata)
        {
            MetadataRecord definition;
            int maxPointID = -1;

            for (int i = 1; i <= metadata.RecordsOnDisk; i++)
            {
                definition = metadata.Read(i);

                if (definition.GeneralFlags.Enabled && definition.HistorianID > maxPointID)
                    maxPointID = definition.HistorianID;
            }

            return maxPointID;
        }

        private void m_archiveReader_HistoricFileListBuildStart(object sender, EventArgs e)
        {
            ShowUpdateMessage("[GSFHistorian] Building list of historic archive files...");
            ShowUpdateMessage("{0}{0}{1} can begin when historic archive file list enumeration has completed.{0}", Environment.NewLine, m_operationName);
        }

        private void m_archiveReader_HistoricFileListBuildComplete(object sender, EventArgs e)
        {
            ShowUpdateMessage("[GSFHistorian] Completed building list of historic archive files.");
            EnableGoButton(true);
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
    }
}
