using System;

namespace openHistorian.Core.StorageSystem.Specialized
{

    unsafe public class LeafNodeMethods<TKey> : ITreeLeafNodeMethods<TKey>
        where TKey : struct, IValueType<TKey>
    {
        int m_keySize;

        BinaryStream m_stream;
        int m_blockSize;
        MethodCall m_writeValue;
        MethodCall m_readValue;
        AllocateNewNode m_allocateNewNode;
        NodeSplitRequired<TKey> m_nodeSplit;
        int m_maximumLeafNodeChildren;
        int m_leafStructureSize;

        uint m_currentNode;
        bool m_currentNodeSupportsWrite;
        byte[] m_buffer;
        int m_bufferCurrentIndex;
        int m_bufferValidLength;

        short m_childCount;
        uint m_nextNode;
        uint m_previousNode;

        public void Initialize(BinaryStream stream, int blockSize, int valueSize, MethodCall writeValue, MethodCall readValue, AllocateNewNode allocateNewNode, NodeSplitRequired<TKey> nodeSplit)
        {
            TKey key = default(TKey);
            m_keySize = key.SizeOf;
            m_stream = stream;
            m_blockSize = blockSize;
            m_writeValue = writeValue;
            m_readValue = readValue;
            m_allocateNewNode = allocateNewNode;
            m_nodeSplit = nodeSplit;
            m_leafStructureSize = m_keySize + valueSize;
            m_maximumLeafNodeChildren = (m_blockSize - NodeHeader.Size) / (m_leafStructureSize);
        }

        public void SetCurrentNode(uint nodeIndex, bool isForWriting)
        {
            //if the current node changed or 
            //if writing was requested but it is in read mode.
            if (true || m_currentNode != nodeIndex || (isForWriting && !m_currentNodeSupportsWrite))
            {
                m_currentNode = nodeIndex;
                m_stream.Position = nodeIndex * m_blockSize;
                if (!m_stream.GetRawDataBlock(isForWriting, out m_buffer, out m_bufferCurrentIndex, out m_bufferValidLength))
                    throw new Exception("Could not aquire block");

                if (m_bufferValidLength < m_blockSize)
                    throw new Exception("Block size not large enough");

                m_currentNodeSupportsWrite = isForWriting;

                fixed (byte* lp = m_buffer)
                {
                    if (lp[m_bufferCurrentIndex] != 0)
                        throw new Exception("The current node is not a leaf.");
                    m_childCount = *(short*)(lp + m_bufferCurrentIndex + 1);
                    m_previousNode = *(uint*)(lp + m_bufferCurrentIndex + 3);
                    m_nextNode = *(uint*)(lp + m_bufferCurrentIndex + 7);
                }
            }
        }

        void RequiresWriting()
        {
            if (!m_currentNodeSupportsWrite)
            {
                SetCurrentNode(m_currentNode, true);
            }
        }

        void SetStreamOffset(int position)
        {
            m_stream.Position = m_currentNode * m_blockSize + position;
        }

        void SplitNode(TKey key)
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

            uint greaterNodeIndex = m_allocateNewNode();
            long sourceStartingAddress = m_currentNode * m_blockSize + NodeHeader.Size + m_leafStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            m_stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode.LoadValue(m_stream);

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

            m_nodeSplit(0, currentNode, firstKeyInGreaterNode, greaterNodeIndex);
            if (key.CompareTo(firstKeyInGreaterNode) > 0)
            {
                SetCurrentNode(greaterNodeIndex, true);
                Insert(key);
            }
            else
            {
                SetCurrentNode(currentNode, true);
                Insert(key);
            }
        }

        /// <summary>
        /// Seeks to the location of the key. Or the position where the key could be inserted to preserve order.
        /// </summary>
        /// <param name="key">the key to look for</param>
        /// <param name="offset">the offset from the start of the node where the index was found</param>
        /// <returns>true if a match was found, false if no match</returns>
        bool SeekToKey(TKey key, out int offset)
        {
            long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;

            int min = 0;
            int max = m_childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                m_stream.Position = startAddress + m_leafStructureSize * mid;
                int tmpKey = key.CompareToStream(m_stream);
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
        public bool Insert(TKey key)
        {
            int offset;
            long nodePositionStart = m_currentNode * m_blockSize;

            if (m_childCount >= m_maximumLeafNodeChildren)
            {
                SplitNode(key);
                return true;
            }

            //Find the best location to insert
            if (SeekToKey(key, out offset)) //If found
                return false;

            int spaceToMove = NodeHeader.Size + m_leafStructureSize * m_childCount - offset;

            //Insert the data
            if (spaceToMove > 0)
            {
                SetStreamOffset(offset);
                m_stream.InsertBytes(m_leafStructureSize, spaceToMove);
            }

            SetStreamOffset(offset);
            key.SaveValue(m_stream);
            m_writeValue();

            //save the header
            m_childCount++;
            SetStreamOffset(1);
            m_stream.Write(m_childCount);
            return true;
        }

        public bool GetValue(TKey key)
        {
            int offset;
            if (SeekToKey(key, out offset))
            {
                SetStreamOffset(offset + m_keySize);
                m_readValue();
                return true;
            }
            return false;
        }

        public uint CreateEmptyNode()
        {
            uint nodeAddress = m_allocateNewNode();
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

        public void PrepareForTableScan(TKey firstKey, TKey lastKey)
        {
            throw new NotImplementedException();
        }

        public bool GetNextKeyTableScan(out TKey key)
        {
            throw new NotImplementedException();
        }

        public void CloseTableScan()
        {
            throw new NotImplementedException();
        }
    }
}
