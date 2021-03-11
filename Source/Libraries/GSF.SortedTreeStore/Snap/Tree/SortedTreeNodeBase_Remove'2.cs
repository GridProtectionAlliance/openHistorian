//******************************************************************************************************
//  SortedTreeNodeBase_Remove`2.cs - Gbtc
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
using GSF.Snap.Types;

namespace GSF.Snap.Tree
{
    public unsafe partial class SortedTreeNodeBase<TKey, TValue>
    {
        //ToDO: Checked
        /// <summary>
        /// Tries to remove the key from the Node.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryRemove(TKey key)
        {
            if (ValidBytes > m_minRecordNodeBytes &&
                LowerKey.IsLessThanOrEqualTo(key) &&
                key.IsLessThan(UpperKey))
            {
                int index = GetIndexOf(key);
                if (index < 0)
                    return false;

                if (RemoveUnlessOverflow(index))
                    return true;
            }
            return TryRemove2(key);
        }

        //ToDO: Checked
        protected bool TryRemove2(TKey key)
        {
            NavigateToNode(key);

            int index = GetIndexOf(key);
            if (index < 0)
                return false;

            if (!RemoveUnlessOverflow(index))
            {
                //ToDo:SplitAndThenRemove
                throw new NotImplementedException();
            }

            if (ValidBytes > m_minRecordNodeBytes) //if the node has not underflowed, we can exit early.
                return true;
            if (IsRightSiblingIndexNull && IsLeftSiblingIndexNull) //If there are no nodes to combine with, we can quit early.
                return true;
            if (RecordCount > 0 && (IsRightSiblingIndexNull || IsLeftSiblingIndexNull)) //There can be fewer than the minimum it is the first or last node on the level.
                return true;

            SparseIndex.CanCombineWithSiblings(LowerKey, (byte)(Level + 1), out bool canCombineWithLeft, out bool canCombineWithRight);


            if (RecordCount == 0) //Only will occur if the right or left node is empty (but not both)
            {
                if (canCombineWithLeft && IsRightSiblingIndexNull)
                    CombineNodes(LeftSiblingNodeIndex, NodeIndex);
                else if (canCombineWithRight && IsLeftSiblingIndexNull)
                    CombineNodes(NodeIndex, RightSiblingNodeIndex);
                else
                    throw new Exception("Should never reach this condition");
                return true;
            }


            int deltaBytesWhenCombining = MaxOverheadWithCombineNodes - HeaderSize;

            if (IsRightSiblingIndexNull) //We can only combine with the left node.
            {
                if (!canCombineWithLeft)
                    throw new Exception("Should never reach this condition");

                if (ValidBytes + GetValidBytes(LeftSiblingNodeIndex) + deltaBytesWhenCombining < BlockSize)
                    CombineNodes(LeftSiblingNodeIndex, NodeIndex);
                else
                    RebalanceNodes(LeftSiblingNodeIndex, NodeIndex);
            }
            else if (IsLeftSiblingIndexNull) //We can only combine with the right node.
            {
                if (!canCombineWithRight)
                    throw new Exception("Should never reach this condition");

                if (ValidBytes + GetValidBytes(RightSiblingNodeIndex) + deltaBytesWhenCombining < BlockSize)
                    CombineNodes(NodeIndex, RightSiblingNodeIndex);
                else
                    RebalanceNodes(NodeIndex, RightSiblingNodeIndex);
            }
            else //I can combine with the right or the left node
            {
                if (canCombineWithLeft && ValidBytes + GetValidBytes(LeftSiblingNodeIndex) + deltaBytesWhenCombining < BlockSize)
                    CombineNodes(LeftSiblingNodeIndex, NodeIndex);
                else if (canCombineWithRight && ValidBytes + GetValidBytes(RightSiblingNodeIndex) + deltaBytesWhenCombining < BlockSize)
                    CombineNodes(NodeIndex, RightSiblingNodeIndex);
                else if (canCombineWithLeft)
                    RebalanceNodes(LeftSiblingNodeIndex, NodeIndex);
                else if (canCombineWithRight)
                    RebalanceNodes(NodeIndex, RightSiblingNodeIndex);
                else
                    throw new Exception("Should never reach this condition");
            }
            return true;
        }

