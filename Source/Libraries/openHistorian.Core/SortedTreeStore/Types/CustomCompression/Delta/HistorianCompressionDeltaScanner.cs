//******************************************************************************************************
//  HistorianCompressionDeltaScanner.cs - Gbtc
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
//  7/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Filters;
using openHistorian.Collections;

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    /// <summary>
    /// A delta encoded scanner
    /// </summary>
    public unsafe class HistorianCompressionDeltaScanner
        : EncodedNodeScannerBase<HistorianKey, HistorianValue>
    {
        ulong m_prevTimestamp;
        ulong m_prevPointId;
        ulong m_prevEntryNumber;
        ulong m_prevValue1;
        ulong m_prevValue2;
        ulong m_prevValue3;
        int m_nextOffset;

        /// <summary>
        /// Creates a new class
        /// </summary>
        /// <param name="level"></param>
        /// <param name="blockSize"></param>
        /// <param name="stream"></param>
        /// <param name="lookupKey"></param>
        public HistorianCompressionDeltaScanner(byte level, int blockSize, BinaryStreamBase stream, Func<HistorianKey, byte, uint> lookupKey)
            : base(level, blockSize, stream, lookupKey, 2)
        {
        }

        protected override void InternalRead(HistorianKey key, HistorianValue value)
        {
            byte* stream = Pointer + m_nextOffset;
            int position = 0;
            key.Timestamp = m_prevTimestamp ^ Compression.Read7BitUInt64(stream, ref position);
            key.PointID = m_prevPointId ^ Compression.Read7BitUInt64(stream, ref position);
            key.EntryNumber = m_prevEntryNumber ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value1 = m_prevValue1 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value2 = m_prevValue2 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value3 = m_prevValue3 ^ Compression.Read7BitUInt64(stream, ref position);

            m_prevTimestamp = key.Timestamp;
            m_prevPointId = key.PointID;
            m_prevEntryNumber = key.EntryNumber;
            m_prevValue1 = value.Value1;
            m_prevValue2 = value.Value2;
            m_prevValue3 = value.Value3;
            m_nextOffset += position;
            IndexOfNextKeyValue++;
        }

        protected override unsafe bool InternalRead(HistorianKey key, HistorianValue value, KeyMatchFilterBase<HistorianKey> filter)
        {
        TryAgain:

            byte* stream = Pointer + m_nextOffset;
            int position = 0;
            key.Timestamp = m_prevTimestamp ^ Compression.Read7BitUInt64(stream, ref position);
            key.PointID = m_prevPointId ^ Compression.Read7BitUInt64(stream, ref position);
            key.EntryNumber = m_prevEntryNumber ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value1 = m_prevValue1 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value2 = m_prevValue2 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value3 = m_prevValue3 ^ Compression.Read7BitUInt64(stream, ref position);

            m_prevTimestamp = key.Timestamp;
            m_prevPointId = key.PointID;
            m_prevEntryNumber = key.EntryNumber;
            m_prevValue1 = value.Value1;
            m_prevValue2 = value.Value2;
            m_prevValue3 = value.Value3;
            m_nextOffset += position;
            IndexOfNextKeyValue++;

            if (filter.Contains(key))
                return true;
            if (IndexOfNextKeyValue >= RecordCount)
                return false;
            goto TryAgain;
        }

        protected override void InternalPeek(HistorianKey key, HistorianValue value)
        {
            byte* stream = Pointer + m_nextOffset;
            int position = 0;
            key.Timestamp = m_prevTimestamp ^ Compression.Read7BitUInt64(stream, ref position);
            key.PointID = m_prevPointId ^ Compression.Read7BitUInt64(stream, ref position);
            key.EntryNumber = m_prevEntryNumber ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value1 = m_prevValue1 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value2 = m_prevValue2 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value3 = m_prevValue3 ^ Compression.Read7BitUInt64(stream, ref position);
        }

        protected override unsafe bool InternalReadWhile(HistorianKey key, HistorianValue value, HistorianKey upperBounds)
        {
            byte* stream = Pointer + m_nextOffset;
            int position = 0;
            key.Timestamp = m_prevTimestamp ^ Compression.Read7BitUInt64(stream, ref position);
            key.PointID = m_prevPointId ^ Compression.Read7BitUInt64(stream, ref position);
            key.EntryNumber = m_prevEntryNumber ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value1 = m_prevValue1 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value2 = m_prevValue2 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value3 = m_prevValue3 ^ Compression.Read7BitUInt64(stream, ref position);

            if (KeyMethods.IsLessThan(key, upperBounds))
            {
                m_prevTimestamp = key.Timestamp;
                m_prevPointId = key.PointID;
                m_prevEntryNumber = key.EntryNumber;
                m_prevValue1 = value.Value1;
                m_prevValue2 = value.Value2;
                m_prevValue3 = value.Value3;
                m_nextOffset += position;
                IndexOfNextKeyValue++;
                return true;
            }
            return false;
        }

        protected override unsafe bool InternalReadWhile(HistorianKey key, HistorianValue value, HistorianKey upperBounds, KeyMatchFilterBase<HistorianKey> filter)
        {
        TryAgain:

            byte* stream = Pointer + m_nextOffset;
            int position = 0;
            key.Timestamp = m_prevTimestamp ^ Compression.Read7BitUInt64(stream, ref position);
            key.PointID = m_prevPointId ^ Compression.Read7BitUInt64(stream, ref position);
            key.EntryNumber = m_prevEntryNumber ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value1 = m_prevValue1 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value2 = m_prevValue2 ^ Compression.Read7BitUInt64(stream, ref position);
            value.Value3 = m_prevValue3 ^ Compression.Read7BitUInt64(stream, ref position);

            if (KeyMethods.IsLessThan(key, upperBounds))
            {
                m_prevTimestamp = key.Timestamp;
                m_prevPointId = key.PointID;
                m_prevEntryNumber = key.EntryNumber;
                m_prevValue1 = value.Value1;
                m_prevValue2 = value.Value2;
                m_prevValue3 = value.Value3;
                m_nextOffset += position;
                IndexOfNextKeyValue++;

                if (filter.Contains(key))
                    return true;
                if (IndexOfNextKeyValue >= RecordCount)
                    return false;
                goto TryAgain;
            }
            return false;
        }

        //protected override unsafe void DecodeRecord(HistorianKey key, HistorianValue value, KeyMatchFilterBase<HistorianKey> filter)
        //{
        //    ReadNext(key, value, true);
        //}

        /// <summary>
        /// Occurs when a new node has been reached and any encoded data that has been generated needs to be cleared.
        /// </summary>
        protected override void ResetEncoder()
        {
            m_prevTimestamp = 0;
            m_prevPointId = 0;
            m_prevEntryNumber = 0;
            m_prevValue1 = 0;
            m_prevValue2 = 0;
            m_prevValue3 = 0;
            m_nextOffset = 0;
        }

    }
}