//******************************************************************************************************
//  FixedSizeNodeScanner`2.cs - Gbtc
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
//  04/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.Snap.Filters;

namespace GSF.Snap.Tree
{
    /// <summary>
    /// The treescanner for a fixed size node.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FixedSizeNodeScanner<TKey, TValue>
        : SortedTreeScannerBase<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private readonly int m_keyValueSize;

        /// <summary>
        /// creates a new class
        /// </summary>
        /// <param name="level"></param>
        /// <param name="blockSize"></param>
        /// <param name="stream"></param>
        /// <param name="lookupKey"></param>
        public FixedSizeNodeScanner(byte level, int blockSize, BinaryStreamPointerBase stream, Func<TKey, byte, uint> lookupKey)
            : base(level, blockSize, stream, lookupKey)
        {
            m_keyValueSize = KeySize + ValueSize;
        }


        protected override unsafe void InternalRead(TKey key, TValue value)
        {
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            key.Read(ptr);
            value.Read(ptr + KeySize);
            IndexOfNextKeyValue++;
        }

        protected override unsafe bool InternalRead(TKey key, TValue value, MatchFilterBase<TKey, TValue> filter)
        {
        TryAgain:
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            key.Read(ptr);
            value.Read(ptr + KeySize);
            IndexOfNextKeyValue++;
            if (filter.Contains(key, value))
                return true;
            if (IndexOfNextKeyValue >= RecordCount)
                return false;
            goto TryAgain;
        }

        protected override unsafe void InternalPeek(TKey key, TValue value)
        {
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            key.Read(ptr);
            value.Read(ptr + KeySize);
        }

        /// <summary>
        /// Using <see cref="SortedTreeScannerBase{TKey,TValue}.Pointer"/> advance to the next KeyValue
        /// </summary>
        protected override unsafe bool InternalReadWhile(TKey key, TValue value, TKey upperBounds)
        {
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            key.Read(ptr);
            value.Read(ptr + KeySize);
            if (key.IsLessThan(upperBounds))
            {
                IndexOfNextKeyValue++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Using <see cref="SortedTreeScannerBase{TKey,TValue}.Pointer"/> advance to the next KeyValue
        /// </summary>
        protected override unsafe bool InternalReadWhile(TKey key, TValue value, TKey upperBounds, MatchFilterBase<TKey, TValue> filter)
        {
        TryAgain:
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            key.Read(ptr);
            value.Read(ptr + KeySize);
            if (key.IsLessThan(upperBounds))
            {
                IndexOfNextKeyValue++;
                if (filter.Contains(key, value))
                    return true;
                if (IndexOfNextKeyValue >= RecordCount)
                    return false;
                goto TryAgain;
            }
            return false;
        }

        /// <summary>
        /// Using <see cref="SortedTreeScannerBase{TKey,TValue}.Pointer"/> advance to the search location of the provided <see cref="key"/>
        /// </summary>
        /// <param name="key">the key to advance to</param>
        protected override unsafe void FindKey(TKey key)
        {
            int offset = KeyMethods.BinarySearch(Pointer, key, RecordCount, m_keyValueSize);
            if (offset < 0)
                offset = ~offset;
            IndexOfNextKeyValue = offset;
        }
    }
}