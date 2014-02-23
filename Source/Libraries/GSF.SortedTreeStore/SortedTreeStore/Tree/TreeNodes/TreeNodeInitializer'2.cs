//******************************************************************************************************
//  TreeNodeInitializer.cs - Gbtc
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
//  4/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    /// <summary>
    /// Allows a tree node to be created from a <see cref="CreateSingleTreeNodeBase"/>. This is desired
    /// to minimize the number of calls to <see cref="TreeNodeInitializer"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class TreeNodeInitializer<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private readonly CreateTreeNodeBase m_treeNode;

        public TreeNodeInitializer(CreateTreeNodeBase treeNode)
        {
            m_treeNode = treeNode;
        }

        public SortedTreeNodeBase<TKey, TValue> CreateTreeNode(byte level)
        {
            return m_treeNode.Create<TKey, TValue>(level);
        }
    }
}