//******************************************************************************************************
//  BPlusTreeAdd.cs - Gbtc
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
//  2/18/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;

namespace openHistorian.Core.StorageSystem.Generic
{
    /// <summary>
    /// This class only concerns itself with the Add requirement of the B+ Tree.
    /// </summary>
    public partial class BPlusTree<TKey, TValue>
    {
        #region [ Methods ]

        void AddItem(TKey key, TValue value, ref uint nodeIndex, ref byte nodeLevel)
        {
            uint currentNodeIndex = nodeIndex;
            byte currentNodeLevel = nodeLevel;

            if (nodeLevel > 0)
            {
                for (byte x = nodeLevel; x > 0; x--)
                {
                    if (cache[x - 1].IsMatch(key))
                    {
                        currentNodeIndex = cache[x - 1].Bucket;
                        currentNodeLevel = (byte)(x - 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            for (int levelCount = currentNodeLevel; levelCount > 0; levelCount--)
            {
#if DEBUG
                Stream.Position = (currentNodeIndex * BlockSize);
                if (Stream.ReadByte() != levelCount)
                    throw new Exception("Node levels corrupt");
#endif
                Stream.Position = currentNodeIndex * BlockSize;
                currentNodeIndex = InternalNodeGetNodeIndex(key);
            }


#if DEBUG
            Stream.Position = (currentNodeIndex * BlockSize);
            if (Stream.ReadByte() != 0)
                throw new Exception("Node levels corrupt");
#endif

            Stream.Position = currentNodeIndex * BlockSize;
            InsertResults results = LeafNodeTryInsertKey(key, value);
            if (results == InsertResults.InsertedOK)
                return;
            if (results == InsertResults.DuplicateKeyError)
                throw new Exception("Key already exists");

            AddItemSplitIsRequired(key, value, ref nodeIndex, ref nodeLevel);
            
        }

        void AddItemSplitIsRequired(TKey key, TValue value, ref uint nodeIndex, ref byte nodeLevel)
        {
            SplitDetails split = AddItemSplitIsRequiredRecursive(key, value, nodeIndex);
            //if the highest layer requires a split, a new root must be created.
            if (split.IsSplit)
            {
                nodeLevel += 1;
                nodeIndex = InternalNodeCreateEmptyNode(nodeLevel, split.LesserIndex, split.Key, split.GreaterIndex);
                ClearCache(nodeLevel);
            }
        }

        SplitDetails AddItemSplitIsRequiredRecursive(TKey key, TValue value, uint currentNodeIndex)
        {
            NodeHeader node = new NodeHeader(Stream, BlockSize, currentNodeIndex);
            if (node.Level > 0)
            {
                //Get the child and call this function recursively
                Stream.Position = currentNodeIndex * BlockSize;
               
                uint childIndex = InternalNodeGetNodeIndex(key);
                
                SplitDetails split = AddItemSplitIsRequiredRecursive(key, value, childIndex);
                
                //if the child was split, add the new split item to this index with the option of splitting this index also
                if (split.IsSplit)
                {
                    return AddToInternalNodeSplitIfRequired(split.Key, split.GreaterIndex, currentNodeIndex);
                }
                return default(SplitDetails);
            }
            else
            {
                return AddToLeafNodeSplitIfRequired(key, value, currentNodeIndex);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="indexToAdd"></param>
        /// <param name="currentNodeIndex"></param>
        /// <returns>The details of the split if it was acomplished.</returns>
        SplitDetails AddToInternalNodeSplitIfRequired(TKey key, uint indexToAdd, uint currentNodeIndex)
        {

            SplitDetails split = default(SplitDetails);
            NodeHeader node = new NodeHeader(Stream, BlockSize, currentNodeIndex);
            if (node.Level == 0)
                throw new Exception();

            InsertResults results = InternalNodeTryInsertKey(key, indexToAdd, currentNodeIndex);
            if (results == InsertResults.InsertedOK)
            {
                ClearCache(node.Level);
                return split;
            }
            if (results == InsertResults.DuplicateKeyError)
                throw new Exception("Key already exists");

            split.IsSplit = true;
            split.LesserIndex = currentNodeIndex;

            InternalNodeSplitNode(split.LesserIndex, out split.GreaterIndex, out split.Key);

            if (key.CompareTo(split.Key) >= 0)//(key >= split.Key)
            {
                InternalNodeTryInsertKey(key, indexToAdd, split.GreaterIndex);
            }
            else
            {
                InternalNodeTryInsertKey(key, indexToAdd, split.LesserIndex);
            }
            ClearCache(node.Level);
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
            NodeHeader node = new NodeHeader(Stream, BlockSize, currentNodeIndex);
            if (node.Level != 0)
                throw new Exception();

            Stream.Position = currentNodeIndex * BlockSize;
            InsertResults results = LeafNodeTryInsertKey(key, value);
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
                Stream.Position = split.GreaterIndex * BlockSize;
                LeafNodeTryInsertKey(key, value);
            }
            else
            {
                Stream.Position = split.LesserIndex * BlockSize;
                LeafNodeTryInsertKey(key, value);
            }
            return split;
        }

        #endregion

    }
}
