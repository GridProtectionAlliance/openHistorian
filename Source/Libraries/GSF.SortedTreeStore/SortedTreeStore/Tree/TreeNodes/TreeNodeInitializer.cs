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
using System.Collections;
using System.Collections.Generic;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Tree.TreeNodes;
using GSF.SortedTreeStore.Tree.TreeNodes.FixedSizeNode;

namespace GSF.SortedTreeStore.Tree
{
    /// <summary>
    /// Allows for customized implementations of <see cref="SortedTreeNodeBase{TKey,TValue}"/> 
    /// to be registered so a <see cref="SortedTree{TKey,TValue}"/> will automatically use
    /// this node.
    /// </summary>
    public static class TreeNodeInitializer
    {
        private static readonly object SyncRoot;
        private static readonly HashSet<Type> RegisteredTypes;
        private static readonly Dictionary<Guid, CreateTreeNodeBase> SingleTreeNode;
        private static readonly Dictionary<Tuple<Guid, Type>, CreateTreeNodeBase> SingleTreeNodeKey;
        private static readonly Dictionary<Tuple<Guid, Type>, CreateTreeNodeBase> SingleTreeNodeValue;
        private static readonly Dictionary<Tuple<Guid, Type, Type>, CreateTreeNodeBase> SingleTreeNodeKeyValue;

        private static readonly Dictionary<Tuple<Guid, Guid>, CreateTreeNodeBase> DualTreeNode;
        private static readonly Dictionary<Tuple<Guid, Guid, Type>, CreateTreeNodeBase> DualTreeNodeKey;
        private static readonly Dictionary<Tuple<Guid, Guid, Type>, CreateTreeNodeBase> DualTreeNodeValue;
        private static readonly Dictionary<Tuple<Guid, Guid, Type, Type>, CreateTreeNodeBase> DualTreeNodeKeyValue;

        static TreeNodeInitializer()
        {
            SyncRoot = new object();
            RegisteredTypes = new HashSet<Type>();
            SingleTreeNode = new Dictionary<Guid, CreateTreeNodeBase>();
            SingleTreeNodeKey = new Dictionary<Tuple<Guid, Type>, CreateTreeNodeBase>();
            SingleTreeNodeValue = new Dictionary<Tuple<Guid, Type>, CreateTreeNodeBase>();
            SingleTreeNodeKeyValue = new Dictionary<Tuple<Guid, Type, Type>, CreateTreeNodeBase>();

            DualTreeNode = new Dictionary<Tuple<Guid, Guid>, CreateTreeNodeBase>();
            DualTreeNodeKey = new Dictionary<Tuple<Guid, Guid, Type>, CreateTreeNodeBase>();
            DualTreeNodeValue = new Dictionary<Tuple<Guid, Guid, Type>, CreateTreeNodeBase>();
            DualTreeNodeKeyValue = new Dictionary<Tuple<Guid, Guid, Type, Type>, CreateTreeNodeBase>();

            Register(new CreateFixedSizeNode());
            Register(new CreateDualFixedSizeNode());
        }

        public static void Register<T>(ISortedTreeValue<T> type) 
            where T : class, new()
        {
            lock (SyncRoot)
            {
                if (RegisteredTypes.Add(type.GetType()))
                {
                    IEnumerable encodingMethods = type.GetEncodingMethods();
                    if (encodingMethods == null)
                        return;

                    foreach (var method in encodingMethods)
                    {
                        var single = method as CreateSingleTreeNodeBase;
                        var dual = method as CreateDualTreeNodeBase;

                        if (single != null)
                            Register(single);
                        else if (dual != null)
                            Register(dual);
                    }
                }
            }
        }

