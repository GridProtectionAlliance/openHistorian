////******************************************************************************************************
////  HistorianCompressionTsScanner.cs - Gbtc
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
//using GSF.Collections;
//using GSF.IO;
//using GSF.Snap.Filters;
//using GSF.Snap.Tree.TreeNodes;

//namespace openHistorian.Snap.Tree
//{
//    /// <summary>
//    /// A custom encoder that can highly compress time series data.
//    /// </summary>
//    public unsafe class HistorianFileTreeNodeScannerEncoding
//        : EncodedNodeScannerBase<HistorianKey, HistorianValue>
//    {
//        int m_nextOffset;
//        ulong m_prevTimestamp;
//        ulong m_prevPointId;

//        /// <summary>
//        /// Creates a new class
//        /// </summary>
//        /// <param name="level"></param>
//        /// <param name="blockSize"></param>
//        /// <param name="stream"></param>
//        /// <param name="lookupKey"></param>
//        public HistorianFileTreeNodeScannerEncoding(byte level, int blockSize, BinaryStreamPointerBase stream, Func<HistorianKey, byte, uint> lookupKey)
//            : base(level, blockSize, stream, lookupKey)
//        {
//            m_nextOffset = 0;
//            m_prevTimestamp = 0;
//            m_prevPointId = 0;
//        }

//        /// <summary>
//        /// Occurs when a new node has been reached and any encoded data that has been generated needs to be cleared.
//        /// </summary>
//        protected override void ResetEncoder()
//        {
//            m_nextOffset = 0;
//            m_prevTimestamp = 0;
//            m_prevPointId = 0;
//        }

//        #region [ Special Override Implementations ]

//        public override bool ReadWhile(HistorianKey key, HistorianValue value, HistorianKey upperBounds)
//        {
//            if (Stream.PointerVersion == PointerVersion && IndexOfNextKeyValue < RecordCount &&
//                (UpperKey.Timestamp < upperBounds.Timestamp ||
//                 UpperKey.Timestamp == upperBounds.Timestamp && UpperKey.PointID < upperBounds.PointID ||
//                 UpperKey.Timestamp == upperBounds.Timestamp && UpperKey.PointID == upperBounds.PointID && UpperKey.EntryNumber < upperBounds.EntryNumber)
//                )
//            {
//                IndexOfNextKeyValue++;
//                byte* stream = Pointer + m_nextOffset;
//                uint code = stream[0];
//                //Compression Stages:
//                //  Stage 1: Big Positive Float. 
//                //  Stage 2: Big Negative Float.
//                //  Stage 3: Zero
//                //  Stage 4: 32 bit
//                //  Stage 5: Catch all

//                if (code < 0x80)
//                {
//                    //If stage 1 (50% success)
//                    //prevTimestamp = prevTimestamp;
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x7);
//                    key.EntryNumber = 0;
//                    value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                    value.Value2 = 0;
//                    value.Value3 = 0;
//                    m_nextOffset += 4;
//                }
//                else if (code < 0xC0)
//                {
//                    //If stage 2 (16% success)
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x3);
//                    key.EntryNumber = 0;
//                    value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                    value.Value2 = 0;
//                    value.Value3 = 0;
//                    m_nextOffset += 4;
//                }
//                else if (code < 0xD0)
//                {
//                    //If stage 3 (28% success)
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointId + 1 + (code & 0xF);
//                    key.EntryNumber = 0;
//                    value.Value1 = 0;
//                    value.Value2 = 0;
//                    value.Value3 = 0;
//                    m_nextOffset += 1;
//                }
//                else if (code < 0xE0)
//                {
//                    //If stage 4 (3% success)
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointId + 1 + (code & 0xF);
//                    key.EntryNumber = 0;
//                    value.Value1 = *(uint*)(stream + 1);
//                    value.Value2 = 0;
//                    value.Value3 = 0;
//                    m_nextOffset += 5;

