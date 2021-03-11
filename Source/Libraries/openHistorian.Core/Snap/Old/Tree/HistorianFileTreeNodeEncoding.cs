////******************************************************************************************************
////  HistorianCompressionTs.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://opensource.org/licenses/MIT
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  07/26/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using GSF;
//using GSF.Snap.Tree;
//using GSF.Snap.Tree.TreeNodes;

//namespace openHistorian.Snap.Tree
//{
//    /// <summary>
//    /// The Node that will be used in the SortedTree that implements a compression method.
//    /// </summary>
//    public unsafe class HistorianFileTreeNodeEncoding
//        : EncodedNodeBase<HistorianKey, HistorianValue>
//    {

//        public HistorianFileTreeNodeEncoding(byte level)
//            : base(level)
//        {
//        }

//        //protected override void AppendSequentailStream(InsertStreamHelper<HistorianKey, HistorianValue> stream, out bool isFull)
//        //{
//        //    HistorianKey Key1 = stream.Key1;
//        //    HistorianKey Key2 = stream.Key2;
//        //    HistorianValue Value1 = stream.Value1;
//        //    HistorianValue Value2 = stream.Value2;
//        //    bool isValid = stream.IsValid;
//        //    bool isStillSequential = stream.IsStillSequential;
//        //    bool isKVP1 = stream.IsKVP1;
//        //    int remainingBytes = RemainingBytes;

//        //    int recordsAdded = 0;
//        //    byte* writePointer = GetWritePointer();
//        //    fixed (byte* buffer = m_buffer1)
//        //    {
//        //        SeekTo(RecordCount);

//        //        if (RecordCount > 0)
//        //        {
//        //            KeyMethods.Copy(m_currentKey, stream.PrevKey);
//        //        }
//        //        else
//        //        {
//        //            KeyMethods.Clear(stream.PrevKey);
//        //        }

//        //    TryAgain:
//        //        if (!isValid || !isStillSequential)
//        //        {
//        //            isFull = false;
//        //            IncrementRecordCounts(recordsAdded, RemainingBytes - remainingBytes);
//        //            ClearNodeCache();
//        //            stream.IsValid = isValid;
//        //            stream.IsKVP1 = isKVP1;
//        //            stream.IsStillSequential = isStillSequential;
//        //            return;
//        //        }

//        //        int length;
//        //        if (isKVP1)
//        //        {
//        //            //Key1,Value1 are the current record
//        //            if (remainingBytes < m_maximumStorageSize)
//        //            {
//        //                length = EncodeRecord(buffer, Key2, Value2, Key1, Value1);
//        //                if (remainingBytes < length)
//        //                {
//        //                    isFull = true;
//        //                    IncrementRecordCounts(recordsAdded, RemainingBytes - remainingBytes);
//        //                    ClearNodeCache();
//        //                    stream.IsValid = isValid;
//        //                    stream.IsKVP1 = isKVP1;
//        //                    stream.IsStillSequential = isStillSequential;
//        //                    return;
//        //                }
//        //            }

//        //            length = EncodeRecord(writePointer + m_nextOffset, Key2, Value2, Key1, Value1);
//        //            remainingBytes -= length;
//        //            recordsAdded++;
//        //            m_nextOffset = m_currentOffset + length;
//        //            //Inlined stream.Next()
//        //            isValid = stream.Stream.Read(Key2, Value2);
//        //            isKVP1 = false;
//        //            isStillSequential = Key1.Timestamp < Key2.Timestamp
//        //                 || (Key1.Timestamp == Key2.Timestamp && Key1.PointID < Key2.PointID)
//        //                 || (Key1.Timestamp == Key2.Timestamp && Key1.PointID == Key2.PointID && Key1.EntryNumber < Key2.EntryNumber);
//        //            //End Inlined
//        //            goto TryAgain;
//        //        }
//        //        else
//        //        {
//        //            //Key2,Value2 are the current record
//        //            if (remainingBytes < m_maximumStorageSize)
//        //            {
//        //                length = EncodeRecord(buffer, Key1, Value1, Key2, Value2);
//        //                if (remainingBytes < length)
//        //                {
//        //                    isFull = true;
//        //                    IncrementRecordCounts(recordsAdded, RemainingBytes - remainingBytes);
//        //                    ClearNodeCache();
//        //                    stream.IsValid = isValid;
//        //                    stream.IsKVP1 = isKVP1;
//        //                    stream.IsStillSequential = isStillSequential;
//        //                    return;
//        //                }
//        //            }

//        //            length = EncodeRecord(writePointer + m_nextOffset, Key1, Value1, Key2, Value2);
//        //            remainingBytes -= length;
//        //            recordsAdded++;
//        //            m_nextOffset = m_currentOffset + length;

//        //            //Inlined stream.Next()
//        //            isValid = stream.Stream.Read(Key1, Value1);
//        //            isKVP1 = true;
//        //            isStillSequential = Key2.Timestamp < Key1.Timestamp
//        //             || (Key2.Timestamp == Key1.Timestamp && Key2.PointID < Key1.PointID)
//        //             || (Key2.Timestamp == Key1.Timestamp && Key2.PointID == Key1.PointID && Key2.EntryNumber < Key1.EntryNumber);
//        //            //End Inlined

