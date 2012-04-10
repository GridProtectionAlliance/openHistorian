//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.StorageSystem.Specialized.TimeKeyPair
//{

//    class LeafNodeMethods : ITreeLeafNodeMethods<KeyType>
//    {
//        const int KeySize = 16;

//        BinaryStream m_stream;
//        int m_valueSize;
//        int m_blockSize;
//        MethodCall m_writeValue;
//        MethodCall m_readValue;
//        AllocateNewNode m_allocateNewNode;
//        NodeSplitRequired<KeyType> m_nodeSplit;
//        int m_maximumLeafNodeChildren;
//        int m_leafStructureSize;

//        uint m_lastNode;
//        KeyType m_lastMatch;
//        int m_lastIndex;


//        public void Initialize(BinaryStream stream, int blockSize, int valueSize, MethodCall writeValue, MethodCall readValue, AllocateNewNode allocateNewNode, NodeSplitRequired<KeyType> nodeSplit)
//        {
//            m_stream = stream;
//            m_blockSize = blockSize;
//            m_valueSize = valueSize;
//            m_writeValue = writeValue;
//            m_readValue = readValue;
//            m_allocateNewNode = allocateNewNode;
//            m_nodeSplit = nodeSplit;
//            m_leafStructureSize = KeySize + valueSize;
//            m_maximumLeafNodeChildren = (m_blockSize - NodeHeader.Size) / (m_leafStructureSize);
//        }

//        void SplitLeafNode(uint nodeToSplitIndex, KeyType key)
//        {
//            uint greaterNodeIndex;
//            KeyType firstKeyInGreaterNode;

//            NodeHeader origionalNode = default(NodeHeader);
//            NodeHeader newNode = default(NodeHeader);
//            NodeHeader foreignNode = default(NodeHeader);

//            origionalNode.Load(m_stream, m_blockSize, nodeToSplitIndex);
//            if (origionalNode.Level != 0)
//                throw new Exception();
//            if (origionalNode.ChildCount < 2)
//                throw new Exception("cannot split a node with fewer than 2 children");
//            uint nextNode = origionalNode.NextNode;

//            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
//            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

//            greaterNodeIndex = m_allocateNewNode();
//            long sourceStartingAddress = nodeToSplitIndex * m_blockSize + NodeHeader.Size + m_leafStructureSize * itemsInFirstNode;
//            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size;

//            //lookup the first key that will be copied
//            m_stream.Position = sourceStartingAddress;
//            firstKeyInGreaterNode.Time = m_stream.ReadDateTime();
//            firstKeyInGreaterNode.Key = m_stream.ReadInt64();

//            //do the copy
//            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_leafStructureSize);

//            //update the first header
//            origionalNode.ChildCount = itemsInFirstNode;
//            origionalNode.NextNode = greaterNodeIndex;
//            origionalNode.Save(m_stream, m_blockSize, nodeToSplitIndex);

//            //update the second header
//            newNode.Level = 0;
//            newNode.ChildCount = itemsInSecondNode;
//            newNode.PreviousNode = nodeToSplitIndex;
//            newNode.NextNode = nextNode;
//            newNode.Save(m_stream, m_blockSize, greaterNodeIndex);

//            //update the node that used to be after the first one.
//            if (nextNode != 0)
//            {
//                foreignNode.Load(m_stream, m_blockSize, nextNode);
//                foreignNode.PreviousNode = greaterNodeIndex;
//                foreignNode.Save(m_stream, m_blockSize, nextNode);
//            }

//            m_nodeSplit(0, nodeToSplitIndex, firstKeyInGreaterNode, greaterNodeIndex);
//            if (key.Time > firstKeyInGreaterNode.Time || (key.Time >= firstKeyInGreaterNode.Time))
//            {
//                Insert(greaterNodeIndex, key);
//            }
//            else
//            {
//                Insert(nodeToSplitIndex, key);
//            }
//        }


//        public bool Insert(uint nodeIndex, KeyType key)
//        {
//            long nodePositionStart = nodeIndex * m_blockSize;
//            m_stream.Position = nodePositionStart;

//            byte level = m_stream.ReadByte();
//            short childCount = m_stream.ReadInt16();

//            if (level != 0)
//                throw new Exception("Unexpected Level");

//            if (childCount >= m_maximumLeafNodeChildren)
//            {
//                SplitLeafNode(nodeIndex, key);
//                return true;
//            }

//            m_stream.Position = nodePositionStart;

//            //Find the best location to insert
//            if (LeafNodeSeekToKey(key, nodeIndex)) //If found
//                return false;

