//******************************************************************************************************
//  InternalNode.cs - Gbtc
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
        int InternalStructureSize;

        int InternalNodeCalculateMaximumChildren()
        {
            return (BlockSize - NodeHeader.Size - sizeof(uint)) / InternalStructureSize;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        /// <param name="nodeToSplitIndex"></param>
        /// <param name="greaterNodeIndex"></param>
        /// <param name="firstKeyInGreaterNode"></param>
        void InternalNodeSplitNode(uint nodeToSplitIndex, out uint greaterNodeIndex, out TKey firstKeyInGreaterNode)
        {

            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(Stream, BlockSize, nodeToSplitIndex);
            if (origionalNode.Level == 0)
                throw new Exception();
            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");
            uint nextNode = origionalNode.NextNode;

            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            greaterNodeIndex = InternalNodeCreateEmptyNode(origionalNode.Level);
            long sourceStartingAddress = nodeToSplitIndex * BlockSize + NodeHeader.Size + sizeof(uint) + InternalStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * BlockSize + NodeHeader.Size + sizeof(uint);

            //lookup the first key that will be copied
            Stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = new TKey();
            firstKeyInGreaterNode.LoadValue(Stream);

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * InternalStructureSize);
            //Set the lookback position as invalid since this node should never be parsed for data before the first key.
            Stream.Position = targetStartingAddress - sizeof(uint);
            Stream.Write(0u);


            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(Stream, BlockSize, nodeToSplitIndex);

            //update the second header
            newNode.Load(Stream, BlockSize, greaterNodeIndex);
            newNode.ChildCount = itemsInSecondNode;
            newNode.PreviousNode = nodeToSplitIndex;
            newNode.NextNode = nextNode;
            newNode.Save(Stream, BlockSize, greaterNodeIndex);

            //update the node that used to be after the first one.
            if (nextNode != 0)
            {
                foreignNode.Load(Stream, BlockSize, nextNode);
                foreignNode.PreviousNode = greaterNodeIndex;
                foreignNode.Save(Stream, BlockSize, nextNode);
            }

        }

        InsertResults InternalNodeTryInsertKey(TKey key, uint childNodeIndex, uint nodeIndex)
        {
            Stream.Position = nodeIndex * BlockSize;
            return InternalNodeTryInsertKey(key, childNodeIndex);
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will seek to the most appropriate location for 
        /// the key to be inserted and insert the data if the leaf is not full. 
        /// </summary>
        /// <param name="key">the key to insert</param>
        /// <param name="childNodeIndex">the child node that corresponds to this key</param>
        /// <returns>The results of the insert</returns>
        InsertResults InternalNodeTryInsertKey(TKey key, uint childNodeIndex)
        {

            long nodePosition = Stream.Position;

            NodeHeader node = new NodeHeader(Stream);

            if (node.ChildCount >= MaximumInternalNodeChildren)
                return InsertResults.NodeIsFullError;

            Stream.Position = nodePosition;

            SearchResults search = InternalNodeSeekToKey(key);
            if (search == SearchResults.StartOfExactMatch)
                return InsertResults.DuplicateKeyError;

            int spaceToMove = NodeHeader.Size + sizeof(uint) + InternalStructureSize * node.ChildCount - (int)(Stream.Position - nodePosition);
#if DEBUG
            if (spaceToMove < 0)
                throw new Exception("Problem calculating the space to move");
            if (spaceToMove == 0 ^ search == SearchResults.StartOfEndOfStream)
                throw new Exception("Problem calculating the space to move");
#endif
            if (spaceToMove > 0)
                Stream.InsertBytes(InternalStructureSize, spaceToMove);

            key.SaveValue(Stream);
            Stream.Write(childNodeIndex);

            node.ChildCount++;
            Stream.Position = nodePosition;
            node.Save(Stream);

            return InsertResults.InsertedOK;
        }

        /// <summary>
        /// Starting from the end of the internal node header, 
        /// this method will return the node index value that contains the provided key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        uint InternalNodeGetNodeIndex(TKey key)
        {
            SearchResults results = InternalNodeSeekToKey(key);
            if (results == SearchResults.StartOfExactMatch)
            {
                Stream.Position += KeySize;
                return Stream.ReadUInt32();
            }
            Stream.Position -= 4;
            return Stream.ReadUInt32();
        }


        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        SearchResults InternalNodeSeekToKey(TKey key)
        {

#if DEBUG
            if (Stream.Position % BlockSize != 0)
                throw new Exception("The position must be set to the beginning of the stream");
#endif

            long startAddress = Stream.Position + NodeHeader.Size + sizeof(uint);

            byte level = Stream.ReadByte();
            short childCount = Stream.ReadInt16();

            if (level == 0)
                throw new Exception();

            int min = 0;
            int max = childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                Stream.Position = startAddress + InternalStructureSize * mid;
                int tmpKey = key.CompareToStream(Stream);
                if (tmpKey == 0)
                {
                    CacheCurrentIndex(level, childCount, startAddress, mid, true, key);
                    Stream.Position = startAddress + InternalStructureSize * mid;
                    return SearchResults.StartOfExactMatch;
                }
                if (tmpKey > 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            CacheCurrentIndex(level, childCount, startAddress, min, false, key);
            Stream.Position = startAddress + InternalStructureSize * min;

            if (childCount == 0 || min == childCount)
                return SearchResults.StartOfEndOfStream;
            return SearchResults.RightAfterClosestMatchWithoutGoingOver;

        }

        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="childNodeBefore"></param>
        /// <param name="key"></param>
        /// <param name="childNodeAfter"></param>
        /// <returns></returns>
        uint InternalNodeCreateEmptyNode(byte level, uint childNodeBefore, TKey key, uint childNodeAfter)
        {
            uint nodeAddress = AllocateNewNode();
            Stream.Position = nodeAddress * BlockSize;

            //Clearing the Node
            //Level = level;
            //ChildCount = 1;
            //NextNode = 0;
            //PreviousNode = 0;
            Stream.Write(level);
            Stream.Write((short)1);
            Stream.Write(0L);

            Stream.Write(childNodeBefore);
            key.SaveValue(Stream);
            Stream.Write(childNodeAfter);

            return nodeAddress;
        }

        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        uint InternalNodeCreateEmptyNode(byte level)
        {
            uint nodeAddress = AllocateNewNode();
            Stream.Position = BlockSize * nodeAddress;

            //Clearing the Node
            //Level = level;
            //ChildCount = 0;
            //NextNode = 0;
            //PreviousNode = 0;
            Stream.Write(level);
            Stream.Write(0L);
            Stream.Write(0);

            return nodeAddress;
        }

    }
}
