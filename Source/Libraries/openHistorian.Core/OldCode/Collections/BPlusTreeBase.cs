////******************************************************************************************************
////  BPlusTreeBase.cs - Gbtc
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
//using System.Collections;
//using System.Collections.Generic;
//using openHistorian.IO;

//namespace openHistorian.Collections
//{

//    /// <summary>
//    /// Provides the basic user methods with any derived B+Tree.  
//    /// This base class translates all of the core methods into simple methods
//    /// that must be implemented by classes derived from this base class.
//    /// </summary>
//    /// <typeparam name="TKey">The unique key that will be used to store data in this tree.</typeparam>
//    /// <typeparam name="TValue">The value that will be associated with each key.</typeparam>
//    /// <remarks>This class does not support concurrent read operations.  This is due to the caching method of each tree.
//    /// If concurrent read operations are desired, clone the tree.  
//    /// Trees cannot be cloned if the user plans to write to the tree.</remarks>
//    public abstract class BPlusTreeBase<TKey, TValue>
//    {
//        #region [ Nested Types ]

//        /// <summary>
//        /// Provides a way to iterate over the results of a query. 
//        /// </summary>
//        /// <remarks>
//        /// It is recommended to implement this class at the leaf node layer and 
//        /// also make the implementation support the concurrent read operations.
//        /// </remarks>
//        public abstract class DataReaderBase : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerator<KeyValuePair<TKey, TValue>>
//        {
//            /// <summary>
//            /// Resets the position of the reader to the start
//            /// </summary>
//            protected abstract void ReaderReset();
//            protected abstract bool ReaderNext();
//            protected abstract TValue ReaderGetValue();
//            protected abstract TKey ReaderGetKey();

//            protected virtual KeyValuePair<TKey, TValue> GetKeyValue()
//            {
//                return new KeyValuePair<TKey, TValue>(ReaderGetKey(), ReaderGetValue());
//            }

//            public virtual IEnumerable<TKey> Keys()
//            {
//                ReaderReset();
//                while (ReaderNext())
//                {
//                    yield return ReaderGetKey();
//                }
//            }
//            public virtual IEnumerable<TValue> Values()
//            {
//                ReaderReset();
//                while (ReaderNext())
//                {
//                    yield return ReaderGetValue();
//                }
//            }

//            /// <summary>
//            /// Since these base classes do not need IDispose, I left the implementation blank.
//            /// If at a future date this is needed, it will need to be properly written.
//            /// But I anticipate this will not occur within a BPlusTree;
//            /// </summary>
//            void IDisposable.Dispose()
//            {
//                //do nothing.
//            }

//            /// <summary>
//            /// Gets the current item.  The results returned here 
//            /// do not thow exceptions if it is before the beginning of the list.
//            /// </summary>
//            KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current
//            {
//                get
//                {
//                    return GetKeyValue();
//                }
//            }

//            /// <summary>
//            /// Gets the current item.  The results returned here 
//            /// do not thow exceptions if it is before the beginning of the list.
//            /// </summary>
//            object IEnumerator.Current
//            {
//                get
//                {
//                    return GetKeyValue();
//                }
//            }

//            bool IEnumerator.MoveNext()
//            {
//                return ReaderNext();
//            }

//            void IEnumerator.Reset()
//            {
//                ReaderReset();
//            }


//            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
//            {
//                return this;
//            }

//            IEnumerator IEnumerable.GetEnumerator()
//            {
//                return this;
//            }
//        }

//        #endregion

//        #region [ Members ]

//        //static Guid s_fileType = new Guid("{7bfa9083-701e-4596-8273-8680a739271d}");
//        int m_blockSize;
//        uint m_rootNodeIndex;
//        byte m_rootNodeLevel;
//        uint m_nextUnallocatedBlock;
//        IBinaryStream m_stream;

//        #endregion

//        #region [ Constructors ]

//        /// <summary>
//        /// Opens an existing <see cref="BPlusTreeBase{TKey,TValue}"/> from the stream.
//        /// </summary>
//        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
//        protected BPlusTreeBase(IBinaryStream stream)
//        {
//            m_stream = stream;
//            LoadHeader();
//        }

