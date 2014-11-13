//******************************************************************************************************
//  GenericEncodedNodeScanner`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  05/07/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.Snap.Encoding;
using GSF.Snap.Filters;

namespace GSF.Snap.Tree.TreeNodes
{
    /// <summary>
    /// Base class for reading from a node that is encoded and must be read sequentally through the node.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public unsafe class GenericEncodedNodeScanner<TKey, TValue>
            : EncodedNodeScannerBase<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        CombinedEncodingBase<TKey, TValue> m_encoding;

        TKey m_prevKey;
        TValue m_prevValue;
        int m_nextOffset;

        /// <summary>
        /// Creates a new class
        /// </summary>
        /// <param name="level"></param>
        /// <param name="blockSize"></param>
        /// <param name="stream"></param>
        /// <param name="lookupKey"></param>
        public GenericEncodedNodeScanner(CombinedEncodingBase<TKey, TValue> encoding, byte level, int blockSize, BinaryStreamPointerBase stream, Func<TKey, byte, uint> lookupKey)
            : base(level, blockSize, stream, lookupKey)
        {
            m_encoding = encoding;
            m_nextOffset = 0;
            m_prevKey = new TKey();
            m_prevValue = new TValue();
            m_prevKey.Clear();
            m_prevValue.Clear();
        }

        /// <summary>
        /// Occurs when a new node has been reached and any encoded data that has been generated needs to be cleared.
        /// </summary>
        protected override void ResetEncoder()
        {
            m_nextOffset = 0;
            m_prevKey.Clear();
            m_prevValue.Clear();
        }

        protected override void InternalPeek(TKey key, TValue value)
        {
            byte* stream = Pointer + m_nextOffset;
            bool endOfStream;
            m_encoding.Decode(stream, m_prevKey, m_prevValue, key, value, out endOfStream);
        }

        protected override void InternalRead(TKey key, TValue value)
        {
            byte* stream = Pointer + m_nextOffset;
            bool endOfStream;
            int length = m_encoding.Decode(stream, m_prevKey, m_prevValue, key, value, out endOfStream);
            key.CopyTo(m_prevKey);
            value.CopyTo(m_prevValue);
            m_nextOffset += length;
            IndexOfNextKeyValue++;
        }

        protected override bool InternalRead(TKey key, TValue value, MatchFilterBase<TKey, TValue> filter)
        {
        TryAgain:
            byte* stream = Pointer + m_nextOffset;
            bool endOfStream;
            int length = m_encoding.Decode(stream, m_prevKey, m_prevValue, key, value, out endOfStream);
            key.CopyTo(m_prevKey);
            value.CopyTo(m_prevValue);
            m_nextOffset += length;
            IndexOfNextKeyValue++;

            if (filter.Contains(key, value))
                return true;
            if (IndexOfNextKeyValue >= RecordCount)
                return false;

            goto TryAgain;
        }

        protected override bool InternalReadWhile(TKey key, TValue value, TKey upperBounds)
        {
            byte* stream = Pointer + m_nextOffset;
            bool endOfStream;
            int length = m_encoding.Decode(stream, m_prevKey, m_prevValue, key, value, out endOfStream);

            if (key.IsLessThan(upperBounds))
            {
                key.CopyTo(m_prevKey);
                value.CopyTo(m_prevValue);
                m_nextOffset += length;
                IndexOfNextKeyValue++;
                return true;
            }
            return false;
        }

        protected override bool InternalReadWhile(TKey key, TValue value, TKey upperBounds, MatchFilterBase<TKey, TValue> filter)
        {
        TryAgain:

            byte* stream = Pointer + m_nextOffset;
            bool endOfStream;
            int length = m_encoding.Decode(stream, m_prevKey, m_prevValue, key, value, out endOfStream);

            if (key.IsLessThan(upperBounds))
            {
                key.CopyTo(m_prevKey);
                value.CopyTo(m_prevValue);
                m_nextOffset += length;
                IndexOfNextKeyValue++;

                if (filter.Contains(key, value))
                    return true;
                if (IndexOfNextKeyValue >= RecordCount)
                    return false;
                goto TryAgain;
            }
            return false;
        }
    }
}