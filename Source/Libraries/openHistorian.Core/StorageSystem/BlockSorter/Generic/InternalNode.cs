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
        public const int InternalStructureSize = sizeof(long) + sizeof(uint);

        public int InternalNodeCalculateMaximumChildren(int blockSize)
        {
            return (blockSize - NodeHeader.Size - sizeof(int)) / InternalStructureSize;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        /// <param name="nodeToSplitIndex"></param>
        /// <param name="greaterNodeIndex"></param>
        /// <param name="firstKeyInGreaterNode"></param>
        public void InternalNodeSplitNode(uint nodeToSplitIndex, out uint greaterNodeIndex, out long firstKeyInGreaterNode)
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
            firstKeyInGreaterNode = Stream.ReadInt64();

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

        public InsertResults InternalNodeTryInsertKey(long key, uint childNodeIndex, uint nodeIndex)
        {
            NavigateToNode(nodeIndex);
            return InternalNodeTryInsertKey(key, childNodeIndex);
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will seek to the most appropriate location for 
        /// the key to be inserted and insert the data if the leaf is not full. 
        /// </summary>
        /// <param name="header">the tree header details</param>
        /// <param name="key">the key to insert</param>
        /// <param name="childNodeIndex">the child node that corresponds to this key</param>
        /// <returns>The results of the insert</returns>
        public InsertResults InternalNodeTryInsertKey(long key, uint childNodeIndex)
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

            Stream.InsertBytes(InternalStructureSize, spaceToMove);
            Stream.Write(key);
            Stream.Write(childNodeIndex);

            node.ChildCount++;
            Stream.Position = nodePosition;
            node.Save(Stream);

            return InsertResults.InsertedOK;
        }

        public uint InternalNodeGetNodeIndex(long key, uint nodeIndex)
        {
            NavigateToNode(nodeIndex);
            return InternalNodeGetNodeIndex(key);
        }

        /// <summary>
        /// Starting from the end of the internal node header, 
        /// this method will return the node index value that contains the provided key
        /// </summary>
        /// <param name="header"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public uint InternalNodeGetNodeIndex(long key)
        {

            SearchResults results = InternalNodeSeekToKey(key);

            if (results == SearchResults.StartOfExactMatch)
            {
                Stream.Position += sizeof(long);
                return Stream.ReadUInt32();
            }
            Stream.Position -= 4;
            return Stream.ReadUInt32();
        }

        public SearchResults InternalNodeSeekToKey(long key, uint nodeIndex)
        {
            NavigateToNode(nodeIndex);
            return InternalNodeSeekToKey(key);
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="header">the tree header details</param>
        /// <param name="key">the key to search for</param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        public SearchResults InternalNodeSeekToKey(long key)
        {
#if DEBUG
            if (Stream.Position % BlockSize != 0)
                throw new Exception("The position must be set to the beginning of the stream");
#endif

            long startAddress = Stream.Position + NodeHeader.Size + sizeof(uint);
            
            NodeHeader node = new NodeHeader(Stream);
            if (node.Level == 0)
                throw new Exception();

            int min = 0;
            int max = node.ChildCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                Stream.Position = startAddress + InternalStructureSize * mid;
                long tmpKey = Stream.ReadInt64();
                if (key == tmpKey)
                {
                    Stream.Position -= sizeof(long);
                    return SearchResults.StartOfExactMatch;
                }
                if (key > tmpKey)
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            Stream.Position = startAddress + InternalStructureSize * min;
            if (node.ChildCount == 0 || min == node.ChildCount)
                return SearchResults.StartOfEndOfStream;
            return SearchResults.RightAfterClosestMatchWithoutGoingOver;
            
        }

        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="level"></param>
        /// <param name="childNodeBefore"></param>
        /// <param name="key"></param>
        /// <param name="childNodeAfter"></param>
        /// <returns></returns>
        public uint InternalNodeCreateEmptyNode(byte level, uint childNodeBefore, long key, uint childNodeAfter)
        {
            long origionalPosition = Stream.Position;
            //round the next block to the nearest boundry.

            uint nodeAddress = AllocateNewNode();
            Stream.Position = nodeAddress * BlockSize;

            NodeHeader node = default(NodeHeader);

            node.Level = level;
            node.ChildCount = 1;
            node.PreviousNode = 0;
            node.NextNode = 0;
            node.Save(Stream, BlockSize, nodeAddress);

            Stream.Write(childNodeBefore);
            Stream.Write(key);
            Stream.Write(childNodeAfter);

            Stream.Position = origionalPosition;

            return nodeAddress;
        }



        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        uint InternalNodeCreateEmptyNode(byte level)
        {
            long origionalPosition = Stream.Position;
            //round the next block to the nearest boundry.

            uint nodeAddress = AllocateNewNode();
            Stream.Position = nodeAddress * BlockSize;

            NodeHeader node = default(NodeHeader);

            node.Level = level;
            node.ChildCount = 0;
            node.PreviousNode = 0;
            node.NextNode = 0;
            node.Save(Stream,BlockSize, nodeAddress);

            Stream.Position = origionalPosition;

            return nodeAddress;
        }

    }
}