//                }
//                else
//                {
//                    //Stage 5: 2%
//                    //Stage 5: Catch All
//                    int size = 1;
//                    if ((code & 16) != 0) //T is set
//                    {
//                        key.Timestamp = m_prevTimestamp + Encoding7Bit.ReadUInt64(stream, ref size);
//                        key.PointID = Encoding7Bit.ReadUInt64(stream, ref size);
//                    }
//                    else
//                    {
//                        key.Timestamp = m_prevTimestamp;
//                        key.PointID = m_prevPointId + Encoding7Bit.ReadUInt64(stream, ref size);
//                    }

//                    if ((code & 8) != 0) //E is set)
//                    {
//                        key.EntryNumber = Encoding7Bit.ReadUInt64(stream, ref size);
//                    }
//                    else
//                    {
//                        key.EntryNumber = 0;
//                    }

//                    if ((code & 4) != 0) //V1 is set)
//                    {
//                        value.Value1 = *(ulong*)(stream + size);
//                        size += 8;
//                    }
//                    else
//                    {
//                        value.Value1 = *(uint*)(stream + size);
//                        size += 4;
//                    }

//                    if ((code & 2) != 0) //V2 is set)
//                    {
//                        value.Value2 = Encoding7Bit.ReadUInt64(stream, ref size);
//                    }
//                    else
//                    {
//                        value.Value2 = 0;
//                    }

//                    if ((code & 1) != 0) //V3 is set)
//                    {
//                        value.Value3 = Encoding7Bit.ReadUInt64(stream, ref size);
//                    }
//                    else
//                    {
//                        value.Value3 = 0;
//                    }
//                    m_nextOffset += size;

//                }

//                m_prevTimestamp = key.Timestamp;
//                m_prevPointId = key.PointID;
//                return true;
//            }
//            return ReadWhileCatchAll(key, value, upperBounds);
//        }

//        #endregion

//        protected override void InternalPeek(HistorianKey key, HistorianValue value)
//        {
//            byte* stream = Pointer + m_nextOffset;
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
//                //prevTimestamp = prevTimestamp;
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x7);
//                key.EntryNumber = 0;
//                value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//            }
//            else if (code < 0xC0)
//            {
//                //If stage 2 (16% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x3);
//                key.EntryNumber = 0;
//                value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//            }
//            else if (code < 0xD0)
//            {
//                //If stage 3 (28% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + (code & 0xF);
//                key.EntryNumber = 0;
//                value.Value1 = 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//            }
//            else if (code < 0xE0)
//            {
//                //If stage 4 (3% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + (code & 0xF);
//                key.EntryNumber = 0;
//                value.Value1 = *(uint*)(stream + 1);
//                value.Value2 = 0;
//                value.Value3 = 0;
//            }
//            else
//            {
//                //Stage 5: 2%
//                //Stage 5: Catch All
//                int size = 1;
//                if ((code & 16) != 0) //T is set
//                {
//                    key.Timestamp = m_prevTimestamp + Encoding7Bit.ReadUInt64(stream, ref size);
//                    key.PointID = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointId + Encoding7Bit.ReadUInt64(stream, ref size);
//                }

//                if ((code & 8) != 0) //E is set)
//                {
//                    key.EntryNumber = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.EntryNumber = 0;
//                }

//                if ((code & 4) != 0) //V1 is set)
//                {
//                    value.Value1 = *(ulong*)(stream + size);
//                    size += 8;
//                }
//                else
//                {
//                    value.Value1 = *(uint*)(stream + size);
//                    size += 4;
//                }

//                if ((code & 2) != 0) //V2 is set)
//                {
//                    value.Value2 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value2 = 0;
//                }

//                if ((code & 1) != 0) //V3 is set)
//                {
//                    value.Value3 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value3 = 0;
//                }
//            }
//        }

