////******************************************************************************************************
////  BPlusTreeLeafNodeBase.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  4/7/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using openHistorian.IO;

//namespace openHistorian.Collections
//{
//    public abstract class BPlusTreeLeafNodeBase<TKey, TValue> : BPlusTreeInternalNodeBase<TKey, TValue>
//    {
//        #region [ Nexted Types ]

//        /// <summary>
//        /// Assists in the read/write operations of the header of a node.
//        /// </summary>
//        struct NodeHeader
//        {
//            public const int Size = 11;
//            public byte NodeLevel;
//            public short NodeRecordCount;
//            public uint LeftSiblingNodeIndex;
//            public uint RightSiblingNodeIndex;

//            public NodeHeader(IBinaryStream stream, int blockSize, uint nodeIndex)
//            {
//                stream.Position = blockSize * nodeIndex;
//                NodeLevel = stream.ReadByte();
//                NodeRecordCount = stream.ReadInt16();
//                LeftSiblingNodeIndex = stream.ReadUInt32();
//                RightSiblingNodeIndex = stream.ReadUInt32();
//            }
//            public void Save(IBinaryStream stream, int blockSize, uint nodeIndex)
//            {
//                stream.Position = blockSize * nodeIndex;
//                stream.Write(NodeLevel);
//                stream.Write(NodeRecordCount);
//                stream.Write(LeftSiblingNodeIndex);
//                stream.Write(RightSiblingNodeIndex);
//            }
//        }

//        private class DataReader : DataReaderBase
//        {

//            /// <summary>
//            /// A reference to the tree data to reference.
//            /// </summary>
//            BPlusTreeLeafNodeBase<TKey, TValue> m_tree;
//            /// <summary>
//            /// The key to start with
//            /// </summary>
//            TKey m_beginKey;
//            /// <summary>
//            /// The key to end on.  The query will not include this key.
//            /// </summary>
//            TKey m_endKey;
//            /// <summary>
//            /// The node index of the first key
//            /// </summary>
//            uint m_origionalNodeIndexOfBeginKey;
//            /// <summary>
//            /// A filter to apply if the value is not null.
//            /// </summary>
//            Func<TKey, bool> m_filter;
//            /// <summary>
//            /// Determines if the reader should stop at the end key or continue on to the end of the stream
//            /// </summary>
//            bool m_readToEndOfStream;

//            //Node Data
//            short m_nodeRecordCount;
//            uint m_rightSiblingNodeIndex;
//            uint m_nodeIndex;

//            bool m_isValid;
//            /// <summary>
//            /// The zero based index of the key that the cursor currently points to.  
//            /// </summary>
//            int m_keyIndexOfCurrentKey;

//            TKey m_key;
//            TValue m_value;

//            public DataReader(BPlusTreeLeafNodeBase<TKey, TValue> tree, TKey beginKey, TKey endKey, uint nodeIndexOfBeginKey, Func<TKey, bool> filter, bool readToEndOfStream)
//            {
//                m_tree = tree;
//                m_beginKey = beginKey;
//                m_endKey = endKey;
//                m_origionalNodeIndexOfBeginKey = nodeIndexOfBeginKey;
//                m_filter = filter;
//                m_readToEndOfStream = readToEndOfStream;
//                ReaderReset();
//            }

//            protected sealed override void ReaderReset()
//            {
//                uint leftSiblingNodeIndex;
//                m_nodeIndex = m_origionalNodeIndexOfBeginKey;
//                m_tree.LoadNodeHeader(m_nodeIndex, false, out m_nodeRecordCount, out leftSiblingNodeIndex, out m_rightSiblingNodeIndex);

//                m_tree.FindOffsetOfKey(m_nodeIndex, m_nodeRecordCount, m_beginKey, out m_keyIndexOfCurrentKey);
//                m_keyIndexOfCurrentKey = (m_keyIndexOfCurrentKey - NodeHeader.Size) / m_tree.m_structureSize;
//            }

//            protected override bool ReaderNext()
//            {
//                while (true)
//                {
//                    //If there are no more records in the current node.
//                    if (m_keyIndexOfCurrentKey >= m_nodeRecordCount)
//                    {
//                        //If the last leaf node, return false
//                        if (m_rightSiblingNodeIndex == 0)
//                            return false;

