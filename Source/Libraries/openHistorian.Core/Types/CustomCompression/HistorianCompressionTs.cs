//******************************************************************************************************
//  HistorianCompressionTs.cs - Gbtc
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

namespace openHistorian.Collections.Generic.CustomCompression
{
    /// <summary>
    /// The Node that will be used in the SortedTree that implements a compression method.
    /// </summary>
    public unsafe class HistorianCompressionTs
        : EncodedNodeBase<HistorianKey, HistorianValue>
    {

        public HistorianCompressionTs(byte level)
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
            //ToDo: Make stage 1 still work on little endian processors.
            int size = 0;

            //Compression Stages:
            //  Stage 1: Big Positive Float. 
            //  Stage 2: Big Negative Float.
            //  Stage 3: Zero
            //  Stage 4: 32 bit
            //  Stage 5: Catch all

            if (currentKey.Timestamp == prevKey.Timestamp
                && currentKey.PointID > prevKey.PointID && (currentKey.PointID - prevKey.PointID <= 16)
                && currentKey.EntryNumber == 0
                && currentValue.Value1 <= uint.MaxValue //must be a 32-bit value
                && currentValue.Value2 == 0
                && currentValue.Value3 == 0)
            {
                uint deltaPointId = (uint)(currentKey.PointID - prevKey.PointID);
                //Could match Stage 1, 2, 3, or 4

                //Check for Stage 3
                if (currentValue.Value1 == 0 && deltaPointId <= 16)
                {
                    //Stage 3: 28% of the time.
                    stream[0] = (byte)(0xC0 | (deltaPointId - 1));
                    return 1;
                }

                //Check for Stage 1
                if ((currentValue.Value1 >> 28) == 4 && deltaPointId <= 8)
                {
                    //Stage 1: 46% of the time
                    //Big Positive Float

                    //Must be stored big endian
                    //ByteCode is 0DDDVVVV
                    stream[0] = (byte)(((currentValue.Value1 >> 24) & 0xF) | (deltaPointId - 1) << 4);
                    stream[1] = (byte)(currentValue.Value1 >> 16);
                    stream[2] = (byte)(currentValue.Value1 >> 8);
                    stream[3] = (byte)currentValue.Value1;
                    return 4;
                }

                //Check for stage 2
                if ((currentValue.Value1 >> 28) == 12 && deltaPointId <= 4)
                {
                    //Must be stored big endian
                    //ByteCode is 10DDVVVV
                    stream[0] = (byte)(0x80 | ((currentValue.Value1 >> 24) & 0xF) | (deltaPointId - 1) << 4);
                    stream[1] = (byte)(currentValue.Value1 >> 16);
                    stream[2] = (byte)(currentValue.Value1 >> 8);
                    stream[3] = (byte)currentValue.Value1;
                    return 4;
                }

                //Check for stage 4
                //All conditions are in the logic statement that enters this block.
                //  deltaPointID <= 16
                stream[0] = (byte)(0xD0 | (deltaPointId - 1));
                *(uint*)(stream + 1) = (uint)currentValue.Value1;
                return 5;

            }

            //Stage 5: Catch All
            stream[0] = 0xE0;
            size = 1;
            if (currentKey.Timestamp != prevKey.Timestamp)
            {
                stream[0] |= 0x10; //Set bit T
                Compression.Write7Bit(stream, ref size, currentKey.Timestamp - prevKey.Timestamp);
                Compression.Write7Bit(stream, ref size, currentKey.PointID);
            }
            else
            {
                Compression.Write7Bit(stream, ref size, currentKey.PointID - prevKey.PointID);
            }


            if (currentKey.EntryNumber != 0)
            {
                stream[0] |= 0x08; //Set bit E
                Compression.Write7Bit(stream, ref size, currentKey.EntryNumber);
            }

            if (currentValue.Value1 > uint.MaxValue)
            {
                stream[0] |= 0x04; //Set Bit V1
                *(ulong*)(stream + size) = currentValue.Value1;
                size += 8;
            }
            else
            {
                *(uint*)(stream + size) = (uint)currentValue.Value1;
                size += 4;
            }

            if (currentValue.Value2 != 0)
            {
                stream[0] |= 0x02; //Set Bit V2
                Compression.Write7Bit(stream, ref size, currentValue.Value2);
            }
            if (currentValue.Value3 != 0)
            {
                //ToDo: Special encoding of flag fields
                stream[0] |= 0x01; //Set Bit V3
                Compression.Write7Bit(stream, ref size, currentValue.Value3);
            }
            return size;
        }

