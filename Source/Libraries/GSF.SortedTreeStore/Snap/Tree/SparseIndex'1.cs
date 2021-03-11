//******************************************************************************************************
//  SparseIndex`1.cs - Gbtc
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
//  03/15/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.IO;
using GSF.Snap.Types;

namespace GSF.Snap.Tree
{
    /// <summary>
    /// Contains information on how to parse the index nodes of the SortedTree
    /// </summary>
    public sealed class SparseIndex<TKey>
        where TKey : SnapTypeBase<TKey>, new()
    {
        #region [ Members ]

        private bool m_isInitialized;
        private int m_blockSize;
        private readonly int m_keySize;
        private readonly TKey m_tmpKey;
        private readonly SnapUInt32 m_tmpValue;
        private BinaryStreamPointerBase m_stream;
        private Func<uint> m_getNextNewNodeIndex;
        private SortedTreeNodeBase<TKey, SnapUInt32>[] m_nodes;
        private readonly SortedTreeNodeBase<TKey, SnapUInt32> m_initializer;

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
        public SparseIndex()
        {
            m_initializer = Library.CreateTreeNode<TKey, SnapUInt32>(EncodingDefinition.FixedSizeCombinedEncoding, 0);
            m_tmpKey = new TKey();
            m_keySize = m_tmpKey.Size;
            m_tmpValue = new SnapUInt32();
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

            int minSize = (m_keySize + sizeof(uint)) * 4 + 12 + 2 * m_keySize; // (4 key pointers) + (Header Size))
            if (blockSize < minSize)
                throw new ArgumentOutOfRangeException("blockSize", string.Format("Must hold at least 4 elements which is {0}", minSize));

            SetCapacity(Math.Max((int)rootNodeLevel, 6));
        }

        #endregion

        #region [ Methods ]

        #region [ Get ]

        /// <summary>
        /// Gets the node index of the first leaf node in the tree.
        /// </summary>
        /// <param name="level">the level of the node requesting the lookup</param>
        /// <returns>the index of the first leaf node</returns>
        public uint GetFirstIndex(byte level)
        {
            if (RootNodeLevel == level)
                return RootNodeIndexAddress;

            uint nodeIndexAddress = RootNodeIndexAddress;
            byte nodeLevel = RootNodeLevel;
            while (nodeLevel > level)
            {
                SortedTreeNodeBase<TKey, SnapUInt32> currentNode = GetNode(nodeLevel);
                currentNode.SetNodeIndex(nodeIndexAddress);
                if (!currentNode.TryGetFirstRecord(m_tmpValue))
                    throw new Exception("Node is empty");
                nodeIndexAddress = m_tmpValue.Value;
                nodeLevel--;
            }
            return nodeIndexAddress;
        }

        /// <summary>
        /// Gets the node index of the last leaf node in the tree.
        /// </summary>
        /// <param name="level">the level of the node requesting the lookup</param>
        /// <returns></returns>
        public uint GetLastIndex(byte level)
        {
            if (RootNodeLevel == level)
                return RootNodeIndexAddress;
            uint nodeIndexAddress = RootNodeIndexAddress;
            byte nodeLevel = RootNodeLevel;
            while (nodeLevel > level)
            {
                SortedTreeNodeBase<TKey, SnapUInt32> currentNode = GetNode(nodeLevel);
                currentNode.SetNodeIndex(nodeIndexAddress);
                if (!currentNode.TryGetLastRecord(m_tmpValue))
                    throw new Exception("Node is empty");
                nodeIndexAddress = m_tmpValue.Value;
                nodeLevel--;
            }
            return nodeIndexAddress;
        }

        /// <summary>
        /// Gets the data for the following key. 
        /// </summary>
        /// <param name="key">The key to look up. Only uses the key portion of the TKeyValue</param>
        /// <param name="level"></param>
        public uint Get(TKey key, byte level)
        {
            if (RootNodeLevel == 0)
                return RootNodeIndexAddress;
            SortedTreeNodeBase<TKey, SnapUInt32> node = FindNode(key, level + 1);
            node.GetOrGetNext(key, m_tmpValue);
            return m_tmpValue.Value;
        }

        /// <summary>
        /// Gets the node at the provided <see cref="level"/> where the provided <see cref="key"/> fits.
        /// </summary>
        /// <param name="key">the key to search for.</param>
        /// <param name="level">the level of the tree </param>
        /// <returns></returns>
        private SortedTreeNodeBase<TKey, SnapUInt32> FindNode(TKey key, int level)
        {
            if (level <= 0)
                throw new ArgumentOutOfRangeException("level", "Cannot be <= 0");
            if (level > RootNodeLevel)
                throw new ArgumentOutOfRangeException("level", "Cannot be greater than the root node level.");

            SortedTreeNodeBase<TKey, SnapUInt32> currentNode;
            //Shortcut
            currentNode = GetNode(level);
            if (currentNode.IsKeyInsideBounds(key))
                return currentNode;

            uint nodeIndexAddress = RootNodeIndexAddress;
            byte nodeLevel = RootNodeLevel;
            while (true)
            {
                currentNode = GetNode(nodeLevel);
                currentNode.SetNodeIndex(nodeIndexAddress);
                if (nodeLevel == level)
                    return currentNode;
                currentNode.GetOrGetNext(key, m_tmpValue);
                nodeIndexAddress = m_tmpValue.Value;
                nodeLevel--;
            }
        }

        /// <summary>
        /// Gets the node associated with the current level.
        /// </summary>
        /// <param name="nodeLevel"></param>
        /// <returns></returns>
        private SortedTreeNodeBase<TKey, SnapUInt32> GetNode(int nodeLevel)
        {
            return m_nodes[nodeLevel - 1];
        }

        #endregion

        /// <summary>
        /// Updates the specified leaf node to the provided key.
        /// </summary>
        public void UpdateKey(TKey oldKey, TKey newKey, byte level)
        {
            if (level <= RootNodeLevel)
            {
                GetNode(level).UpdateKey(oldKey, newKey);
            }
            else
            {
                throw new Exception("Cannot update key of root");
            }
        }

        /// <summary>
        /// Updates the value for the provided key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="level"></param>
        public void UpdateValue(TKey key, SnapUInt32 value, byte level)
        {
            if (level <= RootNodeLevel)
            {
                GetNode(level).UpdateValue(key, value);
            }
            else
            {
                throw new Exception("Cannot update value of root");
            }
        }

        /// <summary>
        /// Removes the specified leaf node from the sparse index
        /// </summary>
        /// <param name="key">The leaf node to remove</param>
        /// <param name="level"></param>
        public void Remove(TKey key, byte level)
        {
            if (level <= RootNodeLevel)
            {
                SortedTreeNodeBase<TKey, SnapUInt32> node = GetNode(level);
                if (!node.TryRemove(key))
                    throw new KeyNotFoundException();
                if (level == RootNodeLevel)
                {
                    if (node.RightSiblingNodeIndex == uint.MaxValue &&
                        node.LeftSiblingNodeIndex == uint.MaxValue &&
                        node.RecordCount == 1)
                    {
                        RootNodeLevel--;
                        node.TryGetFirstRecord(m_tmpKey, m_tmpValue);
                        RootNodeIndexAddress = m_tmpValue.Value;
                        node.Clear();
                    }
                }
            }
            else
            {
                throw new Exception("Cannot update value of root");
            }
        }

        /// <summary>
        /// When attempting to remove or combine a node, we must check the parent to find which one will be supported to remove.
        /// </summary>
        /// <param name="key">the lower key of the node that is being combined or removed.</param>
        /// <param name="level">the level</param>
        /// <param name="canCombineLeft">outputs true if this node may be combined with the left node.</param>
        /// <param name="canCombineRight">outputs false if this node may be combined with the right node.</param>
        public void CanCombineWithSiblings(TKey key, byte level, out bool canCombineLeft, out bool canCombineRight)
        {
            if (level <= RootNodeLevel)
            {
                GetNode(level).CanCombineWithSiblings(key, out canCombineLeft, out canCombineRight);
            }
            else
            {
                throw new Exception("Cannot update value of root");
            }
        }

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
                SnapUInt32 value = new SnapUInt32(pointer);
                GetNode(level).TryInsert(nodeKey, value);
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
            SortedTreeNodeBase<TKey, SnapUInt32> rootNode = GetNode(RootNodeLevel);
            rootNode.CreateEmptyNode(RootNodeIndexAddress);

            //Insert the first entry in the root node.
            m_tmpKey.SetMin();
            m_tmpValue.Value = oldRootNode;
            rootNode.TryInsert(m_tmpKey, m_tmpValue);

            //Insert the second entry in the root node.
            m_tmpValue.Value = leafNodeIndex;
            rootNode.TryInsert(leafKey, m_tmpValue);

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
            m_nodes = new SortedTreeNodeBase<TKey, SnapUInt32>[count];
            for (int x = 0; x < m_nodes.Length; x++)
            {
                m_nodes[x] = m_initializer.Clone((byte)(x + 1));
                m_nodes[x].Initialize(m_stream, m_blockSize, m_getNextNewNodeIndex, this);
            }
        }
    }
}