//******************************************************************************************************
//  TempFile`2.cs - Gbtc
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
//  2/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{

    public class TempFile<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private SortedTreeTable<TKey, TValue> m_sortedTreeFile;
        private readonly ArchiveList<TKey, TValue> m_archiveList;
        private readonly ArchiveInitializer<TKey, TValue> m_initialFile;
        private readonly ArchiveInitializer<TKey, TValue> m_finalFile;

        /// <summary>
        /// Constructs a staging file
        /// </summary>
        /// <param name="archiveList">the place to store the archive files when converted</param>
        /// <param name="initialFile">the initializer that will create the archive files</param>
        /// <param name="finalFile">the file that will be used to dump all of this data</param>
        public TempFile(ArchiveList<TKey, TValue> archiveList, ArchiveInitializer<TKey, TValue> initialFile, ArchiveInitializer<TKey, TValue> finalFile)
        {
            m_initialFile = initialFile;
            m_archiveList = archiveList;
            m_finalFile = finalFile;
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
                m_sortedTreeFile = m_initialFile.CreateArchiveFile();
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

        public void DumpToDisk()
        {
            if (m_sortedTreeFile == null)
                return;

            var newFile = m_finalFile.CreateArchiveFile();
            using (var editor = newFile.BeginEdit())
            using (var reader = m_sortedTreeFile.BeginRead())
            {
                var scan = reader.GetTreeScanner();
                scan.SeekToStart();
                editor.AddPoints(scan);
                editor.Commit();
            }

            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
            {
                edit.Add(newFile, false);
                edit.RemoveAndDelete(m_sortedTreeFile);
            }

            m_sortedTreeFile = null;
        }
    }
}