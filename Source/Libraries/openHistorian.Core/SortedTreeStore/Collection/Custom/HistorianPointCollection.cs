//******************************************************************************************************
//  HistorianCompressedStream.cs - Gbtc
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
//  8/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.IO;
using openHistorian.Collections;
using GSF.SortedTreeStore.Net.Initialization;

namespace GSF.SortedTreeStore.Net.Compression
{
    public unsafe class HistorianPointCollection
        : PointCollectionBase<HistorianKey, HistorianValue>
    {
        public HistorianPointCollection(int capacity)
        {
            Initialize(capacity, 24, 24);
        }

        public override void UnDequeue(HistorianKey key, HistorianValue value)
        {
            if (DequeuePosition == 0)
            {
                if (IsEmpty)
                    Enqueue(key, value);
                else
                    throw new Exception();
                return;
            }
            fixed (byte* lp = RawData)
            {
                ulong* dst = (ulong*)(lp + DequeuePosition - 48);
                dst[0] = key.Timestamp;
                dst[1] = key.PointID;
                dst[2] = key.EntryNumber;
                dst[3] = value.Value1;
                dst[4] = value.Value2;
                dst[5] = value.Value3;
                DequeuePosition -= 48;
            }
        }

        public override void Peek(HistorianKey key)
        {
            if (IsEmpty)
                throw new Exception();
            fixed (byte* lp = RawData)
            {
                ulong* src = (ulong*)(lp + DequeuePosition);
                key.Timestamp = src[0];
                key.PointID = src[1];
                key.EntryNumber = src[2];
            }
        }

        public override void Enqueue(HistorianKey key, HistorianValue value)
        {
            if (IsFull)
                throw new Exception();
            fixed (byte* lp = RawData)
            {
                ulong* dst = (ulong*)(lp + EnqueuePosition);
                dst[0] = key.Timestamp;
                dst[1] = key.PointID;
                dst[2] = key.EntryNumber;
                dst[3] = value.Value1;
                dst[4] = value.Value2;
                dst[5] = value.Value3;
                EnqueuePosition += 48;
            }
        }

        public override void Dequeue(HistorianKey key, HistorianValue value)
        {
            if (IsEmpty)
                throw new Exception();
            fixed (byte* lp = RawData)
            {
                ulong* src = (ulong*)(lp + DequeuePosition);
                key.Timestamp = src[0];
                key.PointID = src[1];
                key.EntryNumber = src[2];
                value.Value1 = src[3];
                value.Value2 = src[4];
                value.Value3 = src[5];
                DequeuePosition += 48;
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
                ulong* dst = (ulong*)(lp + EnqueuePosition);
                ulong* src = (ulong*)(keyValue);
                dst[0] = src[0];
                dst[1] = src[1];
                dst[2] = src[2];
                dst[3] = src[3];
                dst[4] = src[4];
                dst[5] = src[5];
                EnqueuePosition += 48;
            }
        }

        public override int CompareTo(PointCollectionBase<HistorianKey, HistorianValue> other)
        {
            if (IsEmpty && other.IsEmpty)
                return 0;
            if (IsEmpty)
                return 1;
            if (other.IsEmpty)
                return -1;
            fixed (byte* leftPtr = &RawData[DequeuePosition])
            fixed (byte* rightPtr = &other.RawData[other.DequeuePosition])
            {
                ulong* left = (ulong*)leftPtr;
                ulong* right = (ulong*)rightPtr;

                if (left[0] < right[0])
                    return -1;
                if (left[0] > right[0])
                    return 1;
                if (left[1] < right[1])
                    return -1;
                if (left[1] > right[1])
                    return 1;
                if (left[2] < right[2])
                    return -1;
                if (left[2] > right[2])
                    return 1;
                return 0;
            }
        }

        public override int CompareTo(HistorianKey other)
        {
            if (IsEmpty)
                return 1;
            fixed (byte* leftPtr = &RawData[DequeuePosition])
            {
                ulong* left = (ulong*)leftPtr;
                if (left[0] < other.Timestamp)
                    return -1;
                if (left[0] > other.Timestamp)
                    return 1;
                if (left[1] < other.PointID)
                    return -1;
                if (left[1] > other.PointID)
                    return 1;
                if (left[2] < other.EntryNumber)
                    return -1;
                if (left[2] > other.EntryNumber)
                    return 1;
                return 0;
            }
        }

        /// <summary>
        /// Copies data from the destination while the key is less than the comparer
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="comparer"></param>
        /// <returns>
        /// False point about to be copied is greater than or equal to the comparer.
        /// </returns>
        public override bool CopyToWhileLessThan(PointCollectionBase<HistorianKey, HistorianValue> destination, HistorianKey comparer)
        {
            fixed (byte* srcPtr = &RawData[DequeuePosition])
            fixed (byte* dstPtr = &destination.RawData[destination.EnqueuePosition])
            {
                ulong* src = (ulong*)srcPtr;
                ulong* dst = (ulong*)dstPtr;

            TryAgain:

                if (IsEmpty)
                {
                    Clear();
                    return false;
                }
                if (destination.IsFull)
                    return false;

                if (src[0] > comparer.Timestamp)
                    return true;
                if (src[0] == comparer.Timestamp)
                {
                    if (src[1] > comparer.PointID)
                        return true;
                    if (src[1] == comparer.PointID)
                    {
                        if (src[2] >= comparer.EntryNumber)
                            return true;
                    }
                }

                dst[0] = src[0];
                dst[1] = src[1];
                dst[2] = src[2];
                dst[3] = src[3];
                dst[4] = src[4];
                dst[5] = src[5];

                DequeuePosition += 48;
                destination.EnqueuePosition += 48;
                dst += 6;
                src += 6;
                goto TryAgain;

            }
        }

        public override bool CopyToIfLessThan(PointCollectionBase<HistorianKey, HistorianValue> destination, HistorianKey comparer)
        {
            if (IsEmpty)
                throw new Exception();
            if (destination.IsFull)
                throw new Exception();

            fixed (byte* srcPtr = &RawData[DequeuePosition])
            fixed (byte* dstPtr = &destination.RawData[destination.EnqueuePosition])
            {
                ulong* src = (ulong*)srcPtr;
                ulong* dst = (ulong*)dstPtr;

                if (src[0] > comparer.Timestamp)
                    return false;
                if (src[0] == comparer.Timestamp)
                {
                    if (src[1] > comparer.PointID)
                        return false;
                    if (src[1] == comparer.PointID)
                    {
                        if (src[2] >= comparer.EntryNumber)
                            return false;
                    }
                }

                dst[0] = src[0];
                dst[1] = src[1];
                dst[2] = src[2];
                dst[3] = src[3];
                dst[4] = src[4];
                dst[5] = src[5];

                DequeuePosition += 48;
                destination.EnqueuePosition += 48;
            }
            if (IsEmpty)
                Clear();
            return true;
        }

        public override void CopyTo(PointCollectionBase<HistorianKey, HistorianValue> destination)
        {
            if (IsEmpty)
                throw new Exception();
            if (destination.IsFull)
                throw new Exception();

            fixed (byte* srcPtr = &RawData[DequeuePosition])
            fixed (byte* dstPtr = &destination.RawData[destination.EnqueuePosition])
            {
                ulong* src = (ulong*)srcPtr;
                ulong* dst = (ulong*)dstPtr;

                dst[0] = src[0];
                dst[1] = src[1];
                dst[2] = src[2];
                dst[3] = src[3];
                dst[4] = src[4];
                dst[5] = src[5];

                DequeuePosition += 48;
                destination.EnqueuePosition += 48;
            }
            if (IsEmpty)
                Clear();
        }
    }
}