//            int spaceToMove = NodeHeader.Size + m_leafStructureSize * childCount - (int)(m_stream.Position - nodePositionStart);

//            //Insert the data
//            if (spaceToMove > 0)
//                m_stream.InsertBytes(m_leafStructureSize, spaceToMove);
//            m_stream.Write(key.Time);
//            m_stream.Write(key.Key);
//            m_writeValue();

//            //save the header
//            m_stream.Position = nodePositionStart + 1;
//            childCount++;
//            m_stream.Write(childCount);

//            return true;
//        }

//        unsafe bool LeafNodeSeekToKey(KeyType key, uint nodeIndex, bool IsUnsafe)
//        {
//            m_stream.Position = nodeIndex * m_blockSize;

//            byte[] buffer;
//            int currentIndex;
//            int validLength;

//            if (!m_stream.GetRawDataBlock(false, out buffer, out currentIndex, out validLength))
//                throw new Exception("Could not aquire block");

//            if (validLength < m_blockSize)
//                throw new Exception("Block size not large enough");

//            fixed (byte* lp = buffer)
//            {
//                byte level = *(lp + currentIndex);
//                short childCount = *(short*)(lp + currentIndex + 1);
//                if (level != 0)
//                    throw new Exception();

//                int min = 0;
//                int max = childCount - 1;

//                while (min <= max)
//                {
//                    int mid = min + (max - min >> 1);
//                    DateTime tmpTime = *(DateTime*)(lp + currentIndex + NodeHeader.Size + m_leafStructureSize * mid); ;
//                    long tmpKey = *(long*)(lp + currentIndex + NodeHeader.Size + m_leafStructureSize * mid + 8);
//                    if (tmpTime == key.Time && tmpKey == key.Key)
//                    {
//                        //Save the search results
//                        m_lastIndex = mid;
//                        m_lastMatch.Time = tmpTime;
//                        m_lastMatch.Key = tmpKey;
//                        m_lastNode = nodeIndex;

//                        m_stream.Position = m_stream.Position + NodeHeader.Size + m_leafStructureSize * mid;
//                        return true;
//                    }
//                    if (key.Time > tmpTime || (key.Time == tmpTime && key.Key > tmpKey))
//                        min = mid + 1;
//                    else
//                        max = mid - 1;
//                }
//                m_stream.Position = m_stream.Position + NodeHeader.Size + m_leafStructureSize * min;
//                return false;
//            }


//        }

//        bool LeafNodeSeekToKey(KeyType key, uint nodeIndex)
//        {
//            m_stream.Position = nodeIndex * m_blockSize;

//            long startAddress = m_stream.Position + NodeHeader.Size;
//            byte level = m_stream.ReadByte();
//            short childCount = m_stream.ReadInt16();
//            if (level != 0)
//                throw new Exception();

//            int min = 0;
//            int max = childCount - 1;

//            while (min <= max)
//            {
//                int mid = min + (max - min >> 1);
//                m_stream.Position = startAddress + m_leafStructureSize * mid;

//                DateTime tmpTime = m_stream.ReadDateTime();
//                long tmpKey = m_stream.ReadInt64();
//                if (tmpTime == key.Time && tmpKey == key.Key)
//                {
//                    //Save the search results
//                    m_lastIndex = mid;
//                    m_lastMatch.Time = tmpTime;
//                    m_lastMatch.Key = tmpKey;
//                    m_lastNode = nodeIndex;

//                    m_stream.Position -= KeySize;
//                    return true;
//                }
//                if (key.Time > tmpTime || (key.Time == tmpTime && key.Key > tmpKey))
//                    min = mid + 1;
//                else
//                    max = mid - 1;
//            }
//            m_stream.Position = startAddress + m_leafStructureSize * min;
//            return false;
//        }


//        public bool GetValue(uint nodeIndex, KeyType key)
//        {
//            if (LeafNodeSeekToKey(key, nodeIndex))
//            {
//                m_stream.Position += KeySize;
//                m_readValue();
//                return true;
//            }
//            return false;
//        }

//        public uint CreateEmptyNode()
//        {
//            uint nodeAddress = m_allocateNewNode();
//            m_stream.Position = m_blockSize * nodeAddress;

//            //Clearing the Node
//            //Level = 0;
//            //ChildCount = 0;
//            //NextNode = 0;
//            //PreviousNode = 0;
//            m_stream.Write(0L);
//            m_stream.Write(0);

//            return nodeAddress;
//        }
//    }
//}
