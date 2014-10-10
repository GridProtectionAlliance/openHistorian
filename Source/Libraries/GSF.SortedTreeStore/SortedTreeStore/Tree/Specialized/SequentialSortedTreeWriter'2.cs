//******************************************************************************************************
//  SequentialSortedTreeWriter`2.cs - Gbtc
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
//  10/09/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.SortedTreeStore.Tree.Specialized
{
    /// <summary>
    /// A specialized serialization method for writing data to a disk in the SortedTreeStore method.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public static class SequentialSortedTreeWriter<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {

        /// <summary>
        /// Writes the supplied stream to the binary stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
        /// <param name="treeNodeType"></param>
        /// <param name="treeStream"></param>
        public static void Create(BinaryStreamPointerBase stream, int blockSize, EncodingDefinition treeNodeType, TreeStream<TKey, TValue> treeStream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (treeStream == null)
                throw new ArgumentNullException("stream");
            if ((object)treeNodeType == null)
                throw new ArgumentNullException("treeNodeType");
            if (!(treeStream.IsAlwaysSequential && treeStream.NeverContainsDuplicates))
                throw new ArgumentException("Stream must gaurentee sequential reads and that it never will contain a duplicate", "treeStream");

            SortedTreeHeader header = new SortedTreeHeader();
            header.TreeNodeType = treeNodeType;
            header.BlockSize = blockSize;
            header.RootNodeLevel = 0;
            header.RootNodeIndexAddress = 1;
            header.LastAllocatedBlock = 1;

            Func<uint> getNextNewNodeIndex = () =>
            {
                header.LastAllocatedBlock++;
                return header.LastAllocatedBlock;
            };

            SparseIndex<TKey> indexer = new SparseIndex<TKey>();
            indexer.Initialize(stream, header.BlockSize, getNextNewNodeIndex, header.RootNodeLevel, header.RootNodeIndexAddress);
            
            NodeWriter<TKey, TValue> leafStorage = new NodeWriter<TKey, TValue>(treeNodeType, 0, stream, header.BlockSize, getNextNewNodeIndex, indexer);
            leafStorage.CreateEmptyNode(header.RootNodeIndexAddress);
            leafStorage.Insert(treeStream);
            //while (treeStream.Read(key, value))
            //{
            //    leafStorage.Insert(key, value);
            //}

            header.RootNodeLevel = indexer.RootNodeLevel;
            header.RootNodeIndexAddress = indexer.RootNodeIndexAddress;

            header.IsDirty = true;
            header.SaveHeader(stream);
        }
    }
}