//                        //Move to the next node in the linked list.
//                        uint previousNode;
//                        m_nodeIndex = m_rightSiblingNodeIndex;
//                        m_tree.LoadNodeHeader(m_nodeIndex, false, out m_nodeRecordCount, out previousNode, out m_rightSiblingNodeIndex);
//                        m_keyIndexOfCurrentKey = 0;
//                    }

//                    m_tree.Stream.Position = m_nodeIndex * m_tree.BlockSize + m_keyIndexOfCurrentKey * m_tree.m_structureSize + NodeHeader.Size;

//                    m_key = m_tree.LoadKey(m_tree.Stream);

//                    //Check and see if the end of the query has occured
//                    if (!m_readToEndOfStream && m_tree.CompareKeys(m_endKey, m_key) <= 0)
//                        return false;

//                    //move to the next key
//                    m_keyIndexOfCurrentKey++;

//                    if (m_filter == null || m_filter.Invoke(m_key))
//                    {
//                        //Load the current value
//                        m_value = m_tree.LoadValue(m_tree.Stream);
//                        return true;

//                    }
//                } 

//            }

//            protected override TValue ReaderGetValue()
//            {
//                if (!m_isValid)
//                    throw new Exception("Value is no longer valid.  Either the end of the stream has been encoutered or the initial read was never performed");
//                return m_value;
//            }

//            protected override TKey ReaderGetKey()
//            {
//                if (!m_isValid)
//                    throw new Exception("Key is no longer valid.  Either the end of the stream has been encoutered or the initial read was never performed");
//                return m_key;
//            }
//        }

//        #endregion

//        #region [ Members ]

//        int m_keySize;
//        int m_maximumRecordsPerNode;
//        int m_structureSize;

//        #endregion

//        #region [ Constructors ]

//        protected BPlusTreeLeafNodeBase(IBinaryStream stream)
//            : base(stream)
//        {
//            Initialize();
//        }


//        protected BPlusTreeLeafNodeBase(IBinaryStream stream, int blockSize)
//            : base(stream, blockSize)
//        {
//            Initialize();
//        }

//        #endregion

//        #region [ Properties ]

//        #endregion

//        #region [ Methods ]

//        #region [ Abstract Methods ]

//        protected abstract void SaveValue(TValue value, IBinaryStream stream);
//        protected abstract TValue LoadValue(IBinaryStream stream);
//        protected abstract int SizeOfValue();

//        #endregion
//        #region [ Override Methods ]

//        protected override void LeafNodeCreateEmptyNode(uint newNodeIndex)
//        {
//            Stream.Position = BlockSize * newNodeIndex;

//            //Clearing the Node
//            //Level = 0;
//            //ChildCount = 0;
//            //NextNode = 0;
//            //PreviousNode = 0;
//            Stream.Write(0L);
//            Stream.Write(0);
//        }

//        //protected override bool LeafNodeUpdate(uint nodeIndex, TKey key, TValue value)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //protected override bool LeafNodeRemove(uint nodeIndex, TKey key)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        protected override bool LeafNodeInsert(uint nodeIndex, TKey key, TValue value)
//        {
//            short nodeRecordCount;
//            uint leftSiblingNodeIndex;
//            uint rightSiblingNodeIndex;
//            int offset;

//            LoadNodeHeader(nodeIndex, true, out nodeRecordCount, out leftSiblingNodeIndex, out rightSiblingNodeIndex);

//            //Find the best location to insert
//            //This is done before checking if a split is required to prevent splitting 
//            //if a duplicate key is found
//            if (FindOffsetOfKey(nodeIndex, nodeRecordCount, key, out offset)) //If found
//                return false;

//            //Check if the node needs to be split
//            if (nodeRecordCount >= m_maximumRecordsPerNode)
//            {
//                SplitNodeThenInsert(key, value, nodeIndex);
//                return true;
//            }

//            //set the stream's position to the best insert location.
//            Stream.Position = nodeIndex * BlockSize + offset;

//            //Determine the number of bytes that need to be shifted in order to insert the key
//            int bytesAfterInsertPositionToShift = NodeHeader.Size + m_structureSize * nodeRecordCount - offset;
//            if (bytesAfterInsertPositionToShift > 0)
//            {
//                Stream.InsertBytes(m_structureSize, bytesAfterInsertPositionToShift);
//            }

