using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Generic
{
    abstract partial class BPlusTreeBase<TKey, TValue>

    {
        int m_internalNodeKeySize;

        int m_internalNodeMaximumChildren;
        protected int m_internalNodeStructureSize;

        protected uint m_internalNodeCurrentNode;

        protected short m_internalNodeChildCount;
        uint m_internalNodeNextNode;
        uint m_internalNodePreviousNode;

        public void InternalNodeInitialize()
        {
            m_internalNodeKeySize = SizeOfKey();
            m_internalNodeStructureSize = m_internalNodeKeySize + sizeof(uint);
            m_internalNodeMaximumChildren = (m_blockSize - NodeHeader.Size) / (m_internalNodeStructureSize);
        }

        void InternalNodeSetCurrentNode(uint nodeIndex, bool isForWriting)
        {
            m_internalNodeCurrentNode = nodeIndex;
            m_stream.Position = nodeIndex * m_blockSize;
            m_stream.UpdateLocalBuffer(isForWriting);

            m_stream.ReadByte(); //node level
            m_internalNodeChildCount = m_stream.ReadInt16();
            m_internalNodePreviousNode = m_stream.ReadUInt32();
            m_internalNodeNextNode = m_stream.ReadUInt32();
        }

        public void InternalNodeSetCurrentNode(byte nodeLevel, uint nodeIndex, bool isForWriting)
        {
            m_internalNodeCurrentNode = nodeIndex;
            m_stream.Position = nodeIndex * m_blockSize;
            m_stream.UpdateLocalBuffer(isForWriting);

            if (m_stream.ReadByte() != nodeLevel)
                throw new Exception("The current node is not a leaf.");
            m_internalNodeChildCount = m_stream.ReadInt16();
            m_internalNodePreviousNode = m_stream.ReadUInt32();
            m_internalNodeNextNode = m_stream.ReadUInt32();
        }

        void InternalNodeSetStreamOffset(int position)
        {
            m_stream.Position = m_internalNodeCurrentNode * m_blockSize + position;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        void InternalNodeSplitNode(TKey key, uint childNodeIndex)
        {
            uint currentNode = m_internalNodeCurrentNode;
            uint oldNextNode = m_internalNodeNextNode;
            TKey firstKeyInGreaterNode = default(TKey);

            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(m_stream, m_blockSize, m_internalNodeCurrentNode);

            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");

            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            uint greaterNodeIndex = AllocateNewNode();
            long sourceStartingAddress = m_internalNodeCurrentNode * m_blockSize + NodeHeader.Size + sizeof(uint) + m_internalNodeStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size + sizeof(uint);

            //lookup the first key that will be copied
            m_stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = LoadKey(m_stream);

            //do the copy
            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_internalNodeStructureSize);
            //Set the lookback position as invalid since this node should never be parsed for data before the first key.
            m_stream.Position = targetStartingAddress - sizeof(uint);
            m_stream.Write(0u);
            
            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(m_stream, m_blockSize, currentNode);

            //update the second header
            newNode.Level = origionalNode.Level;
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
            NodeWasSplit(origionalNode.Level, currentNode, firstKeyInGreaterNode, greaterNodeIndex);
            if (CompareKeys(key, firstKeyInGreaterNode) > 0)
            {
                InternalNodeSetCurrentNode(greaterNodeIndex, true);
                InternalNodeInsert(key, childNodeIndex);
            }
            else
            {
                InternalNodeSetCurrentNode(currentNode, true);
                InternalNodeInsert(key, childNodeIndex);
            }

        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this method will seek to the most appropriate location for 
        /// the key to be inserted and insert the data if the leaf is not full. 
        /// </summary>
        /// <param name="key">the key to insert</param>
        /// <param name="childNodeIndex">the child node that corresponds to this key</param>
        /// <param name="nodeIndex">the index of the node to be modified</param>
        /// <param name="nodeLevel">the level of the node</param>
        /// <returns>The results of the insert</returns>
        public void InternalNodeInsert(TKey key, uint childNodeIndex)
        {
            int offset;

            long nodePositionStart = m_internalNodeCurrentNode * m_blockSize;

            if (m_internalNodeChildCount >= m_internalNodeMaximumChildren)
            {
                InternalNodeSplitNode(key, childNodeIndex);
                return;
            }

            if (InternalNodeSeekToKey(key, out offset))
                throw new Exception("Duplicate Key");

            int spaceToMove = NodeHeader.Size + sizeof(uint) + m_internalNodeStructureSize * m_internalNodeChildCount - offset;

            if (spaceToMove > 0)
            {
                InternalNodeSetStreamOffset(offset);
                m_stream.InsertBytes(m_internalNodeStructureSize, spaceToMove);
            }

            InternalNodeSetStreamOffset(offset);
            SaveKey(key,m_stream);
            m_stream.Write(childNodeIndex);

            m_internalNodeChildCount++;
            InternalNodeSetStreamOffset(1);
            m_stream.Write(m_internalNodeChildCount);

        }

        /// <summary>
        /// Starting from the end of the internal node header, 
        /// this method will return the node index value that contains the provided key
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns></returns>
        public uint InternalNodeGetNodeIndex(TKey key)
        {
            int offset;
            if (InternalNodeSeekToKey(key, out offset))
            {
                InternalNodeSetStreamOffset(offset + m_internalNodeKeySize);
                return m_stream.ReadUInt32();
            }
            InternalNodeSetStreamOffset(offset - 4);
            return m_stream.ReadUInt32();
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        protected virtual bool InternalNodeSeekToKey(TKey key, out int offset)
        {
            long startAddress = m_internalNodeCurrentNode * m_blockSize + NodeHeader.Size + sizeof(uint);

            int min = 0;
            int max = m_internalNodeChildCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                m_stream.Position = startAddress + m_internalNodeStructureSize * mid;

                int tmpKey = CompareKeys(key, m_stream); ;
                if (tmpKey == 0)
                {
                    offset = NodeHeader.Size + sizeof(uint) + m_internalNodeStructureSize * mid;
                    return true;
                }
                if (tmpKey > 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }

            offset = NodeHeader.Size + sizeof(uint) + m_internalNodeStructureSize * min;
            return false;
        }

        ///<summary>
        ///Allocates a new empty tree node.
        ///</summary>
        ///<param name="level">the level of the internal node</param>
        ///<param name="childNodeBefore">the child value before</param>
        ///<param name="key">the key that seperates the children</param>
        ///<param name="childNodeAfter">the child after or equal to the key</param>
        ///<returns>the index value of this new node.</returns>
        public uint InternalNodeCreateEmptyNode(byte level, uint childNodeBefore, TKey key, uint childNodeAfter)
        {
            uint nodeAddress = AllocateNewNode();
            m_stream.Position = nodeAddress * m_blockSize;

            //Clearing the Node
            //Level = level;
            //ChildCount = 1;
            //NextNode = 0;
            //PreviousNode = 0;
            m_stream.Write(level);
            m_stream.Write((short)1);
            m_stream.Write(0L);
            m_stream.Write(childNodeBefore);
            SaveKey(key,m_stream);
            m_stream.Write(childNodeAfter);
            return nodeAddress;
        }

    }
}
