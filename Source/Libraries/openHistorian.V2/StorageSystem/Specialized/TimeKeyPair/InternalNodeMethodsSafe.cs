//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.StorageSystem.Specialized.TimeKeyPair
//{

//    class InternalNodeMethods : ITreeInternalNodeMethods<KeyType>
//    {
//        const int KeySize = 16;

//        BinaryStream m_stream;
//        int m_blockSize;
//        AllocateNewNode m_allocateNewNode;
//        NodeSplitRequired<KeyType> m_nodeSplit;
//        int m_maximumInternalNodeChildren;
//        int m_internalStructureSize;
//        byte m_nodeLevel;

//        public void Initialize(BinaryStream stream, byte nodeLevel, int blockSize, AllocateNewNode allocateNewNode, NodeSplitRequired<KeyType> nodeSplit)
//        {
//            m_stream = stream;
//            m_nodeLevel = nodeLevel;
//            m_blockSize = blockSize;
//            m_allocateNewNode = allocateNewNode;
//            m_nodeSplit = nodeSplit;
//            m_internalStructureSize = KeySize + sizeof(uint);
//            m_maximumInternalNodeChildren = (m_blockSize - NodeHeader.Size) / (m_internalStructureSize);
//        }

//        /// <summary>
//        /// Splits an existing node into two halfs
//        /// </summary>
//        /// <param name="nodeToSplitIndex">the node index value to split</param>
//        /// <param name="nodeLevel">the level of the node</param>
//        void SplitNode(uint nodeToSplitIndex, byte nodeLevel, KeyType key, uint childNodeIndex)
//        {
//            uint greaterNodeIndex;
//            KeyType firstKeyInGreaterNode;

//            NodeHeader origionalNode = default(NodeHeader);
//            NodeHeader newNode = default(NodeHeader);
//            NodeHeader foreignNode = default(NodeHeader);

//            origionalNode.Load(m_stream, m_blockSize, nodeToSplitIndex);
//            if (origionalNode.Level != nodeLevel)
//                throw new Exception();
//            if (origionalNode.ChildCount < 2)
//                throw new Exception("cannot split a node with fewer than 2 children");
//            uint nextNode = origionalNode.NextNode;

//            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
//            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

//            greaterNodeIndex = m_allocateNewNode();
//            long sourceStartingAddress = nodeToSplitIndex * m_blockSize + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * itemsInFirstNode;
//            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size + sizeof(uint);

//            //lookup the first key that will be copied
//            m_stream.Position = sourceStartingAddress;
//            firstKeyInGreaterNode.Time = m_stream.ReadDateTime();
//            firstKeyInGreaterNode.Key = m_stream.ReadInt64();

//            //do the copy
//            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_internalStructureSize);
//            //Set the lookback position as invalid since this node should never be parsed for data before the first key.
//            m_stream.Position = targetStartingAddress - sizeof(uint);
//            m_stream.Write(0u);


//            //update the first header
//            origionalNode.ChildCount = itemsInFirstNode;
//            origionalNode.NextNode = greaterNodeIndex;
//            origionalNode.Save(m_stream, m_blockSize, nodeToSplitIndex);

//            //update the second header
//            newNode.Level = origionalNode.Level;
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
//            m_nodeSplit(nodeLevel, nodeToSplitIndex, firstKeyInGreaterNode, greaterNodeIndex);
//            if (key.Time > firstKeyInGreaterNode.Time || (key.Time >= firstKeyInGreaterNode.Time))
//            {
//                Insert(greaterNodeIndex, nodeLevel, key, childNodeIndex);
//            }
//            else
//            {
//                Insert(nodeToSplitIndex, nodeLevel, key, childNodeIndex);
//            }
//        }

//        /// <summary>
//        /// Starting from the first byte of the node, 
//        /// this method will seek to the most appropriate location for 
//        /// the key to be inserted and insert the data if the leaf is not full. 
//        /// </summary>
//        /// <param name="key">the key to insert</param>
//        /// <param name="childNodeIndex">the child node that corresponds to this key</param>
//        /// <param name="nodeIndex">the index of the node to be modified</param>
//        /// <param name="nodeLevel">the level of the node</param>
//        /// <returns>The results of the insert</returns>
//        public void Insert(uint nodeIndex, byte nodeLevel, KeyType key, uint childNodeIndex)
//        {

//            long nodePositionStart = nodeIndex * m_blockSize;
//            m_stream.Position = nodePositionStart;

//            byte level = m_stream.ReadByte();
//            short childCount = m_stream.ReadInt16();

//            if (level != nodeLevel)
//                throw new Exception("Corrupt Node Level");

//            if (childCount >= m_maximumInternalNodeChildren)
//            {
//                SplitNode(nodeIndex, nodeLevel, key, childNodeIndex);
//                return;
//            }

//            m_stream.Position = nodePositionStart;

//            if (InternalNodeSeekToKey(key, nodeIndex, level))
//                throw new Exception("Duplicate Key");

//            int spaceToMove = NodeHeader.Size + sizeof(uint) + m_internalStructureSize * childCount - (int)(m_stream.Position - nodePositionStart);

