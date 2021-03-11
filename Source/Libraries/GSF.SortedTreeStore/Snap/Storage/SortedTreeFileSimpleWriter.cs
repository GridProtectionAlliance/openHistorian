//******************************************************************************************************
//  SortedTreeFileSimpleWriter.cs - Gbtc
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
//  10/16/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using GSF.IO.FileStructure;
using GSF.IO.Unmanaged;
using GSF.Snap.Collection;
using GSF.Snap.Services.Reader;
using GSF.Snap.Tree.Specialized;

namespace GSF.Snap.Storage
{
    /// <summary>
    /// Will write a file.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public static class SortedTreeFileSimpleWriter<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        /// <summary>
        /// Creates a new arhive file with the supplied data.
        /// </summary>
        /// <param name="pendingFileName"></param>
        /// <param name="completeFileName"></param>
        /// <param name="blockSize"></param>
        /// <param name="treeNodeType"></param>
        /// <param name="treeStream"></param>
        /// <param name="flags"></param>
        public static void Create(string pendingFileName, string completeFileName, int blockSize, Action<Guid> archiveIdCallback, EncodingDefinition treeNodeType, TreeStream<TKey, TValue> treeStream, params Guid[] flags)
        {
            using (SimplifiedFileWriter writer = new SimplifiedFileWriter(pendingFileName, completeFileName, blockSize, flags))
            {
                if (archiveIdCallback != null)
                    archiveIdCallback(writer.ArchiveId);

                using (ISupportsBinaryStream file = writer.CreateFile(GetFileName()))
                using (BinaryStream bs = new BinaryStream(file))
                {
                    SequentialSortedTreeWriter<TKey, TValue>.Create(bs, blockSize - 32, treeNodeType, treeStream);
                }
                writer.Commit();
            }
        }

        public static void CreateNonSequential(string pendingFileName, string completeFileName, int blockSize, Action<Guid> archiveIdCallback, EncodingDefinition treeNodeType, TreeStream<TKey, TValue> treeStream, params Guid[] flags)
        {
            SortedPointBuffer<TKey, TValue> m_queue;
            m_queue = new SortedPointBuffer<TKey, TValue>(100000, true);
            m_queue.IsReadingMode = false;

            TKey key = new TKey();
            TValue value = new TValue();

            List<SortedTreeTable<TKey, TValue>> pendingFiles = new List<SortedTreeTable<TKey, TValue>>();

            try
            {
                while (treeStream.Read(key, value))
                {
                    if (m_queue.IsFull)
                    {
                        pendingFiles.Add(CreateMemoryFile(treeNodeType, m_queue));
                    }
                    m_queue.TryEnqueue(key, value);
                }

                if (m_queue.Count > 0)
                {
                    pendingFiles.Add(CreateMemoryFile(treeNodeType, m_queue));
                }

                using (UnionTreeStream<TKey, TValue> reader = new UnionTreeStream<TKey, TValue>(pendingFiles.Select(x => new ArchiveTreeStreamWrapper<TKey, TValue>(x)), false))
                {
                    Create(pendingFileName, completeFileName, blockSize, archiveIdCallback, treeNodeType, reader, flags);
                }

            }
            finally
            {
                pendingFiles.ForEach(x => x.Dispose());
            }
        }

        private static SortedTreeTable<TKey, TValue> CreateMemoryFile(EncodingDefinition treeNodeType, SortedPointBuffer<TKey, TValue> buffer)
        {
            buffer.IsReadingMode = true;

            SortedTreeFile file = SortedTreeFile.CreateInMemory(4096);
            SortedTreeTable<TKey, TValue> table = file.OpenOrCreateTable<TKey, TValue>(treeNodeType);
            using (SortedTreeTableEditor<TKey, TValue> edit = table.BeginEdit())
            {
                edit.AddPoints(buffer);
                edit.Commit();
            }

            buffer.IsReadingMode = false;
            return table;
        }

        /// <summary>
        /// Helper method. Creates the <see cref="SubFileName"/> for the default table.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        private static SubFileName GetFileName()
        {
            Guid keyType = new TKey().GenericTypeGuid;
            Guid valueType = new TValue().GenericTypeGuid;
            return SubFileName.Create(SortedTreeFile.PrimaryArchiveType, keyType, valueType);
        }


    }
}
