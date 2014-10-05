//******************************************************************************************************
//  IncrementalStagingFile`2.cs - Gbtc
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
//  02/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// A helper class for <see cref="FirstStageWriter{TKey,TValue}"/> that creates in memory files
    /// that can be incrementally added to until they are dumped to the disk as a compressed file.
    /// </summary>
    /// <typeparam name="TKey">The key</typeparam>
    /// <typeparam name="TValue">The value</typeparam>
    public class IncrementalStagingFile<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private SortedTreeTable<TKey, TValue> m_sortedTreeFile;
        private readonly ArchiveList<TKey, TValue> m_archiveList;
        private readonly ArchiveInitializer<TKey, TValue> m_initialFile;
        private readonly ArchiveInitializer<TKey, TValue> m_finalFile;
        private IncrementalStagingFileSettings m_settings;

        /// <summary>
        /// Creates an <see cref="IncrementalStagingFile{TKey,TValue}"/> that will save to memory upon completion.
        /// </summary>
        /// <param name="list">the list to create new archives on.</param>
        /// <param name="settings">The settings</param>
        /// <returns></returns>
        public IncrementalStagingFile(IncrementalStagingFileSettings settings, ArchiveList<TKey, TValue> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (settings == null)
                throw new ArgumentNullException("settings");

            m_settings = settings.Clone();
            m_archiveList = list;
            m_initialFile = new ArchiveInitializer<TKey, TValue>(m_settings.InitialSettings);
            m_finalFile = new ArchiveInitializer<TKey, TValue>(m_settings.FinalSettings);
        }

        /// <summary>
        /// Gets the current size of the archive file.
        /// </summary>
        public long Size
        {
            get
            {
                if (m_sortedTreeFile == null)
                    return 0;
                return m_sortedTreeFile.BaseFile.ArchiveSize;
            }
        }

        /// <summary>
        /// Appends the entire contents of this stream to the existing archive stage.
        /// </summary>
        /// <param name="stream">the stream to read</param>
        public void Append(TreeStream<TKey, TValue> stream)
        {
            if (m_sortedTreeFile == null)
            {
                m_sortedTreeFile = m_initialFile.CreateArchiveFile();
                using (ArchiveListEditor<TKey, TValue> edit = m_archiveList.AcquireEditLock())
                {
                    //Add the newly created file.
                    edit.Add(m_sortedTreeFile);
                }
            }
            using (var editor = m_sortedTreeFile.BeginEdit())
            {
                editor.AddPoints(stream);
                editor.Commit();
            }
            using (ArchiveListEditor<TKey, TValue> edit = m_archiveList.AcquireEditLock())
            {
                edit.RenewArchiveSnapshot(m_sortedTreeFile.ArchiveId);
            }
        }

        /// <summary>
        /// Dumps all of the current data to the disk.
        /// </summary>
        public void DumpToDisk()
        {
            if (m_sortedTreeFile == null)
                return;

            var newFile = m_finalFile.CreateArchiveFile(m_sortedTreeFile.FirstKey, m_sortedTreeFile.LastKey);
            using (var editor = newFile.BeginEdit())
            using (var reader = m_sortedTreeFile.BeginRead())
            {
                var scan = reader.GetTreeScanner();
                scan.SeekToStart();
                editor.AddPoints(scan);
                editor.Commit();
            }

            newFile.BaseFile.ChangeExtension(m_settings.FinalFileExtension, true, true);

            using (ArchiveListEditor<TKey, TValue> edit = m_archiveList.AcquireEditLock())
            {
                edit.Add(newFile);
                edit.TryRemoveAndDelete(m_sortedTreeFile.ArchiveId);
            }

            m_sortedTreeFile = null;
        }
    }
}