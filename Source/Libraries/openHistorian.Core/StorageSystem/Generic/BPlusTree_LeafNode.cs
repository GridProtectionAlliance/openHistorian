//******************************************************************************************************
//  BPlusTree_LeafNode.cs - Gbtc
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

namespace openHistorian.V2.StorageSystem.Generic
{
    public partial class BPlusTree<TKey, TValue>
    {
        /// <summary>
        /// The number of bytes in a leaf node.  This is the size of the key plus 4 byte pointer.
        /// </summary>
        int m_leafStructureSize;

        int LeafNodeCalculateMaximumChildren()
        {
            return (m_blockSize - NodeHeader.Size) / m_leafStructureSize;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        /// <param name="nodeToSplitIndex">the node index value to split</param>
        /// <param name="greaterNodeIndex">the index value of the second half of the items in the list</param>
        /// <param name="firstKeyInGreaterNode">the key that splits the two entries</param>
        void LeafNodeSplitNode(uint nodeToSplitIndex, out uint greaterNodeIndex, out TKey firstKeyInGreaterNode)
        {
            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(m_stream, m_blockSize, nodeToSplitIndex);
            if (origionalNode.Level != 0)
                throw new Exception();
            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");
            uint nextNode = origionalNode.NextNode;

            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            greaterNodeIndex = AllocateNewNode();
            long sourceStartingAddress = nodeToSplitIndex * m_blockSize + NodeHeader.Size + m_leafStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            m_stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode=new TKey();
            firstKeyInGreaterNode.LoadValue(m_stream);

            //do the copy
            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_leafStructureSize);

            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(m_stream, m_blockSize, nodeToSplitIndex);

            //update the second header
            newNode.Level = 0;
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
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will seek to the most appropriate location for 
        /// the key to be inserted and insert the data if the leaf is not full. 
        /// </summary>
        /// <param name="key">the key to insert</param>
        /// <param name="value">the address to insert into this node</param>
        /// <param name="nodeIndex">the index of the node to be modified</param>
        /// <returns>The results of the insert</returns>
        InsertResults LeafNodeTryInsertKey(TKey key, TValue value, uint nodeIndex)
        {
            long nodePositionStart = nodeIndex * m_blockSize;
            m_stream.Position = nodePositionStart;

            byte level = m_stream.ReadByte();
            short childCount = m_stream.ReadInt16();
            if (childCount >= m_maximumLeafNodeChildren)
                return InsertResults.NodeIsFullError;

            m_stream.Position = nodePositionStart;

            //Find the best location to insert
            SearchResults search = LeafNodeSeekToKey(key,nodeIndex);
            if (search == SearchResults.StartOfExactMatch)
                return InsertResults.DuplicateKeyError;


            int spaceToMove = NodeHeader.Size + m_leafStructureSize * childCount - (int)(m_stream.Position - nodePositionStart);
#if DEBUG
            if (spaceToMove < 0)
                throw new Exception("Problem calculating the space to move");
            if (spaceToMove == 0 ^ search == SearchResults.StartOfEndOfStream)
                throw new Exception("Problem calculating the space to move");
#endif

            //Insert the data
            if (spaceToMove>0) 
                m_stream.InsertBytes(m_leafStructureSize, spaceToMove);
            key.SaveValue(m_stream);
            value.SaveValue(m_stream);
            
            //save the header
            m_stream.Position = nodePositionStart + 1;
            childCount++;
            m_stream.Write(childCount);

            return InsertResults.InsertedOK;
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will return the value address for the key provided.
        /// If the key could not be found, -1 is returned.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="nodeIndex">the index of the node to be modified</param>
        /// <returns></returns>
        TValue LeafNodeGetValueAddress(TKey key, uint nodeIndex)
        {
            if (LeafNodeSeekToKey(key, nodeIndex) == SearchResults.StartOfExactMatch)
            {
                m_stream.Position += m_keySize;
                TValue value = new TValue();
                value.LoadValue(m_stream);
                return value;
            }
            return default(TValue);
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="nodeIndex">the index of the node to be modified</param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        SearchResults LeafNodeSeekToKey(TKey key, uint nodeIndex)
        {
            m_stream.Position = nodeIndex * m_blockSize;

            long startAddress = m_stream.Position + NodeHeader.Size;
            byte level = m_stream.ReadByte();
            short childCount = m_stream.ReadInt16();
            if (level != 0)
                throw new Exception();

            int min = 0;
            int max = childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                m_stream.Position = startAddress + m_leafStructureSize * mid;
                int tmpKey = key.CompareToStream(m_stream);
                if (tmpKey == 0)
                {
                    m_stream.Position -= m_keySize;
                    return SearchResults.StartOfExactMatch;
                }
                if (tmpKey > 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            m_stream.Position = startAddress + m_leafStructureSize * min;
            if (childCount == 0 || min == childCount)
                return SearchResults.StartOfEndOfStream;
            return SearchResults.StartOfInsertPosition;
        }

//        /// <summary>
//        /// Starting from the first byte of the node, 
//        /// this will seek the current node for the best match of the key provided.
//        /// </summary>
//        /// <param name="key">the key to search for</param>
//        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
//        SearchResults LeafNodeSeekToKey(TKey key)
//        {
//#if DEBUG
//            if (Stream.Position % BlockSize != 0)
//                throw new Exception("The position must be set to the beginning of the stream");
//#endif

//            long startAddress = Stream.Position + NodeHeader.Size;
//            byte level = Stream.ReadByte();
//            short childCount = Stream.ReadInt16();
//            if (level != 0)
//                throw new Exception();

//            int min = 0;
//            int max = childCount - 1;

//            int mid = min + (max - min >> 1);
//            int oldMid = mid;

//            Stream.PositionIncrement(LeafStructureSize * mid + NodeHeader.Size - 3);

//            while (min <= max)
//            {
//                int tmpKey = key.CompareToStream(Stream);
//                if (tmpKey == 0)
//                {
//                    Stream.PositionDecrement(KeySize);
//                    return SearchResults.StartOfExactMatch;
//                }
//                if (tmpKey > 0)
//                    min = mid + 1;
//                else
//                    max = mid - 1;

//                mid = min + (max - min >> 1);
//                Stream.PositionIncrement(LeafStructureSize * (mid - oldMid) - KeySize);
//                oldMid = mid;
//            }
//            Stream.Position = startAddress + LeafStructureSize * min;
//            if (childCount == 0 || min == childCount)
//                return SearchResults.StartOfEndOfStream;
//            return SearchResults.RightAfterClosestMatchWithoutGoingOver;
//        }

        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <returns></returns>
        uint LeafNodeCreateEmptyNode()
        {
            uint nodeAddress = AllocateNewNode();
            m_stream.Position = m_blockSize * nodeAddress;

            //Clearing the Node
            //Level = 0;
            //ChildCount = 0;
            //NextNode = 0;
            //PreviousNode = 0;
            m_stream.Write(0L);
            m_stream.Write(0);

            return nodeAddress;
        }

    }
}
