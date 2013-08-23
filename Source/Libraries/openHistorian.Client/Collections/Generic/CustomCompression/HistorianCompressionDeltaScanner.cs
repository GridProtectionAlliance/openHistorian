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
using GSF;
using GSF.IO;

namespace openHistorian.Collections.Generic.CustomCompression
{
    public unsafe class HistorianCompressionDeltaScanner
        : EncodedNodeScannerBase<HistorianKey, HistorianValue>
    {
        private readonly byte[] m_buffer;
        ulong prevTimestamp;
        ulong prevPointId;
        ulong prevEntryNumber;
        ulong prevValue1;
        ulong prevValue2;
        ulong prevValue3;

        public HistorianCompressionDeltaScanner(byte level, int blockSize, BinaryStreamBase stream, Func<HistorianKey, byte, uint> lookupKey, TreeKeyMethodsBase<HistorianKey> keyMethods, TreeValueMethodsBase<HistorianValue> valueMethods)
            : base(level, blockSize, stream, lookupKey, keyMethods, valueMethods)
        {
            m_buffer = new byte[MaximumStorageSize];
        }

        protected override unsafe int DecodeRecord(byte* stream, HistorianKey key, HistorianValue value)
        {
            int position = 0;
            prevTimestamp ^= Compression.Read7BitUInt64(stream, ref position);
            prevPointId ^= Compression.Read7BitUInt64(stream, ref position);
            prevEntryNumber ^= Compression.Read7BitUInt64(stream, ref position);
            prevValue1 ^= Compression.Read7BitUInt64(stream, ref position);
            prevValue2 ^= Compression.Read7BitUInt64(stream, ref position);
            prevValue3 ^= Compression.Read7BitUInt64(stream, ref position);

            key.Timestamp = prevTimestamp;
            key.PointID = prevPointId;
            key.EntryNumber = prevEntryNumber;
            value.Value1 = prevValue1;
            value.Value2 = prevValue2;
            value.Value3 = prevValue3;
            return position;
        }

        protected override void ResetEncoder()
        {
            prevTimestamp = 0;
            prevPointId = 0;
            prevEntryNumber = 0;
            prevValue1 = 0;
            prevValue2 = 0;
            prevValue3 = 0;
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