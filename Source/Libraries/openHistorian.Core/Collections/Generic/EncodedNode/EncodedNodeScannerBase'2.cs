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

namespace openHistorian.Collections.Generic
{
    /// <summary>
    /// Base class for reading from a node that is encoded and must be read sequentally through the node.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class EncodedNodeScannerBase<TKey, TValue>
        : TreeScannerBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        private int m_nextOffset;
        private int m_currentIndex;
        private bool m_skipNextRead;

        protected EncodedNodeScannerBase(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey, TreeKeyMethodsBase<TKey> keyMethods, TreeValueMethodsBase<TValue> valueMethods, byte version)
            : base(level, blockSize, stream, lookupKey, keyMethods, valueMethods, version)
        {
        }

        /// <summary>
        /// Decodes the next record from the byte array into the provided key and value.
        /// </summary>
        /// <param name="stream">the start of the next record.</param>
        /// <param name="key">the key to write to.</param>
        /// <param name="value">the value to write to.</param>
        /// <returns></returns>
        protected abstract unsafe int DecodeRecord(byte* stream, TKey key, TValue value);

        /// <summary>
        /// Occurs when a new node has been reached and any encoded data that has been generated needs to be cleared.
        /// </summary>
        protected abstract void ResetEncoder();


        /// <summary>
        /// Using <see cref="TreeScannerBase{TKey,TValue}.Pointer"/> advance to the next KeyValue
        /// </summary>
        protected override void ReadNext()
        {
            if (m_skipNextRead)
                m_skipNextRead = false;
            else
                InternalRead();
            IndexOfCurrentKeyValue++;
        }

        /// <summary>
        /// Using <see cref="TreeScannerBase{TKey,TValue}.Pointer"/> advance to the search location of the provided <see cref="key"/>
        /// </summary>
        /// <param name="key">the key to advance to</param>
        protected override void FindKey(TKey key)
        {
            OnNoadReload();
            while (InternalRead() && KeyMethods.IsLessThan(CurrentKey, key))
                ;
            if (KeyMethods.IsLessThan(CurrentKey, key))
            {
                IndexOfCurrentKeyValue = m_currentIndex;
                m_skipNextRead = false;
            }
            else
            {
                IndexOfCurrentKeyValue = m_currentIndex - 1;
                m_skipNextRead = true;
            }
        }

        /// <summary>
        /// Occurs when a node's data is reset.
        /// Derived classes can override this 
        /// method if fields need to be reset when a node is loaded.
        /// </summary>
        protected override void OnNoadReload()
        {
            m_nextOffset = HeaderSize;
            m_currentIndex = -1;
            ResetEncoder();
        }

        private unsafe bool InternalRead()
        {
            if (m_currentIndex == RecordCount)
            {
                throw new Exception("Read past the end of the stream");
            }
            m_currentIndex++;
            if (m_currentIndex == RecordCount)
                return false;

            m_nextOffset += DecodeRecord(Pointer + m_nextOffset - HeaderSize, CurrentKey, CurrentValue);
            return true;
        }
    }
}