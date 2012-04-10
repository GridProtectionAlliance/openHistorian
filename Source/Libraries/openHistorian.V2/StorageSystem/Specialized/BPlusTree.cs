//******************************************************************************************************
//  BPlusTree.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  2/27/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace openHistorian.V2.StorageSystem.Specialized
{
    /// <summary>
    /// Provides support for an in memory binary (plus) tree.  
    /// </summary>
    /// <typeparam name="TKey">The unique key to sort on. Must implement <seealso cref="IKeyType{TKey}"/>.</typeparam>
    /// <typeparam name="TValue">They value type to store. Must implement <seealso cref="IValueType"/>.</typeparam>
    /// <remarks>Think of this class as a <see cref="SortedList{TKey,TValue}"/> 
    /// for sorting thousands, millions, billions, or more items.  B+ trees do not suffer the same
    /// performance hit that a <see cref="SortedList{TKey,TValue}"/> does. </remarks>
    public partial class BPlusTree<TKey, TValue>
        where TKey : struct, IKeyType<TKey>
        where TValue : struct, IValueType<TValue>
    {
        #region [ Members ]

        TValue m_tmpValue;
        TKey m_tmpKey;

        ITreeLeafNodeMethods<TKey> m_leafNode;
        ITreeInternalNodeMethods<TKey>[] m_internalNode;

        #endregion

        #region [ Constructors ]



        /// <summary>
        /// Opens an existing <see cref="BPlusTree{TKey,TValue}"/> from the stream.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        public BPlusTree(BinaryStream stream)
        {
            Load(stream);
            Initialize();
        }


        /// <summary>
        /// Creates an empty <see cref="BPlusTree{TKey,TValue}"/>
        /// and uses the underlying stream to save data to it.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        /// <param name="blockSize">the size of one block.  This should exactly match the
        /// amount of data space available in the underlying data object. BPlus trees get their 
        /// performance benefit because there is fewer I/O's required to find and insert blocks.</param>
        public BPlusTree(BinaryStream stream, int blockSize)
        {
            m_stream = stream;
            m_blockSize = blockSize;
            Initialize();

            m_nextUnallocatedBlock = 1; //After calling m_leafNode.CreateEmptyNode, this will become 2
            m_rootIndexAddress = m_leafNode.CreateEmptyNode();
            m_rootIndexLevel = 0;
            Save(stream);
            Load(stream);

        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the standard member variables.
        /// </summary>
        void Initialize()
        {
            TKey key = new TKey();
            m_tmpValue = new TValue();
            m_leafNode = key.GetLeafNodeMethods();
            m_internalNode = new ITreeInternalNodeMethods<TKey>[10];
            for (int x = 0; x < 10; x++)
            {
                var node = key.GetInternalNodeMethods();
                node.Initialize(m_stream, (byte)x, m_blockSize, AllocateNewNode, NodeWasSplit);
                m_internalNode[x] = node;
            }
            m_leafNode.Initialize(m_stream, m_blockSize, m_tmpValue.SizeOf,
                WriteValueToCurrentStreamPosition, ReadValueAtCurrentStreamPosition,
                AllocateNewNode, NodeWasSplit);

        }

        /// <summary>
        /// Inserts the following data into the tree.
        /// </summary>
        /// <param name="key">The unique key value.</param>
        /// <param name="value">The value to insert.</param>
        public void AddData(TKey key, TValue value)
        {
            uint nodeIndex = m_rootIndexAddress;
            for (byte levelCount = m_rootIndexLevel; levelCount > 0; levelCount--)
            {
                m_internalNode[levelCount].SetCurrentNode(nodeIndex,true);
                nodeIndex = m_internalNode[levelCount].GetNodeIndex(key);
            }
            m_tmpValue = value;
            
            m_leafNode.SetCurrentNode(nodeIndex,true);
            if (m_leafNode.Insert(key))
                return;
            throw new Exception("Key already exists");
        }

        //public void RemoveItem(IBlockKey8 key)
        //{
        //    throw new NotSupportedException();
        //}

        //public void SetData(IBlockKey8 key, byte[] data)
        //{
        //    throw new NotSupportedException();
        //}

        /// <summary>
        /// Returns the data for the following key. 
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>Null or the Default structure value if the key does not exist.</returns>
        public TValue GetData(TKey key)
        {
            uint nodeIndex = m_rootIndexAddress;
            for (byte levelCount = m_rootIndexLevel; levelCount > 0; levelCount--)
            {
                m_internalNode[levelCount].SetCurrentNode(nodeIndex, false);
                nodeIndex = m_internalNode[levelCount].GetNodeIndex(key);
            }
            
            m_leafNode.SetCurrentNode(nodeIndex,false);
            if (m_leafNode.GetValue(key))
                return m_tmpValue;
            throw new Exception("Key Not Found");
        }

        public DataReader<TKey,TValue> ExecuteScan(TKey startKey, TKey stopKey)
        {
            uint nodeIndex = m_rootIndexAddress;
            for (byte levelCount = m_rootIndexLevel; levelCount > 0; levelCount--)
            {
                m_internalNode[levelCount].SetCurrentNode(nodeIndex, false);
                nodeIndex = m_internalNode[levelCount].GetNodeIndex(startKey);
            }

            m_leafNode.SetCurrentNode(nodeIndex, false);
            m_leafNode.PrepareForTableScan(startKey,stopKey);
            return new DataReader<TKey, TValue>(m_leafNode,m_stream);
        }

        void ReadValueAtCurrentStreamPosition()
        {
            m_tmpValue.LoadValue(m_stream);
        }

        void WriteValueToCurrentStreamPosition()
        {
            m_tmpValue.SaveValue(m_stream);
        }

        void NodeWasSplit(byte level, uint currentIndex, TKey middleKey, uint newIndex)
        {
            if (m_rootIndexLevel > level)
            {
                uint nodeIndex = m_rootIndexAddress;
                byte nodeLevel = m_rootIndexLevel;

                for (byte levelCount = nodeLevel; levelCount > level+1; levelCount--)
                {
                    m_internalNode[levelCount].SetCurrentNode(nodeIndex, false);
                    nodeIndex = m_internalNode[levelCount].GetNodeIndex(middleKey);
                }
                m_internalNode[level + 1].SetCurrentNode(nodeIndex, true);
                m_internalNode[level + 1].Insert(middleKey, newIndex);
            }
            else
            {
                m_rootIndexLevel += 1;
                m_rootIndexAddress = m_internalNode[m_rootIndexLevel].CreateEmptyNode(m_rootIndexLevel, currentIndex, middleKey, newIndex);
            }
        }

        public void Save()
        {
            Save(m_stream);
        }

        #endregion

    }
}
