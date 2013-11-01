//******************************************************************************************************
//  MethodsUint32.cs - Gbtc
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
using openHistorian.Collections.Generic;

namespace openHistorian.Collections
{
    public class TreeUInt32
        : ISortedTreeKey<TreeUInt32>, ISortedTreeValue<TreeUInt32>
    {
        public TreeUInt32()
        {

        }
        public TreeUInt32(uint value)
        {
            Value = value;
        }

        public uint Value;

        public TreeKeyMethodsBase<TreeUInt32> CreateKeyMethods()
        {
            return new KeyMethodsUInt32();
        }

        public TreeValueMethodsBase<TreeUInt32> CreateValueMethods()
        {
            return new ValueMethodsUInt32();
        }
    }
    public class KeyMethodsUInt32
        : TreeKeyMethodsBase<TreeUInt32>
    {
        // {03F4BD3A-D9CF-4358-B175-A9D38BE6715A}
        public static Guid TypeGuid = new Guid(0x03f4bd3a, 0xd9cf, 0x4358, 0xb1, 0x75, 0xa9, 0xd3, 0x8b, 0xe6, 0x71, 0x5a);

        protected override int GetSize()
        {
            return 4;
        }

        public override void Clear(TreeUInt32 key)
        {
            key.Value = 0;
        }

        public override void SetMin(TreeUInt32 key)
        {
            key.Value = uint.MinValue;
        }

        public override void SetMax(TreeUInt32 key)
        {
            key.Value = uint.MaxValue;
        }

        public override int CompareTo(TreeUInt32 left, TreeUInt32 right)
        {
            return left.Value.CompareTo(right.Value);
        }

        public override unsafe void Write(byte* stream, TreeUInt32 data)
        {
            *(uint*)stream = data.Value;
        }

        public override unsafe void Read(byte* stream, TreeUInt32 data)
        {
            data.Value = *(uint*)stream;
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                return TypeGuid;
            }
        }

        public override unsafe void Copy(TreeUInt32 source, TreeUInt32 destination)
        {
            destination.Value = source.Value;
        }

        public override bool IsBetween(TreeUInt32 lowerBounds, TreeUInt32 key, TreeUInt32 upperBounds)
        {
            uint v1 = lowerBounds.Value;
            uint v2 = key.Value;
            uint v3 = upperBounds.Value;
            return v1 <= v2 & v2 < v3;
        }

        public override bool IsLessThan(TreeUInt32 left, TreeUInt32 right)
        {
            return left.Value < right.Value;
        }

        public override bool IsLessThanOrEqualTo(TreeUInt32 left, TreeUInt32 right)
        {
            return left.Value <= right.Value;
        }

        //ToDo: Origional
        public override unsafe int BinarySearch(byte* pointer, TreeUInt32 key2, int recordCount, int keyValueSize)
        {
            int lastFoundIndex = m_lastFoundIndex;
            uint key = key2.Value;

            //shortcut for sequentially adding. 
            if (lastFoundIndex == recordCount - 1)
            {
                if (key > *(uint*)(pointer + keyValueSize * lastFoundIndex)) //Key > CompareKey
                {
                    m_lastFoundIndex++;
                    return ~recordCount;
                }
            }
            //Shortcut for sequentially getting  
            else if (lastFoundIndex < recordCount)
            {
                if (key == *(uint*)(pointer + keyValueSize * (lastFoundIndex + 1)))
                {
                    m_lastFoundIndex++;
                    return lastFoundIndex + 1;
                }
            }

            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = recordCount - 1;
            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);

                uint compareKey = *(uint*)(pointer + keyValueSize * currentTestIndex);

                if (key == compareKey) //Are Equal
                {
                    m_lastFoundIndex = currentTestIndex;
                    return currentTestIndex;
                }
                if (key > compareKey) //Key > CompareKey
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            m_lastFoundIndex = searchLowerBoundsIndex;
            return ~searchLowerBoundsIndex;
        }
    }

    public class ValueMethodsUInt32
        : TreeValueMethodsBase<TreeUInt32>
    {
        // {03F4BD3A-D9CF-4358-B175-A9D38BE6715A}
        public static Guid TypeGuid = new Guid(0x03f4bd3a, 0xd9cf, 0x4358, 0xb1, 0x75, 0xa9, 0xd3, 0x8b, 0xe6, 0x71, 0x5a);

        protected override int GetSize()
        {
            return 4;
        }

        public override unsafe void Write(byte* stream, TreeUInt32 data)
        {
            *(uint*)stream = data.Value;
        }

        public override void Clear(TreeUInt32 data)
        {
            data.Value = 0;
        }

        public override unsafe void Read(byte* stream, TreeUInt32 data)
        {
            data.Value = *(uint*)stream;
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                return TypeGuid;
            }
        }

        public override unsafe void Copy(TreeUInt32 source, TreeUInt32 destination)
        {
            destination.Value = source.Value;
        }
    }
}