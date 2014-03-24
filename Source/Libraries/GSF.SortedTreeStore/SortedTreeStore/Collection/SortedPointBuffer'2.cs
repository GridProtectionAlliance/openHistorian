//******************************************************************************************************
//  SortedPointBuffer`2.cs - Gbtc
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
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Collection
{
    public class SortedPointBuffer<TKey, TValue>
        : TreeStream<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        TKey m_tmpKey;
        TValue m_tmpValue;

        int[] SortingBlocks1;
        int[] SortingBlocks2;

        public byte[] KeyData;
        public byte[] ValueData;

        public int Capacity;

        public int DequeueIndex;
        public int EnqueueIndex;

        public int KeySize;
        public int ValueSize;

        public int PointSize { get; private set; }

        public int Count
        {
            get
            {
                return (EnqueueIndex - DequeueIndex);
            }
        }

        DoubleValueEncodingBase<TKey, TValue> m_encoding;

        public SortedPointBuffer(int capacity)
        {
            Capacity = capacity;
            m_tmpKey = new TKey();
            m_tmpValue = new TValue();

            KeySize = m_tmpKey.Size;
            ValueSize = m_tmpValue.Size;

            m_encoding = EncodingLibrary.GetEncodingMethod<TKey, TValue>(SortedTree.FixedSizeNode);

            PointSize = m_encoding.MaxCompressionSize;

            KeyData = new byte[capacity * KeySize];
            ValueData = new byte[capacity * ValueSize];

            SortingBlocks1 = new int[capacity];
            SortingBlocks2 = new int[capacity];
        }

        public bool ContainsPoints
        {
            get
            {
                return DequeueIndex != EnqueueIndex;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return DequeueIndex == EnqueueIndex;
            }
        }

        public bool IsFull
        {
            get
            {
                return Capacity == EnqueueIndex;
            }
        }

        public void Clear()
        {
            DequeueIndex = 0;
            EnqueueIndex = 0;
            EOS = false;
        }

        unsafe public bool TryEnqueue(TKey key, TValue value)
        {
            if (IsFull)
                return false;
            fixed (byte* lpk = KeyData, lpv = ValueData)
            {
                key.Write(lpk + EnqueueIndex * KeySize);
                value.Write(lpv + EnqueueIndex * ValueSize);
                EnqueueIndex++;
            }
            return true;
        }

        public unsafe override bool Read(TKey key, TValue value)
        {
            if (IsEmpty)
                return false;
            ReadSorted(DequeueIndex,key,value);
            DequeueIndex++;
            if (IsEmpty)
                Clear();
            return true;
        }

        public unsafe void ReadSorted(int index, TKey key, TValue value)
        {
            fixed (byte* lpk = KeyData, lpv = ValueData)
            {
                key.Read(lpk + SortingBlocks1[index] * KeySize);
                value.Read(lpv + SortingBlocks1[index] * ValueSize);
            }
        }

        public unsafe void Sort()
        {
            fixed (byte* lp = KeyData)
            {
                //InitialSort
                int keySize = KeySize;
                int count = Count;

                for (int x = 0; x < count; x += 2)
                {
                    //Can't sort the last entry if not
                    if (x + 1 == count)
                    {
                        SortingBlocks1[x] = x;
                    }
                    else if (m_tmpKey.IsLessThanOrEqualTo(lp + keySize * x, lp + keySize * (x + 1)))
                    {
                        SortingBlocks1[x] = x;
                        SortingBlocks1[x + 1] = (x + 1);
                    }
                    else
                    {
                        SortingBlocks1[x] = (x + 1);
                        SortingBlocks1[x + 1] = x;
                    }
                }

                bool shouldSwap = false;

                fixed (int* block1 = SortingBlocks1, block2 = SortingBlocks2)
                {
                    int stride = 2;
                    while (true)
                    {
                        if (stride >= count)
                            break;

                        shouldSwap = true;
                        SortLevel(block1, block2, lp, count, stride, keySize);
                        stride *= 2;

                        if (stride >= count)
                            break;

                        shouldSwap = false;
                        SortLevel(block2, block1, lp, count, stride, keySize);
                        stride *= 2;
                    }
                }

                if (shouldSwap)
                {
                    var b1 = SortingBlocks1;
                    SortingBlocks1 = SortingBlocks2;
                    SortingBlocks2 = b1;
                }
            }
        }


        /// <summary>
        /// Does a merge sort on the provided level.
        /// </summary>
        /// <param name="srcIndex">where the current indexes exist</param>
        /// <param name="dstIndex">where the final indexes should go</param>
        /// <param name="ptr">the data</param>
        /// <param name="count">the number of entries at this level</param>
        /// <param name="stride">the number of compares per level</param>
        /// <param name="keySize">the size of the key</param>
        unsafe void SortLevel(int* srcIndex, int* dstIndex, byte* ptr, int count, int stride, int keySize)
        {
            for (int xStart = 0; xStart < count; xStart += stride + stride)
            {
                int d = xStart;
                int dEnd = Math.Min(xStart + stride + stride, count);
                int i1 = xStart;
                int i1End = Math.Min(xStart + stride, count);
                int i2 = Math.Min(xStart + stride, count);
                int i2End = Math.Min(xStart + stride + stride, count);

                if (d != dEnd && i1 != i1End && i2 != i2End)
                {
                    //Check to see if already in order, then I can shortcut

                    if (m_tmpKey.IsLessThanOrEqualTo(ptr + srcIndex[i1End - 1] * keySize, ptr + srcIndex[i2] * keySize))
                    {
                        for (int i = d; i < dEnd; i++)
                        {
                            dstIndex[i] = srcIndex[i];
                        }
                        continue;
                    }
                }

                while (d < dEnd)
                {
                    if (i1 == i1End)
                    {
                        dstIndex[d] = srcIndex[i2];
                        d++;
                        i2++;
                    }
                    else if (i2 == i2End)
                    {
                        dstIndex[d] = srcIndex[i1];
                        d++;
                        i1++;
                    }
                    else if (m_tmpKey.IsLessThanOrEqualTo(ptr + srcIndex[i1] * keySize, ptr + srcIndex[i2] * keySize))
                    {
                        dstIndex[d] = srcIndex[i1];
                        d++;
                        i1++;
                    }
                    else
                    {
                        dstIndex[d] = srcIndex[i2];
                        d++;
                        i2++;
                    }
                }
            }
        }
    }
}
