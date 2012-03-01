//******************************************************************************************************
//  BPlusTree_Add.cs - Gbtc
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

namespace openHistorian.Core.StorageSystem.Generic
{
    public partial class BPlusTree<TKey, TValue>
    {
        #region [ Methods ]

        void AddItem(TKey key, TValue value, ref uint nodeIndex, ref byte nodeLevel)
        {
            uint currentNodeIndex = nodeIndex;
            byte currentNodeLevel = nodeLevel;

            m_cache.GetCachedMatch(ref currentNodeLevel, ref currentNodeIndex, key);

            for (byte levelCount = currentNodeLevel; levelCount > 0; levelCount--)
            {
                currentNodeIndex = InternalNodeGetNodeIndex(key, currentNodeIndex, levelCount);
            }

            InsertResults results = LeafNodeTryInsertKey(key, value, currentNodeIndex);
            if (results == InsertResults.InsertedOK)
                return;
            if (results == InsertResults.DuplicateKeyError)
                throw new Exception("Key already exists");

            AddItemSplitIsRequired(key, value, ref nodeIndex, ref nodeLevel);
        }

        void AddItemSplitIsRequired(TKey key, TValue value, ref uint nodeIndex, ref byte nodeLevel)
        {
            SplitDetails split = AddItemSplitIsRequiredRecursive(key, value, nodeIndex, nodeLevel);
            //if the highest layer requires a split, a new root must be created.
            if (split.IsSplit)
            {
                nodeLevel += 1;
                nodeIndex = InternalNodeCreateEmptyNode(nodeLevel, split.LesserIndex, split.Key, split.GreaterIndex);
                m_cache.ClearCache(nodeLevel);
            }
        }

        SplitDetails AddItemSplitIsRequiredRecursive(TKey key, TValue value, uint currentNodeIndex, byte nodeLevel)
        {
            if (nodeLevel > 0)
            {
                uint childIndex = InternalNodeGetNodeIndex(key, currentNodeIndex,nodeLevel);

                SplitDetails split = AddItemSplitIsRequiredRecursive(key, value, childIndex, (byte) (nodeLevel-1));
                
                //if the child was split, add the new split item to this index with the option of splitting this index also
                if (split.IsSplit)
                {
                    return AddToInternalNodeSplitIfRequired(split.Key, split.GreaterIndex, currentNodeIndex, nodeLevel);
                }
                return default(SplitDetails);
            }
            else
            {
                return AddToLeafNodeSplitIfRequired(key, value, currentNodeIndex);
            }
        }

        SplitDetails AddToInternalNodeSplitIfRequired(TKey key, uint indexToAdd, uint currentNodeIndex, byte nodeLevel)
        {
            SplitDetails split = default(SplitDetails);
            NodeHeader node = new NodeHeader(m_stream, m_blockSize, currentNodeIndex);
            if (node.Level != nodeLevel)
                throw new Exception();

            InsertResults results = InternalNodeTryInsertKey(key, indexToAdd, currentNodeIndex,nodeLevel);
            if (results == InsertResults.InsertedOK)
            {
                m_cache.ClearCache(node.Level);
                return split;
            }
            if (results == InsertResults.DuplicateKeyError)
                throw new Exception("Key already exists");

            split.IsSplit = true;
            split.LesserIndex = currentNodeIndex;

            InternalNodeSplitNode(split.LesserIndex, out split.GreaterIndex, out split.Key,nodeLevel);

            if (key.CompareTo(split.Key) >= 0)//(key >= split.Key)
            {
                InternalNodeTryInsertKey(key, indexToAdd, split.GreaterIndex, nodeLevel);
            }
            else
            {
                InternalNodeTryInsertKey(key, indexToAdd, split.LesserIndex, nodeLevel);
            }
            m_cache.ClearCache(node.Level);
            return split;
        }

        /// <summary>
        /// If a node split is required, this recursive function will need to be called to accomplish the split.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="currentNodeIndex"></param>
        /// <returns>The details of the split if it was acomplished.</returns>
        SplitDetails AddToLeafNodeSplitIfRequired(TKey key, TValue value, uint currentNodeIndex)
        {
            SplitDetails split = default(SplitDetails);
            NodeHeader node = new NodeHeader(m_stream, m_blockSize, currentNodeIndex);
            if (node.Level != 0)
                throw new Exception();

            InsertResults results = LeafNodeTryInsertKey(key, value, currentNodeIndex);
            if (results == InsertResults.InsertedOK)
            {
                return split;
            }
            if (results == InsertResults.DuplicateKeyError)
                throw new Exception("Key already exists");

            //A split is required
            split.IsSplit = true;
            split.LesserIndex = currentNodeIndex;
            LeafNodeSplitNode(split.LesserIndex, out split.GreaterIndex, out split.Key);
            
            //Add the data after the split
            if (key.CompareTo(split.Key) >= 0)//(key >= split.Key)
            {
                LeafNodeTryInsertKey(key, value, split.GreaterIndex);
            }
            else
            {
                LeafNodeTryInsertKey(key, value, split.LesserIndex);
            }
            return split;
        }

        #endregion

    }
}