//            if (spaceToMove > 0)
//                m_stream.InsertBytes(m_internalStructureSize, spaceToMove);

//            m_stream.Write(key.Time);
//            m_stream.Write(key.Key);
//            m_stream.Write(childNodeIndex);

//            childCount++;
//            m_stream.Position = nodePositionStart + 1;
//            m_stream.Write(childCount);

//        }


//        /// <summary>
//        /// Starting from the end of the internal node header, 
//        /// this method will return the node index value that contains the provided key
//        /// </summary>
//        /// <param name="key">the key to search for</param>
//        /// <param name="nodeIndex">the index of the node to be modified</param>
//        /// <param name="nodeLevel">the level of the node</param>
//        /// <returns></returns>
//        public uint GetNodeIndex(uint nodeIndex, byte nodeLevel, KeyType key)
//        {
//            if (InternalNodeSeekToKey(key, nodeIndex, nodeLevel))
//            {
//                m_stream.Position += KeySize;
//                return m_stream.ReadUInt32();
//            }
//            m_stream.Position -= 4;
//            return m_stream.ReadUInt32();
//        }


//        /// <summary>
//        /// Starting from the first byte of the node, 
//        /// this will seek the current node for the best match of the key provided.
//        /// </summary>
//        /// <param name="key">the key to search for</param>
//        /// <param name="nodeIndex">the index of the node</param>
//        /// <param name="nodeLevel">the level of the node</param>
//        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
//        bool InternalNodeSeekToKey(KeyType key, uint nodeIndex, byte nodeLevel)
//        {
//            m_stream.Position = nodeIndex * m_blockSize;

//            long startAddress = m_stream.Position + NodeHeader.Size + sizeof(uint);
//            byte level = m_stream.ReadByte();
//            short childCount = m_stream.ReadInt16();

//            if (nodeLevel != level)
//                throw new Exception("Corrupt BPlusTree: Unexpected Node Level");

//            int min = 0;
//            int max = childCount - 1;

//            while (min <= max)
//            {
//                int mid = min + (max - min >> 1);
//                m_stream.Position = startAddress + m_internalStructureSize * mid;
//                DateTime tmpTime = m_stream.ReadDateTime();
//                long tmpKey = m_stream.ReadInt64();

//                if (tmpTime == key.Time && tmpKey == key.Key)
//                {
//                    m_stream.Position = startAddress + m_internalStructureSize * mid;
//                    return true;
//                }
//                if (key.Time > tmpTime || (key.Time == tmpTime && key.Key > tmpKey))
//                    min = mid + 1;
//                else
//                    max = mid - 1;
//            }

//            m_stream.Position = startAddress + m_internalStructureSize * min;

//            return false;
//        }

//        /// <summary>
//        /// Starting from the first byte of the node, 
//        /// this will seek the current node for the best match of the key provided.
//        /// </summary>
//        /// <param name="key">the key to search for</param>
//        /// <param name="nodeIndex">the index of the node</param>
//        /// <param name="nodeLevel">the level of the node</param>
//        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
//        unsafe bool InternalNodeSeekToKey(KeyType key, uint nodeIndex, byte nodeLevel, bool IsUnsafe)
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

//                if (nodeLevel != level)
//                    throw new Exception("Corrupt BPlusTree: Unexpected Node Level");

//                int min = 0;
//                int max = childCount - 1;

//                while (min <= max)
//                {
//                    int mid = min + (max - min >> 1);

//                    DateTime tmpTime = *(DateTime*)(lp + currentIndex + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * mid); ;
//                    long tmpKey = *(long*)(lp + currentIndex + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * mid + 8);

//                    if (tmpTime == key.Time && tmpKey == key.Key)
//                    {
//                        m_stream.Position = m_stream.Position + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * mid;
//                        return true;
//                    }
//                    if (key.Time > tmpTime || (key.Time == tmpTime && key.Key > tmpKey))
//                        min = mid + 1;
//                    else
//                        max = mid - 1;
//                }

//                m_stream.Position = m_stream.Position + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * min;

//                return false;
//            }
//        }

//        ///<summary>
//        ///Allocates a new empty tree node.
//        ///</summary>
//        ///<param name="level">the level of the internal node</param>
//        ///<param name="childNodeBefore">the child value before</param>
//        ///<param name="key">the key that seperates the children</param>
//        ///<param name="childNodeAfter">the child after or equal to the key</param>
//        ///<returns>the index value of this new node.</returns>
//        public uint CreateEmptyNode(byte level, uint childNodeBefore, KeyType key, uint childNodeAfter)
//        {
//            uint nodeAddress = m_allocateNewNode();
//            m_stream.Position = nodeAddress * m_blockSize;

//            //Clearing the Node
//            //Level = level;
//            //ChildCount = 1;
//            //NextNode = 0;
//            //PreviousNode = 0;
//            m_stream.Write(level);
//            m_stream.Write((short)1);
//            m_stream.Write(0L);
//            m_stream.Write(childNodeBefore);
//            m_stream.Write(key.Time);
//            m_stream.Write(key.Key);
//            m_stream.Write(childNodeAfter);

//            return nodeAddress;
//        }

//    }
//}
