//******************************************************************************************************
//  SequentialSortedTreeWriter`2.cs - Gbtc
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
//  10/09/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.Snap.Types;

namespace GSF.Snap.Tree.Specialized
{
    /// <summary>
    /// A specialized serialization method for writing data to a disk in the SortedTreeStore method.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public static class SequentialSortedTreeWriter<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
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
            if (stream is null)
                throw new ArgumentNullException("stream");
            if (treeStream is null)
                throw new ArgumentNullException("stream");
            if (treeNodeType is null)
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

            SparseIndexWriter<TKey> indexer = new SparseIndexWriter<TKey>();

            NodeWriter<TKey, TValue>.Create(treeNodeType, stream, header.BlockSize, header.RootNodeLevel, header.RootNodeIndexAddress, getNextNewNodeIndex, indexer, treeStream);

            while (indexer.Count > 0)
            {
                indexer.SwitchToReading();
                header.RootNodeLevel++;
                header.RootNodeIndexAddress = getNextNewNodeIndex();

                SparseIndexWriter<TKey> indexer2 = new SparseIndexWriter<TKey>();
                NodeWriter<TKey, SnapUInt32>.Create(EncodingDefinition.FixedSizeCombinedEncoding, stream, header.BlockSize, header.RootNodeLevel, header.RootNodeIndexAddress, getNextNewNodeIndex, indexer2, indexer);

                indexer.Dispose();
                indexer = indexer2;
            }

            indexer.Dispose();

            header.IsDirty = true;
            header.SaveHeader(stream);
        }
    }
}
