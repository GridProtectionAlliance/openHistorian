//******************************************************************************************************
//  NodeWriter`2.cs - Gbtc
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
//  10/09/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Encoding;

namespace GSF.SortedTreeStore.Tree.Specialized
{
    /// <summary>
    /// A class to write data to a node in sequential order only.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public unsafe class NodeWriter<TKey, TValue>
        : NodeHeader<TKey>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        #region [ Members ]
        DoubleValueEncodingBase<TKey, TValue> m_encoding;
        private Func<uint> m_getNextNewNodeIndex;
        protected SparseIndexWriter<TKey> SparseIndex;
        int m_nextOffset;
        private readonly TKey m_prevKey;
        private readonly TValue m_prevValue;
        private byte[] m_buffer1;

        #endregion

        #region [ Constructors ]

        public NodeWriter(EncodingDefinition encodingMethod, byte level, byte version, BinaryStreamPointerBase stream, int blockSize, Func<uint> getNextNewNodeIndex, SparseIndexWriter<TKey> sparseIndex)
            : base(level, version)
        {
            m_encoding = Library.Encodings.GetEncodingMethod<TKey, TValue>(encodingMethod);
            m_prevKey = new TKey();
            m_prevValue = new TValue();

            NodeIndexChanged += OnNodeIndexChanged;
            ClearNodeCache();
            InitializeNode(stream, blockSize);

            SparseIndex = sparseIndex;
            m_getNextNewNodeIndex = getNextNewNodeIndex;

            m_buffer1 = new byte[MaximumStorageSize];
            if ((BlockSize - HeaderSize) / MaximumStorageSize < 4)
                throw new Exception("Tree must have at least 4 records per node. Increase the block size or decrease the size of the records.");
        }

        /// <summary>
        /// Inserts the following value to the tree if it does not exist.
        /// </summary>
        /// <param name="key">The key to add</param>
        /// <param name="value">The value to add</param>
        /// <returns>True if added, False on a duplicate key error</returns>
        /// <remarks>
        /// This is a slower but more complete implementation of <see cref="TryInsert"/>.
        /// Overriding classes can call this method after implementing their own high speed TryGet method.
        /// </remarks>
        public void Insert(TKey key, TValue value)
        {
            if (InsertUnlessFull(key, value))
                return;

            //Check if the node needs to be split
            NewNodeThenInsert(key, value);
        }

        /// <summary>
        /// Creates an empty right sibling node and inserts the provided key in this node.
        /// Note: This should only be called if there is no right sibling and the key should go in
        /// that node.
        /// </summary>
        private void NewNodeThenInsert(TKey key, TValue value)
        {
            TKey dividingKey = new TKey(); //m_tempKey;
            key.CopyTo(dividingKey);

            uint newNodeIndex = m_getNextNewNodeIndex();
            if (!IsRightSiblingIndexNull)
                throw new Exception("Incorrectly implemented");

            RightSiblingNodeIndex = newNodeIndex;

            CreateNewNode(newNodeIndex, 0, (ushort)HeaderSize, NodeIndex, uint.MaxValue, key, UpperKey);

            UpperKey = key;

            SetNodeIndex(newNodeIndex);

            InsertUnlessFull(key, value);

            SparseIndex.Add(dividingKey, newNodeIndex, (byte)(Level + 1));
        }

        #endregion

        /// <summary>
        /// Encodes this record to the provided stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="prevKey"></param>
        /// <param name="prevValue"></param>
        /// <param name="currentKey"></param>
        /// <param name="currentValue"></param>
        /// <returns>returns the number of bytes read from the stream</returns>
        private int EncodeRecord(byte* stream, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue)
        {
            return m_encoding.Encode(stream, prevKey, prevValue, currentKey, currentValue);
        }

        /// <summary>
        /// The maximum size that will ever be needed to encode or decode this data.
        /// </summary>
        protected int MaximumStorageSize
        {
            get
            {
                return m_encoding.MaxCompressionSize;
            }
        }

        /// <summary>
        /// Inserts a point before the current position.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool InsertUnlessFull(TKey key, TValue value)
        {
            fixed (byte* buffer = m_buffer1)
            {
                int length = EncodeRecord(buffer, m_prevKey, m_prevValue, key, value);

                if (RemainingBytes < length)
                    return false;

                EncodeRecord(GetWritePointer() + m_nextOffset, m_prevKey, m_prevValue, key, value);
                //WinApi.MoveMemory(GetWritePointer() + m_nextOffset, buffer, length);
                IncrementOneRecord(length);

                key.CopyTo(m_prevKey);
                value.CopyTo(m_prevValue);
                m_nextOffset += length;
                //ResetPositionCached();

                return true;
            }
        }

        #region [ Starter Code ]

        void OnNodeIndexChanged(object sender, EventArgs e)
        {
            ClearNodeCache();
        }

        void ClearNodeCache()
        {
            m_nextOffset = HeaderSize;
            m_prevKey.Clear();
            m_prevValue.Clear();
        }

        #endregion

    }
}
