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
using System.Windows.Forms;
using GSF;
using GSF.Historian;
using GSF.Historian.Files;
using GSF.IO;

namespace MigrationUtility
{
    partial class MigrationUtility
    {
        private ArchiveReader m_archiveReader;
        private int m_maxPointID;

        private void OpenGSFHistorianArchive(string sourceFilesLocation, string sourceFilesOffloadLocation, bool reopen = false)
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
                        MetadataRecord definition;
                        m_maxPointID = -1;

                        for (int i = 1; i <= m_archiveReader.MetadataFile.RecordsOnDisk; i++)
                        {
                            definition = m_archiveReader.MetadataFile.Read(i);

                            if (definition.GeneralFlags.Enabled && definition.HistorianID > m_maxPointID)
                                m_maxPointID = definition.HistorianID;
                        }

                        string archiveName = FilePath.GetFileName(m_archiveReader.FileName);
                        textBoxInstanceName.Text = archiveName.Substring(0, archiveName.IndexOf("_"));
                        ShowUpdateMessage("[GSFHistorian] Archive reader opened for \"{0}\" historian.", textBoxInstanceName.Text);
                    }
                }
                catch (Exception ex)
                {
                    ShowUpdateMessage("[GSFHistorian] Error attempting to open archive: {0}", ex.Message);
                }
            }
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

        private void m_archiveReader_HistoricFileListBuildStart(object sender, EventArgs e)
        {
            ShowUpdateMessage("[GSFHistorian] Building list of historic archive files...");
            ShowUpdateMessage("{0}{0}Migration can begin when historic archive file list enumeration has completed.{0}", Environment.NewLine);
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
            ShowUpdateMessage("[GSFHistorian] Migration Paused: active archive rollover in progress...");
        }

        private void m_archiveReader_RolloverComplete(object sender, EventArgs e)
        {
            ShowUpdateMessage("[GSFHistorian] Migration Resumed: active archive rollover complete.");
        }
    }
}