//        /// <summary>
//        /// Creates an empty <see cref="BPlusTreeBase{TKey,TValue}"/>
//        /// and uses the underlying stream to save data to it.
//        /// </summary>
//        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
//        /// <param name="blockSize">the size of one block.  This should exactly match the
//        /// amount of data space available in the underlying data object. BPlus trees get their 
//        /// performance benefit because there is fewer I/O's required to find and insert blocks.</param>
//        protected BPlusTreeBase(IBinaryStream stream, int blockSize)
//        {
//            m_stream = stream;
//            m_blockSize = blockSize;
//            m_rootNodeLevel = 0;
//            m_rootNodeIndex = 1;
//            LeafNodeCreateEmptyNode(m_rootNodeIndex);
//            m_nextUnallocatedBlock = 2;
//            SaveHeader();
//            LoadHeader();
//        }

//        #endregion

//        #region [ Properties ]

//        /// <summary>
//        /// Contains the stream for reading and writing and optional cloning.
//        /// </summary>
//        protected IBinaryStream Stream
//        {
//            get
//            {
//                return m_stream;
//            }
//        }
//        /// <summary>
//        /// Contains the block size that the tree nodes will be alligned on.
//        /// </summary>
//        protected int BlockSize
//        {
//            get
//            {
//                return m_blockSize;
//            }
//        }

//        #endregion

//        #region [ Public Methods ]

//        /// <summary>
//        /// Requests a range of data from the BPlusTree.
//        /// </summary>
//        /// <param name="filter">An optional filter that returns true if the provided key should be included in the scan.</param>
//        /// <returns>
//        /// An Enumerable class for getting the results of the query.
//        /// </returns>
//        public DataReaderBase GetRange(Func<TKey, bool> filter = null)
//        {
//            uint nodeIndex = m_rootNodeIndex;
//            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
//            {
//                nodeIndex = InternalNodeGetFirstIndex(nodeLevel, nodeIndex);
//            }
//            TKey key;
//            TValue value;
//            LeafNodeGetFirstKeyValue(nodeIndex, out key, out value);
//            return LeafNodeScan(nodeIndex, key, filter);
//        }
//        /// <summary>
//        /// Requests a range of data from the BPlusTree.
//        /// </summary>
//        /// <param name="filter">An optional filter that returns true if the provided key should be included in the scan.</param>
//        /// <returns>
//        /// An Enumerable class for getting the results of the query.
//        /// </returns>
//        public DataReaderBase GetRange(TKey beginKey, Func<TKey, bool> filter = null)
//        {
//            uint nodeIndex = m_rootNodeIndex;
//            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
//            {
//                nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, beginKey);
//            }

//            return LeafNodeScan(nodeIndex, beginKey, filter);
//        }

//        public DataReaderBase GetRange(TKey beginKey, TKey endKey, Func<TKey, bool> filter = null)
//        {
//            uint nodeIndex = m_rootNodeIndex;
//            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
//            {
//                nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, beginKey);
//            }

//            return LeafNodeScan(nodeIndex, beginKey, endKey, filter);
//        }

//        ////ToDo: Fix this feature
//        //public void Remove(TKey key)
//        //{
//        //    uint nodeIndex = m_rootNodeIndex;
//        //    for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
//        //    {
//        //        nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, key);
//        //    }

//        //    if (LeafNodeRemove(nodeIndex, key))
//        //        return;
//        //    throw new Exception("Key Not Found");
//        //}

//        ////ToDo: Fix this feature.
//        //public void Update(TKey key, TValue value)
//        //{
//        //    uint nodeIndex = m_rootNodeIndex;
//        //    for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
//        //    {
//        //        nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, key);
//        //    }

//        //    if (LeafNodeUpdate(nodeIndex, key, value))
//        //        return;
//        //    throw new Exception("Key Not Found");
//        //}

//        /// <summary>
//        /// Inserts the following data into the tree.
//        /// </summary>
//        /// <param name="key">The unique key value.</param>
//        /// <param name="value">The value to insert.</param>
//        public void Add(TKey key, TValue value)
//        {
//            uint nodeIndex = m_rootNodeIndex;
//            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
//            {
//                nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, key);
//            }

//            if (LeafNodeInsert(nodeIndex, key, value))
//                return;
//            throw new Exception("Key already exists");
//        }

