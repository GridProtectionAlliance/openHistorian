////******************************************************************************************************
////  IncrementalStagingFile`2.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  2/16/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System.Collections.Generic;
//using GSF.SortedTreeStore.Engine.Reader;
//using GSF.SortedTreeStore.Storage;
//using GSF.SortedTreeStore.Tree;

//namespace GSF.SortedTreeStore.Services.Writer
//{
//    /// <summary>
//    /// A helper class for <see cref="FirstStageWriter{TKey,TValue}"/> that creates in memory files
//    /// that can be incrementally added to until they are dumped to the disk as a compressed file.
//    /// </summary>
//    /// <typeparam name="TKey">The key</typeparam>
//    /// <typeparam name="TValue">The value</typeparam>
//    public class IncrementalStagingCollection<TKey, TValue>
//        where TKey : SortedTreeTypeBase<TKey>, new()
//        where TValue : SortedTreeTypeBase<TValue>, new()
//    {
//        List<FileData> m_files;

//        private class FileData
//        {
//            public SortedTreeTable<TKey, TValue> File;
//            public FileData(ArchiveInitializer<TKey, TValue> initialization, ArchiveList<TKey, TValue> list)
//            {
//                File = initialization.CreateArchiveFile();
//                using (ArchiveList<TKey, TValue>.Editor edit = list.AcquireEditLock())
//                {
//                    //Add the newly created file.
//                    edit.Add(File, isLocked: true);
//                }
//            }
//            public void Append()
//            {

//            }


//        }

//        private readonly ArchiveList<TKey, TValue> m_archiveList;
//        private readonly ArchiveInitializer<TKey, TValue> m_initialFile;
//        private readonly ArchiveInitializer<TKey, TValue> m_finalFile;

//        private IncrementalStagingCollection(ArchiveList<TKey, TValue> archiveList, ArchiveInitializer<TKey, TValue> initialFile, ArchiveInitializer<TKey, TValue> finalFile)
//        {
//            m_files = new List<FileData>();
//            m_archiveList = archiveList;
//            m_initialFile = initialFile;
//            m_finalFile = finalFile;
//        }

//        /// <summary>
//        /// Creates an <see cref="IncrementalStagingFile{TKey,TValue}"/> that will save to memory upon completion.
//        /// </summary>
//        /// <param name="list">the list to create new archives on.</param>
//        /// <param name="encoding">the encoding method for the final file</param>
//        /// <returns></returns>
//        public static IncrementalStagingCollection<TKey, TValue> CreateInMemory(ArchiveList<TKey, TValue> list, EncodingDefinition encoding)
//        {
//            var initialFile = ArchiveInitializer<TKey, TValue>.CreateInMemory(SortedTree.FixedSizeNode);
//            var finalFile = ArchiveInitializer<TKey, TValue>.CreateInMemory(encoding);
//            return new IncrementalStagingCollection<TKey, TValue>(list, initialFile, finalFile);
//        }

//        /// <summary>
//        /// Creates an <see cref="IncrementalStagingFile{TKey,TValue}"/> that will save to disk upon completion.
//        /// </summary>
//        /// <param name="list">the list to create new archives on</param>
//        /// <param name="encoding">the encoding method for the final file</param>
//        /// <param name="savePath">the path to save files to</param>
//        /// <returns></returns>
//        public static IncrementalStagingCollection<TKey, TValue> CreateOnDisk(ArchiveList<TKey, TValue> list, EncodingDefinition encoding, string savePath)
//        {
//            var initialFile = ArchiveInitializer<TKey, TValue>.CreateInMemory(SortedTree.FixedSizeNode);
//            var finalFile = ArchiveInitializer<TKey, TValue>.CreateOnDisk(savePath, encoding, "Stage1");
//            return new IncrementalStagingCollection<TKey, TValue>(list, initialFile, finalFile);
//        }

//        /// <summary>
//        /// Gets the current size of the archive file.
//        /// </summary>
//        public long Size
//        {
//            get
//            {
//                long size = 0;
//                foreach (var f in m_files)
//                {
//                    size += f.File.BaseFile.FileSize;
//                }
//                return size;
//            }
//        }


//        /// <summary>
//        /// Appends the entire contents of this stream to the existing archive stage.
//        /// </summary>
//        /// <param name="stream">the stream to read</param>
//        public void Append(TreeStream<TKey, TValue> stream)
//        {
//            if (m_files.Count == 0)
//            {
//                var file = new FileData(m_initialFile, m_archiveList);
//                m_files.Add(file);
//            }

//            SortedTreeTable<TKey, TValue> lastFile = m_files[m_files.Count - 1].File;

//            using (SortedTreeTable<TKey, TValue>.Editor editor = lastFile.BeginEdit())
//            {
//                editor.AddPoints(stream);
//                editor.Commit();
//            }
//            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
//            {
//                edit.RenewSnapshot(lastFile);
//            }
//        }

//        /// <summary>
//        /// Dumps all of the current data to the disk.
//        /// </summary>
//        public void DumpToDisk()
//        {
//            if (m_files.Count == 0)
//                return;

//            var unionList = new List<ArchiveTableSummary<TKey, TValue>>();

//            foreach (var file in m_files)
//            {
//                var snapshot = new ArchiveTableSummary<TKey, TValue>(file.File);
//                unionList.Add(snapshot);
//            }

//            var unionReader = new UnionReader<TKey, TValue>(unionList);

//            var newFile = m_finalFile.CreateArchiveFile();
//            using (var editor = newFile.BeginEdit())
//            {
//                editor.AddPoints(unionReader);
//                editor.Commit();
//            }

//            using (ArchiveList<TKey, TValue>.Editor edit = m_archiveList.AcquireEditLock())
//            {
//                edit.Add(newFile, false);

//                foreach (var file in m_files)
//                {
//                    edit.RemoveAndDelete(file.File);
//                }
//            }
//            m_files.Clear();
//        }

//        /// <summary>
//        /// Makes a clone of this initializer. 
//        /// </summary>
//        /// <returns></returns>
//        public IncrementalStagingCollection<TKey, TValue> Clone()
//        {
//            return new IncrementalStagingCollection<TKey, TValue>(m_archiveList, m_initialFile, m_finalFile);
//        }
//    }
//}