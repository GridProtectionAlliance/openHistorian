//******************************************************************************************************
//  MethodsUint128.cs - Gbtc
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
    public class UInt128
    {
        public ulong Value1;
        public ulong Value2;
    }

    public class KeyMethodsUInt128
        : TreeKeyMethodsBase<UInt128>
    {
        // {655BB169-45E6-4370-9E9B-417ACF445ECB}
        public static Guid TypeGuid = new Guid(0x655bb169, 0x45e6, 0x4370, 0x9e, 0x9b, 0x41, 0x7a, 0xcf, 0x44, 0x5e, 0xcb);

        protected override int GetSize()
        {
            return 16;
        }

        public override void Clear(UInt128 key)
        {
            key.Value1 = 0;
            key.Value2 = 0;
        }

        public override void SetMin(UInt128 key)
        {
            key.Value1 = ulong.MinValue;
            key.Value2 = ulong.MinValue;
        }

        public override void SetMax(UInt128 key)
        {
            key.Value1 = ulong.MaxValue;
            key.Value2 = ulong.MaxValue;
        }

        public override int CompareTo(UInt128 left, UInt128 right)
        {
            if (left.Value1 < right.Value1)
                return -1;
            if (left.Value1 > right.Value1)
                return 1;
            if (left.Value2 < right.Value2)
                return -1;
            if (left.Value2 > right.Value2)
                return 1;
            return 0;
        }

        public override unsafe void Write(byte* stream, UInt128 data)
        {
            *(ulong*)stream = data.Value1;
            *(ulong*)(stream + 8) = data.Value2;
        }

        public override unsafe void Read(byte* stream, UInt128 data)
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

        public override bool IsBetween(UInt128 lowerBounds, UInt128 key, UInt128 upperBounds)
        {
            ulong key1 = key.Value1;
            ulong key2 = key.Value2;
            return (lowerBounds.Value1 < key1 || (lowerBounds.Value1 == key1 && lowerBounds.Value2 <= key2)) &&
                   (key1 < upperBounds.Value1 || (key1 == upperBounds.Value1 && key2 < upperBounds.Value2));
        }

        public override bool IsLessThan(UInt128 left, UInt128 right)
        {
            return left.Value1 < right.Value1 || (left.Value1 == right.Value1 && left.Value2 < right.Value2);
        }

        public override bool IsLessThanOrEqualTo(UInt128 left, UInt128 right)
        {
            return left.Value1 < right.Value1 || (left.Value1 == right.Value1 && left.Value2 <= right.Value2);
        }

        ////ToDo: Origional
        public override unsafe int BinarySearch(byte* pointer, UInt128 key, int recordCount, int keyValueSize)
        {
            int lastFoundIndex = m_lastFoundIndex;
            ulong key1 = key.Value1;
            ulong key2 = key.Value2;
            ulong compareKey1;
            ulong compareKey2;

            //shortcut for sequentially adding. 
            if (lastFoundIndex == recordCount - 1)
            {
                compareKey1 = *(ulong*)(pointer + keyValueSize * lastFoundIndex);
                compareKey2 = *(ulong*)(pointer + keyValueSize * lastFoundIndex + 8);
                if (key1 > compareKey1 || (key1 == compareKey1 && key2 > compareKey2)) //Key > CompareKey
                {
                    m_lastFoundIndex++;
                    return ~recordCount;
                }
            }
                //Shortcut for sequentially getting  
            else if (lastFoundIndex < recordCount)
            {
                compareKey1 = *(ulong*)(pointer + keyValueSize * (lastFoundIndex + 1));
                compareKey2 = *(ulong*)(pointer + keyValueSize * (lastFoundIndex + 1) + 8);

                if (key1 == compareKey1 && key2 == compareKey2)
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

                compareKey1 = *(ulong*)(pointer + keyValueSize * currentTestIndex);
                compareKey2 = *(ulong*)(pointer + keyValueSize * currentTestIndex + 8);
                if (key1 == compareKey1 && key2 == compareKey2) //Are Equal
                {
                    m_lastFoundIndex = currentTestIndex;
                    return currentTestIndex;
                }
                if (key1 > compareKey1 || (key1 == compareKey1 && key2 > compareKey2)) //Key > CompareKey
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            m_lastFoundIndex = searchLowerBoundsIndex;
            return ~searchLowerBoundsIndex;
        }
    }

    internal class ValueMethodsUInt128
        : TreeValueMethodsBase<UInt128>
    {
        // {655BB169-45E6-4370-9E9B-417ACF445ECB}
        public static Guid TypeGuid = new Guid(0x655bb169, 0x45e6, 0x4370, 0x9e, 0x9b, 0x41, 0x7a, 0xcf, 0x44, 0x5e, 0xcb);

        protected override int GetSize()
        {
            return 16;
        }

        public override unsafe void Write(byte* stream, UInt128 data)
        {
            *(ulong*)stream = data.Value1;
            *(ulong*)(stream + 8) = data.Value2;
        }

        public override void Clear(UInt128 data)
        {
            data.Value1 = 0;
            data.Value2 = 0;
        }

        public override unsafe void Read(byte* stream, UInt128 data)
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