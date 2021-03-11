//******************************************************************************************************
//  NodeWriter`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
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
using GSF.Snap.Encoding;

namespace GSF.Snap.Tree.Specialized
{
    /// <summary>
    /// A class to write data to a node in sequential order only.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public unsafe static class NodeWriter<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {

        public static void Create(EncodingDefinition encodingMethod, BinaryStreamPointerBase stream, int blockSize, byte level, uint startingNodeIndex, Func<uint> getNextNewNodeIndex, SparseIndexWriter<TKey> sparseIndex, TreeStream<TKey, TValue> treeStream)
        {
            NodeHeader<TKey> header = new NodeHeader<TKey>(level, blockSize);
            PairEncodingBase<TKey, TValue> encoding = Library.Encodings.GetEncodingMethod<TKey, TValue>(encodingMethod);

            SparseIndexWriter<TKey> sparseIndex1 = sparseIndex;
            Func<uint> getNextNewNodeIndex1 = getNextNewNodeIndex;
            int maximumStorageSize = encoding.MaxCompressionSize;
            byte[] buffer1 = new byte[maximumStorageSize];
            if ((header.BlockSize - header.HeaderSize) / maximumStorageSize < 4)
                throw new Exception("Tree must have at least 4 records per node. Increase the block size or decrease the size of the records.");

            //InsideNodeBoundary = m_BoundsFalse;
            header.NodeIndex = startingNodeIndex;
            header.RecordCount = 0;
            header.ValidBytes = (ushort)header.HeaderSize;
            header.LeftSiblingNodeIndex = uint.MaxValue;
            header.RightSiblingNodeIndex = uint.MaxValue;
            header.LowerKey.SetMin();
            header.UpperKey.SetMax();

            byte* writePointer = stream.GetWritePointer(blockSize * header.NodeIndex, blockSize);
            fixed (byte* buffer = buffer1)
            {
                TKey key1 = new TKey();
                TKey key2 = new TKey();
                TValue value1 = new TValue();
                TValue value2 = new TValue();

                key1.Clear();
                key2.Clear();
                value1.Clear();
                value2.Clear();

            Read1:
                //Read part 1.
                if (treeStream.Read(key1, value1))
                {
                    if (header.RemainingBytes < maximumStorageSize)
                    {
                        if (header.RemainingBytes < encoding.Encode(buffer, key2, value2, key1, value1))
                        {
                            NewNodeThenInsert(header, sparseIndex1, getNextNewNodeIndex1(), writePointer, key1);
                            key2.Clear();
                            value2.Clear();
                            writePointer = stream.GetWritePointer(blockSize * header.NodeIndex, blockSize);
                        }
                    }

                    byte* stream1 = writePointer + header.ValidBytes;
                    header.ValidBytes += (ushort)encoding.Encode(stream1, key2, value2, key1, value1);
                    header.RecordCount++;

                    //Read part 2.
                    if (treeStream.Read(key2, value2))
                    {
                        if (header.RemainingBytes < maximumStorageSize)
                        {
                            if (header.RemainingBytes < encoding.Encode(buffer, key1, value1, key2, value2))
                            {
                                NewNodeThenInsert(header, sparseIndex1, getNextNewNodeIndex1(), writePointer, key2);
                                key1.Clear();
                                value1.Clear();
                                writePointer = stream.GetWritePointer(blockSize * header.NodeIndex, blockSize);
                            }
                        }
                        byte* stream2 = writePointer + header.ValidBytes;
                        header.ValidBytes += (ushort)encoding.Encode(stream2, key1, value1, key2, value2);
                        header.RecordCount++;

                        goto Read1;
                    }
                }
            }
            header.Save(writePointer);
        }

        /// <summary>
        /// Closes the current node and prepares a new node with the supplied key.
        /// </summary>
        /// <param name="sparseIndex"></param>
        /// <param name="newNodeIndex">the index for the next node.</param>
        /// <param name="writePointer">the pointer to the start of the block</param>
        /// <param name="key">the key to use.</param>
        /// <param name="header"></param>
        private static void NewNodeThenInsert(NodeHeader<TKey> header, SparseIndexWriter<TKey> sparseIndex, uint newNodeIndex, byte* writePointer, TKey key)
        {
            TKey dividingKey = new TKey(); //m_tempKey;
            key.CopyTo(dividingKey);

            uint currentNode = header.NodeIndex;

            //Finish this header.
            header.RightSiblingNodeIndex = newNodeIndex;
            key.CopyTo(header.UpperKey);
            header.Save(writePointer);

            //Prepare the next header
            header.NodeIndex = newNodeIndex;
            header.RecordCount = 0;
            header.ValidBytes = (ushort)header.HeaderSize;
            header.LeftSiblingNodeIndex = currentNode;
            header.RightSiblingNodeIndex = uint.MaxValue;
            key.CopyTo(header.LowerKey);
            header.UpperKey.SetMax();

            sparseIndex.Add(currentNode, dividingKey, newNodeIndex);
        }
    }
}
