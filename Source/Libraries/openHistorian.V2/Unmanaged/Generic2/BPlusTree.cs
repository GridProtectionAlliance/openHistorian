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
//  4/5/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System.Collections.Generic;
using openHistorian.V2.Unmanaged.Generic;

namespace openHistorian.V2.Unmanaged.Generic2
{
    /// <summary>
    /// Represents a collection of key/values pairs that is very similiar to a <see cref="SortedList{TKey,TValue}"/> 
    /// except it is optimal for storing millions to billions of entries and doing sequential scan of the data.
    /// Each type must be structs and implement <see cref="IValueType{TKey}"/>.
    /// </summary>
    /// <typeparam name="TKey">The unique key</typeparam>
    /// <typeparam name="TValue">The unique value</typeparam>
    public class BPlusTree<TKey, TValue> : BPlusTreeLeafNodeBase<TKey, TValue>
        where TKey : struct, IValueType<TKey>
        where TValue : struct, IValueType<TValue>
    {

        /// <summary>
        /// Loads an existing <see cref="BPlusTree{TKey,TValue}"/>
        /// from the provided stream.
        /// </summary>
        /// <param name="stream">The stream to load from</param>
        public BPlusTree(BinaryStream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Creates an empty <see cref="BPlusTree{TKey,TValue}"/> 
        /// and writes the data to the provided stream. 
        /// </summary>
        /// <param name="stream">The stream to use to store the tree.</param>
        /// <param name="blockSize">The size in bytes of a single block.</param>
        public BPlusTree(BinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
        }

        protected override void SaveValue(TValue value, BinaryStream stream)
        {
            value.SaveValue(stream);
        }

        protected override TValue LoadValue(BinaryStream stream)
        {
            TValue value = default(TValue);
            value.LoadValue(stream);
            return value;
        }

        protected override int SizeOfValue()
        {
            return default(TValue).SizeOf;
        }

        protected override int SizeOfKey()
        {
            return default(TKey).SizeOf;
        }

        protected override void SaveKey(TKey value, BinaryStream stream)
        {
            value.SaveValue(stream);
        }

        protected override TKey LoadKey(BinaryStream stream)
        {
            TKey value = default(TKey);
            value.LoadValue(stream);
            return value;
        }

        protected override int CompareKeys(TKey first, TKey last)
        {
            return first.CompareTo(last);
        }

        protected override int CompareKeys(TKey first, BinaryStream stream)
        {
            return first.CompareToStream(stream);
        }


    }
}
