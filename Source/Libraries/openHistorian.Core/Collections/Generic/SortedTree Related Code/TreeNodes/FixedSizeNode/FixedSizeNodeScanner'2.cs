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

namespace openHistorian.Collections.Generic.TreeNodes
{
    /// <summary>
    /// The treescanner for a fixed size node.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FixedSizeNodeScanner<TKey, TValue>
        : TreeScannerBase<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
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

        /// <summary>
        /// Using <see cref="TreeScannerBase{TKey,TValue}.Pointer"/> advance to the next KeyValue
        /// </summary>
        protected override unsafe void ReadNext()
        {
            byte* ptr = Pointer + IndexOfNextKeyValue * m_keyValueSize;
            KeyMethods.Read(ptr, CurrentKey);
            ValueMethods.Read(ptr + m_keySize, CurrentValue);
        }

        /// <summary>
        /// Using <see cref="TreeScannerBase{TKey,TValue}.Pointer"/> advance to the search location of the provided <see cref="key"/>
        /// </summary>
        /// <param name="key">the key to advance to</param>
        protected override unsafe int FindKey(TKey key)
        {
            int offset = KeyMethods.BinarySearch(Pointer, key, RecordCount, m_keyValueSize);
            if (offset < 0)
                offset = ~offset;
            return offset;
        }
    }
}