//        protected override void InternalRead(HistorianKey key, HistorianValue value)
//        {
//            byte* stream = Pointer + m_nextOffset;
//            int size;
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
//                //prevTimestamp = prevTimestamp;
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x7);
//                key.EntryNumber = 0;
//                value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 4;
//            }
//            else if (code < 0xC0)
//            {
//                //If stage 2 (16% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x3);
//                key.EntryNumber = 0;
//                value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 4;
//            }
//            else if (code < 0xD0)
//            {
//                //If stage 3 (28% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + (code & 0xF);
//                key.EntryNumber = 0;
//                value.Value1 = 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 1;
//            }
//            else if (code < 0xE0)
//            {
//                //If stage 4 (3% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + (code & 0xF);
//                key.EntryNumber = 0;
//                value.Value1 = *(uint*)(stream + 1);
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 5;
//            }
//            else
//            {
//                //Stage 5: 2%
//                //Stage 5: Catch All
//                size = 1;
//                if ((code & 16) != 0) //T is set
//                {
//                    key.Timestamp = m_prevTimestamp + Encoding7Bit.ReadUInt64(stream, ref size);
//                    key.PointID = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointId + Encoding7Bit.ReadUInt64(stream, ref size);
//                }

//                if ((code & 8) != 0) //E is set)
//                {
//                    key.EntryNumber = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.EntryNumber = 0;
//                }

//                if ((code & 4) != 0) //V1 is set)
//                {
//                    value.Value1 = *(ulong*)(stream + size);
//                    size += 8;
//                }
//                else
//                {
//                    value.Value1 = *(uint*)(stream + size);
//                    size += 4;
//                }

//                if ((code & 2) != 0) //V2 is set)
//                {
//                    value.Value2 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value2 = 0;
//                }

//                if ((code & 1) != 0) //V3 is set)
//                {
//                    value.Value3 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value3 = 0;
//                }
//            }

//            m_prevTimestamp = key.Timestamp;
//            m_prevPointId = key.PointID;
//            m_nextOffset += size;
//            IndexOfNextKeyValue++;
//        }

//        protected override bool InternalRead(HistorianKey key, HistorianValue value, MatchFilterBase<HistorianKey, HistorianValue> filter)
//        {
//            byte* stream = Pointer + m_nextOffset;
//            int totalSize = 0;

//            key.Timestamp = m_prevTimestamp;
//            key.PointID = m_prevPointId;
//            key.EntryNumber = 0;

//            //ulong prevTimestamp = m_prevTimestamp;
//        //ulong prevPointId = m_prevPointId;

//        FilterFailed:
//            IndexOfNextKeyValue++;
//            if (IndexOfNextKeyValue > RecordCount)
//            {
//                m_prevPointId = key.PointID;
//                m_prevTimestamp = key.Timestamp;
//                m_nextOffset += totalSize;
//                return false;
//            }

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
//                //key.Timestamp = prevTimestamp;
//                key.PointID += 1 + ((code >> 4) & 0x7);
//                //key.EntryNumber = 0;

//                value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                if (!filter.Contains(key, value))
//                {
//                    totalSize += 4;
//                    stream += 4;
//                    goto FilterFailed;
//                }
//                totalSize += 4;
//            }
//            else if (code < 0xC0)
//            {
//                //If stage 2 (16% success)
//                //key.Timestamp = prevTimestamp;
//                key.PointID += 1 + ((code >> 4) & 0x3);
//                //key.EntryNumber = 0;

//                value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                if (!filter.Contains(key, value))
//                {
//                    totalSize += 4;
//                    stream += 4;
//                    goto FilterFailed;
//                }
//                totalSize += 4;
//            }
//            else if (code < 0xD0)
//            {
//                //If stage 3 (28% success)
//                //prevTimestamp = prevTimestamp;
//                key.PointID += 1 + (code & 0xF);
//                //key.EntryNumber = 0;

