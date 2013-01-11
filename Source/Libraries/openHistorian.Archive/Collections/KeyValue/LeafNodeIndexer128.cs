//******************************************************************************************************
//  LeafNodeIndexer128.cs - Gbtc
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
//  1/11/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using openHistorian.IO;

namespace openHistorian.Collections.KeyValue
{
    public partial class LeafNodeIndexer128
    {
        /// <summary>
        /// Assists in the read/write operations of the header of a node.
        /// </summary>
        struct NodeHeader
        {
            public const int Size = 21;
            public byte NodeLevel;
            public int NodeRecordCount;
            public long LeftSiblingNodeIndex;
            public long RightSiblingNodeIndex;

            public NodeHeader(BinaryStreamBase stream, int blockSize, long nodeIndex, byte expectedNodeLevel)
            {
                stream.Position = blockSize * nodeIndex;
                NodeLevel = stream.ReadByte();
                NodeRecordCount = stream.ReadInt32();
                LeftSiblingNodeIndex = stream.ReadInt64();
                RightSiblingNodeIndex = stream.ReadInt64();

                if (NodeLevel != expectedNodeLevel)
                    throw new Exception("The current node is not an internal node.");
            }
            public void Save(BinaryStreamBase stream, int blockSize, long nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                stream.Write(NodeLevel);
                stream.Write(NodeRecordCount);
                stream.Write(LeftSiblingNodeIndex);
                stream.Write(RightSiblingNodeIndex);
            }

            public static void Save(BinaryStreamBase stream, byte nodeLevel, int nodeRecordCount, long leftSiblingNodeIndex, long rightSiblingNodeIndex)
            {
                stream.Write(nodeLevel);
                stream.Write(nodeRecordCount);
                stream.Write(leftSiblingNodeIndex);
                stream.Write(rightSiblingNodeIndex);
            }

        }

        #region [ Members ]

