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

using System.Collections.Generic;

namespace openHistorian.Core.StorageSystem.Generic
{
    /// <summary>
    /// Provides support for an in memory binary (plus) tree.  
    /// </summary>
    /// <typeparam name="TKey">The unique key to sort on. Must implement <seealso cref="ITreeType{T}"/>.</typeparam>
    /// <typeparam name="TValue">They value type to store. Must implement <seealso cref="ITreeType{T}"/>.</typeparam>
    /// <remarks>Think of this class as a <see cref="SortedList{TKey,TValue}"/> 
    /// for sorting thousands, millions, billions, or more items.  B+ trees do not suffer the same
    /// performance hit that a <see cref="SortedList{TKey,TValue}"/> does. </remarks>
    public partial class BPlusTree<TKey, TValue>
        where TKey : struct, ITreeType<TKey>
        where TValue : struct, ITreeType<TValue>
    {
        #region [ Members ]

        /// <summary>
        /// The size of the key in bytes
        /// </summary>
        int m_keySize;
        /// <summary>
        /// The size of the value field in bytes
        /// </summary>
        int m_valueSize;

        /// <summary>
        /// Stores the information for the most recently matched internal nodes.
        /// </summary>
        TreeLookupCache<TKey> m_cache;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates the standard member variables.
        /// </summary>
        BPlusTree()
        {
            TKey key = new TKey();
            TValue value = new TValue();
            m_keySize = key.SizeOf;
            m_valueSize = key.SizeOf;
            m_leafStructureSize = key.SizeOf + value.SizeOf;
            m_internalStructureSize = key.SizeOf + sizeof(uint);
        }

        /// <summary>
        /// Opens an existing <see cref="BPlusTree{TKey,TValue}"/> from the stream.
        /// </summary>
        /// <param name="stream">A dedicated stream where data can be read/written to/from.</param>
        public BPlusTree(BinaryStream stream)
            : this()
        {
            Load(stream);
            m_cache = new TreeLookupCache<TKey>(m_stream, m_internalStructureSize);
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
            : this()
        {
            m_stream = stream;
            m_blockSize = blockSize;
            m_maximumLeafNodeChildren = LeafNodeCalculateMaximumChildren();
            m_maximumInternalNodeChildren = InternalNodeCalculateMaximumChildren();
            m_nextUnallocatedBlock = 1;
            m_rootIndexAddress = LeafNodeCreateEmptyNode();
            m_rootIndexLevel = 0;
            Save(stream);
            Load(stream);
            m_cache = new TreeLookupCache<TKey>(m_stream, m_internalStructureSize);
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Inserts the following data into the tree.
        /// </summary>
        /// <param name="key">The unique key value.</param>
        /// <param name="value">The value to insert.</param>
        public void AddData(TKey key, TValue value)
        {
            AddItem(key, value, ref m_rootIndexAddress, ref m_rootIndexLevel);
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
            return GetKey(m_rootIndexAddress, m_rootIndexLevel, key);
        }

        #endregion

    }
}
