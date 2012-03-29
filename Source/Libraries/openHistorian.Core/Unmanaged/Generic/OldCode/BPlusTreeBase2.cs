////******************************************************************************************************
////  BPlusTree.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
////  2/27/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using System.Collections.Generic;

//namespace openHistorian.Core.Unmanaged.Generic
//{
//    /// <summary>
//    /// Provides support for an in memory binary (plus) tree.  
//    /// </summary>
//    /// <remarks>Think of this class as a <see cref="SortedList{TKey,TValue}"/> 
//    /// for sorting thousands, millions, billions, or more items.  B+ trees do not suffer the same
//    /// performance hit that a <see cref="SortedList{TKey,TValue}"/> does. </remarks>
//    public abstract partial class BPlusTreeBase
//    {
//        #region [ Members ]

//        #endregion

//        #region [ Constructors ]

//        /// <summary>
//        /// Opens an existing <see cref="BPlusTreeBase{TKey,TValue}"/> from the stream.
//        /// </summary>
//        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
//        protected BPlusTreeBase(BinaryStream stream)
//        {
//            Load(stream);
//            Initialize();
//        }

//        /// <summary>
//        /// Creates an empty <see cref="BPlusTreeBase{TKey,TValue}"/>
//        /// and uses the underlying stream to save data to it.
//        /// </summary>
//        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
//        /// <param name="blockSize">the size of one block.  This should exactly match the
//        /// amount of data space available in the underlying data object. BPlus trees get their 
//        /// performance benefit because there is fewer I/O's required to find and insert blocks.</param>
//        protected BPlusTreeBase(BinaryStream stream, int blockSize)
//        {
//            m_stream = stream;
//            m_blockSize = blockSize;
//            Initialize();

//            m_nextUnallocatedBlock = 1; //After calling m_leafNode.CreateEmptyNode, this will become 2
//            m_rootIndexAddress = LeafNodeCreateEmptyNode();
//            m_rootIndexLevel = 0;
//            Save(stream);
//            Load(stream);
//        }

//        #endregion

//        #region [Abstract Methods]

//        protected abstract int SizeOfValue();
//        protected abstract int SizeOfKey();
//        protected abstract void SaveValue1();
//        protected abstract void LoadValue1();
//        protected abstract void SaveKey1();
//        protected abstract void LoadKey1();
//        protected abstract void SaveKey2();
//        protected abstract void LoadKey2();
//        protected abstract void SaveKey3();
//        protected abstract void LoadKey3();
//        protected abstract int CompareKeys12();
//        protected abstract int CompareKeys23();
//        protected abstract void CopyKey3ToKey2();
//        protected abstract int CompareKey1WithStream();

//        #endregion

//        #region [ Properties ]

//        #endregion

//        #region [ Methods ]

//        /// <summary>
//        /// Creates the standard member variables.
//        /// </summary>
//        void Initialize()
//        {
//            InternalNodeInitialize();
//            LeafNodeInitialize();
//        }

//        /// <summary>
//        /// Inserts the following data into the tree.
//        /// </summary>
//        protected void AddData()
//        {
//            uint nodeIndex = m_rootIndexAddress;
//            for (byte levelCount = m_rootIndexLevel; levelCount > 0; levelCount--)
//            {
//                InternalNodeSetCurrentNode(levelCount, nodeIndex, true);
//                nodeIndex = InternalNodeGetNodeIndex();
//            }

//            LeafNodeSetCurrentNode(nodeIndex, true);
//            if (LeafNodeInsert())
//                return;
//            throw new Exception("Key already exists");
//        }

//        //public void RemoveItem(IBlockKey8 key)
//        //{
//        //    throw new NotSupportedException();
//        //}

//        //public void SetData(IBlockKey8 key, byte[] data)
//        //{
//        //    throw new NotSupportedException();
//        //}

//        /// <summary>
//        /// Returns the data for the following key. 
//        /// </summary>
//        /// <returns>Null or the Default structure value if the key does not exist.</returns>
//        protected void GetData()
//        {
//            uint nodeIndex = m_rootIndexAddress;
//            for (byte levelCount = m_rootIndexLevel; levelCount > 0; levelCount--)
//            {
//                InternalNodeSetCurrentNode(levelCount, nodeIndex, false);
//                nodeIndex = InternalNodeGetNodeIndex();
//            }
//            LeafNodeSetCurrentNode(nodeIndex, false);
//            if (LeafNodeGetValue())
//                return;
//            throw new Exception("Key Not Found");
//        }

//        //public DataReader<TKey, TValue> ExecuteScan(TKey startKey, TKey stopKey)
//        //{
//        //    uint nodeIndex = m_rootIndexAddress;
//        //    for (byte levelCount = m_rootIndexLevel; levelCount > 0; levelCount--)
//        //    {
//        //        InternalNodeSetCurrentNode(levelCount, nodeIndex, false);
//        //        InternalNodeGetNodeIndex(startKey);
//        //    }

//        //    LeafNodeSetCurrentNode(nodeIndex, false);
//        //    LeafNodePrepareForTableScan(startKey, stopKey);
//        //    return null;//new DataReader<TKey, TValue>(null, m_stream);
//        //}

//        /// <summary>
//        /// Adds Key2 to its parent.
//        /// </summary>
//        /// <param name="level"></param>
//        /// <param name="currentIndex"></param>
//        /// <param name="newIndex"></param>
//        void NodeWasSplit(byte level, uint currentIndex, uint newIndex)
//        {
//            if (m_rootIndexLevel > level)
//            {
//                uint nodeIndex = m_rootIndexAddress;
//                byte nodeLevel = m_rootIndexLevel;

//                for (byte levelCount = nodeLevel; levelCount > level + 1; levelCount--)
//                {
//                    InternalNodeSetCurrentNode(levelCount, nodeIndex, false);
//                    nodeIndex = InternalNodeGetNodeIndex();
//                }
//                InternalNodeSetCurrentNode((byte)(level + 1), nodeIndex, true);
//                InternalNodeInsert(newIndex);
//            }
//            else
//            {
//                m_rootIndexLevel += 1;
//                m_rootIndexAddress = InternalNodeCreateEmptyNode(m_rootIndexLevel, currentIndex, newIndex);
//            }
//        }

//        public void Save()
//        {
//            Save(m_stream);
//        }

//        #endregion

//    }
//}
