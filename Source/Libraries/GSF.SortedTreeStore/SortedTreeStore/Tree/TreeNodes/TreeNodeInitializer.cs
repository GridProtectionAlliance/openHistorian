//******************************************************************************************************
//  TreeNodeInitializer.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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

        private static readonly DualEncodingDictionary<CreateTreeNodeBase> DoubleEncoding;

        static TreeNodeInitializer()
        {
            DoubleEncoding = new DualEncodingDictionary<CreateTreeNodeBase>();
            Register(new CreateFixedSizeNode());
            Register(new CreateDualFixedSizeNode());
        }

        public static void Register(CreateTreeNodeBase encoding)
        {
            if ((object)encoding == null)
                throw new ArgumentNullException("encoding");

            DoubleEncoding.Register(encoding);
        }

        internal static CreateTreeNodeBase GetTreeNodeInitializer<TKey, TValue>(EncodingDefinition encodingMethod)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            if ((object)encodingMethod == null)
                throw new ArgumentNullException("encodingMethod");

            CreateTreeNodeBase encoding;

            if (DoubleEncoding.TryGetEncodingMethod<TKey, TValue>(encodingMethod, out encoding))
                return encoding;

            return new CreateGenericEncodedNode<TKey, TValue>(Library.Encodings.GetEncodingMethod<TKey, TValue>(encodingMethod));
        }

        internal static SortedTreeNodeBase<TKey, TValue> CreateTreeNode<TKey, TValue>(EncodingDefinition encodingMethod, byte level)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            if ((object)encodingMethod == null)
                throw new ArgumentNullException("encodingMethod");

            return GetTreeNodeInitializer<TKey, TValue>(encodingMethod).Create<TKey, TValue>(level);
        }

    }
}