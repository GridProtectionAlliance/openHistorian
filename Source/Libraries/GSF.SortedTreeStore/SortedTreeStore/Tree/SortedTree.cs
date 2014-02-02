//******************************************************************************************************
//  WriteProcessorSettings.cs - Gbtc
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
//  1/21/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Tree.TreeNodes;

namespace GSF.SortedTreeStore.Tree
{
    /// <summary>
    /// A static class for some basic functions of the sortedtree.
    /// </summary>
    public static class SortedTree
    {
        /// <summary>
        /// Registers a user defined TreeNode type
        /// </summary>
        /// <param name="treeNode"></param>
        public static void RegisterTreeNode(CreateTreeNodeBase treeNode)
        {
            TreeNodeInitializer.Register(treeNode);
        }
        /// <summary>
        /// Reads the header data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sparseIndexType"></param>
        /// <param name="treeNodeType"></param>
        /// <param name="blockSize"></param>
        internal static void ReadHeader(BinaryStreamBaseOld stream, out Guid sparseIndexType, out Guid treeNodeType, out int blockSize)
        {
            stream.Position = 0;
            sparseIndexType = stream.ReadGuid();
            treeNodeType = stream.ReadGuid();
            blockSize = stream.ReadInt32();
        }
        /// <summary>
        /// A node where each element is fixed in size.
        /// </summary>
        public static Guid FixedSizeNode
        {
            get
            {
                return CreateFixedSizeNode.TypeGuid;
            }
        }
       
    }
}
