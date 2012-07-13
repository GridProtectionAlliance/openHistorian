//******************************************************************************************************
//  BasicTreeBase.cs - Gbtc
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
    /// <remarks>
    /// This class does not support concurrent read operations.  This is due to the caching method of each tree.
    /// If concurrent read operations are desired, clone the tree.  
    /// Trees cannot be cloned if the user plans to write to the tree.
    /// </remarks>
    public abstract class BasicTreeBase
    {
        BucketInfo[] m_cache;

        protected struct BucketInfo
        {
            public ulong LowerKey1;
            public ulong LowerKey2;
            public ulong UpperKey1;
            public ulong UpperKey2;
            public long NodeIndex;
            public bool IsLowerNull;
            public bool IsUpperNull;
            public bool IsValid;
            public bool Contains(ulong key1, ulong key2)
            {
                //return false;
                return IsValid &&
                    (IsLowerNull || (CompareKeys(LowerKey1, LowerKey2, key1, key2) <= 0)) &&
                    (IsUpperNull || (CompareKeys(key1, key2, UpperKey1, UpperKey2) < 0));
            }
        }

        #region [ Members ]

        byte m_rootNodeLevel;
        int m_blockSize;
        long m_rootNodeIndexAddress;
        long m_lastAllocatedBlock;
        ulong m_firstKey;
        ulong m_lastKey;

        IBinaryStream m_stream;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Opens an existing <see cref="BasicTreeBase"/> from the stream.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        protected BasicTreeBase(IBinaryStream stream)
        {
            m_cache = new BucketInfo[5];
            ClearCache();
            m_stream = stream;
            LoadHeader();
        }

        /// <summary>
        /// Creates an empty <see cref="BasicTreeBase"/>
        /// and uses the underlying stream to save data to it.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        /// <param name="blockSize">the size of one block.  This should exactly match the
        /// amount of data space available in the underlying data object. BPlus trees get their 
        /// performance benefit because there is fewer I/O's required to find and insert blocks.</param>
        protected BasicTreeBase(IBinaryStream stream, int blockSize)
        {
            m_cache = new BucketInfo[5];
            ClearCache();
            m_stream = stream;
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

        protected long RootNodeIndexAddress
        {
            get
            {
                return m_rootNodeIndexAddress;
            }
        }

        protected byte RootNodeLevel
        {
            get
            {
                return m_rootNodeLevel;
            }
        }

        #endregion

        #region [ Public Methods ]

        //Old one that wasn't cached.
        ///// <summary>
        ///// Inserts the following data into the tree.
        ///// </summary>
        ///// <param name="key1">The unique key value.</param>
        ///// <param name="key2">The unique key value.</param>
        ///// <param name="value1">The value to insert.</param>
        ///// <param name="value2">The value to insert.</param>
        //public void Add(ulong key1, ulong key2, ulong value1, ulong value2)
        //{
        //    if (key1 < m_firstKey)
        //        m_firstKey = key1;
        //    if (key1 > m_lastKey)
        //        m_lastKey = key1;

        //    long nodeIndexAddress = m_rootNodeIndexAddress;
        //    for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
        //    {
        //        nodeIndexAddress = InternalNodeGetNodeIndexAddress(nodeLevel, nodeIndexAddress, key1, key2);
        //    }
        //    if (LeafNodeInsert(nodeIndexAddress, key1, key2, value1, value2))
        //        return;
        //    throw new Exception("Key already exists");
        //}

        //public static long ShortcutsTaken = 0;
        //public static long PointsAdded = 0;

        /// <summary>
        /// Inserts the following data into the tree.
        /// </summary>
        /// <param name="key1">The unique key value.</param>
        /// <param name="key2">The unique key value.</param>
        /// <param name="value1">The value to insert.</param>
        /// <param name="value2">The value to insert.</param>
        public void Add(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            if (key1 < m_firstKey)
                m_firstKey = key1;
            if (key1 > m_lastKey)
                m_lastKey = key1;

            long nodeIndexAddress = m_rootNodeIndexAddress;
            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            {
                //PointsAdded++;
                BucketInfo currentBucket = m_cache[nodeLevel - 1];
                if (currentBucket.Contains(key1, key2))
                {
                    //ShortcutsTaken++;
                    nodeIndexAddress = currentBucket.NodeIndex;
                }
                else
                {
                    Array.Clear(m_cache, 0, nodeLevel);
                    currentBucket = InternalNodeGetNodeIndexAddressBucket(nodeLevel, nodeIndexAddress, key1, key2);
                    nodeIndexAddress = currentBucket.NodeIndex;
                    m_cache[nodeLevel - 1] = currentBucket;
                }
            }
            if (LeafNodeInsert(nodeIndexAddress, key1, key2, value1, value2))
                return;
            throw new Exception("Key already exists");
        }

        //public void AddRange(IDataScanner dataScanner)
        //{
        //    ulong key1, key2, value1, value2;
        //    dataScanner.SeekToKey(0, 0);
        //    var isValid = dataScanner.GetNextKey(out key1, out key2, out value1, out value2);
        //    if (key1 < m_firstKey)
        //        m_firstKey = key1;

        //    while (isValid)
        //    {
        //        //Search for the proper save node
        //        long nodeIndexAddress = m_rootNodeIndexAddress;
        //        for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
        //        {
        //            nodeIndexAddress = InternalNodeGetNodeIndexAddress(nodeLevel, nodeIndexAddress, key1, key2);
        //        }

        //        if (!LeafNodeInsert(dataScanner, nodeIndexAddress, ref key1, ref key2, ref value1, ref value2, ref isValid))
        //            throw new Exception("Key already exists");

        //    }
        //    if (key1 > m_lastKey)
        //        m_lastKey = key1;

        //}

        /// <summary>
        /// Returns the data for the following key. 
        /// </summary>
        /// <param name="key1">The key to look up.</param>
        /// <param name="key2">The key to look up.</param>
        /// <param name="value1">the value output</param>
        /// <param name="value2">the value output</param>
        /// <returns>Null or the Default structure value if the key does not exist.</returns>
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
        /// Returns a <see cref="IDataScanner"/> that can be used to parse throught the tree.
        /// </summary>
        /// <returns></returns>
        public IDataScanner GetDataRange()
        {
            return LeafNodeGetScanner();
        }

        #endregion

        #region [ Abstract Methods ]

        protected abstract Guid FileType { get; }

        #region [ Internal Node Methods ]

        protected abstract long InternalNodeGetNodeIndexAddress(byte nodeLevel, long nodeIndex, ulong key1, ulong key2);
        protected abstract BucketInfo InternalNodeGetNodeIndexAddressBucket(byte nodeLevel, long nodeIndex, ulong key1, ulong key2);
        protected abstract void InternalNodeInsert(byte nodeLevel, long nodeIndex, ulong key1, ulong key2, long childNodeIndex);
        protected abstract void InternalNodeCreateNode(long newNodeIndex, byte nodeLevel, long firstNodeIndex, ulong dividingKey1, ulong dividingKey2, long secondNodeIndex);

        #endregion

        #region [ Leaf Node Methods ]

        //protected abstract bool LeafNodeInsert(IDataScanner dataScanner, long nodeIndex, ref ulong key1, ref ulong key2, ref ulong value1, ref ulong value2, ref bool isValid);
        protected abstract bool LeafNodeInsert(long nodeIndex, ulong key1, ulong key2, ulong value1, ulong value2);
        protected abstract bool LeafNodeGetValue(long nodeIndex, ulong key1, ulong key2, out ulong value1, out ulong value2);
        protected abstract void LeafNodeCreateEmptyNode(long newNodeIndex);
        protected abstract IDataScanner LeafNodeGetScanner();

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
            SaveHeader();
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
            ClearCache();
            if (m_rootNodeLevel > nodeLevel)
            {
                long nodeIndex = m_rootNodeIndexAddress;
                for (byte level = m_rootNodeLevel; level > nodeLevel + 1; level--)
                {
                    nodeIndex = InternalNodeGetNodeIndexAddress(level, nodeIndex, dividingKey1, dividingKey2);
                }
                InternalNodeInsert((byte)(nodeLevel + 1), nodeIndex, dividingKey1, dividingKey2, nodeIndexOfRightSibling);
            }
            else
            {
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
        /// <param name="firstKey1"></param>
        /// <param name="firstKey2"></param>
        /// <param name="secondKey1"></param>
        /// <param name="secondKey2"></param>
        /// <returns>1 if the first key is greater. -1 if the second key is greater. 0 if the keys are equal.</returns>
        protected static int CompareKeys(ulong firstKey1, ulong firstKey2, ulong secondKey1, ulong secondKey2)
        {
            if (firstKey1 > secondKey1) return 1;
            if (firstKey1 < secondKey1) return -1;

            if (firstKey2 > secondKey2) return 1;
            if (firstKey2 < secondKey2) return -1;

            return 0;
        }

        void ClearCache()
        {
            Array.Clear(m_cache, 0, 5);
        }


        #endregion



    }
}