        bool m_hasChanged;
        byte m_rootNodeLevel;
        const int KeySize = sizeof(ulong) + sizeof(ulong);
        const int StructureSize = KeySize + sizeof(long);
        int m_maximumRecordsPerNode;
        int m_blockSize;
        long m_rootNodeIndexAddress;
        IndexCache m_cache;
        BinaryStreamBase m_stream;
        Func<long> m_getNextNewNodeIndex;
        
        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an indexer that will index a 128 bit value. 
        /// </summary>
        /// <param name="stream">The stream to use to write the indexer</param>
        /// <param name="blockSize">The size of each block that will be used by this indexer.</param>
        /// <param name="rootNodeLevel">The level of the root node. If this is zero, 
        /// that means there is no index and this tree will pass though the <see cref="rootNodeIndexAddress"/>.</param>
        /// <param name="rootNodeIndexAddress">The index of the root node.</param>
        /// <param name="getNextNewNodeIndex">A method </param>
        public LeafNodeIndexer128(BinaryStreamBase stream, int blockSize, byte rootNodeLevel, long rootNodeIndexAddress, Func<long> getNextNewNodeIndex)
        {
            m_stream = stream;
            m_getNextNewNodeIndex = getNextNewNodeIndex;
            m_cache = new IndexCache();
            m_blockSize = blockSize;
            m_rootNodeLevel = rootNodeLevel;
            m_rootNodeIndexAddress = rootNodeIndexAddress;
            m_maximumRecordsPerNode = (m_blockSize - NodeHeader.Size - sizeof(long)) / StructureSize;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if the node's state variables have changed and need to be saved.
        /// </summary>
        public bool HasChanged
        {
            get
            {
                return m_hasChanged;
            }
        }
        /// <summary>
        /// Gets the state variables that need to be changed.
        /// </summary>
        /// <param name="rootNodeLevel"></param>
        /// <param name="rootNodeIndexAddress"></param>
        public void Save(out byte rootNodeLevel, out long rootNodeIndexAddress)
        {
            m_hasChanged = false;
            rootNodeLevel = m_rootNodeLevel;
            rootNodeIndexAddress = m_rootNodeIndexAddress;
        }

        #endregion

        #region [ Public Methods ]


        /// <summary>
        /// Gets the data for the following key. 
        /// </summary>
        /// <param name="key1">The key to look up.</param>
        /// <param name="key2">The key to look up.</param>
        public long Get(ulong key1, ulong key2)
        {
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
            return nodeIndexAddress;

            //long nodeIndex = m_rootNodeIndexAddress;
            //for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
            //{
            //    nodeIndex = InternalNodeGetNodeIndexAddress(nodeLevel, nodeIndex, key1, key2);
            //}
            //return nodeIndex;
        }

        #endregion

        #region [ Abstract Methods ]

        #region [ Internal Node Methods ]

        long InternalNodeGetNodeIndexAddress(byte nodeLevel, long nodeIndex, ulong key1, ulong key2)
        {
            int offset;
            var header = new NodeHeader(m_stream, m_blockSize, nodeIndex, nodeLevel);
            if (FindOffsetOfKey(nodeIndex, header.NodeRecordCount, key1, key2, out offset))
            {
                //An exact match was found, return the value that is currently being pointed to.
                m_stream.Position = nodeIndex * m_blockSize + offset + KeySize;
                return m_stream.ReadInt64();
            }
            else
            {
                m_stream.Position = nodeIndex * m_blockSize + (offset - sizeof(long));
                return m_stream.ReadInt64();
            }
        }

        NodeDetails InternalNodeGetNodeIndexAddressBucket(byte nodeLevel, long nodeIndex, ulong key1, ulong key2)
        {
            var header = new NodeHeader(m_stream, m_blockSize, nodeIndex, nodeLevel);
            return FindOffsetOfKey(nodeIndex, header.NodeRecordCount, key1, key2);
        }

        void InternalNodeInsert(byte nodeLevel, long nodeIndex, ulong key1, ulong key2, long childNodeIndex)
        {
            int offset;
            var header = new NodeHeader(m_stream, m_blockSize, nodeIndex, nodeLevel);

            //Find the best location to insert
            //This is done before checking if a split is required to prevent splitting 
            //if a duplicate key is found
            if (FindOffsetOfKey(nodeIndex, header.NodeRecordCount, key1, key2, out offset))
                throw new Exception("Duplicate Key");

            //Check if the node needs to be split
            if (header.NodeRecordCount >= m_maximumRecordsPerNode)
            {
                SplitNodeThenInsert(key1, key2, childNodeIndex, nodeIndex, nodeLevel);
                return;
            }

            //set the stream's position to the best insert location.
            m_stream.Position = nodeIndex * m_blockSize + offset;

            int bytesAfterInsertPositionToShift = NodeHeader.Size + sizeof(long) + StructureSize * header.NodeRecordCount - offset;
            if (bytesAfterInsertPositionToShift > 0)
            {
                m_stream.InsertBytes(StructureSize, bytesAfterInsertPositionToShift);
            }

            //Insert the data
            m_stream.Write(key1);
            m_stream.Write(key2);
            m_stream.Write(childNodeIndex);

            //save the header
            header.NodeRecordCount++;
            header.Save(m_stream, m_blockSize, nodeIndex);
        }


        void InternalNodeCreateNode(long newNodeIndex, byte nodeLevel, long firstNodeIndex, ulong dividingKey1, ulong dividingKey2, long secondNodeIndex)
        {
            m_stream.Position = newNodeIndex * m_blockSize;
            NodeHeader.Save(m_stream, nodeLevel, 1, 0, 0);
            m_stream.Write(firstNodeIndex);
            m_stream.Write(dividingKey1);
            m_stream.Write(dividingKey2);
            m_stream.Write(secondNodeIndex);
        }
        #endregion

        #endregion

        #region [ Protected Methods ]

        /// <summary>
        /// Notifies the base class that a node was split. This will then add the new node data to the parent.
        /// </summary>
        /// <param name="nodeIndexOfSplitNode">the index of the existing node that contains the lower half of the data.</param>
        /// <param name="dividingKey1">the first key in the <see cref="nodeIndexOfRightSibling"/></param>
        /// <param name="dividingKey2">the first key in the <see cref="nodeIndexOfRightSibling"/></param>
        /// <param name="nodeIndexOfRightSibling">the index of the later node</param>
        /// <remarks>This class will add the new node data to the parent node, 
        /// or create a new root if the current root is split.</remarks>
        public void NodeWasSplit(long nodeIndexOfSplitNode, ulong dividingKey1, ulong dividingKey2, long nodeIndexOfRightSibling)
        {
            NodeWasSplit(0, nodeIndexOfSplitNode, dividingKey1, dividingKey2, nodeIndexOfRightSibling);
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
        void NodeWasSplit(byte nodeLevel, long nodeIndexOfSplitNode, ulong dividingKey1, ulong dividingKey2, long nodeIndexOfRightSibling)
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
                m_hasChanged = true;
            }
        }

