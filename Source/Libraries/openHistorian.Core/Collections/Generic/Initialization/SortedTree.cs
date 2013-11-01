//******************************************************************************************************
//  SortedTree.cs - Gbtc
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
using GSF.IO;

namespace openHistorian.Collections.Generic
{
    //public delegate SortedTreeKeyMethodsBase CreateKeyMethod();

    //public delegate SortedTreeValueMethodsBase CreateValueMethod();

    public static class SortedTree
    {
        private static readonly object s_syncRoot;

        private static readonly Dictionary<Tuple<Guid, Type, Type>, CreateTreeNodeBase> s_treeNodeKeyValue;
        private static readonly Dictionary<Tuple<Guid, Type>, CreateTreeNodeBase> s_treeNodeKey;
        private static readonly Dictionary<Guid, CreateTreeNodeBase> s_treeNode;

        //static Dictionary<Type, SortedTreeValueMethodsBase> s_valueMethods;

        static SortedTree()
        {
            s_syncRoot = new object();
            s_treeNodeKeyValue = new Dictionary<Tuple<Guid, Type, Type>, CreateTreeNodeBase>();
            s_treeNodeKey = new Dictionary<Tuple<Guid, Type>, CreateTreeNodeBase>();
            s_treeNode = new Dictionary<Guid, CreateTreeNodeBase>();

            RegisterSortedTreeTypes.RegisterTreeNodeType();
        }

        public static void ReadHeader(BinaryStreamBase stream, out Guid sparseIndexType, out Guid treeNodeType, out int blockSize)
        {
            stream.Position = 0;
            sparseIndexType = stream.ReadGuid();
            treeNodeType = stream.ReadGuid();
            blockSize = stream.ReadInt32();
        }

        public static void Register(CreateTreeNodeBase treeNode)
        {
            lock (s_syncRoot)
            {
                if (treeNode.KeyTypeIfFixed == null && treeNode.ValueTypeIfFixed == null)
                {
                    s_treeNode.Add(treeNode.GetTypeGuid, treeNode);
                }
                else if (treeNode.KeyTypeIfFixed != null && treeNode.ValueTypeIfFixed == null)
                {
                    s_treeNodeKey.Add(Tuple.Create(treeNode.GetTypeGuid, treeNode.KeyTypeIfFixed), treeNode);
                }
                else if (treeNode.KeyTypeIfFixed != null && treeNode.ValueTypeIfFixed != null)
                {
                    s_treeNodeKeyValue.Add(Tuple.Create(treeNode.GetTypeGuid, treeNode.KeyTypeIfFixed, treeNode.ValueTypeIfFixed), treeNode);
                }
                else
                {
                    throw new InvalidDataException("Type is not supported");
                }
            }
        }

        //public static TreeValueMethodsBase<TValue> GetTreeValueMethods<TValue>()
        //    where TValue : class, new()
        //{
        //    Type valueType = typeof(TValue);
        //    CreateValueMethodBase<TValue> valueMethods;
        //    object objValueMethods;
        //    lock (s_syncRoot)
        //    {
        //        if (!s_valueMethods.TryGetValue(valueType, out objValueMethods))
        //            throw new Exception("Value Type is not a registered type");
        //    }
        //    valueMethods = (CreateValueMethodBase<TValue>)objValueMethods;
        //    return valueMethods.Create();
        //}

        //public static TreeKeyMethodsBase<TKey> GetTreeKeyMethods<TKey>()
        //    where TKey : class, new()
        //{
        //    Type keyType = typeof(TKey);
        //    CreateKeyMethodBase<TKey> keyMethods;
        //    object objKeyMethods;
        //    lock (s_syncRoot)
        //    {
        //        if (!s_keyMethods.TryGetValue(keyType, out objKeyMethods))
        //            throw new Exception("Key Type is not a registered type");
        //    }
        //    keyMethods = (CreateKeyMethodBase<TKey>)objKeyMethods;
        //    return keyMethods.Create();
        //}

        public static TreeNodeInitializer<TKey, TValue> GetTreeNodeInitializer<TKey, TValue>(Guid compressionMethod)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateTreeNodeBase treeNode;
            lock (s_syncRoot)
            {
                if (!s_treeNodeKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out treeNode))
                    if (!s_treeNodeKey.TryGetValue(Tuple.Create(compressionMethod, keyType), out treeNode))
                        if (!s_treeNode.TryGetValue(compressionMethod, out treeNode))
                            throw new Exception("Type is not registered");
            }
            return new TreeNodeInitializer<TKey, TValue>(treeNode);
        }

        public static TreeNodeBase<TKey, TValue> CreateTreeNode<TKey, TValue>(Guid compressionMethod, byte level)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateTreeNodeBase treeNode;
            lock (s_syncRoot)
            {
                if (!s_treeNodeKeyValue.TryGetValue(Tuple.Create(compressionMethod, keyType, valueType), out treeNode))
                    if (!s_treeNodeKey.TryGetValue(Tuple.Create(compressionMethod, keyType), out treeNode))
                        if (!s_treeNode.TryGetValue(compressionMethod, out treeNode))
                            throw new Exception("Type is not registered");
            }

            return treeNode.Create<TKey, TValue>(level);
        }
    }
}