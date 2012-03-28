//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.Unmanaged.Generic
//{

//    unsafe public class InternalNodeMethods<TKey>
//    {
//        int m_keySize;

//        BinaryStream m_stream;
//        int m_blockSize;

//        AllocateNewNode m_allocateNewNode;
//        NodeSplitRequired<TKey> m_nodeSplit;

//        int m_maximumInternalNodeChildren;
//        int m_internalStructureSize;

//        uint m_currentNode;

//        short m_childCount;
//        uint m_nextNode;
//        uint m_previousNode;

//        #region [Abstract Methods]
//        public int InternalNodeSizeOfKey()
//        {
//            return 1;
//        }
//        public void InternalNodeSaveKeyValue(TKey value, BinaryStream stream)
//        {

//        }
//        public TKey InternalNodeLoadKeyValue(BinaryStream stream)
//        {
//            return default(TKey);
//        }
//        public int InternalNodeCompareKeys(TKey first, TKey last)
//        {
//            return 0;
//        }
//        public int InternalNodeCompareKeys(TKey first, BinaryStream stream)
//        {
//            return 0;
//        }
//        #endregion

//        public void InternalNodeInitialize(BinaryStream stream, byte nodeLevel, int blockSize, AllocateNewNode allocateNewNode, NodeSplitRequired<TKey> nodeSplit)
//        {
//            TKey key = default(TKey);
//            m_keySize = InternalNodeSizeOfKey();
//            m_stream = stream;
//            m_blockSize = blockSize;
//            m_allocateNewNode = allocateNewNode;
//            m_nodeSplit = nodeSplit;
//            m_internalStructureSize = m_keySize + sizeof(uint);
//            m_maximumInternalNodeChildren = (m_blockSize - NodeHeader.Size) / (m_internalStructureSize);
//        }


//        void InternalNodeSetCurrentNode(uint nodeIndex, bool isForWriting)
//        {
//            m_currentNode = nodeIndex;
//            m_stream.Position = nodeIndex * m_blockSize;

//            m_stream.ReadByte(); //node level
//            m_childCount = m_stream.ReadInt16();
//            m_previousNode = m_stream.ReadUInt32();
//            m_nextNode = m_stream.ReadUInt32();
//        }

//        public void InternalNodeSetCurrentNode(byte nodeLevel, uint nodeIndex, bool isForWriting)
//        {
//            m_currentNode = nodeIndex;
//            m_stream.Position = nodeIndex * m_blockSize;


//            if (m_stream.ReadByte() != nodeLevel)
//                throw new Exception("The current node is not a leaf.");
//            m_childCount = m_stream.ReadInt16();
//            m_previousNode = m_stream.ReadUInt32();
//            m_nextNode = m_stream.ReadUInt32();
//        }

//        void InternalNodeSetStreamOffset(int position)
//        {
//            m_stream.Position = m_currentNode * m_blockSize + position;
//        }

//        /// <summary>
//        /// Splits an existing node into two halfs
//        /// </summary>
//        void InternalNodeSplitNode(TKey key, uint childNodeIndex)
//        {
//            uint currentNode = m_currentNode;
//            uint oldNextNode = m_nextNode;
//            TKey firstKeyInGreaterNode = default(TKey);

//            NodeHeader origionalNode = default(NodeHeader);
//            NodeHeader newNode = default(NodeHeader);
//            NodeHeader foreignNode = default(NodeHeader);

//            origionalNode.Load(m_stream, m_blockSize, m_currentNode);

//            if (origionalNode.ChildCount < 2)
//                throw new Exception("cannot split a node with fewer than 2 children");

//            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
//            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

//            uint greaterNodeIndex = m_allocateNewNode();
//            long sourceStartingAddress = m_currentNode * m_blockSize + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * itemsInFirstNode;
//            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size + sizeof(uint);

//            //lookup the first key that will be copied
//            m_stream.Position = sourceStartingAddress;
//            firstKeyInGreaterNode = InternalNodeLoadKeyValue(m_stream);

//            //do the copy
//            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_internalStructureSize);
//            //Set the lookback position as invalid since this node should never be parsed for data before the first key.
//            m_stream.Position = targetStartingAddress - sizeof(uint);
//            m_stream.Write(0u);


//            //update the first header
//            origionalNode.ChildCount = itemsInFirstNode;
//            origionalNode.NextNode = greaterNodeIndex;
//            origionalNode.Save(m_stream, m_blockSize, currentNode);