        long GetNextNewNodeIndex()
        {
            return m_getNextNewNodeIndex();
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="nodeIndex">the index of the node to search</param>
        /// <param name="nodeRecordCount">the number of records already in the current node</param>
        /// <param name="key1">the key to search for</param>
        /// <param name="key2">the key to search for</param>
        /// <param name="offset">the offset from the start of the node where the index was found</param>
        /// <returns>true the key was found in the node, false if was not found. 
        /// If false, the offset is where the key could be inserted</returns>
        /// <remarks>
        /// Search method is a binary search algorithm
        /// </remarks>
        bool FindOffsetOfKey(long nodeIndex, int nodeRecordCount, ulong key1, ulong key2, out int offset)
        {
            long addressOfFirstKey = nodeIndex * m_blockSize + NodeHeader.Size + sizeof(long);
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = nodeRecordCount - 1;

            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);
                m_stream.Position = addressOfFirstKey + StructureSize * currentTestIndex;

                ulong compareKey1 = m_stream.ReadUInt64();
                ulong compareKey2 = m_stream.ReadUInt64();

                int compareKeysResults = CompareKeys(key1, key2, compareKey1, compareKey2);
                if (compareKeysResults == 0)
                {
                    offset = NodeHeader.Size + sizeof(long) + StructureSize * currentTestIndex;
                    return true;
                }
                if (compareKeysResults > 0)
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }
            offset = NodeHeader.Size + sizeof(long) + StructureSize * searchLowerBoundsIndex;
            return false;
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="nodeIndex">the index of the node to search</param>
        /// <param name="nodeRecordCount">the number of records already in the current node</param>
        /// <param name="key1">the key to search for</param>
        /// <param name="key2">the key to search for</param>
        /// <returns>data about the bucket</returns>
        /// <remarks>
        /// Search method is a binary search algorithm
        /// </remarks>
        NodeDetails FindOffsetOfKey(long nodeIndex, int nodeRecordCount, ulong key1, ulong key2)
        {
            NodeDetails nodeDetails = default(NodeDetails);
            nodeDetails.IsValid = true;

            long addressOfFirstKey = nodeIndex * m_blockSize + NodeHeader.Size + sizeof(long);
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = nodeRecordCount - 1;

            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);
                m_stream.Position = addressOfFirstKey + StructureSize * currentTestIndex;

                ulong compareKey1 = m_stream.ReadUInt64();
                ulong compareKey2 = m_stream.ReadUInt64();

                int compareKeysResults = CompareKeys(key1, key2, compareKey1, compareKey2);
                if (compareKeysResults == 0)
                {
                    //offset = NodeHeader.Size + sizeof(long) + StructureSize * currentTestIndex;
                    nodeDetails.LowerKey1 = compareKey1;
                    nodeDetails.LowerKey2 = compareKey2;
                    nodeDetails.IsLowerNull = false;
                    nodeDetails.NodeIndex = m_stream.ReadInt64();

                    if (currentTestIndex == nodeRecordCount)
                    {
                        nodeDetails.IsUpperNull = true;
                    }
                    else
                    {
                        nodeDetails.IsUpperNull = false;
                        nodeDetails.UpperKey1 = m_stream.ReadUInt64();
                        nodeDetails.UpperKey2 = m_stream.ReadUInt64();
                    }
                    return nodeDetails;
                }
                if (compareKeysResults > 0)
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            if (searchLowerBoundsIndex == 0)
            {
                m_stream.Position = addressOfFirstKey - sizeof(long);
                nodeDetails.IsLowerNull = true;
                nodeDetails.NodeIndex = m_stream.ReadInt64();
            }
            else
            {
                m_stream.Position = addressOfFirstKey + searchLowerBoundsIndex * StructureSize - sizeof(long) - KeySize;
                nodeDetails.IsLowerNull = false;
                nodeDetails.LowerKey1 = m_stream.ReadUInt64();
                nodeDetails.LowerKey2 = m_stream.ReadUInt64();
                nodeDetails.NodeIndex = m_stream.ReadInt64();
            }

            if (searchLowerBoundsIndex == nodeRecordCount)
            {
                nodeDetails.IsUpperNull = true;
            }
            else
            {
                nodeDetails.IsUpperNull = false;
                nodeDetails.UpperKey1 = m_stream.ReadUInt64();
                nodeDetails.UpperKey2 = m_stream.ReadUInt64();
            }