//            //Insert the data
//            SaveKey(key, Stream);
//            SaveValue(value, Stream);

//            //save the header
//            SaveNodeHeader(nodeIndex, (short)(nodeRecordCount + 1));
//            return true;
//        }

//        /// <summary>
//        /// Outputs the value associated with the provided key in the given node.
//        /// </summary>
//        /// <param name="nodeIndex">the node to search</param>
//        /// <param name="key">the key to look for</param>
//        /// <param name="value">the associated value.  Null/Default if key is not found</param>
//        /// <returns>true if the key was found, false if the key was not found.</returns>
//        protected override bool LeafNodeGetValue(uint nodeIndex, TKey key, out TValue value)
//        {
//            short nodeRecordCount;
//            uint leftSiblingNodeIndex;
//            uint rightSiblingNodeIndex;
//            int offset;

//            LoadNodeHeader(nodeIndex, false, out nodeRecordCount, out leftSiblingNodeIndex, out rightSiblingNodeIndex);

//            if (FindOffsetOfKey(nodeIndex, nodeRecordCount, key, out offset))
//            {
//                Stream.Position = nodeIndex * BlockSize + (offset + m_keySize);
//                value = LoadValue(Stream);
//                return true;
//            }
//            value = default(TValue);
//            return false;
//        }

//        protected override bool LeafNodeGetFirstKeyValue(uint nodeIndex, out TKey key, out TValue value)
//        {
//            short nodeRecordCount;
//            uint leftSiblingNodeIndex;
//            uint rightSiblingNodeIndex;

//            LoadNodeHeader(nodeIndex, false, out nodeRecordCount, out leftSiblingNodeIndex, out rightSiblingNodeIndex);
//            if (nodeRecordCount > 0)
//            {
//                Stream.Position = nodeIndex * BlockSize + NodeHeader.Size;
//                key = LoadKey(Stream);
//                value = LoadValue(Stream);
//                return true;
//            }
//            key = default(TKey);
//            value = default(TValue);
//            return false;
//        }

//        //protected override bool LeafNodeGetLastKeyValue(uint nodeIndex, out TKey key, out TValue value)
//        //{
//        //    throw new NotImplementedException();
//        //}


//        protected override DataReaderBase LeafNodeScan(uint nodeIndex, TKey beginKey, Func<TKey, bool> filter)
//        {
//            return new DataReader(this, beginKey, default(TKey), nodeIndex, filter, true);
//        }

//        protected override DataReaderBase LeafNodeScan(uint nodeIndex, TKey beginKey, TKey endKey, Func<TKey, bool> filter)
//        {
//            return new DataReader(this, beginKey, endKey, nodeIndex, filter, false);
//        }

//        #endregion

//        #region [ Helper Methods ]

//        void Initialize()
//        {
//            m_keySize = SizeOfKey();
//            m_structureSize = m_keySize + SizeOfValue();
//            m_maximumRecordsPerNode = (BlockSize - NodeHeader.Size) / (m_structureSize);
//        }

//        NodeHeader LoadNodeHeader(uint nodeIndex)
//        {
//            return new NodeHeader(Stream, BlockSize, nodeIndex);
//        }

//        void LoadNodeHeader(uint nodeIndex, bool isForWriting, out short nodeRecordCount, out uint leftSiblingNodeIndex, out uint rightSiblingNodeIndex)
//        {
//            Stream.Position = nodeIndex * BlockSize;
//            Stream.UpdateLocalBuffer(isForWriting);

//            if (Stream.ReadByte() != 0)
//                throw new Exception("The current node is not a leaf.");
//            nodeRecordCount = Stream.ReadInt16();
//            leftSiblingNodeIndex = Stream.ReadUInt32();
//            rightSiblingNodeIndex = Stream.ReadUInt32();
//        }

//        void SaveNodeHeader(uint nodeIndex, short nodeRecordCount)
//        {
//            Stream.Position = nodeIndex * BlockSize + 1;
//            Stream.Write(nodeRecordCount);
//        }


