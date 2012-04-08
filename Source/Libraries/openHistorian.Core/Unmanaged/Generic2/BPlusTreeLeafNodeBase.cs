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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Generic2
{
    public abstract class BPlusTreeLeafNodeBase<TKey,TValue> : BPlusTreeInternalNodeBase<TKey,TValue>
    {

        protected abstract void SaveValue(TValue value, BinaryStream stream);
        protected abstract TValue LoadValue(BinaryStream stream);
        protected abstract int SizeOfValue();

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
        
        int m_keySize;

        int m_maximumLeafNodeChildren;
        protected int m_leafStructureSize;

        protected uint m_currentNode;
        protected short m_childCount;
        uint m_nextNode;
        uint m_previousNode;

        bool m_scanningTable;
        TKey m_startKey;
        TKey m_stopKey;
        int m_oldIndex;

        protected BPlusTreeLeafNodeBase(BinaryStream stream) : base(stream)
        {
        }

        protected BPlusTreeLeafNodeBase(BinaryStream stream, int blockSize) : base(stream, blockSize)
        {
        }

        protected override bool LeafNodeInsert(uint nodeIndex, TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        protected override bool LeafNodeGetValue(uint nodeIndex, TKey key, out TValue value)
        {
            throw new NotImplementedException();
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




        public void LeafNodeInitialize()
        {
            m_keySize = SizeOfKey();
            m_leafStructureSize = m_keySize + SizeOfValue();
            m_maximumLeafNodeChildren = (BlockSize - NodeHeader.Size) / (m_leafStructureSize);
        }

        public void LeafNodeSetCurrentNode(uint nodeIndex, bool isForWriting)
        {
            bool changed = (m_currentNode != nodeIndex);
            m_currentNode = nodeIndex;
            Stream.Position = nodeIndex * BlockSize;
            Stream.UpdateLocalBuffer(isForWriting);

            if (changed)
            {
                if (Stream.ReadByte() != 0)
                    throw new Exception("The current node is not a leaf.");
                m_childCount = Stream.ReadInt16();
                m_previousNode = Stream.ReadUInt32();
                m_nextNode = Stream.ReadUInt32();
            }
        }

        void LeafNodeSetStreamOffset(int position)
        {
            Stream.Position = m_currentNode * BlockSize + position;
        }

        void LeafNodeSplitNode(TKey key, TValue value)
        {
            uint currentNode = m_currentNode;
            uint oldNextNode = m_nextNode;
            TKey firstKeyInGreaterNode = default(TKey);

            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(Stream, BlockSize, m_currentNode);

            if (m_childCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");

            short itemsInFirstNode = (short)(m_childCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(m_childCount - itemsInFirstNode);

            uint greaterNodeIndex = AllocateNewNode();
            long sourceStartingAddress = m_currentNode * BlockSize + NodeHeader.Size + m_leafStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * BlockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            Stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = LoadKey(Stream);

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_leafStructureSize);

            //update the first header
            m_childCount = itemsInFirstNode;
            m_nextNode = greaterNodeIndex;

            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(Stream, BlockSize, currentNode);

            //update the second header
            newNode.Level = 0;
            newNode.ChildCount = itemsInSecondNode;
            newNode.PreviousNode = currentNode;
            newNode.NextNode = oldNextNode;
            newNode.Save(Stream, BlockSize, greaterNodeIndex);

            //update the node that used to be after the first one.
            if (oldNextNode != 0)
            {
                foreignNode.Load(Stream, BlockSize, oldNextNode);
                foreignNode.PreviousNode = greaterNodeIndex;
                foreignNode.Save(Stream, BlockSize, oldNextNode);
            }

            NodeWasSplit(0, currentNode, firstKeyInGreaterNode, greaterNodeIndex);
            if (CompareKeys(key, firstKeyInGreaterNode) > 0)
            {
                LeafNodeSetCurrentNode(greaterNodeIndex, true);
                LeafNodeInsert(key, value);
            }
            else
            {
                LeafNodeSetCurrentNode(currentNode, true);
                LeafNodeInsert(key, value);
            }
        }

        /// <summary>
        /// Seeks to the location of the key. Or the position where the key could be inserted to preserve order.
        /// </summary>
        /// <param name="key">the key to look for</param>
        /// <param name="offset">the offset from the start of the node where the index was found</param>
        /// <returns>true if a match was found, false if no match</returns>
        protected virtual bool LeafNodeSeekToKey(TKey key, out int offset)
        {
            long startAddress = m_currentNode * BlockSize + NodeHeader.Size;

            int min = 0;
            int max = m_childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                Stream.Position = startAddress + m_leafStructureSize * mid;
                int tmpKey = CompareKeys(key, Stream);
                if (tmpKey == 0)
                {
                    offset = NodeHeader.Size + m_leafStructureSize * mid;
                    return true;
                }
                if (tmpKey > 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            offset = NodeHeader.Size + m_leafStructureSize * min;
            return false;
        }

        /// <summary>
        /// Inserts the following key into the current node. Splits the node if required.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if sucessfully inserted, false if a duplicate key was detected.</returns>
        public bool LeafNodeInsert(TKey key, TValue value)
        {
            int offset;
            long nodePositionStart = m_currentNode * BlockSize;

            if (m_childCount >= m_maximumLeafNodeChildren)
            {
                LeafNodeSplitNode(key, value);
                return true;
            }

            //Find the best location to insert
            if (LeafNodeSeekToKey(key, out offset)) //If found
                return false;

            int spaceToMove = NodeHeader.Size + m_leafStructureSize * m_childCount - offset;

            //Insert the data
            if (spaceToMove > 0)
            {
                LeafNodeSetStreamOffset(offset);
                Stream.InsertBytes(m_leafStructureSize, spaceToMove);
            }

            LeafNodeSetStreamOffset(offset);
            SaveKey(key, Stream);
            SaveValue(value, Stream);

            //save the header
            m_childCount++;
            LeafNodeSetStreamOffset(1);
            Stream.Write(m_childCount);
            return true;
        }

        public bool LeafNodeGetValue(TKey key, out TValue value)
        {
            int offset;
            if (LeafNodeSeekToKey(key, out offset))
            {
                LeafNodeSetStreamOffset(offset + m_keySize);
                value = LoadValue(Stream);
                return true;
            }
            value = default(TValue);
            return false;
        }

        public void LeafNodePrepareForTableScan(TKey firstKey, TKey lastKey)
        {
            m_scanningTable = true;
            m_startKey = firstKey;
            m_stopKey = lastKey;
            LeafNodeSeekToKey(firstKey, out m_oldIndex);
            m_oldIndex = (m_oldIndex - NodeHeader.Size) / m_leafStructureSize;
        }

        public bool LeafNodeGetNextKeyTableScan(out TKey key)
        {
            if (m_oldIndex >= m_childCount)
            {
                if (m_nextNode == 0)
                {
                    key = default(TKey);
                    return false;
                }
                LeafNodeSetCurrentNode(m_nextNode, false);
                m_oldIndex = 0;
            }
            Stream.Position = m_currentNode * BlockSize + m_oldIndex * m_leafStructureSize + NodeHeader.Size;
            key = default(TKey);
            key = LoadKey(Stream);

            if (CompareKeys(m_stopKey, key) <= 0)
                return false;
            m_oldIndex++;
            return true;
        }

        public void LeafNodeCloseTableScan()
        {
            m_scanningTable = false;
        }


    }
}
