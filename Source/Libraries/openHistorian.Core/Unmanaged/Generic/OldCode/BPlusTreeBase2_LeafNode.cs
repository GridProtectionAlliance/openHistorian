//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.Unmanaged.Generic
//{
//    abstract partial class BPlusTreeBase
//    {
//        int m_keySize;

//        int m_maximumLeafNodeChildren;
//        protected int m_leafStructureSize;

//        protected uint m_currentNode;
//        protected short m_childCount;
//        uint m_nextNode;
//        uint m_previousNode;

//        bool m_scanningTable;
//        int m_oldIndex;

//        public void LeafNodeInitialize()
//        {
//            m_keySize = SizeOfKey();
//            m_leafStructureSize = m_keySize + SizeOfValue();
//            m_maximumLeafNodeChildren = (m_blockSize - NodeHeader.Size) / (m_leafStructureSize);
//        }

//        public void LeafNodeSetCurrentNode(uint nodeIndex, bool isForWriting)
//        {
//            m_currentNode = nodeIndex;
//            m_stream.Position = nodeIndex * m_blockSize;
//            m_stream.UpdateLocalBuffer(isForWriting);

//            if (m_stream.ReadByte() != 0)
//                throw new Exception("The current node is not a leaf.");
//            m_childCount = m_stream.ReadInt16();
//            m_previousNode = m_stream.ReadUInt32();
//            m_nextNode = m_stream.ReadUInt32();
//        }

//        void LeafNodeSetStreamOffset(int position)
//        {
//            m_stream.Position = m_currentNode * m_blockSize + position;
//        }

//        /// <summary>
//        /// Splits the node into two parts.
//        /// Key2 contains the dividing key
//        /// </summary>
//        void LeafNodeSplitNode()
//        {
//            uint currentNode = m_currentNode;
//            uint oldNextNode = m_nextNode;

//            NodeHeader origionalNode = default(NodeHeader);
//            NodeHeader newNode = default(NodeHeader);
//            NodeHeader foreignNode = default(NodeHeader);

//            origionalNode.Load(m_stream, m_blockSize, m_currentNode);

//            if (m_childCount < 2)
//                throw new Exception("cannot split a node with fewer than 2 children");

//            short itemsInFirstNode = (short)(m_childCount >> 1); // divide by 2.
//            short itemsInSecondNode = (short)(m_childCount - itemsInFirstNode);

//            uint greaterNodeIndex = AllocateNewNode();
//            long sourceStartingAddress = m_currentNode * m_blockSize + NodeHeader.Size + m_leafStructureSize * itemsInFirstNode;
//            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size;

//            //lookup the first key that will be copied
//            m_stream.Position = sourceStartingAddress;
//            LoadKey2();

//            //do the copy
//            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_leafStructureSize);

//            //update the first header
//            m_childCount = itemsInFirstNode;
//            m_nextNode = greaterNodeIndex;

//            origionalNode.ChildCount = itemsInFirstNode;
//            origionalNode.NextNode = greaterNodeIndex;
//            origionalNode.Save(m_stream, m_blockSize, currentNode);

//            //update the second header
//            newNode.Level = 0;
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

//            if (CompareKeys12() >= 0)
//            {
//                LeafNodeSetCurrentNode(greaterNodeIndex, true);
//                LeafNodeInsert();
//            }
//            else
//            {
//                LeafNodeSetCurrentNode(currentNode, true);
//                LeafNodeInsert();
//            }
//            NodeWasSplit(0, currentNode, greaterNodeIndex);
//        }

//        /// <summary>
//        /// Seeks to the location of Key1. Or the position where the key could be inserted to preserve order.
//        /// </summary>
//        /// <param name="offset">the offset from the start of the node where the index was found</param>
//        /// <returns>true if a match was found, false if no match</returns>
//        protected virtual bool LeafNodeSeekToKey(out int offset)
//        {
//            long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;

//            int min = 0;
//            int max = m_childCount - 1;

//            while (min <= max)
//            {
//                int mid = min + (max - min >> 1);
//                m_stream.Position = startAddress + m_leafStructureSize * mid;
//                int tmpKey = CompareKey1WithStream();
//                if (tmpKey == 0)
//                {
//                    offset = NodeHeader.Size + m_leafStructureSize * mid;
//                    return true;
//                }
//                if (tmpKey > 0)
//                    min = mid + 1;
//                else
//                    max = mid - 1;
//            }
//            offset = NodeHeader.Size + m_leafStructureSize * min;
//            return false;
//        }

//        /// <summary>
//        /// Inserts Key1,Value1 into the current node. Splits the node if required.
//        /// </summary>
//        /// <returns>True if sucessfully inserted, false if a duplicate key was detected.</returns>
//        public bool LeafNodeInsert()
//        {
//            int offset;
//            long nodePositionStart = m_currentNode * m_blockSize;

//            if (m_childCount >= m_maximumLeafNodeChildren)
//            {
//                LeafNodeSplitNode();
//                return true;
//            }

//            //Find the best location to insert
//            if (LeafNodeSeekToKey(out offset)) //If found
//                return false;

//            int spaceToMove = NodeHeader.Size + m_leafStructureSize * m_childCount - offset;

//            //Insert the data
//            if (spaceToMove > 0)
//            {
//                LeafNodeSetStreamOffset(offset);
//                m_stream.InsertBytes(m_leafStructureSize, spaceToMove);
//            }

//            LeafNodeSetStreamOffset(offset);
//            SaveKey1();
//            SaveValue1();

//            //save the header
//            m_childCount++;
//            LeafNodeSetStreamOffset(1);
//            m_stream.Write(m_childCount);
//            return true;
//        }

//        /// <summary>
//        /// Uses Key1 to find the associated value and stores to Value1
//        /// </summary>
//        /// <returns>true if key is found, false if not found.</returns>
//        bool LeafNodeGetValue()
//        {
//            int offset;
//            if (LeafNodeSeekToKey(out offset))
//            {
//                LeafNodeSetStreamOffset(offset + m_keySize);
//                LoadValue1();
//                return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// Creates an empty leaf node.
//        /// </summary>
//        /// <returns>The index for the node.</returns>
//        uint LeafNodeCreateEmptyNode()
//        {
//            uint nodeAddress = AllocateNewNode();
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