        public static void Register(CreateSingleTreeNodeBase encoding)
        {
            lock (SyncRoot)
            {
                if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric == null)
                {
                    SingleTreeNode.Add(encoding.Method, encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric != null && encoding.ValueTypeIfNotGeneric == null)
                {
                    SingleTreeNodeKey.Add(Tuple.Create(encoding.Method, encoding.KeyTypeIfNotGeneric), encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric != null)
                {
                    SingleTreeNodeValue.Add(Tuple.Create(encoding.Method, encoding.ValueTypeIfNotGeneric), encoding);
                }
                else
                {
                    SingleTreeNodeKeyValue.Add(Tuple.Create(encoding.Method, encoding.KeyTypeIfNotGeneric, encoding.ValueTypeIfNotGeneric), encoding);
                }
            }
        }

        public static void Register(CreateDualTreeNodeBase encoding)
        {
            lock (SyncRoot)
            {
                if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric == null)
                {
                    DualTreeNode.Add(Tuple.Create(encoding.KeyMethod, encoding.ValueMethod), encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric != null && encoding.ValueTypeIfNotGeneric == null)
                {
                    DualTreeNodeKey.Add(Tuple.Create(encoding.KeyMethod, encoding.ValueMethod, encoding.KeyTypeIfNotGeneric), encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric != null)
                {
                    DualTreeNodeValue.Add(Tuple.Create(encoding.KeyMethod, encoding.ValueMethod, encoding.ValueTypeIfNotGeneric), encoding);
                }
                else
                {
                    DualTreeNodeKeyValue.Add(Tuple.Create(encoding.KeyMethod, encoding.ValueMethod, encoding.KeyTypeIfNotGeneric, encoding.ValueTypeIfNotGeneric), encoding);
                }
            }
        }


        static CreateTreeNodeBase GetTreeNode<TKey, TValue>(Guid compressionMethod)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateTreeNodeBase customEncoding;

            lock (SyncRoot)
            {
                if (!RegisteredTypes.Contains(keyType))
                {
                    Register(new TKey());
                }
                if (!RegisteredTypes.Contains(valueType))
                {
                    Register(new TValue());
                }

                if (SingleTreeNodeKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out customEncoding)
                    || SingleTreeNodeKey.TryGetValue(Tuple.Create(compressionMethod, keyType), out customEncoding)
                    || SingleTreeNodeValue.TryGetValue(Tuple.Create(compressionMethod, valueType), out customEncoding)
                    || SingleTreeNode.TryGetValue(compressionMethod, out customEncoding))
                {
                    return customEncoding;
                }
            }

            return new CreateGenericEncodedNode<TKey, TValue>(EncodingMethodsLibrary.GetEncodingMethod<TKey, TValue>(compressionMethod));
        }

        static CreateTreeNodeBase GetTreeNode<TKey, TValue>(Guid keyEncodingMethod, Guid valueEncodingMethod)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateTreeNodeBase customEncoding;

            lock (SyncRoot)
            {
                if (!RegisteredTypes.Contains(keyType))
                {
                    Register(new TKey());
                }
                if (!RegisteredTypes.Contains(valueType))
                {
                    Register(new TValue());
                }

                if (DualTreeNodeKeyValue.TryGetValue(Tuple.Create(keyEncodingMethod, valueEncodingMethod, keyType, valueType), out customEncoding)
                    || DualTreeNodeKey.TryGetValue(Tuple.Create(keyEncodingMethod, valueEncodingMethod, keyType), out customEncoding)
                    || DualTreeNodeValue.TryGetValue(Tuple.Create(keyEncodingMethod, valueEncodingMethod, valueType), out customEncoding)
                    || DualTreeNode.TryGetValue(Tuple.Create(keyEncodingMethod, valueEncodingMethod), out customEncoding))
                {
                    return customEncoding;
                }
            }
            return new CreateGenericEncodedNode<TKey, TValue>(EncodingMethodsLibrary.GetEncodingMethod<TKey, TValue>(keyEncodingMethod, valueEncodingMethod));
        }

        internal static TreeNodeInitializer<TKey, TValue> GetTreeNodeInitializer<TKey, TValue>(Guid compressionMethod)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {

            return new TreeNodeInitializer<TKey, TValue>(GetTreeNode<TKey, TValue>(compressionMethod));
        }

        internal static TreeNodeInitializer<TKey, TValue> GetTreeNodeInitializer<TKey, TValue>(Guid keyEncodingMethod, Guid valueEncodingMethod)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {

            return new TreeNodeInitializer<TKey, TValue>(GetTreeNode<TKey, TValue>(keyEncodingMethod, valueEncodingMethod));
        }

        internal static SortedTreeNodeBase<TKey, TValue> CreateTreeNode<TKey, TValue>(Guid compressionMethod, byte level)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            return GetTreeNode<TKey, TValue>(compressionMethod).Create<TKey, TValue>(level);
        }

        internal static SortedTreeNodeBase<TKey, TValue> CreateTreeNode<TKey, TValue>(Guid keyEncodingMethod, Guid valueEncodingMethod, byte level)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            return GetTreeNode<TKey, TValue>(keyEncodingMethod, valueEncodingMethod).Create<TKey, TValue>(level);
        }
    }
}