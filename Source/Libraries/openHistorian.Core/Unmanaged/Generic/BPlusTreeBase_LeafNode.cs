using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Generic
{
    abstract partial class BPlusTreeBase<TKey, TValue>
    {
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

        #region [Abstract Methods]
        public int LeafNodeSizeOfKey()
        {
            return SizeOfKey();
        }
        public void LeafNodeSaveKeyValue(TKey value, BinaryStream stream)
        {
            SaveKey(value, stream);
        }
        public TKey LeafNodeLoadKeyValue(BinaryStream stream)
        {
            return LoadKey(stream);
        }
        public int LeafNodeCompareKeys(TKey first, TKey last)
        {
            return CompareKeys(first, last);
        }
        public int LeafNodeCompareKeys(TKey first, BinaryStream stream)
        {
            return CompareKeys(first, stream);
        }
        #endregion

        public void LeafNodeInitialize()
        {
            m_keySize = LeafNodeSizeOfKey();
            m_leafStructureSize = m_keySize + SizeOfValue();
            m_maximumLeafNodeChildren = (m_blockSize - NodeHeader.Size) / (m_leafStructureSize);
        }

        public void LeafNodeSetCurrentNode(uint nodeIndex, bool isForWriting)
        {
            m_currentNode = nodeIndex;
            m_stream.Position = nodeIndex * m_blockSize;
            m_stream.UpdateLocalBuffer(isForWriting);

            if (m_stream.ReadByte() != 0)
                throw new Exception("The current node is not a leaf.");
            m_childCount = m_stream.ReadInt16();
            m_previousNode = m_stream.ReadUInt32();
            m_nextNode = m_stream.ReadUInt32();
        }

        void LeafNodeSetStreamOffset(int position)
        {
            m_stream.Position = m_currentNode * m_blockSize + position;
        }

        void LeafNodeSplitNode(TKey key, TValue value)
        {
            uint currentNode = m_currentNode;
            uint oldNextNode = m_nextNode;
            TKey firstKeyInGreaterNode = default(TKey);

            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(m_stream, m_blockSize, m_currentNode);

            if (m_childCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");

            short itemsInFirstNode = (short)(m_childCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(m_childCount - itemsInFirstNode);

            uint greaterNodeIndex = AllocateNewNode();
            long sourceStartingAddress = m_currentNode * m_blockSize + NodeHeader.Size + m_leafStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            m_stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = LeafNodeLoadKeyValue(m_stream);

            //do the copy
            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_leafStructureSize);

            //update the first header
            m_childCount = itemsInFirstNode;
            m_nextNode = greaterNodeIndex;

            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(m_stream, m_blockSize, currentNode);

            //update the second header
            newNode.Level = 0;
            newNode.ChildCount = itemsInSecondNode;
            newNode.PreviousNode = currentNode;
            newNode.NextNode = oldNextNode;
            newNode.Save(m_stream, m_blockSize, greaterNodeIndex);

            //update the node that used to be after the first one.
            if (oldNextNode != 0)
            {
                foreignNode.Load(m_stream, m_blockSize, oldNextNode);
                foreignNode.PreviousNode = greaterNodeIndex;
                foreignNode.Save(m_stream, m_blockSize, oldNextNode);
            }

            NodeWasSplit(0, currentNode, firstKeyInGreaterNode, greaterNodeIndex);
            if (LeafNodeCompareKeys(key, firstKeyInGreaterNode) > 0)
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
            long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;

            int min = 0;
            int max = m_childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                m_stream.Position = startAddress + m_leafStructureSize * mid;
                int tmpKey = LeafNodeCompareKeys(key, m_stream);
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
            long nodePositionStart = m_currentNode * m_blockSize;

            if (m_childCount >= m_maximumLeafNodeChildren)
            {
                LeafNodeSplitNode(key,value);
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
                m_stream.InsertBytes(m_leafStructureSize, spaceToMove);
            }

            LeafNodeSetStreamOffset(offset);
            LeafNodeSaveKeyValue(key, m_stream);
            SaveValue(value, m_stream);

            //save the header
            m_childCount++;
            LeafNodeSetStreamOffset(1);
            m_stream.Write(m_childCount);
            return true;
        }

        public bool LeafNodeGetValue(TKey key, out TValue value)
        {
            int offset;
            if (LeafNodeSeekToKey(key, out offset))
            {
                LeafNodeSetStreamOffset(offset + m_keySize);
                value = LoadValue(m_stream);
                return true;
            }
            value = default(TValue);
            return false;
        }

        public uint LeafNodeCreateEmptyNode()
        {
            uint nodeAddress = AllocateNewNode();
            m_stream.Position = m_blockSize * nodeAddress;

            //Clearing the Node
            //Level = 0;
            //ChildCount = 0;
            //NextNode = 0;
            //PreviousNode = 0;
            m_stream.Write(0L);
            m_stream.Write(0);

            return nodeAddress;
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
            m_stream.Position = m_currentNode * m_blockSize + m_oldIndex * m_leafStructureSize + NodeHeader.Size;
            key = default(TKey);
            key = LeafNodeLoadKeyValue(m_stream);

            if (LeafNodeCompareKeys(m_stopKey, key) <= 0)
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