//                value.Value1 = 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                if (!filter.Contains(key, value))
//                {
//                    totalSize += 1;
//                    stream += 1;
//                    goto FilterFailed;
//                }
//                totalSize += 1;
//            }
//            else if (code < 0xE0)
//            {
//                //If stage 4 (3% success)
//                //prevTimestamp = prevTimestamp;
//                key.PointID += 1 + (code & 0xF);
//                //key.EntryNumber = 0;

//                value.Value1 = *(uint*)(stream + 1);
//                value.Value2 = 0;
//                value.Value3 = 0;
//                if (!filter.Contains(key, value))
//                {
//                    totalSize += 5;
//                    stream += 5;
//                    goto FilterFailed;
//                }
//                totalSize += 5;
//                //key.Timestamp = prevTimestamp;
//            }
//            else
//            {
//                //Stage 5: 2%
//                //Stage 5: Catch All
//                int size = 1;
//                if ((code & 16) != 0) //T is set
//                {
//                    key.Timestamp += Encoding7Bit.ReadUInt64(stream, ref size);
//                    key.PointID = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.PointID += Encoding7Bit.ReadUInt64(stream, ref size);
//                }

//                if ((code & 8) != 0) //E is set)
//                {
//                    key.EntryNumber = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.EntryNumber = 0;
//                }




//                if ((code & 4) != 0) //V1 is set)
//                {
//                    value.Value1 = *(ulong*)(stream + size);
//                    size += 8;
//                }
//                else
//                {
//                    value.Value1 = *(uint*)(stream + size);
//                    size += 4;
//                }

//                if ((code & 2) != 0) //V2 is set)
//                {
//                    value.Value2 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value2 = 0;
//                }

//                if ((code & 1) != 0) //V3 is set)
//                {
//                    value.Value3 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value3 = 0;
//                }

//                if (!filter.Contains(key, value))
//                {
//                    key.EntryNumber = 0;
//                    size += 4 + ((byte)code & 4);
//                    //if ((code & 4) != 0) //V1 is set)
//                    //{
//                    //    size += 8;
//                    //}
//                    //else
//                    //{
//                    //    size += 4;
//                    //}

//                    if ((code & 2) != 0) //V2 is set)
//                    {
//                        size += Encoding7Bit.MeasureUInt64(stream, size);
//                    }

//                    if ((code & 1) != 0) //V3 is set)
//                    {
//                        size += Encoding7Bit.MeasureUInt64(stream, size);
//                    }

//                    totalSize += size;
//                    stream += size;
//                    goto FilterFailed;
//                }
//                totalSize += size;
//            }

//            m_prevPointId = key.PointID;
//            m_prevTimestamp = key.Timestamp;
//            m_nextOffset += totalSize;
//            return true;
//        }

//        protected override bool InternalReadWhile(HistorianKey key, HistorianValue value, HistorianKey upperBounds)
//        {
//            byte* stream = Pointer + m_nextOffset;
//            int size;
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
//                //prevTimestamp = prevTimestamp;
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x7);
//                key.EntryNumber = 0;
//                value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 4;
//            }
//            else if (code < 0xC0)
//            {
//                //If stage 2 (16% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x3);
//                key.EntryNumber = 0;
//                value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 4;
//            }
//            else if (code < 0xD0)
//            {
//                //If stage 3 (28% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + (code & 0xF);
//                key.EntryNumber = 0;
//                value.Value1 = 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 1;
//            }
//            else if (code < 0xE0)
//            {
//                //If stage 4 (3% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + (code & 0xF);
//                key.EntryNumber = 0;
//                value.Value1 = *(uint*)(stream + 1);
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 5;
//            }
//            else
//            {
//                //Stage 5: 2%
//                //Stage 5: Catch All
//                size = 1;
//                if ((code & 16) != 0) //T is set
//                {
//                    key.Timestamp = m_prevTimestamp + Encoding7Bit.ReadUInt64(stream, ref size);
//                    key.PointID = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointId + Encoding7Bit.ReadUInt64(stream, ref size);
//                }