        protected override unsafe int DecodeRecord(byte* stream, byte* buffer, HistorianKey prevKey, HistorianValue prevValue, HistorianKey currentKey, HistorianValue currentValue)
        {
            int size = 0;
            uint code = stream[0];
            //Compression Stages:
            //  Stage 1: Big Positive Float. 
            //  Stage 2: Big Negative Float.
            //  Stage 3: Zero
            //  Stage 4: 32 bit
            //  Stage 5: Catch all

            if (code < 0x80)
            {
                //If stage 1 (50% success)
                currentKey.Timestamp = prevKey.Timestamp;
                currentKey.PointID = prevKey.PointID + 1 + ((code >> 4) & 0x7);
                currentKey.EntryNumber = 0;
                currentValue.Value1 = (4u<<28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
                currentValue.Value2 = 0;
                currentValue.Value3 = 0;
                return 4;
            }
            if (code < 0xC0)
            {
                //If stage 2 (16% success)
                currentKey.Timestamp = prevKey.Timestamp;
                currentKey.PointID = prevKey.PointID + 1 + ((code >> 4) & 0x3);
                currentKey.EntryNumber = 0;
                currentValue.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
                currentValue.Value2 = 0;
                currentValue.Value3 = 0;
                return 4;
            }
            if (code < 0xD0)
            {
                //If stage 3 (28% success)
                currentKey.Timestamp = prevKey.Timestamp;
                currentKey.PointID = prevKey.PointID + 1 + (code & 0xF);
                currentKey.EntryNumber = 0;
                currentValue.Value1 = 0;
                currentValue.Value2 = 0;
                currentValue.Value3 = 0;
                return 1;
            }
            if (code < 0xE0)
            {
                //If stage 4 (3% success)
                currentKey.Timestamp = prevKey.Timestamp;
                currentKey.PointID = prevKey.PointID + 1 + (code & 0xF);
                currentKey.EntryNumber = 0;
                currentValue.Value1 = *(uint*)(stream + 1);
                currentValue.Value2 = 0;
                currentValue.Value3 = 0;
                return 5;
            }

            //Stage 5: 2%
            //Stage 5: Catch All
            size = 1;
            if ((code & 16) != 0) //T is set
            {
                currentKey.Timestamp = prevKey.Timestamp + Compression.Read7BitUInt64(stream, ref size);
                currentKey.PointID = Compression.Read7BitUInt64(stream, ref size);
            }
            else
            {
                currentKey.Timestamp = prevKey.Timestamp;
                currentKey.PointID = prevKey.PointID + Compression.Read7BitUInt64(stream, ref size);
            }


            if ((code & 8) != 0) //E is set)
            {
                currentKey.EntryNumber = Compression.Read7BitUInt64(stream, ref size);
            }
            else
            {
                currentKey.EntryNumber = 0;
            }

            if ((code & 4) != 0) //V1 is set)
            {
                currentValue.Value1 = *(ulong*)(stream + size);
                size += 8;
            }
            else
            {
                currentValue.Value1 = *(uint*)(stream + size);
                size += 4;
            }

            if ((code & 2) != 0) //V2 is set)
            {
                currentValue.Value2 = Compression.Read7BitUInt64(stream, ref size);
            }
            else
            {
                currentValue.Value2 = 0;
            }

            if ((code & 1) != 0) //V3 is set)
            {
                currentValue.Value3 = Compression.Read7BitUInt64(stream, ref size);
            }
            else
            {
                currentValue.Value3 = 0;
            }
            return size;

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
            return new HistorianCompressionTsScanner(Level, BlockSize, Stream, SparseIndex.Get, KeyMethods.Create(), ValueMethods.Create());
        }
    }
}