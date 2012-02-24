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

namespace Historian.StorageSystem.BlockSorter
{
    static class InternalNode
    {
        public const int InternalStructureSize = sizeof(long) + sizeof(uint);

        public static int CalculateMaximumChildren(int blockSize)
        {
            return (blockSize - NodeHeader.Size - sizeof(int)) / InternalStructureSize;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        /// <param name="header"></param>
        /// <param name="nodeToSplitIndex"></param>
        /// <param name="greaterNodeIndex"></param>
        /// <param name="firstKeyInGreaterNode"></param>
        public static void SplitNode(TreeHeader header, uint nodeToSplitIndex, out uint greaterNodeIndex, out long firstKeyInGreaterNode)
        {
            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(header, nodeToSplitIndex);
            if (origionalNode.Level == 0)
                throw new Exception();
            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");
            uint nextNode = origionalNode.NextNode;


            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);


            greaterNodeIndex = CreateEmptyNode(header, origionalNode.Level);
            long sourceStartingAddress = nodeToSplitIndex * header.BlockSize + NodeHeader.Size + sizeof(uint) + InternalStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * header.BlockSize + NodeHeader.Size + sizeof(uint);

            //lookup the first key that will be copied
            header.Stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = header.Stream.ReadInt64();

            //do the copy
            header.Stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * InternalStructureSize);
            //Set the lookback position as invalid since this node should never be parsed for data before the first key.
            header.Stream.Position = targetStartingAddress - sizeof(uint);
            header.Stream.Write(0u);


            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(header, nodeToSplitIndex);

            //update the second header
            newNode.Load(header, greaterNodeIndex);
            newNode.ChildCount = itemsInSecondNode;
            newNode.PreviousNode = nodeToSplitIndex;
            newNode.NextNode = nextNode;
            newNode.Save(header, greaterNodeIndex);

            //update the node that used to be after the first one.
            if (nextNode != 0)
            {
                foreignNode.Load(header, nextNode);
                foreignNode.PreviousNode = greaterNodeIndex;
                foreignNode.Save(header, nextNode);
            }
        }

