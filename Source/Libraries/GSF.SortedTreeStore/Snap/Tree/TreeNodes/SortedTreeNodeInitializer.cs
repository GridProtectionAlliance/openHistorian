//******************************************************************************************************
//  SortedTreeNodeInitializer.cs - Gbtc
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
//  04/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.Snap.Definitions;
using GSF.Snap.Encoding;
using GSF.Snap.Tree.TreeNodes;
using GSF.Snap.Tree.TreeNodes.FixedSizeNode;

namespace GSF.Snap.Tree
{
    /// <summary>
    /// Allows for customized implementations of <see cref="SortedTreeNodeBase{TKey,TValue}"/> 
    /// to be registered so a <see cref="SortedTree{TKey,TValue}"/> will automatically use
    /// this node.
    /// </summary>
    public class SortedTreeNodeInitializer
    {
        private readonly CombinedEncodingDictionary<SortedTreeNodeBaseDefinition> m_doubleEncoding;

        internal SortedTreeNodeInitializer()
        {
            m_doubleEncoding = new CombinedEncodingDictionary<SortedTreeNodeBaseDefinition>();
        }

        public void Register(SortedTreeNodeBaseDefinition encoding)
        {
            if ((object)encoding == null)
                throw new ArgumentNullException("encoding");

            m_doubleEncoding.Register(encoding);
        }

        internal SortedTreeNodeBaseDefinition GetTreeNodeInitializer<TKey, TValue>(EncodingDefinition encodingMethod)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if ((object)encodingMethod == null)
                throw new ArgumentNullException("encodingMethod");

            SortedTreeNodeBaseDefinition encoding;

            if (m_doubleEncoding.TryGetEncodingMethod<TKey, TValue>(encodingMethod, out encoding))
                return encoding;

            return new GenericEncodedTreeNodeDefinition<TKey, TValue>(Library.Encodings.GetEncodingMethod<TKey, TValue>(encodingMethod));
        }

        internal SortedTreeNodeBase<TKey, TValue> CreateTreeNode<TKey, TValue>(EncodingDefinition encodingMethod, byte level)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if ((object)encodingMethod == null)
                throw new ArgumentNullException("encodingMethod");

            return GetTreeNodeInitializer<TKey, TValue>(encodingMethod).Create<TKey, TValue>(level);
        }

    }
}