//******************************************************************************************************
//  BPlusTreeBase.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  4/7/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.KeyValue
{

    /// <summary>
    /// Provides the basic user methods with any derived B+Tree.  
    /// This base class translates all of the core methods into simple methods
    /// that must be implemented by classes derived from this base class.
    /// </summary>
    /// <typeparam name="TKey">The unique key that will be used to store data in this tree.</typeparam>
    /// <typeparam name="TValue">The value that will be associated with each key.</typeparam>
    /// <remarks>This class does not support concurrent read operations.  This is due to the caching method of each tree.
    /// If concurrent read operations are desired, clone the tree.  
    /// Trees cannot be cloned if the user plans to write to the tree.</remarks>
    public abstract class BasicTreeBase
    {

        #region [ Members ]

        //static Guid s_fileType = new Guid("{7bfa9083-701e-4596-8273-8680a739271d}");
        int m_blockSize;
        uint m_rootNodeIndex;
        byte m_rootNodeLevel;
        uint m_nextUnallocatedBlock;
        IBinaryStream m_stream;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Opens an existing <see cref="BPlusTreeBase{TKey,TValue}"/> from the stream.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        protected BasicTreeBase(IBinaryStream stream)
        {
            m_stream = stream;
            LoadHeader();
        }

        /// <summary>
        /// Creates an empty <see cref="BPlusTreeBase{TKey,TValue}"/>
        /// and uses the underlying stream to save data to it.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        /// <param name="blockSize">the size of one block.  This should exactly match the
        /// amount of data space available in the underlying data object. BPlus trees get their 
        /// performance benefit because there is fewer I/O's required to find and insert blocks.</param>
        protected BasicTreeBase(IBinaryStream stream, int blockSize)
        {
            m_stream = stream;
            m_blockSize = blockSize;
            m_rootNodeLevel = 0;
            m_rootNodeIndex = 1;
            LeafNodeCreateEmptyNode(m_rootNodeIndex);
            m_nextUnallocatedBlock = 2;
            SaveHeader();
            LoadHeader();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Contains the stream for reading and writing and optional cloning.
        /// </summary>
        protected IBinaryStream Stream
        {
            get
            {
                return m_stream;
            }
        }
        /// <summary>
        /// Contains the block size that the tree nodes will be alligned on.
        /// </summary>
        protected int BlockSize
        {
            get
            {
                return m_blockSize;
            }
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Requests a range of data from the BPlusTree.
        /// </summary>
        /// <param name="filter">An optional filter that returns true if the provided key should be included in the scan.</param>
        /// <returns>
        /// An Enumerable class for getting the results of the query.
        /// </returns>
        public void GetRange(Func<long, long, long, long, bool> callback)
        {
            uint nodeIndex = m_rootNodeIndex;
            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            {
                nodeIndex = InternalNodeGetFirstIndex(nodeLevel, nodeIndex);
            }
            long key1;
            long key2;
            long value1;
            long value2;
            LeafNodeGetFirstKeyValue(nodeIndex, out key1, out key2, out value1, out value2);
            LeafNodeScan(nodeIndex, key1, key2, callback);
        }
        /// <summary>
        /// Requests a range of data from the BPlusTree.
        /// </summary>
        /// <param name="filter">An optional filter that returns true if the provided key should be included in the scan.</param>
        /// <returns>
        /// An Enumerable class for getting the results of the query.
        /// </returns>
        public void GetRange(long beginKey1, long beginKey2, Func<long, long, long, long, bool> callback)
        {
            uint nodeIndex = m_rootNodeIndex;
            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            {
                nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, beginKey1, beginKey2);
            }
            LeafNodeScan(nodeIndex, beginKey1, beginKey2, callback);
        }

        public void GetRange(long beginKey1, long beginKey2, long endKey1, long endKey2, Func<long, long, long, long, bool> callback)
        {
            uint nodeIndex = m_rootNodeIndex;
            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            {
                nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, beginKey1, beginKey2);
            }
            LeafNodeScan(nodeIndex, beginKey1, beginKey2, endKey1, endKey2, callback);
        }

        ////ToDo: Fix this feature
        //public void Remove(TKey key)
        //{
        //    uint nodeIndex = m_rootNodeIndex;
        //    for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
        //    {
        //        nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, key);
        //    }

        //    if (LeafNodeRemove(nodeIndex, key))
        //        return;
        //    throw new Exception("Key Not Found");
        //}

        ////ToDo: Fix this feature.
        //public void Update(TKey key, TValue value)
        //{
        //    uint nodeIndex = m_rootNodeIndex;
        //    for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
        //    {
        //        nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, key);
        //    }

        //    if (LeafNodeUpdate(nodeIndex, key, value))
        //        return;
        //    throw new Exception("Key Not Found");
        //}

        /// <summary>
        /// Inserts the following data into the tree.
        /// </summary>
        /// <param name="key">The unique key value.</param>
        /// <param name="value">The value to insert.</param>
        public void Add(long key1, long key2, long value1, long value2)
        {
            uint nodeIndex = m_rootNodeIndex;
            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            {
                nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, key1, key2);
            }

            if (LeafNodeInsert(nodeIndex, key1, key2, value1, value2))
                return;
            throw new Exception("Key already exists");
        }

        /// <summary>
        /// Returns the data for the following key. 
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>Null or the Default structure value if the key does not exist.</returns>
        public void Get(long key1, long key2, out long value1, out long value2)
        {
            uint nodeIndex = m_rootNodeIndex;
            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            {
                nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, key1, key2);
            }

            if (LeafNodeGetValue(nodeIndex, key1, key2, out value1, out value2))
                return;
            throw new Exception("Key Not Found");
        }

        #endregion

        #region [ Abstract Methods ]

        protected abstract Guid FileType { get; }

        #region [ Internal Node Methods ]
        //protected abstract void InternalNodeUpdate(int nodeLevel, uint nodeIndex, TKey oldFirstKey, TKey newFirstKey);

        protected abstract uint InternalNodeGetFirstIndex(int nodeLevel, uint nodeIndex);
        //protected abstract uint InternalNodeGetLastIndex(int nodeLevel, uint nodeIndex);
        protected abstract uint InternalNodeGetIndex(int nodeLevel, uint nodeIndex, long key1, long key2);

        protected abstract void InternalNodeInsert(int nodeLevel, uint nodeIndex, long key1, long key2, uint childNodeIndex);
        protected abstract void InternalNodeCreateNode(uint newNodeIndex, int nodeLevel, uint firstNodeIndex, long dividingKey1, long dividingKey2, uint secondNodeIndex);

        //protected abstract void InternalNodeRemove(int nodeLevel, uint nodeIndex, TKey key);

        #endregion

        #region [ Leaf Node Methods ]

        //protected abstract bool LeafNodeUpdate(uint nodeIndex, TKey key, TValue value);
        protected abstract bool LeafNodeInsert(uint nodeIndex, long key1, long key2, long value1, long value2);
        //protected abstract bool LeafNodeRemove(uint nodeIndex, TKey key);

        protected abstract bool LeafNodeGetValue(uint nodeIndex, long key1, long key2, out long value1, out long value2);
        protected abstract bool LeafNodeGetFirstKeyValue(uint nodeIndex, out long key1, out long key2, out long value1, out long value2);
        //protected abstract bool LeafNodeGetLastKeyValue(uint nodeIndex, out TKey key, out TValue value);
        protected abstract void LeafNodeCreateEmptyNode(uint newNodeIndex);

        protected abstract void LeafNodeScan(uint nodeIndex, long beginKey1, long beingKey2, long endKey1, long endKey2, Func<long, long, long, long, bool> callback);
        protected abstract void LeafNodeScan(uint nodeIndex, long beginKey1, long beingKey2, Func<long, long, long, long, bool> callback);

        #endregion

        #endregion

        #region [ Protected Methods ]

        /// <summary>
        /// Returns the node index address for a freshly allocated block.
        /// The node address is block alligned.
        /// </summary>
        /// <returns></returns>
        protected uint GetNextNewNodeIndex()
        {
            uint newBlock = m_nextUnallocatedBlock;
            m_nextUnallocatedBlock++;
            return newBlock;
        }

        /// <summary>
        /// Notifies the base class that a node was split. This will then add the new node data to the parent.
        /// </summary>
        /// <param name="nodeLevel">the level of the node being added</param>
        /// <param name="nodeIndexOfSplitNode">the index of the existing node that contains the lower half of the data.</param>
        /// <param name="dividingKey">the first key in the <see cref="nodeIndexOfRightSibling"/></param>
        /// <param name="nodeIndexOfRightSibling">the index of the later node</param>
        /// <remarks>This class will add the new node data to the parent node, 
        /// or create a new root if the current root is split.</remarks>
        protected void NodeWasSplit(int nodeLevel, uint nodeIndexOfSplitNode, long dividingKey1, long dividingKey2, uint nodeIndexOfRightSibling)
        {
            if (m_rootNodeLevel > nodeLevel)
            {
                uint nodeIndex = m_rootNodeIndex;
                for (byte level = m_rootNodeLevel; level > nodeLevel + 1; level--)
                {
                    nodeIndex = InternalNodeGetIndex(level, nodeIndex, dividingKey1, dividingKey2);
                }
                InternalNodeInsert(nodeLevel + 1, nodeIndex, dividingKey1, dividingKey2, nodeIndexOfRightSibling);
            }
            else
            {
                m_rootNodeLevel += 1;
                m_rootNodeIndex = GetNextNewNodeIndex();
                InternalNodeCreateNode(m_rootNodeIndex, m_rootNodeLevel, nodeIndexOfSplitNode, dividingKey1, dividingKey2, nodeIndexOfRightSibling);
            }
        }

        //ToDo: Fix this feature
        ///// <summary>
        ///// Notifies the base class that two nodes were rebalanced. On a rebalance, the parent key needs to be updated.
        ///// </summary>
        ///// <param name="nodeLevel">the level of the node being rebalanced</param>
        ///// <param name="oldKeyInLaterBlock">any key that used to be contained in the greater of the two nodes being rebalanced</param>
        ///// <param name="newFirstKeyInLaterBlock">the first key in the greater of the two nodes after being rebalanced</param>
        ///// <remarks>When a child node is rebalanced, it is important to update the parent node to reflect this new change.</remarks>
        //protected void NodeWasRebalanced(byte nodeLevel, TKey oldKeyInLaterBlock, TKey newFirstKeyInLaterBlock)
        //{
        //    if (m_rootNodeLevel > nodeLevel)
        //    {
        //        uint nodeIndex = m_rootNodeIndex;
        //        for (byte level = m_rootNodeLevel; level > nodeLevel + 1; level--)
        //        {
        //            nodeIndex = InternalNodeGetIndex(level, nodeIndex, oldKeyInLaterBlock);
        //        }

        //        InternalNodeUpdate(nodeLevel + 1, nodeIndex, oldKeyInLaterBlock, newFirstKeyInLaterBlock);
        //    }
        //    else
        //    {
        //        throw new Exception("Logic Error: The code should have never entered here");
        //    }
        //}

        //ToDo: Fix this
        ///// <summary>
        ///// When two nodes are combined, the second node needs to be removed from the parent collection.
        ///// </summary>
        ///// <param name="nodeLevel">the level of the node being combined</param>
        ///// <param name="oldKeyInRemovedBlock">A key that used to be contained in the old block that is being removed.</param>
        ///// <param name="nodeToKeep">the node that is being kept</param>
        ///// <param name="nodeToDelete">the node that is being deleted</param>
        //protected void NodeWasCombined(byte nodeLevel, TKey oldKeyInRemovedBlock, uint nodeToKeep, uint nodeToDelete)
        //{
        //    //ToDo: There needs to be some kind of free page allocation routine so when nodes are removed, there location isn't lost.
        //    if (m_rootNodeLevel > nodeLevel)
        //    {
        //        uint nodeIndex = m_rootNodeIndex;
        //        for (byte level = m_rootNodeLevel; level > nodeLevel + 1; level--)
        //        {
        //            nodeIndex = InternalNodeGetIndex(level, nodeIndex, oldKeyInRemovedBlock);
        //        }
        //        InternalNodeRemove(nodeLevel + 1, nodeIndex, oldKeyInRemovedBlock);
        //    }
        //    else
        //    {
        //        m_rootNodeLevel = 0;
        //        m_rootNodeIndex = nodeToKeep;
        //    }
        //}

        #endregion

        #region [ Private Methods ]

        void LoadHeader()
        {
            Stream.Position = 0;
            if (FileType != Stream.ReadGuid())
                throw new Exception("Header Corrupt");
            if (Stream.ReadByte() != 0)
                throw new Exception("Header Corrupt");
            m_nextUnallocatedBlock = Stream.ReadUInt32();
            m_blockSize = Stream.ReadInt32();
            m_rootNodeIndex = Stream.ReadUInt32();
            m_rootNodeLevel = Stream.ReadByte();
        }
        void SaveHeader()
        {
            Stream.Position = 0;
            Stream.Write(FileType);
            Stream.Write((byte)0); //Version
            Stream.Write(m_nextUnallocatedBlock);
            Stream.Write(BlockSize);
            Stream.Write(m_rootNodeIndex); //Root Index
            Stream.Write(m_rootNodeLevel); //Root Index
        }

        protected static int CompareKeys(long firstKey1, long firstKey2, long secondKey1, long secondKey2)
        {
            if (firstKey1 > secondKey1) return 1;
            if (firstKey1 < secondKey1) return -1;

            if (firstKey2 > secondKey2) return 1;
            if (firstKey2 < secondKey2) return -1;

            return 0;
        }


        #endregion



    }
}
