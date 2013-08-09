//******************************************************************************************************
//  StagingFile.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  1/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using openHistorian.Archive;
using openHistorian.Collections.Generic;

namespace openHistorian.Engine.ArchiveWriters
{
    /// <summary>
    /// A file contained within each stage writer that handles combining files and creating new ones.
    /// </summary>
    public class StagingFile<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        private ArchiveTable<TKey, TValue> m_archiveFile;
        private readonly ArchiveList<TKey, TValue> m_archiveList;
        private readonly ArchiveInitializer<TKey, TValue> m_initializer;

        public StagingFile(ArchiveList<TKey, TValue> archiveList, ArchiveInitializer<TKey, TValue> initializer)
        {
            m_initializer = initializer;
            m_archiveList = archiveList;
        }

        /// <summary>
        /// Gets if the staging file is backed by a physical file.
        /// </summary>
        public bool IsFileBacked
        {
            get
            {
                return m_initializer.IsFileBacked;
            }
        }

        public void Append(TreeStream<TKey, TValue> scan)
        {
            if (m_archiveFile == null)
            {
                m_archiveFile = m_initializer.CreateArchiveFile();
                using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
                {
                    //Add the newly created file.
                    edit.Add(m_archiveFile, true);
                }
            }
            using (ArchiveTable<TKey, TValue>.Editor editor = m_archiveFile.BeginEdit())
            {
                editor.AddPoints(scan);
                editor.Commit();
            }
            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
            {
                edit.RenewSnapshot(m_archiveFile);
            }
        }

        public void Combine(ArchiveTable<TKey, TValue> file)
        {
            if (m_archiveFile == null)
            {
                m_archiveFile = m_initializer.CreateArchiveFile();
                using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
                {
                    //Add the newly created file.
                    edit.Add(m_archiveFile, true);
                }
            }
            using (ArchiveTable<TKey, TValue>.Editor editor = m_archiveFile.BeginEdit())
            {
                using (ArchiveTableReadSnapshot<TKey, TValue> reader = file.BeginRead())
                {
                    TreeScannerBase<TKey, TValue> scan = reader.GetTreeScanner();
                    scan.SeekToStart();
                    editor.AddPoints(scan);
                    editor.Commit();
                }
            }
            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
            {
                edit.RenewSnapshot(m_archiveFile);
                edit.RemoveAndDelete(file);
            }
        }

        public ArchiveTable<TKey, TValue> GetFileAndSetNull()
        {
            if (m_archiveFile == null)
                return null;
            ArchiveTable<TKey, TValue> file = m_archiveFile;
            m_archiveFile = null;
            return file;
        }

        /// <summary>
        /// Gets the current size of the archive file.
        /// </summary>
        public long Size
        {
            get
            {
                if (m_archiveFile == null)
                    return 0;
                return m_archiveFile.BaseFile.FileSize;
            }
        }
    }
}