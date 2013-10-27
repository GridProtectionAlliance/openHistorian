//******************************************************************************************************
//  HistorianDataReaderBaseExtensionMethods.cs - Gbtc
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
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian
{
    public static class HistorianDataReaderBaseExtensionMethods
    {
        public static KeyValueStream<TKey, TValue> Read<TKey, TValue>(this HistorianDataReaderBase<TKey,TValue> reader, ulong timestamp)
            where TKey : HistorianKeyBase<TKey>, new()
            where TValue : class, new()
        {
            return reader.Read(QueryFilterTimestamp.CreateFromRange(timestamp, timestamp), QueryFilterPointId.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        public static KeyValueStream<TKey, TValue> Read<TKey, TValue>(this HistorianDataReaderBase<TKey, TValue> reader, QueryFilterTimestamp timeFilter)
            where TKey : HistorianKeyBase<TKey>, new()
            where TValue : class, new()
        {
            return reader.Read(timeFilter, QueryFilterPointId.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        public static KeyValueStream<TKey, TValue> Read<TKey, TValue>(this HistorianDataReaderBase<TKey, TValue> reader)
            where TKey : HistorianKeyBase<TKey>, new()
            where TValue : class, new()
        {
            QueryFilterTimestamp.CreateAllKeysValid();
            return reader.Read(QueryFilterTimestamp.CreateAllKeysValid(), QueryFilterPointId.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        public static KeyValueStream<TKey, TValue> Read<TKey, TValue>(this HistorianDataReaderBase<TKey, TValue> reader, ulong firstTime, ulong lastTime)
            where TKey : HistorianKeyBase<TKey>, new()
            where TValue : class, new()
        {
            return reader.Read(QueryFilterTimestamp.CreateFromRange(firstTime, lastTime), QueryFilterPointId.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        public static KeyValueStream<TKey, TValue> Read<TKey, TValue>(this HistorianDataReaderBase<TKey, TValue> reader, ulong firstTime, ulong lastTime, IEnumerable<ulong> pointIds)
            where TKey : HistorianKeyBase<TKey>, new()
            where TValue : class, new()
        {
            return reader.Read(QueryFilterTimestamp.CreateFromRange(firstTime, lastTime), QueryFilterPointId.CreateFromList(pointIds.ToList()), DataReaderOptions.Default);
        }

        public static KeyValueStream<TKey, TValue> Read<TKey, TValue>(this HistorianDataReaderBase<TKey, TValue> reader, DateTime firstTime, DateTime lastTime, IEnumerable<ulong> pointIds)
            where TKey : HistorianKeyBase<TKey>, new()
            where TValue : class, new()
        {
            return reader.Read(QueryFilterTimestamp.CreateFromRange(firstTime, lastTime), QueryFilterPointId.CreateFromList(pointIds.ToList()), DataReaderOptions.Default);
        }

        public static KeyValueStream<TKey, TValue> Read<TKey, TValue>(this HistorianDataReaderBase<TKey, TValue> reader, QueryFilterTimestamp key1, IEnumerable<ulong> pointIds)
            where TKey : HistorianKeyBase<TKey>, new()
            where TValue : class, new()
        {
            return reader.Read(key1, QueryFilterPointId.CreateFromList(pointIds.ToList()), DataReaderOptions.Default);
        }

    }
}