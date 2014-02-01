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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    public static class TreeNodeInitializer
    {
        private static readonly object SyncRoot;
        private static readonly Dictionary<Tuple<Guid, Type, Type>, CreateTreeNodeBase> TreeNodeKeyValue;
        private static readonly Dictionary<Tuple<Guid, Type>, CreateTreeNodeBase> TreeNodeKey;
        private static readonly Dictionary<Guid, CreateTreeNodeBase> TreeNode;

        static TreeNodeInitializer()
        {
            SyncRoot = new object();
            TreeNodeKeyValue = new Dictionary<Tuple<Guid, Type, Type>, CreateTreeNodeBase>();
            TreeNodeKey = new Dictionary<Tuple<Guid, Type>, CreateTreeNodeBase>();
            TreeNode = new Dictionary<Guid, CreateTreeNodeBase>();

            Register(new CreateFixedSizeNode());

        }

        public static void Register(CreateTreeNodeBase treeNode)
        {
            lock (SyncRoot)
            {
                if (treeNode.KeyTypeIfFixed == null && treeNode.ValueTypeIfFixed == null)
                {
                    TreeNode.Add(treeNode.GetTypeGuid, treeNode);
                }
                else if (treeNode.KeyTypeIfFixed != null && treeNode.ValueTypeIfFixed == null)
                {
                    TreeNodeKey.Add(Tuple.Create(treeNode.GetTypeGuid, treeNode.KeyTypeIfFixed), treeNode);
                }
                else if (treeNode.KeyTypeIfFixed != null && treeNode.ValueTypeIfFixed != null)
                {
                    TreeNodeKeyValue.Add(Tuple.Create(treeNode.GetTypeGuid, treeNode.KeyTypeIfFixed, treeNode.ValueTypeIfFixed), treeNode);
                }
                else
                {
                    throw new InvalidDataException("Type is not supported");
                }
            }
        }

        internal static TreeNodeInitializer<TKey, TValue> GetTreeNodeInitializer<TKey, TValue>(Guid compressionMethod)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateTreeNodeBase treeNode;
            lock (SyncRoot)
            {
                if (!TreeNodeKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out treeNode))
                    if (!TreeNodeKey.TryGetValue(Tuple.Create(compressionMethod, keyType), out treeNode))
                        if (!TreeNode.TryGetValue(compressionMethod, out treeNode))
                        {
                            new TKey().RegisterImplementations();
                            new TValue().RegisterImplementations();

                            if (!TreeNodeKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out treeNode))
                                if (!TreeNodeKey.TryGetValue(Tuple.Create(compressionMethod, keyType), out treeNode))
                                    if (!TreeNode.TryGetValue(compressionMethod, out treeNode))
                                        throw new Exception("Type is not registered");
                        }
            }
            return new TreeNodeInitializer<TKey, TValue>(treeNode);
        }

        public static SortedTreeNodeBase<TKey, TValue> CreateTreeNode<TKey, TValue>(Guid compressionMethod, byte level)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateTreeNodeBase treeNode;
            lock (SyncRoot)
            {
                if (!TreeNodeKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out treeNode))
                    if (!TreeNodeKey.TryGetValue(Tuple.Create(compressionMethod, keyType), out treeNode))
                        if (!TreeNode.TryGetValue(compressionMethod, out treeNode))
                        {
                            new TKey().RegisterImplementations();
                            new TValue().RegisterImplementations();

                            if (!TreeNodeKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out treeNode))
                                if (!TreeNodeKey.TryGetValue(Tuple.Create(compressionMethod, keyType), out treeNode))
                                    if (!TreeNode.TryGetValue(compressionMethod, out treeNode))
                                        throw new Exception("Type is not registered");
                        }
            }

            return treeNode.Create<TKey, TValue>(level);
        }
    }
}