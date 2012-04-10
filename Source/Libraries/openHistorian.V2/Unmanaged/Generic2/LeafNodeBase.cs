//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using openHistorian.Core.StorageSystem.Generic;

//namespace openHistorian.Core.Unmanaged.Generic2
//{

//    partial class BPlusTreeBase<TKey, TValue>
//    {
//        public abstract class LeafNodeBase
//        {
//            /// <summary>
//            /// Assists in the read/write operations of the header of a node.
//            /// </summary>
//            struct NodeHeader
//            {
//                public const int Size = 11;
//                public byte Level;
//                public short ChildCount;
//                public uint PreviousNode;
//                public uint NextNode;

//                public void Load(BinaryStream stream, int blockSize, uint nodeIndex)
//                {
//                    stream.Position = blockSize * nodeIndex;
//                    Load(stream);
//                }

//                public void Load(BinaryStream stream)
//                {
//                    Level = stream.ReadByte();
//                    ChildCount = stream.ReadInt16();
//                    PreviousNode = stream.ReadUInt32();
//                    NextNode = stream.ReadUInt32();
//                }
//                /// <summary>
//                /// Saves the node data to the underlying stream. 
//                /// </summary>
//                /// <param name="header"></param>
//                /// <param name="nodeIndex"></param>
//                public void Save(BinaryStream stream, int blockSize, uint nodeIndex)
//                {
//                    stream.Position = blockSize * nodeIndex;
//                    Save(stream);
//                }
//                /// <summary>
//                /// Saves the node data to the underlying stream.
//                /// </summary>
//                /// <param name="stream"></param>
//                /// <remarks>
//                /// From the current position on the stream, the node header data is written to the stream.
//                /// The position after calling this function is at the end of the header
//                /// </remarks>
//                public void Save(BinaryStream stream)
//                {
//                    stream.Write(Level);
//                    stream.Write(ChildCount);
//                    stream.Write(PreviousNode);
//                    stream.Write(NextNode);
//                }
//            }

//            BPlusTreeBase<TKey, TValue> m_tree;

//            int m_keySize;

//            int m_maximumLeafNodeChildren;
//            int m_leafStructureSize;
//            int m_blockSize;
//            BinaryStream m_leafNodeStream;

//            uint m_currentNode;
//            short m_childCount;
//            uint m_nextNode;
//            uint m_previousNode;

//            bool m_scanningTable;
//            TKey m_startKey;
//            TKey m_stopKey;
//            int m_oldIndex;

//            public void Initialize(BPlusTreeBase<TKey, TValue> tree)
//            {
//                m_tree = tree;
//                m_leafNodeStream = m_tree.m_leafNodeStream;
//                m_blockSize = m_tree.m_blockSize;
//                m_keySize = m_tree.SizeOfKey();
//                m_leafStructureSize = m_keySize + m_tree.SizeOfValue();
//                m_maximumLeafNodeChildren = (m_blockSize - NodeHeader.Size) / (m_leafStructureSize);
//            }

//            void SetCurrentNode(uint nodeIndex, bool isForWriting)
//            {
//                bool changed = (m_currentNode != nodeIndex);
//                m_currentNode = nodeIndex;
//                m_leafNodeStream.Position = nodeIndex * m_blockSize;
//                m_leafNodeStream.UpdateLocalBuffer(isForWriting);

//                if (changed)
//                {
//                    if (m_leafNodeStream.ReadByte() != 0)
//                        throw new Exception("The current node is not a leaf.");
//                    m_childCount = m_leafNodeStream.ReadInt16();
//                    m_previousNode = m_leafNodeStream.ReadUInt32();
//                    m_nextNode = m_leafNodeStream.ReadUInt32();
//                }
//            }

//            void SetStreamOffset(int position)
//            {
//                m_leafNodeStream.Position = m_currentNode * m_blockSize + position;
//            }

//            void SplitNode(TKey key, TValue value)
//            {
//                uint currentNode = m_currentNode;
//                uint oldNextNode = m_nextNode;
//                TKey firstKeyInGreaterNode = default(TKey);

//                NodeHeader origionalNode = default(NodeHeader);
//                NodeHeader newNode = default(NodeHeader);
//                NodeHeader foreignNode = default(NodeHeader);

//                origionalNode.Load(m_leafNodeStream, m_blockSize, m_currentNode);

//                if (m_childCount < 2)
//                    throw new Exception("cannot split a node with fewer than 2 children");

//                short itemsInFirstNode = (short)(m_childCount >> 1); // divide by 2.
//                short itemsInSecondNode = (short)(m_childCount - itemsInFirstNode);

//                uint greaterNodeIndex = m_tree.AllocateNewNode();
//                long sourceStartingAddress = m_currentNode * m_blockSize + NodeHeader.Size + m_leafStructureSize * itemsInFirstNode;
//                long targetStartingAddress = greaterNodeIndex * m_blockSize + NodeHeader.Size;

//                //lookup the first key that will be copied
//                m_leafNodeStream.Position = sourceStartingAddress;
//                firstKeyInGreaterNode = m_tree.LoadKey(m_leafNodeStream);

//                //do the copy
//                m_leafNodeStream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_leafStructureSize);

//                //update the first header
//                m_childCount = itemsInFirstNode;
//                m_nextNode = greaterNodeIndex;

//                origionalNode.ChildCount = itemsInFirstNode;
//                origionalNode.NextNode = greaterNodeIndex;
//                origionalNode.Save(m_leafNodeStream, m_blockSize, currentNode);