        //ToDO: Checked
        private void RebalanceNodes(uint leftNode, uint rightNode)
        {
            Clear();
            Node<TKey> left = m_tempNode1;
            Node<TKey> right = m_tempNode2;
            left.Clear();
            right.Clear();
            left.SetNodeIndex(leftNode);
            right.SetNodeIndex(rightNode);

            int averageSize = (left.ValidBytes + right.ValidBytes) >> 1;

            if (left.ValidBytes < right.ValidBytes)
            {
                //Transfer records from Right to Left
                TransferRecordsFromRightToLeft(left, right, averageSize - left.ValidBytes);

                UpdateBoundsOfNode(left, right);
            }
            else
            {
                //Transfer records from Left to Right
                TransferRecordsFromLeftToRight(left, right, averageSize - right.ValidBytes);

                UpdateBoundsOfNode(left, right);
            }
        }

        private void CombineNodes(uint leftNode, uint rightNode)
        {
            Clear();
            Node<TKey> left = m_tempNode1;
            Node<TKey> right = m_tempNode2;
            left.Clear();
            right.Clear();
            left.SetNodeIndex(leftNode);
            right.SetNodeIndex(rightNode);

            //Bug: Valid Bytes includes the header. Figure out why this does not cause a bug.
            if (left.RecordCount == 0)
            {
                TransferRecordsFromLeftToRight(left, right, left.ValidBytes);
                UpdateBoundsAndRemoveEmptyNode(left, right);
            }
            else if (right.RecordCount == 0)
            {
                TransferRecordsFromRightToLeft(left, right, right.ValidBytes);
                UpdateBoundsAndRemoveEmptyNode(left, right);
            }
            else
            {
                TransferRecordsFromRightToLeft(left, right, right.ValidBytes);
                UpdateBoundsAndRemoveEmptyNode(left, right);
            }
        }


        private void UpdateBoundsOfNode(Node<TKey> leftNode, Node<TKey> rightNode)
        {
            TKey oldLowerKey = new TKey();
            TKey newLowerKey = new TKey();

            rightNode.LowerKey.CopyTo(oldLowerKey);
            newLowerKey.Read(rightNode.GetReadPointerAfterHeader()); //ToDo: Make Generic
            rightNode.LowerKey = newLowerKey;
            leftNode.UpperKey = newLowerKey;

            SparseIndex.UpdateKey(oldLowerKey, newLowerKey, (byte)(Level + 1));
        }

        private void UpdateBoundsAndRemoveEmptyNode(Node<TKey> leftNode, Node<TKey> rightNode)
        {
            if (leftNode.RecordCount == 0) //Remove the left node
            {
                //Change the existing pointer to the left node to point to the right node.
                SparseIndex.UpdateValue(leftNode.LowerKey, new SnapUInt32(rightNode.NodeIndex), (byte)(Level + 1));
                //Now remove the unused key position
                SparseIndex.Remove(rightNode.LowerKey, (byte)(Level + 1));

                rightNode.LowerKey = leftNode.LowerKey;
                rightNode.LeftSiblingNodeIndex = leftNode.LeftSiblingNodeIndex;
                if (leftNode.LeftSiblingNodeIndex != uint.MaxValue)
                {
                    leftNode.SeekToLeftSibling();
                    leftNode.RightSiblingNodeIndex = rightNode.NodeIndex;
                }
            }
            else if (rightNode.RecordCount == 0) //Remove the right node.
            {
                SparseIndex.Remove(rightNode.LowerKey, (byte)(Level + 1));

                leftNode.UpperKey = rightNode.UpperKey;
                leftNode.RightSiblingNodeIndex = rightNode.RightSiblingNodeIndex;

                if (rightNode.RightSiblingNodeIndex != uint.MaxValue)
                {
                    rightNode.SeekToRightSibling();
                    rightNode.LeftSiblingNodeIndex = leftNode.NodeIndex;
                }
            }
            else
            {
                throw new Exception("Should never get here");
            }
        }
    }
}