            return nodeDetails;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        void SplitNodeThenInsert(ulong key1, ulong key2, long value, long firstNodeIndex, byte nodeLevel)
        {
            NodeHeader firstNodeHeader = new NodeHeader(m_stream, m_blockSize, firstNodeIndex, nodeLevel);
            NodeHeader secondNodeHeader = default(NodeHeader);

            //Determine how many entries to shift on the split.
            int recordsInTheFirstNode = (firstNodeHeader.NodeRecordCount >> 1); // divide by 2.
            int recordsInTheSecondNode = (firstNodeHeader.NodeRecordCount - recordsInTheFirstNode - 1); //The first entry of the second node is moved to a parent.

            long secondNodeIndex = GetNextNewNodeIndex();
            long sourceStartingAddress = firstNodeIndex * m_blockSize + NodeHeader.Size + sizeof(long) + StructureSize * recordsInTheFirstNode + KeySize;
            long targetStartingAddress = secondNodeIndex * m_blockSize + NodeHeader.Size;

            //lookup the dividing key
            m_stream.Position = sourceStartingAddress - KeySize;
            ulong dividingKey1 = m_stream.ReadUInt64();
            ulong dividingKey2 = m_stream.ReadUInt64();

            //do the copy
            m_stream.Copy(sourceStartingAddress, targetStartingAddress, recordsInTheSecondNode * StructureSize + sizeof(long));

            //update the node that was the old right sibling
            if (firstNodeHeader.RightSiblingNodeIndex != 0)
            {
                NodeHeader oldRightSibling = new NodeHeader(m_stream, m_blockSize, firstNodeHeader.RightSiblingNodeIndex, nodeLevel);
                oldRightSibling.LeftSiblingNodeIndex = secondNodeIndex;
                oldRightSibling.Save(m_stream, m_blockSize, firstNodeHeader.RightSiblingNodeIndex);
            }

            //update the second header
            secondNodeHeader.NodeLevel = nodeLevel;
            secondNodeHeader.NodeRecordCount = recordsInTheSecondNode;
            secondNodeHeader.LeftSiblingNodeIndex = firstNodeIndex;
            secondNodeHeader.RightSiblingNodeIndex = firstNodeHeader.RightSiblingNodeIndex;
            secondNodeHeader.Save(m_stream, m_blockSize, secondNodeIndex);

            //update the first header
            firstNodeHeader.NodeRecordCount = recordsInTheFirstNode;
            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
            firstNodeHeader.Save(m_stream, m_blockSize, firstNodeIndex);

            NodeWasSplit(nodeLevel, firstNodeIndex, dividingKey1, dividingKey2, secondNodeIndex);
            if (CompareKeys(key1, key2, dividingKey1, dividingKey2) > 0)
            {
                InternalNodeInsert(nodeLevel, secondNodeIndex, key1, key2, value);
            }
            else
            {
                InternalNodeInsert(nodeLevel, firstNodeIndex, key1, key2, value);
            }
        }

        /// <summary>
        /// Compares one key to another key to determine which is greater
        /// </summary>
        /// <returns>1 if the first key is greater. -1 if the second key is greater. 0 if the keys are equal.</returns>
        static int CompareKeys(ulong firstKey1, ulong firstKey2, ulong secondKey1, ulong secondKey2)
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
        static bool IsGreaterThanOrEqualTo(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 > compareKey1) | ((key1 == compareKey1) & (key2 >= compareKey2));
        }

        /// <summary>
        /// Returns true if the first key is greater than the later key.
        /// </summary>
        static bool IsGreaterThan(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 > compareKey1) | ((key1 == compareKey1) & (key2 > compareKey2));
        }

        /// <summary>
        /// Returns true if the first key is less than or equal to the later key
        /// </summary>
        static bool IsLessThanOrEqualTo(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 < compareKey1) | ((key1 == compareKey1) & (key2 <= compareKey2));
        }

        /// <summary>
        /// Returns true if the first key is less than the later key.
        /// </summary>
        static bool IsLessThan(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 < compareKey1) | ((key1 == compareKey1) & (key2 < compareKey2));
        }

        static bool IsEqual(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 == compareKey1) & (key2 == compareKey2);
        }

        static bool IsNotEqual(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
        {
            return (key1 != compareKey1) | (key2 != compareKey2);
        }


        #endregion
    }
}
