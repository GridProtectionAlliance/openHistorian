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
        private DoubleValueEncodingBase<TKey, TValue> m_encoding;
        private Func<uint> m_getNextNewNodeIndex;
        private SparseIndex<TKey> m_sparseIndex;
        private int m_nextOffset;
        private readonly TKey m_prevKey;
        private readonly TValue m_prevValue;
        private byte[] m_buffer1;
        private int m_maximumStorageSize;
        private readonly TKey m_maxKey;

        #endregion

        #region [ Constructors ]

        public NodeWriter(EncodingDefinition encodingMethod, byte level, BinaryStreamPointerBase stream, int blockSize, Func<uint> getNextNewNodeIndex, SparseIndex<TKey> sparseIndex)
            : base(level, stream, blockSize)
        {
            m_encoding = Library.Encodings.GetEncodingMethod<TKey, TValue>(encodingMethod);
            m_prevKey = new TKey();
            m_prevValue = new TValue();
            m_maxKey = new TKey();
            m_maxKey.SetMax();

            NodeIndexChanged += OnNodeIndexChanged;
            ClearNodeCache();

            m_sparseIndex = sparseIndex;
            m_getNextNewNodeIndex = getNextNewNodeIndex;
            m_maximumStorageSize = m_encoding.MaxCompressionSize;
            m_buffer1 = new byte[m_maximumStorageSize];
            if ((BlockSize - HeaderSize) / m_maximumStorageSize < 4)
                throw new Exception("Tree must have at least 4 records per node. Increase the block size or decrease the size of the records.");

        }

        /// <summary>
        /// Inserts the supplied sequential stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Insert(TreeStream<TKey, TValue> stream)
        {
            fixed (byte* buffer = m_buffer1)
            {
                TKey key1 = new TKey();
                TKey key2 = new TKey();
                TValue value1 = new TValue();
                TValue value2 = new TValue();

                key1.Clear();
                key2.Clear();
                value1.Clear();
                value2.Clear();
                int length;
                byte* writePointer = GetWritePointer();

            Read1:
                //Read part 1.
                if (stream.Read(key1, value1))
                {
                    if (RemainingBytes < m_maximumStorageSize)
                    {
                        if (RemainingBytes < EncodeRecord(buffer, key2, value2, key1, value1))
                        {
                            NewNodeThenInsert(key1, value1);
                            key1.Clear();
                            value1.Clear();
                            writePointer = GetWritePointer();
                            goto Read2;
                        }
                    }

                    length = EncodeRecord(writePointer + m_nextOffset, key2, value2, key1, value1);
                    IncrementOneRecord(length);
                    m_nextOffset += length;


                Read2:
                    //Read part 2.
                    if (stream.Read(key2, value2))
                    {
                        if (RemainingBytes < m_maximumStorageSize)
                        {
                            if (RemainingBytes < EncodeRecord(buffer, key1, value1, key2, value2))
                            {
                                NewNodeThenInsert(key2, value2);
                                key2.Clear();
                                value2.Clear();
                                writePointer = GetWritePointer();
                                goto Read1;
                            }
                        }

                        length = EncodeRecord(writePointer + m_nextOffset, key1, value1, key2, value2);
                        IncrementOneRecord(length);
                        m_nextOffset += length;

                        goto Read1;
                    }
                }
            }
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
            RightSiblingNodeIndex = newNodeIndex;
            UpperKey = key;

            CreateNewNode(newNodeIndex, 0, (ushort)HeaderSize, NodeIndex, uint.MaxValue, key, m_maxKey);
            SetNodeIndex(newNodeIndex);

            InsertUnlessFull(key, value);

            m_sparseIndex.Add(dividingKey, newNodeIndex, (byte)(Level + 1));
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
        /// Inserts a point before the current position.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool InsertUnlessFull(TKey key, TValue value)
        {
            if (RemainingBytes < m_maximumStorageSize)
            {
                fixed (byte* buffer = m_buffer1)
                {
                    if (RemainingBytes < EncodeRecord(buffer, m_prevKey, m_prevValue, key, value))
                        return false;
                }
            }

            int length = EncodeRecord(GetWritePointer() + m_nextOffset, m_prevKey, m_prevValue, key, value);
            IncrementOneRecord(length);

            key.CopyTo(m_prevKey);
            value.CopyTo(m_prevValue);
            m_nextOffset += length;
            //ResetPositionCached();

            return true;
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
