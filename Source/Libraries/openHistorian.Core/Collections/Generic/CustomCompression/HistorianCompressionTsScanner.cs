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
using GSF;
using GSF.IO;

namespace openHistorian.Collections.Generic.CustomCompression
{
    public unsafe class HistorianCompressionTsScanner
        : EncodedNodeScannerBase<HistorianKey, HistorianValue>
    {
        //public static int Stage1=0;
        //public static int Stage2=0;
        //public static int Stage3=0;
        //public static int Stage4 = 0;
        //public static int Stage5 = 0;

        private readonly byte[] m_buffer;

        public HistorianCompressionTsScanner(byte level, int blockSize, BinaryStreamBase stream, Func<HistorianKey, byte, uint> lookupKey, TreeKeyMethodsBase<HistorianKey> keyMethods, TreeValueMethodsBase<HistorianValue> valueMethods)
            : base(level, blockSize, stream, lookupKey, keyMethods, valueMethods)
        {
            m_buffer = new byte[MaximumStorageSize];
        }

        protected override unsafe int DecodeRecord(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey currentKey, HistorianValue currentValue)
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
                currentValue.Value1 = (4u << 28) | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
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

        private int MaximumStorageSize
        {
            get
            {
                return KeyValueSize + 6;
            }
        }

        protected override unsafe byte Version
        {
            get
            {
                return 2;
            }
        }
    }
}