//        /// <summary>
//        /// Returns the data for the following key. 
//        /// </summary>
//        /// <param name="key">The key to look up.</param>
//        /// <returns>Null or the Default structure value if the key does not exist.</returns>
//        public TValue Get(TKey key)
//        {
//            uint nodeIndex = m_rootNodeIndex;
//            for (byte nodeLevel = m_rootNodeLevel; nodeLevel > 0; nodeLevel--)
//            {
//                nodeIndex = InternalNodeGetIndex(nodeLevel, nodeIndex, key);
//            }

//            TValue value;
//            if (LeafNodeGetValue(nodeIndex, key, out value))
//                return value;
//            throw new Exception("Key Not Found");
//        }

//        #endregion

//        #region [ Abstract Methods ]

//        protected abstract Guid FileType { get; }

//        #region [ Internal Node Methods ]
//        //protected abstract void InternalNodeUpdate(int nodeLevel, uint nodeIndex, TKey oldFirstKey, TKey newFirstKey);

//        protected abstract uint InternalNodeGetFirstIndex(int nodeLevel, uint nodeIndex);
//        //protected abstract uint InternalNodeGetLastIndex(int nodeLevel, uint nodeIndex);
//        protected abstract uint InternalNodeGetIndex(int nodeLevel, uint nodeIndex, TKey key);

//        protected abstract void InternalNodeInsert(int nodeLevel, uint nodeIndex, TKey key, uint childNodeIndex);
//        protected abstract void InternalNodeCreateNode(uint newNodeIndex, int nodeLevel, uint firstNodeIndex, TKey dividingKey, uint secondNodeIndex);

//        //protected abstract void InternalNodeRemove(int nodeLevel, uint nodeIndex, TKey key);

//        #endregion

//        #region [ Leaf Node Methods ]

//        //protected abstract bool LeafNodeUpdate(uint nodeIndex, TKey key, TValue value);
//        protected abstract bool LeafNodeInsert(uint nodeIndex, TKey key, TValue value);
//        //protected abstract bool LeafNodeRemove(uint nodeIndex, TKey key);

//        protected abstract bool LeafNodeGetValue(uint nodeIndex, TKey key, out TValue value);
//        protected abstract bool LeafNodeGetFirstKeyValue(uint nodeIndex, out TKey key, out TValue value);
//        //protected abstract bool LeafNodeGetLastKeyValue(uint nodeIndex, out TKey key, out TValue value);
//        protected abstract void LeafNodeCreateEmptyNode(uint newNodeIndex);

//        protected abstract DataReaderBase LeafNodeScan(uint nodeIndex, TKey beginKey, TKey endKey, Func<TKey, bool> filter);
//        protected abstract DataReaderBase LeafNodeScan(uint nodeIndex, TKey beginKey, Func<TKey, bool> filter);

//        #endregion

//        #endregion

//        #region [ Protected Methods ]

//        /// <summary>
//        /// Returns the node index address for a freshly allocated block.
//        /// The node address is block alligned.
//        /// </summary>
//        /// <returns></returns>
//        protected uint GetNextNewNodeIndex()
//        {
//            uint newBlock = m_nextUnallocatedBlock;
//            m_nextUnallocatedBlock++;
//            return newBlock;
//        }

//        /// <summary>
//        /// Notifies the base class that a node was split. This will then add the new node data to the parent.
//        /// </summary>
//        /// <param name="nodeLevel">the level of the node being added</param>
//        /// <param name="nodeIndexOfSplitNode">the index of the existing node that contains the lower half of the data.</param>
//        /// <param name="dividingKey">the first key in the <see cref="nodeIndexOfRightSibling"/></param>
//        /// <param name="nodeIndexOfRightSibling">the index of the later node</param>
//        /// <remarks>This class will add the new node data to the parent node, 
//        /// or create a new root if the current root is split.</remarks>
//        protected void NodeWasSplit(int nodeLevel, uint nodeIndexOfSplitNode, TKey dividingKey, uint nodeIndexOfRightSibling)
//        {
//            if (m_rootNodeLevel > nodeLevel)
//            {
//                uint nodeIndex = m_rootNodeIndex;
//                for (byte level = m_rootNodeLevel; level > nodeLevel + 1; level--)
//                {
//                    nodeIndex = InternalNodeGetIndex(level, nodeIndex, dividingKey);
//                }
//                InternalNodeInsert(nodeLevel + 1, nodeIndex, dividingKey, nodeIndexOfRightSibling);
//            }
//            else
//            {
//                m_rootNodeLevel += 1;
//                m_rootNodeIndex = GetNextNewNodeIndex();
//                InternalNodeCreateNode(m_rootNodeIndex, m_rootNodeLevel, nodeIndexOfSplitNode, dividingKey, nodeIndexOfRightSibling);
//            }
//        }

