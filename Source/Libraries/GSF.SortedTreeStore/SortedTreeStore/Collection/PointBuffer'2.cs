//******************************************************************************************************
//  PointBuffer`2.cs - Gbtc
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
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Collection
{
    public class PointBuffer<TKey, TValue>
        : TreeStream<TKey,TValue> 
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        TKey m_tmpKey;
        TValue m_tmpValue;

        public byte[] RawData;
        public int DequeuePosition;
        public int EnqueuePosition;

        public int PointSize { get; private set; }

        public int Count
        {
            get
            {
                return (EnqueuePosition - DequeuePosition) / PointSize;
            }
        }

        DoubleValueEncodingBase<TKey, TValue> m_encoding;

        public PointBuffer(int capacity)
        {
            m_tmpKey = new TKey();
            m_tmpValue = new TValue();
            m_encoding = EncodingLibrary.GetEncodingMethod<TKey, TValue>(SortedTree.FixedSizeNode);
            PointSize = m_encoding.MaxCompressionSize;
            RawData = new byte[capacity * PointSize];
        }

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

        public void Clear()
        {
            DequeuePosition = 0;
            EnqueuePosition = 0;
            EOS = false;
        }

        unsafe public bool TryEnqueue(TKey key, TValue value)
        {
            if (IsFull)
                return false;
            fixed (byte* lp = RawData)
            {
                m_encoding.Encode(lp + EnqueuePosition, null, null, key, value);
                EnqueuePosition += PointSize;
            }
            return true;
        }
        
        unsafe public void Dequeue(TKey key, TValue value)
        {
            bool endOfStream;
            if (IsEmpty)
                throw new Exception();
            fixed (byte* lp = RawData)
            {
                m_encoding.Decode(lp + DequeuePosition, null, null, key, value, out endOfStream);
                DequeuePosition += PointSize;
            }
            if (IsEmpty)
                Clear();
        }

        public unsafe override bool Read(TKey key, TValue value)
        {
            bool endOfStream;
            if (IsEmpty)
                return false;
            fixed (byte* lp = RawData)
            {
                m_encoding.Decode(lp + DequeuePosition, null, null, key, value, out endOfStream);
                DequeuePosition += PointSize;
            }
            if (IsEmpty)
                Clear();
            return true;
        }
    }
}
