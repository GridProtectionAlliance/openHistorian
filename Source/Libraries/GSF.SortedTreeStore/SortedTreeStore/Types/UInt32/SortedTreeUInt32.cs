//******************************************************************************************************
//  SortedTreeUInt32.cs - Gbtc
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
    public class SortedTreeUInt32
        : SortedTreeTypeBase<SortedTreeUInt32>
    {

        public uint Value;

        public SortedTreeUInt32()
        {

        }
        public SortedTreeUInt32(uint value)
        {
            Value = value;
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                // {03F4BD3A-D9CF-4358-B175-A9D38BE6715A}
                return new Guid(0x03f4bd3a, 0xd9cf, 0x4358, 0xb1, 0x75, 0xa9, 0xd3, 0x8b, 0xe6, 0x71, 0x5a);
            }
        }

        public override int Size
        {
            get
            {
                return 4;
            }
        }

        public override void CopyTo(SortedTreeUInt32 destination)
        {
            destination.Value = Value;
        }

        public override int CompareTo(SortedTreeUInt32 other)
        {
            return Value.CompareTo(other.Value);
        }

        public override unsafe int CompareTo(byte* stream)
        {
            return Value.CompareTo(*(uint*)stream);
        }

        public override void SetMin()
        {
            Value = uint.MinValue;
        }

        public override void SetMax()
        {
            Value = uint.MaxValue;
        }

        public override void Clear()
        {
            Value = 0;
        }

        public override void Read(BinaryStreamBase stream)
        {
            Value = stream.ReadUInt32();
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Value);
        }

        #region [ Optional Overrides ]

        // Read(byte*)
        // Write(byte*)
        // IsLessThan(T)
        // IsEqualTo(T)
        // IsGreaterThan(T)
        // IsLessThanOrEqualTo(T)
        // IsBetween(T,T)

        public override unsafe void Read(byte* stream)
        {
            Value = *(uint*)(stream);
        }
        public override unsafe void Write(byte* stream)
        {
            *(uint*)(stream) = Value;
        }
        public override bool IsLessThan(SortedTreeUInt32 right)
        {
            return Value < right.Value;
        }
        public override bool IsEqualTo(SortedTreeUInt32 right)
        {
            return Value == right.Value;

        }
        public override bool IsGreaterThan(SortedTreeUInt32 right)
        {
            return Value > right.Value;

        }
        public override bool IsGreaterThanOrEqualTo(SortedTreeUInt32 right)
        {
            return Value >= right.Value;
        }
        public override bool IsBetween(SortedTreeUInt32 lowerBounds, SortedTreeUInt32 upperBounds)
        {
            return lowerBounds.Value <= Value && Value < upperBounds.Value;
        }

        public override SortedTreeTypeMethods<SortedTreeUInt32> CreateValueMethods()
        {
            return new SortedTreeKeyMethodsUInt32();
        }

        #endregion
    }

}