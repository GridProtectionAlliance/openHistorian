//******************************************************************************************************
//  MethodsHistorianValue.cs - Gbtc
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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace openHistorian.Collections
{
    internal class ValueMethodsHistorianValue
        : SortedTreeValueMethodsBase<HistorianValue>
    {
        // {24DDE7DC-67F9-42B6-A11B-E27C3E62D9EF}
        public static Guid TypeGuid = new Guid(0x24dde7dc, 0x67f9, 0x42b6, 0xa1, 0x1b, 0xe2, 0x7c, 0x3e, 0x62, 0xd9, 0xef);

        protected override int GetSize()
        {
            return 24;
        }

        public override unsafe void Write(byte* stream, HistorianValue data)
        {
            *(ulong*)stream = data.Value1;
            *(ulong*)(stream + 8) = data.Value2;
            *(ulong*)(stream + 16) = data.Value3;
        }

        public override void WriteCompressed(BinaryStreamBaseOld stream, HistorianValue currentValue, HistorianValue previousValue)
        {
            stream.Write7Bit(previousValue.Value1 ^ currentValue.Value1);
            stream.Write7Bit(previousValue.Value2 ^ currentValue.Value2);
            stream.Write7Bit(previousValue.Value3 ^ currentValue.Value3);
        }

        public override void ReadCompressed(BinaryStreamBaseOld stream, HistorianValue currentValue, HistorianValue previousValue)
        {
            currentValue.Value1 = stream.Read7BitUInt64() ^ previousValue.Value1;
            currentValue.Value2 = stream.Read7BitUInt64() ^ previousValue.Value2;
            currentValue.Value3 = stream.Read7BitUInt64() ^ previousValue.Value3;
        }
        public override void WriteCompressed(BinaryStreamBase stream, HistorianValue currentValue, HistorianValue previousValue)
        {
            stream.Write7Bit(previousValue.Value1 ^ currentValue.Value1);
            stream.Write7Bit(previousValue.Value2 ^ currentValue.Value2);
            stream.Write7Bit(previousValue.Value3 ^ currentValue.Value3);
        }

        public override void ReadCompressed(BinaryStreamBase stream, HistorianValue currentValue, HistorianValue previousValue)
        {
            currentValue.Value1 = stream.Read7BitUInt64() ^ previousValue.Value1;
            currentValue.Value2 = stream.Read7BitUInt64() ^ previousValue.Value2;
            currentValue.Value3 = stream.Read7BitUInt64() ^ previousValue.Value3;
        }

        public override void Clear(HistorianValue data)
        {
            data.Value1 = 0;
            data.Value2 = 0;
            data.Value3 = 0;
        }

        public override unsafe void Read(byte* stream, HistorianValue data)
        {
            data.Value1 = *(ulong*)stream;
            data.Value2 = *(ulong*)(stream + 8);
            data.Value3 = *(ulong*)(stream + 16);
        }

        public override unsafe void Copy(HistorianValue source, HistorianValue destination)
        {
            destination.Value1 = source.Value1;
            destination.Value2 = source.Value2;
            destination.Value3 = source.Value3;
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                return TypeGuid;
            }
        }
    }
}