//                if ((code & 8) != 0) //E is set)
//                {
//                    key.EntryNumber = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.EntryNumber = 0;
//                }

//                if ((code & 4) != 0) //V1 is set)
//                {
//                    value.Value1 = *(ulong*)(stream + size);
//                    size += 8;
//                }
//                else
//                {
//                    value.Value1 = *(uint*)(stream + size);
//                    size += 4;
//                }

//                if ((code & 2) != 0) //V2 is set)
//                {
//                    value.Value2 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value2 = 0;
//                }

//                if ((code & 1) != 0) //V3 is set)
//                {
//                    value.Value3 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value3 = 0;
//                }
//            }

//            if (key.Timestamp < upperBounds.Timestamp ||
//                key.Timestamp == upperBounds.Timestamp && key.PointID < upperBounds.PointID ||
//                key.Timestamp == upperBounds.Timestamp && key.PointID == upperBounds.PointID &&
//                key.EntryNumber < upperBounds.EntryNumber)
//            {
//                m_prevTimestamp = key.Timestamp;
//                m_prevPointId = key.PointID;
//                m_nextOffset += size;
//                IndexOfNextKeyValue++;
//                return true;
//            }
//            return false;
//        }

//        protected override bool InternalReadWhile(HistorianKey key, HistorianValue value, HistorianKey upperBounds, MatchFilterBase<HistorianKey, HistorianValue> filter)
//        {
//        TryAgain:
//            byte* stream = Pointer + m_nextOffset;
//            int size;
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
//                //prevTimestamp = prevTimestamp;
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x7);
//                key.EntryNumber = 0;
//                value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 4;
//            }
//            else if (code < 0xC0)
//            {
//                //If stage 2 (16% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + ((code >> 4) & 0x3);
//                key.EntryNumber = 0;
//                value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 4;
//            }
//            else if (code < 0xD0)
//            {
//                //If stage 3 (28% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + (code & 0xF);
//                key.EntryNumber = 0;
//                value.Value1 = 0;
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 1;
//            }
//            else if (code < 0xE0)
//            {
//                //If stage 4 (3% success)
//                key.Timestamp = m_prevTimestamp;
//                key.PointID = m_prevPointId + 1 + (code & 0xF);
//                key.EntryNumber = 0;
//                value.Value1 = *(uint*)(stream + 1);
//                value.Value2 = 0;
//                value.Value3 = 0;
//                size = 5;
//            }
//            else
//            {
//                //Stage 5: 2%
//                //Stage 5: Catch All
//                size = 1;
//                if ((code & 16) != 0) //T is set
//                {
//                    key.Timestamp = m_prevTimestamp + Encoding7Bit.ReadUInt64(stream, ref size);
//                    key.PointID = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointId + Encoding7Bit.ReadUInt64(stream, ref size);
//                }

//                if ((code & 8) != 0) //E is set)
//                {
//                    key.EntryNumber = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    key.EntryNumber = 0;
//                }

//                if ((code & 4) != 0) //V1 is set)
//                {
//                    value.Value1 = *(ulong*)(stream + size);
//                    size += 8;
//                }
//                else
//                {
//                    value.Value1 = *(uint*)(stream + size);
//                    size += 4;
//                }

//                if ((code & 2) != 0) //V2 is set)
//                {
//                    value.Value2 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value2 = 0;
//                }

//                if ((code & 1) != 0) //V3 is set)
//                {
//                    value.Value3 = Encoding7Bit.ReadUInt64(stream, ref size);
//                }
//                else
//                {
//                    value.Value3 = 0;
//                }
//            }

