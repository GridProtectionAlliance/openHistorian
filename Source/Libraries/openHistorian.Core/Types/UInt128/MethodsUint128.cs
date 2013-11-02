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
using GSF.IO;
using openHistorian.Collections.Generic;

namespace openHistorian.Collections
{
    public class TreeUInt128
        : ISortedTreeKey<TreeUInt128>, ISortedTreeValue<TreeUInt128>
    {
        public ulong Value1;
        public ulong Value2;
        public TreeKeyMethodsBase<TreeUInt128> CreateKeyMethods()
        {
            return new KeyMethodsUInt128();
        }

        public TreeValueMethodsBase<TreeUInt128> CreateValueMethods()
        {
            return new ValueMethodsUInt128();
        }
    }

    public class KeyMethodsUInt128
        : TreeKeyMethodsBase<TreeUInt128>
    {
        // {655BB169-45E6-4370-9E9B-417ACF445ECB}
        public static Guid TypeGuid = new Guid(0x655bb169, 0x45e6, 0x4370, 0x9e, 0x9b, 0x41, 0x7a, 0xcf, 0x44, 0x5e, 0xcb);

        protected override int GetSize()
        {
            return 16;
        }

        public override void Clear(TreeUInt128 key)
        {
            key.Value1 = 0;
            key.Value2 = 0;
        }

        public override void SetMin(TreeUInt128 key)
        {
            key.Value1 = ulong.MinValue;
            key.Value2 = ulong.MinValue;
        }

        public override void SetMax(TreeUInt128 key)
        {
            key.Value1 = ulong.MaxValue;
            key.Value2 = ulong.MaxValue;
        }

        public override int CompareTo(TreeUInt128 left, TreeUInt128 right)
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

        public override unsafe void Write(byte* stream, TreeUInt128 data)
        {
            *(ulong*)stream = data.Value1;
            *(ulong*)(stream + 8) = data.Value2;
        }

        public override unsafe void Read(byte* stream, TreeUInt128 data)
        {
            data.Value1 = *(ulong*)stream;
            data.Value2 = *(ulong*)(stream + 8);
        }
        
        public override void ReadCompressed(BinaryStreamBase stream, TreeUInt128 currentKey, TreeUInt128 previousKey)
        {
            currentKey.Value1 = stream.Read7BitUInt64() ^ previousKey.Value1;
            currentKey.Value2 = stream.Read7BitUInt64() ^ previousKey.Value2;
        }

        public override void WriteCompressed(BinaryStreamBase stream, TreeUInt128 currentKey, TreeUInt128 previousKey)
        {
            stream.Write7Bit(previousKey.Value1 ^ currentKey.Value1);
            stream.Write7Bit(previousKey.Value2 ^ currentKey.Value2);
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                return TypeGuid;
            }
        }

        public override bool IsBetween(TreeUInt128 lowerBounds, TreeUInt128 key, TreeUInt128 upperBounds)
        {
            ulong key1 = key.Value1;
            ulong key2 = key.Value2;
            return (lowerBounds.Value1 < key1 || (lowerBounds.Value1 == key1 && lowerBounds.Value2 <= key2)) &&
                   (key1 < upperBounds.Value1 || (key1 == upperBounds.Value1 && key2 < upperBounds.Value2));
        }

        public override bool IsLessThan(TreeUInt128 left, TreeUInt128 right)
        {
            return left.Value1 < right.Value1 || (left.Value1 == right.Value1 && left.Value2 < right.Value2);
        }

        public override bool IsLessThanOrEqualTo(TreeUInt128 left, TreeUInt128 right)
        {
            return left.Value1 < right.Value1 || (left.Value1 == right.Value1 && left.Value2 <= right.Value2);
        }

        ////ToDo: Origional
        public override unsafe int BinarySearch(byte* pointer, TreeUInt128 key, int recordCount, int keyValueSize)
        {
            int lastFoundIndex = LastFoundIndex;
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
                    LastFoundIndex++;
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
                    LastFoundIndex++;
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
                    LastFoundIndex = currentTestIndex;
                    return currentTestIndex;
                }
                if (key1 > compareKey1 || (key1 == compareKey1 && key2 > compareKey2)) //Key > CompareKey
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            LastFoundIndex = searchLowerBoundsIndex;
            return ~searchLowerBoundsIndex;
        }
    }

    internal class ValueMethodsUInt128
        : TreeValueMethodsBase<TreeUInt128>
    {
        // {655BB169-45E6-4370-9E9B-417ACF445ECB}
        public static Guid TypeGuid = new Guid(0x655bb169, 0x45e6, 0x4370, 0x9e, 0x9b, 0x41, 0x7a, 0xcf, 0x44, 0x5e, 0xcb);

        protected override int GetSize()
        {
            return 16;
        }

        public override void ReadCompressed(BinaryStreamBase stream, TreeUInt128 currentValue, TreeUInt128 previousValue)
        {
            currentValue.Value1 = stream.Read7BitUInt64() ^ previousValue.Value1;
            currentValue.Value2 = stream.Read7BitUInt64() ^ previousValue.Value2;
        }

        public override void WriteCompressed(BinaryStreamBase stream, TreeUInt128 currentValue, TreeUInt128 previousValue)
        {
            stream.Write7Bit(previousValue.Value1 ^ currentValue.Value1);
            stream.Write7Bit(previousValue.Value2 ^ currentValue.Value2);
        }

        public override unsafe void Write(byte* stream, TreeUInt128 data)
        {
            *(ulong*)stream = data.Value1;
            *(ulong*)(stream + 8) = data.Value2;
        }

        public override void Clear(TreeUInt128 data)
        {
            data.Value1 = 0;
            data.Value2 = 0;
        }

        public override unsafe void Read(byte* stream, TreeUInt128 data)
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