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

namespace openHistorian.Core.StorageSystem.BlockSorter
{
    static class LeafNode
    {
        public const int LeafStructureSize = sizeof(long) + sizeof(long);

        public static int CalculateMaximumChildren(int blockSize)
        {
            return (blockSize - NodeHeader.Size) / LeafStructureSize;
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
            if (origionalNode.Level != 0)
                throw new Exception();
            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");
            uint nextNode = origionalNode.NextNode;


            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            greaterNodeIndex = CreateEmptyNode(header);
            long sourceStartingAddress = nodeToSplitIndex * header.BlockSize + NodeHeader.Size + LeafStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * header.BlockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            header.Stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = header.Stream.ReadInt64();

            //do the copy
            header.Stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * LeafStructureSize);

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

        public static long GetValueAddress(TreeHeader header, long key, uint nodeIndex)
        {
            header.NavigateToNode(nodeIndex);
            return GetValueAddress(header, key);
        }


        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will return the value address for the key provided.
        /// If the key could not be found, -1 is returned.
        /// </summary>
        /// <param name="header">the tree header details</param>
        /// <param name="key">the key to search for</param>
        /// <returns></returns>
        public static long GetValueAddress(TreeHeader header, long key)
        {
            if (SeekToKey(header, key) == SearchResults.StartOfExactMatch)
            {
                header.Stream.Position += 8;
                return header.Stream.ReadInt64();
            }
            return -1;
        }

        public static InsertResults TryInsertKey(TreeHeader header, long key, long dataAddress, uint nodeIndex)
        {
            header.NavigateToNode(nodeIndex);
            return TryInsertKey(header, key, dataAddress);
        }


        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will seek to the most appropriate location for 
        /// the key to be inserted and insert the data if the leaf is not full. 
        /// </summary>
        /// <param name="header">the tree header details</param>
        /// <param name="key">the key to insert</param>
        /// <param name="dataAddress">the address to insert into this node</param>
        /// <returns>The results of the insert</returns>
        public static InsertResults TryInsertKey(TreeHeader header, long key, long dataAddress)
        {
            //Determine if the node is full. Reset the position afterwards
            long nodePosition = header.Stream.Position;
            NodeHeader node = new NodeHeader(header.Stream);
            if (node.ChildCount >= header.MaximumLeafNodeChildren)
                return InsertResults.NodeIsFullError;
            header.Stream.Position = nodePosition;

            //Find the best location to insert
            SearchResults search = SeekToKey(header, key);
            if (search == SearchResults.StartOfExactMatch)
                return InsertResults.DuplicateKeyError;


            int spaceToMove = NodeHeader.Size + LeafStructureSize * node.ChildCount - (int)(header.Stream.Position - nodePosition);
#if DEBUG
            if (spaceToMove < 0)
                throw new Exception("Problem calculating the space to move");
            if (spaceToMove == 0 ^ search == SearchResults.StartOfEndOfStream)
                throw new Exception("Problem calculating the space to move");
#endif

            //Insert the data
            header.Stream.InsertBytes(LeafStructureSize, spaceToMove);
            header.Stream.Write(key);
            header.Stream.Write(dataAddress);

            //save the header
            header.Stream.Position = nodePosition;
            node.ChildCount++;
            node.Save(header.Stream);

            return InsertResults.InsertedOK;
        }

        public static SearchResults SeekToKey(TreeHeader header, long key, uint nodeIndex)
        {
            header.NavigateToNode(nodeIndex);
            return SeekToKey(header, key);
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
        //            if (node.Level != 0)
        //                throw new Exception();
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
        //                header.Stream.Position += sizeof(long);  //Skip the address
        //            }
        //            return SearchResults.StartOfEndOfStream;
        //        }

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

            long startAddress = header.Stream.Position + NodeHeader.Size;
            NodeHeader node = new NodeHeader(header.Stream);
            if (node.Level != 0)
                throw new Exception();

            int min = 0;
            int max = node.ChildCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                header.Stream.Position = startAddress + LeafStructureSize * mid;
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
            header.Stream.Position = startAddress + LeafStructureSize * min;
            if (node.ChildCount == 0 || min == node.ChildCount)
                return SearchResults.StartOfEndOfStream;
            return SearchResults.RightAfterClosestMatchWithoutGoingOver;
        }




        /// <summary>
        /// Allocates a new empty tree node.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static uint CreateEmptyNode(TreeHeader header)
        {
            long origionalPosition = header.Stream.Position;
            uint nodeAddress = header.AllocateNewNode();

            NodeHeader node;
            node.Level = 0;
            node.ChildCount = 0;
            node.NextNode = 0;
            node.PreviousNode = 0;
            node.Save(header, nodeAddress);

            header.Stream.Position = origionalPosition;

            return nodeAddress;
        }

    }
}