//        //ToDo: Fix this feature
//        ///// <summary>
//        ///// Notifies the base class that two nodes were rebalanced. On a rebalance, the parent key needs to be updated.
//        ///// </summary>
//        ///// <param name="nodeLevel">the level of the node being rebalanced</param>
//        ///// <param name="oldKeyInLaterBlock">any key that used to be contained in the greater of the two nodes being rebalanced</param>
//        ///// <param name="newFirstKeyInLaterBlock">the first key in the greater of the two nodes after being rebalanced</param>
//        ///// <remarks>When a child node is rebalanced, it is important to update the parent node to reflect this new change.</remarks>
//        //protected void NodeWasRebalanced(byte nodeLevel, TKey oldKeyInLaterBlock, TKey newFirstKeyInLaterBlock)
//        //{
//        //    if (m_rootNodeLevel > nodeLevel)
//        //    {
//        //        uint nodeIndex = m_rootNodeIndex;
//        //        for (byte level = m_rootNodeLevel; level > nodeLevel + 1; level--)
//        //        {
//        //            nodeIndex = InternalNodeGetIndex(level, nodeIndex, oldKeyInLaterBlock);
//        //        }

//        //        InternalNodeUpdate(nodeLevel + 1, nodeIndex, oldKeyInLaterBlock, newFirstKeyInLaterBlock);
//        //    }
//        //    else
//        //    {
//        //        throw new Exception("Logic Error: The code should have never entered here");
//        //    }
//        //}

//        //ToDo: Fix this
//        ///// <summary>
//        ///// When two nodes are combined, the second node needs to be removed from the parent collection.
//        ///// </summary>
//        ///// <param name="nodeLevel">the level of the node being combined</param>
//        ///// <param name="oldKeyInRemovedBlock">A key that used to be contained in the old block that is being removed.</param>
//        ///// <param name="nodeToKeep">the node that is being kept</param>
//        ///// <param name="nodeToDelete">the node that is being deleted</param>
//        //protected void NodeWasCombined(byte nodeLevel, TKey oldKeyInRemovedBlock, uint nodeToKeep, uint nodeToDelete)
//        //{
//        //    //ToDo: There needs to be some kind of free page allocation routine so when nodes are removed, there location isn't lost.
//        //    if (m_rootNodeLevel > nodeLevel)
//        //    {
//        //        uint nodeIndex = m_rootNodeIndex;
//        //        for (byte level = m_rootNodeLevel; level > nodeLevel + 1; level--)
//        //        {
//        //            nodeIndex = InternalNodeGetIndex(level, nodeIndex, oldKeyInRemovedBlock);
//        //        }
//        //        InternalNodeRemove(nodeLevel + 1, nodeIndex, oldKeyInRemovedBlock);
//        //    }
//        //    else
//        //    {
//        //        m_rootNodeLevel = 0;
//        //        m_rootNodeIndex = nodeToKeep;
//        //    }
//        //}

//        #endregion

//        #region [ Private Methods ]

//        void LoadHeader()
//        {
//            Stream.Position = 0;
//            if (FileType != Stream.ReadGuid())
//                throw new Exception("Header Corrupt");
//            if (Stream.ReadByte() != 0)
//                throw new Exception("Header Corrupt");
//            m_nextUnallocatedBlock = Stream.ReadUInt32();
//            m_blockSize = Stream.ReadInt32();
//            m_rootNodeIndex = Stream.ReadUInt32();
//            m_rootNodeLevel = Stream.ReadByte();
//        }
//        void SaveHeader()
//        {
//            Stream.Position = 0;
//            Stream.Write(FileType);
//            Stream.Write((byte)0); //Version
//            Stream.Write(m_nextUnallocatedBlock);
//            Stream.Write(BlockSize);
//            Stream.Write(m_rootNodeIndex); //Root Index
//            Stream.Write(m_rootNodeLevel); //Root Index
//        }


//        #endregion


//    }
//}