        public static InsertResults TryInsertKey(TreeHeader header, long key, uint childNodeIndex, uint nodeIndex)
        {
            header.NavigateToNode(nodeIndex);
            return TryInsertKey(header, key, childNodeIndex);
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
        public static InsertResults TryInsertKey(TreeHeader header, long key, uint childNodeIndex)
        {
            long nodePosition = header.Stream.Position;

            NodeHeader node = new NodeHeader(header.Stream);

            if (node.ChildCount >= header.MaximumInternalNodeChildren)
                return InsertResults.NodeIsFullError;

            header.Stream.Position = nodePosition;

            SearchResults search = SeekToKey(header, key);
            if (search == SearchResults.StartOfExactMatch)
                return InsertResults.DuplicateKeyError;

            int spaceToMove = NodeHeader.Size + sizeof(uint) + InternalStructureSize * node.ChildCount - (int)(header.Stream.Position - nodePosition);
#if DEBUG
            if (spaceToMove < 0)
                throw new Exception("Problem calculating the space to move");
            if (spaceToMove == 0 ^ search == SearchResults.StartOfEndOfStream)
                throw new Exception("Problem calculating the space to move");
#endif

            header.Stream.InsertBytes(InternalStructureSize, spaceToMove);
            header.Stream.Write(key);
            header.Stream.Write(childNodeIndex);

            node.ChildCount++;
            header.Stream.Position = nodePosition;
            node.Save(header.Stream);

            return InsertResults.InsertedOK;
        }

        public static uint GetNodeIndex(TreeHeader header, long key, uint nodeIndex)
        {
            header.NavigateToNode(nodeIndex);
            return GetNodeIndex(header, key);
        }

        /// <summary>
        /// Starting from the end of the internal node header, 
        /// this method will return the node index value that contains the provided key
        /// </summary>
        /// <param name="header"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static uint GetNodeIndex(TreeHeader header, long key)
        {

            SearchResults results = SeekToKey(header, key);

            if (results == SearchResults.StartOfExactMatch)
            {
                header.Stream.Position += sizeof(long);
                return header.Stream.ReadUInt32();
            }
            header.Stream.Position -= 4;
            return header.Stream.ReadUInt32();
        }

        public static SearchResults SeekToKey(TreeHeader header, long key, uint nodeIndex)
        {
            header.NavigateToNode(nodeIndex);
            return SeekToKey(header, key);
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="header">the tree header details</param>
        /// <param name="key">the key to search for</param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        public static SearchResults SeekToKey(TreeHeader header, long key)
        {
#if DEBUG
            if (header.Stream.Position % header.BlockSize != 0)
                throw new Exception("The position must be set to the beginning of the stream");
#endif

            long startAddress = header.Stream.Position + NodeHeader.Size + sizeof(uint);
            
            NodeHeader node = new NodeHeader(header.Stream);
            if (node.Level == 0)
                throw new Exception();

            int min = 0;
            int max = node.ChildCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                header.Stream.Position = startAddress + InternalStructureSize * mid;
                long tmpKey = header.Stream.ReadInt64();
                if (key == tmpKey)
                {
                    header.Stream.Position -= sizeof(long);
                    return SearchResults.StartOfExactMatch;
                }
                if (key > tmpKey)
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            header.Stream.Position = startAddress + InternalStructureSize * min;
            if (node.ChildCount == 0 || min == node.ChildCount)
                return SearchResults.StartOfEndOfStream;
            return SearchResults.RightAfterClosestMatchWithoutGoingOver;
            
        }


        //        /// <summary>
        //        /// Starting from the first byte of the node, 
        //        /// this will seek the current node for the best match of the key provided.
        //        /// </summary>
        //        /// <param name="header">the tree header details</param>
        //        /// <param name="key">the key to search for</param>
        //        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        //        public static SearchResults SeekToKey(TreeHeader header, long key)
        //        {
        //#if DEBUG
        //            if (header.Stream.Position % header.BlockSize != 0)
        //                throw new Exception("The position must be set to the beginning of the stream");
        //#endif

        //            NodeHeader node = new NodeHeader(header.Stream);
        //            if (node.Level == 0)
        //                throw new Exception();

        //            header.Stream.Position += sizeof(uint);

        //            for (int x = 0; x < node.ChildCount; x++)
        //            {
        //                long tmpKey1 = header.Stream.ReadInt64();
        //                if (tmpKey1 > key)
        //                {
        //                    header.Stream.Position -= sizeof(long);
        //                    return SearchResults.RightAfterClosestMatchWithoutGoingOver;
        //                }
        //                if (tmpKey1 == key)
        //                {
        //                    header.Stream.Position -= sizeof(long);
        //                    return SearchResults.StartOfExactMatch;
        //                }
        //                header.Stream.Position += sizeof(uint);  //Skip the address
        //            }
        //            return SearchResults.StartOfEndOfStream;
        //        }


        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="level"></param>
        /// <param name="childNodeBefore"></param>
        /// <param name="key"></param>
        /// <param name="childNodeAfter"></param>
        /// <returns></returns>
        public static uint CreateEmptyNode(TreeHeader header, byte level, uint childNodeBefore, long key, uint childNodeAfter)
        {
            long origionalPosition = header.Stream.Position;
            //round the next block to the nearest boundry.

            uint nodeAddress = header.AllocateNewNode();
            header.Stream.Position = nodeAddress * header.BlockSize;

            NodeHeader node = default(NodeHeader);

            node.Level = level;
            node.ChildCount = 1;
            node.PreviousNode = 0;
            node.NextNode = 0;
            node.Save(header, nodeAddress);

            header.Stream.Write(childNodeBefore);
            header.Stream.Write(key);
            header.Stream.Write(childNodeAfter);

            header.Stream.Position = origionalPosition;

            return nodeAddress;
        }



        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        static uint CreateEmptyNode(TreeHeader header, byte level)
        {
            long origionalPosition = header.Stream.Position;
            //round the next block to the nearest boundry.

            uint nodeAddress = header.AllocateNewNode();
            header.Stream.Position = nodeAddress * header.BlockSize;

            NodeHeader node = default(NodeHeader);

            node.Level = level;
            node.ChildCount = 0;
            node.PreviousNode = 0;
            node.NextNode = 0;
            node.Save(header, nodeAddress);

            header.Stream.Position = origionalPosition;

            return nodeAddress;
        }

    }
}
