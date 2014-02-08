//******************************************************************************************************
//  EncodedNodeScannerBase`2.cs - Gbtc
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
//  5/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Filters;

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    /// <summary>
    /// Base class for reading from a node that is encoded and must be read sequentally through the node.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class EncodedNodeScannerBase<TKey, TValue>
        : SortedTreeScannerBase<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        TKey m_tmpKey;
        TValue m_tmpValue;

        protected EncodedNodeScannerBase(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey, byte version)
            : base(level, blockSize, stream, lookupKey, version)
        {
            m_tmpKey = new TKey();
            m_tmpValue = new TValue();
        }

        protected abstract unsafe void DecodeRecord(TKey key, TValue value, KeyMatchFilterBase<TKey> filter);

        /// <summary>
        /// Occurs when a new node has been reached and any encoded data that has been generated needs to be cleared.
        /// </summary>
        protected abstract void ResetEncoder();

        protected override unsafe int ReadNext(TKey key, TValue value, KeyMatchFilterBase<TKey> filter)
        {
            throw new NotImplementedException();
            //int beforeScan = IndexOfNextKeyValue;

            //if (m_skipNextRead)
            //{
            //    m_skipNextRead = false;
            //    KeyMethods.Copy(m_prevKey, key);
            //    ValueMethods.Copy(m_prevValue, value);
            //    if (filter.Contains(key))
            //    {
            //        IndexOfNextKeyValue++;
            //        return 0;
            //    }
            //}

            //m_nextOffset += DecodeRecord(Pointer + m_nextOffset, key, value, filter);
            //return IndexOfNextKeyValue - beforeScan;
        }

        /// <summary>
        /// Using <see cref="SortedTreeScannerBase{TKey,TValue}.Pointer"/> advance to the search location of the provided <see cref="key"/>
        /// </summary>
        /// <param name="key">the key to advance to</param>
        unsafe protected override void FindKey(TKey key)
        {
            OnNoadReload();

            //Find the first occurance where the key that 
            //is read is greater than or equal to the search key.
            //or the end of the stream is encountered.
            while (IndexOfNextKeyValue < RecordCount)
            {
                ReadNext(m_tmpKey, m_tmpValue, advanceIndex: false);
                if (KeyMethods.IsGreaterThanOrEqualTo(m_tmpKey, key))
                {
                    return;
                }
                ReadNext(m_tmpKey, m_tmpValue, advanceIndex: true);
            }
        }

        /// <summary>
        /// Occurs when a node's data is reset.
        /// Derived classes can override this 
        /// method if fields need to be reset when a node is loaded.
        /// </summary>
        protected override void OnNoadReload()
        {
            ResetEncoder();
        }
    }
}