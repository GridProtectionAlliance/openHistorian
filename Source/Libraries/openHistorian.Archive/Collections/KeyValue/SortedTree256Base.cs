//******************************************************************************************************
//  SortedTree256Base.cs - Gbtc
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
using openHistorian.IO;

namespace openHistorian.Collections.KeyValue
{

    /// <summary>
    /// Provides the basic user methods with any derived B+Tree.  
    /// This base class translates all of the core methods into simple methods
    /// that must be implemented by classes derived from this base class.
    /// </summary>
    /// <remarks>
    /// This class does not support concurrent read operations.  This is due to the caching method of each tree.
    /// If concurrent read operations are desired, clone the tree.  
    /// Trees cannot be cloned if the user plans to write to the tree.
    /// </remarks>
    public abstract class SortedTree256Base
    {

        #region [ Members ]

        bool m_skipIntermediateSaves;
        bool m_nodeHeaderChanged;
        byte m_rootNodeLevel;
        int m_blockSize;
        long m_rootNodeIndexAddress;
        long m_lastAllocatedBlock;
        ulong m_firstKey;
        ulong m_lastKey;
        SortedTree256Cache m_cache;
        BinaryStreamBase m_stream1;
        BinaryStreamBase m_stream2;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Opens an existing <see cref="SortedTree256Base"/> from the stream.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        protected SortedTree256Base(BinaryStreamBase stream1, BinaryStreamBase stream2)
        {
            m_cache = new SortedTree256Cache();
            m_stream1 = stream1;
            m_stream2 = stream2;
            LoadHeader();
        }

        /// <summary>
        /// Creates an empty <see cref="SortedTree256Base"/>
        /// and uses the underlying stream to save data to it.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        /// <param name="blockSize">the size of one block.  This should exactly match the
        /// amount of data space available in the underlying data object. BPlus trees get their 
        /// performance benefit because there is fewer I/O's required to find and insert blocks.</param>
        protected SortedTree256Base(BinaryStreamBase stream1, BinaryStreamBase stream2, int blockSize)
        {
            m_cache = new SortedTree256Cache();
            m_stream1 = stream1;
            m_stream2 = stream2;
            m_blockSize = blockSize;
            m_rootNodeLevel = 0;
            m_rootNodeIndexAddress = 1;
            m_lastAllocatedBlock = 1;
            m_firstKey = ulong.MaxValue;
            m_lastKey = ulong.MinValue;
            LeafNodeCreateEmptyNode(m_rootNodeIndexAddress);
            SaveHeader();
            LoadHeader();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The sorted tree will not continuely call the Save method every time the header is changed.
        /// When setting this to true, the user must always manually call the <see cref="Save"/> method
        /// after making changes to this tree
        /// </summary>
        internal bool SkipIntermediateSaves
        {
            get
            {
                return m_skipIntermediateSaves;
            }
            set
            {
                m_skipIntermediateSaves = value;
            }
        }

        /// <summary>
        /// Determines if the tree has any data in it.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return m_firstKey > m_lastKey;
            }
        }

        /// <summary>
        /// The first key in the tree.  
        /// If the first key is after the last key, 
        /// there is no data in the tree.
        /// </summary>
        public ulong FirstKey
        {
            get
            {
                return m_firstKey;
            }
        }

        /// <summary>
        /// The last key in the tree.  
        /// If the first key is after the last key, 
        /// there is no data in the tree.
        /// </summary>
        public ulong LastKey
        {
            get
            {
                return m_lastKey;
            }
        }

        /// <summary>
        /// Contains the stream for reading and writing and optional cloning.
        /// </summary>
        protected BinaryStreamBase Stream
        {
            get
            {
                return m_stream1;
            }
        }

