////******************************************************************************************************
////  ActiveLeafNode.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  2/18/2012 - Steven E. Chisholm
////       Generated original version of source code.
////
////******************************************************************************************************

//using System;

//namespace Historian.StorageSystem.BlockSorter
//{
//    /// <summary>
//    /// Contains the results of a search on a leaf node.
//    /// </summary>
//    struct ActiveLeafNode
//    {
//        /// <summary>
//        /// The number of bytes in a node header.
//        /// </summary>
//        const int NodeHeader = 3;
//        /// <summary>
//        /// The number of bytes per KeyValue in this node.
//        /// </summary>
//        const int LeafBlockSize = 8 + sizeof(long);

//        /// <summary>
//        /// Contains the most active node.
//        /// </summary>
//        public ActiveNode Node { get; private set; }

//        /// <summary>
//        /// Gets whether the search was an exact match.
//        /// </summary>
//        public bool IsExactMatch { get; private set; }

//        /// <summary>
//        /// The index valud of the current key or closest match
//        /// </summary>
//        public short KeyIndex { get; private set; }

//        /// <summary>
//        /// The key that was used to perform the search
//        /// </summary>
//        public long SearchKey { get; private set; }

//        /// <summary>
//        /// The address for the start of the key value result
//        /// </summary>
//        public long KeyAddress { get; private set; }

//        /// <summary>
//        /// If there was an exact match. 
//        /// Returns the address for the associated value of the searched key.
//        /// </summary>
//        public long CurrentValueAddress { get; private set; }
//        //public long CurrentAddress { get; private set; }

//        /// <summary>
//        /// The number of keys that can still be added to this node.
//        /// </summary>
//        public short AvailableKeySpace
//        {
//            get
//            {
//                return (short)((Node.Tree.BlockSize - NodeHeader) / LeafBlockSize - Node.ChildCount);
//            }
//        }

//        /// <summary>
//        /// Performs a serch of the current node for the provided key
//        /// </summary>
//        /// <param name="key">the key to look for</param>
//        /// <param name="node">the node to search for the closes match of the key</param>
//        public ActiveLeafNode(long key, ActiveNode node)
//            : this()
//        {
//            if (!node.IsLeaf)
//                throw new Exception("The current node is not a leaf node.");
            
//            Node = node;
//            SearchKey = key;

//            node.NavigateToOffset(NodeHeader);

//            for (short x = 0; x < node.ChildCount; x++)
//            {
//                long tmpKey1 = node.Tree.Stream.ReadInt64();
//                if (tmpKey1 > key)
//                {
//                    KeyAddress = node.Tree.Stream.Position - 8;
//                    KeyIndex = x;
//                    IsExactMatch = false;
//                    CurrentValueAddress = node.Tree.Stream.ReadInt64();
//                    return;
//                }
//                else if (tmpKey1 == key)
//                {
//                    KeyAddress = node.Tree.Stream.Position - 8;
//                    KeyIndex = x;
//                    IsExactMatch = true;
//                    CurrentValueAddress = node.Tree.Stream.ReadInt64();
//                    return;
//                }
//                else
//                {
//                    //Skip the address
//                    node.Tree.Stream.Position += sizeof(long);
//                }
//            }
//            KeyAddress = node.Tree.Stream.Position - 8 - sizeof(long);
//            KeyIndex = node.ChildCount;
//            IsExactMatch = false;
//            CurrentValueAddress = -1;
//            //CurrentKey = -1;
//        }

//        public void InsertValue(long dataAddress)
//        {
//            if (AvailableKeySpace < 1)
//                throw new Exception("Leaf is Full");
//            if (IsExactMatch)
//                throw new Exception("Duplicate Key Found");

//            Node.Tree.Stream.Position = KeyAddress;

//            Node.Tree.Stream.InsertBytes(LeafBlockSize, BytesToMove());
//            Node.Tree.Stream.Write(SearchKey);
//            Node.Tree.Stream.Write(dataAddress);
//            Node.SetChildCount((short)(Node.ChildCount + 1));
//            return;
//        }
//        int BytesToMove()
//        {
//            return Node.Tree.BlockSize - (KeyIndex * LeafBlockSize) - (AvailableKeySpace * LeafBlockSize) - NodeHeader;
//        }

//        public SplitDetails Split()
//        {
//            ushort startingIndexToCopy = (ushort)(Node.ChildCount >> 1); // divide by 2.
//            ushort entriesToCopy = (ushort)(Node.ChildCount - startingIndexToCopy);

//            long sourceAddress = Node.Address + NodeHeader + LeafBlockSize * startingIndexToCopy; //navigate to this index

//            //populate the return values
//            SplitDetails split;
//            split.IsSplit = true;

//            split.NodeAddress = default(ActiveNode);//Node.Tree.CreateEmptyTreeNode(true, entriesToCopy);
//            Node.Tree.Stream.Position = sourceAddress;
//            split.Key1 = Node.Tree.Stream.ReadInt64();

//            //Copy half of the nodes to the other leaf.
//            int bytesToMove = entriesToCopy * LeafBlockSize;
//            long destinationAddress = split.NodeAddress.Address * (long)Node.Tree.BlockSize + NodeHeader;
//            Node.Tree.Stream.Copy(sourceAddress, destinationAddress, bytesToMove);

//            //overwrite the indexCount
//            Node.SetChildCount((short)startingIndexToCopy);
//            return split;
//        }

//    }
//}