//            //update the second header
//            newNode.Level = origionalNode.Level;
//            newNode.ChildCount = itemsInSecondNode;
//            newNode.PreviousNode = currentNode;
//            newNode.NextNode = oldNextNode;
//            newNode.Save(m_stream, m_blockSize, greaterNodeIndex);

//            //update the node that used to be after the first one.
//            if (oldNextNode != 0)
//            {
//                foreignNode.Load(m_stream, m_blockSize, oldNextNode);
//                foreignNode.PreviousNode = greaterNodeIndex;
//                foreignNode.Save(m_stream, m_blockSize, oldNextNode);
//            }
//            m_nodeSplit(origionalNode.Level, currentNode, firstKeyInGreaterNode, greaterNodeIndex);
//            if (InternalNodeCompareKeys(key, firstKeyInGreaterNode) > 0)
//            {
//                InternalNodeSetCurrentNode(greaterNodeIndex, true);
//                InternalNodeInsert(key, childNodeIndex);
//            }
//            else
//            {
//                InternalNodeSetCurrentNode(currentNode, true);
//                InternalNodeInsert(key, childNodeIndex);
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
//        public void InternalNodeInsert(TKey key, uint childNodeIndex)
//        {
//            int offset;

//            long nodePositionStart = m_currentNode * m_blockSize;

//            if (m_childCount >= m_maximumInternalNodeChildren)
//            {
//                InternalNodeSplitNode(key, childNodeIndex);
//                return;
//            }

//            if (InternalNodeSeekToKey(key, out offset))
//                throw new Exception("Duplicate Key");

//            int spaceToMove = NodeHeader.Size + sizeof(uint) + m_internalStructureSize * m_childCount - offset;

//            if (spaceToMove > 0)
//            {
//                InternalNodeSetStreamOffset(offset);
//                m_stream.InsertBytes(m_internalStructureSize, spaceToMove);
//            }

//            InternalNodeSetStreamOffset(offset);
//            InternalNodeSaveKeyValue(key, m_stream);
//            m_stream.Write(childNodeIndex);

//            m_childCount++;
//            InternalNodeSetStreamOffset(1);
//            m_stream.Write(m_childCount);

//        }

//        /// <summary>
//        /// Starting from the end of the internal node header, 
//        /// this method will return the node index value that contains the provided key
//        /// </summary>
//        /// <param name="key">the key to search for</param>
//        /// <returns></returns>
//        public uint InternalNodeGetNodeIndex(TKey key)
//        {
//            int offset;
//            if (InternalNodeSeekToKey(key, out offset))
//            {
//                InternalNodeSetStreamOffset(offset + m_keySize);
//                return m_stream.ReadUInt32();
//            }
//            InternalNodeSetStreamOffset(offset - 4);
//            return m_stream.ReadUInt32();
//        }

//        /// <summary>
//        /// Starting from the first byte of the node, 
//        /// this will seek the current node for the best match of the key provided.
//        /// </summary>
//        /// <param name="key">the key to search for</param>
//        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
//        bool InternalNodeSeekToKey(TKey key, out int offset)
//        {
//            long startAddress = m_currentNode * m_blockSize + NodeHeader.Size + sizeof(uint);

//            int min = 0;
//            int max = m_childCount - 1;

//            while (min <= max)
//            {
//                int mid = min + (max - min >> 1);
//                m_stream.Position = startAddress + m_internalStructureSize * mid;

//                int tmpKey = InternalNodeCompareKeys(key, m_stream); ;
//                if (tmpKey == 0)
//                {
//                    offset = NodeHeader.Size + sizeof(uint) + m_internalStructureSize * mid;
//                    return true;
//                }
//                if (tmpKey > 0)
//                    min = mid + 1;
//                else
//                    max = mid - 1;
//            }

//            offset = NodeHeader.Size + sizeof(uint) + m_internalStructureSize * min;
//            return false;
//        }
        
//        ///<summary>
//        ///Allocates a new empty tree node.
//        ///</summary>
//        ///<param name="level">the level of the internal node</param>
//        ///<param name="childNodeBefore">the child value before</param>
//        ///<param name="key">the key that seperates the children</param>
//        ///<param name="childNodeAfter">the child after or equal to the key</param>
//        ///<returns>the index value of this new node.</returns>
//        public uint InternalNodeCreateEmptyNode(byte level, uint childNodeBefore, TKey key, uint childNodeAfter)
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
//            InternalNodeSaveKeyValue(key, m_stream);
//            m_stream.Write(childNodeAfter);
//            return nodeAddress;
//        }

//    }
//}
