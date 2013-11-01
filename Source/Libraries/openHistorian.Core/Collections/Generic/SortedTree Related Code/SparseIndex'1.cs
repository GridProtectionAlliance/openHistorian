//******************************************************************************************************
//  SparseIndex`1.cs - Gbtc
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
//  3/15/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.IO;
using openHistorian.Collections.Generic.TreeNodes;

namespace openHistorian.Collections.Generic
{
    /// <summary>
    /// Contains information on how to parse the index nodes of the SortedTree
    /// </summary>
    public sealed class SparseIndex<TKey>
        where TKey : class, ISortedTreeKey<TKey>, new()
    {
        #region [ Members ]

        private bool m_isInitialized;
        private int m_blockSize;
        private readonly TKey m_key;
        private readonly TreeUInt32 m_value;
        private BinaryStreamBase m_stream;
        private Func<uint> m_getNextNewNodeIndex;
        private readonly TreeKeyMethodsBase<TKey> m_keyMethods;
        private TreeNodeBase<TKey, TreeUInt32>[] m_nodes;
        private readonly TreeNodeInitializer<TKey, TreeUInt32> m_initializer;

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
        /// <param name="encodingMethod">The encoding that will be used with each node</param>
        public SparseIndex(Guid encodingMethod)
        {
            m_initializer = TreeNodeInitializer.GetTreeNodeInitializer<TKey, TreeUInt32>(encodingMethod);
            m_key = new TKey();
            m_value = new TreeUInt32();
            m_keyMethods = m_key.CreateKeyMethods();

        }

        /// <summary>
        /// Creates a sparse index on the tree. 
        /// </summary>
        /// <param name="stream">The stream to use to write the index</param>
        /// <param name="blockSize">The size of each node that will be used by this index.</param>
        /// <param name="getNextNewNodeIndex">A method to use when additional nodes must be allocated.</param>
        /// <param name="rootNodeLevel"></param>
        /// <param name="rootNodeIndexAddress"></param>
        public void Initialize(BinaryStreamBase stream, int blockSize, Func<uint> getNextNewNodeIndex, byte rootNodeLevel, uint rootNodeIndexAddress)
        {
            if (m_isInitialized)
                throw new Exception("Duplicate calls to Initialize");
            m_isInitialized = true;
            RootNodeLevel = rootNodeLevel;
            RootNodeIndexAddress = rootNodeIndexAddress;
            m_stream = stream;
            m_getNextNewNodeIndex = getNextNewNodeIndex;
            m_blockSize = blockSize;

            int minSize = (m_keyMethods.Size + sizeof(uint)) * 4 + (12 + 2 * m_keyMethods.Size); // (4 key pointers) + (Header Size))
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
        /// <returns></returns>
        public uint GetFirstIndex()
        {
            if (RootNodeLevel == 0)
                return RootNodeIndexAddress;
            uint nodeIndexAddress = RootNodeIndexAddress;
            byte nodeLevel = RootNodeLevel;
            while (true)
            {
                TreeNodeBase<TKey, TreeUInt32> currentNode = GetNode(nodeLevel);
                currentNode.SetNodeIndex(nodeIndexAddress);
                currentNode.TryGetFirstRecord(m_key, m_value);
                nodeIndexAddress = m_value.Value;
                if (nodeLevel == 1)
                    return nodeIndexAddress;
                nodeLevel--;
            }
        }

        /// <summary>
        /// Gets the node index of the last leaf node in the tree.
        /// </summary>
        /// <returns></returns>
        public uint GetLastIndex()
        {
            if (RootNodeLevel == 0)
                return RootNodeIndexAddress;
            uint nodeIndexAddress = RootNodeIndexAddress;
            byte nodeLevel = RootNodeLevel;
            while (true)
            {
                TreeNodeBase<TKey, TreeUInt32> currentNode = GetNode(nodeLevel);
                currentNode.SetNodeIndex(nodeIndexAddress);
                currentNode.TryGetLastRecord(m_key, m_value);
                nodeIndexAddress = m_value.Value;
                if (nodeLevel == 1)
                    return nodeIndexAddress;
                nodeLevel--;
            }
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
            FindNode(key, level + 1).GetOrGetNext(key, m_value);
            return m_value.Value;
        }

        /// <summary>
        /// Gets the node at the provided <see cref="level"/> where the provided <see cref="key"/> fits.
        /// </summary>
        /// <param name="key">the key to search for.</param>
        /// <param name="level">the level of the tree </param>
        /// <returns></returns>
        private TreeNodeBase<TKey, TreeUInt32> FindNode(TKey key, int level)
        {
            if (level <= 0)
                throw new ArgumentOutOfRangeException("level", "Cannot be <= 0");
            if (level > RootNodeLevel)
                throw new ArgumentOutOfRangeException("level", "Cannot be greater than the root node level.");

            //Shortcut
            TreeNodeBase<TKey, TreeUInt32> currentNode = GetNode(level);
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
                currentNode.TryGet(m_key, m_value);
                nodeIndexAddress = m_value.Value;
                nodeLevel--;
            }
        }

        /// <summary>
        /// Gets the node associated with the current level.
        /// </summary>
        /// <param name="nodeLevel"></param>
        /// <returns></returns>
        private TreeNodeBase<TKey, TreeUInt32> GetNode(int nodeLevel)
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
        public void UpdateValue(TKey key, TreeUInt32 value, byte level)
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
                TreeNodeBase<TKey, TreeUInt32> node = GetNode(level);
                if (!node.TryRemove(key))
                    throw new KeyNotFoundException();
                if (level == RootNodeLevel)
                {
                    if (node.RightSiblingNodeIndex == uint.MaxValue &&
                        node.LeftSiblingNodeIndex == uint.MaxValue &&
                        node.RecordCount == 1)
                    {
                        RootNodeLevel--;
                        node.TryGetFirstRecord(m_key, m_value);
                        RootNodeIndexAddress = m_value.Value;
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
        /// <param name="key">the lower key of the node that is being conbined or removed.</param>
        /// <param name="level">the level</param>
        /// <param name="canCombineLeft">outputs true if this node may be conbined with the left node.</param>
        /// <param name="canCombineRight">outputs false if this node may be conbined with the right node.</param>
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
                TreeUInt32 value = new TreeUInt32(pointer);
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
            TreeNodeBase<TKey, TreeUInt32> rootNode = GetNode(RootNodeLevel);
            rootNode.CreateEmptyNode(RootNodeIndexAddress);

            //Insert the first entry in the root node.
            m_keyMethods.SetMin(m_key);
            m_value.Value = oldRootNode;
            rootNode.TryInsert(m_key, m_value);

            //Insert the second entry in the root node.
            m_value.Value = leafNodeIndex;
            rootNode.TryInsert(leafKey, m_value);

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
            m_nodes = new TreeNodeBase<TKey, TreeUInt32>[count];
            for (int x = 0; x < m_nodes.Length; x++)
            {
                m_nodes[x] = m_initializer.CreateTreeNode((byte)(x + 1));
                m_nodes[x].Initialize(m_stream, m_blockSize, m_getNextNewNodeIndex, this);
            }
        }
    }
}