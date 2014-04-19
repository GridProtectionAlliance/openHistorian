//******************************************************************************************************
//  SortedTreeEngineReaderBaseExtensionMethods.cs - Gbtc
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
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Types;

namespace GSF.SortedTreeStore.Server.Reader
{
    public static class SortedTreeEngineReaderBaseExtensionMethods
    {
        public static TreeStream<TKey, TValue> Read<TKey, TValue>(this IDatabaseReader<TKey, TValue> reader, ulong timestamp)
            where TKey : TimestampPointIDBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return reader.Read(SortedTreeEngineReaderOptions.Default, TimestampFilter.CreateFromRange<TKey>(timestamp, timestamp), null);
        }

        public static TreeStream<TKey, TValue> Read<TKey, TValue>(this IDatabaseReader<TKey, TValue> reader, SeekFilterBase<TKey> timeFilter)
            where TKey : TimestampPointIDBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return reader.Read(SortedTreeEngineReaderOptions.Default, timeFilter, null);
        }

        public static TreeStream<TKey, TValue> Read<TKey, TValue>(this IDatabaseReader<TKey, TValue> reader)
            where TKey : TimestampPointIDBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return reader.Read(SortedTreeEngineReaderOptions.Default, null, null);
        }

        public static TreeStream<TKey, TValue> Read<TKey, TValue>(this IDatabaseReader<TKey, TValue> reader, ulong firstTime, ulong lastTime)
            where TKey : TimestampPointIDBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return reader.Read(SortedTreeEngineReaderOptions.Default, TimestampFilter.CreateFromRange<TKey>(firstTime, lastTime), null);
        }

        public static TreeStream<TKey, TValue> Read<TKey, TValue>(this IDatabaseReader<TKey, TValue> reader, ulong firstTime, ulong lastTime, IEnumerable<ulong> pointIds)
            where TKey : TimestampPointIDBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return reader.Read(SortedTreeEngineReaderOptions.Default, TimestampFilter.CreateFromRange<TKey>(firstTime, lastTime), PointIDFilter.CreateFromList<TKey, TValue>(pointIds.ToList()));
        }

        public static TreeStream<TKey, TValue> Read<TKey, TValue>(this IDatabaseReader<TKey, TValue> reader, DateTime firstTime, DateTime lastTime, IEnumerable<ulong> pointIds)
            where TKey : TimestampPointIDBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return reader.Read(SortedTreeEngineReaderOptions.Default, TimestampFilter.CreateFromRange<TKey>(firstTime, lastTime), PointIDFilter.CreateFromList<TKey,TValue>(pointIds.ToList()));
        }

        public static TreeStream<TKey, TValue> Read<TKey, TValue>(this IDatabaseReader<TKey, TValue> reader, SeekFilterBase<TKey> key1, IEnumerable<ulong> pointIds)
            where TKey : TimestampPointIDBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return reader.Read(SortedTreeEngineReaderOptions.Default, key1, PointIDFilter.CreateFromList<TKey, TValue>(pointIds.ToList()));
        }

    }
}