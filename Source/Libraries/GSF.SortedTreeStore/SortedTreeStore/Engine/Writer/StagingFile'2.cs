//******************************************************************************************************
//  StagingFile`2.cs - Gbtc
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

using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{
    /// <summary>
    /// A file contained within each stage writer that handles combining files and creating new ones.
    /// This class is not thread safe. All calls must be coordinated.
    /// </summary>
    public class StagingFile<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        private SortedTreeTable<TKey, TValue> m_sortedTreeFile;
        private readonly ArchiveList<TKey, TValue> m_archiveList;
        private readonly ArchiveInitializer<TKey, TValue> m_initializer;

        /// <summary>
        /// Constructs a staging file
        /// </summary>
        /// <param name="archiveList">the place to store the archive files when converted</param>
        /// <param name="initializer">the initializer that will create the archive files</param>
        public StagingFile(ArchiveList<TKey, TValue> archiveList, ArchiveInitializer<TKey, TValue> initializer)
        {
            m_initializer = initializer;
            m_archiveList = archiveList;
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
                return m_sortedTreeFile.BaseFile.FileSize;
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
                m_sortedTreeFile = m_initializer.CreateArchiveFile();
                using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
                {
                    //Add the newly created file.
                    edit.Add(m_sortedTreeFile, isLocked: true);
                }
            }
            using (SortedTreeTable<TKey, TValue>.Editor editor = m_sortedTreeFile.BeginEdit())
            {
                editor.AddPoints(stream);
                editor.Commit();
            }
            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
            {
                edit.RenewSnapshot(m_sortedTreeFile);
            }
        }

        /// <summary>
        /// Adds all of the data in <see cref="file"/> to this archive stage queues up 
        /// this existing file for deletion.
        /// </summary>
        /// <param name="file">the file to read from and then delete</param>
        public void CombineAndDelete(SortedTreeTable<TKey, TValue> file)
        {
            if (m_sortedTreeFile == null)
            {
                m_sortedTreeFile = m_initializer.CreateArchiveFile();
                using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
                {
                    //Add the newly created file.
                    edit.Add(m_sortedTreeFile, isLocked: true);
                }
            }
            using (SortedTreeTable<TKey, TValue>.Editor editor = m_sortedTreeFile.BeginEdit())
            {
                using (SortedTreeTableReadSnapshot<TKey, TValue> reader = file.BeginRead())
                {
                    SortedTreeScannerBase<TKey, TValue> scan = reader.GetTreeScanner();
                    scan.SeekToStart();
                    editor.AddPoints(scan);
                    editor.Commit();
                }
            }
            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
            {
                edit.RenewSnapshot(m_sortedTreeFile);
                edit.RemoveAndDelete(file);
            }
        }

        /// <summary>
        /// Extracts the internal <see cref="SortedTreeTable{TKey,TValue}"/> that was created and makes it
        /// where if more data is added to this class, a new archivefile will be created.
        /// </summary>
        /// <returns>the internal archive file. null</returns>
        public SortedTreeTable<TKey, TValue> GetFileAndSetNull()
        {
            if (m_sortedTreeFile == null)
                return null;
            SortedTreeTable<TKey, TValue> file = m_sortedTreeFile;
            m_sortedTreeFile = null;
            return file;
        }
    }
}