//******************************************************************************************************
//  TreeNodeBase_Insert.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  4/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.Collections.Generic
{
    public partial class TreeNodeBase<TKey, TValue>
    {
        /// <summary>
        /// Inserts the following value to the tree if it does not exist.
        /// </summary>
        /// <param name="key">The key to add</param>
        /// <param name="value">The value to add</param>
        /// <returns>True if added, False on a duplicate key error</returns>
        public virtual bool TryInsert(TKey key, TValue value)
        {
            if (KeyMethods.IsBetween(LowerKey, key, UpperKey))
            {
                int index = ~GetIndexOf(key);
                if (index < 0)
                    return false;

                if (InsertUnlessFull(index, key, value))
                    return true;
            }
            return TryInsert2(key, value);
        }

        /// <summary>
        /// Inserts the following value to the tree if it does not exist.
        /// </summary>
        /// <param name="key">The key to add</param>
        /// <param name="value">The value to add</param>
        /// <returns>True if added, False on a duplicate key error</returns>
        /// <remarks>
        /// This is a slower but more complete implementation of <see cref="TryInsert"/>.
        /// Overriding classes can call this method after implementing their own high speed TryGet method.
        /// </remarks>
        protected virtual bool TryInsert2(TKey key, TValue value)
        {
            NavigateToNode(key);

            int index = ~GetIndexOf(key); //See the ~ here.
            if (index < 0)
                return false;

            if (InsertUnlessFull(index, key, value))
                return true;

            //Check if the node needs to be split
            if (IsRightSiblingIndexNull && index == RecordCount)
            {
                NewNodeThenInsert(key, value);
                return true;
            }
            else
            {
                SplitNodeThenInsert(key, value);
                return true;
            }
        }

        /// <summary>
        /// Creates an empty right sibling node and inserts the provided key in this node.
        /// Note: This should only be called if there is no right sibling and the key should go in
        /// that node.
        /// </summary>
        private void NewNodeThenInsert(TKey key, TValue value)
        {
            TKey dividingKey = new TKey(); //m_tempKey;
            KeyMethods.Copy(key, dividingKey);

            uint newNodeIndex = m_getNextNewNodeIndex();
            if (!IsRightSiblingIndexNull)
                throw new Exception("Incorrectly implemented");

            RightSiblingNodeIndex = newNodeIndex;

            CreateNewNode(newNodeIndex, 0, (ushort)HeaderSize, NodeIndex, uint.MaxValue, key, UpperKey);

            UpperKey = key;

            SetNodeIndex(newNodeIndex);

            InsertUnlessFull(0, key, value);

            SparseIndex.Add(dividingKey, newNodeIndex, (byte)(Level + 1));
        }

        /// <summary>
        /// Splits the current node and then inserts the provided key/value into the correct node.
        /// </summary>
        private void SplitNodeThenInsert(TKey key, TValue value)
        {
            uint currentNode = NodeIndex;
            uint newNodeIndex = m_getNextNewNodeIndex();

            TKey dividingKey = new TKey(); //m_tempKey;

            Split(newNodeIndex, dividingKey);

            SetNodeIndex(currentNode);
            if (IsKeyInsideBounds(key))
            {
                InsertUnlessFull(~GetIndexOf(key), key, value);
                KeyMethods.Copy(UpperKey, dividingKey);
            }
            else
            {
                SeekToRightSibling();
                InsertUnlessFull(~GetIndexOf(key), key, value);
                KeyMethods.Copy(LowerKey, dividingKey);
            }

            SparseIndex.Add(dividingKey, newNodeIndex, (byte)(Level + 1));
        }
    }
}