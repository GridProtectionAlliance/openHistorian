//******************************************************************************************************
//  HistorianCompressionDelta.cs - Gbtc
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
using GSF;

namespace openHistorian.Collections.Generic.TreeNodes
{
    /// <summary>
    /// The Node that will be used in the SortedTree that implements a compression method.
    /// </summary>
    public unsafe class HistorianCompressionDelta
        : EncodedNodeBase<HistorianKey, HistorianValue>
    {

        /// <summary>
        /// Creates a new <see cref="HistorianCompressionDelta"/>
        /// </summary>
        /// <param name="level"></param>
        public HistorianCompressionDelta(byte level)
            : base(level, 2)
        {
        }

        //public unsafe int EncodeRecord(byte* buffer, KeyValue256 currentKey, KeyValue256 previousKey)
        //{
        //    int size = 0;
        //    Compression.Write7Bit(buffer, ref size, currentKey.Key1 ^ previousKey.Key1);
        //    Compression.Write7Bit(buffer, ref size, currentKey.Key2 ^ previousKey.Key2);
        //    Compression.Write7Bit(buffer, ref size, currentKey.Value1 ^ previousKey.Value1);
        //    Compression.Write7Bit(buffer, ref size, currentKey.Value2 ^ previousKey.Value2);
        //    return size;
        //}

        //public void DecodeNextRecord(BinaryStreamBase stream, KeyValue256 currentKey)
        //{
        //    currentKey.Key1 ^= stream.Read7BitUInt64();
        //    currentKey.Key2 ^= stream.Read7BitUInt64();
        //    currentKey.Value1 ^= stream.Read7BitUInt64();
        //    currentKey.Value2 ^= stream.Read7BitUInt64();
        //}

        protected override unsafe int EncodeRecord(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey currentKey, HistorianValue currentValue)
        {
            int size = 0;
            Compression.Write7Bit(stream, ref size, currentKey.Timestamp ^ prevKey.Timestamp);
            Compression.Write7Bit(stream, ref size, currentKey.PointID ^ prevKey.PointID);
            Compression.Write7Bit(stream, ref size, currentKey.EntryNumber ^ prevKey.EntryNumber);
            Compression.Write7Bit(stream, ref size, currentValue.Value1 ^ prevValue.Value1);
            Compression.Write7Bit(stream, ref size, currentValue.Value2 ^ prevValue.Value2);
            Compression.Write7Bit(stream, ref size, currentValue.Value3 ^ prevValue.Value3);
            return size;
        }

        protected override unsafe int DecodeRecord(byte* stream, byte* buffer, HistorianKey prevKey, HistorianValue prevValue, HistorianKey currentKey, HistorianValue currentValue)
        {
            int position = 0;
            currentKey.Timestamp = prevKey.Timestamp ^ Compression.Read7BitUInt64(stream, ref position);
            currentKey.PointID = prevKey.PointID ^ Compression.Read7BitUInt64(stream, ref position);
            currentKey.EntryNumber = prevKey.EntryNumber ^ Compression.Read7BitUInt64(stream, ref position);
            currentValue.Value1 = prevValue.Value1 ^ Compression.Read7BitUInt64(stream, ref position);
            currentValue.Value2 = prevValue.Value2 ^ Compression.Read7BitUInt64(stream, ref position);
            currentValue.Value3 = prevValue.Value3 ^ Compression.Read7BitUInt64(stream, ref position);
            return position;
        }

        protected override unsafe int MaximumStorageSize
        {
            get
            {
                return KeyValueSize + 6;
            }
        }

        protected override int MaxOverheadWithCombineNodes
        {
            get
            {
                return MaximumStorageSize * 2;
            }
        }

        public override unsafe TreeScannerBase<HistorianKey, HistorianValue> CreateTreeScanner()
        {
            return new HistorianCompressionDeltaScanner(Level, BlockSize, Stream, SparseIndex.Get, KeyMethods.Create(), ValueMethods.Create());
        }
    }
}