//        /// <summary>
//        /// Seeks to the location of the key or the position where the key could be inserted to preserve order.
//        /// </summary>
//        /// <param name="nodeIndex">the index of the node to search</param>
//        /// <param name="nodeRecordCount">the number of records already in the current node</param>
//        /// <param name="key">the key to look for</param>
//        /// <param name="offset">the offset from the start of the node where the index was found</param>
//        /// <returns>true the key was found in the node, false if was not found.</returns>
//        bool FindOffsetOfKey(uint nodeIndex, int nodeRecordCount, TKey key, out int offset)
//        {
//            long addressOfFirstKey = nodeIndex * BlockSize + NodeHeader.Size;
//            int searchLowerBoundsIndex = 0;
//            int searchHigherBoundsIndex = nodeRecordCount - 1;

//            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
//            {
//                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);
//                Stream.Position = addressOfFirstKey + m_structureSize * currentTestIndex;
//                int compareKeysResults = CompareKeys(key, Stream);
//                if (compareKeysResults == 0) //if keys match, result is found.
//                {
//                    offset = NodeHeader.Size + m_structureSize * currentTestIndex;
//                    return true;
//                }
//                if (compareKeysResults > 0) //if the key is greater than the test index, change the lower bounds
//                    searchLowerBoundsIndex = currentTestIndex + 1;
//                else //if the key is less than the current test index, change the upper bounds.
//                    searchHigherBoundsIndex = currentTestIndex - 1;
//            }
//            offset = NodeHeader.Size + m_structureSize * searchLowerBoundsIndex;
//            return false;
//        }

//        void SplitNodeThenInsert(TKey key, TValue value, uint nodeIndex)
//        {
//            NodeHeader firstNodeHeader = LoadNodeHeader(nodeIndex);
//            NodeHeader secondNodeHeader = default(NodeHeader);

//            //This should never be the case, but it's here none the less.
//            if (firstNodeHeader.NodeRecordCount < 2)
//                throw new Exception("cannot split a node with fewer than 2 children");

//            //Determine how many entries to shift on the split.
//            short recordsInTheFirstNode = (short)(firstNodeHeader.NodeRecordCount >> 1); // divide by 2.
//            short recordsInTheSecondNode = (short)(firstNodeHeader.NodeRecordCount - recordsInTheFirstNode);

//            uint secondNodeIndex = GetNextNewNodeIndex();
//            long sourceStartingAddress = nodeIndex * BlockSize + NodeHeader.Size + m_structureSize * recordsInTheFirstNode;
//            long targetStartingAddress = secondNodeIndex * BlockSize + NodeHeader.Size;

//            //lookup the first key that will be copied
//            Stream.Position = sourceStartingAddress;
//            TKey dividingKey = LoadKey(Stream);

//            //do the copy
//            Stream.Copy(sourceStartingAddress, targetStartingAddress, recordsInTheSecondNode * m_structureSize);

//            //update the node that was the old right sibling
//            if (firstNodeHeader.RightSiblingNodeIndex != 0)
//            {
//                NodeHeader oldRightSibling = LoadNodeHeader(firstNodeHeader.RightSiblingNodeIndex);
//                oldRightSibling.LeftSiblingNodeIndex = secondNodeIndex;
//                oldRightSibling.Save(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
//            }

//            //update the second header
//            secondNodeHeader.NodeLevel = 0;
//            secondNodeHeader.NodeRecordCount = recordsInTheSecondNode;
//            secondNodeHeader.LeftSiblingNodeIndex = nodeIndex;
//            secondNodeHeader.RightSiblingNodeIndex = firstNodeHeader.RightSiblingNodeIndex;
//            secondNodeHeader.Save(Stream, BlockSize, secondNodeIndex);

//            //update the first header
//            firstNodeHeader.NodeRecordCount = recordsInTheFirstNode;
//            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
//            firstNodeHeader.Save(Stream, BlockSize, nodeIndex);

//            NodeWasSplit(0, nodeIndex, dividingKey, secondNodeIndex);
//            if (CompareKeys(key, dividingKey) > 0)
//            {
//                LeafNodeInsert(secondNodeIndex, key, value);
//            }
//            else
//            {
//                LeafNodeInsert(nodeIndex, key, value);
//            }
//        }


//        #endregion

//        #endregion


//    }
//}
