//******************************************************************************************************
//  SparseIndexWriter`1.cs - Gbtc
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
using GSF.SortedTreeStore.Types;

namespace GSF.SortedTreeStore.Tree.Specialized
{
    /// <summary>
    /// Contains information on how to parse the index nodes of the SortedTree
    /// </summary>
    public sealed class SparseIndexWriter<TKey>
        where TKey : SortedTreeTypeBase<TKey>, new()
    {
        #region [ Members ]

        private bool m_isInitialized;
        private int m_blockSize;
        private int m_keySize;
        private readonly TKey m_tmpKey;
        private readonly SortedTreeUInt32 m_tmpValue;
        private BinaryStreamPointerBase m_stream;
        private Func<uint> m_getNextNewNodeIndex;
        private NodeWriter<TKey, SortedTreeUInt32>[] m_nodes;

        /// <summary>
        /// Gets the indexed address for the root node
        /// </summary>
        public uint RootNodeIndexAddress
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the level of the root node. If this is zero, there is only 1 leaf node.
        /// </summary>
        public byte RootNodeLevel
        {
            get;
            protected set;
        }

        /// <summary>
        /// Event raised when the root of the tree changes, 
        /// thus <see cref="RootNodeIndexAddress"/> and <see cref="RootNodeLevel"/> 
        /// need to be saved to the header.
        /// </summary>
        public event EventHandler RootHasChanged;

        /// <summary>
        /// Raises the event
        /// </summary>
        private void OnRootHasChanged()
        {
            EventHandler handler = RootHasChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new sparse index. Be sure to initialize this class by calling <see cref="Initialize"/> before using this.
        /// </summary>
        public SparseIndexWriter()
        {
            m_tmpKey = new TKey();
            m_keySize = m_tmpKey.Size;
            m_tmpValue = new SortedTreeUInt32();
        }

        /// <summary>
        /// Creates a sparse index on the tree. 
        /// </summary>
        /// <param name="stream">The stream to use to write the index</param>
        /// <param name="blockSize">The size of each node that will be used by this index.</param>
        /// <param name="getNextNewNodeIndex">A method to use when additional nodes must be allocated.</param>
        /// <param name="rootNodeLevel">the level of the root node.</param>
        /// <param name="rootNodeIndexAddress">the address location for the root node.</param>
        /// <exception cref="Exception">Throw of duplicate calls are made to this function</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the block size is not large enough to store at least 4 elements.</exception>
        public void Initialize(BinaryStreamPointerBase stream, int blockSize, Func<uint> getNextNewNodeIndex, byte rootNodeLevel, uint rootNodeIndexAddress)
        {
            if (m_isInitialized)
                throw new Exception("Duplicate calls to Initialize");
            m_isInitialized = true;
            RootNodeLevel = rootNodeLevel;
            RootNodeIndexAddress = rootNodeIndexAddress;
            m_stream = stream;
            m_getNextNewNodeIndex = getNextNewNodeIndex;
            m_blockSize = blockSize;

            int minSize = (m_keySize + sizeof(uint)) * 4 + (12 + 2 * m_keySize); // (4 key pointers) + (Header Size))
            if (blockSize < minSize)
                throw new ArgumentOutOfRangeException("blockSize", string.Format("Must hold at least 4 elements which is {0}", minSize));

            SetCapacity(Math.Max((int)rootNodeLevel, 6));
        }

        #endregion

        #region [ Methods ]

        #region [ Get ]

        /// <summary>
        /// Gets the node associated with the current level.
        /// </summary>
        /// <param name="nodeLevel"></param>
        /// <returns></returns>
        private NodeWriter<TKey, SortedTreeUInt32> GetNode(int nodeLevel)
        {
            return m_nodes[nodeLevel - 1];
        }

        #endregion


        /// <summary>
        /// Adds the following node pointer to the sparse index.
        /// </summary>
        /// <param name="nodeKey">the first key in the <see cref="pointer"/>. Only uses the key portion of the TKeyValue</param>
        /// <param name="pointer">the index of the later node</param>
        /// <param name="level">the level of the node being added</param>
        /// <remarks>This class will add the new node data to the parent node, 
        /// or create a new root if the current root is split.</remarks>
        public void Add(TKey nodeKey, uint pointer, byte level)
        {
            if (level <= RootNodeLevel)
            {
                SortedTreeUInt32 value = new SortedTreeUInt32(pointer);
                GetNode(level).Insert(nodeKey, value);
            }
            else //A new root node needs to be created.
            {
                CreateNewRootNode(nodeKey, pointer);
            }
        }

        #endregion

        /// <summary>
        /// Creates a new root node by combining the current root node with the provided node data.
        /// </summary>
        /// <param name="leafKey"></param>
        /// <param name="leafNodeIndex"></param>
        private void CreateNewRootNode(TKey leafKey, uint leafNodeIndex)
        {
            if (RootNodeLevel + 1 > 250)
                throw new Exception("Tree is full. Tree cannot exceede 250 levels in depth.");
            int nodeLevel = RootNodeLevel + 1;
            if (nodeLevel > m_nodes.Length)
                SetCapacity(nodeLevel);

            //Get the ID for the new root node.
            uint oldRootNode = RootNodeIndexAddress;
            RootNodeIndexAddress = m_getNextNewNodeIndex();
            RootNodeLevel += 1;

            //Create the empty node
            NodeWriter<TKey, SortedTreeUInt32> rootNode = GetNode(RootNodeLevel);
            rootNode.CreateEmptyNode(RootNodeIndexAddress);

            //Insert the first entry in the root node.
            m_tmpKey.SetMin();
            m_tmpValue.Value = oldRootNode;
            rootNode.Insert(m_tmpKey, m_tmpValue);

            //Insert the second entry in the root node.
            m_tmpValue.Value = leafNodeIndex;
            rootNode.Insert(leafKey, m_tmpValue);

            OnRootHasChanged();
            //foreach (var node in m_nodes)
            //    node.Clear();
        }

        /// <summary>
        /// Sets the capacity to the following number of levels.
        /// </summary>
        /// <param name="count">the number of levels to include.</param>
        private void SetCapacity(int count)
        {
            m_nodes = new NodeWriter<TKey, SortedTreeUInt32>[count];
            for (int x = 0; x < m_nodes.Length; x++)
            {
                m_nodes[x] = new NodeWriter<TKey, SortedTreeUInt32>(SortedTree.FixedSizeNode, (byte)(x + 1), 1, m_stream, m_blockSize, m_getNextNewNodeIndex, this);
            }
        }
    }
}