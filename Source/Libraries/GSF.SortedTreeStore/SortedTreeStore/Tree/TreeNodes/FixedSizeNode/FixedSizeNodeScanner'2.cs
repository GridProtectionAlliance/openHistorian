//******************************************************************************************************
//  FixedSizeNodeScanner`2.cs - Gbtc
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
//  4/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Filters;

namespace GSF.SortedTreeStore.Tree.TreeNodes.FixedSizeNode
{
    /// <summary>
    /// The treescanner for a fixed size node.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FixedSizeNodeScanner<TKey, TValue>
        : SortedTreeScannerBase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        readonly int m_keyValueSize;
        readonly int m_keySize;

        /// <summary>
        /// creates a new class
        /// </summary>
        /// <param name="level"></param>
        /// <param name="blockSize"></param>
        /// <param name="stream"></param>
        /// <param name="lookupKey"></param>
        public FixedSizeNodeScanner(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey)
            : base(level, blockSize, stream, lookupKey, version: 1)
        {
            m_keyValueSize = (KeyMethods.Size + ValueMethods.Size);
            m_keySize = KeyMethods.Size;
        }


        protected override unsafe void InternalRead(TKey key, TValue value)
        {
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            KeyMethods.Read(ptr, key);
            ValueMethods.Read(ptr + m_keySize, value);
            IndexOfNextKeyValue++;
        }

        protected override unsafe bool InternalRead(TKey key, TValue value, KeyMatchFilterBase<TKey> filter)
        {
        TryAgain:
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            KeyMethods.Read(ptr, key);
            ValueMethods.Read(ptr + m_keySize, value);
            IndexOfNextKeyValue++;
            if (filter.Contains(key))
                return true;
            if (IndexOfNextKeyValue >= RecordCount)
                return false;
            goto TryAgain;
        }

        protected override unsafe void InternalPeek(TKey key, TValue value)
        {
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            KeyMethods.Read(ptr, key);
            ValueMethods.Read(ptr + m_keySize, value);
        }

        /// <summary>
        /// Using <see cref="SortedTreeScannerBase{TKey,TValue}.Pointer"/> advance to the next KeyValue
        /// </summary>
        protected override unsafe bool InternalReadWhile(TKey key, TValue value, TKey upperBounds)
        {
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            KeyMethods.Read(ptr, key);
            ValueMethods.Read(ptr + m_keySize, value);
            if (key.IsLessThan( upperBounds))
            {
                IndexOfNextKeyValue++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Using <see cref="SortedTreeScannerBase{TKey,TValue}.Pointer"/> advance to the next KeyValue
        /// </summary>
        protected override unsafe bool InternalReadWhile(TKey key, TValue value, TKey upperBounds, KeyMatchFilterBase<TKey> filter)
        {
        TryAgain:
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            KeyMethods.Read(ptr, key);
            ValueMethods.Read(ptr + m_keySize, value);
            if (key.IsLessThan( upperBounds))
            {
                IndexOfNextKeyValue++;
                if (filter.Contains(key))
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