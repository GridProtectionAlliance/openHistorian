//******************************************************************************************************
//  SortedTreeInt32.cs - Gbtc
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
    public class SnapInt32
        : SnapTypeBase<SnapInt32>
    {

        public int Value;

        public SnapInt32()
        {

        }
        public SnapInt32(int value)
        {
            Value = value;
        }

        public override Guid GenericTypeGuid =>
            // {9DCCEEBA-D191-49CC-AF03-118C0D7D221A}
            new Guid(0x9dcceeba, 0xd191, 0x49cc, 0xaf, 0x03, 0x11, 0x8c, 0x0d, 0x7d, 0x22, 0x1a);

        public override int Size => 4;

        public override void CopyTo(SnapInt32 destination)
        {
            destination.Value = Value;
        }

        public override int CompareTo(SnapInt32 other)
        {
            return Value.CompareTo(other.Value);
        }

        public override unsafe int CompareTo(byte* stream)
        {
            return Value.CompareTo(*(int*)stream);
        }

        public override void SetMin()
        {
            Value = int.MinValue;
        }

        public override void SetMax()
        {
            Value = int.MaxValue;
        }

        public override void Clear()
        {
            Value = 0;
        }

        public override void Read(BinaryStreamBase stream)
        {
            Value = stream.ReadInt32();
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
            Value = *(int*)stream;
        }
        public override unsafe void Write(byte* stream)
        {
            *(int*)stream = Value;
        }
        public override bool IsLessThan(SnapInt32 right)
        {
            return Value < right.Value;
        }
        public override bool IsEqualTo(SnapInt32 right)
        {
            return Value == right.Value;

        }
        public override bool IsGreaterThan(SnapInt32 right)
        {
            return Value > right.Value;

        }
        public override bool IsGreaterThanOrEqualTo(SnapInt32 right)
        {
            return Value >= right.Value;
        }
        public override bool IsBetween(SnapInt32 lowerBounds, SnapInt32 upperBounds)
        {
            return lowerBounds.Value <= Value && Value < upperBounds.Value;
        }

        public override SnapTypeCustomMethods<SnapInt32> CreateValueMethods()
        {
            return new SnapCustomMethodsInt32();
        }

        #endregion
    }

}