        /// <summary>
        /// Contains the stream for reading and writing and optional cloning.
        /// </summary>
        protected BinaryStreamBase StreamLeaf
        {
            get
            {
                return m_stream2;
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

        /// <summary>
        /// Gets the address of the root node.
        /// </summary>
        protected long RootNodeIndexAddress
        {
            get
            {
                return m_rootNodeIndexAddress;
            }
        }

        /// <summary>
        /// Gets the level of the root node.
        /// </summary>
        protected byte RootNodeLevel
        {
            get
            {
                return m_rootNodeLevel;
            }
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Saves any header data that may have changed.
        /// </summary>
        public void Save()
        {
            SaveHeader();
        }

        /// <summary>
        /// Adds the following data to the tree.
        /// </summary>
        /// <param name="key1">The unique key value.</param>
        /// <param name="key2">The unique key value.</param>
        /// <param name="value1">The value to insert.</param>
        /// <param name="value2">The value to insert.</param>
        public void Add(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            //m_cache.ClearCache();

            if (key1 < m_firstKey)
            {
                m_firstKey = key1;
                m_nodeHeaderChanged = true;
            }
            if (key1 > m_lastKey)
            {
                m_lastKey = key1;
                m_nodeHeaderChanged = true;
            }
            long nodeIndexAddress = m_rootNodeIndexAddress;
            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            {
                if (!m_cache.NodeContains(nodeLevel, key1, key2, ref nodeIndexAddress))
                {
                    NodeDetails nodeDetails = InternalNodeGetNodeIndexAddressBucket(nodeLevel, nodeIndexAddress, key1, key2);
                    nodeIndexAddress = nodeDetails.NodeIndex;
                    m_cache.CacheNode(nodeLevel, nodeDetails);
                }
            }
            if (LeafNodeInsert(nodeIndexAddress, key1, key2, value1, value2))
            {
                if (!m_skipIntermediateSaves && m_nodeHeaderChanged)
                    SaveHeader();
                return;
            }

            throw new Exception("Key already exists");
        }

        /// <summary>
        /// Adds the data contained in the <see cref="treeScanner"/> to this tree.
        /// </summary>
        /// <param name="treeScanner"></param>
        /// <remarks>The tree is only read in order. No seeking of the tree occurs.</remarks>
        public void Add(ITreeScanner256 treeScanner)
        {
            ulong key1, key2, value1, value2;
            var isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
            ulong minKey = m_firstKey;
            ulong maxKey = m_lastKey;

            while (isValid)
            {
                //Search for the proper save node
                long nodeIndexAddress = m_rootNodeIndexAddress;
                for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
                {
                    if (!m_cache.NodeContains(nodeLevel, key1, key2, ref nodeIndexAddress))
                    {
                        NodeDetails nodeDetails = InternalNodeGetNodeIndexAddressBucket(nodeLevel, nodeIndexAddress, key1, key2);
                        nodeIndexAddress = nodeDetails.NodeIndex;
                        m_cache.CacheNode(nodeLevel, nodeDetails);
                    }
                }

                if (!LeafNodeInsert(nodeIndexAddress, treeScanner, ref key1, ref key2, ref value1, ref value2, ref isValid, ref maxKey, ref minKey))
                    throw new Exception("Key already exists");

            }

            if (minKey < m_firstKey)
            {
                m_firstKey = minKey;
                m_nodeHeaderChanged = true;
            }
            if (maxKey > m_lastKey)
            {
                m_lastKey = maxKey;
                m_nodeHeaderChanged = true;
            }

            if (!m_skipIntermediateSaves && m_nodeHeaderChanged)
                SaveHeader();

        }

        /// <summary>
        /// Gets the data for the following key. 
        /// </summary>
        /// <param name="key1">The key to look up.</param>
        /// <param name="key2">The key to look up.</param>
        /// <param name="value1">the value output</param>
        /// <param name="value2">the value output</param>
        public void Get(ulong key1, ulong key2, out ulong value1, out ulong value2)
        {
            long nodeIndex = m_rootNodeIndexAddress;
            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            {
                nodeIndex = InternalNodeGetNodeIndexAddress(nodeLevel, nodeIndex, key1, key2);
            }

            if (LeafNodeGetValue(nodeIndex, key1, key2, out value1, out value2))
                return;
            throw new Exception("Key Not Found");
        }

        /// <summary>
        /// Returns a <see cref="ITreeScanner256"/> that can be used to parse throught the tree.
        /// </summary>
        /// <returns></returns>
        public ITreeScanner256 GetTreeScanner()
        {
            return LeafNodeGetScanner();
        }

        #endregion

        #region [ Abstract Methods ]

        /// <summary>
        /// Gets the type of tree that exists. 
        /// Each encoding type should be unique.  
        /// </summary>
        protected abstract Guid FileType { get; }

        #region [ Internal Node Methods ]

        protected abstract long InternalNodeGetNodeIndexAddress(byte nodeLevel, long nodeIndex, ulong key1, ulong key2);
        protected abstract NodeDetails InternalNodeGetNodeIndexAddressBucket(byte nodeLevel, long nodeIndex, ulong key1, ulong key2);
        protected abstract void InternalNodeInsert(byte nodeLevel, long nodeIndex, ulong key1, ulong key2, long childNodeIndex);
        protected abstract void InternalNodeCreateNode(long newNodeIndex, byte nodeLevel, long firstNodeIndex, ulong dividingKey1, ulong dividingKey2, long secondNodeIndex);

        #endregion

        #region [ Leaf Node Methods ]

        protected abstract bool LeafNodeInsert(long nodeIndex, ITreeScanner256 treeScanner, ref ulong key1, ref ulong key2, ref ulong value1, ref ulong value2, ref bool isValid, ref ulong maxKey, ref ulong minKey);
        protected abstract bool LeafNodeInsert(long nodeIndex, ulong key1, ulong key2, ulong value1, ulong value2);
        protected abstract bool LeafNodeGetValue(long nodeIndex, ulong key1, ulong key2, out ulong value1, out ulong value2);
        protected abstract void LeafNodeCreateEmptyNode(long newNodeIndex);
        protected abstract ITreeScanner256 LeafNodeGetScanner();

        #endregion

        #endregion

        #region [ Protected Methods ]

        /// <summary>
        /// Returns the node index address for a freshly allocated block.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Also saves the header data</remarks>
        protected long GetNextNewNodeIndex()
        {
            m_lastAllocatedBlock++;
            m_nodeHeaderChanged = true;
            return m_lastAllocatedBlock;
        }

        /// <summary>
        /// Notifies the base class that a node was split. This will then add the new node data to the parent.
        /// </summary>
        /// <param name="nodeLevel">the level of the node being added</param>
        /// <param name="nodeIndexOfSplitNode">the index of the existing node that contains the lower half of the data.</param>
        /// <param name="dividingKey1">the first key in the <see cref="nodeIndexOfRightSibling"/></param>
        /// <param name="dividingKey2">the first key in the <see cref="nodeIndexOfRightSibling"/></param>
        /// <param name="nodeIndexOfRightSibling">the index of the later node</param>
        /// <remarks>This class will add the new node data to the parent node, 
        /// or create a new root if the current root is split.</remarks>
        protected void NodeWasSplit(byte nodeLevel, long nodeIndexOfSplitNode, ulong dividingKey1, ulong dividingKey2, long nodeIndexOfRightSibling)
        {
            //m_cache.ClearCache();
            if (m_rootNodeLevel > nodeLevel)
            {
                m_cache.InvalidateCache(nodeLevel);

                long nodeIndex = m_rootNodeIndexAddress;
                for (byte level = m_rootNodeLevel; level > nodeLevel + 1; level--)
                {
                    if (!m_cache.NodeContains(level, dividingKey1, dividingKey2, ref nodeIndex))
                    {
                        NodeDetails nodeDetails = InternalNodeGetNodeIndexAddressBucket(level, nodeIndex, dividingKey1, dividingKey2);
                        nodeIndex = nodeDetails.NodeIndex;
                        m_cache.CacheNode(nodeLevel, nodeDetails);
                    }
                }

                m_cache.InvalidateCache(nodeLevel + 1);
                InternalNodeInsert((byte)(nodeLevel + 1), nodeIndex, dividingKey1, dividingKey2, nodeIndexOfRightSibling);
            }
            else
            {
                m_cache.ClearCache();
                m_rootNodeLevel += 1;
                m_rootNodeIndexAddress = GetNextNewNodeIndex();
                InternalNodeCreateNode(m_rootNodeIndexAddress, m_rootNodeLevel, nodeIndexOfSplitNode, dividingKey1, dividingKey2, nodeIndexOfRightSibling);
                SaveHeader();
            }
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Loads the header.
        /// </summary>
        void LoadHeader()
        {
            Stream.Position = 0;
            if (FileType != Stream.ReadGuid())
                throw new Exception("Header Corrupt");
            if (Stream.ReadByte() != 0)
                throw new Exception("Header Corrupt");
            m_lastAllocatedBlock = Stream.ReadInt64();
            m_blockSize = Stream.ReadInt32();
            m_rootNodeIndexAddress = Stream.ReadInt64();
            m_rootNodeLevel = Stream.ReadByte();
            m_firstKey = Stream.ReadUInt64();
            m_lastKey = Stream.ReadUInt64();
        }

        /// <summary>
        /// Writes the first page of the bplus tree.
        /// </summary>
        void SaveHeader()
        {
            m_nodeHeaderChanged = false;

            long oldPosotion = Stream.Position;
            Stream.Position = 0;
            Stream.Write(FileType);
            Stream.Write((byte)0); //version
            Stream.Write(m_lastAllocatedBlock);
            Stream.Write(m_blockSize);
            Stream.Write(m_rootNodeIndexAddress); //Root Index
            Stream.Write(m_rootNodeLevel); //Root Index
            Stream.Write(m_firstKey);
            Stream.Write(m_lastKey);
            Stream.Position = oldPosotion;
        }
        
        /// <summary>
        /// Compares one key to another key to determine which is greater
        /// </summary>
        /// <returns>1 if the first key is greater. -1 if the second key is greater. 0 if the keys are equal.</returns>
        protected static int CompareKeys(ulong firstKey1, ulong firstKey2, ulong secondKey1, ulong secondKey2)
        {
            if (firstKey1 > secondKey1) return 1;
            if (firstKey1 < secondKey1) return -1;

            if (firstKey2 > secondKey2) return 1;
            if (firstKey2 < secondKey2) return -1;

            return 0;
        }

        //ToDo: Implement these shortcuts

        /// <summary>
        /// Returns true if the first key is greater than or equal to the later key
        /// </summary>
        protected static bool IsGreaterThanOrEqualTo(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 > compareKey1) | ((key1 == compareKey1) & (key2 >= compareKey2));
        }

        /// <summary>
        /// Returns true if the first key is greater than the later key.
        /// </summary>
        protected static bool IsGreaterThan(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 > compareKey1) | ((key1 == compareKey1) & (key2 > compareKey2));
        }

        /// <summary>
        /// Returns true if the first key is less than or equal to the later key
        /// </summary>
        protected static bool IsLessThanOrEqualTo(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 < compareKey1) | ((key1 == compareKey1) & (key2 <= compareKey2));
        }

        /// <summary>
        /// Returns true if the first key is less than the later key.
        /// </summary>
        protected static bool IsLessThan(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 < compareKey1) | ((key1 == compareKey1) & (key2 < compareKey2));
        }

        protected static bool IsEqual(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 == compareKey1) & (key2 == compareKey2);
        }

        protected static bool IsNotEqual(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 != compareKey1) | (key2 != compareKey2);
        }


        #endregion



    }
}