//        //            goto TryAgain;
//        //        }
//        //    }
//        //}

//        //public unsafe int EncodeRecord(byte* buffer, KeyValue256 currentKey, KeyValue256 previousKey)
//        //{
//        //    int size = 0;
//        //    Compression.Write7Bit(buffer, ref size, currentKey.Key1 ^ previousKey.Key1);
//        //    Compression.Write7Bit(buffer, ref size, currentKey.Key2 ^ previousKey.Key2);
//        //    Compression.Write7Bit(buffer, ref size, currentKey.Value1 ^ previousKey.Value1);
//        //    Compression.Write7Bit(buffer, ref size, currentKey.Value2 ^ previousKey.Value2);
//        //    return size;
//        //}

//        //public void DecodeNextRecord(BinaryStreamBase stream, KeyValue256 currentKey)
//        //{
//        //    currentKey.Key1 ^= stream.Read7BitUInt64();
//        //    currentKey.Key2 ^= stream.Read7BitUInt64();
//        //    currentKey.Value1 ^= stream.Read7BitUInt64();
//        //    currentKey.Value2 ^= stream.Read7BitUInt64();
//        //}

//        protected override unsafe int EncodeRecord(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey currentKey, HistorianValue currentValue)
//        {
//            //ToDo: Make stage 1 still work on big endian processors.
//            int size = 0;

//            //Compression Stages:
//            //  Stage 1: Big Positive Float. 
//            //  Stage 2: Big Negative Float.
//            //  Stage 3: Zero
//            //  Stage 4: 32 bit
//            //  Stage 5: Catch all

//            if (currentKey.Timestamp == prevKey.Timestamp
//                && currentKey.PointID > prevKey.PointID && (currentKey.PointID - prevKey.PointID <= 16)
//                && currentKey.EntryNumber == 0
//                && currentValue.Value1 <= uint.MaxValue //must be a 32-bit value
//                && currentValue.Value2 == 0
//                && currentValue.Value3 == 0)
//            {
//                uint deltaPointId = (uint)(currentKey.PointID - prevKey.PointID);
//                //Could match Stage 1, 2, 3, or 4

//                //Check for Stage 3
//                if (currentValue.Value1 == 0 && deltaPointId <= 16)
//                {
//                    //Stage 3: 28% of the time.
//                    stream[0] = (byte)(0xC0 | (deltaPointId - 1));
//                    return 1;
//                }

//                //Check for Stage 1
//                if ((currentValue.Value1 >> 28) == 4 && deltaPointId <= 8)
//                {
//                    //Stage 1: 46% of the time
//                    //Big Positive Float

//                    //Must be stored big endian
//                    //ByteCode is 0DDDVVVV
//                    stream[0] = (byte)(((currentValue.Value1 >> 24) & 0xF) | (deltaPointId - 1) << 4);
//                    stream[1] = (byte)(currentValue.Value1 >> 16);
//                    stream[2] = (byte)(currentValue.Value1 >> 8);
//                    stream[3] = (byte)currentValue.Value1;
//                    return 4;
//                }

//                //Check for stage 2
//                if ((currentValue.Value1 >> 28) == 12 && deltaPointId <= 4)
//                {
//                    //Must be stored big endian
//                    //ByteCode is 10DDVVVV
//                    stream[0] = (byte)(0x80 | ((currentValue.Value1 >> 24) & 0xF) | (deltaPointId - 1) << 4);
//                    stream[1] = (byte)(currentValue.Value1 >> 16);
//                    stream[2] = (byte)(currentValue.Value1 >> 8);
//                    stream[3] = (byte)currentValue.Value1;
//                    return 4;
//                }

//                //Check for stage 4
//                //All conditions are in the logic statement that enters this block.
//                //  deltaPointID <= 16
//                stream[0] = (byte)(0xD0 | (deltaPointId - 1));
//                *(uint*)(stream + 1) = (uint)currentValue.Value1;
//                return 5;

//            }

//            //Stage 5: Catch All
//            stream[0] = 0xE0;
//            size = 1;
//            if (currentKey.Timestamp != prevKey.Timestamp)
//            {
//                stream[0] |= 0x10; //Set bit T
//                Encoding7Bit.Write(stream, ref size, currentKey.Timestamp - prevKey.Timestamp);
//                Encoding7Bit.Write(stream, ref size, currentKey.PointID);
//            }
//            else
//            {
//                Encoding7Bit.Write(stream, ref size, currentKey.PointID - prevKey.PointID);
//            }


//            if (currentKey.EntryNumber != 0)
//            {
//                stream[0] |= 0x08; //Set bit E
//                Encoding7Bit.Write(stream, ref size, currentKey.EntryNumber);
//            }

