//******************************************************************************************************
//  GenericEncodedNode`2.cs - Gbtc
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
//  5/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using GSF.SortedTreeStore.Encoding;

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    internal class CreateGenericEncodedNode<TKey, TValue>
        : CreateTreeNodeBase
        where TKey : class, ISortedTreeValue<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        DoubleValueEncodingBase<TKey, TValue> m_encoding;

        public CreateGenericEncodedNode(DoubleValueEncodingBase<TKey, TValue> encoding)
        {
            m_encoding = encoding;
        }

        public override SortedTreeNodeBase<TKey1, TValue1> Create<TKey1, TValue1>(byte level)
        {
            return (SortedTreeNodeBase<TKey1, TValue1>)(object)new GenericEncodedNode<TKey, TValue>(m_encoding.Clone(), level);
        }
    }

    /// <summary>
    /// A TreeNode abstract class that is used for linearly encoding a class.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public unsafe class GenericEncodedNode<TKey, TValue>
        : EncodedNodeBase<TKey, TValue>
        where TKey : class, ISortedTreeValue<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        DoubleValueEncodingBase<TKey, TValue> m_encoding;

        public GenericEncodedNode(DoubleValueEncodingBase<TKey, TValue> encoding, byte level)
            : base(level, 2)
        {
            m_encoding = encoding;
        }

        public override SortedTreeScannerBase<TKey, TValue> CreateTreeScanner()
        {
            return new GenericEncodedNodeScanner<TKey, TValue>(m_encoding, Level, BlockSize, Stream, SparseIndex.Get);
        }

        protected override int MaxOverheadWithCombineNodes
        {
            get
            {
                return MaximumStorageSize * 2 + 1;
            }
        }

        protected override int EncodeRecord(byte* stream, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue)
        {
            return m_encoding.Compress(stream, prevKey, prevValue, currentKey, currentValue);
        }

        protected override int DecodeRecord(byte* stream, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue)
        {
            return m_encoding.Decompress(stream, prevKey, prevValue, currentKey, currentValue);
        }

        protected override int MaximumStorageSize
        {
            get
            {
                return m_encoding.MaxCompressionSize;
            }
        }
    }
}