//******************************************************************************************************
//  MethodsHistorianKey.cs - Gbtc
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
    public class KeyMethodsHistorianKey
        : TreeKeyMethodsBase<HistorianKey>
    {
        // {6527D41B-9D04-4BFA-8133-05273D521D46}
        public static Guid TypeGuid = new Guid(0x6527d41b, 0x9d04, 0x4bfa, 0x81, 0x33, 0x05, 0x27, 0x3d, 0x52, 0x1d, 0x46);

        protected override int GetSize()
        {
            return 24;
        }

        public override void Clear(HistorianKey key)
        {
            key.Timestamp = 0;
            key.PointID = 0;
            key.EntryNumber = 0;
        }

        public override void SetMin(HistorianKey key)
        {
            key.Timestamp = ulong.MinValue;
            key.PointID = ulong.MinValue;
            key.EntryNumber = ulong.MinValue;
        }

        public override void SetMax(HistorianKey key)
        {
            key.Timestamp = ulong.MaxValue;
            key.PointID = ulong.MaxValue;
            key.EntryNumber = ulong.MaxValue;
        }

        public override int CompareTo(HistorianKey left, HistorianKey right)
        {
            if (left.Timestamp < right.Timestamp)
                return -1;
            if (left.Timestamp > right.Timestamp)
                return 1;
            if (left.PointID < right.PointID)
                return -1;
            if (left.PointID > right.PointID)
                return 1;
            if (left.EntryNumber < right.EntryNumber)
                return -1;
            if (left.EntryNumber > right.EntryNumber)
                return 1;

            return 0;
        }

        public override unsafe void Write(byte* stream, HistorianKey data)
        {
            *(ulong*)stream = data.Timestamp;
            *(ulong*)(stream + 8) = data.PointID;
            *(ulong*)(stream + 16) = data.EntryNumber;
        }

        public override unsafe void Read(byte* stream, HistorianKey data)
        {
            data.Timestamp = *(ulong*)stream;
            data.PointID = *(ulong*)(stream + 8);
            data.EntryNumber = *(ulong*)(stream + 16);
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                return TypeGuid;
            }
        }

        //public override bool IsBetween(HistorianKey lowerBounds, HistorianKey key, HistorianKey upperBounds)
        //{
        //    ulong key1 = key.Value1;
        //    ulong key2 = key.Value2;
        //    return (lowerBounds.Value1 < key1 || (lowerBounds.Value1 == key1 && lowerBounds.Value2 <= key2)) &&
        //           (key1 < upperBounds.Value1 || (key1 == upperBounds.Value1 && key2 < upperBounds.Value2));
        //}

        //public override bool IsLessThan(HistorianKey left, HistorianKey right)
        //{
        //    return left.Value1 < right.Value1 || (left.Value1 == right.Value1 && left.Value2 < right.Value2);
        //}
        //public override bool IsLessThanOrEqualTo(HistorianKey left, HistorianKey right)
        //{
        //    return left.Value1 < right.Value1 || (left.Value1 == right.Value1 && left.Value2 <= right.Value2);
        //}

        ////ToDo: Origional
        //    public override unsafe int BinarySearch(byte* pointer, UInt128 key, int recordCount, int keyValueSize)
        //    {
        //        int lastFoundIndex = m_lastFoundIndex;
        //        ulong key1 = key.Value1;
        //        ulong key2 = key.Value2;
        //        ulong compareKey1;
        //        ulong compareKey2;

        //        //shortcut for sequentially adding. 
        //        if (lastFoundIndex == recordCount - 1)
        //        {
        //            compareKey1 = *(ulong*)(pointer + keyValueSize * lastFoundIndex);
        //            compareKey2 = *(ulong*)(pointer + keyValueSize * lastFoundIndex + 8);
        //            if (key1 > compareKey1 || (key1 == compareKey1 && key2 > compareKey2)) //Key > CompareKey
        //            {
        //                m_lastFoundIndex++;
        //                return ~recordCount;
        //            }
        //        }
        //        //Shortcut for sequentially getting  
        //        else if (lastFoundIndex < recordCount)
        //        {
        //            compareKey1 = *(ulong*)(pointer + keyValueSize * (lastFoundIndex + 1));
        //            compareKey2 = *(ulong*)(pointer + keyValueSize * (lastFoundIndex + 1) + 8);

        //            if (key1 == compareKey1 && key2 == compareKey2)
        //            {
        //                m_lastFoundIndex++;
        //                return lastFoundIndex + 1;
        //            }
        //        }

        //        int searchLowerBoundsIndex = 0;
        //        int searchHigherBoundsIndex = recordCount - 1;
        //        while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
        //        {
        //            int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);

        //            compareKey1 = *(ulong*)(pointer + keyValueSize * currentTestIndex);
        //            compareKey2 = *(ulong*)(pointer + keyValueSize * currentTestIndex + 8);
        //            if (key1 == compareKey1 && key2 == compareKey2) //Are Equal
        //            {
        //                m_lastFoundIndex = currentTestIndex;
        //                return currentTestIndex;
        //            }
        //            if (key1 > compareKey1 || (key1 == compareKey1 && key2 > compareKey2)) //Key > CompareKey
        //                searchLowerBoundsIndex = currentTestIndex + 1;
        //            else
        //                searchHigherBoundsIndex = currentTestIndex - 1;
        //        }

        //        m_lastFoundIndex = searchLowerBoundsIndex;
        //        return ~searchLowerBoundsIndex;
        //    }
    }
}