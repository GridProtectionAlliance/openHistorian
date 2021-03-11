//******************************************************************************************************
//  ArchiveListLog.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  10/02/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using GSF.Diagnostics;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Contains the Pending Deletions for the <see cref="ArchiveList{TKey,TValue}"/>.
    /// This class is thread safe.
    /// </summary>
    internal class ArchiveListLog
           : DisposableLoggingClassBase
    {
        private readonly List<ArchiveListLogFile> m_files = new List<ArchiveListLogFile>();
        private HashSet<Guid> m_allFilesToDelete = new HashSet<Guid>();

        private readonly object m_syncRoot;
        private bool m_disposed;
        private readonly ArchiveListLogSettings m_settings;

        private ArchiveListLogFile m_pendingFile = new ArchiveListLogFile();

        /// <summary>
        /// Creates a log that monitors pending deletions.
        /// </summary>
        /// <param name="settings">Optional settings for the log. If none are specified, the default will not load the settings.</param>
        public ArchiveListLog(ArchiveListLogSettings settings = null)
                : base(MessageClass.Framework)
        {
            if (settings is null)
                settings = new ArchiveListLogSettings();

            m_settings = settings.CloneReadonly();
            m_settings.Validate();

            m_syncRoot = new object();
            m_pendingFile = new ArchiveListLogFile();

            if (m_settings.IsFileBacked)
            {
                foreach (string file in Directory.GetFiles(m_settings.LogPath, m_settings.SearchPattern))
                {
                    ArchiveListLogFile logFile = new ArchiveListLogFile();
                    logFile.Load(file);
                    if (logFile.IsValid)
                    {
                        m_files.Add(logFile);
                    }
                }
            }

            m_allFilesToDelete = new HashSet<Guid>(GetAllFilesToDelete());
        }

        /// <summary>
        /// If the log is file backed
        /// </summary>
        public void SaveLogToDisk()
        {
            if (!m_settings.IsFileBacked)
                return;

            lock (m_syncRoot)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (m_pendingFile.FilesToDelete.Count > 0)
                {
                    string file = m_settings.GenerateNewFileName();
                    m_pendingFile.Save(file);
                    m_files.Add(m_pendingFile);
                    m_pendingFile = new ArchiveListLogFile();
                }
            }
        }

        /// <summary>
        /// Removes any log that is no longer valid from this list.
        /// </summary>
        public void ClearCompletedLogs(HashSet<Guid> allFiles)
        {
            lock (m_syncRoot)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_allFilesToDelete = null;
                m_pendingFile.RemoveDeletedFiles(allFiles);
                for (int x = m_files.Count - 1; x >= 0; x--)
                {
                    m_files[x].RemoveDeletedFiles(allFiles);
                    if (m_files[x].FilesToDelete.Count == 0)
                    {
                        m_files[x].Delete();
                        m_files.RemoveAt(x);
                    }
                }
            }
        }

        /// <summary>
        /// Appends the specified file to the list of files that should be deleted.
        /// </summary>
        /// <param name="archiveId"></param>
        public void AddFileToDelete(Guid archiveId)
        {
            if (!m_settings.IsFileBacked)
                return;
            lock (m_syncRoot)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                m_pendingFile.FilesToDelete.Add(archiveId);
                if (m_allFilesToDelete != null)
                    m_allFilesToDelete.Add(archiveId);
            }

        }

        /// <summary>
        /// Gets if the specified file Id should be deleted based on the delete log.
        /// </summary>
        /// <param name="fileId">the id of the file.</param>
        /// <returns>true if the file should be deleted. False otherwise.</returns>
        public bool ShouldBeDeleted(Guid fileId)
        {
            lock (m_syncRoot)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (m_allFilesToDelete is null)
                {
                    m_allFilesToDelete = GetAllFilesToDelete();
                }

                return m_allFilesToDelete.Contains(fileId);
            }
        }

        /// <summary>
        /// Verify that none of the pending deletion files exist in the editor.
        /// </summary>
        private HashSet<Guid> GetAllFilesToDelete()
        {
            HashSet<Guid> allFiles = new HashSet<Guid>();
            if (m_pendingFile.IsValid)
            {
                allFiles.UnionWith(m_pendingFile.FilesToDelete);
            }
            foreach (ArchiveListLogFile file in m_files)
            {
                if (file.IsValid)
                {
                    allFiles.UnionWith(file.FilesToDelete);
                }
            }
            return allFiles;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ArchiveListLog"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            lock (m_syncRoot)
            {
                if (!m_disposed)
                {
                    try
                    {
                        // This will be done regardless of whether the object is finalized or disposed.

                        if (disposing)
                        {
                            SaveLogToDisk();
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
        }

    }
}
