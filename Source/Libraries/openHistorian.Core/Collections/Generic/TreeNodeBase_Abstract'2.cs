//******************************************************************************************************
//  TreeNodeBase_Abstract.cs - Gbtc
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

namespace openHistorian.Collections.Generic
{
    public partial class TreeNodeBase<TKey, TValue>
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

        protected abstract int GetIndexOf(TKey key);

        protected abstract void Split(uint newNodeIndex, TKey dividingKey);

        protected abstract void TransferRecordsFromRightToLeft(Node<TKey> left, Node<TKey> right, int bytesToTransfer);

        protected abstract void TransferRecordsFromLeftToRight(Node<TKey> left, Node<TKey> right, int bytesToTransfer);
    }
}