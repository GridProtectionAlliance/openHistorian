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
    public class SequentialSortedTreeWriter<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        #region [ Members ]

        private bool m_hasInsertedData;
        protected SparseIndexWriter<TKey> Indexer;
        protected NodeWriter<TKey, TValue> LeafStorage;
        private SortedTreeHeader m_header;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new SortedTree writing to the provided streams and using the specified compression method for the tree node.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
        /// <param name="treeNodeType"></param>
        /// <returns></returns>
        public SequentialSortedTreeWriter(BinaryStreamPointerBase stream, int blockSize, EncodingDefinition treeNodeType)
        {
            if ((object)treeNodeType == null)
                throw new ArgumentNullException("treeNodeType");

            m_header = new SortedTreeHeader();
            Stream = stream;

            m_header.TreeNodeType = treeNodeType;
            m_header.BlockSize = blockSize;

            m_header.RootNodeLevel = 0;
            m_header.RootNodeIndexAddress = 1;
            m_header.LastAllocatedBlock = 1;

            Indexer = new SparseIndexWriter<TKey>();
            Indexer.RootHasChanged += IndexerOnRootHasChanged;
            Indexer.Initialize(Stream, m_header.BlockSize, GetNextNewNodeIndex, m_header.RootNodeLevel, m_header.RootNodeIndexAddress);

            if (SortedTree.FixedSizeNode == treeNodeType)
                LeafStorage = new NodeWriter<TKey, TValue>(treeNodeType, 0, 1, Stream, m_header.BlockSize, GetNextNewNodeIndex, Indexer);
            else
                LeafStorage = new NodeWriter<TKey, TValue>(treeNodeType, 0, 2, Stream, m_header.BlockSize, GetNextNewNodeIndex, Indexer);

            LeafStorage.CreateEmptyNode(m_header.RootNodeIndexAddress);
            m_header.IsDirty = true;
            m_header.SaveHeader(Stream);
        }

        private void IndexerOnRootHasChanged(object sender, EventArgs eventArgs)
        {
            m_header.RootNodeLevel = Indexer.RootNodeLevel;
            m_header.RootNodeIndexAddress = Indexer.RootNodeIndexAddress;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Contains the stream for reading and writing.
        /// </summary>
        protected BinaryStreamPointerBase Stream
        {
            get;
            private set;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Appends the supplied stream to the SortedTree
        /// </summary>
        /// <param name="stream"></param>
        public void Build(TreeStream<TKey, TValue> stream)
        {
            if (m_hasInsertedData)
                throw new Exception("Duplicate calls to AddStream");
            if (!(stream.IsAlwaysSequential && stream.NeverContainsDuplicates))
                throw new ArgumentException("Stream must gaurentee sequential reads and that it never will contain a duplicate", "stream");

            m_hasInsertedData = true;

            TKey key = new TKey();
            TValue value = new TValue();
            int cnt = 0;
            while (stream.Read(key, value))
            {
                if (cnt == 20)
                    cnt++;
                cnt++;

                LeafStorage.Insert(key, value);
            }
            m_header.IsDirty = true;
            m_header.SaveHeader(Stream);
        }

        #endregion

        #region [ Protected Methods ]

        /// <summary>
        /// Returns the node index address for a freshly allocated block.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Also saves the header data</remarks>
        protected uint GetNextNewNodeIndex()
        {
            m_header.LastAllocatedBlock++;
            return m_header.LastAllocatedBlock;
        }

        #endregion

        #region [ Private Methods ]



        #endregion

    }
}
