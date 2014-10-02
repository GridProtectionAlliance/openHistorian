//******************************************************************************************************
//  ServerDatabase`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using GSF.Diagnostics;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Services.Reader;
using GSF.SortedTreeStore.Services.Writer;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;
using GSF.Threading;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Creates an engine for reading/writing data from a SortedTreeStore.
    /// </summary>
    public partial class ServerDatabase<TKey, TValue>
         : ServerDatabaseBase
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        #region [ Members ]

        // Fields
        //private readonly List<ArchiveListRemovalStatus<TKey, TValue>> m_pendingDispose;
        private DatabaseInfo m_info;
        private TKey m_tmpKey;
        private TValue m_tmpValue;
        private WriteProcessor<TKey, TValue> m_archiveWriter;
        private ArchiveList<TKey, TValue> m_archiveList;
        private volatile bool m_disposed;
        private string m_databaseName;
        private List<EncodingDefinition> m_supportedStreamingMethods;

        private string m_intermediateFilePendingExtension;
        private string m_intermediateFileFinalExtension;
        private string m_finalFilePendingExtension;
        private string m_finalFileFinalExtension;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an engine for reading/writing data from a SortedTreeStore.
        /// </summary>
        /// <param name="databaseConfig">the config to use for the database.</param>
        /// <param name="parent">The parent of this log.</param>
        public ServerDatabase(ServerDatabaseConfig databaseConfig, LogSource parent)
            : base(parent)
        {
            if (databaseConfig.DatabaseName == null)
                throw new ArgumentNullException("databaseName");

            switch (databaseConfig.WriterMode)
            {
                case WriterMode.None:
                case WriterMode.InMemory:
                case WriterMode.OnDisk:
                    break;
                default:
                    throw new InvalidEnumArgumentException("writer", (int)databaseConfig.WriterMode, typeof(WriterMode));
            }

            if (databaseConfig.MainPath == null)
                throw new ArgumentNullException("mainPath");

            if (!Directory.Exists(databaseConfig.MainPath))
            {
                Log.Publish(VerboseLevel.Critical, "Missing main directory", "Directory is missing: " + databaseConfig.MainPath, "Without this directory the database will completely not function");
                throw new ArgumentException("Not an existing directory", "mainPath");
            }

            ValidateExtension(databaseConfig.IntermediateFileExtension, out m_intermediateFilePendingExtension, out m_intermediateFileFinalExtension);
            ValidateExtension(databaseConfig.FinalFileExtension, out m_finalFilePendingExtension, out m_finalFileFinalExtension);

            m_tmpKey = new TKey();
            m_tmpValue = new TValue();
            m_databaseName = databaseConfig.DatabaseName;
            m_archiveList = new ArchiveList<TKey, TValue>(Log);
            m_supportedStreamingMethods = databaseConfig.StreamingEncodingMethods.ToList();

            if (databaseConfig.WriterMode == WriterMode.InMemory)
            {
                var settings = new WriteProcessorSettings();
                settings.StagingFile.Encoding = databaseConfig.ArchiveEncodingMethod;
                settings.StagingFile.IsMemoryArchive = true;
                settings.StagingFile.PendingFileExtension = m_intermediateFilePendingExtension;
                settings.StagingFile.CommittedFileExtension = m_intermediateFileFinalExtension;
                
                settings.Stage1Rollover.ArchiveSettings = ArchiveInitializerSettings.CreateInMemory(databaseConfig.ArchiveEncodingMethod, FileFlags.Stage2);
                settings.Stage1Rollover.FinalFileExtension = m_intermediateFileFinalExtension;
                settings.Stage1Rollover.LogPath = databaseConfig.MainPath;
                settings.Stage1Rollover.MaxFileCount = 100;
                settings.Stage1Rollover.TargetFileSize = 100 * 1024 * 1024;
                settings.Stage1Rollover.MatchFlag = FileFlags.Stage1;

                settings.Stage2Rollover.ArchiveSettings = ArchiveInitializerSettings.CreateInMemory(databaseConfig.ArchiveEncodingMethod, FileFlags.Stage3);
                settings.Stage2Rollover.FinalFileExtension = m_finalFileFinalExtension;
                settings.Stage2Rollover.LogPath = databaseConfig.MainPath;
                settings.Stage2Rollover.MaxFileCount = 100;
                settings.Stage2Rollover.TargetFileSize = 1000 * 1024 * 1024;
                settings.Stage2Rollover.MatchFlag = FileFlags.Stage2;

                m_archiveWriter = new WriteProcessor<TKey, TValue>(Log, m_archiveList, settings);
            }
            else if (databaseConfig.WriterMode == WriterMode.OnDisk)
            {
                var settings = new WriteProcessorSettings();
                settings.StagingFile.Encoding = databaseConfig.ArchiveEncodingMethod;
                settings.StagingFile.SavePath = databaseConfig.MainPath;
                settings.StagingFile.IsMemoryArchive = false;
                settings.StagingFile.PendingFileExtension = m_intermediateFilePendingExtension;
                settings.StagingFile.CommittedFileExtension = m_intermediateFileFinalExtension;

                settings.Stage1Rollover.ArchiveSettings = ArchiveInitializerSettings.CreateOnDisk(databaseConfig.MainPath, databaseConfig.ArchiveEncodingMethod, "stage2-", m_intermediateFilePendingExtension, FileFlags.Stage2);
                settings.Stage1Rollover.FinalFileExtension = m_intermediateFileFinalExtension;
                settings.Stage1Rollover.LogPath = databaseConfig.MainPath;
                settings.Stage1Rollover.MaxFileCount = 5;
                settings.Stage1Rollover.TargetFileSize = 100 * 1024 * 1024;
                settings.Stage1Rollover.MatchFlag = FileFlags.Stage1;

                settings.Stage2Rollover.ArchiveSettings = ArchiveInitializerSettings.CreateOnDisk(databaseConfig.MainPath, databaseConfig.ArchiveEncodingMethod, "stage3-", m_finalFilePendingExtension, FileFlags.Stage3);
                settings.Stage2Rollover.FinalFileExtension = m_finalFileFinalExtension;
                settings.Stage2Rollover.LogPath = databaseConfig.MainPath;
                settings.Stage2Rollover.MaxFileCount = 5;
                settings.Stage2Rollover.TargetFileSize = 1000 * 1024 * 1024;
                settings.Stage2Rollover.MatchFlag = FileFlags.Stage2;
                
                m_archiveWriter = new WriteProcessor<TKey, TValue>(Log, m_archiveList, settings);
            }

            AttachFilesOrPaths(new String[] { databaseConfig.MainPath });
            AttachFilesOrPaths(databaseConfig.ImportPaths);
            AttachFilesOrPaths(databaseConfig.FinalWritePaths);
        }


        /// <summary>
        /// Loads the provided files from all of the specified paths.
        /// </summary>
        /// <param name="paths">all of the paths of archive files to attach. These can either be a path, or an individual file name.</param>
        private void AttachFilesOrPaths(IEnumerable<string> paths)
        {
            m_archiveList.LoadFiles(GetAttachedFiles(paths));
        }

        /// <summary>
        /// Converts a list of file or paths to a list of all files.
        /// </summary>
        /// <param name="paths">the path to file names or directories to enumerate.</param>
        /// <returns></returns>
        private IEnumerable<string> GetAttachedFiles(IEnumerable<string> paths)
        {
            var attachedFiles = new List<string>();
            foreach (string path in paths)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        attachedFiles.Add(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        attachedFiles.AddRange(Directory.GetFiles(path, "*" + m_finalFileFinalExtension, SearchOption.TopDirectoryOnly));
                        if (!m_finalFileFinalExtension.Equals(m_intermediateFileFinalExtension, StringComparison.OrdinalIgnoreCase))
                            attachedFiles.AddRange(Directory.GetFiles(path, "*" + m_intermediateFileFinalExtension, SearchOption.TopDirectoryOnly));
                    }
                    else
                    {
                        Log.Publish(VerboseLevel.Warning, "File or path does not exist", path);
                    }

                }
                catch (Exception ex)
                {
                    Log.Publish(VerboseLevel.Error, "Unknown error occured while attaching paths", "Path: " + path, null, ex);
                }

            }
            return attachedFiles;
        }


        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets status information for the SortedTreeEngine
        /// </summary>
        /// <param name="status">where to append the status log.</param>
        public override void GetFullStatus(StringBuilder status)
        {
            m_archiveList.GetFullStatus(status);
        }

        private List<ArchiveDetails> GetAllAttachedFiles()
        {
            return m_archiveList.GetAllAttachedFiles();
        }

        private void DetatchFiles(List<Guid> files)
        {
            using (var editor = m_archiveList.AcquireEditLock())
            {
                foreach (var file in files)
                {
                    ArchiveListRemovalStatus<TKey, TValue> removalStatus;
                    editor.TryRemove(file, out removalStatus);
                }
            }
        }

        private void DeleteFiles(List<Guid> files)
        {
            using (var editor = m_archiveList.AcquireEditLock())
            {
                foreach (var file in files)
                {
                    editor.TryRemoveAndDelete(file);
                }
            }
        }


        /// <summary>
        /// Gets basic information about the current Database.
        /// </summary>
        public override DatabaseInfo Info
        {
            get
            {
                if (m_info == null)
                {
                    m_info = new DatabaseInfo(m_databaseName, m_tmpKey, m_tmpValue, m_supportedStreamingMethods);
                }
                return m_info;
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ServerDatabase{TKey,TValue}"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        m_disposed = true;
                        if (m_archiveWriter != null)
                            m_archiveWriter.Dispose();

                        m_archiveList.Dispose();

                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }


        /// <summary>
        /// Forces a soft commit on the database. A soft commit 
        /// only commits data to memory. This allows other clients to read the data.
        /// While soft committed, this data could be lost during an unexpected shutdown.
        /// Soft commits usually occur within microseconds. 
        /// </summary>
        private void SoftCommit()
        {
            //m_archiveWriter.SoftCommit();
        }

        /// <summary>
        /// Forces a commit to the disk subsystem. Once this returns, the data will not
        /// be lost due to an application crash or unexpected shutdown.
        /// Hard commits can take 100ms or longer depending on how much data has to be committed. 
        /// This requires two consecutive hardware cache flushes.
        /// </summary>
        private void HardCommit()
        {
            //m_archiveWriter.HardCommit();
        }

        private void Write(TKey key, TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (m_archiveWriter == null)
                throw new Exception("Writing is not configured on this historian");

            m_archiveWriter.Write(key, value);
        }

        private void Write(TreeStream<TKey, TValue> stream)
        {
            TKey key = new TKey();
            TValue value = new TValue();
            //ToDo: Prebuffer the points in the stream. It is possible that this call may be behind a slow socket interface, therefore it will lockup the writing speed.
            while (stream.Read(key, value))
                Write(key, value);
        }

        private SequentialReaderStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions,
            SeekFilterBase<TKey> keySeekFilter,
            MatchFilterBase<TKey, TValue> keyMatchFilter,
            WorkerThreadSynchronization workerThreadSynchronization)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Stats.QueriesExecuted++;
            return new SequentialReaderStream<TKey, TValue>(m_archiveList, readerOptions, keySeekFilter, keyMatchFilter, workerThreadSynchronization);
        }

        /// <summary>
        /// Creates a <see cref="ClientDatabase"/>
        /// </summary>
        /// <returns></returns>
        public override ClientDatabaseBase CreateClientDatabase(Client client, Action<ClientDatabaseBase> onDispose)
        {
            return new ClientDatabase(this, client, onDispose);
        }

        #endregion

        private static void ValidateExtension(string extension, out string pending, out string final)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Cannot be null or whitespace", "extension");
            extension = extension.Trim();
            if (extension.Contains('.'))
            {
                extension = extension.Substring(extension.IndexOf('.') + 1);
            }
            pending = ".~" + extension;
            final = "." + extension;
        }



    }
}