//******************************************************************************************************
//  SortedTreeUInt32.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  04/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.Snap.Types
{
    public class SnapUInt32
        : SnapTypeBase<SnapUInt32>
    {

        public uint Value;

        public SnapUInt32()
        {

        }
        public SnapUInt32(uint value)
        {
            Value = value;
        }

        public override Guid GenericTypeGuid =>
            // {03F4BD3A-D9CF-4358-B175-A9D38BE6715A}
            new Guid(0x03f4bd3a, 0xd9cf, 0x4358, 0xb1, 0x75, 0xa9, 0xd3, 0x8b, 0xe6, 0x71, 0x5a);

        public override int Size => 4;

        public override void CopyTo(SnapUInt32 destination)
        {
            destination.Value = Value;
        }

        public override int CompareTo(SnapUInt32 other)
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
            Value = *(uint*)stream;
        }
        public override unsafe void Write(byte* stream)
        {
            *(uint*)stream = Value;
        }
        public override bool IsLessThan(SnapUInt32 right)
        {
            return Value < right.Value;
        }
        public override bool IsEqualTo(SnapUInt32 right)
        {
            return Value == right.Value;

        }
        public override bool IsGreaterThan(SnapUInt32 right)
        {
            return Value > right.Value;

        }
        public override bool IsGreaterThanOrEqualTo(SnapUInt32 right)
        {
            return Value >= right.Value;
        }
        public override bool IsBetween(SnapUInt32 lowerBounds, SnapUInt32 upperBounds)
        {
            return lowerBounds.Value <= Value && Value < upperBounds.Value;
        }

        public override SnapTypeCustomMethods<SnapUInt32> CreateValueMethods()
        {
            return new SnapCustomMethodsUInt32();
        }

        #endregion
    }

}