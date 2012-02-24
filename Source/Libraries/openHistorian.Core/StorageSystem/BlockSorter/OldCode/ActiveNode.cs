////******************************************************************************************************
////  ActiveNode.cs - Gbtc
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

//namespace Historian.StorageSystem.BlockSorter
//{
//    /// <summary>
//    /// Contains the cached values of a node.
//    /// </summary>
//    struct ActiveNode
//    {
//        /// <summary>
//        /// The tree that this node belongs to.
//        /// </summary>
//        public BPlusTree Tree { get; private set; }
//        /// <summary>
//        /// The absolute position address for the start of this node.
//        /// </summary>
//        public long Address { get; private set; }
//        /// <summary>
//        /// The index value of this node
//        /// </summary>
//        public uint IndexAddress { get; private set; }
//        /// <summary>
//        /// Tells if the node is a leaf or an internal node
//        /// </summary>
//        public bool IsLeaf { get; private set; }
//        /// <summary>
//        /// The number of children that are in this node.
//        /// </summary>
//        public short ChildCount { get; private set; }

//        /// <summary>
//        /// Populates an active node with the necessary information
//        /// </summary>
//        /// <param name="indexAddress">the node index to look up</param>
//        /// <param name="tree">The BPlusTree that this node belongs to</param>
//        public ActiveNode(uint indexAddress, BPlusTree tree)
//            : this()
//        {
//            Tree = tree;
//            IndexAddress = indexAddress;
//            Address = indexAddress * (long)Tree.BlockSize;
//            Tree.Stream.Position = Address;
//            IsLeaf = tree.Stream.ReadBoolean();
//            ChildCount = tree.Stream.ReadInt16();
//        }
    
//        /// <summary>
//        /// Moves the current position of the underlying stream to the offset position of the current node.
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="offsetValue"></param>
//        public void NavigateToOffset(int offsetValue)
//        {
//            Tree.Stream.Position = Address + offsetValue;
//        }

//        /// <summary>
//        /// Modifies the child count of this structure and the underlying stream
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="count"></param>
//        public void SetChildCount(short count)
//        {
//            ChildCount = count;
//            Tree.Stream.Position = Address + 1;
//            Tree.Stream.Write(count);
//        }
//    }
//}