//            if (currentValue.Value1 > uint.MaxValue)
//            {
//                stream[0] |= 0x04; //Set Bit V1
//                *(ulong*)(stream + size) = currentValue.Value1;
//                size += 8;
//            }
//            else
//            {
//                *(uint*)(stream + size) = (uint)currentValue.Value1;
//                size += 4;
//            }

//            if (currentValue.Value2 != 0)
//            {
//                stream[0] |= 0x02; //Set Bit V2
//                Encoding7Bit.Write(stream, ref size, currentValue.Value2);
//            }
//            if (currentValue.Value3 != 0)
//            {
//                //ToDo: Special encoding of flag fields
//                stream[0] |= 0x01; //Set Bit V3
//                Encoding7Bit.Write(stream, ref size, currentValue.Value3);
//            }
//            return size;
//        }

//        protected override unsafe int DecodeRecord(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey currentKey, HistorianValue currentValue)
//        {
//            int size = 0;
//            uint code = stream[0];
//            //Compression Stages:
//            //  Stage 1: Big Positive Float. 
//            //  Stage 2: Big Negative Float.
//            //  Stage 3: Zero
//            //  Stage 4: 32 bit
//            //  Stage 5: Catch all

//            if (code < 0x80)
//            {
//                //If stage 1 (50% success)
//                currentKey.Timestamp = prevKey.Timestamp;
//                currentKey.PointID = prevKey.PointID + 1 + ((code >> 4) & 0x7);
//                currentKey.EntryNumber = 0;
//                currentValue.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                currentValue.Value2 = 0;
//                currentValue.Value3 = 0;
//                return 4;
//            }
//            if (code < 0xC0)
//            {
//                //If stage 2 (16% success)
//                currentKey.Timestamp = prevKey.Timestamp;
//                currentKey.PointID = prevKey.PointID + 1 + ((code >> 4) & 0x3);
//                currentKey.EntryNumber = 0;
//                currentValue.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                currentValue.Value2 = 0;
//                currentValue.Value3 = 0;
//                return 4;
//            }
//            if (code < 0xD0)
//            {
//                //If stage 3 (28% success)
//                currentKey.Timestamp = prevKey.Timestamp;
//                currentKey.PointID = prevKey.PointID + 1 + (code & 0xF);
//                currentKey.EntryNumber = 0;
//                currentValue.Value1 = 0;
//                currentValue.Value2 = 0;
//                currentValue.Value3 = 0;
//                return 1;
//            }
//            if (code < 0xE0)
//            {
//                //If stage 4 (3% success)
//                currentKey.Timestamp = prevKey.Timestamp;
//                currentKey.PointID = prevKey.PointID + 1 + (code & 0xF);
//                currentKey.EntryNumber = 0;
//                currentValue.Value1 = *(uint*)(stream + 1);
//                currentValue.Value2 = 0;
//                currentValue.Value3 = 0;
//                return 5;
//            }

//            //Stage 5: 2%
//            //Stage 5: Catch All
//            size = 1;
//            if ((code & 16) != 0) //T is set
//            {
//                currentKey.Timestamp = prevKey.Timestamp + Encoding7Bit.ReadUInt64(stream, ref size);
//                currentKey.PointID = Encoding7Bit.ReadUInt64(stream, ref size);
//            }
//            else
//            {
//                currentKey.Timestamp = prevKey.Timestamp;
//                currentKey.PointID = prevKey.PointID + Encoding7Bit.ReadUInt64(stream, ref size);
//            }


//            if ((code & 8) != 0) //E is set)
//            {
//                currentKey.EntryNumber = Encoding7Bit.ReadUInt64(stream, ref size);
//            }
//            else
//            {
//                currentKey.EntryNumber = 0;
//            }

//            if ((code & 4) != 0) //V1 is set)
//            {
//                currentValue.Value1 = *(ulong*)(stream + size);
//                size += 8;
//            }
//            else
//            {
//                currentValue.Value1 = *(uint*)(stream + size);
//                size += 4;
//            }

//            if ((code & 2) != 0) //V2 is set)
//            {
//                currentValue.Value2 = Encoding7Bit.ReadUInt64(stream, ref size);
//            }
//            else
//            {
//                currentValue.Value2 = 0;
//            }

//            if ((code & 1) != 0) //V3 is set)
//            {
//                currentValue.Value3 = Encoding7Bit.ReadUInt64(stream, ref size);
//            }
//            else
//            {
//                currentValue.Value3 = 0;
//            }
//            return size;

//        }

//        protected override int MaximumStorageSize
//        {
//            get
//            {
//                return KeyValueSize + 6;
//            }
//        }

//        protected override int MaxOverheadWithCombineNodes
//        {
//            get
//            {
//                return MaximumStorageSize * 2;
//            }
//        }

//        public override SortedTreeScannerBase<HistorianKey, HistorianValue> CreateTreeScanner()
//        {
//            return new HistorianFileTreeNodeScannerEncoding(Level, BlockSize, Stream, SparseIndex.Get);
//        }
//    }
//}