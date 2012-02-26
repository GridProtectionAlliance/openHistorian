//******************************************************************************************************
//  LeafNode.cs - Gbtc
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
        public int LeafStructureSize;

        public int LeafNodeCalculateMaximumChildren()
        {
            return (BlockSize - NodeHeader.Size) / LeafStructureSize;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        /// <param name="nodeToSplitIndex"></param>
        /// <param name="greaterNodeIndex"></param>
        /// <param name="firstKeyInGreaterNode"></param>
        public void LeafNodeSplitNode(uint nodeToSplitIndex, out uint greaterNodeIndex, out TKey firstKeyInGreaterNode)
        {
            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(Stream, BlockSize, nodeToSplitIndex);
            if (origionalNode.Level != 0)
                throw new Exception();
            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");
            uint nextNode = origionalNode.NextNode;


            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            greaterNodeIndex = LeafNodeCreateEmptyNode();
            long sourceStartingAddress = nodeToSplitIndex * BlockSize + NodeHeader.Size + LeafStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * BlockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            Stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode=new TKey();
            firstKeyInGreaterNode.LoadValue(Stream);

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * LeafStructureSize);

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

        public TValue LeafNodeGetValueAddress(TKey key, uint nodeIndex)
        {
            Stream.Position = nodeIndex * BlockSize;
            return LeafNodeGetValueAddress(key);
        }


        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will return the value address for the key provided.
        /// If the key could not be found, -1 is returned.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns></returns>
        public TValue LeafNodeGetValueAddress(TKey key)
        {
            if (LeafNodeSeekToKey(key) == SearchResults.StartOfExactMatch)
            {
                Stream.Position += KeySize;
                TValue value = new TValue();
                value.LoadValue(Stream);
                return value;
            }
            return default(TValue);
        }

        public InsertResults LeafNodeTryInsertKey(TKey key, TValue value, uint nodeIndex)
        {
            Stream.Position = nodeIndex * BlockSize;
            return LeafNodeTryInsertKey(key, value);
        }


        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will seek to the most appropriate location for 
        /// the key to be inserted and insert the data if the leaf is not full. 
        /// </summary>
        /// <param name="key">the key to insert</param>
        /// <param name="value">the address to insert into this node</param>
        /// <returns>The results of the insert</returns>
        public InsertResults LeafNodeTryInsertKey(TKey key, TValue value)
        {
            //Determine if the node is full. Reset the position afterwards
            long nodePosition = Stream.Position;
            NodeHeader node = new NodeHeader(Stream);
            if (node.ChildCount >= MaximumLeafNodeChildren)
                return InsertResults.NodeIsFullError;
            Stream.Position = nodePosition;

            //Find the best location to insert
            SearchResults search = LeafNodeSeekToKey(key);
            if (search == SearchResults.StartOfExactMatch)
                return InsertResults.DuplicateKeyError;


            int spaceToMove = NodeHeader.Size + LeafStructureSize * node.ChildCount - (int)(Stream.Position - nodePosition);
#if DEBUG
            if (spaceToMove < 0)
                throw new Exception("Problem calculating the space to move");
            if (spaceToMove == 0 ^ search == SearchResults.StartOfEndOfStream)
                throw new Exception("Problem calculating the space to move");
#endif

            //Insert the data
            Stream.InsertBytes(LeafStructureSize, spaceToMove);
            key.SaveValue(Stream);
            value.SaveValue(Stream);
            
            //save the header
            Stream.Position = nodePosition;
            node.ChildCount++;
            node.Save(Stream);

            return InsertResults.InsertedOK;
        }

        public SearchResults LeafNodeSeekToKey(TKey key, uint nodeIndex)
        {
            Stream.Position = nodeIndex * BlockSize;
            return LeafNodeSeekToKey(key);
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        public SearchResults LeafNodeSeekToKey(TKey key)
        {
#if DEBUG
            if (Stream.Position % BlockSize != 0)
                throw new Exception("The position must be set to the beginning of the stream");
#endif

            long startAddress = Stream.Position + NodeHeader.Size;
            NodeHeader node = new NodeHeader(Stream);
            if (node.Level != 0)
                throw new Exception();

            int min = 0;
            int max = node.ChildCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                Stream.Position = startAddress + LeafStructureSize * mid;
                int tmpKey = key.CompareToStream(Stream);
                if (tmpKey == 0)
                {
                    Stream.Position -= KeySize;
                    return SearchResults.StartOfExactMatch;
                }
                if (tmpKey > 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            Stream.Position = startAddress + LeafStructureSize * min;
            if (node.ChildCount == 0 || min == node.ChildCount)
                return SearchResults.StartOfEndOfStream;
            return SearchResults.RightAfterClosestMatchWithoutGoingOver;
        }

        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <returns></returns>
        public uint LeafNodeCreateEmptyNode()
        {
            long origionalPosition = Stream.Position;
            uint nodeAddress = AllocateNewNode();

            NodeHeader node;
            node.Level = 0;
            node.ChildCount = 0;
            node.NextNode = 0;
            node.PreviousNode = 0;
            node.Save(Stream,BlockSize, nodeAddress);

            Stream.Position = origionalPosition;

            return nodeAddress;
        }

    }
}
