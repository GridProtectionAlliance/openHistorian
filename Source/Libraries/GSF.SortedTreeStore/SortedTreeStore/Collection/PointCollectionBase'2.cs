//******************************************************************************************************
//  PointCollectionBase`2.cs - Gbtc
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
//  2/5/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore
{
    public abstract class PointCollectionBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        public byte[] RawData;
        public int DequeuePosition;
        public int EnqueuePosition;
        public int PointSize { get; private set; }
        public int KeySize { get; private set; }
        public int ValueSize { get; private set; }

        public bool ContainsPoints
        {
            get
            {
                return DequeuePosition != EnqueuePosition;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return DequeuePosition == EnqueuePosition;
            }
        }

        public bool IsFull
        {
            get
            {
                return RawData.Length == EnqueuePosition;
            }
        }


        protected void Initialize(int capacity, int keySize, int valueSize)
        {
            PointSize = keySize + valueSize;
            KeySize = keySize;
            ValueSize = valueSize;
            RawData = new byte[capacity * PointSize];
        }

        public void Clear()
        {
            DequeuePosition = 0;
            EnqueuePosition = 0;
        }

        public abstract void UnDequeue(TKey key, TValue value);

        public void Peek(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public abstract void Peek(TKey key);

        public void Dequeue()
        {
            if (IsEmpty)
                throw new Exception();
            DequeuePosition += PointSize;
            if (IsEmpty)
                Clear();
        }

        public abstract void Enqueue(TKey key, TValue value);

        public abstract void Dequeue(TKey key, TValue value);

        unsafe public abstract void Enqueue(byte* keyValue);

        public abstract int CompareTo(PointCollectionBase<TKey, TValue> other);

        public abstract int CompareTo(TKey other);

        public abstract bool CopyToWhileLessThan(PointCollectionBase<TKey, TValue> destination, TKey comparer);

        public abstract bool CopyToIfLessThan(PointCollectionBase<TKey, TValue> destination, TKey comparer);

        public abstract void CopyTo(PointCollectionBase<TKey, TValue> destination);

    }
}
