using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Specialized.TimeKeyPair
{

    unsafe class LeafNodeMethods : ITreeLeafNodeMethods<KeyType>
    {
        const int KeySize = 16;

        BinaryStream m_stream;
        int m_valueSize;
        int m_blockSize;
        MethodCall m_writeValue;
        MethodCall m_readValue;
        AllocateNewNode m_allocateNewNode;
        NodeSplitRequired<KeyType> m_nodeSplit;
        int m_maximumLeafNodeChildren;
        int m_leafStructureSize;

        uint m_lastNode;
        int m_lastIndex;

        public void Initialize(BinaryStream stream, int blockSize, int valueSize, MethodCall writeValue, MethodCall readValue, AllocateNewNode allocateNewNode, NodeSplitRequired<KeyType> nodeSplit)
        {
            m_stream = stream;
            m_blockSize = blockSize;
            m_valueSize = valueSize;
            m_writeValue = writeValue;
            m_readValue = readValue;
            m_allocateNewNode = allocateNewNode;
            m_nodeSplit = nodeSplit;
            m_leafStructureSize = KeySize + valueSize;
            m_maximumLeafNodeChildren = (m_blockSize - NodeHeader.Size) / (m_leafStructureSize);
        }

        void SplitLeafNode(uint nodeToSplitIndex, long keyTime, long keyKey)
        {
            uint greaterNodeIndex;
            KeyType firstKeyInGreaterNode;

            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(m_stream, m_blockSize, nodeToSplitIndex);
            if (origionalNode.Level != 0)
                throw new Exception();
            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");
            uint nextNode = origionalNode.NextNode;

            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            greaterNodeIndex = m_allocateNewNode();
            long sourceStartingAddress = nodeToSplitIndex * m_blockSize + NodeHeader.Size + m_leafStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            m_stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode.Time = m_stream.ReadDateTime();
            firstKeyInGreaterNode.Key = m_stream.ReadInt64();

            //do the copy
            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_leafStructureSize);

            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(m_stream, m_blockSize, nodeToSplitIndex);

            //update the second header
            newNode.Level = 0;
            newNode.ChildCount = itemsInSecondNode;
            newNode.PreviousNode = nodeToSplitIndex;
            newNode.NextNode = nextNode;
            newNode.Save(m_stream, m_blockSize, greaterNodeIndex);

            //update the node that used to be after the first one.
            if (nextNode != 0)
            {
                foreignNode.Load(m_stream, m_blockSize, nextNode);
                foreignNode.PreviousNode = greaterNodeIndex;
                foreignNode.Save(m_stream, m_blockSize, nextNode);
            }

            m_nodeSplit(0, nodeToSplitIndex, firstKeyInGreaterNode, greaterNodeIndex);
            if (keyTime > firstKeyInGreaterNode.Time.Ticks || (keyTime == firstKeyInGreaterNode.Time.Ticks && keyKey >= firstKeyInGreaterNode.Key))
            {
                Insert(greaterNodeIndex, keyTime, keyKey);
            }
            else
            {
                Insert(nodeToSplitIndex, keyTime, keyKey);
            }
        }

        bool LeafNodeSeekToKey(long keyTime, long keyKey, byte* lp, out int offset)
        {
            byte level = *lp;
            short childCount = *(short*)(lp + 1);
            if (level != 0)
                throw new Exception();

            int min = 0;
            int max = childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);

                long tmpTime = *(long*)(lp + NodeHeader.Size + m_leafStructureSize * mid); ;

                if (tmpTime == keyTime)
                {
                    long tmpKey = *(long*)(lp + NodeHeader.Size + m_leafStructureSize * mid + 8);

                    if (tmpKey == keyKey)
                    {
                        //Save the search results
                        m_lastIndex = mid;
                        //m_lastMatch.Time = tmpTime;
                        //m_lastMatch.Key = tmpKey;

                        offset = NodeHeader.Size + m_leafStructureSize * mid;
                        return true;
                    }
                    if (keyKey > tmpKey)
                        min = mid + 1;
                    else
                        max = mid - 1;

                }
                else if (keyTime > tmpTime)
                {
                    min = mid + 1;
                }
                else
                {
                    max = mid - 1;
                }

                //long tmpTime = *(long*)(lp + NodeHeader.Size + m_leafStructureSize * mid); ;
                //long tmpKey = *(long*)(lp + NodeHeader.Size + m_leafStructureSize * mid + 8);
                //if (tmpTime == keyTime && tmpKey == keyKey)
                //{
                //    //Save the search results
                //    m_lastIndex = mid;
                //    //m_lastMatch.Time = tmpTime;
                //    //m_lastMatch.Key = tmpKey;

                //    offset = NodeHeader.Size + m_leafStructureSize * mid;
                //    return true;
                //}
                //if (keyTime > tmpTime || (keyTime == tmpTime && keyKey > tmpKey))
                //    min = mid + 1;
                //else
                //    max = mid - 1;
            }
            offset = NodeHeader.Size + m_leafStructureSize * min;
            return false;

        }

        bool Insert(uint nodeIndex, long keyTime, long keyKey)
        {
            m_stream.Position = nodeIndex * m_blockSize;

            byte[] buffer;
            int currentIndex;
            int validLength;
            int offset;

            if (!m_stream.GetRawDataBlock(true, out buffer, out currentIndex, out validLength))
                throw new Exception("Could not aquire block");

            if (validLength < m_blockSize)
                throw new Exception("Block size not large enough");

            fixed (byte* lp = buffer)
            {
                byte* lpp = lp + currentIndex;

                byte level = *lpp;
                short childCount = *(short*)(lpp + 1);

                if (level != 0)
                    throw new Exception("Unexpected Level");

                if (childCount >= m_maximumLeafNodeChildren)
                {
                    SplitLeafNode(nodeIndex, keyTime, keyKey);
                    return true;
                }

                //Find the best location to insert
                if (LeafNodeSeekToKey(keyTime, keyKey, lpp, out offset)) //If found
                    return false;

                int spaceToMove = NodeHeader.Size + m_leafStructureSize * childCount - offset;

                //Insert the data
                if (spaceToMove > 0)
                {
                    m_stream.Position = nodeIndex * m_blockSize + offset;
                    m_stream.InsertBytes(m_leafStructureSize, spaceToMove);
                    //InsertBytes(lpp, offset, m_leafStructureSize, spaceToMove);
                }

                *(long*)(lpp + offset) = keyTime;
                *(long*)(lpp + offset + 8) = keyKey;
                m_stream.Position = nodeIndex * m_blockSize + offset + 16;
                m_writeValue();

                //save the header
                childCount++;
                *(short*)(lpp + 1) = childCount;

                return true;
            }
        }

        public bool GetValue(uint nodeIndex, long keyTime, long keyKey)
        {
            m_stream.Position = nodeIndex * m_blockSize;

            byte[] buffer;
            int currentIndex;
            int validLength;
            int offset;

            if (!m_stream.GetRawDataBlock(false, out buffer, out currentIndex, out validLength))
                throw new Exception("Could not aquire block");

            if (validLength < m_blockSize)
                throw new Exception("Block size not large enough");

            fixed (byte* lp = buffer)
            {
                byte* lpp = lp + currentIndex;
                if (LeafNodeSeekToKey(keyTime, keyKey, lpp, out offset))
                {
                    m_stream.Position = nodeIndex * m_blockSize + offset + KeySize;
                    m_readValue();
                    return true;
                }
                return false;
            }
        }

        public bool Insert(uint nodeIndex, KeyType key)
        {
            return Insert(nodeIndex, key.Time.Ticks, key.Key);
        }

        public bool GetValue(uint nodeIndex, KeyType key)
        {
            return GetValue(nodeIndex, key.Time.Ticks, key.Key);
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
    }
}
