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
//  2/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

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

        private IncrementalStagingFile(ArchiveList<TKey, TValue> archiveList, ArchiveInitializer<TKey, TValue> initialFile, ArchiveInitializer<TKey, TValue> finalFile)
        {
            m_archiveList = archiveList;
            m_initialFile = initialFile;
            m_finalFile = finalFile;
        }

        /// <summary>
        /// Creates an <see cref="IncrementalStagingFile{TKey,TValue}"/> that will save to memory upon completion.
        /// </summary>
        /// <param name="list">the list to create new archives on.</param>
        /// <param name="encoding">the encoding method for the final file</param>
        /// <returns></returns>
        public static IncrementalStagingFile<TKey, TValue> CreateInMemory(ArchiveList<TKey, TValue> list, EncodingDefinition encoding)
        {
            var initialFile = ArchiveInitializer<TKey, TValue>.CreateInMemory(SortedTree.FixedSizeNode, FileFlags.Stage0);
            var finalFile = ArchiveInitializer<TKey, TValue>.CreateInMemory(encoding, FileFlags.Stage1);
            return new IncrementalStagingFile<TKey, TValue>(list, initialFile, finalFile);
        }

        /// <summary>
        /// Creates an <see cref="IncrementalStagingFile{TKey,TValue}"/> that will save to disk upon completion.
        /// </summary>
        /// <param name="list">the list to create new archives on</param>
        /// <param name="encoding">the encoding method for the final file</param>
        /// <param name="savePath">the path to save files to</param>
        /// <returns></returns>
        public static IncrementalStagingFile<TKey, TValue> CreateOnDisk(ArchiveList<TKey, TValue> list, EncodingDefinition encoding, string savePath)
        {
            var initialFile = ArchiveInitializer<TKey, TValue>.CreateInMemory(SortedTree.FixedSizeNode, FileFlags.Stage0);
            var finalFile = ArchiveInitializer<TKey, TValue>.CreateOnDisk(savePath, encoding, "Stage1", FileFlags.Stage1);
            return new IncrementalStagingFile<TKey, TValue>(list, initialFile, finalFile);
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
                edit.RenewSnapshot(m_sortedTreeFile.ArchiveId);
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

            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
            {
                edit.Add(newFile, false);
                edit.TryRemoveAndDelete(m_sortedTreeFile.ArchiveId);
            }

            m_sortedTreeFile = null;
        }

        /// <summary>
        /// Makes a clone of this initializer. 
        /// </summary>
        /// <returns></returns>
        public IncrementalStagingFile<TKey, TValue> Clone()
        {
            return new IncrementalStagingFile<TKey, TValue>(m_archiveList, m_initialFile, m_finalFile);
        }
    }
}