//                //update the second header
//                newNode.Level = 0;
//                newNode.ChildCount = itemsInSecondNode;
//                newNode.PreviousNode = currentNode;
//                newNode.NextNode = oldNextNode;
//                newNode.Save(m_leafNodeStream, m_blockSize, greaterNodeIndex);

//                //update the node that used to be after the first one.
//                if (oldNextNode != 0)
//                {
//                    foreignNode.Load(m_leafNodeStream, m_blockSize, oldNextNode);
//                    foreignNode.PreviousNode = greaterNodeIndex;
//                    foreignNode.Save(m_leafNodeStream, m_blockSize, oldNextNode);
//                }

//                m_tree.NodeWasSplit(0, currentNode, firstKeyInGreaterNode, greaterNodeIndex);
//                if (m_tree.CompareKeys(key, firstKeyInGreaterNode) > 0)
//                {
//                    Insert(greaterNodeIndex, key, value);
//                }
//                else
//                {
//                    Insert(currentNode, key, value);
//                }
//            }

//            /// <summary>
//            /// Seeks to the location of the key. Or the position where the key could be inserted to preserve order.
//            /// </summary>
//            /// <param name="key">the key to look for</param>
//            /// <param name="offset">the offset from the start of the node where the index was found</param>
//            /// <returns>true if a match was found, false if no match</returns>
//            protected virtual bool SeekToKey(TKey key, out int offset)
//            {
//                long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;

//                int min = 0;
//                int max = m_childCount - 1;

//                while (min <= max)
//                {
//                    int mid = min + (max - min >> 1);
//                    m_leafNodeStream.Position = startAddress + m_leafStructureSize * mid;
//                    int tmpKey = m_tree.CompareKeys(key, m_leafNodeStream);
//                    if (tmpKey == 0)
//                    {
//                        offset = NodeHeader.Size + m_leafStructureSize * mid;
//                        return true;
//                    }
//                    if (tmpKey > 0)
//                        min = mid + 1;
//                    else
//                        max = mid - 1;
//                }
//                offset = NodeHeader.Size + m_leafStructureSize * min;
//                return false;
//            }

//            /// <summary>
//            /// Inserts the following key into the current node. Splits the node if required.
//            /// </summary>
//            /// <param name="key"></param>
//            /// <returns>True if sucessfully inserted, false if a duplicate key was detected.</returns>
//            public bool Insert(uint nodeIndex, TKey key, TValue value)
//            {

//                int offset;
//                long nodePositionStart = m_currentNode * m_blockSize;

//                if (m_childCount >= m_maximumLeafNodeChildren)
//                {
//                    SplitNode(key, value);
//                    return true;
//                }

//                //Find the best location to insert
//                if (SeekToKey(key, out offset)) //If found
//                    return false;

//                int spaceToMove = NodeHeader.Size + m_leafStructureSize * m_childCount - offset;

//                //Insert the data
//                if (spaceToMove > 0)
//                {
//                    SetStreamOffset(offset);
//                    m_leafNodeStream.InsertBytes(m_leafStructureSize, spaceToMove);
//                }

//                SetStreamOffset(offset);
//                m_tree.SaveKey(key, m_leafNodeStream);
//                m_tree.SaveValue(value, m_leafNodeStream);

//                //save the header
//                m_childCount++;
//                SetStreamOffset(1);
//                m_leafNodeStream.Write(m_childCount);
//                return true;
//            }


//            public bool GetValue(uint nodeIndex, TKey key, out TValue value)
//            {
//                int offset;
//                if (SeekToKey(key, out offset))
//                {
//                    SetStreamOffset(offset + m_keySize);
//                    value = m_tree.LoadValue(m_leafNodeStream);
//                    return true;
//                }
//                value = default(TValue);
//                return false;
//            }

//            public bool GetFirstKeyValue(uint nodeIndex, out TKey key, out TValue value)
//            {
//                key = default(TKey);
//                value = default(TValue);
//                return true;
//            }
//            public bool GetLastKeyValue(uint nodeIndex, out TKey key, out TValue value)
//            {
//                key = default(TKey);
//                value = default(TValue);
//                return true;
//            }

//            public uint CreateEmptyNode()
//            {
//                uint nodeAddress = m_tree.AllocateNewNode();
//                m_leafNodeStream.Position = m_blockSize * nodeAddress;

//                //Clearing the Node
//                //Level = 0;
//                //ChildCount = 0;
//                //NextNode = 0;
//                //PreviousNode = 0;
//                m_leafNodeStream.Write(0L);
//                m_leafNodeStream.Write(0);

//                return nodeAddress;
//            }

//            void PrepareForTableScan(TKey firstKey, TKey lastKey)
//            {
//                m_scanningTable = true;
//                m_startKey = firstKey;
//                m_stopKey = lastKey;
//                SeekToKey(firstKey, out m_oldIndex);
//                m_oldIndex = (m_oldIndex - NodeHeader.Size) / m_leafStructureSize;
//            }

//            bool GetNextKeyTableScan(out TKey key)
//            {
//                if (m_oldIndex >= m_childCount)
//                {
//                    if (m_nextNode == 0)
//                    {
//                        key = default(TKey);
//                        return false;
//                    }
//                    SetCurrentNode(m_nextNode, false);
//                    m_oldIndex = 0;
//                }
//                m_leafNodeStream.Position = m_currentNode * m_blockSize + m_oldIndex * m_leafStructureSize + NodeHeader.Size;
//                key = m_tree.LoadKey(m_leafNodeStream);

//                if (m_tree.CompareKeys(m_stopKey, key) <= 0)
//                    return false;
//                m_oldIndex++;
//                return true;
//            }

//            void CloseTableScan()
//            {
//                m_scanningTable = false;
//            }



//        }
//    }
//}
