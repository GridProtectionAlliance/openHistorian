//******************************************************************************************************
//  BPlusTree_InternalNode.cs - Gbtc
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
//  2/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.Core.StorageSystem.Generic
{
    public partial class BPlusTree<TKey, TValue>
    {
        /// <summary>
        /// The number of bytes in an internal node.  This is the size of the key plus 4 byte pointer.
        /// </summary>
        int m_internalStructureSize;

        int InternalNodeCalculateMaximumChildren()
        {
            return (m_blockSize - NodeHeader.Size - sizeof(uint)) / m_internalStructureSize;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        /// <param name="nodeToSplitIndex">the node index value to split</param>
        /// <param name="greaterNodeIndex">the index value of the second half of the items in the list</param>
        /// <param name="firstKeyInGreaterNode">the key that splits the two entries</param>
        /// <param name="nodeLevel">the level of the node</param>
        void InternalNodeSplitNode(uint nodeToSplitIndex, out uint greaterNodeIndex, out TKey firstKeyInGreaterNode, byte nodeLevel)
        {
            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(m_stream, m_blockSize, nodeToSplitIndex);
            if (origionalNode.Level != nodeLevel)
                throw new Exception();
            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");
            uint nextNode = origionalNode.NextNode;

            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            greaterNodeIndex = AllocateNewNode();
            long sourceStartingAddress = nodeToSplitIndex * m_blockSize + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size + sizeof(uint);

            //lookup the first key that will be copied
            m_stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = new TKey();
            firstKeyInGreaterNode.LoadValue(m_stream);

            //do the copy
            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_internalStructureSize);
            //Set the lookback position as invalid since this node should never be parsed for data before the first key.
            m_stream.Position = targetStartingAddress - sizeof(uint);
            m_stream.Write(0u);


            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(m_stream, m_blockSize, nodeToSplitIndex);

            //update the second header
            newNode.Level = origionalNode.Level;
            newNode.ChildCount = itemsInSecondNode;
            newNode.PreviousNode = nodeToSplitIndex;
            newNode.NextNode = nextNode;
            newNode.Save(m_stream, m_blockSize, greaterNodeIndex);

            //update the node that used to be after the first one.
            if (nextNode != 0)
            {
                foreignNode.Load(m_stream, m_blockSize, nextNode);
                foreignNode.PreviousNode = greaterNodeIndex;
                foreignNode.Save(m_stream, m_blockSize, nextNode);
            }
           m_cache.ClearCache(origionalNode.Level);
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will seek to the most appropriate location for 
        /// the key to be inserted and insert the data if the leaf is not full. 
        /// </summary>
        /// <param name="key">the key to insert</param>
        /// <param name="childNodeIndex">the child node that corresponds to this key</param>
        /// <param name="nodeIndex">the index of the node to be modified</param>
        /// <param name="nodeLevel">the level of the node</param>
        /// <returns>The results of the insert</returns>
        InsertResults InternalNodeTryInsertKey(TKey key, uint childNodeIndex, uint nodeIndex, byte nodeLevel)
        {

            long nodePositionStart = nodeIndex * m_blockSize;
            m_stream.Position = nodePositionStart;

            byte level = m_stream.ReadByte();
            short childCount = m_stream.ReadInt16();

            if (level != nodeLevel)
                throw new Exception("Corrupt Node Level");

            if (childCount >= m_maximumInternalNodeChildren)
                return InsertResults.NodeIsFullError;

            m_stream.Position = nodePositionStart;

            SearchResults search = InternalNodeSeekToKey(key,nodeIndex,level);
            if (search == SearchResults.StartOfExactMatch)
                return InsertResults.DuplicateKeyError;


            int spaceToMove = NodeHeader.Size + sizeof(uint) + m_internalStructureSize * childCount - (int)(m_stream.Position - nodePositionStart);
#if DEBUG
            if (spaceToMove < 0)
                throw new Exception("Problem calculating the space to move");
            if (spaceToMove == 0 ^ search == SearchResults.StartOfEndOfStream)
                throw new Exception("Problem calculating the space to move");
#endif
            if (spaceToMove > 0)
                m_stream.InsertBytes(m_internalStructureSize, spaceToMove);

            key.SaveValue(m_stream);
            m_stream.Write(childNodeIndex);

            childCount++;
            m_stream.Position = nodePositionStart + 1;
            m_stream.Write(childCount);

            m_cache.ClearCache(level);

            return InsertResults.InsertedOK;
        }

        /// <summary>
        /// Starting from the end of the internal node header, 
        /// this method will return the node index value that contains the provided key
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="nodeIndex">the index of the node to be modified</param>
        /// <param name="nodeLevel">the level of the node</param>
        /// <returns></returns>
        uint InternalNodeGetNodeIndex(TKey key, uint nodeIndex, byte nodeLevel)
        {
            //InternalNodeSeekToKey(key, nodeIndex, nodeLevel);
            //return m_cache[nodeLevel - 1].Bucket;
            SearchResults results = InternalNodeSeekToKey(key, nodeIndex, nodeLevel);
            if (results == SearchResults.StartOfExactMatch)
            {
                m_stream.Position += m_keySize;
                return m_stream.ReadUInt32();
            }
            m_stream.Position -= 4;
            return m_stream.ReadUInt32();
        }


        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="nodeIndex">the index of the node</param>
        /// <param name="nodeLevel">the level of the node</param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        SearchResults InternalNodeSeekToKey(TKey key, uint nodeIndex, byte nodeLevel)
        {
            m_stream.Position = nodeIndex * m_blockSize;

            long startAddress = m_stream.Position + NodeHeader.Size + sizeof(uint);
            byte level = m_stream.ReadByte();
            short childCount = m_stream.ReadInt16();

            if (nodeLevel != level)
                throw new Exception("Corrupt BPlusTree: Unexpected Node Level");
            
            int min = 0;
            int max = childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                m_stream.Position = startAddress + m_internalStructureSize * mid;
                int tmpKey = key.CompareToStream(m_stream);
                if (tmpKey == 0)
                {
                    m_cache.CacheExactMatch(level, childCount, startAddress, mid);
                    m_stream.Position = startAddress + m_internalStructureSize * mid;
                    return SearchResults.StartOfExactMatch;
                }
                if (tmpKey > 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }

            m_cache.CacheNotExactMatch(level, childCount, startAddress, min);
            m_stream.Position = startAddress + m_internalStructureSize * min;

            if (childCount == 0 || min == childCount)
                return SearchResults.StartOfEndOfStream;
            return SearchResults.StartOfInsertPosition;

        }

        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <param name="level">the level of the internal node</param>
        /// <param name="childNodeBefore">the child value before</param>
        /// <param name="key">the key that seperates the children</param>
        /// <param name="childNodeAfter">the child after or equal to the key</param>
        /// <returns>the index value of this new node.</returns>
        uint InternalNodeCreateEmptyNode(byte level, uint childNodeBefore, TKey key, uint childNodeAfter)
        {
            uint nodeAddress = AllocateNewNode();
            m_stream.Position = nodeAddress * m_blockSize;

            //Clearing the Node
            //Level = level;
            //ChildCount = 1;
            //NextNode = 0;
            //PreviousNode = 0;
            m_stream.Write(level);
            m_stream.Write((short)1);
            m_stream.Write(0L);
            m_stream.Write(childNodeBefore);
            key.SaveValue(m_stream);
            m_stream.Write(childNodeAfter);

            return nodeAddress;
        }

    }
}
