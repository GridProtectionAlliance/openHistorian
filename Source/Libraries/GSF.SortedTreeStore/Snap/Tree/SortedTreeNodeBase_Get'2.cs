//******************************************************************************************************
//  SortedTreeNodeBase_Get`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  04/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace GSF.Snap.Tree
{
    public partial class SortedTreeNodeBase<TKey, TValue>
    {
        //private TKey[] m_keys;
        //private TValue[] m_values;

        //public TKey[] Keys
        //{
        //    get
        //    {
        //        BuildKeyList();
        //        return m_keys;
        //    }
        //}

        //public TValue[] Values
        //{
        //    get
        //    {
        //        BuildKeyList();
        //        return m_values;
        //    }
        //}

        //public void WriteNodeData(StringBuilder sb)
        //{
        //    BuildKeyList();
        //    sb.AppendLine(string.Format("Node Index: {0} Record Count: {1} Node Level: {2} " +
        //                                "Right Sibling: {3} Left Sibling: {4} Lower Key: {5} Upper Key: {6}", NodeIndex, RecordCount,
        //                                Level, RightSiblingNodeIndex, LeftSiblingNodeIndex, LowerKey, UpperKey));

        //    foreach (TKey key in m_keys)
        //    {
        //        sb.Append(key.ToString());
        //        sb.Append("\t");
        //    }
        //    sb.AppendLine();

        //    foreach (TValue value in m_values)
        //    {
        //        sb.Append(value.ToString());
        //        sb.Append("\t");
        //    }
        //    sb.AppendLine();
        //}

        //private void BuildKeyList()
        //{
        //    if (m_keys is null || m_keys.Length != RecordCount)
        //    {
        //        m_keys = new TKey[RecordCount];
        //        m_values = new TValue[RecordCount];

        //        for (int x = 0; x < RecordCount; x++)
        //        {
        //            m_keys[x] = new TKey();
        //            m_values[x] = new TValue();
        //        }
        //    }
        //    for (int x = 0; x < RecordCount; x++)
        //    {
        //        Read(x, m_keys[x], m_values[x]);
        //    }
        //}

        #region [ Methods ]

        /// <summary>
        /// Seeks to the first node at this level of the tree
        /// </summary>
        public void SeekToFirstNode()
        {
            //Only need to do a seek if I'm not already on the first node.
            if (NodeIndex == uint.MaxValue || LeftSiblingNodeIndex != uint.MaxValue)
            {
                SetNodeIndex(SparseIndex.GetFirstIndex(Level));
            }
        }
        /// <summary>
        /// Seeks to the last node at this level of the tree
        /// </summary>
        public void SeekToLastNode()
        {
            //Only need to do a seek if I'm not already on the last node.
            if (NodeIndex == uint.MaxValue || RightSiblingNodeIndex != uint.MaxValue)
            {
                SetNodeIndex(SparseIndex.GetLastIndex(Level));
            }
        }

        /// <summary>
        /// Gets the first record contained in the current node of the tree.
        /// </summary>
        /// <param name="key">where to write the key</param>
        /// <param name="value">where to write the value</param>
        /// <returns>True if a value was found. False if the tree is empty</returns>
        public bool TryGetFirstRecord(TKey key, TValue value)
        {
            if (NodeIndex == uint.MaxValue)
                throw new Exception("Current node is null");
            if (RecordCount == 0)
                return false;
            Read(0, key, value);
            return true;
        }

        /// <summary>
        /// Gets the first record contained in the current node of the tree.
        /// </summary>
        /// <param name="value">where to write the value</param>
        /// <returns>True if a value was found. False if the tree is empty</returns>
        public bool TryGetFirstRecord(TValue value)
        {
            if (NodeIndex == uint.MaxValue)
                throw new Exception("Current node is null");
            if (RecordCount == 0)
                return false;
            Read(0, value);
            return true;
        }

        /// <summary>
        /// Gets the last record contained in the current node of the tree.
        /// </summary>
        /// <param name="key">where to write the key</param>
        /// <param name="value">where to write the value</param>
        /// <returns>True if a value was found. False if the tree is empty</returns>
        public bool TryGetLastRecord(TKey key, TValue value)
        {
            if (NodeIndex == uint.MaxValue)
                throw new Exception("Current node is null");
            if (RecordCount == 0)
                return false;
            Read(RecordCount - 1, key, value);
            return true;
        }

        /// <summary>
        /// Gets the last record contained in the current node of the tree.
        /// </summary>
        /// <param name="value">where to write the value</param>
        /// <returns>True if a value was found. False if the tree is empty</returns>
        public bool TryGetLastRecord(TValue value)
        {
            if (NodeIndex == uint.MaxValue)
                throw new Exception("Current node is null");
            if (RecordCount == 0)
                return false;
            Read(RecordCount - 1, value);
            return true;
        }

        /// <summary>
        /// Gets the provided key or the key that is directly to the right of this key.
        /// </summary>
        /// <param name="key">the key to find. This value is not modifed</param>
        /// <param name="value">where to write the value.</param>
        public virtual void GetOrGetNext(TKey key, TValue value)
        {
            NavigateToNode(key);
            int index = GetIndexOf(key);
            if (index < 0)
            {
                index = ~index - 1;
                if (index == -1)
                {
                    //This case occurs if the search position is the beginning of the array. This is possible since the 
                    //node lower bouds does not have to point to the first key. In which case, I have to seek to the previous sibling. 
                    SeekToLeftSibling();
                    if (!TryGetLastRecord(value))
                        throw new KeyNotFoundException("There is no value");
                    return;
                }
            }
            Read(index, value);
        }

        /// <summary>
        /// Gets the value for the provided key if it exists.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="value">where to write the value if found.</param>
        /// <returns>True if the value exists, False if not found.</returns>
        public virtual bool TryGet(TKey key, TValue value)
        {
            if (key.IsBetween(LowerKey, UpperKey))
            {
                int index = GetIndexOf(key);
                if (index < 0)
                    return false;

                Read(index, value);
                return true;
            }
            return TryGet2(key, value);
        }

        /// <summary>
        /// Gets the value for the provided key if it exists.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="value">where to write the value if found.</param>
        /// <returns>True if the value exists, False if not found.</returns>
        /// <remarks>
        /// This is a slower but more complete implementation of <see cref="TryGet"/>.
        /// Overriding classes can call this method after implementing their own high speed TryGet method.
        /// </remarks>
        protected virtual bool TryGet2(TKey key, TValue value)
        {
            NavigateToNode(key);
            int index = GetIndexOf(key);
            if (index < 0)
                return false;
            Read(index, value);
            return true;
        }

        #endregion
    }
}