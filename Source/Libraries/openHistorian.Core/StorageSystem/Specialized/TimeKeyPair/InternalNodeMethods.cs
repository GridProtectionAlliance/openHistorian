using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Specialized.TimeKeyPair
{

    unsafe class InternalNodeMethods : ITreeInternalNodeMethods<KeyType>
    {
        const int KeySize = 16;

        BinaryStream m_stream;
        int m_blockSize;
        AllocateNewNode m_allocateNewNode;
        NodeSplitRequired<KeyType> m_nodeSplit;
        int m_maximumInternalNodeChildren;
        int m_internalStructureSize;
        byte m_nodeLevel;

        uint m_lastIndex;
        bool m_matchBefore;
        bool m_matchAfter;
        long m_timeBefore;
        long m_timeAfter;
        long m_keyBefore;
        long m_keyAfter;
        uint m_lastBucket;

        public void Initialize(BinaryStream stream, byte nodeLevel, int blockSize, AllocateNewNode allocateNewNode, NodeSplitRequired<KeyType> nodeSplit)
        {
            m_stream = stream;
            m_nodeLevel = nodeLevel;
            m_blockSize = blockSize;
            m_allocateNewNode = allocateNewNode;
            m_nodeSplit = nodeSplit;
            m_internalStructureSize = KeySize + sizeof(uint);
            m_maximumInternalNodeChildren = (m_blockSize - NodeHeader.Size) / (m_internalStructureSize);
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        /// <param name="nodeToSplitIndex">the node index value to split</param>
        /// <param name="nodeLevel">the level of the node</param>
        void SplitNode(uint nodeToSplitIndex, byte nodeLevel, long keyTime, long keyKey, uint childNodeIndex)
        {
            uint greaterNodeIndex;
            KeyType firstKeyInGreaterNode;

            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(m_stream, m_blockSize, nodeToSplitIndex);
            if (origionalNode.Level != nodeLevel)
                throw new Exception();
            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");
            uint nextNode = origionalNode.NextNode;

            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            greaterNodeIndex = m_allocateNewNode();
            long sourceStartingAddress = nodeToSplitIndex * m_blockSize + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size + sizeof(uint);

            //lookup the first key that will be copied
            m_stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode.Time = m_stream.ReadDateTime();
            firstKeyInGreaterNode.Key = m_stream.ReadInt64();

            //do the copy
            m_stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_internalStructureSize);
            //Set the lookback position as invalid since this node should never be parsed for data before the first key.
            m_stream.Position = targetStartingAddress - sizeof(uint);
            m_stream.Write(0u);


            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(m_stream, m_blockSize, nodeToSplitIndex);

            //update the second header
            newNode.Level = origionalNode.Level;
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
            m_nodeSplit(nodeLevel, nodeToSplitIndex, firstKeyInGreaterNode, greaterNodeIndex);
            if (keyTime > firstKeyInGreaterNode.Time.Ticks || (keyTime == firstKeyInGreaterNode.Time.Ticks && keyKey > firstKeyInGreaterNode.Key))
            {
                Insert(greaterNodeIndex, nodeLevel, keyTime, keyKey, childNodeIndex);
            }
            else
            {
                Insert(nodeToSplitIndex, nodeLevel, keyTime, keyKey, childNodeIndex);
            }
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="lp">the index of the node</param>
        /// <param name="nodeLevel">the level of the node</param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        bool SeekToKey(long keyTime, long keyKey, byte* lp, byte nodeLevel, out int offset)
        {
            byte level = *(lp);
            short childCount = *(short*)(lp + 1);

            if (nodeLevel != level)
                throw new Exception("Corrupt BPlusTree: Unexpected Node Level");

            int min = 0;
            int max = childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);

                long tmpTime = *(long*)(lp + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * mid); ;

                if (tmpTime == keyTime)
                {

                    long tmpKey = *(long*)(lp + NodeHeader.Size + sizeof(uint) + m_internalStructureSize * mid + 8);

                    if (tmpKey == keyKey)
                    {
                        offset = NodeHeader.Size + sizeof (uint) + m_internalStructureSize * mid;

                        m_matchBefore = true;
                        m_timeBefore = tmpTime;
                        m_keyBefore = tmpKey;
                        m_lastBucket = *(uint*)(lp + offset + KeySize);

                        if (mid == (childCount - 1))
                        {
                            m_matchAfter = false;
                        }
                        else
                        {
                            m_matchAfter = true;
                            m_timeAfter = *(long*)(lp + offset + KeySize + 4);
                            m_keyAfter = *(long*)(lp + offset + KeySize + 4 + 8);
                        }

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
            }
            offset = NodeHeader.Size + sizeof(uint) + m_internalStructureSize * min;

            if (min == 0)
            {
                m_matchBefore = false;
                m_lastBucket = *(uint*)(lp + offset - 4);

                m_matchAfter = true;
                m_timeAfter = *(long*)(lp + offset);
                m_keyAfter = *(long*)(lp + offset + 8);
                return false;
            }
            if (min == childCount)
            {
                m_matchBefore = true;
                m_timeBefore = *(long*)(lp + offset - 4 - 16);
                m_keyBefore = *(long*)(lp + offset - 4 - 8);
                m_lastBucket = *(uint*)(lp + offset - 4);
                m_matchAfter = false;
                return false;
            }

            m_matchBefore = true;
            m_timeBefore = *(long*)(lp + offset - 4 - 16);
            m_keyBefore = *(long*)(lp + offset - 4 - 8);
            m_lastBucket = *(uint*)(lp + offset - 4);
            m_matchAfter = true;
            m_timeAfter = *(long*)(lp + offset);
            m_keyAfter = *(long*)(lp + offset + 8);

            return false;
        }

        void Insert(uint nodeIndex, byte nodeLevel, long keyTime, long keyKey, uint childNodeIndex)
        {
            m_lastIndex = 0;

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

                if (level != nodeLevel)
                    throw new Exception("Corrupt Node Level");

                if (childCount >= m_maximumInternalNodeChildren)
                {
                    SplitNode(nodeIndex, nodeLevel, keyTime, keyKey, childNodeIndex);
                    return;
                }

                if (SeekToKey(keyTime, keyKey, lpp, level, out offset))
                    throw new Exception("Duplicate Key");

                int spaceToMove = NodeHeader.Size + sizeof(uint) + m_internalStructureSize * childCount - offset;

                if (spaceToMove > 0)
                {
                    m_stream.Position += offset;
                    m_stream.InsertBytes(m_internalStructureSize, spaceToMove);
                    //InsertBytes(lpp, offset, m_internalStructureSize, spaceToMove);
                }

                *(long*)(lpp + offset) = keyTime;
                *(long*)(lpp + offset + 8) = keyKey;
                *(uint*)(lpp + offset + 16) = childNodeIndex;

                childCount++;
                *(short*)(lpp + 1) = childCount;
            }
        }

        uint GetNodeIndex(uint nodeIndex, byte nodeLevel, long keyTime, long keyKey)
        {
            if (nodeIndex == m_lastIndex)
            {
                if (!m_matchBefore || keyTime > m_timeBefore || (keyTime == m_timeBefore && keyKey >= m_keyBefore))
                {
                    if (!m_matchAfter || keyTime < m_timeAfter || (keyTime == m_timeAfter && keyKey < m_keyAfter))
                    {
                        return m_lastBucket;
                    }
                }
            }
            m_lastIndex = nodeIndex;

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

                if (SeekToKey(keyTime, keyKey, lpp, nodeLevel, out offset))
                {
                    return *(uint*)(lpp + offset + KeySize);
                    //m_stream.Position += KeySize;
                    //return m_stream.ReadUInt32();
                }
                return *(uint*)(lpp + offset - 4);
                //m_stream.Position -= 4;
                //return m_stream.ReadUInt32();
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
        public void Insert(uint nodeIndex, byte nodeLevel, KeyType key, uint childNodeIndex)
        {
            Insert(nodeIndex, nodeLevel, key.Time.Ticks, key.Key, childNodeIndex);
        }

        /// <summary>
        /// Starting from the end of the internal node header, 
        /// this method will return the node index value that contains the provided key
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="nodeIndex">the index of the node to be modified</param>
        /// <param name="nodeLevel">the level of the node</param>
        /// <returns></returns>
        public uint GetNodeIndex(uint nodeIndex, byte nodeLevel, KeyType key)
        {
            return GetNodeIndex(nodeIndex, nodeLevel, key.Time.Ticks, key.Key);
        }

        ///<summary>
        ///Allocates a new empty tree node.
        ///</summary>
        ///<param name="level">the level of the internal node</param>
        ///<param name="childNodeBefore">the child value before</param>
        ///<param name="key">the key that seperates the children</param>
        ///<param name="childNodeAfter">the child after or equal to the key</param>
        ///<returns>the index value of this new node.</returns>
        public uint CreateEmptyNode(byte level, uint childNodeBefore, KeyType key, uint childNodeAfter)
        {
            uint nodeAddress = m_allocateNewNode();
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
            m_stream.Write(key.Time.Ticks);
            m_stream.Write(key.Key);
            m_stream.Write(childNodeAfter);

            return nodeAddress;
        }

    }
}