//            if (key.Timestamp < upperBounds.Timestamp ||
//                key.Timestamp == upperBounds.Timestamp && key.PointID < upperBounds.PointID ||
//                key.Timestamp == upperBounds.Timestamp && key.PointID == upperBounds.PointID &&
//                key.EntryNumber < upperBounds.EntryNumber)
//            {
//                m_prevTimestamp = key.Timestamp;
//                m_prevPointId = key.PointID;
//                m_nextOffset += size;
//                IndexOfNextKeyValue++;
//                if (filter.Contains(key, value))
//                {
//                    return true;
//                }
//                if (IndexOfNextKeyValue >= RecordCount)
//                    return false;
//                goto TryAgain;
//            }
//            return false;
//        }

//        //bool InternalReadBitArray(HistorianKey key, HistorianValue value, PointIDFilter.BitArrayFilter<HistorianKey> filter)
//        //{
//        //    //ToDo: Fix the bug in this code. The correct number of points are not returned.

//        //    fixed (long* array = filter.ArrayBits)
//        //    {
//        //        byte* stream = Pointer + m_nextOffset;
//        //        ulong prevPointID = m_prevPointId;
//        //        int totalSize = 0;
//        //        int size = 0;
//        //        uint maxValue = (uint)filter.MaxValue;
//        //        int IndexOfNextKeyValue = this.IndexOfNextKeyValue;

//        //        key.EntryNumber = 0;

//        //    FilterFailed:
//        //        IndexOfNextKeyValue++;
//        //        if (IndexOfNextKeyValue > RecordCount)
//        //        {
//        //            this.IndexOfNextKeyValue = IndexOfNextKeyValue;
//        //            m_prevPointId = key.PointID;
//        //            m_prevTimestamp = key.Timestamp;
//        //            m_nextOffset += totalSize;
//        //            return false;
//        //        }

//        //        uint code = stream[0];
//        //        //Compression Stages:
//        //        //  Stage 1: Big Positive Float. 
//        //        //  Stage 2: Big Negative Float.
//        //        //  Stage 3: Zero
//        //        //  Stage 4: 32 bit
//        //        //  Stage 5: Catch all
//        //        if (code < 0x80)
//        //        {
//        //            //If stage 1 (50% success)
//        //            //key.Timestamp = prevTimestamp;
//        //            prevPointID += 1 + ((code >> 4) & 0x7);
//        //            //key.EntryNumber = 0;
//        //            if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
//        //            {
//        //                totalSize += 4;
//        //                stream += 4;
//        //                goto FilterFailed;
//        //            }
//        //            value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//        //            value.Value2 = 0;
//        //            value.Value3 = 0;
//        //            totalSize += 4;
//        //        }
//        //        else if (code < 0xC0)
//        //        {
//        //            //If stage 2 (16% success)
//        //            //key.Timestamp = prevTimestamp;
//        //            prevPointID += 1 + ((code >> 4) & 0x3);
//        //            //key.EntryNumber = 0;
//        //            if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
//        //            {
//        //                totalSize += 4;
//        //                stream += 4;
//        //                goto FilterFailed;
//        //            }
//        //            value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
//        //            value.Value2 = 0;
//        //            value.Value3 = 0;
//        //            totalSize += 4;
//        //        }
//        //        else if (code < 0xD0)
//        //        {
//        //            //If stage 3 (28% success)
//        //            //prevTimestamp = prevTimestamp;
//        //            prevPointID += 1 + (code & 0xF);
//        //            //key.EntryNumber = 0;
//        //            if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
//        //            {
//        //                totalSize += 1;
//        //                stream += 1;
//        //                goto FilterFailed;
//        //            }
//        //            value.Value1 = 0;
//        //            value.Value2 = 0;
//        //            value.Value3 = 0;
//        //            totalSize += 1;
//        //        }
//        //        else if (code < 0xE0)
//        //        {
//        //            //If stage 4 (3% success)
//        //            //prevTimestamp = prevTimestamp;
//        //            prevPointID += 1 + (code & 0xF);
//        //            //key.EntryNumber = 0;
//        //            if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
//        //            {
//        //                totalSize += 5;
//        //                stream += 5;
//        //                goto FilterFailed;
//        //            }
//        //            value.Value1 = *(uint*)(stream + 1);
//        //            value.Value2 = 0;
//        //            value.Value3 = 0;
//        //            totalSize += 5;
//        //            //key.Timestamp = prevTimestamp;
//        //        }
//        //        else
//        //        {
//        //            //Stage 5: 2%
//        //            //Stage 5: Catch All
//        //            size = 1;
//        //            if ((code & 16) != 0) //T is set
//        //            {
//        //                m_prevTimestamp += Compression.Read7BitUInt64(stream, ref size);
//        //                prevPointID = Compression.Read7BitUInt64(stream, ref size);
//        //            }
//        //            else
//        //            {
//        //                prevPointID += Compression.Read7BitUInt64(stream, ref size);
//        //            }

