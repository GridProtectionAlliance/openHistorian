//******************************************************************************************************
//  SortedTreeValueMethodsUInt128.cs - Gbtc
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

namespace GSF.SortedTreeStore.Types
{
    internal class SortedTreeValueMethodsUInt128
        : SortedTreeValueMethodsBase<SortedTreeUInt128>
    {
        // {655BB169-45E6-4370-9E9B-417ACF445ECB}
        public static Guid TypeGuid = new Guid(0x655bb169, 0x45e6, 0x4370, 0x9e, 0x9b, 0x41, 0x7a, 0xcf, 0x44, 0x5e, 0xcb);

        protected override int GetSize()
        {
            return 16;
        }

        public override void ReadCompressed(BinaryStreamBaseOld stream, SortedTreeUInt128 currentValue, SortedTreeUInt128 previousValue)
        {
            currentValue.Value1 = stream.Read7BitUInt64() ^ previousValue.Value1;
            currentValue.Value2 = stream.Read7BitUInt64() ^ previousValue.Value2;
        }

        public override void WriteCompressed(BinaryStreamBaseOld stream, SortedTreeUInt128 currentValue, SortedTreeUInt128 previousValue)
        {
            stream.Write7Bit(previousValue.Value1 ^ currentValue.Value1);
            stream.Write7Bit(previousValue.Value2 ^ currentValue.Value2);
        }

        public override void ReadCompressed(BinaryStreamBase stream, SortedTreeUInt128 currentValue, SortedTreeUInt128 previousValue)
        {
            currentValue.Value1 = stream.Read7BitUInt64() ^ previousValue.Value1;
            currentValue.Value2 = stream.Read7BitUInt64() ^ previousValue.Value2;
        }

        public override void WriteCompressed(BinaryStreamBase stream, SortedTreeUInt128 currentValue, SortedTreeUInt128 previousValue)
        {
            stream.Write7Bit(previousValue.Value1 ^ currentValue.Value1);
            stream.Write7Bit(previousValue.Value2 ^ currentValue.Value2);
        }

        public override unsafe void Write(byte* stream, SortedTreeUInt128 data)
        {
            *(ulong*)stream = data.Value1;
            *(ulong*)(stream + 8) = data.Value2;
        }

        public override void Clear(SortedTreeUInt128 data)
        {
            data.Value1 = 0;
            data.Value2 = 0;
        }

        public override unsafe void Read(byte* stream, SortedTreeUInt128 data)
        {
            data.Value1 = *(ulong*)stream;
            data.Value2 = *(ulong*)(stream + 8);
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