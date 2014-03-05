//******************************************************************************************************
//  PointCollectionInitializer.cs - Gbtc
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
//  2/5/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Collection
{
    public static class PointCollectionInitializer
    {
        private static readonly object SyncRoot;
        private static readonly Dictionary<Tuple<Type, Type>, CreatePointCollectionBase> TreeNodeKeyValue;
        private static readonly Dictionary<Type, CreatePointCollectionBase> TreeNodeKey;

        static PointCollectionInitializer()
        {
            SyncRoot = new object();
            TreeNodeKeyValue = new Dictionary<Tuple<Type, Type>, CreatePointCollectionBase>();
            TreeNodeKey = new Dictionary<Type, CreatePointCollectionBase>();
        }

        public static void Register(CreatePointCollectionBase treeNode)
        {
            lock (SyncRoot)
            {
                if (treeNode.KeyTypeIfFixed != null && treeNode.ValueTypeIfFixed == null)
                {
                    TreeNodeKey.Add(treeNode.KeyTypeIfFixed, treeNode);
                }
                else if (treeNode.KeyTypeIfFixed != null && treeNode.ValueTypeIfFixed != null)
                {
                    TreeNodeKeyValue.Add(Tuple.Create(treeNode.KeyTypeIfFixed, treeNode.ValueTypeIfFixed), treeNode);
                }
                else
                {
                    throw new InvalidDataException("Type is not supported");
                }
            }
        }

        public static PointBuffer<TKey, TValue> Create<TKey, TValue>(int capacity)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreatePointCollectionBase treeNode;
            lock (SyncRoot)
            {
                if (!TreeNodeKeyValue.TryGetValue(Tuple.Create(keyType, valueType), out treeNode))
                    if (!TreeNodeKey.TryGetValue(keyType, out treeNode))
                    {
                        //new TKey().RegisterCustomKeyImplementations();
                        //new TValue().RegisterCustomValueImplementations();

                        if (!TreeNodeKeyValue.TryGetValue(Tuple.Create(keyType, valueType), out treeNode))
                            if (!TreeNodeKey.TryGetValue(keyType, out treeNode))
                                return new PointBuffer<TKey, TValue>(capacity);

                    }
            }

            return treeNode.Create<TKey, TValue>(capacity);
        }
    }
}