//        //            if ((code & 8) != 0) //E is set)
//        //            {
//        //                key.EntryNumber = Compression.Read7BitUInt64(stream, ref size);
//        //            }
//        //            else
//        //            {
//        //                key.EntryNumber = 0;
//        //            }

//        //            if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
//        //            {
//        //                key.EntryNumber = 0;
//        //                size += 4 + ((byte)code & 4);
//        //                //if ((code & 4) != 0) //V1 is set)
//        //                //{
//        //                //    size += 8;
//        //                //}
//        //                //else
//        //                //{
//        //                //    size += 4;
//        //                //}

//        //                if ((code & 2) != 0) //V2 is set)
//        //                {
//        //                    if (stream[size] < 128) size += 1;
//        //                    else if (stream[size + 1] < 128) size += 2;
//        //                    else if (stream[size + 2] < 128) size += 3;
//        //                    else if (stream[size + 3] < 128) size += 4;
//        //                    else if (stream[size + 4] < 128) size += 5;
//        //                    else if (stream[size + 5] < 128) size += 6;
//        //                    else if (stream[size + 6] < 128) size += 7;
//        //                    else if (stream[size + 7] < 128) size += 8;
//        //                    else size += 9;
//        //                }

//        //                if ((code & 1) != 0) //V3 is set)
//        //                {
//        //                    if (stream[size] < 128) size += 1;
//        //                    else if (stream[size + 1] < 128) size += 2;
//        //                    else if (stream[size + 2] < 128) size += 3;
//        //                    else if (stream[size + 3] < 128) size += 4;
//        //                    else if (stream[size + 4] < 128) size += 5;
//        //                    else if (stream[size + 5] < 128) size += 6;
//        //                    else if (stream[size + 6] < 128) size += 7;
//        //                    else if (stream[size + 7] < 128) size += 8;
//        //                    else size += 9;
//        //                }

//        //                totalSize += size;
//        //                stream += size;
//        //                goto FilterFailed;
//        //            }


//        //            if ((code & 4) != 0) //V1 is set)
//        //            {
//        //                value.Value1 = *(ulong*)(stream + size);
//        //                size += 8;
//        //            }
//        //            else
//        //            {
//        //                value.Value1 = *(uint*)(stream + size);
//        //                size += 4;
//        //            }

//        //            if ((code & 2) != 0) //V2 is set)
//        //            {
//        //                value.Value2 = Compression.Read7BitUInt64(stream, ref size);
//        //            }
//        //            else
//        //            {
//        //                value.Value2 = 0;
//        //            }

//        //            if ((code & 1) != 0) //V3 is set)
//        //            {
//        //                value.Value3 = Compression.Read7BitUInt64(stream, ref size);
//        //            }
//        //            else
//        //            {
//        //                value.Value3 = 0;
//        //            }
//        //            totalSize += size;
//        //        }

//        //        this.IndexOfNextKeyValue = IndexOfNextKeyValue;
//        //        m_prevPointId = key.PointID;
//        //        m_prevTimestamp = key.Timestamp;
//        //        m_nextOffset += totalSize;
//        //        return true;
//        //    }
//        //}

//    }
//}