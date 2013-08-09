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

        public HistorianCompressionDeltaScanner(byte level, int blockSize, BinaryStreamBase stream, Func<HistorianKey, byte, uint> lookupKey, TreeKeyMethodsBase<HistorianKey> keyMethods, TreeValueMethodsBase<HistorianValue> valueMethods)
            : base(level, blockSize, stream, lookupKey, keyMethods, valueMethods)
        {
            m_buffer = new byte[MaximumStorageSize];
        }

        protected override unsafe int DecodeRecord(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey currentKey, HistorianValue currentValue)
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