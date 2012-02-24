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

namespace Historian.StorageSystem.BlockSorter
{
    /// <summary>
    /// This class only concerns itself with the Add requirement of the B+ Tree.
    /// </summary>
    static class BPlusTreeAdd
    {
        #region [ Methods ]

        public static void AddItem(TreeHeader header, long key, long dataAddress, ref uint nodeIndex, ref byte nodeLevel)
        {
            NodeHeader node = default(NodeHeader);
            uint currentNodeIndex = nodeIndex;

            for (int levelCount = nodeLevel; levelCount > 0; levelCount--)
            {
                node.Load(header, currentNodeIndex);
                if (node.Level != levelCount)
                    throw new Exception("Node levels corrupt");

                currentNodeIndex = InternalNode.GetNodeIndex(header, key, currentNodeIndex);
            }

            node.Load(header, currentNodeIndex);
            if (node.Level != 0)
                throw new Exception("Node levels corrupt");

            InsertResults results = LeafNode.TryInsertKey(header, key, dataAddress, currentNodeIndex);
            if (results == InsertResults.InsertedOK)
                return;
            if (results == InsertResults.DuplicateKeyError)
                throw new Exception("Key already exists");

            AddItemSplitIsRequired(header, key, dataAddress, ref nodeIndex, ref nodeLevel);
        }

        static void AddItemSplitIsRequired(TreeHeader header, long key, long dataAddress, ref uint nodeIndex, ref byte nodeLevel)
        {
            SplitDetails split = AddItemSplitIsRequiredRecursive(header, key, dataAddress, nodeIndex);
            //if the highest layer requires a split, a new root must be created.
            if (split.IsSplit)
            {
                nodeLevel += 1;
                nodeIndex = InternalNode.CreateEmptyNode(header, nodeLevel, split.LesserIndex, split.Key, split.GreaterIndex);
            }
        }

        static SplitDetails AddItemSplitIsRequiredRecursive(TreeHeader header, long key, long dataAddress, uint currentNodeIndex)
        {
            NodeHeader node = new NodeHeader(header, currentNodeIndex);
            if (node.Level > 0)
            {
                //Get the child and call this function recursively
                uint childIndex = InternalNode.GetNodeIndex(header, key, currentNodeIndex);
                SplitDetails split = AddItemSplitIsRequiredRecursive(header, key, dataAddress, childIndex);
                
                //if the child was split, add the new split item to this index with the option of splitting this index also
                if (split.IsSplit)
                {
                    return AddToInternalNodeSplitIfRequired(header, split.Key, split.GreaterIndex, currentNodeIndex);
                }
                return default(SplitDetails);
            }
            else
            {
                return AddToLeafNodeSplitIfRequired(header, key, dataAddress, currentNodeIndex);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="header"></param>
        /// <param name="key"></param>
        /// <param name="indexToAdd"></param>
        /// <param name="currentNodeIndex"></param>
        /// <returns>The details of the split if it was acomplished.</returns>
        static SplitDetails AddToInternalNodeSplitIfRequired(TreeHeader header, long key, uint indexToAdd, uint currentNodeIndex)
        {
            SplitDetails split = default(SplitDetails);
            NodeHeader node = new NodeHeader(header, currentNodeIndex);
            if (node.Level == 0)
                throw new Exception();

            InsertResults results = InternalNode.TryInsertKey(header, key, indexToAdd, currentNodeIndex);
            if (results == InsertResults.InsertedOK)
            {
                return split;
            }
            if (results == InsertResults.DuplicateKeyError)
                throw new Exception("Key already exists");

            split.IsSplit = true;
            split.LesserIndex = currentNodeIndex;

            InternalNode.SplitNode(header, split.LesserIndex, out split.GreaterIndex, out split.Key);

            if (key >= split.Key)
            {
                InternalNode.TryInsertKey(header, key, indexToAdd, split.GreaterIndex);
            }
            else
            {
                InternalNode.TryInsertKey(header, key, indexToAdd, split.LesserIndex);
            }
            return split;
        }


        /// <summary>
        /// If a node split is required, this recursive function will need to be called to accomplish the split.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="key"></param>
        /// <param name="dataAddress"></param>
        /// <param name="currentNodeIndex"></param>
        /// <returns>The details of the split if it was acomplished.</returns>
        static SplitDetails AddToLeafNodeSplitIfRequired(TreeHeader header, long key, long dataAddress, uint currentNodeIndex)
        {
            SplitDetails split = default(SplitDetails);
            NodeHeader node = new NodeHeader(header, currentNodeIndex);
            if (node.Level != 0)
                throw new Exception();

            InsertResults results = LeafNode.TryInsertKey(header, key, dataAddress, currentNodeIndex);
            if (results == InsertResults.InsertedOK)
            {
                return split;
            }
            if (results == InsertResults.DuplicateKeyError)
                throw new Exception("Key already exists");

            //A split is required
            split.IsSplit = true;
            split.LesserIndex = currentNodeIndex;
            LeafNode.SplitNode(header, split.LesserIndex, out split.GreaterIndex, out split.Key);
            
            //Add the data after the split
            if (key >= split.Key)
            {
                LeafNode.TryInsertKey(header, key, dataAddress, split.GreaterIndex);
            }
            else
            {
                LeafNode.TryInsertKey(header, key, dataAddress, split.LesserIndex);
            }
            return split;
        }

        #endregion

    }
}
