//******************************************************************************************************
//  SortedTreeNodeBase_Abstract`2.cs - Gbtc
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

namespace GSF.Snap.Tree
{
    public partial class SortedTreeNodeBase<TKey, TValue>
    {
        protected abstract void InitializeType();

        protected abstract int MaxOverheadWithCombineNodes
        {
            get;
        }

        protected abstract void Read(int index, TValue value);

        protected abstract void Read(int index, TKey key, TValue value);

        protected abstract bool RemoveUnlessOverflow(int index);

        /// <summary>
        /// Inserts the provided key into the current node. 
        /// Note: A duplicate key has already been detected and will never be passed to this function.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract bool InsertUnlessFull(int index, TKey key, TValue value);

        /// <summary>
        /// Requests that the current stream is inserted into the tree. Sequentail insertion can only occur while the stream
        /// is in order and is entirely past the end of the tree. 
        /// </summary>
        /// <param name="stream">the stream data to insert</param>
        /// <param name="isFull">if returning from this function while the node is not yet full, this means the stream 
        /// can no longer be inserted sequentially and we must break out to the root and insert one at a time.</param>
        protected abstract void AppendSequentialStream(InsertStreamHelper<TKey, TValue> stream, out bool isFull);

        protected abstract int GetIndexOf(TKey key);

        protected abstract void Split(uint newNodeIndex, TKey dividingKey);

        protected abstract void TransferRecordsFromRightToLeft(Node<TKey> left, Node<TKey> right, int bytesToTransfer);

        protected abstract void TransferRecordsFromLeftToRight(Node<TKey> left, Node<TKey> right, int bytesToTransfer);
    }
}