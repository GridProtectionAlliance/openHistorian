//******************************************************************************************************
//  HistorianCompressionTsScanner.cs - Gbtc
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
using GSF.Collections;
using GSF.IO;
using GSF.SortedTreeStore.Filters;
using openHistorian.Collections;

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    /// <summary>
    /// A custom encoder that can highly compress time series data.
    /// </summary>
    public unsafe class HistorianCompressionTsScanner
        : EncodedNodeScannerBase<HistorianKey, HistorianValue>
    {
        //public static int Stage1=0;
        //public static int Stage2=0;
        //public static int Stage3=0;
        //public static int Stage4 = 0;
        //public static int Stage5 = 0;

        ulong m_prevTimestamp;
        ulong m_prevPointId;

        /// <summary>
        /// Creates a new class
        /// </summary>
        /// <param name="level"></param>
        /// <param name="blockSize"></param>
        /// <param name="stream"></param>
        /// <param name="lookupKey"></param>
        public HistorianCompressionTsScanner(byte level, int blockSize, BinaryStreamBase stream, Func<HistorianKey, byte, uint> lookupKey)
            : base(level, blockSize, stream, lookupKey, 2)
        {
        }

        int InternalDecode(byte* stream, HistorianKey key, HistorianValue value, KeyMatchFilterBase<HistorianKey> filter)
        {
            int totalSize = 0;
            int size = 0;

            key.Timestamp = m_prevTimestamp;
            key.PointID = m_prevPointId;
            key.EntryNumber = 0;

            //ulong prevTimestamp = m_prevTimestamp;
        //ulong prevPointId = m_prevPointId;

        FilterFailed:
            IndexOfNextKeyValue++;
            if (IndexOfNextKeyValue > RecordCount)
            {
                goto FilterSuccess;
            }

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
                //key.Timestamp = prevTimestamp;
                key.PointID += 1 + ((code >> 4) & 0x7);
                //key.EntryNumber = 0;
                if (!filter.Contains(key))
                {
                    totalSize += 4;
                    stream += 4;
                    goto FilterFailed;
                }
                value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
                value.Value2 = 0;
                value.Value3 = 0;
                totalSize += 4;
            }
            else if (code < 0xC0)
            {
                //If stage 2 (16% success)
                //key.Timestamp = prevTimestamp;
                key.PointID += 1 + ((code >> 4) & 0x3);
                //key.EntryNumber = 0;
                if (!filter.Contains(key))
                {
                    totalSize += 4;
                    stream += 4;
                    goto FilterFailed;
                }
                value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
                value.Value2 = 0;
                value.Value3 = 0;
                totalSize += 4;
            }
            else if (code < 0xD0)
            {
                //If stage 3 (28% success)
                //prevTimestamp = prevTimestamp;
                key.PointID += 1 + (code & 0xF);
                //key.EntryNumber = 0;
                if (!filter.Contains(key))
                {
                    totalSize += 1;
                    stream += 1;
                    goto FilterFailed;
                }
                value.Value1 = 0;
                value.Value2 = 0;
                value.Value3 = 0;
                totalSize += 1;
            }
            else if (code < 0xE0)
            {
                //If stage 4 (3% success)
                //prevTimestamp = prevTimestamp;
                key.PointID += 1 + (code & 0xF);
                //key.EntryNumber = 0;
                if (!filter.Contains(key))
                {
                    totalSize += 5;
                    stream += 5;
                    goto FilterFailed;
                }
                value.Value1 = *(uint*)(stream + 1);
                value.Value2 = 0;
                value.Value3 = 0;
                totalSize += 5;
                //key.Timestamp = prevTimestamp;
            }
            else
            {
                //Stage 5: 2%
                //Stage 5: Catch All
                size = 1;
                if ((code & 16) != 0) //T is set
                {
                    key.Timestamp += Compression.Read7BitUInt64(stream, ref size);
                    key.PointID = Compression.Read7BitUInt64(stream, ref size);
                }
                else
                {
                    key.PointID += Compression.Read7BitUInt64(stream, ref size);
                }

                if ((code & 8) != 0) //E is set)
                {
                    key.EntryNumber = Compression.Read7BitUInt64(stream, ref size);
                }
                else
                {
                    key.EntryNumber = 0;
                }

                if (!filter.Contains(key))
                {
                    key.EntryNumber = 0;
                    size += 4 + ((byte)code & 4);
                    //if ((code & 4) != 0) //V1 is set)
                    //{
                    //    size += 8;
                    //}
                    //else
                    //{
                    //    size += 4;
                    //}

                    if ((code & 2) != 0) //V2 is set)
                    {
                        size += Compression.Measure7BitUInt64(stream, size);
                    }

                    if ((code & 1) != 0) //V3 is set)
                    {
                        size += Compression.Measure7BitUInt64(stream, size);
                    }

                    totalSize += size;
                    stream += size;
                    goto FilterFailed;
                }


                if ((code & 4) != 0) //V1 is set)
                {
                    value.Value1 = *(ulong*)(stream + size);
                    size += 8;
                }
                else
                {
                    value.Value1 = *(uint*)(stream + size);
                    size += 4;
                }

                if ((code & 2) != 0) //V2 is set)
                {
                    value.Value2 = Compression.Read7BitUInt64(stream, ref size);
                }
                else
                {
                    value.Value2 = 0;
                }

                if ((code & 1) != 0) //V3 is set)
                {
                    value.Value3 = Compression.Read7BitUInt64(stream, ref size);
                }
                else
                {
                    value.Value3 = 0;
                }
                totalSize += size;
            }

        FilterSuccess:
            m_prevPointId = key.PointID;
            m_prevTimestamp = key.Timestamp;
            return totalSize;
        }

        protected override unsafe int DecodeRecord(byte* stream, HistorianKey key, HistorianValue value, KeyMatchFilterBase<HistorianKey> filter)
        {
            var bitArray = filter as PointIDFilter.BitArrayFilter<HistorianKey>;
            if (bitArray != null)
                return DecodeBitArray(stream, key, value, bitArray);
            return InternalDecode(stream, key, value, filter);
        }
        
        //0.614
        unsafe int DecodeBitArray(byte* stream, HistorianKey key, HistorianValue value, PointIDFilter.BitArrayFilter<HistorianKey> filter)
        {
            fixed (long* array = filter.ArrayBits)
            {
                ulong prevPointID = m_prevPointId;
                int totalSize = 0;
                int size = 0;
                uint maxValue = (uint)filter.MaxValue;
                int IndexOfNextKeyValue = this.IndexOfNextKeyValue;

                key.EntryNumber = 0;

            FilterFailed:
                IndexOfNextKeyValue++;
                if (IndexOfNextKeyValue > RecordCount)
                {
                    goto FilterSuccess;
                }

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
                    //key.Timestamp = prevTimestamp;
                    prevPointID += 1 + ((code >> 4) & 0x7);
                    //key.EntryNumber = 0;
                    if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
                    {
                        totalSize += 4;
                        stream += 4;
                        goto FilterFailed;
                    }
                    value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
                    value.Value2 = 0;
                    value.Value3 = 0;
                    totalSize += 4;
                }
                else if (code < 0xC0)
                {
                    //If stage 2 (16% success)
                    //key.Timestamp = prevTimestamp;
                    prevPointID += 1 + ((code >> 4) & 0x3);
                    //key.EntryNumber = 0;
                    if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
                    {
                        totalSize += 4;
                        stream += 4;
                        goto FilterFailed;
                    }
                    value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
                    value.Value2 = 0;
                    value.Value3 = 0;
                    totalSize += 4;
                }
                else if (code < 0xD0)
                {
                    //If stage 3 (28% success)
                    //prevTimestamp = prevTimestamp;
                    prevPointID += 1 + (code & 0xF);
                    //key.EntryNumber = 0;
                    if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
                    {
                        totalSize += 1;
                        stream += 1;
                        goto FilterFailed;
                    }
                    value.Value1 = 0;
                    value.Value2 = 0;
                    value.Value3 = 0;
                    totalSize += 1;
                }
                else if (code < 0xE0)
                {
                    //If stage 4 (3% success)
                    //prevTimestamp = prevTimestamp;
                    prevPointID += 1 + (code & 0xF);
                    //key.EntryNumber = 0;
                    if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
                    {
                        totalSize += 5;
                        stream += 5;
                        goto FilterFailed;
                    }
                    value.Value1 = *(uint*)(stream + 1);
                    value.Value2 = 0;
                    value.Value3 = 0;
                    totalSize += 5;
                    //key.Timestamp = prevTimestamp;
                }
                else
                {
                    //Stage 5: 2%
                    //Stage 5: Catch All
                    size = 1;
                    if ((code & 16) != 0) //T is set
                    {
                        m_prevTimestamp += Compression.Read7BitUInt64(stream, ref size);
                        prevPointID = Compression.Read7BitUInt64(stream, ref size);
                    }
                    else
                    {
                        prevPointID += Compression.Read7BitUInt64(stream, ref size);
                    }

                    if ((code & 8) != 0) //E is set)
                    {
                        key.EntryNumber = Compression.Read7BitUInt64(stream, ref size);
                    }
                    else
                    {
                        key.EntryNumber = 0;
                    }

                    if (!(prevPointID <= maxValue && ((array[(int)prevPointID >> BitArray.BitsPerElementShift] & (1L << ((int)prevPointID & BitArray.BitsPerElementMask))) != 0)))
                    {
                        key.EntryNumber = 0;
                        size += 4 + ((byte)code & 4);
                        //if ((code & 4) != 0) //V1 is set)
                        //{
                        //    size += 8;
                        //}
                        //else
                        //{
                        //    size += 4;
                        //}

                        if ((code & 2) != 0) //V2 is set)
                        {
                            size += Compression.Measure7BitUInt64(stream, size);
                        }

                        if ((code & 1) != 0) //V3 is set)
                        {
                            size += Compression.Measure7BitUInt64(stream, size);
                        }

                        totalSize += size;
                        stream += size;
                        goto FilterFailed;
                    }


                    if ((code & 4) != 0) //V1 is set)
                    {
                        value.Value1 = *(ulong*)(stream + size);
                        size += 8;
                    }
                    else
                    {
                        value.Value1 = *(uint*)(stream + size);
                        size += 4;
                    }

                    if ((code & 2) != 0) //V2 is set)
                    {
                        value.Value2 = Compression.Read7BitUInt64(stream, ref size);
                    }
                    else
                    {
                        value.Value2 = 0;
                    }

                    if ((code & 1) != 0) //V3 is set)
                    {
                        value.Value3 = Compression.Read7BitUInt64(stream, ref size);
                    }
                    else
                    {
                        value.Value3 = 0;
                    }
                    totalSize += size;
                }

            FilterSuccess:
                this.IndexOfNextKeyValue = IndexOfNextKeyValue;
                m_prevPointId = prevPointID;
                key.PointID = prevPointID;
                key.Timestamp = m_prevTimestamp;
                return totalSize;
            }
        }

        //protected override unsafe int DecodeRecord(byte* stream, HistorianKey key, HistorianValue value, StreamFilterBase<HistorianKey, HistorianValue> filter)
        //{
        //    int totalSize = 0;
        //    int size;
        //    key.PointID = m_prevPointId;
        //    key.Timestamp = m_prevTimestamp;

        //    //ulong prevTimestamp = m_prevTimestamp;
        ////ulong prevPointId = m_prevPointId;


        //TryAgain:
        //    IndexOfNextKeyValue++;

        //    uint code = stream[0];
        //    //Compression Stages:
        //    //  Stage 1: Big Positive Float. 
        //    //  Stage 2: Big Negative Float.
        //    //  Stage 3: Zero
        //    //  Stage 4: 32 bit
        //    //  Stage 5: Catch all

        //    if (code < 0x80)
        //    {
        //        //If stage 1 (50% success)
        //        //key.Timestamp = prevTimestamp;
        //        key.PointID += 1 + ((code >> 4) & 0x7);
        //        key.EntryNumber = 0;
        //        value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
        //        value.Value2 = 0;
        //        value.Value3 = 0;
        //        size = 4;
        //    }
        //    else if (code < 0xC0)
        //    {
        //        //If stage 2 (16% success)
        //        //key.Timestamp = prevTimestamp;
        //        key.PointID += 1 + ((code >> 4) & 0x3);
        //        key.EntryNumber = 0;
        //        value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
        //        value.Value2 = 0;
        //        value.Value3 = 0;

        //        size = 4;
        //    }
        //    else if (code < 0xD0)
        //    {
        //        //If stage 3 (28% success)
        //        //prevTimestamp = prevTimestamp;
        //        key.PointID += 1 + (code & 0xF);
        //        key.EntryNumber = 0;
        //        value.Value1 = 0;
        //        value.Value2 = 0;
        //        value.Value3 = 0;
        //        size = 1;
        //    }
        //    else if (code < 0xE0)
        //    {
        //        //If stage 4 (3% success)
        //        //prevTimestamp = prevTimestamp;
        //        key.PointID += 1 + (code & 0xF);
        //        key.EntryNumber = 0;
        //        value.Value1 = *(uint*)(stream + 1);
        //        value.Value2 = 0;
        //        value.Value3 = 0;
        //        //key.Timestamp = prevTimestamp;
        //        size = 5;
        //    }
        //    else
        //    {
        //        //Stage 5: 2%
        //        //Stage 5: Catch All
        //        size = 1;
        //        if ((code & 16) != 0) //T is set
        //        {
        //            key.Timestamp += Compression.Read7BitUInt64(stream, ref size);
        //            key.PointID = Compression.Read7BitUInt64(stream, ref size);
        //        }
        //        else
        //        {
        //            key.PointID += Compression.Read7BitUInt64(stream, ref size);
        //        }

        //        if ((code & 8) != 0) //E is set)
        //        {
        //            key.EntryNumber = Compression.Read7BitUInt64(stream, ref size);
        //        }
        //        else
        //        {
        //            key.EntryNumber = 0;
        //        }

        //        if ((code & 4) != 0) //V1 is set)
        //        {
        //            value.Value1 = *(ulong*)(stream + size);
        //            size += 8;
        //        }
        //        else
        //        {
        //            value.Value1 = *(uint*)(stream + size);
        //            size += 4;
        //        }

        //        if ((code & 2) != 0) //V2 is set)
        //        {
        //            value.Value2 = Compression.Read7BitUInt64(stream, ref size);
        //        }
        //        else
        //        {
        //            value.Value2 = 0;
        //        }

        //        if ((code & 1) != 0) //V3 is set)
        //        {
        //            value.Value3 = Compression.Read7BitUInt64(stream, ref size);
        //        }
        //        else
        //        {
        //            value.Value3 = 0;
        //        }
        //    }


        //    if (filter.StopReading(key, value))
        //    {
        //        goto FilterSuccess;
        //    }

        //FilterFailed:
        //    if (IndexOfNextKeyValue < RecordCount)
        //    {
        //        //If we can try again, then do it.
        //        totalSize += size;
        //        stream += size;
        //        goto TryAgain;
        //    }

        //    IndexOfNextKeyValue++;

        //FilterSuccess:
        //    totalSize += size;
        //    m_prevPointId = key.PointID;
        //    m_prevTimestamp = key.Timestamp;
        //    return totalSize;
        //}



        //protected override unsafe int DecodeRecord(byte* stream, HistorianKey key, HistorianValue value, StreamFilterBase<HistorianKey, HistorianValue> filter)
        //{
        //    int totalSize = 0;
        //    int size;
        //    ulong prevTimestamp = m_prevTimestamp;
        //    ulong prevPointId = m_prevPointId;


        //TryAgain:
        //    IndexOfNextKeyValue++;

        //    uint code = stream[0];
        //    //Compression Stages:
        //    //  Stage 1: Big Positive Float. 
        //    //  Stage 2: Big Negative Float.
        //    //  Stage 3: Zero
        //    //  Stage 4: 32 bit
        //    //  Stage 5: Catch all

        //    if (code < 0x80)
        //    {
        //        //If stage 1 (50% success)
        //        //prevTimestamp = prevTimestamp;
        //        prevPointId += 1 + ((code >> 4) & 0x7);
        //        key.EntryNumber = 0;
        //        value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
        //        value.Value2 = 0;
        //        value.Value3 = 0;
        //        key.Timestamp = prevTimestamp;
        //        key.PointID = prevPointId;
        //        size = 4;
        //    }
        //    else if (code < 0xC0)
        //    {
        //        //If stage 2 (16% success)
        //        //prevTimestamp = prevTimestamp;
        //        prevPointId += 1 + ((code >> 4) & 0x3);
        //        key.EntryNumber = 0;
        //        value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
        //        value.Value2 = 0;
        //        value.Value3 = 0;
        //        key.Timestamp = prevTimestamp;
        //        key.PointID = prevPointId;
        //        size = 4;
        //    }
        //    else if (code < 0xD0)
        //    {
        //        //If stage 3 (28% success)
        //        //prevTimestamp = prevTimestamp;
        //        prevPointId += 1 + (code & 0xF);
        //        key.EntryNumber = 0;
        //        value.Value1 = 0;
        //        value.Value2 = 0;
        //        value.Value3 = 0;
        //        key.Timestamp = prevTimestamp;
        //        key.PointID = prevPointId;
        //        size = 1;
        //    }
        //    else if (code < 0xE0)
        //    {
        //        //If stage 4 (3% success)
        //        //prevTimestamp = prevTimestamp;
        //        prevPointId += 1 + (code & 0xF);
        //        key.EntryNumber = 0;
        //        value.Value1 = *(uint*)(stream + 1);
        //        value.Value2 = 0;
        //        value.Value3 = 0;
        //        key.Timestamp = prevTimestamp;
        //        key.PointID = prevPointId;
        //        size = 5;
        //    }
        //    else
        //    {
        //        //Stage 5: 2%
        //        //Stage 5: Catch All
        //        size = 1;
        //        if ((code & 16) != 0) //T is set
        //        {
        //            prevTimestamp += Compression.Read7BitUInt64(stream, ref size);
        //            prevPointId = Compression.Read7BitUInt64(stream, ref size);
        //        }
        //        else
        //        {
        //            //prevTimestamp = prevTimestamp;
        //            prevPointId += Compression.Read7BitUInt64(stream, ref size);
        //        }


        //        if ((code & 8) != 0) //E is set)
        //        {
        //            key.EntryNumber = Compression.Read7BitUInt64(stream, ref size);
        //        }
        //        else
        //        {
        //            key.EntryNumber = 0;
        //        }

        //        if ((code & 4) != 0) //V1 is set)
        //        {
        //            value.Value1 = *(ulong*)(stream + size);
        //            size += 8;
        //        }
        //        else
        //        {
        //            value.Value1 = *(uint*)(stream + size);
        //            size += 4;
        //        }

        //        if ((code & 2) != 0) //V2 is set)
        //        {
        //            value.Value2 = Compression.Read7BitUInt64(stream, ref size);
        //        }
        //        else
        //        {
        //            value.Value2 = 0;
        //        }

        //        if ((code & 1) != 0) //V3 is set)
        //        {
        //            value.Value3 = Compression.Read7BitUInt64(stream, ref size);
        //        }
        //        else
        //        {
        //            value.Value3 = 0;
        //        }

        //        key.Timestamp = prevTimestamp;
        //        key.PointID = prevPointId;
        //    }

        //    totalSize += size;

        //    if (filter.StopReading(key, value))
        //    {
        //        m_prevPointId = prevPointId;
        //        m_prevTimestamp = prevTimestamp;
        //        return totalSize;
        //    }
        //    if (IndexOfNextKeyValue < RecordCount)
        //    {
        //        //If we can try again, then do it.
        //        stream += size;
        //        goto TryAgain;
        //    }

        //    m_prevPointId = prevPointId;
        //    m_prevTimestamp = prevTimestamp;
        //    IndexOfNextKeyValue++;
        //    return totalSize;
        //}

        /// <summary>
        /// Decodes the next record from the byte array into the provided key and value.
        /// </summary>
        /// <param name="stream">the start of the next record.</param>
        /// <param name="key">the key to write to.</param>
        /// <param name="value">the value to write to.</param>
        /// <returns></returns>
        protected override int DecodeRecord(byte* stream, HistorianKey key, HistorianValue value)
        {
            int size;
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
                //prevTimestamp = prevTimestamp;
                m_prevPointId += 1 + ((code >> 4) & 0x7);
                key.EntryNumber = 0;
                value.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
                value.Value2 = 0;
                value.Value3 = 0;
                key.Timestamp = m_prevTimestamp;
                key.PointID = m_prevPointId;
                return 4;
            }
            if (code < 0xC0)
            {
                //If stage 2 (16% success)
                //prevTimestamp = prevTimestamp;
                m_prevPointId += 1 + ((code >> 4) & 0x3);
                key.EntryNumber = 0;
                value.Value1 = (12u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
                value.Value2 = 0;
                value.Value3 = 0;
                key.Timestamp = m_prevTimestamp;
                key.PointID = m_prevPointId;
                return 4;
            }
            if (code < 0xD0)
            {
                //If stage 3 (28% success)
                //prevTimestamp = prevTimestamp;
                m_prevPointId += 1 + (code & 0xF);
                key.EntryNumber = 0;
                value.Value1 = 0;
                value.Value2 = 0;
                value.Value3 = 0;
                key.Timestamp = m_prevTimestamp;
                key.PointID = m_prevPointId;
                return 1;
            }
            if (code < 0xE0)
            {
                //If stage 4 (3% success)
                //prevTimestamp = prevTimestamp;
                m_prevPointId += 1 + (code & 0xF);
                key.EntryNumber = 0;
                value.Value1 = *(uint*)(stream + 1);
                value.Value2 = 0;
                value.Value3 = 0;
                key.Timestamp = m_prevTimestamp;
                key.PointID = m_prevPointId;
                return 5;
            }

            //Stage 5: 2%
            //Stage 5: Catch All
            size = 1;
            if ((code & 16) != 0) //T is set
            {
                m_prevTimestamp += Compression.Read7BitUInt64(stream, ref size);
                m_prevPointId = Compression.Read7BitUInt64(stream, ref size);
            }
            else
            {
                //prevTimestamp = prevTimestamp;
                m_prevPointId += Compression.Read7BitUInt64(stream, ref size);
            }


            if ((code & 8) != 0) //E is set)
            {
                key.EntryNumber = Compression.Read7BitUInt64(stream, ref size);
            }
            else
            {
                key.EntryNumber = 0;
            }

            if ((code & 4) != 0) //V1 is set)
            {
                value.Value1 = *(ulong*)(stream + size);
                size += 8;
            }
            else
            {
                value.Value1 = *(uint*)(stream + size);
                size += 4;
            }

            if ((code & 2) != 0) //V2 is set)
            {
                value.Value2 = Compression.Read7BitUInt64(stream, ref size);
            }
            else
            {
                value.Value2 = 0;
            }

            if ((code & 1) != 0) //V3 is set)
            {
                value.Value3 = Compression.Read7BitUInt64(stream, ref size);
            }
            else
            {
                value.Value3 = 0;
            }

            key.Timestamp = m_prevTimestamp;
            key.PointID = m_prevPointId;
            return size;
        }

        /// <summary>
        /// Occurs when a new node has been reached and any encoded data that has been generated needs to be cleared.
        /// </summary>
        protected override void ResetEncoder()
        {
            m_prevTimestamp = 0;
            m_prevPointId = 0;
        }



    }
}