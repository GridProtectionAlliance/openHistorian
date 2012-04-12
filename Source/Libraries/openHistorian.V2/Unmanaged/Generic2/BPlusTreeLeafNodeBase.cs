//******************************************************************************************************
//  BPlusTreeLeafNodeBase.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  4/7/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.V2.Unmanaged.Generic2
{
    public abstract class BPlusTreeLeafNodeBase<TKey, TValue> : BPlusTreeInternalNodeBase<TKey, TValue>
    {
        #region [ Nexted Types ]

        /// <summary>
        /// Assists in the read/write operations of the header of a node.
        /// </summary>
        struct NodeHeader
        {
            public const int Size = 11;
            public byte Level;
            public short ChildCount;
            public uint PreviousNode;
            public uint NextNode;

            public void Load(BinaryStream stream, int blockSize, uint nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                Load(stream);
            }

            public void Load(BinaryStream stream)
            {
                Level = stream.ReadByte();
                ChildCount = stream.ReadInt16();
                PreviousNode = stream.ReadUInt32();
                NextNode = stream.ReadUInt32();
            }
            /// <summary>
            /// Saves the node data to the underlying stream. 
            /// </summary>
            /// <param name="header"></param>
            /// <param name="nodeIndex"></param>
            public void Save(BinaryStream stream, int blockSize, uint nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                Save(stream);
            }
            /// <summary>
            /// Saves the node data to the underlying stream.
            /// </summary>
            /// <param name="stream"></param>
            /// <remarks>
            /// From the current position on the stream, the node header data is written to the stream.
            /// The position after calling this function is at the end of the header
            /// </remarks>
            public void Save(BinaryStream stream)
            {
                stream.Write(Level);
                stream.Write(ChildCount);
                stream.Write(PreviousNode);
                stream.Write(NextNode);
            }
        }


        private class DataReader : DataReaderBase
        {
            BPlusTreeLeafNodeBase<TKey, TValue> m_tree;
            bool m_scanningTable;
            bool m_isValid;

            TKey m_startKey;
            TKey m_stopKey;
            bool m_readToEndOfStream;

            int m_oldIndex;
            short m_childCount;
            uint m_nextNode;
            uint m_origionalNode;
            uint m_currentNode;
            Func<TKey, bool> m_filter;

            TKey m_key;
            TValue m_value;

            public DataReader(BPlusTreeLeafNodeBase<TKey, TValue> tree, TKey firstKey, TKey lastKey, uint currentNode, Func<TKey, bool> filter, bool readToEndOfStream)
            {
                m_readToEndOfStream = readToEndOfStream;
                m_filter = filter;
                m_tree = tree;
                m_startKey = firstKey;
                m_stopKey = lastKey;
                m_origionalNode = currentNode;
                ReaderReset();
            }

            protected override void ReaderReset()
            {
                m_currentNode = m_origionalNode;
                uint previousNode;
                m_tree.LoadCurrentNode(m_currentNode, false, out m_childCount, out previousNode, out m_nextNode);
                m_tree.LeafNodeSeekToKey(m_startKey, out m_oldIndex, m_currentNode, m_childCount);
                m_oldIndex = (m_oldIndex - NodeHeader.Size) / m_tree.m_structureSize;
            }

            protected override bool ReaderNext()
            {
                do
                {
                    if (m_oldIndex >= m_childCount)
                    {
                        if (m_nextNode == 0)
                            return false;

                        uint previousNode;
                        m_tree.LoadCurrentNode(m_currentNode, false, out m_childCount, out previousNode, out m_nextNode);
                        m_oldIndex = 0;
                    }
                    m_tree.Stream.Position = m_currentNode * m_tree.BlockSize + m_oldIndex * m_tree.m_structureSize +
                                             NodeHeader.Size;

                    m_key = m_tree.LoadKey(m_tree.Stream);
                    if (!m_readToEndOfStream && m_tree.CompareKeys(m_stopKey, m_key) <= 0)
                        return false;
                    m_value = m_tree.LoadValue(m_tree.Stream);

                    m_oldIndex++;

                } while (m_filter != null && !m_filter.Invoke(m_key));

                return true;

            }

            protected override TValue ReaderGetValue()
            {
                if (!m_isValid)
                    throw new Exception("Value is no longer valid.  Either the end of the stream has been encoutered or the initial read was never performed");
                return m_value;
            }

            protected override TKey ReaderGetKey()
            {
                if (!m_isValid)
                    throw new Exception("Key is no longer valid.  Either the end of the stream has been encoutered or the initial read was never performed");
                return m_key;
            }
        }

        #endregion

        #region [ Members ]

        int m_keySize;
        int m_maximumChildren;
        int m_structureSize;

        #endregion

        #region [ Constructors ]

        protected BPlusTreeLeafNodeBase(BinaryStream stream)
            : base(stream)
        {
            Initialize();
        }


        protected BPlusTreeLeafNodeBase(BinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
            Initialize();
        }

        #endregion

        #region [ Properties ]

        #endregion
        #region [ Methods ]

        #region [ Abstract Methods ]

        protected abstract void SaveValue(TValue value, BinaryStream stream);
        protected abstract TValue LoadValue(BinaryStream stream);
        protected abstract int SizeOfValue();

        #endregion
        #region [ Override Methods ]


        protected override bool LeafNodeUpdate(uint nodeIndex, TKey key, TValue value)
        {
            throw new System.NotImplementedException();
        }

        protected override bool LeafNodeRemove(uint nodeIndex, TKey key)
        {
            throw new System.NotImplementedException();
        }

        protected override DataReaderBase LeafNodeScan(uint nodeIndex, TKey startKey, Func<TKey, bool> filter)
        {
            return new DataReader(this, startKey, default(TKey), nodeIndex, filter, true);
        }

        protected override DataReaderBase LeafNodeScan(uint nodeIndex, TKey startKey, TKey stopKey, Func<TKey, bool> filter)
        {
            return new DataReader(this, startKey, stopKey, nodeIndex, filter, false);
        }

        protected override bool LeafNodeInsert(uint nodeIndex, TKey key, TValue value)
        {

            short childCount;
            uint previousNode;
            uint nextNode;

            LoadCurrentNode(nodeIndex, false, out childCount, out previousNode, out nextNode);

            int offset;
            long nodePositionStart = nodeIndex * BlockSize;

            if (childCount >= m_maximumChildren)
            {
                LeafNodeSplitNode(key, value, nodeIndex, nextNode);
                return true;
            }

            //Find the best location to insert
            if (LeafNodeSeekToKey(key, out offset, nodeIndex, childCount)) //If found
                return false;

            int spaceToMove = NodeHeader.Size + m_structureSize * childCount - offset;

            //Insert the data
            if (spaceToMove > 0)
            {
                Stream.Position = nodeIndex * BlockSize + offset;
                Stream.InsertBytes(m_structureSize, spaceToMove);
            }

            Stream.Position = nodeIndex * BlockSize + offset;
            SaveKey(key, Stream);
            SaveValue(value, Stream);

            //save the header
            childCount++;
            Stream.Position = nodeIndex * BlockSize + 1;
            Stream.Write(childCount);
            return true;
        }

        protected override bool LeafNodeGetValue(uint nodeIndex, TKey key, out TValue value)
        {
            short childCount;
            uint previousNode;
            uint nextNode;

            LoadCurrentNode(nodeIndex, false, out childCount, out previousNode, out nextNode);
            int offset;
            if (LeafNodeSeekToKey(key, out offset, nodeIndex, childCount))
            {
                Stream.Position = nodeIndex * BlockSize + (offset + m_keySize);
                value = LoadValue(Stream);
                return true;
            }
            value = default(TValue);
            return false;
        }

        protected override bool LeafNodeGetFirstKeyValue(uint nodeIndex, out TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        protected override bool LeafNodeGetLastKeyValue(uint nodeIndex, out TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }
        protected override uint LeafNodeCreateEmptyNode()
        {
            uint nodeAddress = AllocateNewNode();
            Stream.Position = BlockSize * nodeAddress;

            //Clearing the Node
            //Level = 0;
            //ChildCount = 0;
            //NextNode = 0;
            //PreviousNode = 0;
            Stream.Write(0L);
            Stream.Write(0);

            return nodeAddress;
        }

        #endregion

        #region [ Helper Methods ]
        void Initialize()
        {
            m_keySize = SizeOfKey();
            m_structureSize = m_keySize + SizeOfValue();
            m_maximumChildren = (BlockSize - NodeHeader.Size) / (m_structureSize);
        }

        void LoadCurrentNode(uint nodeIndex, bool isForWriting, out short childCount, out uint previousNode, out uint nextNode)
        {
            Stream.Position = nodeIndex * BlockSize;
            Stream.UpdateLocalBuffer(isForWriting);

            if (Stream.ReadByte() != 0)
                throw new Exception("The current node is not a leaf.");
            childCount = Stream.ReadInt16();
            previousNode = Stream.ReadUInt32();
            nextNode = Stream.ReadUInt32();
        }


        /// <summary>
        /// Seeks to the location of the key. Or the position where the key could be inserted to preserve order.
        /// </summary>
        /// <param name="key">the key to look for</param>
        /// <param name="offset">the offset from the start of the node where the index was found</param>
        /// <returns>true if a match was found, false if no match</returns>
        bool LeafNodeSeekToKey(TKey key, out int offset, uint currentNode, int childCount)
        {
            long startAddress = currentNode * BlockSize + NodeHeader.Size;

            int min = 0;
            int max = childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                Stream.Position = startAddress + m_structureSize * mid;
                int tmpKey = CompareKeys(key, Stream);
                if (tmpKey == 0)
                {
                    offset = NodeHeader.Size + m_structureSize * mid;
                    return true;
                }
                if (tmpKey > 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            offset = NodeHeader.Size + m_structureSize * min;
            return false;
        }

        void LeafNodeSplitNode(TKey key, TValue value, uint currentNode, uint nextNode)
        {
            TKey firstKeyInGreaterNode = default(TKey);

            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(Stream, BlockSize, currentNode);

            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");

            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            uint greaterNodeIndex = AllocateNewNode();
            long sourceStartingAddress = currentNode * BlockSize + NodeHeader.Size + m_structureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * BlockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            Stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = LoadKey(Stream);

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_structureSize);

            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(Stream, BlockSize, currentNode);

            //update the second header
            newNode.Level = 0;
            newNode.ChildCount = itemsInSecondNode;
            newNode.PreviousNode = currentNode;
            newNode.NextNode = nextNode;
            newNode.Save(Stream, BlockSize, greaterNodeIndex);

            //update the node that used to be after the first one.
            if (nextNode != 0)
            {
                foreignNode.Load(Stream, BlockSize, nextNode);
                foreignNode.PreviousNode = greaterNodeIndex;
                foreignNode.Save(Stream, BlockSize, nextNode);
            }

            NodeWasSplit(0, currentNode, firstKeyInGreaterNode, greaterNodeIndex);
            if (CompareKeys(key, firstKeyInGreaterNode) > 0)
            {
                LeafNodeInsert(greaterNodeIndex, key, value);
            }
            else
            {
                LeafNodeInsert(currentNode, key, value);
            }
        }


        #endregion

        #endregion


    }
}
