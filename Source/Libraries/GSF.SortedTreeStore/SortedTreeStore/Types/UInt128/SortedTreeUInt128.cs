//******************************************************************************************************
//  SortedTreeUInt128.cs - Gbtc
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
using System.Collections;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Types
{
    public class SortedTreeUInt128
        : SortedTreeTypeBase<SortedTreeUInt128>
    {
        public ulong Value1;
        public ulong Value2;
        
        public override SortedTreeTypeMethods<SortedTreeUInt128> CreateValueMethods()
        {
            return new SortedTreeKeyMethodsUInt128();
        }

        public override IEnumerable GetEncodingMethods()
        {
            return null;
        }

        public override int CompareTo(SortedTreeUInt128 other)
        {
            if (Value1 < other.Value1)
                return -1;
            if (Value1 > other.Value1)
                return 1;
            if (Value2 < other.Value2)
                return -1;
            if (Value2 > other.Value2)
                return 1;
            return 0;
        }

        public override void SetMin()
        {
            Value1 = ulong.MinValue;
            Value2 = ulong.MinValue;
        }

        public override void SetMax()
        {
            Value1 = ulong.MaxValue;
            Value2 = ulong.MaxValue;
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                // {655BB169-45E6-4370-9E9B-417ACF445ECB}
                return new Guid(0x655bb169, 0x45e6, 0x4370, 0x9e, 0x9b, 0x41, 0x7a, 0xcf, 0x44, 0x5e, 0xcb);
            }
        }

        public override int GetSize
        {
            get
            {
                return 16;
            }
        }

        public override void Clear()
        {
            Value1 = 0;
            Value2 = 0;
        }

        public override void Read(BinaryStreamBase stream)
        {
            Value1 = stream.ReadUInt64();
            Value2 = stream.ReadUInt64();
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Value1);
            stream.Write(Value2);
        }
    }

}