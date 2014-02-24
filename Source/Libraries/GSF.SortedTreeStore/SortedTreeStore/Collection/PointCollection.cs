//******************************************************************************************************
//  PointCollection`2.cs - Gbtc
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
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Collection
{
    unsafe public class PointCollection<TKey, TValue>
        : PointCollectionBase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        TKey m_tmpKey1;
        TKey m_tmpKey2;
        TValue m_tmpValue;

        public PointCollection(int capacity)
        {
            m_tmpKey1 = new TKey();
            m_tmpKey2 = new TKey();
            m_tmpValue = new TValue();
            Initialize(capacity, m_tmpKey1.Size, m_tmpValue.Size);
        }

        public override void UnDequeue(TKey key, TValue value)
        {
            if (DequeuePosition == 0)
            {
                if (IsEmpty)
                    Enqueue(key, value);
                else
                    throw new Exception();
                return;
            }
            fixed (byte* lp = &RawData[DequeuePosition - 48])
            {
                key.Write(lp);
                value.Write(lp + KeySize);
                DequeuePosition -= 48;
            }
        }

        public override void Peek(TKey key)
        {
            if (IsEmpty)
                throw new Exception();
            fixed (byte* lp = RawData)
            {
                key.Read(lp + DequeuePosition);
            }
        }

        unsafe public override void Enqueue(TKey key, TValue value)
        {
            if (IsFull)
                throw new Exception();
            fixed (byte* lp = RawData)
            {
                key.Write(lp + EnqueuePosition);
                value.Write(lp + EnqueuePosition + KeySize);
                EnqueuePosition += PointSize;
            }
        }

        unsafe public override void Dequeue(TKey key, TValue value)
        {
            if (IsEmpty)
                throw new Exception();
            fixed (byte* lp = RawData)
            {
                key.Read(lp + DequeuePosition);
                value.Read(lp + DequeuePosition + KeySize);
                DequeuePosition += PointSize;
            }
            if (IsEmpty)
                Clear();
        }

        public override unsafe void Enqueue(byte* keyValue)
        {
            if (IsFull)
                throw new Exception();
            fixed (byte* lp = RawData)
            {
                m_tmpKey1.MethodCopy(keyValue, lp + EnqueuePosition);
                m_tmpValue.MethodCopy(keyValue + KeySize, lp + EnqueuePosition + KeySize);
                EnqueuePosition += PointSize;
            }
        }

        public override int CompareTo(PointCollectionBase<TKey, TValue> other)
        {
            if (IsEmpty && other.IsEmpty)
                return 0;
            if (IsEmpty)
                return 1;
            if (other.IsEmpty)
                return -1;
            fixed (byte* left = &RawData[DequeuePosition])
            fixed (byte* right = &other.RawData[other.DequeuePosition])
            {
                m_tmpKey1.Read(left);
                m_tmpKey2.Read(right);
                return m_tmpKey1.CompareTo(m_tmpKey2);
            }
        }

        public override int CompareTo(TKey other)
        {
            if (IsEmpty)
                return 1;
            fixed (byte* left = &RawData[DequeuePosition])
            {
                m_tmpKey1.Read(left);
                return m_tmpKey1.CompareTo(other);
            }
        }

        public override bool CopyToWhileLessThan(PointCollectionBase<TKey, TValue> destination, TKey comparer)
        {
            TryAgain:
            if (IsEmpty)
            {
                Clear();
                return false;
            }
            if (destination.IsFull)
                return true;

            if (CompareTo(comparer) < 0)
            {
                CopyTo(destination);
                goto TryAgain;
            }
            return false;
        }

        public override bool CopyToIfLessThan(PointCollectionBase<TKey, TValue> destination, TKey comparer)
        {
            if (IsEmpty)
                throw new Exception();
            if (destination.IsFull)
                throw new Exception();

            if (CompareTo(comparer) < 0)
            {
                CopyTo(destination);
                return true;
            }
            return false;
        }

        public override void CopyTo(PointCollectionBase<TKey, TValue> destination)
        {
            if (IsEmpty)
                throw new Exception();
            if (destination.IsFull)
                throw new Exception();

            Array.Copy(RawData, DequeuePosition, destination.RawData, destination.EnqueuePosition, PointSize);
            DequeuePosition += PointSize;
            destination.EnqueuePosition += PointSize;
            if (IsEmpty)
                Clear();
        }
    }
}
